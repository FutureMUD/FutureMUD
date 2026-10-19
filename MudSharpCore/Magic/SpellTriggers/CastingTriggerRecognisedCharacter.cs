#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Magic.SpellTriggers;

/// <summary>World targeting requires a persisted, unobscured personal identity observation, never a raw ID.</summary>
public sealed class CastingTriggerRecognisedCharacter : CastingTriggerBase
{
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterBuilderFactory("recognisedcharacter", (_, spell) =>
			(new CastingTriggerRecognisedCharacter(new XElement("Trigger", new XElement("MinimumPower", (int)SpellPower.Insignificant),
				new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful)), spell), ""),
			"Targets one currently visible, personally recognised world character", "character", new CastingTriggerRecognisedCharacter().BuildingCommandHelp);
		SpellTriggerFactory.RegisterLoadTimeFactory("recognisedcharacter", (xml, spell) => new CastingTriggerRecognisedCharacter(xml, spell));
	}
	private CastingTriggerRecognisedCharacter() { }
	private CastingTriggerRecognisedCharacter(XElement xml, IMagicSpell spell) : base(xml, spell) { }
	public override string TargetTypes => "character";
	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => true;
	public override string SubtypeBuildingCommandHelp => "Uses one verified identity dub for one loaded primary physical character; concealed and ambiguous identities are refused.";
	public override XElement SaveToXml() => new("Trigger", new XAttribute("type", "recognisedcharacter"),
		new XElement("MinimumPower", (int)MinimumPower), new XElement("MaximumPower", (int)MaximumPower));
	public override IMagicTrigger Clone() => new CastingTriggerRecognisedCharacter(SaveToXml(), Spell);
	internal static IDub[] IdentityDubs(ICharacter caster, ICharacter target) => caster.Dubs.Where(x =>
		x.TargetType == CharacterInstanceIdentityComparer.CharacterRecognitionTargetType && x.HasVerifiedIdentity &&
		!x.WasIdentityConcealed && ReferenceEquals(x.Owner, caster) && x.TargetId == CharacterInstanceIdentityComparer.IdentityId(target)).ToArray();
	public static bool IsRecognised(ICharacter caster, ICharacter target) => !CharacterInstanceIdentityComparer.SameIdentity(caster, target) &&
		(caster.AffectedBy<IAdminSightEffect>() || !target.IsPlayerCharacter ||
		 (!target.AffectedBy<IAdminSightEffect>() && !target.AffectedBy<IAdminInvisEffect>())) &&
		ReferenceEquals(caster.Gameworld, target.Gameworld) && target.IsPrimaryInstance && target.InstanceId > 0 &&
		target.Gameworld.Actors.Any(x => ReferenceEquals(x, target)) && target.Body is { Id: > 0 } body && ReferenceEquals(body.Actor, target) &&
		target.Identity?.Instances.Any(x => ReferenceEquals(x, target)) == true && IdentityDubs(caster, target).Length == 1 &&
		!PsychicSuppressionEffect.Suppresses(caster, "recognition", CharacterInstanceIdentityComparer.IdentityId(target)) &&
		!CharacterRecognitionAuthority.IsObscured(target, caster) && caster.CanSee(target);
	public static ICharacter? Resolve(ICharacter caster, string words)
	{
		var keys = words.Split(['.', ' '], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (keys.Length == 0 || keys.Any(x => x.StartsWith('#') || char.IsDigit(x[0])) || words.EqualToAny("me", "self")) return null;
		var dubs = caster.Dubs.Where(x => x.HasVerifiedIdentity && !x.WasIdentityConcealed && ReferenceEquals(x.Owner, caster) &&
			x.TargetType == CharacterInstanceIdentityComparer.CharacterRecognitionTargetType &&
			keys.All(key => x.Keywords.Any(word => word.StartsWith(key, StringComparison.OrdinalIgnoreCase)))).Take(2).ToArray();
		if (dubs.Length != 1) return null;
		var actors = caster.Gameworld.Actors.Where(x => CharacterInstanceIdentityComparer.IdentityId(x) == dubs[0].TargetId && x.IsPrimaryInstance).Take(2).ToArray();
		return actors.Length == 1 && IsRecognised(caster, actors[0]) ? actors[0] : null;
	}
	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power)) return;
		if (Resolve(actor, additionalArguments.SafeRemainingArgument) is not { } target)
		{ actor.OutputHandler.Send("You cannot identify one visible, personally recognised traveller by those keywords."); return; }
		Spell.CastSpell(actor, target, power);
	}
	public override string Show(ICharacter actor) => $"{"Cast@RecognisedCharacter".ColourName()} - {base.Show(actor)}";
	public override string ShowPlayer(ICharacter actor) => $"Cast Command - {Spell.School.SchoolVerb} cast {Spell.Name.ToLowerInvariant().DoubleQuotes()} <power> <identity dub>";
}
