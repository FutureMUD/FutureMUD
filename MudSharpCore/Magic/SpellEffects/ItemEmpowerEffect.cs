#nullable enable
using System.Globalization;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.GameItems;
using MudSharp.Magic.SpellTriggers;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Builder-configured seven-grade choices, bound on detached invocations before payment.</summary>
public sealed class ItemEmpowerEffect : IMagicSpellEffectTemplate, IMagicSpellEffectInvocationInputs,
	IMagicSpellEffectAdmission, IMagicSpellEffectOperation
{
	private readonly XElement _definition;
	private Option? _selected;
	private IGameItem? _item;
	private ICharacter? _caster;
	private double _amount;
	private sealed record Option(string Name, EquippedItemBonusKind Kind, long TargetId, double[] Amounts);
	private Option[] Options => _definition.Elements("Option").Select(xml => new Option(
		(string)xml.Attribute("name")!, Enum.Parse<EquippedItemBonusKind>((string)xml.Attribute("kind")!, false),
		(long?)xml.Attribute("target") ?? 0, xml.Elements("Amount").Select(x => double.Parse(x.Value, CultureInfo.InvariantCulture)).ToArray())).ToArray();

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("itemempower", (xml, spell) => new ItemEmpowerEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("itemempower", (_, spell) =>
			(new ItemEmpowerEffect(new XElement("Effect", new XAttribute("type", "itemempower"), new XAttribute("version", 1)), spell), ""),
			"Grants a selected equipped item attribute or capacity bonus", "option <name> <kind> <target id or 0> <seven grade amounts>", false, true, ["itemchoice"]);
	}
	public ItemEmpowerEffect(XElement root, IMagicSpell spell)
	{
		_definition = new(root); Spell = spell;
		if ((int?)root.Attribute("version") != 1) throw new FormatException("Item empowerment requires version 1.");
		ValidateOptions();
	}
	public IMagicSpell Spell { get; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger is CastingTriggerItemChoice;
	public XElement SaveToXml() => new(_definition);
	public IMagicSpellEffectTemplate Clone() => new ItemEmpowerEffect(SaveToXml(), Spell);
	public string Show(ICharacter actor) => "Equipped item choices: " + Options.Select(x => x.Name.ColourName()).ListToString();
	private void ValidateOptions()
	{
		var options = Options;
		if (options.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != options.Length ||
			options.Any(x => string.IsNullOrWhiteSpace(x.Name) || x.Name.Any(char.IsWhiteSpace) || !Enum.IsDefined(x.Kind) ||
				x.Amounts.Length != 7 || x.Amounts.Any(a => !double.IsFinite(a) || a < 0) ||
				(x.Kind is EquippedItemBonusKind.Trait or EquippedItemBonusKind.MagicResource ? x.TargetId <= 0 : x.TargetId != 0)))
			throw new FormatException("Item enchantment choices require unique names, exact targets and seven finite non-negative amounts.");
	}
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("option")) return false;
		var name = command.PopSpeech();
		if (!Enum.TryParse<EquippedItemBonusKind>(command.PopSpeech(), true, out var kind) ||
			!long.TryParse(command.PopSpeech(), out var target)) return false;
		var amounts = new List<double>();
		while (!command.IsFinished)
		{
			if (!double.TryParse(command.PopSpeech(), NumberStyles.Float, actor, out var amount)) return false;
			amounts.Add(amount);
		}
		var candidate = SaveToXml();
		candidate.Elements("Option").Where(x => ((string?)x.Attribute("name")).EqualTo(name)).Remove();
		candidate.Add(new XElement("Option", new XAttribute("name", name), new XAttribute("kind", kind),
			new XAttribute("target", target), amounts.Select(x => new XElement("Amount", x))));
		try { _ = new ItemEmpowerEffect(candidate, Spell); } catch (FormatException) { return false; }
		_definition.ReplaceNodes(candidate.Nodes()); Spell.Changed = true;
		actor.OutputHandler.Send($"Configured the {name.ColourName()} equipped item bonus."); return true;
	}
	public bool TryBindInvocationInputs(ICharacter caster, IPerceivable recipient,
		IReadOnlyList<SpellAdditionalParameter> parameters, out string? error)
	{
		error = "Select one configured enchantment and one exact carried item at an available grade.";
		if (Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native || parameters.Count != 1 ||
			parameters[0].ParameterName != "item-enchantment-choice" || parameters[0].Item is not string name ||
			recipient is not IGameItem item) return false;
		var selected = Options.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (selected is null || selected.Amounts[native.InvocationGrade.Value - 1] <= 0 ||
			selected.Kind == EquippedItemBonusKind.Trait && Spell.Gameworld.Traits.Get(selected.TargetId) is not IAttributeDefinition ||
			selected.Kind == EquippedItemBonusKind.MagicResource && Spell.Gameworld.MagicResources.Get(selected.TargetId)?.ResourceType.HasFlag(MagicResourceType.PlayerResource) != true) return false;
		if (_selected is not null && (_selected.Name != selected.Name || !ReferenceEquals(_item, item) || !ReferenceEquals(_caster, caster))) return false;
		_selected = selected; _item = item; _caster = caster; _amount = selected.Amounts[native.InvocationGrade.Value - 1];
		if (!Current()) return false;
		error = null; return true;
	}
	private bool Current()
	{
		bool Carried() => _item is { Deleted: false, Destroyed: false } && _caster is not null &&
			ReferenceEquals(_item.InInventoryOf, _caster.Body) && _caster.Body.ExternalItems.Any(x => ReferenceEquals(x, _item));
		return _selected is not null && Carried() && _caster!.CanSee(_item!) &&
			Spell.Trigger is CastingTriggerItemChoice trigger && trigger.AllowsTarget(_item!, _caster) && Carried();
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) =>
		Current() && ReferenceEquals(caster, _caster) && ReferenceEquals(target, _item)
			? new SpellEquippedItemBonusEffect(_item!, parent, _selected!.Kind, _selected.TargetId, _amount) : null;
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var effect = GetOrApplyEffect(caster, target, outcome, power, parent, additionalParameters);
		return new(effect is null ? MagicEffectOperationStatus.Rejected : MagicEffectOperationStatus.Applied, effect);
	}
	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan duration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null; error = "The selected enchantment or exact carried item is no longer current.";
		if (!Current() || !ReferenceEquals(caster, _caster) || !ReferenceEquals(target, _item)) return false;
		application = new Application(this, caster, target, outcome, power); error = null; return true;
	}
	private sealed record Application(ItemEmpowerEffect Template, ICharacter Caster, IPerceivable Target,
		OpposedOutcomeDegree Outcome, SpellPower Power) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) =>
			Template.GetOrApplyEffect(Caster, Target, Outcome, Power, parent, []) ??
			throw new InvalidOperationException("The exact empowered item moved before application.");
	}
}
