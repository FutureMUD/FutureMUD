#nullable enable
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;

namespace MudSharp.Magic.Powers;

public sealed class PsionicVanishPower : PsionicSustainedSelfPowerBase
{
	public override string PowerType => "Psionic Vanish";
	public override string DatabaseType => "psionicvanish";
	protected override string DefaultBeginVerb => "vanish";
	protected override string DefaultEndVerb => "endvanish";
	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("psionicvanish", (row, world) => new PsionicVanishPower(row, world));
		MagicPowerFactory.RegisterBuilderLoader("psionicvanish", (world, school, name, actor, command) =>
			PsionicV4PowerBuilderHelpers.BuildWithSkill(world, school, name, actor, command,
				trait => new PsionicVanishPower(world, school, name, trait)));
	}
	private PsionicVanishPower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name, trait)
	{
		Blurb = "Cloak your appearance from nearby minds";
		_showHelpText = $"Use {school.SchoolVerb} vanish <room|near|far|veryfar>, or endvanish. Near includes adjacent rooms, far your zone, and veryfar your shard; these require 25, 50 and 75 skill. Observers may resist, and Sense Presence can reveal an unconcealed mind in the same room.";
		DoDatabaseInsert();
	}
	private PsionicVanishPower(MagicPower row, IFuturemud world) : base(row, world) { }
	protected override XElement SaveDefinition() => SaveSustainedSelfDefinition();
	private static int Scope(string text) => text.ToLowerInvariant() switch { "" or "room" => 0, "near" => 1, "far" => 2, "veryfar" => 3, _ => -1 };
	protected override bool ValidateBeginArguments(ICharacter actor, StringStack command)
	{
		var scope = Scope(command.SafeRemainingArgument);
		if (scope >= 0 && actor.TraitValue(SkillCheckTrait) >= scope * 25) return true;
		actor.Send("Choose room, near (25 skill), far (50 skill) or veryfar (75 skill).");
		return false;
	}
	protected override IEffect CreateEffect(ICharacter actor) => new PsionicVanishEffect(actor, this, MagicPowerDistance.SameLocationOnly);
	protected override IEffect CreateEffect(ICharacter actor, StringStack command) => new PsionicVanishEffect(actor, this,
		Scope(command.SafeRemainingArgument) switch { 1 => MagicPowerDistance.AdjacentLocationsOnly, 2 => MagicPowerDistance.SameZoneOnly, 3 => MagicPowerDistance.SameShardOnly, _ => MagicPowerDistance.SameLocationOnly });
	protected override IEnumerable<IEffect> ActiveEffects(ICharacter actor) => actor.EffectsOfType<PsionicVanishEffect>().Where(x => x.Power == this);
}
