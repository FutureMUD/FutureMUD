using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImplantOrganGameItemComponent : ImplantBaseGameItemComponent, IOrganImplant, IImplantReportStatus
{
	protected ImplantOrganGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ImplantOrganGameItemComponentProto)newProto;
		base.UpdateComponentNewPrototype(newProto);
	}

	#region Constructors

	public ImplantOrganGameItemComponent(ImplantOrganGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ImplantOrganGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImplantOrganGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImplantOrganGameItemComponent(ImplantOrganGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		if (!temporary)
		{
			Gameworld.SaveManager.AddInitialisation(this);
		}
		else
		{
			_noSave = true;
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImplantOrganGameItemComponent(this, newParent, temporary);
	}

	#endregion

	public IOrganProto TargetOrgan => _prototype.TargetOrgan;

	public override void OnPowerCutOut()
	{
		base.OnPowerCutOut();
		InstalledBody?.CheckHealthStatus();
	}

	#region IImplantReportStatus

	public string ReportStatus()
	{
		if (!_powered)
		{
			return "\t* Implant is unpowered and non-functional.";
		}

		return
			$"\t* Implant is powered and functioning at {FunctionFactor.ToString("P2", InstalledBody.Actor)} capacity.";
	}

	#endregion
}