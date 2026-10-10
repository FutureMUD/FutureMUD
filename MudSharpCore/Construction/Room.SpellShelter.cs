#nullable enable

using System.Globalization;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.GameItems;

namespace MudSharp.Construction;

public partial class Room
{
	internal void ReconcileShelterItemMembership(IGameItem item)
	{
		var canonical = !item.Deleted && item.InInventoryOf is null && item.ContainedIn is null ? item.Location : null;
		SetPreparedCurrencyMembership(item, ReferenceEquals(canonical, this));
		foreach (var parent in new[] { OwningZone as Location, OwningZone?.Shard as Location }.OfType<Location>().Distinct())
			parent.SetPreparedCurrencyMembership(item, canonical?.GameItems.Any(x => ReferenceEquals(x, item)) == true &&
				parent.Rooms.Any(x => ReferenceEquals(x, canonical)));
		ContentsChanged = true;
	}

	/// <summary>Stage floor material with topology deletion; publish the same committed state without replaying a spill.</summary>
	internal Action StageShelterSurfaceTransfer(FuturemudDatabaseContext context, SpatialLocation destination,
		IEnumerable<ISurfaceLiquidState>? additionalSources = null)
	{
		if (destination.Room is not Room target) throw new InvalidOperationException("Surface conservation requires a native return room.");
		var states = target._surfaceLiquidStates.ToDictionary(x => x.Key,
			x => new SurfaceLiquidState(Gameworld, x.Value.SaveToXml()));
		var key = new SurfaceLiquidLocation(destination.Layer, target.NormaliseSurfaceCoordinate(destination.RoutePositionMetres));
		if (!states.TryGetValue(key, out var combined)) states[key] = combined = new SurfaceLiquidState(Gameworld);
		foreach (var source in _surfaceLiquidStates.Values.Cast<ISurfaceLiquidState>()
			.Concat(additionalSources ?? []).Where(x => !x.IsEmpty))
			ConserveShelterSurfaceMaterial(combined, source);
		var xml = states.Any(x => !x.Value.IsEmpty) ? new XElement("Surfaces", states.Where(x => !x.Value.IsEmpty)
			.Select(x => new XElement("Layer", new XAttribute("id", (int)x.Key.Layer),
				x.Key.RoutePositionMetres is { } point ? new XAttribute("position", point.ToString("R", CultureInfo.InvariantCulture)) : null,
				x.Value.SaveToXml()))).ToString() : null;
		context.Rooms.Find(target.Id)!.SurfaceLiquidData = xml;
		return () =>
		{
			target.LoadSurfaceLiquidState(xml);
			_surfaceLiquidStates.Clear(); _surfaceLiquidChanged = false;
			EnvironmentalExposureService.For(Gameworld).RefreshRoom(target);
		};
	}

	internal static void ConserveShelterSurfaceMaterial(SurfaceLiquidState destination, ISurfaceLiquidState source)
	{
		destination.AddLiquid(source.ContaminatingLiquid.Clone());
		foreach (var residue in source.Residues) destination.AddResidue(residue.Material, residue.OriginalLiquid, residue.Weight);
	}

	internal void FinishCommittedShelterRemoval()
	{
		if (Characters.Any() || GameItems.Any(x => !x.Deleted))
			throw new InvalidOperationException("Committed shelter release still has runtime occupants or goods.");
		SetNoSave(true);
		InvalidatePositionTargets();
		foreach (var overlay in _overlays)
		{
			if (overlay is RoomOverlay native) native.SetNoSave(true);
			Gameworld.SaveManager.Abort(overlay);
		}
		Gameworld.SaveManager.Abort(this);
		Gameworld.EffectScheduler.Destroy(this); Gameworld.Scheduler.Destroy(this);
		EffectHandler.RemoveAllEffects();
		ReleaseEvents();
		Dispose();
	}
}
