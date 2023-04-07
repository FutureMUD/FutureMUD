using System.Xml.Linq;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class DragAidGameItemComponent : GameItemComponent, IDragAid
{
	protected DragAidGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (DragAidGameItemComponentProto)newProto;
	}

	#region Constructors

	public DragAidGameItemComponent(DragAidGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public DragAidGameItemComponent(MudSharp.Models.GameItemComponent component, DragAidGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public DragAidGameItemComponent(DragAidGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(
		rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new DragAidGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region IDragAid Implementation

	public double EffortMultiplier => _prototype.EffortMultiplier;

	public int MaximumUsers => _prototype.MaximumUsers;

	#endregion
}