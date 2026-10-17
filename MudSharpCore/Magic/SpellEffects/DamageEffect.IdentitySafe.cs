#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic.Casting;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

public partial class DamageEffect : IMagicSpellEffectPreparedSelection
{
	// Ground Tremor originates at ground level even when its caster is airborne in that room.
	internal bool AdmitsGroundedRoomDamage { get; init; }
	private readonly Dictionary<IPerceivable, DamageSelection> _damageSelections = new(ReferenceEqualityComparer.Instance);

	private sealed record ItemState(IGameItem Item, long Id, IGameItemProto Prototype, IBody? Custodian,
		IGameItem? Container, IRoom? Location, RoomLayer Layer, IGameItemComponent[] Components)
	{
		public static ItemState Capture(IGameItem item) => new(item, item.Id, item.Prototype, item.InInventoryOf,
			item.ContainedIn, item.Location, item.RoomLayer, item.Components.ToArray());
		public bool Current(IFuturemud world) => !Item.Deleted && !Item.Destroyed && Item.Id == Id &&
			ReferenceEquals(Item.Gameworld, world) && ReferenceEquals(world.Items.Get(Id), Item) && ReferenceEquals(Item.Prototype, Prototype) &&
			ReferenceEquals(Item.InInventoryOf, Custodian) && ReferenceEquals(Item.ContainedIn, Container) &&
			ReferenceEquals(Item.Location, Location) && Item.RoomLayer == Layer &&
			Item.Components.SequenceEqual(Components, ReferenceEqualityComparer.Instance);
	}

	private sealed record DamageSelection(ICharacter Caster, ICharacter Victim, IBody Body, int Grade,
		string Definition, Func<bool> Check) : IMagicSpellEffectPreparedSelectionRawToken
	{
		public bool IsCurrent => Check();
	}

