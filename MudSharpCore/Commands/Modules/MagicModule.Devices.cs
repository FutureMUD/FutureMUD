using MudSharp.Commands.Helpers;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Casting;

#nullable enable
namespace MudSharp.Commands.Modules;

public partial class MagicModule
{
	internal const string MagicDeviceHelp = @"Hold, wield or wear a device configured for worn use, then choose its mode explicitly:
	#3magicdevice show <item>#0
	#3magicdevice charged <item> <complete target selector>#0 - spend one stored charge
	#3magicdevice use <item> <power word|grade 1-7>#0 - activate a fixed energy-reservoir payload
	#3magicdevice donate <item> <whole amount>#0 - donate its configured resource one-for-one
	#3magicdevice focus <item> <capability> <spell> <grade> <targets>#0 - ordinary paid personal casting
	#3magicdevice charge <item> <capability> <spell> <grade> <count>#0 - paid timed production/recharge

Quote multiword item, capability and spell names. Use self for self-targeted payloads.
Recharge requires currently acquired, admitted knowledge and controlled reproducible potency.
Interruption after payment gives no refund; uncertain work requires staff reconciliation.
Charged use never teaches the spell or increases spell proficiency/mastery.";
	internal static void MagicDeviceCommand(ICharacter actor, string input) => DeviceCommand(actor, new StringStack(input.RemoveFirstWord()));
	internal static void DeviceCommand(ICharacter actor, StringStack command)
	{
		var mode = command.PopForSwitch();
		var item = actor.TargetItem(command.PopSpeech());
		if (item?.GetItemType<IChargedMagicDevice>() is not { } device) { actor.Send("Select a held charged magic device."); return; }
		if (mode == "show")
		{
			var spellName = device.ReservoirSpellId is { } fixedSpell ? actor.Gameworld.MagicSpells.Get(fixedSpell)?.Name ?? "missing spell" : "no fixed spell";
			var resourceName = device.ResourceId is { } resourceId ? actor.Gameworld.MagicResources.Get(resourceId)?.Name ?? "missing resource" : "no resource";
			actor.Send(device.Mode == MagicDeviceMode.EnergyReservoir
				? $"{device.Role} reservoir: {device.Energy}/{device.MaximumEnergy} {resourceName}. Fixed spell {spellName}; grades {string.Join(", ", device.GradeCosts.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Value}"))}. {device.DataError}"
				: $"{device.Role}: {device.Charges}/{device.Capacity} charges, spell #{device.SpellId}, grade {device.Grade}. {device.DataError}");
			return;
		}
		if (actor.Gameworld.MagicCasting is not MagicCastingService service) { actor.Send("The configured device service is unavailable."); return; }
		if (mode == "use") { actor.Send(service.ActivateReservoir(actor, item, command.SafeRemainingArgument).Message); return; }
		if (mode == "donate")
		{
			if (!int.TryParse(command.PopSpeech(), out var amount) || !command.IsFinished) { actor.Send("Specify one positive whole donation amount."); return; }
			actor.Send(service.DonateReservoir(actor, item, amount).Message); return;
		}
		if (mode == "charged") { actor.Send(service.ActivateDevice(actor, item, command.SafeRemainingArgument).Message); return; }
		if (mode is not ("charge" or "focus")) { actor.Send("Choose show, charged, use, donate, focus or charge explicitly."); return; }
		var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(command.PopSpeech());
		var spell = actor.Gameworld.MagicSpells.GetByIdOrName(command.PopSpeech());
		if (capability is null || spell is null || !int.TryParse(command.PopSpeech(), out var grade)) { actor.Send("Select a capability, spell and external grade."); return; }
		if (mode == "focus") { actor.Send(service.CastDeviceFocus(new(actor, capability.Id, spell.Id, grade, false, command.SafeRemainingArgument), item).Message); return; }
		if (!int.TryParse(command.PopSpeech(), out var count) || !command.IsFinished) { actor.Send("Specify one positive count of missing charges; production has no target."); return; }
		actor.Send(service.BeginDeviceProduction(actor, item, capability.Id, spell.Id, grade, count).Message);
	}
}
