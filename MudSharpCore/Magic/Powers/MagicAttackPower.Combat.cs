#nullable enable

using System.Globalization;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Magic.Powers;

public partial class MagicAttackPower
{
	public MagicAttackRange AttackRange { get; private set; }
	public int RangeInRooms { get; private set; }
	public bool DealsDamage { get; private set; } = true;
	private readonly List<MagicAttackEffect> _attackEffects = [];
	public IReadOnlyList<MagicAttackEffect> AttackEffects => _attackEffects;
	public string AttackEmote { get; private set; } = "@ direct|directs a surge of force at $1.";

	private void LoadCombatDefinition(XElement root)
	{
		AttackRange = Enum.Parse<MagicAttackRange>(root.Element("AttackRange")?.Value ?? "Melee", true);
		RangeInRooms = int.Parse(root.Element("RangeInRooms")?.Value ?? "0", CultureInfo.InvariantCulture);
		DealsDamage = bool.Parse(root.Element("DealsDamage")?.Value ?? "true");
		AttackEmote = root.Element("AttackEmote")?.Value ?? AttackEmote;
		_attackSpellId = long.Parse(root.Element("AttackSpell")?.Value ?? "0");
		AttackSpellPower = Enum.Parse<SpellPower>(root.Element("AttackSpellPower")?.Value ?? "Standard", true);
		foreach (var element in root.Element("AttackEffects")?.Elements("Effect") ?? [])
		{
			_attackEffects.Add(new MagicAttackEffect(
				Enum.Parse<MagicAttackEffectType>(element.Attribute("type")!.Value, true),
				Enum.Parse<Difficulty>(element.Element("Resistance")!.Value, true),
				double.Parse(element.Element("Strength")!.Value, CultureInfo.InvariantCulture),
				double.Parse(element.Element("DurationSeconds")!.Value, CultureInfo.InvariantCulture),
				element.Element("SuccessEmote")!.Value, element.Element("ResistEmote")!.Value));
		}
		ValidateCombatDefinition();
	}

	private void ValidateCombatDefinition()
	{
		if (!Enum.IsDefined(AttackRange) || !Enum.IsDefined(AttackSpellPower) || _attackSpellId < 0 || RangeInRooms < 0 ||
		    TargetsItems && _attackEffects.Count > 0 ||
		    _attackEffects.Select(x => x.Type).Distinct().Count() != _attackEffects.Count ||
		    _attackEffects.Any(x => !Enum.IsDefined(x.Type) || !Enum.IsDefined(x.Resistance) ||
		                           !double.IsFinite(x.Strength) || x.Strength <= 0 ||
		                           !double.IsFinite(x.DurationSeconds) || x.DurationSeconds < 0) ||
		    (_attackEffects.Any(x => x.Type == MagicAttackEffectType.Pull) &&
		     _attackEffects.Any(x => x.Type == MagicAttackEffectType.Pushback)))
			throw new ApplicationException($"Invalid combat configuration for magic power {Id} ({Name}).");
	}

	private void SaveCombatDefinition(XElement root)
	{
		root.Add(new XElement("CombatVersion", 1), new XElement("AttackRange", AttackRange), new XElement("RangeInRooms", RangeInRooms),
			new XElement("AttackSpell", _attackSpellId), new XElement("AttackSpellPower", AttackSpellPower),
			new XElement("DealsDamage", DealsDamage), new XElement("AttackEmote", new XCData(AttackEmote)),
			new XElement("AttackEffects", _attackEffects.Select(x => new XElement("Effect",
				new XAttribute("type", x.Type), new XElement("Resistance", x.Resistance),
				new XElement("Strength", x.Strength), new XElement("DurationSeconds", x.DurationSeconds),
				new XElement("SuccessEmote", new XCData(x.SuccessEmote)),
				new XElement("ResistEmote", new XCData(x.ResistEmote))))));
	}

	private bool CanAttackTarget(ICharacter actor, ICharacter? target)
	{
		if (!HasValidAttackSpell) return false;
		if (target is null || target == actor || !actor.Powers.Contains(this) || !actor.State.IsAble() ||
		    target.State.HasFlag(CharacterState.Dead) || actor.Movement is not null || actor.IsBlocked("general").Truth ||
		    !actor.CanSpendStamina(StaminaCost) || !actor.CanSee(target)) return false;
		if (WeaponAttack.RequiredPositionStates.Any() && !WeaponAttack.RequiredPositionStates.Contains(actor.PositionState)) return false;
		if (!DealsDamage && AttackSpell is null && !_attackEffects.Any(x => MagicAttackEffectResolver.IsApplicable(actor, target, x.Type))) return false;
		if (AttackRange == MagicAttackRange.Ranged)
			return NaturalRangedAttackMoveBase.TargetIsInRange(actor, target, RangeInRooms) &&
			       VehicleCombatService.Instance.CanCrossVehicleBoundary(actor, target, true, false, out _);
		return actor.ColocatedWith(target) &&
		       (actor.Combat is null || actor.MeleeRange || _attackEffects.Any(x => x.Type == MagicAttackEffectType.Pull)) &&
		       VehicleCombatService.Instance.CanCrossVehicleBoundary(actor, target, false, false, out _);
	}

