using System.Runtime.CompilerServices;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Planes;
using MudSharp.Vehicles;

#nullable enable

namespace MudSharp.Form.Material;

/// <summary>Active simulation clock. No persisted wall-clock interval is ever replayed as injury.</summary>
public sealed class EnvironmentalExposureService
{
	private static readonly ConditionalWeakTable<IFuturemud, EnvironmentalExposureService> Services = new();
	public static EnvironmentalExposureService For(IFuturemud world) => Services.GetValue(world, x => new(x));
	public static void TrackExisting(IFuturemud world, IPerceivable target)
	{
		if (Services.TryGetValue(world, out var service)) service.Track(target);
	}
	public static void ForgetExisting(IFuturemud world, IPerceivable target)
	{
		if (Services.TryGetValue(world, out var service)) service.Forget(target);
	}
	private readonly IFuturemud _world;
	private readonly HashSet<IPerceivable> _active = new(ReferenceEqualityComparer.Instance);
	private readonly HashSet<IPerceivable> _pending = new(ReferenceEqualityComparer.Instance);
	private readonly HashSet<Room> _weatherRooms = new(ReferenceEqualityComparer.Instance);
	private DateTime _lastWeather;
	private readonly Dictionary<IBody, DateTime> _breaths = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IBody, bool> _suppliedBreaths = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<IBody, bool> _airflowBreaths = new(ReferenceEqualityComparer.Instance);
	private readonly Dictionary<(IPerceivable, IBodypart?), ExposureResolution> _atmosphericThermal = new(PatchIdentity.Instance);
	private sealed class PatchIdentity : IEqualityComparer<(IPerceivable, IBodypart?)>
	{
		public static readonly PatchIdentity Instance = new();
		public bool Equals((IPerceivable, IBodypart?) a, (IPerceivable, IBodypart?) b) => ReferenceEquals(a.Item1, b.Item1) && ReferenceEquals(a.Item2, b.Item2);
		public int GetHashCode((IPerceivable, IBodypart?) value) => HashCode.Combine(RuntimeHelpers.GetHashCode(value.Item1), value.Item2 is null ? 0 : RuntimeHelpers.GetHashCode(value.Item2));
	}
	private readonly ConditionalWeakTable<LiquidMixture, object> _watchedVessels = new();
	private readonly ConditionalWeakTable<IPerceivable, object> _resumed = new();
	private readonly ConditionalWeakTable<IBody, IGameItem> _remains = new();
	private DateTime _last;
	private bool _advancing;
	private EnvironmentalExposureOptions _options;
	public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;
	public EnvironmentalExposureResolver Resolver { get; }
	public int ActiveCount => _active.Count;

	public EnvironmentalExposureService(IFuturemud world, Func<DateTime>? clock = null)
	{
		Clock = clock ?? (() => DateTime.UtcNow);
		_world = world; _options = EnvironmentalExposureOptions.Read(world); Resolver = new(world); _last = Clock();
		if (world.HeartbeatManager is not null) world.HeartbeatManager.SecondHeartbeat += Tick;
	}

	private static IPerceivable Subject(IPerceivable target) => target is ICharacter ch ? ch.Body : target;
	public void Settle(IPerceivable target) { Advance(Clock()); }
	public IDisposable Change(IPerceivable target)
	{
		Settle(target);
		var previousLocation = target.Location;
		var previousLayer = target.RoomLayer;
		if (target is IRoom room) return new ChangeScope(() => RefreshRoom(room));
		return new ChangeScope(() =>
		{
			Track(target);
			if (Subject(target) is IBody breathingBody && (target.Location != previousLocation || target.RoomLayer != previousLayer))
				NoInhalation(breathingBody);
			if (target is IGameItem item && item.InInventoryOf is { } body) Track(body);
		});
	}
	private sealed class ChangeScope(Action done) : IDisposable { public void Dispose() => done(); }
	public IDisposable DefinitionsChanging()
	{
		Advance(Clock());
		return new ChangeScope(Refresh);
	}
	public static IDisposable Changing(IPerceivable target) => target.Gameworld is { } world && Services.TryGetValue(world, out var service)
		? service.Change(target) : new ChangeScope(() => { });
	public static IDisposable ChangingDefinitions(IFuturemud world) => Services.TryGetValue(world, out var service)
		? service.DefinitionsChanging() : new ChangeScope(() => { });
	public static void SettleExisting(IPerceivable target)
	{
		if (target.Gameworld is { } world && Services.TryGetValue(world, out var service)) service.Settle(target);
	}
	public static IDisposable ChangingEnvironment(IRoom room)
	{
		if (room.Gameworld is not { } world || !Services.TryGetValue(world, out var service)) return new ChangeScope(() => { });
		service.Advance(service.Clock());
		return new ChangeScope(() => service.RefreshRoom(room));
	}
	public static IDisposable ChangingWeather(IFuturemud world, MudSharp.Climate.IWeatherController controller)
	{
		if (!Services.TryGetValue(world, out var service)) return new ChangeScope(() => { });
		var previousWeather = controller.CurrentWeatherEvent;
		var previousTemperature = controller.CurrentTemperature;
		service.Advance(service.Clock());
		return new ChangeScope(() =>
		{
			if (ReferenceEquals(previousWeather, controller.CurrentWeatherEvent) && previousTemperature == controller.CurrentTemperature) return;
			foreach (var room in service._weatherRooms.Where(x => x.WeatherController == controller).ToArray()) service.RefreshRoom(room);
		});
	}

