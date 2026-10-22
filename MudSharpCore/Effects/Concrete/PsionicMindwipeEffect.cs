#nullable enable

using MudSharp.Magic.Powers;
using MudSharp.Magic;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

/// <summary>Timed broad mental impairment created by a configured mindwipe power.</summary>
public sealed class PsionicMindwipeEffect : Effect, IMagicEffect, IMindImpairmentEffect
{
	public static void InitialiseEffectType() => RegisterFactory("PsionicMindwipe", (effect, owner) => new PsionicMindwipeEffect(effect, owner));

	public PsionicMindwipeEffect(ICharacter owner, PsionicMindwipePower power) : base(owner)
	{
		Power = power;
	}

	private PsionicMindwipeEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect")!;
		Power = Gameworld.MagicPowers.Get(long.Parse(root.Element("Power")!.Value)) as PsionicMindwipePower
		        ?? throw new ApplicationException("A mindwipe effect referenced a missing power.");
	}

	public PsionicMindwipePower Power { get; }
	public IMagicSchool School => Power.School;
	public IMagicPower PowerOrigin => Power;
	public Difficulty DetectMagicDifficulty => Power.DetectableWithDetectMagic;
	public override bool SavingEffect => true;
	public double PsionicInitiatorModifier => -25.0;
	public double PsionicTargetModifier => -25.0;
	public double SkillUseModifier => -25.0;
	public bool PreventsSkillLearning => true;
	protected override string SpecificEffectType => "PsionicMindwipe";
	public override string Describe(IPerceiver voyeur) => $"Mentally impaired by {Power.Name.Colour(Power.School.PowerListColour)}.";
	protected override XElement SaveDefinition() => new("Effect", new XElement("Power", Power.Id));
}
