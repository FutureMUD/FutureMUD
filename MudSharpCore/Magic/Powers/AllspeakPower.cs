#nullable enable

using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed class AllspeakPower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Allspeak";
	public override string DatabaseType => "allspeak";
	protected override string DefaultBeginVerb => "allspeak";
	protected override string DefaultEndVerb => "endallspeak";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("allspeak", (power, gameworld) => new AllspeakPower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("allspeak", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new AllspeakPower(gameworld, school, name, trait));
	}

	private AllspeakPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Comprehend spoken and written language while sustained";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} ALLSPEAK to comprehend languages without learning them.";
		DoDatabaseInsert();
	}

	private AllspeakPower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
	}

	protected override XElement SaveDefinition()
	{
		return SaveSustainedSelfDefinition();
	}

	protected override IEffect CreateEffect(ICharacter actor)
	{
		return new MagicAllspeakEffect(actor, this);
	}

	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor)
	{
		return actor.EffectsOfType<MagicAllspeakEffect>().Where(x => x.Power == this);
	}
}