	public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
	{
		if (!IdentitySafeDamage) return null;
		if (DefinitionError is { } definitionError) throw new InvalidOperationException(definitionError);
		if (_damageSelections.TryGetValue(recipient, out var prior))
		{
			if (!TryConfirmPreparedSelection(caster, recipient, out var error)) throw new InvalidOperationException(error);
			return prior;
		}
		if (recipient is not ICharacter victim || Spell is not MagicSpell { InvocationGrade: >= 1 and <= 7 } native ||
			caster.Body is not { } sourceBody || victim.Body is not { } targetBody ||
			!ReferenceEquals(caster.Gameworld, Gameworld) || !ReferenceEquals(victim.Gameworld, Gameworld) ||
			!CommandExecutionAuthority.IsCurrent(caster, false) || !CommandExecutionAuthority.IsCurrent(victim, false) ||
			!ReferenceEquals(caster.Location, victim.Location) ||
			caster.RoomLayer != victim.RoomLayer && !(AdmitsGroundedRoomDamage && victim.RoomLayer == RoomLayer.GroundLevel))
			throw new InvalidOperationException("Identity-safe damage requires exact current local character bodies and a selected grade.");
		if (!Enum.IsDefined(DamageType)) throw new InvalidOperationException("Choose a defined native damage type.");
		foreach (var expression in new[] { DamageExpression, PainExpression, StunExpression }.OfType<ITraitExpression>())
		{
			if (expression is not CastingExpression bound || !bound.TryEvaluateFixedInputs(out var amount, out var error) ||
				amount is { } value && (!double.IsFinite(value) || value < 0))
				throw new InvalidOperationException("Identity-safe damage requires valid typed invocation expressions with finite nonnegative amounts.");
		}
		var sourceIdentity = caster.Identity; var victimIdentity = victim.Identity;
		var sourceId = caster.Id; var victimId = victim.Id;
		var sourceInstance = caster.InstanceId; var victimInstance = victim.InstanceId;
		var sourceState = caster.State; var victimState = victim.State;
		var sourceRoom = caster.Location; var victimRoom = victim.Location;
		var sourceLayer = caster.RoomLayer; var victimLayer = victim.RoomLayer;
		ICharacter[] Roots(ICharacter actor) => Gameworld.Actors.Concat(Gameworld.Characters).Concat(Gameworld.NPCs).Concat(Gameworld.CachedActors)
			.Where(x => CharacterInstanceIdentityComparer.IdentityId(x) == CharacterInstanceIdentityComparer.IdentityId(actor)).ToArray();
		var sourceRoots = Roots(caster); var victimRoots = Roots(victim);
		var parts = targetBody.Bodyparts.ToArray();
		var partIds = parts.Select(x => (Part: x, Id: x.Id)).ToArray();
		var limbs = targetBody.Limbs.ToArray();
		if (BodypartId > 0 && !parts.Any(x => x.Id == BodypartId) ||
			BodypartId == 0 && Limb != (LimbType)(-1) && !limbs.Any(x => x.LimbType == Limb))
			throw new InvalidOperationException("The explicitly configured damage bodypart or limb is absent from the exact target body.");
		var allItems = targetBody.AllItems.ToArray();
		var worn = targetBody.WornItems.ToArray();
		var prosthetics = targetBody.Prosthetics.ToArray();
		var prostheticChecks = prosthetics.Select(component =>
		{
			var parent = component.Parent; var installed = component.InstalledBody;
			var targetPart = component.TargetBodypart; var targetPrototype = component.TargetBody;
			var included = component.IncludedParts.ToArray();
			return (Func<bool>)(() => ReferenceEquals(component.Parent, parent) && ReferenceEquals(component.InstalledBody, installed) &&
				ReferenceEquals(component.TargetBodypart, targetPart) && ReferenceEquals(component.TargetBody, targetPrototype) &&
				component.IncludedParts.SequenceEqual(included, ReferenceEqualityComparer.Instance));
		}).ToArray();
		var items = allItems.Concat(worn).Concat(prosthetics.Select(x => x.Parent))
			.Distinct(ReferenceEqualityComparer.Instance).Cast<IGameItem>().Select(ItemState.Capture).ToArray();
		var grade = native.InvocationGrade.Value;
		var definition = SaveToXml().ToString(SaveOptions.DisableFormatting);
		var source = Gameworld.MagicSpells.Get(Spell.Id) as MagicSpell ?? throw new InvalidOperationException("Missing exact registered source spell.");
		var sourceDefinition = source.SnapshotModel().Definition;
		var token = new DamageSelection(caster, victim, targetBody, grade, definition, () =>
			ReferenceEquals(caster.Gameworld, Gameworld) && ReferenceEquals(victim.Gameworld, Gameworld) &&
			ReferenceEquals(caster.Body, sourceBody) && ReferenceEquals(sourceBody.Actor, caster) &&
			ReferenceEquals(victim.Body, targetBody) && ReferenceEquals(targetBody.Actor, victim) &&
			ReferenceEquals(caster.Identity, sourceIdentity) && ReferenceEquals(victim.Identity, victimIdentity) &&
			caster.Id == sourceId && victim.Id == victimId && caster.InstanceId == sourceInstance && victim.InstanceId == victimInstance &&
			caster.State == sourceState && victim.State == victimState &&
			ReferenceEquals(caster.Location, sourceRoom) && ReferenceEquals(victim.Location, victimRoom) &&
			caster.RoomLayer == sourceLayer && victim.RoomLayer == victimLayer &&
			CommandExecutionAuthority.IsCurrent(caster, false) && CommandExecutionAuthority.IsCurrent(victim, false) &&
			Roots(caster).SequenceEqual(sourceRoots, ReferenceEqualityComparer.Instance) &&
			Roots(victim).SequenceEqual(victimRoots, ReferenceEqualityComparer.Instance) &&
			targetBody.Bodyparts.SequenceEqual(parts, ReferenceEqualityComparer.Instance) &&
			targetBody.Limbs.SequenceEqual(limbs, ReferenceEqualityComparer.Instance) &&
			partIds.All(x => x.Part.Id == x.Id && ReferenceEquals(Gameworld.BodypartPrototypes.Get(x.Id), x.Part)) &&
			targetBody.AllItems.SequenceEqual(allItems, ReferenceEqualityComparer.Instance) &&
			targetBody.WornItems.SequenceEqual(worn, ReferenceEqualityComparer.Instance) &&
			targetBody.Prosthetics.SequenceEqual(prosthetics, ReferenceEqualityComparer.Instance) &&
			prostheticChecks.All(x => x()) && items.All(x => x.Current(Gameworld)) &&
			ReferenceEquals(Gameworld.MagicSpells.Get(Spell.Id), source) && source.SnapshotModel().Definition == sourceDefinition &&
			native.InvocationGrade == grade && SaveToXml().ToString(SaveOptions.DisableFormatting) == definition);
		if (!token.IsCurrent) throw new InvalidOperationException("The exact damage recipient or custody changed during capture.");
		_damageSelections.Add(recipient, token);
		return token;
	}