	public void Track(IPerceivable target)
	{
		if (_world.SaveManager?.MudBootingMode == true) return;
		if ((target is IGameItem trackedItem ? trackedItem.LocationLevelPerceivable?.Location : target.Location) is Room physicalRoom)
			_weatherRooms.Add(physicalRoom);
		if (target is IGameItem remains && remains.GetItemType<ICorpse>()?.OriginalBody is { } remainsBody)
		{
			if (!_resumed.TryGetValue(remainsBody, out _))
			{
				_resumed.Add(remainsBody, new object());
				DryAt(remainsBody.SurfaceLiquidState, remainsBody, Clock());
			}
			_remains.Remove(remainsBody);
			_remains.Add(remainsBody, remains);
			_active.Remove(remainsBody);
		}
		if (_advancing) { _pending.Add(Subject(target)); return; }
		Advance(Clock());
		target = Subject(target);
		if (target is not (IBody or IGameItem)) return;
		if (!_resumed.TryGetValue(target, out _))
		{
			_resumed.Add(target, new object());
			if (target is ISurfaceContaminable surface) DryAt(surface.SurfaceLiquidState, target, Clock());
		}
		if (target is IBody body) _breaths.TryAdd(body, Clock());
		if (target is IGameItem item)
			foreach (var vessel in item.GetItemTypes<ILiquidContainer>().Where(x => x.OwnsLiquidMixture))
				if (vessel.LiquidMixture is { } liquid)
				{
					liquid.BeforeMutation = () => Settle(item);
					if (!_watchedVessels.TryGetValue(liquid, out _))
					{
						_watchedVessels.Add(liquid, new object());
						liquid.OnLiquidMixtureChanged += _ => { vessel.Changed = true; Track(item); };
					}
				}
		if (IsActive(target)) _active.Add(target); else _active.Remove(target);
	}
	public void Forget(IPerceivable target)
	{
		Settle(target); target = Subject(target); _active.Remove(target);
		if (target is IGameItem remains && remains.GetItemType<ICorpse>()?.OriginalBody is { } original &&
			_remains.TryGetValue(original, out var registered) && ReferenceEquals(registered, remains)) _remains.Remove(original);
		if (target is IBody body) { _breaths.Remove(body); _suppliedBreaths.Remove(body); _airflowBreaths.Remove(body); }
	}
	public void Refresh()
	{
		Advance(Clock());
		_options = EnvironmentalExposureOptions.Read(_world);
		Resolver.Invalidate();
		_last = Clock();
		_active.Clear();
		foreach (var actor in _world.Actors) Track(actor);
		foreach (var item in _world.Items) Track(item);
		foreach (var room in _world.Rooms.OfType<Room>().Where(x => x.SurfaceLiquidStates.Any(s => s.State.IsWet))) _weatherRooms.Add(room);
		foreach (var body in _breaths.Keys.ToArray()) _breaths[body] = _last;
	}
	public void RefreshRoom(IRoom room)
	{
		if (room is Room physicalRoom) _weatherRooms.Add(physicalRoom);
		if (_advancing) return;
		foreach (var actor in room.Characters) Track(actor);
		foreach (var item in room.GameItems) Track(item);
	}

