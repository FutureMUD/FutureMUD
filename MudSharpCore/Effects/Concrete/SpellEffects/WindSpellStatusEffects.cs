#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.Planes;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellLevitationEffect : MagicSpellEffectBase, ILevitationEffect, IDescriptionAdditionEffect,
	ISDescAdditionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellLevitation", (effect, owner) => new SpellLevitationEffect(effect, owner));
	}

	public SpellLevitationEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog,
		bool preserveLayer, RoomLayer targetLayer, string sdescAddendum, string descAddendum, ANSIColour colour)
		: base(owner, parent, prog)
	{
		PreserveLayer = preserveLayer;
		TargetLayer = targetLayer;
		SDescAddendum = sdescAddendum;
		DescAddendum = descAddendum;
		AddendumColour = colour;
	}

	protected SpellLevitationEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		PreserveLayer = bool.Parse(trueRoot?.Element("PreserveLayer")?.Value ?? "true");
		TargetLayer = (RoomLayer)int.Parse(trueRoot?.Element("TargetLayer")?.Value ?? ((int)RoomLayer.InAir).ToString());
		SDescAddendum = trueRoot?.Element("SDescAddendum")?.Value ?? "(levitating)";
		DescAddendum = trueRoot?.Element("DescAddendum")?.Value ?? "@ appears to be suspended in the air.";
		AddendumColour = Telnet.GetColour(trueRoot?.Element("AddendumColour")?.Value ?? "bold cyan");
	}

	public bool PreserveLayer { get; }
	public RoomLayer TargetLayer { get; }
	public string SDescAddendum { get; }
	public string DescAddendum { get; }
	public ANSIColour AddendumColour { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("PreserveLayer", PreserveLayer),
			new XElement("TargetLayer", (int)TargetLayer),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (PreserveLayer || Owner.Location is null)
		{
			return;
		}

		if (Owner.Location.Terrain(Owner as IPerceiver).TerrainLayers.Contains(TargetLayer))
		{
			Owner.RoomLayer = TargetLayer;
		}
	}

	public override void RemovalEffect()
	{
		if (Owner is IPerceiver perceiver && ShouldFallAfterRemoval(perceiver))
		{
			perceiver.FallToGround();
		}

		base.RemovalEffect();
	}

	private bool ShouldFallAfterRemoval(IPerceiver perceiver)
	{
		if (perceiver.EffectsOfType<IPreventFallingEffect>()
		             .Any(x => !ReferenceEquals(x, this) && x.Applies()))
		{
			return false;
		}

		if (perceiver.PositionState?.SafeFromFalling == true)
		{
			return false;
		}

		if (perceiver.Location is null ||
		    ZeroGravityMovementHelper.IsZeroGravity(perceiver.Location, perceiver.RoomLayer, perceiver))
		{
			return false;
		}

		if (!perceiver.RoomLayer.IsHigherThan(RoomLayer.GroundLevel))
		{
			return false;
		}

		if (perceiver.EffectsOfType<Dragging.DragTarget>().Any())
		{
			return false;
		}

		if (perceiver is not ICharacter character)
		{
			return true;
		}

		return character.RidingMount is null &&
		       !character.CombinedEffectsOfType<IImmwalkEffect>().Any() &&
		       !character.SuspendsPhysicalContact();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return PreserveLayer
			? "Magically levitating."
			: $"Magically levitating {TargetLayer.LocativeDescription()}.";
	}

	protected override string SpecificEffectType => "SpellLevitation";

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

public class SpellFeatherFallEffect : MagicSpellEffectBase, IFallDamageMitigationEffect, IDescriptionAdditionEffect,
	ISDescAdditionEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellFeatherFall", (effect, owner) => new SpellFeatherFallEffect(effect, owner));
	}

	public SpellFeatherFallEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog,
		double fallDistanceMultiplier, double fallDamageMultiplier, string sdescAddendum, string descAddendum,
		ANSIColour colour)
		: base(owner, parent, prog)
	{
		FallDistanceMultiplier = Math.Max(0.0, fallDistanceMultiplier);
		FallDamageMultiplier = Math.Max(0.0, fallDamageMultiplier);
		SDescAddendum = sdescAddendum;
		DescAddendum = descAddendum;
		AddendumColour = colour;
	}

	protected SpellFeatherFallEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		var trueRoot = root.Element("Effect");
		FallDistanceMultiplier =
			Math.Max(0.0, double.Parse(trueRoot?.Element("FallDistanceMultiplier")?.Value ?? "0"));
		FallDamageMultiplier =
			Math.Max(0.0, double.Parse(trueRoot?.Element("FallDamageMultiplier")?.Value ?? "0"));
		SDescAddendum = trueRoot?.Element("SDescAddendum")?.Value ?? "(falling lightly)";
		DescAddendum = trueRoot?.Element("DescAddendum")?.Value ?? "@ seems to drift lightly through the air.";
		AddendumColour = Telnet.GetColour(trueRoot?.Element("AddendumColour")?.Value ?? "bold cyan");
	}

	public double FallDistanceMultiplier { get; }
	public double FallDamageMultiplier { get; }
	public string SDescAddendum { get; }
	public string DescAddendum { get; }
	public ANSIColour AddendumColour { get; }

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("FallDistanceMultiplier", FallDistanceMultiplier),
			new XElement("FallDamageMultiplier", FallDamageMultiplier),
			new XElement("SDescAddendum", new XCData(SDescAddendum)),
			new XElement("DescAddendum", new XCData(DescAddendum)),
			new XElement("AddendumColour", AddendumColour.Name)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"Magically feather falling - distance x{FallDistanceMultiplier.ToString("N2", voyeur)} damage x{FallDamageMultiplier.ToString("N2", voyeur)}.";
	}

	protected override string SpecificEffectType => "SpellFeatherFall";

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
