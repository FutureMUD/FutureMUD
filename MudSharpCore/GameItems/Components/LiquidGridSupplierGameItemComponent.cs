#nullable enable
using MudSharp.Construction.Grids;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class LiquidGridSupplierGameItemComponent : GameItemComponent, ICanConnectToLiquidGrid, ILiquidGridSupplier
{
    protected LiquidGridSupplierGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (LiquidGridSupplierGameItemComponentProto)newProto;
    }

    private ILiquidGrid? _grid;

    public ILiquidGrid? LiquidGrid
    {
        get => _grid;
        set
        {
            if (_grid == value)
            {
                return;
            }

            _grid?.LeaveGrid(this);
            _grid = value;
            _grid?.JoinGrid(this);
            Changed = true;
        }
    }

    private ILiquidContainer? SourceContainer =>
        Parent.Components.Except(this).OfType<ILiquidContainer>().FirstOrDefault();

    public LiquidMixture? SuppliedMixture => SourceContainer?.LiquidMixture;
    public double AvailableLiquidVolume => SourceContainer?.LiquidVolume ?? 0.0;

    public LiquidMixture? RemoveLiquidAmount(double amount, MudSharp.Character.ICharacter? who, string action)
    {
        return SourceContainer?.RemoveLiquidAmount(amount, who, action);
    }

    public LiquidGridSupplierGameItemComponent(LiquidGridSupplierGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
    }

    public LiquidGridSupplierGameItemComponent(Models.GameItemComponent component,
        LiquidGridSupplierGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public LiquidGridSupplierGameItemComponent(LiquidGridSupplierGameItemComponent rhs, IGameItem newParent,
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
        return new LiquidGridSupplierGameItemComponent(this, newParent, temporary);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Grid", LiquidGrid?.Id ?? 0)
        ).ToString();
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

        ILiquidContainer? container = SourceContainer;
        return string.Join("\n",
            description,
            $"It is configured to feed liquid from {(container == null ? "no sibling liquid container".ColourError() : "a sibling liquid container".ColourName())} into a liquid grid.",
            $"Connected Grid: {(LiquidGrid == null ? "None".ColourError() : $"#{LiquidGrid.Id.ToString("N0", voyeur)}".ColourValue())}"
        );
    }

    string ICanConnectToGrid.GridType => "Liquid";

    IGrid? ICanConnectToGrid.Grid
    {
        get => LiquidGrid;
        set => LiquidGrid = value as ILiquidGrid;
    }
}
