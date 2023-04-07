using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class BeltableGameItemComponent : GameItemComponent, IBeltable
{
	protected BeltableGameItemComponentProto _prototype;

	public BeltableGameItemComponent(BeltableGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BeltableGameItemComponent(MudSharp.Models.GameItemComponent component, BeltableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public BeltableGameItemComponent(BeltableGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#region IBeltable Members

	public IBelt ConnectedTo { get; set; }

	#endregion

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BeltableGameItemComponent(this, newParent, temporary);
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		if (ConnectedTo == null)
		{
			return false;
		}

		var connectedItem = ConnectedTo;
		connectedItem.RemoveConnectedItem(this);

		var newItemBeltable = newItem?.GetItemType<IBeltable>();
		if (newItemBeltable != null)
		{
			connectedItem.AddConnectedItem(newItemBeltable);
			return true;
		}

		return false;
	}

	public override bool AffectsLocationOnDestruction => true;

	public override int ComponentDieOrder => 1;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BeltableGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	#region Overrides of GameItemComponent

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return base.Decorate(voyeur, name, description, type, colour, flags);
		}

		return $"{description}\n\nThis item can be attached to belts (or similar).";
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	#endregion
}