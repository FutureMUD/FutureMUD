#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellBurningEffect : MagicSpellEffectBase, IDescriptionAdditionEffect, ISDescAdditionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellBurning", (effect, owner) => new SpellBurningEffect(effect, owner));
	}

	public SpellBurningEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog,
		DamageType damageType, double damagePerTick, double painPerTick, double stunPerTick,
		double thermalLoadPerTick, double tickSeconds, double minimumOxidation, bool selfOxidising,
		string sdescAddendum, string descAddendum, ANSIColour colour)
		: base(owner, parent, prog)
	{
		DamageType = damageType;
		DamagePerTick = Math.Max(0.0, damagePerTick);
		PainPerTick = Math.Max(0.0, painPerTick);
		StunPerTick = Math.Max(0.0, stunPerTick);
		ThermalLoadPerTick = Math.Max(0.0, thermalLoadPerTick);
		TickInterval = TimeSpan.FromSeconds(Math.Max(1.0, tickSeconds));
		MinimumOxidation = Math.Max(0.0, minimumOxidation);
		SelfOxidising = selfOxidising;
		SDescAddendum = sdescAddendum;
		DescAddendum = descAddendum;
		AddendumColour = colour;
	}

	protected SpellBurningEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		DamageType = (DamageType)int.Parse(trueRoot?.Element("DamageType")?.Value ?? ((int)DamageType.Burning).ToString());
		DamagePerTick = Math.Max(0.0, double.Parse(trueRoot?.Element("DamagePerTick")?.Value ?? "0"));
		PainPerTick = Math.Max(0.0, double.Parse(trueRoot?.Element("PainPerTick")?.Value ?? "0"));
		StunPerTick = Math.Max(0.0, double.Parse(trueRoot?.Element("StunPerTick")?.Value ?? "0"));
		ThermalLoadPerTick = Math.Max(0.0, double.Parse(trueRoot?.Element("ThermalLoadPerTick")?.Value ?? "0"));
		TickInterval = TimeSpan.FromSeconds(Math.Max(1.0, double.Parse(trueRoot?.Element("TickSeconds")?.Value ?? "10")));
		MinimumOxidation = Math.Max(0.0, double.Parse(trueRoot?.Element("MinimumOxidation")?.Value ?? "0"));
		SelfOxidising = bool.Parse(trueRoot?.Element("SelfOxidising")?.Value ?? "false");
		SDescAddendum = trueRoot?.Element("SDescAddendum")?.Value ?? "(burning)";
		DescAddendum = trueRoot?.Element("DescAddendum")?.Value ?? "@ is burning with magical fire.";
		AddendumColour = Telnet.GetColour(trueRoot?.Element("AddendumColour")?.Value ?? "bold orange");
	}

	public DamageType DamageType { get; }
	public double DamagePerTick { get; }
	public double PainPerTick { get; }
	public double StunPerTick { get; }
	public double ThermalLoadPerTick { get; }
	public TimeSpan TickInterval { get; }
	public double MinimumOxidation { get; }
	public bool SelfOxidising { get; }
	public string SDescAddendum { get; }
	public string DescAddendum { get; }
	public ANSIColour AddendumColour { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("DamageType", (int)DamageType),
			new XElement("DamagePerTick", DamagePerTick),
			new XElement("PainPerTick", PainPerTick),
			new XElement("StunPerTick", StunPerTick),
			new XElement("ThermalLoadPerTick", ThermalLoadPerTick),
			new XElement("TickSeconds", TickInterval.TotalSeconds),
			new XElement("MinimumOxidation", MinimumOxidation),
			new XElement("SelfOxidising", SelfOxidising),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		Owner.Reschedule(this, TickInterval);
	}

	public override void ExpireEffect()
	{
		if (!CanSustainFire())
		{
			base.ExpireEffect();
			return;
		}

		ApplyBurnDamage().ProcessPassiveWounds();
		ApplyThermalLoad();
		Owner.Reschedule(this, TickInterval);
	}

	private bool CanSustainFire()
	{
		if (SelfOxidising)
		{
			return true;
		}

		return (Owner.Location?.Atmosphere as IGas)?.OxidationFactor >= MinimumOxidation;
	}

	private void ApplyThermalLoad()
	{
		if (ThermalLoadPerTick <= 0.0 || Owner is not ICharacter character)
		{
			return;
		}

		var thermal = character.CombinedEffectsOfType<ThermalImbalance>().FirstOrDefault();
		if (thermal is null)
		{
			thermal = new ThermalImbalance(character);
			character.AddEffect(thermal);
		}

		thermal.ImbalanceProgress += ThermalLoadPerTick;
	}

	private IEnumerable<IWound> ApplyBurnDamage()
	{
		switch (Owner)
		{
			case ICharacter character:
				var bodypart = character.Body.RandomBodypart;
				return character.PassiveSufferDamage(new Damage
				{
					ActorOrigin = ParentEffect?.Caster,
					Bodypart = bodypart,
					DamageAmount = DamagePerTick,
					PainAmount = PainPerTick,
					StunAmount = StunPerTick,
					ShockAmount = 0.0,
					DamageType = DamageType,
					AngleOfIncidentRadians = Math.PI * 0.5,
					PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass }
				});
			case IGameItem item:
				return item.PassiveSufferDamage(new Damage
				{
					ActorOrigin = ParentEffect?.Caster,
					DamageAmount = DamagePerTick,
					PainAmount = 0.0,
					StunAmount = 0.0,
					ShockAmount = 0.0,
					DamageType = DamageType,
					AngleOfIncidentRadians = Math.PI * 0.5,
					PenetrationOutcome = new CheckOutcome { Outcome = Outcome.MajorPass }
				});
			default:
				return Enumerable.Empty<IWound>();
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Burning with magical {DamageType.DescribeEnum().ToLowerInvariant()} damage every {TickInterval.Describe(voyeur)}.";
	}

	protected override string SpecificEffectType => "SpellBurning";

	public string GetAdditionalText(IPerceiver voyeur, bool colour)
	{
		return colour ? DescAddendum.Colour(AddendumColour) : DescAddendum;
	}

	public bool PlayerSet => false;

	public string AddendumText => SDescAddendum;

	public string GetAddendumText(bool colour)
	{
		return colour ? SDescAddendum.Colour(AddendumColour) : SDescAddendum;
	}
}

