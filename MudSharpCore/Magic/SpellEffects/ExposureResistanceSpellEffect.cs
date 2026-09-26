#nullable enable
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public sealed class ExposureResistanceSpellEffect : IMagicSpellEffectTemplate
{
	public const string HelpText = "Exposure resistance: multiplier <0-100>, routes <LiquidContact,GasContact,Inhalation,AmbientHeat>, category <name|*>, part <bodypart id|0>. Zero multiplier prevents injury; it does not prevent physical consumption or supply oxygen. Part 0 means the whole target. Character protection does not protect equipment.";
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("exposureresistance", (xml, spell) => new ExposureResistanceSpellEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("exposureresistance", (_, spell) =>
			(new ExposureResistanceSpellEffect(new XElement("Effect"), spell), string.Empty), "Resists environmental exposure injury", HelpText, false, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x => Compatible(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes)).ToArray());
	}
	public ExposureResistanceSpellEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		Multiplier = (double?)xml.Element("Multiplier") ?? 0.5;
		Routes = (ExposureRoute)((int?)xml.Element("Routes") ?? 11);
		Category = (string?)xml.Element("Category") ?? "*";
		PartId = (long?)xml.Element("Part") ?? 0;
	}
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public double Multiplier { get; private set; }
	public ExposureRoute Routes { get; private set; }
	public string Category { get; private set; }
	public long PartId { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	private static bool Compatible(string types) => types is "character" or "characters" or "item" or "items";
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => Compatible(trigger.TargetTypes);
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "exposureresistance"),
		new XElement("Multiplier", Multiplier), new XElement("Routes", (int)Routes), new XElement("Category", Category), new XElement("Part", PartId));
	public IMagicSpellEffectTemplate Clone() => new ExposureResistanceSpellEffect(SaveToXml(), Spell);
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var field = command.PopSpeech().ToLowerInvariant();
		var value = command.SafeRemainingArgument;
		switch (field)
		{
			case "multiplier" when double.TryParse(value, actor, out var number) && ExposureArithmetic.Valid(number) && number <= 100:
				Multiplier = number; break;
			case "routes" when Enum.TryParse<ExposureRoute>(value, true, out var routes) && routes != ExposureRoute.None && ((int)routes & ~15) == 0:
				Routes = routes; break;
			case "category" when !string.IsNullOrWhiteSpace(value): Category = value; break;
			case "part" when long.TryParse(value, out var part) && (part == 0 || Gameworld.BodypartPrototypes.Get(part) is not null): PartId = part; break;
			default: actor.OutputHandler.Send(HelpText); return false;
		}
		Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor));
		return true;
	}
	public string Show(ICharacter actor) => $"Exposure resistance: multiplier {Multiplier.ToString("N3", actor).ColourValue()}, routes {Routes.DescribeEnum().ColourName()}, category {Category.ColourName()}, {(PartId == 0 ? "whole target" : $"part #{PartId}")}.";
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		target is ICharacter or IGameItem && ExposureArithmetic.Valid(Multiplier) && Multiplier <= 100 && ((int)Routes & ~15) == 0
			? new SpellExposureResistanceEffect(target, parent, Multiplier, Routes, Category, PartId) : null;
}
