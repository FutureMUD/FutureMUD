using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeederDisfigurementTemplateUtilityTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	[TestMethod]
	public void SeedTemplates_EmptyDefinitions_NoopsCleanly()
	{
		using var context = BuildContext();
		var body = SeedMinimalBodyContext(context);

		SeederDisfigurementTemplateUtilities.SeedTemplates(context, body);

		Assert.AreEqual(0, context.DisfigurementTemplates.Count(),
			"Empty template lists should leave the disfigurement catalogue untouched.");
		Assert.IsFalse(SeederDisfigurementTemplateUtilities.HasMissingDefinitions(context),
			"An empty definition set should never report missing stock templates.");
	}

	[TestMethod]
	public void SeedTemplates_ScarDefinitions_UpsertCurrentTemplateAndPersistScarFields()
	{
		using var context = BuildContext();
		var body = SeedMinimalBodyContext(context);
		var initialDefinition = new SeederScarTemplateDefinition(
			"Battle Scar",
			"a jagged battle scar",
			"A thick, jagged scar twists across the flesh.",
			SizeSteps: 2,
			Distinctiveness: 4,
			Unique: true,
			DamageHealingScarChance: 0.35,
			SurgeryHealingScarChance: 0.6,
			DamageTypes: new Dictionary<DamageType, WoundSeverity>
			{
				[DamageType.Slashing] = WoundSeverity.Severe
			},
			SurgeryTypes: [SurgicalProcedureType.InvasiveProcedureFinalisation],
			BodypartAliases: ["rarm"],
			OverrideCharacteristicPlain: "scarred",
			OverrideCharacteristicWith: "scarred");

		Assert.IsTrue(
			SeederDisfigurementTemplateUtilities.HasMissingDefinitions(context, scarDefinitions: [initialDefinition]),
			"The catalogue should report the scar template as missing before it is seeded.");

		SeederDisfigurementTemplateUtilities.SeedTemplates(context, body, scarDefinitions: [initialDefinition]);

		Assert.IsFalse(
			SeederDisfigurementTemplateUtilities.HasMissingDefinitions(context, scarDefinitions: [initialDefinition]),
			"The catalogue should no longer report the scar template as missing after seeding.");

		var template = context.DisfigurementTemplates
			.Include(x => x.EditableItem)
			.Single();
		Assert.AreEqual("Scar", template.Type);
		Assert.AreEqual("Battle Scar", template.Name);
		Assert.AreEqual(1, context.DisfigurementTemplates.Count(),
			"Seeding a brand new scar template should create exactly one current template.");
		Assert.AreEqual((int)RevisionStatus.Current, template.EditableItem.RevisionStatus,
			"Seeded templates should be created as current revisions.");

		var definitionXml = XElement.Parse(template.Definition);
		Assert.AreEqual("Scar", definitionXml.Name.LocalName);
		Assert.AreEqual("0.35", definitionXml.Element("DamageHealingScarChance")?.Value);
		Assert.AreEqual("0.6", definitionXml.Element("SurgeryHealingScarChance")?.Value);
		Assert.AreEqual("scarred", definitionXml.Element("OverrideCharacteristicPlain")?.Value);
		Assert.AreEqual("scarred", definitionXml.Element("OverrideCharacteristicWith")?.Value);
		CollectionAssert.AreEqual(
			new[] { "10" },
			definitionXml.Element("Shapes")!.Elements("Shape").Select(x => x.Value).ToArray(),
			"Bodypart aliases should resolve to the matching bodypart shape id.");
		Assert.AreEqual(
			(int)DamageType.Slashing,
			int.Parse(definitionXml.Element("Damages")!.Element("Damage")!.Attribute("type")!.Value));
		Assert.AreEqual(
			(int)WoundSeverity.Severe,
			int.Parse(definitionXml.Element("Damages")!.Element("Damage")!.Attribute("severity")!.Value));
		Assert.AreEqual(
			(int)SurgicalProcedureType.InvasiveProcedureFinalisation,
			int.Parse(definitionXml.Element("Surgeries")!.Element("Surgery")!.Value));

		var updatedDefinition = initialDefinition with
		{
			ShortDescription = "a refined battle scar",
			Distinctiveness = 6,
			DamageHealingScarChance = 0.8
		};
		SeederDisfigurementTemplateUtilities.SeedTemplates(context, body, scarDefinitions: [updatedDefinition]);

		template = context.DisfigurementTemplates
			.Include(x => x.EditableItem)
			.Single();
		definitionXml = XElement.Parse(template.Definition);
		Assert.AreEqual(1, context.DisfigurementTemplates.Count(),
			"Reseeding the same scar template should update in place rather than creating a duplicate current revision.");
		Assert.AreEqual("a refined battle scar", template.ShortDescription);
		Assert.AreEqual("0.8", definitionXml.Element("DamageHealingScarChance")?.Value);
		Assert.AreEqual("6", definitionXml.Element("Distinctiveness")?.Value);
	}

	[TestMethod]
	public void SeedTemplates_TattooDefinitions_PersistResolvedKnowledgeInkAndChargenMetadata()
	{
		using var context = BuildContext();
		var body = SeedMinimalBodyContext(context);
		context.ChargenResources.Add(new ChargenResource
		{
			Id = 20,
			Name = "Karma",
			PluralName = "Karma",
			Alias = "karma",
			Type = "test",
			TextDisplayedToPlayerOnAward = string.Empty,
			TextDisplayedToPlayerOnDeduct = string.Empty,
			MaximumResourceFormula = "0"
		});
		context.Colours.AddRange(
			new Colour { Id = 30, Name = "Black", Fancy = "black" },
			new Colour { Id = 31, Name = "Blue", Fancy = "blue" });
		context.Knowledges.Add(new Knowledge
		{
			Id = 40,
			Name = "Tattoo Lore",
			Description = "Tattoo knowledge",
			LongDescription = "Tattoo knowledge",
			Type = "Art",
			Subtype = "Tattoo"
		});
		context.SaveChanges();

		var definition = new SeederTattooTemplateDefinition(
			"Anchor",
			"an anchor tattoo",
			"A neatly inked anchor marks the skin.",
			MinimumBodypartSize: SizeCategory.Small,
			RequiredKnowledgeName: "Tattoo Lore",
			MinimumSkill: 75.5,
			InkColours: new Dictionary<string, double>
			{
				["Black"] = 0.75,
				["Blue"] = 0.25
			},
			BodypartAliases: ["rarm"],
			ChargenCosts: new Dictionary<string, int>
			{
				["karma"] = 3
			},
			OverrideCharacteristicPlain: "inked",
			OverrideCharacteristicWith: "inked");

		SeederDisfigurementTemplateUtilities.SeedTemplates(context, body, tattooDefinitions: [definition]);

		var template = context.DisfigurementTemplates
			.Include(x => x.EditableItem)
			.Single();
		var definitionXml = XElement.Parse(template.Definition);
		Assert.AreEqual("Tattoo", template.Type);
		Assert.AreEqual("Anchor", template.Name);
		Assert.AreEqual("75.5", definitionXml.Element("MinimumSkill")?.Value);
		Assert.AreEqual("40", definitionXml.Element("RequiredKnowledge")?.Value);
		Assert.AreEqual("inked", definitionXml.Element("OverrideCharacteristicPlain")?.Value);
		Assert.AreEqual("inked", definitionXml.Element("OverrideCharacteristicWith")?.Value);
		CollectionAssert.AreEquivalent(
			new[] { "30", "31" },
			definitionXml.Element("Inks")!.Elements("Ink").Select(x => x.Value).ToArray(),
			"Ink colours should resolve by seeded colour name.");
		CollectionAssert.AreEqual(
			new[] { "10" },
			definitionXml.Element("Shapes")!.Elements("Shape").Select(x => x.Value).ToArray(),
			"Bodypart aliases should resolve to the target bodypart shape.");
		var chargenCost = definitionXml.Element("ChargenCosts")!.Element("Cost");
		Assert.IsNotNull(chargenCost);
		Assert.AreEqual("20", chargenCost.Attribute("resource")?.Value);
		Assert.AreEqual("3", chargenCost.Attribute("amount")?.Value);
	}

	[TestMethod]
	public void HumanSeeder_DefaultDisfigurementHooks_AreEmptyByDefault()
	{
		Assert.AreEqual(0, HumanSeeder.TattooTemplatesForTesting.Count,
			"Human stock tattoo hooks should start empty until a later content pass adds templates.");
		Assert.AreEqual(0, HumanSeeder.ScarTemplatesForTesting.Count,
			"Human stock scar hooks should start empty until a later content pass adds templates.");
	}

	private static BodyProto SeedMinimalBodyContext(FuturemudDatabaseContext context)
	{
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "builder",
			Password = "password",
			Email = "builder@example.com",
			LastLoginIp = "127.0.0.1",
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = "reg",
			RecoveryCode = "recovery",
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow
		});
		var shape = new BodypartShape
		{
			Id = 10,
			Name = "Arm"
		};
		var body = new BodyProto
		{
			Id = 11,
			Name = "Test Body",
			ConsiderString = "a test body",
			WielderDescriptionSingle = "hand",
			WielderDescriptionPlural = "hands",
			LegDescriptionSingular = "leg",
			LegDescriptionPlural = "legs"
		};
		var bodypart = new BodypartProto
		{
			Id = 12,
			Body = body,
			BodyId = body.Id,
			Name = "rarm",
			Description = "right arm",
			BodypartShape = shape,
			BodypartShapeId = shape.Id
		};
		context.BodypartShapes.Add(shape);
		context.BodyProtos.Add(body);
		context.BodypartProtos.Add(bodypart);
		context.SaveChanges();
		return body;
	}
}
