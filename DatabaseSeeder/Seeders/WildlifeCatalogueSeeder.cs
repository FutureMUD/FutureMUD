#nullable enable

using DatabaseSeeder;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.Traps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Installs the finished, stock-owned wildlife controller catalogue. The older Animal-prefixed
/// controller rows intentionally remain untouched as builder examples; this package owns the
/// Wildlife- and Managed Animal-prefixed rows and repairs them on every rerun.
/// </summary>
public sealed class WildlifeCatalogueSeeder : IDatabaseSeeder
{
	private const string StockPrefix = "Wildlife";
	private const int ShelterDecaySeconds = 30 * 24 * 60 * 60;

	public bool SafeToRunMoreThanOnce => true;
	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

	public int SortOrder => 303;
	public string Name => "Production Wildlife AI Catalogue";
	public string Tagline => "Finished wild, mythical-beast and managed-animal controllers with group templates";
	public string FullDescription =>
		"Installs production-ready Wildlife and Managed Animal AI archetypes, wildlife group templates, habitat tags, shelter anchors and a source-owned race recommendation manifest. These stock-owned rows are repaired on rerun; clone a row before intentional customisation. The older Animal rows remain as unchanged legacy examples.";

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any() || !context.FutureProgs.Any() || !context.Materials.Any() ||
		    !context.TraitDefinitions.Any() ||
		    !context.GameItemComponentProtos.Any(x => x.Name == "Holdable"))
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		return SeederRepeatabilityHelper.ClassifyByPresence(
			WildlifeCatalogue.IndividualProfileNames
				.Concat(WildlifeCatalogue.GroupTemplateNames)
				.Select(name => context.ArtificialIntelligences.Any(x => x.Name == name) ||
				                context.GroupAiTemplates.Any(x => x.Name == name)));
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		IReadOnlyList<string> validationErrors = WildlifeCatalogue.ValidateCatalogForTesting();
		if (validationErrors.Any())
		{
			throw new InvalidOperationException(
				$"The production wildlife catalogue is internally inconsistent: {string.Join("; ", validationErrors)}");
		}

		using var transaction = context.Database.BeginTransaction();
		try
		{
			var tags = EnsureWildlifeTags(context);
			EnsureWildlifeHabitatTagsOnStockTerrains(context, tags.Habitats);
			var progs = EnsureSupportProgs(context);
			var shelters = EnsureShelterAnchorsAndCrafts(context, tags, progs);
			EnsureAnimalProfiles(context, progs, shelters);
			var webTrap = EnsureNaturalTrap(context, "Wildlife Trap - Web Snare", "entangled in sticky webbing",
				"Stock natural trap for web-building wildlife.");
			var burrowTrap = EnsureNaturalTrap(context, "Wildlife Trap - Burrow Ambush", "caught in a collapsing burrow ambush",
				"Stock natural trap for burrowing ambush wildlife.");
			EnsureAuxiliaryAis(context, progs, webTrap, burrowTrap);
			EnsureGroupTemplates(context, progs);
			context.SaveChanges();
			transaction.Commit();

			return $"Installed or refreshed {WildlifeCatalogue.IndividualProfiles.Count:N0} finished individual wildlife profiles, " +
			       $"{WildlifeCatalogue.GroupTemplates.Count:N0} group templates, {shelters.Count:N0} shelter anchor crafts, " +
			       $"and {WildlifeCatalogue.Recommendations.Count:N0} race recommendations. Legacy Animal example rows were not changed.";
		}
		catch
		{
			transaction.Rollback();
			throw;
		}
	}

	private static WildlifeTagSet EnsureWildlifeTags(FuturemudDatabaseContext context)
	{
		Tag? terrain = context.Tags.FirstOrDefault(x => x.Name == "Terrain");
		Tag habitatRoot = EnsureTag(context, "Wildlife Habitat", terrain);
		var habitats = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
		foreach (string name in WildlifeCatalogue.HabitatTagNames)
		{
			// A few of these deliberately reuse long-standing terrain tags such as Wetland.
			// They are a shared vocabulary, not stock-owned taxonomy nodes, so never steal a
			// builder's existing parent merely to place the tag beneath Wildlife Habitat.
			habitats[name] = EnsureTag(context, name, habitatRoot, repairParent: false);
		}

		Tag shelterRoot = EnsureTag(context, "Wildlife Shelter", null);
		var shelters = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
		foreach (ShelterDefinition definition in WildlifeCatalogue.Shelters)
		{
			shelters[definition.Key] = EnsureTag(context, definition.TagName, shelterRoot);
		}

		context.SaveChanges();
		return new WildlifeTagSet(habitats, shelterRoot, shelters);
	}

	private static Tag EnsureTag(FuturemudDatabaseContext context, string name, Tag? parent, bool repairParent = true)
	{
		Tag? tag = context.Tags.Local.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ??
		           context.Tags.FirstOrDefault(x => x.Name == name);
		if (tag is null)
		{
			tag = new Tag { Name = name, Parent = parent };
			context.Tags.Add(tag);
			return tag;
		}

		if (repairParent && parent is not null && tag.ParentId != parent.Id)
		{
			tag.Parent = parent;
		}

		return tag;
	}

	private static void EnsureWildlifeHabitatTagsOnStockTerrains(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, Tag> habitatTags)
	{
		foreach ((string terrainName, IReadOnlyCollection<string> tags) in WildlifeCatalogue.StockTerrainHabitatTags)
		{
			Terrain? terrain = context.Terrains.FirstOrDefault(x => x.Name == terrainName);
			if (terrain is null)
			{
				continue;
			}

			var ids = terrain.TagInformation
				.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Select(x => long.TryParse(x, out long id) ? id : 0L)
				.Where(x => x > 0)
				.ToHashSet();
			foreach (string tagName in tags)
			{
				if (habitatTags.TryGetValue(tagName, out Tag? tag))
				{
					ids.Add(tag.Id);
				}
			}

			terrain.TagInformation = string.Join(",", ids.OrderBy(x => x));
		}

		context.SaveChanges();
	}

	private static WildlifeSupportProgs EnsureSupportProgs(FuturemudDatabaseContext context)
	{
		static FutureProg Ensure(FuturemudDatabaseContext db, string name, string description, string text,
			params (ProgVariableTypes Type, string Name)[] parameters)
		{
			return SeederRepeatabilityHelper.EnsureProg(
				db,
				name,
				"Wildlife AI",
				"Production Wildlife",
				ProgVariableTypes.Boolean,
				description,
				text,
				false,
				false,
				FutureProgStaticType.NotStatic,
				parameters);
		}

		const string characterCell = "@cell.Terrain";
		Ensure(context, WildlifeCatalogue.TerrestrialHabitatProg,
			"Allows terrestrial wildlife to stay on a tagged non-vacuum terrestrial habitat.",
			$"return istagged({characterCell}, \"Terrestrial\") and not(istagged({characterCell}, \"Vacuum\"))",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.GrasslandHabitatProg,
			"Selects open grassland, shrubland and agricultural habitat for grazing wildlife.",
			$"return istagged({characterCell}, \"Grassland\") or istagged({characterCell}, \"Shrubland\") or istagged({characterCell}, \"Agricultural Land\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.WoodlandHabitatProg,
			"Selects woodland, shrubland and cliff habitat for arboreal wildlife and roosting birds.",
			$"return istagged({characterCell}, \"Woodland\") or istagged({characterCell}, \"Shrubland\") or istagged({characterCell}, \"Cliff\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.HighlandHabitatProg,
			"Selects highland, cliff, tundra and grassland habitat for mountain and polar wildlife.",
			$"return istagged({characterCell}, \"Highland\") or istagged({characterCell}, \"Cliff\") or istagged({characterCell}, \"Tundra\") or istagged({characterCell}, \"Grassland\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.DesertHabitatProg,
			"Selects desert, shrubland and highland habitat for dryland wildlife.",
			$"return istagged({characterCell}, \"Desert\") or istagged({characterCell}, \"Shrubland\") or istagged({characterCell}, \"Highland\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.CaveHabitatProg,
			"Selects cave and subterranean habitat for burrowing and denning wildlife.",
			$"return istagged({characterCell}, \"Cave\") or istagged({characterCell}, \"Subterranean\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.WetlandHabitatProg,
			"Selects wetland, riverine, freshwater and lake habitat for amphibious wildlife.",
			$"return istagged({characterCell}, \"Wetland\") or istagged({characterCell}, \"Riverine\") or istagged({characterCell}, \"Freshwater\") or istagged({characterCell}, \"Lake\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.FreshwaterHabitatProg,
			"Selects freshwater, lake and riverine habitat for freshwater aquatic wildlife.",
			$"return istagged({characterCell}, \"Freshwater\") or istagged({characterCell}, \"Lake\") or istagged({characterCell}, \"Riverine\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.MarineHabitatProg,
			"Selects marine, coast, open-ocean and reef habitat for saltwater wildlife.",
			$"return istagged({characterCell}, \"Marine\") or istagged({characterCell}, \"Coast\") or istagged({characterCell}, \"Open Ocean\") or istagged({characterCell}, \"Reef\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.AquaticHabitatProg,
			"Selects aquatic freshwater or marine habitat for aquatic wildlife.",
			$"return istagged({characterCell}, \"Aquatic\") or istagged({characterCell}, \"Freshwater\") or istagged({characterCell}, \"Marine\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.ManagedHabitatProg,
			"Selects rural, agricultural and other human-influenced habitat for managed animals.",
			$"return istagged({characterCell}, \"Rural\") or istagged({characterCell}, \"Agricultural Land\") or istagged({characterCell}, \"Human Influenced\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.ShelterHabitatProg,
			"Selects caves, woodland, shrubland and cliff terrain as viable shelter and den habitat.",
			$"return istagged({characterCell}, \"Cave\") or istagged({characterCell}, \"Subterranean\") or istagged({characterCell}, \"Woodland\") or istagged({characterCell}, \"Shrubland\") or istagged({characterCell}, \"Cliff\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.NestHabitatProg,
			"Selects woodland, wetland and cliff terrain as viable nest and roost habitat.",
			$"return istagged({characterCell}, \"Woodland\") or istagged({characterCell}, \"Wetland\") or istagged({characterCell}, \"Cliff\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.AvoidUrbanProg,
			"Identifies urban terrain that ordinary wild animals avoid while selecting a path.",
			$"return istagged({characterCell}, \"Urban\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Location, "cell"));
		Ensure(context, WildlifeCatalogue.AnimalPreyProg,
			"Allows a predator to choose an animal target; AnimalAI excludes its own race and group before this policy runs.",
			"return isanimal(@target)",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Character, "target"));
		Ensure(context, WildlifeCatalogue.IntruderProg,
			"Identifies non-animal intruders for defensive, parental and territorial wildlife reactions.",
			"return not(isanimal(@target))",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Character, "target"));
		Ensure(context, WildlifeCatalogue.GroupAnimalPreyProg,
			"Allows a wildlife group template to identify animal prey through its single-target threat contract.",
			"return isanimal(@target)",
			(ProgVariableTypes.Character, "target"));
		Ensure(context, WildlifeCatalogue.GroupIntruderProg,
			"Allows a wildlife group template to identify non-animal intruders through its single-target threat contract.",
			"return not(isanimal(@target))",
			(ProgVariableTypes.Character, "target"));
		Ensure(context, WildlifeCatalogue.ProtectedYoungProg,
			"Identifies a same-race juvenile that a parental wildlife profile protects.",
			"return samerace(@animal.Race, @target.Race) and (@target.AgeCategory == \"Baby\" or @target.AgeCategory == \"Child\" or @target.AgeCategory == \"Youth\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Character, "target"));
		Ensure(context, WildlifeCatalogue.ShelterNeededProg,
			"Requests shelter for a wildlife animal exposed outdoors after dark.",
			"return @animal.Location.Outdoors >= 2 and @animal.Location.Light < 10",
			(ProgVariableTypes.Character, "animal"));
		Ensure(context, WildlifeCatalogue.CanBuildShelterProg,
			"Allows shelter construction only while the animal is not underwater.",
			"return not(isunderwater(@animal.Location, @animal.Layer))",
			(ProgVariableTypes.Character, "animal"));
		Ensure(context, WildlifeCatalogue.ShelterAnchorProg,
			"Identifies only stock-tagged wildlife shelter anchors so wildlife cannot claim arbitrary items.",
			"return istagged(@item, \"Wildlife Shelter\")",
			(ProgVariableTypes.Character, "animal"), (ProgVariableTypes.Item, "item"));

		context.SaveChanges();
		return WildlifeSupportProgs.Load(context);
	}

	private static void EnsureAnimalProfiles(FuturemudDatabaseContext context, WildlifeSupportProgs progs,
		IReadOnlyDictionary<string, ShelterSeedResult> shelters)
	{
		long alwaysTrueId = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue").Id;
		long alwaysFalseId = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse").Id;
		long alwaysOneId = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysOne")?.Id ?? 0L;

		foreach (WildlifeAnimalProfile profile in WildlifeCatalogue.IndividualProfiles)
		{
			ArtificialIntelligence ai = SeederRepeatabilityHelper.EnsureNamedEntity(
				context.ArtificialIntelligences,
				profile.Name,
				x => x.Name,
				() =>
				{
					var created = new ArtificialIntelligence();
					context.ArtificialIntelligences.Add(created);
					return created;
				});
			ai.Name = profile.Name;
			ai.Type = "Animal";
			ai.Definition = profile.BuildDefinition(progs, shelters, alwaysTrueId, alwaysFalseId, alwaysOneId);
		}

		context.SaveChanges();
	}

	private static void EnsureGroupTemplates(FuturemudDatabaseContext context, WildlifeSupportProgs progs)
	{
		long alwaysFalseId = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse").Id;
		foreach (WildlifeGroupTemplate template in WildlifeCatalogue.GroupTemplates)
		{
			GroupAiTemplate group = SeederRepeatabilityHelper.EnsureNamedEntity(
				context.GroupAiTemplates,
				template.Name,
				x => x.Name,
				() =>
				{
					var created = new GroupAiTemplate();
					context.GroupAiTemplates.Add(created);
					return created;
				});
			group.Name = template.Name;
			group.Definition = template.BuildDefinition(progs, alwaysFalseId);
		}

		context.SaveChanges();
	}

	private static IReadOnlyDictionary<string, ShelterSeedResult> EnsureShelterAnchorsAndCrafts(
		FuturemudDatabaseContext context, WildlifeTagSet tags, WildlifeSupportProgs progs)
	{
		Account account = context.Accounts.OrderBy(x => x.Id).First();
		Material wood = context.Materials.FirstOrDefault(x => x.Name == "wood") ?? context.Materials.First();
		GameItemComponentProto holdable = context.GameItemComponentProtos.First(x => x.Name == "Holdable");
		DateTime now = DateTime.UtcNow;
		long nextItemId = Math.Max(
			context.GameItemProtos.Select(x => (long?)x.Id).Max() ?? 0L,
			context.GameItemProtos.Local.Select(x => x.Id).DefaultIfEmpty(0L).Max()) + 1L;
		var items = new Dictionary<string, GameItemProto>(StringComparer.OrdinalIgnoreCase);

		foreach (ShelterDefinition definition in WildlifeCatalogue.Shelters)
		{
			GameItemProto? item = context.GameItemProtos
				.FirstOrDefault(x => x.UniqueName == definition.PrototypeUniqueName && x.RevisionNumber == 0);
			if (item is null)
			{
				item = new GameItemProto
				{
					Id = nextItemId++,
					RevisionNumber = 0,
					EditableItem = Editable(account, now, "Production Wildlife AI Catalogue shelter anchor."),
					UniqueName = definition.PrototypeUniqueName
				};
				context.GameItemProtos.Add(item);
			}

			item.Name = definition.Noun;
			item.UniqueName = definition.PrototypeUniqueName;
			item.Keywords = definition.Keywords;
			item.MaterialId = wood.Id;
			item.Size = definition.Size;
			item.Weight = definition.Weight;
			item.ReadOnly = false;
			item.LongDescription = definition.LongDescription;
			item.BuilderNotes =
				"Stock-owned wildlife shelter anchor. Wildlife claims refresh its thirty-day decay timer; clone before customisation.";
			item.BaseItemQuality = (int)ItemQuality.Standard;
			item.ShortDescription = definition.ShortDescription;
			item.FullDescription = definition.FullDescription;
			item.PermitPlayerSkins = false;
			item.CostInBaseCurrency = 0M;
			item.IsHiddenFromPlayers = false;
			item.MorphGameItemProtoId = null;
			item.MorphTimeSeconds = ShelterDecaySeconds;
			item.MorphEmote = "$0 collapse|collapses and decays into nothing$.";

			Tag shelterTag = tags.Shelters[definition.Key];
			EnsureItemTag(context, item, tags.ShelterRoot);
			EnsureItemTag(context, item, shelterTag);
			EnsureItemComponent(context, item, holdable);
			items[definition.Key] = item;
		}

		context.SaveChanges();

		TraitDefinition trait = context.TraitDefinitions.OrderBy(x => x.Id).First();
		FutureProg appearProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue");
		long nextCraftId = Math.Max(
			context.Crafts.Select(x => (long?)x.Id).Max() ?? 0L,
			context.Crafts.Local.Select(x => x.Id).DefaultIfEmpty(0L).Max()) + 1L;
		var results = new Dictionary<string, ShelterSeedResult>(StringComparer.OrdinalIgnoreCase);

		foreach (ShelterDefinition definition in WildlifeCatalogue.Shelters)
		{
			GameItemProto item = items[definition.Key];
			Craft? craft = context.Crafts.FirstOrDefault(x => x.Name == definition.CraftName && x.RevisionNumber == 0);
			if (craft is null)
			{
				craft = new Craft
				{
					Id = nextCraftId++,
					RevisionNumber = 0,
					EditableItem = Editable(account, now, "Input-free wildlife shelter anchor craft."),
					Name = definition.CraftName
				};
				context.Crafts.Add(craft);
			}

			craft.Blurb = $"Creates {definition.ShortDescription} as a wildlife shelter anchor.";
			craft.ActionDescription = definition.ActionDescription;
			craft.Category = "Wildlife";
			craft.Interruptable = true;
			craft.ToolQualityWeighting = 0.0;
			craft.InputQualityWeighting = 0.0;
			craft.CheckQualityWeighting = 1.0;
			craft.FreeSkillChecks = 1;
			craft.FailThreshold = (int)Outcome.MajorFail;
			craft.CheckTraitId = trait.Id;
			craft.CheckDifficulty = (int)Difficulty.Trivial;
			craft.FailPhase = 1;
			craft.QualityFormula = "5 + (outcome / 3)";
			craft.AppearInCraftsListProgId = appearProg.Id;
			craft.ActiveCraftItemSdesc = $"an in-progress {definition.Noun} craft";
			craft.IsPracticalCheck = true;

			context.CraftInputs.RemoveRange(context.CraftInputs
				.Where(x => x.CraftId == craft.Id && x.CraftRevisionNumber == craft.RevisionNumber)
				.ToList());
			context.CraftTools.RemoveRange(context.CraftTools
				.Where(x => x.CraftId == craft.Id && x.CraftRevisionNumber == craft.RevisionNumber)
				.ToList());
			context.CraftPhases.RemoveRange(context.CraftPhases
				.Where(x => x.CraftPhaseId == craft.Id && x.CraftPhaseRevisionNumber == craft.RevisionNumber)
				.ToList());
			context.CraftProducts.RemoveRange(context.CraftProducts
				.Where(x => x.CraftId == craft.Id && x.CraftRevisionNumber == craft.RevisionNumber)
				.ToList());

			craft.CraftPhases.Add(new CraftPhase
			{
				Craft = craft,
				PhaseNumber = 1,
				PhaseLengthInSeconds = 30,
				Echo = definition.BuildEcho,
				FailEcho = "$0 abandon|abandons the attempt before the shelter can take shape.",
				ExertionLevel = 0,
				StaminaUsage = 0.0
			});
			craft.CraftProducts.Add(new CraftProduct
			{
				Craft = craft,
				IsFailProduct = false,
				OriginalAdditionTime = now,
				ProductType = "SimpleProduct",
				Definition = new XElement("Definition",
					new XElement("ProductProducedId", item.Id),
					new XElement("Quantity", 1),
					new XElement("Skin", 0)).ToString()
			});
			results[definition.Key] = new ShelterSeedResult(item.Id, craft.Id);
		}

		context.SaveChanges();
		return new ReadOnlyDictionary<string, ShelterSeedResult>(results);
	}

	private static void EnsureItemTag(FuturemudDatabaseContext context, GameItemProto item, Tag tag)
	{
		if (context.GameItemProtosTags.Any(x => x.GameItemProtoId == item.Id &&
		                                         x.GameItemProtoRevisionNumber == item.RevisionNumber &&
		                                         x.TagId == tag.Id))
		{
			return;
		}

		context.GameItemProtosTags.Add(new GameItemProtosTags
		{
			GameItemProto = item,
			Tag = tag,
			GameItemProtoRevisionNumber = item.RevisionNumber
		});
	}

	private static void EnsureItemComponent(FuturemudDatabaseContext context, GameItemProto item,
		GameItemComponentProto component)
	{
		if (context.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemProtoId == item.Id &&
			                                                x.GameItemProtoRevision == item.RevisionNumber &&
			                                                x.GameItemComponentProtoId == component.Id &&
			                                                x.GameItemComponentRevision == component.RevisionNumber))
		{
			return;
		}

		context.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
		{
			GameItemProto = item,
			GameItemComponent = component,
			GameItemProtoRevision = item.RevisionNumber,
			GameItemComponentRevision = component.RevisionNumber
		});
	}

	private static TrapTemplateReference EnsureNaturalTrap(FuturemudDatabaseContext context, string trapName,
		string restraintDescription, string comment)
	{
		Account account = context.Accounts.OrderBy(x => x.Id).First();
		DateTime now = DateTime.UtcNow;
		TrapTemplate? template = context.TrapTemplates
			.FirstOrDefault(x => x.Name == trapName && x.RevisionNumber == 0);
		if (template is null)
		{
			long nextId = Math.Max(
				context.TrapTemplates.Select(x => (long?)x.Id).Max() ?? 0L,
				context.TrapTemplates.Local.Select(x => x.Id).DefaultIfEmpty(0L).Max()) + 1L;
			template = new TrapTemplate
			{
				Id = nextId,
				RevisionNumber = 0,
				Name = trapName,
				EditableItem = Editable(account, now, comment)
			};
			context.TrapTemplates.Add(template);
		}

		template.Definition = new XElement("TrapTemplate",
			new XAttribute("source", TrapSourceKind.Natural),
			new XAttribute("disarm", TrapDisarmPolicy.Safe),
			new XAttribute("lifecycle", TrapLifecyclePolicy.Indefinite),
			new XAttribute("charges", 1),
			new XAttribute("cooldown", TimeSpan.Zero),
			new XElement("Triggers",
				new XElement("Trigger", new XAttribute("type", TrapTriggerType.Proximity))),
			new XElement("Payloads",
				new XElement("Payload",
					new XAttribute("type", TrapPayloadType.Restraint),
					new XAttribute("delay", TimeSpan.Zero),
					new XAttribute("target", TrapTargetSelector.Triggerer),
					new XElement("Parameter", new XAttribute("name", "duration"), "00:00:20"),
					new XElement("Parameter", new XAttribute("name", "description"), restraintDescription))),
			new XElement("Components")).ToString();
		template.EditableItem.RevisionStatus = (int)RevisionStatus.Current;
		context.SaveChanges();
		return new TrapTemplateReference(template.Id, template.RevisionNumber);
	}

	private static void EnsureAuxiliaryAis(FuturemudDatabaseContext context, WildlifeSupportProgs progs,
		TrapTemplateReference webTrap, TrapTemplateReference burrowTrap)
	{
		foreach ((string name, string siteProgName, TrapTemplateReference trap) in new[]
		         {
			         (WildlifeCatalogue.WebAmbushAuxiliaryAi, WildlifeCatalogue.ShelterHabitatProg, webTrap),
			         (WildlifeCatalogue.BurrowAmbushAuxiliaryAi, WildlifeCatalogue.CaveHabitatProg, burrowTrap)
		         })
		{
			ArtificialIntelligence ai = SeederRepeatabilityHelper.EnsureNamedEntity(
				context.ArtificialIntelligences,
				name,
				x => x.Name,
				() =>
				{
					var created = new ArtificialIntelligence();
					context.ArtificialIntelligences.Add(created);
					return created;
				});
			ai.Name = name;
			ai.Type = "NaturalTrap";
			ai.Definition = new XElement("Definition",
				new XElement("TrapTemplateId", trap.Id),
				new XElement("TrapTemplateRevision", trap.Revision),
				new XElement("DeployEnabledProg", progs.CanBuildShelter),
				new XElement("SiteProg", progs[siteProgName])).ToString();
		}

		context.SaveChanges();
	}

	private static EditableItem Editable(Account account, DateTime now, string comment)
	{
		return new EditableItem
		{
			RevisionNumber = 0,
			RevisionStatus = (int)RevisionStatus.Current,
			BuilderAccountId = account.Id,
			ReviewerAccountId = account.Id,
			BuilderDate = now,
			ReviewerDate = now,
			BuilderComment = comment,
			ReviewerComment = comment
		};
	}

	internal sealed record ShelterSeedResult(long PrototypeId, long CraftId);
	private sealed record TrapTemplateReference(long Id, int Revision);

	private sealed record WildlifeTagSet(
		IReadOnlyDictionary<string, Tag> Habitats,
		Tag ShelterRoot,
		IReadOnlyDictionary<string, Tag> Shelters);
}

