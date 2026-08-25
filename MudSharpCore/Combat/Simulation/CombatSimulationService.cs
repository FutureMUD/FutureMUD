using System.Diagnostics;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.Templates;
using BodyImplementation = MudSharp.Body.Implementations.Body;

#nullable enable

namespace MudSharp.Combat.Simulation;

public sealed class CombatSimulationService : ICombatSimulationService
{
	private const int NoCombatProgressEventThreshold = 1_000;
	private static readonly TimeSpan NoCombatProgressVirtualTimeThreshold = TimeSpan.FromSeconds(30);

	private sealed record SourceSnapshot(
		CombatSimulationParticipantRequest Request,
		XElement? CharacterEffects,
		XElement? BodyEffects);

	private sealed record RuntimeParticipant(
		CombatSimulationParticipantRequest Request,
		ICharacter Character,
		string Name);

	private sealed record CombatProgressSnapshot(
		CharacterState State,
		IPerceiver? Target,
		bool MeleeRange,
		CombatStrategyMode Strategy,
		double BloodVolume,
		double Stamina,
		int Wounds,
		int TranscriptEntries);

	private static int _simulationRunning;
	private static long _nextTemporaryCellId = -1_000_000;

	private static IReadOnlyList<ICell> StagedCells(CombatSimulationRequest request)
	{
		var cells = new List<ICell>();
		if (request.Scene is not null)
		{
			cells.Add(request.Scene);
		}

		if (request.Cells is not null)
		{
			cells.AddRange(request.Cells.OfType<ICell>());
		}

		cells.AddRange(request.Participants
			.Select(x => x.StartingCell)
			.OfType<ICell>());
		return cells.Distinct(ReferenceEqualityComparer.Instance).Cast<ICell>().ToList();
	}

	private static bool IsStagedCell(IReadOnlyList<ICell> cells, ICell cell)
	{
		return cells.Any(x => ReferenceEquals(x, cell));
	}

	public IReadOnlyList<CombatSimulationValidationMessage> Validate(CombatSimulationRequest request)
	{
		var messages = new List<CombatSimulationValidationMessage>();
		if (request.Scene is null)
		{
			messages.Add(new CombatSimulationValidationMessage(true, "A combat scene is required."));
		}
		else if (request.Cells is not null && !IsStagedCell(request.Cells, request.Scene))
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The default combat cell must be included in the staged cells."));
		}

