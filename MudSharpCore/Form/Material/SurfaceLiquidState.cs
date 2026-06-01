using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Form.Material;

public sealed class SurfaceResidue : ISurfaceResidue
{
	public SurfaceResidue(ISolid material, ILiquid? originalLiquid, double weight)
	{
		Material = material;
		OriginalLiquid = originalLiquid;
		Weight = weight;
	}

	public ISolid Material { get; }
	public ILiquid? OriginalLiquid { get; }
	public double Weight { get; private set; }

	public void AddWeight(double weight)
	{
		if (weight <= 0.0 || double.IsNaN(weight))
		{
			return;
		}

		Weight += weight;
	}

	public void RemoveWeight(double weight)
	{
		if (weight <= 0.0 || double.IsNaN(weight))
		{
			return;
		}

		Weight = Math.Max(0.0, Weight - weight);
	}

	public XElement SaveToXml()
	{
		return new XElement("Residue",
			new XAttribute("material", Material.Id),
			new XAttribute("liquid", OriginalLiquid?.Id ?? 0),
			new XAttribute("weight", Weight.ToString("R", CultureInfo.InvariantCulture))
		);
	}
}

public sealed class SurfaceLiquidState : ISurfaceLiquidState
{
	private readonly IFuturemud _gameworld;
	private readonly Action? _changed;
	private readonly List<SurfaceResidue> _residues = new();

	public SurfaceLiquidState(IFuturemud gameworld, Action? changed = null)
	{
		_gameworld = gameworld;
		_changed = changed;
		ContaminatingLiquid = LiquidMixture.CreateEmpty(gameworld);
		LastResolvedUtc = DateTime.UtcNow;
	}

	public SurfaceLiquidState(IFuturemud gameworld, XElement? root, Action? changed = null) : this(gameworld, changed)
	{
		if (root is null)
		{
			return;
		}

		LastResolvedUtc = DateTime.TryParse(root.Element("LastResolvedUtc")?.Value, CultureInfo.InvariantCulture,
			DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result)
			? result
			: DateTime.UtcNow;

		if (root.Element("Mix") is { } mixRoot)
		{
			ContaminatingLiquid = new LiquidMixture(mixRoot, gameworld);
		}

		foreach (var element in root.Element("Residues")?.Elements("Residue") ?? Enumerable.Empty<XElement>())
		{
			var material = gameworld.Materials.Get(long.Parse(element.Attribute("material")?.Value ?? "0"));
			if (material is null)
			{
				continue;
			}

			var liquid = gameworld.Liquids.Get(long.Parse(element.Attribute("liquid")?.Value ?? "0"));
			var weight = double.Parse(element.Attribute("weight")?.Value ?? "0", CultureInfo.InvariantCulture);
			if (weight <= 0.0 || double.IsNaN(weight))
			{
				continue;
			}

			_residues.Add(new SurfaceResidue(material, liquid, weight));
		}
	}

	public LiquidMixture ContaminatingLiquid { get; private set; }
	public IEnumerable<ISurfaceResidue> Residues => _residues;
	public double LiquidVolume => ContaminatingLiquid.TotalVolume;
	public double ResidueWeight => _residues.Sum(x => x.Weight);
	public double AddedWeight => ContaminatingLiquid.TotalWeight + ResidueWeight;
	public bool IsEmpty => ContaminatingLiquid.IsEmpty && ResidueWeight <= 0.0;
	public bool IsWet => !ContaminatingLiquid.IsEmpty;
	public DateTime LastResolvedUtc { get; set; }

	public ILiquid? LiquidRequired
	{
		get
		{
			var liquidSolvent = ContaminatingLiquid.Instances
				.Where(x => x.Liquid.Solvent is not null)
				.FirstMax(x => x.Amount)
				?.Liquid.Solvent;
			if (liquidSolvent is not null)
			{
				return liquidSolvent;
			}

			var residue = _residues
				.Where(x => (x.Material.Solvent ?? x.OriginalLiquid?.Solvent) is not null)
				.FirstMax(x => x.Weight);
			return residue?.Material.Solvent ?? residue?.OriginalLiquid?.Solvent;
		}
	}

