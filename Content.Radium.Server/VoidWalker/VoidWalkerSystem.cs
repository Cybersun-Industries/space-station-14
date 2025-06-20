using Content.Radium.Shared.VoidWalker;
using Content.Server.Popups;
using Content.Shared.Interaction.Components;
using Content.Shared.Item;
using Content.Shared.Storage;
using Robust.Shared.Map;

namespace Content.Radium.Server.VoidWalker;

public class VoidShifterSystem : SharedVoidShifterSystem
{

    [Dependency] private PopupSystem _popup = default!;

    private List<EntityUid> _savedEntities = new();


    public override void Initialize()
    {
        SubscribeLocalEvent<VoidShifterComponent, VoidShiftingEvent>(OnVoidShift);
    }

    private void OnVoidShift(EntityUid uid, VoidShifterComponent comp, VoidShiftingEvent args)
    {
        if (args.Shifted == null)
        {
            Log.Error("[VoidWalker] Received Shifted == null as event parameter, used on {0}", args.User);
            return;
        }

        var xForm = Transform(args.User);
        if (xForm.MapID == MapId.Nullspace)
        {
            _popup.PopupCursor(Loc.GetString("voidwalker-shifter-unknown-map"));
            return;
        }
        ApplyVoidWalkerComponent(args.User, (bool) args.Shifted);
        ApplyUnremovableComponent(args.User, xForm, (bool) args.Shifted);
    }

    private void ApplyVoidWalkerComponent(EntityUid uid, bool shifted)
    {
        if (shifted)
        {
            RemComp<VoidWalkerComponent>(uid);
            return;
        }
        EnsureComp<VoidWalkerComponent>(uid);
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
                    Log.Error("[VoidWalker] an entity {0} was removed before its <UnremoveableComponent> was removed", entity);
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




    }
}
