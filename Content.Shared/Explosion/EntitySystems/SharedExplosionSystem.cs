using Content.Shared.Armor;
using Content.Shared.Explosion.Components;

namespace Content.Shared.Explosion.EntitySystems;

/// <summary>
/// Lets code in shared trigger explosions and handles explosion resistance examining.
/// All processing is still done clientside.
/// </summary>
public abstract class SharedExplosionSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ExplosionResistanceComponent, ArmorExamineEvent>(OnArmorExamine);
    }

    private void OnArmorExamine(Entity<ExplosionResistanceComponent> ent, ref ArmorExamineEvent args)
    {
        var value = MathF.Round((1f - ent.Comp.DamageCoefficient) * 100, 1);

        if (value == 0)
            return;

        args.Msg.PushNewline();
        args.Msg.AddMarkupOrThrow(Loc.GetString(ent.Comp.Examine, ("value", value)));
    }
}
