using MudSharp.Body;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Form.Material;

/// <summary>Part states own quantities. The aggregate is a read-only view of the original instances.</summary>
public sealed class BodySurfaceLiquidState : ILocalisedSurfaceLiquidState
{
	private readonly IFuturemud _world;
	private readonly Func<IEnumerable<IExternalBodypart>> _parts;
	private readonly Action _changed;
	private readonly Action? _beforeMutation;
	private readonly SortedDictionary<long, SurfaceLiquidState> _states = new();
	private bool _reconciling;

	public BodySurfaceLiquidState(IFuturemud world, Func<IEnumerable<IExternalBodypart>> parts, Action changed, XElement? root = null, Action? beforeMutation = null)
	{
		_world = world; _parts = parts; _changed = changed; _beforeMutation = beforeMutation;
		if (root?.Attribute("Version")?.Value == "2")
		{
			foreach (var part in root.Elements("Part"))
				if (long.TryParse((string?)part.Attribute("Id"), out var id) && !_states.ContainsKey(id))
					_states[id] = new SurfaceLiquidState(world, part.Element("Surface"), changed, beforeMutation);
		}
		else if (root is not null)
		{
			// A zero-id state also preserves liquid when temporarily no external anatomy exists.
			_states[0] = new SurfaceLiquidState(world, root, changed, beforeMutation);
		}
		ReconcileParts();
	}

	public IEnumerable<(long PartId, ISurfaceLiquidState State)> Parts => _states.Select(x => (x.Key, (ISurfaceLiquidState)x.Value));
	public ISurfaceLiquidState ForPart(IExternalBodypart part)
	{
		if (!_states.TryGetValue(part.Id, out var state)) _states[part.Id] = state = new SurfaceLiquidState(_world, _changed, _beforeMutation);
		return state;
	}

	public void ReconcileParts()
	{
		if (_reconciling) return;
		_reconciling = true;
		try
		{
			var parts = _parts().DistinctBy(x => x.Id).OrderBy(x => x.Id).ToArray();
			if (parts.Length == 0) return;
			var ids = parts.Select(x => x.Id).ToHashSet();
			var orphans = _states.Where(x => !ids.Contains(x.Key)).ToArray();
			foreach (var orphan in orphans)
			{
				_states.Remove(orphan.Key);
				Distribute(orphan.Value, parts);
			}
		}
		finally { _reconciling = false; }
	}

	private void Distribute(ISurfaceLiquidState source, IExternalBodypart[] parts)
	{
		var weights = parts.Select(x => Math.Max(0.0, x.RelativeHitChance)).ToArray();
		var sum = weights.Sum();
		for (var i = 0; i < parts.Length; i++)
		{
			var fraction = sum > 0.0 ? weights[i] / sum : 1.0 / parts.Length;
			var state = (SurfaceLiquidState)ForPart(parts[i]);
			var existingClock = state.IsEmpty ? source.LastResolvedUtc : state.LastResolvedUtc;
			if (!source.ContaminatingLiquid.IsEmpty) state.AddLiquid(source.ContaminatingLiquid.Clone(source.LiquidVolume * fraction));
			foreach (var residue in source.Residues) state.AddResidue(residue.Material, residue.OriginalLiquid, residue.Weight * fraction);
			state.LastResolvedUtc = source.LastResolvedUtc > existingClock ? source.LastResolvedUtc : existingClock;
		}
	}

	public LiquidMixture ContaminatingLiquid => LiquidMixture.CreateReadOnlyView(_states.Values.SelectMany(x => x.ContaminatingLiquid.Instances), _world);
	public IEnumerable<ISurfaceResidue> Residues => _states.Values.SelectMany(x => x.Residues);
	public double LiquidVolume => _states.Values.Sum(x => x.LiquidVolume);
	public double ResidueWeight => _states.Values.Sum(x => x.ResidueWeight);
	public double AddedWeight => _states.Values.Sum(x => x.AddedWeight);
	public bool IsEmpty => _states.Values.All(x => x.IsEmpty);
	public bool IsWet => _states.Values.Any(x => x.IsWet);
	public DateTime LastResolvedUtc
	{
		get => _states.Count == 0 ? DateTime.UtcNow : _states.Values.Min(x => x.LastResolvedUtc);
		set { foreach (var state in _states.Values) state.LastResolvedUtc = value; }
	}
	public ILiquid? LiquidRequired => _states.Values.Select(x => x.LiquidRequired).FirstOrDefault(x => x is not null);
	public double LiquidAmountConsumed => _states.Values.Sum(x => x.LiquidAmountConsumed);