public class SpellTrackMarkEffect : MagicSpellEffectBase, ITrackIntensityEffect, ISDescAdditionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellTrackMark", (effect, owner) => new SpellTrackMarkEffect(effect, owner));
		RegisterFactory("SpellTrackmark", (effect, owner) => new SpellTrackMarkEffect(effect, owner));
	}

	public SpellTrackMarkEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog,
		double visualMultiplier, double olfactoryMultiplier, double visualBonus, double olfactoryBonus,
		TrackCircumstances additionalCircumstances, string sdescAddendum, ANSIColour colour)
		: base(owner, parent, prog)
	{
		VisualTrackIntensityMultiplier = Math.Max(0.0, visualMultiplier);
		OlfactoryTrackIntensityMultiplier = Math.Max(0.0, olfactoryMultiplier);
		VisualTrackIntensityBonus = Math.Max(0.0, visualBonus);
		OlfactoryTrackIntensityBonus = Math.Max(0.0, olfactoryBonus);
		AdditionalTrackCircumstances = additionalCircumstances;
		SDescAddendum = sdescAddendum;
		AddendumColour = colour;
	}

	protected SpellTrackMarkEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		VisualTrackIntensityMultiplier = Math.Max(0.0, double.Parse(trueRoot?.Element("VisualMultiplier")?.Value ?? "1"));
		OlfactoryTrackIntensityMultiplier = Math.Max(0.0, double.Parse(trueRoot?.Element("OlfactoryMultiplier")?.Value ?? "1"));
		VisualTrackIntensityBonus = Math.Max(0.0, double.Parse(trueRoot?.Element("VisualBonus")?.Value ?? "0"));
		OlfactoryTrackIntensityBonus = Math.Max(0.0, double.Parse(trueRoot?.Element("OlfactoryBonus")?.Value ?? "0"));
		AdditionalTrackCircumstances = (TrackCircumstances)int.Parse(trueRoot?.Element("AdditionalCircumstances")?.Value ?? "0");
		SDescAddendum = trueRoot?.Element("SDescAddendum")?.Value ?? "(leaving luminous tracks)";
		AddendumColour = Telnet.GetColour(trueRoot?.Element("AddendumColour")?.Value ?? "bold cyan");
	}

	public double VisualTrackIntensityMultiplier { get; }
	public double OlfactoryTrackIntensityMultiplier { get; }
	public double VisualTrackIntensityBonus { get; }
	public double OlfactoryTrackIntensityBonus { get; }
	public TrackCircumstances AdditionalTrackCircumstances { get; }
	public string SDescAddendum { get; }
	public ANSIColour AddendumColour { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("VisualMultiplier", VisualTrackIntensityMultiplier),
			new XElement("OlfactoryMultiplier", OlfactoryTrackIntensityMultiplier),
			new XElement("VisualBonus", VisualTrackIntensityBonus),
			new XElement("OlfactoryBonus", OlfactoryTrackIntensityBonus),
			new XElement("AdditionalCircumstances", (int)AdditionalTrackCircumstances),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Altering tracks: visual x{VisualTrackIntensityMultiplier.ToString("N2", voyeur)} +{VisualTrackIntensityBonus.ToString("N2", voyeur)}, olfactory x{OlfactoryTrackIntensityMultiplier.ToString("N2", voyeur)} +{OlfactoryTrackIntensityBonus.ToString("N2", voyeur)}.";
	}

	protected override string SpecificEffectType => "SpellTrackMark";

	public string AddendumText => SDescAddendum;

	public string GetAddendumText(bool colour)
	{
		return colour ? SDescAddendum.Colour(AddendumColour) : SDescAddendum;
	}
}
