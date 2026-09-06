#nullable enable

using System.Globalization;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public sealed partial class MagicDefensePower : SustainedMagicPower, IMagicDefensePower
{
	public override string PowerType => "Magic Defense";
	public override string DatabaseType => "magicdefense";
	public string BeginVerb { get; private set; } = "defense";
	public string EndVerb { get; private set; } = "enddefense";
	public override IEnumerable<string> Verbs => [BeginVerb, EndVerb];
	public MagicDefenseMode DefenseMode { get; private set; }
	public MagicDefenseThreat Threats { get; private set; } = MagicDefenseThreat.Weapon | MagicDefenseThreat.Natural | MagicDefenseThreat.Ranged | MagicDefenseThreat.Magic;
	public ITraitDefinition DefenseTrait { get; private set; }
	public Difficulty DefenseDifficulty { get; private set; } = Difficulty.Normal;
	public double ReactionStamina { get; private set; }
	public int MaximumCharges { get; private set; } = 3;
	public double MaximumCapacity { get; private set; } = 30;
	public bool RequiresVision { get; private set; } = true;
	public bool RequiresAttackerVision { get; private set; }
	public bool RequiresUpright { get; private set; }
	public int FreeHands { get; private set; }
	public IFutureProg? EligibilityProg { get; private set; }
	public HashSet<DamageType> DamageTypes { get; } = [];
	public string BeginEmote { get; private set; } = "@ gather|gathers a protective veil of force.";
	public string EndEmote { get; private set; } = "$0's protective veil dissipates.";
	public string SuccessEmote { get; private set; } = "$0's protective force intercepts $1's attack.";
	public string FailEmote { get; private set; } = "$1's attack breaches $0's protection.";
	public string ReactionVerb => "reaction";

	public static void RegisterLoader()
	{
		MagicPowerFactory.RegisterLoader("magicdefense", (power, world) => new MagicDefensePower(power, world));
		MagicPowerFactory.RegisterBuilderLoader("magicdefense", (world, school, name, actor, command) =>
		{
			var trait = world.Traits.GetByIdOrName(command.SafeRemainingArgument);
			if (trait is null) { actor.Send("Which defense skill or attribute should this power use?"); return null; }
			return new MagicDefensePower(world, school, name, trait);
		});
	}

	private MagicDefensePower(IFuturemud world, IMagicSchool school, string name, ITraitDefinition trait) : base(world, school, name)
	{
		DefenseTrait = trait;
		ConcentrationPointsToSustain = 1;
		DetectableWithDetectMagic = Difficulty.Normal;
		Blurb = "Maintain a magical combat defense";
		_showHelpText = "Activate this power to make its defense available in combat; cancel it to stop its resource use.";
		DoDatabaseInsert();
	}

	public MagicDefensePower(Models.MagicPower model, IFuturemud world) : base(model, world)
	{
		var root = XElement.Parse(model.Definition);
		string Read(string name, string fallback) => root.Element(name)?.Value ?? fallback;
		BeginVerb = Read("BeginVerb", "defense"); EndVerb = Read("EndVerb", "enddefense");
		DefenseMode = Enum.Parse<MagicDefenseMode>(Read("DefenseMode", "Opposed"), true);
		Threats = Enum.Parse<MagicDefenseThreat>(Read("Threats", Threats.ToString()), true);
		DefenseTrait = world.Traits.Get(long.Parse(Read("DefenseTrait", "0"))) ?? throw new ApplicationException($"Invalid defense trait on power {Id}.");
		DefenseDifficulty = Enum.Parse<Difficulty>(Read("DefenseDifficulty", "Normal"), true);
		ReactionStamina = double.Parse(Read("ReactionStamina", "0"), CultureInfo.InvariantCulture);
		MaximumCharges = int.Parse(Read("MaximumCharges", "3"), CultureInfo.InvariantCulture);
		MaximumCapacity = double.Parse(Read("MaximumCapacity", "30"), CultureInfo.InvariantCulture);
		RequiresVision = bool.Parse(Read("RequiresVision", "true"));
		RequiresAttackerVision = bool.Parse(Read("RequiresAttackerVision", "false"));
		RequiresUpright = bool.Parse(Read("RequiresUpright", "false"));
		FreeHands = int.Parse(Read("FreeHands", "0"));
		EligibilityProg = world.FutureProgs.Get(long.Parse(Read("EligibilityProg", "0")));
		BeginEmote = Read("BeginEmote", BeginEmote); EndEmote = Read("EndEmote", EndEmote);
		SuccessEmote = Read("SuccessEmote", SuccessEmote); FailEmote = Read("FailEmote", FailEmote);
		foreach (var element in root.Element("DamageTypes")?.Elements("Type") ?? []) DamageTypes.Add(Enum.Parse<DamageType>(element.Value, true));
		if (!Enum.IsDefined(DefenseMode) || !Enum.IsDefined(DefenseDifficulty) || ((int)Threats & ~31) != 0 ||
		    !double.IsFinite(ReactionStamina) || ReactionStamina < 0 || MaximumCharges < 1 ||
		    !double.IsFinite(MaximumCapacity) || MaximumCapacity <= 0 || FreeHands < 0 || DamageTypes.Any(x => !Enum.IsDefined(x)))
			throw new ApplicationException($"Invalid defense configuration on power {Id}.");
	}

	protected override XElement SaveDefinition()
	{
		var root = new XElement("Definition", new XElement("BeginVerb", BeginVerb), new XElement("EndVerb", EndVerb),
			new XElement("DefenseMode", DefenseMode), new XElement("Threats", (int)Threats), new XElement("DefenseTrait", DefenseTrait.Id),
			new XElement("DefenseDifficulty", (int)DefenseDifficulty), new XElement("ReactionStamina", ReactionStamina),
			new XElement("MaximumCharges", MaximumCharges), new XElement("MaximumCapacity", MaximumCapacity),
			new XElement("RequiresVision", RequiresVision), new XElement("RequiresAttackerVision", RequiresAttackerVision),
			new XElement("RequiresUpright", RequiresUpright), new XElement("FreeHands", FreeHands), new XElement("EligibilityProg", EligibilityProg?.Id ?? 0),
			new XElement("DamageTypes", DamageTypes.Select(x => new XElement("Type", x))),
			new XElement("BeginEmote", new XCData(BeginEmote)), new XElement("EndEmote", new XCData(EndEmote)),
			new XElement("SuccessEmote", new XCData(SuccessEmote)), new XElement("FailEmote", new XCData(FailEmote)));
		AddBaseDefinition(root);
		SaveSustainedDefinition(root);
		return root;
	}

	public bool CanDefend(ICharacter defender, ICombatMove attack)
	{
		if (!defender.Powers.Contains(this) || !defender.State.IsAble() || defender.IsBlocked("general").Truth ||
		    !defender.Race.CombatSettings.CanDefend || !defender.CanSpendStamina(ReactionStamina) ||
		    !CanAffordToInvokePower(defender, ReactionVerb).Truth || (RequiresUpright && !defender.PositionState.Upright) ||
		    defender.Body.FunctioningFreeHands.Count() < FreeHands || (RequiresVision && !defender.CanSee(attack.Assailant)) ||
		    (RequiresAttackerVision && !attack.Assailant.CanSee(defender)) ||
		    CanInvokePowerProg?.ExecuteBool(defender, attack.Assailant) == false ||
		    EligibilityProg?.ExecuteBool(defender, attack.Assailant) == false) return false;
		if (attack is IMagicPowerAttackMove powerAttack && !powerAttack.AttackPower.ValidDefenseTypes.Contains(DefenseType.Magic)) return false;
		var threat = MagicDefenseMove.Classify(attack);
		if (threat == MagicDefenseThreat.None || (Threats & threat) != threat) return false;
		if (DefenseMode == MagicDefenseMode.Absorption && (threat.HasFlag(MagicDefenseThreat.Control) ||
		    attack is IMagicPowerAttackMove magic && !magic.AttackPower.DealsDamage)) return false;
		if (DamageTypes.Count == 0) return true;
		if (attack is IWeaponAttackMove weapon && weapon.Attack is not null) return DamageTypes.Contains(weapon.Attack.Profile.DamageType);
		if (attack is IRangedWeaponAttackMove ranged)
		{
			var types = ranged.Weapon.ProjectileDamageTypes.ToList();
			return types.Count > 0 && types.All(DamageTypes.Contains);
		}
		return false;
	}

	public void ConsumeReaction(ICharacter defender)
	{
		ConsumePowerCosts(defender, ReactionVerb);
		PsionicActivityNotifier.Notify(defender, this, $"defended with {Name}");
	}
	protected override void ExpireSustainedEffect(ICharacter actor) => actor.RemoveAllEffects<MagicDefense>(x => x.Power == this, true);
	public override void UseCommand(ICharacter actor, string verb, StringStack command)
	{
		if (verb.EqualTo(EndVerb)) { ExpireSustainedEffect(actor); return; }
		if (actor.EffectsOfType<MagicDefense>().Any(x => x.Power == this)) { actor.Send("You already maintain that defense."); return; }
		if (!actor.Powers.Contains(this) || !actor.State.IsAble() || actor.Movement is not null || actor.IsBlocked("general").Truth ||
		    !CanAffordToInvokePower(actor, BeginVerb).Truth || CanInvokePowerProg?.ExecuteBool(actor, actor) == false)
		{ actor.Send("You cannot activate that defense at present."); return; }
		ConsumePowerCosts(actor, BeginVerb);
		var outcome = Gameworld.GetCheck(CheckType.GenericSkillCheck).Check(actor, DefenseDifficulty, DefenseTrait);
		if (outcome.IsFail()) { actor.Send("You fail to establish your defense."); return; }
		actor.AddEffect(new MagicDefense(actor, this), GetDuration(outcome.SuccessDegrees()));
		PsionicActivityNotifier.Notify(actor, this, $"activated {Name}");
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(BeginEmote, actor, actor)));
	}

	protected override void ShowSubtype(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Mode: {DefenseMode.DescribeEnum()} | Threats: {Threats} | Skill: {DefenseTrait.Name} ({DefenseDifficulty.DescribeEnum()})");
		sb.AppendLine($"Charges: {MaximumCharges.ToString("N0", actor)} | Capacity: {MaximumCapacity.ToString("N2", actor)} | Reaction stamina: {ReactionStamina.ToString("N2", actor)}");
		sb.AppendLine($"Vision: {RequiresVision.ToColouredString()} | Attacker vision: {RequiresAttackerVision.ToColouredString()} | Upright: {RequiresUpright.ToColouredString()} | Free hands: {FreeHands.ToString("N0", actor)}");
		sb.AppendLine($"Eligibility: {EligibilityProg?.Name ?? "Always"} | Damage types: {(DamageTypes.Count == 0 ? "All" : DamageTypes.Select(x => x.DescribeEnum()).ListToString())}");
		sb.AppendLine($"Verbs: {BeginVerb}, {EndVerb}; reaction costs use the {ReactionVerb} cost entry.");
		sb.AppendLine($"Begin: {BeginEmote}\nEnd: {EndEmote}\nSuccess: {SuccessEmote}\nFailure: {FailEmote}");
	}
}
