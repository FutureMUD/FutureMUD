#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using DbFutureProg = MudSharp.Models.FutureProg;
using RuntimeFutureProg = MudSharp.FutureProg.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargenFreeKnowledgeProgCompilationTests
{
	[ClassInitialize]
	public static void InitialiseFutureProg(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

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
		ProgVariableTypes returnType, ProgVariableTypes firstParameterType, bool includeTraitParameter = false)
	{
		DbFutureProg prog = new()
		{
			Id = id,
			FunctionName = name,
			FunctionComment = name,
			FunctionText = returnType == ProgVariableTypes.Boolean ? "return true" :
				"var knowledges as knowledge collection\nreturn @knowledges",
			ReturnTypeDefinition = returnType.ToStorageString(),
			Category = "Tests",
			Subcategory = "Chargen",
			Public = false,
			AcceptsAnyParameters = false,
			StaticType = 0
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

	private static Mock<IFutureProg> RuntimeGate(long id, string name, ProgVariableTypes firstParameterType)
	{
		Mock<IFutureProg> gate = new();
		gate.SetupGet(x => x.Id).Returns(id);
		gate.SetupGet(x => x.Name).Returns(name);
		gate.SetupGet(x => x.FrameworkItemType).Returns("FutureProg");
		gate.SetupGet(x => x.FunctionName).Returns(name);
		gate.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		gate.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
			.Returns((IEnumerable<ProgVariableTypes> parameters) =>
			{
				ProgVariableTypes[] types = parameters.ToArray();
				return types.Length == 2 && types[0].CompatibleWith(firstParameterType) &&
				       types[1].CompatibleWith(ProgVariableTypes.Trait);
			});
		return gate;
	}

	private static IUneditableAll<IFutureProg> BuildProgRepository(IEnumerable<IFutureProg> progs)
	{
		List<IFutureProg> items = progs.ToList();
		Mock<IUneditableAll<IFutureProg>> repository = new();
		repository.Setup(x => x.GetEnumerator()).Returns(() => items.GetEnumerator());
		repository.SetupGet(x => x.Count).Returns(items.Count);
		return repository.Object;
	}

	[TestMethod]
	public void ReconciledHealthAndCultureBlocks_CompileAsFutureProg()
	{
		using FuturemudDatabaseContext context = BuildContext();
		DbFutureProg target = AddProg(
			context,
			1,
			ChargenFreeKnowledgeProgReconciler.ProgName,
			ProgVariableTypes.Knowledge | ProgVariableTypes.Collection,
			ProgVariableTypes.Toon);
		DbFutureProg healthGate = AddProg(context, 2, "HealthCanPickBroadMedicalKnowledgeAtChargen",
			ProgVariableTypes.Boolean, ProgVariableTypes.Toon, true);
		DbFutureProg scriptGate = AddProg(context, 3, "CanPickLatinScriptKnowledge",
			ProgVariableTypes.Boolean, ProgVariableTypes.Chargen, true);
		Knowledge healthKnowledge = new()
		{
			Id = 1,
			Name = "Medicine",
			Description = "Medicine",
			LongDescription = "Medicine",
			Type = "Medicine",
			Subtype = "Human",
			LearnableType = 0,
			LearnDifficulty = 0,
			TeachDifficulty = 0,
			LearningSessionsRequired = 0,
			CanAcquireProg = healthGate,
			CanAcquireProgId = healthGate.Id
		};
		Knowledge scriptKnowledge = new()
		{
			Id = 2,
			Name = "Latin Script",
			Description = "Latin Script",
			LongDescription = "Latin Script",
			Type = "Script",
			Subtype = "Alphabet",
			LearnableType = 0,
			LearnDifficulty = 0,
			TeachDifficulty = 0,
			LearningSessionsRequired = 0,
			CanAcquireProg = scriptGate,
			CanAcquireProgId = scriptGate.Id
		};
		context.Knowledges.AddRange(healthKnowledge, scriptKnowledge);
		context.Scripts.Add(new Script
		{
			Id = 1,
			Name = "Latin",
			KnownScriptDescription = "Latin",
			UnknownScriptDescription = "Latin",
			Knowledge = scriptKnowledge,
			KnowledgeId = scriptKnowledge.Id,
			DocumentLengthModifier = 1.0,
			InkUseModifier = 1.0
		});
		context.SaveChanges();
		ChargenFreeKnowledgeProgReconcileResult result = ChargenFreeKnowledgeProgReconciler.Reconcile(context);
		Assert.AreEqual(ChargenFreeKnowledgeProgReconcileStatus.Updated, result.Status);

		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.FutureProgs).Returns(BuildProgRepository([
			RuntimeGate(2, healthGate.FunctionName, ProgVariableTypes.Toon).Object,
			RuntimeGate(3, scriptGate.FunctionName, ProgVariableTypes.Chargen).Object
		]));
		RuntimeFutureProg runtimeProg = new(
			gameworld.Object,
			ChargenFreeKnowledgeProgReconciler.ProgName,
			ProgVariableTypes.Knowledge | ProgVariableTypes.Collection,
			[Tuple.Create(ProgVariableTypes.Chargen, "ch")],
			target.FunctionText);

		Assert.IsTrue(runtimeProg.Compile(), runtimeProg.CompileError);
	}
}
