using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.Utilities.Chargen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.NPC.Templates;
using F = MudSharp_Unit_Tests.ChargenSkillSelectionGroupTests.Fixture;
using Group = MudSharp.CharacterCreation.ChargenSkillSelectionGroup;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargenSkillSelectionGroupIntegrationTests
{
	private static T RequiredStrings<T>(T entity) where T : class
	{
		foreach (var property in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string) && x.CanWrite && x.GetValue(entity) is null)) property.SetValue(entity, "");
		return entity;
	}
	private static void Set(object target, string name, object value) => target.GetType().GetProperty(name)!.SetValue(target, value);
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void SG17_SG18_SeedBeforeOrAfterChargen_UpsertsAndOffersWorkingGroup(bool groupFirst)
	{
		using var context = Context(); var f = new F();
		context.Accounts.Add(RequiredStrings(new MudSharp.Models.Account { Id = 1, Name = "Fixture" }));
		context.Races.Add(RequiredStrings(new MudSharp.Models.Race { Id = 1, Name = "Human", AllowedGenders = "Male Female" }));
		context.FutureProgs.Add(RequiredStrings(new MudSharp.Models.FutureProg { Id = 1, FunctionName = "AlwaysTrue", FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() }));
		var traits = f.Skills.Select(x => RequiredStrings(new MudSharp.Models.TraitDefinition { Id = x.Id, Name = x.Name, Type = (int)TraitType.Skill })).ToList();
		context.TraitDefinitions.AddRange(traits); context.SaveChanges();
		var definition = new SkillGroupSeedDefinition("fixture-household", "Household Training", "The household teaches useful crafts.", 2, 2, 1, ExistingSkillPolicy.NewOnly, true, traits, f.Eligibility.Object);
		var conflicts = new List<string>();
		void SeedGroup() => ChargenSkillSelectionGroupSeeder.Upsert(context, definition, conflicts);
		if (groupFirst) SeedGroup();
		new ChargenSeeder().SeedData(context, new Dictionary<string, string> { ["rpp"] = "no", ["bp"] = "no", ["class"] = "no", ["role-first"] = "race", ["attributemode"] = "order", ["skillmode"] = "picker", ["merits"] = "merit", ["customdescs"] = "no" });
		SeedGroup();
		var dbgroup = context.ChargenSkillSelectionGroups.Include(x => x.Members).Single();
		dbgroup.Description = "Builder's prose."; context.SaveChanges();
		ChargenSkillSelectionGroupSeeder.Upsert(context, definition with { Description = "Updated stock prose." }, conflicts);
		Assert.AreEqual("Builder's prose.", dbgroup.Description); Assert.AreEqual(1, context.ChargenSkillSelectionGroups.Count()); Assert.AreEqual(3, dbgroup.Members.Count);
		Assert.IsTrue(conflicts.Count > 0);
		f.Groups.Add(new Group(dbgroup, f.World.Object));
		var storyboardModel = context.ChargenScreenStoryboards.Single(x => x.ChargenStage == (int)ChargenStage.SelectSkills);
		var storyboard = new SkillPickerScreenStoryboard(f.World.Object, storyboardModel);
		var screen = storyboard.GetScreen(f.Chargen); Assert.IsInstanceOfType(screen, typeof(SkillGroupScreen));
		screen.HandleCommand("pick Hearthcraft"); screen.HandleCommand("pick Mending");
		Assert.AreEqual(2, f.Claims.GroupSkills.Count());
	}

	[TestMethod]
	public void SG04_SG05_ActualCostStoryboard_RefundsBaseOnlyAndChargesBoostOnce()
	{
		var f = new F(); var group = f.Group(0, 1); var resource = new Mock<IChargenResource>();
		f.World.SetupGet(x => x.ChargenResources).Returns(F.Repository<IChargenResource>([resource.Object]));
		var storyboard = (SkillCostPickerScreenStoryboard)Activator.CreateInstance(typeof(SkillCostPickerScreenStoryboard), true)!;
		var count = new Mock<IFutureProg>(); count.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(0);
		var boostBase = new Mock<IFutureProg>(); boostBase.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(2M);
		Set(storyboard, "Gameworld", f.World.Object); Set(storyboard, "FreeSkillsProg", f.Free.Object); Set(storyboard, "BoostResource", resource.Object);
		Set(storyboard, "NumberOfFreeSkillPicksProg", count.Object); Set(storyboard, "BaseBoostCostProg", boostBase.Object);
		Set(storyboard, "AdditionalSkillsCostExpression", new ExpressionEngine.Expression("picks * 10"));
		Set(storyboard, "BoostCostExpression", new ExpressionEngine.Expression("base * boosts"));
		f.Chargen.SelectedSkills.Add(f.Skills[0]); f.Chargen.SelectedSkillBoosts[f.Skills[0]] = 3;
		var r = f.Resolver(); Assert.AreEqual(16, storyboard.ChargenCosts(f.Chargen).Single().Cost);
		r.Pick(group, f.Skills[0]); Assert.AreEqual(6, storyboard.ChargenCosts(f.Chargen).Single().Cost);
		f.Claims = ChargenSkillClaims.Load(f.Claims.Save()); r = f.Resolver(); Assert.AreEqual(6, storyboard.ChargenCosts(f.Chargen).Single().Cost);
		r.Unpick(group, f.Skills[0]); Assert.AreEqual(16, storyboard.ChargenCosts(f.Chargen).Single().Cost);
	}

	[TestMethod]
	public void SG11_CompiledFutureProgs_AcceptChargenAndRejectWrongArguments()
	{
		FutureProgTestBootstrap.EnsureInitialised(); var f = new F();
		var good = new MudSharp.FutureProg.FutureProg(f.World.Object, "groupfixture", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Chargen, "applicant")], "return true");
		Assert.IsTrue(Group.ValidProg(good));
		Assert.AreEqual(true, good.Execute(MudSharp.CharacterCreation.Chargen.CreateSkillEvaluationContext(f.Chargen, [])));
		var wrong = new MudSharp.FutureProg.FutureProg(f.World.Object, "wrongfixture", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Character, "applicant")], "return true");
		Assert.IsFalse(Group.ValidProg(wrong));
		var broken = new MudSharp.FutureProg.FutureProg(f.World.Object, "brokenfixture", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Chargen, "applicant")], "return @missingvariable");
		Assert.IsFalse(Group.ValidProg(broken));
	}

	[TestMethod]
	public void SG19_GeneratedAdapter_DeterministicSavedChoicesAndAuthoritativeExplicitValues()
	{
		var f = new F(); f.Group(2, 2, [1, 2]);
		var culture = new Mock<ICulture>(); var start = new Mock<IFutureProg>(); start.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(10.0);
		culture.SetupGet(x => x.SkillStartingValueProg).Returns(start.Object);
		foreach (var skill in f.Skills)
		{
			var cap = new Mock<ITraitExpression>(); cap.Setup(x => x.Evaluate(It.IsAny<IHaveTraits>(), null, TraitBonusContext.None)).Returns(100.0);
			Mock.Get((MudSharp.Body.Traits.Subtypes.ISkillDefinition)skill).SetupGet(x => x.Cap).Returns(cap.Object);
		}
		var template = new SimpleCharacterTemplate { Gameworld = f.World.Object, SelectedCulture = culture.Object,
			SkillValues = [(f.Skills[2], 75.0)], SelectedAttributes = [], SelectedRoles = [] };
		var variable = (VariableNPCTemplate)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(VariableNPCTemplate));
		variable.SetNoSave(true);
		variable.ApplySkillGroupsToGeneratedTemplate(template);
		Assert.AreEqual(1, template.SelectedSkills.Count);
		variable.ConfigureSkillGroups(1234, GeneratedOptionalSkillPolicy.Decline);
		variable.ApplySkillGroupsToGeneratedTemplate(template);
		var claims = template.SkillGroupClaims!;
		Assert.AreEqual(3, template.SelectedSkills.Count); Assert.AreEqual(75.0, template.SkillValues.Single(x => x.Item1 == f.Skills[2]).Item2);
		var saved = claims.Save().ToString();
		variable.ApplySkillGroupsToGeneratedTemplate(template);
		Assert.AreEqual(saved, template.SkillGroupClaims!.Save().ToString()); Assert.AreEqual(3, template.SkillValues.Count);
	}

	[TestMethod]
	[DoNotParallelize]
	public void SG10_BuilderSetters_SaveReloadRetireAndKeepStableIdentity()
	{
		using var context = Context(); var f = new F(); var original = f.Group();
		var model = new MudSharp.Models.ChargenSkillSelectionGroup { Id = original.Id, StableKey = original.StableKey, Name = original.Name, MinimumPicks = 2, MaximumPicks = 2, EligibilityProgId = 1 };
		foreach (var skill in f.Skills) model.Members.Add(new() { TraitDefinitionId = skill.Id });
		context.ChargenSkillSelectionGroups.Add(model); context.SaveChanges();
		var contextProperty = typeof(FMDB).GetProperty("Context")!;
		var countProperty = typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!;
		var oldContext = contextProperty.GetValue(null); var oldCount = countProperty.GetValue(null);
		contextProperty.SetValue(null, context); countProperty.SetValue(null, 1u);
		try
		{
			var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
			actor.SetupGet(x => x.Gameworld).Returns(f.World.Object);
			foreach (var command in new[] { "name Renamed", "picks 0 2", "order 100", "existing countknown", "member remove Mending", "member add Mending", "eligibility GroupEligible", "enabled yes" })
				Assert.IsTrue(original.BuildingCommand(actor.Object, new StringStack(command)), command);
			var reloaded = new Group(context.ChargenSkillSelectionGroups.Include(x => x.Members).Single(), f.World.Object);
			Assert.AreEqual("Renamed", reloaded.Name); Assert.AreEqual(original.StableKey, reloaded.StableKey); Assert.IsTrue(reloaded.Enabled);
			Assert.AreEqual(0, reloaded.MinimumPicks); Assert.AreEqual(2, reloaded.MaximumPicks); Assert.AreEqual(3, reloaded.Members.Count);
			reloaded.Retire(); Assert.IsFalse(model.Enabled); Assert.IsTrue(model.Retired);
			var commandMethod = typeof(MudSharp.Commands.Modules.ChargenModule).GetMethod("ChargenSkillGroup", BindingFlags.NonPublic | BindingFlags.Static)!;
			commandMethod.Invoke(null, [actor.Object, "chargenskillgroup create \"Builder Household\""]);
			var created = context.ChargenSkillSelectionGroups.Single(x => x.Name == "Builder Household");
			commandMethod.Invoke(null, [actor.Object, $"chargenskillgroup clone {created.Id} \"Cloned Household\""]);
			var cloned = context.ChargenSkillSelectionGroups.Single(x => x.Name == "Cloned Household");
			Assert.IsFalse(created.Enabled); Assert.IsFalse(cloned.Enabled); Assert.AreNotEqual(created.StableKey, cloned.StableKey);
		}
		finally { contextProperty.SetValue(null, oldContext); countProperty.SetValue(null, oldCount); }
	}

	[TestMethod]
	[DoNotParallelize]
	public void SG12_RealChargenXml_ReconnectPreservesGroupOrdinaryAndBoostClaims()
	{
		using var context = Context(); var f = new F(); f.World.DefaultValue = DefaultValue.Mock;
		f.Application.SetupGet(x => x.Stage).Returns(ChargenStage.SelectSkills);
		var group = f.Group(1, 1); f.Chargen.SelectedSkills.Add(f.Skills[0]); f.Chargen.SelectedSkillBoosts[f.Skills[0]] = 2;
		var resolver = f.Resolver(); resolver.Pick(group, f.Skills[0]); resolver.Finish(group);
		var source = (MudSharp.CharacterCreation.Chargen)MudSharp.CharacterCreation.Chargen.CreateSkillEvaluationContext(f.Chargen, []);
		source.RestoreSkillClaims(f.Claims); source.SelectedSkills = f.Chargen.SelectedSkills.ToList(); source.SelectedSkillBoosts = f.Chargen.SelectedSkillBoosts.ToDictionary(x => x.Key, x => x.Value);
		var skills = (SkillSkipperScreenStoryboard)Activator.CreateInstance(typeof(SkillSkipperScreenStoryboard), true)!;
		Set(skills, "Gameworld", f.World.Object); Set(skills, "FreeSkillsProg", f.Free.Object);
		var locations = new StartingLocationPickerScreenStoryboard(f.World.Object, new MudSharp.Models.ChargenScreenStoryboard { StageDefinition = "<Definition><Locations /></Definition>" });
		var storyboards = new Mock<IChargenStoryboard>();
		storyboards.SetupGet(x => x.StageScreenMap).Returns(new Dictionary<ChargenStage, IChargenScreenStoryboard> { [ChargenStage.SelectSkills] = skills, [ChargenStage.SelectStartingLocation] = locations });
		f.World.SetupGet(x => x.ChargenStoryboard).Returns(storyboards.Object);
		var contextProperty = typeof(FMDB).GetProperty("Context")!;
		var countProperty = typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!;
		var oldContext = contextProperty.GetValue(null); var oldCount = countProperty.GetValue(null);
		contextProperty.SetValue(null, context); countProperty.SetValue(null, 1u);
		try
		{
			var restored = new MudSharp.CharacterCreation.Chargen(new MudSharp.Models.Chargen { Id = 22, Definition = source.SaveToXml().ToString() }, null!, f.World.Object, f.Chargen.Account);
			Assert.IsTrue(restored.SkillClaims.Groups[group.StableKey].Complete);
			Assert.IsTrue(restored.SkillClaims.Ordinary.Contains(1)); Assert.AreEqual(0, restored.SkillClaims.ChargeableOrdinary.Count());
			Assert.AreEqual(2, restored.SelectedSkillBoosts[f.Skills[0]]);
			Assert.AreEqual(ChargenScreenState.Complete, restored.CurrentScreen.State);
		}
		finally { contextProperty.SetValue(null, oldContext); countProperty.SetValue(null, oldCount); }
	}
}
