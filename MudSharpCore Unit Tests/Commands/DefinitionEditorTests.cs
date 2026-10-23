#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.GameItems.Decorators;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
[DoNotParallelize]
public class DefinitionEditorTests
{
	[DataTestMethod]
	[DataRow("Standard")]
	[DataRow("OGL")]
	[DataRow("Timebound")]
	[DataRow("Limb")]
	[DataRow("Static")]
	[DataRow("Branch")]
	[DataRow("PassivePerception")]
	[DataRow("BonusAbsent")]
	[DataRow("standard")]
	public void CheckTemplate_CreateAndEdit_RetainsIdentityAndCompleteDifficultyTable(string method)
	{
		using var database = new InMemoryDatabaseTestScope();
		var actor = EditorActor();
		Command(actor, "CheckTemplate", $"checktemplate new \"Test Template\" {method}");
		var original = database.Context.CheckTemplates.Single();
		Command(actor, "CheckTemplate", "checktemplate set difficulty Easy 12.5");
		Command(actor, "CheckTemplate", "checktemplate set difficulty Easy NaN");
		Command(actor, "CheckTemplate", "checktemplate set name Renamed");
		database.Context.ChangeTracker.Clear();
		var saved = database.Context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).Single();
		Assert.AreEqual(original.Id, saved.Id);
		Assert.AreEqual("Renamed", saved.Name);
		Assert.AreEqual(12.5, saved.CheckTemplateDifficulties.Single(x => x.Difficulty == (int)Difficulty.Easy).Modifier);
		CollectionAssert.AreEquivalent(Enum.GetValues<Difficulty>().Select(x => (int)x).ToArray(), saved.CheckTemplateDifficulties.Select(x => x.Difficulty).ToArray());
		Assert.IsNotNull(StandardCheck.LoadCheck(new MudSharp.Models.Check { CheckTemplate = saved }, actor.Object.Gameworld));
	}

	[TestMethod]
	public void AdvancedCheckDefinition_PreservesQuotedXmlAndRejectsInvalidFormulaWithoutChangingDraft()
	{
		using var database = new InMemoryDatabaseTestScope();
		var actor = EditorActor();
		Command(actor, "CheckTemplate", "checktemplate new Passive PassivePerception");
		Command(actor, "CheckTemplate", "checktemplate set definition <Definition minimum_time=\"2\" maximum_time=\"5\"><CoreTraitExpression>1</CoreTraitExpression><PassiveFuzziness>0.15</PassiveFuzziness></Definition>");
		var before = database.Context.CheckTemplates.Single().Definition;
		Command(actor, "CheckTemplate", "checktemplate set core_expression 1 +");
		Command(actor, "CheckTemplate", "checktemplate set maximum_time 2147483647");
		Command(actor, "CheckTemplate", "checktemplate set name Renamed");
		var saved = database.Context.CheckTemplates.Include(x => x.CheckTemplateDifficulties).Single();
		Assert.AreEqual(before, saved.Definition);
		var culture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
			var loaded = (PassivePerceptionCheck)StandardCheck.LoadCheck(new MudSharp.Models.Check { CheckTemplate = saved }, actor.Object.Gameworld);
			Assert.AreEqual(0.15, loaded.PassiveFuzziness);
			Assert.AreEqual("1", loaded.CoreTraitExpression.OriginalFormulaText);
		}
		finally { CultureInfo.CurrentCulture = culture; }
	}

	[TestMethod]
	public void CheckTemplate_InvalidMethodSettings_DoNotPoisonNextSaveOrAssignment()
	{
		using var database = new InMemoryDatabaseTestScope();
		var actor = EditorActor();
		Command(actor, "CheckTemplate", "checktemplate new Timer Timebound");
		Command(actor, "CheckTemplate", "checktemplate set maximum_time 10");
		Command(actor, "CheckTemplate", "checktemplate set minimum_time 5");
		var before = database.Context.CheckTemplates.Single().Definition;
		Command(actor, "CheckTemplate", "checktemplate set minimum_time 20");
		Command(actor, "CheckTemplate", "checktemplate set name Timer2");
		Assert.AreEqual(before, database.Context.CheckTemplates.Single().Definition);
		Command(actor, "CheckTemplate", "checktemplate assign Timer2 2147483647");
		Assert.AreEqual(0, database.Context.Checks.Count());
		var checkType = Enum.GetValues<CheckType>().First();
		var row = new MudSharp.Models.Check { Type = (int)checkType };
		database.Context.Checks.Add(row); database.Context.SaveChanges();
		Command(actor, "CheckTemplate", $"checktemplate assign Timer2 {checkType}");
		Assert.AreEqual(database.Context.CheckTemplates.Single().Id, row.CheckTemplateId);
		Assert.AreEqual((int)checkType, row.Type);
	}

	[DataTestMethod]
	[DataRow("SimpleNumeric")]
	[DataRow("CurrentMax")]
	[DataRow("Percentage")]
	[DataRow("Range")]
	public void TraitDecorator_AuthoredAndCloned_LoadsThroughRuntimeFactory(string type)
	{
		using var database = new InMemoryDatabaseTestScope();
		var actor = EditorActor();
		Command(actor, "TraitDecorator", $"traitdecorator new Original {type}");
		var original = database.Context.TraitDecorators.Single();
		Command(actor, "TraitDecorator", "traitdecorator clone Original Copy");
		database.Context.ChangeTracker.Clear();
		var rows = database.Context.TraitDecorators.OrderBy(x => x.Id).ToArray();
		Assert.AreEqual(2, rows.Length);
		Assert.AreNotEqual(original.Id, rows[1].Id);
		Assert.AreEqual("Original", DecoratorBase.GetDecorator(rows[0], false).Name);
		Assert.AreEqual("Copy", DecoratorBase.GetDecorator(rows[1], false).Name);
	}

	[DataTestMethod]
	[DataRow("Suffix")]
	[DataRow("Pile")]
	[DataRow("Bites")]
	[DataRow("Range")]
	public void StackDecorator_AuthoredAndCloned_LoadsThroughRuntimeFactory(string type)
	{
		using var database = new InMemoryDatabaseTestScope();
		var actor = EditorActor();
		Command(actor, "StackDecorator", $"stackdecorator new Original {type}");
		Command(actor, "StackDecorator", "stackdecorator clone Original Copy");
		database.Context.ChangeTracker.Clear();
		var rows = database.Context.StackDecorators.OrderBy(x => x.Id).ToArray();
		Assert.AreEqual(2, rows.Length);
		Assert.AreNotEqual(rows[0].Id, rows[1].Id);
		Assert.IsNotNull(StackDecorator.LoadStackDecorator(rows[0]));
		Assert.IsNotNull(StackDecorator.LoadStackDecorator(rows[1]));
	}

	[TestMethod]
	public void DecoratorRanges_TypedChangesRejectGapsAndRemainLoadableUnderAnotherCulture()
	{
		using var database = new InMemoryDatabaseTestScope();
		var actor = EditorActor();
		Command(actor, "StackDecorator", "stackdecorator new Quantities Range");
		Command(actor, "StackDecorator", "stackdecorator set ranges set 1 0 1.5 little");
		Command(actor, "StackDecorator", "stackdecorator set ranges add 1.5 10 lots");
		var expected = database.Context.StackDecorators.Single().Definition;
		Command(actor, "StackDecorator", "stackdecorator set ranges add 11 20 invalid");
		Command(actor, "StackDecorator", "stackdecorator set description Updated");
		Assert.AreEqual(expected, database.Context.StackDecorators.Single().Definition);
		var culture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			Assert.IsNotNull(StackDecorator.LoadStackDecorator(database.Context.StackDecorators.Single()));
		}
		finally { CultureInfo.CurrentCulture = culture; }
		Command(actor, "TraitDecorator", "traitdecorator new Grades Range");
		Command(actor, "TraitDecorator", "traitdecorator set ranges set 1 0 50 novice");
		Command(actor, "TraitDecorator", "traitdecorator set ranges add 50 100 expert");
		var contents = database.Context.TraitDecorators.Single().Contents;
		Command(actor, "TraitDecorator", "traitdecorator set ranges remove 1");
		Assert.IsNotNull(DecoratorBase.GetDecorator(database.Context.TraitDecorators.Single(), false));
		Assert.AreNotEqual(contents, database.Context.TraitDecorators.Single().Contents);
	}

	private static Mock<ICharacter> EditorActor()
	{
		var world = BuilderWorkflowRegressionTests.World();
		world.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition>());
		world.SetupGet(x => x.TraitExpressions).Returns(new All<ITraitExpression>());
		var actor = BuilderWorkflowRegressionTests.Actor(world.Object);
		var editors = new Dictionary<Type, object?>();
		actor.Setup(x => x.SetEditingItem(It.IsAny<It.IsAnyType>())).Callback(new InvocationAction(call => editors[call.Method.GetGenericArguments()[0]] = call.Arguments[0]));
		actor.Setup(x => x.EditingItem<It.IsAnyType>()).Returns(new InvocationFunc(call => editors.GetValueOrDefault(call.Method.GetGenericArguments()[0])));
		return actor;
	}

	private static void Command(Mock<ICharacter> actor, string method, string input) => BuilderWorkflowRegressionTests.Invoke(typeof(BuilderModule), method, actor.Object, input);
}
