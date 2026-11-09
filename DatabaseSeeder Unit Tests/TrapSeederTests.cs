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
using MudSharp.Traps;

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
	public void SeedData_InstalledTrapDefinitionsLoadAndValidate()
	{
		using var context = BuildContext();
		SeedPrerequisites(context);
		var seeder = new TrapSeeder();

		seeder.SeedData(context, new Dictionary<string, string>());
		seeder.SeedData(context, new Dictionary<string, string>());

		Assert.IsTrue(context.TrapTemplates.Any());

		foreach (var stockTemplate in context.TrapTemplates
			         .Where(x => x.Name.StartsWith("Stock Trap - "))
			         .AsEnumerable())
		{
			var definition = XElement.Parse(stockTemplate.Definition);
			foreach (var trigger in definition.Element("Triggers")!.Elements("Trigger")
			         .Select(TrapTriggerDefinition.LoadFromXml))
			{
				Assert.IsTrue(TrapTriggerDefinition.TryValidateParameters(trigger.TriggerType, trigger.Parameters, out _),
					stockTemplate.Name);
			}

			foreach (var payload in definition.Element("Payloads")!.Elements("Payload")
			         .Select(TrapPayloadDefinition.LoadFromXml))
			{
				Assert.IsTrue(payload.Delay >= TimeSpan.Zero,
					$"{stockTemplate.Name} has a negative payload delay.");
				Assert.IsTrue(TrapPayloadDefinition.TryValidateParameters(payload.PayloadType, payload.Parameters, out _),
					stockTemplate.Name);
			}
		}
	}
}