	public ICombatMove CreateMove(ICharacter actor, ICharacter target) => AttackRange == MagicAttackRange.Ranged
		? new RangedMagicPowerAttackMove(actor, target, this)
		: new MagicPowerAttackMove(actor, target, this);

	private void UseCombatCommand(ICharacter actor, StringStack command)
	{
		var target = command.IsFinished ? actor.CombatTarget as ICharacter :
			actor.Location.Characters.Concat(actor.SeenTargets.OfType<ICharacter>()).Distinct()
				.Where(x => x != actor && actor.CanSee(x)).GetFromItemListByKeyword(command.SafeRemainingArgument, actor);
		if (target is null || !CanInvokePower(actor, target))
		{
			actor.Send("You cannot attack that target with this power at present.");
			return;
		}
		if (actor.Combat is null || actor.Combat != target.Combat)
		{
			if (!actor.CanEngage(target)) { actor.Send(actor.WhyCannotEngage(target)); return; }
			actor.Engage(target, AttackRange == MagicAttackRange.Melee);
		}
		actor.RemoveAllEffects<ISelectedCombatAction>();
		actor.AddEffect(SelectedCombatAction.GetEffectMagicAttack(actor, target, this));
		actor.Send($"You prepare to use {Name.ColourName()} against {target.HowSeen(actor)}.");
	}

	private static bool IsCombatBuildingCommand(string text) => text.ToLowerInvariant() is "range" or "damage" or "rider" or "attackemote" or "spell" or "spellpower";
	private bool BuildingCommandCombat(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "spell": return BuildingCommandAttackSpell(actor, command);
			case "spellpower":
				if (!Enum.TryParse<SpellPower>(command.SafeRemainingArgument, true, out var spellPower) || !Enum.IsDefined(spellPower))
				{ actor.Send("Choose a valid spell power level."); return false; }
				AttackSpellPower = spellPower; Changed = true; actor.Send($"The attack payload uses {spellPower.DescribeEnum().ColourValue()} power."); return true;
			case "range":
				if (!Enum.TryParse<MagicAttackRange>(command.PopSpeech(), true, out var range) || !Enum.IsDefined(range) ||
				    (range == MagicAttackRange.Ranged && (!int.TryParse(command.SafeRemainingArgument, out var _) || int.Parse(command.SafeRemainingArgument) < 0)))
				{ actor.Send("Use range melee or range ranged <rooms>."); return false; }
				AttackRange = range;
				RangeInRooms = range == MagicAttackRange.Ranged ? int.Parse(command.SafeRemainingArgument) : 0;
				break;
			case "damage":
				if (!bool.TryParse(command.SafeRemainingArgument, out var damage)) { actor.Send("Use damage true or false."); return false; }
				DealsDamage = damage;
				break;
			case "attackemote":
				var emote = new Emote(command.SafeRemainingArgument, actor, actor, actor);
				if (!emote.Valid) { actor.Send(emote.ErrorMessage); return false; }
				AttackEmote = command.SafeRemainingArgument;
				break;
			case "rider":
				if (!Enum.TryParse<MagicAttackEffectType>(command.PopSpeech(), true, out var type) || !Enum.IsDefined(type))
				{ actor.Send("Choose BreakClinch, Disarm, Stagger, Knockdown, Pushback or Pull."); return false; }
				var option = command.PopSpeech();
				if (option.EqualTo("remove")) { _attackEffects.RemoveAll(x => x.Type == type); break; }
				var previous = _attackEffects.FirstOrDefault(x => x.Type == type);
				if (option.EqualTo("success") || option.EqualTo("resist"))
				{
					if (previous is null) { actor.Send("Configure that rider first."); return false; }
					var echo = new Emote(command.SafeRemainingArgument, actor, actor, actor);
					if (!echo.Valid) { actor.Send(echo.ErrorMessage); return false; }
					_attackEffects.Remove(previous);
					_attackEffects.Add(option.EqualTo("success") ? previous with { SuccessEmote = command.SafeRemainingArgument } : previous with { ResistEmote = command.SafeRemainingArgument });
					break;
				}
				if (!Enum.TryParse<Difficulty>(option, true, out var difficulty) || !Enum.IsDefined(difficulty) ||
				    !double.TryParse(command.PopSpeech(), out var strength) || !double.IsFinite(strength) || strength <= 0 ||
				    !double.TryParse(command.PopSpeech(), out var seconds) || !double.IsFinite(seconds) || seconds < 0)
				{ actor.Send("Use rider <type> <resistance difficulty> <strength> <seconds>, remove, success <emote>, or resist <emote>."); return false; }
				if (_attackEffects.Any(x => (type == MagicAttackEffectType.Pull && x.Type == MagicAttackEffectType.Pushback) || (type == MagicAttackEffectType.Pushback && x.Type == MagicAttackEffectType.Pull)))
				{ actor.Send("Pull and pushback cannot be combined."); return false; }
				_attackEffects.RemoveAll(x => x.Type == type);
				_attackEffects.Add(new(type, difficulty, strength, seconds, previous?.SuccessEmote ?? "@ overwhelm|overwhelms $1 with force.", previous?.ResistEmote ?? "$1 resist|resists the force of $0's attack."));
				break;
		}
		Changed = true;
		actor.Send("The power's combat configuration has been updated.");
		return true;
	}
}
