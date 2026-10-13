#nullable enable
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.Health;
using MudSharp.Health.Infections;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework.Scheduling;
using MudSharp.Body;
using MudSharp.Framework.Save;

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

public class SpellInsomniaEffect : SimpleSpellStatusEffectBase, IPreventSleepEffect
{
	public const string DefaultSleepPreventionEcho = "A magical wakefulness keeps you from sleeping.";

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellInsomnia", (effect, owner) => new SpellInsomniaEffect(effect, owner));
	}

	public SpellInsomniaEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null)
		: base(owner, parent, prog)
	{
	}

	private SpellInsomniaEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public string SleepPreventionEcho => DefaultSleepPreventionEcho;

	public override string Describe(IPerceiver voyeur)
	{
		return "Kept awake by magical insomnia.";
	}

	protected override string SpecificEffectType => "SpellInsomnia";
}

public class SpellFearEffect : SimpleSpellStatusEffectBase, IFearEffect, IIndependentlyExpiringSpellEffect
{
	public DateTime? ExpiryUtc { get; }
	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellFear", (effect, owner) => new SpellFearEffect(effect, owner));
	}

	public SpellFearEffect(IPerceivable owner, IMagicSpellEffectParent parent, IFutureProg? prog = null, DateTime? expiryUtc = null)
		: base(owner, parent, prog)
	{
		ExpiryUtc = expiryUtc;
	}

	private SpellFearEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		ExpiryUtc = DateTime.TryParse(root.Element("Effect")?.Element("ExpiryUtc")?.Value, System.Globalization.CultureInfo.InvariantCulture,
			System.Globalization.DateTimeStyles.RoundtripKind, out var expiry) ? expiry : null;
	}
	protected override XElement SaveDefinition() => SimpleSaveDefinition(ExpiryUtc.HasValue ? new XElement("ExpiryUtc", ExpiryUtc.Value) : null);
	public override void InitialEffect()
	{
		base.InitialEffect();
		if (ExpiryUtc is { } expiry)
		{
			if (expiry <= RuntimeClock.UtcNow) Owner.RemoveEffect(this, true);
			else if (!Gameworld.EffectScheduler.IsScheduled(this)) Gameworld.EffectScheduler.AddSchedule(new EffectSchedule(this, expiry - RuntimeClock.UtcNow) { TriggerETA = expiry });
		}
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

	public override void RemovalEffect()
	{
		if (Owner is ICharacter character)
		{
			character.CheckCanFly(this);
		}

		base.RemovalEffect();
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

	protected SpellWaterBreathingEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
	}

	public virtual bool AppliesToFluid(IFluid fluid)
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
			character.Body.RemoveDrugDosages(IsLegacyUnownedDose);
			character.Body.Dose(Drug, Vector, Grams, this);
		}
	}

	public override void RemovalEffect()
	{
		if (Owner is ICharacter character)
		{
			character.Body.RemoveDrugDosages(x => ReferenceEquals(x.Originator, this) || IsLegacyUnownedDose(x));
		}

		base.RemovalEffect();
	}

	private bool IsLegacyUnownedDose(DrugDosage dosage)
	{
		return dosage.Originator is null &&
		       dosage.Drug == Drug &&
		       dosage.OriginalVector == Vector &&
		       Math.Abs(dosage.Grams - Grams) < 0.001;
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
	private IBody? _ownedBody;
	private long _ownedBodyId;
	private bool _loadedOwned;
	private bool _applyingOwned;
	private bool _removingOwned;

	public static void InitialiseEffectType()
	{
		RegisterFactory("SpellDisease", (effect, owner) => new SpellDiseaseEffect(effect, owner));
	}

	public SpellDiseaseEffect(IPerceivable owner, IMagicSpellEffectParent parent, InfectionType infectionType,
		Difficulty virulenceDifficulty, double intensity, double virulence, string intensityFormulaText,
		string virulenceFormulaText, IFutureProg? prog = null, string? ownedFamily = null,
		DrugVector exposureVector = default, IBody? ownedBody = null)
		: base(owner, parent, prog)
	{
		InfectionType = infectionType;
		VirulenceDifficulty = virulenceDifficulty;
		Intensity = intensity;
		Virulence = virulence;
		IntensityFormulaText = intensityFormulaText;
		VirulenceFormulaText = virulenceFormulaText;
		OwnedFamily = ownedFamily;
		ExposureVector = exposureVector;
		_ownedBody = ownedFamily is null ? null : ownedBody ?? (owner as ICharacter)?.Body;
		_ownedBodyId = _ownedBody?.Id ?? 0;
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
		OwnedFamily = effect.Element("OwnedFamily")?.Value;
		ExposureVector = (DrugVector)((int?)effect.Element("ExposureVector") ?? 0);
		_loadedOwned = OwnedFamily is not null;
		_ownedBodyId = (long?)effect.Element("OwnedBodyId") ?? 0;
		if (_loadedOwned && owner is ICharacter ownedCharacter &&
			_ownedBodyId == ownedCharacter.Body.Id)
			_ownedBody = ownedCharacter.Body;
		if (owner is ICharacter character && AppliedInfectionId != 0)
		{
			_appliedInfection = (OwnedFamily is null ? character.Body : _ownedBody)?.PartInfections
				.FirstOrDefault(x => x.Id == AppliedInfectionId && (OwnedFamily is null ||
					x.InfectionType == InfectionType && x.Wound is null && x.Bodypart is null));
		}
	}

	public InfectionType InfectionType { get; }
	public Difficulty VirulenceDifficulty { get; }
	public double Intensity { get; }
	public double Virulence { get; }
	public string IntensityFormulaText { get; }
	public string VirulenceFormulaText { get; }
	public long AppliedInfectionId { get; private set; }
	public string? OwnedFamily { get; }
	public DrugVector ExposureVector { get; }
	public IBody? OwnedBody => _ownedBody;
	public IInfection? AppliedInfection => _appliedInfection;

	public override void InitialEffect()
	{
		base.InitialEffect();
		if (OwnedFamily is not null)
		{
			InitialOwnedEffect();
			return;
		}
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
		if (OwnedFamily is not null)
		{
			RemoveOwnedEffect();
			base.RemovalEffect();
			return;
		}
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

	private void InitialOwnedEffect()
	{
		if (_applyingOwned) throw new InvalidOperationException("Owned sickness cannot be re-entered during native exposure.");
		if (_appliedInfection is not null) return;
		if (_loadedOwned) throw new InvalidOperationException("Saved owned sickness has no exact tracked infection; re-exposure is refused.");
		if (Owner is not ICharacter character || _ownedBody is null || !ReferenceEquals(character.Body, _ownedBody) ||
			string.IsNullOrWhiteSpace(OwnedFamily) || InfectionType != InfectionType.SownSickness ||
			ExposureVector != DrugVector.Touched || VirulenceDifficulty != Difficulty.Normal ||
			!double.IsFinite(Intensity) || Intensity is < 1 or > 7 || Virulence != 1)
			throw new InvalidOperationException("Invalid owned sickness recipient, body or native exposure configuration.");
		_applyingOwned = true;
		try
		{
			_appliedInfection = Infection.LoadNewInfection(InfectionType, VirulenceDifficulty, Intensity, _ownedBody,
				null, null, Virulence);
			if (_appliedInfection is ILateInitialisingItem late) late.IdRegistered += InfectionIdentityRegistered;
			if (!ReferenceEquals(character.Body, _ownedBody))
				throw new InvalidOperationException("Owned sickness body changed during native infection construction; the paid write is uncertain.");
			_ownedBody.AddInfection(_appliedInfection);
			MarkOwnedDirty(character);
			if (!ReferenceEquals(character.Body, _ownedBody) || !NativeInfectionStillOwned(_appliedInfection) ||
				!_ownedBody.PartInfections.Any(x => ReferenceEquals(x, _appliedInfection)))
				throw new InvalidOperationException("Owned sickness changed during native infection exposure; the paid write is uncertain.");
			_ownedBody.StartHealthTick();
			if (!ReferenceEquals(character.Body, _ownedBody) || !NativeInfectionStillOwned(_appliedInfection) ||
				!_ownedBody.PartInfections.Any(x => ReferenceEquals(x, _appliedInfection)))
				throw new InvalidOperationException("Owned sickness changed during native health activation; the paid write is uncertain.");
		}
		catch { MarkOwnedDirty(character); throw; }
		finally { _applyingOwned = false; }
	}

	private void InfectionIdentityRegistered(ILateInitialisingItem item)
	{
		if (!ReferenceEquals(item, _appliedInfection)) return;
		AppliedInfectionId = item.Id;
		Changed = true;
	}

	private void RemoveOwnedEffect()
	{
		if (_removingOwned) throw new InvalidOperationException("Owned sickness removal cannot be re-entered.");
		if (_ownedBody is null || _appliedInfection is null) return;
		var infection = _appliedInfection;
		if (!NativeInfectionStillOwned(infection))
			throw new InvalidOperationException("Owned sickness native infection was remapped; cleanup is refused.");
		_removingOwned = true;
		try
		{
			// Expiry still cleans the original physical body when the character has changed bodies.
			_ownedBody.RemoveInfection(infection);
			if (Owner is ICharacter character) MarkOwnedDirty(character);
			if (!NativeInfectionStillOwned(infection))
				throw new InvalidOperationException("Owned sickness native infection was remapped during removal; the paid write is uncertain.");
			if (infection is ILateInitialisingItem late)
			{
				late.IdRegistered -= InfectionIdentityRegistered;
				Gameworld.SaveManager.Abort(infection);
			}
			infection.Delete();
			if (_ownedBody.PartInfections.Any(x => ReferenceEquals(x, infection)))
				throw new InvalidOperationException("Owned sickness infection reappeared during deletion; the paid removal is uncertain.");
			_appliedInfection = null;
			AppliedInfectionId = 0;
		}
		finally { _removingOwned = false; }
	}

	private bool NativeInfectionStillOwned(IInfection infection) => infection is not Infection native ||
		ReferenceEquals(native.Owner, _ownedBody) && native.Wound is null && native.Bodypart is null;

	private void MarkOwnedDirty(ICharacter character)
	{
		character.Changed = true;
		Changed = true;
		if (_ownedBody is not null) _ownedBody.Changed = true;
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
			new XElement("AppliedInfectionId", _appliedInfection?.Id ?? AppliedInfectionId),
			OwnedFamily is null ? null : new XElement("OwnedFamily", OwnedFamily),
			OwnedFamily is null ? null : new XElement("ExposureVector", (int)ExposureVector),
			OwnedFamily is null ? null : new XElement("OwnedBodyId", _ownedBodyId)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Afflicted with {InfectionType.Describe()}.";
	}

	protected override string SpecificEffectType => "SpellDisease";
}
