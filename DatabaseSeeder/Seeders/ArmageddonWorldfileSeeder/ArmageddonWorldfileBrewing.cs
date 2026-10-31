#nullable enable

using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	internal const string BrewOverrideRegister = "armw_brew_override";
	internal const string BrewCureRegister = "armw_brew_cure";
	internal const string BrewPoisonRegister = "armw_brew_poison";

	private void InstallHerbBrewing()
	{
		var skill = OwnedBehaviourId("arm.skill.brew", nameof(TraitDefinition));
		if (skill is null) throw new InvalidOperationException("Herb brewing requires the installed Brew skill.");
		var herbs = _effectiveItems.Where(x => x.Value.Source.ItemType == 33).ToArray();
		var outputs = Enumerable.Range(13250, 6).Concat(Enumerable.Range(13270, 6)).ToArray();
		foreach (var vnum in outputs)
		{
			if (!_items.TryGetValue(vnum, out var proto) || !_effectiveItems.TryGetValue(proto.Id, out var item) ||
			    item.Source.ItemType != 34 || item.Values[1] != (vnum < 13270 ? 1 : 0))
				throw new InvalidOperationException($"Brewing requires the complete tablet/vial output Vnum{vnum}.");
		}
		if (!_items.TryGetValue(13248, out var vial))
			throw new InvalidOperationException("Brewing requires empty vial Vnum13248.");
		if (herbs.Any(x => x.Value.Values[2] is < 0 or > 5))
			throw new InvalidOperationException("A brewing herb has a colour without a qualified output.");

		var tag = new Tag { Id = Next<Tag>(), Name = "Armageddon Brewing Herb" };
		_db.Tags.Add(tag);
		var kinds = new StringBuilder();
		var colours = new StringBuilder();
		foreach (var herb in herbs)
		{
			var proto = _items.Values.Concat(_resetItemVariants.Values).Single(x => x.Id == herb.Key);
			proto.GameItemProtosTags.Add(new GameItemProtosTags { TagId = tag.Id });
			kinds.AppendLine($"if (@item.proto == {proto.Id})\nreturn {herb.Value.Values[0]}\nend if");
			colours.AppendLine($"if (@item.proto == {proto.Id})\nreturn {herb.Value.Values[2]}\nend if");
		}
		kinds.AppendLine("return 0"); colours.AppendLine("return 0");
		var kindProg = Prog("ArmageddonWorldfileBrewHerbKind", kinds.ToString(), ProgVariableTypes.Number, ("item", ProgVariableTypes.Item));
		var colourProg = Prog("ArmageddonWorldfileBrewHerbColour", colours.ToString(), ProgVariableTypes.Number, ("item", ProgVariableTypes.Item));
		var recipe = Prog("ArmageddonWorldfileBrewRecipe", BrewingRecipeProg(), ProgVariableTypes.Number,
			("a", ProgVariableTypes.Number), ("b", ProgVariableTypes.Number), ("c", ProgVariableTypes.Number));
		var gate = Prog("ArmageddonWorldfileCanBrew", $"return gettrait(@actor, totrait({skill.Value})) > 0",
			ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
		foreach (var isVial in new[] { false, true })
		foreach (var count in new[] { 2, 3 })
		{
			var form = isVial ? "Vial" : "Tablet";
			var text = new StringBuilder("var herbs = collectiondictionaryget(@inputs, \"1\")\n");
			for (var i = 0; i < count; i++)
				text.AppendLine($"var herb{i} = getindex(@herbs, {i})\nvar kind{i} = @{kindProg.FunctionName}(@herb{i})");
			text.AppendLine($"var recipe = @{recipe.FunctionName}(@kind0, @kind1, {(count == 3 ? "@kind2" : "0 - 1")})");
			text.AppendLine($"var colourherb = getindex(@herbs, random(0, {count - 1}))\nvar colour = @{colourProg.FunctionName}(@colourherb)");
			foreach (var colour in Enumerable.Range(0, 6))
			{
				var proto = _items[(isVial ? 13270 : 13250) + colour];
				var values = _effectiveItems[proto.Id].Values;
				text.AppendLine($"if (@colour == {colour})\nvar product = loaditem({proto.Id})\nvar cure = {values[0]}\nvar poison = {values[2]}");
				text.AppendLine("if (@recipe > 0)\ncure = @recipe\nend if\nif (@recipe < 0)\npoison = 0 - @recipe\nend if");
				text.AppendLine($"setregister @product \"{BrewCureRegister}\" @cure\nsetregister @product \"{BrewPoisonRegister}\" @poison\nsetregister @product \"{BrewOverrideRegister}\" 1\nreturn @product\nend if");
			}
			text.AppendLine("return null(\"item\")");
			var product = Prog($"ArmageddonWorldfileBrew{form}{count}", text.ToString(), ProgVariableTypes.Item,
				("inputs", ProgVariableTypes.Item | ProgVariableTypes.CollectionDictionary));
			var craft = new Craft
			{
				Id = Next<Craft>(), RevisionNumber = 0, EditableItem = Approval(),
				Name = $"brew {count}-herb {form.ToLowerInvariant()}", Category = "Herbal Preparations",
				Blurb = $"Mix {count} herbs into a {form.ToLowerInvariant()}.", ActionDescription = "brewing herbs",
				ActiveCraftItemSdesc = "an unfinished herbal preparation", Interruptable = true,
				AppearInCraftsListProgId = gate.Id, CanUseProgId = gate.Id,
				CheckTraitId = null, FailPhase = 1, QualityFormula = "5", IsPracticalCheck = true,
				CheckQualityWeighting = 1.0, InputQualityWeighting = 1.0, ToolQualityWeighting = 1.0
			};
			craft.CraftInputs.Add(new CraftInput { Id = Next<CraftInput>(), InputType = "Tag", InputQualityWeight = 1.0,
				OriginalAdditionTime = DateTime.UtcNow, Definition = new XElement("Definition",
					new XElement("TargetTagId", tag.Id), new XElement("Quantity", count)).ToString() });
			if (isVial)
				craft.CraftInputs.Add(new CraftInput { Id = Next<CraftInput>(), InputType = "SimpleItem", InputQualityWeight = 1.0,
					OriginalAdditionTime = DateTime.UtcNow.AddSeconds(1), Definition = new XElement("Definition",
						new XElement("TargetItemId", vial.Id), new XElement("Quantity", 1),
						new XElement("RequireEmptyLiquidContainer", true)).ToString() });
			craft.CraftProducts.Add(new CraftProduct { Id = Next<CraftProduct>(), ProductType = "Prog",
				OriginalAdditionTime = DateTime.UtcNow, Definition = new XElement("Definition", new XElement("ItemProg", product.Id)).ToString() });
			// Native crafts enter their first phase after five seconds; reserve inputs, then wait to finish.
			craft.CraftPhases.Add(new CraftPhase { PhaseNumber = 1, PhaseLengthInSeconds = 175.0,
				Echo = isVial ? "$0 begin|begins mixing $i1 into $i2." : "$0 begin|begins mixing $i1.",
				FailEcho = "$0 stop|stops brewing.", ExertionLevel = 1, StaminaUsage = 0.0 });
			craft.CraftPhases.Add(new CraftPhase { PhaseNumber = 2, PhaseLengthInSeconds = 1.0,
				Echo = isVial ? "$0 mix|mixes $i1 into $i2 to make $p1." : "$0 mix|mixes $i1 to make $p1.",
				FailEcho = "$0 stop|stops brewing.", ExertionLevel = 1, StaminaUsage = 0.0 });
			_db.Crafts.Add(craft);
			Record($"brewing.{form}.{count}", nameof(Craft), craft.Id, 0, new
			{
				Herbs = count, Form = form, Yield = 1, Seconds = 180, VialVnum = isVial ? 13248 : 0,
				Disposition = "Native preparation with a timed completion phase; Brew admission without a success roll; random ingredient colour; source recipe override preserves all other prototype fields and unmatched mixtures. Existing oral antidote/poison delivery and empty-vial return apply."
			});
		}
		_counts["herb brewing crafts"] = 4;
		Adapt("herb brewing", 0, "Active tablet/vial preparations use native crafts and existing oral doses. Disabled alcohol/SANBREW paths are not invented. Three-herb generic-recipe wildcards and cure-before-poison priority are retained. Source 30 command-queue ticks at 24 pulses per tick and four pulses per real second become 180 seconds; native craft interruption/reservation rules apply.");
		Flush();
	}

	internal static string BrewingRecipeProg()
	{
		// Positive results override the cure; negative results override the poison; zero preserves the template.
		var text = new StringBuilder();
		foreach (var (left, right) in new[] { ("a", "b"), ("b", "c"), ("a", "b") })
			text.AppendLine($"if (@{left} < @{right})\nvar swap = @{left}\n{left} = @{right}\n{right} = @swap\nend if");
		foreach (var (a, b, c, result) in new[]
		{
			(6, 4, -1, 1), (7, 6, 2, 2), (8, 7, 1, 3), (8, 4, 3, 5), (5, 3, 1, 6), (5, 3, 2, 7),
			(5, 1, -1, -1), (7, 6, 5, -2), (4, 3, 2, -3), (3, 2, 1, -5), (8, 7, 4, -7)
		})
			text.AppendLine($"if (@a == {a} and @b == {b}{(c == -1 ? "" : $" and @c == {c}")})\nreturn {(result < 0 ? $"0 - {-result}" : result.ToString())}\nend if");
		text.AppendLine("return 0");
		return text.ToString();
	}
}
