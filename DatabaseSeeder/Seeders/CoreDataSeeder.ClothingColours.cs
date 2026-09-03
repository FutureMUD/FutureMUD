#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	internal sealed record ClothingColourProfileSpec(string Name, string ComponentName, string Description,
		IReadOnlyList<string> Values);

	// Appearance choices for identified clothing consumers, not dye recipes or period-price evidence.
	// Material rationale and sources: Industrialised_Clothing_Wave2_Infrastructure_and_Gate2.md.
	internal static IReadOnlyList<ClothingColourProfileSpec> ClothingColourProfiles { get; } = Array.AsReadOnly(new[]
	{
		new ClothingColourProfileSpec("Clothing_Leather_Colours", "Variable_LeatherColour",
			"Natural and dyed leather tones for the $colour variable.",
			Array.AsReadOnly(new[] { "black", "brown", "light brown", "dark brown", "tan brown", "reddish brown", "red", "green" })),
		new ClothingColourProfileSpec("Clothing_Wood_Colours", "Variable_WoodColour",
			"Natural wood tones for the $colour variable; no paint, stain, species or surface treatment is implied.",
			Array.AsReadOnly(new[] { "light brown", "brown", "dark brown", "reddish brown", "beige" })),
		new ClothingColourProfileSpec("Clothing_Lacquer_Colours", "Variable_LacquerColour",
			"Dark brown, black and red lacquer tones for the $colour variable; no ceremonial colour rule is imposed.",
			Array.AsReadOnly(new[] { "dark brown", "black", "red" }))
	});

	internal static void EnsureClothingColourProfiles(FuturemudDatabaseContext context)
	{
		var existingNames = context.CharacteristicProfiles.Select(x => x.Name).ToArray()
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var missing = ClothingColourProfiles.Where(x => !existingNames.Contains(x.Name)).ToArray();
		if (missing.Length == 0) return;

		var definitions = context.CharacteristicDefinitions.ToArray()
			.Where(x => x.Name.Equals("Colour", StringComparison.OrdinalIgnoreCase)).ToArray();
		if (definitions.Length != 1 || definitions[0].Name != "Colour" || definitions[0].ParentId is not null ||
			definitions[0].Type != 2 || definitions[0].Model != "standard" || definitions[0].Pattern != "^colou?r$")
			throw new InvalidOperationException("Clothing colour profiles require one canonical Colour definition with its stock channel pattern.");
		var definition = definitions[0];
		var values = context.CharacteristicValues.Where(x => x.DefinitionId == definition.Id).ToArray();
		var pending = new List<CharacteristicProfile>();
		foreach (var spec in missing)
		{
			foreach (var name in spec.Values)
			{
				var matches = values.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
				if (matches.Length != 1 || matches[0].Name != name)
					throw new InvalidOperationException($"Clothing colour profile {spec.Name} requires one exact Colour value '{name}'. Restore the prerequisite before retrying.");
			}
			pending.Add(new CharacteristicProfile
			{
				Name = spec.Name, Type = "Standard", TargetDefinitionId = definition.Id,
				Description = spec.Description,
				Definition = new XElement("Definition", spec.Values.Select(x => new XElement("Value", x))).ToString()
			});
		}

		// Resolve every missing profile before adding any; existing/customised profiles are never overwritten.
		context.CharacteristicProfiles.AddRange(pending);
		context.SaveChanges();
	}
}
