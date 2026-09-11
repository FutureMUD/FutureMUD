#nullable enable
namespace MudSharp.Magic.SpellTriggers;

public sealed class SubstanceTrigger(bool items) : IMagicTrigger
{
	public MagicTriggerType TriggerType => MagicTriggerType.Substance;
	public bool TriggerYieldsTarget => true;
	public bool TriggerMayFailToYieldTarget => false;
	public string TargetTypes => items ? "item" : "character";
	public IMagicTrigger Clone() => new SubstanceTrigger(items);
	public XElement SaveToXml() => new("Trigger", new XAttribute("type", items ? "substanceitem" : "substancecharacter"));
	public string Show(ICharacter actor) => $"Magical substance applied to {(items ? "an item" : "a character")}";
	public string ShowPlayer(ICharacter actor) => "Delivered by a magical substance";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.Send("The carrier supplies the target. Select substancecharacter or substanceitem to change target type.");
		return false;
	}
	public static void RegisterFactory()
	{
		foreach (var item in new[] { false, true })
		{
			var type = item ? "substanceitem" : "substancecharacter";
			SpellTriggerFactory.RegisterLoadTimeFactory(type, (_, _) => new SubstanceTrigger(item));
			SpellTriggerFactory.RegisterBuilderFactory(type, (_, _) => (new SubstanceTrigger(item), string.Empty),
				"A prepared magical substance payload", item ? "item" : "character", "The exposure supplies the target.");
		}
	}
}
