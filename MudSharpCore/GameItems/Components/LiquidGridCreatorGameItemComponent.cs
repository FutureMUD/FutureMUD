#nullable enable
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
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
		Grid = CreateGrid(loader?.Location, temporary);
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
		Grid = temporary ? rhs.Grid : CreateGridCopy(rhs.Grid);
	}

	private void LoadFromXml(XElement root)
	{
		var gridId = long.Parse(root.Element("Grid")!.Value);
		Grid = Gameworld.Grids.Get(gridId) as ILiquidGrid ?? CreateGrid(Parent.TrueLocations.FirstOrDefault());
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
		Grid?.Delete();
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

	private ILiquidGrid CreateGrid(ICell? initialLocation, bool temporary = false)
	{
		var grid = new LiquidGrid(Gameworld, initialLocation);
		if (temporary)
		{
			return grid;
		}

		Gameworld.Add(grid);
		Gameworld.SaveManager.DirectInitialise((ILateInitialisingItem)grid);
		return grid;
	}

	private ILiquidGrid CreateGridCopy(ILiquidGrid source)
	{
		var grid = new LiquidGrid(source);
		Gameworld.Add(grid);
		Gameworld.SaveManager.DirectInitialise((ILateInitialisingItem)grid);
		return grid;
	}
}
