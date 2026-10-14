#nullable enable

using MudSharp.Construction;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class ThresholdAlarmEffect : RoomSpellEffectTemplateBase
{
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("thresholdalarm", (xml, spell) => new ThresholdAlarmEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("thresholdalarm", (_, spell) =>
			(new ThresholdAlarmEffect(new XElement("Effect", new XAttribute("type", "thresholdalarm")), spell), string.Empty),
			"Notifies the caster of a native room arrival without revealing the entrant", HelpText,
			false, true, StandaloneSpellEffectTemplateHelper.RoomTriggerTypes);
	}

	public ThresholdAlarmEffect(XElement root, IMagicSpell spell) : base(root, spell) { }
	protected override string BuilderEffectType => "thresholdalarm";
	public const string HelpText = "This effect has no options. It notifies the caster once per native room-arrival event while the spell lasts, without naming the entrant.";
	protected override IMagicSpellEffect? CreateEffect(ICharacter caster, IRoom target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters) =>
		caster.Location is not null && ReferenceEquals(caster.Location, target) &&
		ReferenceEquals(caster.Gameworld, target.Gameworld)
			? new SpellThresholdAlarmEffect(target, parent, caster)
			: null;
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send(HelpText);
		return false;
	}
	public override string Show(ICharacter actor) => "Threshold Alarm: notice the caster of each eligible native room arrival.";
	public override IMagicSpellEffectTemplate Clone() => new ThresholdAlarmEffect(SaveToXml(), Spell);
}
