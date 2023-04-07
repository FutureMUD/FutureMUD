using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Plans;

public class InventoryPlanPhaseTemplate : IInventoryPlanPhaseTemplate
{
	private readonly List<IInventoryPlanAction> _actions = new();

	public InventoryPlanPhaseTemplate(XElement root, IFuturemud gameworld, int phaseNumber)
	{
		PhaseNumber = phaseNumber;
		foreach (var item in root.Elements("Action"))
		{
			_actions.Add(InventoryPlanAction.LoadAction(item, gameworld));
		}
	}

	public InventoryPlanPhaseTemplate(int phaseNumber, IEnumerable<IInventoryPlanAction> actions)
	{
		PhaseNumber = phaseNumber;
		_actions.AddRange(actions);
	}

	public int PhaseNumber { get; set; }
	public IEnumerable<IInventoryPlanAction> Actions => _actions;

	#region Implementation of IXmlSavable

	public XElement SaveToXml()
	{
		return new XElement("Phase",
			from action in _actions
			select action.SaveToXml()
		);
	}

	#endregion

	public void AddAction(IInventoryPlanAction action)
	{
		_actions.Add(action);
	}

	public void RemoveAction(IInventoryPlanAction action)
	{
		_actions.Remove(action);
	}
}