using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using System.Runtime.CompilerServices;

#nullable enable

namespace MudSharp.Form.Material;

public sealed record ExposurePatch(IPerceivable Target, IBodypart? Part, IMaterial Material, double Area,
	double Capacity, double Transmission = 1.0, int LayerDepth = 0, IPerceivable? PhysicalAnchor = null)
{
	private static ICell? LocationOf(IPerceivable target) => target is IGameItem item ? item.LocationLevelPerceivable?.Location : target.Location;
	public ICell? Location => LocationOf(PhysicalAnchor ?? Target);
	public RoomLayer Layer => (PhysicalAnchor ?? Target).RoomLayer;
	public IBody? TargetBody => Target as IBody ?? (Target as IGameItem)?.GetItemType<ICorpse>()?.OriginalBody;
	private ICell? OriginalLocation { get; } = LocationOf(PhysicalAnchor ?? Target);
	private RoomLayer OriginalLayer { get; } = (PhysicalAnchor ?? Target).RoomLayer;
	private IGameItem? OriginalContainer { get; } = (Target as IGameItem)?.ContainedIn;
	private IBody? OriginalInventory { get; } = (Target as IGameItem)?.InInventoryOf;
	public bool StillCurrent => Location == OriginalLocation && Layer == OriginalLayer && PhysicalAnchor is not IGameItem { Deleted: true } &&
		(Target is not IGameItem item || !item.Deleted && (Part is not null && TargetBody is not null || item.Material == Material) && item.ContainedIn == OriginalContainer && item.InInventoryOf == OriginalInventory) &&
		(TargetBody is not { } body || Part is null || (body.Bodyparts.Contains(Part) || body.Organs.Contains(Part)) && body.GetMaterial(Part) == Material);
}

public sealed record ExposureResolution(ExposurePatch Patch, IEnvironmentalReaction Reaction,
	double Work, double RawDamage, double Pain, double Stun, double Consumed);

/// <summary>Finite sources are debited before callbacks. Evaluation copies are never treated as resources.</summary>
public sealed class EnvironmentalExposureResolver
{
	private readonly IFuturemud _world;
	private bool _resolving;
	private sealed class Notice { public DateTime Next; public DateTime ExhaustionNext; }
	private readonly ConditionalWeakTable<IPerceivable, Notice> _notices = new();
	private readonly HashSet<string> _reportedErrors = new();
	private readonly Dictionary<(IFluid, IMaterial, ExposureRoute, double), IReadOnlyList<IEnvironmentalReaction>> _candidates = new();
	private readonly Dictionary<(IFluid, IMaterial, ExposureRoute, double), IReadOnlyList<IEnvironmentalReaction>> _staticSelections = new();
	public EnvironmentalExposureResolver(IFuturemud world) { _world = world; }
	public void Invalidate() { _candidates.Clear(); _staticSelections.Clear(); }
	private IReadOnlyList<IEnvironmentalReaction> Candidates(IFluid fluid, ExposurePatch patch, ExposureRoute route, double temperature,
		string source, double strength, double seconds, double quantity, bool dryRun)
	{
		var key = (fluid, patch.Material, route, temperature);
		if (!_candidates.TryGetValue(key, out var rules))
		{
			var errors = new List<string>();
			_candidates[key] = rules = FindCandidates(fluid, patch.Material, route, temperature, errors);
			if (!dryRun) foreach (var error in errors) Report($"{fluid.Name}: {error}");
		}
		if (rules.All(x => x.ApplicabilityProg is null))
		{
			if (_staticSelections.TryGetValue(key, out var selection)) return selection;
			var errors = new List<string>();
			_staticSelections[key] = selection = Choose(rules, patch.Material, errors);
			if (!dryRun) foreach (var error in errors) Report($"{fluid.Name}: {error}");
			return selection;
		}
		var applicable = rules.Where(rule =>
		{
			if (rule.ApplicabilityProg is not { } prog) return true;
			if (dryRun) return false; // A diagnostic never executes arbitrary builder code.
			try
			{
				var result = prog.Execute<bool?>(Context(patch, rule, route, source, strength, seconds, quantity));
				if (result is null) Report($"Applicability prog #{prog.Id} returned no Boolean result for {rule.Name}; rule suppressed.");
				return result == true;
			}
			catch (Exception ex) { Report($"Applicability prog #{prog.Id} failed for {rule.Name}: {ex.GetType().Name}."); return false; }
		}).ToArray();
		var overlapErrors = new List<string>();
		var chosen = Choose(applicable, patch.Material, overlapErrors);
		if (!dryRun) foreach (var error in overlapErrors) Report($"{fluid.Name}: {error}");
		return chosen;
	}
	private void Report(string error)
	{
		if (_reportedErrors.Count >= 128 || !_reportedErrors.Add(error)) return;
		var template = _world.GetStaticString("EnvironmentalExposureDiagnostic") ?? "Environmental exposure: {0}";
		try { _world.SystemMessage(string.Format(template, error), true); }
		catch (FormatException) { _world.SystemMessage($"Environmental exposure: {error}", true); }
	}
	private void Notify(IPerceivable owner, string? template, bool exhaustion, params object[] values)
	{
		if (string.IsNullOrEmpty(template)) return;
		var notice = _notices.GetValue(owner, _ => new());
		var now = DateTime.UtcNow;
		if (now < (exhaustion ? notice.ExhaustionNext : notice.Next)) return;
		var interval = _world.GetStaticDouble("EnvironmentalExposureMessageInterval");
		var next = now.AddSeconds(double.IsFinite(interval) ? Math.Clamp(interval, 1, 3600) : 15);
		if (exhaustion) notice.ExhaustionNext = next; else notice.Next = next;
		try { owner.OutputHandler?.Send(string.Format(template, values)); }
		catch (FormatException) { Report("Invalid exposure message template; message suppressed."); }
	}