	public double LiquidAmountConsumed
	{
		get
		{
			var liquidAmount = ContaminatingLiquid.Instances
				.Where(x => x.Liquid.Solvent is not null)
				.Sum(x => x.Amount * Math.Max(x.Liquid.SolventVolumeRatio, double.Epsilon));
			var residueAmount = _residues
				.Where(x => (x.Material.Solvent ?? x.OriginalLiquid?.Solvent) is not null)
				.Sum(x => x.Weight * Math.Max(x.Material.SolventRatio, double.Epsilon));
			return liquidAmount + residueAmount;
		}
	}

	public void AddLiquid(LiquidMixture liquid)
	{
		if (liquid.IsEmpty)
		{
			return;
		}

		ContaminatingLiquid.AddLiquid(liquid.Clone());
		LastResolvedUtc = DateTime.UtcNow;
		_changed?.Invoke();
	}

	public LiquidMixture? RemoveLiquidVolume(double volume)
	{
		if (volume <= 0.0 || ContaminatingLiquid.IsEmpty)
		{
			return null;
		}

		var removed = ContaminatingLiquid.RemoveLiquidVolume(Math.Min(volume, ContaminatingLiquid.TotalVolume));
		LastResolvedUtc = DateTime.UtcNow;
		_changed?.Invoke();
		return removed;
	}

	public bool CleanWithLiquid(LiquidMixture? liquid, double amount)
	{
		if (liquid is null || liquid.IsEmpty || amount <= 0.0)
		{
			return false;
		}

		var availableSolvent = Math.Min(amount, liquid.TotalVolume);
		var cleanedAny = false;
		foreach (var instance in ContaminatingLiquid.Instances.ToList())
		{
			if (availableSolvent <= 0.0 || instance.Liquid.Solvent is null)
			{
				continue;
			}

			if (!liquid.CountsAs(instance.Liquid.Solvent).Truth)
			{
				continue;
			}

			var solventRatio = Math.Max(instance.Liquid.SolventVolumeRatio, double.Epsilon);
			var removable = Math.Min(instance.Amount, availableSolvent / solventRatio);
			if (removable <= 0.0)
			{
				continue;
			}

			ContaminatingLiquid.RemoveLiquidVolume(instance, removable);
			cleanedAny = true;
			availableSolvent -= removable * solventRatio;
		}

		foreach (var residue in _residues.ToList())
		{
			if (availableSolvent <= 0.0)
			{
				break;
			}

			var required = residue.Material.Solvent ?? residue.OriginalLiquid?.Solvent;
			if (required is null || !liquid.CountsAs(required).Truth)
			{
				continue;
			}

			var solventRatio = Math.Max(residue.Material.SolventRatio, double.Epsilon);
			var removable = Math.Min(residue.Weight, availableSolvent / solventRatio);
			residue.RemoveWeight(removable);
			cleanedAny = true;
			availableSolvent -= removable * solventRatio;
		}

		if (!cleanedAny)
		{
			return false;
		}

		_residues.RemoveAll(x => x.Weight <= 0.0 || double.IsNaN(x.Weight));
		LastResolvedUtc = DateTime.UtcNow;
		_changed?.Invoke();
		return IsEmpty;
	}

	public void Dry(double amount, bool roomSurface = false)
	{
		if (!DryInternal(amount, roomSurface))
		{
			return;
		}

		LastResolvedUtc = DateTime.UtcNow;
		_changed?.Invoke();
	}

