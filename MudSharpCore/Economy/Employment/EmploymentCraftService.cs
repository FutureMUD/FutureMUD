using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Work.Crafts;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentCraftService
{
	private const string CraftPayloadPrefix = "craft-v1";

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

		var (success, error) = craft.CanDoCraft(actor, null, true, false);
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

		if (MatchingActiveCraft(actor, craft) is { } active)
		{
			operationalState = InProgressState(active.Component, PriorItemIds(context, actor.Location));
			reason = string.Empty;
			return true;
		}

		if (TryGetPriorCraftState(context, craft, out var state))
		{
			if (TryResolveActiveCraftItem(actor, state.ActiveItemId, craft, out var activeComponent))
			{
				var canResume = craft.CanResumeCraft(actor, activeComponent);
				if (!canResume.Success)
				{
					reason = canResume.Error;
					return false;
				}

				craft.ResumeCraft(actor, activeComponent);
				operationalState = InProgressState(activeComponent, state.PreExistingItemIds);
				reason = string.Empty;
				return true;
			}

			return TryCompleteCraft(context, actor, craft, state.PreExistingItemIds, out reason,
				out operationalState);
		}

		if (!CanStartCraft(context, craftSelector, actor, out reason))
		{
			return false;
		}

		var preExistingItemIds = PriorItemIds(context, actor.Location);
		craft.BeginCraft(actor);
		if (MatchingActiveCraft(actor, craft) is not { } started)
		{
			reason = $"The craft {craft.Name} did not create a native active craft effect.";
			return false;
		}

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Started craft {craft.Name}.", context.CurrentTask?.CorrelationId);
		operationalState = InProgressState(started.Component, preExistingItemIds);
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

		operationalState = new EmploymentActionStepOperationalState(
			OperationalPayload: $"Craft station validated: {stationSelector}",
			RouteResult: long.TryParse(stationSelector, out var cellId)
				? $"cell={cellId:F0}"
				: null);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Validated craft station {stationSelector}.", context.CurrentTask?.CorrelationId);
		reason = string.Empty;
		return true;
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

	private static EmploymentActionStepOperationalState InProgressState(IActiveCraftGameItemComponent component,
		IReadOnlyCollection<long> preExistingItemIds)
	{
		return new EmploymentActionStepOperationalState(
			OperationalPayload: "craft-status=inprogress",
			CraftJobReference: SerializeCraftState(component, preExistingItemIds));
	}

	private static bool TryCompleteCraft(EmploymentTaskContext context, ICharacter actor, ICraft craft,
		IReadOnlyCollection<long> preExistingItemIds, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		var location = actor.Location;
		var candidates = location is null
			? new List<IGameItem>()
			: context.AvailableItems(location)
			         .Where(x => !preExistingItemIds.Contains(x.Id))
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

		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			candidates.Any()
				? $"Completed craft {craft.Name} and adopted {candidates.Count:N0} output item(s) into task custody."
				: $"Completed craft {craft.Name}.",
			context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			OperationalPayload: "craft-status=complete",
			SelectedResources: candidates.Any()
				? EmploymentTaskContext.FormatTaskItemCustody("collect", actor.Id, candidates)
				: null);
		reason = string.Empty;
		return true;
	}

	private static IReadOnlyCollection<long> PriorItemIds(EmploymentTaskContext context, ICell? location)
	{
		return location is null
			? []
			: context.AvailableItems(location).Select(x => x.Id).Distinct().ToList();
	}

	private sealed record CraftState(long CraftId, int Revision, long ActiveItemId,
		IReadOnlyCollection<long> PreExistingItemIds);

	private static bool TryGetPriorCraftState(EmploymentTaskContext context, ICraft craft, out CraftState state)
	{
		state = null!;
		var stateRecord = context.CurrentTask?.StepOperationalStates.ElementAtOrDefault(context.CurrentStepIndex);
		var text = stateRecord?.CraftJobReference;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		var values = text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                 .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		                 .Where(x => x.Length == 2)
		                 .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
		if (!values.TryGetValue("prefix", out var prefix) ||
		    !prefix.EqualTo(CraftPayloadPrefix) ||
		    !values.TryGetValue("craft", out var craftText) ||
		    !long.TryParse(craftText, out var craftId) ||
		    craftId != craft.Id ||
		    !values.TryGetValue("revision", out var revisionText) ||
		    !int.TryParse(revisionText, out var revision) ||
		    revision != craft.RevisionNumber)
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
		state = new CraftState(craftId, revision, activeItemId, preExisting);
		return true;
	}

	private static string SerializeCraftState(IActiveCraftGameItemComponent component,
		IReadOnlyCollection<long> preExistingItemIds)
	{
		return string.Join(";",
			$"prefix={CraftPayloadPrefix}",
			$"craft={component.Craft.Id:F0}",
			$"revision={component.Craft.RevisionNumber:F0}",
			$"active={component.Parent.Id:F0}",
			$"pre={preExistingItemIds.Select(x => x.ToString("F0")).ListToCommaSeparatedValues()}");
	}
}
