#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private void InstallPlacedMasks()
	{
		var placed = _source.Resets.Where(x => x.Opcode is "O" or "G" or "E" or "P")
			.Where(x => x.Arguments.Length > 1).Select(x => x.Arguments[1]).ToHashSet();
		foreach (var (id, effective) in _effectiveItems.Where(x => x.Value.Source.ItemType == 31 && placed.Contains(x.Value.Source.Vnum)))
		{
			var source = effective.Source;
			var proto = _items.Values.Concat(_resetItemVariants.Values).First(x => x.Id == id);
			var components = proto.GameItemProtosGameItemComponentProtos.Select(link =>
				_db.GameItemComponentProtos.Local.FirstOrDefault(x => x.Id == link.GameItemComponentProtoId && x.RevisionNumber == link.GameItemComponentRevision)
				?? _db.GameItemComponentProtos.Single(x => x.Id == link.GameItemComponentProtoId && x.RevisionNumber == link.GameItemComponentRevision)).ToArray();
			// This one-shot installer never reconciles or overwrites a builder's existing mask component.
			if (components.Any(x => x.Type == "IdentityObscurer")) continue;
			if (!components.Any(x => x.Type == "Wearable") || (source.Flags[2] & ((1 << 4) | (1 << 18))) == 0)
			{
				Adapt("mask", source.Vnum, "No supported source head/face wearable mapping; identity obscuring was not added.");
				continue;
			}
			// The source reader asks for the bracketed key, not the similarly named plain keyword.
			var descriptions = source.ExtraDescriptions.Concat(components.Where(x => x.Type == "Keyword Descriptions")
				.SelectMany(x => XElement.Parse(x.Definition).Descendants("Entry").Select(entry =>
					new ArmWorldExtra(entry.Element("Keyword")!.Value, entry.Element("Prose")!.Value))));
			var description = descriptions.LastOrDefault(x => x.Keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Any(key => key.Equals("[mask_desc]", StringComparison.OrdinalIgnoreCase)))?.Description;
			if (source.Vnum == 14304)
			{
				Adapt("mask", source.Vnum, "Excluded pending callback evidence; plain mask_desc metadata is not activated.");
				continue;
			}
			var preserveFullDescription = string.IsNullOrWhiteSpace(description);
			var mask = Component("mask.identity." + id, "IdentityObscurer", new XElement("Definition",
				new XElement("RemovalEcho", ""),
				new XElement("ShortDescription", "&a_an[$?height[$height[@, ]]$frame] figure wearing @item"),
				new XElement("FullDescription", description ?? ""),
				new XElement("PreserveFullDescription", preserveFullDescription), new XElement("Difficulty", (int)Difficulty.Insane),
				new XElement("Keywords"), new XElement("Characteristics")));
			AttachDeliveryComponent(proto, mask, new XElement("Definition"));
			Record("mask.identity." + id, nameof(GameItemComponentProto), mask.Id, mask.RevisionNumber,
				new { SourceVnum = source.Vnum, PreserveFullDescription = preserveFullDescription, Disposition = "Placed head/face mask with source [mask_desc] or original wearer full-description fallback; native worn identity obscuring, native height/frame short description, source full prose and ordinary removal. Native transparent-wearable bypass and recognition contracts apply. No new hooks, disguises or characteristic values." });
		}
	}
}