internal sealed class WildlifeSupportProgs
{
	private readonly IReadOnlyDictionary<string, long> _ids;

	private WildlifeSupportProgs(IReadOnlyDictionary<string, long> ids)
	{
		_ids = ids;
	}

	public long this[string name] => _ids[name];
	public long Terrestrial => this[WildlifeCatalogue.TerrestrialHabitatProg];
	public long Grassland => this[WildlifeCatalogue.GrasslandHabitatProg];
	public long Woodland => this[WildlifeCatalogue.WoodlandHabitatProg];
	public long Highland => this[WildlifeCatalogue.HighlandHabitatProg];
	public long Desert => this[WildlifeCatalogue.DesertHabitatProg];
	public long Cave => this[WildlifeCatalogue.CaveHabitatProg];
	public long Wetland => this[WildlifeCatalogue.WetlandHabitatProg];
	public long Freshwater => this[WildlifeCatalogue.FreshwaterHabitatProg];
	public long Marine => this[WildlifeCatalogue.MarineHabitatProg];
	public long Aquatic => this[WildlifeCatalogue.AquaticHabitatProg];
	public long Managed => this[WildlifeCatalogue.ManagedHabitatProg];
	public long Shelter => this[WildlifeCatalogue.ShelterHabitatProg];
	public long Nest => this[WildlifeCatalogue.NestHabitatProg];
	public long AvoidUrban => this[WildlifeCatalogue.AvoidUrbanProg];
	public long AnimalPrey => this[WildlifeCatalogue.AnimalPreyProg];
	public long Intruder => this[WildlifeCatalogue.IntruderProg];
	public long GroupAnimalPrey => this[WildlifeCatalogue.GroupAnimalPreyProg];
	public long GroupIntruder => this[WildlifeCatalogue.GroupIntruderProg];
	public long ProtectedYoung => this[WildlifeCatalogue.ProtectedYoungProg];
	public long ShelterNeeded => this[WildlifeCatalogue.ShelterNeededProg];
	public long CanBuildShelter => this[WildlifeCatalogue.CanBuildShelterProg];
	public long ShelterAnchor => this[WildlifeCatalogue.ShelterAnchorProg];

