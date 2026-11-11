#nullable enable

using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ClanSeederPrivilegeTests
{
	[TestMethod]
	[TestCategory("Integration")]
	public void RomanMilitary_BeneficiariusRanks_LinkToDedicatedPaygrade()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		new ClanSeeder().SeedData(context, new Dictionary<string, string>());
		var legion = context.Clans.Single(x => x.Name == "Roman Legion Template");
		var paygrade = legion.Paygrades.Single(x => x.Name == "Caligatius Beneficiarius");
		// The legacy misspelling silently left both staff ranks without their dedicated pay option.
		// Exercise that persisted relationship rather than snapshotting the title/address wording.
		foreach (var name in new[] { "Beneficiarius Cohortis", "Beneficiarius Alae" })
		{
			var rank = legion.Ranks.Single(x => x.Name == name);
			Assert.IsTrue(rank.RanksPaygrades.Any(x => x.PaygradeId == paygrade.Id),
				$"{name} must be eligible for its dedicated staff paygrade.");
		}
	}

	[TestMethod]
	[TestCategory("Integration")]
	public void OtherAddresses_FreshInstallAndRerun_PreserveEditsClearedDefaultsAndExistingIdentities()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		// An unowned, pre-existing stock-name identity must not be adopted or repaired.
		var existing = new Clan { Name = "Peerage Template", Alias = "peerage", FullName = "Builder's Peerage",
			IsTemplate = true, Description = "Keep this", PayIntervalReferenceDate = "1-1-1", PayIntervalReferenceTime = "UTC 0:0:0" };
		context.Clans.Add(existing);
		context.SaveChanges();
		var seeder = new ClanSeeder();
		seeder.SeedData(context, new Dictionary<string, string>());
		var otherClans = context.Clans.Where(x => x.Id != existing.Id)
			.AsEnumerable().Where(x => !context.FutureProgs.Any(p => p.FunctionName == $"ClanTemplate_{x.Id}_Profile")).ToList();
		var rows = otherClans.SelectMany(x => x.Ranks).SelectMany(x => x.FormsOfAddress).ToList();
		var offices = otherClans.SelectMany(x => x.Appointments).Where(x => x.FormsOfAddress.Count > 0).ToList();
		Assert.IsNotEmpty(rows);
		Assert.IsNotEmpty(offices);
		Assert.IsEmpty(existing.Ranks);
		Assert.IsEmpty(existing.Appointments);
		Assert.AreEqual("Keep this", existing.Description);
		var customised = rows.First(); customised.Text = "A deliberately custom address";
		var cleared = rows.First(x => x.RankId != customised.RankId).Rank;
		context.RankFormsOfAddress.RemoveRange(cleared.FormsOfAddress);
		cleared.FormsOfAddress.Clear();
		var office = offices.First(); office.AddressPriority = 741;
		context.SaveChanges();
		var rankIds = context.RankFormsOfAddress.Select(x => x.Id).ToList();
		var officeIds = context.AppointmentFormsOfAddress.Select(x => x.Id).ToList();
		seeder.SeedData(context, new Dictionary<string, string>());
		CollectionAssert.AreEquivalent(rankIds, context.RankFormsOfAddress.Select(x => x.Id).ToList());
		CollectionAssert.AreEquivalent(officeIds, context.AppointmentFormsOfAddress.Select(x => x.Id).ToList());
		Assert.AreEqual("A deliberately custom address", context.RankFormsOfAddress.Find(customised.Id)!.Text);
		Assert.IsEmpty(cleared.FormsOfAddress);
		Assert.AreEqual(741, context.Appointments.Find(office.Id)!.AddressPriority);
		Assert.IsEmpty(existing.Ranks);
		Assert.IsEmpty(existing.Appointments);
	}
}
