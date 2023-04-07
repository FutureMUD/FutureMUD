using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.GameItems.Inventory.Plans;
using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Body;

namespace MudSharp.GameItems.Inventory {
    public enum InventoryPlanFeasibility {
        Feasible,
        NotFeasibleNotEnoughHands,
        NotFeasibleNotEnoughWielders,
        NotFeasibleMissingItems
    }

    [Flags]
    public enum InventoryPlanOptions
    {
        None = 0,
        DoNotClearHands = 1,
        DoNotRestoreItems = 2,
    }

    public interface IInventoryPlanTemplate : IXmlSavable, IHaveFuturemud {
        IEnumerable<IInventoryPlanPhaseTemplate> Phases { get; }
        IInventoryPlanPhaseTemplate FirstPhase { get; }
        IEnumerable<InventoryPlanActionResult> PeekPlanResults(ICharacter executor, IInventoryPlanPhase phase, IInventoryPlan plan);
        InventoryPlanFeasibility PlanIsFeasible(ICharacter executor, IInventoryPlanPhase phase);
        IEnumerable<InventoryPlanActionResult> ExecutePhase(ICharacter executor, IInventoryPlanPhase phase, IInventoryPlan plan);
        IInventoryPlan CreatePlan(ICharacter executor);
        void FinalisePlan(ICharacter executor, bool restore, IInventoryPlan plan, IList<IGameItem> exemptItems);
        InventoryPlanOptions Options { get; set; }
        IEnumerable<(IInventoryPlanAction Action, InventoryPlanFeasibility Reason)> InfeasibleActions(ICharacter actor, IInventoryPlanPhase phase);
    }

    public abstract class InventoryPlanXmlDefinitionFactory
    {
	    public static XElement CreateInventoryDefinition((string Tag, InventoryState State, int Quantity)[] actions)
	    {
		    return new XElement("Plan",
			    new XElement("Phase",
				    from action in actions
				    select action.State switch
				    {
					    InventoryState.Held => new XElement("Action", new XAttribute("state", "held")),
					    InventoryState.Wielded => new XElement("Action", new XAttribute("state", "wielded")),
					    InventoryState.Worn => new XElement("Action", new XAttribute("state", "worn")),
					    InventoryState.Dropped => new XElement("Action", new XAttribute("state", "dropped")),
					    InventoryState.Sheathed => new XElement("Action", new XAttribute("state", "sheathed")),
					    InventoryState.InContainer => new XElement("Action", new XAttribute("state", "incontainer")),
					    InventoryState.Attached => new XElement("Action", new XAttribute("state", "held")),
						InventoryState.Prosthetic => new XElement("Action", new XAttribute("state", "prosthetic")),
					    InventoryState.Implanted => new XElement("Action", new XAttribute("state", "implanted")),
					    InventoryState.Consumed => new XElement("Action", new XAttribute("state", "consume")),
					    InventoryState.ConsumedLiquid => new XElement("Action", new XAttribute("state", "consumeliquid")),

						_ => throw new NotImplementedException()
				    }
				)
		    );

	    }

	}
}