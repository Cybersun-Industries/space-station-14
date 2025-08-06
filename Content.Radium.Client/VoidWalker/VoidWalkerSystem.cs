using System.Linq;
using Content.Radium.Shared.VoidWalker;
using Content.Shared.Item;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.RCD.Components;
using Content.Shared.Stealth.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Radium.Client.VoidWalker;

public sealed class VoidWalkerSystem : SharedVoidShifterSystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly ILightManager _lightManager = default!;
    [Dependency] private readonly EntityManager _entMan = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IPlayerManager _playerMan = default!;

    private VoidWalkerOverlay _overlay = default!;
    private IEnumerable<EntityUid>? _entityUids;
    private List<EntityUid> _eligibleEnts = new(); // fixed typo i made while underslept
    private ShaderInstance? _shader;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VoidWalkerComponent, VoidShiftingEvent>(OnVoidShift); // TODO: this doesnt work most prob because voidwalker gets applied after sub???

        _shader = _protoMan.Index<ShaderPrototype>("TransparencyShader").Instance();
        _overlay = new();
    }

    private void OnVoidShift(EntityUid uid, VoidWalkerComponent comp, VoidShiftingEvent args)
    {
        if (args.User != _playerMan.LocalEntity)
        {
            Log.Warning("[Voidwalker] Got wrong args.User ({0}), ignoring void shifting!", args.User);
            return;
        }

        if (comp.IsActive)
        {
            _overlayMan.AddOverlay(_overlay);
            _lightManager.DrawLighting = false;
            HideAllEntities(uid);
        }
        else
        {
            _overlayMan.RemoveOverlay(_overlay);
            _lightManager.DrawLighting = true;
            ShowAllEntities(uid);
        }
    }

    private void HideAllEntities(EntityUid user)
    {
        _entityUids = _entMan.GetEntities();
        _eligibleEnts.Clear();
        foreach (var uid in _entMan.GetEntities())
        {
            if (// HasComp<ItemComponent>(uid) meow!
                /*|| */HasComp<ItemComponent>(uid) && TryComp<SpriteComponent>(uid, out var sprite)
                && uid != user)
            {
                _eligibleEnts.Add(uid);
                sprite.PostShader = _shader;
            }
        }
    }

    private void ShowAllEntities(EntityUid user)
    {
        if (_entityUids == null)
        {
            Log.Error("[VoidWalker] WTF I GET NO ENTITES FROM ENTMAN DAMN!!!");
            return;
        }

        foreach (var uid in _eligibleEnts)
        {
            if (TryComp<SpriteComponent>(uid, out var sprite))
            {
                sprite.PostShader = null;
            }
        }
    }



}
