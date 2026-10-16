#nullable enable

using MudSharp.GameItems;
using MudSharp.Magic.SpellEffects;

namespace MudSharp.Magic.SpellTriggers;

public sealed class CastingTriggerThresholdDestination : CastingTriggerBase
{
	private CastingTriggerThresholdDestination() { }
	private CastingTriggerThresholdDestination(XElement xml, IMagicSpell spell) : base(xml, spell) { }
	public static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterLoadTimeFactory("thresholddestination", (xml, spell) => new CastingTriggerThresholdDestination(xml, spell));
		SpellTriggerFactory.RegisterBuilderFactory("thresholddestination", (_, spell) => (new CastingTriggerThresholdDestination(Definition(), spell), ""),
			"Selects one exact carried Arcane Mark or recognised visible traveller", "room", new CastingTriggerThresholdDestination().BuildingCommandHelp);
	}
	private static XElement Definition() => new("Trigger", new XAttribute("type", "thresholddestination"), new XElement("MinimumPower", (int)SpellPower.Insignificant), new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful));
	public override XElement SaveToXml() { var xml = Definition(); xml.SetElementValue("MinimumPower", (int)MinimumPower); xml.SetElementValue("MaximumPower", (int)MaximumPower); return xml; }
	public override IMagicTrigger Clone() => new CastingTriggerThresholdDestination(SaveToXml(), Spell);
	public override string TargetTypes => "room";
	public override bool TriggerYieldsTarget => true;
	public override bool TriggerMayFailToYieldTarget => true;
	public override string SubtypeBuildingCommandHelp => "Use mark <owned carried Arcane Mark keywords> or traveller <verified identity dub>. No raw IDs.";
	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power)) return;
		var mode = additionalArguments.PopSpeech(); var words = additionalArguments.SafeRemainingArgument;
		IGameItem? mark = null; ICharacter? traveller = null;
		if (mode.EqualTo("mark"))
		{
			var keys = words.Split(['.', ' '], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			if (keys.Length == 0 || keys.Any(x => x.StartsWith('#') || char.IsDigit(x[0]))) return;
			var matches = actor.Body.DirectItems.Where(x => keys.All(key => x.HasKeyword(key, actor, true)) &&
				SpellTravelAnchorResolver.TryResolve(actor, x, SpellTravelAnchorMode.ArcaneMarkedRoom, out _, out _)).Take(2).ToArray();
			if (matches.Length == 1) mark = matches[0];
		}
		else if (mode.EqualTo("traveller")) traveller = CastingTriggerRecognisedCharacter.Resolve(actor, words);
		if (mark is null && traveller is null) { actor.OutputHandler.Send("Select one exact permitted Arcane Mark or personally recognised traveller."); return; }
		Spell.CastSpell(actor, actor.Location, power, new SpellAdditionalParameter { ParameterName = SpellThresholdDestinationRequest.ParameterName, Item = new SpellThresholdDestinationRequest(mark, traveller) });
	}
	public override string Show(ICharacter actor) => $"Threshold destination - {base.Show(actor)}";
	public override string ShowPlayer(ICharacter actor) => $"{Spell.School.SchoolVerb} cast {Spell.Name.ToLowerInvariant().DoubleQuotes()} <power> mark|traveller <keywords>";
}
