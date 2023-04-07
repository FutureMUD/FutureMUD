using System;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class StackableGameItemComponent : GameItemComponent, IStackable
{
	private StackableGameItemComponentProto _prototype;

	public StackableGameItemComponent(StackableGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public StackableGameItemComponent(MudSharp.Models.GameItemComponent component,
		StackableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public StackableGameItemComponent(StackableGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_quantity = rhs._quantity;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new StackableGameItemComponent(this, newParent, temporary);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type == DescriptionType.Short
			? _prototype.DescriptionDecorator.Describe(name, description, Quantity)
			: description;
	}

	public override int DecorationPriority => 0;

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short;
	}

	public override double ComponentWeightMultiplier => Math.Max(1.0, Quantity);

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemStackable = newItem?.GetItemType<StackableGameItemComponent>();
		if (newItemStackable == null)
		{
			return false;
		}

		newItemStackable.Quantity = Quantity;
		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (StackableGameItemComponentProto)newProto;
	}

	protected void LoadFromXml(XElement root)
	{
		var attribute = root.Attribute("Quantity");
		if (attribute != null)
		{
			_quantity = Convert.ToInt32(attribute.Value);
		}
	}

	protected override string SaveToXml()
	{
		return "<Definition Quantity=\"" + Quantity + "\"/>";
	}

	#region IStackable Members

	private int _quantity = 1;

	public int Quantity
	{
		get => _quantity;
		set
		{
			_quantity = value;
			Changed = true;
			HandleDescriptionUpdate();
		}
	}

	public ItemGetResponse CanGet(int quantity)
	{
		return DropsWhole(quantity) ? ItemGetResponse.CanGet : ItemGetResponse.CanGetStack;
	}

	public IGameItem Get(int quantity)
	{
		return DropsWhole(quantity) ? Parent : Split(quantity);
	}

	public bool DropsWhole(int quantity)
	{
		return quantity == 0 || quantity >= Quantity;
	}

	public IGameItem Split(int quantity)
	{
		var newItem = new GameItem((GameItem)Parent);
		((StackableGameItemComponent)newItem.GetItemType<IStackable>()).Quantity = quantity;
		Quantity -= quantity;
		return newItem;
	}

	public IGameItem PeekSplit(int quantity)
	{
		var newItem = new GameItem((GameItem)Parent, true);
		((StackableGameItemComponent)newItem.GetItemType<IStackable>()).Quantity = Math.Min(quantity, Quantity);
		return newItem;
	}

	#endregion
}