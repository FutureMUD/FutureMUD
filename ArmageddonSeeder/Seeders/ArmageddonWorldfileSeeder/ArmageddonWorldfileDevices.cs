#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPlacedMagicDevices()
	{
		var placed = _source.Resets.Where(x => x.Opcode is "O" or "G" or "E" or "P")
			.Where(x => !ArmageddonWorldfilePackage.ExcludedPlaygroundZones.Contains(x.Zone) && x.Arguments.Length > 1)
			.Select(x => x.Arguments[1]).ToHashSet();
		var candidates = _effectiveItems.Where(x => x.Value.Source.ItemType == 4 && placed.Contains(x.Value.Source.Vnum))
			.Where(x => x.Value.Values[0] > 0 && x.Value.Values[1] > 0).ToArray();
		if (candidates.Length == 0) return;
		var mana = OwnedBehaviourId(ArmageddonProgressionInstaller.ManaResourceKey, nameof(MagicResource))
			?? throw new InvalidOperationException("Placed magic devices require the installed Mana resource.");
		foreach (var (id, effective) in candidates)
		{
			var (key, delivery, openSky, minimumMana) = effective.Values[3] switch
			{
				11 => ("arm.spell.ember_lance", MagicDeviceDelivery.OtherCharacters, false, 20),
				21 => ("arm.spell.cinder_rain", MagicDeviceDelivery.NativeArea, true, 0),
				28 => ("arm.spell.raise_servitor", MagicDeviceDelivery.RoomCorpses, false, 0),
				45 => ("arm.spell.banish_traveller", MagicDeviceDelivery.OtherCharacters, false, 0),
				_ => ("", MagicDeviceDelivery.OtherCharacters, false, 0)
			};
			if (key.Length == 0)
			{
				Adapt("magic device", effective.Source.Vnum, "Unmapped source payload retained inert; no invented charged spell.");
				continue;
			}
			var spell = OwnedBehaviourId(key, nameof(MagicSpell))
				?? throw new InvalidOperationException($"Placed magic device Vnum{effective.Source.Vnum} requires {key}.");
			var costs = Enumerable.Range(1, Math.Min(7, effective.Values[1]))
				.ToDictionary(grade => grade, grade => StaffEnergyCost(effective.Values[1], grade, minimumMana));
			var definition = MagicDeviceDefinition.Serialize(MagicDeviceKind.Staff, MagicDeviceRole.Charged,
				MagicDeviceEligibility.Anyone, 1, 60, 0, 0, 0, 0, Difficulty.Normal, Outcome.Pass, [spell],
				new XElement("Plan", new XElement("Phase")), MagicDeviceMode.EnergyReservoir,
				spell, mana, effective.Values[0], effective.Values[2], true, delivery, costs, openSky);
			var component = Component("reservoirdevice." + id, "ChargedMagicDevice", XElement.Parse(definition));
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			AttachDeliveryComponent(proto, component, XElement.Parse(MagicDeviceDefinition.ReservoirState(effective.Values[2])));
			Record("reservoirdevice." + id, nameof(GameItemProto), id, proto.RevisionNumber, new
			{
				SourceVnum = effective.Source.Vnum, SourceMaximumPower = effective.Values[1], Spell = key,
				MaximumEnergy = effective.Values[0], InitialEnergy = effective.Values[2], Costs = costs,
				Delivery = delivery.ToString(), RequiresOpenSky = openSky,
				Disposition = "One native Mana per donated energy; fixed native payload, selectable source power words, one payment per room activation. Equipped access, no acquired spell or control check. Native wards and resistance apply."
			});
		}
	}

	internal static int StaffEnergyCost(int maximumPower, int grade, int minimumMana)
	{
		if (maximumPower < 1 || grade < 1 || grade > Math.Min(7, maximumPower))
			throw new ArgumentOutOfRangeException(nameof(grade));
		// Source mana_amount_level increments vdiff before its integer divisions.
		return 10 * Math.Max(50 / (maximumPower - grade + 2), minimumMana);
	}
}
