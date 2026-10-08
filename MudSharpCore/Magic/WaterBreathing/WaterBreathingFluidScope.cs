#nullable enable

using System.Collections.ObjectModel;
using MudSharp.Form.Material;

namespace MudSharp.Magic.WaterBreathing;

/// <summary>Explicit authored water mappings; no automatic all-liquid, gas or CountsAs grant.</summary>
internal sealed class WaterBreathingFluidScope
{
	private readonly ILiquid[] _liquids;
	private readonly long[] _ids;
	private readonly ReadOnlyCollection<long> _readOnlyIds;

	public WaterBreathingFluidScope(IEnumerable<ILiquid> selectedWaterLiquids)
	{
		ArgumentNullException.ThrowIfNull(selectedWaterLiquids);
		_liquids = selectedWaterLiquids.ToArray();
		if (_liquids.Length == 0 || _liquids.Any(x => x is null || x.Id <= 0))
			throw new ArgumentException("Select at least one existing native water liquid.", nameof(selectedWaterLiquids));
		_ids = _liquids.Select(x => x.Id).ToArray();
		if (_ids.Distinct().Count() != _ids.Length)
			throw new ArgumentException("Water liquid mappings must have distinct native identities.", nameof(selectedWaterLiquids));
		_readOnlyIds = Array.AsReadOnly(_ids);
	}

	public IReadOnlyList<long> LiquidIds => _readOnlyIds;

	public bool Allows(IFluid? fluid)
	{
		if (fluid is not ILiquid) return false;
		for (var index = 0; index < _liquids.Length; index++)
			if (ReferenceEquals(fluid, _liquids[index]) && fluid.Id == _ids[index]) return true;
		return false;
	}

	/// <summary>Catalogue replacement requires fresh admission even when numeric IDs are unchanged.</summary>
	public bool MatchesConfiguredLiquids(IEnumerable<ILiquid> liquids) =>
		_liquids.SequenceEqual(liquids, ReferenceEqualityComparer.Instance) &&
		_liquids.Select(x => x.Id).SequenceEqual(_ids);
}
