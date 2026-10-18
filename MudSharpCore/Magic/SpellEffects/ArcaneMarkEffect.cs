#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Freezes a directly carried item's marking room; this is not a Rune's live location.</summary>
public sealed class ArcaneMarkEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection,
	IMagicSpellEffectAdmission, IMagicInterdictionTagProvider
{
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record Selection(ICharacter Caster, IGameItem Item, IRoom SavedRoom, int Grade,
		string Definition, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
	}

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("arcanemark", (xml, spell) => new ArcaneMarkEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("arcanemark", (_, spell) =>
			(new ArcaneMarkEffect(new XElement("Effect", new XAttribute("type", "arcanemark"),
				new XElement("Value", "An angular arcane glyph.")), spell), string.Empty),
			"Stores an exact carried item's marked room for later admitted travel",
			"Use value <description> to edit the mark's informational text. The saved-room identity is captured by casting, never typed by a player.",
			false, true, SpellTriggerFactory.MagicTriggerTypes.Where(x =>
				SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "item" or "items").ToArray());
	}

	public ArcaneMarkEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		Value = (string?)xml.Element("Value") ?? string.Empty;
		if (Value.Length > 256) throw new FormatException("Arcane Mark text exceeds 256 characters.");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Value { get; private set; }
	public IEnumerable<MagicInterdictionTag> MagicInterdictionTags =>
		[new(ArmageddonSimpleStockContent.ArcaneMarkTag, Value)];
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "arcanemark"), new XElement("Value", new XCData(Value)));
	public IMagicSpellEffectTemplate Clone() => new ArcaneMarkEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes is "item" or "items";
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Arcane Mark",
		("Stored Destination", "caster's room at application".ColourValue()), ("Text", Value.ColourValue()));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualTo("value") || command.IsFinished || command.SafeRemainingArgument.Length > 256)
		{
			actor.OutputHandler.Send("Use value <description> (at most 256 characters)."); return false;
		}
		Value = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor));
		return true;
	}

	private bool Eligible(ICharacter caster, IGameItem item) =>
		item.Id > 0 && !item.Deleted && !item.Destroyed && caster.Body is { } body &&
		ReferenceEquals(body.Actor, caster) && ReferenceEquals(item.InInventoryOf, body) &&
		item.ContainedIn is null && ReferenceEquals(caster.Gameworld, Gameworld) &&
		ReferenceEquals(item.Gameworld, Gameworld) && ReferenceEquals(Gameworld.Items.Get(item.Id), item) &&
		caster.Location is { Id: > 0 } room && ReferenceEquals(Gameworld.Rooms.Get(room.Id), room) &&
		item.TrueLocations.Contains(room);

	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return existing;
		}
		if (recipient is not IGameItem item || !Eligible(caster, item) ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native)
			throw new InvalidOperationException("Arcane Mark requires a directly carried item and a selected grade.");
		var room = caster.Location;
		var body = caster.Body;
		var identity = caster.Identity;
		var instance = caster.InstanceId;
		var layer = caster.RoomLayer;
		var grade = native.InvocationGrade.Value;
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var token = new Selection(caster, item, room, grade, definition, () =>
			Eligible(caster, item) && ReferenceEquals(caster.Body, body) &&
			ReferenceEquals(caster.Identity, identity) && caster.InstanceId == instance &&
			ReferenceEquals(caster.Location, room) && caster.RoomLayer == layer &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		if (!token.IsCurrent) throw new InvalidOperationException("Arcane Mark's item or room changed during selection.");
		_selections.Add(recipient, token);
		return token;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The prepared Arcane Mark item, room or caster changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) ||
			!ReferenceEquals(token.Item, recipient) || !token.IsCurrent) return false;
		_selections[recipient] = token;
		error = null;
		return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "No current Arcane Mark selection was prepared.";
		return _selections.TryGetValue(recipient, out var token) &&
			TryReusePreparedSelection(token, caster, recipient, out error);
	}

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		error = "Arcane Mark requires a directly carried item and positive bounded duration.";
		if (target is not IGameItem item || !Eligible(caster, item) ||
			resolvedDuration <= TimeSpan.Zero || resolvedDuration > TimeSpan.FromDays(365)) return false;
		if (!_selections.ContainsKey(target)) CapturePreparedSelection(caster, target);
		if (!TryConfirmPreparedSelection(caster, target, out error)) return false;
		application = new Application(this, _selections[target]);
		return true;
	}

	private sealed record Application(ArcaneMarkEffect Template, Selection Selection) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			if (!Selection.IsCurrent || !CommandExecutionScope.TryContinue() ||
				!ReferenceEquals(parent.Owner, Selection.Item))
				throw new InvalidOperationException("Arcane Mark's saved-room frame changed before application.");
			return new SpellMagicTagEffect(Selection.Item, parent, ArmageddonSimpleStockContent.ArcaneMarkTag,
				Template.Value, savedRoomId: Selection.SavedRoom.Id, remainingUses: Selection.Grade);
		}
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		string? error = null;
		IMagicSpellEffectApplication? application = null;
		if (target is null || parent is not MagicSpellParent native ||
			!TryPrepareApplication(caster, target, outcome, power, native.ResolvedDuration, out application, out error))
			throw new InvalidOperationException(error ?? "Arcane Mark was not admitted.");
		return application!.Create(parent);
	}
}
