#nullable enable
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Construction.Grids;

public class LiquidGrid : GridBase, ILiquidGrid
{
    private readonly List<long> _connectedSupplierIds = [];
    private readonly List<ILiquidGridSupplier> _connectedSuppliers = [];

    public LiquidGrid(Models.Grid grid, IFuturemud gameworld) : base(grid, gameworld)
    {
        XElement root = XElement.Parse(grid.Definition);
        foreach (XElement element in root.Elements("Supplier"))
        {
            _connectedSupplierIds.Add(long.Parse(element.Value));
        }
    }

    public LiquidGrid(IFuturemud gameworld, ICell? initialLocation) : base(gameworld, initialLocation)
    {
    }

    public LiquidGrid(ILiquidGrid rhs) : base(rhs)
    {
    }

    public override string GridType => "Liquid";

    public override void LoadTimeInitialise()
    {
        base.LoadTimeInitialise();
        foreach (long id in _connectedSupplierIds)
        {
            ILiquidGridSupplier? supplier = Locations.SelectMany(x => x.GameItems).FirstOrDefault(x => x.Id == id)
                            ?.GetItemType<ILiquidGridSupplier>();
            if (supplier == null)
            {
                continue;
            }

            _connectedSuppliers.Add(supplier);
        }

        _connectedSupplierIds.Clear();
    }

    protected override XElement SaveDefinition()
    {
        XElement root = base.SaveDefinition();
        foreach (ILiquidGridSupplier supplier in _connectedSuppliers)
        {
            root.Add(new XElement("Supplier", supplier.Parent.Id));
        }

        return root;
    }

    public double TotalLiquidVolume => _connectedSuppliers.Sum(x => x.AvailableLiquidVolume);

    public LiquidMixture CurrentLiquidMixture
    {
        get
        {
            LiquidMixture mixture = LiquidMixture.CreateEmpty(Gameworld);
            foreach (LiquidMixture? supplierMixture in _connectedSuppliers.Select(x => x.SuppliedMixture).Where(x => x?.IsEmpty == false))
            {
                mixture.AddLiquid(supplierMixture!.Clone());
            }

            return mixture;
        }
    }

    public void JoinGrid(ILiquidGridSupplier supplier)
    {
        if (_connectedSuppliers.Contains(supplier))
        {
            return;
        }

        _connectedSuppliers.Add(supplier);
        Changed = true;
    }

    public void LeaveGrid(ILiquidGridSupplier supplier)
    {
        if (_connectedSuppliers.Remove(supplier))
        {
            Changed = true;
        }
    }

    public LiquidMixture? RemoveLiquidAmount(double amount, ICharacter? who, string action)
    {
        if (amount <= 0.0 || TotalLiquidVolume <= 0.0)
        {
            return null;
        }

        List<(ILiquidGridSupplier Supplier, double Volume)> suppliers = _connectedSuppliers
                        .Select(x => (Supplier: x, Volume: x.AvailableLiquidVolume))
                        .Where(x => x.Volume > 0.0)
                        .ToList();
        if (!suppliers.Any())
        {
            return null;
        }

        double targetAmount = Math.Min(amount, suppliers.Sum(x => x.Volume));
        LiquidMixture removed = LiquidMixture.CreateEmpty(Gameworld);
        double remainingToRemove = targetAmount;
        double totalVolume = suppliers.Sum(x => x.Volume);

        foreach ((ILiquidGridSupplier? supplier, double volume) in suppliers)
        {
            double proportionalAmount = targetAmount * volume / totalVolume;
            double amountToRemove = Math.Min(volume, proportionalAmount);
            if (amountToRemove <= 0.0)
            {
                continue;
            }

            LiquidMixture? mixture = supplier.RemoveLiquidAmount(amountToRemove, who, action);
            if (mixture?.IsEmpty == false)
            {
                removed.AddLiquid(mixture);
                remainingToRemove -= mixture.TotalVolume;
            }
        }

        if (remainingToRemove > 0.0)
        {
            foreach ((ILiquidGridSupplier? supplier, double _) in suppliers.Where(x => x.Supplier.AvailableLiquidVolume > 0.0))
            {
                if (remainingToRemove <= 0.0)
                {
                    break;
                }

                LiquidMixture? mixture = supplier.RemoveLiquidAmount(remainingToRemove, who, action);
                if (mixture?.IsEmpty == false)
                {
                    removed.AddLiquid(mixture);
                    remainingToRemove -= mixture.TotalVolume;
                }
            }
        }

        return removed.IsEmpty ? null : removed;
    }

    public override string Show(ICharacter actor)
    {
        LiquidMixture mixture = CurrentLiquidMixture;
        return string.Join("\n",
            $"Grid #{Id.ToString("N0", actor)}",
            $"Type: {"Liquid".Colour(Telnet.BoldCyan)}",
            $"Locations: {Locations.Count().ToString("N0", actor).ColourValue()}",
            $"Suppliers: {_connectedSuppliers.Count.ToString("N0", actor).ColourValue()}",
            $"Current Volume: {TotalLiquidVolume.ToString("N2", actor).ColourValue()}",
            $"Mixture: {(mixture.IsEmpty ? "Empty".ColourError() : mixture.ColouredLiquidLongDescription)}"
        );
    }
}
