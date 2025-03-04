using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class BeltGameItemComponent : GameItemComponent, IBelt
{
	protected BeltGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override void Delete()
	{
		base.Delete();
		foreach (var item in ConnectedItems.ToList())
		{
			_connectedItems.Remove(item);
			item.ConnectedTo = null;
			item.Parent.Delete();
		}
	}

	public override void Quit()
	{
		foreach (var item in ConnectedItems)
		{
			item.Parent.Quit();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BeltGameItemComponent(this, newParent, temporary);
	}

	public override double ComponentWeight
	{
		get { return ConnectedItems.Sum(x => x.Parent.Weight); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return ConnectedItems.Sum(x => x.Parent.Buoyancy(fluidDensity));
	}

	public override bool Take(IGameItem item)
	{
		if (ConnectedItems.Any(x => x.Parent == item))
		{
			var beltable = item.GetItemType<IBeltable>();
			_connectedItems.Remove(beltable);
			beltable.ConnectedTo = null;
			Changed = true;
			return true;
		}

		return false;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		if (!ConnectedItems.Any())
		{
			return false;
		}

		var newItemBelt = newItem?.GetItemType<IBelt>();
		if (newItemBelt == null)
		{
			foreach (var item in ConnectedItems.ToList())
			{
				if (location != null)
				{
					item.ConnectedTo = null;
					location.Insert(item.Parent);
				}
				else
				{
					item.Delete();
				}
			}
		}
		else
		{
			foreach (var item in ConnectedItems.ToList())
			{
				if (newItemBelt.CanAttachBeltable(item) == IBeltCanAttachBeltableResult.Success)
				{
					newItemBelt.AddConnectedItem(item);
				}
				else if (location != null)
				{
					item.ConnectedTo = null;
					location.Insert(item.Parent);
				}
				else
				{
					item.Delete();
				}
			}
		}

		_connectedItems.Clear();
		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BeltGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new object[]
			{
				from item in ConnectedItems
				select new XElement("Connected", item.Parent.Id)
			}
		).ToString();
	}

	#region IBelt Members

	public SizeCategory MaximumSize => _prototype.MaximumSize;

	public int MaximumNumberOfBeltedItems => _prototype.MaximumNumberOfBeltedItems;

	private readonly List<IBeltable> _connectedItems = new();
	public IEnumerable<IBeltable> ConnectedItems => _connectedItems;

	public void AddConnectedItem(IBeltable item)
	{
		item.ConnectedTo?.RemoveConnectedItem(item);
		_connectedItems.Add(item);
		item.ConnectedTo = this;
		Changed = true;
	}

	public void RemoveConnectedItem(IBeltable item)
	{
		_connectedItems.Remove(item);
		item.ConnectedTo = null;
		Changed = true;
	}

	public IBeltCanAttachBeltableResult CanAttachBeltable(IBeltable beltable)
	{
		if (_connectedItems.Count >= MaximumNumberOfBeltedItems)
		{
			return IBeltCanAttachBeltableResult.FailureExceedMaximumNumber;
		}

		return beltable.Parent.Size > MaximumSize
			? IBeltCanAttachBeltableResult.FailureTooLarge
			: IBeltCanAttachBeltableResult.Success;
	}

	#endregion

	#region Constructors

	public BeltGameItemComponent(BeltGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BeltGameItemComponent(MudSharp.Models.GameItemComponent component, BeltGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		foreach (var element in root.Elements("Connected"))
		{
			var item = Gameworld.TryGetItem(long.Parse(element.Value), true);
			if (item?.IsItemType<IBeltable>() != true)
			{
				continue;
			}

			_connectedItems.Add(item.GetItemType<IBeltable>());
			item.GetItemType<IBeltable>().ConnectedTo = this;
		}
	}

	public BeltGameItemComponent(BeltGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override void FinaliseLoad()
	{
		foreach (var item in _connectedItems)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	#endregion
}