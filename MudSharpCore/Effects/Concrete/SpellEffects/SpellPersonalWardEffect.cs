using MudSharp.Magic;

#nullable enable
#nullable disable warnings

namespace MudSharp.Effects.Concrete.SpellEffects;

public class SpellPersonalWardEffect : MagicInterdictionSpellEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPersonalWard", (effect, owner) => new SpellPersonalWardEffect(effect, owner));
	}

	public SpellPersonalWardEffect(IPerceivable owner, IMagicSpellEffectParent parent, IMagicSchool school,
		MagicInterdictionMode mode, MagicInterdictionCoverage coverage, bool includesSubschools, IFutureProg? prog, bool oneUse = false)
		: base(owner, parent, school, mode, coverage, includesSubschools, prog, oneUse)
	{
	}

	protected SpellPersonalWardEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return InterdictionDescription(voyeur, "Personal");
	}

	protected override XElement SaveDefinition()
	{
		return SaveInterdictionDefinition();
	}

	protected override string SpecificEffectType => "SpellPersonalWard";
}
