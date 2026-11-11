#nullable enable
using MudSharp.Community;
using MudSharp.Framework.Save;

namespace MudSharp.Economy.Estates;

public partial class Estate
{
	internal Action CaptureLendingRollback()
	{
		var oldAssets = _assets.ToList();
		var oldClaims = _claims.ToList();
		var oldPayouts = _payouts.ToList();
		var oldEstateStatus = EstateStatus;
		var oldFinalisationDate = FinalisationDate;
		var estateChanged = Changed;
		var assetStates = oldAssets.OfType<EstateAsset>()
			.ToDictionary(x => x, x => (x.IsTransferred, x.IsLiquidated, x.LiquidatedValue, x.OwnershipShare, x.Changed));
		var claimStates = oldClaims.OfType<EstateClaim>()
			.ToDictionary(x => x, x => (x.Amount, x.Reason, x.Status, x.StatusReason, x.Changed));
		var payoutStates = oldPayouts.OfType<EstatePayout>()
			.ToDictionary(x => x, x => (x.CollectedDate, x.Changed));

		return () =>
		{
			// SQL rollback discards new rows. Remove their runtime save-queue entries.
			foreach (var item in _assets.Cast<object>().Except(oldAssets).Concat(_claims.Cast<object>().Except(oldClaims)).Concat(_payouts.Cast<object>().Except(oldPayouts)))
			{
				if (item is SaveableItem saveable)
				{
					Gameworld.SaveManager.Abort(saveable);
					saveable.Changed = false;
				}
			}

			_assets.Clear(); _assets.AddRange(oldAssets);
			_claims.Clear(); _claims.AddRange(oldClaims);
			_payouts.Clear(); _payouts.AddRange(oldPayouts);

			foreach (var (item, state) in assetStates)
			{
				Gameworld.SaveManager.Abort(item);
				item.Changed = false;
				item.IsTransferred = state.IsTransferred;
				item.IsLiquidated = state.IsLiquidated;
				item.LiquidatedValue = state.LiquidatedValue;
				item.OwnershipShare = state.OwnershipShare;
				item.Changed = false;
				if (state.Changed) item.Changed = true;
			}
			foreach (var (item, state) in claimStates)
			{
				Gameworld.SaveManager.Abort(item);
				item.Changed = false;
				item.Amount = state.Amount;
				item.Reason = state.Reason;
				item.Status = state.Status;
				item.StatusReason = state.StatusReason;
				item.Changed = false;
				if (state.Changed) item.Changed = true;
			}
			foreach (var (item, state) in payoutStates)
			{
				Gameworld.SaveManager.Abort(item);
				item.Changed = false;
				item.CollectedDate = state.CollectedDate;
				item.Changed = false;
				if (state.Changed) item.Changed = true;
			}

			Gameworld.SaveManager.Abort(this);
			Changed = false;
			EstateStatus = oldEstateStatus;
			FinalisationDate = oldFinalisationDate;
			if (estateChanged) Changed = true;
		};
	}

	internal Action CaptureAuctionCompletionRollback(AuctionItem auctionItem)
	{
		var asset = FindAsset(auctionItem.Asset) as EstateAsset;
		if (asset == null) return static () => { };
		var wasLiquidated = asset.IsLiquidated;
		var liquidatedValue = asset.LiquidatedValue;
		var wasChanged = asset.Changed;
		return () =>
		{
			Gameworld.SaveManager.Abort(asset);
			asset.Changed = false;
			asset.IsLiquidated = wasLiquidated;
			asset.LiquidatedValue = liquidatedValue;
			asset.Changed = false;
			if (wasChanged) asset.Changed = true;
		};
	}
}
