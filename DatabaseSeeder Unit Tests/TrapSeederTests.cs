#nullable enable

using DatabaseSeeder.Seeders;
using DatabaseSeeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ProgVariableTypes = MudSharp.FutureProg.ProgVariableTypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TrapSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static void SeedPrerequisites(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "TrapSeeder",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "traps@example.com",
			LastLoginIp = "127.0.0.1",
			CultureName = "en-AU",
			TimeZoneId = "UTC",
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow
		});
		context.CheckTemplates.Add(new CheckTemplate
		{
			Id = 1,
			Name = "Skill Check",
			Definition = "<Definition />",
			CheckMethod = "Standard"
		});
		context.TraitDecorators.Add(new TraitDecorator
		{
			Id = 1,
			Name = "General Skill",
			Type = "Standard",
			Contents = "<Definition />"
		});
		context.Improvers.Add(new Improver
		{
			Id = 1,
			Name = "Skill Improver",
			Type = "Standard",
			Definition = "<Definition />"
		});
		context.FutureProgs.AddRange(
			new FutureProg
			{
				Id = 1,
				FunctionName = "AlwaysTrue",
				FunctionComment = string.Empty,
				FunctionText = "return true",
				ReturnType = (long)ProgVariableTypes.Boolean,
				Category = "Tests",
				Subcategory = "Traps"
			},
			new FutureProg
			{
				Id = 2,
				FunctionName = "AlwaysFalse",
				FunctionComment = string.Empty,
				FunctionText = "return false",
				ReturnType = (long)ProgVariableTypes.Boolean,
				Category = "Tests",
				Subcategory = "Traps"
			});
		context.Liquids.Add(new Liquid { Id = 1, Name = "water", DisplayColour = "#0" });
		context.Gases.Add(new Gas
		{
			Id = 1,
			Name = "smoke",
			Description = "smoke",
			SmellText = "smoke",
			VagueSmellText = "something smoky",
			DisplayColour = "#0"
		});
		context.SaveChanges();
	}

	[TestMethod]
	public void SeedData_InstallsIdempotentCompleteStockTrapPackage()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new TrapSeeder();

		seeder.SeedData(context, new Dictionary<string, string>());
		seeder.SeedData(context, new Dictionary<string, string>());

		var trapCheckTypes = new[]
		{
			(int)CheckType.SetTrapCheck,
			(int)CheckType.SpotTrapCheck,
			(int)CheckType.SearchForTrapCheck,
			(int)CheckType.AvoidTrapCheck,
			(int)CheckType.DisarmTrapCheck,
			(int)CheckType.DispelTrapCheck,
			(int)CheckType.EscapeTrapCheck
		};
		Assert.AreEqual(ShouldSeedResult.MayAlreadyBeInstalled, seeder.ShouldSeedData(context));
		Assert.AreEqual(1, context.TraitDefinitions.Count(x => x.Name == "Traps"));
		Assert.AreEqual(7, context.Checks.Count(x => trapCheckTypes.Contains(x.Type)));
		Assert.AreEqual(9, context.TrapTemplates.Count(x => x.Name.StartsWith("Stock Trap - ")));

		var natural = context.TrapTemplates.Single(x => x.Name == "Stock Trap - Spider Web");
		var magical = context.TrapTemplates.Single(x => x.Name == "Stock Trap - Magical Glyph");
		Assert.AreEqual("Natural", XElement.Parse(natural.Definition).Attribute("source")?.Value);
		Assert.AreEqual("Magical", XElement.Parse(magical.Definition).Attribute("source")?.Value);
	}
}
