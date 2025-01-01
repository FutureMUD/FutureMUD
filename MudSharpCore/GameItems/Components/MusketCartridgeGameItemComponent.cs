using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;
public class MusketCartridgeGameItemComponent : AmmunitionGameItemComponent
{
	protected MusketCartridgeGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (MusketCartridgeGameItemComponentProto)newProto;
	}

	#region Constructors
	public MusketCartridgeGameItemComponent(MusketCartridgeGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public MusketCartridgeGameItemComponent(Models.GameItemComponent component, MusketCartridgeGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MusketCartridgeGameItemComponent(MusketCartridgeGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MusketCartridgeGameItemComponent(this, newParent, temporary);
	}
	#endregion

	#region Saving
	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}
	#endregion

	public IAmmunitionType AmmoType => _prototype.AmmoType;
	public double BulletBore => _prototype.BulletBore;
	public IGameItemProto BulletProto => _prototype.BulletProto;
}