	public static WildlifeSupportProgs Load(FuturemudDatabaseContext context)
	{
		var ids = context.FutureProgs
			.Where(x => WildlifeCatalogue.SupportProgNames.Contains(x.FunctionName))
			.ToDictionary(x => x.FunctionName, x => x.Id, StringComparer.OrdinalIgnoreCase);
		return new WildlifeSupportProgs(new ReadOnlyDictionary<string, long>(ids));
	}
}

internal sealed record ShelterDefinition(
	string Key,
	string TagName,
	string PrototypeUniqueName,
	string CraftName,
	string Noun,
	string Keywords,
	string ShortDescription,
	string LongDescription,
	string FullDescription,
	string ActionDescription,
	string BuildEcho,
	int Size,
	double Weight);

internal sealed record WildlifeAnimalProfile(
	string Name,
	string Description,
	string Movement,
	string Home,
	string Feeding,
	string Water,
	string Threat,
	string Awareness,
	string Refuge,
	string Activity,
	string PreferredHabitatProg,
	string ToleratedHabitatProg,
	string HabitatDescription,
	int MovementRange,
	int AwarenessRange,
	double WanderChance,
	string Senses,
	string OrdinaryResponse,
	string HungryPreyResponse,
	string AttackedResponse,
	string TerritoryResponse,
	string ParentingResponse,
	string SeasonalResponse,
	string? ShelterKey = null,
	string? RefugeHabitatProg = null,
	bool Shelter = false,
	bool Seasonal = false,
	bool Nesting = false,
	bool Parenting = false,
	bool GroupShelterSharing = false,
	bool ShareTerritory = false,
	bool ShareTerritoryWithOtherRaces = false,
	string? DormantSeason = null,
	string? AggressiveSeason = null,
	string DormancyMode = "Rest",
	string TargetFlyingLayer = "InAir",
	string TargetRestingLayer = "HighInTrees",
	string PreferredTreeLayer = "HighInTrees",
	string SecondaryTreeLayer = "InTrees",
	string RefugeLayer = "HighInTrees",
	string EngageDelay = "1d500+750",
	string EngageEmote = "",
	string PostureEmote = "@ posture|postures warily at $1.",
	string PostureDuration = "1d20+20",
	string? NestingSeason = null,
	string? SeasonalHabitatSeason = null,
	string? SeasonalHabitatProg = null)
{
	public string BuildDefinition(WildlifeSupportProgs progs, IReadOnlyDictionary<string, WildlifeCatalogueSeeder.ShelterSeedResult> shelters,
		long alwaysTrueId, long alwaysFalseId, long alwaysOneId)
	{
		long preferred = progs[PreferredHabitatProg];
		long tolerated = progs[ToleratedHabitatProg];
		long shelterProg = progs[RefugeHabitatProg ?? WildlifeCatalogue.ShelterHabitatProg];
		long craftId = ShelterKey is not null && shelters.TryGetValue(ShelterKey, out var shelter)
			? shelter.CraftId
			: 0L;
		bool predator = Feeding.In("Predator", "DenPredator", "Omnivore", "DenOmnivore");
		bool defensive = Threat == "Defend" || OrdinaryResponse.In("Posture", "Attack") ||
		                 TerritoryResponse.In("Posture", "Attack") || ParentingResponse.In("Posture", "Attack");
		long attackProg = predator ? progs.AnimalPrey : defensive ? progs.Intruder : alwaysFalseId;
		long threatProg = Awareness == "None" ? alwaysFalseId : progs.Intruder;
		long movementCellProg = tolerated;
		long descentProg = Movement == "Arboreal" ? alwaysTrueId : alwaysFalseId;
		long ecologicalSiteProg = Nesting ? progs.Nest : shelterProg;

		return new XElement("Definition",
			new XComment(Description),
			new XElement("Movement",
				new XAttribute("type", Movement),
				new XElement("Range", MovementRange),
				new XElement("AmphibiousWaterBias", Movement == "Amphibious" ? 0.65 : 0.50),
				new XElement("WanderChancePerMinute", WanderChance),
				new XElement("WanderEmote", new XCData(string.Empty)),
				new XElement("MovementEnabledProg", alwaysTrueId),
				new XElement("MovementCellProg", movementCellProg),
				new XElement("PreferredHabitatProg", preferred),
				new XElement("ToleratedHabitatProg", tolerated),
				new XElement("AmphibiousLandCellProg", progs.Terrestrial),
				new XElement("AmphibiousWaterCellProg", progs.Aquatic),
				new XElement("AllowDescentProg", descentProg),
				new XElement("TargetFlyingLayer", TargetFlyingLayer),
				new XElement("TargetRestingLayer", TargetRestingLayer),
				new XElement("PreferredTreeLayer", PreferredTreeLayer),
				new XElement("SecondaryTreeLayer", SecondaryTreeLayer)),
			new XElement("Home",
				new XAttribute("type", Home),
				new XElement("SuitableTerritoryProg", preferred),
				new XElement("DesiredTerritorySizeProg", alwaysOneId),
				new XElement("WillShareTerritory", ShareTerritory),
				new XElement("WillShareTerritoryWithOtherRaces", ShareTerritoryWithOtherRaces),
				new XElement("AllowGroupShelterSharing", GroupShelterSharing),
				new XElement("BurrowCraftId", craftId),
				new XElement("BurrowSiteProg", shelterProg),
				new XElement("BuildEnabledProg", progs.CanBuildShelter),
				new XElement("HomeLocationProg", 0),
				new XElement("AnchorItemProg", ShelterKey is null ? 0 : progs.ShelterAnchor)),
			new XElement("Feeding",
				new XAttribute("type", Feeding),
				new XElement("WillAttackProg", attackProg),
				new XElement("UseActiveNeeds", true),
				new XElement("EngageDelayDiceExpression", new XCData(EngageDelay)),
				new XElement("EngageEmote", new XCData(EngageEmote))),
			new XElement("Water", new XAttribute("type", Water)),
			new XElement("Threat",
				new XAttribute("type", Threat),
				new XElement("OrdinaryResponse", OrdinaryResponse),
				new XElement("HungryPreyResponse", HungryPreyResponse),
				new XElement("AttackedResponse", AttackedResponse),
				new XElement("TerritoryResponse", TerritoryResponse),
				new XElement("ParentingResponse", ParentingResponse),
				new XElement("SeasonalResponse", SeasonalResponse),
				new XElement("PostureEmote", new XCData(PostureEmote)),
				new XElement("PostureDurationDiceExpression", new XCData(PostureDuration))),
			new XElement("Awareness",
				new XAttribute("type", Awareness),
				new XElement("ThreatProg", threatProg),
				new XElement("AvoidCellProg", Name.StartsWith("Managed Animal", StringComparison.OrdinalIgnoreCase)
					? alwaysFalseId
					: progs.AvoidUrban),
				new XElement("Range", AwarenessRange),
				new XElement("MemoryMinutes", Senses == "Tracking" ? 30 : 10),
				new XElement("Senses", Senses)),
			new XElement("Refuge",
				new XAttribute("type", Refuge),
				new XElement("Layer", RefugeLayer),
				new XElement("CellProg", Refuge == "None" ? alwaysFalseId : shelterProg),
				new XElement("ReturnSeconds", 60)),
			new XElement("Activity",
				new XAttribute("type", Activity),
				new XElement("SleepEnabled", Activity != "Always"),
				new XElement("DormancyMode", DormancyMode),
				new XElement("RestEmote", new XCData("@ settle|settles into a quiet resting posture.")),
				DormantSeason is null ? null : new XElement("DormantSeasonGroup", DormantSeason),
				AggressiveSeason is null ? null : new XElement("AggressiveSeasonGroup", AggressiveSeason),
				NestingSeason is null ? null : new XElement("NestingSeasonGroup", NestingSeason)),
			new XElement("Ecology",
				new XElement("ShelterEnabled", Shelter),
				new XElement("SeasonalEnabled", Seasonal),
				new XElement("NestingEnabled", Nesting),
				new XElement("ParentingEnabled", Parenting),
				new XElement("ShelterNeededProg", Shelter ? progs.ShelterNeeded : alwaysFalseId),
				new XElement("ShelterCellProg", Shelter ? shelterProg : alwaysFalseId),
				new XElement("SeasonalCellProg", Seasonal ? preferred : alwaysFalseId),
				new XElement("NestSiteProg", Nesting ? ecologicalSiteProg : alwaysFalseId),
				new XElement("ProtectProg", Parenting ? progs.ProtectedYoung : alwaysFalseId),
				SeasonalHabitatSeason is null || SeasonalHabitatProg is null
					? null
					: new XElement("SeasonalHabitat",
						new XAttribute("seasonGroup", SeasonalHabitatSeason), progs[SeasonalHabitatProg])),
			new XElement("OpenDoors", false),
			new XElement("UseKeys", false),
			new XElement("SmashLockedDoors", false),
			new XElement("CloseDoorsBehind", false),
			new XElement("UseDoorguards", false),
			new XElement("MoveEvenIfObstructionInWay", false)).ToString(SaveOptions.DisableFormatting);
	}
}

internal sealed record WildlifeGroupTemplate(
	string Name,
	string Kind,
	string Tactic,
	string Scope,
	string PreferredHabitatProg,
	string ShelterHabitatProg,
	string ThreatProg,
	IReadOnlyCollection<int> ActiveTimes,
	int MovementRange,
	double WanderChance,
	string Description)
{
	public string BuildDefinition(WildlifeSupportProgs progs, long alwaysFalseId)
	{
		return new XElement("Template",
			new XComment(Description),
			new XElement("AvoidCellProg", alwaysFalseId),
			new XElement("ConsidersThreatProg", progs[ThreatProg]),
			new XElement("Emotes"),
			new XElement("GroupType",
				new XAttribute("typename", "wildlife"),
				new XElement("Gender", 0),
				new XElement("ActiveTimes", ActiveTimes.Select(x => new XElement("Time", x))),
				new XElement("Kind", Kind),
				new XElement("Tactic", Tactic),
				new XElement("ControlScope", Scope),
				new XElement("PreferredCellProg", progs[PreferredHabitatProg]),
				new XElement("ShelterCellProg", progs[ShelterHabitatProg]),
				new XElement("MovementRange", MovementRange),
				new XElement("WanderChancePerMinute", WanderChance))).ToString(SaveOptions.DisableFormatting);
	}
}

internal sealed record WildlifeRecommendation(
	string RaceName,
	bool Mythical,
	string IndividualAiTemplate,
	string GroupTemplate,
	string SocialModel,
	string PreferredHabitat,
	string ToleratedHabitat,
	string ActivityPolicy,
	string ShelterPolicy,
	string ThreatPolicy,
	string SensesPolicy,
	IReadOnlyCollection<string> AuxiliaryAiTemplates,
	string? ManagedIndividualAiTemplate,
	string? ManagedGroupTemplate);

/// <summary>
/// Source-owned ecology metadata and generated recommendation manifest. This is deliberately
/// data-first: it gives every installed normal animal and every explicitly eligible mythical
/// beast one exact wild controller plus either a concrete group template or Solitary.
/// </summary>
internal static class WildlifeCatalogue
{
	public const string TerrestrialHabitatProg = "Wildlife AI - Terrestrial Habitat";
	public const string GrasslandHabitatProg = "Wildlife AI - Grassland Habitat";
	public const string WoodlandHabitatProg = "Wildlife AI - Woodland Habitat";
	public const string HighlandHabitatProg = "Wildlife AI - Highland Habitat";
	public const string DesertHabitatProg = "Wildlife AI - Desert Habitat";
	public const string CaveHabitatProg = "Wildlife AI - Cave Habitat";
	public const string WetlandHabitatProg = "Wildlife AI - Wetland Habitat";
	public const string FreshwaterHabitatProg = "Wildlife AI - Freshwater Habitat";
	public const string MarineHabitatProg = "Wildlife AI - Marine Habitat";
	public const string AquaticHabitatProg = "Wildlife AI - Aquatic Habitat";
	public const string ManagedHabitatProg = "Wildlife AI - Managed Habitat";
	public const string ShelterHabitatProg = "Wildlife AI - Shelter Habitat";
	public const string NestHabitatProg = "Wildlife AI - Nest Habitat";
	public const string AvoidUrbanProg = "Wildlife AI - Avoid Urban";
	public const string AnimalPreyProg = "Wildlife AI - Animal Prey";
	public const string IntruderProg = "Wildlife AI - Non-Animal Intruder";
	public const string GroupAnimalPreyProg = "Wildlife Group AI - Animal Prey";
	public const string GroupIntruderProg = "Wildlife Group AI - Non-Animal Intruder";
	public const string ProtectedYoungProg = "Wildlife AI - Protected Young";
	public const string ShelterNeededProg = "Wildlife AI - Shelter Needed";
	public const string CanBuildShelterProg = "Wildlife AI - Can Build Shelter";
	public const string ShelterAnchorProg = "Wildlife AI - Shelter Anchor";
	public const string WebAmbushAuxiliaryAi = "Wildlife - Web Ambush Trap";
	public const string BurrowAmbushAuxiliaryAi = "Wildlife - Burrow Ambush Trap";

