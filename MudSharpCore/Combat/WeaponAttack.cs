using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using Newtonsoft.Json;

namespace MudSharp.Combat;

public class WeaponAttack : CombatAction, IWeaponAttack
{
	protected WeaponAttack(MudSharp.Models.WeaponAttack attack, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromDatabase(attack);
	}

	protected virtual void LoadFromDatabase(MudSharp.Models.WeaponAttack dbitem)
	{
		Id = dbitem.Id;
		_name = dbitem.Name;
		Verb = (MeleeWeaponVerb)dbitem.Verb;
		UsabilityProg = Gameworld.FutureProgs.Get(dbitem.FutureProgId ?? 0);
		Profile = new SimpleDamageProfile(dbitem, Gameworld);
		MoveType = (BuiltInCombatMoveType)dbitem.MoveType;
		Intentions = (CombatMoveIntentions)dbitem.Intentions;
		RecoveryDifficultySuccess = (Difficulty)dbitem.RecoveryDifficultySuccess;
		RecoveryDifficultyFailure = (Difficulty)dbitem.RecoveryDifficultyFailure;
		StaminaCost = dbitem.StaminaCost;
		BaseDelay = dbitem.BaseDelay;
		Weighting = dbitem.Weighting;
		BodypartShape = Gameworld.BodypartShapes.Get(dbitem.BodypartShapeId ?? 0);
		ExertionLevel = (ExertionLevel)dbitem.ExertionLevel;
		Orientation = (Orientation)dbitem.Orientation;
		Alignment = (Alignment)dbitem.Alignment;
		HandednessOptions = (AttackHandednessOptions)dbitem.HandednessOptions;
		_requiredPositionStates.AddRange(dbitem.RequiredPositionStateIds.Split(' ').Select(x => long.Parse(x))
		                                       .Select(x => PositionState.GetState(x)));
	}

