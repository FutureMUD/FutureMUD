using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ClothingInsulationGameItemComponent : GameItemComponent, IInsulating
{
	protected ClothingInsulationGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ClothingInsulationGameItemComponentProto)newProto;
	}

	#region Constructors

	public ClothingInsulationGameItemComponent(ClothingInsulationGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ClothingInsulationGameItemComponent(MudSharp.Models.GameItemComponent component,
		ClothingInsulationGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ClothingInsulationGameItemComponent(ClothingInsulationGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ClothingInsulationGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return "</Definition>";
	}

	#endregion

	public double InsulatingDegrees => _prototype.InsulatingDegrees;
	public double ReflectingDegrees => _prototype.ReflectingDegrees;
}