#nullable enable
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Health.Infections;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete.SpellEffects;

public abstract class SimpleSpellStatusEffectBase : MagicSpellEffectBase
{
	protected SimpleSpellStatusEffectBase(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	protected SimpleSpellStatusEffectBase(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	protected XElement SimpleSaveDefinition(params object[] additionalElements)
	{
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			additionalElements
		);
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition();
	}
}

public abstract class PerceptionGrantingSpellStatusEffectBase : SimpleSpellStatusEffectBase
{
	protected PerceptionGrantingSpellStatusEffectBase(IPerceivable owner, IMagicSpellEffectParent parent,
		IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	protected PerceptionGrantingSpellStatusEffectBase(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	protected abstract PerceptionTypes GrantedTypes { get; }

	public override PerceptionTypes PerceptionGranting => GrantedTypes;
}

public class SpellSilenceEffect : SimpleSpellStatusEffectBase, ISilencedEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellSilence", (effect, owner) => new SpellSilenceEffect(effect, owner));
	}

	public SpellSilenceEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellSilenceEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically silenced.";
	}

	protected override string SpecificEffectType => "SpellSilence";
}

public class SpellSleepEffect : SimpleSpellStatusEffectBase, ISleepEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellSleep", (effect, owner) => new SpellSleepEffect(effect, owner));
	}

	public SpellSleepEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellSleepEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (Owner is ICharacter character && !character.State.HasFlag(CharacterState.Sleeping))
		{
			character.Sleep();
		}
	}

	public override void RemovalEffect()
	{
		if (Owner is ICharacter character &&
			character.State.HasFlag(CharacterState.Sleeping) &&
			character.EffectsOfType<ISleepEffect>().Count() <= 1)
		{
			character.Awaken();
		}

		base.RemovalEffect();
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "In a magical sleep.";
	}

	protected override string SpecificEffectType => "SpellSleep";
}

public class SpellFearEffect : SimpleSpellStatusEffectBase, IFearEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellFear", (effect, owner) => new SpellFearEffect(effect, owner));
	}

	public SpellFearEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellFearEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (Owner is ICharacter character && character.Combat != null)
		{
			character.CombatStrategyMode = CombatStrategyMode.Flee;
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Overcome with magical fear.";
	}

	protected override string SpecificEffectType => "SpellFear";
}

public class SpellParalysisEffect : SimpleSpellStatusEffectBase, IForceParalysisEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellParalysis", (effect, owner) => new SpellParalysisEffect(effect, owner));
	}

	public SpellParalysisEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellParalysisEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public bool ShouldParalyse => true;

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically paralysed.";
	}

	protected override string SpecificEffectType => "SpellParalysis";
}

public class SpellFlightEffect : SimpleSpellStatusEffectBase, IFlightEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellFlight", (effect, owner) => new SpellFlightEffect(effect, owner));
	}

	public SpellFlightEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellFlightEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically able to fly.";
	}

	protected override string SpecificEffectType => "SpellFlight";
}

public class SpellWaterBreathingEffect : SimpleSpellStatusEffectBase, IAdditionalBreathableFluidEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellWaterBreathing", (effect, owner) => new SpellWaterBreathingEffect(effect, owner));
	}

	public SpellWaterBreathingEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellWaterBreathingEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public bool AppliesToFluid(IFluid fluid)
	{
		return fluid != null;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Magically able to breathe water.";
	}

	protected override string SpecificEffectType => "SpellWaterBreathing";
}

public class SpellCurseEffect : SimpleSpellStatusEffectBase, ICurseEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellCurse", (effect, owner) => new SpellCurseEffect(effect, owner));
	}

	public SpellCurseEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellCurseEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Cursed by hostile magic.";
	}

	protected override string SpecificEffectType => "SpellCurse";
}

public class SpellDetectInvisibleEffect : PerceptionGrantingSpellStatusEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellDetectInvisible", (effect, owner) => new SpellDetectInvisibleEffect(effect, owner));
	}

	public SpellDetectInvisibleEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellDetectInvisibleEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	protected override PerceptionTypes GrantedTypes => PerceptionTypes.VisualMagical;

	public override string Describe(IPerceiver voyeur)
	{
		return "Able to perceive the invisible.";
	}

	protected override string SpecificEffectType => "SpellDetectInvisible";
}

public class SpellDetectEtherealEffect : PerceptionGrantingSpellStatusEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellDetectEthereal", (effect, owner) => new SpellDetectEtherealEffect(effect, owner));
	}

	public SpellDetectEtherealEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellDetectEtherealEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	protected override PerceptionTypes GrantedTypes => PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal;

	public override string Describe(IPerceiver voyeur)
	{
		return "Able to perceive ethereal things.";
	}

	protected override string SpecificEffectType => "SpellDetectEthereal";
}

public class SpellDetectMagickEffect : PerceptionGrantingSpellStatusEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellDetectMagick", (effect, owner) => new SpellDetectMagickEffect(effect, owner));
	}

	public SpellDetectMagickEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellDetectMagickEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	protected override PerceptionTypes GrantedTypes => PerceptionTypes.SenseMagical;

	public override string Describe(IPerceiver voyeur)
	{
		return "Able to sense nearby magic.";
	}

	protected override string SpecificEffectType => "SpellDetectMagick";
}

public class SpellInfravisionEffect : PerceptionGrantingSpellStatusEffectBase, IDarksightEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellInfravision", (effect, owner) => new SpellInfravisionEffect(effect, owner));
	}

	public SpellInfravisionEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellInfravisionEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	protected override PerceptionTypes GrantedTypes => PerceptionTypes.VisualInfrared;

	public Difficulty MinimumEffectiveDifficulty => Difficulty.Normal;

	public override string Describe(IPerceiver voyeur)
	{
		return "Able to see by heat and in darkness.";
	}

	protected override string SpecificEffectType => "SpellInfravision";
}

