using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Grids;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ElectricGridCreatorGameItemComponent : GameItemComponent
{
	protected ElectricGridCreatorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ElectricGridCreatorGameItemComponentProto)newProto;
	}

	public IElectricalGrid Grid { get; protected set; }

	#region Constructors

	public ElectricGridCreatorGameItemComponent(ElectricGridCreatorGameItemComponentProto proto, IGameItem parent,
		ICharacter loader, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		Grid = new ElectricalGrid(Gameworld, loader?.Location);
		Gameworld.Add(Grid);
	}

	public ElectricGridCreatorGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectricGridCreatorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ElectricGridCreatorGameItemComponent(ElectricGridCreatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		if (!temporary)
		{
			Grid = new ElectricalGrid(rhs.Grid);
			Gameworld.Add(Grid);
		}
		else
		{
			Grid = rhs.Grid;
		}
	}

	protected void LoadFromXml(XElement root)
	{
		Grid = (IElectricalGrid)Gameworld.Grids.Get(long.Parse(root.Element("Grid").Value));
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectricGridCreatorGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", Grid.Id)
		).ToString();
	}

	#endregion

	public override void Delete()
	{
		base.Delete();
		Grid?.Delete();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.ItemFinishedLoading)
		{
			if (!Grid.Locations.Any() && Parent.TrueLocations.Any())
			{
				Grid.ExtendTo(Parent.TrueLocations.First());
			}
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.ItemFinishedLoading) || base.HandlesEvent(types);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Full)
		{
			var sb = new StringBuilder();
			sb.AppendLine(description);
			sb.AppendLine();
			sb.AppendLine(
				$"The grid is currently supplying a total of {Grid.TotalSupply.ToString("N2", voyeur).ColourValue()} watts and drawing down {Grid.TotalDrawdown.ToString("N2", voyeur).ColourValue()} watts.");
			return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}
}