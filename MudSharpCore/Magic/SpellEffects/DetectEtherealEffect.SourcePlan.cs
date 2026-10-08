#nullable enable

using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Magic.SpellEffects;

public partial class DetectEtherealEffect
{
	private sealed class SourceConsumptionObservation
	{
		public HashSet<int> AttemptedPhases { get; } = [];
		public HashSet<int> CompletedPhases { get; } = [];
		public bool Consumed { get; set; }
		public bool Finished { get; set; }
	}

	/// <summary>Invokes the native plan unchanged, then proves the source action's mutation.
	/// This wrapper never performs a debit, item mutation, restoration or rollback.</summary>
	private sealed class SourceEtherealPlanTemplate(DetectEtherealEffect effect, SourceEtherealSelection token,
		InventoryPlanTemplate native, SourceConsumptionObservation observed) : IInventoryPlanTemplate
	{
		public IFuturemud Gameworld => native.Gameworld;
		public IEnumerable<IInventoryPlanPhaseTemplate> Phases => native.Phases;
		public IInventoryPlanPhaseTemplate FirstPhase => native.FirstPhase;
		public InventoryPlanOptions Options { get => native.Options; set => native.Options = value; }
		public XElement SaveToXml() => native.SaveToXml();
		public IInventoryPlan CreatePlan(ICharacter executor) => new InventoryPlan(executor, this);
		public IEnumerable<InventoryPlanActionResult> PeekPlanResults(ICharacter executor, IInventoryPlanPhase phase, IInventoryPlan plan) =>
			native.PeekPlanResults(executor, phase, plan);
		public InventoryPlanFeasibility PlanIsFeasible(ICharacter executor, IInventoryPlanPhase phase) => native.PlanIsFeasible(executor, phase);
		public IEnumerable<(IInventoryPlanAction Action, InventoryPlanFeasibility Reason)> InfeasibleActions(ICharacter actor, IInventoryPlanPhase phase) =>
			native.InfeasibleActions(actor, phase);
		public void FinalisePlan(ICharacter executor, bool restore, IInventoryPlan plan, IList<IGameItem> exemptItems) =>
			native.FinalisePlan(executor, restore, plan, exemptItems);

		public IEnumerable<InventoryPlanActionResult> ExecutePhase(ICharacter executor, IInventoryPlanPhase phase, IInventoryPlan plan)
		{
			if (!ReferenceEquals(executor, token.Caster) || !observed.AttemptedPhases.Add(phase.PhaseNumber))
				throw new InvalidOperationException("Source plan execution cannot change caster or replay an attempted phase.");
			effect.ConfirmSource(token, executor, executor, !observed.Consumed);
			var dedicated = phase.ScoutedItems.Where(x => Equals(x.Action.OriginalReference, token.Scope.ComponentReference)).ToArray();
			if (token.Exempt && dedicated.Length != 0 || dedicated.Length > 1 ||
				dedicated.Length == 1 && (observed.Consumed || token.Component is null ||
					!ReferenceEquals(dedicated[0].Primary, token.Component.Item) ||
					phase.ScoutedItems.Any(x => !Equals(x.Action.OriginalReference, token.Scope.ComponentReference) &&
						(ReferenceEquals(x.Primary, token.Component.Item) || ReferenceEquals(x.Secondary, token.Component.Item)))))
				throw new InvalidOperationException("Source phase has ambiguous component ownership or duplicate consumption.");
			var results = native.ExecutePhase(executor, phase, plan).ToArray();
			effect.ConfirmSource(token, executor, executor, false);
			if (dedicated.Length == 1)
			{
				var sourceResults = results.Where(x => Equals(x.OriginalReference, token.Scope.ComponentReference)).ToArray();
				if (sourceResults.Length != 1 || sourceResults[0].ActionState != DesiredItemState.Consumed ||
					!ReferenceEquals(sourceResults[0].PrimaryTarget, token.Component!.Item))
					throw new InvalidOperationException("Native source consumption returned an ambiguous result.");
				// A Consumed result alone is not evidence: refills, moved/replaced items and
				// failed whole-item deletion remain uncertain and cannot grant perception.
				effect.ConfirmObservedConsumption(token); observed.Consumed = true;
			}
			if (observed.Consumed) effect.ConfirmObservedConsumption(token);
			observed.CompletedPhases.Add(phase.PhaseNumber);
			observed.Finished = observed.CompletedPhases.SetEquals(Phases.Select(x => x.PhaseNumber)) && (token.Exempt || observed.Consumed);
			return results;
		}
	}
}
