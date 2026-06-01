using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Work.Crafts;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentCraftService
{
	public static bool CanStartCraft(EmploymentTaskContext context, string craftSelector, ICharacter actor,
		out string reason)
	{
		if (!TryResolveCraft(actor, craftSelector, out var craft, out reason))
		{
			return false;
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
		if (!CanStartCraft(context, craftSelector, actor, out reason) ||
		    !TryResolveCraft(actor, craftSelector, out var craft, out reason))
		{
			return false;
		}

		craft.BeginCraft(actor);
		context.RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Started craft {craft.Name}.", context.CurrentTask?.CorrelationId);
		operationalState = new EmploymentActionStepOperationalState(
			CraftJobReference: $"craft={craft.Id};revision={craft.RevisionNumber}");
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
}
