// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Radium.Common.CCVar;
using Content.Server.Access.Systems;
using Content.Server.GameTicking;
using Content.Server.Power.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.Voting;
using Content.Server.Voting.Managers;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using JetBrains.Annotations;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Radium.Server.Voting.Systems;

public sealed class StationAutodebugSystem : EntitySystem
{
    [Dependency] private readonly IVoteManager _voteManager = null!;
    [Dependency] private readonly IPlayerManager _playerManager = null!;
    [Dependency] private readonly StationSystem _stationSystem = null!;
    [Dependency] private readonly IConfigurationManager _configurationManager = null!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = null!;
    [Dependency] private readonly AccessSystem _accessSystem = null!;

    private readonly Dictionary<AutodebugVoteTypes, VoteOptions> _autodebugVoteOptions = new();

    private readonly Dictionary<AutodebugVoteTypes, (int, VoteFinishedEventHandler)>
        _autodebugParameters = new();

    public TimeSpan DebugCooldownTime = TimeSpan.Zero;

    public override void Initialize()
    {
        base.Initialize();
        InitializeVoteOptions();
        InitializeVoteParameters();
        SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(_ =>
        {
            DebugCooldownTime = TimeSpan.Zero;

            var isRoundstartVoteEnabled = _configurationManager.GetCVar(RadiumCVars.AutodebugRoundstartVoteEnabled);

            if (!isRoundstartVoteEnabled)
                return;

            StartBulkDebug();
        });
    }

    private void InitializeVoteOptions()
    {
        _autodebugVoteOptions.Add(AutodebugVoteTypes.Energy,
            new VoteOptions
            {
                DisplayVotes = true,
                Duration = TimeSpan.FromSeconds(_configurationManager.GetCVar(RadiumCVars.AutodebugVoteTime)),
                InitiatorPlayer = null,
                InitiatorText = Loc.GetString("autodebug-actor"),
                InitiatorTimeout = null,
                Title = Loc.GetString("autodebug-energy-title"),
                Options =
                [
                    (Loc.GetString("autodebug-accept"), AutodebugResponseTypes.Accept),
                    (Loc.GetString("autodebug-deny"), AutodebugResponseTypes.Deny),
                ],
            });

        _autodebugVoteOptions.Add(AutodebugVoteTypes.Access,
            new VoteOptions
            {
                DisplayVotes = true,
                Duration = TimeSpan.FromSeconds(_configurationManager.GetCVar(RadiumCVars.AutodebugVoteTime)),
                InitiatorPlayer = null,
                InitiatorText = Loc.GetString("autodebug-actor"),
                InitiatorTimeout = null,
                Title = Loc.GetString("autodebug-access-title"),
                Options =
                [
                    (Loc.GetString("autodebug-accept"), AutodebugResponseTypes.Accept),
                    (Loc.GetString("autodebug-deny"), AutodebugResponseTypes.Deny),
                ],
            });
    }

    private void InitializeVoteParameters()
    {
        _autodebugParameters.Add(AutodebugVoteTypes.Energy,
            (_configurationManager.GetCVar(RadiumCVars.AutodebugEnergyMaxPlayerThreshold), OnEnergyDebug));

        _autodebugParameters.Add(AutodebugVoteTypes.Access,
            (_configurationManager.GetCVar(RadiumCVars.AutodebugAccessMaxPlayerThreshold), OnAccessDebug));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (DebugCooldownTime.Ticks > 0)
        {
            DebugCooldownTime = DebugCooldownTime.Subtract(TimeSpan.FromSeconds(frameTime));
        }
    }

    /// <summary>
    /// Starts all predefined debug votes listed in <see cref="AutodebugVoteTypes"/> simultaneously.
    /// If the system is currently on cooldown, no votes will be started and the method returns <c>false</c>.
    /// </summary>
    ///
    /// <returns>
    /// <c>true</c> if the votes were successfully started; <c>false</c> if the system is on cooldown.
    /// </returns>
    [PublicAPI]
    public bool StartBulkDebug()
    {
        if (DebugCooldownTime.Ticks > 0)
            return false;

        var autodebugTypes = Enum.GetValues<AutodebugVoteTypes>();

        foreach (var type in autodebugTypes)
        {
            if (type == AutodebugVoteTypes.Custom)
                continue;

            TryStartDebugVote(type);
        }

        DebugCooldownTime = TimeSpan.FromSeconds(_configurationManager.GetCVar(RadiumCVars.AutodebugVoteCooldown));
        return true;
    }

