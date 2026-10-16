#nullable enable

using MudSharp.GameItems;
using MudSharp.Magic.SpellEffects;

namespace MudSharp.Magic.SpellTriggers;

internal static class TravelRuneSelector
{
	public static IGameItem? Resolve(ICharacter caster, string words)
	{
		var keys = words.Split(['.', ' '], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (keys.Length == 0 || keys.Any(x => x.StartsWith('#') || char.IsDigit(x[0]))) return null;
		var items = caster.Gameworld.Items.Where(x => keys.All(key => x.HasKeyword(key, caster, true)) &&
			SpellTravelAnchorResolver.TryResolve(caster, x, SpellTravelAnchorMode.AnchorRuneLiveRoom, out _, out _)).Take(2).ToArray();
		return items.Length == 1 ? items[0] : null;
	}
}

public sealed class CastingTriggerWaystepDestination : CastingTriggerBase
{
	private CastingTriggerWaystepDestination() { }
	private CastingTriggerWaystepDestination(XElement xml, IMagicSpell spell) : base(xml, spell) { }
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterLoadTimeFactory("waystepdestination", (xml, spell) => new CastingTriggerWaystepDestination(xml, spell));
		SpellTriggerFactory.RegisterBuilderFactory("waystepdestination", (_, spell) => (new CastingTriggerWaystepDestination(Builder(), spell), ""),
			"Selects a weighted Waystep pool or exact owned live Anchor Rune", "character", new CastingTriggerWaystepDestination().BuildingCommandHelp);
	}
	private static XElement Builder() => new("Trigger", new XAttribute("type", "waystepdestination"), new XElement("MinimumPower", (int)SpellPower.Insignificant), new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful));
	public override XElement SaveToXml() { var xml = Builder(); xml.SetElementValue("MinimumPower", (int)MinimumPower); xml.SetElementValue("MaximumPower", (int)MaximumPower); return xml; }
	public override IMagicTrigger Clone() => new CastingTriggerWaystepDestination(SaveToXml(), Spell);
	public override string TargetTypes => "character";
	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => true;
	public override string SubtypeBuildingCommandHelp => "Use random, or rune <owned Anchor Rune keywords>. No room or item IDs.";
	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power)) return;
		var mode = additionalArguments.PopSpeech(); IGameItem? rune = null;
		if (mode.EqualTo("rune")) rune = TravelRuneSelector.Resolve(actor, additionalArguments.SafeRemainingArgument);
		else if (!mode.EqualTo("random") || !additionalArguments.IsFinished) { actor.OutputHandler.Send(SubtypeBuildingCommandHelp); return; }
		if (mode.EqualTo("rune") && rune is null) { actor.OutputHandler.Send("Select one exact legally owned active Anchor Rune."); return; }
		Spell.CastSpell(actor, actor, power, new SpellAdditionalParameter { ParameterName = SpellTravelDestinationRequest.ParameterName, Item = new SpellTravelDestinationRequest(rune) });
	}
	public override string Show(ICharacter actor) => $"Waystep destination - {base.Show(actor)}";
	public override string ShowPlayer(ICharacter actor) => $"{Spell.School.SchoolVerb} cast {Spell.Name.ToLowerInvariant().DoubleQuotes()} <power> random|rune <keywords>";
}

public sealed class CastingTriggerBanishTraveller : CastingTriggerBase
{
	private CastingTriggerBanishTraveller() { }
	private CastingTriggerBanishTraveller(XElement xml, IMagicSpell spell) : base(xml, spell) { }
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterLoadTimeFactory("banishtraveller", (xml, spell) => new CastingTriggerBanishTraveller(xml, spell));
		SpellTriggerFactory.RegisterBuilderFactory("banishtraveller", (_, spell) => (new CastingTriggerBanishTraveller(Builder(), spell), ""),
			"Selects a local resisted traveller for the weighted banishment pool", "character", new CastingTriggerBanishTraveller().BuildingCommandHelp);
	}
	private static XElement Builder() => new("Trigger", new XAttribute("type", "banishtraveller"), new XElement("MinimumPower", (int)SpellPower.Insignificant), new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful));
	public override XElement SaveToXml() { var xml = Builder(); xml.SetElementValue("MinimumPower", (int)MinimumPower); xml.SetElementValue("MaximumPower", (int)MaximumPower); return xml; }
	public override IMagicTrigger Clone() => new CastingTriggerBanishTraveller(SaveToXml(), Spell);
	public override string TargetTypes => "character";
	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => true;
	public override string SubtypeBuildingCommandHelp => "Select one visible local traveller. Rune/save-waiver targeting is not bound and refuses safely.";
	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power)) return;
		if (actor.TargetActor(additionalArguments.SafeRemainingArgument) is not { } target) { actor.OutputHandler.Send("You do not see that traveller here."); return; }
		Spell.CastSpell(actor, target, power, new SpellAdditionalParameter { ParameterName = SpellTravelDestinationRequest.ParameterName, Item = new SpellTravelDestinationRequest(null) });
	}
	public override string Show(ICharacter actor) => $"Banish traveller - {base.Show(actor)}";
	public override string ShowPlayer(ICharacter actor) => $"{Spell.School.SchoolVerb} cast {Spell.Name.ToLowerInvariant().DoubleQuotes()} <power> <local traveller>";
}
