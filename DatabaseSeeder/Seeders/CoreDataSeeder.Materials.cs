#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Material;
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
			if (material.MaterialsTags.Any(x => x.TagId == tag.Id))
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
		AddMaterial("resin", MaterialBehaviourType.Paste, 1.1, true, 8000, 12000, 0.0, 0.19, 0.0001, 1800,
			"Natural Materials");
		AddMaterial("pitch", MaterialBehaviourType.Paste, 1.2, false, 8000, 12000, 0.0, 0.15, 0.0001, 1700,
			"Manufactured Materials");
		AddMaterial("tar", MaterialBehaviourType.Grease, 1.15, false, 8000, 12000, 0.0, 0.12, 0.0001, 1600,
			"Manufactured Materials");
		AddMaterial("shellac", MaterialBehaviourType.Paste, 1.1, true, 8000, 12000, 0.0, 0.18, 0.0001, 1700,
			"Animal Product");
		AddMaterial("latex", MaterialBehaviourType.Elastomer, 0.95, true, 12000, 25000, 0.0, 0.14, 0.0001, 2000,
			"Elastomer");
		AddMaterial("charcoal", MaterialBehaviourType.Powder, 0.4, true, 2000, 2000, 0.2, 0.08, 0.0001, 1000,
			"Natural Materials");
		AddMaterial("soot", MaterialBehaviourType.Powder, 0.25, true, 1000, 1000, 0.2, 0.08, 0.0001, 900,
			"Natural Materials");
		AddMaterial("chalk dust", MaterialBehaviourType.Powder, 0.7, false, 1000, 1000, 0.1, 0.16, 0.0001, 900,
			"Stone");
		AddMaterial("sponge", MaterialBehaviourType.Fabric, 0.3, true, 2000, 5000, 5.0, 0.05, 0.0001, 600,
			"Natural Materials");

		EnsureAlias(materials["PTFE"], "teflon");
		EnsureAlias(materials["silicone rubber"], "silicone");
		EnsureAlias(materials["ethylene-vinyl acetate"], "eva");
		EnsureAlias(materials["olefin fabric"], "polyolefin fabric");

		context.SaveChanges();
	}
}
