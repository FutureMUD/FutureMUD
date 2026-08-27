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
		Assert.AreEqual(11, context.TrapTemplates.Count(x => x.Name.StartsWith("Stock Trap - ")));
		Assert.AreEqual(11, context.Tags.Count(x => x.Name == "Trap Components" || x.Parent != null && x.Parent.Name == "Trap Components"));
		Assert.AreEqual("Functions", context.Tags.Single(x => x.Name == "Trap Components").Parent?.Name);

		foreach (var mechanical in context.TrapTemplates
			         .Where(x => x.Name.StartsWith("Stock Trap - "))
			         .AsEnumerable()
			         .Where(x => XElement.Parse(x.Definition).Attribute("source")?.Value == "Mechanical"))
		{
			var roles = XElement.Parse(mechanical.Definition).Element("Components")!.Elements("Component")
				.Select(x => Enum.Parse<TrapComponentRole>(x.Attribute("role")!.Value))
				.ToList();
			Assert.IsTrue(roles.Any(x => x.HasFlag(TrapComponentRole.Trigger)), mechanical.Name);
			Assert.IsTrue(roles.Any(x => x.HasFlag(TrapComponentRole.Payload)), mechanical.Name);
		}

		var natural = context.TrapTemplates.Single(x => x.Name == "Stock Trap - Spider Web");
		var magical = context.TrapTemplates.Single(x => x.Name == "Stock Trap - Magical Glyph");
		var magicalExplosion = context.TrapTemplates.Single(x => x.Name == "Stock Trap - Magical Explosion Glyph");
		Assert.AreEqual("Natural", XElement.Parse(natural.Definition).Attribute("source")?.Value);
		Assert.AreEqual("Magical", XElement.Parse(magical.Definition).Attribute("source")?.Value);
		var explosivePayload = XElement.Parse(magicalExplosion.Definition).Element("Payloads")!.Element("Payload")!;
		Assert.AreEqual(TrapPayloadType.ExplosiveDamage.ToString(), explosivePayload.Attribute("type")?.Value);
		var explosiveParameters = explosivePayload.Elements("Parameter")
			.ToDictionary(x => x.Attribute("name")!.Value, x => x.Value, StringComparer.OrdinalIgnoreCase);
		CollectionAssert.IsSubsetOf(
			new[] { "damage", "pain", "stun", "damagetype", "explosionsize", "maximumproximity", "elevation" },
			explosiveParameters.Keys.ToList());

		foreach (var directDamage in context.TrapTemplates
			         .Where(x => x.Name.StartsWith("Stock Trap - "))
			         .AsEnumerable()
			         .SelectMany(x => XElement.Parse(x.Definition).Element("Payloads")!.Elements("Payload"))
			         .Where(x => x.Attribute("type")?.Value == TrapPayloadType.DirectDamage.ToString()))
		{
			var parameters = directDamage.Elements("Parameter")
				.ToDictionary(x => x.Attribute("name")!.Value, x => x.Value, StringComparer.OrdinalIgnoreCase);
			Assert.IsTrue(parameters.ContainsKey("damage"));
			Assert.IsTrue(parameters.ContainsKey("pain"));
			Assert.IsTrue(parameters.ContainsKey("stun"));
		}

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