	private bool IsActive(IPerceivable target)
	{
		if (target is IGameItem { Deleted: true }) return false;
		if (target is IBody ownedBody && _remains.TryGetValue(ownedBody, out _)) return false;
		if (target is IGameItem remains && remains.GetItemType<ICorpse>()?.OriginalBody is { } corpseBody &&
			corpseBody.SurfaceLiquidState.IsWet) return true;
		if (target is ISurfaceContaminable surface && surface.SurfaceLiquidState.IsWet) return true;
		if (_options.Mode != EnvironmentalExposureMode.Enabled) return false;
		if (target is IGameItem item && item.GetItemTypes<ILiquidContainer>().Any(x => x.OwnsLiquidMixture && x.LiquidMixture?.Instances.Any(i => i.Liquid.EnvironmentalReactions.Any()) == true)) return true;
		var location = target is IGameItem gi ? gi.LocationLevelPerceivable?.Location : target.Location;
		if (location is null) return false;
		if (location.Atmosphere?.EnvironmentalReactions.Any() == true || location.EffectsOfType<TrapGasCloudEffect>().Any()) return true;
		if (location.IsSwimmingLayer(target.RoomLayer) && location.Terrain(null).WaterFluid is ILiquid liquid && liquid.EnvironmentalReactions.Any()) return true;
		if (location is IRoomLiquidSurface room && room.SurfaceLiquidStates.Any(x => x.State.ContaminatingLiquid.Instances.Any(i => i.Liquid.EnvironmentalReactions.Any()))) return true;
		var temperature = location.CurrentTemperature(null);
		var anatomy = target as IBody ?? (target as IGameItem)?.GetItemType<ICorpse>()?.OriginalBody;
		return anatomy is { } b ? b.Bodyparts.Any(p => b.GetMaterial(p) is ISolid { HeatDamagePoint: { } t } && temperature > t) :
			target is IGameItem g && g.Material.HeatDamagePoint is { } threshold && temperature > threshold;
	}

	private void Tick()
	{
		if ((Clock() - _last).TotalSeconds >= _options.Interval) Advance(Clock());
		if ((Clock() - _lastWeather).TotalSeconds < 5 || _advancing) return;
		_lastWeather = Clock();
		_advancing = true;
		try
		{
			foreach (var room in _weatherRooms.ToArray())
			{
				room.SurfaceWeatherTick();
				if (!room.Characters.Any() && !room.GameItems.Any() && !room.SurfaceLiquidStates.Any(x => x.State.IsWet)) _weatherRooms.Remove(room);
			}
		}
		finally { _advancing = false; }
		foreach (var target in _pending.ToArray()) if (IsActive(target)) _active.Add(target); else _active.Remove(target);
		_pending.Clear();
	}
	public void Advance(DateTime now)
	{
		if (_world.SaveManager?.MudBootingMode == true) { _last = now; return; }
		// Serialisation must never enqueue another interval's wounds while the save queue is draining.
		if (_world.SaveManager?.Flushing == true) return;
		if (_advancing || now <= _last) return;
		var elapsed = Math.Min((now - _last).TotalSeconds, _options.MaximumInterval);
		_last = now;
		var options = EnvironmentalExposureOptions.Read(_world);
		if (options != _options) { _options = options; return; }
		_advancing = true;
		try
		{
			Resolver.Invalidate();
			var remaining = elapsed;
			while (remaining > 1e-10)
			{
				using var healthBatch = ContinuousExposureDamage.BeginHealthBatch();
				_atmosphericThermal.Clear();
				var seconds = Math.Min(remaining, options.Substep);
				var targets = _active.OrderBy(x => x.FrameworkItemType).ThenBy(x => x.Id).ToArray();
				var puddles = new Dictionary<ISurfaceLiquidState, List<ExposurePatch>>(ReferenceEqualityComparer.Instance);
				foreach (var target in targets)
				{
					if (target is IGameItem { Deleted: true }) continue;
					if (options.Mode == EnvironmentalExposureMode.Enabled)
					{
						if (target is IBody body && !_remains.TryGetValue(body, out _)) AdvanceBody(body, seconds, puddles);
						else if (target is IGameItem item) AdvanceItem(item, seconds, now - TimeSpan.FromSeconds(remaining - seconds));
					}
					else if (target is IGameItem corpseItem && corpseItem.GetItemType<ICorpse>()?.OriginalBody is { } originalBody)
						DryAt(originalBody.SurfaceLiquidState, originalBody, now - TimeSpan.FromSeconds(remaining - seconds));
					if (target is ISurfaceContaminable surface) DryAt(surface.SurfaceLiquidState, target, now - TimeSpan.FromSeconds(remaining - seconds));
				}
				foreach (var (state, patches) in puddles)
					foreach (var stage in patches.GroupBy(x => x.LayerDepth).OrderBy(x => x.Key))
						Resolver.Liquid(state.ContaminatingLiquid, false, stage.ToArray(), ExposureSourceKind.Puddle, "puddle", seconds,
							patches[0].Target.Location?.CurrentTemperature(null) ?? 20);
				foreach (var target in _pending.ToArray()) if (IsActive(target)) _active.Add(target); else _active.Remove(target);
				_pending.Clear();
				remaining -= seconds;
			}
		}
		finally { _advancing = false; }
		foreach (var target in _active.ToArray()) if (!IsActive(target)) _active.Remove(target);
	}