	public static IReadOnlyList<IEnvironmentalReaction> SelectRules(IFluid fluid, IMaterial material, ExposureRoute route,
		double temperature, out IReadOnlyList<string> diagnostics)
	{
		var errors = new List<string>();
		var result = Choose(FindCandidates(fluid, material, route, temperature, errors), material, errors);
		diagnostics = errors;
		return result;
	}
	private static IReadOnlyList<IEnvironmentalReaction> FindCandidates(IFluid fluid, IMaterial material, ExposureRoute route,
		double temperature, List<string> errors)
	{
		var matches = new List<IEnvironmentalReaction>();
		foreach (var rule in fluid.EnvironmentalReactions.Where(x => x.Version == 2 && x.Routes.HasFlag(route)))
		{
			var validation = rule.ValidationErrors(fluid is IGas).ToArray();
			if (validation.Length > 0) { errors.Add($"{rule.Name}: {string.Join("; ", validation)}"); continue; }
			if (rule.MinimumTemperature is { } min && temperature < min || rule.MaximumTemperature is { } max && temperature > max) continue;
			if (rule.TargetMaterial == material || rule.TargetTags.Any(t => material.Tags.Any(x => x.IsA(t)))) matches.Add(rule);
		}
		return matches;
	}
	private static IReadOnlyList<IEnvironmentalReaction> Choose(IEnumerable<IEnvironmentalReaction> matches, IMaterial material, List<string> errors)
	{
		var selected = new List<IEnvironmentalReaction>();
		foreach (var channel in matches.GroupBy(x => x.Channel, StringComparer.OrdinalIgnoreCase))
		{
			var ordered = channel.OrderByDescending(x => x.Priority).ThenByDescending(x => x.TargetMaterial == material).ToArray();
			if (ordered.Length > 1 && ordered[0].Priority == ordered[1].Priority && (ordered[0].TargetMaterial == material) == (ordered[1].TargetMaterial == material))
			{ errors.Add($"Ambiguous {channel.Key} rules for {material.Name}; channel quarantined."); continue; }
			selected.Add(ordered[0]);
		}
		return selected;
	}

