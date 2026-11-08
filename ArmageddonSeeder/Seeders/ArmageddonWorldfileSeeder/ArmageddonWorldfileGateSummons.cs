#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal static string? NamedGatePackageError(ArmageddonWorldfilePackage source) =>
		source.GateSummons.Length != 10 || source.GateSummons.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != 10 ||
		source.GateSummons.Any(x => string.IsNullOrWhiteSpace(x.Name) || x.TokenVnums.Length == 0 || x.TokenVnums.Any(id => id <= 0 || !source.Items.Any(item => item.Vnum == id)) ||
			!source.Npcs.Any(npc => npc.Vnum == x.NpcVnum))
			? "Regenerate the private worldfile package: ten template-backed Gate rituals with resolved tokens are required." : null;

	private void InstallNamedGateSummons()
	{
		if (NamedGatePackageError(_source) is { } error) throw new InvalidOperationException(error);
		var spellId = OwnedBehaviourId("arm.spell.call_outsider", nameof(MagicSpell)) ?? throw new InvalidOperationException("Call Outsider is missing.");
		var spell = _db.MagicSpells.Find(spellId)!;
		var xml = XElement.Parse(spell.Definition);
		var effect = xml.Element("Effects")!.Elements().Single(x => (string?)x.Attribute("type") == "createnpc");
		var choices = new XElement("NamedChoices", new XAttribute("version", 1), new XAttribute("materialReference", "summon.material"),
			new XElement("ControlBreak", new XAttribute("base", 3), new XAttribute("perFollower", 40)),
			new XElement("Lifetime", new XAttribute("secondsPerGrade", 600), new XAttribute("minimumExtraGrades", 2), new XAttribute("maximumExtraGrades", 4)));
		var controller = CreateAi("Commandable Summoned Creature", "SpellCompanion", new XElement("Definition",
			new XElement("Mode", "companion"), new XElement("Purpose", "owned_outsider"), new XElement("LossChanceCap", 0),
			new XElement("Permitted", new[] { "follow", "stop", "guard", "attack" }.Select(order => new XElement("Order", order)))));
		foreach (var ritual in _source.GateSummons)
		{
			if (!_npcs.TryGetValue(ritual.NpcVnum, out var source) || ritual.TokenVnums.Length == 0 || ritual.TokenVnums.Any(x => !_items.ContainsKey(x)))
				throw new InvalidOperationException($"Named summon {ritual.Name} has an unresolved template or token.");
			var token = new Tag { Id = Next<Tag>(), Name = "Ritual token for " + ritual.Name };
			_db.Tags.Add(token);
			foreach (var proto in _effectiveItems.Where(x => ritual.TokenVnums.Contains(x.Value.Source.Vnum))
				.Select(x => _db.GameItemProtos.Local.Single(p => p.Id == x.Key)))
				proto.GameItemProtosTags.Add(new GameItemProtosTags { TagId = token.Id });
			var template = new NpcTemplate
			{
				Id = Next<NpcTemplate>(), RevisionNumber = 0, Type = source.Type, Name = source.Name,
				UniqueName = Key(ritual.NpcVnum) + "Summoned", Definition = source.Definition, EditableItem = Approval(),
				BuilderNotes = "Named ritual summon; preserves the imported definition with native temporary companion control."
			};
			foreach (var ai in source.NpctemplatesArtificalIntelligences)
				template.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences { AiId = ai.AiId, NpcTemplateId = template.Id, NpcTemplateRevisionNumber = 0 });
			template.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences { AiId = controller.Id, NpcTemplateId = template.Id, NpcTemplateRevisionNumber = 0 });
			_db.NpcTemplates.Add(template);
			choices.Add(new XElement("Choice", new XAttribute("name", ritual.Name), new XAttribute("template", template.Id),
				new XAttribute("uniqueTemplate", source.Id), new XAttribute("tokenTag", token.Id),
				new XAttribute("materialGrade", 6), new XAttribute("minimumGrade", 6)));
			Record("gate." + ritual.NpcVnum, nameof(NpcTemplate), template.Id, 0, new { ritual.Name, ritual.NpcVnum, ritual.TokenVnums, TokenTag = token.Id, MinimumGrade = 6 });
		}
		effect.Add(choices);
		xml.Element("Trigger")!.SetAttributeValue("type", "roomchoice");
		spell.Definition = xml.ToString();
		spell.Description += " An optional named ritual requires grade 6+, its specific carried token (retained) and one Conjuration rank 5+ component. " +
			"A named creature cannot be duplicated. It lasts ten real minutes per grade plus two to four grades, and may break free and attack; each already controlled follower increases that risk.";
		Record("gate.spell", nameof(MagicSpell), spell.Id, null, new { ExcludedExistingCharacterJoke = true, NativePrepaymentSafety = true, Definition = xml.ToString() });
	}
}
