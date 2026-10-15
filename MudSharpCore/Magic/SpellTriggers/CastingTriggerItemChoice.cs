#nullable enable
using MudSharp.GameItems;

namespace MudSharp.Magic.SpellTriggers;

/// <summary>One explicit configured choice followed by an exact carried item.</summary>
public sealed class CastingTriggerItemChoice : CastingTriggerItem
{
	public new static void RegisterFactory()
	{
		SpellTriggerFactory.RegisterLoadTimeFactory("itemchoice", (root, spell) => new CastingTriggerItemChoice(root, spell));
		SpellTriggerFactory.RegisterBuilderFactory("itemchoice", (_, spell) =>
			(new CastingTriggerItemChoice(new XElement("Trigger", new XElement("MinimumPower", (int)SpellPower.Insignificant),
				new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful), new XElement("TargetFilterProg", 0)), spell), ""),
			"Selects an item enchantment option and a carried item", "item", "filterprog <prog|none> - optional item policy");
	}
	public CastingTriggerItemChoice(XElement root, IMagicSpell spell) : base(root, spell) { }
	public override XElement SaveToXml()
	{
		var xml = base.SaveToXml(); xml.SetAttributeValue("type", "itemchoice"); return xml;
	}
	public override IMagicTrigger Clone() => new CastingTriggerItemChoice(SaveToXml(), Spell);
	public override string ShowPlayer(ICharacter actor) =>
		$"Cast Command - {Spell.School.SchoolVerb} cast {Spell.Name.ToLowerInvariant().DoubleQuotes()} <power> <enchantment choice> <carried item>";
	public override void DoTriggerCast(ICharacter actor, StringStack additionalArguments)
	{
		if (!CheckBaseTriggerCase(actor, additionalArguments, out var power)) return;
		var choice = additionalArguments.PopSpeech();
		var target = actor.TargetItem(additionalArguments.SafeRemainingArgument);
		if (string.IsNullOrWhiteSpace(choice) || target is null ||
			!ReferenceEquals(target.InInventoryOf, actor.Body) || !actor.Body.ExternalItems.Any(x => ReferenceEquals(x, target)) ||
			!AllowsTarget(target, actor))
		{
			actor.OutputHandler.Send("Specify an enchantment choice and one accessible item you carry.".ColourCommand()); return;
		}
		Spell.CastSpell(actor, target, power, new SpellAdditionalParameter { ParameterName = "item-enchantment-choice", Item = choice });
	}
}
