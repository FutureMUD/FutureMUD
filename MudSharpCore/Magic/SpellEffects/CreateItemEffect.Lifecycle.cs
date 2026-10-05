#nullable enable

using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Lifecycle;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class CreateItemEffect
{
	private string? _lifecycleLoadError;
	private XElement? _unreadableLifecycle;
	private string? _lifetimeFormula;
	private long _permanentPrototypeId;
	private double? _preparedLifetimeSeconds;
	public SpellLifecycleMode? LifecycleMode { get; private set; }
	public string LifecycleFamily { get; private set; } = "";
	public int? PermanentGrade { get; private set; }
	public IGameItemProto? PermanentPrototype => Gameworld.ItemProtos.Get(_permanentPrototypeId);
	public ITraitExpression? LifetimeExpression { get; internal set; }
	public bool CountByGrade { get; private set; }
	public bool WornLight { get; private set; }
	public bool PrimaryHand { get; private set; }

	public string? DefinitionError => _lifecycleLoadError ?? (LifecycleMode is null ? null :
		Gameworld.SpellOwnedItems is null ? "Spell-owned native item creation is unavailable." :
		Quantity != 1 || _itemSkinId != 0 || !string.IsNullOrEmpty(LoadString) ? "Lifecycle items require quantity one per output, no skin and no load string." :
		string.IsNullOrWhiteSpace(LifecycleFamily) || LifecycleFamily.Length > 128 ? "Set a creation family of at most 128 characters." :
		OutputPolicyError() is { } policyError ? policyError :
		NativeItemCreationEligibility.Error(ItemPrototype, Gameworld) is { } error ? error :
		PermanentGrade is not null && NativeItemCreationEligibility.Error(PermanentPrototype, Gameworld) is { } permanentError ? permanentError :
		PermanentGrade is not null && PermanentPrototype!.IsItemType<ProgLightGameItemComponentProto>() ? "Permanent grade overrides cannot select lights; configure explicit worn-light placement." :
		CountByGrade && (!ItemPrototype.IsItemType<FoodGameItemComponentProto>() || PermanentGrade is not null) ? "Grade-count creation requires plain food without a permanent grade override." :
		WornLight && (!ItemPrototype.IsItemType<ProgLightGameItemComponentProto>() || CountByGrade || PermanentGrade is not null) ? "Worn-light placement requires one wearable light without a permanent grade override." :
		PrimaryHand && (CountByGrade || !ItemPrototype.IsItemType<MeleeWeaponGameItemComponentProto>() ||
			PermanentGrade is not null && !PermanentPrototype!.IsItemType<MeleeWeaponGameItemComponentProto>()) ? "Primary-hand placement requires one native melee weapon at each output grade." :
		ItemPrototype.IsItemType<ProgLightGameItemComponentProto>() && !WornLight ? "Lifecycle lights require worn-light placement." :
		LifecycleMode == SpellLifecycleMode.TemporaryCleanup && (LifetimeExpression is null || LifetimeExpression.HasErrors()) ? "Set a valid lifetime expression in real seconds." :
		LifetimeExpression?.NonTraitParameters.Contains("outcome", StringComparer.OrdinalIgnoreCase) == true ? "Item lifetime must be determinable before payment; outcome is not supported." :
		ItemQuality.HasErrors() ? "Set a valid quality expression." : null);

	private void LoadLifecycle(XElement? root)
	{
		if (root is null) return;
		if (!int.TryParse((string?)root.Attribute("version"), out var version) || version != 1 ||
			!Enum.TryParse<SpellLifecycleMode>((string?)root.Attribute("mode"), true, out var mode) ||
			mode is not (SpellLifecycleMode.Permanent or SpellLifecycleMode.TemporaryCleanup) ||
			root.Element("Count") is { } count && count.Value is not ("single" or "grade") ||
			root.Element("Placement") is { } placement && placement.Value is not ("standard" or "wornlight" or "primaryhand") ||
			root.Element("PermanentOutput") is { } output &&
			(!int.TryParse((string?)output.Attribute("grade"), out var grade) || grade is < 1 or > 7 ||
			 !long.TryParse(output.Value, out var prototypeId) || prototypeId <= 0))
		{
			_lifecycleLoadError = "Invalid item lifecycle schema, mode or permanent grade output.";
			_unreadableLifecycle = new(root); return;
		}
		LifecycleMode = mode; LifecycleFamily = root.Element("Family")?.Value ?? "";
		try { LoadOutputPolicies(root); }
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			_lifecycleLoadError = "Invalid item output policy: " + ex.Message;
			_unreadableLifecycle = new(root); return;
		}
		CountByGrade = root.Element("Count")?.Value == "grade";
		WornLight = root.Element("Placement")?.Value == "wornlight";
		PrimaryHand = root.Element("Placement")?.Value == "primaryhand";
		_lifetimeFormula = root.Element("Seconds")?.Value;
		if (_lifetimeFormula is not null) LifetimeExpression = new TraitExpression(_lifetimeFormula, Gameworld);
		if (root.Element("PermanentOutput") is { } permanent)
		{ PermanentGrade = int.Parse(permanent.Attribute("grade")!.Value); _permanentPrototypeId = long.Parse(permanent.Value); }
	}

	private XElement? SaveLifecycle() => _unreadableLifecycle is not null ? new(_unreadableLifecycle) : LifecycleMode is { } mode
		? new("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", mode), new XElement("Family", LifecycleFamily),
			_lifetimeFormula is not null ? new XElement("Seconds", _lifetimeFormula) : null,
			CountByGrade ? new XElement("Count", "grade") : null,
			WornLight ? new XElement("Placement", "wornlight") : PrimaryHand ? new XElement("Placement", "primaryhand") : null,
			PermanentGrade is { } grade ? new XElement("PermanentOutput", new XAttribute("grade", grade), _permanentPrototypeId) : null,
			SaveOutputPolicies())
		: null;

	internal bool ValidateInvocation(ICharacter caster, out string? error) => ValidateRecipientInvocation(caster, null, out error);

	internal bool ValidateRecipientInvocation(ICharacter caster, IPerceivable? recipient, out string? error)
	{
		error = DefinitionError;
		if (error is not null || LifecycleMode is null) return error is null;
		if (Spell is not MagicSpell { InvocationGrade: { } grade } native)
		{ error = "Lifecycle item creation requires a selected-grade native casting invocation."; return false; }
		try
		{
			if (_eligibilityProgId != 0 && EligibilityProg!.ExecuteBool(caster) != true)
			{ error = "The configured environment prevents this item creation."; return false; }
		}
		catch (Exception ex) { error = "Item eligibility could not be evaluated: " + ex.Message; return false; }
		if (WornLight && recipient is not null)
		{
			if (recipient is not ICharacter character || !ReferenceEquals(character.Gameworld, Gameworld))
			{ error = "A worn light requires a character recipient in this world."; return false; }
			var preview = new GameItem(ItemPrototype, caster, MudSharp.GameItems.ItemQuality.Standard, deferSpellInitialisation: true);
			var profile = ItemPrototype.GetItemType<WearableGameItemComponentProto>().DefaultProfile;
			if (profile.Profile(character.Body) is not { Count: > 0 } || !character.Body.CanWear(preview, profile))
			{ error = "The recipient cannot wear the configured light profile."; return false; }
		}
		if (PrimaryHand)
		{
			if (recipient is not ICharacter character || !ReferenceEquals(character.Gameworld, Gameworld))
			{ error = "Primary-hand placement requires a character recipient in this world."; return false; }
			var prototype = PreparePrototype(grade);
			var preview = new GameItem(prototype, caster, MudSharp.GameItems.ItemQuality.Standard, deferSpellInitialisation: true);
			var hand = PrimaryWieldHand(character);
			if (hand is null || hand.Hands(preview) != 1 || !character.Body.CanGet(preview, 0) || !character.Body.CanWield(preview, hand))
			{ error = "The recipient needs a usable free primary hand and capacity for the created weapon."; return false; }
		}
		if (LifecycleMode == SpellLifecycleMode.Permanent || PermanentGrade == grade) return true;
		if (_preparedLifetimeSeconds is not null) return true;
		try
		{
			var seconds = LifetimeExpression!.Evaluate(caster, native.CastingTrait, TraitBonusContext.SpellDuration);
			if (_lifetimeMultiplierProgId != 0) seconds *= LifetimeMultiplierProg!.ExecuteDouble(caster);
			if (!double.IsFinite(seconds) || seconds <= 0 || seconds > (DateTime.MaxValue - RuntimeClock.UtcNow).TotalSeconds ||
				TimeSpan.FromSeconds(seconds) <= TimeSpan.Zero)
				error = "The item lifetime must be finite, positive and representable as an absolute UTC deadline.";
			else _preparedLifetimeSeconds = seconds;
		}
		catch (Exception ex) { error = "Item lifetime could not be prepared: " + ex.Message; }
		return error is null;
	}

	public bool TryPrepareApplication(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome,
		SpellPower power, TimeSpan resolvedDuration, out IMagicSpellEffectApplication? application, out string? error)
	{
		application = null;
		if (!ValidateRecipientInvocation(caster, target, out error)) return false;
		if (LifecycleMode is null)
		{ application = new LegacyItemCreation(this, caster, target, outcome, power); return true; }
		if (!ReferenceEquals(caster.Gameworld, Gameworld) || !ReferenceEquals(target.Gameworld, Gameworld) || target is not (ICharacter or ICell or IGameItem))
		{ error = "The item recipient must be a supported target in this world."; return false; }
		var native = (MagicSpell)Spell; var grade = native.InvocationGrade!.Value;
		var mode = PermanentGrade == grade ? SpellLifecycleMode.Permanent : LifecycleMode!.Value;
		var prototype = PreparePrototype(grade);
		try
		{
			var qualityValue = Math.Floor(ItemQuality.EvaluateDoubleWith(("base", (int)prototype.BaseItemQuality), ("power", (int)power), ("outcome", (int)outcome)));
			if (!double.IsFinite(qualityValue) || qualityValue < int.MinValue || qualityValue > int.MaxValue || !Enum.IsDefined((ItemQuality)(int)qualityValue))
			{ error = "The created item quality must resolve to a native quality."; return false; }
			application = new NativeItemCreation(Enumerable.Range(0, CountByGrade ? grade : 1).Select(_ => Guid.NewGuid()).ToArray(), this, caster, target, prototype, (ItemQuality)(int)qualityValue, grade, mode,
				mode == SpellLifecycleMode.Permanent ? null : _preparedLifetimeSeconds, native.InvocationOriginId);
			return true;
		}
		catch (Exception ex) { error = "Native item creation could not be prepared: " + ex.Message; return false; }
	}

	private sealed record LegacyItemCreation(CreateItemEffect Effect, ICharacter Caster, IPerceivable Target,
		OpposedOutcomeDegree Outcome, SpellPower Power) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent) => Effect.GetOrApplyEffect(Caster, Target, Outcome, Power, parent, []);
	}

	private sealed record NativeItemCreation(Guid[] Ids, CreateItemEffect Effect, ICharacter Caster, IPerceivable Target,
		IGameItemProto Prototype, ItemQuality Quality, int Grade, SpellLifecycleMode Mode, double? Seconds, Guid? Invocation) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			var now = RuntimeClock.UtcNow;
			for (var i = 0; i < Ids.Length; ++i)
			{
			var origin = new SpellLifecycleOrigin(Ids[i], Effect.Spell.Id, Grade, CharacterInstanceIdentityComparer.IdentityId(Caster),
				Effect.LifecycleFamily, Mode, now, Seconds is { } seconds ? now.AddSeconds(seconds) : null,
				$"native-createitem; output={i + 1}/{Ids.Length}; prototype={Prototype.Id}/{Prototype.RevisionNumber}; invocation={Invocation}; parent={(parent as MagicSpellParent)?.Identity}");
			var item = Effect.Gameworld.SpellOwnedItems!.Create(Prototype, Caster, Quality, origin);
			Effect.PlaceOwnedItem(item, Caster, Target);
			}
			return null!;
		}
	}

	private void PlaceOwnedItem(IGameItem item, ICharacter caster, IPerceivable target)
	{
		Gameworld.Add(item);
		item.SetOwner(target as ICharacter ?? caster);
		if (PrimaryHand && target is ICharacter wielder)
		{
			var hand = PrimaryWieldHand(wielder);
			if (hand is null || hand.Hands(item) != 1 || !wielder.Body.CanGet(item, 0) || !wielder.Body.CanWield(item, hand))
				throw new InvalidOperationException("The admitted primary hand is no longer available for the created weapon.");
			var gotten = wielder.Body.Get(item, 0, null, true, ItemCanGetIgnore.None, null);
			if (!ReferenceEquals(gotten, item) || !ReferenceEquals(item.InInventoryOf, wielder.Body) ||
				item.ContainedIn is not null || !wielder.Body.HeldItems.Contains(item))
				throw new InvalidOperationException("The created weapon changed custody while entering the admitted primary hand.");
			if (!wielder.Body.Wield(item, hand, silent: true) ||
				!ReferenceEquals(item.InInventoryOf, wielder.Body) || item.ContainedIn is not null ||
				!wielder.Body.WieldedItems.Contains(item) || !ReferenceEquals(item.GetItemType<IWieldable>().PrimaryWieldedLocation, hand))
				throw new InvalidOperationException("The created weapon could not enter the admitted primary hand.");
		}
		else if (WornLight && target is ICharacter recipient)
		{
			item.Get(recipient.Body);
			recipient.Body.WearExternally(item, item.GetItemType<IWearable>().DefaultProfile);
			if (!recipient.Body.WornItems.Contains(item)) throw new InvalidOperationException("The created light could not enter its admitted wear profile.");
		}
		else if (target is ICharacter character && character.Body.CanGet(item, 0)) character.Body.Get(item, silent: true);
		else if (target is IGameItem host && host.GetItemType<IContainer>() is { } container && container.CanPut(item)) container.Put(null, item, false);
		else if (target is IGameItem sheathHost && sheathHost.GetItemType<ISheath>() is { } sheath && sheath.CanSheath(item)) sheath.Content = item.GetItemType<IWieldable>();
		else if (target is ICell cell && !ReferenceEquals(cell, caster.Location)) cell.Insert(item, true);
		else { item.RoomLayer = target.RoomLayer; item.InsertAtSource(target is IGameItem outputHost ? outputHost.LocationLevelPerceivable : target is ICell ? caster : target, true); }
		item.HandleEvent(EventType.ItemFinishedLoading, item); item.Login();
	}

	internal static IWield? PrimaryWieldHand(ICharacter recipient) => recipient.Body.WieldLocs
		.Where(x => x.Alignment.LeftRightOnly() == recipient.Body.Handedness.LeftRightOnly())
		.OrderBy(x => x.Id)
		.FirstOrDefault();

	private bool BuildingCommandLifecycle(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "lifecycle":
				var text = command.PopSpeech();
				if (text.EqualTo("legacy")) LifecycleMode = null;
				else if (Enum.TryParse<SpellLifecycleMode>(text, true, out var mode) && mode is SpellLifecycleMode.Permanent or SpellLifecycleMode.TemporaryCleanup) LifecycleMode = mode;
				else { actor.OutputHandler.Send("Specify legacy, permanent or temporarycleanup."); return false; }
				_lifecycleLoadError = null; _unreadableLifecycle = null; break;
			case "family":
				if (string.IsNullOrWhiteSpace(command.SafeRemainingArgument) || command.SafeRemainingArgument.Length > 128)
				{ actor.OutputHandler.Send("Specify a family of at most 128 characters."); return false; }
				LifecycleFamily = command.SafeRemainingArgument; break;
			case "count":
				var count = command.PopSpeech().ToLowerInvariant();
				if (count is not ("single" or "grade")) { actor.OutputHandler.Send("Specify single or grade (plain food only)."); return false; }
				CountByGrade = count == "grade"; break;
			case "placement":
				var placement = command.PopSpeech().ToLowerInvariant();
				if (placement is not ("standard" or "wornlight" or "primaryhand")) { actor.OutputHandler.Send("Specify standard, wornlight or primaryhand."); return false; }
				WornLight = placement == "wornlight"; PrimaryHand = placement == "primaryhand"; break;
			case "lifetime":
				var expression = new TraitExpression(command.SafeRemainingArgument, Gameworld);
				if (expression.HasErrors() || expression.NonTraitParameters.Contains("outcome", StringComparer.OrdinalIgnoreCase))
				{ actor.OutputHandler.Send("Specify a valid real-seconds lifetime determinable before the casting check."); return false; }
				LifetimeExpression = expression; _lifetimeFormula = command.SafeRemainingArgument; break;
			case "permanent":
				var gradeText = command.PopSpeech();
				if (gradeText.EqualTo("none")) { PermanentGrade = null; _permanentPrototypeId = 0; break; }
				if (!int.TryParse(gradeText, out var grade) || grade is < 1 or > 7 || Gameworld.ItemProtos.GetByIdOrName(command.SafeRemainingArgument) is not { } prototype ||
					NativeItemCreationEligibility.Error(prototype, Gameworld) is not null)
				{ actor.OutputHandler.Send("Specify an exact configured grade 1-7 and an approved plain-item prototype, or none."); return false; }
				PermanentGrade = grade; _permanentPrototypeId = prototype.Id; break;
		}
		Spell.Changed = true; actor.OutputHandler.Send("Item creation lifecycle configuration updated."); return true;
	}
}
