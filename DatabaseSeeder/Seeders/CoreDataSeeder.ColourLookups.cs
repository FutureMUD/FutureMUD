#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	internal sealed record StockColourSignature(long Id, int Basic, int Red, int Green, int Blue, string Fancy)
	{
		internal bool Matches(Colour? colour, string name) => colour is not null &&
			colour.Id == Id && colour.Name == name && colour.Basic == Basic &&
			colour.Red == Red && colour.Green == Green && colour.Blue == Blue && colour.Fancy == Fancy;
	}

	internal sealed record StockColourLookupRepair(string Name, StockColourSignature Primary, StockColourSignature Secondary)
	{
		internal string QualifiedName => $"{Name} (stock colour {Secondary.Id.ToString(CultureInfo.InvariantCulture)})";
	}

	// Compatibility identities, not extra colours. These fixed colour IDs and exact old signatures
	// are from CoreDataSeeder.Colours.cs; characteristic-value IDs vary with earlier seed data.
	internal static IReadOnlyList<StockColourLookupRepair> StockColourLookupRepairs { get; } =
		Array.AsReadOnly<StockColourLookupRepair>(
	[
		new("ebony", new(17, 0, 10, 10, 10, "the deep, rich black of polished ebony"), new(25, 0, 0, 0, 0, "the deep, rich black of polished ebony")),
		new("cerulean", new(19, 4, 0, 75, 255, "the vibrant, bright cyan of cerulean"), new(91, 11, 0, 75, 255, "a rich, pure cerulean blue")),
		new("blotched red", new(117, 3, 255, 0, 0, ""), new(170, 3, 255, 0, 0, "")),
		new("dull orange", new(118, 7, 255, 165, 0, ""), new(184, 7, 255, 165, 0, "")),
		new("faded purple", new(123, 8, 128, 0, 128, ""), new(198, 8, 128, 0, 128, "")),
		new("violet red", new(224, 8, 128, 0, 128, "the colour of violet red"), new(254, 8, 128, 0, 128, "the colour of violet red")),
		new("hot pink", new(225, 9, 255, 192, 203, "the colour of hot pink"), new(256, 9, 255, 192, 203, "the colour of hot pink")),
		new("maroon red", new(226, 3, 255, 0, 0, "the colour of maroon red"), new(258, 3, 255, 0, 0, "the colour of maroon red")),
		new("plum purple", new(227, 8, 128, 0, 128, "the colour of plum purple"), new(260, 8, 128, 0, 128, "the colour of plum purple")),
		new("magenta red", new(228, 3, 255, 0, 0, "the colour of magenta red"), new(262, 3, 255, 0, 0, "the colour of magenta red")),
		new("cobalt blue", new(229, 4, 0, 0, 255, "the strikingly rich, deep blue colour of cobalt"), new(264, 4, 0, 0, 255, "the strikingly rich, deep blue colour of cobalt")),
		new("light steel blue", new(230, 4, 0, 0, 255, "the colour of light steel blue"), new(266, 4, 0, 0, 255, "the colour of light steel blue")),
		new("slate gray", new(231, 2, 127, 127, 127, "the colour of slate gray"), new(268, 2, 127, 127, 127, "the colour of slate gray")),
		new("turquoise blue", new(232, 4, 0, 0, 255, "the colour of turquoise blue"), new(270, 4, 0, 0, 255, "the colour of turquoise blue")),
		new("cyan blue", new(233, 11, 0, 75, 255, "the colour of cyan blue"), new(272, 11, 0, 75, 255, "the colour of cyan blue")),
		new("cobalt green", new(234, 5, 0, 255, 0, "the colour of cobalt green"), new(274, 5, 0, 255, 0, "the colour of cobalt green")),
		new("lime green", new(235, 5, 0, 255, 0, "the colour of lime green"), new(276, 5, 0, 255, 0, "the colour of lime green")),
		new("ivory white", new(236, 1, 255, 255, 255, "the colour of ivory white"), new(278, 1, 255, 255, 255, "the colour of ivory white")),
		new("goldenrod yellow", new(237, 6, 255, 255, 0, "the colour of goldenrod yellow"), new(280, 6, 255, 255, 0, "the colour of goldenrod yellow")),
		new("dark khaki", new(238, 10, 175, 175, 0, "the colour of dark khaki"), new(282, 10, 175, 175, 0, "the colour of dark khaki")),
		new("banana yellow", new(239, 6, 255, 255, 0, "the colour of banana yellow"), new(284, 6, 255, 255, 0, "the colour of banana yellow")),
		new("orange red", new(240, 3, 255, 0, 0, "the colour of orange red"), new(286, 3, 255, 0, 0, "the colour of orange red")),
		new("moccasin brown", new(241, 10, 175, 175, 0, "the colour of moccasin brown"), new(288, 10, 175, 175, 0, "the colour of moccasin brown")),
		new("tan yellow", new(242, 6, 255, 255, 0, "the colour of tan yellow"), new(290, 6, 255, 255, 0, "the colour of tan yellow")),
		new("brick brown", new(243, 10, 175, 175, 0, "the colour of brick brown"), new(292, 10, 175, 175, 0, "the colour of brick brown")),
		new("carrot orange", new(244, 7, 255, 165, 0, "the colour of carrot orange"), new(294, 7, 255, 165, 0, "the colour of carrot orange")),
		new("peachpuff pink", new(245, 9, 255, 192, 203, "the colour of peachpuff pink"), new(296, 9, 255, 192, 203, "the colour of peachpuff pink")),
		new("sienna brown", new(246, 10, 175, 175, 0, "the colour of sienna brown"), new(298, 10, 175, 175, 0, "the colour of sienna brown")),
		new("saddle brown", new(247, 10, 175, 175, 0, "the colour of saddle brown"), new(300, 10, 175, 175, 0, "the colour of saddle brown")),
		new("salmon pink", new(248, 9, 255, 192, 203, "the colour of salmon pink"), new(302, 9, 255, 192, 203, "the colour of salmon pink")),
		new("sepia brown", new(249, 10, 175, 175, 0, "the colour of sepia brown"), new(304, 10, 175, 175, 0, "the colour of sepia brown")),
		new("fire brick brown", new(250, 10, 175, 175, 0, "the colour of fire brick brown"), new(306, 10, 175, 175, 0, "the colour of fire brick brown")),
		new("teal blue", new(251, 4, 0, 0, 255, "the colour of teal blue"), new(308, 4, 0, 0, 255, "the colour of teal blue")),
		new("dark gray", new(252, 2, 127, 127, 127, "the colour of dark gray"), new(310, 2, 127, 127, 127, "the colour of dark gray")),
	]);

	private static void ReconcileStockColourLookups(FuturemudDatabaseContext context)
	{
		var definitions = context.CharacteristicDefinitions
			.Where(x => x.Name == "Colour")
			.ToArray();
		if (definitions.Length != 1) return;
		var definition = definitions[0];
		if (definition.Name != "Colour" || definition.Type != 2 || definition.Model != "standard" ||
			definition.Pattern is not ("colou?r" or "^colou?r$") || definition.ParentId is not null)
			return;

		var values = context.CharacteristicValues
			.Where(x => x.DefinitionId == definition.Id)
			.ToArray();
		var ids = StockColourLookupRepairs.SelectMany(x => new[] { x.Primary.Id, x.Secondary.Id }).Distinct().ToArray();
		var colours = context.Colours.Where(x => ids.Contains(x.Id)).ToDictionary(x => x.Id);
		var changes = new List<(CharacteristicValue Value, string Name)>();
		var patternChanges = new List<CharacteristicDefinition>();
		if (definition.Pattern == "colou?r") patternChanges.Add(definition);
		foreach (var channel in new[] { 1, 2, 3 })
		{
			var name = $"Colour{channel}";
			var candidates = context.CharacteristicDefinitions.Where(x => x.Name == name).ToArray();
			if (candidates.Length == 1 && candidates[0].Name == name && candidates[0].Type == 2 &&
				candidates[0].Model == "standard" && candidates[0].ParentId == definition.Id &&
				candidates[0].Pattern == $"colou?r{channel}")
				patternChanges.Add(candidates[0]);
		}
		foreach (var repair in StockColourLookupRepairs)
		{
			if (!repair.Primary.Matches(colours.GetValueOrDefault(repair.Primary.Id), repair.Name) ||
				!repair.Secondary.Matches(colours.GetValueOrDefault(repair.Secondary.Id), repair.Name))
				continue;
			var primary = values.Where(x => x.Value == repair.Primary.Id.ToString(CultureInfo.InvariantCulture)).ToArray();
			var secondary = values.Where(x => x.Value == repair.Secondary.Id.ToString(CultureInfo.InvariantCulture)).ToArray();
			if (primary.Length != 1 || secondary.Length != 1 ||
				!IsOriginalStockColourValue(primary[0], repair.Name) ||
				!IsOriginalStockColourValue(secondary[0], repair.Name))
				continue;
			if (values.Any(x => x.Name.Equals(repair.QualifiedName, StringComparison.OrdinalIgnoreCase)) ||
				values.Count(x => x.Name.Equals(repair.Name, StringComparison.OrdinalIgnoreCase)) != 2)
				continue;
			changes.Add((secondary[0], repair.QualifiedName));
		}

		// Never delete or redirect a value, rewrite a colour, change profile XML, or touch stored
		// item/character selections. Runtime display comes from the unchanged referenced Colour.
		foreach (var (value, name) in changes) value.Name = name;
		// Match complete variable names, so Colour cannot capture finecolour or Colour1/2/3.
		foreach (var changedDefinition in patternChanges) changedDefinition.Pattern = $"^{changedDefinition.Pattern}$";
		if (changes.Count > 0 || patternChanges.Count > 0) context.SaveChanges();
	}

	private static bool IsOriginalStockColourValue(CharacteristicValue value, string name) =>
		value.Name == name && !value.Default && value.Pluralisation == 0 &&
		string.IsNullOrEmpty(value.AdditionalValue) && value.FutureProgId is null && value.OngoingValidityProgId is null;
}
