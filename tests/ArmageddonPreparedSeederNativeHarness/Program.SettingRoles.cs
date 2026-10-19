#nullable enable

// Native combined Setting qualification on the harness-owned disposable database.
// Integration:
// 1. Place this partial beside Program.CoreFoundations.cs only after coordinator review.
// 2. Call QualifySettingNativeRolesAndHeritage(restored) after the complete Setting install,
//    OUTSIDE any context scope, BEFORE the deliberate Mercenary multiplier=1.5 edit.
//    The native world must be disposed before the seeder resumes/reruns.
// 3. PreparedSeederMain switch: "--setting-role-reader" => ReadSettingRoleQualification(args[1]).
//    RunItemReaderProcess uses this assembly's PreparedSeederEntryPoint, not GNHProgram.Main.
// 4. Requires current-schema, classes-on, fully installed baseline. Never invokes
//    SeedProductionLoaderFixture (that helper rewrites traits/cultures/anatomy).
// 5. The existing full server login/cold-login lane remains separate acceptance.

using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.RPG.Merits.Interfaces;
using NativeCharacter = MudSharp.Character.Character;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record SettingRoleReader(string Database, DateTime Now, long Merchant, long Burglar,
		long Pilot, long Child, long[] Applications, ArmPilotReceipt[] Drivers, string Phase);

	private static void QualifySettingNativeRolesAndHeritage(TestDatabase database)
	{
		ConfigureNativeDatabase(database.ConnectionString);
		long accountId, cultureId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(TestDatabase.HasOwnedPrefix(database.Name), "Role qualification requires an owned disposable database.");
			// Real persisted Player account, no character/body/application clone.
			var account = new Db.Account();
			db.Entry(account).CurrentValues.SetValues(db.Accounts.AsNoTracking().OrderBy(x => x.Id).First());
			account.Id = 0; account.Name = "SettingQual" + Guid.NewGuid().ToString("N")[..10];
			account.Email = string.Empty; account.LastLoginTime = null;
			account.AuthorityGroupId = db.AuthorityGroups.Single(x => x.AuthorityLevel == (int)PermissionLevel.Player).Id;
			db.Accounts.Add(account); db.SaveChanges(); accountId = account.Id;
			// Admin's AlwaysOneHundred starting prog would mask the authored Pilot40.
			// Copy only fixture culture values/naming joins, changing its starting prog to native zero.
			// Shared Human/Admin and every Armageddon profile gate remain untouched.
			var source = db.Cultures.AsNoTracking().Single(x => x.Name == "Admin");
			var culture = new Db.Culture(); db.Entry(culture).CurrentValues.SetValues(source);
			culture.Id = 0; culture.Name = "Setting role qualification " + account.Name;
			culture.Description = "Disposable native role qualification; not a playable setting background.";
			culture.SkillStartingValueProgId = db.FutureProgs.Single(x => x.FunctionName == "AlwaysZero").Id;
			db.Cultures.Add(culture); db.SaveChanges(); cultureId = culture.Id;
			foreach (var link in db.CulturesNameCultures.AsNoTracking().Where(x => x.CultureId == source.Id).ToArray())
				db.CulturesNameCultures.Add(new() { CultureId = cultureId, Gender = link.Gender, NameCultureId = link.NameCultureId });
			db.SaveChanges();
		}
		var pilotFixture = SeedSettingQualificationPilotRooms(database);
		SeedSettingActionFixtures(database);
		var clock = new HarnessClock();
		using var time = RuntimeClock.Push(clock);
		SettingRoleReader receipt;
		using (var world = LoadProductionRestartWorld(clock))
		using (var session = new FMDB())
		{
			((IFuturemudLoader)world).LoadChargen();
			LoadArmageddonPilotCatalogue(world);
			var ids = SettingQualificationIds(database);
			var merchantApp = NewSettingQualificationApplication(world, accountId, cultureId, "QualificationMerchant");
			merchantApp.SelectedRoles.Add(world.Roles.Get(ids["arm.class.merchant"]));
			merchantApp.SelectedRoles.Add(world.Roles.Get(ids["arm.subclass.mercenary"]));
			FinaliseAndSaveSettingApplication(merchantApp, SettingQualificationFreeProg(world));
			var merchant = PersistSettingQualificationCharacter(world, merchantApp);
			CheckSettingMerchant(merchant, world.Traits.Get(ids["arm.skill.pilot"]));

			var burglarApp = NewSettingQualificationApplication(world, accountId, cultureId, "QualificationBurglar");
			burglarApp.SelectedRoles.Add(world.Roles.Get(ids["arm.class.burglar"]));
			burglarApp.SelectedRoles.Add(world.Roles.Get(ids["arm.subclass.bard"]));
			FinaliseAndSaveSettingApplication(burglarApp, SettingQualificationFreeProg(world));
			var burglar = PersistSettingQualificationCharacter(world, burglarApp);
			var child = world.Traits.Get(ids["arm.skill.sleight_of_hand"]);
			Require(!burglar.HasTrait(child), "Burglar/Bard must not acquire the shared child during creation.");
			Require(burglar.EffectsOfType<RoleSkillBonusClaims>().Single().Opening(child.Id, -1) == 40,
				"Deferred Sleight opening must be10+30, retaining main cap80 and Steal50 prerequisite.");
			Require(!RoleSkillProgressionService.CanAcquire(burglar, child), "Absent/below-threshold parent must block the child.");
			Require(burglar.Merits.Any(x => x.Id == ids[ArmageddonMundaneMerits.BardKey]), "Bard learning merit is missing.");

			var drivers = new List<ArmPilotReceipt>();
			foreach (var roomScale in new[] { false, true })
			{
				var app=NewSettingQualificationApplication(world,accountId,cultureId,roomScale ? "QualificationRoomDriver" : "QualificationItemDriver");
				app.StartingLocation=new StartingLocation { Name="Disposable driving origin", Blurb="Qualification only", Location=world.Rooms.Get(pilotFixture.Origin), Role=app.StartingLocation.Role };
				app.SelectedRoles.Add(world.Roles.Get(ids["arm.class.merchant"]));
				app.SelectedRoles.Add(world.Roles.Get(ids["arm.subclass.mercenary"]));
				FinaliseAndSaveSettingApplication(app,SettingQualificationFreeProg(world));
				var driver=PersistSettingQualificationCharacter(world,app);
				driver.SetTraitValue(world.Traits.Get(ids["arm.skill.pilot"]),60); // fixture proficiency within native cap95
				drivers.Add(RunArmageddonNativePilot(world,clock,pilotFixture,driver,roomScale));
			}
			using (var config=NewIndependentContext(database.ConnectionString)) { config.StaticConfigurations.Find("DirectionSenseObscurationProg")!.Definition="0"; config.SaveChanges(); }
			QualifySettingNativeActions(world,(roleKey,label)=> {
				var app=NewSettingQualificationApplication(world,accountId,cultureId,"Action"+label);
				app.SelectedRoles.Add(world.Roles.Get(ids[roleKey]));
				FinaliseAndSaveSettingApplication(app,SettingQualificationFreeProg(world));
				var actor=PersistSettingQualificationCharacter(world,app);
				var targetApp=NewSettingQualificationApplication(world,accountId,cultureId,"Target"+label);
				FinaliseAndSaveSettingApplication(targetApp,SettingQualificationFreeProg(world));
				return (actor,PersistSettingQualificationCharacter(world,targetApp));
			},ids);
			var applications = CheckSettingHeritageApplications(world, accountId, cultureId, ids).ToArray();
			world.SaveManager.Flush();
			using var db = NewIndependentContext(database.ConnectionString);
			Require(db.CharacterTraits.Single(x => x.CharacterId == merchant.Id && x.TraitDefinitionId == ids["arm.skill.pilot"]).Value == 40,
				"Actual native Pilot40 insert is missing.");
			Require(!db.CharacterTraits.Any(x => x.CharacterId == burglar.Id && x.TraitDefinitionId == child.Id) &&
				XElement.Parse(db.Characters.Single(x => x.Id == burglar.Id).EffectData).Descendants("Claim")
					.Any(x => (long?)x.Attribute("trait") == child.Id && (double?)x.Attribute("floor") == 40),
				"Native save did not persist the pending bonus without acquiring the child.");
			receipt = new(database.Name, RuntimeClock.UtcNow, merchant.Id, burglar.Id, ids["arm.skill.pilot"], child.Id,
				applications.Append(merchantApp.Id).Append(burglarApp.Id).ToArray(), drivers.ToArray(), "acquire");
		}
		// No producer world remains alive while either cold reader writes durable state.
		RunItemReaderProcess(receipt, "--setting-role-reader");
		RunItemReaderProcess(receipt with { Phase = "verify" }, "--setting-role-reader");
		Console.WriteLine("SETTING-native-roles-heritage=PASS real-production-world real-Chargen-and-Character native-Pilot40-cap95-liver2 deferred40-first-acquisition-only two-cold-readers compiled-language200-100-60 gates-unchanged");
	}

	private static ArmPilotFixture SeedSettingQualificationPilotRooms(TestDatabase database)
	{
		long origin,destination,material;
		using (var db=NewIndependentContext(database.ConnectionString))
		{
			var source=db.Rooms.Include(x=>x.Zones).First(x=>x.CurrentOverlayId!=null && x.Zones.Any());
			var overlay=db.RoomOverlays.Single(x=>x.Id==source.CurrentOverlayId);
			long Room(string name)
			{
				var room=(Db.Room)db.Entry(source).CurrentValues.Clone().ToObject(); room.Id=0; room.CurrentOverlayId=null;
				foreach(var zone in source.Zones) room.Zones.Add(zone);
				db.Rooms.Add(room); db.SaveChanges();
				var copy=(Db.RoomOverlay)db.Entry(overlay).CurrentValues.Clone().ToObject(); copy.Id=0;copy.RoomId=room.Id;copy.RoomName=name;copy.RoomDescription="An isolated native qualification ground.";copy.AddedLight=1000;
				db.RoomOverlays.Add(copy);db.SaveChanges();room.CurrentOverlayId=copy.Id;db.SaveChanges();return room.Id;
			}
			origin=Room("Setting driving origin");destination=Room("Setting driving destination");
			material=db.Materials.OrderBy(x=>x.Id).First().Id;
		}
		return SeedArmageddonNativePilotFixture(database,origin,destination,material);
	}

	private static Dictionary<string, long> SettingQualificationIds(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired && x.LogicalId != null)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
	}

	private static Chargen NewSettingQualificationApplication(IFuturemud world, long accountId, long cultureId, string name)
	{
		Db.Account account;
		using (new FMDB()) account = FMDB.Context.Accounts.Single(x => x.Id == accountId);
		var app = new Chargen((IChargenMenu)null!, world, world.TryAccount(account)); // normal public constructor; no UI dispatch
		app.SelectedRace = world.Races.Single(x => x.Name == "Human");
		app.SelectedEthnicity = world.Ethnicities.Single(x => x.Name == "Admin" && x.ParentRace == app.SelectedRace);
		app.SelectedCulture = world.Cultures.Get(cultureId);
		app.SelectedGender = Gender.Male; app.Handedness = Alignment.Right;
		(app.SelectedHeight, app.SelectedWeight) = app.SelectedRace.DefaultHeightWeightModel(app.SelectedGender).GetRandomHeightWeight();
		app.SelectedBirthday = app.SelectedCulture.PrimaryCalendar.GetRandomBirthday(25);
		app.SelectedName = app.SelectedCulture.NameCultureForGender(app.SelectedGender).GetPersonalName(name);
		app.SelectedSdesc = "a qualification applicant"; app.SelectedFullDesc = "A disposable native qualification character.";
		app.SelectedAttributes = app.SelectedRace.Attributes.Select(x => (ITrait)TraitFactory.LoadAttribute(x, app, 10)).ToList();
		app.SelectedCharacteristics = app.SelectedRace.Characteristics(app.SelectedGender)
			.Select(x => (x, app.SelectedEthnicity.CharacteristicChoices[x].GetRandomCharacteristic(app))).ToList();
		app.StartingLocation = ((StartingLocationPickerScreenStoryboard)world.ChargenStoryboard.StageScreenMap[ChargenStage.SelectStartingLocation])
			.Locations.First(x => x.Location is not null && x.Role is not null);
		app.SelectedRoles.Add(app.StartingLocation.Role);
		return app;
	}

	private static MudSharp.FutureProg.IFutureProg SettingQualificationFreeProg(IFuturemud world)
	{
		// Exact configured storyboard pointer, not a name-based replacement or reimplemented grant.
		var method = typeof(Chargen).GetMethod("FreeSkillsProgForStoryboard", BindingFlags.Static | BindingFlags.NonPublic)!;
		return (MudSharp.FutureProg.IFutureProg)method.Invoke(null,
			[world.ChargenStoryboard.StageScreenMap[ChargenStage.SelectSkills]])!;
	}

	private static void FinaliseAndSaveSettingApplication(Chargen app, MudSharp.FutureProg.IFutureProg free)
	{
		Require(free.Compile(), "Configured free-skills prog did not compile: " + free.CompileError);
		var resolver = new SkillGroupResolver(app, free);
		Require(resolver.Complete, "Actual skill projection failed: " + string.Join("; ", resolver.Errors));
		// Invoke the SAME finalisation/save methods called by native approval; avoid approval's
		// outbound email/Discord and reviewer notifications in this bounded in-process qualification.
		typeof(Chargen).GetMethod("PerformPostCreationProcessing", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(app, null);
		typeof(Chargen).GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(app, null);
		Require(app.Id > 0, "Real application was not persisted.");
	}

	private static NativeCharacter PersistSettingQualificationCharacter(IFuturemud world, Chargen app)
	{
		var character = new NativeCharacter(world, app);
		world.SaveManager.Flush(); // native queued Character/Body DatabaseInsert and SetIDFromDatabase
		Require(character.Id > 0 && character.Body.Id > 0, "Native character/body insert did not finish.");
		world.Add(character, false);
		character.Register(null);
		if (!character.Location.Characters.Contains(character)) character.Location.Enter(character);
		return character;
	}

	private static void CheckSettingMerchant(ICharacter character, ITraitDefinition pilot)
	{
		Require(character.HasTrait(pilot) && character.TraitRawValue(pilot) == 40 && character.TraitMaxValue(pilot) == 95,
			"Merchant/Mercenary native Pilot opening/cap must be40/95.");
		var merit = character.Merits.OfType<ILiverClearanceMerit>().Single(x => x.Applies(character));
		var organ = character.Body.OrganFunction<LiverProto>();
		var baseline = character.Body.BaseLiverAlcoholRemovalKilogramsPerHour;
		Require(merit.ClearanceMultiplier == 2 && organ > 0 && baseline > 0 &&
			Math.Abs(character.Body.LiverAlcoholRemovalKilogramsPerHour / (organ * baseline) - 2) < 1e-9,
			"Real healthy liver must apply stock clearance multiplier2 after organ function.");
	}

	private static IEnumerable<long> CheckSettingHeritageApplications(IFuturemud world, long accountId, long cultureId,
		IReadOnlyDictionary<string, long> ids)
	{
		// Pinned approved expectations, independent of the installer's Grants implementation.
		var samples = new (string Profile, string Culture, (string Language, int Value)[] Expected)[] {
			("human", "allanaki", [("sirihish", 200)]),
			("city_elf", "allanaki", [("allundean", 200), ("sirihish", 100)]),
			("desert_elf", "desert_elven", [("allundean", 200)]),
			("dwarf", "labyrinth", [("mirukkim", 200), ("sirihish", 100)]),
			("half_elf", "tuluki", [("sirihish", 200), ("allundean", 60)]),
			("half_giant", "tuluki", [("sirihish", 200)]),
			("mul", "labyrinth", [("sirihish", 200), ("mirukkim", 100)]),
			("human", "nomadic", [("bendune", 200)]) };
		var free = SettingQualificationFreeProg(world);
		foreach (var (profileKey, cultureKey, expected) in samples)
		{
			var app = NewSettingQualificationApplication(world, accountId, cultureId, "Heritage" + profileKey.Replace("_", "") + cultureKey);
			app.SelectedRace = world.Races.Get(ids["arm.race." + profileKey]);
			app.SelectedEthnicity = world.Ethnicities.Get(ids["arm.ethnicity." + profileKey + ".default"]);
			app.SelectedCulture = world.Cultures.Get(ids["arm.culture." + cultureKey]);
			Require(!app.SelectedRace.ChargenAvailable(app), "Physical profile gate was unexpectedly enabled.");
			var start = app.SelectedCulture.SkillStartingValueProg;
			Require(start.Compile(), "Heritage starting prog failed: " + start.CompileError);
			FinaliseAndSaveSettingApplication(app, free);
			foreach (var (language, value) in expected)
			{
				var skill = world.Traits.Get(ids["arm.skill." + language]);
				Require(app.SelectedSkills.Contains(skill) && app.SkillValues.Single(x => x.Item1 == skill).Item2 == value,
					$"Compiled native grant differs: {profileKey}/{cultureKey}/{language}/{value}.");
				var native = world.FutureProgs.Get(ids["arm.language." + language + ".native"]);
				Require(native.Compile() && native.ExecuteBool(app) == (value == 200), "Native-language compiled identity differs.");
			}
			// Persisted Chargen only; NEVER construct a Character from these gated scaffolds.
			yield return app.Id;
		}
	}

	private static int ReadSettingRoleQualification(string encoded)
	{
		var input = JsonSerializer.Deserialize<SettingRoleReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database);
		ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime);
		using var time = RuntimeClock.Push(clock);
		using var world = LoadProductionRestartWorld(clock);
		using var session = new FMDB();
		((IFuturemudLoader)world).LoadChargen();
		LoadArmageddonPilotCatalogue(world);
		LoadArmageddonPilotInstances(world);
		foreach (var driver in input.Drivers) AssertArmageddonNativePilotReload(world,world.TryGetCharacter(driver.Driver,true),driver);
		var merchant = world.TryGetCharacter(input.Merchant, true);
		var burglar = world.TryGetCharacter(input.Burglar, true);
		CheckSettingMerchant(merchant, world.Traits.Get(input.Pilot));
		var child = (ISkillDefinition)world.Traits.Get(input.Child);
		var claims = burglar.EffectsOfType<RoleSkillBonusClaims>().Single();
		if (input.Phase == "acquire")
		{
			Require(!burglar.HasTrait(child) && claims.Opening(child.Id, -1) == 40 && !child.CanLearn(burglar),
				"First cold load lost deferred claim or bypassed its parent.");
			UnlockSettingQualificationParent(burglar, child);
			RoleSkillProgressionService.EvaluateBranches(burglar); // actual CanLearn, AddTrait, consume and branch log
			Require(burglar.HasTrait(child) && burglar.TraitRawValue(child) == 40 && burglar.TraitMaxValue(child) == 80 &&
				claims.Opening(child.Id, 10) == 10, "First actual branch did not consume exactly one opening40 claim.");
			burglar.SetTraitValue(child, 43); // subsequent earned value must survive, never reset to opening40
			RoleSkillProgressionService.EvaluateBranches(burglar);
			Require(burglar.TraitRawValue(child) == 43, "Repeated branch evaluation replayed the bonus.");
		}
		else
		{
			Require(input.Phase == "verify" && burglar.TraitRawValue(child) == 43 && claims.Opening(child.Id, 10) == 10,
				"Second cold load lost earned value or resurrected consumed bonus.");
			Require(!burglar.AddTrait(child, 999) && burglar.TraitRawValue(child) == 43, "Duplicate acquisition changed a possessed skill.");
			RoleSkillProgressionService.EvaluateBranches(burglar);
			Require(burglar.TraitRawValue(child) == 43, "Cold branch evaluation replayed a creation bonus.");
		}
		// Actual saved Chargen hydration must retain all native and secondary values.
		foreach (var id in input.Applications)
		{
			var row = FMDB.Context.Chargens.AsNoTracking().Single(x => x.Id == id);
			var saved = XElement.Parse(row.Definition).Element("SkillValues")!.Elements("Skill")
				.ToDictionary(x => (long)x.Attribute("ID")!, x => (double)x.Attribute("Value")!);
			var app = new Chargen(row, world, FMDB.Context.Accounts.Single(x => x.Id == row.AccountId));
			Require(saved.Count == app.SkillValues.Count && app.SkillValues.All(x => saved[x.Item1.Id] == x.Item2),
				"Native Chargen hydration changed composed skill values.");
		}
		world.SaveManager.Flush();
		using var db = NewIndependentContext(database.ConnectionString);
		Require(db.CharacterTraits.Single(x => x.CharacterId == input.Burglar && x.TraitDefinitionId == input.Child).Value == 43,
			"Native cold save did not persist earned value43.");
		Console.WriteLine($"SETTING-native-role-reader={input.Phase} PASS persisted-roles-merits-caps-claims-applications");
		return 0;
	}

	private static void UnlockSettingQualificationParent(ICharacter character, ITraitDefinition child)
	{
		var entitlement = RoleSkillProgressionService.Entitlements(character)[child.Id];
		if (entitlement.ParentTraitId is not { } parentId) return;
		var parent = character.Gameworld.Traits.Get(parentId);
		if (!character.HasTrait(parent))
		{
			UnlockSettingQualificationParent(character, parent);
			RoleSkillProgressionService.EvaluateBranches(character);
			Require(character.HasTrait(parent), "Native parent branch could not be acquired.");
		}
		character.SetTraitValue(parent, Math.Max(character.TraitRawValue(parent), entitlement.Threshold));
	}
}
