#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Planes;

namespace MudSharp.Climate;

internal static class WeatherResidue
{
	internal static void Deposit(IRoom room, WeatherHazardSettings settings)
	{
		if (settings.ResidueMaterialId <= 0) return;
		var material = room.Gameworld.Materials.Get(settings.ResidueMaterialId);
		if (material is null || !double.IsFinite(settings.ResiduePerMinute) || settings.ResiduePerMinute <= 0 ||
			!double.IsFinite(settings.ResidueMaximum) || settings.ResidueMaximum <= 0) return;
		void Coat(ISurfaceContaminable owner)
		{
			var weight = owner.SurfaceLiquidState.Residues.Where(x => x.Material == material).Sum(x => x.Weight);
			var amount = Math.Min(settings.ResiduePerMinute, settings.ResidueMaximum - weight);
			if (amount > 0) owner.SurfaceLiquidState.DepositResidue(material, amount);
		}
		foreach (var actor in room.Characters.ToArray())
		{
			if (!ReferenceEquals(actor.Location, room) || actor.Body is null || actor.GetPlanarPresence().SuspendsPhysicalContact ||
				!WeatherHazardService.Exposed(room, actor)) continue;
			// Native residue renders on the body and clothes, and native cleaning removes it.
			Coat(actor.Body);
			foreach (var item in actor.Body.WornItems.Concat(actor.Body.HeldItems).Concat(actor.Body.WieldedItems).Distinct().ToArray())
				if (ReferenceEquals(item.InInventoryOf, actor.Body)) Coat(item);
		}
		foreach (var item in room.GameItems.ToArray())
			if (ReferenceEquals(item.Location, room) && WeatherHazardService.Exposed(room, item)) Coat(item);
	}
}
