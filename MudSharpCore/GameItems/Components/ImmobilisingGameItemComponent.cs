using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ImmobilisingGameItemComponent : WearableGameItemComponent, IImmobilise
{
	#region Constructors

	public ImmobilisingGameItemComponent(ImmobilisingGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, null, temporary)
	{
	}

	public ImmobilisingGameItemComponent(MudSharp.Models.GameItemComponent component,
		ImmobilisingGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ImmobilisingGameItemComponent(ImmobilisingGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ImmobilisingGameItemComponent(this, newParent, temporary);
	}

	#endregion
}