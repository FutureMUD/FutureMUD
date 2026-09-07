using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Framework;
using MudSharp.FutureProg;
using RuntimeGroup = MudSharp.CharacterCreation.ChargenSkillSelectionGroup;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargenSkillSelectionGroupTests
{
	internal sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IChargen> Application { get; } = new();
		public IChargen Chargen => Application.Object;
		public List<IChargenSkillSelectionGroup> Groups { get; } = [];
		public List<ITraitDefinition> Skills { get; } = [];
		public List<IFutureProg> Progs { get; } = [];
		public Mock<IFutureProg> Eligibility { get; } = new();
		public Mock<IFutureProg> Free { get; } = new();
		public List<ITraitDefinition> Mandatory { get; } = [];
		public Fixture()
		{
			Application.SetupAllProperties();
			Application.SetupGet(x => x.Gameworld).Returns(World.Object);
			Application.SetupGet(x => x.SkillClaims).Returns(() => Claims);
			Chargen.Account = DummyAccount.Instance;
			Chargen.SelectedSkills = []; Chargen.SkillValues = []; Chargen.SelectedAttributes = [];
			Chargen.SelectedRoles = []; Chargen.SelectedMerits = []; Chargen.SelectedKnowledges = [];
			Chargen.SelectedNotes = []; Chargen.SelectedAccents = []; Chargen.SelectedCharacteristics = [];
			Chargen.SelectedEntityDescriptionPatterns = []; Chargen.MissingBodyparts = [];
			Chargen.SelectedDisfigurements = []; Chargen.SelectedScars = []; Chargen.SelectedTattoos = [];
			Chargen.SelectedProstheses = []; Chargen.SelectedSkillBoosts = []; Chargen.SelectedSkillBoostCosts = [];
			World.SetupGet(x => x.Traits).Returns(Repository(Skills));
			World.SetupGet(x => x.FutureProgs).Returns(Repository(Progs));
			World.SetupGet(x => x.ChargenSkillSelectionGroups).Returns(Groups);
			Eligibility.SetupGet(x => x.Id).Returns(1);
			Eligibility.SetupGet(x => x.FunctionName).Returns("GroupEligible");
			Eligibility.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			Eligibility.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Chargen]);
			Eligibility.Setup(x => x.Compile()).Returns(true);
			Eligibility.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(true);
			Progs.Add(Eligibility.Object);
			Free.Setup(x => x.ExecuteCollection<ITraitDefinition>(It.IsAny<object[]>())).Returns(() => Mandatory.ToList());
			foreach (var name in new[] { "Hearthcraft", "Mending", "Yardwork" })
			{
				var skill = new Mock<ISkillDefinition>();
				skill.SetupGet(x => x.Id).Returns(Skills.Count + 1);
				skill.SetupGet(x => x.Name).Returns(name);
				skill.SetupGet(x => x.Group).Returns("Crafts");
				skill.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
				skill.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
				skill.Setup(x => x.ChargenAvailable(It.IsAny<ICharacterTemplate>())).Returns(true);
				Skills.Add(skill.Object);
			}
		}
		public ChargenSkillClaims Claims { get; set; } = new();
		internal static IUneditableAll<T> Repository<T>(List<T> values) where T : class, IFrameworkItem
		{
			var mock = new Mock<IUneditableAll<T>>();
			mock.Setup(x => x.GetEnumerator()).Returns(() => values.GetEnumerator());
			mock.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => values.FirstOrDefault(x => x.Id == id));
			return mock.Object;
		}
		public RuntimeGroup Group(int min = 2, int max = 2, long[]? members = null, ExistingSkillPolicy policy = ExistingSkillPolicy.NewOnly)
		{
			var model = new MudSharp.Models.ChargenSkillSelectionGroup
			{
				Id = Groups.Count + 1, StableKey = "group-" + Groups.Count, Name = "Household Training " + Groups.Count,
				Description = "The household teaches its children to tend the hearth, mend useful things and help with the work of the yard.",
				Enabled = true, MinimumPicks = min, MaximumPicks = max, EligibilityProgId = 1,
				ExistingSkillPolicy = (int)policy
			};
			foreach (var id in members ?? [1, 2, 3]) model.Members.Add(new MudSharp.Models.ChargenSkillSelectionGroupMember { TraitDefinitionId = id });
			var group = new RuntimeGroup(model, World.Object); Groups.Add(group); return group;
		}
		public SkillGroupResolver Resolver() => new(Chargen, Free.Object);
	}

	[TestMethod]
	public void SG01_NonLanguageExactlyTwo_GrantsOnlyExplicitMembers()
	{
		var f = new Fixture(); var group = f.Group(); var r = f.Resolver();
		Assert.IsNull(r.Pick(group, f.Skills[0]));
		Assert.IsNotNull(r.Finish(group));
		Assert.IsNull(r.Pick(group, f.Skills[1])); Assert.IsNull(r.Finish(group));
		Assert.IsNotNull(r.Pick(group, f.Skills[2]));
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, f.Chargen.SelectedSkills.Select(x => x.Id).ToArray());
		Assert.IsTrue(r.Complete);
	}

	[TestMethod]
	public void SG02_EligibilityFalse_OffersNothing()
	{
		var f = new Fixture(); f.Group(); f.Eligibility.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(false);
		var r = f.Resolver(); Assert.AreEqual(0, r.Groups.Count); Assert.IsTrue(r.Complete);
		Assert.AreEqual(0, f.Chargen.SelectedSkills.Count);
	}

	[TestMethod]
	public void SG04_SG05_PaidReallocation_RepricesFromClaimsAndPreservesBoosts()
	{
		var f = new Fixture(); var group = f.Group(0, 1);
		f.Chargen.SelectedSkills.Add(f.Skills[0]); f.Chargen.SelectedSkillBoosts[f.Skills[0]] = 3;
		var r = f.Resolver(); Assert.AreEqual(1, f.Claims.ChargeableOrdinary.Count());
		Assert.IsNull(r.Pick(group, f.Skills[0])); Assert.AreEqual(0, f.Claims.ChargeableOrdinary.Count());
		for (var i = 0; i < 3; i++) r.Revalidate();
		Assert.AreEqual(0, f.Claims.ChargeableOrdinary.Count());
		r.Unpick(group, f.Skills[0]); Assert.AreEqual(1, f.Claims.ChargeableOrdinary.Count());
		Assert.IsTrue(f.Chargen.SelectedSkills.Contains(f.Skills[0])); Assert.AreEqual(3, f.Chargen.SelectedSkillBoosts[f.Skills[0]]);
	}

	[TestMethod]
	public void SG06_CountKnown_RequiresExplicitCreditAndPreservesBaseline()
	{
		var f = new Fixture(); f.Mandatory.Add(f.Skills[0]); var group = f.Group(1, 1, policy: ExistingSkillPolicy.CountKnown);
		var r = f.Resolver(); Assert.IsFalse(r.Complete); Assert.AreEqual(0, f.Claims.Groups[group.StableKey].Skills.Count);
		Assert.IsNull(r.Pick(group, f.Skills[0])); Assert.IsTrue(f.Claims.Groups[group.StableKey].Credits.Contains(1));
		r.Unpick(group, f.Skills[0]); Assert.IsTrue(f.Chargen.SelectedSkills.Contains(f.Skills[0]));
	}

	[TestMethod]
	public void SG06_NewOnly_ExcludesMandatoryWithoutReducingMinimum()
	{
		var f = new Fixture(); f.Mandatory.AddRange(f.Skills); var group = f.Group(); var r = f.Resolver();
		Assert.AreEqual(0, r.Candidates[group.StableKey].Count); Assert.IsFalse(r.Complete); Assert.IsTrue(r.Errors.Count > 0);
	}

	[TestMethod]
	public void SG07_SG08_Overlap_PreservesNarrowGroupsAndUniqueOwnership()
	{
		var f = new Fixture(); var broad = f.Group(1, 1, [1, 2]); var narrow = f.Group(1, 1, [1]); var r = f.Resolver();
		Assert.IsNotNull(r.Pick(broad, f.Skills[0])); Assert.IsNull(r.Pick(broad, f.Skills[1]));
		Assert.IsNull(r.Pick(narrow, f.Skills[0])); Assert.IsNotNull(r.Pick(narrow, f.Skills[1]));
	}

	[TestMethod]
	public void SG08_Overlap_MultipleCapacityAndOptionalChoiceCannotStrandRequiredGroup()
	{
		var f = new Fixture(); var optional = f.Group(0, 2, [1, 2]); f.Group(2, 2, [1, 2]); var r = f.Resolver();
		Assert.IsNotNull(r.Pick(optional, f.Skills[0]));
	}

	[TestMethod]
	public void SG09_EmptyOptional_RequiresDeclineAndCreatesNoOrdinaryCapacity()
	{
		var f = new Fixture(); var group = f.Group(0, 2, []); var r = f.Resolver();
		Assert.IsFalse(r.Complete); Assert.IsNull(r.Finish(group)); Assert.IsTrue(r.Complete);
		Assert.AreEqual(0, f.Claims.Ordinary.Count);
	}

	[TestMethod]
	[DataRow("signature")]
	[DataRow("null")]
	[DataRow("exception")]
	[DataRow("compile")]
	[DataRow("missingtrait")]
	public void SG11_InvalidConfiguration_FailsClosed(string failure)
	{
		var f = new Fixture(); f.Group(members: failure == "missingtrait" ? [99, 1] : null);
		switch (failure)
		{
			case "signature": f.Eligibility.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Character]); break;
			case "null": f.Eligibility.Setup(x => x.Execute(It.IsAny<object[]>())).Returns((object)null!); break;
			case "exception": f.Eligibility.Setup(x => x.Execute(It.IsAny<object[]>())).Throws<InvalidOperationException>(); break;
			case "compile": f.Eligibility.Setup(x => x.Compile()).Returns(false); break;
		}
		var r = f.Resolver(); Assert.IsFalse(r.Complete); Assert.IsTrue(r.Errors.Count > 0);
	}

	[TestMethod]
	public void SG12_SaveReload_ReconstructsClaimsWithoutRefundOrReroll()
	{
		var f = new Fixture(); var group = f.Group(); var r = f.Resolver(); r.Pick(group, f.Skills[1]);
		f.Claims = ChargenSkillClaims.Load(f.Claims.Save()); r = f.Resolver();
		CollectionAssert.AreEquivalent(new long[] { 2 }, f.Claims.Groups[group.StableKey].Skills.ToArray());
		Assert.AreEqual(0, f.Claims.Ordinary.Count); Assert.AreEqual(0, f.Claims.Mandatory.Count);
	}

	[TestMethod]
	public void SG13_BackgroundChange_RemovesOnlyGroupOwnership()
	{
		var f = new Fixture(); var group = f.Group(1, 1); f.Chargen.SelectedSkills.Add(f.Skills[0]); f.Mandatory.Add(f.Skills[2]);
		var r = f.Resolver(); r.Pick(group, f.Skills[0]);
		f.Eligibility.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(false); r.Revalidate();
		CollectionAssert.AreEquivalent(new long[] { 1, 3 }, f.Chargen.SelectedSkills.Select(x => x.Id).ToArray());
		Assert.AreEqual(1, f.Claims.ChargeableOrdinary.Count());
	}

	[TestMethod]
	public void SG14_RenameAndMemberRevision_RetainsIdentityAndValidClaims()
	{
		var f = new Fixture(); var group = f.Group(); var r = f.Resolver(); r.Pick(group, f.Skills[0]); r.Pick(group, f.Skills[1]); r.Finish(group);
		var replacement = new MudSharp.Models.ChargenSkillSelectionGroup { Id = group.Id, StableKey = group.StableKey, Name = "Renamed", Revision = 2, Enabled = true, EligibilityProgId = 1, MinimumPicks = 1, MaximumPicks = 2 };
		replacement.Members.Add(new() { TraitDefinitionId = 1 }); f.Groups[0] = new RuntimeGroup(replacement, f.World.Object);
		r.Revalidate(); Assert.IsFalse(r.Complete); CollectionAssert.AreEquivalent(new long[] { 1 }, f.Claims.Groups[group.StableKey].Skills.ToArray());
		Assert.AreEqual(2, f.Claims.Groups[group.StableKey].Revision);
	}

	[TestMethod]
	[DataRow(typeof(SkillPickerScreenStoryboard))]
	[DataRow(typeof(SkillCostPickerScreenStoryboard))]
	[DataRow(typeof(SkillSkipperScreenStoryboard))]
	[DataRow(typeof(SkillBoostSkipperScreenStoryboard))]
	public void SG03_SG15_SG20_AllScreens_EnterSharedRequiredPhaseAndRenderFullNames(Type type)
	{
		var f = new Fixture(); f.Mandatory.Add(f.Skills[2]); f.Group();
		var storyboard = (IChargenScreenStoryboard)Activator.CreateInstance(type, true)!;
		type.GetProperty("FreeSkillsProg")!.SetValue(storyboard, f.Free.Object);
		var screen = storyboard.GetScreen(f.Chargen);
		Assert.IsInstanceOfType(screen, typeof(SkillGroupScreen)); Assert.AreNotEqual(ChargenScreenState.Complete, screen.State);
		var text = screen.Display(); Assert.IsTrue(text.IndexOf("Mandatory skills", StringComparison.Ordinal) < text.IndexOf("Household Training", StringComparison.Ordinal));
		StringAssert.Contains(text, "Hearthcraft"); StringAssert.Contains(text, "Mending");
		screen.HandleCommand("pick Hearthcraft"); screen.HandleCommand("done");
		Assert.AreNotEqual(ChargenScreenState.Complete, screen.State);
	}

	[TestMethod]
	public void SG16_OrdinarySkill_CannotQualifyItsOwnGroup()
	{
		var f = new Fixture(); f.Group(); f.Chargen.SelectedSkills.Add(f.Skills[0]);
		f.Eligibility.Setup(x => x.Execute(It.IsAny<object[]>())).Returns((object[] args) => ((IChargen)args[0]).SelectedSkills.Contains(f.Skills[0]));
		var r = f.Resolver(); Assert.AreEqual(0, r.Groups.Count); Assert.AreEqual(1, f.Claims.Ordinary.Count);
	}

	[TestMethod]
	public void SG12_SG15_Skipper_OnlyCompletesAfterSavedRequiredDecision()
	{
		var f = new Fixture(); f.Group(1, 1);
		var storyboard = (SkillSkipperScreenStoryboard)Activator.CreateInstance(typeof(SkillSkipperScreenStoryboard), true)!;
		typeof(SkillSkipperScreenStoryboard).GetProperty("FreeSkillsProg")!.SetValue(storyboard, f.Free.Object);
		var screen = storyboard.GetScreen(f.Chargen);
		screen.HandleCommand("pick Mending"); screen.HandleCommand("done");
		Assert.AreEqual(ChargenScreenState.Complete, screen.State);
		f.Claims = ChargenSkillClaims.Load(f.Claims.Save()); screen = storyboard.GetScreen(f.Chargen);
		Assert.AreEqual(ChargenScreenState.Complete, screen.State);
		CollectionAssert.AreEquivalent(new long[] { 2 }, f.Chargen.SelectedSkills.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void SG16_Suggestions_RunOnlyAfterGroupsAndSurviveDisplay()
	{
		var f = new Fixture(); f.Group(1, 1); var suggestions = new Mock<IFutureProg>();
		suggestions.Setup(x => x.ExecuteCollection<ITraitDefinition>(It.IsAny<object[]>())).Returns([f.Skills[2]]);
		var count = new Mock<IFutureProg>(); count.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(1);
		f.World.SetupGet(x => x.ChargenResources).Returns(Fixture.Repository<MudSharp.CharacterCreation.Resources.IChargenResource>([]));
		var storyboard = (SkillPickerScreenStoryboard)Activator.CreateInstance(typeof(SkillPickerScreenStoryboard), true)!;
		foreach (var pair in new Dictionary<string, object> { ["Gameworld"] = f.World.Object, ["Blurb"] = "Select ordinary skills.", ["FreeSkillsProg"] = f.Free.Object, ["SuggestedSkillsProg"] = suggestions.Object, ["NumberOfSkillPicksProg"] = count.Object })
			typeof(SkillPickerScreenStoryboard).GetProperty(pair.Key)!.SetValue(storyboard, pair.Value);
		var screen = storyboard.GetScreen(f.Chargen); screen.Display();
		suggestions.Verify(x => x.ExecuteCollection<ITraitDefinition>(It.IsAny<object[]>()), Times.Never);
		screen.HandleCommand("pick Mending"); screen.HandleCommand("done"); screen.Display(); screen.Display();
		suggestions.Verify(x => x.ExecuteCollection<ITraitDefinition>(It.IsAny<object[]>()), Times.Once);
		CollectionAssert.AreEquivalent(new long[] { 2, 3 }, f.Chargen.SelectedSkills.Select(x => x.Id).ToArray());
		CollectionAssert.AreEquivalent(new long[] { 3 }, f.Claims.Ordinary.ToArray());
	}

	[TestMethod]
	public void SG20_ExistingLatin1Encoder_PreservesFullAccentedSkillNames()
	{
		var f = new Fixture(); f.Group(1, 1);
		Mock.Get((ISkillDefinition)f.Skills[0]).SetupGet(x => x.Name).Returns("Métallurgie domestique");
		var storyboard = (SkillSkipperScreenStoryboard)Activator.CreateInstance(typeof(SkillSkipperScreenStoryboard), true)!;
		var screen = storyboard.GetScreen(f.Chargen);
		var rendered = screen.Display().ConvertToLatin1();
		StringAssert.Contains(rendered, "Métallurgie domestique");
		screen.HandleCommand("pick \"Métallurgie domestique\"");
		Assert.IsTrue(f.Claims.GroupSkills.Contains(1));
	}

	[TestMethod]
	public void SG11_BrokenMemberProg_ReportsErrorWithoutPartiallyOpeningGroup()
	{
		var f = new Fixture(); var filter = new Mock<IFutureProg>();
		filter.SetupGet(x => x.Id).Returns(2); filter.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Chargen, ProgVariableTypes.Trait]);
		filter.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean); filter.Setup(x => x.Compile()).Returns(true);
		filter.Setup(x => x.Execute(It.IsAny<object[]>())).Returns((object)null!); f.Progs.Add(filter.Object);
		var model = new MudSharp.Models.ChargenSkillSelectionGroup { Id = 1, StableKey = "broken-member", Name = "Filtered Crafts", Enabled = true, MinimumPicks = 1, MaximumPicks = 1, EligibilityProgId = 1, MemberEligibilityProgId = 2 };
		model.Members.Add(new() { TraitDefinitionId = 1 }); f.Groups.Add(new RuntimeGroup(model, f.World.Object));
		var storyboard = (SkillSkipperScreenStoryboard)Activator.CreateInstance(typeof(SkillSkipperScreenStoryboard), true)!;
		var screen = storyboard.GetScreen(f.Chargen); StringAssert.Contains(screen.Display(), "configuration error");
		Assert.AreNotEqual(ChargenScreenState.Complete, screen.State);
	}

	[TestMethod]
	public void SG06_IndependentGrant_IsBaselineAndCannotBeSelfReclassified()
	{
		var f = new Fixture(); var group = f.Group(1, 1); f.Claims.Independent.Add(1); var r = f.Resolver();
		Assert.IsFalse(r.Candidates[group.StableKey].Contains(f.Skills[0]));
		f.Claims = ChargenSkillClaims.Load(f.Claims.Save()); r = f.Resolver();
		Assert.IsTrue(f.Chargen.SelectedSkills.Contains(f.Skills[0])); Assert.AreEqual(0, f.Claims.Ordinary.Count);
	}

	[TestMethod]
	public void SG13_RestoredUnavailableOrdinaryChoice_RemainsUntilExplicitlyRemoved()
	{
		var f = new Fixture(); f.Group(1, 1); f.Chargen.SelectedSkills.Add(f.Skills[1]);
		var count = new Mock<IFutureProg>(); count.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(0);
		f.World.SetupGet(x => x.ChargenResources).Returns(Fixture.Repository<MudSharp.CharacterCreation.Resources.IChargenResource>([]));
		var storyboard = (SkillPickerScreenStoryboard)Activator.CreateInstance(typeof(SkillPickerScreenStoryboard), true)!;
		foreach (var pair in new Dictionary<string, object> { ["Gameworld"] = f.World.Object, ["Blurb"] = "Select ordinary skills.", ["FreeSkillsProg"] = f.Free.Object, ["NumberOfSkillPicksProg"] = count.Object })
			typeof(SkillPickerScreenStoryboard).GetProperty(pair.Key)!.SetValue(storyboard, pair.Value);
		var screen = storyboard.GetScreen(f.Chargen); screen.HandleCommand("pick Mending"); screen.HandleCommand("done");
		f.Eligibility.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(false);
		Mock.Get((ISkillDefinition)f.Skills[1]).Setup(x => x.ChargenAvailable(It.IsAny<ICharacterTemplate>())).Returns(false);
		screen.Display(); Assert.IsTrue(f.Chargen.SelectedSkills.Contains(f.Skills[1]));
		screen.HandleCommand("Mending"); Assert.IsFalse(f.Chargen.SelectedSkills.Contains(f.Skills[1])); Assert.AreEqual(0, f.Claims.Ordinary.Count);
	}

	[TestMethod]
	[DataRow(TraitType.Attribute)]
	[DataRow(TraitType.DerivedAttribute)]
	public void SG11_AttributeMembers_CannotActivate(TraitType type)
	{
		var f = new Fixture(); var attribute = new Mock<ITraitDefinition>();
		attribute.SetupGet(x => x.Id).Returns(4); attribute.SetupGet(x => x.TraitType).Returns(type);
		f.Skills.Add(attribute.Object); var group = f.Group(1, 1, [4]);
		Assert.IsTrue(group.Validate().Any()); Assert.IsFalse(f.Resolver().Complete);
	}
}
