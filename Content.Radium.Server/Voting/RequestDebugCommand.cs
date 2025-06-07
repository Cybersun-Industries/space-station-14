// SPDX-FileCopyrightText: 2025 CybersunBot <cybersunbot@proton.me>
// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Radium.Server.Voting.Systems;

namespace Content.Radium.Server.Voting;

using Content.Shared.Administration;
using Robust.Shared.Console;

[AnyCommand]
public sealed class RequestDebugCommand : IConsoleCommand
{
    [Dependency] private readonly IEntityManager _entManager = null!;
    public string Command => "spacemagic";

    public string Description =>
        "Start server vote for debugging batteries and access. Can be used once per 30 minutes.";

    public string Help => "Usage: spacemagic";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var autodebugSystem = _entManager.System<StationAutodebugSystem>();

        if (autodebugSystem.DebugCooldownTime.Ticks > 0)
        {
            shell.WriteLine($"Space magic cooldown: {autodebugSystem.DebugCooldownTime} ticks");
            return;
        }

        autodebugSystem.StartBulkDebug();
    }
}
