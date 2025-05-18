using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Radium.Shared.Medical.Surgery.Events;
using Content.Shared.Body.Components;
using Content.Shared.Body.Part;
using Content.Shared.Body.Systems;
using Robust.Shared.Containers;
using Robust.Shared.Player;

namespace Content.Client._Shitcode.UserInterface.Systems.Surgery.Widgets.Systems;

public sealed class ClientDamagePartsSystem : EntitySystem
{
    public event EventHandler<IReadOnlyDictionary<(BodyPartType, BodyPartSymmetry), (int, bool)>>?
        SyncParts;


    public event EventHandler? Dispose;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BodyComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<BodyComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
        SubscribeNetworkEvent<SyncPartsEvent>(OnPartsSync);
    }


    private void OnPartsSync(SyncPartsEvent ev)
    {
        SyncParts?.Invoke(this,
            GetDamagedParts<BodyPartType, BodyPartSymmetry>(GetEntity(ev.Uid)));
    }

    public IReadOnlyDictionary<(BodyPartType, BodyPartSymmetry), (int, bool)>? PartsCondition(EntityUid? uid)
    {
        return uid is not null
            ? GetDamagedParts<BodyPartType, BodyPartSymmetry>(uid.Value)
            : null;
    }

    private void OnPlayerAttached(EntityUid uid, BodyComponent component, LocalPlayerAttachedEvent args)
    {
        SyncParts?.Invoke(this, PartsCondition(uid)!);
    }

    private void OnPlayerDetached(EntityUid uid, BodyComponent component, LocalPlayerDetachedEvent args)
    {
        Dispose?.Invoke(this, EventArgs.Empty);
    }

    public IEnumerable<(EntityUid Id, T0 Component)> GetBodyChildren<T0, T1>(EntityUid euid,
        T1 bodyComponent) where T0 : IComponent
    {
        var result = new List<(EntityUid, T0)>();
        var parts = GetBodyChildren(euid, bodyComponent as BodyComponent);
        foreach (var part in parts)
        {
            if (!TryComp<T0>(part.Id, out var component))
                continue;

            result.Add((part.Id, component));
        }

        return result;
    }

    public IReadOnlyDictionary<(T0, T1), (int, bool)> GetDamagedParts<T0, T1>(EntityUid euid)
        where T0 : struct where T1 : struct
    {
        var result = new Dictionary<(T0, T1), (int, bool)>();

        if (!TryComp<BodyComponent>(euid, out var bodyComponent))
            return result.ToFrozenDictionary();

        foreach (var (_, component) in GetBodyChildren<BodyPartComponent, BodyComponent>(euid, bodyComponent))
        {
            try
            {
                if (!Enum.TryParse<T0>(component.PartType.ToString(), out var partType))
                    continue;
                var partSymmetry = Enum.Parse<T1>(component.Symmetry.ToString());


                result.Add((partType, partSymmetry), (component.Wounds.Count, false));
            }
            catch (Exception)
            {
                Logger.GetSawmill("Surgery").Error("Exception in adding parts!");
            }
        }

        return result;
    }

    //BELOW ARE COPY OF SharedBodySystem METHODS. EXTREME SHITCODE BECAUSE IoC troubles.
    public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyChildren(
        EntityUid? id,
        BodyComponent? body = null,
        BodyPartComponent? rootPart = null)
    {
        if (id is null
            || !Resolve(id.Value, ref body, logMissing: false)
            || body.RootContainer.ContainedEntity is null
            || !Resolve(body.RootContainer.ContainedEntity.Value, ref rootPart))
        {
            yield break;
        }

        foreach (var child in GetBodyPartChildren(body.RootContainer.ContainedEntity.Value, rootPart))
        {
            yield return child;
        }
    }

    public IEnumerable<(EntityUid Id, BodyPartComponent Component)> GetBodyPartChildren(
        EntityUid partId,
        BodyPartComponent? part = null)
    {
        if (!Resolve(partId, ref part, logMissing: false))
            yield break;

        yield return (partId, part);

        foreach (var containerSlotId in part.Children.Keys.Select(GetPartSlotContainerId))
        {
            if (!TryGetContainer(partId, containerSlotId, out var container))
                continue;
            foreach (var containedEnt in container.ContainedEntities)
            {
                if (!TryComp(containedEnt, out BodyPartComponent? childPart))
                    continue;

                foreach (var value in GetBodyPartChildren(containedEnt, childPart))
                {
                    yield return value;
                }
            }
        }
    }
    public bool TryGetContainer(
        EntityUid uid,
        string id,
        [NotNullWhen(true)] out BaseContainer? container,
        ContainerManagerComponent? containerManager = null)
    {
        if (Resolve(uid, ref containerManager, false))
            return containerManager.Containers.TryGetValue(id, out container);

        container = null;
        return false;

    }

    public static string GetPartSlotContainerId(string slotId)
    {
        return SharedBodySystem.PartSlotContainerIdPrefix + slotId;
    }
}
