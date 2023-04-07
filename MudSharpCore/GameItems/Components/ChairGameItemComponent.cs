using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Position;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class ChairGameItemComponent : GameItemComponent, IChair
{
	private ChairGameItemComponentProto _prototype;

	public ChairGameItemComponent(ChairGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ChairGameItemComponent(MudSharp.Models.GameItemComponent component, ChairGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ChairGameItemComponent(ChairGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ChairGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ChairGameItemComponentProto)newProto;
	}

	protected void LoadFromXml(XElement root)
	{
		// Nothing to do
	}

	protected override string SaveToXml()
	{
		return "<Chair/>";
	}

	#region IChair Members

	public int ChairSlotsUsed => _prototype.ChairSlotsUsed;

	public int OccupantCapacity => _prototype.ChairOccupantCapacity;

	public ITable Table { get; protected set; }

	public IEnumerable<IPerceivable> Occupants
	{
		get { return Parent.TargetedBy.Where(x => x.PositionModifier == PositionModifier.On); }
	}

	public void SetTable(ITable table, bool nosave = false)
	{
		if (table != null)
		{
			table.AddChair(null, this);
			Parent.Get(null);
			if (nosave)
			{
				Parent.LoadTimeSetContainedIn(table.Parent);
			}
			else
			{
				Parent.ContainedIn = table.Parent;
			}
		}
		else
		{
			Table?.RemoveChair(null, this);
			Parent.ContainedIn = null;
		}

		Table = table;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemChair = newItem?.GetItemType<ChairGameItemComponent>();
		if (newItemChair == null)
		{
			SetTable(null);
			return false;
		}

		var table = Table;
		SetTable(null);
		newItemChair.SetTable(table);
		return true;
	}

	public override bool AffectsLocationOnDestruction => true;

	public override int ComponentDieOrder => 1;

	public override bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier)
	{
		return modifier != PositionModifier.On || Occupants.Count() < OccupantCapacity;
	}

	#endregion
}