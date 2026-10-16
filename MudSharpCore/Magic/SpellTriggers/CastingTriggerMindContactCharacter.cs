#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.Magic.SpellTriggers;

/// <summary>Resolves only the exact active physical character linked to the caster by a live mind contact.</summary>
public sealed class CastingTriggerMindContactCharacter : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("mindcontactcharacter", (_, spell) =>
			(new CastingTriggerMindContactCharacter(new XElement("Trigger",
				new XElement("MinimumPower", (int)SpellPower.Insignificant),
				new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful)), spell), string.Empty),
			"Targets one character in an active mind contact", "character",
			new CastingTriggerMindContactCharacter().BuildingCommandHelp);
		SpellTriggerFactory.RegisterLoadTimeFactory("mindcontactcharacter",
			(xml, spell) => new CastingTriggerMindContactCharacter(xml, spell));
	}

	private CastingTriggerMindContactCharacter(XElement root, IMagicSpell spell) : base(root, spell) { }
	private CastingTriggerMindContactCharacter() { }

	public override XElement SaveToXml() => new("Trigger", new XAttribute("type", "mindcontactcharacter"),
		new XElement("MinimumPower", (int)MinimumPower), new XElement("MaximumPower", (int)MaximumPower));
	public override IMagicTrigger Clone() => new CastingTriggerMindContactCharacter(SaveToXml(), Spell);
	public override string TargetTypes => "character";
	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => true;
	public override string SubtypeBuildingCommandHelp =>
		"Targets an exact, active physical character currently connected to your mind. Self is excluded.";

	public static bool IsCurrentContact(ICharacter actor, ICharacter target) =>
		!CharacterInstanceIdentityComparer.SamePhysicalInstanceOrBody(actor, target) &&
		actor.InstanceId > 0 && actor.Gameworld.Actors.Any(x => ReferenceEquals(x, actor)) &&
		actor.Body is { Id: > 0 } casterBody && ReferenceEquals(casterBody.Actor, actor) &&
		actor.Identity?.Instances.Any(x => ReferenceEquals(x, actor)) == true &&
		ReferenceEquals(actor.Gameworld, target.Gameworld) && target.InstanceId > 0 &&
		actor.Gameworld.Actors.Any(x => ReferenceEquals(x, target)) &&
		target.Body is { Id: > 0 } body && ReferenceEquals(body.Actor, target) &&
		target.Identity?.Instances.Any(x => ReferenceEquals(x, target)) == true &&
		actor.EffectsOfType<ConnectMindEffect>().Any(effect =>
			actor.Effects.Any(x => ReferenceEquals(x, effect)) &&
			ReferenceEquals(effect.TargetCharacter, target) &&
			ReferenceEquals(effect.ChildEffect.CharacterOwner, target) &&
			ReferenceEquals(effect.ChildEffect.OriginatorCharacter, actor) &&
			ReferenceEquals(effect.ChildEffect.OriginatorEffect, effect) &&
			target.Effects.Any(x => ReferenceEquals(x, effect.ChildEffect)) &&
			effect.MindPower.TargetIsInRange(actor, target, effect.MindPower.PowerDistance));

	public static ICharacter? Resolve(ICharacter actor, string words)
	{
		if (string.IsNullOrWhiteSpace(words) || words.EqualToAny("self", "me")) return null;
		var keywords = words.Split(['.', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (keywords.Length == 0 || keywords.Any(x => x.StartsWith('#') || char.IsDigit(x[0]))) return null;
		var contacts = actor.EffectsOfType<ConnectMindEffect>()
			.Select(x => x.TargetCharacter).Where(x => x is not null && IsCurrentContact(actor, x))
			.Distinct().ToArray();
		var matches = contacts.Where(x => keywords.All(keyword =>
			x.HasKeyword(keyword, actor, true) || actor.HasDubFor(x, keyword))).Take(2).ToArray();
		return matches.Length == 1 ? matches[0] : null;
	}

	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power)) return;
		var target = Resolve(actor, additionalArguments.SafeRemainingArgument);
		if (target is null)
		{
			actor.OutputHandler.Send("You have no active mind contact with that traveller.");
			return;
		}
		Spell.CastSpell(actor, target, power);
	}

	public override string Show(ICharacter actor) =>
		$"{"Cast@MindContactCharacter".ColourName()} - {base.Show(actor)}";
	public override string ShowPlayer(ICharacter actor) =>
		$"Cast Command - {Spell.School.SchoolVerb} cast {(Spell.Name.Contains(' ') ? Spell.Name.ToLowerInvariant().DoubleQuotes() : Spell.Name.ToLowerInvariant())} <power> <mind contact>";
}
