using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Planes;

#nullable enable

namespace MudSharp.Form.Material;

/// <summary>A delivery has one bounded entry dose, even when split across parts or callbacks.</summary>
public sealed class ExposureDeliveryScope : IDisposable
{
	[ThreadStatic] private static ExposureDeliveryScope? _current;
	private readonly bool _owns;
	public static double? OriginalVolume => _current?._volume;
	public static bool IsRefresh => _current?._refresh == true;
	private readonly double _volume;
	private readonly bool _refresh;
	private readonly HashSet<IGameItem> _activeItems = new(ReferenceEqualityComparer.Instance);
	private double _entryWork = 1.0;
	private sealed class ExitScope(Action exit) : IDisposable { public void Dispose() => exit(); }
	public static IDisposable? EnterItem(IGameItem item)
	{
		var scope = _current!;
		return scope._activeItems.Add(item) ? new ExitScope(() => scope._activeItems.Remove(item)) : null;
	}
	public static double ReserveEntryWork(double requested)
	{
		if (_current is null) return requested;
		var result = Math.Min(_current._entryWork, Math.Max(0, requested));
		_current._entryWork -= result;
		return result;
	}
	public ExposureDeliveryScope(double volume, bool refresh = false)
	{
		_volume = volume; _refresh = refresh;
		if (_current is null) { _current = this; _owns = true; }
	}
	public void Dispose() { if (_owns) _current = null; }
}

public static class ExposureTransport
{
	internal static Dictionary<IExternalBodypart, double> BodyAreas(IBody body)
	{
		var parts = body.Bodyparts.OfType<IExternalBodypart>().Distinct().ToArray();
		var total = parts.Sum(x => Math.Max(0, x.RelativeHitChance));
		return parts.ToDictionary(x => x, x => total > 0 ? Math.Max(0, x.RelativeHitChance) / total : 1.0 / parts.Length);
	}

	public static double BodyArea(IBody body, IExternalBodypart part)
	{
		var total = body.Bodyparts.OfType<IExternalBodypart>().Sum(x => Math.Max(0, x.RelativeHitChance));
		return total > 0 ? Math.Max(0, part.RelativeHitChance) / total : 1.0 / Math.Max(1, body.Bodyparts.OfType<IExternalBodypart>().Count());
	}
	public static double ItemArea(IGameItem item) => Math.Clamp(Math.Pow((int)item.Size / 5.0, 2), 0.04, 16.0);
	public static double Transmission(IGameItem item, ExposureRoute route)
	{
		if (item.Deleted) return 1.0;
		if (route == ExposureRoute.LiquidContact && item.GetItemType<IWearable>()?.Waterproof == true) return 0.0;
		return item.Material.ExposureProperties.Transmission(route);
	}

	public static IReadOnlyList<ExposurePatch> ExternalPatches(IBody body, IEnumerable<IExternalBodypart> parts, ExposureRoute route)
	{
		var patches = new List<ExposurePatch>();
		var areas = BodyAreas(body);
		var coating = body.LiquidAbsorbtionAmounts.Coating;
		foreach (var part in parts.OrderBy(x => x.Id))
		{
			var area = areas.GetValueOrDefault(part);
			var transmission = 1.0;
			var depth = 0;
			foreach (var item in body.WornItemsFor(part).Reverse().Distinct().ToArray())
			{
				if (item.Deleted) continue;
				var capacity = item.LiquidAbsorbtionAmounts;
				patches.Add(new(item, null, item.Material, area, capacity.Coating + capacity.Absorb, transmission, depth++));
				transmission *= Transmission(item, route);
			}
			patches.Add(new(body, part, body.GetMaterial(part), area, coating * area, transmission, depth));
		}
		// A garment's total contacted area is distributed across its wear regions once.
		return patches.GroupBy(x => (x.Target, x.Part, x.Material, x.Capacity, x.LayerDepth)).Select(g =>
			new ExposurePatch(g.Key.Target, g.Key.Part, g.Key.Material, g.Sum(x => x.Area), g.Key.Capacity,
				g.Sum(x => x.Area * x.Transmission) / Math.Max(1e-10, g.Sum(x => x.Area)), g.Key.LayerDepth)).ToArray();
	}

