using System.Collections.Concurrent;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Inventory;
using MudSharp.Magic.Vancian;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using MudSharp.Form.Audio;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService : IMagicCastingService
{
	private readonly IFuturemud _world;
	private readonly IMagicCastingStateStore _store;
	private readonly Func<DateTime> _clock;
	private readonly Func<double> _random;
	private readonly Func<int, int> _areaRandom;
	private readonly Action<string>? _checkpoint;
	private readonly Action? _flush;
	private readonly ConcurrentDictionary<long, object> _guards = new();
	private readonly ConcurrentDictionary<long, byte> _mutating = new();
	private readonly ConcurrentDictionary<Guid, CastingOperation> _uncertain = new();
	public MagicCastingService(IFuturemud world, IMagicCastingStateStore? store = null, Func<DateTime>? clock = null,
		Func<double>? random = null, Action<string>? checkpoint = null, Action? flush = null, Func<int, int>? areaRandom = null)
	{
		_world = world; _store = store ?? new MagicCastingStateStore(); _clock = clock ?? (() => DateTime.UtcNow);
		_random = random ?? Random.Shared.NextDouble; _checkpoint = checkpoint; _flush = flush;
		_areaRandom = areaRandom ?? Random.Shared.Next;
	}
	public static ICharacter Owner(ICharacter actor) => actor.Identity?.PrimaryInstance ?? actor;
	private object Guard(ICharacter actor) => _guards.GetOrAdd(Owner(actor).Id, _ => new object());
	public AcquiredSpell? Acquisition(ICharacter character, long spellId) => _store.Acquisition(Owner(character).Id, spellId);

	public IReadOnlyList<MagicCastingRoute> Routes(ICharacter actor, long? spellId = null)
	{
		return _world.MagicCapabilities.OfType<IMagicCastingCapability>().Where(x => x.CastingPolicy is not null)
			.SelectMany(c => c.CastingPolicy!.Admissions.Where(x => !spellId.HasValue || x.SpellId == spellId).Select(a =>
			{
				var error = Preflight(actor, c.Id, a.SpellId, a.MinimumGrade, false);
				return new MagicCastingRoute(c.Id, a.Key, a.SpellId, a.TraitId ?? c.CastingPolicy.DefaultTraitId,
					c.CastingPolicy.ReserveResourceId, error is null, error ?? "Available; target, components and payment require casting.");
			})).ToArray();
	}

	public string? Preflight(ICharacter actor, long capabilityId, long spellId, int grade, bool overreach)
	{
		try { return ResolveRoute(actor, capabilityId, spellId, grade, overreach, out _, out _, out _, out _, out _); }
		catch (Exception ex) { return $"Casting configuration cannot be resolved: {ex.Message}"; }
	}

	private string? ResolveRoute(ICharacter actor, long capabilityId, long spellId, int grade, bool overreach,
		out IMagicCastingCapability capability, out MagicCastingAdmission admission, out MagicSpell spell,
		out ITraitDefinition trait, out Difficulty difficulty, MagicCastingMode mode = MagicCastingMode.Manifest,
		Guid? continuingOperation = null, string method = "Say")
	{
		capability = null!; admission = null!; spell = null!; trait = null!; difficulty = default;
		if (_world.MagicCapabilities.Get(capabilityId) is not IMagicCastingCapability c || c.CastingPolicy is not { Enabled: true } p)
			return "That capability has no enabled configured casting route.";
		capability = c;
		if (!actor.Capabilities.Any(x => x.Id == c.Id)) return "That capability is not currently available to this body.";
		if (c.CastingConfigurationErrors().FirstOrDefault() is { } config) return config;
		if (p.Admissions.FirstOrDefault(x => x.SpellId == spellId) is not { } a) return "This capability does not explicitly admit that spell.";
		admission = a;
		if (_world.MagicSpells.Get(spellId) is not MagicSpell s || !s.ReadyForGame || s.Trigger is not ICastMagicTrigger || s.GradeProfile is not { } profile)
			return "The admitted spell is not a ready ordinary cast-trigger spell with a valid grade profile.";
		spell = s;
		var acquired = Acquisition(actor, spellId);
		if (acquired is null) return "You have not acquired that spell.";
		if (acquired.ProfileVersion != profile.Version) return "The acquired spell's grade profile version needs staff reconciliation.";
		trait = _world.Traits.Get(a.TraitId ?? p.DefaultTraitId)!;
		if (!Owner(actor).HasTrait(trait)) return "The acquired spell's native proficiency skill is missing; staff must repair its grant.";
		if (grade < a.MinimumGrade || grade > a.MaximumGrade || profile.Grades.FirstOrDefault(x => x.Grade == grade) is not { } g)
			return "That grade is outside this admission's allowed range.";
		if (grade > acquired.ControlledGrade && (!overreach || grade != acquired.ControlledGrade + 1))
			return "Only the single next grade may be attempted with explicit overreach.";
		if (overreach && grade != acquired.ControlledGrade + 1) return "Overreach must request exactly your next uncontrolled grade.";
		var requiredProficiency = a.RequiredProficiency(g);
		if (overreach && Owner(actor).TraitRawValue(trait) < requiredProficiency) return $"Overreach into grade {grade} requires raw proficiency {requiredProficiency}.";
		if (!Enum.IsDefined(mode)) return "Unknown casting mode.";
		if (mode == MagicCastingMode.Area && profile.Area is null) return "Area casting is unavailable until a separate explicit area policy is authored.";
		var practice = mode == MagicCastingMode.Practice ? profile.Practice : null;
		if (mode == MagicCastingMode.Practice && practice is not { Enabled: true }) return "Practice is not enabled for this spell.";
		if (practice?.MaximumGrade is { } maximum && grade > maximum) return "That grade exceeds this spell's configured practice maximum.";
		if (practice is not null && (actor.State.HasFlag(CharacterState.Paralysed) ||
			actor.CombinedEffectsOfType<MudSharp.Effects.Interfaces.IForceParalysisEffect>().Any(x => x.ShouldParalyse && x.Applies())))
			return "Paralysis prevents you from continuing practice.";
		if (practice is not null && (actor.Combat is not null || actor.Movement is not null && !practice.AllowMovement ||
			actor.CombinedEffectsOfType<MudSharp.Effects.Interfaces.IActionEffect>().Any(x => x.IsBlockingEffect("general") &&
				(x is not MagicPracticeAction action || action.OperationId != continuingOperation))))
			return "Finish or stop other blocking work, movement or combat before practising.";
		var deliveryPolicy = mode == MagicCastingMode.Area ? profile.Area!.Deliveries.SingleOrDefault(x => x.Method.EqualTo(method)) :
			mode == MagicCastingMode.Manifest ? profile.Incantation?.Deliveries.SingleOrDefault(x => x.Method.EqualTo(method)) : null;
		if (mode == MagicCastingMode.Area && deliveryPolicy is null) return "That native speech/area combination is not explicitly enabled for this spell.";
		var deliverySteps = deliveryPolicy?.DifficultySteps ?? 0;
		var steps = (long)(practice?.Difficulty ?? s.CastingDifficulty) + g.DifficultySteps + (overreach ? profile.OverreachDifficultySteps : 0) + deliverySteps;
		if (steps < 0 || steps >= (int)Difficulty.Impossible || !Enum.IsDefined((Difficulty)steps)) return "The resolved casting difficulty is impossible or out of range.";
		difficulty = (Difficulty)steps;
		if (VancianMagicService.CastingError(actor) is { } physical) return physical;
		if (actor.Body is null || actor.Location is null) return "You need a physical body in a location.";
		if (!MagicCastingMethods.TryVolume(method, out var volume)) return "Unknown native speech method.";
		if ((practice?.RequiresSpeech ?? true) && SpeechEligibility(actor, volume) is { } speechError) return speechError;
		if ((practice?.RequiresFreeHand ?? true) && !actor.Body.FunctioningFreeHands.Any()) return "You need a functioning free hand to manipulate this casting.";
		if (actor.CombinedEffectsOfType<MagicSpellLockout>().Any(x => x.Applies(s.School))) return "You are currently locked out from casting this spell.";
		if (ReserveConflict(actor, p.ReserveResourceId)) return "This reserve has incompatible passive and gathering-only entitlements.";
		if (!MagicResourceCapacity.TryGetCap(_world.MagicResources.Get(p.ReserveResourceId)!, Owner(actor), out _, out var capError))
			return $"The reserve capacity is invalid: {capError}";
		return CastingQuarantineReason(actor, spellId, trait.Id, p.ReserveResourceId, continuingOperation: continuingOperation);
	}

	private sealed record Prepared(MagicCastingQuote Quote, MagicSpell Spell, SpellTargetResolution Target,
		IReadOnlyList<CastingPayment> Payments, long[] Items, string Configuration, AreaPlan? Area = null);
	public MagicCastingQuote Quote(MagicCastingIntent intent)
	{
		try { return Prepare(intent).Quote; }
		catch (Exception ex) { return new(null, $"Casting preflight refused: {ex.Message}"); }
	}
	private Prepared Prepare(MagicCastingIntent intent, MagicSpell? preparedSelectionSource = null)
	{
		var actor = intent.Actor;
		if (ResolveRoute(actor, intent.CapabilityId, intent.SpellId, intent.Grade, intent.Overreach,
			out var capability, out var admission, out var spell, out var trait, out var difficulty, intent.Mode, method: intent.Method) is { } refusal)
			throw new InvalidOperationException(refusal);
		var policy = capability.CastingPolicy!;
		if (intent.OriginId == Guid.Empty) throw new InvalidOperationException("An invocation origin must be a nonempty correlation ID.");
		var delivery = ResolveDelivery(intent, spell, capability);
		if (intent.Targets.Length > 4096) throw new InvalidOperationException("The target specification exceeds 4096 characters.");
		var power = spell.GradeProfile!.Grades.Single(x => x.Grade == intent.Grade).Power;
		if (intent.Mode == MagicCastingMode.Practice && !string.IsNullOrWhiteSpace(intent.Targets))
			throw new InvalidOperationException("Practice is target-free; do not specify a target.");
		var area = intent.Mode == MagicCastingMode.Area ? PrepareArea(intent, spell) : null;
		var target = area is not null ? new SpellTargetResolution(new PerceivableGroup(area.Candidates.Select(x => (IPerceivable)x.Target).ToArray()), []) :
			intent.Mode == MagicCastingMode.Practice ? new SpellTargetResolution(null, []) :
			SpellTargetCapture.Resolve(actor, spell, power, new StringStack(intent.Targets), spell.GradeProfile.Incantation is not null);
		if (target is null) throw new InvalidOperationException("No valid target was resolved.");
		var targets = target.Target is PerceivableGroup group ? group.Members : target.Target is { } single ? new[] { single } : [];
		foreach (var individual in targets)
			if (!actor.CanInteractPlanar(individual, PlanarInteractionKind.Magic)) throw new InvalidOperationException("Your current plane cannot reach a target with magic.");
		var controlledGrade = Acquisition(actor, spell.Id)!.ControlledGrade;
		// Practice never clones/binds effect templates or invokes their target/caster application paths.
		var copy = intent.Mode == MagicCastingMode.Practice ? spell : spell.CastingCopy(actor, trait, intent.Grade, power, difficulty, controlledGrade);
		if (intent.Mode != MagicCastingMode.Practice) copy.InvocationOriginId = intent.OriginId;
		if (intent.Mode != MagicCastingMode.Practice)
		{
			if (preparedSelectionSource is not null) ReusePreparedSelections(copy, preparedSelectionSource, actor, targets);
			foreach (var (effect, recipients) in copy.SpellEffects.Select(x => (x, targets))
				.Concat(copy.CasterSpellEffects.Select(x => (x, (IEnumerable<IPerceivable>)new[] { actor }))))
				foreach (var recipient in recipients)
				{
					if (effect is MudSharp.Magic.SpellEffects.CreateItemEffect item && !item.ValidateRecipientInvocation(actor, recipient, out var itemError))
						throw new InvalidOperationException(itemError);
					if (effect is MudSharp.Magic.SpellEffects.CreateLiquidEffect liquid && !liquid.ValidateInvocation(actor, recipient, out var liquidError))
						throw new InvalidOperationException(liquidError);
					if (effect is MudSharp.Magic.SpellEffects.AnimateCorpseSpellEffect animation && !animation.ValidateInvocation(actor, recipient, out var animationError))
						throw new InvalidOperationException(animationError);
					if (effect is IMagicSpellEffectPreparedSelection selection) selection.CapturePreparedSelection(actor, recipient);
				}
		}
		List<CastingPayment> payments = [];
		foreach (var (resource, expression) in copy.CastingCosts)
		{
			var amount = resource.Id == policy.SourceResourceId && spell.GradeProfile.Efficiency is { } efficiency
				? efficiency.Cost(controlledGrade, intent.Grade)
				: (intent.Mode == MagicCastingMode.Practice ? CastingNumerics.Bind(expression, trait, intent.Grade, power,
					$"practice/cost/{resource.Id}", _world, controlledGrade) : expression)
					.EvaluateWith(actor, trait, TraitBonusContext.SpellCost, ("self", ReferenceEquals(actor, target.Target) ? 1 : 0));
			var destination = resource.Id == policy.SourceResourceId ? _world.MagicResources.Get(policy.ReserveResourceId)! : resource;
			if (resource.Id == policy.SourceResourceId && intent.Overreach) amount *= spell.GradeProfile.OverreachMultiplier;
			if (resource.Id == policy.SourceResourceId)
				amount *= area?.Policy.Deliveries.Single(x => x.Method.EqualTo(intent.Method)).EnergyMultiplier ?? delivery?.EnergyMultiplier ?? 1;
			if (intent.Mode == MagicCastingMode.Practice && resource.Id == policy.SourceResourceId) amount *= spell.GradeProfile.Practice!.EnergyMultiplier;
			if (!double.IsFinite(amount) || amount < 0) throw new InvalidOperationException($"cost/{resource.Id}: amount must be finite and non-negative.");
			var holder = destination.Id == policy.ReserveResourceId ? Owner(actor) : ReserveHolder(actor, destination.Id);
			payments.Add(new(holder, destination, amount));
		}
		payments = payments.GroupBy(x => (Holder: x.Holder.Id, Resource: x.Resource.Id))
			.Select(x => new CastingPayment(x.First().Holder, x.First().Resource, x.Sum(y => y.Amount)))
			.OrderBy(x => x.Holder.Id).ThenBy(x => x.Resource.Id).ToList();
		foreach (var cost in payments)
		{
			if (!MagicResourceCapacity.TryGetCap(cost.Resource, cost.Holder, out var cap, out var capError))
				throw new InvalidOperationException($"The {cost.Resource.Name} capacity is invalid: {capError}");
			var balance = cost.Holder.MagicResourceAmounts.GetValueOrDefault(cost.Resource);
			if (!double.IsFinite(cost.Amount) || !double.IsFinite(balance) || Math.Min(balance, cap) < cost.Amount ||
				!cost.Holder.CanUseResource(cost.Resource, cost.Amount)) throw new InvalidOperationException($"Insufficient {cost.Resource.Name} for the combined cost.");
			if (QuarantineReason(actor, reserveId: cost.Resource.Id) is { } q) throw new InvalidOperationException(q);
		}
		var plan = (intent.Mode == MagicCastingMode.Practice ? spell.PracticeInventoryPlanTemplate! : copy.InventoryPlanTemplate).CreatePlan(actor);
		if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible) throw new InvalidOperationException("The actual component plan is infeasible; check materials and free manipulators.");
		var items = ValidateDeviceFocus(intent, plan, PlanItems(plan), target);
		if (items.Length > 512) throw new InvalidOperationException("This component plan exceeds the 512 input receipt bound.");
		if (QuarantineReason(actor, itemIds: items) is { } inputError) throw new InvalidOperationException(inputError);
		var invocation = new ResolvedMagicCastingInvocation(intent.OriginId ?? Guid.NewGuid(), actor.InstanceId, actor.Body.Id, Owner(actor).Id,
			capability.Id, policy.Identity, admission.Key, spell.Id, spell.School.Id, trait.Id, Owner(actor).Id, policy.ReserveResourceId,
			intent.Grade, power, intent.Overreach, difficulty, intent.Targets, Array.AsReadOnly(target.Parameters),
			Array.AsReadOnly(payments.Select(x => new MagicCastingCost(x.Holder.Id, x.Resource.Id, x.Amount)).ToArray()), policy.Version, spell.GradeProfile.Version, controlledGrade, intent.Mode, delivery, area?.Receipt);
		return new(new(invocation, "Advisory quote; casting revalidates all inputs."), copy, target, payments.AsReadOnly(), items, CaptureConfiguration(spell), area);
	}

	private static string CaptureConfiguration(MagicSpell spell)
	{
		var expressions = spell.CastingCosts.Values.Append(spell.EffectDurationExpression)
			.Concat(spell.SpellEffects.Concat(spell.CasterSpellEffects).SelectMany(ScrollSpellCompatibility.Expressions).Select(x => x.Expression))
			.Where(x => x is not null).Select(x => new
			{
				x.OriginalFormulaText,
				Parameters = x.Parameters.OrderBy(p => p.Key).Select(p => new { p.Key, Trait = p.Value.Trait.Id, p.Value.CanBranch, p.Value.CanImprove })
			});
		return System.Text.Json.JsonSerializer.Serialize(new { Spell = spell.SnapshotModel(), Expressions = expressions });
	}

	private static long[] PlanItems(IInventoryPlan plan) => plan.PeekPlanResults()
		.SelectMany(x => new[] { x.PrimaryTarget?.Id, x.SecondaryTarget?.Id })
		.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToArray();

	public string? QuarantineReason(ICharacter actor, long? spellId = null, long? traitId = null, long? reserveId = null, IEnumerable<long>? itemIds = null)
		=> CastingQuarantineReason(actor, spellId, traitId, reserveId, itemIds);

	private string? CastingQuarantineReason(ICharacter actor, long? spellId = null, long? traitId = null,
		long? reserveId = null, IEnumerable<long>? itemIds = null, Guid? continuingOperation = null)
	{
		var owner = Owner(actor).Id;
		var items = itemIds?.ToHashSet() ?? [];
		foreach (var op in _store.Unresolved(items.Count > 0 ? null : owner)
			.Concat(_uncertain.Values.Where(x => x.CharacterId == owner || items.Count > 0)).DistinctBy(x => x.Id))
		{
			if (op.Id == continuingOperation && op.CharacterId == owner) continue;
			XElement root;
			try
			{
				root = XElement.Parse(op.Definition);
				if (root.Name != "Casting" || (string?)root.Attribute("version") != "1" ||
					root.Elements("Cost").Any(x => !long.TryParse((string?)x.Attribute("resource"), out _)) ||
					root.Elements("Item").Any(x => !long.TryParse((string?)x.Attribute("id"), out _)))
					return $"Casting operation {op.Id} has an invalid receipt; this identity needs staff review before further casting mutations.";
			}
			catch (System.Xml.XmlException)
			{
				return $"Casting operation {op.Id} has an unreadable receipt; this identity needs staff review before further casting mutations.";
			}
			if (op.CharacterId == owner && (op.SpellId == spellId || op.TraitId == traitId || op.ReserveId == reserveId ||
				root.Elements("Cost").Any(x => (long)x.Attribute("resource")! == reserveId)) ||
				root.Elements("Item").Any(x => items.Contains((long)x.Attribute("id")!)))
				return $"Casting operation {op.Id} ({op.Stage}) needs staff review: {op.Diagnostic}. Its spell, trait, reserves and physical inputs are quarantined.";
		}
		return null;
	}

	private void Flush(ICharacter actor)
	{
		if (_flush is not null) { _flush(); return; }
		Owner(actor).Changed = true; actor.Changed = true;
		_world.SaveManager.Flush();
	}
}
