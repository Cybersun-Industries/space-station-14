using System.Linq;
using System.Numerics;
using Content.Server.Administration;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Server.Radium.Warps.Components;
using Content.Server.Warps;
using Content.Shared.Administration;
using Content.Shared.DragDrop;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Gravity;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Tag;
using Robust.Server.GameObjects;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Physics;
using Robust.Shared.Player;

namespace Content.Server.Radium.Warps;

public class  WarperSystem : EntitySystem
{

    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly WarpPointSystem _warpPointSystem = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;



    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<WarperComponent, ActivateInWorldEvent>(OnActivateInWorldEvent);
        SubscribeLocalEvent<WarperComponent, DragDropDraggedEvent>(OnDragDrop);
        SubscribeLocalEvent<WarperComponent, MoveFinished>(OnMoveFinished);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnCleanup);
    }


    private WarperComponent? FindStairsDown(MapId id)
    {
        foreach (var warper in EntityManager.EntityQuery<WarperComponent>())
        {
            if (Transform(warper.Owner).MapID == id && warper.Dungeon)
            {
                return warper;
            }
        }
        return null;
    }

    private void DoWarp(EntityUid uid, EntityUid user, EntityUid victim, WarperComponent component)
    {
        if (component.ID is null)
        {
            Logger.DebugS("warper", "Warper has no destination");
            _popupSystem.PopupEntity(Loc.GetString("warper-goes-nowhere", ("warper", uid)), user, Filter.Entities(user));
            return;
        }

        var dest = _warpPointSystem.FindWarpPoint(component.ID);
        if (dest is null)
        {
            Logger.DebugS("warper", String.Format("Warp destination '{0}' not found", component.ID));
            _popupSystem.PopupEntity(Loc.GetString("warper-goes-nowhere", ("warper", uid)), user, Filter.Entities(user));
            return;
        }

        var entMan = IoCManager.Resolve<IEntityManager>();
        TransformComponent? destXform;
        entMan.TryGetComponent<TransformComponent>(dest.Value, out destXform);
        if (destXform is null)
        {
            Logger.DebugS("warper", String.Format("Warp destination '{0}' has no transform", component.ID));
            _popupSystem.PopupEntity(Loc.GetString("warper-goes-nowhere", ("warper", uid)), user, Filter.Entities(user));
            return;
        }

        // Check that the destination map is initialized and return unless in aghost mode.
        var mapMgr = IoCManager.Resolve<IMapManager>();
        var destMap = destXform.MapID;
        if (!mapMgr.IsMapInitialized(destMap) || mapMgr.IsMapPaused(destMap))
        {
            if (!entMan.HasComponent<GhostComponent>(user))
            {
                // Normal ghosts cannot interact, so if we're here this is already an admin ghost.
                Logger.DebugS("warper", String.Format("Player tried to warp to '{0}', which is not on a running map", component.ID));
                _popupSystem.PopupEntity(Loc.GetString("warper-goes-nowhere", ("warper", uid)), user, Filter.Entities(user));
                return;
            }
        }

        var xform = entMan.GetComponent<TransformComponent>(victim);
        xform.Coordinates = destXform.Coordinates;
        xform.AttachToGridOrMap();
        if (entMan.TryGetComponent(victim, out PhysicsComponent? phys))
        {
            _physics.SetLinearVelocity(victim, Vector2.Zero);
        }
    }

    private void OnDragDrop(EntityUid uid, WarperComponent component, DragDropEvent args)
    {
        if (args.Handled)
            return;

        args.Handled = true;
        var userUid = args.User;
        var doAfterArgs = new DoAfterEventArgs(userUid, 5, default, uid)
        {
            BreakOnTargetMove = true,
            BreakOnUserMove = true,
            BreakOnDamage = true,
            BreakOnStun = true,
            NeedHand = true,
            TargetFinishedEvent = new MoveFinished(userUid, args.Dragged)
        };

        _doAfter.DoAfter(doAfterArgs);
    }

    private sealed class MoveFinished : EntityEventArgs
    {
        public EntityUid VictimUid;
        public EntityUid UserUid;

        public MoveFinished(EntityUid userUid, EntityUid victimUid)
        {
            UserUid = userUid;
            VictimUid = victimUid;
        }
    }

    private void OnMoveFinished(EntityUid uid, WarperComponent component, MoveFinished args)
    {
        DoWarp(component.Owner, args.UserUid, args.VictimUid, component);
    }

    [AdminCommand(AdminFlags.Debug)]
    sealed class NextLevelCommand : IConsoleCommand
    {
        public string Command => "nextlevel";
        public string Description => "Find the ladder down and force-apply it";
        public string Help => "nextlevel";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (shell.Player == null || shell.Player.AttachedEntity == null || shell.Player.AttachedEntityTransform == null)
            {
                shell.WriteLine("You need a player and attached entity to use this command.");
                return;
            }

            var sysMan = IoCManager.Resolve<IEntitySystemManager>();
            var _ent = IoCManager.Resolve<IEntityManager>();
            var _warper = sysMan.GetEntitySystem<WarperSystem>();
            var player = shell.Player.AttachedEntity.Value;
            var stairs = _warper.FindStairsDown(shell.Player.AttachedEntityTransform.MapID);
            if (stairs == null)
            {
                shell.WriteLine("No stairs found!");
                return;
            }

            if (stairs.RequiresCompletion && !_warper.CheckComplete(stairs.Owner, stairs))
            {
                shell.WriteLine("The level is not complete but you force your way down anyway.");
                stairs.RequiresCompletion = false;
            }
            _warper.OnActivate(stairs.Owner, stairs, new ActivateInWorldEvent(player, stairs.Owner));
        }
    }
}
