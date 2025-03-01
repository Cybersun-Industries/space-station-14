using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    public static readonly CVarDef<string> DiscordFaxMachineWebhook =
        CVarDef.Create("discord.fax_machine_webhook", string.Empty, CVar.SERVERONLY | CVar.CONFIDENTIAL);
}
