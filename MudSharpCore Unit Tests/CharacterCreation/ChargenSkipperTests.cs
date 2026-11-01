#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Disfigurements;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

namespace MudSharp_Unit_Tests.CharacterCreation;

[TestClass]
public class ChargenSkipperTests
{
	[TestMethod]
	[DataRow("return AddChargenMissingBodypart(@ch, \"arm\")")]
	[DataRow("return AddChargenProsthesis(@ch, ItemPrototype(1))")]
	[DataRow("return AddChargenScar(@ch, \"arm\", \"Slashing\", \"Moderate\")")]
	[DataRow("return AddChargenScar(@ch, \"arm\", \"Slashing\", \"Moderate\", \"Amputation\")")]
	[DataRow("return AddChargenTattoo(@ch, 5, \"arm\")")]
	[DataRow("var text as text dictionary\nreturn AddChargenTattoo(@ch, 5, \"arm\", @text)")]
	public void DisfigurementFunctions_ChargenSignatures_Compile(string script)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var f = new Fixture();
		var prog = new MudSharp.FutureProg.FutureProg(f.World.Object, "test", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Chargen, "ch")], script);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
	}

	[TestMethod]
	public void DisfigurementFunction_CompiledProg_MutatesChargenAndRejectsDuplicateGrant()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var f = new Fixture();
		var arm = f.Part(1, "arm");
		var prog = new MudSharp.FutureProg.FutureProg(f.World.Object, "test", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Chargen, "ch")], "return AddChargenMissingBodypart(@ch, \"arm\")");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.IsTrue(prog.ExecuteBool(f.Chargen));
		Assert.IsFalse(prog.ExecuteBool(f.Chargen));
		Assert.AreSame(arm, f.Chargen.MissingBodyparts.Single());
	}

	[TestMethod]
	public void GetScreen_NoProgs_CompletesSilentlyAndResetsOrInitialisesLikePickers()
	{
		var f = new Fixture();
		f.Chargen.SelectedMerits.Add(Mock.Of<ICharacterMerit>());
		f.Chargen.SelectedAccents.Add(Mock.Of<IAccent>());
		f.Chargen.SelectedDisfigurements = null!;
		f.Chargen.SelectedTattoos = null!;
		f.Chargen.SelectedScars = null!;
		f.Chargen.MissingBodyparts = null!;
		f.Chargen.SelectedProstheses = null!;
		ChargenScreenStoryboard[] boards = [f.Accents(), f.Quirks(), f.Disfigurements()];
		foreach (var board in boards)
		{
			var screen = board.GetScreen(f.Chargen);
			Assert.AreEqual(ChargenScreenState.Complete, screen.State);
			Assert.AreEqual(board.Stage, screen.AssociatedStage);
			Assert.AreEqual("", screen.Display());
			Assert.AreEqual("", screen.HandleCommand("anything"));
			Assert.IsFalse(board.ChargenCosts(f.Chargen).Any());
		}

		Assert.IsEmpty(f.Chargen.SelectedAccents);
		Assert.IsEmpty(f.Chargen.SelectedMerits);
		Assert.IsNotNull(f.Chargen.SelectedDisfigurements);
		Assert.IsNotNull(f.Chargen.SelectedTattoos);
		Assert.IsNotNull(f.Chargen.SelectedScars);
		Assert.IsNotNull(f.Chargen.MissingBodyparts);
		Assert.IsNotNull(f.Chargen.SelectedProstheses);
	}

	[TestMethod]
	public void AccentSkipper_ProgResultsAndReentry_KeepKnownDistinctGrantsAndSupplyFallbacks()
	{
		var f = new Fixture();
		var first = f.Language(1);
		var second = f.Language(2);
		var unknown = f.Language(3, known: false);
		var automatic = f.Accent(1, first);
		var native = f.Accent(2, second, role: AccentRole.Native);
		f.Accent(3, second);
		var granted = f.Accent(4, first, available: false);
		var invalid = f.Accent(5, unknown);
		f.Chargen.SelectedEthnicity = Mock.Of<IEthnicity>(x => x.NativeLanguage == second);
		f.Chargen.SelectedNativeLanguage = unknown;
		f.Chargen.SelectedAccents.Add(invalid);
		var prog = f.Prog(10, "GiveAccentsProg", ProgVariableTypes.Accent | ProgVariableTypes.Collection);
		prog.Setup(x => x.ExecuteCollection<IAccent>(It.IsAny<object[]>()))
			.Returns([granted, granted, invalid, null!]);
		var board = f.Accents();
		for (var i = 0; i < 2; i++)
		{
			board.GetScreen(f.Chargen);
			CollectionAssert.AreEquivalent(new[] { automatic, granted, native }, f.Chargen.SelectedAccents.ToArray());
			Assert.IsNull(f.Chargen.SelectedNativeLanguage);
		}
		prog.Verify(x => x.ExecuteCollection<IAccent>(It.Is<object[]>(x => x.Length == 1 && x[0] == f.Chargen)), Times.Exactly(2));
	}

	[TestMethod]
	public void AccentSkipper_NoProg_UsesNativeLanguageFallbackAndPreservesValidNativeChoice()
	{
		var f = new Fixture();
		var language = f.Language(1);
		f.Accent(1, language, role: AccentRole.Fallback);
		var native = f.Accent(2, language, role: AccentRole.Native);
		f.Chargen.SelectedNativeLanguage = language;
		f.Accents().GetScreen(f.Chargen);
		Assert.AreSame(language, f.Chargen.SelectedNativeLanguage);
		CollectionAssert.AreEqual(new[] { native }, f.Chargen.SelectedAccents.ToArray());
	}

	[TestMethod]
	public void QuirkSkipper_Reentry_FiltersNonCharacterMeritsAndClearsPreviousGrants()
	{
		var f = new Fixture();
		var merit = Mock.Of<ICharacterMerit>();
		var prog = f.Prog(10, "GiveQuirksProg", ProgVariableTypes.Merit | ProgVariableTypes.Collection);
		prog.Setup(x => x.ExecuteCollection<IMerit>(It.IsAny<object[]>()))
			.Returns([merit, merit, Mock.Of<IMerit>(), null!]);
		f.Quirks().GetScreen(f.Chargen);
		CollectionAssert.AreEqual(new[] { merit }, f.Chargen.SelectedMerits.ToArray());
		prog.Setup(x => x.ExecuteCollection<IMerit>(It.IsAny<object[]>())).Returns((IEnumerable<IMerit>)null!);
		f.Quirks().GetScreen(f.Chargen);
		Assert.IsEmpty(f.Chargen.SelectedMerits);
	}

	[TestMethod]
	public void DisfigurementSkipper_ExistingSelectionsAndHooks_PreservesMigratesAndRunsInSectionOrder()
	{
		var f = new Fixture();
		var part = Mock.Of<IBodypart>();
		var tattoo = Mock.Of<ITattooTemplate>();
		var scar = Mock.Of<IScar>();
		f.Chargen.SelectedDisfigurements.Add((tattoo, part));
		f.Chargen.SelectedScars.Add(scar);
		f.Chargen.MissingBodyparts.Add(part);
		var order = new List<string>();
		var names = new[] { "MissingBodypartsProg", "GiveProsthesesProg", "GiveScarsProg", "GiveTattoosProg" };
		for (var i = 0; i < names.Length; i++)
		{
			var name = names[i];
			f.Prog(i + 1, name, ProgVariableTypes.Void)
				.Setup(x => x.Execute(It.IsAny<object[]>())).Callback(() =>
				{
					Assert.IsNotNull(f.Chargen.SelectedProstheses);
					order.Add(name);
				}).Returns((object)null!);
		}

		var board = f.Disfigurements();
		board.GetScreen(f.Chargen);
		board.GetScreen(f.Chargen);
		CollectionAssert.AreEqual(names.Concat(names).ToArray(), order.ToArray());
		Assert.AreEqual(1, f.Chargen.SelectedTattoos.Count);
		Assert.AreSame(tattoo, f.Chargen.SelectedTattoos[0].Tattoo);
		Assert.AreSame(scar, f.Chargen.SelectedScars.Single());
		Assert.AreSame(part, f.Chargen.MissingBodyparts.Single());
		Assert.IsEmpty(f.Chargen.SelectedDisfigurements);
	}

	[TestMethod]
	[DataRow("AccentSkipper", "GiveAccentsProg", "accents")]
	[DataRow("QuirkSkipper", "GiveQuirksProg", "quirks")]
	[DataRow("DisfigurementSkipper", "MissingBodypartsProg", "missingbodyparts")]
	[DataRow("DisfigurementSkipper", "GiveProsthesesProg", "prostheses")]
	[DataRow("DisfigurementSkipper", "GiveScarsProg", "scars")]
	[DataRow("DisfigurementSkipper", "GiveTattoosProg", "tattoos")]
	public void BuildingCommand_ProgConfiguration_ValidatesAndRoundTripsOptionalProgs(string type, string property, string command)
	{
		var f = new Fixture();
		var returnType = type == "AccentSkipper" ? ProgVariableTypes.Accent | ProgVariableTypes.Collection :
			type == "QuirkSkipper" ? ProgVariableTypes.Merit | ProgVariableTypes.Collection : ProgVariableTypes.Void;
		var prog = f.Prog(10, property, returnType);
		var board = f.Board(type, "<Definition />");
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		Assert.IsFalse(board.BuildingCommand(actor.Object, new StringStack(command)));
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		Assert.IsFalse(board.BuildingCommand(actor.Object, new StringStack($"{command} 10")));
		prog.SetupGet(x => x.ReturnType).Returns(returnType);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(false);
		Assert.IsFalse(board.BuildingCommand(actor.Object, new StringStack($"{command} 10")));
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
			.Returns<IEnumerable<ProgVariableTypes>>(x => x.SequenceEqual([ProgVariableTypes.Chargen]));
		Assert.IsTrue(board.BuildingCommand(actor.Object, new StringStack($"{command} 10")));
		Assert.IsTrue(board.Changed);
		var save = typeof(ChargenScreenStoryboard).GetMethod("SaveDefinition", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
		var xml = (string)save.Invoke(board, null)!;
		Assert.AreEqual("10", XElement.Parse(xml).Element(property)!.Value);
		var loaded = f.Board(type, xml);
		Assert.AreSame(prog.Object, loaded.GetType().GetProperty(property)!.GetValue(loaded));
		Assert.IsTrue(loaded.BuildingCommand(actor.Object, new StringStack($"{command} none")));
		Assert.IsNull(loaded.GetType().GetProperty(property)!.GetValue(loaded));
		Assert.AreEqual("0", XElement.Parse((string)save.Invoke(loaded, null)!).Element(property)!.Value);
	}

	[TestMethod]
	public void AddMissingBodypart_UnknownVitalAndDuplicateParts_RejectsInvalidSelections()
	{
		var f = new Fixture();
		var arm = f.Part(1, "arm");
		f.Part(2, "head", vital: true);
		Assert.IsFalse(ChargenDisfigurementFunction.AddMissingBodypart(f.Chargen, "unknown"));
		Assert.IsFalse(ChargenDisfigurementFunction.AddMissingBodypart(f.Chargen, "head"));
		Assert.IsTrue(ChargenDisfigurementFunction.AddMissingBodypart(f.Chargen, "arm"));
		Assert.IsFalse(ChargenDisfigurementFunction.AddMissingBodypart(f.Chargen, "1"));
		CollectionAssert.AreEqual(new[] { arm }, f.Chargen.MissingBodyparts.ToArray());
	}

	[TestMethod]
	public void AddTattoo_CustomTextAndMissingParts_ValidatesTextAndAvoidsDuplicates()
	{
		var f = new Fixture();
		var arm = f.Part(1, "arm");
		var tattoo = new Mock<ITattooTemplate>();
		tattoo.SetupGet(x => x.Id).Returns(5);
		tattoo.SetupGet(x => x.TextSlots).Returns([new TattooTemplateTextSlot("name", 5, true, null!, null!, default, null!, 0, "", "")]);
		f.Tattoos.Add(tattoo.Object);
		var function = new ChargenDisfigurementFunction([], f.World.Object, "addchargentattoo");
		Assert.IsFalse(function.AddTattoo(f.Chargen, 5, "arm", null));
		Assert.IsFalse(function.AddTattoo(f.Chargen, 5, "arm", new Dictionary<string, IProgVariable> { ["name"] = new TextVariable("too long") }));
		var text = new Dictionary<string, IProgVariable> { ["NAME"] = new TextVariable("Luke") };
		Assert.IsTrue(function.AddTattoo(f.Chargen, 5, "arm", text));
		Assert.IsFalse(function.AddTattoo(f.Chargen, 5, "arm", text));
		Assert.AreEqual("Luke", f.Chargen.SelectedTattoos.Single().TextValues["name"].Text);
		f.Chargen.SelectedTattoos.Clear();
		f.Chargen.MissingBodyparts.Add(arm);
		Assert.IsFalse(function.AddTattoo(f.Chargen, 5, "arm", text));
	}

	[TestMethod]
	public void AddProsthesis_DerivedMissingPart_AcceptsCompatibleTargetAndRejectsEquivalentConflict()
	{
		var f = new Fixture();
		var derived = f.Part(1, "derivedarm");
		var generic = Mock.Of<IBodypart>();
		Mock.Get(derived).Setup(x => x.CountsAs(generic)).Returns(true);
		Mock.Get(derived).Setup(x => x.CountsAs(derived)).Returns(true);
		var body = f.Chargen.SelectedRace.BaseBody;
		Mock.Get(body).Setup(x => x.CountsAs(body)).Returns(true);
		f.Chargen.MissingBodyparts.Add(derived);
		IGameItemProto Item(IBodypart target)
		{
			var proto = (ProstheticGameItemComponentProto)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(ProstheticGameItemComponentProto));
			typeof(ProstheticGameItemComponentProto).GetProperty(nameof(proto.TargetBody))!.SetValue(proto, body);
			typeof(ProstheticGameItemComponentProto).GetProperty(nameof(proto.TargetBodypart))!.SetValue(proto, target);
			var item = new Mock<IGameItemProto>();
			item.Setup(x => x.GetItemType<ProstheticGameItemComponentProto>()).Returns(proto);
			return item.Object;
		}

		Assert.IsTrue(ChargenDisfigurementFunction.AddProsthesis(f.Chargen, Item(generic)));
		Assert.IsFalse(ChargenDisfigurementFunction.AddProsthesis(f.Chargen, Item(derived)));
		Assert.AreEqual(1, f.Chargen.SelectedProstheses.Count);
	}

	[TestMethod]
	public void AddScar_WoundAndSurgicalCauses_GeneratesScarsAndRejectsInvalidOrDuplicateSelections()
	{
		var f = new Fixture();
		var arm = f.Part(1, "arm");
		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.CurrentDateTime).Returns(MudDateTime.Never);
		f.Chargen.SelectedCulture = Mock.Of<ICulture>(x => x.PrimaryCalendar == calendar.Object);
		f.World.Setup(x => x.GetStaticConfiguration("ScarOrientationByBodypartShape"))
			.Returns("<Orientations Default=\"linear\" />");
		var function = new ChargenDisfigurementFunction([], f.World.Object, "addchargenscar");
		Assert.IsFalse(function.AddScar(f.Chargen, "arm", "invalid", "Moderate", null));
		Assert.IsFalse(function.AddScar(f.Chargen, "arm", "Slashing", "None", null));
		Assert.IsFalse(function.AddScar(f.Chargen, "arm", "Slashing", "Moderate", "invalid"));
		Assert.IsTrue(function.AddScar(f.Chargen, "arm", "Slashing", "Moderate", null));
		Assert.IsFalse(function.AddScar(f.Chargen, "arm", "Slashing", "Moderate", null));
		Assert.IsTrue(function.AddScar(f.Chargen, "arm", "Slashing", "Moderate", "Amputation"));
		Assert.AreEqual(2, f.Chargen.SelectedScars.Count);
		Assert.IsFalse(f.Chargen.SelectedScars[0].IsSurgical);
		Assert.IsTrue(f.Chargen.SelectedScars[1].IsSurgical);
		Assert.AreSame(arm, f.Chargen.SelectedScars[0].Bodypart);
		f.Chargen.MissingBodyparts.Add(arm);
		Assert.IsFalse(function.AddScar(f.Chargen, "arm", "Slashing", "Severe", null));
	}

	[TestMethod]
	[DoNotParallelize]
	public void Storyboard_SwapToSkippersAndBack_PersistsRegisteredTypesWithUsableDefaults()
	{
		using var db = new InMemoryDatabaseTestScope();
		var f = new Fixture();
		var stages = new[] { ChargenStage.SelectAccents, ChargenStage.SelectMerits, ChargenStage.SelectDisfigurements };
		var types = new[] { "AccentSkipper", "QuirkSkipper", "DisfigurementSkipper" };
		for (var i = 0; i < stages.Length; i++)
		{
			db.Context.ChargenScreenStoryboards.Add(new MudSharp.Models.ChargenScreenStoryboard
			{
				Id = i + 1, ChargenStage = (int)stages[i], ChargenType = types[i], StageDefinition = "<Definition />", Order = i * 10
			});
		}
		db.Context.SaveChanges();
		var graph = new ChargenStoryboard(f.World.Object);
		var pickers = new[] { "AccentPicker", "QuirkPicker", "DisfigurementPicker" };
		for (var i = 0; i < stages.Length; i++)
		{
			graph.SwapStoryboard(stages[i], pickers[i]);
			Assert.AreEqual(pickers[i], db.Context.ChargenScreenStoryboards.Find(i + 1L)!.ChargenType);
			graph.SwapStoryboard(stages[i], types[i]);
		}
		graph.SwapStoryboard(ChargenStage.SelectMerits, "MeritPicker");
		graph.SwapStoryboard(ChargenStage.SelectMerits, "QuirkSkipper");
		var reloaded = new ChargenStoryboard(f.World.Object);
		CollectionAssert.AreEquivalent(types, reloaded.StageScreenMap.Values.Select(x => x.Name).ToArray());
		foreach (var stage in stages) Assert.AreEqual(ChargenScreenState.Complete, reloaded.StageScreenMap[stage].GetScreen(f.Chargen).State);
	}

	internal sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IChargen> Application { get; } = new();
		public IChargen Chargen => Application.Object;
		public All<IFutureProg> Progs { get; } = new();
		public All<ILanguage> Languages { get; } = new();
		public All<IAccent> AccentsList { get; } = new();
		public RevisableAll<IDisfigurementTemplate> Tattoos { get; } = new();
		private readonly Mock<IRace> _race = new();
		private readonly List<IBodypart> _parts = [];

		public Fixture()
		{
			Application.SetupAllProperties();
			Application.SetupGet(x => x.GetObject).Returns(() => Chargen);
			Chargen.SelectedSkills = []; Chargen.SkillValues = []; Chargen.SelectedAccents = []; Chargen.SelectedMerits = [];
			Chargen.SelectedDisfigurements = []; Chargen.SelectedTattoos = []; Chargen.SelectedScars = [];
			Chargen.MissingBodyparts = []; Chargen.SelectedProstheses = [];
			Chargen.SelectedRace = _race.Object;
			var body = new Mock<IBodyPrototype>();
			body.Setup(x => x.BodypartsFor(_race.Object, It.IsAny<MudSharp.Form.Shape.Gender>())).Returns(_parts);
			_race.SetupGet(x => x.BaseBody).Returns(body.Object);
			World.SetupGet(x => x.FutureProgs).Returns(Progs);
			World.SetupGet(x => x.Languages).Returns(Languages);
			World.SetupGet(x => x.Accents).Returns(AccentsList);
			World.SetupGet(x => x.DisfigurementTemplates).Returns(Tattoos);
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		}

		public Mock<IFutureProg> Prog(long id, string name, ProgVariableTypes type)
		{
			var prog = new Mock<IFutureProg>();
			prog.SetupGet(x => x.Id).Returns(id);
			prog.SetupGet(x => x.Name).Returns(name);
			prog.SetupGet(x => x.FunctionName).Returns(name);
			prog.SetupGet(x => x.ReturnType).Returns(type);
			prog.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Chargen]);
			prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
				.Returns<IEnumerable<ProgVariableTypes>>(x => x.SequenceEqual([ProgVariableTypes.Chargen]));
			Progs.Add(prog.Object);
			return prog;
		}

		public ILanguage Language(long id, bool known = true)
		{
			var language = new Mock<ILanguage>();
			language.SetupGet(x => x.Id).Returns(id);
			language.SetupGet(x => x.LinkedTrait).Returns(Mock.Of<ITraitDefinition>());
			Languages.Add(language.Object);
			if (known) Chargen.SelectedSkills.Add(language.Object.LinkedTrait);
			return language.Object;
		}

		public IAccent Accent(long id, ILanguage language, bool available = true, AccentRole role = AccentRole.Fallback)
		{
			var accent = new Mock<IAccent>();
			accent.SetupGet(x => x.Id).Returns(id);
			accent.SetupGet(x => x.Language).Returns(language);
			accent.SetupGet(x => x.Role).Returns(role);
			accent.Setup(x => x.IsAvailableInChargen(Chargen)).Returns(available);
			AccentsList.Add(accent.Object);
			return accent.Object;
		}

		public IBodypart Part(long id, string name, bool vital = false)
		{
			var part = new Mock<IExternalBodypart>();
			part.SetupGet(x => x.Id).Returns(id);
			part.SetupGet(x => x.Name).Returns(name);
			part.SetupGet(x => x.CanSever).Returns(true);
			part.SetupGet(x => x.IsVital).Returns(vital);
			_parts.Add(part.Object);
			return part.Object;
		}

		private string Definition => new XElement("Definition", Progs.Select(x => new XElement(x.Name, x.Id))).ToString();
		public AccentSkipperScreenStoryboard Accents() => (AccentSkipperScreenStoryboard)Board("AccentSkipper", Definition);
		public QuirkSkipperScreenStoryboard Quirks() => (QuirkSkipperScreenStoryboard)Board("QuirkSkipper", Definition);
		public DisfigurementSkipperScreenStoryboard Disfigurements() => (DisfigurementSkipperScreenStoryboard)Board("DisfigurementSkipper", Definition);
		public ChargenScreenStoryboard Board(string type, string xml)
		{
			var row = new MudSharp.Models.ChargenScreenStoryboard { StageDefinition = xml };
			return type switch
			{
				"AccentSkipper" => new AccentSkipperScreenStoryboard(World.Object, row),
				"QuirkSkipper" => new QuirkSkipperScreenStoryboard(World.Object, row),
				_ => new DisfigurementSkipperScreenStoryboard(World.Object, row)
			};
		}
	}
}
