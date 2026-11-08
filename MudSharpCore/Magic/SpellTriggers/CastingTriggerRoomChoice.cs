#nullable enable

namespace MudSharp.Magic.SpellTriggers;

/// <summary>A room spell with an optional explicit configured choice.</summary>
public sealed class CastingTriggerRoomChoice : CastingTriggerRoom
{
	public new static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterLoadTimeFactory("roomchoice", (xml, spell) => new CastingTriggerRoomChoice(xml, spell));
		SpellTriggerFactory.RegisterBuilderFactory("roomchoice", (_, spell) =>
			(new CastingTriggerRoomChoice(new XElement("Trigger", new XElement("MinimumPower", (int)SpellPower.Insignificant),
				new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful)), spell), ""),
			"Targets the caster's room with an optional configured choice", "room", "No additional settings.");
	}
	public CastingTriggerRoomChoice(XElement xml, IMagicSpell spell) : base(xml, spell) { }
	public override XElement SaveToXml() { var xml = base.SaveToXml(); xml.SetAttributeValue("type", "roomchoice"); return xml; }
	public override IMagicTrigger Clone() => new CastingTriggerRoomChoice(SaveToXml(), Spell);
	public override string ShowPlayer(ICharacter actor) => base.ShowPlayer(actor) + " [summon choice]";
	public override void DoTriggerCast(ICharacter actor, StringStack arguments)
	{
		if (!CheckBaseTriggerCase(actor, arguments, out var power)) return;
		var choice = arguments.PopSpeech();
		if (!arguments.IsFinished) { actor.Send("Specify at most one summon choice; quote names containing spaces."); return; }
		Spell.CastSpell(actor, actor.Location, power, string.IsNullOrEmpty(choice) ? [] :
			[new SpellAdditionalParameter { ParameterName = "npc-choice", Item = choice }]);
	}
}
