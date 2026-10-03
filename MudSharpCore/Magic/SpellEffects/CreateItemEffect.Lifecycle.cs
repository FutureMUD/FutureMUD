#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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

	public string? DefinitionError => _lifecycleLoadError ?? (LifecycleMode is null ? null :
		Gameworld.SpellOwnedItems is null ? "Spell-owned native item creation is unavailable." :
		Quantity != 1 || _itemSkinId != 0 || !string.IsNullOrEmpty(LoadString) ? "Lifecycle weapons require quantity one, no skin and no load string." :
		string.IsNullOrWhiteSpace(LifecycleFamily) || LifecycleFamily.Length > 128 ? "Set a creation family of at most 128 characters." :
		NativeItemCreationEligibility.Error(ItemPrototype, Gameworld) is { } error ? error :
		PermanentGrade is not null && NativeItemCreationEligibility.Error(PermanentPrototype, Gameworld) is { } permanentError ? permanentError :
		LifecycleMode == SpellLifecycleMode.TemporaryCleanup && (LifetimeExpression is null || LifetimeExpression.HasErrors()) ? "Set a valid lifetime expression in real seconds." :
		LifetimeExpression?.NonTraitParameters.Contains("outcome", StringComparer.OrdinalIgnoreCase) == true ? "Item lifetime must be determinable before payment; outcome is not supported." :
		ItemQuality.HasErrors() ? "Set a valid quality expression." : null);

	private void LoadLifecycle(XElement? root)
	{
		if (root is null) return;
		if (!int.TryParse((string?)root.Attribute("version"), out var version) || version != 1 ||
			!Enum.TryParse<SpellLifecycleMode>((string?)root.Attribute("mode"), true, out var mode) ||
			mode is not (SpellLifecycleMode.Permanent or SpellLifecycleMode.TemporaryCleanup) ||
			root.Element("PermanentOutput") is { } output &&
			(!int.TryParse((string?)output.Attribute("grade"), out var grade) || grade is < 1 or > 7 ||
			 !long.TryParse(output.Value, out var prototypeId) || prototypeId <= 0))
		{
			_lifecycleLoadError = "Invalid item lifecycle schema, mode or permanent grade output.";
			_unreadableLifecycle = new(root); return;
		}
		LifecycleMode = mode; LifecycleFamily = root.Element("Family")?.Value ?? "";
		_lifetimeFormula = root.Element("Seconds")?.Value;
		if (_lifetimeFormula is not null) LifetimeExpression = new TraitExpression(_lifetimeFormula, Gameworld);
		if (root.Element("PermanentOutput") is { } permanent)
		{ PermanentGrade = int.Parse(permanent.Attribute("grade")!.Value); _permanentPrototypeId = long.Parse(permanent.Value); }
	}

	private XElement? SaveLifecycle() => _unreadableLifecycle is not null ? new(_unreadableLifecycle) : LifecycleMode is { } mode
		? new("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", mode), new XElement("Family", LifecycleFamily),
			_lifetimeFormula is not null ? new XElement("Seconds", _lifetimeFormula) : null,
			PermanentGrade is { } grade ? new XElement("PermanentOutput", new XAttribute("grade", grade), _permanentPrototypeId) : null)
		: null;

	internal bool ValidateInvocation(ICharacter caster, out string? error)
	{
		error = DefinitionError;
		if (error is not null || LifecycleMode is null) return error is null;
		if (Spell is not MagicSpell { InvocationGrade: { } grade } native)
		{ error = "Lifecycle item creation requires a selected-grade native casting invocation."; return false; }
		if (LifecycleMode == SpellLifecycleMode.Permanent || PermanentGrade == grade) return true;
		if (_preparedLifetimeSeconds is not null) return true;
		try
		{
			var seconds = LifetimeExpression!.Evaluate(caster, native.CastingTrait, TraitBonusContext.SpellDuration);
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
		if (!ValidateInvocation(caster, out error)) return false;
		if (LifecycleMode is null)
		{ application = new LegacyItemCreation(this, caster, target, outcome, power); return true; }
		if (!ReferenceEquals(caster.Gameworld, Gameworld) || !ReferenceEquals(target.Gameworld, Gameworld) || target is not (ICharacter or ICell or IGameItem))
		{ error = "The item recipient must be a supported target in this world."; return false; }
		var native = (MagicSpell)Spell; var grade = native.InvocationGrade!.Value;
		var mode = PermanentGrade == grade ? SpellLifecycleMode.Permanent : LifecycleMode!.Value;
		var prototype = PermanentGrade == grade ? PermanentPrototype! : ItemPrototype;
		try
		{
			var qualityValue = Math.Floor(ItemQuality.EvaluateDoubleWith(("base", (int)prototype.BaseItemQuality), ("power", (int)power), ("outcome", (int)outcome)));
			if (!double.IsFinite(qualityValue) || qualityValue < int.MinValue || qualityValue > int.MaxValue || !Enum.IsDefined((ItemQuality)(int)qualityValue))
			{ error = "The created item quality must resolve to a native quality."; return false; }
			application = new NativeItemCreation(Guid.NewGuid(), this, caster, target, prototype, (ItemQuality)(int)qualityValue, grade, mode,
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

	private sealed record NativeItemCreation(Guid Id, CreateItemEffect Effect, ICharacter Caster, IPerceivable Target,
		IGameItemProto Prototype, ItemQuality Quality, int Grade, SpellLifecycleMode Mode, double? Seconds, Guid? Invocation) : IMagicSpellEffectApplication
	{
		public IMagicSpellEffect Create(IMagicSpellEffectParent parent)
		{
			var now = RuntimeClock.UtcNow;
			var origin = new SpellLifecycleOrigin(Id, Effect.Spell.Id, Grade, CharacterInstanceIdentityComparer.IdentityId(Caster),
				Effect.LifecycleFamily, Mode, now, Seconds is { } seconds ? now.AddSeconds(seconds) : null,
				$"native-createitem; prototype={Prototype.Id}/{Prototype.RevisionNumber}; invocation={Invocation}; parent={(parent as MagicSpellParent)?.Identity}");
			var item = Effect.Gameworld.SpellOwnedItems!.Create(Prototype, Caster, Quality, origin);
			Effect.PlaceOwnedItem(item, Caster, Target);
			return null!;
		}
	}

	private void PlaceOwnedItem(IGameItem item, ICharacter caster, IPerceivable target)
	{
		Gameworld.Add(item);
		item.SetOwner(target as ICharacter ?? caster);
		if (target is ICharacter character && character.Body.CanGet(item, 0)) character.Body.Get(item, silent: true);
		else if (target is IGameItem host && host.GetItemType<IContainer>() is { } container && container.CanPut(item)) container.Put(null, item, false);
		else if (target is IGameItem sheathHost && sheathHost.GetItemType<ISheath>() is { } sheath && sheath.CanSheath(item)) sheath.Content = item.GetItemType<IWieldable>();
		else if (target is ICell cell && !ReferenceEquals(cell, caster.Location)) cell.Insert(item, true);
		else { item.RoomLayer = target.RoomLayer; item.InsertAtSource(target is IGameItem outputHost ? outputHost.LocationLevelPerceivable : target is ICell ? caster : target, true); }
		item.HandleEvent(EventType.ItemFinishedLoading, item); item.Login();
	}

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
