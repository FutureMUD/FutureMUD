#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

/// <summary>Maintains a configurable room ward which blocks cross-boundary mental traffic.</summary>
public sealed class PsionicDomePower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Psionic Dome";
	public override string DatabaseType => "psionicdome";
	protected override string DefaultBeginVerb => "dome";
	protected override string DefaultEndVerb => "enddome";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicdome", (power, world) => new PsionicDomePower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicdome", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicDomePower(world, school, name, trait)));
	}
	private PsionicDomePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait)
	{
		Blurb = "Sustain a ward over the current room";
		_showHelpText = $"Use {school.SchoolVerb.ToUpperInvariant()} DOME to ward the current room against cross-boundary psionic traffic. Use {school.SchoolVerb.ToUpperInvariant()} ENDDOME to release it.";
		ConcentrationPointsToSustain = 1.0;
		DoDatabaseInsert();
	}
	private PsionicDomePower(MagicPower model, IFuturemud world) : base(model, world) { }
	protected override XElement SaveDefinition() => SaveSustainedSelfDefinition();
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (verb.EqualTo(BeginVerb) && actor.Location is null)
		{
			actor.OutputHandler.Send("You need to be in a room to establish a psionic dome.");
			return;
		}
		base.UseCommand(actor, verb, command);
	}
	protected override IEffect CreateEffect(ICharacter actor) => new PsionicDomeEffect(actor, this);
	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor) => actor.EffectsOfType<PsionicDomeEffect>().Where(x => x.Power == this);
	protected override CheckType ActivationCheckType => CheckType.MagicTelepathyCheck;
}
