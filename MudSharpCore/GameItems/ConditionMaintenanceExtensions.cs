using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems;

public static class ConditionMaintenanceExtensions
{
	public static void UseCondition(this IGameItemComponent component, ItemConditionUseKind useKind,
		Outcome outcome = Outcome.NotTested, double degree = 0.0, double damage = 0.0,
		double absorbed = 0.0, double passed = 0.0)
	{
		if (component is not IConditionDegradingComponent degrading)
		{
			return;
		}

		degrading.UseCondition(new ItemConditionUseContext(useKind, outcome, degree, damage, absorbed, passed));
	}
}
