#nullable enable

using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPlacedTravelRunes()
	{
		var placed = _source.Resets.Where(x => x.Opcode is "O" or "G" or "E" or "P")
			.Where(x => x.Arguments.Length > 1).Select(x => x.Arguments[1]).ToHashSet();
		var runes = _effectiveItems.Where(x => x.Value.Source.ItemType == 29 && placed.Contains(x.Value.Source.Vnum)).ToArray();
		if (runes.Length == 0) return;
		var waystepId = OwnedBehaviourId("arm.spell.waystep", nameof(MagicSpell))
			?? throw new System.InvalidOperationException("Placed teleport runes require the native Waystep spell.");
		var waystep = _db.MagicSpells.Find(waystepId)!;
		var definition = XElement.Parse(waystep.Definition);
		definition.Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "waystep")
			.SetElementValue("RuneUsesPoolRestrictions", false);
		waystep.Definition = definition.ToString();
		waystep.Description = "Travel to a weighted random destination, or use rune <keywords> for one unambiguous owned or detector-accessible public rune. " +
			"Random destinations exclude restricted, lethal and indoor rooms. Explicit rune destinations bypass those pool filters. " +
			"Both routes retain same-plane, no-magic, ward and native relocation safety checks. Public runes must be directly in a room.";
		Record("travelrune.waystep-policy", nameof(MagicSpell), waystep.Id, null, new { RuneUsesPoolRestrictions = false });
		foreach (var (id, effective) in runes)
		{
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			// Creator metadata is never inferred from the unused value slots of ITEM_TELEPORT.
			if (effective.Source.ExtraDescriptions.Any(x => x.Keyword is "[PC_info]" or "[ACCOUNT_info]"))
			{
				Adapt("teleport rune", effective.Source.Vnum, "Creator-tagged rune retained without inventing a native owner or public admission.");
				continue;
			}
			AddItemOnLoadProg(proto, "PublicTravelRune", "addpublictravelanchoreffect(@item, \"SenseMagical\")\nreturn true");
			foreach (var item in _db.GameItems.Local.Where(x => x.GameItemProtoId == id))
			{
				var effects = XElement.Parse(item.EffectData ?? "<Effects/>");
				if (!effects.Elements("Effect").Any(x => (string?)x.Element("Type") == "PublicTravelAnchor"))
					effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "PublicTravelAnchor"),
						new XElement("Original", 0), new XElement("Remaining", 0),
						new XElement("Effect", new XElement("RequiredPerception", (long)MudSharp.Effects.PerceptionTypes.SenseMagical))));
				item.EffectData = effects.ToString();
			}
			Record("travelrune." + id, nameof(GameItemProto), id, proto.RevisionNumber, new
			{
				SourceVnum = effective.Source.Vnum, RequiredPerception = "SenseMagical",
				Disposition = "Creatorless public anchor. Native detection reveals and admits it; direct room placement supplies the live destination. Unused source values are not room IDs. Existing travel plane, ward and safety rules apply."
			});
		}
	}
}
