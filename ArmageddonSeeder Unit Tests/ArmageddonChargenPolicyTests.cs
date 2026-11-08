#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.Models;
using ChargenScreenStoryboard = MudSharp.Models.ChargenScreenStoryboard;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonChargenPolicyTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		// Like the existing heritage fixture, this models policy fields rather than a full playable database.
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), options => options.EnableNullChecks(false)).Options);

	[TestMethod]
	public void SettingMenus_FilterUnrelatedContent_PreserveSettingAndLaterBuilderChoices()
	{
		using var db = Context();
		db.FutureProgs.Add(new FutureProg { Id = 2, FunctionName = "AlwaysFalse", FunctionText = "return false" });
		db.Races.AddRange(new Race { Id = 10, Name = "Human", AvailabilityProgId = 1 }, new Race { Id = 11, Name = "Robot", AvailabilityProgId = 1 });
		db.Cultures.AddRange(new Culture { Id = 10, Name = "Allanaki", AvailabilityProgId = 3 }, new Culture { Id = 11, Name = "Earth", AvailabilityProgId = 1 });
		foreach (var type in new[] { nameof(Race), nameof(Culture) })
			db.SeederManagedRecords.Add(new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = ArmageddonHeritage.Module,
				StableKey = type == nameof(Race) ? "arm.race.human" : "arm.culture.allanaki", EntityType = type, LogicalId = 10 });
		db.Knowledges.AddRange(new Knowledge { Id = 10, Name = "Robot Surgery", CanAcquireProgId = 1 }, new Knowledge { Id = 11, Name = "Chiurgery", CanAcquireProgId = 3 });
		db.Merits.Add(new Merit { Id = 10, Name = "Astral Sight", Type = "Planar State", Definition = "<Definition><ChargenAvailableProg>1</ChargenAvailableProg><Other>keep</Other></Definition>" });
		db.ChargenScreenStoryboards.Add(new ChargenScreenStoryboard { Id = 10, ChargenType = "RolePicker", ChargenStage = (int)ChargenStage.SelectRole,
			StageDefinition = "<Screen><RoleTypes><RoleType Type='Class' CanSelectNone='false'/><RoleType Type='Profession' CanSelectNone='false'/></RoleTypes></Screen>" });
		db.SaveChanges();
		var messages = new List<string>();
		ArmageddonChargenPolicy.Install(db, messages);
		Assert.AreEqual(1L, db.Races.Find(10L)!.AvailabilityProgId);
		Assert.AreEqual(2L, db.Races.Find(11L)!.AvailabilityProgId);
		Assert.AreEqual(3L, db.Cultures.Find(10L)!.AvailabilityProgId);
		Assert.AreEqual(2L, db.Cultures.Find(11L)!.AvailabilityProgId);
		Assert.AreEqual(2L, db.Knowledges.Find(10L)!.CanAcquireProgId);
		Assert.AreEqual(3L, db.Knowledges.Find(11L)!.CanAcquireProgId);
		var merit = XElement.Parse(db.Merits.Find(10L)!.Definition);
		Assert.AreEqual("2", merit.Element("ChargenAvailableProg")!.Value);
		Assert.AreEqual("keep", merit.Element("Other")!.Value);
		var roles = XElement.Parse(db.ChargenScreenStoryboards.Find(10L)!.StageDefinition).Element("RoleTypes")!.Elements().ToArray();
		Assert.AreEqual("false", roles[0].Attribute("CanSelectNone")!.Value);
		Assert.AreEqual("true", roles[1].Attribute("CanSelectNone")!.Value);
		var count = db.SeederManagedRecords.Count();
		db.Races.Find(11L)!.AvailabilityProgId = 42;
		db.SaveChanges();
		ArmageddonChargenPolicy.Install(db, messages);
		Assert.AreEqual(count, db.SeederManagedRecords.Count());
		Assert.AreEqual(42L, db.Races.Find(11L)!.AvailabilityProgId);
		Assert.IsTrue(messages.Any(x => x.Contains("builder edit")));
		Assert.IsTrue(db.SeederManagedRecords.Where(x => x.Module == ArmageddonSettingLinks.Module).AsEnumerable().All(x => ArmageddonSettingLinks.Valid(db, x)));
	}

	[TestMethod]
	public void WorldfileStart_ReplacesOnlyStockPlaceholder_WhenGajExists()
	{
		using var db = Context();
		db.ChargenRoles.Add(new ChargenRole { Id = 1, Type = (int)ChargenRoleType.StartingLocation, Name = "Default Starting Location", ChargenBlurb = "default" });
		var screen = new ChargenScreenStoryboard { Id = 1, ChargenType = "StartingLocationPicker", ChargenStage = (int)ChargenStage.SelectStartingLocation,
			StageDefinition = "<Screen><Locations><Location><Name>Guest Lounge</Name><Blurb>As you have not yet set up any other starting areas, the default starting area will be the guest lounge.</Blurb><Location>1</Location><Role>1</Role><OnCommenceProg>0</OnCommenceProg></Location><Location><Name>Custom</Name><Location>99</Location><Role>1</Role><OnCommenceProg>42</OnCommenceProg></Location><Location><Name>Guest Lounge</Name><Blurb>Builder introduction</Blurb><Location>1</Location><Role>1</Role><OnCommenceProg>0</OnCommenceProg></Location></Locations></Screen>" };
		db.ChargenScreenStoryboards.Add(screen);
		db.SaveChanges();
		Assert.IsFalse(ArmageddonChargenPolicy.ConfigureWorldfileStart(db));
		db.Rooms.Add(new Room { Id = 100, UniqueName = "Vnum50615" });
		db.SaveChanges();
		Assert.IsTrue(ArmageddonChargenPolicy.ConfigureWorldfileStart(db));
		var locations = XElement.Parse(screen.StageDefinition).Element("Locations")!.Elements().ToArray();
		Assert.AreEqual("100", locations[0].Element("Location")!.Value);
		Assert.AreEqual("99", locations[1].Element("Location")!.Value);
		Assert.AreEqual("42", locations[1].Element("OnCommenceProg")!.Value);
		Assert.AreEqual("1", locations[2].Element("Location")!.Value);
		Assert.AreEqual("Builder introduction", locations[2].Element("Blurb")!.Value);
		Assert.IsFalse(ArmageddonChargenPolicy.ConfigureWorldfileStart(db));
	}
}