public class SpellComprehendLanguageEffect : SimpleSpellStatusEffectBase, IComprehendLanguageEffect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellComprehendLanguage", (effect, owner) => new SpellComprehendLanguageEffect(effect, owner));
	}

	public SpellComprehendLanguageEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellComprehendLanguageEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public override string Describe(IPerceiver voyeur)
	{
		return "Able to comprehend spoken and written language.";
	}

	protected override string SpecificEffectType => "SpellComprehendLanguage";
}

public class SpellPoisonEffect : SimpleSpellStatusEffectBase
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellPoison", (effect, owner) => new SpellPoisonEffect(effect, owner));
	}

	public SpellPoisonEffect(IPerceivable owner, IMagicSpellEffectParent parent, IDrug drug, DrugVector vector,
		double grams, string gramsFormulaText, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		Drug = drug;
		Vector = vector;
		Grams = grams;
		GramsFormulaText = gramsFormulaText;
	}

	private SpellPoisonEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect")!;
		Drug = Gameworld.Drugs.Get(long.Parse(effect.Element("Drug")!.Value))!;
		Vector = (DrugVector)int.Parse(effect.Element("Vector")!.Value);
		Grams = double.Parse(effect.Element("Grams")!.Value);
		GramsFormulaText = effect.Element("GramsFormula")?.Value ?? "0";
	}

	public IDrug Drug { get; } = null!;
	public DrugVector Vector { get; }
	public double Grams { get; }
	public string GramsFormulaText { get; }

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (Owner is ICharacter character)
		{
			character.Body.Dose(Drug, Vector, Grams, this);
		}
	}

	public override void RemovalEffect()
	{
		if (Owner is ICharacter character)
		{
			character.Body.RemoveDrugDosages(x => ReferenceEquals(x.Originator, this));
		}

		base.RemovalEffect();
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("Drug", Drug.Id),
			new XElement("Vector", (int)Vector),
			new XElement("Grams", Grams),
			new XElement("GramsFormula", new XCData(GramsFormulaText))
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Poisoned with {Drug.Name.ColourValue()}.";
	}

	protected override string SpecificEffectType => "SpellPoison";
}

public class SpellDiseaseEffect : SimpleSpellStatusEffectBase
{
	private IInfection? _appliedInfection;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellDisease", (effect, owner) => new SpellDiseaseEffect(effect, owner));
	}

	public SpellDiseaseEffect(IPerceivable owner, IMagicSpellEffectParent parent, InfectionType infectionType,
		Difficulty virulenceDifficulty, double intensity, double virulence, string intensityFormulaText,
		string virulenceFormulaText, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
		InfectionType = infectionType;
		VirulenceDifficulty = virulenceDifficulty;
		Intensity = intensity;
		Virulence = virulence;
		IntensityFormulaText = intensityFormulaText;
		VirulenceFormulaText = virulenceFormulaText;
	}

	private SpellDiseaseEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		XElement effect = root.Element("Effect")!;
		InfectionType = (InfectionType)int.Parse(effect.Element("InfectionType")!.Value);
		VirulenceDifficulty = (Difficulty)int.Parse(effect.Element("VirulenceDifficulty")!.Value);
		Intensity = double.Parse(effect.Element("Intensity")!.Value);
		Virulence = double.Parse(effect.Element("Virulence")!.Value);
		IntensityFormulaText = effect.Element("IntensityFormula")?.Value ?? "0";
		VirulenceFormulaText = effect.Element("VirulenceFormula")?.Value ?? "1";
		AppliedInfectionId = long.Parse(effect.Element("AppliedInfectionId")?.Value ?? "0");
		if (owner is ICharacter character && AppliedInfectionId != 0)
		{
			_appliedInfection = character.Body.PartInfections.FirstOrDefault(x => x.Id == AppliedInfectionId);
		}
	}

	public InfectionType InfectionType { get; }
	public Difficulty VirulenceDifficulty { get; }
	public double Intensity { get; }
	public double Virulence { get; }
	public string IntensityFormulaText { get; }
	public string VirulenceFormulaText { get; }
	public long AppliedInfectionId { get; private set; }

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (_appliedInfection != null || Owner is not ICharacter character)
		{
			return;
		}

		_appliedInfection = Infection.LoadNewInfection(InfectionType, VirulenceDifficulty, Intensity, character.Body,
			null, null, Virulence);
		character.Body.AddInfection(_appliedInfection);
		AppliedInfectionId = _appliedInfection.Id;
	}

	public override void RemovalEffect()
	{
		if (Owner is ICharacter character)
		{
			_appliedInfection ??= AppliedInfectionId == 0
				? null
				: character.Body.PartInfections.FirstOrDefault(x => x.Id == AppliedInfectionId);
			if (_appliedInfection != null)
			{
				character.Body.RemoveInfection(_appliedInfection);
				_appliedInfection.Delete();
				_appliedInfection = null;
				AppliedInfectionId = 0;
			}
		}

		base.RemovalEffect();
	}

	protected override XElement SaveDefinition()
	{
		return SimpleSaveDefinition(
			new XElement("InfectionType", (int)InfectionType),
			new XElement("VirulenceDifficulty", (int)VirulenceDifficulty),
			new XElement("Intensity", Intensity),
			new XElement("Virulence", Virulence),
			new XElement("IntensityFormula", new XCData(IntensityFormulaText)),
			new XElement("VirulenceFormula", new XCData(VirulenceFormulaText)),
			new XElement("AppliedInfectionId", _appliedInfection?.Id ?? AppliedInfectionId)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Afflicted with {InfectionType.Describe()}.";
	}

	protected override string SpecificEffectType => "SpellDisease";
}
