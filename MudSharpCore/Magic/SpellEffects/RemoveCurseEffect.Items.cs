#nullable enable

using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class RemoveCurseEffect
{
	public bool IncludeItems { get; private set; }
	private static bool CompatibleTargetTypes(string types) =>
		StandaloneSpellEffectTemplateHelper.IsCharacterTarget(types) || types is "item" or "items" or "perceivable" or "perceivables";
	protected override void LoadFromXml(XElement root) => IncludeItems = (bool?)root.Element("IncludeItems") ?? false;
	protected override void SaveToXml(XElement root) => root.Add(new XElement("IncludeItems", IncludeItems));
	public override bool IsCompatibleWithTrigger(IMagicTrigger trigger) =>
		IncludeItems ? CompatibleTargetTypes(trigger.TargetTypes) : base.IsCompatibleWithTrigger(trigger);
	public override IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem item || !IncludeItems)
			return base.GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		foreach (var curse in item.GetItemTypes<ICurseRemovableItem>().ToArray()) curse.RemoveCurse();
		return null;
	}
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("items"))
		{
			actor.OutputHandler.Send("Use items to toggle cleansing of designated item curses.");
			return false;
		}
		IncludeItems = !IncludeItems;
		Spell.Changed = true;
		actor.OutputHandler.Send($"Cleansing item curses: {IncludeItems.ToColouredString()}.");
		return true;
	}
	public override string Show(ICharacter actor) => base.Show(actor) + $" Item curses: {IncludeItems.ToColouredString()}.";
}
