#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.NPC;
using MudSharp.NPC.Templates;
using MudSharp.Magic.Capabilities;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;
using MudSharp.Economy;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Groups;
using MudSharp.Magic;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
[DoNotParallelize]
public class BuilderWorkflowRegressionTests
{
	[TestMethod]
	[DataRow("+tavern", true)]
	[DataRow("-tavern", false)]
	public void ItemGroupFilter_CombinesNamesAndAllFormsWithoutDroppingEmptyGroups(string filter, bool positive)
	{
		var world = World();
		var actor = Actor(world.Object);
		var matchingForm = Mock.Of<IGameItemGroupForm>(x => x.Describe(actor.Object, It.IsAny<IEnumerable<IGameItem>>()) == "in a tavern");
		var otherForm = Mock.Of<IGameItemGroupForm>(x => x.Describe(actor.Object, It.IsAny<IEnumerable<IGameItem>>()) == "in a garden");
		var groups = new All<IGameItemGroup>();
		groups.Add(Mock.Of<IGameItemGroup>(x => x.Id == 1 && x.Name == "TavernNameOnly" && x.Forms == Array.Empty<IGameItemGroupForm>()));
		groups.Add(Mock.Of<IGameItemGroup>(x => x.Id == 2 && x.Name == "MixedForms" && x.Forms == new[] { matchingForm, otherForm }));
		groups.Add(Mock.Of<IGameItemGroup>(x => x.Id == 3 && x.Name == "EmptyGroup" && x.Forms == Array.Empty<IGameItemGroupForm>()));
		world.SetupGet(x => x.ItemGroups).Returns(groups);
		world.SetupGet(x => x.ItemProtos).Returns(new RevisableAll<IGameItemProto>());
		string? table = null;
		Mock.Get(actor.Object.OutputHandler).Setup(x => x.Send(It.IsAny<string>(), true, false))
			.Callback<string, bool, bool>((text, _, _) => table = text).Returns(true);
		Invoke(typeof(ItemBuilderModule), "ItemGroupList", actor.Object, new StringStack(filter));
		Assert.IsNotNull(table);
		Assert.AreEqual(positive, table.Contains("TavernNameOnly"));
		Assert.AreEqual(positive, table.Contains("MixedForms"));
		Assert.AreEqual(!positive, table.Contains("EmptyGroup"));
	}