	public static IReadOnlyList<ExposurePatch> ItemPatches(IGameItem item, ExposureRoute route)
	{
		if (item.GetItemType<ICorpse>()?.OriginalBody is not { } body)
		{
			var capacity = item.LiquidAbsorbtionAmounts;
			return new[] { new ExposurePatch(item, null, item.Material, ItemArea(item), capacity.Coating + capacity.Absorb) };
		}
		return ExternalPatches(body, body.Bodyparts.OfType<IExternalBodypart>(), route).Select(p =>
			new ExposurePatch(ReferenceEquals(p.Target, body) ? item : p.Target, p.Part, p.Material,
				p.Area, p.Capacity, p.Transmission, p.LayerDepth, item)).ToArray();
	}

	public static void Body(IBody body, LiquidMixture mixture, IEnumerable<IExternalBodypart> parts, LiquidExposureDirection direction, double doseShare = 1.0, IGameItem? remains = null)
	{
		if (mixture.IsEmpty || !EnvironmentalExposureService.Physical((IPerceivable?)remains ?? body.Actor)) return;
		using var batch = MagicalExposure.BeginExposure();
		using var delivery = new ExposureDeliveryScope(mixture.TotalVolume);
		EnvironmentalExposureService.For(body.Gameworld).Settle(body);
		OnFire.ExtinguishWith((IPerceivable?)remains ?? body.Actor, mixture);
		// The service owns the drying clock during immersion/soak refreshes. Resolving the
		// entire body again for every refreshed part is both quadratic and advances it early.
		if (!ExposureDeliveryScope.IsRefresh) body.ResolveSurfaceLiquidDrying();
		var targets = parts.DistinctBy(x => x.Id).OrderBy(x => x.Id).ToArray();
		var total = targets.Sum(x => Math.Max(0, x.RelativeHitChance));
		var volume = mixture.TotalVolume;
		foreach (var part in targets)
		{
			var quantity = volume * (total > 0 ? Math.Max(0, part.RelativeHitChance) / total : 1.0 / targets.Length);
			var local = mixture.RemoveLiquidVolume(quantity);
			if (local is null) continue;
			var clothing = direction == LiquidExposureDirection.Irrelevant ? body.WornItemsFor(part).Reverse().Distinct().ToArray() : Array.Empty<IGameItem>();
			var held = direction == LiquidExposureDirection.Irrelevant ? body.HeldOrWieldedItemsFor(part).Where(x => !x.Deleted && EnvironmentalExposureService.Physical(x)).Distinct().ToArray() : Array.Empty<IGameItem>();
			var share = doseShare / (1 + clothing.Length);
			if (direction == LiquidExposureDirection.Irrelevant)
			{
				// Held objects share the delivered volume with the hand; they are not worn barriers.
				var heldVolume = held.Length > 0 ? local.TotalVolume * 0.5 / held.Length : 0;
				foreach (var item in held)
				{
					var heldDose = local.RemoveLiquidVolume(heldVolume);
					if (heldDose is not null) Item(item, heldDose, null, LiquidExposureDirection.FromOnTop, doseShare);
				}
				foreach (var item in clothing)
				{
					if (local.IsEmpty) break;
					if (ItemSurface(item, local, part, direction, share)) { Runoff(item, local); break; }
					if (Transmission(item, ExposureRoute.LiquidContact) <= 0) { Runoff(item, local); break; }
				}
			}
			if (local.IsEmpty) continue;
			var state = ((ILocalisedSurfaceLiquidState)body.SurfaceLiquidState).ForPart(part);
			var capacity = body.LiquidAbsorbtionAmountsForBodyparts(new[] { part }).Coating;
			Contact((IPerceivable?)remains ?? body, local, new((IPerceivable?)remains ?? body, part, body.GetMaterial(part), BodyArea(body, part), capacity), share);
			Clean(body, state, local);
			Retain(body, state, local, capacity, direction, new[] { part });
			Runoff((IPerceivable?)remains ?? body, local);
		}
		EnvironmentalExposureService.For(body.Gameworld).Track((IPerceivable?)remains ?? body);
	}

