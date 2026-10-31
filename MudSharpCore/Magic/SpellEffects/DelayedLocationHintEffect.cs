#nullable enable

using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Records the recipient's location and later reveals a bounded native route back to it.</summary>
public sealed class DelayedLocationHintEffect : IMagicSpellEffectTemplate
{
	private readonly XElement _definition;
	public DelayedLocationHintEffect(XElement definition, IMagicSpell spell) { _definition = new(definition); Spell = spell; }
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	private double BaseDelay => (double?)_definition.Element("BaseDelay") ?? 0;
	private double DelayPerGrade => (double?)_definition.Element("DelayPerGrade") ?? 0;
	private int BaseRange => (int?)_definition.Element("BaseRange") ?? 0;
	private int RangePerGrade => (int?)_definition.Element("RangePerGrade") ?? 0;
	public string? DefinitionError => !double.IsFinite(BaseDelay) || !double.IsFinite(DelayPerGrade) ||
		BaseDelay < 0 || DelayPerGrade < 0 || BaseDelay + DelayPerGrade <= 0 || BaseDelay + DelayPerGrade * 7 > 86400 ||
		BaseRange < 0 || RangePerGrade < 0 || (long)BaseRange + RangePerGrade < 1 || (long)BaseRange + 7L * RangePerGrade > 140
		? "Location hints require a positive delay of at most one day and a range of 1–140 exits at every grade." : null;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new DelayedLocationHintEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is SpellTriggers.CastingTriggerSelf or SpellTriggers.CastingTriggerCharacter;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("delayedlocationhint", (xml, spell) => new DelayedLocationHintEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("delayedlocationhint", (_, spell) => (new DelayedLocationHintEffect(new XElement("Effect",
			new XAttribute("type", "delayedlocationhint"), new XElement("BaseDelay", 30), new XElement("DelayPerGrade", 30),
			new XElement("BaseRange", 10), new XElement("RangePerGrade", 10)), spell), ""),
			"Remember a location and later reveal a route back", "delay <base seconds> <seconds per grade> | range <base exits> <exits per grade>", false, true, ["self", "character"]);
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power,
		IMagicSpellEffectParent parent, SpellAdditionalParameter[] parameters)
	{
		if (DefinitionError is not null || target is not ICharacter actor || actor.Location is null) return null;
		var grade = (Spell as MagicSpell)?.InvocationGrade ?? Math.Clamp((int)power - (int)SpellPower.ExtremelyWeak + 1, 1, 7);
		return new SpellDelayedLocationHintEffect(actor, parent, actor.Location.Id, actor.Body.Id,
			TimeSpan.FromSeconds(BaseDelay + DelayPerGrade * grade), BaseRange + RangePerGrade * grade);
	}
	public string Show(ICharacter actor) => $"Remember the recipient's room; after {BaseDelay.ToString("N2", actor)} + grade × {DelayPerGrade.ToString("N2", actor)} seconds reveal a route within {BaseRange.ToString("N0", actor)} + grade × {RangePerGrade.ToString("N0", actor)} exits.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var setting = command.PopSpeech().ToLowerInvariant();
		var before = new XElement(_definition);
		if (setting == "delay" && double.TryParse(command.PopSpeech(), out var delay) && double.TryParse(command.PopSpeech(), out var perGrade))
		{
			_definition.SetElementValue("BaseDelay", delay); _definition.SetElementValue("DelayPerGrade", perGrade);
		}
		else if (setting == "range" && int.TryParse(command.PopSpeech(), out var range) && int.TryParse(command.PopSpeech(), out var rangePerGrade))
		{
			_definition.SetElementValue("BaseRange", range); _definition.SetElementValue("RangePerGrade", rangePerGrade);
		}
		else return false;
		if (!command.IsFinished || DefinitionError is not null)
		{
			actor.OutputHandler.Send((DefinitionError ?? "Unexpected additional arguments.").ColourError());
			_definition.ReplaceNodes(before.Nodes()); return false;
		}
		Spell.Changed = true; actor.OutputHandler.Send("The delayed location hint has been updated."); return true;
	}
}
