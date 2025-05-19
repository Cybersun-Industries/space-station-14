using System.Linq;
using Content.Radium.Common.CCVar;
using Content.Server.GameTicking;
using Content.Server.Power.Components;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.Voting;
using Content.Server.Voting.Managers;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Utility;

namespace Content.Radium.Server.Voting.Systems;

public sealed class StationAutodebugSystem : EntitySystem
{
    [Dependency] private readonly IVoteManager _voteManager = null!;
    [Dependency] private readonly IPlayerManager _playerManager = null!;
    [Dependency] private readonly StationSystem _stationSystem = null!;
    [Dependency] private readonly IConfigurationManager _configurationManager = null!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(OnJobsAssigned);
    }

    private void OnJobsAssigned(RulePlayerJobsAssignedEvent ev)
    {
        var totalPlayers = _playerManager.Sessions.Count(session => session.Status != SessionStatus.Disconnected);
        var minPlayerThreshold = _configurationManager.GetCVar(RadiumCVars.AutodebugEnergyMinPlayerThreshold);

        if (totalPlayers > minPlayerThreshold)
            return;

        var energyDebugVoteOption = new VoteOptions()
        {
            DisplayVotes = true,
            Duration = TimeSpan.FromMinutes(1),
            InitiatorPlayer = null,
            InitiatorText = Loc.GetString("autodebug-actor"),
            InitiatorTimeout = null,
            Title = Loc.GetString("autodebug-energy-title"),
            Options =
            [
                (Loc.GetString("autodebug-accept"), AutodebugResponseTypes.Accept),
                (Loc.GetString("autodebug-deny"), AutodebugResponseTypes.Deny),
            ],
        };
        var energyVote = _voteManager.CreateVote(energyDebugVoteOption);
        energyVote.OnFinished += OnEnergyVoteFinished;
    }

    private void OnEnergyVoteFinished(IVoteHandle sender, VoteFinishedEventArgs args)
    {
        if (args.Winner == null)
            return;

        if (!Enum.TryParse<AutodebugResponseTypes>(args.Winner.ToString(), out var winner))
            return;

        if (winner != AutodebugResponseTypes.Accept)
            return;

        var batteryQuery = EntityQueryEnumerator<BatteryComponent>();
        while (batteryQuery.MoveNext(out var uid, out _))
        {
            var baseStation = _stationSystem.GetStations()
                .FirstOrNull(HasComp<StationEventEligibleComponent>);

            if (_stationSystem.GetOwningStation(uid) != baseStation)
                return;

            var recharger = EnsureComp<BatterySelfRechargerComponent>(uid);
            var battery = EnsureComp<BatteryComponent>(uid);

            recharger.AutoRecharge = true;
            recharger.AutoRechargeRate = battery.MaxCharge;
            recharger.AutoRechargePause = false;
        }
    }
}
