#nullable enable

namespace MudSharp.Magic.SpellTriggers;

/// <summary>A typed effect context supplied by a successful combat power, never a casting command.</summary>
public sealed class AttackHitTrigger(bool targetsItems) : IMagicTrigger
{
	public bool TargetsItems { get; } = targetsItems;
	public MagicTriggerType TriggerType => MagicTriggerType.AttackHit;
	public bool TriggerYieldsTarget => true;
	public bool TriggerMayFailToYieldTarget => false;
	public string TargetTypes => TargetsItems ? "item" : "character";
	public IMagicTrigger Clone() => new AttackHitTrigger(TargetsItems);
	public XElement SaveToXml() => new("Trigger", new XAttribute("type", TargetsItems ? "attackitem" : "attackcharacter"));
	public string Show(ICharacter actor) => $"Successful magical attack on {(TargetsItems ? "an item" : "a character")}";
	public string ShowPlayer(ICharacter actor) => "Applied by a combat power";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.Send("This trigger receives its target from the attack. Use attackcharacter or attackitem to select its context.");
		return false;
	}
	public static void RegisterFactory()
	{
		foreach (var items in new[] { false, true })
		{
			var type = items ? "attackitem" : "attackcharacter";
			SpellTriggerFactory.RegisterLoadTimeFactory(type, (_, _) => new AttackHitTrigger(items));
			SpellTriggerFactory.RegisterBuilderFactory(type, (_, _) => (new AttackHitTrigger(items), string.Empty),
				"An effect payload delivered by a successful magical attack", items ? "item" : "character",
				"No casting command or additional arguments; the attack supplies the target.");
		}
	}
}