	public void AddLiquid(LiquidMixture liquid)
	{
		var parts = _parts().OrderBy(x => x.Id).ToArray();
		if (parts.Length == 0)
		{
			if (!_states.TryGetValue(0, out var state)) _states[0] = state = new SurfaceLiquidState(_world, _changed, _beforeMutation);
			state.AddLiquid(liquid);
			return;
		}
		var source = new SurfaceLiquidState(_world);
		source.AddLiquid(liquid);
		Distribute(source, parts);
	}

	public bool TryAddDriedLiquid(LiquidMixture liquid, bool roomSurface = false)
	{
		var source = new SurfaceLiquidState(_world);
		if (!source.TryAddDriedLiquid(liquid, roomSurface)) return false;
		var parts = _parts().OrderBy(x => x.Id).ToArray();
		if (parts.Length == 0) { _states.TryAdd(0, new SurfaceLiquidState(_world, _changed, _beforeMutation)); foreach (var r in source.Residues) _states[0].AddResidue(r.Material, r.OriginalLiquid, r.Weight); }
		else Distribute(source, parts);
		_changed();
		return true;
	}

	public LiquidMixture? RemoveLiquidVolume(double volume)
	{
		var total = LiquidVolume;
		if (!ExposureArithmetic.Valid(volume) || total <= 0 || volume <= 0) return null;
		var result = LiquidMixture.CreateEmpty(_world);
		foreach (var state in _states.Values.ToArray())
			if (state.RemoveLiquidVolume(Math.Min(volume, total) * state.LiquidVolume / total) is { } portion) result.AddLiquid(portion);
		return result;
	}
	public bool CleanWithLiquid(LiquidMixture? liquid, double amount)
	{
		var weights = _states.Values.Select(x => (State: x, Weight: x.LiquidAmountConsumed)).ToArray();
		var total = weights.Sum(x => x.Weight);
		foreach (var (state, weight) in weights) if (total > 0) state.CleanWithLiquid(liquid, amount * weight / total);
		return IsEmpty;
	}
	public void Dry(double amount, bool roomSurface = false)
	{
		var total = LiquidVolume;
		foreach (var state in _states.Values.ToArray()) if (total > 0) state.Dry(amount * state.LiquidVolume / total, roomSurface);
	}
	public bool ResolveDrying(TimeSpan interval, double minimumDryVolume, double dryFraction, bool roomSurface = false, int maxTicks = 24, DateTime? utcNow = null)
	{
		var total = LiquidVolume;
		var changed = false;
		foreach (var state in _states.Values.ToArray()) if (total > 0)
			changed |= state.ResolveDrying(interval, minimumDryVolume * state.LiquidVolume / total, dryFraction, roomSurface, maxTicks, utcNow);
		return changed;
	}
	private SurfaceLiquidState DescriptionState()
	{
		var result = new SurfaceLiquidState(_world);
		result.AddLiquid(ContaminatingLiquid);
		foreach (var residue in Residues) result.AddResidue(residue.Material, residue.OriginalLiquid, residue.Weight);
		return result;
	}
	public ItemSaturationLevel SaturationLevel(double coating, double absorb) => SaturationLevelForLiquid(LiquidVolume, coating, absorb);
	public ItemSaturationLevel SaturationLevelForLiquid(double total, double coating, double absorb) => new SurfaceLiquidState(_world).SaturationLevelForLiquid(total, coating, absorb);
	public string GetAddendumText(double coating, double absorb, bool colour) => DescriptionState().GetAddendumText(coating, absorb, colour);
	public string GetAdditionalText(double coating, double absorb, IPerceiver voyeur, bool colour) => DescriptionState().GetAdditionalText(coating, absorb, voyeur, colour);
	public XElement SaveToXml() => new("Surface", new XAttribute("Version", 2), _states.Where(x => !x.Value.IsEmpty)
		.Select(x => new XElement("Part", new XAttribute("Id", x.Key), x.Value.SaveToXml())));
}
