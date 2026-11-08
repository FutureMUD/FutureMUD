#nullable enable

using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallSpecialisedTools()
	{
		foreach (var (id, effective) in _effectiveItems.Where(x => x.Value.Source.ItemType == 7 && x.Value.Values[0] == 1))
		{
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			var component = Component("climbingaid." + id, "Climbing Aid", new XElement("Definition",
				new XElement("CheckBonus", effective.Values[1]), new XElement("WorksHeld", true), new XElement("WorksWorn", true)));
			AttachDeliveryComponent(proto, component, new XElement("Definition"));
			Record("tool.climbing." + id, nameof(GameItemProto), id, proto.RevisionNumber, new
			{
				SourceVnum = effective.Source.Vnum, Bonus = effective.Values[1],
				Disposition = "Strongest held or worn climbing aid supplies a flat native climb-check bonus. Any wear location qualifies; carrying alone does not. Native hand, stamina and movement rules remain unchanged."
			});
			_counts["climbing aid definitions"] = _counts.GetValueOrDefault("climbing aid definitions") + 1;
		}
		foreach (var (id, effective) in _effectiveItems.Where(x => x.Value.Source.ItemType == 7 && x.Value.Values[0] == 6))
		{
			var proto = _db.GameItemProtos.Local.Single(x => x.Id == id);
			var component = Component("lockpick." + id, "Locksmithing Tool", new XElement("Definition",
				new XElement("DifficultyAdjustment", 0), new XElement("CheckBonus", effective.Values[1]),
				new XElement("Breakable", true), new XElement("UsableForInstallation", false),
				new XElement("UsableForConfiguration", false), new XElement("UsableForFabrication", false)));
			AttachDeliveryComponent(proto, component, new XElement("Definition"));
			Record("tool.lockpick." + id, nameof(GameItemProto), id, proto.RevisionNumber, new
			{
				SourceVnum = effective.Source.Vnum, Bonus = effective.Values[1],
				Disposition = "Held native locksmithing tool for picking/relocking. Source quality becomes a flat native check bonus; native major-failure breakage replaces progressive wear. No fabrication, pairing or installation privileges."
			});
			_counts["lockpick definitions"] = _counts.GetValueOrDefault("lockpick definitions") + 1;
		}
		Flush();
	}
}