	private object?[] Context(ExposurePatch patch, IEnvironmentalReaction rule, ExposureRoute route,
		string source, double strength, double seconds, double quantity) => new object?[]
	{
		patch.Target, patch.TargetBody?.Id ?? 0L, patch.Part?.Id ?? 0L, patch.Material.Id,
		source, rule.Category, route.ToString(), patch.Location, (int)patch.Layer, strength, seconds, quantity
	};

	private double Modifier(ExposurePatch patch, IEnvironmentalReaction rule, ExposureRoute route, string source,
		double strength, double seconds, double quantity)
	{
		if (rule.NoReaction) return 0.0;
		if (rule.IntensityProg is not { } intensity) return 1.0;
		try
		{
			var args = Context(patch, rule, route, source, strength, seconds, quantity);
			var result = intensity.Execute<double?>(args);
			if (result is { } value && ExposureArithmetic.Valid(value)) return Math.Min(100.0, value);
			Report($"Intensity prog #{intensity.Id} returned an invalid result for {rule.Name}; rule suppressed."); return 0.0;
		}
		catch (Exception ex) { Report($"Intensity prog failed for {rule.Name}: {ex.GetType().Name}; rule suppressed."); return 0.0; }
	}

	public double ThermalModifier(ExposurePatch patch, string source, double seconds, bool dryRun = false)
	{
		if (!EnvironmentalExposureOptions.AllowsCurrent(_world, patch.Target, ExposureRoute.AmbientHeat)) return 0;
		if ((patch.Material as ISolid)?.ExposureProperties.ThermalIntensityProgId is not { } id) return 1.0;
		if (dryRun) return 0.0;
		var prog = _world.FutureProgs.Get(id);
		if (prog is null || !ExposureProgContract.Valid(prog, "intensity"))
		{
			Report($"Ambient intensity prog #{id} for {patch.Material.Name} is missing or has an invalid signature; ambient injury suppressed.");
			return 0;
		}
		try
		{
			var value = prog.Execute<double?>(patch.Target, patch.TargetBody?.Id ?? 0L, patch.Part?.Id ?? 0L, patch.Material.Id,
				source, "heat", ExposureRoute.AmbientHeat.ToString(), patch.Location, (int)patch.Layer,
				patch.Area * patch.Transmission, seconds, 0.0);
			if (value is { } number && ExposureArithmetic.Valid(number)) return Math.Min(100, number);
			Report($"Ambient intensity prog #{id} for {patch.Material.Name} returned an invalid number; ambient injury suppressed.");
		}
		catch (Exception ex) { Report($"Ambient intensity prog #{id} for {patch.Material.Name} failed: {ex.GetType().Name}; ambient injury suppressed."); }
		return 0;
	}

