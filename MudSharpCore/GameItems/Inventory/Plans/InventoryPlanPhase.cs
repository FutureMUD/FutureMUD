using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanPhase : IInventoryPlanPhase
{
	public int PhaseNumber => Template.PhaseNumber;
	public IInventoryPlanPhaseTemplate Template { get; set; }
	public List<(IInventoryPlanAction Action, IGameItem Primary, IGameItem Secondary)> ScoutedItems { get; } = new();

	public InventoryPlanPhase(IInventoryPlanPhaseTemplate template)
	{
		Template = template;
	}
}