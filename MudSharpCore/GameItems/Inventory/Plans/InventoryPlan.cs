using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlan : IInventoryPlan
{
	private bool _finalised;
	private readonly List<int> _scoutedPhases;
	public List<IInventoryPlanItemEffect> AssociatedEffects { get; } = new();

	public InventoryPlan(ICharacter executor, IInventoryPlanTemplate template)
	{
		Character = executor;
		Template = template;
		_scoutedPhases = new List<int>(template.Phases.Count());
		Phase = Template.Phases.Min(x => x.PhaseNumber);
	}

	~InventoryPlan()
	{
		FinalisePlanNoRestore();
	}

	public IInventoryPlanTemplate Template { get; set; }
	public ICharacter Character { get; set; }
	public int Phase { get; set; }
	public Dictionary<int, InventoryPlanPhase> Phases { get; } = new();

	private void CheckScouting(int fromPhase, int toPhase)
	{
		for (var i = fromPhase; i <= toPhase; i++)
		{
			if (_scoutedPhases.Contains(i))
			{
				continue;
			}

			_scoutedPhases.Add(i);
			var template = Template.Phases.FirstOrDefault(x => x.PhaseNumber == i);
			if (template == null)
			{
				continue;
			}

			var phase = new InventoryPlanPhase(template);
			foreach (var action in template.Actions)
			{
				var primary = action.ScoutTarget(Character);
				phase.ScoutedItems.Add((action, primary, action.ScoutSecondary(Character, primary)));
			}

			Phases[template.PhaseNumber] = phase;
		}
	}

	public IEnumerable<InventoryPlanActionResult> PeekPlanResults()
	{
		CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
		var results = new List<InventoryPlanActionResult>();
		foreach (var phase in Phases.Values.OrderBy(x => x.PhaseNumber))
		{
			results.AddRange(Template.PeekPlanResults(Character, phase, this));
		}

		return results;
	}

	public IEnumerable<InventoryPlanActionResult> ExecutePhase()
	{
		CheckScouting(Phase, Phase);
		return Template.ExecutePhase(Character, Phases[Phase++], this);
	}

	public IEnumerable<InventoryPlanActionResult> ExecuteWholePlan()
	{
		CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
		var result = new List<InventoryPlanActionResult>();
		foreach (var phase in Phases.Values.OrderBy(x => x.PhaseNumber))
		{
			result.AddRange(Template.ExecutePhase(Character, phase, this));
		}

		return result;
	}

	public IEnumerable<InventoryPlanActionResult> ExecutePlan(int fromPhase)
	{
		CheckScouting(fromPhase, Template.Phases.Max(x => x.PhaseNumber));
		var result = new List<InventoryPlanActionResult>();
		foreach (var phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase).OrderBy(x => x.PhaseNumber))
		{
			result.AddRange(Template.ExecutePhase(Character, phase, this));
		}

		return result;
	}

	public IEnumerable<InventoryPlanActionResult> ExecutePlan(int fromPhase, int toPhase)
	{
		CheckScouting(fromPhase, toPhase);
		var result = new List<InventoryPlanActionResult>();
		foreach (var phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase && x.PhaseNumber <= toPhase)
		                            .OrderBy(x => x.PhaseNumber))
		{
			result.AddRange(Template.ExecutePhase(Character, phase, this));
		}

		return result;
	}

	public InventoryPlanFeasibility CurrentPhaseIsFeasible()
	{
		CheckScouting(Phase, Phase);
		return Template.PlanIsFeasible(Character, Phases[Phase]);
	}

	public InventoryPlanFeasibility PlanIsFeasible(int fromPhase)
	{
		CheckScouting(fromPhase, Template.Phases.Max(x => x.PhaseNumber));
		foreach (var phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase).OrderBy(x => x.PhaseNumber))
		{
			var result = Template.PlanIsFeasible(Character, phase);
			if (result != InventoryPlanFeasibility.Feasible)
			{
				return result;
			}
		}

		return InventoryPlanFeasibility.Feasible;
	}

	public InventoryPlanFeasibility PlanIsFeasible(int fromPhase, int toPhase)
	{
		CheckScouting(fromPhase, toPhase);
		foreach (var phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase && x.PhaseNumber <= toPhase)
		                            .OrderBy(x => x.PhaseNumber))
		{
			var result = Template.PlanIsFeasible(Character, phase);
			if (result != InventoryPlanFeasibility.Feasible)
			{
				return result;
			}
		}

		return InventoryPlanFeasibility.Feasible;
	}

	public InventoryPlanFeasibility PlanIsFeasible()
	{
		CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
		foreach (var phase in Phases.Values.OrderBy(x => x.PhaseNumber))
		{
			var result = Template.PlanIsFeasible(Character, phase);
			if (result != InventoryPlanFeasibility.Feasible)
			{
				return result;
			}
		}

		return InventoryPlanFeasibility.Feasible;
	}

	public IEnumerable<(IInventoryPlanAction, InventoryPlanFeasibility)> InfeasibleActions()
	{
		var actions = new List<(IInventoryPlanAction, InventoryPlanFeasibility)>();
		CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
		foreach (var phase in Phases.Values.OrderBy(x => x.PhaseNumber))
		{
			actions.AddRange(Template.InfeasibleActions(Character, phase));
		}

		return actions;
	}

	public IEnumerable<InventoryPlanActionResult> ScoutAllTargets(int fromPhase)
	{
		CheckScouting(fromPhase, Template.Phases.Max(x => x.PhaseNumber));
		foreach (var phase in Template.Phases)
		{
			if (phase.PhaseNumber < fromPhase)
			{
				continue;
			}

			foreach (var action in phase.Actions)
			{
				var target = action.ScoutTarget(Character);
				yield return new InventoryPlanActionResult
				{
					PrimaryTarget = target,
					SecondaryTarget = action.ScoutSecondary(Character, target),
					ActionState = action.DesiredState,
					OriginalReference = action.OriginalReference
				};
			}
		}
	}

	public IEnumerable<InventoryPlanActionResult> ScoutAllTargets(int fromPhase, int toPhase)
	{
		CheckScouting(fromPhase, toPhase);
		foreach (var phase in Template.Phases)
		{
			if (phase.PhaseNumber < fromPhase || phase.PhaseNumber > toPhase)
			{
				continue;
			}

			foreach (var action in phase.Actions)
			{
				var target = action.ScoutTarget(Character);
				yield return new InventoryPlanActionResult
				{
					PrimaryTarget = target,
					SecondaryTarget = action.ScoutSecondary(Character, target),
					ActionState = action.DesiredState,
					OriginalReference = action.OriginalReference
				};
			}
		}
	}

	public IEnumerable<InventoryPlanActionResult> ScoutAllTargets()
	{
		CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
		foreach (var phase in Template.Phases)
		foreach (var action in phase.Actions)
		{
			var target = action.ScoutTarget(Character);
			yield return new InventoryPlanActionResult
			{
				PrimaryTarget = target,
				SecondaryTarget = action.ScoutSecondary(Character, target),
				ActionState = action.DesiredState,
				OriginalReference = action.OriginalReference
			};
		}
	}

	public void FinalisePlan()
	{
		if (!_finalised)
		{
			Template.FinalisePlan(Character, true, this, null);
			_finalised = true;
		}
	}

	public void FinalisePlanNoRestore()
	{
		if (!_finalised)
		{
			Template.FinalisePlan(Character, false, this, null);
			_finalised = true;
		}
	}

	public void FinalisePlanWithExemptions(IList<IGameItem> exemptItems)
	{
		if (!_finalised)
		{
			Template.FinalisePlan(Character, true, this, exemptItems);
			_finalised = true;
		}
	}

	public int LastPhaseForItem(IGameItem item)
	{
		return Phases.Where(x => x.Value.ScoutedItems.Any(y => y.Primary == item || y.Secondary == item))
		             .FirstMax(x => x.Key).Key;
	}

	public bool IsItemFinished(IGameItem item)
	{
		return LastPhaseForItem(item) < Phase;
	}

	public void SetPhase(int phase)
	{
		Phase = phase;
	}

	public bool IsFinished => Phase > Phases.Count;
}