#nullable enable

namespace MudSharp.Commands.Modules;

internal partial class InventoryModule
{
	[PlayerCommand("MagicDevice", "magicdevice")]
	[HelpInfo("magicdevice", MagicModule.MagicDeviceHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void MagicDeviceCommand(ICharacter actor, string input) => MagicModule.MagicDeviceCommand(actor, input);
}
