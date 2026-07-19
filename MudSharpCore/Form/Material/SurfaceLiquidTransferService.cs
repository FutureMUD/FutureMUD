#nullable enable

using MudSharp.GameItems;

namespace MudSharp.Form.Material;

internal enum SurfaceLiquidRoute
{
	Ignore,
	Discard,
	Surface
}

internal static class SurfaceLiquidTransferService
{
	public static SurfaceLiquidRoute SelectRoute(bool mixtureIsEmpty, bool puddlesEnabled, bool isSwimmingLayer)
	{
		return SelectRoute(mixtureIsEmpty, () => puddlesEnabled, isSwimmingLayer);
	}

	public static SurfaceLiquidRoute SelectRoute(bool mixtureIsEmpty, Func<bool> puddlesEnabled,
		bool isSwimmingLayer)
	{
		if (mixtureIsEmpty || isSwimmingLayer)
		{
			return SurfaceLiquidRoute.Ignore;
		}

		return puddlesEnabled() ? SurfaceLiquidRoute.Surface : SurfaceLiquidRoute.Discard;
	}

	public static void TransferToSurface(ISurfaceLiquidState state, LiquidMixture mixture, double maximumVolume)
	{
		state.AddLiquid(mixture);
		if (state.LiquidVolume > maximumVolume)
		{
			state.RemoveLiquidVolume(state.LiquidVolume - maximumVolume);
		}

		mixture.SetLiquidVolume(0.0);
	}

	public static void DryAfterMachineCycle(ISurfaceLiquidState state, double minimumDryVolume)
	{
		state.Dry(Math.Max(minimumDryVolume, state.LiquidVolume * 0.30));
	}

	public static void TransferToFood(ISurfaceLiquidState state, IPreparedFood food, double proportion)
	{
		if (proportion <= 0.0)
		{
			return;
		}

		if (!state.ContaminatingLiquid.IsEmpty)
		{
			food.AbsorbLiquid(
				state.ContaminatingLiquid.Clone(state.ContaminatingLiquid.TotalVolume * proportion),
				"transferred surface liquid");
		}

		foreach (var residue in state.Residues.Where(x => x.Weight > 0.0))
		{
			food.AddIngredient(new FoodIngredientInstance
			{
				Role = "residue",
				Description = residue.Material.MaterialDescription,
				TasteText = residue.Material.MaterialDescription,
				MaterialId = residue.Material.Id,
				Weight = residue.Weight * proportion,
				Quality = ItemQuality.Standard
			});
		}
	}
}
