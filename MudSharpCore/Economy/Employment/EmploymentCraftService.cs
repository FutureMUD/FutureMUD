using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Work.Crafts;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentCraftService
{
	private const string LegacyCraftPayloadPrefix = "craft-v1";
	private const string CraftPayloadVersion = "craft-v2";
	private const string CraftStationPayloadVersion = "craft-station-v1";
	private const string CraftReservationDurationMinutesSetting = "EmploymentCraftReservationDurationMinutes";
	private const string CraftStationCapacitySetting = "EmploymentCraftStationReservationCapacity";
	private const double DefaultReservationDurationMinutes = 30.0;
	private static readonly JsonSerializerOptions CraftStateJsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public static bool CanStartCraft(EmploymentTaskContext context, string craftSelector, ICharacter actor,
		out string reason)
	{
		if (!TryResolveCraft(actor, craftSelector, out var craft, out reason))
		{
			return false;
		}

		if (MatchingActiveCraft(actor, craft) is not null || TryGetPriorCraftState(context, craft, out _))
		{
			reason = string.Empty;
			return true;
		}

		if (!actor.State.IsAble())
		{
			reason = $"The assigned employee cannot craft while {actor.State.DescribeEnum(true)}.";
			return false;
		}

		if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			reason =
				$"The assigned employee must first stop {actor.Effects.Where(x => x.IsBlockingEffect("general")).Select(x => x.BlockingDescription("general", actor)).ListToString()}.";
			return false;
		}

		var (success, error) = craft.CanDoCraft(actor, null!, true, false);
		if (success)
		{
			reason = string.Empty;
			return true;
		}

		reason = error;
		return false;
	}

	public static bool TryStartCraft(EmploymentTaskContext context, ICharacter actor, string craftSelector,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!TryResolveCraft(actor, craftSelector, out var craft, out reason))
		{
			return false;
		}

		var hasPriorState = TryGetPriorCraftState(context, craft, out var priorState);
		if (MatchingActiveCraft(actor, craft) is { } active)
		{
			if (active.Component.HasFailed)
			{
				if (!hasPriorState)
				{
					reason = "Craft " + craft.Name + " has failed, but this task has no prior employment craft state to identify salvage safely.";
					return false;
				}

				return TryRecoverFailedCraft(context, actor, craft, priorState, out reason, out operationalState);
			}

			if (!TryCreateOrRefreshCraftState(context, actor, craft, active.Component,
				    hasPriorState ? priorState.PreExistingItemIds : PriorItemIds(context, actor.Location),
				    hasPriorState ? priorState.TaskInputItemIds : CurrentTaskInputItemIds(context, actor),
				    hasPriorState ? priorState : null,
				    out var refreshedState, out reason))
			{
				return false;
			}

			operationalState = InProgressState(refreshedState);
			reason = string.Empty;
			return true;
		}

		if (hasPriorState)
		{
			if (TryResolveActiveCraftItem(actor, priorState.ActiveItemId, craft, out var activeComponent))
			{
				if (activeComponent.HasFailed)
				{
					return TryRecoverFailedCraft(context, actor, craft, priorState, out reason, out operationalState);
				}

				var canResume = craft.CanResumeCraft(actor, activeComponent);
				if (!canResume.Success)
				{
					reason = canResume.Error;
					return false;
				}

				if (!TryCreateOrRefreshCraftState(context, actor, craft, activeComponent,
					    priorState.PreExistingItemIds, priorState.TaskInputItemIds, priorState, out var refreshedState, out reason))
				{
					return false;
				}

				craft.ResumeCraft(actor, activeComponent);
				operationalState = InProgressState(refreshedState);
				reason = string.Empty;
				return true;
			}

			return TryCompleteCraft(context, actor, craft, priorState, out reason, out operationalState);
		}

		if (!CanStartCraft(context, craftSelector, actor, out reason))
		{
			return false;
		}

		var preExistingItemIds = PriorItemIds(context, actor.Location);
		if (!TryCreateOrRefreshCraftState(context, actor, craft, null, preExistingItemIds,
			    CurrentTaskInputItemIds(context, actor), null, out var reservedState, out reason))
		{
			return false;
		}

		craft.BeginCraft(actor);
		if (MatchingActiveCraft(actor, craft) is not { } started)
		{
			ReleaseReservationLocks(context.CurrentTask, actor.Gameworld, reservedState);
			reason = $"The craft {craft.Name} did not create a native active craft effect.";
			return false;
		}

		var startedState = reservedState with
		{
			ActiveItemId = started.Component.Parent.Id
		};
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Started craft {craft.Name}.", context.CurrentTask?.CorrelationId);
		operationalState = InProgressState(startedState);
		reason = string.Empty;
		return true;
	}

	public static bool CanUseCraftStation(EmploymentTaskContext context, ICharacter actor, string stationSelector,
		out string reason)
	{
		if (actor.Location is null)
		{
			reason = "The assigned employee is not in a location.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(stationSelector) || stationSelector.EqualTo("here"))
		{
			reason = string.Empty;
			return true;
		}

		if (long.TryParse(stationSelector, out var cellId))
		{
			var cell = actor.Gameworld.Cells.Get(cellId);
			if (cell is null)
			{
				reason = $"There is no cell with id {cellId:N0}.";
				return false;
			}

			if (!context.CanPath(actor, cell))
			{
				reason = "The assigned employee cannot path to the selected craft station location.";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		var item = actor.TargetLocalOrHeldItem(stationSelector);
		if (item is null)
		{
			reason = $"The assigned employee cannot see any craft station matching {stationSelector.ColourCommand()} here.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public static bool TryUseCraftStation(EmploymentTaskContext context, ICharacter actor, string stationSelector,
		out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanUseCraftStation(context, actor, stationSelector, out reason))
		{
			return false;
		}

		var station = CreateStationReservation(actor, stationSelector);
		var stationCapacity = StationReservationCapacity(actor.Gameworld);
		if (!TryApplyStationReservationLock(context.CurrentTask, actor.Gameworld, station, stationCapacity, out reason))
		{
			operationalState = EmploymentActionStepOperationalState.Empty;
			return false;
		}

		operationalState = new EmploymentActionStepOperationalState(
			OperationalPayload: $"Craft station validated: {station.Description}",
			ReservationReference: station.ExpiresAt > DateTimeOffset.MinValue
				? $"craft-station capacity={stationCapacity:N0};expires={station.ExpiresAt:O}"
				: null,
			RouteResult: station.CellId is not null ? $"cell={station.CellId.Value:F0}" : null,
			CraftJobReference: SerializeStationState(station));
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Validated craft station {station.Description}.", context.CurrentTask?.CorrelationId);
		reason = string.Empty;
		return true;
	}

	public static void ReleaseCraftReservations(IEmploymentActiveTask? task, IFuturemud? gameworld = null)
	{
		if (task is null)
		{
			return;
		}

		gameworld ??= task.AssignedEmployee?.Gameworld ?? (task.Employer as IHaveFuturemud)?.Gameworld;
		if (gameworld is null)
		{
			return;
		}

		var itemIds = new HashSet<long>();
		foreach (var state in task.StepOperationalStates)
		{
			if (TryParseCraftState(state.CraftJobReference, null, out var craftState))
			{
				itemIds.UnionWith(ReservationItemIds(craftState));
			}

			if (TryParseStationState(state.CraftJobReference, out var stationState) &&
			    stationState.ItemId is { } stationItemId)
			{
				itemIds.Add(stationItemId);
			}
		}

		foreach (var item in itemIds
			         .Select(x => gameworld.TryGetItem(x, true))
			         .Where(x => x is not null)
			         .Cast<IGameItem>())
		{
			item.RemoveAllEffects<EmploymentCraftReservationEffect>(
				x => x.CorrelationId == task.CorrelationId,
				true);
		}
	}

	private static void ReleaseReservationLocks(IEmploymentActiveTask? task, IFuturemud? gameworld, CraftState state)
	{
		if (task is null || gameworld is null)
		{
			return;
		}

		foreach (var item in ReservationItemIds(state)
			         .Select(x => gameworld.TryGetItem(x, true))
			         .Where(x => x is not null)
			         .Cast<IGameItem>())
		{
			item.RemoveAllEffects<EmploymentCraftReservationEffect>(
				x => x.CorrelationId == task.CorrelationId,
				true);
		}
	}

	public static string DescribeCraftReference(string craftReference, IPerceiver voyeur)
	{
		if (TryParseCraftState(craftReference, null, out var craftState))
		{
			var itemCount = ReservationItemIds(craftState).Count;
			var expires = craftState.ExpiresAt > DateTimeOffset.MinValue
				? craftState.ExpiresAt.LocalDateTime.ToString("g", voyeur)
				: "legacy";
			var active = craftState.ActiveItemId > 0
				? $"active #{craftState.ActiveItemId:N0}, "
			: string.Empty;
			var station = craftState.Station is not null
				? $", station {craftState.Station.Description}"
			: string.Empty;
			var output = craftState.OutputItemIds.Any()
				? $", outputs {craftState.OutputItemIds.Select(x => $"#{x:N0}").ListToString()}"
			: string.Empty;
		var taskInputs = craftState.TaskInputItemIds.Any()
			? $", task inputs {craftState.TaskInputItemIds.Select(x => $"#{x:N0}").ListToString()}"
			: string.Empty;
		return $"{craftState.CraftName} r{craftState.Revision:N0} ({active}{itemCount:N0} reserved item(s), expires {expires}{station}{output}{taskInputs})";
		}

		if (TryParseStationState(craftReference, out var stationState))
		{
			return $"station {stationState.Description} reserved until {stationState.ExpiresAt.LocalDateTime.ToString("g", voyeur)}";
		}

		return craftReference;
	}

	private static bool TryResolveCraft(ICharacter actor, string craftSelector, out ICraft craft, out string reason)
	{
		craft = null!;
		if (string.IsNullOrWhiteSpace(craftSelector))
		{
			reason = "Which craft should this step start?";
			return false;
		}

		var resolved = actor.Gameworld.Crafts.GetByIdOrName(craftSelector);
		if (resolved is null)
		{
			reason = $"There is no craft matching {craftSelector.ColourCommand()}.";
			return false;
		}

		craft = resolved;
		if (!craft.AppearInCraftsList(actor) && !actor.IsAdministrator())
		{
			reason = $"The assigned employee does not know craft {craft.Name}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static IActiveCraftEffect? MatchingActiveCraft(ICharacter actor, ICraft craft)
	{
		return actor.EffectsOfType<IActiveCraftEffect>()
		            .FirstOrDefault(x => x.Component.Craft.Id == craft.Id &&
		                                 x.Component.Craft.RevisionNumber == craft.RevisionNumber);
	}

	private static bool TryResolveActiveCraftItem(ICharacter actor, long activeItemId, ICraft craft,
		out IActiveCraftGameItemComponent component)
	{
		component = null!;
		if (activeItemId <= 0)
		{
			return false;
		}

		var item = actor.Gameworld.TryGetItem(activeItemId, true);
		component = item?.GetItemType<IActiveCraftGameItemComponent>()!;
		return component is not null &&
		       component.Craft.Id == craft.Id &&
		       component.Craft.RevisionNumber == craft.RevisionNumber &&
		       !component.HasFinished;
	}

	private static bool TryCreateOrRefreshCraftState(EmploymentTaskContext context, ICharacter actor, ICraft craft,
		IActiveCraftGameItemComponent? component, IReadOnlyCollection<long> preExistingItemIds,
		IReadOnlyCollection<long> taskInputItemIds, CraftState? priorState, out CraftState state, out string reason)
	{
		var now = DateTimeOffset.UtcNow;
		var shouldRefresh = priorState?.Reservation is null ||
		                    priorState.ExpiresAt <= now ||
		                    priorState.CraftId != craft.Id ||
		                    priorState.Revision != craft.RevisionNumber;
		var reservation = priorState?.Reservation;
		if (shouldRefresh)
		{
			var fromPhase = component?.Phase ?? 1;
			var reservationResult = craft.CreateResourceReservation(actor, component!, fromPhase);
			if (!reservationResult.Success)
			{
				state = null!;
				reason = reservationResult.Error;
				return false;
			}

			reservation = reservationResult.Reservation;
		}

		var duration = ReservationDuration(actor);
		state = new CraftState(
			CraftPayloadVersion,
			craft.Id,
			craft.RevisionNumber,
			craft.Name,
			component?.Parent.Id ?? priorState?.ActiveItemId ?? 0L,
			preExistingItemIds.Any() ? preExistingItemIds : priorState?.PreExistingItemIds ?? [],
			taskInputItemIds.Any() ? taskInputItemIds : priorState?.TaskInputItemIds ?? [],
			shouldRefresh ? now : priorState!.ReservedAt,
			shouldRefresh ? now.Add(duration) : priorState!.ExpiresAt,
			reservation,
			FindPriorStationReservation(context) ?? priorState?.Station,
			priorState?.OutputItemIds ?? []);
		if (!TryApplyReservationLocks(context.CurrentTask, actor.Gameworld, state, component, out reason))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static EmploymentActionStepOperationalState InProgressState(CraftState state)
	{
		return new EmploymentActionStepOperationalState(
			OperationalPayload: "craft-status=inprogress",
			ReservationReference: $"craft expires={state.ExpiresAt:O}",
			CraftJobReference: SerializeCraftState(state));
	}

	private static bool TryCompleteCraft(EmploymentTaskContext context, ICharacter actor, ICraft craft,
		CraftState state, out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		var location = actor.Location;
		var candidates = location is null
			? new List<IGameItem>()
			: context.AvailableItems(location)
			         .Where(x => !state.PreExistingItemIds.Contains(x.Id))
			         .Where(x => x.GetItemType<IActiveCraftGameItemComponent>() is null)
			         .DistinctBy(x => x.Id)
			         .ToList();
		if (candidates.Any())
		{
			if (!context.TryCollectTaskItems(actor, candidates.Select(x => (x, location!)).ToList(), out reason))
			{
				operationalState = EmploymentActionStepOperationalState.Empty;
				return false;
			}
		}

		var completedState = state with
		{
			ActiveItemId = 0,
			OutputItemIds = candidates.Select(x => x.Id).ToList()
		};
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			candidates.Any()
				? $"Completed craft {craft.Name} and adopted {candidates.Count:N0} output item(s) into task custody."
				: $"Completed craft {craft.Name}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			OperationalPayload: "craft-status=complete",
			SelectedResources: candidates.Any()
				? context.FormatTaskItemCustodyForActor("collect", actor, candidates)
				: null,
			CraftJobReference: SerializeCraftState(completedState));
		ReleaseCraftReservations(context.CurrentTask, actor.Gameworld);
		reason = string.Empty;
		return true;
	}

	private static bool TryRecoverFailedCraft(EmploymentTaskContext context, ICharacter actor, ICraft craft,
		CraftState state, out string reason, out EmploymentActionStepOperationalState operationalState)
	{
		var location = actor.Location;
		var salvage = location is null
			? new List<IGameItem>()
			: context.AvailableItems(location)
			         .Where(x => !state.PreExistingItemIds.Contains(x.Id))
			         .Where(x => x.GetItemType<IActiveCraftGameItemComponent>() is null)
			         .DistinctBy(x => x.Id)
			         .ToList();
		if (salvage.Any() &&
		    !context.TryCollectTaskItems(actor, salvage.Select(x => (x, location!)).ToList(), out reason))
		{
			operationalState = EmploymentActionStepOperationalState.Empty;
			return false;
		}

		var failedState = state with
		{
			ActiveItemId = 0,
			OutputItemIds = salvage.Select(x => x.Id).ToList()
		};
		reason = salvage.Any()
			? "Craft " + craft.Name + " failed; adopted " + salvage.Count.ToString("N0") + " salvage item(s) into task custody for manager review."
			: "Craft " + craft.Name + " failed with no visible salvage for manager review.";
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor, reason,
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			OperationalPayload: "craft-status=failed",
			SelectedResources: salvage.Any()
				? context.FormatTaskItemCustodyForActor("collect", actor, salvage)
				: null,
			FailureDiagnostic: reason,
			CraftJobReference: SerializeCraftState(failedState));
		ReleaseCraftReservations(context.CurrentTask, actor.Gameworld);
		return true;
	}

	private static IReadOnlyCollection<long> PriorItemIds(EmploymentTaskContext context, ICell? location)
	{
		return location is null
			? []
			: context.AvailableItems(location).Select(x => x.Id).Distinct().ToList();
	}

	private static IReadOnlyCollection<long> CurrentTaskInputItemIds(EmploymentTaskContext context, ICharacter actor)
	{
		return context.CarriedTaskItems(actor)
		              .Select(x => x.Id)
		              .Distinct()
		              .ToList();
	}

	private static TimeSpan ReservationDuration(ICharacter actor)
	{
		var minutes = actor.Gameworld.GetStaticDouble(CraftReservationDurationMinutesSetting);
		if (!double.IsFinite(minutes) || minutes <= 0.0)
		{
			minutes = DefaultReservationDurationMinutes;
		}

		return TimeSpan.FromMinutes(minutes);
	}

	private static int StationReservationCapacity(IFuturemud? gameworld)
	{
		var configured = gameworld?.GetStaticDouble(CraftStationCapacitySetting) ?? 0.0;
		if (!double.IsFinite(configured) || configured <= 0.0)
		{
			return 1;
		}

		return Math.Max(1, (int)Math.Floor(configured));
	}

	private static CraftStationReservation CreateStationReservation(ICharacter actor, string stationSelector)
	{
		var now = DateTimeOffset.UtcNow;
		var expires = now.Add(ReservationDuration(actor));
		if (string.IsNullOrWhiteSpace(stationSelector) || stationSelector.EqualTo("here"))
		{
			return new CraftStationReservation(
				CraftStationPayloadVersion,
				string.IsNullOrWhiteSpace(stationSelector) ? "here" : stationSelector,
				actor.Location?.Id,
				null,
				actor.Location?.GetFriendlyReference(actor) ?? "here",
				now,
				expires);
		}

		if (long.TryParse(stationSelector, out var cellId))
		{
			var cell = actor.Gameworld.Cells.Get(cellId);
			return new CraftStationReservation(
				CraftStationPayloadVersion,
				stationSelector,
				cellId,
				null,
				cell?.GetFriendlyReference(actor) ?? $"cell #{cellId:N0}",
				now,
				expires);
		}

		var item = actor.TargetLocalOrHeldItem(stationSelector);
		return new CraftStationReservation(
			CraftStationPayloadVersion,
			stationSelector,
			actor.Location?.Id,
			item?.Id,
			item?.HowSeen(actor) ?? stationSelector,
			now,
			expires);
	}

	private static CraftStationReservation? FindPriorStationReservation(EmploymentTaskContext context)
	{
		var task = context.CurrentTask;
		if (task is null)
		{
			return null;
		}

		for (var i = Math.Min(context.CurrentStepIndex, task.StepOperationalStates.Count) - 1; i >= 0; i--)
		{
			if (task.StepStates[i] != EmploymentActionStepStatus.Completed)
			{
				continue;
			}

			if (TryParseStationState(task.StepOperationalStates[i].CraftJobReference, out var station))
			{
				return station;
			}
		}

		return null;
	}

	private static bool TryApplyReservationLocks(IEmploymentActiveTask? task, IFuturemud? gameworld, CraftState state,
		IActiveCraftGameItemComponent? component, out string reason)
	{
		reason = string.Empty;
		if (task is null || gameworld is null || state.ExpiresAt <= DateTimeOffset.UtcNow)
		{
			return true;
		}

		var duration = state.ExpiresAt - DateTimeOffset.UtcNow;
		var reservedItems = new List<(IGameItem Item, string Description)>();
		foreach (var itemId in ReservationItemIds(state, component))
		{
			if (gameworld.TryGetItem(itemId, true) is not { } item)
			{
				reason = $"Reserved craft resource item #{itemId:N0} could not be found.";
				return false;
			}

			var conflictingReservation = ActiveReservationEffects(item)
			                             .FirstOrDefault(x => x.CorrelationId != task.CorrelationId);
			if (conflictingReservation is not null)
			{
				reason =
					$"{item.Name} is already reserved for employment task {conflictingReservation.TaskName}.";
				return false;
			}

			reservedItems.Add((item, DescribeReservedItem(state, itemId)));
		}

		foreach (var item in reservedItems)
		{
			RefreshReservationEffect(item.Item, task, item.Description, state.ExpiresAt, duration);
		}

		return true;
	}

	private static bool TryApplyStationReservationLock(IEmploymentActiveTask? task, IFuturemud? gameworld,
		CraftStationReservation station, int stationCapacity, out string reason)
	{
		reason = string.Empty;
		if (task is null ||
		    gameworld is null ||
		    station.ItemId is not { } itemId ||
		    station.ExpiresAt <= DateTimeOffset.UtcNow)
		{
			return true;
		}

		if (gameworld.TryGetItem(itemId, true) is not { } item)
		{
			reason = $"Reserved craft station item #{itemId:N0} could not be found.";
			return false;
		}

		var conflictingReservations = ActiveReservationEffects(item)
		                              .Where(x => x.CorrelationId != task.CorrelationId)
		                              .ToList();
		if (conflictingReservations.Count >= stationCapacity)
		{
			var taskNames = conflictingReservations.Select(x => x.TaskName).Distinct().ListToString();
			var occupancy = $"{conflictingReservations.Count.ToString("N0")}/{stationCapacity.ToString("N0")}";
			var taskSuffix = string.IsNullOrWhiteSpace(taskNames) ? "." : $" by {taskNames}.";
			reason = $"{item.Name} has {occupancy} employment craft station reservations in use{taskSuffix}";
			return false;
		}

		RefreshReservationEffect(item, task, $"craft station {station.Description}", station.ExpiresAt,
			station.ExpiresAt - DateTimeOffset.UtcNow);
		return true;
	}

	private static void RefreshReservationEffect(IGameItem item, IEmploymentActiveTask task, string description,
		DateTimeOffset expiresAt, TimeSpan duration)
	{
		if (duration <= TimeSpan.Zero)
		{
			return;
		}

		item.RemoveAllEffects<EmploymentCraftReservationEffect>(
			x => x.CorrelationId == task.CorrelationId,
			false);
		item.AddEffect(new EmploymentCraftReservationEffect(
			item,
			task.Id,
			task.CorrelationId,
			task.Name,
			description,
			expiresAt), duration);
	}

	private static IReadOnlyCollection<EmploymentCraftReservationEffect> ActiveReservationEffects(IGameItem item)
	{
		var now = DateTimeOffset.UtcNow;
		var effects = (item.EffectsOfType<EmploymentCraftReservationEffect>() ??
		               Enumerable.Empty<EmploymentCraftReservationEffect>())
			.ToList();
		var expired = effects.Where(x => x.ExpiresAt <= now).ToList();
		if (expired.Any())
		{
			item.RemoveAllEffects<EmploymentCraftReservationEffect>(x => x.ExpiresAt <= now, true);
		}

		return effects
		       .Where(x => x.ExpiresAt > now)
		       .ToList();
	}

	private static IReadOnlyCollection<long> ReservationItemIds(CraftState state,
		IActiveCraftGameItemComponent? component = null)
	{
		var ids = new List<long>();
		if (state.Reservation is not null)
		{
			ids.AddRange(state.Reservation.Inputs
			                  .Where(x => component is null ||
			                              x.ConsumedPhase <= 0 ||
			                              x.ConsumedPhase >= component.Phase)
			                  .SelectMany(x => x.ItemIds));
			ids.AddRange(state.Reservation.Tools
			                  .Where(x => component is null || x.Phase >= component.Phase)
			                  .Select(x => x.ItemId));
		}

		if (state.Station?.ItemId is { } stationItemId)
		{
			ids.Add(stationItemId);
		}

		return ids.Where(x => x > 0).Distinct().ToList();
	}

	private static string DescribeReservedItem(CraftState state, long itemId)
	{
		var tool = state.Reservation?.Tools.FirstOrDefault(x => x.ItemId == itemId);
		if (tool is not null)
		{
			return $"craft tool {tool.ToolName} for phase {tool.Phase:N0}";
		}

		var input = state.Reservation?.Inputs.FirstOrDefault(x => x.ItemIds.Contains(itemId));
		if (input is not null)
		{
			return $"craft input {input.InputName}";
		}

		if (state.Station?.ItemId == itemId)
		{
			return $"craft station {state.Station.Description}";
		}

		return "craft resource";
	}

	private static string SerializeCraftState(CraftState state)
	{
		return JsonSerializer.Serialize(state, CraftStateJsonOptions);
	}

	private static string SerializeStationState(CraftStationReservation state)
	{
		return JsonSerializer.Serialize(state, CraftStateJsonOptions);
	}

	private static bool TryGetPriorCraftState(EmploymentTaskContext context, ICraft craft, out CraftState state)
	{
		var stateRecord = context.CurrentTask?.StepOperationalStates.ElementAtOrDefault(context.CurrentStepIndex);
		return TryParseCraftState(stateRecord?.CraftJobReference, craft, out state);
	}

	private static bool TryParseCraftState(string? text, ICraft? craft, out CraftState state)
	{
		state = null!;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (text.TrimStart().StartsWith("{", StringComparison.Ordinal))
		{
			try
			{
				var candidate = JsonSerializer.Deserialize<CraftState>(text, CraftStateJsonOptions);
				if (candidate is null ||
				    !candidate.Version.EqualTo(CraftPayloadVersion) ||
				    (craft is not null &&
				     (candidate.CraftId != craft.Id || candidate.Revision != craft.RevisionNumber)))
				{
					return false;
				}

				state = candidate with
				{
					PreExistingItemIds = candidate.PreExistingItemIds ?? [],
					TaskInputItemIds = candidate.TaskInputItemIds ?? [],
					OutputItemIds = candidate.OutputItemIds ?? [],
					Reservation = NormaliseReservation(candidate.Reservation)
				};
				return true;
			}
			catch (JsonException)
			{
				return false;
			}
		}

		var values = text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                 .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		                 .Where(x => x.Length == 2)
		                 .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
		if (!values.TryGetValue("prefix", out var prefix) ||
		    !prefix.EqualTo(LegacyCraftPayloadPrefix) ||
		    !values.TryGetValue("craft", out var craftText) ||
		    !long.TryParse(craftText, out var craftId) ||
		    (craft is not null && craftId != craft.Id) ||
		    !values.TryGetValue("revision", out var revisionText) ||
		    !int.TryParse(revisionText, out var revision) ||
		    (craft is not null && revision != craft.RevisionNumber))
		{
			return false;
		}

		var activeItemId = values.TryGetValue("active", out var activeText) &&
		                   long.TryParse(activeText, out var parsedActive)
			? parsedActive
			: 0L;
		var preExisting = values.TryGetValue("pre", out var preText)
			? preText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			         .Select(x => long.TryParse(x, out var value) ? value : 0L)
			         .Where(x => x > 0)
			         .ToList()
			: [];
		state = new CraftState(
			LegacyCraftPayloadPrefix,
			craftId,
			revision,
			craft?.Name ?? $"craft #{craftId:N0}",
			activeItemId,
			preExisting,
			[],
			DateTimeOffset.MinValue,
			DateTimeOffset.MinValue,
			null,
			null,
			[]);
		return true;
	}

	private static bool TryParseStationState(string? text, out CraftStationReservation state)
	{
		state = null!;
		if (string.IsNullOrWhiteSpace(text) ||
		    !text.TrimStart().StartsWith("{", StringComparison.Ordinal))
		{
			return false;
		}

		try
		{
			var candidate = JsonSerializer.Deserialize<CraftStationReservation>(text, CraftStateJsonOptions);
			if (candidate is null || !candidate.Version.EqualTo(CraftStationPayloadVersion))
			{
				return false;
			}

			state = candidate;
			return true;
		}
		catch (JsonException)
		{
			return false;
		}
	}

	private static CraftResourceReservation? NormaliseReservation(CraftResourceReservation? reservation)
	{
		if (reservation is null)
		{
			return null;
		}

		return reservation with
		{
			Inputs = reservation.Inputs?.Select(x => x with { ItemIds = x.ItemIds ?? [] }).ToList() ?? [],
			Tools = reservation.Tools ?? []
		};
	}

	private sealed record CraftState(
		string Version,
		long CraftId,
		int Revision,
		string CraftName,
		long ActiveItemId,
		IReadOnlyCollection<long> PreExistingItemIds,
		IReadOnlyCollection<long> TaskInputItemIds,
		DateTimeOffset ReservedAt,
		DateTimeOffset ExpiresAt,
		CraftResourceReservation? Reservation,
		CraftStationReservation? Station,
		IReadOnlyCollection<long> OutputItemIds);

	private sealed record CraftStationReservation(
		string Version,
		string Selector,
		long? CellId,
		long? ItemId,
		string Description,
		DateTimeOffset ReservedAt,
		DateTimeOffset ExpiresAt);
}