	public IReadOnlyList<ExposureResolution> Liquid(LiquidMixture mixture, bool replenishing,
		IReadOnlyList<ExposurePatch> patches, ExposureSourceKind kind, string source, double seconds,
		double temperature, ExposureRoute route = ExposureRoute.LiquidContact, bool dryRun = false, bool splash = false, ILiquid? excludedLiquid = null)
	{
		if (_resolving || !ExposureArithmetic.Valid(seconds) || seconds <= 0 || mixture.IsEmpty) return Array.Empty<ExposureResolution>();
		var options = EnvironmentalExposureOptions.Read(_world);
		var eligible = patches.Where(x => options.Allows(x.Target, route) && x.StillCurrent && x.Area > 0 && x.Transmission > 0).ToArray();
		if (eligible.Length == 0) return Array.Empty<ExposureResolution>();
		_resolving = true;
		try
		{
			using var healthBatch = ContinuousExposureDamage.BeginHealthBatch();
			var owned = dryRun ? mixture.Clone() : mixture;
			var totals = new Dictionary<(ExposurePatch, IEnvironmentalReaction), ExposureResolution>();
			var exhausted = new List<(IPerceivable Target, string Liquid)>();
			var remaining = Math.Min(seconds, options.MaximumInterval);
			while (remaining > 1e-10 && !owned.IsEmpty)
			{
				var dt = splash || replenishing ? remaining : Math.Min(remaining, options.Substep);
				var initialTotal = owned.TotalVolume;
				foreach (var instance in owned.Instances.Where(x => x.Liquid != excludedLiquid).ToArray())
				{
					if (instance.Amount <= options.MinimumVolume)
					{
						if (!replenishing) owned.RemoveLiquidInstance(instance);
						continue;
					}
					var contacts = new List<(ExposurePatch Patch, IEnvironmentalReaction Rule, double E)>();
					foreach (var patch in eligible)
					{
						// A splash is a bounded volume dose, divided across patches. It is not a heartbeat.
						var e = splash ? instance.Amount / Math.Max(options.SplashReferenceVolume, ExposureDeliveryScope.OriginalVolume ?? initialTotal) * patch.Area / eligible.Sum(x => x.Area) :
							replenishing ? patch.Area * instance.Amount / initialTotal : ExposureArithmetic.ComponentIntensity(instance.Amount, initialTotal, patch.Capacity, patch.Area);
						var rules = Candidates(instance.Liquid, patch, route, temperature, source, e, dt, instance.Amount, dryRun);
						var strengths = rules.Select(rule => (Rule: rule, Modifier: dryRun ? rule.NoReaction ? 0 : 1 : Modifier(patch, rule, route, source, e, dt, instance.Amount)))
							.Where(x => x.Modifier > 0 && patch.StillCurrent && EnvironmentalExposureOptions.AllowsCurrent(_world, patch.Target, route)).ToArray();
						if (splash && !dryRun && strengths.Length > 0) e = ExposureDeliveryScope.ReserveEntryWork(e * dt) / dt;
						foreach (var (rule, modifier) in strengths)
						{
							var intensity = e * patch.Transmission * modifier;
							if (intensity > 0 && patch.StillCurrent) contacts.Add((patch, rule, intensity));
						}
					}
					var demands = contacts.Select(x => new ExposureArithmetic.Demand(x.E, x.Rule.DamageRate,
						x.Rule.PainRate, x.Rule.StunRate, x.Rule.Consumption == ReactionConsumption.PerExposure ? x.Rule.ConsumptionRate : 0)).ToArray();
					IReadOnlyList<ExposureArithmetic.Result> results;
					try { results = ExposureArithmetic.Resolve(demands, dt, replenishing ? null : instance.Amount); }
					catch (ArgumentOutOfRangeException) { Report($"Invalid or overflowing exposure for {instance.Liquid.Name}; interval suppressed."); continue; }
					// Debit once for every simultaneous patch before any damage or notification callback.
					var consumed = results.Sum(x => x.Consumed);
					if (!replenishing && consumed > 0 && consumed >= instance.Amount - options.MinimumVolume)
						exhausted.AddRange(contacts.Select(x => (x.Patch.Target, instance.Liquid.Name)));
					if (!replenishing && consumed > 0) owned.RemoveLiquidVolume(instance, consumed);
					for (var i = 0; i < contacts.Count; i++)
					{
						var (patch, rule, _) = contacts[i]; var result = results[i];
						if (!replenishing && result.Consumed > 0 && rule.SpentLiquid is { } spent)
							owned.AddLiquid(new LiquidInstance { Liquid = spent, Amount = result.Consumed });
						var key = (patch, rule);
						totals.TryGetValue(key, out var old);
						totals[key] = new(patch, rule, (old?.Work ?? 0) + result.Work,
							(old?.RawDamage ?? 0) + result.Damage * options.Scale, (old?.Pain ?? 0) + result.Pain * options.Scale,
							(old?.Stun ?? 0) + result.Stun * options.Scale, (old?.Consumed ?? 0) + result.Consumed);
					}
				}
				remaining -= dt;
			}
			if (!dryRun)
			{
				foreach (var result in totals.Values) Commit(result, route, kind, source, seconds);
				foreach (var notice in exhausted.DistinctBy(x => x.Target, ReferenceEqualityComparer.Instance))
					if (EnvironmentalExposureOptions.AllowsCurrent(_world, notice.Target, route))
						Notify(notice.Target, _world.GetStaticString("EnvironmentalExposureExhausted"), true, notice.Liquid);
			}
			return totals.Values.ToArray();
		}
		finally { _resolving = false; }
	}

