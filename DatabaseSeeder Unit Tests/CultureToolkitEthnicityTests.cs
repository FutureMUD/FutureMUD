#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitEthnicityTests
{
	[TestMethod]
	public void RetainedMembershipsDelegateReviewedNamesAndPreserveBuilderCharacteristicOverrides()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var structure = new NameCulture { Name = "Fixture retained structure", Definition = "<Definition />" };
		var characteristic = new CharacteristicDefinition();
		var firstProfile = new CharacteristicProfile { TargetDefinition = characteristic };
		var otherProfile = new CharacteristicProfile { TargetDefinition = characteristic };
		var suggestions = new FutureProg { FunctionName = "AlwaysTrue" };
		context.AddRange(structure, firstProfile, otherProfile, suggestions);
		context.SaveChanges();
		var source = new Ethnicity { Name = "Fixture source", ChargenBlurb = "Fixture source description" };
		source.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics { CharacteristicDefinitionId = characteristic.Id, CharacteristicProfileId = firstProfile.Id });
		var desired = new Ethnicity { Name = "Finnish", ChargenBlurb = "Supplied overlay prose" };
		var names = Enum.GetValues<Gender>().ToDictionary(x => (short)x, _ => structure.Id);
		CultureEthnicityDefinition[] definitions = [new("ethnicity.finnish", source, desired, names,
			new("fixture.source", "ethnicity.finnish", "fixture", ["finnish"], [1L], []))];
		var conflicts = new List<string>();
		var first = CultureToolkitEthnicities.Upsert(context, "earlymodern", definitions, new Dictionary<string, Ethnicity>(), conflicts);
		var ethnicity = first.Ethnicities["ethnicity.finnish"];
		CultureToolkitNameSeeder.Upsert(context, new CultureToolkitCatalogue(), "earlymodern", first.Ethnicities, suggestions, conflicts, first.OriginalNameCultures);
		var reviewedMale = ethnicity.EthnicitiesNameCultures.Single(x => x.Gender == (short)Gender.Male).NameCultureId;
		Assert.AreNotEqual(structure.Id, reviewedMale);
		context.Remove(ethnicity.EthnicitiesCharacteristics.Single());
		context.SaveChanges();
		ethnicity.EthnicitiesCharacteristics.Add(new EthnicitiesCharacteristics { CharacteristicDefinitionId = characteristic.Id, CharacteristicProfileId = otherProfile.Id });
		context.SaveChanges();
		conflicts.Clear();
		var second = CultureToolkitEthnicities.Upsert(context, "earlymodern", definitions, new Dictionary<string, Ethnicity>(), conflicts);
		Assert.AreEqual(reviewedMale, ethnicity.EthnicitiesNameCultures.Single(x => x.Gender == (short)Gender.Male).NameCultureId);
		Assert.AreEqual(otherProfile.Id, ethnicity.EthnicitiesCharacteristics.Single().CharacteristicProfileId);
		Assert.AreEqual(1, conflicts.Count);
		StringAssert.Contains(conflicts[0], "characteristic:");
		var female = ethnicity.EthnicitiesNameCultures.Single(x => x.Gender == (short)Gender.Female);
		context.Remove(female);
		context.SaveChanges();
		CultureToolkitNameSeeder.Upsert(context, new CultureToolkitCatalogue(), "earlymodern", second.Ethnicities, suggestions, conflicts, second.OriginalNameCultures);
		Assert.IsFalse(ethnicity.EthnicitiesNameCultures.Any(x => x.Gender == (short)Gender.Female));
	}
}