	[TestMethod]
	[DataRow(true, "first", "second", "first\nsecond")]
	[DataRow(true, "first\n", "second", "first\nsecond")]
	[DataRow(true, "first", "", "first")]
	[DataRow(false, "first", "second", "second")]
	public void ProgText_EditorSubmission_PreservesAppendAndReplaceSemantics(bool append, string initial, string entered, string expected)
	{
		var actor = Actor(new Mock<IFuturemud>().Object);
		var prog = new Mock<IFutureProg>();
		prog.SetupProperty(x => x.FunctionText, initial);
		prog.SetupGet(x => x.FunctionName).Returns("test");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Void);
		prog.SetupGet(x => x.AcceptsAnyParameters).Returns(true);
		prog.Setup(x => x.Compile()).Returns(true);
		Action<string, IOutputHandler, object[]>? submit = null;
		Action<IOutputHandler, object[]>? cancel = null;
		actor.Setup(x => x.EditorMode(It.IsAny<Action<string, IOutputHandler, object[]>>(), It.IsAny<Action<IOutputHandler, object[]>>(),
			It.IsAny<double>(), It.IsAny<string>(), It.IsAny<EditorOptions>(), It.IsAny<object[]>()))
			.Callback<Action<string, IOutputHandler, object[]>, Action<IOutputHandler, object[]>, double, string, EditorOptions, object[]>(
				(post, abort, _, _, _, _) => { submit = post; cancel = abort; });
		Invoke(typeof(ProgModule), "ProgSetText", actor.Object, prog.Object, new StringStack(""), append);
		Assert.IsNotNull(cancel);
		cancel(actor.Object.OutputHandler, []);
		Assert.AreEqual(initial, prog.Object.FunctionText);
		Assert.IsNotNull(submit);
		submit(entered, actor.Object.OutputHandler, []);
		Assert.AreEqual(expected, prog.Object.FunctionText);
		prog.Verify(x => x.Compile(), Times.Once);
	}

	[TestMethod]
	[DataRow("10")]
	[DataRow("Test Route")]
	[DataRow("\"Test Route\"")]
	[DataRow("#10r2")]
	[DataRow("10 2")]
	public void RouteApproval_CurrentAndPending_SelectsPendingRevision(string selector)
	{
		var current = Route(10, 1, RevisionStatus.Current);
		var pending = Route(10, 2, RevisionStatus.PendingRevision);
		Assert.AreSame(pending, VehicleModule.ResolveVehicleRouteRevisionForApproval([current, pending], selector));
		Assert.IsNull(VehicleModule.ResolveVehicleRouteRevisionForApproval([current, pending], "10r1"));
	}

	[TestMethod]
	public void RouteEditing_ExistingDraftAndAmbiguousNames_UsesDraftWithoutChoosingAnUnrelatedRecord()
	{
		var current = Route(10, 1, RevisionStatus.Current);
		var draft = Route(10, 2, RevisionStatus.UnderDesign);
		Assert.AreSame(draft, VehicleModule.ResolveVehicleRouteRevisionForEditing([current, draft], "10"));
		Assert.AreSame(current, VehicleModule.ResolveVehicleRouteRevisionForEditing([current, draft], "10 1"));
		Assert.IsNull(VehicleModule.ResolveVehicleRouteRevisionForEditing([current, draft, Route(20, 1, RevisionStatus.Current)], "Test Route"));
		Assert.IsNull(VehicleModule.ResolveVehicleRouteRevisionForEditing([current, draft], ""));
	}

	[TestMethod]
	public void RouteSelectors_NumericLeadingName_ResolvesExactRoute()
	{
		var route = Mock.Of<IVehicleRoute>(x => x.Id == 10 && x.RevisionNumber == 2 && x.Status == RevisionStatus.PendingRevision && x.Name == "42 Express");
		Assert.AreSame(route, VehicleModule.ResolveVehicleRouteRevisionForEditing([route], "42 Express"));
		Assert.AreSame(route, VehicleModule.ResolveVehicleRouteRevisionForApproval([route], "42 Express"));
	}

	[TestMethod]
	[DataRow("\"\"")]
	[DataRow("unknown")]
	public void Lists_UnsupportedFilters_ReturnWithoutMutation(string filter)
	{
		var world = World(); world.SetupGet(x => x.Clocks).Returns(new All<IClock>());
		var actor = Actor(world.Object);
		BaseBuilderModule.GenericList(actor.Object, new StringStack(filter), EditableItemHelper.ClockHelper);
		actor.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never);
	}

	[TestMethod]
	public void Timezone_CreateLegacyUpdateAndOrdinaryEdit_ReachTheirDistinctFlows()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = World(); var actor = Actor(world.Object);
		BuilderEditingEffect<IMudTimeZone>? editing = null;
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect => editing = (BuilderEditingEffect<IMudTimeZone>)effect);
		actor.Setup(x => x.CombinedEffectsOfType<BuilderEditingEffect<IMudTimeZone>>())
			.Returns(() => editing is null ? [] : new[] { editing });
		var clock = new Mock<IClock>();
		clock.SetupGet(x => x.Id).Returns(1);
		clock.SetupGet(x => x.Gameworld).Returns(world.Object);
		clock.SetupGet(x => x.Name).Returns("Clock");
		var zones = new All<IMudTimeZone>(); clock.SetupGet(x => x.Timezones).Returns(zones);
		clock.Setup(x => x.AddTimezone(It.IsAny<IMudTimeZone>())).Callback<IMudTimeZone>(x => zones.Add(x));
		var clocks = new All<IClock>(); clocks.Add(clock.Object); world.SetupGet(x => x.Clocks).Returns(clocks);
		Invoke(typeof(TimeModule), "Timezone", actor.Object, "timezone edit new 1 TST \"Test Zone\" 2 30");
		var zone = zones.Single();
		Assert.AreEqual(2, zone.OffsetHours); Assert.AreEqual(30, zone.OffsetMinutes);
		Invoke(typeof(TimeModule), "Timezone", actor.Object, "timezone edit 1 TST \"Updated Zone\" 3 15");
		Assert.AreEqual("Updated Zone", zone.Description); Assert.AreEqual(3, zone.OffsetHours);
		Invoke(typeof(TimeModule), "Timezone", actor.Object, $"timezone edit {zone.Id}");
		actor.Verify(x => x.AddEffect(It.Is<IEffect>(e => e is BuilderEditingEffect<IMudTimeZone> && ((BuilderEditingEffect<IMudTimeZone>)e).EditingItem == zone)), Times.Exactly(2));
		((MudTimeZone)zone).Save(); database.Context.SaveChanges(); database.Context.ChangeTracker.Clear();
		Assert.AreEqual(15, database.Context.Timezones.Single().OffsetMinutes);
	}

	[TestMethod]
	public void VehiclePrototypeShow_NoSelector_UsesSelectedPrototype()
	{
		var world = new Mock<IFuturemud>();
		var actor = Actor(world.Object);
		var other = new Mock<IVehiclePrototype>();
		var selected = new Mock<IVehiclePrototype>();
		var prototypes = new RevisableAll<IVehiclePrototype>();
		other.SetupGet(x => x.Id).Returns(1);
		selected.SetupGet(x => x.Id).Returns(2);
		prototypes.Add(other.Object); prototypes.Add(selected.Object);
		world.SetupGet(x => x.VehiclePrototypes).Returns(prototypes);
		actor.Setup(x => x.EditingItem<IVehiclePrototype>()).Returns(selected.Object);
		Invoke(typeof(VehicleModule), "VehicleProtoShow", actor.Object, new StringStack(""));
		selected.Verify(x => x.Show(actor.Object), Times.Once);
		other.Verify(x => x.Show(It.IsAny<ICharacter>()), Times.Never);
	}

	[TestMethod]
	public void MultiSpawner_TemplateWeights_EditRemoveAndRoundTripWithoutReusingTheSelector()
	{
		var world = World();
		var templates = new RevisableAll<INPCTemplate>();
		templates.Add(Mock.Of<INPCTemplate>(x => x.Id == 5 && x.Name == "City Guard"));
		world.SetupGet(x => x.NpcTemplates).Returns(templates);
		var actor = Actor(world.Object).Object;
		var model = new MudSharp.Models.NPCSpawner { Id = 1, Name = "Spawner", SpawnStrategy = (int)SpawnStrategy.Multi, Definition = "<Definition><Templates /></Definition>" };
		var spawner = new NPCSpawner(model, world.Object);
		Assert.IsTrue(spawner.BuildingCommand(actor, new StringStack("npc \"City Guard\" 2")));
		Assert.IsFalse(spawner.BuildingCommand(actor, new StringStack("npc 5 NaN")));
		Assert.IsFalse(spawner.BuildingCommand(actor, new StringStack("npc 5")));
		model.Definition = spawner.SaveDefinition();
		spawner = new NPCSpawner(model, world.Object);
		Assert.AreEqual(2.0, (double)XElement.Parse(spawner.SaveDefinition()).Element("Templates")!.Element("Template")!.Attribute("weight")!);
		Assert.IsTrue(spawner.BuildingCommand(actor, new StringStack("npc 5 3")));
		Assert.IsTrue(spawner.BuildingCommand(actor, new StringStack("npc \"City Guard\" remove")));
		Assert.IsFalse(XElement.Parse(spawner.SaveDefinition()).Element("Templates")!.Elements().Any());
	}

	[TestMethod]
	public void LanguageMutual_QuotedTarget_ChangesOnlyTheSelectedDirectionAndSurvivesReload()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = World();
		var traits = new All<ITraitDefinition>(); traits.Add(Mock.Of<ITraitDefinition>(x => x.Id == 1));
		world.SetupGet(x => x.Traits).Returns(traits);
		var models = new All<ILanguageDifficultyModel>(); models.Add(Mock.Of<ILanguageDifficultyModel>(x => x.Id == 1));
		world.SetupGet(x => x.LanguageDifficultyModels).Returns(models);
		var languages = new All<ILanguage>(); world.SetupGet(x => x.Languages).Returns(languages);
		var a = new MudSharp.Models.Language { Id = 1, Name = "Local", LinkedTraitId = 1, DifficultyModel = 1, UnknownLanguageDescription = "unknown" };
		var b = new MudSharp.Models.Language { Id = 2, Name = "Neighbour Speech", LinkedTraitId = 1, DifficultyModel = 1, UnknownLanguageDescription = "unknown" };
		database.Context.Languages.AddRange(a, b); database.Context.SaveChanges();
		var local = new Language(a, world.Object); var neighbour = new Language(b, world.Object);
		languages.Add(local); languages.Add(neighbour);
		var actor = Actor(world.Object).Object;
		Assert.IsTrue(local.BuildingCommand(actor, new StringStack("mutual \"Neighbour Speech\" Easy")));
		Assert.AreEqual(Difficulty.Easy, local.MutualIntelligability(neighbour));
		Assert.AreEqual(Difficulty.Impossible, neighbour.MutualIntelligability(local));
		Assert.IsFalse(local.BuildingCommand(actor, new StringStack("mutual 2 garbage")));
		local.Save(); database.Context.SaveChanges(); database.Context.ChangeTracker.Clear();
		var saved = database.Context.Languages.Include(x => x.MutualIntelligabilitiesListenerLanguage).Single(x => x.Id == 1);
		Assert.AreEqual(Difficulty.Easy, new Language(saved, world.Object).MutualIntelligability(neighbour));
	}

	[TestMethod]
	public void CombatMessage_AuxiliaryRestrictions_AddResaveAndRemovePersistWithoutDuplicateJoins()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = World();
		var row = new MudSharp.Models.CombatMessage { Id = 1, Message = "message", FailureMessage = "failure", Type = (int)BuiltInCombatMoveType.AuxiliaryMove };
		database.Context.CombatMessages.Add(row); database.Context.SaveChanges();
		var message = new CombatMessage(row, world.Object);
		message.AuxiliaryActionIds.Add(7);
		message.Save(); database.Context.SaveChanges();
		message.Save(); database.Context.SaveChanges();
		Assert.AreEqual(1, database.Context.CombatMessagesCombatActions.Count());
		message.AuxiliaryActionIds.Clear();
		message.Save(); database.Context.SaveChanges(); database.Context.ChangeTracker.Clear();
		var saved = database.Context.CombatMessages.Include(x => x.CombatMessagesCombatActions).Single();
		Assert.AreEqual(0, new CombatMessage(saved, world.Object).AuxiliaryActionIds.Count);
	}

	[TestMethod]
	public void RandomNameDice_QuotedElement_RejectsMalformedDiceAndPersistsValidExpression()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = World();
		var culture = new Mock<INameCulture>();
		culture.SetupGet(x => x.Gameworld).Returns(world.Object);
		culture.SetupGet(x => x.NameCultureElements).Returns(new[] { new NameCultureElement { Name = "Given Name", Usage = NameUsage.BirthName } });
		var row = new MudSharp.Models.RandomNameProfile { Id = 1, Name = "Names", NameCultureId = 1 };
		database.Context.RandomNameProfiles.Add(row); database.Context.SaveChanges();
		var profile = new RandomNameProfile(row, culture.Object);
		var actor = Actor(world.Object).Object;
		Assert.IsTrue(profile.BuildingCommand(actor, new StringStack("dice \"Given Name\" 1d2")));
		Assert.IsFalse(profile.BuildingCommand(actor, new StringStack("dice \"Given Name\" bad-dice")));
		Assert.AreEqual("1d2", profile.NameUsageDiceExpressions[NameUsage.BirthName]);
		profile.Save(); database.Context.SaveChanges(); database.Context.ChangeTracker.Clear();
		var saved = database.Context.RandomNameProfiles.Include(x => x.RandomNameProfilesDiceExpressions).Single();
		Assert.AreEqual("1d2", new RandomNameProfile(saved, culture.Object).NameUsageDiceExpressions[NameUsage.BirthName]);
	}

	[TestMethod]
	public void CapabilityPromptVisibility_ToggleAndReload_PreservesBothValuesWithLegacyDefault()
	{
		var fixture = new MagicCastingFixture();
		var capability = fixture.Earth;
		Assert.IsTrue(capability.ShowMagicResourcesInPrompt);
		foreach (var expected in new[] { false, true })
		{
			Assert.IsTrue(capability.BuildingCommand(fixture.Actor.Object, new StringStack("showpower")));
			var row = new MudSharp.Models.MagicCapability { Id = capability.Id, Name = capability.Name, MagicSchoolId = 1,
				PowerLevel = 1, CapabilityModel = "skilllevel", Definition = capability.SaveToXml() };
			capability = (SkillLevelBasedMagicCapability)MagicCapabilityFactory.LoadCapability(row, fixture.World.Object);
			Assert.AreEqual(expected, capability.ShowMagicResourcesInPrompt);
		}
	}

	[TestMethod]
	public void GenericBuilders_UnsupportedCloneAndUnknownReviewSelector_DoNotInvokeMutationOrChooseARecord()
	{
		var world = World(); var actor = Actor(world.Object);
		BaseBuilderModule.GenericClone(actor.Object, new StringStack("anything"), EditableItemHelper.BankHelper);
		BaseBuilderModule.GenericClone(actor.Object, new StringStack("anything"), EditableItemHelper.PlaneHelper);
		var command = new StringStack("mine"); command.PopSpeech();
		BaseBuilderModule.GenericReviewDefault(actor.Object, command, EditableRevisableItemHelper.ItemSkinHelper);
		world.VerifyGet(x => x.ItemSkins, Times.Never);
		actor.Verify(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	internal static void Invoke(Type type, string method, params object[] args) =>
		type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args);

	internal static Mock<IFuturemud> World()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		return world;
	}

	internal static Mock<ICharacter> Actor(IFuturemud world)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(world);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.Id == 1 && x.LineFormatLength == 120));
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		actor.SetupGet(x => x.LineFormatLength).Returns(120);
		return actor;
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SkinClone_PermissionsAndNewPrivateDraft_PersistIndependently(bool owner)
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = World(); var actor = Actor(world.Object);
		var progs = new All<IFutureProg>(); progs.Add(Mock.Of<IFutureProg>(x => x.Id == 1));
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		var prototypes = new RevisableAll<IGameItemProto>();
		prototypes.Add(Mock.Of<IGameItemProto>(x => x.Id == 1 && x.Status == RevisionStatus.Current && x.PermitPlayerSkins));
		world.SetupGet(x => x.ItemProtos).Returns(prototypes);
		var row = new MudSharp.Models.GameItemSkin { Id = 1, RevisionNumber = 2, Name = "Original", ItemProtoId = 1, CanUseSkinProgId = 1, IsPublic = true,
			FullDescription = "Description", EditableItem = new MudSharp.Models.EditableItem { BuilderAccountId = owner ? 1 : 2, RevisionStatus = (int)RevisionStatus.Current } };
		database.Context.GameItemSkins.Add(row); database.Context.SaveChanges();
		var skin = new GameItemSkin(row, world.Object);
		var skins = new RevisableAll<IGameItemSkin>(); skins.Add(skin);
		world.SetupGet(x => x.ItemSkins).Returns(skins);
		world.Setup(x => x.Add(It.IsAny<IGameItemSkin>())).Callback<IGameItemSkin>(x => skins.Add(x));
		BaseBuilderModule.GenericRevisableBuildingCommand(actor.Object, new StringStack("clone \"\" WrongSource"), EditableRevisableItemHelper.ItemSkinHelper);
		Assert.AreEqual(1, database.Context.GameItemSkins.Count());
		BaseBuilderModule.GenericRevisableBuildingCommand(actor.Object, new StringStack("clone Original Copy"), EditableRevisableItemHelper.ItemSkinHelper);
		Assert.AreEqual(owner ? 2 : 1, database.Context.GameItemSkins.Count());
		if (!owner) return;
		var clone = database.Context.GameItemSkins.Include(x => x.EditableItem).Single(x => x.Id != 1);
		Assert.AreEqual("Copy", clone.Name);
		Assert.AreEqual(0, clone.RevisionNumber);
		Assert.AreEqual(1L, clone.EditableItem.BuilderAccountId);
		Assert.AreEqual((int)RevisionStatus.UnderDesign, clone.EditableItem.RevisionStatus);
		Assert.IsFalse(clone.IsPublic);
		Assert.AreEqual("Description", clone.FullDescription);
		Assert.AreEqual("Original", row.Name);
		Assert.AreEqual((int)RevisionStatus.Current, row.EditableItem.RevisionStatus);
	}

	[TestMethod]
	public void MagicSchoolClone_SavesIndependentIdentityAndRequestedName()
	{
		using var database = new InMemoryDatabaseTestScope();
		var world = World();
		var row = new MudSharp.Models.MagicSchool { Id = 1, Name = "Original", SchoolVerb = "cast", SchoolAdjective = "arcane", PowerListColour = "green" };
		database.Context.MagicSchools.Add(row); database.Context.SaveChanges();
		var source = new MagicSchool(row, world.Object);
		var clone = source.Clone("New School");
		Assert.AreNotEqual(source.Id, clone.Id);
		database.Context.ChangeTracker.Clear();
		Assert.AreEqual("New School", new MagicSchool(database.Context.MagicSchools.Single(x => x.Id == clone.Id), world.Object).Name);
		Assert.AreEqual("Original", database.Context.MagicSchools.Single(x => x.Id == 1).Name);
	}

	[TestMethod]
	[DataRow("programming \"control unit\" logic test")]
	[DataRow("programming item \"control unit\" logic test")]
	public void Programming_QuotedTarget_PreservesTargetAcrossBothCommandForms(string input)
	{
		var world = World(); world.Setup(x => x.GetStaticBool("ProgrammingCommandEnabled")).Returns(true);
		var actor = Actor(world.Object);
		Invoke(typeof(ElectronicsModule), "Programming", actor.Object, input);
		actor.Verify(x => x.TargetItem("control unit"), Times.Once);
		actor.Verify(x => x.TargetItem("control"), Times.Never);
	}

	[TestMethod]
	[DataRow("accept", true)]
	[DataRow("reject", false)]
	[DataRow("expire", false)]
	public void ShopAccountClosure_ProposalMutatesOnlyOnAcceptance(string decision, bool removed)
	{
		var field = typeof(Accept).GetField("_standardAcceptPhrasing", BindingFlags.Static | BindingFlags.NonPublic)!;
		var previous = field.GetValue(null);
		try
		{
			field.SetValue(null, "Accept or reject.");
			var actor = Actor(World().Object); var shop = new Mock<IShop>();
			var account = Mock.Of<ILineOfCreditAccount>(x => x.AccountName == "Trade Credit");
			shop.SetupGet(x => x.LineOfCreditAccounts).Returns([account]);
			Accept? proposal = null;
			actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect => proposal = (Accept)effect);
			Invoke(typeof(EconomyModule), "ShopAccountClose", actor.Object, shop.Object, new StringStack("\"Trade Credit\""));
			Assert.IsNotNull(proposal);
			shop.Verify(x => x.RemoveLineOfCredit(account), Times.Never);
			switch (decision)
			{
				case "accept": proposal.Proposal.Accept(""); break;
				case "reject": proposal.Proposal.Reject(""); break;
				default: proposal.Proposal.Expire(); break;
			}
			shop.Verify(x => x.RemoveLineOfCredit(account), removed ? Times.Once : Times.Never);
		}
		finally { field.SetValue(null, previous); }
	}

	private static IVehicleRoute Route(long id, int revision, RevisionStatus status) =>
		Mock.Of<IVehicleRoute>(x => x.Id == id && x.RevisionNumber == revision && x.Status == status && x.Name == "Test Route");
}