	public bool ResolveDrying(TimeSpan interval, double minimumDryVolume, double dryFraction, bool roomSurface = false,
		int maxTicks = 24)
	{
		if (ContaminatingLiquid.IsEmpty || interval <= TimeSpan.Zero || maxTicks <= 0)
		{
			return false;
		}

		var now = DateTime.UtcNow;
		var elapsed = now - LastResolvedUtc;
		if (elapsed <= TimeSpan.Zero)
		{
			return false;
		}

		var ticks = Math.Min(maxTicks, (int)Math.Floor(elapsed.TotalSeconds / interval.TotalSeconds));
		if (ticks <= 0)
		{
			return false;
		}

		var changed = false;
		for (var i = 0; i < ticks; i++)
		{
			if (ContaminatingLiquid.IsEmpty)
			{
				break;
			}

			var amount = Math.Max(minimumDryVolume, ContaminatingLiquid.TotalVolume * Math.Max(dryFraction, 0.0));
			changed |= DryInternal(amount, roomSurface);
		}

		if (!changed)
		{
			return false;
		}

		LastResolvedUtc = ContaminatingLiquid.IsEmpty
			? now
			: LastResolvedUtc + TimeSpan.FromTicks(interval.Ticks * ticks);
		_changed?.Invoke();
		return true;
	}

	public ItemSaturationLevel SaturationLevel(double coating, double absorb)
	{
		return SaturationLevelForLiquid(ContaminatingLiquid.TotalVolume, coating, absorb);
	}

	public ItemSaturationLevel SaturationLevelForLiquid(double total, double coating, double absorb)
	{
		if (total <= 0.0)
		{
			return ItemSaturationLevel.Dry;
		}

		var innerCapacity = Math.Max(absorb, double.Epsilon);
		if (total >= innerCapacity)
		{
			return total > absorb + coating ? ItemSaturationLevel.Saturated : ItemSaturationLevel.Soaked;
		}

		return total >= innerCapacity * 0.5 ? ItemSaturationLevel.Wet : ItemSaturationLevel.Damp;
	}

	public string GetAddendumText(double coating, double absorb, bool colour)
	{
		var texts = new List<string>();
		foreach (var liquid in GroupLiquidsForShortDescription())
		{
			switch (SaturationLevelForLiquid(liquid.Volume, coating, absorb))
			{
				case ItemSaturationLevel.Damp:
					texts.Add(colour ? liquid.Liquid.DampShortDescription.Colour(liquid.Liquid.DisplayColour) : liquid.Liquid.DampShortDescription);
					break;
				case ItemSaturationLevel.Wet:
					texts.Add(colour ? liquid.Liquid.WetShortDescription.Colour(liquid.Liquid.DisplayColour) : liquid.Liquid.WetShortDescription);
					break;
				case ItemSaturationLevel.Soaked:
				case ItemSaturationLevel.Saturated:
					texts.Add(colour ? liquid.Liquid.DrenchedShortDescription.Colour(liquid.Liquid.DisplayColour) : liquid.Liquid.DrenchedShortDescription);
					break;
			}
		}

		texts.AddRange(_residues.Select(x => colour && x.Material.ResidueColour is not null
			? x.Material.ResidueSdesc.Colour(x.Material.ResidueColour)
			: x.Material.ResidueSdesc));
		return texts.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ListToString(conjunction: "", separator: " ");
	}

	public string GetAdditionalText(double coating, double absorb, IPerceiver voyeur, bool colour)
	{
		var texts = new List<string>();
		foreach (var liquid in GroupLiquidsForLongDescription())
		{
			switch (SaturationLevelForLiquid(liquid.Volume, coating, absorb))
			{
				case ItemSaturationLevel.Damp:
					texts.Add(colour ? liquid.Liquid.DampDescription.Colour(liquid.Liquid.DisplayColour) : liquid.Liquid.DampDescription);
					break;
				case ItemSaturationLevel.Wet:
					texts.Add(colour ? liquid.Liquid.WetDescription.Colour(liquid.Liquid.DisplayColour) : liquid.Liquid.WetDescription);
					break;
				case ItemSaturationLevel.Soaked:
				case ItemSaturationLevel.Saturated:
					texts.Add(colour ? liquid.Liquid.DrenchedDescription.Colour(liquid.Liquid.DisplayColour) : liquid.Liquid.DrenchedDescription);
					break;
			}
		}

		texts.AddRange(_residues.Select(x =>
		{
			var text = string.IsNullOrWhiteSpace(x.Material.ResidueDesc)
				? $"It is stained with {x.Material.MaterialDescription.ToLowerInvariant()}."
				: string.Format(x.Material.ResidueDesc, "some");
			return colour && x.Material.ResidueColour is not null ? text.Colour(x.Material.ResidueColour) : text;
		}));

		return texts.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ListToString(separator: "\n\t");
	}

