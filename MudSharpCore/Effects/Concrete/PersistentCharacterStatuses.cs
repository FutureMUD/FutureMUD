#nullable enable

using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.Movement;

namespace MudSharp.Effects.Concrete;

/// <summary>Spell-independent perception changes, saved through the ordinary effect lifecycle.</summary>
public class PerceptionStatus : Effect
{
	public PerceptionStatus(IPerceivable owner, string key, PerceptionTypes granting, PerceptionTypes obscuring = PerceptionTypes.None) : base(owner)
	{
		Key = key; Granting = granting; HiddenTypes = obscuring;
	}
	protected PerceptionStatus(XElement root, IPerceivable owner) : base(root, owner)
	{
		var data = root.Element("Status")!;
		Key = data.Attribute("key")!.Value;
		Granting = (PerceptionTypes)(long)data.Attribute("granting")!;
		HiddenTypes = (PerceptionTypes)(long)data.Attribute("obscuring")!;
	}
	public string Key { get; }
	public PerceptionTypes Granting { get; }
	public PerceptionTypes HiddenTypes { get; }
	public override PerceptionTypes PerceptionGranting => Granting;
	public override PerceptionTypes Obscuring => HiddenTypes;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "PerceptionStatus";
	protected override XElement SaveDefinition() => new("Status", new XAttribute("key", Key),
		new XAttribute("granting", (long)Granting), new XAttribute("obscuring", (long)HiddenTypes));
	public override string Describe(IPerceiver voyeur) => $"{Key}: grants {Granting.DescribeEnum()}, obscures {HiddenTypes.DescribeEnum()}.";
	public static void InitialiseEffectType() => RegisterFactory("PerceptionStatus", (xml, owner) => new PerceptionStatus(xml, owner));
}

public sealed class InfravisionStatus : PerceptionStatus, IDarksightEffect
{
	public InfravisionStatus(IPerceivable owner) : base(owner, "infravision", PerceptionTypes.VisualInfrared) { }
	private InfravisionStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public Difficulty MinimumEffectiveDifficulty => Difficulty.Normal;
	protected override string SpecificEffectType => "InfravisionStatus";
	public new static void InitialiseEffectType() => RegisterFactory("InfravisionStatus", (xml, owner) => new InfravisionStatus(xml, owner));
}

public sealed class BlindnessStatus : Effect, IBlindnessEffect
{
	public BlindnessStatus(IPerceivable owner) : base(owner) { }
	private BlindnessStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "BlindnessStatus";
	public override string Describe(IPerceiver voyeur) => "Blind.";
	public static void InitialiseEffectType() => RegisterFactory("BlindnessStatus", (xml, owner) => new BlindnessStatus(xml, owner));
}

public sealed class SilenceStatus : Effect, ISilencedEffect
{
	public SilenceStatus(IPerceivable owner) : base(owner) { }
	private SilenceStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "SilenceStatus";
	public override string Describe(IPerceiver voyeur) => "Silenced.";
	public static void InitialiseEffectType() => RegisterFactory("SilenceStatus", (xml, owner) => new SilenceStatus(xml, owner));
}

public sealed class FlightStatus : Effect, IFlightEffect
{
	public FlightStatus(IPerceivable owner) : base(owner) { }
	private FlightStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "FlightStatus";
	public override string Describe(IPerceiver voyeur) => "Able to fly.";
	public override void RemovalEffect()
	{
		if (Owner is ICharacter character) character.CheckCanFly(this);
		base.RemovalEffect();
	}
	public static void InitialiseEffectType() => RegisterFactory("FlightStatus", (xml, owner) => new FlightStatus(xml, owner));
}

public sealed class PoisonSenseStatus : Effect, ISensePoisonEffect
{
	public PoisonSenseStatus(IPerceivable owner) : base(owner) { }
	private PoisonSenseStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "PoisonSenseStatus";
	public override string Describe(IPerceiver voyeur) => "Able to sense toxins through native inspection.";
	public static void InitialiseEffectType() => RegisterFactory("PoisonSenseStatus", (xml, owner) => new PoisonSenseStatus(xml, owner));
}

public sealed class ClimbingStatus : Effect, IClimbingEffect
{
	public ClimbingStatus(IPerceivable owner) : base(owner) { }
	private ClimbingStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "ClimbingStatus";
	public override string Describe(IPerceiver voyeur) => "Able to climb through native movement rules.";
	public static void InitialiseEffectType() => RegisterFactory("ClimbingStatus", (xml, owner) => new ClimbingStatus(xml, owner));
}

