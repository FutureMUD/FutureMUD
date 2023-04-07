using System;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Framework;

namespace MudSharp.GameItems.Inventory.Size;

/// <summary>
///     WearableSizeRules are mostly designed to be consumed by the IWearableSize interface in determining item fit
/// </summary>
public class WearableSizeRules : IWearableSizeRules
{
	public WearableSizeRules(WearableSizeParameterRule rule, IBodyPrototype body)
	{
		Body = body;
		MinHeightFactor = rule.MinHeightFactor;
		MaxHeightFactor = rule.MaxHeightFactor;
		MinWeightFactor = rule.MinWeightFactor;
		MaxWeightFactor = rule.MaxWeightFactor ?? 1.0;
		MinTraitFactor = rule.MinTraitFactor ?? 1.0;
		MaxTraitFactor = rule.MaxTraitFactor ?? 1.0;
		if (!rule.IgnoreTrait)
		{
			IgnoreAttribute = false;
			WhichTrait = body.Gameworld.Traits.Get(rule.TraitId ?? 0L);
		}
		else
		{
			IgnoreAttribute = true;
			WhichTrait = null;
		}

		var root = XElement.Parse(rule.WeightVolumeRatios);
		WeightVolumeRatios = new RankedRange<ItemVolumeFitDescription>();
		foreach (var sub in root.Elements("Ratio"))
		{
			WeightVolumeRatios.Add((ItemVolumeFitDescription)Convert.ToInt32(sub.Attribute("Item").Value),
				Convert.ToDouble(sub.Attribute("Min").Value), Convert.ToDouble(sub.Attribute("Max").Value));
		}

		root = XElement.Parse(rule.TraitVolumeRatios);
		TraitVolumeRatios = new RankedRange<ItemVolumeFitDescription>();
		foreach (var sub in root.Elements("Ratio"))
		{
			TraitVolumeRatios.Add((ItemVolumeFitDescription)Convert.ToInt32(sub.Attribute("Item").Value),
				Convert.ToDouble(sub.Attribute("Min").Value), Convert.ToDouble(sub.Attribute("Max").Value));
		}

		root = XElement.Parse(rule.HeightLinearRatios);
		HeightLinearRatios = new RankedRange<ItemLinearFitDescription>();
		foreach (var sub in root.Elements("Ratio"))
		{
			HeightLinearRatios.Add((ItemLinearFitDescription)Convert.ToInt32(sub.Attribute("Item").Value),
				Convert.ToDouble(sub.Attribute("Min").Value), Convert.ToDouble(sub.Attribute("Max").Value));
		}
	}

	/// <summary>
	///     The body for which these rules apply
	/// </summary>
	public IBodyPrototype Body { get; protected set; }

	public double MinHeightFactor { get; protected set; }

	public double MaxHeightFactor { get; protected set; }

	public double MinWeightFactor { get; protected set; }

	public double MaxWeightFactor { get; protected set; }

	public double MinTraitFactor { get; protected set; }

	public double MaxTraitFactor { get; protected set; }

	public ITraitDefinition WhichTrait { get; protected set; }

	public bool IgnoreAttribute { get; protected set; }

	public RankedRange<ItemVolumeFitDescription> WeightVolumeRatios { get; protected set; }

	public RankedRange<ItemVolumeFitDescription> TraitVolumeRatios { get; protected set; }

	public RankedRange<ItemLinearFitDescription> HeightLinearRatios { get; protected set; }
}