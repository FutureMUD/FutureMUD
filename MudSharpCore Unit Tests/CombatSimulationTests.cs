using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Simulation;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Law;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSimulationTests
{
	[TestMethod]
	[DataRow(null, true)]
	[DataRow("", true)]
	[DataRow("Production", true)]
	[DataRow("staging", true)]
	[DataRow("Development", false)]
	[DataRow("dev", false)]
	[DataRow("Test", false)]
	[DataRow("local", false)]
	public void IsProductionEnvironment_KnownAndUnknownValues_UsesFailClosedPolicy(
		string? environment,
		bool expected)
	{
		Assert.AreEqual(expected, CombatSimulationCommand.IsProductionEnvironment(environment));
	}

	[TestMethod]
	public void Validate_TwoTeamsAndValidLimits_HasNoStructuralErrors()
	{
		var request = CreateRequest("red", "blue");

		var messages = new CombatSimulationService().Validate(request);

		Assert.IsFalse(messages.Any(x => x.IsError));
	}

	[TestMethod]
	public void Validate_OnlyOneTeam_ReturnsStructuralError()
	{
		var request = CreateRequest("red", "RED");

		var messages = new CombatSimulationService().Validate(request);

		Assert.IsTrue(messages.Any(x => x.IsError && x.Message.Contains("opposing teams")));
	}

	[TestMethod]
	public void ConstantsPushRandom_SameSeed_ReplaysSequenceAndRestoresAmbientValue()
	{
		int[] first;
		int[] second;
		using (MudSharp.Framework.Constants.PushRandom(new Random(8675309)))
		{
			first = Enumerable.Range(0, 5)
				.Select(_ => MudSharp.Framework.Constants.Random.Next())
				.ToArray();
		}

		using (MudSharp.Framework.Constants.PushRandom(new Random(8675309)))
		{
			second = Enumerable.Range(0, 5)
				.Select(_ => MudSharp.Framework.Constants.Random.Next())
				.ToArray();
		}

		CollectionAssert.AreEqual(first, second);
	}

	[TestMethod]
	public void RuntimeSideEffectContext_NestedScopes_RestorePreviousPolicy()
	{
		Assert.IsFalse(RuntimeSideEffectContext.IsCrimeCreationSuppressed);

		using (RuntimeSideEffectContext.SuppressCrimeCreation())
		{
			Assert.IsTrue(RuntimeSideEffectContext.IsCrimeCreationSuppressed);
			using (RuntimeSideEffectContext.SuppressCrimeCreation())
			{
				Assert.IsTrue(RuntimeSideEffectContext.IsCrimeCreationSuppressed);
			}

			Assert.IsTrue(RuntimeSideEffectContext.IsCrimeCreationSuppressed);
		}

		Assert.IsFalse(RuntimeSideEffectContext.IsCrimeCreationSuppressed);
	}

	[TestMethod]
	public void LegalAuthorityCheckPossibleCrime_Suppressed_CreatesNoCrime()
	{
		var authority = (LegalAuthority)RuntimeHelpers.GetUninitializedObject(typeof(LegalAuthority));
		var criminal = new Mock<ICharacter>();

		using var scope = RuntimeSideEffectContext.SuppressCrimeCreation();
		var crimes = authority.CheckPossibleCrime(criminal.Object, CrimeTypes.Murder, null!, null!, string.Empty,
			null!, true, null!).ToList();

		Assert.AreEqual(0, crimes.Count);
	}

	[TestMethod]
	public void CombatSimulationRuntimeScope_SameSeedAndEpoch_ReplaysAmbientStateAndRestoresPolicy()
	{
		var epoch = new DateTimeOffset(2030, 4, 5, 6, 7, 8, TimeSpan.Zero);
		var gameworld = new Mock<IFuturemud>();
		int[] first;
		int[] second;
		double[] firstExpressions;
		double[] secondExpressions;
		var expression = new ExpressionEngine.Expression("rand(1,1000000) + dice(2,1000000)");
		var firstFingerprint = new CombatSimulationExecutionFingerprint(8675309);

		using (new CombatSimulationRuntimeScope(gameworld.Object, new AdvancingTimeProvider(epoch), 8675309,
				firstFingerprint))
		{
			Assert.AreEqual(epoch.UtcDateTime, RuntimeClock.UtcNow);
			Assert.IsTrue(RuntimeSideEffectContext.IsCrimeCreationSuppressed);
			first = Enumerable.Range(0, 5)
				.Select(_ => Constants.Random.Next())
				.ToArray();
			firstExpressions = Enumerable.Range(0, 5)
				.Select(_ => expression.EvaluateDouble())
				.ToArray();
		}

		Assert.IsFalse(RuntimeSideEffectContext.IsCrimeCreationSuppressed);
		var secondFingerprint = new CombatSimulationExecutionFingerprint(8675309);
		using (new CombatSimulationRuntimeScope(gameworld.Object, new AdvancingTimeProvider(epoch), 8675309,
				secondFingerprint))
		{
			second = Enumerable.Range(0, 5)
				.Select(_ => Constants.Random.Next())
				.ToArray();
			secondExpressions = Enumerable.Range(0, 5)
				.Select(_ => expression.EvaluateDouble())
				.ToArray();
		}

		CollectionAssert.AreEqual(first, second);
		CollectionAssert.AreEqual(firstExpressions, secondExpressions);
		Assert.AreEqual(
			firstFingerprint.Complete(CombatSimulationRunStatus.Completed, "red", TimeSpan.FromSeconds(1), 10, []),
			secondFingerprint.Complete(CombatSimulationRunStatus.Completed, "red", TimeSpan.FromSeconds(1), 10, []));
		Assert.IsFalse(RuntimeSideEffectContext.IsCrimeCreationSuppressed);
	}

	[TestMethod]
	public void CombatSimulationRuntimeScope_DoesNotFlowSandboxServicesToChildWork()
	{
		var epoch = new DateTimeOffset(2030, 4, 5, 6, 7, 8, TimeSpan.Zero);
		var gameworld = new Mock<IFuturemud>();

		using var scope = new CombatSimulationRuntimeScope(gameworld.Object, new AdvancingTimeProvider(epoch),
			8675309, new CombatSimulationExecutionFingerprint(8675309));
		var childState = Task.Run(() =>
			(RuntimeClock.UtcNow, RuntimeSideEffectContext.IsCrimeCreationSuppressed))
			.GetAwaiter()
			.GetResult();

		Assert.AreNotEqual(epoch.UtcDateTime, childState.Item1);
		Assert.IsFalse(childState.Item2);
	}

	[TestMethod]
	public void CombatSimulationExecutionFingerprint_DifferentTrace_ProducesDifferentDigest()
	{
		var first = new CombatSimulationExecutionFingerprint(1234);
		first.RecordRandom("next", 5);
		var second = new CombatSimulationExecutionFingerprint(1234);
		second.RecordRandom("next", 6);

		Assert.AreNotEqual(
			first.Complete(CombatSimulationRunStatus.Completed, "red", TimeSpan.Zero, 1, []),
			second.Complete(CombatSimulationRunStatus.Completed, "red", TimeSpan.Zero, 1, []));
	}

	[TestMethod]
	public void Validate_ManualCombatSettings_ReportsNoInputRisks()
	{
		var settings = new Mock<ICharacterCombatSettings>();
		settings.SetupGet(x => x.InventoryManagement).Returns(AutomaticInventorySettings.FullyManual);
		settings.SetupGet(x => x.MovementManagement).Returns(AutomaticMovementSettings.FullyManual);
		settings.SetupGet(x => x.ManualPositionManagement).Returns(true);
		settings.SetupGet(x => x.RangedManagement).Returns(AutomaticRangedSettings.FullyManual);
		var request = CreateRequest("red", "blue", settings.Object);

		var messages = new CombatSimulationService().Validate(request);

		Assert.IsTrue(messages.Any(x => x.Message.Contains("manual inventory")));
		Assert.IsTrue(messages.Any(x => x.Message.Contains("manual movement")));
		Assert.IsTrue(messages.Any(x => x.Message.Contains("manual ranged")));
		Assert.IsTrue(messages.Any(x => x.Message.Contains("no weighted automatic attack")));
	}

	[TestMethod]
	public void CombatSimulationCell_DatabaseLocationId_UsesPersistentSourceCell()
	{
		var source = new Mock<ICell>();
		source.SetupGet(x => x.Id).Returns(42L);
		source.SetupGet(x => x.Gameworld).Returns(new Mock<MudSharp.Framework.IFuturemud>().Object);
		source.SetupGet(x => x.Overlays).Returns(Array.Empty<ICellOverlay>());

		var simulationCell = new Cell(source.Object, -1L);

		Assert.AreEqual(-1L, simulationCell.Id);
		Assert.AreEqual(42L, simulationCell.DatabaseLocationId);
	}

	[TestMethod]
	public void CombatSimulationRunStatus_DescribeEnum_ProducesReadableStatus()
	{
		Assert.AreEqual("Event Limit", CombatSimulationRunStatus.EventLimit.DescribeEnum(true));
	}

	[TestMethod]
	public void InitialiseCombatSimulationBody_InvokesBodyLogin()
	{
		var body = new Mock<IBody>();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);

		CombatSimulationService.InitialiseCombatSimulationBody(character.Object);

		body.Verify(x => x.Login(), Times.Once);
	}

	[TestMethod]
	public void ValidateBatch_ValidSeedSequence_HasNoStructuralErrors()
	{
		var messages = new CombatSimulationService().ValidateBatch(CreateBatchRequest(1_000, 25, 5));

		Assert.IsFalse(messages.Any(x => x.IsError));
	}

	[TestMethod]
	public void ValidateBatch_OverflowingSeedSequence_ReportsError()
	{
		var messages = new CombatSimulationService().ValidateBatch(CreateBatchRequest(int.MaxValue, 1, 2));

		Assert.IsTrue(messages.Any(x => x.IsError && x.Message.Contains("32-bit seed")));
	}

	private static CombatSimulationBatchRequest CreateBatchRequest(int firstSeed, int seedIncrement, int runCount)
	{
		var request = CreateRequest("red", "blue");
		return new CombatSimulationBatchRequest(
			Guid.NewGuid(),
			request.RequestedBy,
			request.Scene,
			request.Participants,
			firstSeed,
			seedIncrement,
			runCount,
			request.MaximumVirtualTime,
			request.MaximumEvents,
			request.MaximumWallClockTime,
			TimeSpan.FromMinutes(10),
			true);
	}

	private static CombatSimulationRequest CreateRequest(
		string firstTeam,
		string secondTeam,
		ICharacterCombatSettings? firstSettings = null)
	{
		var first = new Mock<INPCTemplate>();
		first.SetupGet(x => x.Id).Returns(1);
		first.SetupGet(x => x.Name).Returns("First");
		first.SetupGet(x => x.DefaultCombatSetting).Returns(firstSettings);
		var second = new Mock<INPCTemplate>();
		second.SetupGet(x => x.Id).Returns(2);
		second.SetupGet(x => x.Name).Returns("Second");
		return new CombatSimulationRequest(
			Guid.NewGuid(),
			new Mock<ICharacter>().Object,
			new Mock<ICell>().Object,
			[
				new CombatSimulationParticipantRequest(1, firstTeam,
					CombatSimulationSourceType.NpcTemplate, null, first.Object),
				new CombatSimulationParticipantRequest(2, secondTeam,
					CombatSimulationSourceType.NpcTemplate, null, second.Object)
			],
			12345,
			TimeSpan.FromMinutes(30),
			100_000,
			10_000,
			TimeSpan.FromSeconds(60),
			false);
	}
}