	public XElement SaveToXml()
	{
		return new XElement("Surface",
			new XElement("LastResolvedUtc", LastResolvedUtc.ToString("O", CultureInfo.InvariantCulture)),
			ContaminatingLiquid.SaveToXml(),
			new XElement("Residues", _residues.Select(x => x.SaveToXml()))
		);
	}

	private bool DryInternal(double amount, bool roomSurface)
	{
		if (amount <= 0.0 || ContaminatingLiquid.IsEmpty)
		{
			return false;
		}

		var dried = Math.Min(amount, ContaminatingLiquid.TotalVolume);
		var total = ContaminatingLiquid.TotalVolume;
		var ratios = ContaminatingLiquid.Instances
			.Select(x => (Instance: x, Ratio: x.Amount / total))
			.ToList();

		ContaminatingLiquid.RemoveLiquidVolume(dried);
		foreach (var (instance, ratio) in ratios)
		{
			if (instance.Liquid.DriedResidue is null)
			{
				continue;
			}

			if (roomSurface && !instance.Liquid.LeaveResiduesInRooms)
			{
				continue;
			}

			AddResidue(instance.Liquid.DriedResidue, instance.Liquid, dried * ratio * instance.Liquid.ResidueVolumePercentage);
		}

		return true;
	}

	private void AddResidue(ISolid material, ILiquid originalLiquid, double weight)
	{
		if (weight <= 0.0 || double.IsNaN(weight))
		{
			return;
		}

		var existing = _residues.FirstOrDefault(x => x.Material == material && x.OriginalLiquid == originalLiquid);
		if (existing is not null)
		{
			existing.AddWeight(weight);
			return;
		}

		_residues.Add(new SurfaceResidue(material, originalLiquid, weight));
	}

	private IEnumerable<(ILiquid Liquid, double Volume)> GroupLiquidsForShortDescription()
	{
		var liquids = new Dictionary<ILiquid, double>();
		foreach (var instance in ContaminatingLiquid.Instances)
		{
			var existing = liquids.Keys.FirstOrDefault(x =>
				x.DampShortDescription == instance.Liquid.DampShortDescription &&
				x.WetShortDescription == instance.Liquid.WetShortDescription &&
				x.DrenchedShortDescription == instance.Liquid.DrenchedShortDescription);
			if (existing is not null)
			{
				liquids[existing] += instance.Amount;
			}
			else
			{
				liquids[instance.Liquid] = instance.Amount;
			}
		}

		return liquids.Select(x => (x.Key, x.Value));
	}

	private IEnumerable<(ILiquid Liquid, double Volume)> GroupLiquidsForLongDescription()
	{
		var liquids = new Dictionary<ILiquid, double>();
		foreach (var instance in ContaminatingLiquid.Instances)
		{
			var existing = liquids.Keys.FirstOrDefault(x =>
				x.DampDescription == instance.Liquid.DampDescription &&
				x.WetDescription == instance.Liquid.WetDescription &&
				x.DrenchedDescription == instance.Liquid.DrenchedDescription);
			if (existing is not null)
			{
				liquids[existing] += instance.Amount;
			}
			else
			{
				liquids[instance.Liquid] = instance.Amount;
			}
		}

		return liquids.Select(x => (x.Key, x.Value));
	}
}
