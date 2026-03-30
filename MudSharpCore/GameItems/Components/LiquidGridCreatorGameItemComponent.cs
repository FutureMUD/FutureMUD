#nullable enable
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction.Grids;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class LiquidGridCreatorGameItemComponent : GameItemComponent
{
	protected LiquidGridCreatorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LiquidGridCreatorGameItemComponentProto)newProto;
	}

	public ILiquidGrid Grid { get; protected set; }

	public LiquidGridCreatorGameItemComponent(LiquidGridCreatorGameItemComponentProto proto, IGameItem parent,
		ICharacter? loader, bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		Grid = new LiquidGrid(Gameworld, loader?.Location);
		Gameworld.Add(Grid);
	}

	public LiquidGridCreatorGameItemComponent(Models.GameItemComponent component,
		LiquidGridCreatorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public LiquidGridCreatorGameItemComponent(LiquidGridCreatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		Grid = temporary ? rhs.Grid : new LiquidGrid(rhs.Grid);
		if (!temporary)
		{
			Gameworld.Add(Grid);
		}
	}

	private void LoadFromXml(XElement root)
	{
		Grid = (ILiquidGrid)Gameworld.Grids.Get(long.Parse(root.Element("Grid")!.Value));
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LiquidGridCreatorGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", Grid.Id)
		).ToString();
	}

	public override void Delete()
	{
		base.Delete();
		Grid.Delete();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.ItemFinishedLoading && !Grid.Locations.Any() && Parent.TrueLocations.Any())
		{
			Grid.ExtendTo(Parent.TrueLocations.First());
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
		if (type != DescriptionType.Full)
		{
			return base.Decorate(voyeur, name, description, type, colour, flags);
		}

		var mixture = Grid.CurrentLiquidMixture;
		return string.Join("\n",
			description,
			"",
			$"The grid is currently storing {Grid.TotalLiquidVolume.ToString("N2", voyeur).ColourValue()} volume units of {(mixture.IsEmpty ? "nothing".ColourError() : mixture.ColouredLiquidLongDescription)}."
		);
	}
}
