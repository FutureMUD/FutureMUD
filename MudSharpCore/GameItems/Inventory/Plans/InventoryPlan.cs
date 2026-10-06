using MudSharp.NPC.AI;

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
        using var commandExecution = CommandExecutionScope.EnterBodyOperation(Character);
        for (int i = fromPhase; i <= toPhase; i++)
        {
            if (!CommandExecutionScope.TryContinue(Character)) return;
            if (_scoutedPhases.Contains(i))
            {
                continue;
            }

            IInventoryPlanPhaseTemplate template = Template.Phases.FirstOrDefault(x => x.PhaseNumber == i);
            if (template == null)
            {
                continue;
            }

            InventoryPlanPhase phase = new(template);
            foreach (IInventoryPlanAction action in template.Actions)
            {
                if (!CommandExecutionScope.TryContinue(Character)) return;
                IGameItem primary = action.ScoutTarget(Character);
                if (!CommandExecutionScope.TryContinue(Character)) return;
                var secondary = action.ScoutSecondary(Character, primary);
                if (!CommandExecutionScope.TryContinue(Character)) return;
                phase.ScoutedItems.Add((action, primary, secondary));
            }

            Phases[template.PhaseNumber] = phase;
            _scoutedPhases.Add(i);
        }
    }

    public IEnumerable<InventoryPlanActionResult> PeekPlanResults()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
        List<InventoryPlanActionResult> results = new();
        foreach (InventoryPlanPhase phase in Phases.Values.OrderBy(x => x.PhaseNumber))
        {
            results.AddRange(Template.PeekPlanResults(Character, phase, this));
        }

        return results;
    }

    public IEnumerable<InventoryPlanActionResult> ExecutePhase()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(Phase, Phase);
        if (!CommandExecutionScope.TryContinue(Character)) return [];
        return Template.ExecutePhase(Character, Phases[Phase++], this);
    }

    public IEnumerable<InventoryPlanActionResult> ExecuteWholePlan()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
        List<InventoryPlanActionResult> result = new();
        foreach (InventoryPlanPhase phase in Phases.Values.OrderBy(x => x.PhaseNumber))
        {
            if (!CommandExecutionScope.TryContinue(Character)) break;
            result.AddRange(Template.ExecutePhase(Character, phase, this));
        }

        return result;
    }

    public IEnumerable<InventoryPlanActionResult> ExecutePlan(int fromPhase)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(fromPhase, Template.Phases.Max(x => x.PhaseNumber));
        List<InventoryPlanActionResult> result = new();
        foreach (InventoryPlanPhase phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase).OrderBy(x => x.PhaseNumber))
        {
            if (!CommandExecutionScope.TryContinue(Character)) break;
            result.AddRange(Template.ExecutePhase(Character, phase, this));
        }

        return result;
    }

    public IEnumerable<InventoryPlanActionResult> ExecutePlan(int fromPhase, int toPhase)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(fromPhase, toPhase);
        List<InventoryPlanActionResult> result = new();
        foreach (InventoryPlanPhase phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase && x.PhaseNumber <= toPhase)
                                    .OrderBy(x => x.PhaseNumber))
        {
            if (!CommandExecutionScope.TryContinue(Character)) break;
            result.AddRange(Template.ExecutePhase(Character, phase, this));
        }

        return result;
    }

    public InventoryPlanFeasibility CurrentPhaseIsFeasible()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(Phase, Phase);
        if (!CommandExecutionScope.TryContinue(Character)) return InventoryPlanFeasibility.NotFeasibleMissingItems;
        return Template.PlanIsFeasible(Character, Phases[Phase]);
    }

    public InventoryPlanFeasibility PlanIsFeasible(int fromPhase)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(fromPhase, Template.Phases.Max(x => x.PhaseNumber));
        if (!CommandExecutionScope.TryContinue(Character)) return InventoryPlanFeasibility.NotFeasibleMissingItems;
        foreach (InventoryPlanPhase phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase).OrderBy(x => x.PhaseNumber))
        {
            InventoryPlanFeasibility result = Template.PlanIsFeasible(Character, phase);
            if (result != InventoryPlanFeasibility.Feasible)
            {
                return result;
            }
        }

        return InventoryPlanFeasibility.Feasible;
    }

    public InventoryPlanFeasibility PlanIsFeasible(int fromPhase, int toPhase)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(fromPhase, toPhase);
        if (!CommandExecutionScope.TryContinue(Character)) return InventoryPlanFeasibility.NotFeasibleMissingItems;
        foreach (InventoryPlanPhase phase in Phases.Values.Where(x => x.PhaseNumber >= fromPhase && x.PhaseNumber <= toPhase)
                                    .OrderBy(x => x.PhaseNumber))
        {
            InventoryPlanFeasibility result = Template.PlanIsFeasible(Character, phase);
            if (result != InventoryPlanFeasibility.Feasible)
            {
                return result;
            }
        }

        return InventoryPlanFeasibility.Feasible;
    }

    public InventoryPlanFeasibility PlanIsFeasible()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
        if (!CommandExecutionScope.TryContinue(Character)) return InventoryPlanFeasibility.NotFeasibleMissingItems;
        foreach (InventoryPlanPhase phase in Phases.Values.OrderBy(x => x.PhaseNumber))
        {
            InventoryPlanFeasibility result = Template.PlanIsFeasible(Character, phase);
            if (result != InventoryPlanFeasibility.Feasible)
            {
                return result;
            }
        }

        return InventoryPlanFeasibility.Feasible;
    }

    public IEnumerable<(IInventoryPlanAction, InventoryPlanFeasibility)> InfeasibleActions()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        List<(IInventoryPlanAction, InventoryPlanFeasibility)> actions = new();
        CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
        foreach (InventoryPlanPhase phase in Phases.Values.OrderBy(x => x.PhaseNumber))
        {
            actions.AddRange(Template.InfeasibleActions(Character, phase));
        }

        return actions;
    }

    public IEnumerable<InventoryPlanActionResult> ScoutAllTargets(int fromPhase)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(fromPhase, Template.Phases.Max(x => x.PhaseNumber));
        foreach (IInventoryPlanPhaseTemplate phase in Template.Phases)
        {
            if (phase.PhaseNumber < fromPhase)
            {
                continue;
            }

            foreach (IInventoryPlanAction action in phase.Actions)
            {
                IGameItem target = action.ScoutTarget(Character);
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
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(fromPhase, toPhase);
        foreach (IInventoryPlanPhaseTemplate phase in Template.Phases)
        {
            if (phase.PhaseNumber < fromPhase || phase.PhaseNumber > toPhase)
            {
                continue;
            }

            foreach (IInventoryPlanAction action in phase.Actions)
            {
                IGameItem target = action.ScoutTarget(Character);
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
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        CheckScouting(0, Template.Phases.Max(x => x.PhaseNumber));
        foreach (IInventoryPlanPhaseTemplate phase in Template.Phases)
        {
            foreach (IInventoryPlanAction action in phase.Actions)
            {
                IGameItem target = action.ScoutTarget(Character);
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

    public void FinalisePlan()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        if (!_finalised)
        {
            Template.FinalisePlan(Character, true, this, null);
            _finalised = true;
        }
    }

    public void FinalisePlanNoRestore()
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        if (!_finalised)
        {
            Template.FinalisePlan(Character, false, this, null);
            _finalised = true;
        }
    }

    public void FinalisePlanWithExemptions(IList<IGameItem> exemptItems)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        if (!_finalised)
        {
            Template.FinalisePlan(Character, true, this, exemptItems);
            _finalised = true;
        }
    }

    public int LastPhaseForItem(IGameItem item)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        return Phases.Where(x => x.Value.ScoutedItems.Any(y => y.Primary == item || y.Secondary == item))
                     .FirstMax(x => x.Key).Key;
    }

    public bool IsItemFinished(IGameItem item)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        return LastPhaseForItem(item) < Phase;
    }

    public void SetPhase(int phase)
    {
        using var commandExecution = MudSharp.NPC.AI.CommandExecutionScope.EnterBodyOperation(Character);
        Phase = phase;
    }

    public bool IsFinished => Phase > Phases.Count;
}