    public static bool ValidateCustomVoteArgs(
        [NotNullWhen(true)] int? maxPlayersThreshold,
        [NotNullWhen(true)] VoteOptions? voteOptions,
        [NotNullWhen(true)] VoteFinishedEventHandler? handler)
    {
        return maxPlayersThreshold != null && voteOptions != null && handler != null;
    }

    /// <summary>
    /// Attempts to start a new debug vote.
    /// Returns <c>true</c> if the vote was successfully started; otherwise, <c>false</c>.
    ///
    /// When <paramref name="voteType"/> is <see cref="AutodebugVoteTypes.Custom"/>, the parameters
    /// <paramref name="maxPlayersThreshold"/>, <paramref name="voteOptions"/>, and <paramref name="handler"/> must be non-null.
    /// If any of these are null, the method returns <c>false</c>.
    ///
    /// For all non-custom vote types, default parameters are already defined internally.
    /// </summary>
    ///
    /// <param name="voteType">The type of debug vote to start. Select <see cref="AutodebugVoteTypes.Custom"/> for custom vote</param>
    /// <param name="maxPlayersThreshold">
    /// The maximum number of players required for the vote to execute <paramref name="handler"/>.
    /// Required only for <see cref="AutodebugVoteTypes.Custom"/>.
    /// </param>
    /// <param name="voteOptions">
    /// The options to display in the vote UI.
    /// Required only for <see cref="AutodebugVoteTypes.Custom"/>.
    /// </param>
    /// <param name="handler">
    /// The callback to invoke if the vote passes.
    /// Required only for <see cref="AutodebugVoteTypes.Custom"/>.
    /// </param>
    /// <returns><c>true</c> if the vote was successfully started; otherwise, <c>false</c>.</returns>
    [PublicAPI]
    public bool TryStartDebugVote(AutodebugVoteTypes voteType,
        int? maxPlayersThreshold = null,
        VoteOptions? voteOptions = null,
        VoteFinishedEventHandler? handler = null)
    {
        var isCustom = voteType == AutodebugVoteTypes.Custom;

        if (isCustom && !ValidateCustomVoteArgs(maxPlayersThreshold, voteOptions, handler))
            return false;

        var totalPlayers = _playerManager.Sessions.Count(session => session.Status != SessionStatus.Disconnected);

        var maxPlayerThreshold = isCustom ? maxPlayersThreshold : _autodebugParameters[voteType].Item1;

        if (totalPlayers > maxPlayerThreshold)
            return false;

        var vote = _voteManager.CreateVote((isCustom ? voteOptions : _autodebugVoteOptions[voteType])!);

        vote.OnFinished += (sender, args) =>
        {
            if (!IsVoteWon(args))
                return;

            (isCustom ? handler : _autodebugParameters[voteType].Item2)!.Invoke(sender, args);
        };
        return true;
    }

    private static bool IsVoteWon(VoteFinishedEventArgs args)
    {
        if (args.Winner == null)
            return false;

        if (!Enum.TryParse<AutodebugResponseTypes>(args.Winner.ToString(), out var winner))
            return false;

        return winner == AutodebugResponseTypes.Accept;
    }

    /// <summary>
    /// Default handler for <see cref="AutodebugVoteTypes.Energy"/>
    /// </summary>
    private void OnEnergyDebug(IVoteHandle sender, VoteFinishedEventArgs args)
    {
        var baseStation = _stationSystem.GetStations()
            .FirstOrNull(HasComp<StationEventEligibleComponent>);

        var batteryQuery = EntityQueryEnumerator<BatteryComponent>();
        while (batteryQuery.MoveNext(out var uid, out _))
        {
            if (_stationSystem.GetOwningStation(uid) != baseStation && baseStation != null)
                continue;

            var recharger = EnsureComp<BatterySelfRechargerComponent>(uid);
            var battery = EnsureComp<BatteryComponent>(uid);

            recharger.AutoRecharge = true;
            recharger.AutoRechargeRate = battery.MaxCharge;
            recharger.AutoRechargePause = false;
        }
    }

    /// <summary>
    /// Default handler for <see cref="AutodebugVoteTypes.Access"/>
    /// </summary>
    private void OnAccessDebug(IVoteHandle sender, VoteFinishedEventArgs args)
    {
        var batteryQuery = EntityQueryEnumerator<AccessComponent>();
        while (batteryQuery.MoveNext(out var uid, out _))
        {
            var allAccess = _prototypeManager
                .EnumeratePrototypes<AccessLevelPrototype>()
                .Select(p => new ProtoId<AccessLevelPrototype>(p.ID))
                .ToArray();

            _accessSystem.TrySetTags(uid, allAccess);
        }
    }
}