public sealed class PacifismStatus : Effect, IPacifismEffect
{
	public PacifismStatus(IPerceivable owner) : base(owner) { }
	private PacifismStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	public bool IsPeaceful => true;
	public bool IsSuperPeaceful => false;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "PacifismStatus";
	public override string Describe(IPerceiver voyeur) => "Peaceful; unwilling to initiate violence.";
	public static void InitialiseEffectType() => RegisterFactory("PacifismStatus", (xml, owner) => new PacifismStatus(xml, owner));
}

public sealed class MovementTempoStatus : Effect, IMovementDelayEffect
{
	public MovementTempoStatus(IPerceivable owner, double multiplier) : base(owner)
	{
		Multiplier = Validate(multiplier);
	}
	private MovementTempoStatus(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		Multiplier = Validate((double)xml.Element("Multiplier")!);
	}
	public double Multiplier { get; }
	public double DelayMultiplier(IMoveSpeed speed) => Multiplier;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "MovementTempoStatus";
	protected override XElement SaveDefinition() => new("Multiplier", Multiplier);
	public override string Describe(IPerceiver voyeur) => $"Movement time x{Multiplier.ToString("N2", voyeur)}.";
	public static void InitialiseEffectType() => RegisterFactory("MovementTempoStatus", (xml, owner) => new MovementTempoStatus(xml, owner));
	private static double Validate(double value) => double.IsFinite(value) && value is >= 0.1 and <= 10
		? value : throw new ArgumentOutOfRangeException(nameof(value), "Movement time multiplier must be within 0.1..10.");
}

public sealed class CombatTempoStatus : Effect, ICombatDelayMultiplierEffect
{
	public CombatTempoStatus(IPerceivable owner, double multiplier) : base(owner)
	{
		Multiplier = Validate(multiplier);
	}
	private CombatTempoStatus(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		Multiplier = Validate((double)xml.Element("Multiplier")!);
	}
	public double Multiplier { get; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "CombatTempoStatus";
	protected override XElement SaveDefinition() => new("Multiplier", Multiplier);
	public override string Describe(IPerceiver voyeur) => $"Combat recovery time x{Multiplier.ToString("N2", voyeur)}.";
	public static void InitialiseEffectType() => RegisterFactory("CombatTempoStatus", (xml, owner) => new CombatTempoStatus(xml, owner));
	private static double Validate(double value) => double.IsFinite(value) && value is >= 0.1 and <= 10
		? value : throw new ArgumentOutOfRangeException(nameof(value), "Combat recovery time multiplier must be within 0.1..10.");
}

public class SpottingAwarenessStatus : Effect, ICheckBonusEffect
{
	public SpottingAwarenessStatus(IPerceivable owner, double bonus) : base(owner)
	{
		CheckBonus = Validate(bonus);
	}
	protected SpottingAwarenessStatus(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		CheckBonus = Validate((double)xml.Element("Bonus")!);
	}
	public double CheckBonus { get; }
	protected virtual CheckType Check => CheckType.SpotStealthCheck;
	public bool AppliesToCheck(CheckType type) => type == Check && Applies() &&
		Owner is ICharacter actor && actor.State.IsConscious() && !actor.State.HasFlag(CharacterState.Sleeping) && actor.Combat is null;
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "SpottingAwarenessStatus";
	protected override XElement SaveDefinition() => new("Bonus", CheckBonus);
	public override string Describe(IPerceiver voyeur) => $"{Check.DescribeEnum()} awareness bonus {CheckBonus.ToString("N2", voyeur)}.";
	public static void InitialiseEffectType() => RegisterFactory("SpottingAwarenessStatus", (xml, owner) => new SpottingAwarenessStatus(xml, owner));
	private static double Validate(double bonus) => double.IsFinite(bonus) && bonus is >= -1000 and <= 1000
		? bonus : throw new ArgumentOutOfRangeException(nameof(bonus));
}

public sealed class ListeningAwarenessStatus : SpottingAwarenessStatus, IPassiveListeningEffect
{
	public ListeningAwarenessStatus(IPerceivable owner, double bonus) : base(owner, bonus) { }
	private ListeningAwarenessStatus(XElement xml, IPerceivable owner) : base(xml, owner) { }
	protected override CheckType Check => CheckType.GenericListenCheck;
	protected override string SpecificEffectType => "ListeningAwarenessStatus";
	public new static void InitialiseEffectType() => RegisterFactory("ListeningAwarenessStatus", (xml, owner) => new ListeningAwarenessStatus(xml, owner));
}
