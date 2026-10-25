#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;

namespace MudSharp.Magic.Powers;

public sealed class PsionicSensePresencePower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Sense Presence";
	public override string DatabaseType => "psionicsensepresence";
	protected override string DefaultBeginVerb => "sensepresence";
	protected override string DefaultEndVerb => "endsensepresence";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicsensepresence", (row, world) => new PsionicSensePresencePower(row, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicsensepresence", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicSensePresencePower(world, school, name, trait)));
	}
	private PsionicSensePresencePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait)
	{
		Blurb = "Sense nearby minds through stealth and psionic vanishing";
		_showHelpText = $"Use {school.SchoolVerb} sensepresence to sense nearby minds, or endsensepresence to stop. This helps notice sneaking psionic characters and reveals vanished minds in your room unless their mental signature is concealed.";
		DoDatabaseInsert();
	}
	private PsionicSensePresencePower(MagicPower row, IFuturemud world) : base(row, world) { }
	protected override XElement SaveDefinition() => SaveSustainedSelfDefinition();
	protected override IEffect CreateEffect(ICharacter actor) => new PsionicSensePresenceEffect(actor, this);
	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor) => actor.EffectsOfType<PsionicSensePresenceEffect>().Where(x => x.Power == this);
}
