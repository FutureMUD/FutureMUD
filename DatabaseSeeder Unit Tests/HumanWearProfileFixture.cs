#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems.Inventory;
using MudSharp.Models;
using WearProfile = MudSharp.Models.WearProfile;
using Match = System.Text.RegularExpressions.Match;

namespace MudSharp_Unit_Tests;

internal sealed class HumanWearProfileFixture : IDisposable
{
	internal static readonly string[] ExpectedProfiles =
	[
		"Split Drawers", "Camisole", "Nappy", "Infant Bodysuit", "Infant Gown", "Short Stays", "Rear Skirt Support",
		"Girdle", "Stocking Support Belt", "High-Neck Shirt", "Bib Overalls", "Trained Skirt", "Cutaway Coat", "Tailcoat",
		"Hooded Long Coat", "Hooded Long Coat Lowered", "Detachable Collar", "Standing Collar", "Detachable Cuffs", "Shirtfront", "Braces", "Rank Slides",
		"Ribbon Bar", "Long Gloves", "Bonnet", "Hairnet", "Net Skirt", "Waist Apron", "Open-Back Gown", "High-Neck Jacket", "Garters",
		"Cutaway Jacket", "Low-Cut Shoes", "Backless Shoes", "Infant Footwear", "High-Neck Dress", "Short Wrap Jacket",
		"High-Waisted Skirt", "Wide Over-Robe", "Clerical Collar", "Academic Hood", "Nappy Pins", "Spats"
	];

	internal static string ComponentName(string profile) => $"Wear_{profile.Replace(' ', '_')}";

	internal FuturemudDatabaseContext Context { get; } = new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
	internal HumanWearProfileFixture(bool explicitBones = false, bool inventoryHands = true, bool seedBaseline = true)
	{
		Context.Accounts.Add(new Account { Id = 1, Name = "wear-profile-tests", CultureName = "en-AU", TimeZoneId = "UTC", UnitPreference = "metric" });
		Context.BodyProtos.Add(new BodyProto
		{
			Id = 1, Name = "Humanoid", ConsiderString = "a humanoid", LegDescriptionPlural = "legs", LegDescriptionSingular = "leg",
			WielderDescriptionPlural = "hands", WielderDescriptionSingle = "hand"
		});
		// Read the actual source anatomy/shape/type vocabulary; this fixture proves geometry,
		// not race/gender sizing or a complete HumanSeeder fresh-world installation.
		var source = SeederSourceTestHelper.ReadPartialFamily("HumanSeeder");
		var matches = Regex.Matches(source, "CreateBodypart\\(baseHumanoid,\\s*\"(?<name>[^\"]+)\",\\s*\"[^\"]+\",\\s*\"(?<shape>[^\"]+)\",\\s*(?<type>[^,]+),")
			.Cast<Match>().GroupBy(x => x.Groups["name"].Value).Select(x => x.First()).ToArray();
		Assert.IsTrue(matches.Length >= 80);
		var shapes = matches.Select(x => x.Groups["shape"].Value).Distinct().Select((name, index) => new BodypartShape { Id = index + 1, Name = name }).ToArray();
		Context.BodypartShapes.AddRange(shapes);
		Context.BodypartProtos.AddRange(matches.Select((match, index) => new BodypartProto
		{
			Id = index + 1, BodyId = 1, Name = match.Groups["name"].Value, Description = match.Groups["name"].Value,
			BodypartShapeId = shapes.Single(x => x.Name == match.Groups["shape"].Value).Id,
			BodypartType = (int)(Regex.Replace(match.Groups["type"].Value, @"\s+", "") switch
			{
				"drapeableType" => explicitBones ? BodypartTypeEnum.Wear : BodypartTypeEnum.BonyDrapeable,
				"niDrapeableType" => explicitBones ? BodypartTypeEnum.Wear : BodypartTypeEnum.NonImmobilisingBonyDrapeable,
				"gwType" => explicitBones ? BodypartTypeEnum.GrabbingWielding : BodypartTypeEnum.BonyGrabbingWielding,
				"_questionAnswers[\"inventory\"].ToLowerInvariant().Equals(\"hands\")?gwType:BodypartTypeEnum.Wielding" =>
					!inventoryHands ? BodypartTypeEnum.Wielding : explicitBones ? BodypartTypeEnum.GrabbingWielding : BodypartTypeEnum.BonyGrabbingWielding,
				var type when type.StartsWith("BodypartTypeEnum.", StringComparison.Ordinal) => Enum.Parse<BodypartTypeEnum>(type.Split('.')[1]),
				var type => throw new InvalidDataException($"Unrecognised stock bodypart type expression: {type}")
			})
		}));
		Context.SaveChanges();
		// Use the actual fresh-install baseline writer, not empty placeholder profiles. Additional
		// stock is installed by the same refresh path exercised by each test below.
		if (seedBaseline) HumanSeeder.SeedHumanWearProfilesForTesting(Context, includeAdditional: false);
	}
	internal string[] Signatures() => Context.WearProfiles.OrderBy(x => x.Id).AsEnumerable()
		.Select(x => $"{x.Id}|{x.Name}|{x.Type}|{x.Description}|{x.WearlocProfiles}")
		.Concat(Context.GameItemComponentProtos.OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber).AsEnumerable()
			.Select(x => $"{x.Id}:{x.RevisionNumber}|{x.Name}|{x.Type}|{x.Description}|{x.Definition}")).ToArray();
	public void Dispose() => Context.Dispose();
}