	private void DryAt(ISurfaceLiquidState state, IPerceivable target, DateTime now)
	{
		if (state.IsEmpty) return;
		var duration = Math.Max(1, _world.GetStaticDouble(target is IBody ? "BodyLiquidContaminationEffectDuration" : "LiquidContaminationEffectDuration") * Math.Max(state.ContaminatingLiquid.RelativeEnthalpy, double.Epsilon));
		var rate = target is IGameItem item ? item.TimeRateMultiplier(ItemTimeRateType.SurfaceLiquidDrying) : 1.0;
		if (rate <= 0) { state.LastResolvedUtc = now; return; }
		state.ResolveDrying(TimeSpan.FromSeconds(duration / rate), 0.02 / (_world.UnitManager?.BaseFluidToLitres ?? 1), 0.1, utcNow: now);
	}

	internal static bool Physical(IPerceivable target) => !target.SuspendsPhysicalContact() &&
		(target.Gameworld.DefaultPlane is null || target.CurrentPlane() == target.Gameworld.DefaultPlane);
	public static IExternalBodypart[] ImmersedParts(IBody body)
	{
		if (body.Location is null || !body.Location.IsSwimmingLayer(body.RoomLayer) || body.PositionState == PositionFlying.Instance ||
			body.Actor.IsProtectedFromSurfaceWater(out _) || !Physical(body.Actor)) return Array.Empty<IExternalBodypart>();
		return body.Bodyparts.OfType<IExternalBodypart>().Where(x => body.Location.IsUnderwaterLayer(body.RoomLayer) ||
			x.Orientation is not (Orientation.High or Orientation.Highest) && x.BodypartType is not (BodypartTypeEnum.Mouth or BodypartTypeEnum.Blowhole)).ToArray();
	}

	public void RefreshImmersion(IBody body, bool fillOnly = false)
	{
		if (body.Location?.Terrain(null).WaterFluid is not ILiquid liquid) return;
		var parts = ImmersedParts(body);
		if (!fillOnly) body.ResolveSurfaceLiquidDrying();
		var coating = body.LiquidAbsorbtionAmounts.Coating;
		var areas = ExposureTransport.BodyAreas(body);
		using var refresh = new ExposureDeliveryScope(coating, true);
		foreach (var part in parts)
		{
			// Replenishing terrain creates only the bounded quantity actually used to exchange local wetness.
			var area = areas.GetValueOrDefault(part);
			var layers = body.WornItemsFor(part).Distinct().ToArray();
			var amount = coating * area + layers.Sum(x => x.LiquidAbsorbtionAmounts.Coating + x.LiquidAbsorbtionAmounts.Absorb) * area;
			if (fillOnly)
				amount = Math.Max(0, amount - ((ILocalisedSurfaceLiquidState)body.SurfaceLiquidState).ForPart(part).LiquidVolume -
					layers.Sum(x => x.SurfaceLiquidState.LiquidVolume) * area);
			if (amount <= _options.MinimumVolume) continue;
			ExposureTransport.Body(body, new LiquidMixture(liquid, amount, _world), new[] { part }, LiquidExposureDirection.Irrelevant);
		}
		foreach (var item in parts.SelectMany(body.HeldOrWieldedItemsFor).Distinct().ToArray()) RefreshImmersion(item, liquid, fillOnly);
	}
	public void RefreshImmersion(IGameItem item, ILiquid liquid, bool fillOnly = false)
	{
		if (item.GetItemType<ICorpse>()?.OriginalBody is { } body)
		{
			if (!fillOnly) body.ResolveSurfaceLiquidDrying();
			var coating = body.LiquidAbsorbtionAmounts.Coating;
			var areas = ExposureTransport.BodyAreas(body);
			using var bodyRefresh = new ExposureDeliveryScope(coating, true);
			foreach (var part in body.Bodyparts.OfType<IExternalBodypart>())
			{
				var area = areas.GetValueOrDefault(part);
				var layers = body.WornItemsFor(part).Distinct().ToArray();
				var amount = coating * area +
					layers.Sum(x => x.LiquidAbsorbtionAmounts.Coating + x.LiquidAbsorbtionAmounts.Absorb) * area;
				if (fillOnly)
					amount = Math.Max(0, amount - ((ILocalisedSurfaceLiquidState)body.SurfaceLiquidState).ForPart(part).LiquidVolume -
						layers.Sum(x => x.SurfaceLiquidState.LiquidVolume) * area);
				if (amount <= _options.MinimumVolume) continue;
				ExposureTransport.Body(body, new LiquidMixture(liquid, amount, _world), new[] { part },
					LiquidExposureDirection.Irrelevant, remains: item);
			}
			return;
		}
		var capacity = item.LiquidAbsorbtionAmounts;
		using var refresh = new ExposureDeliveryScope(capacity.Coating + capacity.Absorb, true);
		ExposureTransport.Item(item, new LiquidMixture(liquid, Math.Max(0, capacity.Coating + capacity.Absorb - (fillOnly ? item.SurfaceLiquidState.LiquidVolume : 0)), _world), null, LiquidExposureDirection.FromOnTop);
	}

