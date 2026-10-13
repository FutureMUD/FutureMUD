#nullable enable

using System;
using System.Globalization;
using System.Xml.Linq;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Computes an approved movement-time multiplier from the selected external grade.</summary>
public sealed class MovementTempoSpellEffect : CharacterSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("movementtempo", (root, spell) => new MovementTempoSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("movementtempo", (commands, spell) =>
			(new MovementTempoSpellEffect(new XElement("Effect", new XAttribute("type", "movementtempo"),
				new XElement("Slow", false), new XElement("RatePerGrade", "0.05")), spell), string.Empty),
			"Changes ordinary movement time by selected spell grade", HelpText, false, true,
			StandaloneSpellEffectTemplateHelper.CharacterTriggerTypes);
	}

	private MovementTempoSpellEffect(XElement root, IMagicSpell spell) : base(root, spell)
	{
	}

	public bool Slow { get; private set; }
	public double RatePerGrade { get; private set; }
	protected override string BuilderEffectType => "movementtempo";
	protected override string ShowText => "Movement Tempo";
	public const string HelpText = "Options: mode <fast|slow>, rate <per-grade fraction from 0 to 1>. Native movement admission and terrain remain in force.";

	protected override void LoadFromXml(XElement root)
	{
		Slow = bool.Parse(root.Element("Slow")?.Value ?? throw new FormatException("Missing movement tempo mode."));
		RatePerGrade = double.Parse(root.Element("RatePerGrade")?.Value ??
			throw new FormatException("Missing movement tempo rate."), CultureInfo.InvariantCulture);
		if (!double.IsFinite(RatePerGrade) || RatePerGrade is <= 0 or > 1)
			throw new FormatException("Movement tempo rate must be finite and within (0,1].");
	}

	protected override void SaveToXml(XElement root)
	{
		root.Add(new XElement("Slow", Slow), new XElement("RatePerGrade",
			RatePerGrade.ToString("R", CultureInfo.InvariantCulture)));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		if (action == "mode" && !command.IsFinished)
		{
			var mode = command.PopSpeech().ToLowerInvariant();
			if (!command.IsFinished || mode is not ("fast" or "slow"))
			{
				actor.OutputHandler.Send(HelpText);
				return false;
			}
			Slow = mode == "slow";
		}
		else if (action == "rate" && !command.IsFinished)
		{
			if (!double.TryParse(command.PopSpeech(), NumberStyles.Float, CultureInfo.InvariantCulture, out var rate) ||
				!command.IsFinished || !double.IsFinite(rate) || rate is <= 0 or > 1)
			{
				actor.OutputHandler.Send(HelpText);
				return false;
			}
			RatePerGrade = rate;
		}
		else
		{
			actor.OutputHandler.Send(HelpText);
			return false;
		}
		Spell.Changed = true;
		actor.OutputHandler.Send($"Movement time now uses {(Slow ? "slow" : "fast").ColourValue()} mode at {RatePerGrade.ToString("P2", actor).ColourValue()} per grade.");
		return true;
	}

	public override string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, ShowText,
		("Mode", (Slow ? "Slow" : "Fast").ColourValue()),
		("Rate per grade", RatePerGrade.ToString("P2", actor).ColourValue()));

	public static double MultiplierForGrade(int grade, bool slow, double ratePerGrade)
	{
		if (grade is < 1 or > 7 || !double.IsFinite(ratePerGrade) || ratePerGrade is <= 0 or > 1)
			throw new ArgumentOutOfRangeException(nameof(grade), "Select grade 1..7 and a finite rate in (0,1].");
		var factor = 1 + ratePerGrade * grade;
		return slow ? factor : 1 / factor;
	}

	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, ICharacter target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters) =>
		Spell is MagicSpell { InvocationGrade: >= 1 and <= 7 } native
			? new SpellMovementTempoEffect(target, parent,
				MultiplierForGrade(native.InvocationGrade.Value, Slow, RatePerGrade))
			: null;

	public override IMagicSpellEffectTemplate Clone() => new MovementTempoSpellEffect(SaveToXml(), Spell);
}
