using Content.Server.Antag;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Roles;
using Content.Shared.Humanoid;
namespace Content.Server.GameTicking.Rules;
public sealed class ThiefVoxesRuleSystem : GameRuleSystem<ThiefVoxesRuleComponent>
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ThiefVoxesRuleComponent, AfterAntagEntitySelectedEvent>(AfterAntagSelected);
        SubscribeLocalEvent<ThiefVoxesRuleComponent, GetBriefingEvent>(OnGetBriefing);
    }
    // Greeting upon thief activation
    private void AfterAntagSelected(Entity<ThiefVoxesRuleComponent> mindId, ref AfterAntagEntitySelectedEvent args)
    {
        var ent = args.EntityUid;
        _antag.SendBriefing(ent, MakeBriefing(ent), null, null);
    }
    // Character screen briefing
    private void OnGetBriefing(Entity<ThiefVoxesRuleComponent> role, ref GetBriefingEvent args)
    {
        var ent = args.Mind.Comp.OwnedEntity;
        if (ent is null)
            return;
        args.Append(MakeBriefing(ent.Value));
    }
    private string MakeBriefing(EntityUid ent)
    {
        var isHuman = HasComp<HumanoidAppearanceComponent>(ent);
        var briefing = isHuman
            ? Loc.GetString("thiefvoxes-role-greeting-human")
            : Loc.GetString("thiefvoxes-role-greeting-animal");
        if (isHuman)
            briefing += "\n \n" + Loc.GetString("thiefvoxes-role-greeting-equipment") + "\n";
        return briefing;
    }
}
