using Content.Radium.Shared.VoidWalker;
using Content.Server.Popups;
using Content.Server.Stealth;
using Content.Shared.Coordinates;
using Content.Shared.Eye;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Item;
using Content.Shared.Physics;
using Content.Shared.Stealth.Components;
using Content.Shared.Storage;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Map;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization.Manager;

namespace Content.Radium.Server.VoidWalker;

public sealed class VoidShifterSystem : SharedVoidShifterSystem
{

    [Dependency] private PopupSystem _popup = default!;
    [Dependency] private VisibilitySystem _visibility = default!;
    [Dependency] private EyeSystem _eye = default!;
    [Dependency] private StealthSystem _stealth = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private FixtureSystem _fixture = default!;
    [Dependency] private AudioSystem _audio = default!;

    private List<EntityUid> _savedEntities = new();
    private Fixture? _fix1;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoidShifterComponent, VoidShiftingEvent>(OnVoidShift);

    }

    private void OnVoidShift(EntityUid uid, VoidShifterComponent comp, VoidShiftingEvent args)
    {

        if (!TryComp<VoidWalkerComponent>(args.User, out var walker))
        {
            Log.Error("[VoidWalker] We got user {0} who doesnt have voidwalker component but tried to use voidshifter!", args.User);
            return;
        }
        if (args.Shifted == null)
        {
            Log.Error("[VoidWalker] Received <Shifted == null> as event parameter, used on {0}", args.User);
            return;
        }



        var xForm = Transform(args.User);
        if (xForm.MapID == MapId.Nullspace)
        {
            _popup.PopupCursor(Loc.GetString("voidwalker-shifter-unknown-map"));
            return;
        }

        ApplyVoidWalkerComponent(args.User, walker, (bool) args.Shifted);
        ApplyUnremovableComponent(args.User, xForm, (bool) args.Shifted);
        ApplyVisibilityComponent(args.User, (bool) args.Shifted);
        ApplyShiftingFX(args.User);
        RaiseLocalEvent(args.User, args, false);
    }

    private void ApplyVoidWalkerComponent(EntityUid uid, VoidWalkerComponent walker, bool shifted)
    {
        if (shifted)
        {
            walker.IsActive = false;
            return;
        }

        walker.IsActive = true;


    }

    private void ApplyVisibilityComponent(EntityUid uid, bool shifted)
    {
        var visibility = EnsureComp<VisibilityComponent>(uid);

        Entity<VisibilityComponent?> ent = (uid, visibility);


        if (!TryComp<EyeComponent>(uid, out var eye))
            return;

        Entity<EyeComponent?> entEye = (uid, eye);

        var stealth = TryComp<StealthComponent>(uid, out var stealthComp);

        if (_fix1 == null || _fixture.GetFixtureOrNull(uid, "fix1") != _fix1)
                        _fix1 = _fixture.GetFixtureOrNull(uid, "fix1");

        if (!shifted)
        {
            if (stealth)
                _stealth.SetEnabled(uid, false);

            if (_fix1 != null)
                _physics.SetCollisionLayer(uid, "fix1", _fix1, _fix1.CollisionLayer & ~(int) CollisionGroup.BulletImpassable);
            else
                Log.Error("[VoidWalker] What the fuck, I got no fixture from a fucking Entity: {0}", uid, uid.ToCoordinates());

            _eye.SetVisibilityMask(uid, eye.VisibilityMask | (int) VisibilityFlags.VoidWalker, eye);
            // _eye.RefreshVisibilityMask(entEye); for some reason this causes an entity to set its eye vis mask back to normal (1)
            _visibility.AddLayer(ent, (ushort) VisibilityFlags.VoidWalker, false);
            _visibility.RemoveLayer(ent, (ushort) VisibilityFlags.Normal, false);
            _visibility.RefreshVisibility(uid);
        }
        else
        {
            if (stealth)
                _stealth.SetEnabled(uid, true);

            if (_fix1 != null)
                _physics.SetCollisionLayer(uid, "fix1", _fix1, _fix1.CollisionLayer | (int) CollisionGroup.BulletImpassable);
            else
                Log.Error("[VoidWalker] Couldnt revert collision, there's no fixture in this Entity: {0}", uid);

            _eye.SetVisibilityMask(uid, eye.VisibilityMask & ~(int) VisibilityFlags.VoidWalker, eye);
            _eye.RefreshVisibilityMask(entEye);
            _visibility.RemoveLayer(ent, (ushort) VisibilityFlags.VoidWalker, false);
            _visibility.AddLayer(ent, (ushort) VisibilityFlags.Normal, false);
            _visibility.RefreshVisibility(uid);
        }
    }

    private void ApplyUnremovableComponent(EntityUid uid, TransformComponent xForm, bool shifted)
    {
        if (_savedEntities.Count != 0 && shifted)
        {
            foreach (var entity in _savedEntities)
            {
                if (entity.IsValid())
                    RemComp<UnremoveableComponent>(entity);
                else
                    Log.Warning("[VoidWalker] an entity {0} was removed before its <UnremoveableComponent> was removed", entity);
            }
            _savedEntities.Clear();
        }
        else if (!shifted)
        {
            var query = xForm.ChildEnumerator;
            while (query.MoveNext(out var child))
            {
                if (HasComp<ItemComponent>(child))
                {
                    AddComp<UnremoveableComponent>(child);
                    _savedEntities.Add(child);
                }

                if (HasComp<StorageComponent>(child))
                {
                    var childXForm = Transform(child);
                    ApplyUnremovableComponent(uid, childXForm, false);
                }
            }
        }
        else
        {
            Log.Error("[VoidWalker] _savedEntities List<EntityUid>.Count != 0 or <shifted> parameter got out of control!");
        }

        // TODO: Write some code here for preventing interactions you dumb ass!!!

    }

    private void ApplyShiftingFX(EntityUid uid)
    {
        _audio.PlayPvs(new SoundPathSpecifier("/Audio/_DV/CosmicCult/ability_glare.ogg"), uid);
        Spawn("CosmicGlareAbilityVFX", Transform(uid).Coordinates);
    }
}