	private void AdvanceBody(IBody body, double seconds, Dictionary<ISurfaceLiquidState, List<ExposurePatch>> puddles)
	{
		if (!Physical(body.Actor)) return;
		var immersed = ImmersedParts(body);
		var pool = immersed.Length > 0 ? body.Location.Terrain(null).WaterFluid as ILiquid : null;
		var temperature = body.Location?.CurrentTemperature(null) ?? 20;
		var immersedHeld = immersed.SelectMany(body.HeldOrWieldedItemsFor).Distinct().ToArray();
		if (pool is not null)
		{
			var patches = ExposureTransport.ExternalPatches(body, immersed, ExposureRoute.LiquidContact).ToList();
			patches.AddRange(immersedHeld.Select(ItemPatch));
			Resolver.Liquid(new LiquidMixture(pool, 1, _world), true, patches, ExposureSourceKind.Immersion,
				$"pool:{body.Location!.Id}:{body.RoomLayer}", seconds, temperature);
			RefreshImmersion(body, true);
		}
		var outside = body.Bodyparts.OfType<IExternalBodypart>().Except(immersed).ToArray();
		if (body.Location is { } room && outside.Length > 0)
		{
			var patches = ExposureTransport.ExternalPatches(body, outside, ExposureRoute.GasContact).ToList();
			patches.AddRange(body.HeldOrWieldedItems.Distinct().Except(immersedHeld).Select(ItemPatch));
			Atmosphere(room, body.RoomLayer, patches, seconds, temperature);
			Heat(ExposureTransport.ExternalPatches(body, outside, ExposureRoute.AmbientHeat).Concat(body.HeldOrWieldedItems.Distinct().Except(immersedHeld).Select(ItemPatch)).ToArray(), room, seconds, temperature);
		}
		if (body.SurfaceLiquidState is ILocalisedSurfaceLiquidState local)
		{
			local.ReconcileParts();
			var parts = body.Bodyparts.OfType<IExternalBodypart>().ToDictionary(x => x.Id);
			var areas = ExposureTransport.BodyAreas(body);
			var coating = body.LiquidAbsorbtionAmounts.Coating;
			foreach (var (id, state) in local.Parts.ToArray())
			{
				var part = parts.GetValueOrDefault(id);
				if (part is null || state.IsEmpty) continue;
				// The pool supplies contact on immersed surfaces; its retained species is not a second dose.
				var area = areas.GetValueOrDefault(part);
				var patch = new ExposurePatch(body, part, body.GetMaterial(part), area, coating * area);
				Retained(state, patch, seconds, temperature, immersed.Contains(part) ? pool : null);
			}
			MudSharp.Magic.MagicalExposure.RetainedLiquidsChanged(body);
		}
		if (body.Location is Room concrete && immersed.Length == 0 && body.PositionState != PositionFlying.Instance && !body.Actor.IsProtectedFromSurfaceWater(out _))
		{
			var feet = body.Bodyparts.OfType<IExternalBodypart>().Where(x => x.Orientation == Orientation.Lowest).ToArray();
			foreach (var state in concrete.ExposureSurfaceStates(body.RoomLayer, body.Actor))
			{
				if (!puddles.TryGetValue(state, out var patches)) puddles[state] = patches = new();
				patches.AddRange(ExposureTransport.ExternalPatches(body, feet, ExposureRoute.LiquidContact));
			}
		}
	}

