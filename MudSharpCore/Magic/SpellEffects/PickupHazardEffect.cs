#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Applies permanent one-shot item HP/stun hazards without imposing legal ownership.</summary>
public sealed class PickupHazardEffect : IMagicSpellEffectTemplate, IMagicSpellEffectOperation, IMagicSpellEffectPreparedSelection
{
	private sealed record Selection(ICharacter Caster, IGameItem Item, int Grade, string Definition, Func<bool> Check)
		: IMagicSpellEffectPreparedSelectionRawToken { public bool IsCurrent => Check(); }
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	public PickupHazardEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		DamageSides = (int?)xml.Element("DamageSides") ?? 4;
		StunSides = (int?)xml.Element("StunSides") ?? 6;
		DicePerGrade = (int?)xml.Element("DicePerGrade") ?? 1;
		ImmunityProgId = (long?)xml.Element("ImmunityProg") ?? 0;
		MinimumDispelGrade = (int?)xml.Element("MinimumDispelGrade") ?? 1;
		Echo = (string?)xml.Element("Echo") ?? "An enchantment flares as you try to take it.";
		Validate();
	}
	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public int DamageSides { get; private set; }
	public int StunSides { get; private set; }
	public int DicePerGrade { get; private set; }
	public long ImmunityProgId { get; private set; }
	public int MinimumDispelGrade { get; private set; }
	public string Echo { get; private set; }
	public bool IsInstantaneous => true;
	public bool RequiresTarget => true;
	private void Validate()
	{
		if (DamageSides < 1 || DamageSides > Dice.MaximumSidesPerDie || StunSides < 1 || StunSides > Dice.MaximumSidesPerDie ||
			DicePerGrade < 1 || DicePerGrade > Dice.MaximumDiceCount / 7 || ImmunityProgId < 0 || MinimumDispelGrade < 1)
			throw new FormatException("Invalid pickup hazard dice or dispel policy.");
	}
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "pickuphazard"),
		new XElement("DamageSides", DamageSides), new XElement("StunSides", StunSides), new XElement("DicePerGrade", DicePerGrade),
		new XElement("ImmunityProg", ImmunityProgId), new XElement("MinimumDispelGrade", MinimumDispelGrade), new XElement("Echo", new XCData(Echo)));
	public IMagicSpellEffectTemplate Clone() => new PickupHazardEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes is "item" or "items";
	public string Show(ICharacter actor) => $"Permanent pickup hazard: {DicePerGrade.ToString("N0", actor)} dice/grade, d{DamageSides.ToString("N0", actor)} HP and d{StunSides.ToString("N0", actor)} stun; immunity prog #{ImmunityProgId.ToString("N0", actor)}, minimum dispel grade {MinimumDispelGrade.ToString("N0", actor)}.";
	public const string Help = "Use dice <dice per grade> <HP sides> <stun sides>, immunity <boolean(character,item) prog|none>, dispel <minimum grade>, or echo <text>. This creates a permanent owner-agnostic, one-shot HP/stun pickup hazard; native HP strategies are required for harm.";
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "dice":
				if (!int.TryParse(command.PopSpeech(), out var count) || count < 1 || count > Dice.MaximumDiceCount / 7 ||
					!int.TryParse(command.PopSpeech(), out var hp) || hp < 1 || hp > Dice.MaximumSidesPerDie ||
					!int.TryParse(command.PopSpeech(), out var stun) || stun < 1 || stun > Dice.MaximumSidesPerDie || !command.IsFinished)
				{ actor.Send(Help); return false; }
				DicePerGrade = count; DamageSides = hp; StunSides = stun; break;
			case "immunity":
				var name = command.SafeRemainingArgument;
				var prog = name.EqualTo("none") ? null : Gameworld.FutureProgs.GetByIdOrName(name);
				if (!name.EqualTo("none") && (prog is null || prog.ReturnType != ProgVariableTypes.Boolean ||
					!prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Item]))) { actor.Send(Help); return false; }
				ImmunityProgId = prog?.Id ?? 0; break;
			case "dispel":
				if (!int.TryParse(command.PopSpeech(), out var grade) || grade < 1 || !command.IsFinished) { actor.Send(Help); return false; }
				MinimumDispelGrade = grade; break;
			case "echo": Echo = command.SafeRemainingArgument; break;
			default: actor.Send(Help); return false;
		}
		Spell.Changed = true; actor.Send(Show(actor)); return true;
	}
	private bool Eligible(ICharacter caster, IGameItem item) => !item.Deleted && !item.Destroyed &&
		!item.Effects.OfType<ItemPickupHazardEffect>().Any(x => x.IsIntrinsicMagicActive) &&
		ReferenceEquals(item.Gameworld, Gameworld) && ReferenceEquals(caster.Gameworld, Gameworld) &&
		caster.Location is { } room && item.TrueLocations.Contains(room) && caster.CanSee(item) &&
		caster.CanReachItem(item, requireInventoryPermission: false).Truth;
	private Selection Capture(ICharacter caster, IGameItem item, int grade)
	{
		if (!Eligible(caster, item) || grade is < 1 or > 7) throw new InvalidOperationException("A hazard requires an eligible reachable local item and selected grade.");
		var body = caster.Body; var instance = caster.InstanceId; var identity = caster.Identity;
		var room = caster.Location; var layer = caster.RoomLayer; var location = item.Location;
		var container = item.ContainedIn; var holder = item.InInventoryOf;
		var previous = item.Effects.OfType<ItemPickupHazardEffect>().ToArray();
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var result = new Selection(caster, item, grade, definition, () => !item.Deleted && !item.Destroyed &&
			ReferenceEquals(caster.Body, body) && caster.InstanceId == instance && ReferenceEquals(caster.Identity, identity) &&
			ReferenceEquals(caster.Location, room) && caster.RoomLayer == layer && ReferenceEquals(item.Location, location) &&
			ReferenceEquals(item.ContainedIn, container) && ReferenceEquals(item.InInventoryOf, holder) &&
			item.Effects.OfType<ItemPickupHazardEffect>().SequenceEqual(previous) &&
			SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		_selections[item] = result; return result;
	}
	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return existing;
		}
		if (recipient is not IGameItem item || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } spell)
			throw new InvalidOperationException("A pickup hazard needs an item and a native selected grade.");
		return Capture(caster, item, spell.InvocationGrade.Value);
	}
	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "The prepared item hazard changed before payment.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) || !ReferenceEquals(token.Item, recipient) ||
			!token.IsCurrent || !Eligible(caster, token.Item) || !token.IsCurrent || token.Definition != SaveToXml().ToString(SaveOptions.DisableFormatting) ||
			Spell is MagicSpell { InvocationGrade: { } grade } && grade != token.Grade) return false;
		_selections[recipient] = token; error = null; return true;
	}
	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "No current item hazard selection exists.";
		return _selections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}
	public MagicEffectOperation Apply(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power,
		IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		if (target is not IGameItem item || !Eligible(caster, item)) return new(MagicEffectOperationStatus.Rejected, null);
		var grade = Spell is MagicSpell { InvocationGrade: { } selected } ? selected : Math.Clamp((int)power - (int)SpellPower.ExtremelyWeak + 1, 1, 7);
		if (!_selections.TryGetValue(item, out var selection)) selection = Capture(caster, item, grade);
		if (!TryConfirmPreparedSelection(caster, item, out _) || !selection.IsCurrent) return new(MagicEffectOperationStatus.Rejected, null);
		foreach (var spent in item.Effects.OfType<ItemPickupHazardEffect>().ToArray()) item.RemoveEffect(spent);
		var count = grade * DicePerGrade;
		item.AddEffect(new ItemPickupHazardEffect(item, $"{count}d{DamageSides}", $"{count}d{StunSides}", ImmunityProgId, MinimumDispelGrade, Echo));
		if (!item.Effects.OfType<ItemMagicAura>().Any()) item.AddEffect(new ItemMagicAura(item));
		_selections.Remove(item);
		return new(MagicEffectOperationStatus.Applied, null);
	}
	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome, SpellPower power,
		IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters) => Apply(caster, target, outcome, power, parent, additionalParameters).Effect;
	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("pickuphazard", (xml, spell) => new PickupHazardEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("pickuphazard", (_, spell) =>
			(new PickupHazardEffect(new XElement("Effect"), spell), ""), "Creates a permanent one-shot HP/stun item pickup hazard",
			Help, true, true, SpellTriggerFactory.MagicTriggerTypes.Where(x => SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "item" or "items").ToArray());
	}
}
