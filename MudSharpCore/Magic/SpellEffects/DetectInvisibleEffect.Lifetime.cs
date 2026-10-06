#nullable enable

using System.Globalization;

namespace MudSharp.Magic.SpellEffects;

public partial class DetectInvisibleEffect : IMagicSpellEffectLifetimePolicy
{
	public MagicSpellLifetimePolicy? LifetimePolicy { get; private set; }
	public string? LifetimePolicyError { get; private set; }
	private XElement? _invalidLifetimePolicy;

	internal static string? PolicyError(MagicSpellLifetimePolicy policy)
	{
		if (string.IsNullOrEmpty(policy.Group) || policy.Group.Length > 128 ||
			policy.Group.Any(x => !char.IsAsciiLetterOrDigit(x) && x is not '.' and not '_' and not '-'))
			return "The lifetime group must contain 1-128 ASCII letters, digits, dots, underscores or hyphens.";
		if (policy.UnitSeconds <= 0 || policy.MaximumUnits <= 0 ||
			(double)policy.UnitSeconds * policy.MaximumUnits > TimeSpan.MaxValue.TotalSeconds)
			return "Lifetime unit seconds and maximum units must be positive and yield a representable duration.";
		return null;
	}

	internal static MagicSpellLifetimePolicy ReadPolicy(XElement root)
	{
		if ((int?)root.Attribute("version") != 1 || (string?)root.Attribute("mode") != "accumulate" ||
			(bool?)root.Attribute("retainStrongestGrade") != true)
			throw new FormatException("Unsupported detection lifetime policy version, mode or strength retention.");
		var policy = new MagicSpellLifetimePolicy((string?)root.Attribute("group") ?? "",
			(int?)root.Attribute("unitSeconds") ?? 0, (int?)root.Attribute("maximumUnits") ?? 0);
		if (PolicyError(policy) is { } error) throw new FormatException(error);
		return policy;
	}

	internal static XElement WritePolicy(MagicSpellLifetimePolicy policy) => new("LifetimePolicy",
		new XAttribute("version", 1), new XAttribute("mode", "accumulate"), new XAttribute("group", policy.Group),
		new XAttribute("unitSeconds", policy.UnitSeconds), new XAttribute("maximumUnits", policy.MaximumUnits),
		new XAttribute("retainStrongestGrade", true));

	protected override void LoadFromXml(XElement root)
	{
		if (root.Element("LifetimePolicy") is not { } element) return;
		try { LifetimePolicy = ReadPolicy(element); }
		catch (Exception error) when (error is FormatException or OverflowException or ArgumentException)
		{
			LifetimePolicyError = error.Message;
			_invalidLifetimePolicy = new XElement(element);
		}
	}

	protected override void SaveToXml(XElement root)
	{
		if (_invalidLifetimePolicy is not null) root.Add(new XElement(_invalidLifetimePolicy));
		else if (LifetimePolicy is { } policy) root.Add(WritePolicy(policy));
	}

	public override string Show(ICharacter actor) => base.Show(actor) +
		(LifetimePolicyError is { } error ? $"\nInvalid lifetime policy: {error.ColourError()}" :
		 LifetimePolicy is { } policy ? $"\nAccumulate group {policy.Group.ColourValue()}, {policy.UnitSeconds.ToString("N0", actor)} seconds/unit, cap {policy.MaximumUnits.ToString("N0", actor)} units; retain strongest source grade." :
		 "\nLifetime: ordinary spell duration. Use lifetime accumulate <group> <seconds per unit> <maximum units>, or lifetime off.");

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("lifetime"))
		{
			actor.OutputHandler.Send("Use lifetime accumulate <group> <seconds per unit> <maximum units>, or lifetime off.");
			return false;
		}
		var mode = command.PopSpeech();
		if (mode.EqualTo("off") && command.IsFinished)
		{
			LifetimePolicy = null; LifetimePolicyError = null; _invalidLifetimePolicy = null;
			Spell.Changed = true; actor.OutputHandler.Send("This effect now uses ordinary spell duration."); return true;
		}
		var group = command.PopSpeech();
		var seconds = command.PopSpeech(); var maximum = command.PopSpeech();
		if (!mode.EqualTo("accumulate") || !command.IsFinished ||
			!int.TryParse(seconds, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unit) ||
			!int.TryParse(maximum, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cap))
		{
			actor.OutputHandler.Send("Use lifetime accumulate <group> <seconds per unit> <maximum units>, or lifetime off."); return false;
		}
		var policy = new MagicSpellLifetimePolicy(group, unit, cap);
		if (PolicyError(policy) is { } error) { actor.OutputHandler.Send(error.ColourError()); return false; }
		LifetimePolicy = policy; LifetimePolicyError = null; _invalidLifetimePolicy = null; Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor)); return true;
	}
}