	private static ExposurePatch ItemPatch(IGameItem item) => ItemPatch(item, null);
	private static ExposurePatch ItemPatch(IGameItem item, IPerceivable? anchor)
	{
		var capacity = item.LiquidAbsorbtionAmounts;
		return new(item, null, item.Material, ExposureTransport.ItemArea(item), capacity.Coating + capacity.Absorb, PhysicalAnchor: anchor);
	}
	private void AdvanceItem(IGameItem item, double seconds, DateTime now)
	{
		if (!Physical(item)) return;
		var location = item.LocationLevelPerceivable?.Location;
		var temperature = location?.CurrentTemperature(null) ?? 20;
		var immersed = location?.IsSwimmingLayer(item.RoomLayer) == true && item.ContainedIn is null && item.InInventoryOf is null;
		var pool = immersed ? location!.Terrain(null).WaterFluid as ILiquid : null;
		if (item.GetItemType<ICorpse>()?.OriginalBody is { } corpseBody && corpseBody.SurfaceLiquidState is ILocalisedSurfaceLiquidState corpseState)
		{
			corpseState.ReconcileParts();
			foreach (var (id, state) in corpseState.Parts.ToArray())
			{
				var part = corpseBody.Bodyparts.OfType<IExternalBodypart>().FirstOrDefault(x => x.Id == id);
				if (part is null || state.IsEmpty) continue;
				Retained(state, new(item, part, corpseBody.GetMaterial(part), ExposureTransport.BodyArea(corpseBody, part),
					corpseBody.LiquidAbsorbtionAmountsForBodyparts(new[] { part }).Coating), seconds, temperature, pool);
			}
			DryAt(corpseState, corpseBody, now);
			MudSharp.Magic.MagicalExposure.RetainedLiquidsChanged(corpseBody);
		}
		var patch = ItemPatch(item);
		if (item.GetItemType<ICorpse>() is not null)
			Resolver.Liquid(item.SurfaceLiquidState.ContaminatingLiquid, false,
				ExposureTransport.ItemPatches(item, ExposureRoute.LiquidContact).Where(x => ReferenceEquals(x.Target, item)).ToArray(),
				ExposureSourceKind.Retained, $"surface:{item.Id}", seconds, temperature, excludedLiquid: pool);
		else Retained(item.SurfaceLiquidState, patch, seconds, temperature, pool);
		foreach (var vessel in item.GetItemTypes<ILiquidContainer>().Where(x => x.OwnsLiquidMixture).ToArray())
		{
			if (vessel.LiquidMixture?.IsEmpty != false) continue;
			Resolver.Liquid(vessel.LiquidMixture, false, new[] { patch with { Capacity = Math.Max(patch.Capacity, vessel.LiquidCapacity) } },
				ExposureSourceKind.ContainerInterior, $"vessel:{item.Id}", seconds, temperature);
			vessel.Changed = true;
		}
		if (location is not null && item.InInventoryOf is null && item.ContainedIn is null)
		{
			if (pool is not null)
			{
				Resolver.Liquid(new LiquidMixture(pool, 1, _world), true, ExposureTransport.ItemPatches(item, ExposureRoute.LiquidContact), ExposureSourceKind.Immersion,
					$"pool:{location.Id}:{item.RoomLayer}", seconds, temperature);
				RefreshImmersion(item, pool, true);
			}
			else
			{
				Atmosphere(location, item.RoomLayer, ExposureTransport.ItemPatches(item, ExposureRoute.GasContact), seconds, temperature);
				Heat(ExposureTransport.ItemPatches(item, ExposureRoute.AmbientHeat), location, seconds, temperature);
			}
		}
		Soak(item, seconds);
		MudSharp.Magic.MagicalExposure.RetainedLiquidsChanged(item);
	}

	private void Retained(ISurfaceLiquidState state, ExposurePatch patch, double seconds, double temperature, ILiquid? pool)
	{
		if (state.ContaminatingLiquid.IsEmpty) return;
		Resolver.Liquid(state.ContaminatingLiquid, false, new[] { patch }, ExposureSourceKind.Retained,
			$"surface:{patch.Target.Id}:{patch.Part?.Id}", seconds, temperature, excludedLiquid: pool);
	}

	private void Soak(IGameItem item, double seconds)
	{
		if (item.Deleted || item.InInventoryOf is not { } body || item.SurfaceLiquidState.IsEmpty) return;
		var rate = item.Material.ExposureProperties.SoakPerSecond * ExposureTransport.Transmission(item, ExposureRoute.LiquidContact);
		if (rate <= 0) return;
		var transfer = item.SurfaceLiquidState.RemoveLiquidVolume(item.SurfaceLiquidState.LiquidVolume * (1.0 - Math.Exp(-rate * seconds)));
		if (transfer is null) return;
		using var delivery = new ExposureDeliveryScope(transfer.TotalVolume, true);
		var parts = body.Bodyparts.OfType<IExternalBodypart>().Where(x => body.WornItemsFor(x).Contains(item)).ToArray();
		if (parts.Length == 0) { item.SurfaceLiquidState.AddLiquid(transfer); return; }
		ExposureTransport.PassInward(item, transfer);
	}

