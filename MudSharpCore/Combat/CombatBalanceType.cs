using MudSharp.Body.Traits;
using MudSharp.Framework;

namespace MudSharp.Combat;

public class CombatBalanceType : FrameworkItem, IFrameworkItem
{
	public string RecoveryMessage { get; set; }
	public string OffBalanceMessage { get; set; }
	private TraitExpression BalanceMultiplierTraitExpression { get; set; }
	public override string FrameworkItemType => "CombatBalanceType";

	public double GetMultiplier(IHaveTraits checkee)
	{
		return BalanceMultiplierTraitExpression?.Evaluate(checkee) ?? 1.0;
	}
}