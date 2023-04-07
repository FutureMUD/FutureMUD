using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;

namespace MudSharp.GameItems.Components;

public class ArmourGameItemComponent : GameItemComponent, IArmour
{
	protected ArmourGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ArmourGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ArmourGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#region IArmour Implementation

	public IArmourType ArmourType => _prototype.ArmourType;

	#endregion

	#region Implementation of IAbsorbDamage

	public IDamage SufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		var characterOwner = Parent.GetItemType<WearableGameItemComponent>()?.WornBy?.Actor;
		return characterOwner == null
			? damage
			: _prototype.ArmourType.AbsorbDamage(damage, this, characterOwner, ref wounds, false);
	}

	public IDamage PassiveSufferDamage(IDamage damage, ref List<IWound> wounds)
	{
		var characterOwner = Parent.GetItemType<WearableGameItemComponent>()?.WornBy?.Actor;
		return characterOwner == null
			? damage
			: _prototype.ArmourType.AbsorbDamage(damage, this, characterOwner, ref wounds, true);
	}

	public void ProcessPassiveWound(IWound wound)
	{
		Parent.ProcessPassiveWound(wound);
	}

	#endregion

	#region Constructors

	public ArmourGameItemComponent(ArmourGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ArmourGameItemComponent(MudSharp.Models.GameItemComponent component, ArmourGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
	}

	public ArmourGameItemComponent(ArmourGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	#endregion
}