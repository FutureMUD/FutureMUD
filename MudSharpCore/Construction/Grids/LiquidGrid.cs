#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Construction.Grids;

public class LiquidGrid : GridBase, ILiquidGrid
{
	private readonly List<long> _connectedSupplierIds = [];
	private readonly List<ILiquidGridSupplier> _connectedSuppliers = [];

	public LiquidGrid(Models.Grid grid, IFuturemud gameworld) : base(grid, gameworld)
	{
		var root = XElement.Parse(grid.Definition);
		foreach (var element in root.Elements("Supplier"))
		{
			_connectedSupplierIds.Add(long.Parse(element.Value));
		}
	}

	public LiquidGrid(IFuturemud gameworld, ICell? initialLocation) : base(gameworld, initialLocation)
	{
	}

	public LiquidGrid(ILiquidGrid rhs) : base(rhs)
	{
		foreach (var supplier in rhs.Locations
			         .SelectMany(x => x.GameItems)
			         .Select(x => x.GetItemType<ILiquidGridSupplier>())
			         .Where(x => x != null)
			         .Cast<ILiquidGridSupplier>())
		{
			_connectedSuppliers.Add(supplier);
		}
	}

	public override string GridType => "Liquid";

	public override void LoadTimeInitialise()
	{
		base.LoadTimeInitialise();
		foreach (var id in _connectedSupplierIds)
		{
			var supplier = Locations.SelectMany(x => x.GameItems).FirstOrDefault(x => x.Id == id)
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
		var root = base.SaveDefinition();
		foreach (var supplier in _connectedSuppliers)
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
			var mixture = LiquidMixture.CreateEmpty(Gameworld);
			foreach (var supplierMixture in _connectedSuppliers.Select(x => x.SuppliedMixture).Where(x => x?.IsEmpty == false))
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

		var suppliers = _connectedSuppliers
		                .Select(x => (Supplier: x, Volume: x.AvailableLiquidVolume))
		                .Where(x => x.Volume > 0.0)
		                .ToList();
		if (!suppliers.Any())
		{
			return null;
		}

		var targetAmount = Math.Min(amount, suppliers.Sum(x => x.Volume));
		var removed = LiquidMixture.CreateEmpty(Gameworld);
		var remainingToRemove = targetAmount;
		var totalVolume = suppliers.Sum(x => x.Volume);

		foreach (var (supplier, volume) in suppliers)
		{
			var proportionalAmount = targetAmount * volume / totalVolume;
			var amountToRemove = Math.Min(volume, proportionalAmount);
			if (amountToRemove <= 0.0)
			{
				continue;
			}

			var mixture = supplier.RemoveLiquidAmount(amountToRemove, who, action);
			if (mixture?.IsEmpty == false)
			{
				removed.AddLiquid(mixture);
				remainingToRemove -= mixture.TotalVolume;
			}
		}

		if (remainingToRemove > 0.0)
		{
			foreach (var (supplier, _) in suppliers.Where(x => x.Supplier.AvailableLiquidVolume > 0.0))
			{
				if (remainingToRemove <= 0.0)
				{
					break;
				}

				var mixture = supplier.RemoveLiquidAmount(remainingToRemove, who, action);
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
		var mixture = CurrentLiquidMixture;
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