	protected WeaponAttack(IFuturemud gameworld, BuiltInCombatMoveType type)
	{
		Gameworld = gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.WeaponAttack
			{
				Alignment = (int)Alignment.FrontRight,
				BaseAngleOfIncidence = Math.PI * 0.5,
				Orientation = (int)Orientation.Centre,
				BaseAttackerDifficulty = (int)Difficulty.Normal,
				BaseBlockDifficulty = (int)Difficulty.Normal,
				BaseParryDifficulty = (int)Difficulty.Normal,
				BaseDodgeDifficulty = (int)Difficulty.Normal,
				BaseDelay = 1.0,
				DamageType = (int)DamageType.Crushing,
				MoveType = (int)type,
				StaminaCost = 1.0,
				Verb = (int)MeleeWeaponVerb.Bash,
				Name = "New Attack",
				Weighting = 100,
				HandednessOptions = (int)AttackHandednessOptions.Any,
				RecoveryDifficultyFailure = (int)Difficulty.Hard,
				RecoveryDifficultySuccess = (int)Difficulty.Normal,
				Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
				                    CombatMoveIntentions.Kill),
				PainExpressionId = Gameworld.GetStaticLong("DefaultWeaponAttackPainExpressionId"),
				StunExpressionId = Gameworld.GetStaticLong("DefaultWeaponAttackStunExpressionId"),
				DamageExpressionId = Gameworld.GetStaticLong("DefaultWeaponAttackDamageExpressionId"),
				RequiredPositionStateIds =
					$"{PositionStanding.Instance.Id} {PositionFlying.Instance.Id} {PositionFloatingInWater.Instance.Id} {PositionSwimming.Instance.Id}"
			};
			FMDB.Context.WeaponAttacks.Add(dbitem);
			SeedInitialData(dbitem);
			FMDB.Context.SaveChanges();
			LoadFromDatabase(dbitem);
		}
	}

	protected virtual void SeedInitialData(MudSharp.Models.WeaponAttack attack)
	{
		// Do nothing
	}

	#region Overrides of Item

	public override string FrameworkItemType { get; } = "WeaponAttack";

	#endregion
	public override string ActionTypeName => "weapon attack";
	public virtual bool UsableAttack(IPerceiver attacker, IGameItem weapon, IPerceiver target,
		AttackHandednessOptions handedness,
		bool ignorePosition,
		params BuiltInCombatMoveType[] types)
	{
		return types.Contains(MoveType) &&
		       (handedness == AttackHandednessOptions.Any || HandednessOptions == AttackHandednessOptions.Any ||
		        HandednessOptions == handedness) &&
		       Intentions.HasFlag((attacker as ICharacter)?.CombatSettings.RequiredIntentions ??
		                          CombatMoveIntentions.None) &&
		       (Intentions & (attacker as ICharacter)?.CombatSettings.ForbiddenIntentions ??
		        CombatMoveIntentions.None) == 0 &&
		       (ignorePosition || RequiredPositionStates.Contains(attacker.PositionState)) &&
		       ((bool?)UsabilityProg?.Execute(attacker, weapon, target) ?? true);
	}

	public MeleeWeaponVerb Verb { get; set; }
	public IBodypartShape BodypartShape { get; set; }
	public IDamageProfile Profile { get; set; }
	public Orientation Orientation { get; set; }
	public Alignment Alignment { get; set; }
	public AttackHandednessOptions HandednessOptions { get; set; }

	public T GetAttackType<T>() where T : class, IWeaponAttack
	{
		return this as T;
	}

	public bool IsAttackType<T>() where T : class, IWeaponAttack
	{
		return this is T;
	}

	public static IWeaponAttack LoadWeaponAttack(MudSharp.Models.WeaponAttack attack, IFuturemud gameworld)
	{
		switch ((BuiltInCombatMoveType)attack.MoveType)
		{
			case BuiltInCombatMoveType.CoupDeGrace:
			case BuiltInCombatMoveType.ScreechAttack:
				return new FixedBodypartWeaponAttack(attack, gameworld);
			case BuiltInCombatMoveType.StaggeringBlow:
			case BuiltInCombatMoveType.StaggeringBlowUnarmed:
			case BuiltInCombatMoveType.UnbalancingBlow:
			case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
			case BuiltInCombatMoveType.StaggeringBlowClinch:
			case BuiltInCombatMoveType.UnbalancingBlowClinch:
			case BuiltInCombatMoveType.DownedAttack:
			case BuiltInCombatMoveType.DownedAttackUnarmed:
				return new SecondaryDifficultyWeaponAttack(attack, gameworld);
			case BuiltInCombatMoveType.ExtendGrapple:
			case BuiltInCombatMoveType.WrenchAttack:
				return new TargetLimbWeaponAttack(attack, gameworld);
			case BuiltInCombatMoveType.EnvenomingAttack:
			case BuiltInCombatMoveType.EnvenomingAttackClinch:
				return new EnvenomingAttack(attack, gameworld);
			default:
				return new WeaponAttack(attack, gameworld);
		}
	}

	public static IWeaponAttack NewWeaponAttack(BuiltInCombatMoveType type, IFuturemud gameworld)
	{
		switch (type)
		{
			case BuiltInCombatMoveType.CoupDeGrace:
			case BuiltInCombatMoveType.ScreechAttack:
				return new FixedBodypartWeaponAttack(gameworld, type);
			case BuiltInCombatMoveType.StaggeringBlow:
			case BuiltInCombatMoveType.StaggeringBlowUnarmed:
			case BuiltInCombatMoveType.StaggeringBlowClinch:
			case BuiltInCombatMoveType.UnbalancingBlowClinch:
			case BuiltInCombatMoveType.UnbalancingBlow:
			case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
			case BuiltInCombatMoveType.DownedAttack:
			case BuiltInCombatMoveType.DownedAttackUnarmed:
				return new SecondaryDifficultyWeaponAttack(gameworld, type);
			case BuiltInCombatMoveType.ExtendGrapple:
			case BuiltInCombatMoveType.WrenchAttack:
				return new TargetLimbWeaponAttack(gameworld, type);
			case BuiltInCombatMoveType.EnvenomingAttack:
			case BuiltInCombatMoveType.EnvenomingAttackClinch:
				return new EnvenomingAttack(gameworld, type);
			default:
				return new WeaponAttack(gameworld, type);
		}
	}

	#region Implementation of IKeyworded

	public override IEnumerable<string> Keywords => new[] { Name, Verb.Describe() };

	#endregion

	#region Building Commands

	public override void Save()
	{
		var dbitem = FMDB.Context.WeaponAttacks.Find(Id);
		dbitem.Alignment = (int)Alignment;
		dbitem.BaseDelay = BaseDelay;
		dbitem.BaseAngleOfIncidence = Profile.BaseAngleOfIncidence;
		dbitem.BaseAttackerDifficulty = (int)Profile.BaseAttackerDifficulty;
		dbitem.BaseBlockDifficulty = (int)Profile.BaseBlockDifficulty;
		dbitem.BaseDodgeDifficulty = (int)Profile.BaseDodgeDifficulty;
		dbitem.BaseParryDifficulty = (int)Profile.BaseParryDifficulty;
		dbitem.BodypartShapeId = BodypartShape?.Id;
		dbitem.DamageType = (int)Profile.DamageType;
		dbitem.ExertionLevel = (int)ExertionLevel;
		dbitem.FutureProgId = UsabilityProg?.Id;
		dbitem.HandednessOptions = (int)HandednessOptions;
		dbitem.Intentions = (long)Intentions;
		dbitem.MoveType = (int)MoveType;
		dbitem.Name = Name;
		dbitem.Orientation = (int)Orientation;
		dbitem.RecoveryDifficultyFailure = (int)RecoveryDifficultyFailure;
		dbitem.RecoveryDifficultySuccess = (int)RecoveryDifficultySuccess;
		dbitem.StaminaCost = StaminaCost;
		dbitem.Verb = (int)Verb;
		dbitem.DamageExpressionId = Profile.DamageExpression.Id;
		dbitem.PainExpressionId = Profile.PainExpression.Id;
		dbitem.StunExpressionId = Profile.StunExpression.Id;
		var weaponType = Gameworld.WeaponTypes.FirstOrDefault(x => x.Attacks.Contains(this));
		dbitem.WeaponTypeId = weaponType?.Id;
		dbitem.Weighting = Weighting;
		dbitem.RequiredPositionStateIds =
			_requiredPositionStates.Select(x => x.Id.ToString("F0")).ListToCommaSeparatedValues(" ");
		SaveAttackSpecificData(dbitem);
		Changed = false;
	}

	protected virtual void SaveAttackSpecificData(MudSharp.Models.WeaponAttack attack)
	{
	}

	public override string HelpText => @"The following options are available for this building command:

	#3name#0 - the name of the attack
	#3weapon <type|none>#0 - set or clear the weapon type this attack belongs to
	#3delay <number>#0 - the number of seconds delay after using this attack
	#3weight <number>#0 - the relative weighting of the engine selecting this attack
	#3verb <verb>#0 - the general verb of the attack
	#3handedness <any|1h|2h|dual|shield>#0 - sets the handedness required
	#3alignment <alignment>#0 - the alignment of attacks from a right-handed individual to someone face to face
	#3orientation <orientation>#0 the orientation of the attack from two standing opponents
	#3difficulty <difficulty>#0 - the difficulty of the attack roll
	#3dodge <difficulty>#0 - the difficulty of a dodge roll
	#3parry <difficulty>#0 - the difficulty of a parry roll
	#3block <difficulty>#0 - the difficulty of a block roll
	#3exertion <exertion>#0 - the minimum exertion level to set (if not already higher) when the attack is used
	#3recover <difficulty pass> <difficulty fail>#0 - the difficulty of a check made after pass/fail that slightly alters the delay of the attack
	#3stamina <amount>#0 - the base stamina cost of the attack
	#3damagetype <damage type>#0 - sets the damage type to the specified type
	#3verb <verb>#0 - sets the attack verb (used in messaging)
	#3angle <angle in degrees>#0 - sets the base angle the attack hits at, which can affect armour. Use 90 degrees if you don't know what you're doing.
	#3prog <prog>#0 - a prog taking character, item, character as parameters and returning a boolean, to determine whether this attack can be used
	#3bodypart <part shape>#0 - sets a bodypart shape this attack is typically associated with, e.g. hand, teeth, etc
	#3intention <intention1> [<intention2>...<intentionn>]#0 - toggles the specified attack intentions
	#3damage <name|id>#0 - sets a nominated expression as the damage expression
	#3stun <name|id>#0 - sets a nominated expression as the stun expression
	#3pain <name|id>#0 - sets a nominated expression as the pain expression
	#3position <name>#0 - toggles a particular position (standing, swimming, etc) required to use this attack";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "weapon":
				return BuildingCommandWeapon(actor, command);
			case "alignment":
				return BuildingCommandAlignment(actor, command);
			case "orientation":
				return BuildingCommandOrientation(actor, command);
			case "difficulty":
				return BuildingCommandDifficulty(actor, command);
			case "dodge":
				return BuildingCommandDodge(actor, command);
			case "block":
				return BuildingCommandBlock(actor, command);
			case "parry":
				return BuildingCommandParry(actor, command);
			case "damagetype":
				return BuildingCommandDamagetype(actor, command);
			case "verb":
				return BuildingCommandVerb(actor, command);
			case "angle":
				return BuildingCommandAngle(actor, command);
			case "handedness":
			case "hand":
				return BuildingCommandHandedness(actor, command);
			case "bodypart":
				return BuildingCommandBodypart(actor, command);
			case "damage":
				return BuildingCommandDamage(actor, command);
			case "pain":
				return BuildingCommandPain(actor, command);
			case "stun":
				return BuildingCommandStun(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandStun(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait expression do you want this weapon attack to use for stun?");
			return false;
		}

		var expression = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.TraitExpressions.Get(value)
			: Gameworld.TraitExpressions.GetByName(command.Last);
		if (expression == null)
		{
			actor.OutputHandler.Send("There is no such expression.");
			return false;
		}

		Profile.StunExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack now uses trait expression #{expression.Id.ToString("N0", actor)} for stun:  {expression.ToString()}");
		return true;
	}

	private bool BuildingCommandPain(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait expression do you want this weapon attack to use for pain?");
			return false;
		}

		var expression = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.TraitExpressions.Get(value)
			: Gameworld.TraitExpressions.GetByName(command.Last);
		if (expression == null)
		{
			actor.OutputHandler.Send("There is no such expression.");
			return false;
		}

		Profile.PainExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack now uses trait expression #{expression.Id.ToString("N0", actor)} for pain:  {expression.ToString()}");
		return true;
	}

	private bool BuildingCommandDamage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trait expression do you want this weapon attack to use for damage?");
			return false;
		}

		var expression = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.TraitExpressions.Get(value)
			: Gameworld.TraitExpressions.GetByName(command.Last);
		if (expression == null)
		{
			actor.OutputHandler.Send("There is no such expression.");
			return false;
		}

		Profile.DamageExpression = expression;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack now uses trait expression #{expression.Id.ToString("N0", actor)} for damage:  {expression.ToString()}");
		return true;
	}

	private bool BuildingCommandBodypart(ICharacter actor, StringStack command)
	{
		if (Gameworld.WeaponTypes.Any(x => x.Attacks.Contains(this)))
		{
			actor.OutputHandler.Send(
				"This option can't be used while this weapon attack is associated with a weapon type.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart shape should this attack be associated with?");
			return false;
		}

		var shape = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.BodypartShapes.Get(value)
			: Gameworld.BodypartShapes.GetByName(command.Last);
		if (shape == null)
		{
			actor.OutputHandler.Send("There is no such bodypart shape.");
			return false;
		}

		BodypartShape = shape;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack is now associated with the {shape.Name.Colour(Telnet.Green)} bodypart shape.");
		return true;
	}

	private bool BuildingCommandHandedness(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify any, 1h, 2h, dual or shield as an option for this command.");
			return false;
		}

		AttackHandednessOptions option;
		string description;
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "any":
			case "all":
				option = AttackHandednessOptions.Any;
				description = "always";
				break;
			case "1h":
				option = AttackHandednessOptions.OneHandedOnly;
				description = "one handed only";
				break;
			case "2h":
				option = AttackHandednessOptions.TwoHandedOnly;
				description = "two handed only";
				break;
			case "dual":
				option = AttackHandednessOptions.DualWieldOnly;
				description = "only when dual wielding";
				break;
			case "shield":
				option = AttackHandednessOptions.SwordAndBoardOnly;
				description = "only when also wielding a shield";
				break;
			default:
				actor.OutputHandler.Send(
					"You must specify any, 1h, 2h, dual or shield as an option for this command.");
				return false;
		}

		HandednessOptions = option;
		Changed = true;
		actor.OutputHandler.Send($"This attack will now be able to used {description}.");
		return true;
	}

	private bool BuildingCommandAngle(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What angle in degrees should the attack come in at as a base amount. Some armour types may handle acute/oblique attacks differently.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("That is not a valid number.");
			return false;
		}

		if (value > 90.0 || value <= 0.0)
		{
			actor.OutputHandler.Send("The angle must be between 0 (exclusive) and 90 (inclusive) degrees.");
			return false;
		}

		Profile.BaseAngleOfIncidence = value.DegreesToRadians();
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack's base angle of incidence is now {$"{value.ToString("N3", actor)}°".Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandVerb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What verb should this attack use to describe its action? The valid types are: {Enum.GetNames(typeof(MeleeWeaponVerb)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
			return false;
		}

		if (!Utilities.TryParseEnum<MeleeWeaponVerb>(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send(
				$"That is not a valid verb for melee attacks. The valid types are: {Enum.GetNames(typeof(MeleeWeaponVerb)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
			return false;
		}

		Verb = value;
		Changed = true;
		actor.OutputHandler.Send($"This attack now uses the {Verb.Describe().Colour(Telnet.Green)} verb.");
		return true;
	}

	private bool BuildingCommandDamagetype(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What damage type should this weapon attack do? The valid types are: {Enum.GetNames(typeof(DamageType)).Select(x => x.Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		if (!WoundExtensions.TryGetDamageType(command.PopSpeech(), out var damage))
		{
			actor.OutputHandler.Send(
				$"That is not a valid damage type. The valid types are: {Enum.GetNames(typeof(DamageType)).Select(x => x.Colour(Telnet.Green)).ListToString()}.");
			return false;
		}

		Profile.DamageType = damage;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack now does damage of type {Profile.DamageType.Describe().Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandParry(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How difficult should this attack be to parry? See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send("You must enter a valid difficulty. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		Profile.BaseParryDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack is now rated at {difficulty.Describe().Colour(Telnet.Green)} difficulty to parry.");
		return true;
	}

	private bool BuildingCommandBlock(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How difficult should this attack be to block? See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send("You must enter a valid difficulty. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		Profile.BaseBlockDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack is now rated at {difficulty.Describe().Colour(Telnet.Green)} difficulty to block.");
		return true;
	}

	private bool BuildingCommandDodge(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How difficult should this attack be to dodge? See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send("You must enter a valid difficulty. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		Profile.BaseDodgeDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack is now rated at {difficulty.Describe().Colour(Telnet.Green)} difficulty to dodge.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How difficult should this attack be? See SHOW DIFFICULTIES for a list.");
			return false;
		}

		if (!CheckExtensions.GetDifficulty(command.SafeRemainingArgument, out var difficulty))
		{
			actor.OutputHandler.Send("You must enter a valid difficulty. See SHOW DIFFICULTIES for a list.");
			return false;
		}

		Profile.BaseAttackerDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack is now rated at {difficulty.Describe().Colour(Telnet.Green)} difficulty.");
		return true;
	}

	private bool BuildingCommandOrientation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What orientation on the target should this attack come from when being used from the right hand towards two entities facing each other?\nValid values are: {Enum.GetNames(typeof(Orientation)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
			return false;
		}

		if (!Utilities.TryParseEnum<Orientation>(command.PopSpeech(), out var orientation))
		{
			actor.OutputHandler.Send(
				$"What alignment on the target should this attack come from when being used from the right hand towards two entities facing each other?\nValid values are: {Enum.GetNames(typeof(Orientation)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
			return false;
		}

		if (orientation == Orientation.Irrelevant)
		{
			actor.OutputHandler.Send("Irrelevant is not a valid orientation to select in this context.");
			return false;
		}

		Orientation = orientation;
		actor.OutputHandler.Send(
			$"This attack will now hit the {Orientation.Describe().Colour(Telnet.Cyan)} orientation of its target when used from the right hand of the attacker to an opponent facing them.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandAlignment(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What alignment on the target should this attack come from when being used from the right hand towards two entities facing each other?\nValid values are: {Enum.GetNames(typeof(Alignment)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
			return false;
		}

		if (!Utilities.TryParseEnum<Alignment>(command.PopSpeech(), out var alignment))
		{
			actor.OutputHandler.Send(
				$"What alignment on the target should this attack come from when being used from the right hand towards two entities facing each other?\nValid values are: {Enum.GetNames(typeof(Alignment)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
			return false;
		}

		if (alignment == Alignment.Irrelevant)
		{
			actor.OutputHandler.Send("Irrelevant is not a valid alignment to select in this context.");
			return false;
		}

		Alignment = alignment;
		actor.OutputHandler.Send(
			$"This attack will now hit the {Alignment.Describe().Colour(Telnet.Cyan)} alignment of its target when used from the right hand of the attacker to an opponent facing them.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandWeapon(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which weapon type should this attack belong to, or use NONE for an unarmed attack.");
			return false;
		}

		if (command.Peek().EqualTo("none"))
		{
			foreach (var type in Gameworld.WeaponTypes.Where(x => x.Attacks.Contains(this)))
			{
				type.RemoveAttack(this);
			}

			Changed = true;
			actor.OutputHandler.Send(
				"This attack now is not assigned to any weapon. Ensure that you update the relevant naturalattack entry.");
			return true;
		}

		var weapontype = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.WeaponTypes.Get(value)
			: Gameworld.WeaponTypes.GetByName(command.SafeRemainingArgument);
		if (weapontype == null)
		{
			actor.OutputHandler.Send("There is no such weapon type.");
			return false;
		}

		foreach (var type in Gameworld.WeaponTypes.Where(x => x.Attacks.Contains(this)))
		{
			type.RemoveAttack(this);
		}

		foreach (var race in Gameworld.Races)
		{
			race.RemoveNaturalAttacksAssociatedWith(this);
		}

		weapontype.AddAttack(this);
		Changed = true;
		actor.OutputHandler.Send(
			$"This attack is now assigned to the {weapontype.Name.Colour(Telnet.Cyan)} weapon type.");
		return true;
	}

	public string DescribeForAttacksCommand(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(Name.ColourName());
		switch (HandednessOptions)
		{
			case AttackHandednessOptions.Any:
				break;
			case AttackHandednessOptions.OneHandedOnly:
				sb.Append(" [1h-only]".Colour(Telnet.BoldWhite));
				break;
			case AttackHandednessOptions.TwoHandedOnly:
				sb.Append(" [2h-only]".Colour(Telnet.BoldWhite));
				break;
			case AttackHandednessOptions.DualWieldOnly:
				sb.Append(" [dual]".Colour(Telnet.BoldWhite));
				break;
			case AttackHandednessOptions.SwordAndBoardOnly:
				sb.Append(" [with shield]".Colour(Telnet.BoldWhite));
				break;
		}

		sb.Append(Telnet.Yellow.Colour);
		sb.Append(" (");
		sb.Append(Alignment.Describe());
		sb.Append(' ');
		sb.Append(Orientation.Describe());
		sb.Append(Telnet.RESET);

		sb.Append(") ");
		sb.Append(Intentions.DescribeBrief().ColourValue());
		if (Gameworld.GetStaticBool("UseSimpleAttackListing"))
		{
			return sb.ToString();
		}

		sb.Append(" - ");
		sb.Append(StaminaCost.ToString("N2", actor).ColourValue());
		sb.Append("st ");
		sb.Append((BaseDelay * Gameworld.GetStaticDouble("CombatSpeedMultiplier")).ToString("N1", actor).ColourValue());
		sb.Append("s ");
		sb.Append(Profile.BaseAttackerDifficulty.DescribeBrief(true));
		sb.Append(" vs b: ");
		sb.Append(Profile.BaseBlockDifficulty.DescribeBrief(true));
		sb.Append(" d: ");
		sb.Append(Profile.BaseDodgeDifficulty.DescribeBrief(true));
		sb.Append(" p: ");
		sb.Append(Profile.BaseParryDifficulty.DescribeBrief(true));

		DescribeForAttacksCommandInternal(sb, actor);
		return sb.ToString();
	}

	protected virtual void DescribeForAttacksCommandInternal(StringBuilder sb, ICharacter actor)
	{
		// Do nothing
	}

	public string ShowBuilder(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Weapon Attack {Id.ToString("N0", actor)} - {Name}");
		sb.AppendLine($"Weapon Type: {Gameworld.WeaponTypes.FirstOrDefault(x => x.Attacks.Contains(this))?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}");
		sb.AppendLine($"Move Type: {MoveType.Describe().Colour(Telnet.Green)}");
		sb.AppendLine(
			$"Position States: {RequiredPositionStates.Select(x => x.DescribeLocationMovementParticiple.TitleCase().ColourValue()).ListToCommaSeparatedValues(", ")}");
		
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Base Delay: {BaseDelay.ToString("N2", actor)}s".ColourValue(),
			$"Alignment: {Alignment.Describe().Colour(Telnet.Green)}",
			$"Orientation: {Orientation.Describe().Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Difficulty: {Profile.BaseAttackerDifficulty.Describe().Colour(Telnet.Green)}",
			$"Base Stamina: {StaminaCost.ToString("N3", actor).Colour(Telnet.Green)}",
			$"Angle: {$"{Profile.BaseAngleOfIncidence.RadiansToDegrees().ToString("N2", actor)}°".Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Dodge: {Profile.BaseDodgeDifficulty.Describe().Colour(Telnet.Green)}",
			$"Parry: {Profile.BaseParryDifficulty.Describe().Colour(Telnet.Green)}",
			$"Block: {Profile.BaseBlockDifficulty.Describe().Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Exertion: {ExertionLevel.Describe().Colour(Telnet.Green)}",
			$"Recover Failure: {RecoveryDifficultyFailure.Describe().Colour(Telnet.Green)}",
			$"Recover Success: {RecoveryDifficultySuccess.Describe().Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Handedness: {HandednessOptions.Describe().Colour(Telnet.Green)}",
			$"Verb: {Verb.Describe().Colour(Telnet.Green)}",
			$"Weighting: {Weighting.ToString("N2", actor).Colour(Telnet.Green)}"
		);
		sb.AppendLineColumns((uint)actor.LineFormatLength, 3,
			$"Damage Type: {Enum.GetName(typeof(DamageType), Profile.DamageType).SplitCamelCase().Colour(Telnet.Green)}",
			$"Usability Prog: {(UsabilityProg != null ? $"{UsabilityProg.FunctionName}".FluentTagMXP("send", $"href='show futureprog {UsabilityProg.Id}'") : "None".Colour(Telnet.Red))}",
			$"Bodypart: {BodypartShape?.Name.Colour(Telnet.Green) ?? "None".Colour(Telnet.Red)}"
		);
		sb.AppendLine($"Intentions: {Intentions.Describe()}");
		sb.AppendLine();
		sb.AppendLine(
			$"Damage Formula: [ID #{Profile.DamageExpression.Id}] {Profile.DamageExpression.Formula.OriginalExpression.Colour(Telnet.Yellow)}");
		sb.AppendLine(
			$"Pain Formula: [ID #{Profile.PainExpression.Id}] {Profile.PainExpression.Formula.OriginalExpression.Colour(Telnet.Yellow)}");
		sb.AppendLine(
			$"Stun Formula: [ID #{Profile.StunExpression.Id}] {Profile.StunExpression.Formula.OriginalExpression.Colour(Telnet.Yellow)}");
		sb.AppendLine(ShowBuilderInternal(actor));
		sb.AppendLine();
		sb.AppendLine("Combat Message Hierarchy:");
		var messages = Gameworld.CombatMessageManager.CombatMessages.Where(x => x.CouldApply(this))
		                        .OrderByDescending(x => x.Priority).ThenByDescending(x => x.Outcome ?? Outcome.None)
		                        .ThenBy(x => x.Prog != null).ToList();
		var i = 1;
		foreach (var message in messages)
		{
			sb.AppendLine(
				$"{i++.ToOrdinal()}) [#{message.Id.ToString("N0", actor)}] {message.Message.ColourCommand()} [{message.Chance.ToString("P3", actor).Colour(Telnet.Green)}]{(message.Outcome.HasValue ? $" [{message.Outcome.Value.DescribeColour()}]" : "")}{(message.Prog != null ? $" [{message.Prog.FunctionName} (#{message.Prog.Id})]".FluentTagMXP("send", $"href='show futureprog {message.Prog.Id}'") : "")}");
		}

		return sb.ToString();
	}

	protected virtual string ShowBuilderInternal(ICharacter actor)
	{
		return string.Empty;
	}

	public IWeaponAttack CloneWeaponAttack()
	{
		using (new FMDB())
		{
			var weaponType = Gameworld.WeaponTypes.FirstOrDefault(x => x.Attacks.Contains(this));
			var dbnew = new Models.WeaponAttack
			{
				Alignment = (int)Alignment,
				BaseAngleOfIncidence = Profile.BaseAngleOfIncidence,
				BaseAttackerDifficulty = (int)Profile.BaseAttackerDifficulty,
				BaseBlockDifficulty = (int)Profile.BaseBlockDifficulty,
				BaseDodgeDifficulty = (int)Profile.BaseDodgeDifficulty,
				BaseParryDifficulty = (int)Profile.BaseParryDifficulty,
				BaseDelay = BaseDelay,
				BodypartShapeId = BodypartShape?.Id,
				DamageType = (int)Profile.DamageType,
				ExertionLevel = (int)ExertionLevel,
				FutureProgId = UsabilityProg?.Id,
				HandednessOptions = (int)HandednessOptions,
				Intentions = (long)Intentions,
				MoveType = (int)MoveType,
				Name = base.Name,
				Orientation = (int)Orientation,
				RecoveryDifficultyFailure = (int)RecoveryDifficultyFailure,
				RecoveryDifficultySuccess = (int)RecoveryDifficultySuccess,
				StaminaCost = StaminaCost,
				Verb = (int)Verb,
				DamageExpressionId = Profile.DamageExpression.Id,
				PainExpressionId = Profile.PainExpression.Id,
				StunExpressionId = Profile.StunExpression.Id,
				WeaponTypeId = weaponType?.Id,
				Weighting = Weighting,
				RequiredPositionStateIds = _requiredPositionStates.Select(x => x.Id.ToString("F0"))
				                                                  .ListToCommaSeparatedValues(" ")
			};
			FMDB.Context.WeaponAttacks.Add(dbnew);
			AddAttackSpecificCloneData(dbnew);
			FMDB.Context.SaveChanges();
			var newAttack = LoadWeaponAttack(dbnew, Gameworld);
			weaponType?.AddAttack(newAttack);
			foreach (var message in Gameworld.CombatMessageManager.CombatMessages.Where(
				         x => x.WeaponAttackIds.Contains(Id)))
			{
				message.WeaponAttackIds.Add(newAttack.Id);
				message.Changed = true;
			}

			return newAttack;
		}
	}

	protected virtual void AddAttackSpecificCloneData(MudSharp.Models.WeaponAttack attack)
	{
	}

	#endregion
}