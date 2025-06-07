// SPDX-FileCopyrightText: 2025 freeze2222 <opop1094@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Radium.Server.IoC;

namespace Content.Radium.Server.Entry;

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
public sealed class EntryPoint : GameServer
{
    public override void Init()
    {
        base.Init();

        ServerRadiumContentIoC.Register();

        IoCManager.BuildGraph();
    }
}
