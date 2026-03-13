#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HealthSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void ValidateDefaultSurgeryTargetAliasesForTesting_CurrentTargets_HasNoIssues()
	{
		var issues = HealthSeeder.ValidateDefaultSurgeryTargetAliasesForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void BuildTargetDefinition_OrganicHumanoid_ResolvesInheritedAndLocalBodyparts()
	{
		using var context = BuildContext();
		var humanoidBody = new BodyProto
		{
			Id = 1,
			Name = "Humanoid"
		};
		var organicHumanoidBody = new BodyProto
		{
			Id = 2,
			Name = "Organic Humanoid",
			CountsAs = humanoidBody,
			CountsAsId = humanoidBody.Id
		};

		context.BodyProtos.AddRange(humanoidBody, organicHumanoidBody);
		context.BodypartProtos.AddRange(
			new BodypartProto
			{
				Id = 10,
				Body = humanoidBody,
				BodyId = humanoidBody.Id,
				Name = "lupperarm",
				Description = "left upper arm"
			},
			new BodypartProto
			{
				Id = 11,
				Body = organicHumanoidBody,
				BodyId = organicHumanoidBody.Id,
				Name = "brain",
				Description = "brain"
			});

		var definition = HealthSeeder.BuildTargetDefinition(context, organicHumanoidBody, "lupperarm", "brain");
		var targetIds = XElement.Parse(definition)
			.Element("Parts")!
			.Elements("Part")
			.Select(x => long.Parse(x.Value))
			.ToArray();

		CollectionAssert.AreEqual(new long[] { 10, 11 }, targetIds);
	}
}
