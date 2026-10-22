#nullable enable
using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Body.PartProtos;

namespace MudSharp.Health.Strategies;

public partial class ComplexLivingHealthStrategy
{
	public override DirectHealthCostChannels SupportedDirectHealthCostChannels =>
		DirectHealthCostChannels.Damage | DirectHealthCostChannels.Pain | DirectHealthCostChannels.Stun;

	public override bool TryPlanDirectHealthCost(IHaveWounds owner, IBodypart bodypart, double damage, double pain,
		double stun, WoundSeverity maximumSeverity, out DirectHealthCostPlan plan, out string error)
	{
		// The ordinary bone path may split damage into a random fracture. A direct price
		// must select a stable external cellular target before any bodily mutation.
		if (bodypart is IBone)
		{
			plan = null!;
			error = "A bone cannot be an exact direct cellular health-cost target.";
			return false;
		}
		return OrganicDirectHealthCosts.TryPlan(this, owner, bodypart, damage, pain, stun,
			maximumSeverity, out plan, out error);
	}

	public override IReadOnlyList<IWound> ApplyDirectHealthCost(IHaveWounds owner, DirectHealthCostPlan plan) =>
		OrganicDirectHealthCosts.Apply(this, owner, plan);
	public override bool TryPlanDirectHealthConsequence(IHaveWounds owner, IBodypart bodypart, double damage, double pain,
		double stun, out DirectHealthCostPlan plan, out string error)
	{
		if (bodypart is IBone) { plan = null!; error = "A bone is not an exact cellular consequence target."; return false; }
		return OrganicDirectHealthCosts.TryPlan(this, owner, bodypart, damage, pain, stun, WoundSeverity.Horrifying,
			out plan, out error, allowIncapacitation: true);
	}
}