	public const string TimidGroundForager = "Wildlife - Timid Ground Forager";
	public const string BurrowingForager = "Wildlife - Burrowing Forager";
	public const string TerritorialGrazer = "Wildlife - Territorial Grazer";
	public const string DefensiveGrazer = "Wildlife - Defensive Grazer";
	public const string SeasonalGrazer = "Wildlife - Seasonal Grazer";
	public const string HibernatingForager = "Wildlife - Hibernating Forager";
	public const string HibernatingOmnivore = "Wildlife - Hibernating Omnivore";
	public const string OpportunistOmnivore = "Wildlife - Opportunist Omnivore";
	public const string NocturnalScavenger = "Wildlife - Nocturnal Scavenger";
	public const string GroundStalkingPredator = "Wildlife - Ground Stalking Predator";
	public const string DenningPredator = "Wildlife - Denning Predator";
	public const string AmbushPredator = "Wildlife - Ambush Predator";
	public const string ArborealForager = "Wildlife - Arboreal Forager";
	public const string ArborealPredator = "Wildlife - Arboreal Predator";
	public const string GroundFeedingBird = "Wildlife - Ground-Feeding Bird";
	public const string RoostingBird = "Wildlife - Arboreal-Roost Bird";
	public const string MigratoryFlier = "Wildlife - Migratory Flier";
	public const string Raptor = "Wildlife - Raptor";
	public const string FreshwaterForager = "Wildlife - Freshwater Forager";
	public const string MarineForager = "Wildlife - Marine Forager";
	public const string AquaticPredator = "Wildlife - Aquatic Predator";
	public const string SurfaceBreathingPredator = "Wildlife - Surface-Breathing Predator";
	public const string RiverinePredator = "Wildlife - Riverine Predator";
	public const string AmphibiousForager = "Wildlife - Amphibious Forager";
	public const string AquaticScavenger = "Wildlife - Aquatic Scavenger";
	public const string ColonialInsect = "Wildlife - Colonial Insect";
	public const string MythicAerialPredator = "Wildlife - Mythic Aerial Predator";
	public const string MythicGuardian = "Wildlife - Mythic Guardian";
	public const string ManagedLivestockGrazer = "Managed Animal - Livestock Grazer";
	public const string ManagedLivestockOmnivore = "Managed Animal - Livestock Omnivore";
	public const string ManagedPoultry = "Managed Animal - Poultry";
	public const string ManagedWaterfowl = "Managed Animal - Waterfowl";
	public const string ManagedCompanionPredator = "Managed Animal - Companion Predator";
	public const string ManagedCompanionForager = "Managed Animal - Companion Forager";
	public const string ManagedOrnamentalAquatic = "Managed Animal - Ornamental Aquatic";

	public const string TimidGrazingHerd = "Wildlife Group - Timid Grazing Herd";
	public const string DefensiveGrazingHerd = "Wildlife Group - Defensive Grazing Herd";
	public const string TerritorialGrazingHerd = "Wildlife Group - Territorial Grazing Herd";
	public const string SeasonalGrazingHerd = "Wildlife Group - Seasonal Grazing Herd";
	public const string ProtectiveFamilyHerd = "Wildlife Group - Protective Family Herd";
	public const string CursorialHuntingPack = "Wildlife Group - Cursorial Hunting Pack";
	public const string DenningPredatorFamily = "Wildlife Group - Denning Predator Family";
	public const string TerritorialPride = "Wildlife Group - Territorial Pride";
	public const string ScavengerClan = "Wildlife Group - Scavenger Clan";
	public const string GroundFeedingFlock = "Wildlife Group - Ground-Feeding Flock";
	public const string ArborealRoostFlock = "Wildlife Group - Arboreal-Roost Flock";
	public const string MigratoryFlight = "Wildlife Group - Migratory Flight";
	public const string DefensiveWaterfowlFlock = "Wildlife Group - Defensive Waterfowl Flock";
	public const string FreshwaterSchool = "Wildlife Group - Freshwater School";
	public const string MarineSchool = "Wildlife Group - Marine School";
	public const string AquaticHuntingSchool = "Wildlife Group - Aquatic-Hunting School";
	public const string SurfaceBreathingPod = "Wildlife Group - Surface-Breathing Pod";
	public const string AmphibiousColony = "Wildlife Group - Amphibious Colony";
	public const string BurrowingColony = "Wildlife Group - Burrowing Colony";
	public const string InsectColonySwarm = "Wildlife Group - Insect Colony and Swarm";
	public const string RaptorFamily = "Wildlife Group - Raptor Family";
	public const string MythicAerialHuntingFlight = "Wildlife Group - Mythic Aerial Hunting Flight";
	public const string ManagedLivestockHerd = "Managed Animal Group - Livestock Herd";
	public const string ManagedPoultryWaterfowlFlock = "Managed Animal Group - Poultry and Waterfowl Flock";
	public const string ManagedCompanionPack = "Managed Animal Group - Companion Pack";

	public static readonly IReadOnlyCollection<string> HabitatTagNames =
	[
		"Grassland", "Shrubland", "Woodland", "Highland", "Cliff", "Cave", "Subterranean", "Wetland",
		"Riverine", "Freshwater", "Lake", "Marine", "Coast", "Open Ocean", "Reef", "Polar", "Tundra",
		"Desert", "Agricultural Land"
	];

	public static readonly IReadOnlyCollection<string> SupportProgNames =
	[
		TerrestrialHabitatProg, GrasslandHabitatProg, WoodlandHabitatProg, HighlandHabitatProg, DesertHabitatProg,
		CaveHabitatProg, WetlandHabitatProg, FreshwaterHabitatProg, MarineHabitatProg, AquaticHabitatProg,
		ManagedHabitatProg, ShelterHabitatProg, NestHabitatProg, AvoidUrbanProg, AnimalPreyProg, IntruderProg,
		GroupAnimalPreyProg, GroupIntruderProg,
		ProtectedYoungProg, ShelterNeededProg, CanBuildShelterProg, ShelterAnchorProg
	];