	public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster,
		IPerceivable recipient, out string? error)
	{
		error = "The prepared damage body, location, identity, custody or definition changed.";
		if (!IdentitySafeDamage || selection is not DamageSelection token || !ReferenceEquals(caster, token.Caster) ||
			!ReferenceEquals(recipient, token.Victim) || Spell is not MagicSpell native || native.InvocationGrade != token.Grade ||
			SaveToXml().ToString(SaveOptions.DisableFormatting) != token.Definition || !token.IsCurrent) return false;
		_damageSelections[recipient] = token;
		error = null; return true;
	}

	public bool TryConfirmPreparedSelection(ICharacter caster, IPerceivable recipient, out string? error)
	{
		error = null;
		if (!IdentitySafeDamage) return true;
		error = "The prepared damage body, location, identity, custody or definition changed.";
		return _damageSelections.TryGetValue(recipient, out var token) && TryReusePreparedSelection(token, caster, recipient, out error);
	}

	private MagicEffectOperation ApplyIdentitySafe(ICharacter caster, IPerceivable target, OpposedOutcomeDegree outcome, SpellPower power)
	{
		if (!_damageSelections.TryGetValue(target, out var selection) || !TryConfirmPreparedSelection(caster, target, out var error))
			throw new InvalidOperationException("Identity-safe damage needs the unchanged prepared native selection.");
		using var fence = new InvocationFence(caster, selection.Victim);
		var damageExpression = DamageExpression; var painExpression = PainExpression; var stunExpression = StunExpression;
		bool Current() => !fence.Invalid && selection.IsCurrent &&
			ReferenceEquals(DamageExpression, damageExpression) && ReferenceEquals(PainExpression, painExpression) &&
			ReferenceEquals(StunExpression, stunExpression);
		var body = selection.Body;
		double Evaluate(ITraitExpression expression)
		{
			var amount = ReferenceEquals(expression, damageExpression) &&
				SpellAreaDamageScope.SharedDamage(caster, target, Spell) is { } shared
					? shared : expression.EvaluateWith(caster,
						values: [("power", (int)power), ("outcome", (int)outcome)]);
			var result = SpellAreaDamageScope.ScaleDamage(caster, target, Spell, amount);
			if (!Current() || !double.IsFinite(result) || result < 0)
				throw new InvalidOperationException("A damage evaluation changed its frozen selection or yielded invalid potency.");
			return result;
		}
		var damage = Evaluate(DamageExpression);
		var pain = PainExpression is null ? damage : Evaluate(PainExpression);
		var stun = StunExpression is null ? damage : Evaluate(StunExpression);
		IBodypart? part = BodypartId != 0 ? body.Bodyparts.FirstOrDefault(x => x.Id == BodypartId) : null;
		if (BodypartId != 0 && part is null) throw new InvalidOperationException("The selected native bodypart is absent; no random fallback is permitted.");
		if (part is null && Limb != (LimbType)(-1))
		{
			var limb = body.Limbs.FirstOrDefault(x => x.LimbType == Limb);
			part = limb is null ? null : body.BodypartsForLimb(limb).GetWeightedRandom(x => x.RelativeHitChance);
			if (part is null) throw new InvalidOperationException("The selected native limb is absent; no random fallback is permitted.");
		}
		part ??= body.RandomBodypart;
		if (part is null || !Current() || !body.Bodyparts.Any(x => ReferenceEquals(x, part)) ||
			!ReferenceEquals(Gameworld.BodypartPrototypes.Get(part.Id), part))
			throw new InvalidOperationException("The exact native damage bodypart changed during selection.");
		if (!CommandExecutionScope.TryContinue() || !Current())
			throw new InvalidOperationException("Damage authority or frozen native selection changed before mutation.");
		var guard = new DirectDamageGuard(selection, part, Gameworld, Current);
		var before = body.Wounds.Concat(body.WornItems.Concat(body.Prosthetics.Select(x => x.Parent))
			.Distinct(ReferenceEqualityComparer.Instance).Cast<IGameItem>().SelectMany(x => x.Wounds))
			.Distinct(ReferenceEqualityComparer.Instance).Cast<IWound>()
			.ToDictionary(x => x, x => (Damage: x.CurrentDamage, Pain: x.CurrentPain, Stun: x.CurrentStun));
		var wounds = body.SufferDamage(new Damage
		{
			DamageType = DamageType, DamageAmount = damage, PainAmount = pain, StunAmount = stun,
			ActorOrigin = caster, TargetBody = body, Bodypart = part, MutationGuard = guard
		}).ToArray();
		var applied = wounds.Any(x => !before.TryGetValue(x, out var prior)
			? x.CurrentDamage > 0 || x.CurrentPain > 0 || x.CurrentStun > 0
			: x.CurrentDamage > prior.Damage || x.CurrentPain > prior.Pain || x.CurrentStun > prior.Stun);
		return new(applied ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
	}

	private sealed class InvocationFence : IDisposable
	{
		private readonly ICharacter[] _actors;
		public bool Invalid { get; private set; }
		public InvocationFence(ICharacter caster, ICharacter victim)
		{
			_actors = new[] { caster, victim }.Distinct(ReferenceEqualityComparer.Instance).Cast<ICharacter>().ToArray();
			foreach (var actor in _actors)
			{
				actor.CurrentBodyChanged += BodyChanged; actor.OnQuit += Removed; actor.OnDeleted += Removed;
			}
		}
		private void BodyChanged(ICharacter actor, IBody before, IBody after) => Invalid = true;
		private void Removed(IPerceivable actor) => Invalid = true;
		public void Dispose()
		{
			foreach (var actor in _actors)
			{
				actor.CurrentBodyChanged -= BodyChanged; actor.OnQuit -= Removed; actor.OnDeleted -= Removed;
			}
		}
	}

	private sealed class DirectDamageGuard(DamageSelection selection, IBodypart part, IFuturemud world, Func<bool> current) : IDamageMutationGuard
	{
		private readonly long _partId = part.Id;

		public void Validate(IHaveWounds recipient, IDamage damage)
		{
			var allowed = ReferenceEquals(recipient, selection.Body) || recipient is IGameItem item &&
				selection.Body.WornItems.Concat(selection.Body.Prosthetics.Select(x => x.Parent)).Any(x => ReferenceEquals(x, item));
			if (!allowed || !current() || !ReferenceEquals(damage.MutationGuard, this) ||
				!ReferenceEquals(damage.ActorOrigin, selection.Caster) || !ReferenceEquals(damage.TargetBody, selection.Body) ||
				part.Id != _partId || !ReferenceEquals(world.BodypartPrototypes.Get(_partId), part))
				throw new InvalidOperationException("The frozen native damage body or item custody changed before wound mutation.");
		}
	}
}