	public static void Item(IGameItem item, LiquidMixture mixture, IBodypart? part, LiquidExposureDirection direction, double doseShare = 1.0)
	{
		if (mixture.IsEmpty || item.Deleted || !EnvironmentalExposureService.Physical(item)) return;
		using var batch = MagicalExposure.BeginExposure();
		using var delivery = new ExposureDeliveryScope(mixture.TotalVolume);
		using var traversal = ExposureDeliveryScope.EnterItem(item);
		if (traversal is null) return;
		EnvironmentalExposureService.For(item.Gameworld).Settle(item);
		if (item.GetItemType<ICorpse>()?.OriginalBody is { } corpseBody)
		{
			var corpseParts = corpseBody.Bodyparts.OfType<IExternalBodypart>().Where(p => part is null || ReferenceEquals(p, part));
			Body(corpseBody, mixture, corpseParts, LiquidExposureDirection.Irrelevant, doseShare, item);
			return;
		}
		var downstream = item.InInventoryOf is { } wearer && direction is LiquidExposureDirection.FromOnTop or LiquidExposureDirection.FromUnderneath
			? wearer.Bodyparts.OfType<IExternalBodypart>().Where(p => (part is null || p == part) && wearer.WornItemsFor(p).Contains(item))
				.Select(p => wearer.WornItemsFor(p).Count()).DefaultIfEmpty(0).Max() : 0;
		var share = doseShare / Math.Max(1, downstream + 1);
		if (ItemSurface(item, mixture, part, direction, share)) { Runoff(item, mixture); return; }
		if (mixture.IsEmpty || item.Deleted) return;
		var transmission = Transmission(item, ExposureRoute.LiquidContact);
		if (transmission <= 0) { Runoff(item, mixture); return; }
		if (direction != LiquidExposureDirection.FromInside && item.GetItemType<IOpenable>()?.IsOpen != false)
		{
			foreach (var child in item.GetItemTypes<IContainer>().SelectMany(x => x.Contents).Distinct().ToArray())
			{
				if (mixture.IsEmpty) break;
				Item(child, mixture, null, LiquidExposureDirection.FromContainer, share);
			}
		}
		if (direction is LiquidExposureDirection.FromInside or LiquidExposureDirection.Irrelevant && item.ContainedIn is { } container)
			Item(container, mixture, part, LiquidExposureDirection.FromInside, share);
		if (item.GetItemType<IBelt>() is { } belt)
			foreach (var attached in belt.ConnectedItems.ToArray()) Item(attached.Parent, mixture, null, LiquidExposureDirection.FromOnTop, share);
		if (item.InInventoryOf is { } body && direction is LiquidExposureDirection.FromOnTop or LiquidExposureDirection.FromUnderneath)
		{
			var parts = part is IExternalBodypart external ? new[] { external } :
				body.Bodyparts.OfType<IExternalBodypart>().Where(x => body.WornItemsFor(x).Contains(item)).ToArray();
			var initial = mixture.TotalVolume;
			var sum = parts.Sum(x => Math.Max(0, x.RelativeHitChance));
			foreach (var p in parts)
			{
				var local = mixture.RemoveLiquidVolume(initial * (sum > 0 ? p.RelativeHitChance / sum : 1.0 / parts.Length));
				if (local is null) continue;
				var layers = body.WornItemsFor(p);
				if (direction == LiquidExposureDirection.FromOnTop) layers = layers.Reverse();
				foreach (var inner in layers.SkipWhile(x => x != item).Skip(1).ToArray())
				{
					if (ItemSurface(inner, local, p, direction, share)) { Runoff(inner, local); break; }
					if (Transmission(inner, ExposureRoute.LiquidContact) <= 0) { Runoff(inner, local); break; }
				}
				if (direction == LiquidExposureDirection.FromOnTop) Body(body, local, new[] { p }, LiquidExposureDirection.FromOnTop, share);
				else Runoff(item, local);
			}
		}
		if (direction is not (LiquidExposureDirection.FromContainer or LiquidExposureDirection.FromInside)) Runoff(item, mixture);
	}

	private static bool ItemSurface(IGameItem item, LiquidMixture mixture, IBodypart? part, LiquidExposureDirection direction, double doseShare = 1.0)
	{
		if (mixture.IsEmpty || item.Deleted) return true;
		MudSharp.Effects.Concrete.OnFire.ExtinguishWith(item, mixture);
		item.ResolveSurfaceLiquidDrying();
		var amounts = item.LiquidAbsorbtionAmounts;
		var locationOwner = item.LocationLevelPerceivable;
		var location = locationOwner?.Location;
		var point = location is null ? null : (double?)RouteSpatialService.Instance.GetEffectiveLocation(locationOwner!).RoutePositionMetres;
		var layer = item.RoomLayer;
		Contact(item, mixture, new(item, null, item.Material, ItemArea(item), amounts.Coating + amounts.Absorb), doseShare);
		if (item.Deleted)
		{
			if (location is MudSharp.Construction.Room room) room.AddLiquidToSurfaceAt(mixture, layer, point);
			else location?.AddLiquidToSurface(mixture, layer, locationOwner);
			return true;
		}
		foreach (var component in item.Components.ToArray()) if (component.ExposeToLiquid(mixture) || mixture.IsEmpty) return true;
		Clean(item, item.SurfaceLiquidState, mixture);
		Retain(item, item.SurfaceLiquidState, mixture, amounts.Coating + amounts.Absorb, direction, null);
		// The fraction blocked by a permeable surface runs off, and remains a real finite resource.
		var blocked = mixture.RemoveLiquidVolume(mixture.TotalVolume * (1.0 - Transmission(item, ExposureRoute.LiquidContact)));
		if (blocked is not null) Runoff(item, blocked);
		EnvironmentalExposureService.For(item.Gameworld).Track(item);
		return false;
	}