	public static readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> StockTerrainHabitatTags =
		new ReadOnlyDictionary<string, IReadOnlyCollection<string>>(
			new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase)
			{
				// These are the exact stock terrain names from CoreDataSeeder. The wildlife
				// package appends only its own tags, leaving every existing terrain tag intact.
				["Barn"] = ["Agricultural Land"],
				["Indoor Spring"] = ["Agricultural Land", "Freshwater"],
				["Animal Trail"] = ["Agricultural Land"],
				["Trail"] = ["Agricultural Land"],
				["Dirt Road"] = ["Agricultural Land"],
				["Compacted Dirt Road"] = ["Agricultural Land"],
				["Gravel Road"] = ["Agricultural Land"],
				["Cobblestone Road"] = ["Agricultural Land"],
				["Rural Street"] = ["Agricultural Land"],
				["Grasslands"] = ["Grassland"],
				["Savannah"] = ["Grassland", "Shrubland"],
				["Steppe"] = ["Grassland", "Highland"],
				["Shrublands"] = ["Shrubland"],
				["Shortgrass Prairie"] = ["Grassland"],
				["Tallgrass Prairie"] = ["Grassland"],
				["Heath"] = ["Shrubland"],
				["Pasture"] = ["Grassland", "Agricultural Land"],
				["Meadow"] = ["Grassland"],
				["Field"] = ["Grassland", "Agricultural Land"],
				["Tundra"] = ["Tundra", "Polar", "Highland"],
				["Flood Plain"] = ["Grassland", "Riverine", "Freshwater", "Agricultural Land"],
				["Chaparral"] = ["Shrubland", "Desert"],
				["Badlands"] = ["Desert", "Highland"],
				["Salt Flat"] = ["Desert"],
				["Hills"] = ["Highland"],
				["Foothills"] = ["Highland"],
				["Mound"] = ["Highland"],
				["Drumlin"] = ["Highland"],
				["Butte"] = ["Highland", "Desert"],
				["Kuppe"] = ["Highland"],
				["Mesa"] = ["Highland", "Desert"],
				["Canyon"] = ["Highland", "Desert"],
				["Knoll"] = ["Highland"],
				["Moor"] = ["Highland", "Shrubland"],
				["Tell"] = ["Highland"],
				["Dunes"] = ["Desert"],
				["Plateau"] = ["Highland"],
				["Escarpment"] = ["Cliff", "Highland"],
				["Scree Slope"] = ["Cliff", "Highland"],
				["Talus Field"] = ["Cliff", "Highland"],
				["Mountainside"] = ["Highland", "Cliff"],
				["Mountain Pass"] = ["Highland", "Cliff"],
				["Mountain Ridge"] = ["Highland", "Cliff"],
				["Cliff Face"] = ["Cliff", "Highland"],
				["Cliff Edge"] = ["Cliff", "Highland"],
				["Valley"] = ["Grassland"],
				["Vale"] = ["Grassland"],
				["Dell"] = ["Woodland", "Grassland"],
				["Glen"] = ["Woodland", "Grassland"],
				["Strath"] = ["Woodland", "Grassland"],
				["Combe"] = ["Grassland"],
				["Ravine"] = ["Highland", "Cliff"],
				["Gorge"] = ["Highland", "Cliff"],
				["Gully"] = ["Highland", "Cliff"],
				["Bramble"] = ["Shrubland", "Woodland"],
				["Boreal Forest"] = ["Woodland", "Polar"],
				["Broadleaf Forest"] = ["Woodland"],
				["Temperate Coniferous Forest"] = ["Woodland"],
				["Temperate Rainforest"] = ["Woodland", "Wetland"],
				["Tropical Rainforest"] = ["Woodland", "Wetland"],
				["Woodland"] = ["Woodland"],
				["Grove"] = ["Woodland", "Agricultural Land"],
				["Plantation Forest"] = ["Woodland", "Agricultural Land"],
				["Orchard"] = ["Woodland", "Agricultural Land"],
				["Bog"] = ["Wetland"],
				["Fen"] = ["Wetland"],
				["Marsh"] = ["Wetland"],
				["Salt Marsh"] = ["Wetland", "Coast"],
				["Wetland"] = ["Wetland"],
				["Mangrove Swamp"] = ["Wetland", "Coast", "Woodland"],
				["Swamp Forest"] = ["Wetland", "Woodland"],
				["Tropical Freshwater Swamp"] = ["Wetland", "Freshwater", "Woodland"],
				["Temperate Freshwater Swamp"] = ["Wetland", "Freshwater", "Woodland"],
				["Sandy Desert"] = ["Desert"],
				["Rocky Desert"] = ["Desert", "Highland"],
				["Coastal Desert"] = ["Desert", "Coast"],
				["Oasis"] = ["Desert", "Freshwater", "Wetland"],
				["Glacier"] = ["Polar", "Tundra"],
				["Ice Field"] = ["Polar", "Tundra"],
				["Snowfield"] = ["Polar", "Tundra"],
				["Grotto"] = ["Cave", "Subterranean"],
				["Cave Entrance"] = ["Cave", "Subterranean"],
				["Cave"] = ["Cave", "Subterranean"],
				["Cavern"] = ["Cave", "Subterranean"],
				["Cave Pool"] = ["Cave", "Subterranean", "Freshwater"],
				["Underground Water"] = ["Cave", "Subterranean", "Freshwater"],
				["Riverbank"] = ["Riverine", "Freshwater", "Wetland"],
				["Shallow River"] = ["Riverine", "Freshwater"],
				["River"] = ["Riverine", "Freshwater"],
				["Deep River"] = ["Riverine", "Freshwater"],
				["Lake Shore"] = ["Lake", "Freshwater", "Wetland"],
				["Shallow Lake"] = ["Lake", "Freshwater"],
				["Lake"] = ["Lake", "Freshwater"],
				["Deep Lake"] = ["Lake", "Freshwater"],
				["Sandy Beach"] = ["Coast", "Marine"],
				["Rocky Beach"] = ["Coast", "Marine", "Cliff"],
				["Beachrock"] = ["Coast", "Marine", "Cliff"],
				["Ocean Shallows"] = ["Coast", "Marine"],
				["Ocean Surf"] = ["Coast", "Marine"],
				["Cove"] = ["Coast", "Marine"],
				["Tide Pool"] = ["Coast", "Marine", "Reef"],
				["Shoal"] = ["Coast", "Marine", "Reef"],
				["Ocean"] = ["Marine", "Open Ocean"],
				["Bay"] = ["Marine", "Coast"],
				["Sound"] = ["Marine", "Coast"],
				["Deep Ocean"] = ["Marine", "Open Ocean"],
				["Lagoon"] = ["Marine", "Coast", "Freshwater"],
				["Estuary"] = ["Marine", "Coast", "Freshwater", "Riverine"],
				["Coral Reef"] = ["Marine", "Reef"],
				["Reef"] = ["Marine", "Reef"],
				["Mudflat"] = ["Wetland", "Coast", "Marine"]
			});

	public static readonly IReadOnlyList<ShelterDefinition> Shelters =
	[
		new("Burrow", "Wildlife Shelter - Burrow", "WildlifeShelter_Burrow", "Wildlife Shelter - Build Burrow",
			"burrow", "burrow earth entrance", "a freshly-dug burrow",
			"A freshly-dug burrow breaks the earth here, its narrow entrance edged with loose soil.",
			"The burrow is a compact wildlife shelter that will collapse if no occupant refreshes it for thirty real-world days.",
			"digging a burrow", "@ dig|digs and packs the earth into a secure burrow.", 2, 35000.0),
		new("Den", "Wildlife Shelter - Den", "WildlifeShelter_Den", "Wildlife Shelter - Build Den",
			"den", "den brush hollow", "a brush-lined den",
			"A brush-lined den has been worked into a sheltered hollow here.",
			"The den is a wildlife shelter anchor; use is recorded through its saving claim effect.",
			"building a den", "@ gather|gathers brush and builds a sheltered den.", 3, 45000.0),
		new("GroundNest", "Wildlife Shelter - Ground Nest", "WildlifeShelter_GroundNest", "Wildlife Shelter - Build Ground Nest",
			"nest", "nest grass reed ground", "a grass-lined ground nest",
			"A grass-lined nest cups the ground in a carefully chosen sheltered patch.",
			"The nest is a wildlife shelter anchor for ground-nesting animals.",
			"building a ground nest", "@ arrange|arranges grass and reeds into a ground nest.", 1, 12000.0),
		new("TreeNest", "Wildlife Shelter - Tree Nest", "WildlifeShelter_TreeNest", "Wildlife Shelter - Build Tree Nest",
			"nest", "nest roost branches", "a woven tree nest",
			"A woven nest of twigs and leaves rests securely among the branches.",
			"The tree nest is a wildlife shelter anchor for arboreal roosting and nesting animals.",
			"building a tree nest", "@ weave|weaves twigs and leaves into a secure tree nest.", 1, 8000.0),
		new("Lair", "Wildlife Shelter - Lair", "WildlifeShelter_Lair", "Wildlife Shelter - Build Lair",
			"lair", "lair cave hollow", "a claimed lair",
			"A deliberately claimed lair occupies the most defensible part of this shelter.",
			"The lair is a wildlife shelter anchor for territorial and mythical predators.",
			"claiming a lair", "@ claim|claims and arranges a defensible lair.", 4, 90000.0),
		new("Lodge", "Wildlife Shelter - Lodge", "WildlifeShelter_Lodge", "Wildlife Shelter - Build Lodge",
			"lodge", "lodge dam sticks", "a stick-built lodge",
			"A compact lodge of interlocked branches and packed vegetation sits at the water's edge.",
			"The lodge is a wildlife shelter anchor for social riverine builders.",
			"building a lodge", "@ interlock|interlocks branches into a compact lodge.", 4, 110000.0),
		new("WebNest", "Wildlife Shelter - Web Nest", "WildlifeShelter_WebNest", "Wildlife Shelter - Build Web Nest",
			"nest", "web nest silk", "a silken web nest",
			"A dense silken web nest spans a sheltered angle here.",
			"The web nest is a wildlife shelter anchor for web-building and ambush species.",
			"spinning a web nest", "@ spin|spins a dense silken web nest.", 1, 1000.0)
	];

	public static readonly IReadOnlyCollection<string> ManagedEligibleRaceNames =
		AgricultureSeeder.StockHerdAnimalRaceNamesForWildlife
			.Concat(["Cat", "Dog", "Ferret", "Hamster", "Parrot", "Macaw", "Cockatoo", "Koi"])
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();

	public static readonly IReadOnlyList<WildlifeAnimalProfile> IndividualProfiles = BuildIndividualProfiles();
	public static readonly IReadOnlyList<WildlifeGroupTemplate> GroupTemplates = BuildGroupTemplates();
	public static readonly IReadOnlyList<WildlifeRecommendation> Recommendations = BuildRecommendations();
	public static IReadOnlyCollection<string> IndividualProfileNames =>
		IndividualProfiles.Select(x => x.Name).OrderBy(x => x).ToArray();
	public static IReadOnlyCollection<string> GroupTemplateNames =>
		GroupTemplates.Select(x => x.Name).OrderBy(x => x).ToArray();
	public static string RecommendationManifestJsonForTesting => System.Text.Json.JsonSerializer.Serialize(
		Recommendations.OrderBy(x => x.RaceName, StringComparer.OrdinalIgnoreCase),
		new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

	/// <summary>
	/// Validates the data-only contract before any database writes. Keeping this public to the
	/// seeder test assembly makes catalogue drift fail fast instead of producing half-configured
	/// builder rows.
	/// </summary>
	internal static IReadOnlyList<string> ValidateCatalogForTesting()
	{
		var issues = new List<string>();
		var knownResponses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Ignore", "Avoid", "Flee", "Posture", "Attack"
		};
		var knownSenses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"None", "Vigilant", "Hiding", "Stalking", "Tracking"
		};
		var knownSeasons = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Spring", "Summer", "Autumn", "Winter"
		};
		var knownDormancyModes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Rest", "Hibernation", "Torpor"
		};

		void RequireUnique(IEnumerable<string> names, string category)
		{
			foreach (IGrouping<string, string> duplicate in names.GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			         .Where(x => x.Count() > 1))
			{
				issues.Add($"duplicate {category} '{duplicate.Key}'");
			}
		}

		RequireUnique(IndividualProfiles.Select(x => x.Name), "individual profile");
		RequireUnique(GroupTemplates.Select(x => x.Name), "group template");
		RequireUnique(Shelters.Select(x => x.Key), "shelter definition");
		RequireUnique(Recommendations.Select(x => $"{x.Mythical}:{x.RaceName}"), "race recommendation");
		var profiles = IndividualProfiles
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
		var groups = GroupTemplates
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
		var shelters = Shelters
			.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

		foreach (WildlifeAnimalProfile profile in IndividualProfiles)
		{
			if (!SupportProgNames.Contains(profile.PreferredHabitatProg) ||
			    !SupportProgNames.Contains(profile.ToleratedHabitatProg) ||
			    profile.RefugeHabitatProg is not null && !SupportProgNames.Contains(profile.RefugeHabitatProg))
			{
				issues.Add($"{profile.Name} references an unknown habitat prog");
			}

			if (profile.ShelterKey is not null && !shelters.ContainsKey(profile.ShelterKey))
			{
				issues.Add($"{profile.Name} references missing shelter key '{profile.ShelterKey}'");
			}

			if (!knownSenses.Contains(profile.Senses))
			{
				issues.Add($"{profile.Name} uses unknown senses policy '{profile.Senses}'");
			}

			if (new[]
			    {
				    profile.OrdinaryResponse, profile.HungryPreyResponse, profile.AttackedResponse,
				    profile.TerritoryResponse, profile.ParentingResponse, profile.SeasonalResponse
			    }.Any(x => !knownResponses.Contains(x)))
			{
				issues.Add($"{profile.Name} contains an unknown contextual threat response");
			}

			if (profile.DormantSeason is not null && !knownSeasons.Contains(profile.DormantSeason) ||
			    profile.AggressiveSeason is not null && !knownSeasons.Contains(profile.AggressiveSeason) ||
			    profile.NestingSeason is not null && !knownSeasons.Contains(profile.NestingSeason) ||
			    profile.SeasonalHabitatSeason is not null && !knownSeasons.Contains(profile.SeasonalHabitatSeason))
			{
				issues.Add($"{profile.Name} contains an unknown hemisphere-aware season group");
			}

			if ((profile.SeasonalHabitatSeason is null) != (profile.SeasonalHabitatProg is null) ||
			    profile.SeasonalHabitatProg is not null && !SupportProgNames.Contains(profile.SeasonalHabitatProg))
			{
				issues.Add($"{profile.Name} has an incomplete or unknown seasonal habitat preference");
			}

			if (!knownDormancyModes.Contains(profile.DormancyMode))
			{
				issues.Add($"{profile.Name} contains an unknown dormancy mode '{profile.DormancyMode}'");
			}
		}

		foreach (WildlifeGroupTemplate group in GroupTemplates)
		{
			if (!SupportProgNames.Contains(group.PreferredHabitatProg) ||
			    !SupportProgNames.Contains(group.ShelterHabitatProg) ||
			    !SupportProgNames.Contains(group.ThreatProg))
			{
				issues.Add($"{group.Name} references an unknown wildlife support prog");
			}

			if (!group.ThreatProg.In(GroupAnimalPreyProg, GroupIntruderProg))
			{
				issues.Add($"{group.Name} must use a single-target group threat prog");
			}

			if (group.ActiveTimes.Count == 0 || group.MovementRange < 1 ||
			    group.WanderChance is < 0.0 or > 1.0)
			{
				issues.Add($"{group.Name} has an invalid activity or movement configuration");
			}
		}

		var normalRaceNames = AnimalAIStockTemplates.AnimalRecommendationsForTesting.Keys
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var mappedNormalNames = Recommendations.Where(x => !x.Mythical).Select(x => x.RaceName)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (!normalRaceNames.SetEquals(mappedNormalNames))
		{
			issues.Add("normal animal recommendations are not exhaustive and one-to-one");
		}

		var eligibleMythicalNames = MythicalAnimalSeeder.TemplatesForTesting.Values
			.Where(x => x.WildlifeEligible)
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var mappedMythicalNames = Recommendations.Where(x => x.Mythical).Select(x => x.RaceName)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (!eligibleMythicalNames.SetEquals(mappedMythicalNames))
		{
			issues.Add("eligible mythical-beast recommendations are not exhaustive and one-to-one");
		}

		foreach (WildlifeRecommendation recommendation in Recommendations)
		{
			if (!profiles.ContainsKey(recommendation.IndividualAiTemplate))
			{
				issues.Add($"{recommendation.RaceName} references missing individual profile '{recommendation.IndividualAiTemplate}'");
			}

			if (recommendation.GroupTemplate != "Solitary" && !groups.ContainsKey(recommendation.GroupTemplate))
			{
				issues.Add($"{recommendation.RaceName} references missing group template '{recommendation.GroupTemplate}'");
			}

			if (recommendation.AuxiliaryAiTemplates.Any(x =>
				    !x.In(WebAmbushAuxiliaryAi, BurrowAmbushAuxiliaryAi)))
			{
				issues.Add($"{recommendation.RaceName} references an unknown auxiliary AI");
			}

			if (recommendation.ManagedIndividualAiTemplate is null != recommendation.ManagedGroupTemplate is null)
			{
				issues.Add($"{recommendation.RaceName} has an incomplete managed recommendation");
			}

			if (recommendation.ManagedIndividualAiTemplate is not null &&
			    !profiles.ContainsKey(recommendation.ManagedIndividualAiTemplate))
			{
				issues.Add($"{recommendation.RaceName} references missing managed profile");
			}

			if (recommendation.ManagedGroupTemplate is not null && !groups.ContainsKey(recommendation.ManagedGroupTemplate))
			{
				issues.Add($"{recommendation.RaceName} references missing managed group template");
			}
		}

		foreach (string raceName in ManagedEligibleRaceNames)
		{
			WildlifeRecommendation? recommendation = Recommendations.FirstOrDefault(x =>
				!x.Mythical && x.RaceName.Equals(raceName, StringComparison.OrdinalIgnoreCase));
			if (recommendation?.ManagedIndividualAiTemplate is null || recommendation.ManagedGroupTemplate is null)
			{
				issues.Add($"managed-eligible race '{raceName}' lacks both managed recommendations");
			}
		}

		return issues;
	}

	private static IReadOnlyList<WildlifeAnimalProfile> BuildIndividualProfiles()
	{
		return
		[
			new(TimidGroundForager,
				"A small, skittish ground forager that scans vigilantly, avoids intrusion and moves between open feeding patches.",
				"Ground", "None", "Forager", "Drink", "Flee", "Wimpy", "None", "Crepuscular",
				GrasslandHabitatProg, TerrestrialHabitatProg, "grassland and shrubland", 10, 4, 0.38, "Hiding",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee"),
			new(BurrowingForager,
				"A denning forager that feeds at dawn and dusk, uses marked burrows and shelters after dark.",
				"Ground", "Denning", "Forager", "Drink", "Flee", "Skittish", "Den", "Crepuscular",
				GrasslandHabitatProg, TerrestrialHabitatProg, "grassland, shrubland and sheltered soil", 10, 5, 0.28, "Hiding",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee", "Burrow", ShelterHabitatProg, true, false, false, false),
			new(TerritorialGrazer,
				"A diurnal grazer that maintains a home range, groups with its own kind and postures before retreating.",
				"Ground", "Territorial", "Forager", "Drink", "Flee", "Wary", "Home", "Diurnal",
				GrasslandHabitatProg, TerrestrialHabitatProg, "grassland and agricultural margins", 18, 5, 0.26, "Vigilant",
				"Avoid", "Ignore", "Flee", "Posture", "Posture", "Posture", null, null, false, false, false, false,
				false, true, false, null, "Spring"),
			new(DefensiveGrazer,
				"A large defensive grazer that keeps a territory, watches intruders and escalates a warning posture when pressed.",
				"Ground", "Territorial", "Forager", "Drink", "Defend", "Guarding", "Home", "Diurnal",
				GrasslandHabitatProg, TerrestrialHabitatProg, "open grazing country", 20, 6, 0.20, "Vigilant",
				"Posture", "Ignore", "Attack", "Posture", "Attack", "Attack", null, null, true, true, false, true,
				true, true, false, null, "Spring", EngageEmote: "@ stamp|stamps and lowers its head towards $1.",
				PostureEmote: "@ posture|plants itself squarely and warns $1 away."),
			new(SeasonalGrazer,
				"A seasonal, shelter-seeking grazer that ranges across open country and becomes markedly defensive in its breeding season.",
				"Ground", "Territorial", "Forager", "Drink", "Flee", "Wary", "Home", "Diurnal",
				HighlandHabitatProg, TerrestrialHabitatProg, "highland, tundra and open grassland", 24, 6, 0.30, "Vigilant",
				"Avoid", "Ignore", "Flee", "Posture", "Posture", "Attack", "Lair", ShelterHabitatProg, true, true, false, true,
				true, true, false, null, "Spring", SeasonalHabitatSeason: "Winter", SeasonalHabitatProg: GrasslandHabitatProg),
			new(HibernatingForager,
				"A temperate forager that shelters through winter, wakes for hunger, thirst and immediate threats, and resumes spring ranging.",
				"Ground", "Denning", "Forager", "Drink", "Flee", "Wary", "Den", "Diurnal",
				WoodlandHabitatProg, TerrestrialHabitatProg, "woodland and sheltered shrubland", 14, 5, 0.22, "Hiding",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Posture", "Den", ShelterHabitatProg, true, true, false, false,
				false, false, false, "Winter", "Spring", DormancyMode: "Hibernation"),
			new(HibernatingOmnivore,
				"A temperate omnivore that shelters and hibernates through winter, waking only for acute survival needs or immediate danger.",
				"Ground", "Denning", "Omnivore", "Drink", "HungryPredator", "Wary", "Den", "Diurnal",
				WoodlandHabitatProg, TerrestrialHabitatProg, "woodland and sheltered shrubland", 16, 6, 0.18, "Hiding",
				"Avoid", "Attack", "Attack", "Posture", "Posture", "Posture", "Den", ShelterHabitatProg, true, true, false, true,
				false, false, false, "Winter", "Spring", DormancyMode: "Hibernation",
				EngageEmote: "@ rise|rises from cover and advances towards $1."),
			new(OpportunistOmnivore,
				"A terrestrial omnivore that forages and scavenges first, then hunts an animal prey only when hungry.",
				"Ground", "None", "Omnivore", "Drink", "HungryPredator", "Wary", "None", "Always",
				TerrestrialHabitatProg, TerrestrialHabitatProg, "mixed terrestrial habitat", 16, 5, 0.30, "Tracking",
				"Avoid", "Attack", "Flee", "Posture", "Posture", "Posture", EngageEmote: "@ advance|advances warily towards $1."),
			new(NocturnalScavenger,
				"A night-active scavenger that hides by day, avoids intruders and searches for carrion and discarded food.",
				"Ground", "None", "Scavenger", "Drink", "Flee", "Wary", "None", "Nocturnal",
				TerrestrialHabitatProg, TerrestrialHabitatProg, "mixed terrestrial habitat", 14, 5, 0.32, "Hiding",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee"),
			new(GroundStalkingPredator,
				"A solitary territorial predator that stalks animal prey while hungry and defends its range with a visible warning posture.",
				"Ground", "Territorial", "Predator", "Drink", "HungryPredator", "Wary", "Home", "Crepuscular",
				WoodlandHabitatProg, TerrestrialHabitatProg, "woodland, shrubland and cliff country", 20, 7, 0.26, "Tracking",
				"Avoid", "Attack", "Attack", "Posture", "Posture", "Attack", null, null, true, true, false, true,
				false, false, false, null, "Spring", EngageEmote: "@ stalk|stalks towards $1."),
			new(DenningPredator,
				"A nocturnal denning predator that tracks prey while hungry, drags food home and protects its marked shelter.",
				"Ground", "Denning", "DenPredator", "Drink", "HungryPredator", "Wary", "Den", "Nocturnal",
				WoodlandHabitatProg, TerrestrialHabitatProg, "woodland, shrubland and caves", 18, 7, 0.22, "Tracking",
				"Avoid", "Attack", "Attack", "Posture", "Attack", "Attack", "Den", ShelterHabitatProg, true, true, false, true,
				true, false, false, null, "Spring", EngageEmote: "@ stalk|stalks towards $1."),
			new(AmbushPredator,
				"A low-wandering ambush predator that uses a marked den or web nest, hides and strikes animal prey at night.",
				"Ground", "Denning", "DenPredator", "Drink", "HungryPredator", "Skittish", "Den", "Nocturnal",
				CaveHabitatProg, TerrestrialHabitatProg, "caves, subterranean terrain and sheltered edges", 10, 6, 0.10, "Stalking",
				"Ignore", "Attack", "Attack", "Posture", "Attack", "Attack", "WebNest", CaveHabitatProg, true, false, false, true,
				false, false, false, null, "Spring", EngageDelay: "1d800+1200", EngageEmote: "@ lunge|lunges suddenly towards $1."),
			new(ArborealForager,
				"A tree-moving diurnal forager that descends for needs, returns to the canopy and uses roost sites at rest.",
				"Arboreal", "None", "Forager", "Drink", "Flee", "Wary", "Trees", "Diurnal",
				WoodlandHabitatProg, WoodlandHabitatProg, "woodland and cliff-edge canopy", 14, 6, 0.34, "Vigilant",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee", null, NestHabitatProg, false, false, true, false),
			new(ArborealPredator,
				"A tree-moving predator that waits in cover, tracks animal prey and returns to high branches after hunting.",
				"Arboreal", "Territorial", "Predator", "Drink", "HungryPredator", "Wary", "Trees", "Nocturnal",
				WoodlandHabitatProg, WoodlandHabitatProg, "woodland canopy and cliff ledges", 16, 7, 0.20, "Stalking",
				"Avoid", "Attack", "Attack", "Posture", "Posture", "Attack", null, NestHabitatProg, false, false, true, true,
				false, false, false, null, "Spring", EngageEmote: "@ drop|drops silently towards $1."),
			new(GroundFeedingBird,
				"A ground-feeding bird that scans frequently, flees from intruders and uses a small ground nest during its nesting season.",
				"Ground", "Denning", "Forager", "Drink", "Flee", "Skittish", "Den", "Diurnal",
				GrasslandHabitatProg, TerrestrialHabitatProg, "grassland, wetland edge and farmland", 14, 6, 0.34, "Vigilant",
				"Flee", "Ignore", "Flee", "Flee", "Posture", "Posture", "GroundNest", NestHabitatProg, true, true, true, true,
				true, true, false, null, "Spring", NestingSeason: "Spring"),
			new(RoostingBird,
				"A diurnal, arboreal-roosting bird that returns to a marked tree nest and protects young during nesting season.",
				"Arboreal", "Denning", "Forager", "Drink", "Flee", "Skittish", "Trees", "Diurnal",
				WoodlandHabitatProg, WoodlandHabitatProg, "woodland canopy and roosting cliffs", 16, 7, 0.35, "Vigilant",
				"Flee", "Ignore", "Flee", "Flee", "Posture", "Posture", "TreeNest", NestHabitatProg, true, true, true, true,
				true, true, false, null, "Spring", NestingSeason: "Spring"),
			new(MigratoryFlier,
				"A long-ranging flier that follows coastal, wetland and woodland stops while remaining alert to danger.",
				"Fly", "None", "Forager", "Drink", "Flee", "Wary", "Sky", "Diurnal",
				WetlandHabitatProg, TerrestrialHabitatProg, "wetland, coast and terrestrial migration stops", 42, 7, 0.45, "Vigilant",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee", TargetFlyingLayer: "HighInAir", TargetRestingLayer: "HighInAir",
				RefugeLayer: "HighInAir", SeasonalHabitatSeason: "Winter", SeasonalHabitatProg: MarineHabitatProg),
			new(Raptor,
				"A raptor that hunts animal prey from the air, scans widely and returns to a protected cliff or tree roost.",
				"Fly", "Denning", "DenPredator", "Drink", "HungryPredator", "Wary", "Sky", "Diurnal",
				HighlandHabitatProg, TerrestrialHabitatProg, "cliffs, highland and woodland hunting ground", 36, 9, 0.32, "Vigilant",
				"Avoid", "Attack", "Attack", "Posture", "Attack", "Attack", "TreeNest", NestHabitatProg, true, true, true, true,
				true, false, false, null, "Spring", TargetFlyingLayer: "HighInAir", TargetRestingLayer: "HighInAir",
				RefugeLayer: "HighInAir", EngageEmote: "@ stoop|stoops from above towards $1.", NestingSeason: "Spring"),
			new(FreshwaterForager,
				"A schooling freshwater forager that remains in lakes and rivers, following food and group movement.",
				"Swim", "None", "Forager", "Immerse", "Flee", "Wary", "Water", "Always",
				FreshwaterHabitatProg, FreshwaterHabitatProg, "freshwater lakes and rivers", 28, 6, 0.42, "Vigilant",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee", RefugeHabitatProg: FreshwaterHabitatProg),
			new(MarineForager,
				"A marine forager that travels through coastal and open-ocean water while remaining in a school or loose feeding aggregation.",
				"Swim", "None", "Forager", "Immerse", "Flee", "Wary", "Water", "Always",
				MarineHabitatProg, MarineHabitatProg, "marine coast, reef and open ocean", 34, 6, 0.45, "Vigilant",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee", RefugeHabitatProg: MarineHabitatProg),
			new(AquaticPredator,
				"An aquatic predator that tracks animal prey while hungry and stays within its freshwater or marine habitat policy.",
				"Swim", "Territorial", "Predator", "Immerse", "HungryPredator", "Wary", "Water", "Always",
				AquaticHabitatProg, AquaticHabitatProg, "aquatic freshwater or marine habitat", 34, 7, 0.32, "Tracking",
				"Avoid", "Attack", "Attack", "Posture", "Posture", "Attack", RefugeHabitatProg: AquaticHabitatProg,
				EngageEmote: "@ surge|surges through the water towards $1."),
			new(SurfaceBreathingPredator,
				"A surface-breathing aquatic predator that hunts from water, returns to the surface and coordinates readily in a pod.",
				"Swim", "Territorial", "Predator", "Surface", "HungryPredator", "Wary", "Water", "Always",
				MarineHabitatProg, MarineHabitatProg, "marine and coastal surface water", 36, 8, 0.36, "Tracking",
				"Avoid", "Attack", "Attack", "Posture", "Posture", "Attack", RefugeHabitatProg: MarineHabitatProg,
				EngageEmote: "@ surface|surfaces and surges towards $1."),
			new(RiverinePredator,
				"A riverine ambush predator that waits in wetland and river habitat, postures at its territory edge and attacks when provoked.",
				"Amphibious", "Territorial", "Predator", "Surface", "HungryPredator", "Wary", "Water", "Crepuscular",
				WetlandHabitatProg, WetlandHabitatProg, "wetland, riverine and freshwater habitat", 22, 7, 0.16, "Stalking",
				"Avoid", "Attack", "Attack", "Posture", "Posture", "Attack", "Lair", WetlandHabitatProg, true, true, false, true,
				false, false, false, null, "Spring", EngageEmote: "@ lunge|lunges from the water at $1."),
			new(AmphibiousForager,
				"An amphibious forager that alternates between wetland water and its adjoining land habitat, hiding when threatened.",
				"Amphibious", "None", "Forager", "Immerse", "Flee", "Wary", "Water", "Diurnal",
				WetlandHabitatProg, WetlandHabitatProg, "wetland, freshwater and riverine habitat", 20, 6, 0.28, "Hiding",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee", RefugeHabitatProg: WetlandHabitatProg),
			new(AquaticScavenger,
				"An aquatic scavenger that searches its water habitat for carrion and edible detritus while avoiding direct conflict.",
				"Swim", "None", "Scavenger", "Immerse", "Flee", "Wary", "Water", "Always",
				AquaticHabitatProg, AquaticHabitatProg, "aquatic freshwater and marine habitat", 26, 5, 0.38, "Hiding",
				"Flee", "Ignore", "Flee", "Flee", "Flee", "Flee", RefugeHabitatProg: AquaticHabitatProg),
			new(ColonialInsect,
				"A colonial insect that feeds locally, returns to a shared shelter anchor and uses vigilant sentry-like scanning.",
				"Ground", "Denning", "Forager", "Drink", "Defend", "Wary", "Den", "Diurnal",
				WoodlandHabitatProg, TerrestrialHabitatProg, "woodland, shrubland and agricultural margins", 12, 5, 0.26, "Vigilant",
				"Ignore", "Ignore", "Attack", "Posture", "Attack", "Attack", "WebNest", ShelterHabitatProg, true, true, false, true,
				true, true, false, null, "Spring"),
			new(MythicAerialPredator,
				"A large mythical aerial predator that hunts animal prey, defends a high lair and ranges over broad mountainous habitat.",
				"Fly", "Denning", "DenPredator", "Drink", "HungryPredator", "Guarding", "Sky", "Always",
				HighlandHabitatProg, TerrestrialHabitatProg, "highland, cliff and broad terrestrial hunting territory", 50, 10, 0.26, "Vigilant",
				"Posture", "Attack", "Attack", "Posture", "Attack", "Attack", "Lair", HighlandHabitatProg, true, true, false, true,
				false, false, false, null, "Spring", TargetFlyingLayer: "HighInAir", TargetRestingLayer: "HighInAir",
				RefugeLayer: "HighInAir", EngageEmote: "@ wheel|wheels through the air towards $1."),
			new(MythicGuardian,
				"A non-sapient mythical guardian-beast that browses, holds a territory and relies on posture before direct attack.",
				"Ground", "Territorial", "Forager", "Drink", "Defend", "Guarding", "Home", "Always",
				HighlandHabitatProg, TerrestrialHabitatProg, "highland, woodland and open grazing terrain", 22, 8, 0.20, "Vigilant",
				"Posture", "Ignore", "Attack", "Posture", "Attack", "Attack", "Lair", ShelterHabitatProg, true, true, false, true,
				false, false, false, null, "Spring", EngageEmote: "@ challenge|challenges $1 with a fierce display."),
			new(ManagedLivestockGrazer,
				"A calm managed grazer that stays close to rural and agricultural terrain, groups reliably and does not hunt handlers or ordinary people.",
				"Ground", "Territorial", "Forager", "Drink", "Passive", "Wary", "Home", "Diurnal",
				ManagedHabitatProg, ManagedHabitatProg, "rural, agricultural and human-influenced terrain", 14, 4, 0.20, "Vigilant",
				"Ignore", "Ignore", "Flee", "Ignore", "Posture", "Ignore", "Lodge", ManagedHabitatProg, true, false, false, true,
				true, true, true),
			new(ManagedLivestockOmnivore,
				"A calm managed omnivore that forages and scavenges around agricultural land without initiating hunts against domestic handlers.",
				"Ground", "Territorial", "Opportunist", "Drink", "Passive", "Wary", "Home", "Diurnal",
				ManagedHabitatProg, ManagedHabitatProg, "rural, agricultural and human-influenced terrain", 14, 4, 0.22, "Vigilant",
				"Ignore", "Ignore", "Flee", "Ignore", "Posture", "Ignore", "Den", ManagedHabitatProg, true, false, false, true,
				true, true, true),
			new(ManagedPoultry,
				"A calm managed poultry profile that feeds in agricultural terrain, returns to a shared nest or roost and flees ordinary disturbance.",
				"Ground", "Denning", "Forager", "Drink", "Passive", "Skittish", "Den", "Diurnal",
				ManagedHabitatProg, ManagedHabitatProg, "farmyards, fields and rural settlements", 12, 5, 0.24, "Vigilant",
				"Ignore", "Ignore", "Flee", "Ignore", "Posture", "Ignore", "GroundNest", ManagedHabitatProg, true, false, true, true,
				true, true, true, NestingSeason: "Spring"),
			new(ManagedWaterfowl,
				"A calm managed waterfowl profile that stays around rural freshwater, nests communally and uses a protective posture near young.",
				"Amphibious", "Denning", "Forager", "Immerse", "Passive", "Wary", "Water", "Diurnal",
				ManagedHabitatProg, WetlandHabitatProg, "rural freshwater, ponds and wet fields", 14, 5, 0.24, "Vigilant",
				"Ignore", "Ignore", "Flee", "Ignore", "Posture", "Ignore", "GroundNest", WetlandHabitatProg, true, false, true, true,
				true, true, true, NestingSeason: "Spring"),
			new(ManagedCompanionPredator,
				"A calm managed companion predator that remains near human-influenced terrain and responds defensively only when attacked or its young are threatened.",
				"Ground", "Denning", "Opportunist", "Drink", "Passive", "Wary", "Den", "Diurnal",
				ManagedHabitatProg, ManagedHabitatProg, "settlements, rural land and managed homes", 14, 5, 0.20, "Vigilant",
				"Ignore", "Ignore", "Flee", "Ignore", "Posture", "Ignore", "Den", ManagedHabitatProg, true, false, false, true,
				true, true, true),
			new(ManagedCompanionForager,
				"A calm managed companion forager that remains in human-influenced terrain, uses roost or den anchors and avoids conflict.",
				"Arboreal", "Denning", "Forager", "Drink", "Passive", "Wary", "Trees", "Diurnal",
				ManagedHabitatProg, ManagedHabitatProg, "human-influenced rural and residential habitat", 12, 5, 0.20, "Vigilant",
				"Ignore", "Ignore", "Flee", "Ignore", "Posture", "Ignore", "TreeNest", ManagedHabitatProg, true, false, true, true,
				true, true, true, NestingSeason: "Spring"),
			new(ManagedOrnamentalAquatic,
				"A calm ornamental aquatic animal that remains in managed freshwater and does not initiate hostile encounters.",
				"Swim", "None", "Forager", "Immerse", "Passive", "Wary", "Water", "Diurnal",
				ManagedHabitatProg, FreshwaterHabitatProg, "managed ponds, rural freshwater and human-influenced water", 16, 4, 0.20, "Vigilant",
				"Ignore", "Ignore", "Flee", "Ignore", "Ignore", "Ignore", RefugeHabitatProg: FreshwaterHabitatProg)
		];
	}

	private static IReadOnlyList<WildlifeGroupTemplate> BuildGroupTemplates()
	{
		const string herdScope = "Movement, Feeding, Threats, Activity, Senses";
		const string huntingScope = "Movement, Threats, Activity, Senses";
		const string shelterScope = "Movement, Threats, Activity, Shelter, Senses";
		const string aquaticScope = "Movement, Feeding, Activity, Senses";
		const string aquaticHuntingScope = "Movement, Activity, Senses";
		const string managedScope = "Movement, Feeding, Activity, Shelter, Senses";
		const string managedCompanionScope = "Movement, Activity, Shelter, Senses";
		return
		[
			new(TimidGrazingHerd, "Herd", "Timid", herdScope, GrasslandHabitatProg, ShelterHabitatProg, GroupIntruderProg,
				[1, 2, 3], 22, 0.28, "A cautious grazing herd that scans, gathers stragglers and retreats from intruders."),
			new(DefensiveGrazingHerd, "Herd", "Defensive", herdScope, GrasslandHabitatProg, ShelterHabitatProg, GroupIntruderProg,
				[1, 2, 3], 20, 0.24, "A large grazing herd that gives a coordinated warning posture before protecting its members."),
			new(TerritorialGrazingHerd, "Herd", "Territorial", shelterScope, GrasslandHabitatProg, ShelterHabitatProg, GroupIntruderProg,
				[1, 2, 3], 18, 0.20, "A territorial herbivore herd that maintains a home range and defends it only after posturing."),
			new(SeasonalGrazingHerd, "Herd", "SeasonalGrazing", aquaticScope, HighlandHabitatProg, ShelterHabitatProg, GroupIntruderProg,
				[1, 2, 3], 28, 0.34, "A seasonal herd that rotates through reachable preferred habitat after local grazing periods."),
			new(ProtectiveFamilyHerd, "Family", "Defensive", shelterScope, WetlandHabitatProg, ShelterHabitatProg, GroupIntruderProg,
				[1, 2, 3], 18, 0.18, "A family herd that keeps young together, uses a shelter and protects them after warning postures."),
			new(CursorialHuntingPack, "Pack", "Hunting", huntingScope, WoodlandHabitatProg, ShelterHabitatProg, GroupAnimalPreyProg,
				[0, 1, 4], 24, 0.30, "A cursorial hunting pack that scouts, closes on a shared prey focus and retreats as a group."),
			new(DenningPredatorFamily, "Family", "Hunting", shelterScope, WoodlandHabitatProg, ShelterHabitatProg, GroupAnimalPreyProg,
				[0, 1, 4], 18, 0.22, "A denning predator family that hunts cooperatively and returns to its marked shelter."),
			new(TerritorialPride, "Pride", "Territorial", shelterScope, GrasslandHabitatProg, ShelterHabitatProg, GroupAnimalPreyProg,
				[0, 1, 4], 24, 0.22, "A territorial pride that coordinates a focused hunt and protects a defended home range."),
			new(ScavengerClan, "Pack", "Scavenging", huntingScope, TerrestrialHabitatProg, ShelterHabitatProg, GroupAnimalPreyProg,
				[0, 1, 4], 18, 0.28, "A scavenger clan that remains social while it searches for food and can focus a disturbed prey target."),
			new(GroundFeedingFlock, "Flock", "Timid", herdScope, GrasslandHabitatProg, NestHabitatProg, GroupIntruderProg,
				[1, 2, 3], 16, 0.32, "A ground-feeding flock with vigilant sentries, coordinated retreat and shared feeding movement."),
			new(ArborealRoostFlock, "Flock", "Roosting", shelterScope, WoodlandHabitatProg, NestHabitatProg, GroupIntruderProg,
				[1, 2, 3], 18, 0.34, "An arboreal flock that feeds by day and returns together to a sheltered roost."),
			new(MigratoryFlight, "Flock", "Roosting", shelterScope, WetlandHabitatProg, NestHabitatProg, GroupIntruderProg,
				[1, 2, 3], 42, 0.45, "A migratory flight that uses reachable habitat stops, regroups airborne stragglers and roosts safely."),
			new(DefensiveWaterfowlFlock, "Flock", "Defensive", herdScope, WetlandHabitatProg, NestHabitatProg, GroupIntruderProg,
				[1, 2, 3], 16, 0.26, "A waterfowl flock that gathers young, postures around nest sites and retreats to water."),
			new(FreshwaterSchool, "School", "Aquatic", aquaticScope, FreshwaterHabitatProg, FreshwaterHabitatProg, GroupIntruderProg,
				[0, 1, 2, 3, 4], 30, 0.42, "A freshwater school that stays in lakes and rivers, scans its water column and moves as a unit."),
			new(MarineSchool, "School", "Aquatic", aquaticScope, MarineHabitatProg, MarineHabitatProg, GroupIntruderProg,
				[0, 1, 2, 3, 4], 36, 0.45, "A marine school that maintains ambient water movement across coastal and open-ocean habitat."),
			new(AquaticHuntingSchool, "School", "Hunting", aquaticHuntingScope, AquaticHabitatProg, AquaticHabitatProg, GroupAnimalPreyProg,
				[0, 1, 2, 3, 4], 34, 0.34, "A coordinated aquatic-hunting school that chooses a shared prey focus."),
			new(SurfaceBreathingPod, "Pod", "Aquatic", aquaticScope, MarineHabitatProg, MarineHabitatProg, GroupIntruderProg,
				[0, 1, 2, 3, 4], 38, 0.34, "A surface-breathing pod that remains in marine water, keeps members together and scouts visibly."),
			new(AmphibiousColony, "Colony", "Amphibious", shelterScope, WetlandHabitatProg, WetlandHabitatProg, GroupIntruderProg,
				[1, 2, 3, 4], 18, 0.30, "An amphibious colony that uses wetland shelter, scouts its margins and retreats into water."),
			new(BurrowingColony, "Colony", "Defensive", shelterScope, GrasslandHabitatProg, CaveHabitatProg, GroupIntruderProg,
				[1, 2, 3], 14, 0.24, "A burrowing colony that maintains a shared shelter, organizes sentries and protects young."),
			new(InsectColonySwarm, "Swarm", "Defensive", shelterScope, WoodlandHabitatProg, ShelterHabitatProg, GroupIntruderProg,
				[1, 2, 3], 14, 0.32, "An insect colony or swarm that has sentries, shared shelter and defensive coordinated posturing."),
			new(RaptorFamily, "Family", "Hunting", shelterScope, HighlandHabitatProg, NestHabitatProg, GroupAnimalPreyProg,
				[1, 2, 3], 32, 0.30, "A raptor family that maintains a high roost, scouts and coordinates its prey focus."),
			new(MythicAerialHuntingFlight, "Flock", "Hunting", huntingScope, HighlandHabitatProg, HighlandHabitatProg, GroupAnimalPreyProg,
				[0, 1, 2, 3, 4], 50, 0.28, "A mythic aerial hunting flight that scouts broad terrain and converges on prey from above."),
			new(ManagedLivestockHerd, "Managed", "Managed", managedScope, ManagedHabitatProg, ManagedHabitatProg, GroupIntruderProg,
				[1, 2, 3], 14, 0.18, "A calm managed livestock herd that stays around agricultural habitat and retains its own shelter."),
			new(ManagedPoultryWaterfowlFlock, "Managed", "Managed", managedScope, ManagedHabitatProg, WetlandHabitatProg, GroupIntruderProg,
				[1, 2, 3], 12, 0.20, "A calm managed flock for poultry and waterfowl that remains around farm habitat and common roosts."),
			new(ManagedCompanionPack, "Managed", "Managed", managedCompanionScope, ManagedHabitatProg, ManagedHabitatProg, GroupIntruderProg,
				[1, 2, 3], 12, 0.16, "A calm managed companion group that remains close to human-influenced habitat without hunting handlers.")
		];
	}

	private static IReadOnlyList<WildlifeRecommendation> BuildRecommendations()
	{
		var profiles = IndividualProfiles.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var recommendations = new List<WildlifeRecommendation>();
		foreach ((string raceName, string legacyProfile) in AnimalAIStockTemplates.AnimalRecommendationsForTesting
			         .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
		{
			string profile = MapNormalProfile(raceName, legacyProfile);
			recommendations.Add(CreateRecommendation(raceName, false, profile, WildGroupFor(raceName, profile, false),
				profiles, ManagedProfileFor(raceName), ManagedGroupFor(raceName)));
		}

		foreach (MythicalAnimalSeeder.MythicalRaceTemplate template in MythicalAnimalSeeder.TemplatesForTesting.Values
			         .Where(x => x.WildlifeEligible)
			         .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
		{
			string legacy = AnimalAIStockTemplates.MythicalRecommendationsForTesting.TryGetValue(template.Name,
				out string? existing)
				? existing
				: AnimalAIStockTemplates.MythicGuardian;
			string profile = MapMythicalProfile(template.Name, legacy);
			recommendations.Add(CreateRecommendation(template.Name, true, profile, WildGroupFor(template.Name, profile, true),
				profiles, null, null));
		}

		var duplicate = recommendations.GroupBy(x => x.RaceName, StringComparer.OrdinalIgnoreCase)
			.FirstOrDefault(x => x.Count() > 1);
		if (duplicate is not null)
		{
			throw new InvalidOperationException($"Wildlife recommendation metadata contains duplicate race '{duplicate.Key}'.");
		}

		return recommendations.OrderBy(x => x.RaceName, StringComparer.OrdinalIgnoreCase).ToArray();
	}

	private static WildlifeRecommendation CreateRecommendation(string raceName, bool mythical, string individualProfile,
		string groupTemplate, IReadOnlyDictionary<string, WildlifeAnimalProfile> profiles, string? managedProfile,
		string? managedGroup)
	{
		WildlifeAnimalProfile profile = profiles[individualProfile];
		return new WildlifeRecommendation(
			raceName,
			mythical,
			individualProfile,
			groupTemplate,
			groupTemplate == "Solitary" ? "Solitary" : GroupSocialModel(groupTemplate),
			profile.HabitatDescription,
			profile.ToleratedHabitatProg.Replace("Wildlife AI - ", string.Empty),
			string.Join("; ", new[]
			{
				profile.Activity,
				profile.DormantSeason is null ? null : $"dormant in {profile.DormantSeason}",
				profile.NestingSeason is null ? null : $"nests in {profile.NestingSeason}",
				profile.SeasonalHabitatSeason is null ? null :
					$"prefers {profile.SeasonalHabitatProg?.Replace("Wildlife AI - ", string.Empty)} in {profile.SeasonalHabitatSeason}"
			}.OfType<string>()),
			profile.ShelterKey ?? "None",
			$"ordinary {profile.OrdinaryResponse}; attacked {profile.AttackedResponse}; territory {profile.TerritoryResponse}",
			profile.Senses,
			AuxiliaryAisFor(raceName, individualProfile),
			managedProfile,
			managedGroup);
	}

	private static string MapNormalProfile(string raceName, string legacyProfile)
	{
		if (legacyProfile == AnimalAIStockTemplates.SwimmingForager)
		{
			return raceName.In("Carp", "Koi", "Salmon") ? FreshwaterForager : MarineForager;
		}

		if (legacyProfile == AnimalAIStockTemplates.SwimmingPredator)
		{
			return raceName == "Perch" ? RiverinePredator : AquaticPredator;
		}

		if (legacyProfile == AnimalAIStockTemplates.AmphibiousPredator)
		{
			return RiverinePredator;
		}

		if (raceName == "Bear")
		{
			return HibernatingOmnivore;
		}

		return legacyProfile switch
		{
			var x when x == AnimalAIStockTemplates.SmallSkittishForager => TimidGroundForager,
			var x when x == AnimalAIStockTemplates.BurrowingForager => BurrowingForager,
			var x when x == AnimalAIStockTemplates.TerritorialGrazer => TerritorialGrazer,
			var x when x == AnimalAIStockTemplates.LargeDefensiveGrazer => DefensiveGrazer,
			var x when x == AnimalAIStockTemplates.OpportunistOmnivore => OpportunistOmnivore,
			var x when x == AnimalAIStockTemplates.HuntingOmnivore => OpportunistOmnivore,
			var x when x == AnimalAIStockTemplates.ShelteringGrazer => SeasonalGrazer,
			var x when x == AnimalAIStockTemplates.ParentalDefender => DefensiveGrazer,
			var x when x == AnimalAIStockTemplates.AmphibiousForager => AmphibiousForager,
			var x when x == AnimalAIStockTemplates.NocturnalScavenger => NocturnalScavenger,
			var x when x == AnimalAIStockTemplates.TerritorialPredator => GroundStalkingPredator,
			var x when x == AnimalAIStockTemplates.DenningPredator => DenningPredator,
			var x when x == AnimalAIStockTemplates.BurrowingAmbushPredator => AmbushPredator,
			var x when x == AnimalAIStockTemplates.ArborealForager => ArborealForager,
			var x when x == AnimalAIStockTemplates.ArborealPredator => ArborealPredator,
			var x when x == AnimalAIStockTemplates.SkittishBird => GroundFeedingBird,
			var x when x == AnimalAIStockTemplates.NestingBird => RoostingBird,
			var x when x == AnimalAIStockTemplates.Raptor => Raptor,
			var x when x == AnimalAIStockTemplates.EternalFlier => MigratoryFlier,
			var x when x == AnimalAIStockTemplates.FlyingScavenger => MigratoryFlier,
			var x when x == AnimalAIStockTemplates.SurfaceSwimmingPredator => SurfaceBreathingPredator,
			var x when x == AnimalAIStockTemplates.SwimmingScavenger => AquaticScavenger,
			_ => TimidGroundForager
		};
	}

	private static string MapMythicalProfile(string raceName, string legacyProfile)
	{
		if (raceName == "Giant Ant")
		{
			return ColonialInsect;
		}

		if (raceName == "Hippocamp")
		{
			return FreshwaterForager;
		}

		if (raceName == "Huorn")
		{
			return MythicGuardian;
		}

		return legacyProfile switch
		{
			var x when x == AnimalAIStockTemplates.MythicFlyingPredator => MythicAerialPredator,
			var x when x == AnimalAIStockTemplates.EternalFlier => MigratoryFlier,
			var x when x == AnimalAIStockTemplates.MythicGuardian => MythicGuardian,
			var x when x == AnimalAIStockTemplates.DenningPredator => DenningPredator,
			var x when x == AnimalAIStockTemplates.DenningOmnivore => OpportunistOmnivore,
			var x when x == AnimalAIStockTemplates.TerritorialPredator => GroundStalkingPredator,
			var x when x == AnimalAIStockTemplates.BurrowingAmbushPredator => AmbushPredator,
			var x when x == AnimalAIStockTemplates.AmphibiousPredator => RiverinePredator,
			var x when x == AnimalAIStockTemplates.LargeDefensiveGrazer => DefensiveGrazer,
			var x when x == AnimalAIStockTemplates.HuntingOmnivore => OpportunistOmnivore,
			var x when x == AnimalAIStockTemplates.SwimmingForager => FreshwaterForager,
			_ => MythicGuardian
		};
	}

	private static string WildGroupFor(string raceName, string profile, bool mythical)
	{
		if (mythical && profile == MythicAerialPredator)
		{
			return MythicAerialHuntingFlight;
		}

		if (raceName.In("Ant", "Bee", "Hornet", "Wasp", "Giant Ant"))
		{
			return InsectColonySwarm;
		}

		if (raceName.In("Rabbit", "Guinea Pig"))
		{
			return BurrowingColony;
		}

		if (raceName.In("Cow", "Ox", "Horse", "Donkey", "Mule", "Goat", "Sheep", "Llama", "Alpaca",
			    "Giraffe", "Elk", "Kangaroo", "Tapir"))
		{
			return TimidGrazingHerd;
		}

		if (raceName.In("Bison", "Buffalo", "Mammoth", "Moose", "Rhinocerous", "Warthog"))
		{
			return DefensiveGrazingHerd;
		}

		if (raceName.In("Camel", "Reindeer", "Deer", "Wallaby"))
		{
			return SeasonalGrazingHerd;
		}

		if (raceName.In("Elephant", "Hippopotamus", "Capybara", "Bunyip", "Yacumama"))
		{
			return ProtectiveFamilyHerd;
		}

		if (raceName.In("Wolf", "Coyote", "Dingo", "Dog", "Jackal", "Dire-Wolf", "Warg"))
		{
			return CursorialHuntingPack;
		}

		if (raceName.In("Badger", "Ferret", "Fox", "Mink", "Polecat", "Stoat", "Weasel", "Wolverine", "Dire-Bear"))
		{
			return DenningPredatorFamily;
		}

		if (raceName.In("Lion", "Panther"))
		{
			return TerritorialPride;
		}

		if (raceName.In("Hyena", "Vulture", "Condor", "Crow", "Raven"))
		{
			return ScavengerClan;
		}

		if (raceName.In("Goose", "Swan", "Duck", "Mandarin Duck", "Flamingo", "Pelican"))
		{
			return DefensiveWaterfowlFlock;
		}

		if (raceName.In("Chicken", "Turkey", "Pheasant", "Quail", "Grouse", "Kiwi", "Peacock", "Ostrich", "Emu", "Rhea",
			    "Crane", "Heron", "Ibis", "Stork", "Cassowary", "Moa"))
		{
			return GroundFeedingFlock;
		}

		if (raceName.In("Cockatoo", "Macaw", "Parrot", "Toucan", "Woodpecker", "Hoatzin", "Pigeon", "Finch", "Robin", "Sparrow",
			    "Wren", "Lyrebird"))
		{
			return ArborealRoostFlock;
		}

		if (raceName.In("Albatross", "Seagull", "Swallow", "Hummingbird", "Butterfly", "Dragonfly", "Moth", "Phoenix"))
		{
			return MigratoryFlight;
		}

		if (raceName.In("Eagle", "Falcon", "Hawk", "Owl", "Kingfisher", "Kookaburra"))
		{
			return RaptorFamily;
		}

		if (raceName.In("Carp", "Koi", "Salmon", "Perch"))
		{
			return FreshwaterSchool;
		}

		if (raceName.In("Anchovy", "Herring", "Mackerel", "Pilchard", "Sardine", "Baleen Whale"))
		{
			return MarineSchool;
		}

		if (raceName.In("Cod", "Haddock", "Pollock", "Tuna", "Shark", "Squid", "Giant Squid", "Octopus"))
		{
			return AquaticHuntingSchool;
		}

		if (raceName.In("Dolphin", "Orca", "Porpoise", "Sea Lion", "Seal", "Toothed Whale", "Walrus"))
		{
			return SurfaceBreathingPod;
		}

		if (raceName.In("Frog", "Toad", "Turtle", "Platypus"))
		{
			return AmphibiousColony;
		}

		return "Solitary";
	}

	private static string? ManagedProfileFor(string raceName)
	{
		if (!ManagedEligibleRaceNames.Contains(raceName, StringComparer.OrdinalIgnoreCase))
		{
			return null;
		}

		if (raceName == "Koi")
		{
			return ManagedOrnamentalAquatic;
		}

		if (raceName.In("Duck", "Goose"))
		{
			return ManagedWaterfowl;
		}

		if (raceName.In("Chicken", "Turkey", "Pigeon", "Quail", "Pheasant", "Peacock", "Ostrich", "Emu", "Rhea"))
		{
			return ManagedPoultry;
		}

		if (raceName.In("Cat", "Dog", "Ferret"))
		{
			return ManagedCompanionPredator;
		}

		if (raceName.In("Hamster", "Parrot", "Macaw", "Cockatoo"))
		{
			return ManagedCompanionForager;
		}

		return raceName == "Pig" ? ManagedLivestockOmnivore : ManagedLivestockGrazer;
	}

	private static string? ManagedGroupFor(string raceName)
	{
		if (!ManagedEligibleRaceNames.Contains(raceName, StringComparer.OrdinalIgnoreCase))
		{
			return null;
		}

		if (raceName.In("Chicken", "Turkey", "Pigeon", "Quail", "Pheasant", "Peacock", "Ostrich", "Emu", "Rhea", "Duck", "Goose"))
		{
			return ManagedPoultryWaterfowlFlock;
		}

		if (raceName.In("Cat", "Dog", "Ferret", "Hamster", "Parrot", "Macaw", "Cockatoo"))
		{
			return ManagedCompanionPack;
		}

		return ManagedLivestockHerd;
	}

	private static string GroupSocialModel(string groupTemplate)
	{
		return groupTemplate.Contains("Herd", StringComparison.OrdinalIgnoreCase) ? "Herd" :
			groupTemplate.Contains("Pack", StringComparison.OrdinalIgnoreCase) ? "Pack" :
			groupTemplate.Contains("Pride", StringComparison.OrdinalIgnoreCase) ? "Pride" :
			groupTemplate.Contains("Flock", StringComparison.OrdinalIgnoreCase) || groupTemplate.Contains("Flight", StringComparison.OrdinalIgnoreCase) ? "Flock" :
			groupTemplate.Contains("School", StringComparison.OrdinalIgnoreCase) ? "School" :
			groupTemplate.Contains("Pod", StringComparison.OrdinalIgnoreCase) ? "Pod" :
			groupTemplate.Contains("Colony", StringComparison.OrdinalIgnoreCase) || groupTemplate.Contains("Swarm", StringComparison.OrdinalIgnoreCase) ? "Colony" :
			groupTemplate.Contains("Family", StringComparison.OrdinalIgnoreCase) ? "Family" : "Managed";
	}

	private static IReadOnlyCollection<string> AuxiliaryAisFor(string raceName, string profile)
	{
		if (raceName.In("Spider", "Tarantula", "Giant Spider") || profile == AmbushPredator && raceName.Contains("Spider", StringComparison.OrdinalIgnoreCase))
		{
			return [WebAmbushAuxiliaryAi];
		}

		if (raceName.In("Scorpion", "Giant Scorpion", "Ankheg", "Giant Centipede", "Colossal Worm", "Giant Worm"))
		{
			return [BurrowAmbushAuxiliaryAi];
		}

		return Array.Empty<string>();
	}
}
