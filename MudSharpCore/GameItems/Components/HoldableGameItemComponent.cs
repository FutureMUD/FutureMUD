using System.Linq;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class HoldableGameItemComponent : GameItemComponent, IHoldable
{
	protected HoldableGameItemComponentProto _prototype;

	public HoldableGameItemComponent(HoldableGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public HoldableGameItemComponent(MudSharp.Models.GameItemComponent component, HoldableGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	public HoldableGameItemComponent(HoldableGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override void Delete()
	{
		base.Delete();
		HeldBy?.Take(Parent);
	}

	public override bool AffectsLocationOnDestruction => true;

	public override int ComponentDieOrder => 100;

	public override bool Die(IGameItem newItem, ICell location)
	{
		if (HeldBy == null)
		{
			return false;
		}

		var newItemHoldable = newItem?.GetItemType<HoldableGameItemComponent>();
		if (newItemHoldable != null)
		{
			newItem.Get(HeldBy);
			HeldBy.SwapInPlace(Parent, newItem);
		}

		else if (newItem != null)
		{
			location.Insert(newItem);
		}


		HeldBy.Take(Parent);
		HeldBy = null;
		CurrentInventoryDescription = "";
		return false;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new HoldableGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (HoldableGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	#region IHoldable Members

	public string CurrentInventoryDescription { get; set; }

	public IBody HeldBy { get; set; }

	#endregion
}