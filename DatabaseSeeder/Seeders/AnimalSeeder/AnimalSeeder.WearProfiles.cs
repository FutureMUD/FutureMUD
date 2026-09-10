#nullable enable

using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
	private sealed record StockAnimalWearProfileLocation(
		string Location,
		bool Mandatory,
		bool NoArmour,
		bool Transparent,
		bool PreventsRemoval,
		bool HidesSevered);

	private sealed record StockAnimalWearProfileDefinition(
		string Name,
		string WearStringInventory,
		string WearAction1st,
		string WearAction3rd,
		string WearAffix,
		string Description,
		bool RequireContainerIsEmpty,
		bool Bulky,
		IReadOnlyList<StockAnimalWearProfileLocation> Locations);

	private static StockAnimalWearProfileLocation AnimalLoc(string location, bool mandatory, bool noArmour,
		bool transparent, bool preventsRemoval, bool hidesSevered)
	{
		return new StockAnimalWearProfileLocation(location, mandatory, noArmour, transparent, preventsRemoval,
			hidesSevered);
	}

	private static readonly StockAnimalWearProfileDefinition[] StockUngulateWearProfiles =
	[
		new("Saddle", "worn on", "settle", "settles", "on",
			"Worn as a riding or pack saddle over the withers and back", false, true,
			[
				AnimalLoc("withers", true, true, false, true, false),
				AnimalLoc("uback", true, true, false, true, false),
				AnimalLoc("lback", true, true, false, true, false),
				AnimalLoc("rloin", false, true, false, true, false),
				AnimalLoc("lloin", false, true, false, true, false),
				AnimalLoc("urflank", false, true, false, true, false),
				AnimalLoc("ulflank", false, true, false, true, false)
			]),
		new("Bridle", "worn on", "fit", "fits", "on",
			"Worn as a bridle around the head, muzzle, cheeks and ears", false, false,
			[
				AnimalLoc("head", true, true, true, true, false),
				AnimalLoc("bhead", true, true, true, true, false),
				AnimalLoc("muzzle", true, true, true, true, false),
				AnimalLoc("rcheek", true, true, true, true, false),
				AnimalLoc("lcheek", true, true, true, true, false),
				AnimalLoc("rear", false, true, true, true, false),
				AnimalLoc("lear", false, true, true, true, false),
				AnimalLoc("rjaw", false, true, true, true, false),
				AnimalLoc("ljaw", false, true, true, true, false),
				AnimalLoc("nose", false, true, true, true, false)
			]),
		new("Bit", "worn in", "place", "places", "in",
			"Worn as a bit seated in the mouth", false, false,
			[
				AnimalLoc("mouth", true, true, true, true, false),
				AnimalLoc("tongue", false, true, true, true, false)
			]),
		new("Chanfron", "worn on", "fasten", "fastens", "on",
			"Worn as armour over the face and upper head", false, true,
			[
				AnimalLoc("head", true, false, false, true, false),
				AnimalLoc("bhead", true, false, false, true, false),
				AnimalLoc("muzzle", true, false, false, true, false),
				AnimalLoc("rcheek", true, false, false, true, false),
				AnimalLoc("lcheek", true, false, false, true, false),
				AnimalLoc("reyesocket", false, false, true, true, false),
				AnimalLoc("leyesocket", false, false, true, true, false),
				AnimalLoc("nose", false, false, false, true, false)
			]),
		new("Criniere", "worn on", "fasten", "fastens", "on",
			"Worn as segmented armour along the neck", false, true,
			[
				AnimalLoc("neck", true, false, false, true, false),
				AnimalLoc("bneck", true, false, false, true, false),
				AnimalLoc("throat", false, false, false, true, false),
				AnimalLoc("withers", false, false, false, true, false)
			]),
		new("Croupiere", "worn on", "fasten", "fastens", "on",
			"Worn as armour over the rump and base of the tail", false, true,
			[
				AnimalLoc("rrump", true, false, false, true, false),
				AnimalLoc("lrump", true, false, false, true, false),
				AnimalLoc("utail", false, false, false, true, false),
				AnimalLoc("mtail", false, false, false, true, false),
				AnimalLoc("lback", false, false, false, true, false)
			]),
		new("Flanchards", "worn on", "fasten", "fastens", "on",
			"Worn as armour over the flanks and belly", false, true,
			[
				AnimalLoc("urflank", true, false, false, true, false),
				AnimalLoc("ulflank", true, false, false, true, false),
				AnimalLoc("lrflank", true, false, false, true, false),
				AnimalLoc("llflank", true, false, false, true, false),
				AnimalLoc("belly", false, false, false, true, false)
			]),
		new("Peytral", "worn on", "fasten", "fastens", "on",
			"Worn as armour over the chest and shoulders", false, true,
			[
				AnimalLoc("rbreast", true, false, false, true, false),
				AnimalLoc("lbreast", true, false, false, true, false),
				AnimalLoc("rshoulder", true, false, false, true, false),
				AnimalLoc("lshoulder", true, false, false, true, false),
				AnimalLoc("withers", false, false, false, true, false)
			]),
		new("Caparison", "worn over", "drape", "drapes", "over",
			"Worn as a cloth or padded covering over the body and flanks", false, true,
			[
				AnimalLoc("withers", true, true, false, true, false),
				AnimalLoc("uback", true, true, false, true, false),
				AnimalLoc("lback", true, true, false, true, false),
				AnimalLoc("rloin", true, true, false, true, false),
				AnimalLoc("lloin", true, true, false, true, false),
				AnimalLoc("rrump", false, true, false, true, false),
				AnimalLoc("lrump", false, true, false, true, false),
				AnimalLoc("urflank", true, true, false, true, false),
				AnimalLoc("ulflank", true, true, false, true, false),
				AnimalLoc("lrflank", false, true, false, true, false),
				AnimalLoc("llflank", false, true, false, true, false),
				AnimalLoc("rshoulder", false, true, false, true, false),
				AnimalLoc("lshoulder", false, true, false, true, false)
			])
	];

	internal static string StockUngulateWearBodyNameForTesting => "Ungulate";

	internal static IReadOnlyList<string> StockUngulateWearProfileNamesForTesting =>
		StockUngulateWearProfiles.Select(x => x.Name).ToArray();

	internal static IReadOnlyList<string> StockUngulateWearComponentNamesForTesting =>
		StockUngulateWearProfiles.Select(x => AnimalWearComponentName(x.Name)).ToArray();

	internal static IReadOnlyList<(string Name, IReadOnlyList<string> Locations)>
		StockUngulateWearProfileDefinitionsForTesting =>
		StockUngulateWearProfiles
			.Select(x => (x.Name, (IReadOnlyList<string>)x.Locations.Select(y => y.Location).ToArray()))
			.ToArray();

	internal static bool SeedStockUngulateWearProfilesForTesting(FuturemudDatabaseContext context)
	{
		AnimalSeeder seeder = new()
		{
			_context = context
		};
		BodyProto? ungulateBody = context.BodyProtos.FirstOrDefault(x => x.Name == StockUngulateWearBodyNameForTesting);
		return ungulateBody is not null && seeder.EnsureStockUngulateWearProfiles(ungulateBody);
	}

	private static string AnimalWearComponentName(string profileName)
	{
		return $"Wear_{profileName.Replace(' ', '_')}";
	}

	private bool EnsureStockUngulateWearProfiles()
	{
		BodyProto? ungulateBody = _context.BodyProtos.FirstOrDefault(x => x.Name == StockUngulateWearBodyNameForTesting);
		return ungulateBody is not null && EnsureStockUngulateWearProfiles(ungulateBody);
	}

	private bool EnsureStockUngulateWearProfiles(BodyProto ungulateBody)
	{
		Dictionary<string, WearProfile> profiles = _context.WearProfiles
		                                                   .Where(x => x.BodyPrototypeId == ungulateBody.Id)
		                                                   .AsEnumerable()
		                                                   .Where(x => !string.IsNullOrWhiteSpace(x.Name))
		                                                   .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
		                                                   .ToDictionary(x => x.Key, x => x.First(),
			                                                   StringComparer.OrdinalIgnoreCase);
		long nextWearProfileId = _context.WearProfiles
		                                 .Select(x => x.Id)
		                                 .AsEnumerable()
		                                 .DefaultIfEmpty(0L)
		                                 .Max() + 1;
		bool dirty = false;

		foreach (StockAnimalWearProfileDefinition definition in StockUngulateWearProfiles)
		{
			string wearlocXml = BuildAnimalWearlocProfileXml(definition);
			if (profiles.TryGetValue(definition.Name, out WearProfile? profile))
			{
				dirty |= UpdateAnimalWearProfile(profile, definition, wearlocXml);
				continue;
			}

			profile = new WearProfile
			{
				Id = nextWearProfileId++,
				BodyPrototypeId = ungulateBody.Id,
				Name = definition.Name,
				WearStringInventory = definition.WearStringInventory,
				WearAction1st = definition.WearAction1st,
				WearAction3rd = definition.WearAction3rd,
				WearAffix = definition.WearAffix,
				Description = definition.Description,
				Type = "Direct",
				RequireContainerIsEmpty = definition.RequireContainerIsEmpty,
				WearlocProfiles = wearlocXml
			};
			_context.WearProfiles.Add(profile);
			profiles[profile.Name] = profile;
			dirty = true;
		}

		Account? dbaccount = _context.Accounts.FirstOrDefault();
		if (dbaccount is null)
		{
			if (dirty)
			{
				_context.SaveChanges();
			}

			return dirty;
		}

		Dictionary<string, GameItemComponentProto> components = _context.GameItemComponentProtos
		                                                                 .AsEnumerable()
		                                                                 .Where(x => !string.IsNullOrWhiteSpace(x.Name))
		                                                                 .GroupBy(x => x.Name,
			                                                                 StringComparer.OrdinalIgnoreCase)
		                                                                 .ToDictionary(x => x.Key, x => x.First(),
			                                                                 StringComparer.OrdinalIgnoreCase);
		long nextComponentId = _context.GameItemComponentProtos
		                               .Select(x => x.Id)
		                               .AsEnumerable()
		                               .DefaultIfEmpty(0L)
		                               .Max() + 1;
		DateTime now = DateTime.UtcNow;

		foreach (StockAnimalWearProfileDefinition definition in StockUngulateWearProfiles)
		{
			if (!profiles.TryGetValue(definition.Name, out WearProfile? profile))
			{
				continue;
			}

			string componentName = AnimalWearComponentName(definition.Name);
			string componentDefinition = BuildAnimalWearableComponentDefinition(profile, definition.Bulky);
			string description = $"Permits the item to be worn in the {definition.Name} wear configuration";
			if (components.TryGetValue(componentName, out GameItemComponentProto? component))
			{
				if (!string.Equals(component.Type, "Wearable", StringComparison.Ordinal) ||
				    component.Description != description ||
				    !AnimalWearXmlEquivalent(component.Definition, componentDefinition))
				{
					component.Type = "Wearable";
					component.Description = description;
					component.Definition = componentDefinition;
					dirty = true;
				}

				continue;
			}

			component = new GameItemComponentProto
			{
				Id = nextComponentId++,
				RevisionNumber = 0,
				Name = componentName,
				Description = description,
				Type = "Wearable",
				Definition = componentDefinition,
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = 4,
					BuilderAccountId = dbaccount.Id,
					BuilderDate = now,
					BuilderComment = "Auto-generated by the system",
					ReviewerAccountId = dbaccount.Id,
					ReviewerComment = "Auto-generated by the system",
					ReviewerDate = now
				}
			};
			_context.GameItemComponentProtos.Add(component);
			components[component.Name] = component;
			dirty = true;
		}

		if (dirty)
		{
			_context.SaveChanges();
		}

		return dirty;
	}

	private static bool UpdateAnimalWearProfile(WearProfile profile, StockAnimalWearProfileDefinition definition,
		string wearlocXml)
	{
		bool dirty = false;
		if (profile.WearStringInventory != definition.WearStringInventory)
		{
			profile.WearStringInventory = definition.WearStringInventory;
			dirty = true;
		}

		if (profile.WearAction1st != definition.WearAction1st)
		{
			profile.WearAction1st = definition.WearAction1st;
			dirty = true;
		}

		if (profile.WearAction3rd != definition.WearAction3rd)
		{
			profile.WearAction3rd = definition.WearAction3rd;
			dirty = true;
		}

		if (profile.WearAffix != definition.WearAffix)
		{
			profile.WearAffix = definition.WearAffix;
			dirty = true;
		}

		if (profile.Description != definition.Description)
		{
			profile.Description = definition.Description;
			dirty = true;
		}

		if (!string.Equals(profile.Type, "Direct", StringComparison.Ordinal))
		{
			profile.Type = "Direct";
			dirty = true;
		}

		if (profile.RequireContainerIsEmpty != definition.RequireContainerIsEmpty)
		{
			profile.RequireContainerIsEmpty = definition.RequireContainerIsEmpty;
			dirty = true;
		}

		if (!AnimalWearXmlEquivalent(profile.WearlocProfiles, wearlocXml))
		{
			profile.WearlocProfiles = wearlocXml;
			dirty = true;
		}

		return dirty;
	}

	private static string BuildAnimalWearlocProfileXml(StockAnimalWearProfileDefinition definition)
	{
		return new XElement("Profiles",
			from location in definition.Locations
			select new XElement("Profile",
				new XAttribute("Bodypart", location.Location),
				new XAttribute("Transparent", location.Transparent),
				new XAttribute("NoArmour", location.NoArmour),
				new XAttribute("PreventsRemoval", location.PreventsRemoval),
				new XAttribute("Mandatory", location.Mandatory),
				new XAttribute("HidesSevered", location.HidesSevered))
		).ToString();
	}

	private static string BuildAnimalWearableComponentDefinition(WearProfile defaultProfile, bool bulky)
	{
		return new XElement("Definition",
			new XAttribute("DisplayInventoryWhenWorn", true),
			new XAttribute("Bulky", bulky),
			new XElement("Profiles",
				new XAttribute("Default", defaultProfile.Id),
				new XElement("Profile", defaultProfile.Id))
		).ToString();
	}

	private static bool AnimalWearXmlEquivalent(string? lhs, string rhs)
	{
		if (string.IsNullOrWhiteSpace(lhs))
		{
			return false;
		}

		try
		{
			return XNode.DeepEquals(XElement.Parse(lhs), XElement.Parse(rhs));
		}
		catch
		{
			return false;
		}
	}

	private static bool HasMissingAnimalWearProfiles(FuturemudDatabaseContext context)
	{
		BodyProto? ungulateBody = context.BodyProtos.FirstOrDefault(x => x.Name == StockUngulateWearBodyNameForTesting);
		if (ungulateBody is null)
		{
			return false;
		}

		HashSet<string> existingProfileNames = context.WearProfiles
		                                              .Where(x => x.BodyPrototypeId == ungulateBody.Id)
		                                              .Select(x => x.Name)
		                                              .AsEnumerable()
		                                              .ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (StockUngulateWearProfiles.Any(x => !existingProfileNames.Contains(x.Name)))
		{
			return true;
		}

		HashSet<string> existingComponentNames = context.GameItemComponentProtos
		                                                .Select(x => x.Name)
		                                                .AsEnumerable()
		                                                .ToHashSet(StringComparer.OrdinalIgnoreCase);
		return StockUngulateWearProfiles
		       .Select(x => AnimalWearComponentName(x.Name))
		       .Any(x => !existingComponentNames.Contains(x));
	}
}
