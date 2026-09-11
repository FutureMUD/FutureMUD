using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Health;

#nullable enable
namespace MudSharp.Magic;

public static class MagicalExposure
{
	private sealed record Part(double Quantity, SubstanceCharge Charge);
	private sealed class DeliveryComparer : IEqualityComparer<(IPerceivable Target, IMagicalSubstance Substance, DrugVector Vector, bool Surface)>
	{
		public bool Equals((IPerceivable Target, IMagicalSubstance Substance, DrugVector Vector, bool Surface) x,
			(IPerceivable Target, IMagicalSubstance Substance, DrugVector Vector, bool Surface) y) =>
			ReferenceEquals(x.Target, y.Target) && x.Substance.Id == y.Substance.Id && x.Vector == y.Vector && x.Surface == y.Surface;
		public int GetHashCode((IPerceivable Target, IMagicalSubstance Substance, DrugVector Vector, bool Surface) value) =>
			HashCode.Combine(System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value.Target), value.Substance.Id, value.Vector, value.Surface);
	}
	private sealed class Batch
	{
		public int Depth;
		public readonly Dictionary<(IPerceivable Target, IMagicalSubstance Substance, DrugVector Vector, bool Surface), List<Part>> Deliveries = new(new DeliveryComparer());
	}
	[ThreadStatic] private static Batch? _batch;
	private sealed class Scope(Batch batch) : IDisposable
	{
		private bool _disposed;
		public void Dispose()
		{
			if (_disposed) return;
			_disposed = true;
			if (--batch.Depth != 0) return;
			_batch = null;
			foreach (var (key, parts) in batch.Deliveries)
			{
				DeliverParts(key.Target, key.Substance, key.Vector, key.Surface, parts);
				if (!key.Surface) continue;
				var surface = (key.Target is ICharacter c ? c.Body : key.Target) as ISurfaceContaminable;
				if (surface is null) continue;
				foreach (var (instance, _) in RetainedLiquids(key.Target))
					if (instance.MagicalCharges.TryGetValue(key.Substance.Id, out var charge))
						foreach (var part in parts.Where(x => x.Charge.Lot == charge.Lot))
						{
							charge.Spent.UnionWith(part.Charge.Spent);
							charge.Suppressed.UnionWith(part.Charge.Suppressed);
						}
				RetainedLiquidsChanged(key.Target);
			}
		}
	}
	/// <summary>Coalesces physical traversal of one delivery action before minimum-dose checks.</summary>
	public static IDisposable BeginExposure()
	{
		_batch ??= new();
		_batch.Depth++;
		return new Scope(_batch);
	}
	public static void Liquid(IPerceivable recipient, LiquidMixture mixture, DrugVector vector, bool surface = false)
	{
		if (recipient is IBody body) recipient = body.Actor;
		if (recipient is not ICharacter && recipient is not IGameItem || mixture.IsEmpty) return;
		using var scope = BeginExposure();
		foreach (var substance in recipient.Gameworld.MagicalSubstances.Where(x => x.Vectors.HasFlag(vector) &&
			x.Bindings.Any(b => b.Carrier == SubstanceCarrier.Liquid && mixture.Instances.Any(i => i.Liquid.Id == b.Id)) && !x.ReadinessErrors.Any()))
			foreach (var group in mixture.Instances
				.Select(x => (Instance: x, Binding: substance.Bindings.FirstOrDefault(b => b.Carrier == SubstanceCarrier.Liquid && b.Id == x.Liquid.Id)))
				.Where(x => x.Binding is not null).ToList())
			{
				if (!group.Instance.MagicalCharges.TryGetValue(substance.Id, out var charge))
					group.Instance.MagicalCharges[substance.Id] = charge = new();
				Queue(recipient, substance, group.Instance.Amount * group.Binding!.QuantityPerUnit, vector, charge, surface);
			}
	}
	internal static IEnumerable<(LiquidInstance Instance, double Quantity)> RetainedLiquids(IPerceivable target)
	{
		if ((target is ICharacter c ? c.Body : target) is ISurfaceContaminable surface)
			foreach (var instance in surface.SurfaceLiquidState.ContaminatingLiquid.Instances) yield return (instance, instance.Amount);
		if (target is IGameItem item && item.GetItemType<IPreparedFood>() is { MagicalIngredientMixture: { } mixture } food)
			foreach (var instance in mixture.Instances) yield return (instance, instance.Amount * food.RemainingServings);
	}
	internal static void RetainedLiquidsChanged(IPerceivable target)
	{
		((target is ICharacter c ? c.Body : target) as ISurfaceContaminable)?.SurfaceLiquidChanged();
		if (target is IGameItem item && item.GetItemType<IPreparedFood>() is { } food) food.Changed = true;
	}
	public static bool HasLiquidPayload(LiquidMixture mixture, DrugVector vector) => mixture.Gameworld.MagicalSubstances?.Any(s =>
		s.Vectors.HasFlag(vector) && !s.ReadinessErrors.Any() && s.Bindings.Any(b => b.Carrier == SubstanceCarrier.Liquid && mixture.Instances.Any(x => x.Liquid.Id == b.Id))) == true;
	public static void Carrier(IPerceivable recipient, SubstanceCarrier carrier, long id, double amount, DrugVector vector)
	{
		if (recipient is IBody body) recipient = body.Actor;
		if (!SubstanceDose.IsPositive(amount)) return;
		using var scope = BeginExposure();
		foreach (var substance in recipient.Gameworld.MagicalSubstances.Where(x => x.Vectors.HasFlag(vector) &&
			x.Bindings.Any(b => b.Carrier == carrier && b.Id == id) && !x.ReadinessErrors.Any()))
			foreach (var binding in substance.Bindings.Where(x => x.Carrier == carrier && x.Id == id))
				Queue(recipient, substance, amount * binding.QuantityPerUnit, vector, new(), false);
	}
	private static void Queue(IPerceivable recipient, IMagicalSubstance substance, double quantity, DrugVector vector, SubstanceCharge charge, bool surface)
	{
		if (!SubstanceDose.IsPositive(quantity)) return;
		var key = (recipient, substance, vector, surface);
		if (!_batch!.Deliveries.TryGetValue(key, out var parts)) _batch.Deliveries[key] = parts = new();
		parts.Add(new(quantity, charge));
	}
	private static void DeliverParts(IPerceivable recipient, IMagicalSubstance substance, DrugVector vector, bool surface, List<Part> parts)
	{
		foreach (var entry in substance.Entries)
		{
			var spell = substance.Gameworld.MagicSpells.Get(entry.SpellId);
			if (spell is not MagicSpell concrete || spell.Trigger.TargetTypes != (recipient is ICharacter ? "character" : "item")) continue;
			var spendsCharge = entry.Lifecycle == SubstanceLifecycle.Activation || entry.Lifecycle == SubstanceLifecycle.Periodic && entry.PulseMode == SubstancePulseMode.Timed;
			var eligible = parts.Where(x => !x.Charge.Suppressed.Contains(entry.Key) && (!spendsCharge || !x.Charge.Spent.Contains(entry.Key))).ToList();
			var dose = Math.Min(entry.MaximumDose, eligible.Sum(x => x.Quantity) / substance.ReferenceDose);
			if (dose <= 0 || dose < entry.MinimumDose) continue;
			if (spendsCharge) foreach (var part in eligible) part.Charge.Spent.Add(entry.Key);
			var context = new SubstanceResolutionContext(recipient, substance, dose);
			if (!SubstanceSpellResolver.CanApply(concrete, context))
			{
				foreach (var part in eligible) part.Charge.Suppressed.Add(entry.Key);
				continue;
			}
			if (entry.Lifecycle == SubstanceLifecycle.Activation)
			{
				SubstanceSpellResolver.Emit(concrete, context);
				var instant = new MagicSpellParent(recipient, spell, null!, substance.Power);
				foreach (var effect in spell.SpellEffects.Where(x => x.IsInstantaneous)) SubstanceSpellResolver.Apply(effect, spell, context, instant, dose);
				if (spell.SpellEffects.All(x => x.IsInstantaneous)) continue;
			}
			var parent = recipient.EffectsOfType<SubstanceExposureEffect>().FirstOrDefault(x =>
				x.SubstanceId == substance.Id && x.Entry.Key == entry.Key && entry.Stacking != SubstanceStacking.Independent);
			if (parent is null)
			{
				parent = new SubstanceExposureEffect(recipient, substance, entry);
				recipient.AddEffect(parent, TimeSpan.FromSeconds(1));
			}
			parent.AddExposure(eligible.Select(x => (x.Quantity, x.Charge)), vector, surface);
		}
	}
}
