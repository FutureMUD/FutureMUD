#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	private void SeedMaterials(FuturemudDatabaseContext context)
	{
		SeedMaterialsBase(context);

		var tags = context.Tags
			.AsEnumerable()
			.ToDictionary(x => x.Name, x => x, StringComparer.InvariantCultureIgnoreCase);
		var materials = context.Materials
			.Include(x => x.MaterialAliases)
			.Include(x => x.MaterialsTags)
			.AsEnumerable()
			.ToDictionary(x => x.Name, x => x, StringComparer.InvariantCultureIgnoreCase);

		if (tags.Remove("Water Soluable", out var waterSolubleTag))
		{
			waterSolubleTag.Name = "Water Soluble";
			tags[waterSolubleTag.Name] = waterSolubleTag;
		}

		void EnsureTag(Material material, string tagName)
		{
			var tag = tags[tagName];
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

		void RemoveTag(Material material, string tagName)
		{
			if (!tags.TryGetValue(tagName, out var tag))
			{
				return;
			}

			foreach (var relation in material.MaterialsTags.Where(x => x.TagId == tag.Id).ToList())
			{
				material.MaterialsTags.Remove(relation);
				context.MaterialsTags.Remove(relation);
			}
		}

		void EnsureAlias(Material material, params string[] aliases)
		{
			foreach (var alias in aliases)
			{
				var canonicalAlias = alias.ToLowerInvariant();
				if (material.MaterialAliases.Any(x => x.Alias.Equals(canonicalAlias, StringComparison.InvariantCultureIgnoreCase)))
				{
					continue;
				}

				material.MaterialAliases.Add(new MaterialAlias
				{
					Material = material,
					Alias = canonicalAlias
				});
			}
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

		void RenameMaterial(string currentName, string newName, params string[] aliases)
		{
			var material = materials[currentName];
			materials.Remove(currentName);
			material.Name = newName;
			material.MaterialDescription = newName;
			materials[newName] = material;
			EnsureAlias(material, aliases);
		}

		void AddMaterial(string name, MaterialBehaviourType type, double relativeDensity, bool organic,
			double shearStrength, double impactStrength, double absorbency, double thermalConductivity,
			double electricalConductivity, double specificHeatCapacity, params string[] materialTags)
		{
			if (materials.ContainsKey(name))
			{
				return;
			}

			var material = new Material
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
			foreach (var tag in materialTags)
			{
				EnsureTag(material, tag);
			}
		}

		RenameMaterial("coach", "coachwood", "coach");
		RenameMaterial("aubergene", "aubergine", "aubergene", "eggplant", "brinjal");

		materials["salt"].Organic = false;
		materials["guano"].BehaviourType = (int)MaterialBehaviourType.Feces;
		materials["cream"].Organic = true;

		EnsureTag(materials["cardboard"], "Paper Product");
		EnsureTag(materials["paper"], "Paper Product");
		EnsureTag(materials["papyrus"], "Paper Product");
		EnsureTag(materials["soap"], "Manufactured Materials");
		EnsureTag(materials["paraffin wax"], "Manufactured Materials");
		EnsureTag(materials["calcium hydroxide"], "Manufactured Materials");
		EnsureTag(materials["calcium oxide"], "Manufactured Materials");
		EnsureTag(materials["lye"], "Manufactured Materials");
		EnsureTag(materials["portland cement"], "Manufactured Materials");
		EnsureTag(materials["roman cement"], "Manufactured Materials");
		EnsureTag(materials["slaked lime"], "Manufactured Materials");

		RemoveTag(materials["soap"], "Natural Materials");
		RemoveTag(materials["soap"], "Animal Product");
		RemoveTag(materials["paraffin wax"], "Natural Materials");
		RemoveTag(materials["calcium hydroxide"], "Natural Materials");
		RemoveTag(materials["calcium oxide"], "Natural Materials");
		RemoveTag(materials["lye"], "Natural Materials");
		RemoveTag(materials["portland cement"], "Natural Materials");
		RemoveTag(materials["roman cement"], "Natural Materials");
		RemoveTag(materials["slaked lime"], "Natural Materials");

		foreach (var name in new[]
		{
			"duck", "goose", "goat", "turkey", "boar", "buffalo", "bison", "elk", "horse", "pigeon", "quail",
			"ostrich", "mutton", "kangaroo", "alligator"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, "Meat");
		}

		foreach (var name in new[]
		{
			"cherry", "fig", "date", "coconut", "guava", "grapefruit", "mandarin", "tangerine", "persimmon",
			"quince", "cranberry", "raspberry", "blackberry", "strawberry", "blueberry", "watermelon",
			"cantaloupe", "honeydew", "lychee", "papaya", "kiwi fruit", "passionfruit", "currant", "dragonfruit"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, "Fruit");
		}

		EnsureAlias(materials["kiwi fruit"], "kiwi");

		foreach (var name in new[]
		{
			"celery", "asparagus", "artichoke", "kale", "chard", "parsnip", "rutabaga", "okra", "cassava", "taro",
			"plantain", "mushroom", "brussels sprout", "shallot", "bok choy", "watercress", "radicchio",
			"fennel bulb"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Vegetable");
		}

		foreach (var name in new[]
		{
			"chive", "marjoram", "lemongrass", "bay leaf", "tarragon", "savory", "chamomile", "lavender", "sorrel",
			"nettle"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, "Herb");
		}

		foreach (var name in new[]
		{
			"cardamom", "allspice", "anise", "star anise", "caraway", "fenugreek", "mustard seed", "vanilla",
			"sumac", "sesame seed", "poppy seed", "mace", "wasabi", "white pepper"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, "Spice");
		}

		AddMaterial("madder root", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb", "Textile Dye");
		AddMaterial("indigo dye cake", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Textile Dye");
		foreach (var name in new[] { "taffeta", "ribbon", "calico", "chintz" })
		{
			AddMaterial(name, MaterialBehaviourType.Fabric, 0.45, true, 10000, 25000, 1.0, 0.14, 0.0001, 500,
				"Natural Fiber Fabric", "Primary Production Commodity");
		}
		foreach (var name in new[] { "ramie cloth", "barkcloth", "camelid wool", "raffia cloth" })
		{
			AddMaterial(name, MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500,
				"Natural Fiber Fabric", "Primary Production Commodity");
		}

		AddMaterial("gourd shell", MaterialBehaviourType.Shell, 0.2, true, 12000, 18000, 0.15, 0.08, 0.0001, 1800,
			"Natural Materials", "Primary Production Commodity");
		AddMaterial("papier-mache", MaterialBehaviourType.Fabric, 0.45, true, 6000, 16000, 0.8, 0.12, 0.0001, 1300,
			"Paper Product", "Manufactured Materials", "Primary Production Commodity");
		AddMaterial("birch bark", MaterialBehaviourType.Fabric, 0.55, true, 9000, 18000, 0.7, 0.12, 0.0001, 1400,
			"Natural Fiber Fabric", "Primary Production Commodity");
		AddMaterial("hemp cloth", MaterialBehaviourType.Fabric, 1.45, true, 14000, 30000, 1.8, 0.14, 0.0001, 500,
			"Natural Fiber Fabric", "Primary Production Commodity");
		AddMaterial("brocade", MaterialBehaviourType.Fabric, 1.7, true, 12000, 28000, 1.6, 0.14, 0.0001, 500,
			"Natural Fiber Fabric", "Manufactured Materials", "Primary Production Commodity");
		AddMaterial("damask", MaterialBehaviourType.Fabric, 1.4, true, 11000, 26000, 1.5, 0.14, 0.0001, 500,
			"Natural Fiber Fabric", "Manufactured Materials", "Primary Production Commodity");
		AddMaterial("silk gauze", MaterialBehaviourType.Fabric, 0.35, true, 6500, 14000, 0.9, 0.10, 0.0001, 500,
			"Natural Fiber Fabric", "Manufactured Materials", "Primary Production Commodity");
		AddMaterial("featherwork", MaterialBehaviourType.Feather, 0.3, true, 5000, 10000, 0.5, 0.08, 0.0001, 1300,
			"Animal Product", "Manufactured Materials", "Primary Production Commodity");
		AddMaterial("beadwork", MaterialBehaviourType.Fabric, 1.8, true, 9000, 22000, 0.6, 0.18, 0.0001, 500,
			"Manufactured Materials", "Primary Production Commodity");

		AddMaterial("logwood", MaterialBehaviourType.Wood, 0.75, true, 40000, 10000, 0.05, 0.14, 0.0001, 420,
			"Hardwood", "Textile Dye", "Primary Production Commodity");
		AddMaterial("cochineal", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Animal Product", "Textile Dye", "Primary Production Commodity");
		AddMaterial("tobacco leaf", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb", "Primary Production Commodity");
		AddMaterial("type metal", MaterialBehaviourType.Metal, 9.4, false, 50000, 120000, 0.0, 35.0, 4800000.0, 160,
			"Renaissance Age", "Primary Production Metal Stock", "Primary Production Commodity");
		AddMaterial("printing ink", MaterialBehaviourType.Paste, 1.1, true, 1000, 1000, 0.25, 0.14, 0.0001, 500,
			"Manufactured Materials", "Writing Product", "Primary Production Commodity");
		AddMaterial("molasses", MaterialBehaviourType.Paste, 1.4, true, 1000, 1000, 0.2, 0.14, 0.0001, 500,
			"Food", "Primary Production Commodity");
		AddMaterial("sugar loaf", MaterialBehaviourType.Food, 1.6, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Food", "Primary Production Commodity");
		AddMaterial("tobacco twist", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.05, 0.14, 0.0001, 500,
			"Herb", "Primary Production Commodity");
		AddMaterial("snuff", MaterialBehaviourType.Powder, 0.7, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb", "Primary Production Commodity");
		AddMaterial("roasted coffee", MaterialBehaviourType.Food, 0.6, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Food", "Primary Production Commodity");
		AddMaterial("cacao bean", MaterialBehaviourType.Food, 0.6, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Food Crop", "Primary Production Commodity");
		AddMaterial("cacao nibs", MaterialBehaviourType.Food, 0.6, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Food", "Primary Production Commodity");
		AddMaterial("chocolate block", MaterialBehaviourType.Food, 1.3, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Food", "Primary Production Commodity");
		AddMaterial("tea brick", MaterialBehaviourType.Food, 0.7, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Food", "Primary Production Commodity");
		AddMaterial("cotton fibre", MaterialBehaviourType.Plant, 0.15, true, 1000, 1000, 1.0, 0.08, 0.0001, 500,
			"Fiber Crop", "Primary Production Commodity");
		AddMaterial("ochre pigment", MaterialBehaviourType.Powder, 2.8, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Textile Dye", "Stone");
		AddMaterial("alum mordant", MaterialBehaviourType.Powder, 1.7, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Textile Mordant");
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
		EnsureTag(materials["alum"], "Textile Mordant");
		EnsureTag(materials["ephedra"], "Herb");
		EnsureTag(materials["foxglove"], "Herb");
		EnsureTag(materials["gut"], "Animal Product");
		EnsureTag(materials["gut"], "Crafting Animal Product");
		EnsureTag(materials["henbane"], "Herb");
		EnsureTag(materials["mandrake"], "Herb");
		EnsureTag(materials["yarrow"], "Herb");
		ReleaseAliasFromOtherMaterials("alum", "alum");
		foreach (var name in new[]
		         {
			         "woad leaves", "weld", "kermes grain", "alkanet root", "henna leaf", "pomegranate rind",
			         "walnut hull", "oak gall", "orchil lichen"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Herb", "Textile Dye");
		}

		AddMaterial("lac dye cake", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Textile Dye");
		AddMaterial("murex purple dye", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Textile Dye");
		AddMaterial("orpiment", MaterialBehaviourType.Powder, 3.5, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Stone");
		AddMaterial("realgar", MaterialBehaviourType.Powder, 3.6, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Stone");
		AddMaterial("verdigris pigment", MaterialBehaviourType.Powder, 1.9, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Stone");
		AddMaterial("lead white pigment", MaterialBehaviourType.Powder, 6.1, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Stone");
		AddMaterial("red ochre pigment", MaterialBehaviourType.Powder, 2.8, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Textile Dye", "Stone");
		AddMaterial("yellow ochre pigment", MaterialBehaviourType.Powder, 2.8, false, 1000, 1000, 0.0, 0.14,
			0.0001, 500, "Textile Dye", "Stone");
		AddMaterial("egyptian blue frit", MaterialBehaviourType.Powder, 2.8, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Stone");
		AddMaterial("bone black pigment", MaterialBehaviourType.Powder, 1.8, true, 1000, 1000, 0.2, 0.08, 0.0001,
			1000, "Textile Dye");
		EnsureTag(materials["saffron"], "Textile Dye");
		EnsureAlias(materials["madder root"], "madder");
		EnsureAlias(materials["indigo dye cake"], "indigo");
		EnsureAlias(materials["logwood"], "campeachy wood");
		EnsureAlias(materials["tobacco leaf"], "tobacco");
		EnsureAlias(materials["type metal"], "printer's metal", "printers metal");
		EnsureAlias(materials["printing ink"], "oil-based printing ink", "printer's ink", "printers ink");
		EnsureAlias(materials["molasses"], "treacle");
		EnsureAlias(materials["sugar loaf"], "loaf sugar");
		EnsureAlias(materials["tobacco twist"], "rope tobacco");
		EnsureAlias(materials["snuff"], "powdered tobacco");
		EnsureAlias(materials["roasted coffee"], "roasted coffee bean");
		EnsureAlias(materials["cacao bean"], "cocoa bean");
		EnsureAlias(materials["cacao nibs"], "cocoa nibs");
		EnsureAlias(materials["chocolate block"], "chocolate cake");
		EnsureAlias(materials["tea brick"], "tea cake");
		EnsureAlias(materials["cotton fibre"], "cotton fiber");
		EnsureAlias(materials["ochre pigment"], "ochre");
		EnsureAlias(materials["saffron"], "crocus");
		EnsureAlias(materials["woad leaves"], "woad", "isatis");
		EnsureAlias(materials["weld"], "dyer's weld", "dyers weld", "dyer's rocket");
		EnsureAlias(materials["kermes grain"], "kermes", "scarlet grain");
		EnsureAlias(materials["alkanet root"], "alkanet", "dyer's bugloss", "dyers bugloss");
		EnsureAlias(materials["henna leaf"], "henna");
		EnsureAlias(materials["pomegranate rind"], "pomegranate peel");
		EnsureAlias(materials["walnut hull"], "walnut husk");
		EnsureAlias(materials["oak gall"], "gallnut", "oak apple");
		EnsureAlias(materials["orchil lichen"], "orchil", "orseille");
		EnsureAlias(materials["lac dye cake"], "lac dye", "lac");
		EnsureAlias(materials["murex purple dye"], "tyrian purple", "royal purple", "murex dye");
		EnsureAlias(materials["verdigris pigment"], "verdigris");
		EnsureAlias(materials["lead white pigment"], "lead white");
		EnsureAlias(materials["red ochre pigment"], "red ochre");
		EnsureAlias(materials["yellow ochre pigment"], "yellow ochre");
		EnsureAlias(materials["egyptian blue frit"], "egyptian blue", "blue frit");
		EnsureAlias(materials["bone black pigment"], "bone black");

		AddMaterial("bog iron ore", MaterialBehaviourType.Ore, 3.4, false, 10000, 200000, 0.05, 0.14, 0.0001,
			500, "Iron Ore");
		AddMaterial("magnetite sand", MaterialBehaviourType.Ore, 4.2, false, 10000, 200000, 0.0, 0.14, 0.0001,
			500, "Iron Ore");
		AddMaterial("placer gold concentrate", MaterialBehaviourType.Ore, 3.8, false, 10000, 200000, 0.0, 0.14,
			0.0001, 500, "Gold Ore");
		AddMaterial("rock salt", MaterialBehaviourType.Stone, 2.1, false, 10000, 200000, 0.0, 0.14, 0.0001,
			500, "Economically Useful Stone");
		AddMaterial("volcanic ash", MaterialBehaviourType.Powder, 0.9, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Natural Materials");
		AddMaterial("shell lime", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Manufactured Materials");
		AddMaterial("coral", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500,
			"Natural Materials");
		AddMaterial("coral lime", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Manufactured Materials");
		AddMaterial("rock crystal", MaterialBehaviourType.Stone, 2.65, false, 60000000, 200000, 0.0, 0.14,
			0.0001, 500, "Gemstone");
		AddMaterial("faience", MaterialBehaviourType.Ceramic, 1.9, false, 40000, 100000, 0.0, 0.002, 0.0001,
			500, "Faience");
		AddMaterial("enamel", MaterialBehaviourType.Ceramic, 2.5, false, 33000, 90000, 0.0, 1.0, 0.0001, 840,
			"Enamel", "Glass");
		AddMaterial("niello", MaterialBehaviourType.Metal, 6.0, false, 10500, 0, 0.0, 17.9, 14500000, 500,
			"Inlay Material", "Medieval Age");
		AddMaterial("silver-gilt", MaterialBehaviourType.Metal, 12.0, false, 85000, 0, 0.0, 17.9, 14500000,
			500, "Gilded Metal", "Precious Metal", "Medieval Age");
		AddMaterial("gilded bronze", MaterialBehaviourType.Metal, 9.0, false, 314000, 0, 0.0, 17.9, 14500000,
			500, "Gilded Metal", "Bronze Age");
		AddMaterial("gilded copper", MaterialBehaviourType.Metal, 9.2, false, 68000, 0, 0.0, 17.9, 14500000,
			500, "Gilded Metal", "Bronze Age");
		AddMaterial("mother-of-pearl", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001,
			500, "Shell", "Gemstone");
		AddMaterial("nacre", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500,
			"Shell", "Gemstone");
		AddMaterial("cowrie shell", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001,
			500, "Shell");
		AddMaterial("conch shell", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001,
			500, "Shell");
		AddMaterial("tortoiseshell", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001,
			500, "Tortoiseshell", "Animal Product");
		foreach (var name in new[]
		         {
			         "flower", "fresh flower", "wilted flower", "petal", "rose", "violet", "daisy", "jasmine",
			         "lotus flower", "marigold", "lily", "chrysanthemum", "blossom"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 0.2, true, 1000, 1000, 0.1, 0.08, 0.0001, 500,
				"Flower");
		}

		foreach (var name in new[] { "dried flower", "dried petal" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 0.2, true, 1000, 1000, 0.1, 0.08, 0.0001, 500,
				"Dried Flower");
		}

		foreach (var name in new[] { "ivy", "laurel" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 0.4, true, 5000, 5000, 0.1, 0.08, 0.0001, 500,
				"Leaf");
		}

		AddMaterial("rush", MaterialBehaviourType.Plant, 0.3, true, 5000, 5000, 0.1, 0.08, 0.0001, 500,
			"Rush");
		AddMaterial("greenstone", MaterialBehaviourType.Stone, 3.0, false, 60000000, 200000, 0.0, 0.14,
			0.0001, 500, "Gemstone");
		AddMaterial("copperas", MaterialBehaviourType.Powder, 1.9, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Economically Useful Stone");
		AddMaterial("potash", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Manufactured Materials");
		AddMaterial("barilla plant", MaterialBehaviourType.Plant, 0.8, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Natural Materials");
		AddMaterial("barilla ash", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Manufactured Materials");
		AddMaterial("kelp ash", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001,
			500, "Manufactured Materials");

		EnsureTag(materials["native gold"], "Native Gold Ore");
		EnsureTag(materials["native nickel"], "Native Nickel Ore");
		RemoveTag(materials["native gold"], "Native Nickel Ore");
		RemoveTag(materials["native nickel"], "Native Gold Ore");
		EnsureTag(materials["volcanic ash"], "Economically Useful Stone");
		EnsureTag(materials["pozzolanic ash"], "Economically Useful Stone");
		EnsureTag(materials["plaster"], "Manufactured Materials");
		EnsureTag(materials["soda ash"], "Manufactured Materials");
		EnsureAlias(materials["halite"], "rock salt");
		EnsureAlias(materials["rock salt"], "halite");
		EnsureAlias(materials["kaolinite"], "kaolin");
		EnsureAlias(materials["kaolinite clay"], "kaolin clay");
		EnsureAlias(materials["pozzolanic ash"], "pozzolana", "pozzolan");
		EnsureAlias(materials["volcanic ash"], "pozzolana ash", "pozzolanic volcanic ash");
		EnsureAlias(materials["greenstone"], "pounamu");
		EnsureAlias(materials["copperas"], "vitriol", "green vitriol");
		EnsureAlias(materials["seaweed"], "kelp");
		EnsureTag(materials["coral"], "Shell");
		EnsureAlias(materials["rock crystal"], "crystal", "clear quartz");
		EnsureAlias(materials["faience"], "fayence");
		EnsureAlias(materials["enamel"], "enamel inlay");
		EnsureAlias(materials["niello"], "niello inlay");
		EnsureAlias(materials["silver-gilt"], "gilt silver", "vermeil");
		EnsureAlias(materials["gilded bronze"], "gilt bronze");
		EnsureAlias(materials["gilded copper"], "gilt copper");
		EnsureAlias(materials["mother-of-pearl"], "mother of pearl");
		EnsureAlias(materials["nacre"], "nacreous shell");
		EnsureAlias(materials["cowrie shell"], "cowrie");
		EnsureAlias(materials["conch shell"], "conch");
		EnsureAlias(materials["tortoiseshell"], "turtle shell");
		EnsureAlias(materials["rush"], "rushes");
		EnsureAlias(materials["laurel"], "bay laurel");

		foreach (var name in new[]
		{
			"flatbread", "biscuit", "cracker", "cake", "muffin", "bagel", "bun", "tortilla", "croissant",
			"dumpling"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Baked Good");
		}

		foreach (var name in new[]
		{
			"butter", "ghee", "cream cheese", "cottage cheese", "curd", "whey", "clotted cream", "buttermilk"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, "Food",
				"Animal Product");
		}

		AddMaterial("milk", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, "Food",
			"Animal Product", "Pastoral Product", "Raw Milk");
		AddMaterial("egg", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, "Food",
			"Animal Product", "Pastoral Product", "Egg Product");
		AddMaterial("saffron crocus", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Herb", "Textile Dye");
		EnsureTag(materials["milk"], "Pastoral Product");
		EnsureTag(materials["milk"], "Raw Milk");
		EnsureTag(materials["egg"], "Pastoral Product");
		EnsureTag(materials["egg"], "Egg Product");
		EnsureTag(materials["wool"], "Animal Product");
		EnsureTag(materials["wool"], "Pastoral Product");
		EnsureTag(materials["wool"], "Raw Wool");
		EnsureTag(materials["feces"], "Pastoral Product");
		EnsureTag(materials["feces"], "Manure Product");
		EnsureTag(materials["compost"], "Animal Product");
		EnsureTag(materials["compost"], "Pastoral Product");
		EnsureTag(materials["compost"], "Manure Product");
		EnsureAlias(materials["egg"], "eggs");
		EnsureAlias(materials["saffron crocus"], "crocus flower", "saffron flower");

		foreach (var name in new[]
		{
			"oat", "millet", "quinoa", "buckwheat", "sugarcane", "cacao", "coffee bean", "tea leaf", "hop",
			"alfalfa"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[] { "cotton crop", "hemp crop", "jute crop", "ramie", "sisal" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Fiber Crop");
		}

		foreach (var name in new[] { "canola", "rapeseed", "sesame", "sunflower", "safflower", "peanut crop", "olive crop" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Oil Crop");
		}

		foreach (var name in new[] { "almond", "hazelnut", "sugar beet", "beetroot", "squash" })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[] { "straw", "hay" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 0.2, true, 1000, 1000, 0.1, 0.08, 0.0001, 500,
				"Vegetation", "Agricultural Crop");
		}
		AddMaterial("reed", MaterialBehaviourType.Plant, 0.3, true, 5000, 5000, 0.1, 0.08, 0.0001, 500,
			"Vegetation");

		AddMaterial("firewood", MaterialBehaviourType.Wood, 0.5, true, 10000, 10000, 0.02, 0.15, 0.0001, 500,
			"Wood");
		AddMaterial("hazel", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420,
			"Hardwood");
		AddMaterial("rattan", MaterialBehaviourType.Wood, 0.4, true, 30000, 10000, 0.08, 0.14, 0.0001, 420,
			"Wood");
		AddMaterial("cane", MaterialBehaviourType.Wood, 0.4, true, 25000, 8000, 0.08, 0.14, 0.0001, 420,
			"Wood");
		AddMaterial("wicker", MaterialBehaviourType.Wood, 0.35, true, 15000, 8000, 0.12, 0.12, 0.0001, 500,
			"Manufactured Wood");

		EnsureAlias(materials["corn"], "maize");

		foreach (var name in new[]
		{
			"emmer wheat", "einkorn wheat", "spelt wheat", "naked barley", "new glume wheat", "teff",
			"fonio", "finger millet", "pearl millet", "foxtail millet", "proso millet", "amaranth",
			"canihua", "pitseed goosefoot", "maygrass", "little barley", "erect knotweed", "african rice"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[]
		{
			"grass pea", "cowpea", "mung bean", "pigeon pea", "bambara groundnut", "lupin", "adzuki bean"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[]
		{
			"oca", "ulluco", "mashua", "arrowroot", "lotus root", "water chestnut", "jerusalem artichoke",
			"bottle gourd"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Vegetable", "Food Crop");
		}

		foreach (var name in new[] { "chia", "marshelder seed", "niger seed" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Oil Crop");
		}

		foreach (var name in new[] { "kenaf" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Fiber Crop");
		}

		foreach (var name in new[] { "indigo crop" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Textile Dye");
		}

		foreach (var name in new[] { "breadfruit", "mulberry", "jackfruit", "tamarind" })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Fruit");
		}

		foreach (var name in new[] { "walnut nut", "pistachio", "cashew", "pecan", "macadamia", "kola nut" })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[] { "carob pod" })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[] { "cinnamon bark" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Spice");
		}

		foreach (var name in new[]
		         {
			         "tepary bean", "lima bean", "runner bean", "wild rice", "ethiopian oat", "guinea millet",
			         "african yam bean", "kersting groundnut", "lablab bean", "tarwi bean"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[]
		         {
			         "groundnut tuber", "camas bulb", "prairie turnip", "jicama", "maca root", "yacon root",
			         "arracacha root", "ahipa root", "enset starch", "mangelwurzel"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Vegetable", "Food Crop");
		}

		foreach (var name in new[]
		         {
			         "tomatillo", "chayote", "fluted pumpkin leaf", "jute mallow", "garden egg", "spinach",
			         "radish", "leek", "chicory", "rhubarb", "nopal pad"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Vegetable", "Food Crop");
		}

		foreach (var name in new[]
		         {
			         "egusi seed", "pumpkin seed", "poppy seed", "palm oil crop"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Oil Crop");
		}

		foreach (var name in new[] { "sainfoin hay", "clover hay" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 0.2, true, 1000, 1000, 0.1, 0.08, 0.0001, 500,
				"Vegetation", "Agricultural Crop");
		}

		foreach (var name in new[]
		         {
			         "prickly pear fruit", "pineapple", "palm fruit", "baobab fruit"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Fruit");
		}

		foreach (var name in new[] { "shea nut", "brazil nut" })
		{
			AddMaterial(name, MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Food Crop");
		}

		foreach (var name in new[]
		         {
			         "roselle", "agave leaf", "baobab leaf", "palm frond", "mulberry leaf", "cork bark",
			         "oak bark", "acorn", "maple sap"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Vegetation");
		}

		foreach (var name in new[] { "agave fibre", "henequen fibre", "raffia fibre" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Fiber Crop");
		}

		foreach (var name in new[]
		         {
			         "pine resin", "mastic resin", "gum arabic", "frankincense resin", "myrrh resin", "latex"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Paste, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Natural Materials");
		}

		foreach (var name in new[] { "yerba mate leaf" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Herb");
		}

		foreach (var name in new[]
		         {
			         "mopane", "miombo wood", "mangrove wood", "mulberry wood", "agarwood", "mesquite"
		         })
		{
			AddMaterial(name, MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420,
				"Hardwood");
		}

		foreach (var name in new[] { "rubber" })
		{
			AddMaterial(name, MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
				"Natural Materials");
		}

		foreach (var name in new[] { "charcoal" })
		{
			AddMaterial(name, MaterialBehaviourType.Powder, 0.4, true, 1000, 1000, 0.0, 0.08, 0.0001, 500,
				"Manufactured Materials");
		}

		EnsureAlias(materials["canihua"], "canahua", "kaniwa");
		EnsureAlias(materials["pitseed goosefoot"], "goosefoot", "chenopod");
		EnsureAlias(materials["marshelder seed"], "sumpweed", "marsh elder");
		EnsureAlias(materials["bambara groundnut"], "bambara nut", "earth pea");
		EnsureAlias(materials["african rice"], "oryza glaberrima");
		EnsureAlias(materials["mulberry"], "mulberry fruit");
		EnsureAlias(materials["macadamia"], "macadamia nut");
		EnsureAlias(materials["carob pod"], "carob");
		EnsureAlias(materials["cinnamon bark"], "cinnamon");
		EnsureAlias(materials["kola nut"], "cola nut");
		EnsureAlias(materials["kiwi fruit"], "kiwifruit");
		EnsureAlias(materials["walnut nut"], "walnut kernel", "walnut meat");
		EnsureAlias(materials["tepary bean"], "tepary");
		EnsureAlias(materials["kersting groundnut"], "kersting's groundnut", "kerstings groundnut");
		EnsureAlias(materials["sainfoin hay"], "sainfoin");
		EnsureAlias(materials["clover hay"], "clover");
		EnsureAlias(materials["yerba mate leaf"], "yerba mate", "mate leaf");
		EnsureAlias(materials["gum arabic"], "acacia gum");

		foreach (var name in AgricultureSeeder.StockSeedMaterialNamesForTesting)
		{
			if (materials.TryGetValue(name, out var material))
			{
				EnsureTag(material, "Agriculture Seedable");
			}
		}

		foreach (var name in new[]
		{
			"acacia", "applewood", "birch", "blackwood", "cypress", "holly", "hornbeam", "juniper", "larch",
			"linden", "olive wood", "pearwood", "poplar", "redwood", "rowan", "sycamore", "tamarind wood",
			"wenge", "ironwood", "jacaranda", "rosewood"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420,
				"Hardwood");
		}

		foreach (var name in new[]
		{
			"travertine", "serpentine", "pumice", "tuff", "scoria", "soapstone", "flint", "breccia", "dolostone",
			"arkose", "laterite", "marlstone", "trachyte", "komatiite", "anorthosite"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
				"Stone");
		}

		foreach (var name in new[]
		{
			"garnet", "zircon", "labradorite", "fluorite", "iolite", "kunzite", "morganite", "chrysoberyl",
			"bloodstone", "smoky quartz", "amazonite", "rhodonite", "charoite", "larimar", "tiger's eye",
			"unakite"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
				"Gemstone");
		}

		foreach (var name in new[]
		{
			"satin", "velvet", "lace", "muslin", "corduroy", "flannel", "gabardine", "seersucker", "twill",
			"cambric"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500,
				"Natural Fiber Fabric");
		}

		foreach (var name in new[]
		{
			"rayon", "viscose", "chiffon", "organza", "fleece", "jersey knit", "aramid fiber", "olefin fabric"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Fabric, 1.3, false, 10000, 25000, 0.0, 0.14, 0.0001, 500,
				"Synthetic Fiber Fabric");
		}

		foreach (var name in new[]
		{
			"ceramic tile", "stone tile", "porcelain tile", "mortar", "grout", "clinker", "firebrick",
			"glazed ceramic", "enamelware", "refractory brick"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Ceramic, 2.0, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
				"Ceramic");
		}

		foreach (var name in new[]
		{
			"acetal", "epoxy resin", "phenolic resin", "PTFE", "ethylene-vinyl acetate", "neoprene",
			"silicone rubber", "vinyl ester resin", "polyethylene foam"
		})
		{
			AddMaterial(name, MaterialBehaviourType.Plastic, 1.0, false, 25000, 55000, 0.0, 0.2, 0.0001, 1200,
				"Plastic");
		}

		AddMaterial("bone", MaterialBehaviourType.Bone, 1.8, true, 25000, 60000, 0.05, 0.32, 0.0001, 1300,
			"Animal Product");
		AddMaterial("gut", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Animal Product");
		AddMaterial("muscle", MaterialBehaviourType.Muscle, 1.05, true, 12000, 15000, 0.05, 0.5, 0.0001, 3400,
			"Animal Product");
		AddMaterial("blood", MaterialBehaviourType.Blood, 1.06, true, 1000, 1000, 0.0, 0.5, 0.7, 3600,
			"Animal Product");
		AddMaterial("skin", MaterialBehaviourType.Skin, 1.1, true, 12000, 15000, 0.2, 0.14, 0.0001, 2500,
			"Animal Skin");
		AddMaterial("claw", MaterialBehaviourType.Claw, 1.2, true, 20000, 50000, 0.05, 0.14, 0.0001, 500,
			"Animal Product");
		AddMaterial("beak", MaterialBehaviourType.Beak, 1.2, true, 20000, 50000, 0.05, 0.14, 0.0001, 500,
			"Animal Product");
		AddMaterial("rawhide", MaterialBehaviourType.Skin, 1.4, true, 18000, 10000, 0.25, 0.14, 0.0001, 500,
			"Animal Skin", "Animal Product");
		AddMaterial("sinew", MaterialBehaviourType.Muscle, 1.3, true, 30000, 40000, 0.1, 0.14, 0.0001, 500,
			"Animal Product");
		AddMaterial("horsehair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500,
			"Hair", "Animal Product");
		AddMaterial("goat leather", MaterialBehaviourType.Leather, 1.4, true, 26000, 10000, 0.2, 0.14, 0.0001, 500,
			"Leather");
		AddMaterial("sheep leather", MaterialBehaviourType.Leather, 1.3, true, 22000, 10000, 0.22, 0.14, 0.0001,
			500, "Leather");
		AddMaterial("resin", MaterialBehaviourType.Paste, 1.1, true, 8000, 12000, 0.0, 0.19, 0.0001, 1800,
			"Natural Materials");
		AddMaterial("pitch", MaterialBehaviourType.Paste, 1.2, false, 8000, 12000, 0.0, 0.15, 0.0001, 1700,
			"Manufactured Materials");
		AddMaterial("tar", MaterialBehaviourType.Grease, 1.15, false, 8000, 12000, 0.0, 0.12, 0.0001, 1600,
			"Manufactured Materials");
		AddMaterial("shellac", MaterialBehaviourType.Paste, 1.1, true, 8000, 12000, 0.0, 0.18, 0.0001, 1700,
			"Animal Product");
		AddMaterial("lacquer", MaterialBehaviourType.Paste, 1.1, true, 10000, 15000, 0.0, 0.18, 0.0001, 1700,
			"Manufactured Materials");
		AddMaterial("latex", MaterialBehaviourType.Elastomer, 0.95, true, 12000, 25000, 0.0, 0.14, 0.0001, 2000,
			"Elastomer");
		AddMaterial("charcoal", MaterialBehaviourType.Powder, 0.4, true, 2000, 2000, 0.2, 0.08, 0.0001, 1000,
			"Natural Materials");
		AddMaterial("soot", MaterialBehaviourType.Powder, 0.25, true, 1000, 1000, 0.2, 0.08, 0.0001, 900,
			"Natural Materials");
		EnsureAlias(materials["soot"], "lamp black", "carbon black");
		AddMaterial("chalk dust", MaterialBehaviourType.Powder, 0.7, false, 1000, 1000, 0.1, 0.16, 0.0001, 900,
			"Stone");
		AddMaterial("sponge", MaterialBehaviourType.Fabric, 0.3, true, 2000, 5000, 5.0, 0.05, 0.0001, 600,
			"Natural Materials");
		EnsureAlias(materials["rawhide"], "raw hide");
		EnsureAlias(materials["reed"], "reeds");
		EnsureAlias(materials["horsehair"], "horse hair");
		EnsureAlias(materials["goat leather"], "goatskin", "goatskin leather");
		EnsureAlias(materials["sheep leather"], "sheepskin", "sheepskin leather");
		EnsureAlias(materials["lacquer"], "urushi");

		AddMaterial("limonite ore", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Ore", "Iron Ore");
		AddMaterial("bog iron ore", MaterialBehaviourType.Ore, 3.2, false, 10000, 160000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Ore", "Iron Ore");
		AddMaterial("iron bloom", MaterialBehaviourType.Metal, 7.0, false, 80000, 0, 0.0, 17.9, 14500000, 500,
			"Primary Production Metal Stock", "Iron Age");
		AddMaterial("wrought iron billet", MaterialBehaviourType.Metal, 7.74, false, 107000, 0, 0.0, 17.9, 14500000, 500,
			"Primary Production Metal Stock", "Iron Age");
		AddMaterial("slag", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Waste");
		AddMaterial("dried peat", MaterialBehaviourType.Soil, 0.45, true, 100, 5000, 0.0, 0.10, 0.0001, 500,
			"Primary Production Fuel");
		AddMaterial("coal", MaterialBehaviourType.Stone, 1.35, false, 10000, 200000, 0.0, 0.25, 0.0001, 900,
			"Primary Production Fuel", "Hot Fire");
		AddMaterial("coke", MaterialBehaviourType.Stone, 0.85, false, 10000, 200000, 0.0, 0.20, 0.0001, 900,
			"Primary Production Fuel", "Hot Fire");
		AddMaterial("stone rubble", MaterialBehaviourType.Stone, 2.4, false, 10000, 120000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Waste", "Primary Production Aggregate");
		AddMaterial("gravel", MaterialBehaviourType.Stone, 1.8, false, 10000, 120000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Aggregate");
		AddMaterial("prepared clay", MaterialBehaviourType.Soil, 1.25, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Clay");
		AddMaterial("green brick", MaterialBehaviourType.Soil, 1.8, false, 1000, 10000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Clay");
		AddMaterial("fired brick", MaterialBehaviourType.Ceramic, 1.9, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			"Primary Production Stone", "Primary Production Clay", "Ceramic");
		AddMaterial("roof tile", MaterialBehaviourType.Ceramic, 1.9, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
			"Primary Production Stone", "Primary Production Clay", "Ceramic");
		AddMaterial("glass batch", MaterialBehaviourType.Powder, 1.6, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Glass Stock");
		AddMaterial("glass blank", MaterialBehaviourType.Ceramic, 2.5, false, 33000, 90000, 0.0, 1.0, 0.0001, 840,
			"Primary Production Glass Stock", "Glass");
		AddMaterial("potash", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Flux", "Primary Production Glass Stock", "Primary Production Alkali");
		AddMaterial("natron", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Alkali", "Primary Production Glass Stock");
		AddMaterial("bitumen", MaterialBehaviourType.Paste, 1.1, false, 8000, 12000, 0.0, 0.12, 0.0001, 1600,
			"Primary Production Tar And Pitch", "Primary Production Fuel");
		AddMaterial("malachite pigment", MaterialBehaviourType.Powder, 1.9, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Pigment", "Textile Dye");
		AddMaterial("azurite pigment", MaterialBehaviourType.Powder, 2.8, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Pigment", "Textile Dye");
		AddMaterial("cinnabar pigment", MaterialBehaviourType.Powder, 3.5, false, 1000, 1000, 0.0, 0.14, 0.0001, 500,
			"Primary Production Pigment", "Textile Dye");

		EnsureTag(materials["hematite"], "Primary Production Ore");
		EnsureTag(materials["magnetite"], "Primary Production Ore");
		EnsureTag(materials["cassiterite"], "Primary Production Ore");
		EnsureTag(materials["galena"], "Primary Production Ore");
		EnsureTag(materials["malachite"], "Primary Production Ore");
		EnsureTag(materials["malachite"], "Primary Production Pigment");
		EnsureTag(materials["native copper"], "Primary Production Ore");
		EnsureTag(materials["azurite"], "Primary Production Ore");
		EnsureTag(materials["azurite"], "Primary Production Pigment");
		EnsureTag(materials["cinnabar"], "Primary Production Ore");
		EnsureTag(materials["cinnabar"], "Primary Production Pigment");
		EnsureTag(materials["halite"], "Primary Production Salt");
		EnsureTag(materials["limestone"], "Primary Production Stone");
		EnsureTag(materials["limestone"], "Primary Production Flux");
		EnsureTag(materials["sandstone"], "Primary Production Stone");
		EnsureTag(materials["granite"], "Primary Production Stone");
		EnsureTag(materials["slate"], "Primary Production Stone");
		EnsureTag(materials["marble"], "Primary Production Stone");
		EnsureTag(materials["basalt"], "Primary Production Stone");
		EnsureTag(materials["sand"], "Primary Production Aggregate");
		EnsureTag(materials["sand"], "Primary Production Glass Stock");
		EnsureTag(materials["clay"], "Primary Production Clay");
		EnsureTag(materials["fire clay"], "Primary Production Clay");
		EnsureTag(materials["fire clay"], "Primary Production Refractory");
		EnsureTag(materials["brick"], "Primary Production Stone");
		EnsureTag(materials["brick"], "Primary Production Clay");
		EnsureTag(materials["firebrick"], "Primary Production Stone");
		EnsureTag(materials["firebrick"], "Primary Production Refractory");
		EnsureTag(materials["glass"], "Primary Production Glass Stock");
		EnsureTag(materials["charcoal"], "Primary Production Fuel");
		EnsureTag(materials["charcoal"], "Hot Fire");
		EnsureTag(materials["peat"], "Primary Production Fuel");
		EnsureTag(materials["calcium oxide"], "Primary Production Binder");
		EnsureTag(materials["slaked lime"], "Primary Production Binder");
		EnsureTag(materials["calcium hydroxide"], "Primary Production Binder");
		EnsureTag(materials["mortar"], "Primary Production Binder");
		EnsureTag(materials["plaster"], "Primary Production Binder");
		EnsureTag(materials["gypsum"], "Primary Production Binder");
		EnsureTag(materials["soda ash"], "Primary Production Flux");
		EnsureTag(materials["soda ash"], "Primary Production Glass Stock");
		EnsureTag(materials["soda ash"], "Primary Production Alkali");
		EnsureTag(materials["lye"], "Primary Production Alkali");
		EnsureTag(materials["wood ash"], "Primary Production Alkali");
		EnsureTag(materials["wood ash"], "Primary Production Waste");
		EnsureTag(materials["tar"], "Primary Production Tar And Pitch");
		EnsureTag(materials["pitch"], "Primary Production Tar And Pitch");
		EnsureTag(materials["resin"], "Primary Production Tar And Pitch");
		EnsureTag(materials["ochre pigment"], "Primary Production Pigment");
		EnsureTag(materials["red ochre pigment"], "Primary Production Pigment");
		EnsureTag(materials["yellow ochre pigment"], "Primary Production Pigment");
		EnsureTag(materials["sulfur"], "Primary Production Pigment");
		EnsureTag(materials["sulfur"], "Primary Production Ore");
		EnsureTag(materials["brimstone"], "Primary Production Pigment");
		EnsureTag(materials["brimstone"], "Primary Production Ore");
		EnsureTag(materials["saltpeter"], "Primary Production Alkali");
		EnsureTag(materials["wrought iron"], "Primary Production Metal Stock");
		EnsureTag(materials["sponge iron"], "Primary Production Metal Stock");
		EnsureTag(materials["copper"], "Primary Production Metal Stock");
		EnsureTag(materials["tin"], "Primary Production Metal Stock");
		EnsureTag(materials["lead"], "Primary Production Metal Stock");
		EnsureTag(materials["bronze"], "Primary Production Metal Stock");
		EnsureTag(materials["brass"], "Primary Production Metal Stock");

		EnsureAlias(materials["hematite"], "hematite ore", "haematite", "red iron ore");
		EnsureAlias(materials["magnetite"], "magnetite ore", "black iron ore");
		EnsureAlias(materials["cassiterite"], "tin ore", "ore tin");
		EnsureAlias(materials["galena"], "lead ore", "ore lead");
		EnsureAlias(materials["malachite"], "malachite ore", "copper carbonate ore");
		EnsureAlias(materials["native copper"], "native copper ore");
		EnsureAlias(materials["halite"], "rock salt");
		EnsureAlias(materials["salt"], "sea salt", "brine salt", "boiled salt");
		EnsureAlias(materials["peat"], "turf");
		EnsureAlias(materials["dried peat"], "dry turf", "peat fuel");
		EnsureAlias(materials["coal"], "mineral coal", "sea coal");
		EnsureAlias(materials["coke"], "coked coal");
		EnsureAlias(materials["limestone"], "chalk", "lime stone", "limestone flux");
		EnsureAlias(materials["sandstone"], "building sandstone");
		EnsureAlias(materials["granite"], "hardstone");
		EnsureAlias(materials["marble"], "building marble");
		EnsureAlias(materials["basalt"], "traprock");
		EnsureAlias(materials["slate"], "roofing slate");
		EnsureAlias(materials["stone rubble"], "rubble");
		EnsureAlias(materials["gravel"], "aggregate");
		EnsureAlias(materials["sand"], "silica sand");
		EnsureAlias(materials["calcium oxide"], "quicklime", "burnt lime");
		EnsureAlias(materials["slaked lime"], "hydrated lime");
		EnsureAlias(materials["mortar"], "lime mortar");
		EnsureAlias(materials["gypsum"], "plaster stone");
		EnsureAlias(materials["plaster"], "gypsum plaster");
		EnsureAlias(materials["clay"], "potter's clay");
		EnsureAlias(materials["fire clay"], "fireclay", "refractory clay");
		EnsureAlias(materials["prepared clay"], "clay body");
		EnsureAlias(materials["green brick"], "unfired brick");
		EnsureAlias(materials["fired brick"], "brick");
		EnsureAlias(materials["firebrick"], "refractory brick");
		EnsureAlias(materials["roof tile"], "tile");
		EnsureAlias(materials["glass batch"], "batch");
		EnsureAlias(materials["glass blank"], "glass stock");
		EnsureAlias(materials["potash"], "pearl ash");
		EnsureAlias(materials["soda ash"], "soda");
		EnsureAlias(materials["natron"], "natural soda");
		EnsureAlias(materials["lye"], "caustic lye");
		EnsureAlias(materials["wood ash"], "ash");
		EnsureAlias(materials["tar"], "pine tar", "wood tar");
		EnsureAlias(materials["pitch"], "boiled pitch");
		EnsureAlias(materials["resin"], "pine resin");
		EnsureAlias(materials["bitumen"], "asphalt");
		EnsureAlias(materials["ochre pigment"], "ochre");
		EnsureAlias(materials["malachite pigment"], "malachite green", "green earth");
		EnsureAlias(materials["azurite pigment"], "blue copper pigment");
		EnsureAlias(materials["cinnabar pigment"], "vermilion ore");
		EnsureAlias(materials["saltpeter"], "nitre", "niter", "potassium nitrate");
		EnsureAlias(materials["iron bloom"], "bloom");
		EnsureAlias(materials["wrought iron billet"], "iron billet");
		EnsureAlias(materials["slag"], "furnace slag");

		EnsureAlias(materials["PTFE"], "teflon");
		EnsureAlias(materials["silicone rubber"], "silicone");
		EnsureAlias(materials["ethylene-vinyl acetate"], "eva");
		EnsureAlias(materials["olefin fabric"], "polyolefin fabric");

		context.SaveChanges();
	}

    private void SeedMaterialsBase(FuturemudDatabaseContext context)
    {
        #region Tags

        Dictionary<string, Tag> tags = new(StringComparer.InvariantCultureIgnoreCase);

        void AddTag(string name, string? parent)
        {
            Tag tag = new()
            {
                Name = name
            };
            if (parent != null)
            {
                tag.Parent = tags[parent];
            }

            context.Tags.Add(tag);
            tags[name] = tag;
            context.SaveChanges();
        }

        AddTag("Materials", null);
        AddTag("Functions", null);
        AddTag("Material Functions", "Functions");
        AddTag("Hot Fire", "Material Functions");
        AddTag("Primary Production", "Material Functions");
        AddTag("Primary Production Ore", "Primary Production");
        AddTag("Primary Production Flux", "Primary Production");
        AddTag("Primary Production Fuel", "Primary Production");
        AddTag("Primary Production Stone", "Primary Production");
        AddTag("Primary Production Clay", "Primary Production");
        AddTag("Primary Production Aggregate", "Primary Production");
        AddTag("Primary Production Metal Stock", "Primary Production");
        AddTag("Primary Production Binder", "Primary Production");
        AddTag("Primary Production Glass Stock", "Primary Production");
        AddTag("Primary Production Salt", "Primary Production");
        AddTag("Primary Production Alkali", "Primary Production");
        AddTag("Primary Production Tar And Pitch", "Primary Production");
        AddTag("Primary Production Pigment", "Primary Production");
        AddTag("Primary Production Refractory", "Primary Production");
        AddTag("Primary Production Waste", "Primary Production");
        AddTag("Primary Production Resource", "Primary Production");
        AddTag("Primary Production Commodity", "Primary Production");
        AddTag("Simplified", "Materials");
        AddTag("Animal Product", "Materials");
        AddTag("Butchery Output", "Animal Product");
        AddTag("Raw Meat Cut", "Butchery Output");
        AddTag("Raw Hide", "Butchery Output");
        AddTag("Offal", "Butchery Output");
        AddTag("Trophy Part", "Butchery Output");
        AddTag("Venom Organ", "Butchery Output");
        AddTag("Crafting Animal Product", "Butchery Output");
        AddTag("Apiary Product", "Animal Product");
        AddTag("Raw Honeycomb", "Apiary Product");
        AddTag("Pressed Honey", "Apiary Product");
        AddTag("Rendered Beeswax", "Apiary Product");
        AddTag("Pastoral Product", "Animal Product");
        AddTag("Raw Milk", "Pastoral Product");
        AddTag("Raw Wool", "Pastoral Product");
        AddTag("Egg Product", "Pastoral Product");
        AddTag("Manure Product", "Pastoral Product");
        AddTag("Shell", "Animal Product");
        AddTag("Tortoiseshell", "Animal Product");
        AddTag("Natural Materials", "Materials");
        AddTag("Manufactured Materials", "Materials");
        AddTag("Stone", "Natural Materials");
        AddTag("Vegetation", "Natural Materials");
        AddTag("Flower", "Vegetation");
        AddTag("Dried Flower", "Flower");
        AddTag("Leaf", "Vegetation");
        AddTag("Rush", "Vegetation");
        AddTag("Economically Useful Stone", "Stone");
        AddTag("Feldspar", "Economically Useful Stone");
        AddTag("Calcite", "Economically Useful Stone");
        AddTag("Gypsum", "Economically Useful Stone");
        AddTag("Soda Ash", "Economically Useful Stone");
        AddTag("Zeolite", "Economically Useful Stone");
        AddTag("Gemstone", "Economically Useful Stone");
        AddTag("Metal Ore", "Stone");
        AddTag("Aluminium Ore", "Metal Ore");
        AddTag("Antimony Ore", "Metal Ore");
        AddTag("Arsenic Ore", "Metal Ore");
        AddTag("Barium Ore", "Metal Ore");
        AddTag("Beryllium Ore", "Metal Ore");
        AddTag("Bismuth Ore", "Metal Ore");
        AddTag("Boron Ore", "Metal Ore");
        AddTag("Cesium Ore", "Metal Ore");
        AddTag("Chromium Ore", "Metal Ore");
        AddTag("Cobalt Ore", "Metal Ore");
        AddTag("Copper Ore", "Metal Ore");
        AddTag("Copper Oxide Ore", "Copper Ore");
        AddTag("Copper Sulphide Ore", "Copper Ore");
        AddTag("Gold Ore", "Metal Ore");
        AddTag("Hafnium Ore", "Metal Ore");
        AddTag("Iron Ore", "Metal Ore");
        AddTag("Lead Ore", "Metal Ore");
        AddTag("Lithium Ore", "Metal Ore");
        AddTag("Magnesium Ore", "Metal Ore");
        AddTag("Manganese Ore", "Metal Ore");
        AddTag("Mercury Ore", "Metal Ore");
        AddTag("Molybdenum Ore", "Metal Ore");
        AddTag("Nickel Ore", "Metal Ore");
        AddTag("Niobium Ore", "Metal Ore");
        AddTag("Palladium Ore", "Metal Ore");
        AddTag("Platinum Ore", "Metal Ore");
        AddTag("Potassium Ore", "Metal Ore");
        AddTag("Rare Earth Ore", "Metal Ore");
        AddTag("Rhodium Ore", "Metal Ore");
        AddTag("Rubidium Ore", "Metal Ore");
        AddTag("Silver Ore", "Metal Ore");
        AddTag("Sodium Ore", "Metal Ore");
        AddTag("Strontium Ore", "Metal Ore");
        AddTag("Tantalum Ore", "Metal Ore");
        AddTag("Tin Ore", "Metal Ore");
        AddTag("Titanium Ore", "Metal Ore");
        AddTag("Thorium Ore", "Metal Ore");
        AddTag("Tungsten Ore", "Metal Ore");
        AddTag("Vanadium Ore", "Metal Ore");
        AddTag("Zinc Ore", "Metal Ore");
        AddTag("Native Copper Ore", "Copper Ore");
        AddTag("Native Nickel Ore", "Nickel Ore");
        AddTag("Native Gold Ore", "Gold Ore");
        AddTag("Native Silver Ore", "Silver Ore");
        AddTag("Native Platinum Ore", "Platinum Ore");
        AddTag("Native Tin Ore", "Tin Ore");
        AddTag("Economic Stone", "Stone");
        AddTag("Elemental Metal", "Natural Materials");
        AddTag("Manufactured Metal", "Manufactured Materials");
        AddTag("Glass", "Manufactured Materials");
        AddTag("Bronze Age", "Manufactured Metal");
        AddTag("Iron Age", "Manufactured Metal");
        AddTag("Medieval Age", "Manufactured Metal");
        AddTag("Renaissance Age", "Manufactured Metal");
        AddTag("Industrial Age", "Manufactured Metal");
        AddTag("Modern Age", "Manufactured Metal");
        AddTag("Cast Metal", "Manufactured Metal");
        AddTag("Forged Metal", "Manufactured Metal");
        AddTag("Precious Metal", "Manufactured Metal");
        AddTag("Gilded Metal", "Manufactured Metal");
        AddTag("Inlay Material", "Manufactured Metal");
        AddTag("Soil", "Natural Materials");
        AddTag("Wood", "Natural Materials");
        AddTag("Hardwood", "Wood");
        AddTag("Softwood", "Wood");
        AddTag("Manufactured Wood", "Wood");
        AddTag("Food", "Natural Materials");
        AddTag("Meat", "Food");
        AddTag("Vegetable", "Food");
        AddTag("Fruit", "Food");
        AddTag("Baked Good", "Food");
        AddTag("Herb", "Food");
        AddTag("Spice", "Food");
        AddTag("Textile Dye", "Materials");
        AddTag("Textile Mordant", "Materials");
        AddTag("Hair", "Natural Materials");
        AddTag("Agricultural Crop", "Natural Materials");
        AddTag("Agriculture Seedable", "Agricultural Crop");
        AddTag("Food Crop", "Agricultural Crop");
        AddTag("Fiber Crop", "Agricultural Crop");
        AddTag("Oil Crop", "Agricultural Crop");
        AddTag("Animal Skin", "Natural Materials");
        AddTag("Thick Animal Skin", "Animal Skin");
        AddTag("Leather", "Natural Materials");
        AddTag("Fabric", "Manufactured Materials");
        AddTag("Natural Fiber Fabric", "Fabric");
        AddTag("Animal Fiber Fabric", "Fabric");
        AddTag("Synthetic Fiber Fabric", "Fabric");
        AddTag("Blended Fiber Fabric", "Fabric");
        AddTag("Ceramic", "Manufactured Materials");
        AddTag("Faience", "Ceramic");
        AddTag("Enamel", "Glass");
        AddTag("Plastic", "Manufactured Materials");
        AddTag("Elastomer", "Materials");
        AddTag("Elemental Materials", "Materials");
        AddTag("Paper Product", "Manufactured Materials");
        AddTag("Writing Product", "Materials");
        AddTag("Gunpowder", "Materials");
        AddTag("Liquids", "Materials");
        AddTag("Water", "Liquids");
        AddTag("Detergent", "Liquids");
        AddTag("Water Soluable", "Liquids");
        AddTag("Beverage", "Liquids");
        AddTag("Alcoholic", "Beverage");
        AddTag("Fuel", "Liquids");
        AddTag("Disgusting", "Liquids");
        AddTag("Ritual Offerings", "Liquids");
        AddTag("Libation", "Ritual Offerings");
        AddTag("Lamp Oil", "Ritual Offerings");
        AddTag("Blood Offering", "Ritual Offerings");

        #endregion

        Dictionary<string, Material> materials = new(StringComparer.InvariantCultureIgnoreCase);
        Dictionary<Material, string> solvents = new();

        void AddMaterial(string name, MaterialBehaviourType type, double relativeDensity, bool organic,
            double shearStrength, double impactStrength, double absorbency, double thermalConductivity,
            double electricalConductivity, double specificHeatCapacity, ResidueInformation? residue = null,
            params string[] materialTags)
        {
            Material material = new()
            {
                Name = name,
                MaterialDescription = name,
                BehaviourType = (int)type,
                Type = 0,
                Density = 1000 * relativeDensity,
                Organic = organic,
                Absorbency = absorbency,
                ShearYield = shearStrength,
                ImpactYield = impactStrength > 0.0 ? impactStrength : shearStrength * 2.2,
                ElectricalConductivity = electricalConductivity,
                ThermalConductivity = thermalConductivity,
                SpecificHeatCapacity = specificHeatCapacity
            };
            if (residue != null)
            {
                material.ResidueSdesc = residue.ResidueSdesc;
                material.ResidueDesc = residue.ResidueDesc;
                material.ResidueColour = residue.ResidueColour;
                material.SolventVolumeRatio = residue.SolventRatio;
                if (residue.Solvent != null)
                {
                    solvents[material] = residue.Solvent;
                }
            }
            else
            {
                material.ResidueColour = "white";
                material.SolventVolumeRatio = 1.0;
            }

            materials[name] = material;
            context.Materials.Add(material);
            foreach (string tag in materialTags)
            {
                material.MaterialsTags.Add(new MaterialsTags { Material = material, Tag = tags[tag] });
            }

            context.SaveChanges();
        }

        void AddMaterialAliases(string materialName, params string[] aliases)
        {
            Material material = materials[materialName];
            foreach (string? alias in aliases.Select(x => x.ToLowerInvariant()).Distinct())
            {
                material.MaterialAliases.Add(new MaterialAlias
                {
                    Material = material,
                    Alias = alias
                });
            }

            context.SaveChanges();
        }

        #region Simplified
        AddMaterial("textile", MaterialBehaviourType.Fabric, 1.0, true, 10000, 10000, 0.3, 10.0, 0.0001, 500, null, "Simplified", "Fabric");
        AddMaterial("wood", MaterialBehaviourType.Wood, 0.5, true, 10000, 10000, 0.01, 0.15, 0.0001, 500, null, "Simplified", "Wood");
        AddMaterial("metal", MaterialBehaviourType.Metal, 7.0, false, 40000, 10000, 0.0, 18.0, 14500000, 500, null, "Simplified", "Manufactured Metal");
        AddMaterial("stone", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, null, "Simplified", "Stone");
        AddMaterial("glass", MaterialBehaviourType.Ceramic, 2.5, false, 33000, 90000, 0.0, 1.0, 0.0001, 840, null, "Simplified", "Glass");
        AddMaterial("vegetation", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.01, 10.0, 0.0001, 500, null, "Simplified", "Vegetation");
        AddMaterial("ceramic", MaterialBehaviourType.Ceramic, 2.4, false, 40000, 120000, 0.0, 1.5, 0.0001, 800, null, "Simplified", "Ceramic");
        AddMaterial("meat", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.1, 0.14, 0.0001, 500, null, "Simplified", "Meat");
        AddMaterial("other", MaterialBehaviourType.Mana, 1.0, false, 10000, 10000, 0.3, 0.14, 0.0001, 500, null, "Simplified");
        #endregion

        #region Metals

        AddMaterial("aluminium", MaterialBehaviourType.Metal, 2.7, false, 90000, 0, 0.0, 205.0, 37700000, 900,
            materialTags: "Modern Age");
        AddMaterialAliases("aluminium", "aluminum");
        AddMaterial("antimony", MaterialBehaviourType.Metal, 6.68, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("arsenic", MaterialBehaviourType.Metal, 5.7, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("arsenical bronze", MaterialBehaviourType.Metal, 7.85, false, 250000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("bell bronze", MaterialBehaviourType.Metal, 8.5, false, 250000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Bronze Age", "Cast Metal"]);
        AddMaterial("beryllium", MaterialBehaviourType.Metal, 1.85, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("bismuth bronze", MaterialBehaviourType.Metal, 7.85, false, 314000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("bismuth", MaterialBehaviourType.Metal, 9.79, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("boron", MaterialBehaviourType.Metal, 2.3, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("brass", MaterialBehaviourType.Metal, 8.4, false, 207000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("bromine", MaterialBehaviourType.Metal, 6.6, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("bronze", MaterialBehaviourType.Metal, 8.7, false, 314000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("cadmium", MaterialBehaviourType.Metal, 8.69, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("carbon steel", MaterialBehaviourType.Metal, 7.85, false, 340000, 0, 0.0, 50.0, 6100000, 490,
            materialTags: "Modern Age");
        AddMaterial("cast iron", MaterialBehaviourType.Metal, 7.1, false, 40000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Iron Age", "Cast Metal"]);
        AddMaterial("cesium", MaterialBehaviourType.Metal, 1.93, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("chromium", MaterialBehaviourType.Metal, 7.15, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("cobalt", MaterialBehaviourType.Metal, 8.86, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Renaissance Age");
        AddMaterial("copper", MaterialBehaviourType.Metal, 8.96, false, 68000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("crucible steel", MaterialBehaviourType.Metal, 7.85, false, 240000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Renaissance Age");
        AddMaterial("electrum", MaterialBehaviourType.Metal, 8.8, false, 85000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("gallium", MaterialBehaviourType.Metal, 5.91, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("galvanized steel", MaterialBehaviourType.Metal, 7.95, false, 320000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("germanium", MaterialBehaviourType.Metal, 5.5, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("gold", MaterialBehaviourType.Metal, 19.3, false, 120000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Bronze Age", "Precious Metal"]);
        AddMaterial("hafnium", MaterialBehaviourType.Metal, 13.3, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("high tensile steel", MaterialBehaviourType.Metal, 7.85, false, 650000, 0, 0.0, 50.0, 6100000, 490,
            materialTags: "Modern Age");
        AddMaterial("indium", MaterialBehaviourType.Metal, 7.31, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("iridium", MaterialBehaviourType.Metal, 22.562, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Industrial Age", "Precious Metal"]);
        AddMaterial("lead", MaterialBehaviourType.Metal, 11.3, false, 131000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("magnesium", MaterialBehaviourType.Metal, 1.74, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("manganese steel", MaterialBehaviourType.Metal, 8.0, false, 450000, 0, 0.0, 28.0, 1350000, 500,
            materialTags: "Modern Age");
        AddMaterial("manganese", MaterialBehaviourType.Metal, 7.3, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("mild bronze", MaterialBehaviourType.Metal, 8.7, false, 275000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("mild steel", MaterialBehaviourType.Metal, 7.85, false, 250000, 0, 0.0, 50.0, 6100000, 490,
            materialTags: "Modern Age");
        AddMaterialAliases("mild steel", "steel");
        AddMaterial("molybdenum", MaterialBehaviourType.Metal, 10.2, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("neodymium", MaterialBehaviourType.Metal, 7.01, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("nickel brass", MaterialBehaviourType.Metal, 8.4, false, 207000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("nickel", MaterialBehaviourType.Metal, 8.9, false, 58600, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("niobium", MaterialBehaviourType.Metal, 8.57, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("open hearth steel", MaterialBehaviourType.Metal, 7.85, false, 300000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("orichalcum", MaterialBehaviourType.Metal, 8.4, false, 207000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("osmium", MaterialBehaviourType.Metal, 22.59, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Industrial Age", "Precious Metal"]);
        AddMaterial("palladium", MaterialBehaviourType.Metal, 12.0, false, 180000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Industrial Age", "Precious Metal"]);
        AddMaterial("pewter", MaterialBehaviourType.Metal, 7.25, false, 30000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("phosphorus", MaterialBehaviourType.Metal, 1.82, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("pig iron", MaterialBehaviourType.Metal, 7.1, false, 75000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("platinum", MaterialBehaviourType.Metal, 21.5, false, 165000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Iron Age", "Precious Metal"]);
        AddMaterial("potassium", MaterialBehaviourType.Metal, 0.89, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("powder-coated steel", MaterialBehaviourType.Metal, 8.0, false, 320000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("rare earth metal", MaterialBehaviourType.Metal, 2.99, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("rhenium", MaterialBehaviourType.Metal, 20.8, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("rhodium", MaterialBehaviourType.Metal, 12.4, false, 700000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Industrial Age", "Precious Metal"]);
        AddMaterial("rubidium", MaterialBehaviourType.Metal, 1.53, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("ruthenium", MaterialBehaviourType.Metal, 12.2, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Industrial Age", "Precious Metal"]);
        AddMaterial("selenium", MaterialBehaviourType.Metal, 4.81, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("silicon", MaterialBehaviourType.Metal, 2.33, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("silver", MaterialBehaviourType.Metal, 10.5, false, 55700, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Bronze Age", "Precious Metal"]);
        AddMaterial("sodium", MaterialBehaviourType.Metal, 0.97, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("spelter", MaterialBehaviourType.Metal, 7.85, false, 25000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("sponge iron", MaterialBehaviourType.Metal, 7.1, false, 75000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Iron Age");
        AddMaterial("stainless steel", MaterialBehaviourType.Metal, 7.9, false, 290000, 0, 0.0, 16.0, 1450000, 500,
            materialTags: "Modern Age");
        AddMaterial("spring steel", MaterialBehaviourType.Metal, 7.85, false, 620000, 0, 0.0, 49.0, 5000000, 480,
            materialTags: "Modern Age");
        AddMaterial("tool steel", MaterialBehaviourType.Metal, 7.75, false, 550000, 0, 0.0, 24.0, 1400000, 460,
            materialTags: "Modern Age");
        AddMaterial("sterling silver", MaterialBehaviourType.Metal, 7.85, false, 55700, 0, 0.0, 17.9, 14500000, 500,
            materialTags: ["Renaissance Age", "Precious Metal"]);
        AddMaterial("strontium", MaterialBehaviourType.Metal, 2.64, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("sulfur", MaterialBehaviourType.Metal, 2, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("tantalum", MaterialBehaviourType.Metal, 16.4, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("tellurium", MaterialBehaviourType.Metal, 6.24, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("thallium", MaterialBehaviourType.Metal, 11.8, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("thorium", MaterialBehaviourType.Metal, 11.7, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("tin", MaterialBehaviourType.Metal, 7.26, false, 11800, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("titanium", MaterialBehaviourType.Metal, 4.51, false, 275000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("tungsten", MaterialBehaviourType.Metal, 19.3, false, 750000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");
        AddMaterial("uranium", MaterialBehaviourType.Metal, 19.1, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("vanadium", MaterialBehaviourType.Metal, 6.0, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Modern Age");
        AddMaterial("wootz steel", MaterialBehaviourType.Metal, 7.85, false, 240000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Medieval Age");
        AddMaterial("wrought iron", MaterialBehaviourType.Metal, 7.74, false, 107000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Iron Age");
        AddMaterial("zinc", MaterialBehaviourType.Metal, 7.14, false, 124000, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Bronze Age");
        AddMaterial("zirconium", MaterialBehaviourType.Metal, 6.52, false, 10500, 0, 0.0, 17.9, 14500000, 500,
            materialTags: "Industrial Age");

        #endregion

        #region Ore

        AddMaterial("acanthite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Silver Ore");
        AddMaterial("anglesite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Lead Ore");
        AddMaterial("argentite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Silver Ore");
        AddMaterial("arsenopyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Arsenic Ore");
        AddMaterial("azurite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Oxide Ore");
        AddMaterial("barite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Barium Ore");
        AddMaterial("bastnasite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Rare Earth Ore");
        AddMaterial("bauxite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Aluminium Ore");
        AddMaterial("bertrandite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Beryllium Ore");
        AddMaterial("bismuthinite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Bismuth Ore");
        AddMaterial("borax", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Boron Ore");
        AddMaterial("bornite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Sulphide Ore");
        AddMaterial("braunite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Manganese Ore");
        AddMaterial("brochantite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Oxide Ore");
        AddMaterial("brucite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Magnesium Ore");
        AddMaterial("calaverite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Gold Ore");
        AddMaterial("carnallite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Magnesium Ore");
        AddMaterial("cassiterite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Tin Ore");
        AddMaterial("celestite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Strontium Ore");
        AddMaterial("cerargyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Silver Ore");
        AddMaterial("cerussite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Lead Ore");
        AddMaterial("chalcopyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Sulphide Ore");
        AddMaterial("chalcocite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Sulphide Ore");
        AddMaterial("chromite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Chromium Ore");
        AddMaterial("chrysocolla", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Oxide Ore");
        AddMaterial("cinnabar", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Mercury Ore");
        AddMaterial("cobaltite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Cobalt Ore");
        AddMaterial("colemanite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Boron Ore");
        AddMaterial("columbite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Niobium Ore");
        AddMaterial("cuprite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Oxide Ore");
        AddMaterial("djurleite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Sulphide Ore");
        AddMaterial("dolomite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Magnesium Ore");
        AddMaterial("galena", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Lead Ore");
        AddMaterial("halite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Sodium Ore");
        AddMaterial("hausmannite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Manganese Ore");
        AddMaterial("hematite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Iron Ore");
        AddMaterial("ilmenite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Titanium Ore");
        AddMaterial("kernite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Boron Ore");
        AddMaterial("lepidolite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Rubidium Ore");
        AddMaterial("leucoxene", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Titanium Ore");
        AddMaterial("loparite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Rare Earth Ore");
        AddMaterial("magnesite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Magnesium Ore");
        AddMaterial("magnetite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Iron Ore");
        AddMaterial("malachite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Oxide Ore");
        AddMaterial("molybdenite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Molybdenum Ore");
        AddMaterial("monazite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Rare Earth Ore");
        AddMaterial("native copper", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Native Copper Ore");
        AddMaterial("native gold", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Native Nickel Ore");
        AddMaterial("native nickel", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Native Gold Ore");
        AddMaterial("native platinum", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Native Platinum Ore");
        AddMaterial("native silver", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Native Silver Ore");
        AddMaterial("native tin", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Native Tin Ore");
        AddMaterial("olivine", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Magnesium Ore");
        AddMaterial("pegmatite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Lithium Ore");
        AddMaterial("pentlandite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Nickel Ore");
        AddMaterial("petzite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Gold Ore");
        AddMaterial("pollucite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Cesium Ore");
        AddMaterial("proustite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Silver Ore");
        AddMaterial("pyrargyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Silver Ore");
        AddMaterial("pyrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Iron Ore");
        AddMaterial("pyrolusite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Manganese Ore");
        AddMaterial("rhodochrosite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Manganese Ore");
        AddMaterial("rutile", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Titanium Ore");
        AddMaterial("scheelite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Tungsten Ore");
        AddMaterial("siderite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Iron Ore");
        AddMaterial("sperrylite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Platinum Ore");
        AddMaterial("sphalerite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Zinc Ore");
        AddMaterial("stibnite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Antimony Ore");
        AddMaterial("strontianite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Strontium Ore");
        AddMaterial("sylvanite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Gold Ore");
        AddMaterial("sylvite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Potassium Ore");
        AddMaterial("tantalite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Tantalum Ore");
        AddMaterial("tenorite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Copper Oxide Ore");
        AddMaterial("tetrahedrite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Copper Sulphide Ore", "Silver Ore", "Antimony Ore");
        AddMaterial("ulexite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Boron Ore");
        AddMaterial("witherite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Barium Ore");
        AddMaterial("wolframite", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Tungsten Ore");
        AddMaterial("zircon", MaterialBehaviourType.Ore, 4.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            materialTags: "Hafnium Ore");

        #endregion

        #region Stone

        ResidueInformation dustResidue = new("(dusty)", "It is covered in a layer of dust");
        AddMaterial("limestone", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Calcite");
        AddMaterial("orthoclase", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Feldspar");
        AddMaterial("microcline", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Feldspar");
        AddMaterial("albite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Feldspar");
        AddMaterial("gypsum", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Gypsum");
        AddMaterial("alabaster", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Gypsum");
        AddMaterial("selenite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Gypsum");
        AddMaterial("satinspar", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Gypsum");
        AddMaterial("perlite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("soda ash", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Soda Ash");
        AddMaterial("chabazite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Zeolite");
        AddMaterial("clinoptilolite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Zeolite");
        AddMaterial("mordenite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Zeolite");
        AddMaterial("wollastonite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("vermiculite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("talc", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, dustResidue,
            "Economically Useful Stone");
        AddMaterial("pyrophyllite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("graphite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("kyanite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("andalusite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("muscovite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("phlogopite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("brimstone", MaterialBehaviourType.Stone, 2.07, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("kaolinite", MaterialBehaviourType.Stone, 2.07, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");
        AddMaterial("saltpeter", MaterialBehaviourType.Stone, 2.07, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Economically Useful Stone");

        AddMaterial("sandstone", MaterialBehaviourType.Stone, 2.4, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("siltstone", MaterialBehaviourType.Stone, 2.5, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("mudstone", MaterialBehaviourType.Stone, 2.51, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("shale", MaterialBehaviourType.Stone, 2.25, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("claystone", MaterialBehaviourType.Stone, 2.7, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("conglomerate", MaterialBehaviourType.Stone, 2.0, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("chert", MaterialBehaviourType.Stone, 2.65, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("chalk", MaterialBehaviourType.Stone, 2.71, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("granite", MaterialBehaviourType.Stone, 2.6, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("diorite", MaterialBehaviourType.Stone, 2.87, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("gabbro", MaterialBehaviourType.Stone, 2.92, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("rhyolite", MaterialBehaviourType.Stone, 2.6, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("basalt", MaterialBehaviourType.Stone, 2.85, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("andesite", MaterialBehaviourType.Stone, 2.43, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("dacite", MaterialBehaviourType.Stone, 2.4, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("obsidian", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("jet", MaterialBehaviourType.Stone, 1.32, false, 10000, 200000, 0.0, 0.14, 0.0001, 500, dustResidue,
            "Stone");
        AddMaterial("quartzite", MaterialBehaviourType.Stone, 2.6, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("slate", MaterialBehaviourType.Stone, 2.75, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("phyllite", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("schist", MaterialBehaviourType.Stone, 2.9, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("gneiss", MaterialBehaviourType.Stone, 2.8, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");
        AddMaterial("marble", MaterialBehaviourType.Stone, 2.78, false, 10000, 200000, 0.0, 0.14, 0.0001, 500,
            dustResidue, "Stone");

        #endregion

        #region Gem Stones

        AddMaterial("diamond", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("amethyst", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("sapphire", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("turquoise", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("opal", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("jade", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("emerald", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("amber", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("pearl", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("lapis lazuli", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("topaz", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("nephrite", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("moonstone", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("agate", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("quartz", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("rose quartz", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("tourmaline", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("onyx", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("peridot", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("ruby", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("citrine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("alexandrite", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("spinel", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("aquamarine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("carnelian", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("jasper", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("aventurine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("chalcedony", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500,
            null, "Gemstone");
        AddMaterial("beryl", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("sunstone", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");
        AddMaterial("ametrine", MaterialBehaviourType.Stone, 3.5, false, 60000000, 200000, 0.0, 0.14, 0.0001, 500, null,
            "Gemstone");

        #endregion

        #region Wood

        ResidueInformation sawdust = new("(sawdust)", "It is covered in a layer of sawdust", "yellow");
        AddMaterial("alder", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("ash", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("aspen", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("bamboo", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("beech", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("boxwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("cedar", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Softwood");
        AddMaterial("cherry", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("chestnut", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("coach", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("cork", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("cottonwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("dogwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("ebony", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("elm", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("eucalyptus", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("fir", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Softwood");
        AddMaterial("hickory", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("mahogany", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("maple", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("oak", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("particle board", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420,
            sawdust, "Manufactured Wood");
        AddMaterial("medium density fiberboard", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14,
            0.0001, 420, sawdust, "Manufactured Wood");
        AddMaterial("plywood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Manufactured Wood");
        AddMaterial("pine", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Softwood");
        AddMaterial("sandalwood", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("spruce", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("teak", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("walnut", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("willow", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");
        AddMaterial("yew", MaterialBehaviourType.Wood, 0.5, true, 40000, 10000, 0.05, 0.14, 0.0001, 420, sawdust,
            "Hardwood");

        #endregion

        #region Soil

        AddMaterial("clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
        AddMaterial("sodic clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
        AddMaterial("kaolinite clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
        AddMaterial("fire clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
        AddMaterial("pelagic clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 7.0), "Soil");
        AddMaterial("silty clay", MaterialBehaviourType.Soil, 1.21, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 6.0), "Soil");
        AddMaterial("sandy clay", MaterialBehaviourType.Soil, 1.330, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 5.0), "Soil");
        AddMaterial("clay loam", MaterialBehaviourType.Soil, 1.32, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 5.0), "Soil");
        AddMaterial("sandy clay loam", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
        AddMaterial("silty clay loam", MaterialBehaviourType.Soil, 1.29, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
        AddMaterial("loam", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
        AddMaterial("sandy loam", MaterialBehaviourType.Soil, 1.56, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
        AddMaterial("silt loam", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(muddy)", "It is covered in a layer of dry mud", "yellow", "water", 4.0), "Soil");
        AddMaterial("loamy sand", MaterialBehaviourType.Soil, 1.41, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(sandy)", "It is covered in a layer of dry sand", "yellow", "water", 2.0), "Soil");
        AddMaterial("silt", MaterialBehaviourType.Soil, 1.45, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(silty)", "It is covered in a layer of dry silt", "yellow", "water", 2.0), "Soil");
        AddMaterial("sand", MaterialBehaviourType.Soil, 1.71, false, 100, 5000, 0.0, 0.14, 0.0001, 500,
            new ResidueInformation("(sandy)", "It is covered in a layer of dry sand", "yellow", "water", 2.0), "Soil");
        AddMaterial("peat", MaterialBehaviourType.Soil, 0.85, false, 100, 5000, 0.0, 0.14, 0.0001, 500, null, "Soil");

        #endregion

        #region Food
        AddMaterial("food", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food");
        AddMaterial("beef", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("pork", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("lamb", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("chicken", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("rabbit", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("venison", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("game bird", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("camel", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("dog", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("cat", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("insect", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("fish", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("crab", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("shark", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("whale", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("shellfish", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("shrimp", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("squid", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("mollusc", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("human meat", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("game mammal", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("snake", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("lizard", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("frog", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");
        AddMaterial("turtle", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null,
            "Meat");
        AddMaterial("snail", MaterialBehaviourType.Meat, 1.3, true, 10000, 10000, 0.0, 0.14, 0.0001, 500, null, "Meat");

        AddMaterial("fruit", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("apple", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("banana", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("pear", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("peach", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("plum", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("grape", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("mango", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("pineapple", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Fruit");
        AddMaterial("melon", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("pomegranate", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Fruit");
        AddMaterial("berry", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("apricot", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Fruit");
        AddMaterial("olive", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("tree nut", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Fruit");
        AddMaterial("peanut", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("lemon", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("lime", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");
        AddMaterial("orange", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Fruit");

        AddMaterial("vegetable", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("potato", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("tomato", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("pepper", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("pumpkin", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("sweet potato", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("pea", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("green bean", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("bean", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("legume", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("lentil", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("corn", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("onion", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("garlic", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("leek", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("carrot", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("lettuce", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("spinach", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("cabbage", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("cauliflower", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("broccoli", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("radish", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("aubergene", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("avocado", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("beet", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("cucumber", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("zucchini", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("yam", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("tuber", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("turnip", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");
        AddMaterial("greens", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Vegetable");

        AddMaterial("herb", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
        AddMaterial("parsley", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("sage", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
        AddMaterial("rosemary", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("thyme", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
        AddMaterial("oregano", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("basil", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
        AddMaterial("dill", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
        AddMaterial("mint", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
        AddMaterial("cilantro", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("fennel", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Herb");
        AddMaterial("peppermint", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("aloe vera", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("yarrow", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("mandrake", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("henbane", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("foxglove", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("ephedra", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");
        AddMaterial("poppy latex", MaterialBehaviourType.Paste, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Herb");

        AddMaterial("spice", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("black pepper", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("salt", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Spice");
        AddMaterial("chilli powder", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("coriander", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("cayenne pepper", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("paprika", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("cumin", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("cinnamon", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("nutmeg", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("cloves", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("turmeric", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("saffron", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");
        AddMaterial("ginger", MaterialBehaviourType.Powder, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Spice");

        AddMaterial("pie", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Baked Good");
        AddMaterial("pastry", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Baked Good");
        AddMaterial("bread", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Baked Good");
        AddMaterial("sourdough bread", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Baked Good");
        AddMaterial("rye bread", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Baked Good");
        AddMaterial("dough", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Baked Good");

        AddMaterial("fruit jelly", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food");
        AddMaterial("milk cream", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food", "Animal Product");
        AddMaterial("cheese", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food",
            "Animal Product");
        AddMaterial("honey", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food",
            "Animal Product", "Apiary Product");
        AddMaterial("honeycomb", MaterialBehaviourType.Food, 0.95, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food",
            "Animal Product", "Apiary Product");
        AddMaterial("yoghurt", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null, "Food",
            "Animal Product");

        #endregion

        #region Crops

        AddMaterial("wheat", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("sorghum", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("chickpea", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("barley", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("rye", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("rice", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("bitter vetch", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("soybean", MaterialBehaviourType.Food, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Food Crop");
        AddMaterial("flax", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Fiber Crop");

        #endregion

        #region Animal Skins and Leather

        AddMaterial("animal skin", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("cow hide", MaterialBehaviourType.Skin, 1.4, true, 20000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("deer hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("bear hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Thick Animal Skin");
        AddMaterial("dog hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("cat hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("fox hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("pig hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("wolf hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("snake hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("alligator hide", MaterialBehaviourType.Skin, 1.4, true, 15000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Thick Animal Skin");
        AddMaterial("crocodile hide", MaterialBehaviourType.Skin, 1.4, true, 20000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Thick Animal Skin");
        AddMaterial("lion hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Thick Animal Skin");
        AddMaterial("tiger hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Thick Animal Skin");
        AddMaterial("elephant hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Thick Animal Skin");
        AddMaterial("rhino hide", MaterialBehaviourType.Skin, 1.4, true, 17500, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Thick Animal Skin");
        AddMaterial("rabbit hide", MaterialBehaviourType.Skin, 1.4, true, 12000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Animal Skin");
        AddMaterial("small mammal hide", MaterialBehaviourType.Skin, 1.4, true, 12000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Animal Skin");

        AddMaterial("leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Leather", "Simplified");
        AddMaterial("cow leather", MaterialBehaviourType.Leather, 1.4, true, 32000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Leather");
        AddMaterial("deer leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("bear leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("dog leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Leather");
        AddMaterial("cat leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Leather");
        AddMaterial("fox leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Leather");
        AddMaterial("pig leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500, null,
            "Leather");
        AddMaterial("wolf leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("snake leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("alligator leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("crocodile leather", MaterialBehaviourType.Leather, 1.4, true, 32000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("lion leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("tiger leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("elephant leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("rhino leather", MaterialBehaviourType.Leather, 1.4, true, 28000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("rabbit leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001, 500,
            null, "Leather");
        AddMaterial("small mammal leather", MaterialBehaviourType.Leather, 1.4, true, 25000, 10000, 0.2, 0.14, 0.0001,
            500, null, "Leather");

        #endregion

        #region Hair

        AddMaterial("fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null, "Hair");
        AddMaterial("hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null, "Hair");
        AddMaterial("human hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("dog hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("cat hair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("bear fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("fox fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("lion fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("tiger fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("rabbit fur", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("ermine", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null,
            "Hair");
        AddMaterial("vair", MaterialBehaviourType.Hair, 1.4, true, 15000, 10000, 0.1, 0.14, 0.0001, 500, null, "Hair");

        #endregion

        #region Cloth

        AddMaterial("broadcloth", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("burlap", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("canvas", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("cotton", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("denim", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("felt", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Fiber Fabric");
        AddMaterial("hemp", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("hessian", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("jute", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("linen", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("silk", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Natural Fiber Fabric");
        AddMaterial("tweed", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Fiber Fabric");
        AddMaterial("wool", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Fiber Fabric");
        AddMaterial("cashmere", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Fiber Fabric");
        AddMaterial("mohair", MaterialBehaviourType.Fabric, 1.5, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Fiber Fabric");

        #endregion

        #region Ceramic

        AddMaterial("bone china", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
            null, "Ceramic");
        AddMaterial("brick", MaterialBehaviourType.Ceramic, 1.9, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
            "Ceramic");
        AddMaterial("concrete", MaterialBehaviourType.Ceramic, 2.4, false, 40000, 250000, 0.0, 0.002, 0.0001, 500, null,
            "Ceramic");
        AddMaterial("earthenware", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
            null, "Ceramic");
        AddMaterial("fiberglass", MaterialBehaviourType.Ceramic, 2.5, false, 100000, 200000, 0.0, 1.04, 0.0001, 800,
            null, "Ceramic", "Glass");
        AddMaterialAliases("fiberglass", "fibreglass", "glass fibre");
        AddMaterial("fired clay", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
            null, "Ceramic");
        AddMaterial("silicate glass", MaterialBehaviourType.Ceramic, 2.5, false, 33000, 90000, 0.0, 1.0, 0.0001, 840, null,
            "Ceramic", "Glass");
        AddMaterial("soda-lime glass", MaterialBehaviourType.Ceramic, 2.5, false, 33000, 90000, 0.0, 1.0, 0.0001, 840, null,
            "Ceramic", "Glass");
        AddMaterial("borosilicate glass", MaterialBehaviourType.Ceramic, 2.23, false, 40000, 105000, 0.0, 1.2, 0.0001, 830, null,
            "Ceramic", "Glass");
        AddMaterial("lead glass", MaterialBehaviourType.Ceramic, 3.1, false, 30000, 85000, 0.0, 0.8, 0.0001, 500, null,
            "Ceramic", "Glass");
        AddMaterial("tempered glass", MaterialBehaviourType.Ceramic, 2.5, false, 55000, 180000, 0.0, 1.0, 0.0001, 840, null,
            "Ceramic", "Glass");
        AddMaterial("reinforced concrete", MaterialBehaviourType.Ceramic, 2.9, false, 80000, 350000, 0.0, 0.002, 0.0001,
            500, null, "Ceramic");
        AddMaterial("plaster", MaterialBehaviourType.Ceramic, 0.35, false, 40000, 100000, 0.0, 0.002, 0.0001, 500, null,
            "Ceramic");
        AddMaterial("porcelain", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
            null, "Ceramic");
        AddMaterial("stoneware", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
            null, "Ceramic");
        AddMaterial("terracotta", MaterialBehaviourType.Ceramic, 0.7, false, 40000, 100000, 0.0, 0.002, 0.0001, 500,
            null, "Ceramic");

        #endregion

        #region Plastics

        AddMaterial("ABS plastic", MaterialBehaviourType.Plastic, 1.04, false, 40000, 90000, 0, 0.18, 0.0001, 1300, null,
            "Plastic");
        AddMaterial("acrylic", MaterialBehaviourType.Plastic, 1.18, false, 45000, 95000, 0, 0.19, 0.0001, 1470, null,
            "Plastic");
        AddMaterial("acrylic fiber", MaterialBehaviourType.Fabric, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500,
            null, "Plastic", "Synthetic Fiber Fabric");
        AddMaterial("glass-reinforced plastic", MaterialBehaviourType.Plastic, 1.85, false, 70000, 150000, 0, 0.30,
            0.0001, 900, null, "Plastic");
        AddMaterialAliases("glass-reinforced plastic", "grp", "frp");
        AddMaterial("low-density polyethylene", MaterialBehaviourType.Plastic, 0.92, false, 10000, 22000, 0, 0.33,
            0.0001, 1900, null, "Plastic");
        AddMaterialAliases("low-density polyethylene", "ldpe");
        AddMaterial("high-density polyethylene", MaterialBehaviourType.Plastic, 0.95, false, 26000, 60000, 0, 0.48,
            0.0001, 1900, null, "Plastic");
        AddMaterialAliases("high-density polyethylene", "hdpe");
        AddMaterial("melamine formaldehyde", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001,
            500, null, "Plastic");
        AddMaterial("microfiber", MaterialBehaviourType.Fabric, 1.35, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
            "Plastic", "Synthetic Fiber Fabric");
        AddMaterial("nylon", MaterialBehaviourType.Fabric, 1.3, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
            "Plastic", "Synthetic Fiber Fabric");
        AddMaterial("polycarbonate", MaterialBehaviourType.Plastic, 1.2, false, 65000, 140000, 0, 0.20, 0.0001, 1200, null,
            "Plastic");
        AddMaterial("polyethylene terephthalate", MaterialBehaviourType.Plastic, 1.38, false, 55000, 120000, 0, 0.24,
            0.0001, 1200, null, "Plastic");
        AddMaterialAliases("polyethylene terephthalate", "pet", "pete");
        AddMaterial("polyester", MaterialBehaviourType.Fabric, 1.3, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
            "Plastic", "Synthetic Fiber Fabric");
        AddMaterial("poly-cotton blend", MaterialBehaviourType.Fabric, 1.3, false, 10000, 25000, 0, 0.14, 0.0001, 500,
            null, "Plastic", "Blended Fiber Fabric");
        AddMaterial("polypropylene", MaterialBehaviourType.Plastic, 0.90, false, 25000, 55000, 0, 0.22, 0.0001, 1900,
            null, "Plastic");
        AddMaterialAliases("polypropylene", "pp");
        AddMaterial("polystyrene", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500,
            null, "Plastic");
        AddMaterial("polyurethane", MaterialBehaviourType.Plastic, 0.975, false, 10000, 25000, 0, 0.14, 0.0001, 500,
            null, "Plastic");
        AddMaterial("polyvinyl chloride", MaterialBehaviourType.Plastic, 1.38, false, 18000, 40000, 0, 0.19, 0.0001,
            900, null, "Plastic");
        AddMaterialAliases("polyvinyl chloride", "pvc");
        AddMaterial("spandex", MaterialBehaviourType.Fabric, 1.15, false, 10000, 25000, 0, 0.14, 0.0001, 500, null,
            "Plastic", "Synthetic Fiber Fabric");
        AddMaterial("synthetic rubber", MaterialBehaviourType.Elastomer, 0.975, false, 10000, 25000, 0, 0.14, 0.0001,
            500, null, "Plastic");

        #endregion

        #region Miscellaneous

        AddMaterial("cardboard", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Paper Product");
        AddMaterial("paper", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Writing Product");
        AddMaterial("papyrus", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Writing Product");
        AddMaterial("parchment", MaterialBehaviourType.Fabric, 1.52, true, 2000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Writing Product", "Animal Product");
        AddMaterial("shell", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("chitin", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("keratin", MaterialBehaviourType.Shell, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("scale", MaterialBehaviourType.Scale, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("tooth", MaterialBehaviourType.Tooth, 1.0, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("tusk", MaterialBehaviourType.Tooth, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("antler", MaterialBehaviourType.Horn, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("horn", MaterialBehaviourType.Horn, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("ivory", MaterialBehaviourType.Tooth, 1.2, true, 20000, 50000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("fin", MaterialBehaviourType.Flesh, 1.0, true, 10000, 25000, 2.0, 0.14, 0.0001, 500, null,
            "Animal Product");
        AddMaterial("grass", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("leaf", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("moss", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("seaweed", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("slime", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("vine", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("compost", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("mulch", MaterialBehaviourType.Plant, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("feces", MaterialBehaviourType.Feces, 1.0, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials", "Animal Product");
        AddMaterial("feather", MaterialBehaviourType.Feather, 1.283, true, 1000, 1000, 0.05, 0.14, 0.0001, 500, null,
            "Natural Materials", "Animal Product");
        AddMaterial("soap", MaterialBehaviourType.Soap, 0.2, true, 1000, 1000, 0.05, 0.14, 0.0001, 500, null,
            "Natural Materials", "Animal Product");
        AddMaterial("beeswax", MaterialBehaviourType.Wax, 0.2, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials", "Animal Product", "Apiary Product");
        AddMaterial("guano", MaterialBehaviourType.Wax, 0.2, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials", "Animal Product");
        AddMaterial("paraffin wax", MaterialBehaviourType.Wax, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("cream", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("gel", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("jelly", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("grease", MaterialBehaviourType.Grease, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("lard", MaterialBehaviourType.Grease, 0.2, true, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("paste", MaterialBehaviourType.Cream, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("glue", MaterialBehaviourType.Paste, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("dried adhesive", MaterialBehaviourType.Paste, 0.2, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials");
        AddMaterial("gunpowder", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0.0, 0.14, 0.0001, 500, null,
            "Manufactured Materials", "Gunpowder");

        AddMaterial("flame", MaterialBehaviourType.Mana, 1.0, false, 1, 1, 0.0, 0, 0, 0, null, "Elemental Materials");
        AddMaterial("mana", MaterialBehaviourType.Mana, 1.0, false, 1, 1, 0.0, 0, 0, 0, null, "Elemental Materials");
        AddMaterial("spirit energy", MaterialBehaviourType.Spirit, 1.0, false, 1, 1, 0.0, 0, 0, 0, null,
            "Elemental Materials");
        AddMaterial("natural rubber", MaterialBehaviourType.Elastomer, 0.975, true, 20000, 35000, 0, 0.14, 0.0001, 500,
            null, "Elastomer");
        AddMaterial("vulcanized rubber", MaterialBehaviourType.Elastomer, 0.975, true, 20000, 35000, 0, 0.14, 0.0001,
            500, null, "Elastomer");
        AddMaterial("calcium hydroxide", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500,
            null, "Natural Materials");
        AddMaterial("calcium oxide", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("lye", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("portland cement", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500,
            null, "Natural Materials");
        AddMaterial("pozzolanic ash", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500,
            null, "Natural Materials");
        AddMaterial("roman cement", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("slaked lime", MaterialBehaviourType.Powder, 0.975, false, 1000, 1000, 0, 0.14, 0.0001, 500, null,
            "Natural Materials");
        AddMaterial("wood ash", MaterialBehaviourType.Powder, 0.975, true, 1000, 1000, 0, 0.14, 0.0001, 500, null,
            "Natural Materials");

        #endregion

        context.SaveChanges();

        Dictionary<string, Liquid> liquids = new(StringComparer.InvariantCultureIgnoreCase);
        Dictionary<Liquid, string> liquidCountsAs = new();
        Dictionary<Liquid, string> liquidSolvents = new();

        void AddLiquid(string name, string description, string longDescription, string taste, string? vagueTaste,
            string smell, string? vagueSmell, double tasteIntensity, double smellIntensity, double alcohol,
            double food, double unusedNutritionValue, double water, double satiated, double viscosity, double density,
            bool organic, string displayColour, string dampDesc, string wetDesc, string drenchedDesc,
            string dampSdesc, string wetSdesc, string drenchedSdesc, double solventVolumeRatio, string? dried,
            double residueVolumePercentage, LiquidInjectionConsequence injectionConsequence,
            (string Liquid, ItemQuality Quality)? countsAs, double thermalConductivity = 0.609,
            double electricalConductivity = 0.005, double specificHeatCapacity = 4181, string? solvent = null,
            params string[] materialTags)
        {
            Liquid liquid = new()
            {
                Name = name,
                AlcoholLitresPerLitre = alcohol,
                BoilingPoint = 100,
                DampDescription = dampDesc,
                DampShortDescription = dampSdesc,
                Density = density,
                TasteIntensity = tasteIntensity,
                SmellIntensity = smellIntensity,
                TasteText = taste,
                VagueTasteText = vagueTaste ?? taste,
                SmellText = smell,
                VagueSmellText = vagueSmell ?? smell,
                Viscosity = viscosity,
                DrinkSatiatedHoursPerLitre = satiated,
                FoodSatiatedHoursPerLitre = food,
                SolventVolumeRatio = solventVolumeRatio,
                DrenchedDescription = drenchedDesc,
                DrenchedShortDescription = drenchedSdesc,
                WetDescription = wetDesc,
                WetShortDescription = wetSdesc,
                InjectionConsequence = (int)injectionConsequence,
                WaterLitresPerLitre = water,
                Description = description,
                LongDescription = longDescription,
                ThermalConductivity = thermalConductivity,
                SpecificHeatCapacity = specificHeatCapacity,
                ElectricalConductivity = electricalConductivity,
                Organic = organic,
                DisplayColour = displayColour,
                ResidueVolumePercentage = residueVolumePercentage
            };
            context.Liquids.Add(liquid);
            liquids[name] = liquid;
            if (dried != null)
            {
                liquid.DriedResidueId = materials[dried].Id;
            }

            if (countsAs.HasValue && !string.IsNullOrEmpty(countsAs.Value.Liquid))
            {
                liquid.CountAsQuality = (int)countsAs.Value.Quality;
                liquidCountsAs[liquid] = countsAs.Value.Liquid;
            }
            else
            {
                liquid.CountAsQuality = 0;
            }

            if (solvent != null)
            {
                liquidSolvents[liquid] = solvent;
            }

            foreach (string tag in materialTags)
            {
                liquid.LiquidsTags.Add(new LiquidsTags { Liquid = liquid, Tag = tags[tag] });
            }
        }

        #region Water

        AddLiquid("water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
            "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
            false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
            0.05, LiquidInjectionConsequence.Harmful, null, materialTags: ["Water", "Beverage"]);
        AddLiquid("rain water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
            "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
            false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
            0.05, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Excellent), materialTags: ["Water", "Beverage"]);
        AddLiquid("tap water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
            "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
            false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
            0.05, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Excellent), materialTags: ["Water", "Beverage"]);
        AddLiquid("spring water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
            "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
            false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
            0.05, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Excellent), materialTags: ["Water", "Beverage"]);
        AddLiquid("pool water", "a clear liquid", "a clear, translucent liquid", "It has no real taste",
            "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0, 0, 1.0, 12.0, 1.0, 1.0,
            false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null,
            0.05, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Excellent), materialTags: ["Water"]);
        AddLiquid("river water", "a clear liquid", "a clear, translucent liquid with some small impurities",
            "It has no real taste", "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0,
            0, 1.0, 12.0, 1.0, 1.0, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
            "(soaked)", 1.0, null, 0.05, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Good), materialTags: ["Water"]);
        AddLiquid("lake water", "a clear liquid", "a clear, translucent liquid with some small impurities",
            "It has no real taste", "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0,
            0, 1.0, 12.0, 1.0, 1.0, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
            "(soaked)", 1.0, null, 0.05, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Good), materialTags: ["Water"]);
        AddLiquid("swamp water", "a clear liquid", "a clear, translucent liquid with some small impurities",
            "It has no real taste", "It has no real taste", "It has no real smell", "It has no real smell", 1, 1, 0, 0,
            0, 1.0, 12.0, 1.0, 1.0, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
            "(soaked)", 1.0, null, 0.05, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Standard), materialTags: ["Water"]);
        AddLiquid("salt water", "a clear liquid", "a clear, translucent liquid", "It has a strong salty taste",
            "It has a strong salty taste", "It smells strongly of salt", "It smells of salt", 1000, 100, 0, 0, 0, -0.5,
            -6.0, 1.0, 1.029, false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)",
            "(soaked)", 1.0, "salt", 0.029, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Poor), materialTags: ["Water"]);
        AddLiquid("brackish water", "a clear liquid", "a clear, translucent liquid", "It has a salty taste",
            "It has a salty taste", "It smells of salt", "It smells of salt", 500, 50, 0, 0, 0, -0.25, -3.0, 1.0, 1.015,
            false, "blue", "It is damp", "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, "salt",
            0.015, LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Standard), materialTags: ["Water"]);
        AddLiquid("saline solution", "a clear liquid", "a clear, translucent liquid",
            "It has a very, very mild salty taste", "It has no real taste", "It has no real smell",
            "It has no real smell", 100, 1, 0, 0, 0, 0.9, 9.0, 1.0, 1.009, false, "blue", "It is damp", "It is wet",
            "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, "salt", 0.009,
            LiquidInjectionConsequence.Hydrating,
            ("water", ItemQuality.Good), materialTags: ["Water"]);
        AddLiquid("dextrose solution", "a clear liquid", "a clear, translucent liquid",
            "It has a very, very mild sweet and salty taste", "It has no real taste", "It has no real smell",
            "It has no real smell", 100, 1, 0, 5.0, 200, 0.9, 9.0, 1.0, 1.009, false, "blue", "It is damp", "It is wet",
            "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null, 0.009, LiquidInjectionConsequence.Hydrating,
            ("water", ItemQuality.Good), materialTags: ["Water"]);
        AddLiquid("detergent", "a clear, soapy liquid", "a clear, soapy liquid",
            "It has a strong soapy taste", "It has a strong soapy taste", "It smells strongly of soap",
            "It smells of soap", 1000, 100, 0, 0, 0, -0.5, -6.0, 1.0, 1.029, false, "bold blue", "It is damp",
            "It is wet", "It is soaking wet", "(soap-damp)", "(soap-wet)", "(soap-soaked)", 1.0, null, 0.029,
            LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Legendary), materialTags: ["Water", "Detergent"]);
        AddLiquid("soapy water", "a clear liquid with soap suds", "a clear, translucent liquid with soap suds",
            "It has a strong soapy taste", "It has a strong soapy taste", "It smells strongly of soap",
            "It smells of soap", 1000, 100, 0, 0, 0, -0.5, -6.0, 1.0, 1.029, false, "bold blue", "It is damp",
            "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 1.0, null, 0.029,
            LiquidInjectionConsequence.Harmful,
            ("water", ItemQuality.Legendary), materialTags: ["Water", "Detergent"]);

        #endregion

        context.SaveChanges();
        var defaultWater = liquids["water"];

        #region Biofluids

        Material driedBlood = new()
        {
            Name = "dried blood",
            MaterialDescription = "dried blood",
            Density = 1520,
            Organic = true,
            Type = 0,
            BehaviourType = 19,
            ThermalConductivity = 0.2,
            ElectricalConductivity = 0.0001,
            SpecificHeatCapacity = 420,
            IgnitionPoint = 555.3722,
            HeatDamagePoint = 412.0389,
            ImpactFracture = 1000,
            ImpactYield = 1000,
            ImpactStrainAtYield = 2,
            ShearFracture = 1000,
            ShearYield = 1000,
            ShearStrainAtYield = 2,
            YoungsModulus = 0.1,
            SolventId = defaultWater.Id,
            SolventVolumeRatio = 4,
            ResidueDesc = "It is covered in {0}dried blood",
            ResidueColour = "red",
            Absorbency = 0
        };
        context.Materials.Add(driedBlood);
        Liquid blood = new()
        {
            Name = "blood",
            Description = "blood",
            LongDescription = "a virtually opaque dark red fluid",
            TasteText = "It has a sharply metallic, umami taste",
            VagueTasteText = "It has a metallic taste",
            SmellText = "It has a metallic, coppery smell",
            VagueSmellText = "It has a faintly metallic smell",
            TasteIntensity = 200,
            SmellIntensity = 10,
            AlcoholLitresPerLitre = 0,
            WaterLitresPerLitre = 0.8,
            DrinkSatiatedHoursPerLitre = 6,
            FoodSatiatedHoursPerLitre = 4,
            Viscosity = 1,
            Density = 1,
            Organic = true,
            ThermalConductivity = 0.609,
            ElectricalConductivity = 0.005,
            SpecificHeatCapacity = 4181,
            FreezingPoint = -20,
            BoilingPoint = 100,
            DisplayColour = "bold red",
            DampDescription = "It is damp with blood",
            WetDescription = "It is wet with blood",
            DrenchedDescription = "It is drenched with blood",
            DampShortDescription = "(blood damp)",
            WetShortDescription = "(bloody)",
            DrenchedShortDescription = "(blood drenched)",
            SolventId = defaultWater.Id,
            SolventVolumeRatio = 5,
            InjectionConsequence = (int)LiquidInjectionConsequence.BloodReplacement,
            ResidueVolumePercentage = 0.05,
            DriedResidue = driedBlood
        };
        context.Liquids.Add(blood);

        Material driedSweat = new()
        {
            Name = "dried Sweat",
            MaterialDescription = "dried sweat",
            Density = 1520,
            Organic = true,
            Type = 0,
            BehaviourType = 19,
            ThermalConductivity = 0.2,
            ElectricalConductivity = 0.0001,
            SpecificHeatCapacity = 420,
            IgnitionPoint = 555.3722,
            HeatDamagePoint = 412.0389,
            ImpactFracture = 1000,
            ImpactYield = 1000,
            ImpactStrainAtYield = 2,
            ShearFracture = 1000,
            ShearYield = 1000,
            ShearStrainAtYield = 2,
            YoungsModulus = 0.1,
            SolventId = defaultWater.Id,
            SolventVolumeRatio = 3,
            ResidueDesc = "It is covered in {0}dried sweat",
            ResidueColour = "yellow",
            Absorbency = 0
        };
        context.Materials.Add(driedSweat);
        Liquid sweat = new()
        {
            Name = "sweat",
            Description = "sweat",
            LongDescription = "a relatively clear, translucent fluid that smells strongly of body odor",
            TasteText = "It tastes like a pungent, salty lick of someone's underarms",
            VagueTasteText = "It tastes very unpleasant, like underarm stench",
            SmellText = "It has the sharp, pungent smell of body odor",
            VagueSmellText = "It has the sharp, pungent smell of body odor",
            TasteIntensity = 200,
            SmellIntensity = 200,
            AlcoholLitresPerLitre = 0,
            WaterLitresPerLitre = 0.95,
            DrinkSatiatedHoursPerLitre = 5,
            FoodSatiatedHoursPerLitre = 0,
            Viscosity = 1,
            Density = 1,
            Organic = true,
            ThermalConductivity = 0.609,
            ElectricalConductivity = 0.005,
            SpecificHeatCapacity = 4181,
            FreezingPoint = -20,
            BoilingPoint = 100,
            DisplayColour = "yellow",
            DampDescription = "It is damp with sweat",
            WetDescription = "It is wet and smelly with sweat",
            DrenchedDescription = "It is soaking wet and smelly with sweat",
            DampShortDescription = "(sweat-damp)",
            WetShortDescription = "(sweaty)",
            DrenchedShortDescription = "(sweat-drenched)",
            SolventId = defaultWater.Id,
            SolventVolumeRatio = 5,
            InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
            ResidueVolumePercentage = 0.05,
            DriedResidue = driedSweat
        };
        context.Liquids.Add(sweat);

        Material driedVomit = new()
        {
            Name = "dried vomit",
            MaterialDescription = "dried vomit",
            Density = 1520,
            Organic = true,
            Type = 0,
            BehaviourType = 19,
            ThermalConductivity = 0.2,
            ElectricalConductivity = 0.0001,
            SpecificHeatCapacity = 420,
            IgnitionPoint = 555.3722,
            HeatDamagePoint = 412.0389,
            ImpactFracture = 1000,
            ImpactYield = 1000,
            ImpactStrainAtYield = 2,
            ShearFracture = 1000,
            ShearYield = 1000,
            ShearStrainAtYield = 2,
            YoungsModulus = 0.1,
            SolventId = defaultWater.Id,
            SolventVolumeRatio = 3,
            ResidueDesc = "It is covered in {0}dried vomit",
            ResidueColour = "yellow",
            Absorbency = 0
        };
        context.Materials.Add(driedVomit);
        Liquid vomit = new()
        {
            Name = "vomit",
            Description = "vomit",
            LongDescription = "a stinking mixture of digestive liquids and partially digested food",
            TasteText = "It just tastes like vomit...I'm sure you don't need a description",
            VagueTasteText = "It just tastes like vomit...I'm sure you don't need a description",
            SmellText = "It smells awful, a naturally repugnant stench associated with sickness",
            VagueSmellText = "It smells awful, a naturally repugnant stench associated with sickness",
            TasteIntensity = 500,
            SmellIntensity = 500,
            AlcoholLitresPerLitre = 0,
            WaterLitresPerLitre = 0.6,
            DrinkSatiatedHoursPerLitre = 2,
            FoodSatiatedHoursPerLitre = 0,
            Viscosity = 5,
            Density = 1.3,
            Organic = true,
            ThermalConductivity = 0.609,
            ElectricalConductivity = 0.005,
            SpecificHeatCapacity = 4181,
            FreezingPoint = -20,
            BoilingPoint = 100,
            DisplayColour = "yellow",
            DampDescription = "It is stained with vomit",
            WetDescription = "It is wet and covered with vomit",
            DrenchedDescription = "It is absolutely drenched with wet vomit",
            DampShortDescription = "(vomit-stained)",
            WetShortDescription = "(vomit-covered)",
            DrenchedShortDescription = "(vomit-drenched)",
            SolventId = defaultWater.Id,
            SolventVolumeRatio = 10,
            InjectionConsequence = (int)LiquidInjectionConsequence.Deadly,
            ResidueVolumePercentage = 0.05,
            DriedResidue = driedVomit
        };
        context.Liquids.Add(vomit);

        #endregion

        #region Drinks

        AddLiquid("lager", "an amber liquid", "a fairly translucent dark amber fluid",
            "It has a smooth, crisp and moderately bitter taste", "It has the bitter taste of beer",
            "It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.06, 3.7, 340, 0.92, 12.0, 1.0, 1.0,
            true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
            "(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("light lager", "an amber liquid", "a fairly translucent dark amber fluid",
            "It has a smooth and moderately bitter taste", "It has the bitter taste of beer",
            "It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.035, 2.4, 205, 0.95, 12.0, 1.0, 1.0,
            true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
            "(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("pale ale", "an amber liquid",
            "a moderately translucent dark amber fluid with a tendency to form frothy light amber foam",
            "It has a strong, rich bitter taste", "It has the bitter taste of beer", "It has a bitter, alcoholic smell",
            "It has a bitter smell", 150, 40, 0.08, 3.7, 340, 0.92, 12.0, 1.0, 1.0, true, "yellow",
            "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)", "(wet)", "(soaked)",
            5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("amber ale", "an amber liquid",
            "a moderately translucent dark amber fluid with a tendency to form frothy light amber foam",
            "It has a smooth, crisp and moderately bitter taste", "It has the bitter taste of beer",
            "It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.08, 3.7, 340, 0.92, 12.0, 1.0, 1.0,
            true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
            "(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("dark ale", "an amber liquid", "a fairly translucent dark amber fluid",
            "It has a rich and very bitter taste", "It has the bitter taste of beer",
            "It has a bitter, alcoholic smell", "It has a bitter smell", 150, 40, 0.08, 5.0, 500, 0.90, 10.0, 1.0, 1.0,
            true, "yellow", "It is damp with beer", "It is wet with beer", "It is soaking wet with beer", "(damp)",
            "(wet)", "(soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Alcoholic"]);

        AddLiquid("red wine", "an dark burgundy liquid", "a transparent dark burgundy fluid",
            "It has a dry and sharp taste, with a distinct after note of tannin", "It has the sharp taste of wine",
            "It has a sharp, alcoholic smell", "It has a sharp smell", 150, 40, 0.14, 2.0, 200, 0.8, 8.0, 1.0, 1.0,
            true, "magenta", "It is damp with wine", "It is wet with wine", "It is soaking wet with wine",
            "(wine-damp)", "(wine-wet)", "(wine-soaked)", 7.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("watered red wine", "a burgundy liquid", "a transparent burgundy fluid",
            "It has a faint sharp taste, with a slight after note of tannin", "It has the taste of watered-down wine",
            "It has a slightly alcoholic smell", "It has a sharp smell", 150, 40, 0.035, 1.0, 100, 0.97, 12.0, 1.0, 1.0,
            true, "magenta", "It is damp with wine", "It is wet with wine", "It is soaking wet with wine",
            "(wine-damp)", "(wine-wet)", "(wine-soaked)", 3.5, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("white wine", "a clear amber liquid", "a transparent amber fluid", "It has a dry and floral taste",
            "It has the dry, floral taste of white wine", "It has a floral, alcoholic smell",
            "It has a slightly alcoholic smell", 150, 40, 0.14, 2.0, 200, 0.8, 8.0, 1.0, 1.0, true, "yellow",
            "It is damp with wine", "It is wet with wine", "It is soaking wet with wine", "(wine-damp)", "(wine-wet)",
            "(wine-soaked)", 7.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Alcoholic"]);

        AddLiquid("vodka", "a clear liquid", "a crystal clear liquid smelling strongly of alcohol",
            "It has little taste beyond that of the very strong alcohol",
            "It has little taste beyond that of the very strong alcohol", "It smells strongly of alcohol",
            "It smells strongly of alcohol", 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("bourbon whiskey", "a translucent brown liquid",
            "a translucent, dark brown liquid smelling strongly of alcohol",
            "It tastes first and foremost of alcohol, but it is supplemented by an oakey, sweet note that contrasts with it",
            null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("scotch whiskey", "a translucent brown liquid",
            "a translucent, dark brown liquid smelling strongly of alcohol",
            "It tastes first and foremost of alcohol, but it is supplemented by an oakey, dry note that contrasts with it",
            null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("whiskey", "a translucent brown liquid",
            "a translucent, dark brown liquid smelling strongly of alcohol",
            "It tastes first and foremost of alcohol, but it is supplemented by an oakey, dry note that contrasts with it",
            null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("rum", "a translucent brown liquid", "a translucent, dark brown liquid smelling strongly of alcohol",
            "It tastes first and foremost of alcohol, but it is supplemented by a very sweet, sugary aftertaste", null,
            "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("gin", "a clear liquid", "a clear liquid smelling strongly of alcohol",
            "It tastes first and foremost of alcohol, followed by the unmistakable and unique taste of juniper berry",
            null, "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);
        AddLiquid("tequila", "a transparent brown liquid", "a transparent brown liquid smelling strongly of alcohol",
            "It tastes first and foremost of alcohol, but it is supplemented by the distinctive taste of agave", null,
            "It smells strongly of alcohol", null, 250, 150, 0.4, 5.4, 390, 0.5, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null,
            solvent: "water", materialTags: ["Alcoholic"]);

        AddLiquid("ethanol", "a clear liquid", "a crystal clear liquid smelling strongly of alcohol",
            "It has little taste beyond that of the very strong alcohol",
            "It has little taste beyond that of the very strong alcohol", "It smells strongly of alcohol",
            "It smells strongly of alcohol", 500, 500, 1.0, 5.4, 390, -0.1, -3.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Deadly, ("fuel", ItemQuality.VeryGood),
            solvent: "water", materialTags: ["Alcoholic", "Disgusting", "Fuel"]);
        AddLiquid("methanol", "a clear liquid", "a crystal clear liquid smelling strongly of alcohol",
            "It has little taste beyond that of the very strong alcohol",
            "It has little taste beyond that of the very strong alcohol", "It smells strongly of alcohol",
            "It smells strongly of alcohol", 500, 500, 1.0, 5.4, 390, -0.1, -3.0, 1.0, 1.0, true, "yellow",
            "It is damp with alcohol", "It is wet with alcohol", "It is soaking wet with alcohol", "(damp)",
            "(liquor-wet)", "(liquor-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Deadly, ("fuel", ItemQuality.VeryGood),
            solvent: "water", materialTags: ["Alcoholic", "Disgusting", "Fuel"]);

        AddLiquid("orange juice", "orange juice", "a translucent orange liquid with fruit pulp",
            "It tastes like orange juice", "It tastes like orange juice", "It smells of oranges",
            "It smells of oranges", 200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "orange", "It is damp with juice",
            "It is wet with juice", "It is soaking wet with juice", "(damp)", "(wet)", "(juice-soaked)", 5.0, null,
            0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("apple juice", "apple juice", "a transparent brown liquid with fruit pulp",
            "It tastes like apple juice", "It tastes like apple juice", "It smells of apples", "It smells of apples",
            200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with juice", "It is wet with juice",
            "It is soaking wet with juice", "(damp)", "(wet)", "(juice-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("pineapple juice", "pineapple juice", "a translucent yellow liquid with fruit pulp",
            "It tastes like pineapple juice", "It tastes like pineapple juice", "It smells of pineapples",
            "It smells of pineapples", 200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "yellow",
            "It is damp with juice", "It is wet with juice", "It is soaking wet with juice", "(damp)", "(wet)",
            "(juice-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("pomegranate juice", "pomegranate juice", "a translucent purple liquid with fruit pulp",
            "It tastes like pomegranate juice", "It tastes like pomegranate juice", "It smells of pomegranates",
            "It smells of pomegranates", 200, 100, 0, 5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "magenta",
            "It is damp with juice", "It is wet with juice", "It is soaking wet with juice", "(damp)", "(wet)",
            "(juice-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);

        AddLiquid("white wine vinegar", "a clear liquid", "a clear, translucent liquid", "It tastes like vinegar", null,
            "It smells of vinegar", null, 200, 100, 0, 5.4, 390, 0.5, 0.0, 1.0, 1.0, true, "magenta", "It is damp",
            "It is wet", "It is soaking wet", "(damp)", "(wet)", "(soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water");
        AddLiquid("balsamic vineger", "a dark brown liquid", "a translucent dark brown liquid",
            "It tastes like balsamic vinegar", null, "It smells of pomegranates", null, 200, 100, 0, 5.4, 390, 0.5, 0,
            1.0, 1.0, true, "magenta", "It is damp with vinegar", "It is wet with vinegar",
            "It is soaking wet with vinegar", "(damp)", "(wet)", "(vinegar-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water");

        AddLiquid("olive oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like olive oil", null,
            "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow", "It is damp with oil",
            "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)", "(oil-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
        AddLiquid("vegetable oil", "a transparent yellow oil", "a transparent yellow oil",
            "It tastes like vegetable oil", null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0,
            true, "yellow", "It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)",
            "(oily)", "(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard),
            solvent: "soapy water");
        AddLiquid("canola oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like canola oil",
            null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow",
            "It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)",
            "(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
        AddLiquid("peanut oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like peanut oil",
            null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow",
            "It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)",
            "(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
        AddLiquid("sesame oil", "a transparent yellow oil", "a transparent yellow oil", "It tastes like sesame oil",
            null, "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow",
            "It is damp with oil", "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)",
            "(oil-soaked)", 5.0, null, 0.05, LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Substandard), solvent: "soapy water");
        AddLiquid(name: "whale oil", description: "a transparent dark yellow oil", longDescription: "a transparent dark yellow oil", taste: "It tastes like whale oil", vagueTaste: null,
            smell: "It smells of oil", vagueSmell: null, tasteIntensity: 200, smellIntensity: 100, alcohol: 0, food: 5.4, unusedNutritionValue: 0.0, water: 0.5, satiated: 0, viscosity: 2.0, density: 1.0, organic: true, displayColour: "yellow", dampDesc: "It is damp with oil",
            wetDesc: "It is covered with oil", drenchedDesc: "It is soaking with oil", dampSdesc: "(oil-damp)", wetSdesc: "(oily)", drenchedSdesc: "(oil-soaked)", solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.05,
            injectionConsequence: LiquidInjectionConsequence.Harmful, countsAs: ("fuel", ItemQuality.Heroic), solvent: "soapy water");
        AddLiquid("sperm oil", "a transparent dark yellow oil", "a transparent dark yellow oil", "It tastes like sperm oil", null,
            "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow", "It is damp with oil",
            "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)", "(oil-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Heroic), solvent: "soapy water");
        AddLiquid("train oil", "a transparent dark yellow oil", "a transparent dark yellow oil", "It tastes like train oil", null,
            "It smells of oil", null, 200, 100, 0, 5.4, 390, 0.5, 0, 2.0, 1.0, true, "yellow", "It is damp with oil",
            "It is covered with oil", "It is soaking with oil", "(oil-damp)", "(oily)", "(oil-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, ("fuel", ItemQuality.Excellent), solvent: "soapy water");

        AddLiquid("milk", "a creamy white liquid", "a translucent white liquid",
            "It tastes creamy and sweet, with a full bodied flavour", null, "It smells like milk", null, 200, 100, 0,
            5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "bold white", "It is damp with milk", "It is wet with milk",
            "It is soaking wet with milk", "(damp)", "(wet)", "(milk-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("goat's milk", "a creamy white liquid", "a translucent white liquid",
            "It tastes creamy and sweet, with a full bodied flavour", null, "It smells like milk", null, 200, 100, 0,
            5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "bold white", "It is damp with milk", "It is wet with milk",
            "It is soaking wet with milk", "(damp)", "(wet)", "(milk-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("sheep's milk", "a creamy white liquid", "a translucent white liquid",
            "It tastes creamy and sweet, with a full bodied flavour", null, "It smells like milk", null, 200, 100, 0,
            5.4, 390, 0.9, 9.0, 1.0, 1.0, true, "bold white", "It is damp with milk", "It is wet with milk",
            "It is soaking wet with milk", "(damp)", "(wet)", "(milk-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);

        AddLiquid("tea", "a clear brown liquid", "a transparent brown liquid",
            "It tastes bitter and aromatic, like black tea", null, "It smells like tea", null, 200, 100, 0, 1.0, 50,
            0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with tea", "It is wet with tea",
            "It is soaking wet with tea", "(damp)", "(wet)", "(tea-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("tea with milk", "a light brown liquid", "a translucent light brown liquid",
            "It tastes bitter and aromatic, like black tea mixed with milk", null, "It smells like tea", null, 100, 50,
            0, 1.0, 50, 0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with tea", "It is wet with tea",
            "It is soaking wet with tea", "(damp)", "(wet)", "(tea-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("green tea", "a clear brown liquid", "a translucent brown liquid",
            "It tastes bitter and aromatic, like green tea", null, "It smells like tea", null, 200, 100, 0, 1.0, 50,
            0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with tea", "It is wet with tea",
            "It is soaking wet with tea", "(damp)", "(wet)", "(tea-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("coffee", "a dark brown liquid", "a translucent dark brown liquid",
            "It tastes bitter and rich, like black coffee", null, "It smells like coffee", null, 200, 100, 0, 0, 0, 9.0,
            1.0, 1.0, 1.0, true, "yellow", "It is damp with coffee", "It is wet with coffee",
            "It is soaking wet with tea", "(damp)", "(wet)", "(coffee-soaked)", 5.0, null, 0.05,
            LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);
        AddLiquid("latte", "a light brown liquid", "a translucent light brown liquid",
            "It tastes slightly bitter and rich, like black coffee mixed with milk", null, "It smells like coffee",
            null, 100, 50, 0, 2, 200, 0.9, 9.0, 1.0, 1.0, true, "yellow", "It is damp with coffee",
            "It is wet with coffee", "It is soaking wet with tea", "(damp)", "(wet)", "(coffee-soaked)", 5.0, null,
            0.05, LiquidInjectionConsequence.Harmful, null, solvent: "water", materialTags: ["Beverage"]);

        #endregion

        #region Fuels
        AddLiquid(name: "fuel", description: "a clear liquid", longDescription: "a clear, translucent liquid",
            taste: "It has little taste beyond that of the very strong alcohol",
            vagueTaste: "It has little taste beyond that of the very strong alcohol",
            smell: "It smells strongly of pure alcohol",
            vagueSmell: "It smells strongly of alcohol", tasteIntensity: 1000, smellIntensity: 100, alcohol: 1.0,
            food: 5.4, unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 1.0, density: 1.029, organic: true,
            displayColour: "yellow", dampDesc: "It is damp with alcohol",
            wetDesc: "It is soaking wet with alcohol", drenchedDesc: "It is drenched with alcohol", dampSdesc: "(damp)",
            wetSdesc: "(liquor-soaked)", drenchedSdesc: "(liquor-drenched)", solventVolumeRatio: 1.0, dried: null,
            residueVolumePercentage: 0.029,
            injectionConsequence: LiquidInjectionConsequence.Harmful, countsAs: null, materialTags: ["Fuel"]);
        AddLiquid(name: "kerosene", description: "a clear liquid",
            longDescription: "a transparent fluid",
            taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has the taste of kerosene fuel; YUCK!",
            smell: "It smells like kerosene",
            vagueSmell: "It smells like kerosene", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
            unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 0.85, density: 0.9, organic: false,
            displayColour: "magenta", dampDesc: "It is damp with kerosene",
            wetDesc: "It is soaked with kerosene", drenchedDesc: "It is drenched with kerosene",
            dampSdesc: "(kerosene-damp)", wetSdesc: "(kerosene-soaked)", drenchedSdesc: "(kerosene-drenched)",
            solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
            injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.VeryGood), materialTags: ["Fuel"]);
        AddLiquid(name: "gasoline", description: "a clear liquid",
            longDescription: "a transparent, orangey-amber fluid",
            taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has the taste of gasoline fuel; YUCK!",
            smell: "It smells like gasoline",
            vagueSmell: "It smells like gasoline", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
            unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 0.85, density: 0.9, organic: false,
            displayColour: "magenta", dampDesc: "It is damp with gasoline",
            wetDesc: "It is soaked with gasoline", drenchedDesc: "It is drenched with gasoline",
            dampSdesc: "(gasoline-damp)", wetSdesc: "(gasoline-soaked)", drenchedSdesc: "(gasoline-drenched)",
            solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
            injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Heroic), materialTags: ["Fuel"]);
        AddLiquid(name: "diesel", description: "a clear liquid",
            longDescription: "a transparent, yellowy-amber fluid",
            taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has the taste of diesel fuel; YUCK!",
            smell: "It smells like diesel",
            vagueSmell: "It smells like diesel", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
            unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 0.85, density: 0.9, organic: false,
            displayColour: "magenta", dampDesc: "It is damp with diesel",
            wetDesc: "It is soaked with diesel", drenchedDesc: "It is drenched with diesel",
            dampSdesc: "(diesel-damp)", wetSdesc: "(diesel-soaked)", drenchedSdesc: "(diesel-drenched)",
            solventVolumeRatio: 5.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
            injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Heroic), materialTags: ["Fuel"]);
        AddLiquid(name: "crude oil", description: "a thick, black liquid",
            longDescription: "a thick yellow-black liquid",
            taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
            smell: "It smells like sulfur",
            vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
            unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 10, density: 0.9, organic: false,
            displayColour: "bold magenta", dampDesc: "It is damp with crude oil",
            wetDesc: "It is soaked with crude oil", drenchedDesc: "It is drenched with crude oil",
            dampSdesc: "(petroleum-damp)", wetSdesc: "(petroleum-soaked)", drenchedSdesc: "(petroleum-drenched)",
            solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
            injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Standard), materialTags: ["Fuel"]);
        AddLiquid(name: "heavy crude oil", description: "a very thick, black liquid",
            longDescription: "a thick yellow-black liquid",
            taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
            smell: "It smells like sulfur",
            vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
            unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 4, density: 0.8, organic: false,
            displayColour: "bold magenta", dampDesc: "It is damp with crude oil",
            wetDesc: "It is soaked with crude oil", drenchedDesc: "It is drenched with crude oil",
            dampSdesc: "(petroleum-damp)", wetSdesc: "(petroleum-soaked)", drenchedSdesc: "(petroleum-drenched)",
            solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
            injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Standard), materialTags: ["Fuel"]);
        AddLiquid(name: "light crude oil", description: "a sticky, black liquid",
            longDescription: "a sticky yellow-black liquid",
            taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
            smell: "It smells like sulfur",
            vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
            unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 30, density: 0.9, organic: false,
            displayColour: "bold magenta", dampDesc: "It is damp with crude oil",
            wetDesc: "It is soaked with crude oil", drenchedDesc: "It is drenched with crude oil",
            dampSdesc: "(petroleum-damp)", wetSdesc: "(petroleum-soaked)", drenchedSdesc: "(petroleum-drenched)",
            solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
            injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Standard), materialTags: ["Fuel"]);
        AddLiquid(name: "heavy fuel oil", description: "a viscous, black liquid",
            longDescription: "a thick black liquid",
            taste: "It has a sulfurous, sweet, slimy taste", vagueTaste: "It has an unimaginably bad taste, you feel in great danger from drinking this; YUCK!",
            smell: "It smells like sulfur",
            vagueSmell: "It smells like sulfur", tasteIntensity: 1000, smellIntensity: 100, alcohol: 0.0, food: 0.0,
            unusedNutritionValue: 0.0, water: -0.5, satiated: -6.0, viscosity: 4, density: 0.8, organic: false,
            displayColour: "bold magenta", dampDesc: "It is damp with fuel oil",
            wetDesc: "It is soaked with fuel oil", drenchedDesc: "It is drenched with fuel oil",
            dampSdesc: "(oil-damp)", wetSdesc: "(oil-soaked)", drenchedSdesc: "(oil-drenched)",
            solventVolumeRatio: 25.0, dried: null, residueVolumePercentage: 0.029, solvent: "detergent",
            injectionConsequence: LiquidInjectionConsequence.Deadly, countsAs: ("fuel", ItemQuality.Excellent), materialTags: ["Fuel"]);
        #endregion

        context.SaveChanges();

        foreach (KeyValuePair<Material, string> solvent in solvents)
        {
            solvent.Key.SolventId = liquids[solvent.Value].Id;
        }

        foreach (KeyValuePair<Liquid, string> solvent in liquidSolvents)
        {
            solvent.Key.SolventId = liquids[solvent.Value].Id;
        }

        foreach (KeyValuePair<Liquid, string> countAs in liquidCountsAs)
        {
            countAs.Key.CountAsId = liquids[countAs.Value].Id;
        }

        context.SaveChanges();
    }
}
