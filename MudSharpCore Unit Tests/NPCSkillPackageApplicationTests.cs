#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.NPC.Templates;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NPCSkillPackageApplicationTests
{
	[TestMethod]
	public void GenericBuilderHelper_RegistersCompleteNPCSkillPackageWorkflow()
	{
		var helper = EditableItemHelper.NPCSkillPackageHelper;
		Assert.AreEqual("npcskillpackage", helper.CommandName);
		Assert.AreEqual(typeof(INPCSkillPackage), helper.CastToType);
		Assert.IsNotNull(helper.EditableNewAction);
		Assert.IsNotNull(helper.EditableCloneAction);
		Assert.IsNotNull(helper.EditableDeleteAction);
		StringAssert.Contains(helper.DefaultCommandHelp, "set skill");
	}

	[TestMethod]
	public void PackageNames_BlankAndNumericAreRejectedForAllBuilderCreationPaths()
	{
		Assert.IsFalse(NPCSkillPackage.TryNormaliseName("   ", out _, out _));
		Assert.IsFalse(NPCSkillPackage.TryNormaliseName("12345", out _, out _));
		Assert.IsTrue(NPCSkillPackage.TryNormaliseName("universal common", out var name, out _));
		Assert.AreEqual("Universal Common", name);
	}

	[TestMethod]
	public void GenericDelete_QueuesConfirmationWithoutImmediatelyDeleting()
	{
		var standardPhrasingField = typeof(Accept)
			.GetField("_standardAcceptPhrasing", BindingFlags.Static | BindingFlags.NonPublic)!;
		var originalPhrasing = standardPhrasingField.GetValue(null);
		try
		{
			standardPhrasingField.SetValue(null, "Type ACCEPT to confirm.");
			var package = new Mock<INPCSkillPackage>();
			package.SetupGet(x => x.Id).Returns(42L);
			package.SetupGet(x => x.Name).Returns("Delete Me");
			var packages = new All<INPCSkillPackage>();
			packages.Add(package.Object);
			var gameworld = new Mock<IFuturemud>();
			gameworld.SetupGet(x => x.NpcSkillPackages).Returns(packages);
			var actor = new Mock<MudSharp.Character.ICharacter>();
			actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			actor.SetupGet(x => x.OutputHandler)
				.Returns(new Mock<MudSharp.PerceptionEngine.IOutputHandler>().Object);

			BaseBuilderModule.GenericDelete(actor.Object, new StringStack("Delete Me"),
				EditableItemHelper.NPCSkillPackageHelper);

			actor.Verify(x => x.AddEffect(It.IsAny<Accept>(), TimeSpan.FromSeconds(120)), Times.Once);
			gameworld.Verify(x => x.Destroy(package.Object), Times.Never);
			Assert.AreSame(package.Object, packages.Get(42L));
		}
		finally
		{
			standardPhrasingField.SetValue(null, originalPhrasing);
		}
	}

	[TestMethod]
	public void RaceDefaults_InheritParentPackagesWithoutDuplicatingDirectSelections()
	{
		var parentPackage = Package();
		var directPackage = Package();
		var packages = Race.ResolveDefaultSkillPackages(
			[parentPackage],
			[parentPackage, directPackage]).ToList();

		CollectionAssert.AreEquivalent(new[] { parentPackage, directPackage }, packages);
	}

	private static ITraitDefinition Trait(string name)
	{
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Name).Returns(name);
		return trait.Object;
	}

	private static INPCSkillPackage Package(params NPCSkillPackageEntry[] entries)
	{
		var package = new Mock<INPCSkillPackage>();
		package.SetupGet(x => x.Skills).Returns(entries);
		return package.Object;
	}

	[TestMethod]
	public void SimpleApplication_IsChanceAwareClampsAndNeverLowers()
	{
		var lower = Trait("Lower");
		var raise = Trait("Raise");
		var fail = Trait("Fail");
		var negative = Trait("Negative");
		var add = Trait("Add");
		var package = Package(
			new NPCSkillPackageEntry(lower, 1.0, 50.0, 0.0, 0.0),
			new NPCSkillPackageEntry(raise, 1.0, 40.0, 0.0, 0.0),
			new NPCSkillPackageEntry(fail, 0.25, 30.0, 0.0, 0.0),
			new NPCSkillPackageEntry(negative, 1.0, 10.0, 0.0, -0.5),
			new NPCSkillPackageEntry(add, 1.0, 30.0, 0.0, 0.5));
		var skills = new List<(ITraitDefinition Skill, double Value)> { (lower, 60.0), (raise, 20.0) };
		var chanceRolls = new Queue<double>([0.0, 0.0, 0.75, 0.0, 0.0]);
		var values = new Dictionary<ITraitDefinition, double>
		{
			[lower] = 50.0,
			[raise] = 40.0,
			[negative] = -10.0,
			[add] = 30.0
		};

		var result = SimpleNPCTemplate.ApplySkillPackageToValues(skills, package,
			() => chanceRolls.Dequeue(), entry => values[entry.Skill]);

		Assert.AreEqual(1, result.Added);
		Assert.AreEqual(1, result.Raised);
		Assert.AreEqual(2, result.Skipped);
		Assert.AreEqual(1, result.FailedChance);
		Assert.AreEqual(60.0, skills.Single(x => x.Skill == lower).Value);
		Assert.AreEqual(40.0, skills.Single(x => x.Skill == raise).Value);
		Assert.AreEqual(30.0, skills.Single(x => x.Skill == add).Value);
		Assert.IsFalse(skills.Any(x => x.Skill == negative));
	}

	[TestMethod]
	public void VariableApplication_UsesStrictWeightedExpectedValueAndCopiesWholeDistribution()
	{
		var equal = Trait("Equal");
		var upgrade = Trait("Upgrade");
		var add = Trait("Add");
		var templates = new List<VariableSkillTemplate>
		{
			new() { Trait = equal, Chance = 0.5, SkillMean = 50.0, SkillStddev = 4.0, SkillSkewness = -0.2 },
			new() { Trait = upgrade, Chance = 0.5, SkillMean = 20.0, SkillStddev = 2.0, SkillSkewness = 0.0 }
		};
		var package = Package(
			new NPCSkillPackageEntry(equal, 0.25, 100.0, 9.0, 0.7),
			new NPCSkillPackageEntry(upgrade, 0.25, 50.0, 7.0, 0.6),
			new NPCSkillPackageEntry(add, 0.5, 30.0, 5.0, -0.4));

		var result = VariableNPCTemplate.ApplySkillPackageToTemplates(templates, package);

		Assert.AreEqual(1, result.Added);
		Assert.AreEqual(1, result.Raised);
		Assert.AreEqual(1, result.Skipped);
		Assert.AreEqual(-0.2, templates.Single(x => x.Trait == equal).SkillSkewness,
			"Equal weighted values must preserve the existing distribution.");
		var upgraded = templates.Single(x => x.Trait == upgrade);
		Assert.AreEqual(0.25, upgraded.Chance);
		Assert.AreEqual(50.0, upgraded.SkillMean);
		Assert.AreEqual(7.0, upgraded.SkillStddev);
		Assert.AreEqual(0.6, upgraded.SkillSkewness);
	}

	[TestMethod]
	public void VariableSkillXml_OldFormatDefaultsSkewAndNewFormatRoundTripsIt()
	{
		var trait = Trait("Listen");
		var legacy = XElement.Parse("<Skill Chance=\"0.5\" Mean=\"30\" Stddev=\"4\" Trait=\"17\" />");
		var loadedLegacy = VariableSkillTemplate.LoadFromXml(legacy, id =>
		{
			Assert.AreEqual(17L, id);
			return trait;
		});
		Assert.AreEqual(0.0, loadedLegacy.SkillSkewness);

		var current = new VariableSkillTemplate
		{
			Trait = trait,
			Chance = 0.75,
			SkillMean = 40.0,
			SkillStddev = 6.0,
			SkillSkewness = -0.65
		};
		var roundTripped = VariableSkillTemplate.LoadFromXml(current.SaveToXml(), _ => trait);
		Assert.AreEqual(-0.65, roundTripped.SkillSkewness);
		Assert.AreEqual(0.75, roundTripped.Chance);
		Assert.AreEqual(40.0, roundTripped.SkillMean);
		Assert.AreEqual(6.0, roundTripped.SkillStddev);
	}

	[TestMethod]
	public void FutureProgSurface_RegistersTypedLookupApplyAndDotProperties()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var functions = FutureProg.GetFunctionCompilerInformations().ToList();
		Assert.IsTrue(functions.Any(x => x.FunctionName == "npcskillpackage" &&
			x.Parameters.SequenceEqual([ProgVariableTypes.Number]) &&
			x.ReturnType == ProgVariableTypes.NPCSkillPackage));
		Assert.IsTrue(functions.Any(x => x.FunctionName == "npcskillpackage" &&
			x.Parameters.SequenceEqual([ProgVariableTypes.Text]) &&
			x.ReturnType == ProgVariableTypes.NPCSkillPackage));
		Assert.IsTrue(functions.Any(x => x.FunctionName == "applyskillpackage" &&
			x.Parameters.SequenceEqual([ProgVariableTypes.Character, ProgVariableTypes.NPCSkillPackage]) &&
			x.ReturnType == ProgVariableTypes.Number));
		var properties = ProgVariable.DotReferenceCompileInfos[ProgVariableTypes.NPCSkillPackage].PropertyTypeMap;
		Assert.AreEqual(ProgVariableTypes.Number, properties["id"]);
		Assert.AreEqual(ProgVariableTypes.Text, properties["name"]);
		Assert.AreEqual(ProgVariableTypes.Collection | ProgVariableTypes.Trait, properties["skills"]);
	}

	[TestMethod]
	public void ApplySkillPackageFutureProg_AddsRaisesNeverLowersAndReturnsChangeCount()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var lower = Trait("Lower");
		var raise = Trait("Raise");
		var add = Trait("Add");
		var package = new Mock<INPCSkillPackage>();
		package.SetupGet(x => x.Type).Returns(ProgVariableTypes.NPCSkillPackage);
		package.SetupGet(x => x.GetObject).Returns(package.Object);
		package.SetupGet(x => x.Skills).Returns([
			new NPCSkillPackageEntry(lower, 1.0, 50.0, 0.0, 0.0),
			new NPCSkillPackageEntry(raise, 1.0, 40.0, 0.0, 0.0),
			new NPCSkillPackageEntry(add, 1.0, 30.0, 0.0, 0.0)
		]);
		var character = new Mock<MudSharp.Character.ICharacter>();
		character.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		character.SetupGet(x => x.GetObject).Returns(character.Object);
		character.Setup(x => x.HasTrait(lower)).Returns(true);
		character.Setup(x => x.TraitRawValue(lower)).Returns(60.0);
		character.Setup(x => x.HasTrait(raise)).Returns(true);
		character.Setup(x => x.TraitRawValue(raise)).Returns(20.0);
		character.Setup(x => x.HasTrait(add)).Returns(false);

		var compiler = FutureProg.GetFunctionCompilerInformations().Single(x =>
			x.FunctionName == "applyskillpackage" &&
			x.Parameters.SequenceEqual([ProgVariableTypes.Character, ProgVariableTypes.NPCSkillPackage]));
		var function = compiler.CompilerFunction(
			[new ConstantFunction(character.Object), new ConstantFunction(package.Object)],
			new Mock<MudSharp.Framework.IFuturemud>().Object);

		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual(2m, function.Result.GetObject);
		character.Verify(x => x.SetTraitValue(lower, It.IsAny<double>()), Times.Never);
		character.Verify(x => x.SetTraitValue(raise, 40.0), Times.Once);
		character.Verify(x => x.AddTrait(add, 30.0), Times.Once);
	}
}
