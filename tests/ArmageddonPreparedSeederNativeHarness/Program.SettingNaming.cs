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

	private static void QualifyExecutableNaming(TestDatabase database, IFuturemud world)
	{
		// The producer owns one loaded world (including chargen); cold readers are separate processes.
		using var session = new FMDB();
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
		screen.HandleCommand("Siltwalker");
		SaveNamingApplication(app);
		var row = FMDB.Context.Chargens.AsNoTracking().Single(x => x.Id == app.Id);
		app = new Chargen(row, world, account);
		screen = storyboard.GetScreen(app);
		Require(screen.Display().Contains("Nickname"), "Native Chargen hydration lost the optional nickname step.");
		screen.HandleCommand("\"Silt Runner\"");
		Require(screen.State == ChargenScreenState.Complete && app.SelectedName.GetName(NameStyle.FullName) == "Agis Siltwalker", "Native given/family name differs.");
		Require(app.SelectedName.GetName(NameStyle.FullWithNickname) == "Agis Siltwalker \"Silt Runner\"", "Native multiword nickname was lost.");
		SaveNamingApplication(app);

		var pterran = world.NameCultures.Get(FMDB.Context.SeederManagedRecords.Single(x => x.StableKey == "arm.naming.pterran").LogicalId!.Value);
		var given = pterran.GetPersonalName("/elements birthname Ptek", true);
		Require(given?.GetName(NameStyle.FullName) == "Ptek" && given.GetName(NameStyle.SurnameOnly) == "Ptek", "Optional names did not fall back to the given name.");
		var givenApp = NewSettingQualificationApplication(world, account.Id, cultureId, "QualificationGiven");
		givenApp.SelectedName = given!;
		SaveNamingApplication(givenApp);
		world.SaveManager.Flush();
		using var db = NewIndependentContext(database.ConnectionString);
		CheckExecutableNamingRows(database, db);
		Console.WriteLine("SETTING-executable-naming=PASS native-NamePicker given-family-nickname actual-Chargen-save-hydration optional-name-fallback");
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
				Require(name.GetName(NameStyle.FullName) == "Agis Siltwalker" && name.GetName(NameStyle.FullWithNickname) == "Agis Siltwalker \"Silt Runner\"", "Cold native given/family/nickname formatting differs.");
			else Require(name.GetName(NameStyle.FullName) == "Ptek" && name.GetName(NameStyle.SurnameOnly) == "Ptek", "Cold native optional-name fallback differs.");
		}
		Console.WriteLine("SETTING-executable-naming-cold=PASS two-native-applications given-family-nickname optional-name-fallback");
	}
}
