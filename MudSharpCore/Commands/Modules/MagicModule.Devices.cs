using MudSharp.Commands.Helpers;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	[PlayerCommand("MagicDevice", "magicdevice")]
	[HelpInfo("magicdevice", @"Hold or wield the device and choose its mode explicitly:
	#3magicdevice show <item>#0
	#3magicdevice charged <item> <complete target selector>#0 - spend one stored charge
	#3magicdevice focus <item> <capability> <spell> <grade> <targets>#0 - ordinary paid personal casting
	#3magicdevice charge <item> <capability> <spell> <grade> <count>#0 - paid timed production/recharge

Quote multiword item, capability and spell names. Use self for self-targeted payloads.
Recharge requires currently acquired, admitted knowledge and controlled reproducible potency.
Interruption after payment gives no refund; uncertain work requires staff reconciliation.
Charged use never teaches the spell or increases spell proficiency/mastery.", AutoHelp.HelpArgOrNoArg)]
	protected static void MagicDevice(ICharacter actor, string input) => DeviceCommand(actor, new StringStack(input.RemoveFirstWord()));
	internal static void DeviceCommand(ICharacter actor, StringStack command)
	{
		var mode = command.PopForSwitch();
		var item = actor.TargetItem(command.PopSpeech());
		if (item?.GetItemType<IChargedMagicDevice>() is not { } device) { actor.Send("Select a held charged magic device."); return; }
		if (mode == "show") { actor.Send($"{device.Role}: {device.Charges}/{device.Capacity} charges, spell #{device.SpellId}, grade {device.Grade}. {device.DataError}"); return; }
		if (actor.Gameworld.MagicCasting is not MagicCastingService service) { actor.Send("The configured device service is unavailable."); return; }
		if (mode == "charged") { actor.Send(service.ActivateDevice(actor, item, command.SafeRemainingArgument).Message); return; }
		if (mode is not ("charge" or "focus")) { actor.Send("Choose show, charged, focus or charge explicitly."); return; }
		var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(command.PopSpeech());
		var spell = actor.Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech());
		if (capability is null || spell is null || !int.TryParse(command.PopSpeech(), out var grade)) { actor.Send("Select a capability, spell and external grade."); return; }
		if (mode == "focus") { actor.Send(service.CastDeviceFocus(new(actor, capability.Id, spell.Id, grade, false, command.SafeRemainingArgument), item).Message); return; }
		if (!int.TryParse(command.PopSpeech(), out var count) || !command.IsFinished) { actor.Send("Specify one positive count of missing charges; production has no target."); return; }
		actor.Send(service.BeginDeviceProduction(actor, item, capability.Id, spell.Id, grade, count).Message);
	}
}
