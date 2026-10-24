#nullable enable

using System.Linq;
using MudSharp.Database;
using MudSharp.Models;
using BasicColour = MudSharp.Form.Colour.BasicColour;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	private sealed record PigmentColour(long Id, string Name, BasicColour Basic, int Red, int Green, int Blue)
	{
		public Colour CreateColour() => new()
		{
			Id = Id, Name = Name, Basic = (int)Basic, Red = Red, Green = Green, Blue = Blue,
			Fancy = $"the colour of {Name}"
		};
	}

	// These IDs are already persisted by the historical pigment characteristic values.
	// RGB values are representative display swatches, not chemical pigment specifications.
	private static PigmentColour[] PigmentColours { get; } =
	[
		new(316, "madder red", BasicColour.Red, 165, 36, 48),
		new(317, "kermes scarlet", BasicColour.Red, 190, 30, 45),
		new(318, "lac crimson", BasicColour.Red, 150, 25, 60),
		new(319, "alkanet purple", BasicColour.Purple, 100, 55, 100),
		new(320, "orchil violet", BasicColour.Purple, 135, 75, 145),
		new(321, "tyrian purple", BasicColour.Purple, 102, 2, 60),
		new(322, "woad blue", BasicColour.Blue, 65, 95, 145),
		new(323, "egyptian blue", BasicColour.Blue, 16, 52, 166),
		new(324, "azurite blue", BasicColour.Blue, 35, 85, 155),
		new(325, "lapis blue", BasicColour.Blue, 38, 65, 130),
		new(326, "malachite green", BasicColour.Green, 45, 140, 85),
		new(327, "verdigris green", BasicColour.Green, 65, 145, 130),
		new(328, "orpiment yellow", BasicColour.Yellow, 240, 195, 35),
		new(329, "realgar orange", BasicColour.Orange, 225, 105, 35),
		new(330, "hematite red", BasicColour.Red, 145, 55, 45),
		new(331, "cinnabar red", BasicColour.Red, 220, 55, 35),
		new(332, "red ochre", BasicColour.Red, 160, 70, 45),
		new(333, "yellow ochre", BasicColour.Yellow, 195, 150, 45),
		new(334, "lamp black", BasicColour.Black, 20, 20, 20),
		new(335, "bone black", BasicColour.Black, 35, 32, 30),
		new(336, "lead white", BasicColour.White, 245, 242, 232),
		new(337, "chalk white", BasicColour.White, 240, 238, 225),
		new(338, "walnut brown", BasicColour.Brown, 100, 65, 40),
		new(339, "oak-gall black", BasicColour.Black, 35, 30, 35),
		new(340, "pomegranate yellow", BasicColour.Yellow, 200, 165, 65),
		new(341, "saffron yellow", BasicColour.Yellow, 245, 195, 45),
		new(342, "henna orange", BasicColour.Orange, 185, 85, 40),
		new(343, "faded madder red", BasicColour.Red, 165, 100, 100),
		new(344, "dull ochre", BasicColour.Yellow, 155, 130, 70),
		new(345, "dusty red ochre", BasicColour.Red, 150, 100, 80),
		new(346, "dull egyptian blue", BasicColour.Blue, 80, 100, 145),
		new(347, "faded woad blue", BasicColour.Blue, 110, 130, 155),
		new(348, "smoky lamp black", BasicColour.Black, 65, 62, 60),
		new(349, "chalky white", BasicColour.White, 225, 222, 210),
		new(350, "tarnished lead white", BasicColour.White, 205, 200, 175),
		new(351, "muddy walnut brown", BasicColour.Brown, 110, 90, 65),
		new(352, "dull verdigris green", BasicColour.Green, 95, 130, 115),
		new(353, "faded tyrian purple", BasicColour.Purple, 130, 90, 115),
		new(354, "stained saffron yellow", BasicColour.Yellow, 185, 155, 75)
	];

	private static void EnsureMissingPigmentColours(FuturemudDatabaseContext context)
	{
		var definitions = context.CharacteristicDefinitions.Where(x => x.Name == "Colour").ToArray();
		if (definitions.Length != 1 || definitions[0].Name != "Colour" || definitions[0].Type != 2 ||
			definitions[0].Model != "standard" || definitions[0].ParentId is not null ||
			definitions[0].Pattern is not ("colou?r" or "^colou?r$"))
			return;

		var definitionId = definitions[0].Id;
		var values = context.CharacteristicValues.Where(x => x.DefinitionId == definitionId).ToArray();
		var existingIds = context.Colours.Select(x => x.Id).ToHashSet();
		foreach (var pigment in PigmentColours)
		{
			// Only fill vacant stock IDs still referenced by their original named value.
			// Existing colours and every characteristic/profile/selection remain untouched.
			var id = pigment.Id.ToString(CultureInfo.InvariantCulture);
			if (!existingIds.Contains(pigment.Id) && values.Count(x => x.Name == pigment.Name && x.Value == id) == 1)
				context.Colours.Add(pigment.CreateColour());
		}

		context.SaveChanges();
	}
}
