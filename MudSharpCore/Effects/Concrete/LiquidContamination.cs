using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class LiquidContamination : Effect, ILiquidContaminationEffect, IDescriptionAdditionEffect, IEffectAddsWeight,
	ISDescAdditionEffect
{
	private static TimeSpan BaseEffectDuration =>
		TimeSpan.FromSeconds(Futuremud.Games.First().GetStaticDouble("LiquidContaminationEffectDuration"));

	public static TimeSpan EffectDuration(LiquidMixture mixture)
	{
		return BaseEffectDuration * mixture.RelativeEnthalpy;
	}

	public static bool ShouldDripsEcho => Futuremud.Games.First().GetStaticBool("ShouldLiquidOverflowEchoToRoom");

	public LiquidContamination(IPerceivable owner, LiquidMixture liquid, IFutureProg applicabilityProg = null) : base(
		owner, applicabilityProg)
	{
		ContaminatingLiquid = liquid;
	}

	protected LiquidContamination(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		LoadFromDb(effect.Element("Effect"));
	}

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return true;
	}

	#region Implementation of ILiquidContaminationEffect

	private LiquidMixture _contaminatingLiquid;

	public LiquidMixture ContaminatingLiquid
	{
		get => _contaminatingLiquid;
		set
		{
			if (_contaminatingLiquid != null)
			{
				_contaminatingLiquid.OnLiquidMixtureChanged -= ContaminatingLiquidOnLiquidMixtureChanged;
			}

			_contaminatingLiquid = value;
			if (_contaminatingLiquid != null)
			{
				_contaminatingLiquid.OnLiquidMixtureChanged += ContaminatingLiquidOnLiquidMixtureChanged;
			}
		}
	}

	private DateTime _lastDripEcho = DateTime.MinValue;

	private void ContaminatingLiquidOnLiquidMixtureChanged(LiquidMixture mixture)
	{
		if (mixture.IsEmpty)
		{
			Owner.RemoveEffect(this, true);
			return;
		}

		var ownerItem = (IGameItem)Owner;
		var (coat, absorb) = ownerItem.LiquidAbsorbtionAmounts;
		var saturation = ownerItem.SaturationLevel;
		double maxAbsorbed = 0.0;
		switch (saturation)
		{
			case ItemSaturationLevel.Dry:
				maxAbsorbed = (coat + absorb) * 0.98;
				break;
			case ItemSaturationLevel.Damp:
				maxAbsorbed = (coat + absorb) * 0.8;
				break;
			case ItemSaturationLevel.Wet:
				maxAbsorbed = (coat + absorb) * 0.5;
				break;
			case ItemSaturationLevel.Soaked:
				maxAbsorbed = (coat + absorb) * 0.35;
				break;
			case ItemSaturationLevel.Saturated:
				maxAbsorbed = (coat + absorb) * 0.01;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if (mixture.TotalVolume > maxAbsorbed)
		{
			_contaminatingLiquid.OnLiquidMixtureChanged -= ContaminatingLiquidOnLiquidMixtureChanged;
			var difference = mixture.TotalVolume - maxAbsorbed;
			ContaminatingLiquid.SetLiquidVolume(maxAbsorbed);
			var location = ownerItem.TrueLocations.FirstOrDefault();
			if (location != null && ownerItem.ContainedIn == null &&
			    !ownerItem.Location.IsSwimmingLayer(ownerItem.RoomLayer))
			{
				if (DateTime.UtcNow - _lastDripEcho > TimeSpan.FromSeconds(120) && ShouldDripsEcho)
				{
					_lastDripEcho = DateTime.UtcNow;
					string amount, verb;
					if (difference <= 0.002 / Gameworld.UnitManager.BaseFluidToLitres)
					{
						amount = "a few drops of";
						verb = "drip off of";
					}
					else if (difference <= 0.005 / Gameworld.UnitManager.BaseFluidToLitres)
					{
						amount = "a tiny amount of";
						verb = "drips off of";
					}
					else if (difference <= 0.01 / Gameworld.UnitManager.BaseFluidToLitres)
					{
						amount = "a small amount of";
						verb = "drips off of";
					}
					else if (difference <= 0.05 / Gameworld.UnitManager.BaseFluidToLitres)
					{
						amount = "some of";
						verb = "drips off of";
					}
					else if (difference <= 0.15 / Gameworld.UnitManager.BaseFluidToLitres)
					{
						amount = "a large amount of";
						verb = "leaks through";
					}
					else
					{
						amount = "an enormous amount of";
						verb = "gushes out of";
					}

					ownerItem.OutputHandler.Handle(new EmoteOutput(
						new Emote(
							$"{amount} {ContaminatingLiquid.ColouredLiquidDescription} {verb} $0 and onto the ground.",
							ownerItem, ownerItem), style: OutputStyle.NoNewLine,
						flags: OutputFlags.NoticeCheckRequired | OutputFlags.Insigificant));
				}
				
				var locationPerceivable = (IPerceiver)ownerItem.LocationLevelPerceivable;
				var excess = new LiquidMixture(mixture);
				excess.SetLiquidVolume(difference);
				PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(excess, location, locationPerceivable?.RoomLayer ?? RoomLayer.GroundLevel, locationPerceivable);
			}

			_contaminatingLiquid.OnLiquidMixtureChanged += ContaminatingLiquidOnLiquidMixtureChanged;
		}

		Changed = true;
	}

	public void AddLiquid(LiquidMixture liquid)
	{
		foreach (var instance in ContaminatingLiquid.Instances.Where(x => x.Liquid.Solvent != null).ToList())
		{
			var totalCleanableAmount =
				Math.Min(liquid.Instances.Where(x => x.Liquid.LiquidCountsAs(x.Liquid.Solvent)).Sum(x => x.Amount),
					instance.Amount * instance.Liquid.SolventVolumeRatio);
			if (totalCleanableAmount > 0)
			{
				ContaminatingLiquid.RemoveLiquidVolume(instance, totalCleanableAmount);
			}
		}

		ContaminatingLiquid.AddLiquid(liquid);
	}

	#endregion

	protected void LoadFromDb(XElement root)
	{
		// Legacy liquids:
		if (root.Element("Liquid") != null)
		{
			ContaminatingLiquid = new LiquidMixture(new[]
			{
				new LiquidInstance
				{
					Liquid = Gameworld.Liquids.Get(long.Parse(root.Element("Liquid")?.Value ?? "0")),
					Amount = double.Parse(root.Element("Quantity")?.Value ?? "0")
				}
			}, Gameworld);
		}
		// New Liquids
		else
		{
			ContaminatingLiquid = new LiquidMixture(root.Element("Mix"), Gameworld);
		}
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("LiquidContamination", (effect, owner) => new LiquidContamination(effect, owner));
	}

	#region Overrides of Effect

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", ContaminatingLiquid.SaveToXml());
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Liquid contamination with {ContaminatingLiquid.Instances.Select(x => x.Liquid.Name).Distinct().ListToString()} {Gameworld.UnitManager.Describe(ContaminatingLiquid.TotalVolume, Framework.Units.UnitType.FluidVolume, voyeur)}";
	}

	protected override string SpecificEffectType { get; } = "LiquidContamination";

	public override bool SavingEffect { get; } = true;

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new LiquidContamination(newItem, ContaminatingLiquid, ApplicabilityProg);
		}

		return null;
	}

	/// <summary>
	/// Performs a small amount of cleaning with a liquid.
	/// </summary>
	/// <param name="liquid">A liquid being used to clean</param>
	/// <param name="amount">The amount of liquid being used</param>
	/// <returns>True if the effect is now totally cleaned</returns>
	public bool CleanWithLiquid(LiquidMixture liquid, double amount)
	{
		if (liquid == null)
		{
			return false;
		}

		var instancesToClean = new List<LiquidInstance>();
		foreach (var instance in _contaminatingLiquid.Instances.ToList())
		{
			if (liquid.CountsAs(instance.Liquid.Solvent).Truth)
			{
				instancesToClean.Add(instance);
			}
		}

		if (!instancesToClean.Any())
		{
			return false;
		}

		var totalRemovableVolume = instancesToClean.Sum(x => x.Amount);
		foreach (var instance in instancesToClean)
		{
			instance.Amount -= instance.Amount / totalRemovableVolume;
		}

		_contaminatingLiquid.ContentsUpdated();
		Changed = true;
		return ContaminatingLiquid.TotalVolume <= 0.0;
	}

	/// <summary>Fires when an effect is removed, including a matured scheduled effect</summary>
	public override void RemovalEffect()
	{
		if (ContaminatingLiquid != null)
		{
			ContaminatingLiquid.OnLiquidMixtureChanged -= ContaminatingLiquidOnLiquidMixtureChanged;
		}
	}

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void ExpireEffect()
	{
		//Decay down one stage of quanity (drenched->wet->damp->gone)
		//TODO: The decay/message thresholds should probably be tunable at some point
		//TODO: The tick frequency of the LiquidContamination effects should be tunable. Could be tunable
		//      per liquid or globally. 
		if (ContaminatingLiquid.TotalVolume <= 0.0)
		{
			base.ExpireEffect();
			return;
		}

		var dried = Math.Max(0.02 / Gameworld.UnitManager.BaseFluidToLitres, ContaminatingLiquid.TotalVolume * 0.1);
		var ratios = ContaminatingLiquid.Instances
		                                .Select(x => (Instance: x, x.Amount / ContaminatingLiquid.TotalVolume))
		                                .ToList();
		if (dried >= ContaminatingLiquid.TotalVolume)
		{
			ContaminatingLiquid = null;
		}
		else
		{
			ContaminatingLiquid.RemoveLiquidVolume(dried);
		}

		//Add or incremement residue material based on how much has dried.
		foreach (var (instance, amount) in ratios)
		{
			if (instance.Liquid.DriedResidue != null)
			{
				if (Owner.EffectsOfType<ResidueContamination>().All(x => x.Material != instance.Liquid.DriedResidue))
				{
					Owner.AddEffect(new ResidueContamination(Owner, instance.Liquid.DriedResidue,
						instance.Liquid, dried * amount * instance.Liquid.ResidueVolumePercentage,
						ApplicabilityProg));
				}
				else
				{
					Owner.EffectsOfType<ResidueContamination>()
					     .First(x => x.Material == instance.Liquid.DriedResidue)
					     .Weight += dried * amount * instance.Liquid.ResidueVolumePercentage;
				}
			}
		}

		//If we have any liquid left over, reapply liquid effect with remaining fluid
		if (ContaminatingLiquid != null)
		{
			Owner.Reschedule(this, EffectDuration(ContaminatingLiquid));
			return;
		}

		base.ExpireEffect();
	}

	#endregion

	#region Implementation of ISDescAdditionEffect

	public string AddendumText
	{
		get
		{
			var ownerItem = (IGameItem)Owner;
			var liquids = new Dictionary<ILiquid, double>();
			foreach (var liquid in ContaminatingLiquid.Instances)
			{
				var existing = liquids.Keys.FirstOrDefault(x =>
					x.DampShortDescription == liquid.Liquid.DampShortDescription &&
					x.WetShortDescription == liquid.Liquid.WetShortDescription &&
					x.DrenchedShortDescription == liquid.Liquid.DrenchedShortDescription
				);
				if (existing != null)
				{
					liquids[existing] += liquid.Amount;
				}
				else
				{
					liquids[liquid.Liquid] = liquid.Amount;
				}
			}

			var texts = new List<string>(liquids.Keys.Count);
			foreach (var instance in liquids)
			{
				var saturation = ownerItem.SaturationLevelForLiquid(instance.Value);
				switch (saturation)
				{
					case ItemSaturationLevel.Dry:
						continue;
					case ItemSaturationLevel.Damp:
						texts.Add(instance.Key.DampShortDescription);
						continue;
					case ItemSaturationLevel.Wet:
						texts.Add(instance.Key.WetShortDescription);
						continue;
					case ItemSaturationLevel.Soaked:
					case ItemSaturationLevel.Saturated:
						texts.Add(instance.Key.DrenchedShortDescription);
						continue;
				}
			}

			return texts.ListToString(conjunction: "", separator: " ");
		}
	}

	public string AdditionalText
	{
		get
		{
			var ownerItem = (IGameItem)Owner;
			var liquids = new Dictionary<ILiquid, double>();
			foreach (var liquid in ContaminatingLiquid.Instances)
			{
				var existing = liquids.Keys.FirstOrDefault(x =>
					x.DampDescription == liquid.Liquid.DampDescription &&
					x.WetDescription == liquid.Liquid.WetDescription &&
					x.DrenchedDescription == liquid.Liquid.DrenchedDescription
				);
				if (existing != null)
				{
					liquids[existing] += liquid.Amount;
				}
				else
				{
					liquids[liquid.Liquid] = liquid.Amount;
				}
			}

			var texts = new List<string>(liquids.Keys.Count);
			foreach (var instance in liquids)
			{
				var saturation = ownerItem.SaturationLevelForLiquid(instance.Value);
				switch (saturation)
				{
					case ItemSaturationLevel.Dry:
						continue;
					case ItemSaturationLevel.Damp:
						texts.Add(instance.Key.DampDescription);
						continue;
					case ItemSaturationLevel.Wet:
						texts.Add(instance.Key.WetDescription);
						continue;
					case ItemSaturationLevel.Soaked:
					case ItemSaturationLevel.Saturated:
						texts.Add(instance.Key.DrenchedDescription);
						continue;
				}
			}

			return texts.ListToString();
		}
	}

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		var ownerItem = (IGameItem)Owner;
		var liquids = new Dictionary<ILiquid, double>();
		foreach (var liquid in ContaminatingLiquid.Instances)
		{
			var existing = liquids.Keys.FirstOrDefault(x =>
				x.DampDescription == liquid.Liquid.DampDescription &&
				x.WetDescription == liquid.Liquid.WetDescription &&
				x.DrenchedDescription == liquid.Liquid.DrenchedDescription
			);
			if (existing != null)
			{
				liquids[existing] += liquid.Amount;
			}
			else
			{
				liquids[liquid.Liquid] = liquid.Amount;
			}
		}

		var texts = new List<string>(liquids.Keys.Count);
		foreach (var instance in liquids)
		{
			var saturation = ownerItem.SaturationLevelForLiquid(instance.Value);
			switch (saturation)
			{
				case ItemSaturationLevel.Dry:
					continue;
				case ItemSaturationLevel.Damp:
					texts.Add(colour
						? instance.Key.DampDescription.Colour(instance.Key.DisplayColour)
						: instance.Key.DampDescription);
					continue;
				case ItemSaturationLevel.Wet:
					texts.Add(colour
						? instance.Key.WetDescription.Colour(instance.Key.DisplayColour)
						: instance.Key.WetDescription);
					continue;
				case ItemSaturationLevel.Soaked:
				case ItemSaturationLevel.Saturated:
					texts.Add(colour
						? instance.Key.DrenchedDescription.Colour(instance.Key.DisplayColour)
						: instance.Key.DrenchedDescription);
					continue;
			}
		}

		return texts.ListToString();
	}

	public string GetAddendumText(bool colour)
	{
		var ownerItem = (IGameItem)Owner;
		var liquids = new Dictionary<ILiquid, double>();
		foreach (var liquid in ContaminatingLiquid.Instances)
		{
			var existing = liquids.Keys.FirstOrDefault(x =>
				x.DampShortDescription == liquid.Liquid.DampShortDescription &&
				x.WetShortDescription == liquid.Liquid.WetShortDescription &&
				x.DrenchedShortDescription == liquid.Liquid.DrenchedShortDescription
			);
			if (existing != null)
			{
				liquids[existing] += liquid.Amount;
			}
			else
			{
				liquids[liquid.Liquid] = liquid.Amount;
			}
		}

		var texts = new List<string>(liquids.Keys.Count);
		foreach (var instance in liquids)
		{
			var saturation = ownerItem.SaturationLevelForLiquid(instance.Value);
			switch (saturation)
			{
				case ItemSaturationLevel.Dry:
					continue;
				case ItemSaturationLevel.Damp:
					texts.Add(colour
						? instance.Key.DampShortDescription.Colour(instance.Key.DisplayColour)
						: instance.Key.DampShortDescription);
					continue;
				case ItemSaturationLevel.Wet:
					texts.Add(colour
						? instance.Key.WetShortDescription.Colour(instance.Key.DisplayColour)
						: instance.Key.WetShortDescription);
					continue;
				case ItemSaturationLevel.Soaked:
				case ItemSaturationLevel.Saturated:
					texts.Add(colour
						? instance.Key.DrenchedShortDescription.Colour(instance.Key.DisplayColour)
						: instance.Key.DrenchedShortDescription);
					continue;
			}
		}

		return texts.ListToString(conjunction: "", separator: " ");
	}

	public bool PlayerSet { get; } = false;

	#endregion

	#region Implementation of ICleanableEffect

	public TimeSpan BaseCleanTime
		=> TimeSpan.FromSeconds(Gameworld.GetStaticDouble("BaseCleanTime"));

	public ITag CleaningToolTag
		=> Gameworld.Tags.Get(Gameworld.GetStaticLong("LiquidCleaningToolTag"));

	public ILiquid LiquidRequired => ContaminatingLiquid.Instances.Where(x => x.Liquid.Solvent != null)
	                                                    .FirstMax(x => x.Amount)?.Liquid.Solvent;

	public string EmoteBeginClean => Gameworld.GetStaticString("EmoteBeginClean");
	public string EmoteStopClean => Gameworld.GetStaticString("EmoteStopClean");
	public string EmoteFinishClean => Gameworld.GetStaticString("EmoteFinishClean");
	public string PromptStatusLine => Gameworld.GetStaticString("CleanPromptStatusLine");

	public double LiquidAmountConsumed
	{
		get
		{
			var required = LiquidRequired;
			return ContaminatingLiquid.Instances
			                          .Where(x => required.LiquidCountsAs(x.Liquid.Solvent))
			                          .Sum(x => x.Amount * x.Liquid.SolventVolumeRatio);
		}

		set
		{
			ContaminatingLiquid.SetLiquidVolume(value);
			if (value > 0.0)
			{
				Owner.RescheduleIfLonger(this, EffectDuration(ContaminatingLiquid));
			}

			Changed = true;
		}
	}

	#endregion

	#region Implementation of IEffectAddsWeight

	public double AddedWeight => ContaminatingLiquid?.TotalWeight ?? 0.0;

	#endregion
}