	private void Atmosphere(IRoom room, RoomLayer layer, IReadOnlyList<ExposurePatch> patches, double seconds, double temperature)
	{
		patches = Contents(patches, ExposureRoute.GasContact);
		if (!room.IsUnderwaterLayer(layer) && room.Atmosphere is { } gas)
			foreach (var result in Resolver.Gas(gas, patches, ExposureRoute.GasContact, ExposureSourceKind.Atmosphere, $"atmosphere:{room.Id}:{layer}", 1, seconds, temperature, dryRun: true, evaluateProgs: true))
			{
				if (result.Work > 0 && result.Reaction.DamageType == DamageType.Burning && _options.Allows(result.Patch.Target, ExposureRoute.AmbientHeat) && result.Reaction.Channel.Equals("thermal", StringComparison.OrdinalIgnoreCase))
					_atmosphericThermal[(result.Patch.Target, result.Patch.Part)] = result;
				else Resolver.Commit(result, ExposureRoute.GasContact, ExposureSourceKind.Atmosphere, $"atmosphere:{room.Id}:{layer}", seconds);
			}
		foreach (var (cloud, strength) in Clouds(room, layer))
			Resolver.Gas(cloud.Gas!, patches, ExposureRoute.GasContact, ExposureSourceKind.Cloud, $"cloud:{cloud.SourceIdentity}", strength, seconds, temperature);
	}
	private void Heat(IReadOnlyList<ExposurePatch> patches, IRoom room, double seconds, double temperature)
	{
		patches = Contents(patches, ExposureRoute.AmbientHeat);
		foreach (var patch in patches.Where(x => x.StillCurrent && x.Location == room && _options.Allows(x.Target, ExposureRoute.AmbientHeat)))
		{
			var material = patch.Material as ISolid;
			var rate = ExposureArithmetic.ThermalRate(temperature, material?.HeatDamagePoint,
				material?.ExposureProperties.ThermalSlope ?? _options.HeatSlope, material?.ExposureProperties.ThermalCap ?? _options.HeatCap);
			var amount = rate <= 0 ? 0 : rate * patch.Area * patch.Transmission * seconds * _options.Scale * Resolver.ThermalModifier(patch, $"atmosphere:{room.Id}", seconds);
			if (_atmosphericThermal.Remove((patch.Target, patch.Part), out var gasHeat))
			{
				// One physical heat source contributes each injury channel once. A pain-only gas rule
				// must not disappear merely because ambient temperature produces greater damage.
				var ambientContext = new ExposureDamageContext(ExposureRoute.AmbientHeat, ExposureSourceKind.Ambient, $"atmosphere:{room.Id}", "heat", "thermal", Guid.Empty, seconds);
				var gasContext = new ExposureDamageContext(ExposureRoute.GasContact, ExposureSourceKind.Atmosphere, $"atmosphere:{room.Id}", gasHeat.Reaction.Category, "thermal", gasHeat.Reaction.Id, seconds);
				var ambientMultiplier = EnvironmentalExposureResolver.ResistanceMultiplier(patch, ambientContext);
				var gasMultiplier = EnvironmentalExposureResolver.ResistanceMultiplier(patch, gasContext);
				Resolver.Commit(gasHeat with { RawDamage = Math.Max(amount * ambientMultiplier, gasHeat.RawDamage * gasMultiplier),
					Pain = Math.Max(amount * ambientMultiplier, gasHeat.Pain * gasMultiplier), Stun = gasHeat.Stun * gasMultiplier },
					ExposureRoute.GasContact, ExposureSourceKind.Atmosphere, $"atmosphere:{room.Id}:{patch.Target.RoomLayer}", seconds, resistanceApplied: true);
				continue;
			}
			EnvironmentalExposureResolver.ApplyDamage(patch, new(ExposureRoute.AmbientHeat, ExposureSourceKind.Ambient, $"atmosphere:{room.Id}", "heat", "thermal", Guid.Empty, seconds), DamageType.Burning, amount, amount, 0);
		}
	}

	private static IReadOnlyList<ExposurePatch> Contents(IEnumerable<ExposurePatch> patches, ExposureRoute route)
	{
		var result = new List<ExposurePatch>();
		var visited = new HashSet<(IPerceivable, IBodypart?)>(PatchIdentity.Instance);
		void Visit(ExposurePatch patch)
		{
			if (!visited.Add((patch.Target, patch.Part))) return;
			result.Add(patch);
			if (patch.Target is not IGameItem item || item.Deleted) return;
			var factor = item.GetItemType<IOpenable>()?.IsOpen != false ? 1.0 : ExposureTransport.Transmission(item, route);
			foreach (var child in item.GetItemTypes<IContainer>().SelectMany(x => x.Contents).Distinct().ToArray())
				foreach (var inner in ExposureTransport.ItemPatches(child, route))
					Visit(new ExposurePatch(inner.Target, inner.Part, inner.Material, inner.Area, inner.Capacity,
						inner.Transmission * patch.Transmission * factor, inner.LayerDepth, inner.PhysicalAnchor ?? patch.PhysicalAnchor));
		}
		foreach (var patch in patches) Visit(patch);
		return result;
	}