	public IReadOnlyList<ExposureResolution> Gas(IFluid fluid, IReadOnlyList<ExposurePatch> patches, ExposureRoute route,
		ExposureSourceKind kind, string source, double strength, double seconds, double temperature, bool dryRun = false, bool evaluateProgs = false)
	{
		var results = new List<ExposureResolution>();
		if (_resolving || !ExposureArithmetic.Valid(strength) || !ExposureArithmetic.Valid(seconds)) return results;
		_resolving = true;
		try
		{
			using var healthBatch = ContinuousExposureDamage.BeginHealthBatch();
			var options = EnvironmentalExposureOptions.Read(_world);
			foreach (var patch in patches.Where(x => options.Allows(x.Target, route)))
			foreach (var rule in Candidates(fluid, patch, route, temperature, source, strength, seconds, 0, dryRun && !evaluateProgs))
			{
				var work = patch.Area * patch.Transmission * strength * seconds * (dryRun && !evaluateProgs ? rule.NoReaction ? 0 : 1 : Modifier(patch, rule, route, source, strength, seconds, 0));
				if (!ExposureArithmetic.Valid(work)) { if (!dryRun) Report($"Invalid or overflowing exposure for {fluid.Name}; interval suppressed."); continue; }
				var result = new ExposureResolution(patch, rule, work, rule.DamageRate * work * options.Scale,
					rule.PainRate * work * options.Scale, rule.StunRate * work * options.Scale, 0);
				results.Add(result);
				if (!dryRun) Commit(result, route, kind, source, seconds);
			}
			return results;
		}
		finally { _resolving = false; }
	}

	internal void Commit(ExposureResolution result, ExposureRoute route, ExposureSourceKind kind, string source, double seconds, bool resistanceApplied = false)
	{
		if (result.Work <= 0 || !result.Patch.StillCurrent || !EnvironmentalExposureOptions.AllowsCurrent(_world, result.Patch.Target, route)) return;
		var context = new ExposureDamageContext(route, kind, source, result.Reaction.Category, result.Reaction.Channel, result.Reaction.Id, seconds, result.Patch.TargetBody?.Id ?? 0);
		var resistance = resistanceApplied ? 1.0 : ResistanceMultiplier(result.Patch, context);
		var actual = ApplyDamage(result.Patch, context, result.Reaction.DamageType, result.RawDamage * resistance, result.Pain * resistance, result.Stun * resistance, true);
		var owner = result.Patch.Target;
		if (actual.Damage + actual.Pain + actual.Stun > 0)
		{
			var template = result.Reaction.Message ?? _world.GetStaticString(owner is IGameItem ? "EnvironmentalExposureItemDeterioration" : "EnvironmentalExposureContinuing");
			Notify(owner, template, false, owner is IGameItem item ? item.Name : result.Reaction.Name,
				owner is IGameItem ? result.Reaction.Name : result.Patch.Part?.FullDescription() ?? "body");
		}
		else if (result.RawDamage + result.Pain + result.Stun > 0)
			Notify(owner, _world.GetStaticString(resistance < 1 ? "EnvironmentalExposureProtection" : "EnvironmentalExposureWarning"), false, result.Reaction.Name);
		try
		{
			result.Reaction.NotificationProg?.Execute(Context(result.Patch, result.Reaction, route, source, result.Work / Math.Max(seconds, 1e-10),
				seconds, result.Consumed).Concat(new object?[] { actual.Damage, actual.Pain, actual.Stun, result.Consumed }).ToArray());
		}
		catch (Exception ex) { Report($"Notification prog for {result.Reaction.Name} failed after commit: {ex.GetType().Name}; work will not be replayed."); }
	}

