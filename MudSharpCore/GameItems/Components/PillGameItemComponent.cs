using MudSharp.Body;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;

namespace MudSharp.GameItems.Components;

public class PillGameItemComponent : GameItemComponent, ISwallowable
{
	protected PillGameItemComponentProto _prototype;

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PillGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PillGameItemComponentProto)newProto;
	}

	#region Constructors

	public PillGameItemComponent(PillGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public PillGameItemComponent(PillGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public PillGameItemComponent(MudSharp.Models.GameItemComponent component, PillGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	public void Swallow(IBody body)
	{
		body.Dose(_prototype.Drug, DrugVector.Ingested, _prototype.GramsPerPill);
		_prototype.OnSwallowProg?.Execute(body.Actor, Parent);
		Parent.Delete();
	}

	#endregion
}