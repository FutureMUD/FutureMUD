#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using MudSharp.Community.Templates;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ClanSeederPrivilegeTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void AddressUpgrade_ExistingStockProfiles_AddsMissingFieldsOnceAndPreservesBuilderContent()
	{
		using var context = BuildContext(); SeedPrerequisites(context);
		var seeder = new ClanSeeder();
		seeder.SeedData(context, new Dictionary<string, string>());
		var profile = context.FutureProgs.First(x => x.Subcategory == "Template Profiles" && x.FunctionComment.Contains("catholic."));
		const string prefix = "ClanTemplateProfile_v1\n";
		var data = JsonNode.Parse(profile.FunctionComment[prefix.Length..])!;
		var clan = context.Clans.Find(data["ClanId"]!.GetValue<long>())!;
		var addresses = clan.Ranks.SelectMany(x => x.FormsOfAddress).ToList();
		Assert.IsNotEmpty(addresses);
		var custom = addresses[0]; custom.Text = "Builder's chosen address";
		foreach (var row in addresses.Where(x => x != custom)) context.RankFormsOfAddress.Remove(row);
		foreach (var rank in clan.Ranks)
			foreach (var row in rank.FormsOfAddress.Where(x => x != custom).ToList()) rank.FormsOfAddress.Remove(row);
		foreach (var rank in data["Definition"]!["ranks"]!.AsArray()) rank!.AsObject().Remove("FormsOfAddress");
		var recipeRank = clan.Ranks.First(x => x.FormsOfAddress.Count == 0);
		var recipeKey = data["Ranks"]!.AsObject().Single(x => x.Value!.GetValue<long>() == recipeRank.Id).Key;
		var recipeRecord = data["Definition"]!["ranks"]!.AsArray().Single(x => x!["key"]!.GetValue<string>() == recipeKey)!;
		data["Definition"]!["live_bindings"]!.AsArray().Add(JsonSerializer.SerializeToNode(new ClanTemplateBindingDefinition { Role = "custom_courtesy" }));
		recipeRecord["FormsOfAddress"] = JsonSerializer.SerializeToNode(new[] { new ClanTemplateAddressRule { Key = "bespoke", Text = "Builder profile style", IsPublic = true,
			Conditions = [new() { Role = "custom_courtesy", Kind = "membership" }] } });
		var priorityOffice = clan.Appointments.First(x => x.FormsOfAddress.Count == 0);
		var priorityKey = data["Appointments"]!.AsObject().Single(x => x.Value!.GetValue<long>() == priorityOffice.Id).Key;
		data["Definition"]!["appointments"]!.AsArray().Single(x => x!["key"]!.GetValue<string>() == priorityKey)!["AddressPriority"] = 73;
		data.AsObject().Remove("AddressContentVersion");
		data["Definition"]!["Description"] = "Keep the builder's profile prose";
		clan.Description = "Keep the builder's clan prose";
		var office = clan.Appointments.First(); office.MaximumSimultaneousHolders = 19;
		profile.FunctionComment = prefix + data.ToJsonString(); context.SaveChanges();
		Assert.AreEqual(ShouldSeedResult.ExtraPackagesAvailable, seeder.ShouldSeedData(context));
		seeder.SeedData(context, new Dictionary<string, string>());
		Assert.AreEqual("Builder's chosen address", context.RankFormsOfAddress.Find(custom.Id)!.Text);
		Assert.AreEqual("Keep the builder's clan prose", clan.Description);
		Assert.AreEqual(19, office.MaximumSimultaneousHolders);
		var updated = JsonNode.Parse(profile.FunctionComment[prefix.Length..])!;
		Assert.AreEqual(1, updated["AddressContentVersion"]!.GetValue<int>());
		Assert.AreEqual("Builder profile style", updated["Definition"]!["ranks"]!.AsArray().Single(x => x!["key"]!.GetValue<string>() == recipeKey)!["FormsOfAddress"]![0]!["Text"]!.GetValue<string>());
		Assert.AreEqual("Builder profile style", recipeRank.FormsOfAddress.Single().Text);
		Assert.AreEqual(73, updated["Definition"]!["appointments"]!.AsArray().Single(x => x!["key"]!.GetValue<string>() == priorityKey)!["AddressPriority"]!.GetValue<int>());
		Assert.AreEqual(73, priorityOffice.AddressPriority);
		Assert.AreEqual("Keep the builder's profile prose", updated["Definition"]!["Description"]!.GetValue<string>());
		Assert.IsTrue(clan.Ranks.Any(x => x.FormsOfAddress.Any(y => y.Id != custom.Id)));
		var ids = context.RankFormsOfAddress.Select(x => x.Id).ToList();
		seeder.SeedData(context, new Dictionary<string, string>());
		CollectionAssert.AreEquivalent(ids, context.RankFormsOfAddress.Select(x => x.Id).ToList());
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
	}
}
