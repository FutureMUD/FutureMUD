#nullable enable

using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private const string NamingQualificationDescription = "A disposable executable naming qualification applicant.";

	private static void SaveNamingApplication(Chargen app)
	{
		app.SelectedFullDesc = NamingQualificationDescription;
		typeof(Chargen).GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(app, null);
		Require(app.Id > 0, "Native naming application was not saved.");
	}

	private static void QualifyExecutableNaming(TestDatabase database)
	{
		var clock = new HarnessClock();
		using var time = RuntimeClock.Push(clock);
		using var world = LoadProductionRestartWorld(clock);
		using var session = new FMDB();
		((IFuturemudLoader)world).LoadChargen();
		var account = FMDB.Context.Accounts.OrderBy(x => x.Id).First();
		var cultureId = FMDB.Context.SeederManagedRecords.Single(x => x.StableKey == "arm.culture.desert_elven").LogicalId!.Value;
		var app = NewSettingQualificationApplication(world, account.Id, cultureId, "QualificationNaming");
		var storyboard = world.ChargenStoryboard.StageScreenMap[ChargenStage.SelectName];
		var screen = storyboard.GetScreen(app);
		var target = FMDB.Context.SeederManagedRecords.Single(x => x.StableKey == "arm.naming.desert_elven").LogicalId!.Value;
		var options = (INameCulture[])screen.GetType().GetMethod("AvailableCultures", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(screen, null)!;
		var choice = Array.FindIndex(options, x => x.Id == target);
		Require(choice >= 0, "Expected native upbringing naming culture is absent from its actual options.");
		var selection = screen.HandleCommand("/style " + (choice + 1));
		var selected = (INameCulture)screen.GetType().GetField("NameCulture", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(screen)!;
		Require(selected.Id == target, "Native naming style selection failed: " + selection);
		screen.HandleCommand("Agis");
		screen.HandleCommand("");
		screen.HandleCommand("\"Silt Runner\" Dust-Cutter");
		screen.HandleCommand("Captain");
		Require(screen.HandleCommand("Night Runner Tribe").Contains("confirm", StringComparison.OrdinalIgnoreCase), "Native tribal declaration missing.");
		SaveNamingApplication(app);
		var row = FMDB.Context.Chargens.AsNoTracking().Single(x => x.Id == app.Id);
		app = new Chargen(row, world, account);
		screen = storyboard.GetScreen(app);
		Require(screen.Display().Contains("tribal membership"), "Native Chargen hydration lost pending tribal confirmation.");
		Require(screen.HandleCommand("yes").Contains("display", StringComparison.OrdinalIgnoreCase), "Native explicit tribe-display choice missing.");
		SaveNamingApplication(app);
		row = FMDB.Context.Chargens.AsNoTracking().Single(x => x.Id == app.Id);
		app = new Chargen(row, world, account);
		screen = storyboard.GetScreen(app);
		Require(screen.Display().Contains("display", StringComparison.OrdinalIgnoreCase), "Native Chargen hydration lost display choice.");
		screen.HandleCommand("yes");
		Require(screen.HandleCommand("Fire Bow").Contains("selected parent"), "Native clan picker accepted another tribe's clan.");
		screen.HandleCommand("Dark Moons");
		Require(screen.State == ChargenScreenState.Complete && app.SelectedName.GetName(NameStyle.FullName) == "Agis Night Runner Tribe", "Native composed tribal name differs.");
		Require(app.SelectedName.GetName(NameStyle.FullWithNickname).EndsWith("\"Silt Runner\" \"Dust-Cutter\""), "Native alias boundaries were lost.");
		SaveNamingApplication(app);

		var pterran = world.NameCultures.Get(FMDB.Context.SeederManagedRecords.Single(x => x.StableKey == "arm.naming.pterran").LogicalId!.Value);
		var earned = pterran.GetPersonalName("/elements birthname Ptek earnedname Aandu confirm earnedname affiliation \"Pterran Vale\"", true);
		Require(earned?.GetName(NameStyle.FullName) == "Aandu Ptek", "Native earned-name prefix differs.");
		Require(pterran.GetPersonalName("/elements birthname Ptek earnedname Aandu", true) is null, "Native parser accepted unconfirmed earned name.");
		var earnedApp = NewSettingQualificationApplication(world, account.Id, cultureId, "QualificationEarned");
		earnedApp.SelectedName = earned!;
		SaveNamingApplication(earnedApp);
		world.SaveManager.Flush();
		using var db = NewIndependentContext(database.ConnectionString);
		CheckExecutableNamingRows(database, db);
		Console.WriteLine("SETTING-executable-naming=PASS native-NamePicker optional-entry explicit-confirmations display-choice clan-parent alias-boundaries actual-Chargen-save-hydration earned-prefix no-profile-activation");
	}

	private static void CheckExecutableNamingRows(TestDatabase database, FuturemudDatabaseContext db)
	{
		var rows = db.Chargens.AsNoTracking().Where(x => x.Definition.Contains(NamingQualificationDescription)).ToArray();
		Require(rows.Length == 2, "Executable naming application receipt inventory differs.");
		foreach (var row in rows)
		{
			var xml = XElement.Parse(row.Definition).Element("SelectedName")!.Element("Name")!;
			var cultureId = (long)xml.Attribute("culture")!;
			var model = db.NameCultures.AsNoTracking().Single(x => x.Id == cultureId);
			// Native runtime constructors, with only the world registry mocked: no substitute formatter/parser.
			var world = new Moq.Mock<IFuturemud>();
			world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
			world.SetupGet(x => x.SaveManager).Returns(Moq.Mock.Of<MudSharp.Framework.Save.ISaveManager>());
			var culture = new NameCulture(model, world.Object);
			var name = new PersonalName(culture, xml);
			Require(name.EntryProgress is null, "Completed native application retained unfinished naming progress.");
			if (name.GetName(NameStyle.GivenOnly) == "Agis")
			{
				Require(name.GetName(NameStyle.FullName) == "Agis Night Runner Tribe" && name.GetName(NameStyle.FullWithNickname) == "Agis Night Runner Tribe \"Silt Runner\" \"Dust-Cutter\"", "Cold native tribal/display/alias formatting differs.");
				Require(xml.Elements("Element").Any(x => x.Attribute("usage")!.Value == "ClanName" && x.Value == "Dark Moons") &&
					xml.Elements("Element").Any(x => x.Attribute("usage")!.Value == "Title" && x.Value == "Captain"), "Cold native metadata components were lost.");
			}
			else Require(name.GetName(NameStyle.FullName) == "Aandu Ptek" && xml.Elements("Element").Any(x => x.Attribute("usage")!.Value == "Affiliation" && x.Value == "Pterran Vale"), "Cold native earned-name/community formatting differs.");
		}
		Console.WriteLine("SETTING-executable-naming-cold=PASS two-native-applications exact-metadata confirmations display alias-boundaries earned-prefix");
	}
}
