#nullable enable

using System.Xml.Linq;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellTriggers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Source emotional content admitted by the character casting pipeline.</summary>
public abstract partial class SourceEmotionalEffect : IMagicSpellEffectTemplate,
	IMagicSpellEffectPreparedSelection, IMagicSpellEffectOperation
{
	public const string RuntimeIntegrationError = "Source emotions require a prepared character/self invocation; direct effect and prepared payload application are unsupported.";
	private IMagicSpellEffectPreparedSelectionToken? _selection;
	internal IMagicSpellEffectPreparedSelectionToken? Selection => _selection;
	private XElement _profileXml;
	protected SourceEmotionalEffect(XElement root, IMagicSpell spell, EmotionalSpellKind kind)
	{
		Spell = spell; Kind = kind;
		_profileXml = new(root.Element("SourceProfile") ?? EmptyProfile(kind));
		ReadProfile();
	}
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public EmotionalSpellKind Kind { get; }
	public EmotionalStockProfile? Profile { get; private set; }
	public string? ProfileError { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	protected abstract string EffectType { get; }
	public abstract IMagicSpellEffectTemplate Clone();

	private void ReadProfile()
	{
		try
		{
			var profile = EmotionalStockProfile.Read(_profileXml);
			if (profile.Kind != Kind) throw new ArgumentException("The source profile belongs to the other emotional spell.");
			Profile = profile; ProfileError = null;
		}
		catch (Exception error) when (error is ArgumentException or FormatException or OverflowException or InvalidOperationException)
		{
			Profile = null; ProfileError = error.Message;
		}
	}

	public XElement SaveToXml() => new("Effect", new XAttribute("type", EffectType), new XElement(_profileXml));
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is CastingTriggerCharacter or CastingTriggerSelf;
	public string Show(ICharacter actor) => $"Source {Kind}: " +
		(Profile is { } profile
			? $"group {profile.Group}; {profile.UnitSeconds.ToString("N0", actor)} seconds/unit; cap {profile.CapUnits.ToString("N0", actor)}; intensity {profile.Intensity.ToString("N2", actor)}; native trait #{profile.TraitId.ToString("N0", actor)}; units {profile.UnitsPerSourcePoint.ToString("N2", actor)}; attack break {profile.BreakOnAdmittedAttack.ToColouredString()}."
			: $"invalid profile: {ProfileError}") +
		$"\n{EmotionalStockProfile.HistoricalExceptionLimit}\n{RuntimeIntegrationError}";

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		throw new InvalidOperationException(RuntimeIntegrationError);
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		throw new InvalidOperationException(RuntimeIntegrationError);
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient) =>
		_selection = ((MagicSpell)Spell).CaptureEmotionalSelection(this, caster, recipient, _selection);
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		try
		{
			((MagicSpell)Spell).CaptureEmotionalSelection(this, caster, recipient, selection);
			_selection = selection; error = null; return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		try
		{
			if (_selection is null) throw new InvalidOperationException(RuntimeIntegrationError);
			((MagicSpell)Spell).CaptureEmotionalSelection(this, caster, recipient, _selection);
			error = null; return true;
		}
		catch (InvalidOperationException exception) { error = exception.Message; return false; }
	}

	private static XElement EmptyProfile(EmotionalSpellKind kind) => new("SourceProfile",
		new XAttribute("version", 1), new XAttribute("kind", kind), new XAttribute("eligibility", 0),
		new XAttribute("intensity", 0), new XAttribute("trait", 0), new XAttribute("units", kind == EmotionalSpellKind.Fury ? 1 : 0),
		new XAttribute("attackbreak", kind == EmotionalSpellKind.Calm),
		new XElement("Lifetime", new XAttribute("group", kind == EmotionalSpellKind.Fury ? ArmageddonRousedFuryStock.LifetimeGroup : ArmageddonStillAngerStock.LifetimeGroup),
			new XAttribute("seconds", 600), new XAttribute("cap", kind == EmotionalSpellKind.Fury ? 36 : 24)),
		new XElement("Fallback", new XAttribute("numerator", kind == EmotionalSpellKind.Fury ? 3 : 2),
			new XAttribute("denominator", 1), new XAttribute("endurance", 0)),
		new XElement("Terrains"), new XElement("Saves"), new XElement("HistoricalExceptions", new XAttribute("status", "unmapped")));
}

public sealed class SourceFuryEffect : SourceEmotionalEffect
{
	public static void RegisterPreparationFactory() => RegisterFactory();
	public SourceFuryEffect(XElement root, IMagicSpell spell) : base(root, spell, EmotionalSpellKind.Fury) { }
	protected override string EffectType => "sourcefury";
	public override IMagicSpellEffectTemplate Clone() => new SourceFuryEffect(SaveToXml(), Spell);
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("sourcefury", (root, spell) => new SourceFuryEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("sourcefury", (_, spell) =>
			(new SourceFuryEffect(new XElement("Effect"), spell), string.Empty), "Source Fury with explicit attribute and terrain mappings",
			BuilderHelp, false, true, ["character", "self"]);
	}
}

public sealed class SourceCalmEffect : SourceEmotionalEffect
{
	public static void RegisterPreparationFactory() => RegisterFactory();
	public SourceCalmEffect(XElement root, IMagicSpell spell) : base(root, spell, EmotionalSpellKind.Calm) { }
	protected override string EffectType => "sourcecalm";
	public override IMagicSpellEffectTemplate Clone() => new SourceCalmEffect(SaveToXml(), Spell);
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("sourcecalm", (root, spell) => new SourceCalmEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("sourcecalm", (_, spell) =>
			(new SourceCalmEffect(new XElement("Effect"), spell), string.Empty), "Source Calm with selective cessation and admitted attack break",
			BuilderHelp, false, true, ["character", "self"]);
	}
}