	public static double ResistanceMultiplier(ExposurePatch patch, ExposureDamageContext context, bool dryRun = false)
	{
		context = context with { TargetBodyId = patch.TargetBody?.Id ?? 0 };
		var effects = patch.Target is IBody body ? body.CombinedEffectsOfType<IExposureResistance>() : patch.Target.EffectsOfType<IExposureResistance>();
		return effects.Where(effect => !dryRun || effect.ApplicabilityProg is null)
			.Aggregate(1.0, (current, effect) => effect.ExposureDamageMultiplier(context, patch.Part) is var value && ExposureArithmetic.Valid(value)
				? Math.Clamp(current * value, 0, 100) : 0);
	}

	public static (double Damage, double Pain, double Stun) ApplyDamage(ExposurePatch patch, ExposureDamageContext context, DamageType type, double damage, double pain, double stun, bool resistanceApplied = false)
	{
		if ((damage <= 0 && pain <= 0 && stun <= 0) || !ExposureArithmetic.Valid(damage) || !ExposureArithmetic.Valid(pain) || !ExposureArithmetic.Valid(stun) ||
			!patch.StillCurrent || !EnvironmentalExposureOptions.AllowsCurrent(patch.Target.Gameworld, patch.Target, context.Route) || patch.Target is IGameItem { Deleted: true }) return default;
		var multiplier = resistanceApplied ? 1.0 : ResistanceMultiplier(patch, context);
		if (!patch.StillCurrent || !EnvironmentalExposureOptions.AllowsCurrent(patch.Target.Gameworld, patch.Target, context.Route)) return default;
		damage *= multiplier; pain *= multiplier; stun *= multiplier;
		if (damage + pain + stun <= 0) return default;
		// Continuous wounds can only accumulate on the addressed part. Avoid copying every
		// other part's wounds for each patch, including when the target is a corpse.
		var before = (patch.TargetBody?.Wounds ?? (patch.Target as IHaveWounds)?.Wounds)?
			.Where(x => ReferenceEquals(x.Bodypart, patch.Part))
			.ToDictionary(x => x, x => (x.CurrentDamage, x.CurrentPain, x.CurrentStun)) ?? new();
		var delivery = new Damage { ExposureContext = context with { TargetBodyId = patch.TargetBody?.Id ?? 0 }, TargetBody = patch.TargetBody, DamageType = type, Bodypart = patch.Part,
			DamageAmount = damage, PainAmount = pain, StunAmount = stun };
		IEnumerable<IWound> wounds = Array.Empty<IWound>();
		// Stable per source/rule/patch: dice inside authored armour formulae cannot change merely
		// because the same continuous interval was divided into a different number of callbacks.
		using var armourRandom = DamageRandom(patch, context);
		switch (patch.Target)
		{
			case IBody body when patch.Part is null || body.Bodyparts.Contains(patch.Part) || body.Organs.Contains(patch.Part):
				wounds = body.PassiveSufferDamage(delivery); break;
			case IGameItem item when !item.Deleted:
				wounds = item.PassiveSufferDamage(delivery); break;
		}
		var committed = wounds.Distinct().ToArray();
		var actual = (committed.Sum(x => Math.Max(0, x.CurrentDamage - before.GetValueOrDefault(x).CurrentDamage)),
			committed.Sum(x => Math.Max(0, x.CurrentPain - before.GetValueOrDefault(x).CurrentPain)),
			committed.Sum(x => Math.Max(0, x.CurrentStun - before.GetValueOrDefault(x).CurrentStun)));
		ContinuousExposureDamage.ProcessWounds(patch.TargetBody, committed);
		return actual;
	}

	internal static IDisposable DamageRandom(ExposurePatch patch, ExposureDamageContext context)
	{
		var bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes($"{context.ReactionId}:{context.SourceIdentity}:{patch.Target.Id}:{patch.Part?.Id}"));
		return ExpressionEngine.Expression.PushRandom(new Random(BitConverter.ToInt32(bytes, 0)));
	}
}
