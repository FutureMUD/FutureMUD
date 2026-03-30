#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction.Grids;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class GridLiquidSourceGameItemComponent : GameItemComponent, ILiquidContainer, ICanConnectToLiquidGrid
{
	protected GridLiquidSourceGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (GridLiquidSourceGameItemComponentProto)newProto;
	}

	private ILiquidGrid? _grid;

	public ILiquidGrid? LiquidGrid
	{
		get => _grid;
		set
		{
			_grid = value;
			Changed = true;
		}
	}

	public LiquidMixture? LiquidMixture
	{
		get => LiquidGrid?.CurrentLiquidMixture;
		set { }
	}

	public double LiquidCapacity => 0.0;
	public bool CanBeEmptiedWhenInRoom => true;
	public double LiquidVolume => LiquidGrid?.TotalLiquidVolume ?? 0.0;

	public void AddLiquidQuantity(double amount, ICharacter who, string action)
	{
	}

	public void ReduceLiquidQuantity(double amount, ICharacter who, string action)
	{
	}

	public void MergeLiquid(LiquidMixture otherMixture, ICharacter who, string action)
	{
		throw new InvalidOperationException("You cannot add liquid directly to a grid liquid source.");
	}

	public LiquidMixture? RemoveLiquidAmount(double amount, ICharacter who, string action)
	{
		return LiquidGrid?.RemoveLiquidAmount(amount, who, action);
	}

	public GridLiquidSourceGameItemComponent(GridLiquidSourceGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public GridLiquidSourceGameItemComponent(Models.GameItemComponent component,
		GridLiquidSourceGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public GridLiquidSourceGameItemComponent(GridLiquidSourceGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	private void LoadFromXml(XElement root)
	{
		LiquidGrid = Gameworld.Grids.Get(long.Parse(root.Element("Grid")?.Value ?? "0")) as ILiquidGrid;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new GridLiquidSourceGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Grid", LiquidGrid?.Id ?? 0)
		).ToString();
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full || type == DescriptionType.Contents || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var mixture = LiquidMixture;
		switch (type)
		{
			case DescriptionType.Contents:
				if (mixture?.IsEmpty == false)
				{
					return $"It is currently supplying {mixture.ColouredLiquidLongDescription}.";
				}

				return $"{description}\n\nIt is currently dry.";
			case DescriptionType.Evaluate:
				return string.Join("\n",
					description,
					$"It is connected to {(LiquidGrid == null ? "no liquid grid".ColourError() : $"liquid grid #{LiquidGrid.Id.ToString("N0", voyeur)}".ColourValue())}.",
					$"The grid currently has {LiquidVolume.ToString("N2", voyeur).ColourValue()} volume units available."
				);
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.AppendLine(description);
				sb.AppendLine(
					$"It is connected to {(LiquidGrid == null ? "no liquid grid".ColourError() : $"liquid grid #{LiquidGrid.Id.ToString("N0", voyeur)}".ColourValue())}.");
				if (mixture?.IsEmpty == false)
				{
					sb.AppendLine($"It is currently supplying {mixture.ColouredLiquidLongDescription}.");
				}

				return sb.ToString();
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	public bool IsOpen => true;

	public bool CanOpen(IBody opener)
	{
		return false;
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		return WhyCannotOpenReason.NotOpenable;
	}

	public void Open()
	{
	}

	public bool CanClose(IBody closer)
	{
		return false;
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		return WhyCannotCloseReason.NotOpenable;
	}

	public void Close()
	{
	}

	public event OpenableEvent? OnOpen;
	public event OpenableEvent? OnClose;

	string ICanConnectToGrid.GridType => "Liquid";

	IGrid? ICanConnectToGrid.Grid
	{
		get => LiquidGrid;
		set => LiquidGrid = value as ILiquidGrid;
	}
}
