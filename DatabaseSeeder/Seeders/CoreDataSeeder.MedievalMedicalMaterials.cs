#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public sealed class CoreDataMedievalMedicalMaterialsSeeder : IDatabaseSeeder
{
	private static readonly string[] RequiredMaterialNames =
	[
		"alum",
		"ephedra",
		"foxglove",
		"gut",
		"henbane",
		"mandrake",
		"yarrow"
	];

	private static readonly string[] RequiredMaterialTagNames =
	[
		"Animal Product",
		"Herb",
		"Textile Mordant"
	];

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

	public int SortOrder => 1;
	public string Name => "Core Data Medieval Medical Materials Seeder";
	public string Tagline => "Adds core exact materials for medieval medical item catalogues";
	public string FullDescription =>
		"Installs exact core material records for medieval medical and apothecary stock that should be authored directly rather than substituted with generic herb, sinew, or mineral abstractions.";
	public bool SafeToRunMoreThanOnce => true;

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || RequiredMaterialTagNames.Any(tag => !context.Tags.Any(x => x.Name == tag)))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		return RequiredMaterialNames.All(name => context.Materials.Any(x => x.Name == name))
			? ShouldSeedResult.MayAlreadyBeInstalled
			: ShouldSeedResult.ReadyToInstall;
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		SeedMedievalMedicalMaterialAdditions(context);
		context.SaveChanges();
		return "Successfully installed medieval medical core materials.";
	}

	internal static void SeedMedievalMedicalMaterialAdditions(FuturemudDatabaseContext context)
	{
		var tags = context.Tags
			.AsEnumerable()
			.ToDictionary(x => x.Name, x => x, StringComparer.InvariantCultureIgnoreCase);
		var materials = context.Materials
			.Include(x => x.MaterialAliases)
			.Include(x => x.MaterialsTags)
			.AsEnumerable()
			.ToDictionary(x => x.Name, x => x, StringComparer.InvariantCultureIgnoreCase);

		void EnsureTag(Material material, string tagName)
		{
			if (!tags.TryGetValue(tagName, out var tag))
			{
				return;
			}

			if (material.MaterialsTags.Any(x => x.TagId == tag.Id || x.Tag?.Id == tag.Id))
			{
				return;
			}

			material.MaterialsTags.Add(new MaterialsTags
			{
				Material = material,
				Tag = tag
			});
		}

		void ReleaseAliasFromOtherMaterials(string alias, string reservedMaterialName)
		{
			foreach (var material in materials.Values)
			{
				if (material.Name.Equals(reservedMaterialName, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}

				foreach (var relation in material.MaterialAliases
					         .Where(x => x.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase))
					         .ToList())
				{
					material.MaterialAliases.Remove(relation);
					context.MaterialAliases.Remove(relation);
				}
			}
		}

		void AddMaterial(string name, MaterialBehaviourType type, double relativeDensity, bool organic,
			double shearStrength, double impactStrength, double absorbency, double thermalConductivity,
			double electricalConductivity, double specificHeatCapacity, params string[] materialTags)
		{
			if (!materials.TryGetValue(name, out var material))
			{
				material = new Material
				{
					Name = name,
					MaterialDescription = name,
					BehaviourType = (int)type,
					Type = (int)MaterialType.Solid,
					Density = 1000 * relativeDensity,
					Organic = organic,
					Absorbency = absorbency,
					ShearYield = shearStrength,
					ImpactYield = impactStrength > 0.0 ? impactStrength : shearStrength * 2.2,
					ElectricalConductivity = electricalConductivity,
					ThermalConductivity = thermalConductivity,
					SpecificHeatCapacity = specificHeatCapacity,
					ResidueColour = "white",
					SolventVolumeRatio = 1.0
				};
				context.Materials.Add(material);
				materials[name] = material;
			}

			foreach (var tag in materialTags)
			{
				EnsureTag(material, tag);
			}
		}

		ReleaseAliasFromOtherMaterials("alum", "alum");

		AddMaterial("alum", MaterialBehaviourType.Powder, 1.7, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Textile Mordant");
		AddMaterial("ephedra", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb");
		AddMaterial("foxglove", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb");
		AddMaterial("gut", MaterialBehaviourType.Remains, 1.1, true, 9000, 9000, 0.35, 0.45, 0.0001, 2500,
			"Animal Product", "Crafting Animal Product");
		AddMaterial("henbane", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb");
		AddMaterial("mandrake", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb");
		AddMaterial("yarrow", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb");
	}
}
