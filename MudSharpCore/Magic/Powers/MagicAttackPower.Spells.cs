#nullable enable

using MudSharp.Magic.SpellTriggers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public partial class MagicAttackPower
{
	private long _attackSpellId;
	public IMagicSpell? AttackSpell => _attackSpellId == 0 ? null : Gameworld.MagicSpells.Get(_attackSpellId);
	public SpellPower AttackSpellPower { get; private set; } = SpellPower.Standard;
	protected virtual bool TargetsItems => false;
	public bool HasValidAttackSpell => _attackSpellId == 0 || AttackSpell is { ReadyForGame: true } spell &&
		spell.School == School && spell.Trigger is AttackHitTrigger trigger && trigger.TargetsItems == TargetsItems &&
		spell.CasterSpellEffects.All(x => x.IsCompatibleWithTrigger(new AttackHitTrigger(false)));

	public void ApplyAttackSpell(ICharacter actor, IPerceivable target, CheckOutcome outcome)
	{
		if (outcome.IsPass() && HasValidAttackSpell) AttackSpell?.ResolveAttackSpell(actor, target, AttackSpellPower, outcome);
	}

	private bool BuildingCommandAttackSpell(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_attackSpellId = 0; Changed = true; actor.Send("This attack no longer applies a spell payload."); return true;
		}
		var spell = Gameworld.MagicSpells.GetByIdOrName(command.SafeRemainingArgument);
		if (spell is null || !spell.ReadyForGame || spell.School != School ||
		    spell.Trigger is not AttackHitTrigger trigger || trigger.TargetsItems != TargetsItems ||
		    spell.CasterSpellEffects.Any(x => !x.IsCompatibleWithTrigger(new AttackHitTrigger(false))))
		{
			actor.Send($"Choose a ready spell in this school with an {(TargetsItems ? "attackitem" : "attackcharacter")} trigger and character-compatible caster effects.");
			return false;
		}
		_attackSpellId = spell.Id; Changed = true;
		actor.Send($"Successful hits now apply {spell.Name.ColourName()}, including its caster effects. Attack costs replace spell casting costs.");
		return true;
	}
}
