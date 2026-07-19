using MudSharp.Form.Material;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Effects.Concrete;

public sealed class SurfaceContaminationEffect : Effect, ICleanableEffect, IIngredientTransferEffect
{
	private ISurfaceContaminable SurfaceOwner => (ISurfaceContaminable)Owner;

	public SurfaceContaminationEffect(ISurfaceContaminable owner) : base(owner)
	{
	}

	protected override string SpecificEffectType => "SurfaceContamination";

	public override string Describe(IPerceiver voyeur)
	{
		var state = SurfaceOwner.SurfaceLiquidState;
		return $"Surface contamination with {Gameworld.UnitManager.Describe(state.LiquidVolume, Framework.Units.UnitType.FluidVolume, voyeur)} liquid and {Gameworld.UnitManager.Describe(state.ResidueWeight, Framework.Units.UnitType.Mass, voyeur)} residue";
	}

	public override bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return !SurfaceOwner.SurfaceLiquidState.IsEmpty;
	}

	public TimeSpan BaseCleanTime => TimeSpan.FromSeconds(Gameworld.GetStaticDouble("BaseCleanTime"));

	public ITag CleaningToolTag
	{
		get
		{
			var tagId = SurfaceOwner.SurfaceLiquidState.IsWet
				? Gameworld.GetStaticLong("LiquidCleaningToolTag")
				: Gameworld.GetStaticLong("ResidueCleaningToolTag");
			return Gameworld.Tags.Get(tagId)!;
		}
	}

	public ILiquid? LiquidRequired => SurfaceOwner.SurfaceLiquidState.LiquidRequired;

	public double LiquidAmountConsumed
	{
		get => SurfaceOwner.SurfaceLiquidState.LiquidAmountConsumed;
		set
		{
			// Cleaning state is driven by CleanWithLiquid so that the actual solvent amount controls removal.
		}
	}

	public string EmoteBeginClean => Gameworld.GetStaticString("EmoteBeginClean");
	public string EmoteStopClean => Gameworld.GetStaticString("EmoteStopClean");
	public string EmoteFinishClean => Gameworld.GetStaticString("EmoteFinishClean");
	public string PromptStatusLine => Gameworld.GetStaticString("CleanPromptStatusLine");

	public bool CleanWithLiquid(LiquidMixture? liquid, double amount)
	{
		var cleaned = SurfaceOwner.SurfaceLiquidState.CleanWithLiquid(liquid, amount);
		if (SurfaceOwner.SurfaceLiquidState.IsEmpty)
		{
			Owner.RemoveEffect(this, true);
		}

		return cleaned;
	}

	public void TransferToFood(IPreparedFood food, double proportion)
	{
		if (proportion <= 0.0)
		{
			return;
		}

		var state = SurfaceOwner.SurfaceLiquidState;
		if (!state.ContaminatingLiquid.IsEmpty)
		{
			food.AbsorbLiquid(state.ContaminatingLiquid.Clone(state.ContaminatingLiquid.TotalVolume * proportion), "transferred surface liquid");
		}

		foreach (var residue in state.Residues)
		{
			if (residue.Weight <= 0.0)
			{
				continue;
			}

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
