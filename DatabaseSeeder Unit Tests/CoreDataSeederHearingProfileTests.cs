#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederHearingProfileTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void SeedDefaultHearingProfiles_SeedsRequestedSimpleAndCompositeProfiles()
	{
		using FuturemudDatabaseContext context = BuildContext();

		CoreDataSeeder.SeedDefaultHearingProfiles(context);

		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Universal" && x.Type == "Simple"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Near Waterfall" && x.Type == "Simple"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Near River" && x.Type == "Simple"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Near Ocean" && x.Type == "Simple"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Near Industrial Machinery" && x.Type == "Simple"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Commercial Workday Cycle" && x.Type == "TimeOfDay"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Entertainment Weekday Cycle" && x.Type == "TimeOfDay"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Commercial District" && x.Type == "WeekdayTimeOfDay"));
		Assert.IsTrue(context.HearingProfiles.Any(x => x.Name == "Entertainment District" && x.Type == "WeekdayTimeOfDay"));
	}

	[TestMethod]
	public void SeedDefaultHearingProfiles_CompositeProfilesReferenceExpectedChildren()
	{
		using FuturemudDatabaseContext context = BuildContext();

		CoreDataSeeder.SeedDefaultHearingProfiles(context);

		HearingProfile commercialDistrict = context.HearingProfiles.Single(x => x.Name == "Commercial District");
		HearingProfile entertainmentDistrict = context.HearingProfiles.Single(x => x.Name == "Entertainment District");

		List<string> commercialChildren = ResolveReferencedProfileNames(context, commercialDistrict);
		List<string> entertainmentChildren = ResolveReferencedProfileNames(context, entertainmentDistrict);

		CollectionAssert.AreEquivalent(
			new[] { "Commercial Workday Cycle", "Commercial Weekend Cycle" },
			commercialChildren);
		CollectionAssert.AreEquivalent(
			new[] { "Entertainment Weekday Cycle", "Entertainment Weekend Cycle" },
			entertainmentChildren);
	}

	private static List<string> ResolveReferencedProfileNames(FuturemudDatabaseContext context, HearingProfile profile)
	{
		XElement definition = XElement.Parse(profile.Definition);
		return definition
			.Descendants()
			.Attributes("ProfileID")
			.Select(x => long.Parse(x.Value))
			.Select(id => context.HearingProfiles.Single(x => x.Id == id).Name)
			.ToList();
	}
}