	public bool LastBreathWasSupplied(IBody body) => _suppliedBreaths.GetValueOrDefault(body);
	private IEnumerable<(TrapGasCloudEffect Cloud, double Strength)> Clouds(IRoom room, RoomLayer layer)
	{
		var clouds = room.EffectsOfType<TrapGasCloudEffect>().Where(x => x.Layer == layer && x.Gas is not null && x.ContactStrength > 0)
			.DistinctBy(x => x.SourceIdentity).ToArray();
		var total = clouds.Sum(x => x.ContactStrength);
		var scale = total > 0 ? Math.Min(1, _options.CloudStrengthCap / total) : 0;
		return clouds.Select(x => (x, x.ContactStrength * scale));
	}
	public bool LastBreathWasSuppressed(IBody body) => _airflowBreaths.TryGetValue(body, out var airflow) && !airflow;
	public void NoInhalation(IBody body) { _breaths[body] = Clock(); _suppliedBreaths[body] = false; _airflowBreaths[body] = false; }
	public void Inhaled(IBody body, IFluid fluid, bool supplied = false)
	{
		var now = Clock();
		var seconds = _breaths.TryGetValue(body, out var last) ? Math.Clamp((now - last).TotalSeconds, 0, 10) : 0;
		_breaths[body] = now; _suppliedBreaths[body] = supplied;
		_airflowBreaths[body] = true;
		if (body.Location is not { } location) return;
		ResolveRespiratorySample(new(body, fluid, $"breath:{body.Id}:{fluid.Id}", location, body.RoomLayer, 1, seconds, true, supplied, true));
	}
	public static IBodypart[] RespiratoryParts(IBody body) => body.Bodyparts.Concat(body.Organs).Where(x => body.BreathingStrategy.Name switch
		{
			"gills" => x.BodypartType == BodypartTypeEnum.Gill,
			"blowhole" => x.BodypartType is BodypartTypeEnum.Lung or BodypartTypeEnum.Trachea or BodypartTypeEnum.Blowhole,
			"simple" => x.BodypartType is BodypartTypeEnum.Lung or BodypartTypeEnum.Trachea,
			"partless" => x is IExternalBodypart,
			_ => false
		}).Distinct().ToArray();
	public void ResolveRespiratorySample(RespiratoryExposureSample sample)
	{
		var body = sample.Body; var fluid = sample.Fluid; var seconds = sample.Seconds;
		if (!sample.Airflow || !sample.WithdrawalSucceeded || !ExposureArithmetic.Valid(seconds) || seconds <= 0 ||
			!ExposureArithmetic.Valid(sample.Strength) || sample.Strength <= 0 || !Physical(body.Actor) ||
			body.Location != sample.Location || body.RoomLayer != sample.Layer) return;
		var parts = RespiratoryParts(body);
		if (parts.Length == 0) return;
		// Partless respiration is a whole-body abstract injury, distributed over existing external
		// parts by mass/area; it neither invents organs nor selects an unrelated internal organ.
		var patches = parts.Select(p => new ExposurePatch(body, p, body.GetMaterial(p),
			body.BreathingStrategy.Name == "partless" ? ExposureTransport.BodyArea(body, (IExternalBodypart)p) : 1.0 / parts.Length, 1,
			fluid is ILiquid ? sample.Strength : 1)).ToArray();
		if (fluid is ILiquid liquid)
			Resolver.Liquid(new LiquidMixture(liquid, Math.Max(body.Race.BreathingRate(body, fluid), 1e-9), _world), true, patches, ExposureSourceKind.Breath, sample.SourceIdentity, seconds, sample.Location.CurrentTemperature(null), ExposureRoute.Inhalation);
		else Resolver.Gas(fluid, patches, ExposureRoute.Inhalation, ExposureSourceKind.Breath, sample.SourceIdentity, sample.Strength, seconds, sample.Location.CurrentTemperature(null));
		if (!sample.Supplied)
			foreach (var (cloud, strength) in Clouds(body.Location, body.RoomLayer))
				Resolver.Gas(cloud.Gas!, patches, ExposureRoute.Inhalation, ExposureSourceKind.Cloud, $"cloud:{cloud.SourceIdentity}", strength, seconds, body.Location.CurrentTemperature(null));
	}
}
