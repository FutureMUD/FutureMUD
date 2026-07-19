#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Linq;
using DbFutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargenFreeKnowledgeProgReconcilerTests
{
	private const string StockBody = """
		var knowledges as knowledge collection
		// Builder-authored rule
		if (@ch.Race.Name == "Human")
		  // Custom content remains here
		end if
		return @knowledges
		""";

	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options =
			new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
				.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static DbFutureProg AddProg(FuturemudDatabaseContext context, long id, string name,
		ProgVariableTypes returnType, ProgVariableTypes firstParameterType, string functionText = "return true",
		bool includeTraitParameter = false)
	{
		DbFutureProg prog = new()
		{
			Id = id,
			FunctionName = name,
			FunctionComment = $"{name} test prog",
			FunctionText = functionText,
			ReturnTypeDefinition = returnType.ToStorageString(),
			Category = "Tests",
			Subcategory = "Chargen",
			Public = false,
			AcceptsAnyParameters = false,
			StaticType = (int)FutureProgStaticType.NotStatic
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			FutureProgId = id,
			ParameterIndex = 0,
			ParameterName = "ch",
			ParameterTypeDefinition = firstParameterType.ToStorageString()
		});
		if (includeTraitParameter)
		{
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				FutureProgId = id,
				ParameterIndex = 1,
				ParameterName = "trait",
				ParameterTypeDefinition = ProgVariableTypes.Trait.ToStorageString()
			});
		}

		context.FutureProgs.Add(prog);
		return prog;
	}

	private static DbFutureProg AddTarget(FuturemudDatabaseContext context, string functionText = StockBody)
	{
		return AddProg(
			context,
			1,
			ChargenFreeKnowledgeProgReconciler.ProgName,
			ProgVariableTypes.Knowledge | ProgVariableTypes.Collection,
			ProgVariableTypes.Toon,
			functionText);
	}

	private static Knowledge AddKnowledge(FuturemudDatabaseContext context, long id, string name, string type,
		DbFutureProg acquisitionProg)
	{
		Knowledge knowledge = new()
		{
			Id = id,
			Name = name,
			Description = name,
			LongDescription = name,
			Type = type,
			Subtype = "Test",
			LearnableType = 0,
			LearnDifficulty = 0,
			TeachDifficulty = 0,
			LearningSessionsRequired = 0,
			CanAcquireProg = acquisitionProg,
			CanAcquireProgId = acquisitionProg.Id
		};
		context.Knowledges.Add(knowledge);
		return knowledge;
	}

	private static void AddScript(FuturemudDatabaseContext context, long id, string name, Knowledge knowledge)
	{
		context.Scripts.Add(new Script
		{
			Id = id,
			Name = name,
			KnownScriptDescription = name,
			UnknownScriptDescription = name,
			Knowledge = knowledge,
			KnowledgeId = knowledge.Id,
			DocumentLengthModifier = 1.0,
			InkUseModifier = 1.0
		});
	}

	[TestMethod]
	public void Reconcile_InsertsDeterministicManagedBlocks_PreservesCustomText_AndIsIdempotent()
	{
		using FuturemudDatabaseContext context = BuildContext();
		DbFutureProg target = AddTarget(context);
		DbFutureProg broadGate = AddProg(context, 2, "HealthCanPickBroadMedicalKnowledgeAtChargen",
			ProgVariableTypes.Boolean, ProgVariableTypes.Toon, includeTraitParameter: true);
		DbFutureProg surgicalGate = AddProg(context, 3, "HealthCanPickSurgicalKnowledgeAtChargen",
			ProgVariableTypes.Boolean, ProgVariableTypes.Toon, includeTraitParameter: true);
		DbFutureProg scriptGate = AddProg(context, 4, "CanPickLatinScriptKnowledge",
			ProgVariableTypes.Boolean, ProgVariableTypes.Chargen, includeTraitParameter: true);
		AddKnowledge(context, 10, "Zeta Medicine", "Medicine", broadGate);
		AddKnowledge(context, 11, "Alpha Surgery", "Medicine", surgicalGate);
		Knowledge scriptKnowledge = AddKnowledge(context, 12, "Latin Script", "Script", scriptGate);
		AddScript(context, 20, "Latin", scriptKnowledge);
		context.SaveChanges();

		Assert.IsTrue(ChargenFreeKnowledgeProgReconciler.HasRepairableHealthDrift(context));
		Assert.IsTrue(ChargenFreeKnowledgeProgReconciler.HasRepairableCultureDrift(context));
		ChargenFreeKnowledgeProgReconcileResult result =
			ChargenFreeKnowledgeProgReconciler.Reconcile(context);

		Assert.AreEqual(ChargenFreeKnowledgeProgReconcileStatus.Updated, result.Status);
		StringAssert.Contains(target.FunctionText, "// Builder-authored rule");
		StringAssert.Contains(target.FunctionText, ChargenFreeKnowledgeProgReconciler.HealthStartMarker);
		StringAssert.Contains(target.FunctionText, ChargenFreeKnowledgeProgReconciler.CultureStartMarker);
		StringAssert.Contains(target.FunctionText,
			"@HealthCanPickBroadMedicalKnowledgeAtChargen(@ch, @skill)");
		StringAssert.Contains(target.FunctionText,
			"@ch.Skills.Any(skill, @skill.Name == \"Literacy\") and @ch.Skills.Any(skill, @CanPickLatinScriptKnowledge(@ch, @skill))");
		Assert.IsTrue(target.FunctionText.IndexOf("Alpha Surgery", StringComparison.Ordinal) <
		              target.FunctionText.IndexOf("Zeta Medicine", StringComparison.Ordinal));
		Assert.AreEqual(ProgVariableTypes.Chargen.ToStorageString(),
			target.FutureProgsParameters.Single().ParameterTypeDefinition);
		Assert.AreEqual(1, target.FunctionText.Split(ChargenFreeKnowledgeProgReconciler.HealthStartMarker).Length - 1);
		Assert.AreEqual(1, target.FunctionText.Split(ChargenFreeKnowledgeProgReconciler.CultureStartMarker).Length - 1);

		string firstPass = target.FunctionText;
		ChargenFreeKnowledgeProgReconcileResult rerun = ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		Assert.AreEqual(ChargenFreeKnowledgeProgReconcileStatus.Unchanged, rerun.Status);
		Assert.AreEqual(firstPass, target.FunctionText);
		Assert.IsFalse(ChargenFreeKnowledgeProgReconciler.HasRepairableHealthDrift(context));
		Assert.IsFalse(ChargenFreeKnowledgeProgReconciler.HasRepairableCultureDrift(context));
	}

	[TestMethod]
	public void Reconcile_LeavesUnownedKnowledgeAndScriptRulesAlone()
	{
		using FuturemudDatabaseContext context = BuildContext();
		DbFutureProg target = AddTarget(context);
		DbFutureProg customHealthGate = AddProg(context, 2, "CustomMedicalGate", ProgVariableTypes.Boolean,
			ProgVariableTypes.Toon, includeTraitParameter: true);
		DbFutureProg customScriptGate = AddProg(context, 3, "CustomScriptGate", ProgVariableTypes.Boolean,
			ProgVariableTypes.Chargen, includeTraitParameter: true);
		AddKnowledge(context, 10, "Custom Medicine", "Medicine", customHealthGate);
		Knowledge customScript = AddKnowledge(context, 11, "Custom Script Knowledge", "Script", customScriptGate);
		AddScript(context, 20, "Custom", customScript);
		context.SaveChanges();

		ChargenFreeKnowledgeProgReconcileResult result = ChargenFreeKnowledgeProgReconciler.Reconcile(context);

		Assert.AreEqual(ChargenFreeKnowledgeProgReconcileStatus.Updated, result.Status,
			"The stock Toon parameter should still be upgraded to Chargen.");
		Assert.IsFalse(target.FunctionText.Contains("ToKnowledge(\"Custom Medicine\")", StringComparison.Ordinal));
		Assert.IsFalse(target.FunctionText.Contains("ToKnowledge(\"Custom Script Knowledge\")", StringComparison.Ordinal));
		Assert.IsFalse(target.FunctionText.Contains(ChargenFreeKnowledgeProgReconciler.HealthStartMarker,
			StringComparison.Ordinal));
		Assert.IsFalse(target.FunctionText.Contains(ChargenFreeKnowledgeProgReconciler.CultureStartMarker,
			StringComparison.Ordinal));
	}

	[TestMethod]
	public void Reconcile_MalformedManagedMarkers_RefusesAllChanges()
	{
		using FuturemudDatabaseContext context = BuildContext();
		string malformedBody = $"""
			var knowledges as knowledge collection
			{ChargenFreeKnowledgeProgReconciler.HealthStartMarker}
			{ChargenFreeKnowledgeProgReconciler.HealthStartMarker}
			{ChargenFreeKnowledgeProgReconciler.HealthEndMarker}
			return @knowledges
			""";
		DbFutureProg target = AddTarget(context, malformedBody);
		DbFutureProg healthGate = AddProg(context, 2, "HealthCanPickBroadMedicalKnowledgeAtChargen",
			ProgVariableTypes.Boolean, ProgVariableTypes.Toon, includeTraitParameter: true);
		AddKnowledge(context, 10, "Medicine", "Medicine", healthGate);
		context.SaveChanges();

		ChargenFreeKnowledgeProgReconcileResult result = ChargenFreeKnowledgeProgReconciler.Reconcile(context);

		Assert.AreEqual(ChargenFreeKnowledgeProgReconcileStatus.Unsafe, result.Status);
		Assert.AreEqual(malformedBody, target.FunctionText);
		Assert.AreEqual(ProgVariableTypes.Toon.ToStorageString(),
			target.FutureProgsParameters.Single().ParameterTypeDefinition);
		Assert.IsFalse(ChargenFreeKnowledgeProgReconciler.HasRepairableHealthDrift(context));
	}

	[TestMethod]
	public void Reconcile_MissingReturnAnchor_RefusesAllChanges()
	{
		using FuturemudDatabaseContext context = BuildContext();
		DbFutureProg target = AddTarget(context, "var knowledges as knowledge collection");
		DbFutureProg healthGate = AddProg(context, 2, "HealthCanPickBroadMedicalKnowledgeAtChargen",
			ProgVariableTypes.Boolean, ProgVariableTypes.Toon, includeTraitParameter: true);
		AddKnowledge(context, 10, "Medicine", "Medicine", healthGate);
		context.SaveChanges();

		ChargenFreeKnowledgeProgReconcileResult result = ChargenFreeKnowledgeProgReconciler.Reconcile(context);

		Assert.AreEqual(ChargenFreeKnowledgeProgReconcileStatus.Unsafe, result.Status);
		Assert.AreEqual("var knowledges as knowledge collection", target.FunctionText);
		Assert.IsFalse(ChargenFreeKnowledgeProgReconciler.HasRepairableHealthDrift(context));
	}

	[TestMethod]
	public void Reconcile_MissingChargenProg_IsInformationalNoOp()
	{
		using FuturemudDatabaseContext context = BuildContext();

		ChargenFreeKnowledgeProgReconcileResult result = ChargenFreeKnowledgeProgReconciler.Reconcile(context);

		Assert.AreEqual(ChargenFreeKnowledgeProgReconcileStatus.Missing, result.Status);
		StringAssert.Contains(result.Message, "when the Character Creation seeder runs");
	}
}
