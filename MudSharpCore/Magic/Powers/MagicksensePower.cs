#nullable enable

using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Magic.Powers;

public sealed class MagicksensePower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Magicksense";
	public override string DatabaseType => "magicksense";
	protected override string DefaultBeginVerb => "magicksense";
	protected override string DefaultEndVerb => "endmagicksense";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("magicksense", (power, gameworld) => new MagicksensePower(power, gameworld));
		MagicPowerFactory.RegisterBuilderLoader("magicksense", BuilderLoader);
	}

	private static IMagicPower? BuilderLoader(IFuturemud gameworld, IMagicSchool school, string name, ICharacter actor,
		StringStack command)
	{
		return PsionicV4PowerBuilderHelpers.BuildWithSkill(gameworld, school, name, actor, command,
			trait => new MagicksensePower(gameworld, school, name, trait));
	}

	private MagicksensePower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait) : base(gameworld, school, name, trait)
	{
		Blurb = "Sense magical auras while sustained";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} MAGICKSENSE to gain the normal magical aura sense.";
		DoDatabaseInsert();
	}

	private MagicksensePower(MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
	}

	protected override XElement SaveDefinition()
	{
		return SaveSustainedSelfDefinition();
	}

	protected override IEffect CreateEffect(ICharacter actor)
	{
		return new MagicMagicksenseEffect(actor, this);
	}

	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor)
	{
		return actor.EffectsOfType<MagicMagicksenseEffect>().Where(x => x.Power == this);
	}
}