		if (request.Cells is not null && request.Cells.Count == 0)
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"At least one staged combat cell is required."));
		}

		if (request.Cells is not null && request.Cells.Count != request.Cells.Distinct(ReferenceEqualityComparer.Instance).Count())
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"Each staged combat cell may only be included once."));
		}

		if (request.Participants.Count < 2)
		{
			messages.Add(new CombatSimulationValidationMessage(true, "At least two combatants are required."));
		}

		if (request.Participants
			    .Select(x => x.Team)
			    .Distinct(StringComparer.InvariantCultureIgnoreCase)
			    .Count() < 2)
		{
			messages.Add(new CombatSimulationValidationMessage(true, "At least two opposing teams are required."));
		}

		if (request.Participants.GroupBy(x => x.Slot).Any(x => x.Count() > 1))
		{
			messages.Add(new CombatSimulationValidationMessage(true, "Combatant slot numbers must be unique."));
		}

		if (request.MaximumVirtualTime <= TimeSpan.Zero || request.MaximumVirtualTime > TimeSpan.FromDays(1))
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The virtual-time limit must be greater than zero and no more than one day."));
		}

		if (request.MaximumEvents is < 1 or > 1_000_000)
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The event limit must be between 1 and 1,000,000."));
		}

		if (request.MaximumTranscriptEntries is < 0 or > 100_000)
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The transcript limit must be between 0 and 100,000 entries."));
		}

		if (request.MaximumWallClockTime <= TimeSpan.Zero || request.MaximumWallClockTime > TimeSpan.FromMinutes(10))
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The wall-clock limit must be greater than zero and no more than ten minutes."));
		}

		foreach (var participant in request.Participants)
		{
			var startingCell = participant.StartingCell ?? request.Scene;
			if (startingCell is null)
			{
				messages.Add(new CombatSimulationValidationMessage(true,
					$"Combatant slot {participant.Slot:N0} has no starting cell."));
			}
			else
			{
				if (request.Cells is not null && !IsStagedCell(request.Cells, startingCell))
				{
					messages.Add(new CombatSimulationValidationMessage(true,
						$"Combatant slot {participant.Slot:N0} starts in a cell that is not staged for this simulation."));
				}

				var terrain = startingCell.Terrain(null);
				if (terrain is not null && !terrain.TerrainLayers.Contains(participant.StartingLayer))
				{
					messages.Add(new CombatSimulationValidationMessage(true,
						$"Combatant slot {participant.Slot:N0} starts on {participant.StartingLayer.DescribeEnum(true)}, which is not available in its selected cell."));
				}
			}

			if (string.IsNullOrWhiteSpace(participant.Team))
			{
				messages.Add(new CombatSimulationValidationMessage(true,
					$"Combatant slot {participant.Slot:N0} has no team."));
			}

			var settings = participant.SourceType switch
			{
				CombatSimulationSourceType.Character => participant.Character?.CombatSettings,
				CombatSimulationSourceType.NpcTemplate => participant.NpcTemplate?.DefaultCombatSetting,
				_ => null
			};

			switch (participant.SourceType)
			{
				case CombatSimulationSourceType.Character when participant.Character is null:
					messages.Add(new CombatSimulationValidationMessage(true,
						$"Combatant slot {participant.Slot:N0} has no character source."));
					continue;
				case CombatSimulationSourceType.NpcTemplate when participant.NpcTemplate is null:
					messages.Add(new CombatSimulationValidationMessage(true,
						$"Combatant slot {participant.Slot:N0} has no NPC-template source."));
					continue;
			}

			if (participant.Character?.State.HasFlag(CharacterState.Dead) == true)
			{
				messages.Add(new CombatSimulationValidationMessage(true,
					$"{participant.SourceDescription} is dead and cannot enter a simulation."));
			}

			if (participant.Character?.Combat is not null)
			{
				messages.Add(new CombatSimulationValidationMessage(false,
					$"{participant.SourceDescription} is already in combat; its pre-combat live state will be cloned."));
			}

			if (settings is null)
			{
				messages.Add(new CombatSimulationValidationMessage(false,
					$"{participant.SourceDescription} has no explicit combat settings; the engine fallback will be used."));
			}
			else if (settings.InventoryManagement == AutomaticInventorySettings.FullyManual)
			{
				messages.Add(new CombatSimulationValidationMessage(false,
					$"{participant.SourceDescription} uses fully manual inventory management and may not draw or reload weapons."));
			}

			if (settings is not null &&
			    (settings.MovementManagement == AutomaticMovementSettings.FullyManual ||
			     settings.ManualPositionManagement))
			{
				messages.Add(new CombatSimulationValidationMessage(false,
					$"{participant.SourceDescription} uses manual movement or position management and may stall without input."));
			}

			if (settings?.RangedManagement == AutomaticRangedSettings.FullyManual)
			{
				messages.Add(new CombatSimulationValidationMessage(false,
					$"{participant.SourceDescription} uses fully manual ranged management and may not fire automatically."));
			}

			if (settings is not null &&
			    settings.WeaponUsePercentage + settings.NaturalWeaponPercentage + settings.MagicUsePercentage +
			    settings.PsychicUsePercentage + settings.AuxiliaryPercentage <= 0.0)
			{
				messages.Add(new CombatSimulationValidationMessage(false,
					$"{participant.SourceDescription} has no weighted automatic attack type."));
			}

			if (participant.NpcTemplate?.OnLoadProg is not null)
			{
				messages.Add(new CombatSimulationValidationMessage(false,
					$"{participant.SourceDescription} has an on-load prog. It will run in the simulation and may have external side effects."));
			}
		}

		messages.Add(new CombatSimulationValidationMessage(false,
			"EF SaveChanges calls and ordinary save-queue work are suppressed in the simulation; direct Dapper commands and external services invoked by hooks or progs cannot be unwound."));
		return messages;
	}

	public IReadOnlyList<CombatSimulationValidationMessage> ValidateBatch(CombatSimulationBatchRequest request)
	{
		var messages = Validate(new CombatSimulationRequest(
			request.BatchId,
			request.RequestedBy,
			request.Scene,
			request.Participants,
			request.FirstSeed,
			request.MaximumVirtualTime,
			request.MaximumEvents,
			0,
			request.MaximumWallClockTime,
			request.Force,
			request.Cells)).ToList();

		if (request.RunCount is < 1 or > 100)
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The batch run count must be between 1 and 100."));
		}

		if (request.MaximumBatchWallClockTime <= TimeSpan.Zero ||
		    request.MaximumBatchWallClockTime > TimeSpan.FromMinutes(10))
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The batch wall-clock limit must be greater than zero and no more than ten minutes."));
		}

		if (request.RunCount > 0 && !TryGetBatchSeed(request, request.RunCount - 1, out _))
		{
			messages.Add(new CombatSimulationValidationMessage(true,
				"The starting seed and increment must produce a valid 32-bit seed for every run."));
		}

		return messages;
	}

	public CombatSimulationBatchResult RunBatch(CombatSimulationBatchRequest request)
	{
		var validation = ValidateBatch(request).ToList();
		if (validation.Any(x => x.IsError) || (!request.Force && validation.Any()))
		{
			return EmptyBatchResult(request, validation,
				validation.Any(x => x.IsError)
					? "The combat-simulation batch has validation errors."
					: "The combat-simulation batch has warnings; run it with force after reviewing them.");
		}

		var batchWallClock = Stopwatch.StartNew();
		var batchEpoch = TimeProvider.System.GetUtcNow();
		var results = new List<CombatSimulationResult>();
		string? errorMessage = null;
		for (var runIndex = 0; runIndex < request.RunCount; runIndex++)
		{
			var remainingWallClock = request.MaximumBatchWallClockTime - batchWallClock.Elapsed;
			if (remainingWallClock <= TimeSpan.Zero)
			{
				errorMessage = $"The batch wall-clock limit of {request.MaximumBatchWallClockTime.Describe(request.RequestedBy)} was reached after {results.Count:N0} run(s).";
				break;
			}

			if (!TryGetBatchSeed(request, runIndex, out var seed))
			{
				errorMessage = "The batch seed sequence exceeded the supported 32-bit range.";
				break;
			}

			var perRunWallClock = remainingWallClock < request.MaximumWallClockTime
				? remainingWallClock
				: request.MaximumWallClockTime;
			results.Add(Run(new CombatSimulationRequest(
				Guid.NewGuid(),
				request.RequestedBy,
				request.Scene,
				request.Participants,
				seed,
				request.MaximumVirtualTime,
				request.MaximumEvents,
				0,
				perRunWallClock,
				request.Force,
				request.Cells), batchEpoch, request.RunCount > 1 && request.SeedIncrement == 0));
		}

		return BuildBatchResult(request, results, validation, batchWallClock.Elapsed, errorMessage);
	}

	public CombatSimulationResult Run(CombatSimulationRequest request)
	{
		return Run(request, TimeProvider.System.GetUtcNow(), false);
	}

	private CombatSimulationResult Run(
		CombatSimulationRequest request,
		DateTimeOffset simulationEpoch,
		bool captureRandomCallSites)
	{
		var validation = Validate(request).ToList();
		if (validation.Any(x => x.IsError) || (!request.Force && validation.Any()))
		{
			return EmptyResult(request, CombatSimulationRunStatus.ValidationFailed, validation,
				validation.Any(x => x.IsError)
					? "The simulation has validation errors."
					: "The simulation has warnings; run it with force after reviewing them.");
		}

		if (Interlocked.CompareExchange(ref _simulationRunning, 1, 0) != 0)
		{
			return EmptyResult(request, CombatSimulationRunStatus.Error, validation,
				"Another combat simulation is already running.");
		}

		var wallClock = Stopwatch.StartNew();
		var snapshots = new List<SourceSnapshot>();
		var sourceCellEffects = new Dictionary<ICell, XElement?>(ReferenceEqualityComparer.Instance);
		var originalActors = new HashSet<ICharacter>(ReferenceEqualityComparer.Instance);
		var originalCachedActors = new HashSet<ICharacter>(ReferenceEqualityComparer.Instance);
		var originalBodies = new HashSet<MudSharp.Body.IBody>(ReferenceEqualityComparer.Instance);
		var originalItems = new HashSet<MudSharp.GameItems.IGameItem>(ReferenceEqualityComparer.Instance);
		var cleanupSimulationArtifacts = false;
		var runtimeParticipants = new List<RuntimeParticipant>();
		var simulationCells = new Dictionary<ICell, Cell>(ReferenceEqualityComparer.Instance);
		var simulationExits = new List<IExit>();
		CombatSimulationTranscript? transcript = null;
		IDbContextTransaction? transaction = null;
		IDisposable? databaseScope = null;
		FMDB? database = null;
		CombatSimulationRuntimeScope? runtimeScope = null;
		var timeProvider = new AdvancingTimeProvider(simulationEpoch);
		var startedAt = timeProvider.GetUtcNow();
		var eventCount = 0;
		var status = CombatSimulationRunStatus.Error;
		string? winningTeam = null;
		string? errorMessage = null;
		CombatSimulationResult? result = null;
		var executionFingerprint = new CombatSimulationExecutionFingerprint(request.Seed, captureRandomCallSites);

		try
		{
			runtimeScope = new CombatSimulationRuntimeScope(
				request.RequestedBy.Gameworld,
				timeProvider,
				request.Seed,
				executionFingerprint);

			snapshots = CaptureSourceSnapshots(request);
			foreach (var sourceCell in StagedCells(request).OfType<Cell>())
			{
				sourceCellEffects[sourceCell] = sourceCell.SaveEffects();
			}
			originalActors.UnionWith(request.RequestedBy.Gameworld.Actors);
			originalCachedActors.UnionWith(request.RequestedBy.Gameworld.CachedActors);
			originalBodies.UnionWith(request.RequestedBy.Gameworld.Bodies);
			originalItems.UnionWith(request.RequestedBy.Gameworld.Items);
			cleanupSimulationArtifacts = true;

			databaseScope = FMDB.BeginIsolatedScope(suppressEfWrites: true);
			database = new FMDB();
			transaction = FMDB.Context!.Database.BeginTransaction();

			transcript = new CombatSimulationTranscript(timeProvider, startedAt, request.MaximumTranscriptEntries,
				executionFingerprint);
			foreach (var sourceCell in StagedCells(request))
			{
				var simulationCell = new Cell(sourceCell, Interlocked.Decrement(ref _nextTemporaryCellId));
				request.RequestedBy.Gameworld.Add(simulationCell);
				simulationCells[sourceCell] = simulationCell;
				if (sourceCellEffects.TryGetValue(sourceCell, out var sourceEffects) && sourceEffects is not null)
				{
					TryRestoreEffects(() => simulationCell.RestoreCombatSimulationEffects(sourceEffects), validation,
						$"Some effects in {sourceCell.Name} could not be cloned and were omitted.");
				}
			}

			CreateSimulationTopology(request, simulationCells, simulationExits, validation, executionFingerprint);

			foreach (var snapshot in snapshots)
			{
				executionFingerprint.RecordMaterialisation(snapshot.Request);
				var sourceCell = snapshot.Request.StartingCell ?? request.Scene;
				var participant = MaterialiseParticipant(snapshot, simulationCells[sourceCell], transcript, validation,
					executionFingerprint);
				runtimeParticipants.Add(participant);
			}

			request.RequestedBy.Gameworld.SaveManager.Flush();

			var combat = new CombatSimulationCombat(request.RequestedBy.Gameworld);
			foreach (var participant in runtimeParticipants)
			{
				combat.JoinCombat(participant.Character, participant.Request.Team);
			}

			foreach (var participant in runtimeParticipants)
			{
				participant.Character.AcquireTarget();
				participant.Character.MeleeRange = participant.Request.StartsInMelee;
				participant.Character.CombatStrategyMode = participant.Request.StartsInMelee
					? participant.Character.CombatSettings.PreferredMeleeMode
					: participant.Character.CombatSettings.PreferredRangedMode;
			}

			request.RequestedBy.Gameworld.HeartbeatManager.StartHeartbeatTick();
			var terminal = new Dictionary<ICharacter, CombatSimulationOutcome>(ReferenceEqualityComparer.Instance);
			var lastProgress = CaptureCombatProgress(runtimeParticipants, transcript);
			var lastProgressAt = timeProvider.GetUtcNow();
			var eventCountAtLastProgress = eventCount;
			while (true)
			{
				UpdateTerminalParticipants(runtimeParticipants, terminal, combat);
				var activeTeams = runtimeParticipants
					.Where(x => !terminal.ContainsKey(x.Character))
					.Select(x => x.Request.Team)
					.Distinct(StringComparer.InvariantCultureIgnoreCase)
					.ToList();
				if (activeTeams.Count <= 1)
				{
					winningTeam = activeTeams.SingleOrDefault();
					status = CombatSimulationRunStatus.Completed;
					break;
				}

				if (wallClock.Elapsed >= request.MaximumWallClockTime)
				{
					status = CombatSimulationRunStatus.WallClockLimit;
					break;
				}

				if (eventCount >= request.MaximumEvents)
				{
					status = CombatSimulationRunStatus.EventLimit;
					break;
				}

				var elapsed = timeProvider.GetUtcNow() - startedAt;
				if (elapsed >= request.MaximumVirtualTime)
				{
					status = CombatSimulationRunStatus.VirtualTimeLimit;
					break;
				}

				var nextTrigger = new[]
					{
						request.RequestedBy.Gameworld.Scheduler.NextTriggerUtc,
						request.RequestedBy.Gameworld.EffectScheduler.NextTriggerUtc
					}
					.Where(x => x.HasValue)
					.Select(x => x!.Value)
					.DefaultIfEmpty()
					.Min();
				if (nextTrigger == default)
				{
					status = CombatSimulationRunStatus.Stalemate;
					break;
				}

				var maximumUtc = (startedAt + request.MaximumVirtualTime).UtcDateTime;
				timeProvider.AdvanceTo(nextTrigger > maximumUtc ? maximumUtc : nextTrigger);
				request.RequestedBy.Gameworld.Scheduler.CheckSchedules();
				var schedulerFired = request.RequestedBy.Gameworld.Scheduler.LastCheckFiredCount;
				eventCount += schedulerFired;
				request.RequestedBy.Gameworld.EffectScheduler.CheckSchedules();
				var effectSchedulerFired = request.RequestedBy.Gameworld.EffectScheduler.LastCheckFiredCount;
				eventCount += effectSchedulerFired;
				executionFingerprint.RecordSchedulerTick(timeProvider.GetUtcNow(), schedulerFired,
					effectSchedulerFired, eventCount);

				var currentProgress = CaptureCombatProgress(runtimeParticipants, transcript);
				if (!currentProgress.SequenceEqual(lastProgress))
				{
					lastProgress = currentProgress;
					lastProgressAt = timeProvider.GetUtcNow();
					eventCountAtLastProgress = eventCount;
					continue;
				}

				if (eventCount - eventCountAtLastProgress >= NoCombatProgressEventThreshold &&
				    timeProvider.GetUtcNow() - lastProgressAt >= NoCombatProgressVirtualTimeThreshold)
				{
					validation.Add(new CombatSimulationValidationMessage(false,
						$"No combat-relevant participant state changed for {NoCombatProgressVirtualTimeThreshold.Describe(request.RequestedBy)} virtual time and {NoCombatProgressEventThreshold:N0} fired schedules. The staged combat is stalled and may require automatic engagement or a different combat setting."));
					status = CombatSimulationRunStatus.Stalemate;
					break;
				}
			}

			UpdateTerminalParticipants(runtimeParticipants, terminal, combat);
			foreach (var participant in runtimeParticipants.Where(x => !terminal.ContainsKey(x.Character)))
			{
				terminal[participant.Character] = status == CombatSimulationRunStatus.Completed
					? CombatSimulationOutcome.SurvivingWinner
					: CombatSimulationOutcome.Stalemate;
			}

			combat.EndCombat(false);
			request.RequestedBy.Gameworld.SaveManager.Flush();
			result = BuildResult(request, status, winningTeam, eventCount, runtimeParticipants, terminal,
				validation, transcript, timeProvider.GetUtcNow() - startedAt, wallClock.Elapsed, errorMessage,
				executionFingerprint);
		}
		catch (Exception ex)
		{
			errorMessage = DescribeFailure(ex);
			result = BuildResult(request, CombatSimulationRunStatus.Error, null, eventCount, runtimeParticipants,
				new Dictionary<ICharacter, CombatSimulationOutcome>(ReferenceEqualityComparer.Instance), validation,
				transcript, timeProvider.GetUtcNow() - startedAt, wallClock.Elapsed, errorMessage, executionFingerprint);
		}
		finally
		{
			var cleanupErrors = new List<string>();
			if (cleanupSimulationArtifacts)
			{
				TryCleanup(
					() => Cleanup(request.RequestedBy.Gameworld, originalActors, originalCachedActors, originalBodies,
						originalItems, simulationExits, simulationCells.Values), cleanupErrors);
			}

			TryCleanup(() => transaction?.Rollback(), cleanupErrors);
			TryCleanup(() => transaction?.Dispose(), cleanupErrors);
			TryCleanup(() => runtimeScope?.Dispose(), cleanupErrors);
			TryCleanup(() => database?.Dispose(), cleanupErrors);
			TryCleanup(() => databaseScope?.Dispose(), cleanupErrors);
			Interlocked.Exchange(ref _simulationRunning, 0);

			if (cleanupErrors.Count > 0)
			{
				var cleanupMessage = $"Combat simulation cleanup reported errors: {cleanupErrors.ListToString()}.";
				validation.Add(new CombatSimulationValidationMessage(true, cleanupMessage));
				if (result is not null)
				{
					result = result with
					{
						Status = CombatSimulationRunStatus.Error,
						ErrorMessage = string.IsNullOrWhiteSpace(result.ErrorMessage)
							? cleanupMessage
							: $"{result.ErrorMessage} {cleanupMessage}"
					};
				}

				request.RequestedBy.OutputHandler.Send(
					cleanupMessage.ColourError());
			}
		}

		return result ?? EmptyResult(request, CombatSimulationRunStatus.Error, validation,
			"The simulation did not produce a result.");
	}

	private static void TryCleanup(Action action, ICollection<string> errors)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			errors.Add(ex.Message);
		}
	}

	private static string DescribeFailure(Exception exception)
	{
#if DEBUG
		return exception.ToString();
#else
		var message = exception.GetBaseException().Message;
		var lines = message
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var embeddedInnerException = lines.FirstOrDefault(x => x.StartsWith("--->", StringComparison.Ordinal));
		if (embeddedInnerException is not null)
		{
			var separator = embeddedInnerException.IndexOf(": ", StringComparison.Ordinal);
			if (separator >= 0)
			{
				return embeddedInnerException[(separator + 2)..];
			}
		}

		return lines.FirstOrDefault() ?? exception.GetType().Name;
#endif
	}

	private static List<SourceSnapshot> CaptureSourceSnapshots(CombatSimulationRequest request)
	{
		return request.Participants.Select(participant =>
		{
			if (participant.Character is not Character.Character character ||
			    participant.Character.Body is not BodyImplementation body)
			{
				return new SourceSnapshot(participant, null, null);
			}

			return new SourceSnapshot(participant, character.SaveEffects(), body.SaveEffects());
		}).ToList();
	}

	private static void CreateSimulationTopology(
		CombatSimulationRequest request,
		IReadOnlyDictionary<ICell, Cell> simulationCells,
		ICollection<IExit> simulationExits,
		ICollection<CombatSimulationValidationMessage> validation,
		CombatSimulationExecutionFingerprint executionFingerprint)
	{
		var seenSourceExits = new HashSet<IExit>(ReferenceEqualityComparer.Instance);
		foreach (var sourceCell in StagedCells(request))
		{
			foreach (var sourceCellExit in request.RequestedBy.Gameworld.ExitManager.GetExitsFor(sourceCell))
			{
				var sourceExit = sourceCellExit.Exit;
				if (!seenSourceExits.Add(sourceExit) || !simulationCells.TryGetValue(sourceCellExit.Destination, out var destination))
				{
					continue;
				}

				if (sourceExit.Door is not null)
				{
					validation.Add(new CombatSimulationValidationMessage(false,
						$"The staged exit from {sourceCell.Name} to {sourceCellExit.Destination.Name} has a door and was omitted so the simulation cannot mutate live door state."));
					continue;
				}

				var transientExit = new TransientExit(
					request.RequestedBy.Gameworld,
					simulationCells[sourceCell],
					destination,
					sourceExit,
					sourceCell,
					$"combat-simulation:{request.RunId:D}:{sourceExit.Id:N0}");
				request.RequestedBy.Gameworld.ExitManager.RegisterTransientExit(transientExit);
				simulationExits.Add(transientExit);
				executionFingerprint.RecordTopology(
					$"exit:{sourceCell.Id}:{sourceCellExit.Destination.Id}:{sourceExit.Id}:{sourceCellExit.OutboundDirection}");
			}
		}

		foreach (var sourceCell in StagedCells(request).OrderBy(x => x.Id))
		{
			executionFingerprint.RecordTopology($"cell:{sourceCell.Id}");
		}
	}

	private static RuntimeParticipant MaterialiseParticipant(
		SourceSnapshot snapshot,
		Cell simulationCell,
		CombatSimulationTranscript transcript,
		ICollection<CombatSimulationValidationMessage> validation,
		CombatSimulationExecutionFingerprint executionFingerprint)
	{
		ICharacter character;
		CombatSimulationNpc? materialisedNpc = null;
		if (snapshot.Request.SourceType == CombatSimulationSourceType.Character)
		{
			var source = snapshot.Request.Character!;
			var template = (SimpleCharacterTemplate)source.GetCharacterTemplate() with
			{
				SelectedStartingLocation = simulationCell,
				SelectedRoles = []
			};
			var clone = new CombatSimulationCharacter(source.Gameworld, template);
			character = clone;
			source.Gameworld.Add(clone, true);
			simulationCell.Enter(clone, noSave: true, roomLayer: snapshot.Request.StartingLayer);
			clone.CopyCombatSimulationStateFrom(source);
			((BodyImplementation)clone.Body).CopyCombatSimulationBiologyFrom(source.Body);
			CharacterInstanceService.CloneInventory(source, clone, out var inventoryResult);
			if (inventoryResult.FailedCloned > 0)
			{
				validation.Add(new CombatSimulationValidationMessage(false,
					$"{inventoryResult.FailedCloned:N0} inventory item(s) could not be cloned for {snapshot.Request.SourceDescription}."));
			}

			if (snapshot.CharacterEffects is not null)
			{
				TryRestoreEffects(() => clone.RestoreCombatSimulationEffects(snapshot.CharacterEffects), validation,
					$"Some character effects could not be cloned for {snapshot.Request.SourceDescription}.");
			}

			if (snapshot.BodyEffects is not null)
			{
				TryRestoreEffects(() => ((BodyImplementation)clone.Body).RestoreCombatSimulationEffects(snapshot.BodyEffects), validation,
					$"Some body effects could not be cloned for {snapshot.Request.SourceDescription}.");
			}
		}
		else
		{
			var npcTemplate = snapshot.Request.NpcTemplate!;
			var template = npcTemplate.GetCharacterTemplate(simulationCell);
			var npc = new CombatSimulationNpc(npcTemplate.Gameworld, template, npcTemplate);
			materialisedNpc = npc;
			character = npc;
			npcTemplate.Gameworld.Add(npc, true);
			simulationCell.Enter(npc, noSave: true, roomLayer: snapshot.Request.StartingLayer);
			foreach (var warning in npcTemplate.ApplyTemplateLoadAdditions(npc, false))
			{
				validation.Add(new CombatSimulationValidationMessage(false,
					$"{snapshot.Request.SourceDescription}: {warning}"));
			}

			npcTemplate.OnLoadProg?.Execute(npc);
		}

		character.PositionState = snapshot.Request.StartingPosition ?? PositionStanding.Instance;
		character.PositionModifier = PositionModifier.None;
		character.PositionTarget = null;

		var name = character.PersonalName.GetName(Character.Name.NameStyle.SimpleFull);
		character.Register(new CombatSimulationOutputHandler(transcript,
			$"#{snapshot.Request.Slot:N0} {name}",
			$"slot:{snapshot.Request.Slot}"));
		InitialiseCombatSimulationBody(character);
		materialisedNpc?.HandleEvent(EventType.NPCOnGameLoadFinished, materialisedNpc);
		RecordMaterialisedRuntimeState(executionFingerprint, snapshot.Request.Slot, character);
		return new RuntimeParticipant(snapshot.Request, character, name);
	}

	internal static void InitialiseCombatSimulationBody(ICharacter character)
	{
		character.Body.Login();
	}

	private static void RecordMaterialisedRuntimeState(
		CombatSimulationExecutionFingerprint fingerprint,
		int slot,
		ICharacter character)
	{
		var template = character.GetCharacterTemplate();
		fingerprint.RecordMaterialisationRuntimeState(slot, "identity",
		[
			$"race:{character.Race.Id}",
			$"culture:{character.Culture.Id}",
			$"gender:{(int)character.Gender.Enum}",
			$"height:{character.Height.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}",
			$"weight:{character.Weight.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}",
			$"stamina:{character.CurrentStamina.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}",
			$"location:{(character.Location as Cell)?.DatabaseLocationId ?? character.Location.Id}",
			$"layer:{(int)character.RoomLayer}",
			$"position:{character.PositionState.Id}"
		]);
		fingerprint.RecordMaterialisationRuntimeState(slot, "attributes", template.SelectedAttributes
			.OrderBy(x => x.Definition.Id)
			.Select(x => $"{x.Definition.Id}:{x.RawValue.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}"));
		fingerprint.RecordMaterialisationRuntimeState(slot, "skills", template.SkillValues
			.OrderBy(x => x.Item1.Id)
			.Select(x => $"{x.Item1.Id}:{x.Item2.ToString("R", System.Globalization.CultureInfo.InvariantCulture)}"));
		fingerprint.RecordMaterialisationRuntimeState(slot, "characteristics", template.SelectedCharacteristics
			.OrderBy(x => x.Item1.Id)
			.Select(x => $"{x.Item1.Id}:{x.Item2.Id}"));
		fingerprint.RecordMaterialisationRuntimeState(slot, "bodyparts", character.Body.Bodyparts
			.OrderBy(x => x.Id)
			.Select(x => x.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)));
		fingerprint.RecordMaterialisationRuntimeState(slot, "organs", character.Body.Organs
			.OrderBy(x => x.Id)
			.Select(x => x.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)));
		fingerprint.RecordMaterialisationRuntimeState(slot, "bones", character.Body.Bones
			.OrderBy(x => x.Id)
			.Select(x => x.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)));
		fingerprint.RecordMaterialisationRuntimeState(slot, "character-effects", character.Effects
			.Select(x => x.GetType().FullName ?? x.GetType().Name)
			.Order());
		fingerprint.RecordMaterialisationRuntimeState(slot, "body-effects", character.Body.Effects
			.Select(x => x.GetType().FullName ?? x.GetType().Name)
			.Order());
	}

	private static void TryRestoreEffects(
		Action action,
		ICollection<CombatSimulationValidationMessage> validation,
		string warning)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			validation.Add(new CombatSimulationValidationMessage(false, $"{warning} {ex.Message}"));
		}
	}

	private static void UpdateTerminalParticipants(
		IEnumerable<RuntimeParticipant> participants,
		IDictionary<ICharacter, CombatSimulationOutcome> terminal,
		CombatSimulationCombat combat)
	{
		var participantList = participants.ToList();
		foreach (var participant in participantList)
		{
			var character = participant.Character;
			if (character.State.HasFlag(CharacterState.Dead))
			{
				// A combatant may be removed as incapacitated or grappled and subsequently die from
				// bleeding or another heartbeat-driven effect. Death is the final state and must
				// supersede that earlier terminal outcome in the report.
				terminal[character] = CombatSimulationOutcome.Dead;
				if (character.Combat == combat)
				{
					combat.LeaveCombat(character);
				}

				continue;
			}

			if (terminal.ContainsKey(character))
			{
				continue;
			}

			CombatSimulationOutcome? outcome = null;
			if (character.CombinedEffectsOfType<IBeingGrappled>().Any(x => x.UnderControl))
			{
				outcome = CombatSimulationOutcome.FullGrappleControl;
			}
			else if (!character.State.IsAble())
			{
				outcome = CombatSimulationOutcome.Incapacitated;
			}
			if (outcome is null)
			{
				continue;
			}

			terminal[character] = outcome.Value;
			if (character.Combat == combat)
			{
				combat.LeaveCombat(character);
			}
		}

		foreach (var participant in participantList
			         .Where(x => !terminal.ContainsKey(x.Character) && x.Character.Combat is null)
			         .ToList())
		{
			var hasActiveOpponent = participantList.Any(x =>
				!string.Equals(x.Request.Team, participant.Request.Team,
					StringComparison.InvariantCultureIgnoreCase) &&
				!terminal.ContainsKey(x.Character));
			if (!hasActiveOpponent)
			{
				continue;
			}

			terminal[participant.Character] = combat.DepartureModeFor(participant.Character) == CombatStrategyMode.Flee
				? CombatSimulationOutcome.Fled
				: CombatSimulationOutcome.Withdrew;
		}
	}

	private static IReadOnlyList<CombatProgressSnapshot> CaptureCombatProgress(
		IEnumerable<RuntimeParticipant> participants,
		CombatSimulationTranscript transcript)
	{
		var transcriptEntries = transcript.Entries.Count;
		return participants
			.OrderBy(x => x.Request.Slot)
			.Select(x => new CombatProgressSnapshot(
				x.Character.State,
				x.Character.CombatTarget,
				x.Character.MeleeRange,
				x.Character.CombatStrategyMode,
				x.Character.Body.CurrentBloodVolumeLitres,
				x.Character.Body.CurrentStamina,
				x.Character.Body.Wounds.Count(),
				transcriptEntries))
			.ToList();
	}

	private static CombatSimulationResult BuildResult(
		CombatSimulationRequest request,
		CombatSimulationRunStatus status,
		string? winningTeam,
		int eventCount,
		IEnumerable<RuntimeParticipant> participants,
		IReadOnlyDictionary<ICharacter, CombatSimulationOutcome> outcomes,
		IReadOnlyList<CombatSimulationValidationMessage> validation,
		CombatSimulationTranscript? transcript,
		TimeSpan virtualDuration,
		TimeSpan wallClockDuration,
		string? errorMessage,
		CombatSimulationExecutionFingerprint executionFingerprint)
	{
		var results = participants.Select(x =>
		{
			var body = x.Character.Body;
			return new CombatSimulationParticipantResult(
				x.Request.Slot,
				x.Request.Team,
				x.Name,
				outcomes.GetValueOrDefault(x.Character, CombatSimulationOutcome.Unknown),
				x.Character.State,
				body.TotalBloodVolumeLitres > 0.0
					? body.CurrentBloodVolumeLitres / body.TotalBloodVolumeLitres
					: 0.0,
				body.MaximumStamina > 0.0 ? body.CurrentStamina / body.MaximumStamina : 0.0,
				body.Wounds.Count(),
				x.Character.CombinedEffectsOfType<IBeingGrappled>().Any(y => y.UnderControl));
		}).ToList();

		var boundedVirtualDuration = virtualDuration < TimeSpan.Zero || virtualDuration > request.MaximumVirtualTime
			? request.MaximumVirtualTime
			: virtualDuration;
		var fingerprint = executionFingerprint.Complete(status, winningTeam, boundedVirtualDuration, eventCount, results);

		return new CombatSimulationResult(
			request.RunId,
			status,
			winningTeam,
			request.Seed,
			boundedVirtualDuration,
			wallClockDuration,
			eventCount,
			results,
			validation,
			transcript?.Entries.ToList() ?? [],
			request.MaximumTranscriptEntries > 0 && transcript?.Truncated == true,
			fingerprint,
			executionFingerprint.TraceSummary,
			errorMessage);
	}

	private static bool TryGetBatchSeed(CombatSimulationBatchRequest request, int runIndex, out int seed)
	{
		var value = (long)request.FirstSeed + ((long)request.SeedIncrement * runIndex);
		if (value is < int.MinValue or > int.MaxValue)
		{
			seed = default;
			return false;
		}

		seed = (int)value;
		return true;
	}

	private static CombatSimulationBatchResult BuildBatchResult(
		CombatSimulationBatchRequest request,
		IReadOnlyList<CombatSimulationResult> runs,
		IReadOnlyList<CombatSimulationValidationMessage> validation,
		TimeSpan batchWallClockDuration,
		string? errorMessage)
	{
		var totalVirtualDuration = runs.Aggregate(TimeSpan.Zero, (total, run) => total + run.VirtualDuration);
		var totalWallClockDuration = runs.Aggregate(TimeSpan.Zero, (total, run) => total + run.WallClockDuration);
		var teamResults = request.Participants
			.Select(x => x.Team)
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.OrderBy(x => x)
			.Select(team => new CombatSimulationBatchTeamResult(
				team,
				runs.Count(x => string.Equals(x.WinningTeam, team, StringComparison.InvariantCultureIgnoreCase)),
				runs.Count == 0
					? 0.0
					: (double)runs.Count(x => string.Equals(x.WinningTeam, team,
						StringComparison.InvariantCultureIgnoreCase)) / runs.Count))
			.ToList();
		var statusResults = runs
			.GroupBy(x => x.Status)
			.OrderBy(x => (int)x.Key)
			.Select(x => new CombatSimulationBatchStatusResult(x.Key, x.Count()))
			.ToList();
		var outcomeResults = runs
			.SelectMany(x => x.Participants)
			.GroupBy(x => x.Outcome)
			.OrderBy(x => (int)x.Key)
			.Select(x => new CombatSimulationBatchOutcomeResult(x.Key, x.Count()))
			.ToList();

		return new CombatSimulationBatchResult(
			request.BatchId,
			request.FirstSeed,
			request.SeedIncrement,
			request.RunCount,
			runs,
			teamResults,
			statusResults,
			outcomeResults,
			totalVirtualDuration,
			runs.Count == 0 ? TimeSpan.Zero : TimeSpan.FromTicks(totalVirtualDuration.Ticks / runs.Count),
			runs.Count == 0 ? TimeSpan.Zero : runs.Min(x => x.VirtualDuration),
			runs.Count == 0 ? TimeSpan.Zero : runs.Max(x => x.VirtualDuration),
			totalWallClockDuration,
			runs.Count == 0 ? TimeSpan.Zero : TimeSpan.FromTicks(totalWallClockDuration.Ticks / runs.Count),
			batchWallClockDuration,
			validation,
			errorMessage);
	}

	private static CombatSimulationResult EmptyResult(
		CombatSimulationRequest request,
		CombatSimulationRunStatus status,
		IReadOnlyList<CombatSimulationValidationMessage> validation,
		string error)
	{
		var executionFingerprint = new CombatSimulationExecutionFingerprint(request.Seed);
		var fingerprint = executionFingerprint.Complete(status, null, TimeSpan.Zero, 0, []);
		return new CombatSimulationResult(request.RunId, status, null, request.Seed, TimeSpan.Zero, TimeSpan.Zero, 0,
			[], validation, [], false, fingerprint, executionFingerprint.TraceSummary, error);
	}

	private static CombatSimulationBatchResult EmptyBatchResult(
		CombatSimulationBatchRequest request,
		IReadOnlyList<CombatSimulationValidationMessage> validation,
		string error)
	{
		return new CombatSimulationBatchResult(
			request.BatchId,
			request.FirstSeed,
			request.SeedIncrement,
			request.RunCount,
			[],
			[],
			[],
			[],
			TimeSpan.Zero,
			TimeSpan.Zero,
			TimeSpan.Zero,
			TimeSpan.Zero,
			TimeSpan.Zero,
			TimeSpan.Zero,
			TimeSpan.Zero,
			validation,
			error);
	}

	private static void Cleanup(
		IFuturemud gameworld,
		ISet<ICharacter> originalActors,
		ISet<ICharacter> originalCachedActors,
		ISet<MudSharp.Body.IBody> originalBodies,
		ISet<MudSharp.GameItems.IGameItem> originalItems,
		IEnumerable<IExit> simulationExits,
		IEnumerable<Cell> simulationCells)
	{
		foreach (var simulationExit in simulationExits)
		{
			gameworld.ExitManager.UnregisterTransientExit(simulationExit);
		}

		foreach (var actor in gameworld.Actors.Where(x => !originalActors.Contains(x)).ToList())
		{
			DetachActor(actor);
			gameworld.Destroy(actor);
			(gameworld as ICombatSimulationRuntimeRegistry)?.ForgetCombatSimulationActor(actor);
		}

		if (gameworld is ICombatSimulationRuntimeRegistry registry)
		{
			foreach (var actor in gameworld.CachedActors.Where(x => !originalCachedActors.Contains(x)).ToList())
			{
				DetachActor(actor);
				registry.ForgetCombatSimulationActor(actor);
			}
		}

		foreach (var item in gameworld.Items.Where(x => !originalItems.Contains(x)).ToList())
		{
			item.Location?.Extract(item);
			gameworld.Destroy(item);
		}

		foreach (var body in gameworld.Bodies.Where(x => !originalBodies.Contains(x)).ToList())
		{
			gameworld.Destroy(body);
		}

		foreach (var simulationCell in simulationCells)
		{
			gameworld.Destroy(simulationCell);
		}
	}

	private static void DetachActor(ICharacter actor)
	{
		actor.Combat?.LeaveCombat(actor);
		if (actor.Location is Cell location && actor is Character.Character character)
		{
			location.RemoveCombatSimulationArtifact(actor);
			character.DetachCombatSimulationLocation();
			return;
		}

		actor.Location?.Leave(actor);
	}
}
