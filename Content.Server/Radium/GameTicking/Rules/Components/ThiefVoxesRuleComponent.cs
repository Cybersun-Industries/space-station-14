using Content.Shared.Random;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Radium.GameTicking.Rules.Components;

/// <summary>
/// Stores data for <see cref="ThiefVoxesRuleSystem"/>.
/// </summary>
[RegisterComponent, Access(typeof(ThiefVoxesRuleSystem))]
public sealed partial class ThiefVoxesRuleComponent : Component;
