using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.Effects.Concrete;

public class BodyLiquidContamination : Effect, ILiquidContaminationEffect, IDescriptionAdditionEffect, IEffectAddsWeight
{
	public static BodyLiquidContamination CreateOrMergeEffect(ICharacter actor, LiquidMixture mixture, IEnumerable<IExternalBodypart> bodyparts)
	{
		var existing = actor.CombinedEffectsOfType<BodyLiquidContamination>().FirstOrDefault(x => bodyparts.All(y => x.Bodyparts.Contains(y)));
		if (existing is not null)
		{
			existing.AddLiquid(mixture);
			actor.Body.RescheduleIfLonger(existing, EffectDuration(existing.ContaminatingLiquid));
			return existing;
		}

		var effect = new BodyLiquidContamination(actor.Body, mixture, bodyparts);
		actor.Body.AddEffect(effect, EffectDuration(mixture));
		return effect;
	}

	private static TimeSpan BaseEffectDuration =>
		TimeSpan.FromSeconds(Futuremud.Games.First().GetStaticDouble("BodyLiquidContaminationEffectDuration"));

	public static TimeSpan EffectDuration(LiquidMixture mixture)
	{
		return BaseEffectDuration * mixture.RelativeEnthalpy;
	}

	public List<IExternalBodypart> Bodyparts { get; set; }
	public string BodypartGroupDescription { get; set; }
	public IBody BodyOwner { get; set; }

	public BodyLiquidContamination(IBody owner, LiquidMixture mixture, IEnumerable<IExternalBodypart> bodyparts) : base(owner, null)
	{
		BodyOwner = owner;
		Bodyparts = new List<IExternalBodypart>(bodyparts);
		BodypartGroupDescription = BodyOwner.DescribeBodypartGroup(Bodyparts);
		ContaminatingLiquid = mixture;
	}

	protected BodyLiquidContamination(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		BodyOwner = (IBody)owner;
		LoadFromDb(effect.Element("Effect"));
	}

	protected void LoadFromDb(XElement root)
	{
		ContaminatingLiquid = new LiquidMixture(root.Element("Mix"), Gameworld);
		Bodyparts = root.Element("Parts").Value
		                .Split(' ')
		                .Select(x => long.Parse(x))
		                .Select(x => BodyOwner.Prototype.AllExternalBodyparts.FirstOrDefault(y => y.Id == x))
		                .ToList();
		BodypartGroupDescription = root.Element("BodypartGroupDescription").Value;
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			ContaminatingLiquid.SaveToXml(),
			new XElement("Parts",
				new XCData(Bodyparts.Select(x => x.Id.ToString()).ListToString(separator: " ", conjunction: ""))),
			new XElement("BodypartGroupDescription", new XCData(BodypartGroupDescription))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Character Liquid contamination with {ContaminatingLiquid.Instances.Select(x => x.Liquid.Name).Distinct().ListToString()} {Gameworld.UnitManager.Describe(ContaminatingLiquid.TotalVolume, Framework.Units.UnitType.FluidVolume, voyeur)}";
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("BodyLiquidContamination", (effect, owner) => new BodyLiquidContamination(effect, owner));
	}

	protected override string SpecificEffectType => "BodyLiquidContamination";

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

	#region Implementation of ICleanableEffect

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


	#region Implementation of ISDescAdditionEffect
	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		var saturation = BodyOwner.SaturationLevelForLiquid(Bodyparts);
		switch (saturation)
		{
			case ItemSaturationLevel.Dry:
				return "";
		}

		var partDescription = BodyOwner.DescribeBodypartGroup(Bodyparts);
		return $"{BodyOwner.ApparentGender(voyeur).Possessive(true)} {partDescription} {(partDescription.ContainsPlural() ? "are" : "is")} {saturation.DescribeEnum()} with {(colour ? ContaminatingLiquid.ColouredLiquidDescription : ContaminatingLiquid.LiquidDescription)}.";
	}
	#endregion

	public bool PlayerSet => false;

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

	private void ContaminatingLiquidOnLiquidMixtureChanged(LiquidMixture mixture)
	{
		if (mixture.IsEmpty)
		{
			Owner.RemoveEffect(this, true);
			return;
		}
		var (coat, _) = BodyOwner.LiquidAbsorbtionAmounts;
		var saturation = BodyOwner.SaturationLevel;
		double maxAbsorbed = 0.0;
		switch (saturation)
		{
			case ItemSaturationLevel.Dry:
				maxAbsorbed = (coat) * 0.98;
				break;
			case ItemSaturationLevel.Damp:
				maxAbsorbed = (coat) * 0.8;
				break;
			case ItemSaturationLevel.Wet:
				maxAbsorbed = (coat) * 0.5;
				break;
			case ItemSaturationLevel.Soaked:
				maxAbsorbed = (coat) * 0.35;
				break;
			case ItemSaturationLevel.Saturated:
				maxAbsorbed = (coat) * 0.01;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		if (mixture.TotalVolume > maxAbsorbed)
		{
			_contaminatingLiquid.OnLiquidMixtureChanged -= ContaminatingLiquidOnLiquidMixtureChanged;
			var difference = mixture.TotalVolume - maxAbsorbed;
			ContaminatingLiquid.SetLiquidVolume(maxAbsorbed);
			var location = BodyOwner.Location;
			if (location != null && !location.IsSwimmingLayer(BodyOwner.RoomLayer))
			{
				if (DateTime.UtcNow - _lastDripEcho > TimeSpan.FromSeconds(120))
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
						verb = "flows off of";
					}
					else
					{
						amount = "an enormous amount of";
						verb = "flows off of";
					}

					BodyOwner.Actor.OutputHandler.Handle(new EmoteOutput(
						new Emote(
							$"{amount} {ContaminatingLiquid.ColouredLiquidDescription} {verb} $0's {BodyOwner.DescribeBodypartGroup(Bodyparts)} and onto the ground.",
							BodyOwner.Actor, BodyOwner.Actor), style: OutputStyle.NoNewLine,
						flags: OutputFlags.NoticeCheckRequired | OutputFlags.Insigificant));
				}

				var excess = new LiquidMixture(mixture);
				excess.SetLiquidVolume(difference);
				PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(excess, location, BodyOwner.RoomLayer, BodyOwner.Actor);
			}

			_contaminatingLiquid.OnLiquidMixtureChanged += ContaminatingLiquidOnLiquidMixtureChanged;
		}
		Changed = true;
	}

	private DateTime _lastDripEcho = DateTime.MinValue;
}