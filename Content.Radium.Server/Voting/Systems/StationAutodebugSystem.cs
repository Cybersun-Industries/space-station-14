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
    private readonly Dictionary<AutodebugVoteTypes, (int, VoteFinishedEventHandler)> _autodebugParameters = new();
    public TimeSpan DebugCooldownTime = TimeSpan.Zero;

    public override void Initialize()
    {
        base.Initialize();
        InitializeVoteOptions();
        InitializeVoteParameters();
        SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(_ =>
        {
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
            (_configurationManager.GetCVar(RadiumCVars.AutodebugEnergyMinPlayerThreshold), OnEnergyDebug));

        _autodebugParameters.Add(AutodebugVoteTypes.Access,
            (_configurationManager.GetCVar(RadiumCVars.AutodebugAccessMinPlayerThreshold), OnAccessDebug));
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (DebugCooldownTime.Ticks > 0)
        {
            DebugCooldownTime = DebugCooldownTime.Subtract(TimeSpan.FromSeconds(frameTime));
        }
    }

    [PublicAPI]
    public void StartBulkDebug()
    {
        if (DebugCooldownTime.Ticks > 0)
            return;

        StartDebugVote(AutodebugVoteTypes.Energy);
        StartDebugVote(AutodebugVoteTypes.Access);
        DebugCooldownTime = TimeSpan.FromSeconds(_configurationManager.GetCVar(RadiumCVars.AutodebugVoteCooldown));
    }

    [PublicAPI]
    public void StartDebugVote(AutodebugVoteTypes voteType)
    {
        var totalPlayers = _playerManager.Sessions.Count(session => session.Status != SessionStatus.Disconnected);
        var minPlayerThreshold = _autodebugParameters[voteType].Item1;

        if (totalPlayers > minPlayerThreshold)
            return;

        var vote = _voteManager.CreateVote(_autodebugVoteOptions[voteType]);
        vote.OnFinished += (sender, args) =>
        {
            if (!IsVoteWon(args))
                return;

            _autodebugParameters[voteType].Item2.Invoke(sender, args);
        };
    }

    private static bool IsVoteWon(VoteFinishedEventArgs args)
    {
        if (args.Winner == null)
            return false;

        if (!Enum.TryParse<AutodebugResponseTypes>(args.Winner.ToString(), out var winner))
            return false;

        return winner == AutodebugResponseTypes.Accept;
    }

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
