using MudSharp.Body.Traits;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic.Resources;
using MudSharp.PerceptionEngine.Lists;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private sealed record SelectionAdmission((IMagicSpellEffectPreparedSelection Effect, IPerceivable Recipient)[] Selections,
		Func<bool>[] StructuralChecks);

	/// <summary>Capture raw inputs once before the later live admissions. No policy evaluation or capacity cache.</summary>
	private SelectionAdmission? CaptureSelectionAdmission(MagicCastingIntent intent, Prepared prepared)
	{
		var actor = intent.Actor;
		var targets = prepared.Target.Target is PerceivableGroup group ? group.Members.ToArray() :
			prepared.Target.Target is { } single ? [single] : Array.Empty<IPerceivable>();
		var selections = prepared.Spell.SpellEffects.SelectMany(effect => targets.Select(recipient => (effect, recipient)))
			.Concat(prepared.Spell.CasterSpellEffects.Select(effect => (effect, recipient: (IPerceivable)actor)))
			.Where(x => x.effect is IMagicSpellEffectPreparedSelection selection && selection.CapturePreparedSelection(actor, x.recipient) is not null)
			.Select(x => ((IMagicSpellEffectPreparedSelection)x.effect, x.recipient)).ToArray();
		if (selections.Length == 0) return null;
		var checks = new List<Func<bool>>();
		foreach (var (effect, recipient) in selections)
			if (effect.CapturePreparedSelection(actor, recipient) is IMagicSpellEffectPreparedSelectionRawToken raw)
				checks.Add(() => raw.IsCurrent);
		void Value<T>(Func<T> read) { var before = read(); checks.Add(() => EqualityComparer<T>.Default.Equals(before, read())); }
		void Reference<T>(Func<T> read) where T : class? { var before = read(); checks.Add(() => ReferenceEquals(before, read())); }
		void Sequence<T>(Func<IEnumerable<T>> read) where T : class
		{ var before = read().ToArray(); checks.Add(() => before.SequenceEqual(read(), ReferenceEqualityComparer.Instance)); }
		void Character(ICharacter character)
		{
			Reference(() => character.Body); Reference(() => Owner(character)); Value(() => character.InstanceId);
			Value(() => character.Body?.Id); Value(() => Owner(character).Id); Reference(() => character.Location);
			Value(() => character.RoomLayer); Value(() => character.State); Reference(() => character.Combat); Reference(() => character.Movement);
			Sequence(() => character.Merits); Sequence(() => character.Effects);
			if (character.Body is { } body) {
				Sequence(() => body.Effects); Sequence(() => body.HeldOrWieldedItems); Sequence(() => body.Traits);
				foreach (var trait in body.Traits) Value(() => trait.RawValue);
			}
			Sequence(() => character.Traits);
			foreach (var trait in character.Traits) Value(() => trait.RawValue);
		}
		Character(actor); if (!ReferenceEquals(Owner(actor), actor)) Character(Owner(actor));
		var items = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
		void Item(IGameItem item)
		{
			if (!items.Add(item)) return;
			Reference(() => item.Location); Value(() => item.RoomLayer); Reference(() => item.InInventoryOf); Reference(() => item.ContainedIn);
			Reference(() => item.Prototype); Value(() => item.Deleted); Value(() => item.Destroyed); Value(() => item.Quantity);
			Sequence(() => item.Components);
			if (item.GetItemType<ILiquidContainer>() is { } container) {
				Reference(() => item.GetItemType<ILiquidContainer>()); Value(() => container.OwnsLiquidMixture); Value(() => container.IsOpen);
				Value(() => container.LiquidCapacity); Value(() => container.LiquidMixture?.SaveToXml().ToString(SaveOptions.DisableFormatting));
			}
			if (item.ContainedIn is { } parent) Item(parent);
		}
		foreach (var target in targets) {
			Reference(() => target.Location); Value(() => target.RoomLayer);
			if (target is ICharacter character && !ReferenceEquals(character, actor)) Character(character);
			if (target is IGameItem item) Item(item);
		}
		foreach (var id in prepared.Items) {
			Reference(() => _world.Items.Get(id)); if (_world.Items.Get(id) is { } item) Item(item);
		}
		var spell = (MagicSpell)_world.MagicSpells.Get(intent.SpellId)!;
		Reference(() => _world.MagicSpells.Get(intent.SpellId)); Value(() => CaptureConfiguration(spell));
		foreach (var capability in _world.MagicCapabilities.OfType<IMagicCastingCapability>()) {
			Reference(() => _world.MagicCapabilities.Get(capability.Id));
			Value(() => System.Text.Json.JsonSerializer.Serialize(capability.CastingPolicy));
		}
		foreach (var payment in prepared.Payments) {
			Reference(() => _world.MagicResources.Get(payment.Resource.Id)); Reference(() => ReserveHolder(actor, payment.Resource.Id));
			Reference(() => Owner(payment.Holder)); Value(() => IsConfiguredReserve(_world, payment.Resource.Id));
			Value(() => payment.Holder.MagicResourceAmounts.GetValueOrDefault(payment.Resource));
			if (payment.Resource is SimpleMagicResource resource) {
				Reference(() => resource.ResourceCapProg); Value(() => resource.ResourceCapProg?.FunctionText);
				Value(() => resource.ResourceCapProg?.ReturnType); Value(() => resource.ResourceCapProg?.CompileError);
				Value(() => resource.AttributeCapacity);
			}
		}
		if (_deviceFocus.TryGetValue(intent, out var use)) {
			Item(use.Device.Parent); Value(() => use.Device.Charges); Value(() => use.Device.NextCharge); Value(() => use.Device.Reservation);
			checks.Add(() => DeviceStructuralError(actor, use, targets: use.Targets) is null);
		}
		return new(selections, checks.ToArray());
	}

	/// <summary>Device admission runs first. Then finish ordinary capacity callbacks, confirm frozen choices,
	/// and audit raw inputs. Only exact synchronous debits receive the sampled capacity; all other queries stay live.</summary>
	private MagicResourceCapacityAdmission AdmitSelectionPayment(MagicCastingIntent intent, Prepared prepared,
		SelectionAdmission selection, MagicResourceCapacityAdmission? deviceAdmission)
	{
		var admission = deviceAdmission;
		if (admission is null) {
			var payments = prepared.Payments.Select(payment => {
				if (!MagicResourceCapacity.TryGetCap(payment.Resource, payment.Holder, out var capacity, out var error))
					throw new InvalidOperationException(error);
				var balance = payment.Holder.MagicResourceAmounts.GetValueOrDefault(payment.Resource);
				if (!double.IsFinite(balance) || Math.Min(balance, capacity) < payment.Amount)
					throw new InvalidOperationException("Insufficient admitted selection payment.");
				return new MagicResourceCapacityAdmission.Payment(payment.Holder, payment.Resource, payment.Amount, capacity);
			}).ToArray();
			admission = new(payments);
		}
		foreach (var (effect, recipient) in selection.Selections)
			if (!effect.TryConfirmPreparedSelection(intent.Actor, recipient, out var error))
				throw new InvalidOperationException("Final prepared selection admission changed: " + error);
		// Do not call policy, visibility, reach, entitlement or capacity evaluators below this boundary.
		if (selection.StructuralChecks.Any(check => !check()))
			throw new InvalidOperationException("Captured selection actor, target, custody, payment or configuration changed during admission.");
		return admission;
	}
}
