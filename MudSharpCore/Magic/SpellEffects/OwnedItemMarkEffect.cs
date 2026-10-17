#nullable enable

using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Exact, owner-scoped item marking for the two S4 item spells.</summary>
public sealed class OwnedItemMarkEffect : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection,
	IMagicSpellEffectAdmission
{
	private readonly Dictionary<IPerceivable, Selection> _selections = new(ReferenceEqualityComparer.Instance);
	private sealed record Selection(ICharacter Caster, IGameItem Item, ItemOwnershipReference Owner,
		int Grade, string Definition, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
	}

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("owneditemmark", (xml, spell) => new OwnedItemMarkEffect(xml, spell));
		SpellEffectFactory.RegisterBuilderFactory("owneditemmark", (_, spell) =>
			(new OwnedItemMarkEffect(new XElement("Effect", new XAttribute("type", "owneditemmark"),
				new XElement("Mode", "glyph")), spell), string.Empty),
			"Marks an owned item as a custodian glyph or travel anchor",
			HelpText, false, true,
			SpellTriggerFactory.MagicTriggerTypes.Where(x =>
				SpellTriggerFactory.BuilderInfoForType(x).TargetTypes is "item" or "items").ToArray());
	}

	public OwnedItemMarkEffect(XElement xml, IMagicSpell spell)
	{
		Spell = spell;
		Mode = (string?)xml.Element("Mode") ?? string.Empty;
		if (Mode is not ("glyph" or "anchor")) throw new FormatException("Use glyph or anchor for an owned item mark.");
	}

	public IMagicSpell Spell { get; }
	public IFuturemud Gameworld => Spell.Gameworld;
	public string Mode { get; private set; }
	public bool IsInstantaneous => false;
	public bool RequiresTarget => true;
	public const string HelpText = "Use mode glyph to refuse unauthorised taking, or mode anchor to register an exact owned travel item. Both require a legally owned, local item and a positive spell duration.";
	public XElement SaveToXml() => new("Effect", new XAttribute("type", "owneditemmark"), new XElement("Mode", Mode));
	public IMagicSpellEffectTemplate Clone() => new OwnedItemMarkEffect(SaveToXml(), Spell);
	public bool IsCompatibleWithTrigger(IMagicTrigger trigger) => trigger.TargetTypes is "item" or "items";
	public string Show(ICharacter actor) => SpellEffectPresentation.Describe(actor, "Owned Item Mark", ("Mode", Mode.ColourName()));
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopSpeech().ToLowerInvariant() != "mode" ||
			command.SafeRemainingArgument.ToLowerInvariant() is not ("glyph" or "anchor") )
		{
			actor.OutputHandler.Send(HelpText);
			return false;
		}
		Mode = command.SafeRemainingArgument.ToLowerInvariant();
		Spell.Changed = true;
		actor.OutputHandler.Send(Show(actor));
		return true;
	}

	private bool Eligible(ICharacter caster, IGameItem item) =>
		!item.Deleted && !item.Destroyed && item.Id > 0 && item.IsOwnedBy(caster) &&
		item.OwnershipReference is { Id: > 0 } && caster.Body is { } body &&
		ReferenceEquals(body.Actor, caster) && ReferenceEquals(caster.Gameworld, Gameworld) &&
		ReferenceEquals(item.Gameworld, Gameworld) && ReferenceEquals(Gameworld.Items.Get(item.Id), item) &&
		caster.Location is { } room && item.TrueLocations.Contains(room) &&
		caster.CanReachItem(item, requireInventoryPermission: false).Truth;

	public IMagicSpellEffectPreparedSelectionToken CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (_selections.TryGetValue(recipient, out var existing))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return existing;
		}
		if (recipient is not IGameItem item || !Eligible(caster, item) ||
			Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native)
			throw new InvalidOperationException("An owned item mark needs a reachable, legally owned item and a selected grade.");
		var owner = item.OwnershipReference!.Value;
		var body = caster.Body;
		var identity = caster.Identity;
		var instance = caster.InstanceId;
		var room = caster.Location;
		var layer = caster.RoomLayer;
		var location = item.Location;
		var container = item.ContainedIn;
		var custodian = item.InInventoryOf;
		var grade = native.InvocationGrade.Value;
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var token = new Selection(caster, item, owner, grade, definition, () =>
			Eligible(caster, item) && ReferenceEquals(caster.Body, body) &&
			ReferenceEquals(caster.Identity, identity) && caster.InstanceId == instance &&
			ReferenceEquals(caster.Location, room) && caster.RoomLayer == layer &&
			ReferenceEquals(item.Location, location) && ReferenceEquals(item.ContainedIn, container) &&
			ReferenceEquals(item.InInventoryOf, custodian) && item.OwnershipReference == owner &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		if (!token.IsCurrent) throw new InvalidOperationException("The owned item changed during mark selection.");
		_selections.Add(recipient, token);
		return token;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The prepared item, owner, custody or caster changed.";
		if (selection is not Selection token || !ReferenceEquals(token.Caster, caster) ||
			!ReferenceEquals(token.Item, recipient) || !token.IsCurrent) return false;
		_selections[recipient] = token;
		error = null;
		return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = "No current owned item mark was prepared.";
		return _selections.TryGetValue(recipient, out var token) &&
			TryReusePreparedSelection(token, caster, recipient, out error);
	}

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		error = "The owned item mark has no current item, owner or positive bounded duration.";
		if (target is not IGameItem item || !Eligible(caster, item) ||
			resolvedDuration <= TimeSpan.Zero || resolvedDuration > TimeSpan.FromDays(365)) return false;
		if (!_selections.ContainsKey(target)) CapturePreparedSelection(caster, target);
		if (!TryConfirmPreparedSelection(caster, target, out error)) return false;
		application = new Application(this, _selections[target]);
		return true;
	}

	private sealed record Application(OwnedItemMarkEffect Template, Selection Selection) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			if (!Selection.IsCurrent || !CommandExecutionScope.TryContinue() ||
				!ReferenceEquals(parent.Owner, Selection.Item))
				throw new InvalidOperationException("The owned item mark changed before application.");
			return Template.Mode == "glyph"
				? new SpellCustodianGlyphEffect(Selection.Item, parent, Selection.Owner)
				: new SpellAnchorRuneEffect(Selection.Item, parent, Selection.Owner);
		}
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target,
		OpposedOutcomeDegree outcome, SpellPower power, IMagicSpellEffectParent parent,
		SpellAdditionalParameter[] additionalParameters)
	{
		string? error = null;
		if (target is null || parent is not MagicSpellParent native ||
			!TryPrepareApplication(caster, target, outcome, power, native.ResolvedDuration, out var application, out error))
			throw new InvalidOperationException(error ?? "The owned item mark was not admitted.");
		return application!.Create(parent);
	}
}
