using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public partial class MagicAttackPower : MagicPowerBase, IMagicAttackPower
{
    public override string PowerType => "Magic Attack";

    #region Overrides of MagicPowerBase

    /// <inheritdoc />
    public override string DatabaseType => "magicattack";

    #endregion

    public static void RegisterLoader()
    {
        MagicPowerFactory.RegisterLoader("magicattack", (power, gameworld) => new MagicAttackPower(power, gameworld));
        MagicPowerFactory.RegisterLoader("Magic Attack", (power, gameworld) => new MagicAttackPower(power, gameworld));
        MagicPowerFactory.RegisterBuilderLoader("magicattack", (gameworld, school, name, actor, command) =>
        {
            if (command.IsFinished)
            {
                actor.OutputHandler.Send("Which skill do you want to use for the skill check?");
                return null;
            }

            ITraitDefinition skill = gameworld.Traits.GetByIdOrName(command.PopSpeech());
            if (skill is null)
            {
                actor.OutputHandler.Send("There is no such skill or attribute.");
                return null;
            }

            if (command.IsFinished)
            {
                actor.OutputHandler.Send("Which weapon attack should be associated with this power?");
                return null;
            }

            IWeaponAttack wa = actor.Gameworld.WeaponAttacks.GetByIdOrName(command.SafeRemainingArgument);
            if (wa is null)
            {
                actor.OutputHandler.Send($"There is no such weapon attack identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
                return null;
            }

            return new MagicAttackPower(gameworld, school, name, skill, wa);
        });
    }

    protected override XElement SaveDefinition()
    {
        XElement definition = new("Definition",
                new XElement("Verb", _verb),
    new XElement("PowerIntentions", (long)PowerIntentions),
    new XElement("ValidDefenseTypes",
        from item in _validDefenseTypes
        select new XElement("Defense", (int)item)
    ),
    new XElement("WeaponAttack", WeaponAttack.Id),
    new XElement("AttackerTrait", AttackerTrait.Id),
    new XElement("Reach", Reach),
                new XElement("MoveType", (int)MoveType)
        );
        AddBaseDefinition(definition);
		SaveCombatDefinition(definition);
        return definition;
    }

    protected MagicAttackPower(IFuturemud gameworld, IMagicSchool school, string name, ITraitDefinition trait, IWeaponAttack wa) : base(gameworld, school, name)
    {
        Blurb = "Use a magical attack on others";
        _showHelpText = "Use this power's verb and a visible target to queue a combat attack. Automatic use follows your combat settings' magic or psychic channel.";
        _verb = "blast";
        _validDefenseTypes.AddRange([DefenseType.Block, DefenseType.Parry, DefenseType.Dodge, DefenseType.Magic]);
        PowerIntentions = CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill;
        AttackerTrait = trait;
        WeaponAttack = wa;
        Reach = 1;
        DoDatabaseInsert();
    }

    protected MagicAttackPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
    {
        XElement root = XElement.Parse(power.Definition);
		LoadCombatDefinition(root);
        XElement element = root.Element("Verb");
        if (element == null)
        {
            throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain a Verb element.");
        }

        _verb = element.Value;

        element = root.Element("PowerIntentions");
        if (element == null)
        {
            throw new ApplicationException(
                $"MagicAttackPower {Id} ({Name}) did not contain a PowerIntentions element.");
        }

        if (!long.TryParse(element.Value, out long value))
        {
            CombatMoveIntentions flag = CombatMoveIntentions.None;
            string[] split = element.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in split)
            {
                if (Utilities.TryParseEnum<CombatMoveIntentions>(item, out CombatMoveIntentions sub))
                {
                    flag |= sub;
                    continue;
                }

                throw new ApplicationException(
                    $"MagicAttackPower {Id} ({Name}) had an invalid CombatMoveIntention - {item}");
            }

            PowerIntentions = flag;
        }
        else
        {
            PowerIntentions = (CombatMoveIntentions)value;
        }

        element = root.Element("ValidDefenseTypes");
        if (element == null)
        {
            throw new ApplicationException(
                $"MagicAttackPower {Id} ({Name}) did not contain a ValidDefenseTypes element.");
        }

        foreach (XElement subelement in element.Elements())
        {
            if (!Utilities.TryParseEnum<DefenseType>(subelement.Value, out DefenseType defense))
            {
                throw new ApplicationException(
                    $"MagicAttackPower {Id} ({Name}) had an invalid DefenseType value - {subelement.Value}");
            }

            _validDefenseTypes.Add(defense);
        }

        if (root.Element("CombatVersion") is null && !_validDefenseTypes.Contains(DefenseType.Magic)) _validDefenseTypes.Add(DefenseType.Magic);
        element = root.Element("WeaponAttack");
        if (element == null)
        {
            throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain a WeaponAttack element.");
        }

        WeaponAttack = long.TryParse(element.Value, out value)
            ? Gameworld.WeaponAttacks.Get(value)
            : Gameworld.WeaponAttacks.GetByName(element.Value);
        if (WeaponAttack == null)
        {
            throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not have a valid WeaponAttack.");
        }

        element = root.Element("AttackerTrait");
        if (element == null)
        {
            throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not contain an AttackerTrait element.");
        }

        AttackerTrait = long.TryParse(element.Value, out value)
            ? Gameworld.Traits.Get(value)
            : Gameworld.Traits.GetByName(element.Value);
        if (AttackerTrait == null)
        {
            throw new ApplicationException(
                $"MagicAttackPower {Id} ({Name}) had an AttackerTrait that did not refer to a valid TraitDefinition.");
        }

        element = root.Element("Reach");
        Reach = int.TryParse(element?.Value ?? "0", out int reach) ? reach : 0;

        element = root.Element("MoveType");
        if (element == null)
        {
            throw new ApplicationException($"MagicAttackPower {Id} ({Name}) did not have a MoveType element.");
        }

        if (!Utilities.TryParseEnum<BuiltInCombatMoveType>(element.Value, out BuiltInCombatMoveType movetype))
        {
            throw new ApplicationException(
                $"MagicAttackPower {Id} ({Name}) had a MoveType that did not refer to a valid BuiltInCombatMoveType - {element.Value}");
        }
    }

    public override void UseCommand(ICharacter actor, string verb, StringStack command)
    {
		UseCombatCommand(actor, command);
    }

    public virtual bool CanInvokePower(ICharacter invoker, ICharacter target)
    {
		if (!CanAttackTarget(invoker, target)) return false;
        if (CanInvokePowerProg?.Execute<bool?>(invoker, target) == false)
        {
            return false;
        }

        if (!CanAffordToInvokePower(invoker, _verb).Truth)
        {
            return false;
        }

        if (WeaponAttack.UsabilityProg?.Execute<bool?>(invoker, null, target) == false)
        {
            return false;
        }

        return true;
    }

    public void UseAttackPower(IMagicPowerAttackMove move)
    {
        ConsumePowerCosts(move.Assailant, _verb);
		PsionicActivityNotifier.Notify(move.Assailant, this, $"used {Name}", move.CharacterTargets);
    }

    private string _verb;
    public override IEnumerable<string> Verbs => new[] { _verb };
    public CombatMoveIntentions PowerIntentions { get; private set; }

    private readonly List<DefenseType> _validDefenseTypes = new();
    public IEnumerable<DefenseType> ValidDefenseTypes => _validDefenseTypes;
    public ITraitDefinition AttackerTrait { get; private set; }
    public IWeaponAttack WeaponAttack { get; private set; }
    public int Reach { get; private set; }
    public virtual BuiltInCombatMoveType MoveType => WeaponAttack.MoveType;

    /// <inheritdoc />
    protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
    {
		sb.AppendLine($"Range: {AttackRange.DescribeEnum()}, {RangeInRooms.ToString("N0", actor)} rooms | Damage: {DealsDamage.ToColouredString()}");
		sb.AppendLine($"Attack echo: {AttackEmote}");
		sb.AppendLine($"Spell payload: {(AttackSpell?.Name ?? (_attackSpellId == 0 ? "None" : "Missing spell")).ColourName()} ({AttackSpellPower.DescribeEnum()})");
		foreach (var rider in AttackEffects) sb.AppendLine($"{rider.Type.DescribeEnum()}: {rider.Resistance.DescribeEnum()}, strength {rider.Strength.ToString("N2", actor)}, duration {rider.DurationSeconds.ToString("N2", actor)}s\n  Success: {rider.SuccessEmote}\n  Resisted: {rider.ResistEmote}");
        sb.AppendLine($"Power Verb: {_verb.ColourCommand()}");
        sb.AppendLine($"AttackerTrait Trait: {AttackerTrait.Name.ColourValue()}");
        sb.AppendLine($"Weapon Attack: {WeaponAttack.Name.MXPSend($"wa show {WeaponAttack.Id}", $"wa show {WeaponAttack.Id}")}");
        sb.AppendLine($"Move Type: {MoveType.DescribeEnum().ColourValue()}");
        sb.AppendLine($"Valid Defenses: {ValidDefenseTypes.Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
        sb.AppendLine($"Intentions: {PowerIntentions.GetSingleFlags().Select(x => x.Describe().ColourValue()).ListToString()}");
        sb.AppendLine($"Reach: {Reach.ToString("N0", actor)}");
    }

    public double BaseDelay => WeaponAttack.BaseDelay;
    public ExertionLevel ExertionLevel => WeaponAttack.ExertionLevel;
    public double StaminaCost => WeaponAttack.StaminaCost;
    public double Weighting => WeaponAttack.Weighting;
    public Orientation Orientation => WeaponAttack.Orientation;
    public Alignment Alignment => WeaponAttack.Alignment;
    public Difficulty BaseBlockDifficulty => WeaponAttack.Profile.BaseBlockDifficulty;
    public Difficulty BaseParryDifficulty => WeaponAttack.Profile.BaseParryDifficulty;
    public Difficulty BaseDodgeDifficulty => WeaponAttack.Profile.BaseDodgeDifficulty;

    #region Building Commands
    protected override string SubtypeHelpText => @"
	#3range melee|ranged <rooms>#0 - attack targeting mode
	#3damage <true|false>#0 - enable or disable wounds
	#3spell <spell|none>#0 - attach a matching attack-trigger spell, including caster effects
	#3spellpower <level>#0 - set the attached payload's power level
	#3attackemote <text>#0 - attack echo
	#3rider <BreakClinch|Disarm|Stagger|Knockdown|Pushback|Pull> <difficulty> <strength> <seconds>#0
	#3rider <type> remove|success <text>|resist <text>#0 - edit a rider
	#3verb <verb>#0 - sets the verb to manually activate this power
	#3attack <which>#0 - sets the weapon attack associated with this power
	#3reach <##>#0 - sets the reach of this attack
	#3trait <which>#0 - sets the attack trait used with this power
	#3intention <which>#0 - toggles an intention used with this power
	#3defense <which>#0 - toggles a defense type that is usable against this attack";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
		if (IsCombatBuildingCommand(command.PeekSpeech())) return BuildingCommandCombat(actor, command);
        switch (command.PopForSwitch())
        {
			case "verb":
				var verb = command.PopSpeech().ToLowerInvariant();
				if (string.IsNullOrWhiteSpace(verb) || verb.Any(char.IsWhiteSpace) || !command.IsFinished)
				{ actor.Send("Specify a single word for the attack verb."); return false; }
				if (verb.EqualTo(_verb)) return true;
				InvocationCosts[verb] = InvocationCosts[_verb].ToList();
				InvocationCosts.Remove(_verb);
				_verb = verb; Changed = true;
				actor.Send($"Use {verb.ColourCommand()} to invoke this attack. Its invocation costs are preserved.");
				return true;
            case "skill":
            case "trait":
                return BuildingCommandTrait(actor, command);
            case "reach":
                return BuildingCommandReach(actor, command);
            case "intention":
            case "intentions":
                return BuildingCommandIntentions(actor, command);
            case "attack":
                return BuildingCommandAttack(actor, command);
            case "defense":
            case "defence":
            case "defences":
            case "defenses":
                return BuildingCommandDefence(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandDefence(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which defense type do you want to toggle?\nThe valid types are {Enum.GetValues<DefenseType>().ListToColouredString()}.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<DefenseType>(out DefenseType value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid defense type.\nThe valid types are {Enum.GetValues<DefenseType>().ListToColouredString()}.");
            return false;
        }

        if (value == DefenseType.None)
        {
            _validDefenseTypes.Clear();
            Changed = true;
            actor.OutputHandler.Send("There will no longer be any defense against this attack.");
            return false;
        }

        if (_validDefenseTypes.Contains(value))
        {
            _validDefenseTypes.Remove(value);
            Changed = true;
            actor.OutputHandler.Send($"This attack can no longer be defended against by the {value.DescribeEnum().ColourValue()} defense type.");
            return true;
        }

        _validDefenseTypes.Add(value);
        Changed = true;
        actor.OutputHandler.Send($"This attack can now be defended against by the {value.DescribeEnum().ColourValue()} defense type.");
        return true;
    }

    private bool BuildingCommandAttack(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which weapon attack do you want this power to be associated with?");
            return false;
        }

        IWeaponAttack wa = Gameworld.WeaponAttacks.GetByIdOrName(command.SafeRemainingArgument);
        if (wa is null)
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid weapon attack.");
            return false;
        }

        if (!wa.MoveType.IsMagicAttack())
        {
            actor.OutputHandler.Send($"The weapon attack {wa.Name.ColourName()} (#{wa.Id.ToStringN0(actor)}) is not a valid magic attack.");
            return false;
        }

        WeaponAttack = wa;
        Changed = true;
        actor.OutputHandler.Send($"This power will now use the weapon attack {wa.Name.ColourName()} (#{wa.Id.ToStringN0(actor)}).");
        return true;
    }

    private bool BuildingCommandIntentions(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which combat move intention do you want to toggle?\nValid values are {Enum.GetValues<CombatMoveIntentions>().ListToColouredString()}.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<CombatMoveIntentions>(out CombatMoveIntentions value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid combat move intention.\nValid values are {Enum.GetValues<CombatMoveIntentions>().ListToColouredString()}.");
            return false;
        }

        if (PowerIntentions.HasFlag(value))
        {
            PowerIntentions &= ~value;
            Changed = true;
            actor.OutputHandler.Send($"This power no longer has the {value.DescribeEnum().ColourName()} intention.");
            return true;
        }

        PowerIntentions |= value;
        Changed = true;
        actor.OutputHandler.Send($"This power now has the {value.DescribeEnum().ColourName()} intention.");
        return true;
    }

    private bool BuildingCommandReach(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What should be the reach of this weapon?");
            return false;
        }

        if (!int.TryParse(command.SafeRemainingArgument, out int value) || value < 0)
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number zero or greater.");
            return false;
        }

        Reach = value;
        Changed = true;
        actor.OutputHandler.Send($"This attack now has a reach of {value.ToStringN0Colour(actor)}.");
        return true;
    }

    private bool BuildingCommandTrait(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which trait should be used for attack rolls with this power?");
            return false;
        }

        ITraitDefinition trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
        if (trait is null)
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid trait.");
            return false;
        }

        AttackerTrait = trait;
        Changed = true;
        actor.OutputHandler.Send($"This power will now use the trait {trait.Name.ColourValue()} for attack rolls.");
        return true;
    }

    #region Building SubCommands
    #endregion

    #endregion
}