	private static void Clean(IPerceivable owner, ISurfaceLiquidState state, LiquidMixture mixture)
	{
		foreach (var effect in owner.EffectsOfType<ICleanableEffect>().Where(x => x is not SurfaceContaminationEffect &&
			mixture.Instances.Any(i => x.LiquidRequired is not null && i.Liquid.LiquidCountsAs(x.LiquidRequired))).ToArray())
			if (effect.CleanWithLiquid(mixture, mixture.TotalVolume)) owner.RemoveEffect(effect, true);
		state.CleanWithLiquid(mixture, mixture.TotalVolume);
	}

	public static void PassInward(IGameItem from, LiquidMixture mixture)
	{
		if (from.InInventoryOf is not { } body) return;
		var parts = body.Bodyparts.OfType<IExternalBodypart>().Where(p => body.WornItemsFor(p).Contains(from)).ToArray();
		var total = parts.Sum(p => BodyArea(body, p));
		var volume = mixture.TotalVolume;
		foreach (var part in parts)
		{
			var local = mixture.RemoveLiquidVolume(volume * (total > 0 ? BodyArea(body, part) / total : 1.0 / parts.Length));
			if (local is null) continue;
			foreach (var inner in body.WornItemsFor(part).Reverse().SkipWhile(x => x != from).Skip(1).ToArray())
			{
				if (local.IsEmpty) break;
				if (ItemSurface(inner, local, part, LiquidExposureDirection.FromOnTop)) { Runoff(inner, local); break; }
			}
			if (!local.IsEmpty) Body(body, local, new[] { part }, LiquidExposureDirection.FromOnTop);
		}
	}

	private static void Contact(IPerceivable owner, LiquidMixture mixture, ExposurePatch patch, double doseShare)
	{
		if (ExposureDeliveryScope.IsRefresh) return;
		EnvironmentalExposureService.For(owner.Gameworld).Resolver.Liquid(mixture, false, new[] { patch },
			ExposureSourceKind.Splash, "delivery", doseShare, owner.Location?.CurrentTemperature(null) ?? 20, splash: true);
	}

	public static void Retain(IPerceivable owner, ISurfaceLiquidState state, LiquidMixture incoming, double capacity,
		LiquidExposureDirection direction, IEnumerable<IExternalBodypart>? parts)
	{
		if (incoming.IsEmpty) return;
		// Exchange a bounded local volume even when already saturated. Displacement is runoff, never deletion.
		var amount = Math.Min(incoming.TotalVolume, Math.Max(0, capacity));
		var displaced = state.RemoveLiquidVolume(Math.Max(0, state.LiquidVolume + amount - capacity));
		var retained = incoming.RemoveLiquidVolume(amount);
		if (retained is not null)
		{
			MagicalExposure.Liquid(owner, retained, DrugVector.Touched, true);
			state.AddLiquid(retained);
			LiquidExposureStrategies.SurfaceReactions.Expose(owner, retained, direction, parts);
		}
		// Displaced wetness rejoins the finite delivery and follows the remaining layer topology.
		if (displaced is not null) incoming.AddLiquid(displaced);
	}

	public static void Runoff(IPerceivable owner, LiquidMixture mixture)
	{
		if (mixture.IsEmpty) return;
		var locationOwner = owner is IGameItem item ? item.LocationLevelPerceivable : owner is IBody body ? body.Actor : owner;
		if (locationOwner?.Location is null)
		{
			// During loading or detached inventory manipulation there is no world surface to own runoff.
			// Keep the real liquid on its surface until a location is available instead of losing it.
			if (owner is ISurfaceContaminable surface)
			{
				surface.SurfaceLiquidState.AddLiquid(mixture);
				mixture.SetLiquidVolume(0);
			}
			return;
		}
		PuddleGameItemComponentProto.TopUpOrCreateNewPuddle(mixture, locationOwner.Location, locationOwner.RoomLayer, locationOwner);
		mixture.SetLiquidVolume(0);
	}
}
