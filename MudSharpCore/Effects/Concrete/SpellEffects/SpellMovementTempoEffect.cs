#nullable enable

using System;
using System.Globalization;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Movement;

namespace MudSharp.Effects.Concrete.SpellEffects;

/// <summary>A spell-owned multiplier on ordinary movement time; admission remains with native movement.</summary>
public sealed class SpellMovementTempoEffect : MagicSpellEffectBase, IMovementDelayEffect
{
	public static void InitialiseEffectType() =>
		RegisterFactory("SpellMovementTempo", (effect, owner) => new SpellMovementTempoEffect(effect, owner));

	public SpellMovementTempoEffect(IPerceivable owner, IMagicSpellEffectParent parent, double multiplier)
		: base(owner, parent, null)
	{
		Multiplier = Validate(multiplier);
	}

	private SpellMovementTempoEffect(XElement root, IPerceivable owner) : base(root, owner)
	{
		Multiplier = Validate(double.Parse(root.Element("Effect")?.Element("Multiplier")?.Value ??
			throw new FormatException("Missing spell movement multiplier."), CultureInfo.InvariantCulture));
	}

	public double Multiplier { get; }
	public double DelayMultiplier(IMoveSpeed speed) => Multiplier;
	protected override string SpecificEffectType => "SpellMovementTempo";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Multiplier",
		Multiplier.ToString("R", CultureInfo.InvariantCulture)));
	public override string Describe(IPerceiver voyeur) =>
		$"Movement time x{Multiplier.ToString("N2", voyeur)}.";

	private static double Validate(double value) => double.IsFinite(value) && value is >= 0.1 and <= 10
		? value : throw new ArgumentOutOfRangeException(nameof(value), "Movement time multiplier must be finite and within 0.1..10.");
}
