#nullable enable

using System.Globalization;

namespace MudSharp.Magic.SpellEffects;

public partial class DetectEtherealEffect : IMagicSpellEffectLifetimePolicy
{
	public MagicSpellLifetimePolicy? LifetimePolicy { get; private set; }
	public string? LifetimePolicyError { get; private set; }
	private XElement? _invalidLifetimePolicy;

	protected override void LoadFromXml(XElement root)
	{
		if (root.Element("LifetimePolicy") is not { } element) return;
		try { LifetimePolicy = DetectInvisibleEffect.ReadPolicy(element); }
		catch (Exception error) when (error is FormatException or OverflowException or ArgumentException)
		{
			LifetimePolicyError = error.Message;
			_invalidLifetimePolicy = new XElement(element);
		}
	}

	protected override void SaveToXml(XElement root)
	{
		if (_invalidLifetimePolicy is not null) root.Add(new XElement(_invalidLifetimePolicy));
		else if (LifetimePolicy is { } policy) root.Add(DetectInvisibleEffect.WritePolicy(policy));
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
		if (DetectInvisibleEffect.PolicyError(policy) is { } error) { actor.OutputHandler.Send(error.ColourError()); return false; }
		LifetimePolicy = policy; LifetimePolicyError = null; _invalidLifetimePolicy = null; Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor)); return true;
	}
}
