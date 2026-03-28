using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Estates;

public class Estate : SaveableItem, IEstate, ILazyLoadDuringIdleTime
{
	public static IEnumerable<IEstate> CreateEstatesForCharacterDeath(ICharacter character)
	{
		var estates = new Dictionary<long, Estate>();

		Estate GetEstate(IEconomicZone zone)
		{
			if (!estates.TryGetValue(zone.Id, out var estate))
			{
				estate = new Estate(zone, character, character.EstateHeir);
				estates[zone.Id] = estate;
			}

			return estate;
		}

		foreach (var property in character.Gameworld.Properties.Where(x => x.PropertyOwners.Any(y => y.Owner == character)))
		{
			GetEstate(property.EconomicZone).AddAsset(new EstateAsset(GetEstate(property.EconomicZone), property, false));
		}

		foreach (var account in character.Gameworld.BankAccounts.Where(x => x.IsAccountOwner(character)))
		{
			var estate = GetEstate(account.Bank.EconomicZone);
			estate.AddAsset(new EstateAsset(estate, account, false));
			var securedAmount = Math.Max(0.0M, account.CurrentMonthFees + Math.Max(0.0M, account.CurrentBalance * -1.0M));
			if (securedAmount > 0.0M)
			{
				estate.AddClaim(new EstateClaim(estate, account.Bank, securedAmount,
					$"Secured bank debt for account {account.AccountReference}", ClaimStatus.NotAssessed, true, account));
			}
		}

		foreach (var loc in character.Gameworld.LineOfCreditAccounts.Where(x => x.IsAccountOwner(character)))
		{
			var zone = character.Location == null ? null : DetermineZone(character.Gameworld, character.Location);
			if (zone == null)
			{
				continue;
			}

			var estate = GetEstate(zone);
			if (loc.OutstandingBalance > 0.0M)
			{
				estate.AddClaim(new EstateClaim(estate, loc, loc.OutstandingBalance,
					$"Outstanding line of credit debt for {loc.AccountName}", ClaimStatus.NotAssessed, true));
			}
		}

		var itemZone = character.Location == null ? null : DetermineZone(character.Gameworld, character.Location);
		if (itemZone != null)
		{
			var estate = GetEstate(itemZone);
			foreach (var item in character.Body.AllItems.Distinct())
			{
				if (item.HasOwner && item.Owner != character)
				{
					continue;
				}

				estate.AddAsset(new EstateAsset(estate, item, !item.HasOwner));
			}
		}

		return estates.Values;
	}

	private static IEconomicZone DetermineZone(IFuturemud gameworld, ICell cell)
	{
		return gameworld.Properties.FirstOrDefault(x => x.PropertyLocations.Contains(cell))?.EconomicZone ??
		       gameworld.Banks.FirstOrDefault(x => x.BranchLocations.Contains(cell))?.EconomicZone ??
		       gameworld.AuctionHouses.FirstOrDefault(x => x.AuctionHouseCell == cell)?.EconomicZone ??
		       gameworld.EconomicZones.FirstOrDefault(x => x.ZoneForTimePurposes == cell.Zone);
	}

	public Estate(MudSharp.Models.Estate estate, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = estate.Id;
		EconomicZone = gameworld.EconomicZones.Get(estate.EconomicZoneId);
		_characterId = estate.CharacterId;
		EstateStatus = (EstateStatus)estate.EstateStatus;
		EstateStartTime = new MudDateTime(estate.EstateStartTime, gameworld);
		FinalisationDate = string.IsNullOrWhiteSpace(estate.FinalisationDate)
			? null
			: new MudDateTime(estate.FinalisationDate, gameworld);
		if (estate.InheritorId.HasValue && !string.IsNullOrWhiteSpace(estate.InheritorType))
		{
			_inheritorReference = new FrameworkItemReference(estate.InheritorId.Value, estate.InheritorType, gameworld);
		}

		foreach (var claim in estate.EstateClaims)
		{
			_claims.Add(new EstateClaim(claim, this));
		}

		foreach (var asset in estate.EstateAssets)
		{
			_assets.Add(new EstateAsset(asset, this));
		}

		Gameworld.Add(this);
		EconomicZone.AddEstate(this);
		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public Estate(IEconomicZone economicZone, ICharacter character, IFrameworkItem inheritor)
	{
		Gameworld = economicZone.Gameworld;
		EconomicZone = economicZone;
		_character = character;
		_characterId = character.Id;
		EstateStatus = EstateStatus.Undiscovered;
		EstateStartTime = economicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		SetInheritor(inheritor);
		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.Estate
			{
				EconomicZoneId = economicZone.Id,
				CharacterId = character.Id,
				EstateStatus = (int)EstateStatus,
				EstateStartTime = EstateStartTime.GetDateTimeString(),
				FinalisationDate = null,
				InheritorId = _inheritorReference?.Id,
				InheritorType = _inheritorReference?.FrameworkItemType
			};
			FMDB.Context.Estates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		Gameworld.Add(this);
		EconomicZone.AddEstate(this);
	}

	public override string FrameworkItemType => "Estate";

	public override void Save()
	{
		var dbitem = FMDB.Context.Estates.Find(Id);
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.CharacterId = Character.Id;
		dbitem.EstateStatus = (int)EstateStatus;
		dbitem.EstateStartTime = EstateStartTime.GetDateTimeString();
		dbitem.FinalisationDate = FinalisationDate?.GetDateTimeString();
		dbitem.InheritorId = _inheritorReference?.Id;
		dbitem.InheritorType = _inheritorReference?.FrameworkItemType;
		Changed = false;
	}

	public void DoLoad()
	{
		_ = Character;
		_ = Inheritor;
	}

	private long _characterId;
	private ICharacter _character;
	private FrameworkItemReference _inheritorReference;
	private IFrameworkItem _inheritor;

	public IEconomicZone EconomicZone { get; }
	public ICharacter Character => _character ??= Gameworld.TryGetCharacter(_characterId, true);
	public IFrameworkItem Inheritor
	{
		get
		{
			if (_inheritor == null && _inheritorReference != null)
			{
				_inheritor = _inheritorReference.GetItem;
			}

			return _inheritor;
		}
	}

	public EstateStatus EstateStatus { get; set; }
	public MudDateTime EstateStartTime { get; private set; }
	public MudDateTime FinalisationDate { get; set; }

	private readonly List<IEstateClaim> _claims = new();
	public IEnumerable<IEstateClaim> Claims => _claims;

	private readonly List<IEstateAsset> _assets = new();
	public IEnumerable<IEstateAsset> Assets => _assets;

	public void AddClaim(IEstateClaim claim)
	{
		_claims.Add(claim);
		Changed = true;
	}

	public void RemoveClaim(IEstateClaim claim)
	{
		_claims.Remove(claim);
		Changed = true;
	}

	public void UpdateClaim(IEstateClaim claim)
	{
		if (_claims.All(x => x.Id != claim.Id))
		{
			_claims.Add(claim);
		}

		Changed = true;
	}

	public void AddAsset(IEstateAsset asset)
	{
		if (_assets.All(x => x.Id != asset.Id))
		{
			_assets.Add(asset);
		}

		Changed = true;
	}

	public void RemoveAsset(IEstateAsset asset)
	{
		_assets.Remove(asset);
		Changed = true;
	}

	public bool CheckStatus()
	{
		if (EstateStatus == EstateStatus.Finalised || EstateStatus == EstateStatus.Cancelled)
		{
			return false;
		}

		if (EstateStatus == EstateStatus.ClaimPhase)
		{
			if (EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime >= FinalisationDate)
			{
				if (Claims.Any(x => x.Status == ClaimStatus.Approved))
				{
					StartLiquidation();
				}
				else
				{
					Finalise();
				}
				return true;
			}

			return false;
		}

		if (EstateStatus == EstateStatus.Liquidating)
		{
			if (CanFinaliseLiquidation())
			{
				Finalise();
				return true;
			}

			return false;
		}

		if (EstateStatus == EstateStatus.Undiscovered &&
		    EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime >=
		    EstateStartTime + EconomicZone.EstateDefaultDiscoverTime)
		{
			EstateStatus = EstateStatus.ClaimPhase;
			FinalisationDate = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime +
			                   EconomicZone.EstateClaimPeriodLength;
			Changed = true;
			return true;
		}

		return false;
	}

	public bool StartLiquidation()
	{
		EstateStatus = EstateStatus.Liquidating;
		FinalisationDate = null;
		Changed = true;

		foreach (var asset in Assets.Where(x => !x.IsTransferred).ToList())
		{
			switch (asset.Asset)
			{
				case IBankAccount account:
				{
					var value = Math.Max(0.0M, account.CurrentBalance);
					if (value > 0.0M)
					{
						account.WithdrawFromTransaction(value, $"Estate liquidation for character {Character.Id}");
						account.Bank.CurrencyReserves[account.Currency] -= value;
						account.Bank.Changed = true;
					}

					account.SetStatus(BankAccountStatus.Closed);
					asset.IsLiquidated = true;
					asset.LiquidatedValue = value;
					break;
				}
				case IGameItem item:
					item.SetOwner(this);
					break;
				case IProperty property:
					if (!property.PropertyOwners.Any(x => x.Owner == this))
					{
						property.TransferProperty(this);
					}

					break;
			}
		}

		var auctionHouse = EconomicZone.EstateAuctionHouse;
		if (auctionHouse == null)
		{
			return false;
		}

		foreach (var asset in Assets.Where(AssetNeedsAuctionListing).ToList())
		{
			TryCreateAuctionListing(auctionHouse, asset, DefaultReservePrice(asset), null);
		}

		return true;
	}

	public bool TryCreateAuctionListing(IAuctionHouse auctionHouse, IEstateAsset asset, decimal reservePrice,
		decimal? buyoutPrice)
	{
		if (EstateStatus != EstateStatus.Liquidating || auctionHouse == null || reservePrice <= 0.0M)
		{
			return false;
		}

		if (auctionHouse.EconomicZone != EconomicZone || !CanCreateAuctionListing(asset, auctionHouse))
		{
			return false;
		}

		switch (asset.Asset)
		{
			case IGameItem item:
				item.SetOwner(this);
				item.ContainedIn?.Take(item);
				item.InInventoryOf?.Take(item);
				item.Location?.Extract(item);
				break;
			case IProperty property:
				if (!property.PropertyOwners.Any(x => x.Owner == this))
				{
					property.TransferProperty(this);
				}

				break;
			default:
				return false;
		}

		auctionHouse.AddAuctionItem(new AuctionItem
		{
			Asset = asset.Asset,
			Seller = this,
			PayoutTarget = this,
			ListingDateTime = auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime,
			FinishingDateTime =
				new MudDateTime(auctionHouse.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime) +
				auctionHouse.DefaultListingTime,
			MinimumPrice = reservePrice,
			BuyoutPrice = buyoutPrice > reservePrice ? buyoutPrice : null
		});
		return true;
	}

	public void RecordAuctionCompletion(AuctionItem item, AuctionBid winningBid)
	{
		var asset = FindAsset(item.Asset);
		if (asset == null)
		{
			return;
		}

		if (winningBid != null)
		{
			asset.IsLiquidated = true;
			asset.LiquidatedValue = winningBid.Bid;
			return;
		}

		asset.IsLiquidated = false;
		asset.LiquidatedValue = null;
	}

	public bool HasPendingLiquidationLots =>
		(EconomicZone.EstateAuctionHouse != null &&
		 EconomicZone.EstateAuctionHouse.ActiveAuctionItems.Any(x => x.IsSeller(this))) ||
		(EconomicZone.EstateAuctionHouse != null &&
		 EconomicZone.EstateAuctionHouse.UnclaimedItems.Any(x =>
			 x.AuctionItem.IsSeller(this) &&
			 x.WinningBid == null));

	public void Finalise()
	{
		var inheritor = ResolveInheritor();
		var approvedClaims = Claims.Where(x => x.Status == ClaimStatus.Approved).ToList();
		if (!approvedClaims.Any())
		{
			foreach (var asset in Assets.Where(x => !x.IsTransferred && !x.IsLiquidated))
			{
				TransferAsset(asset, inheritor);
			}
		}
		else
		{
			if (EstateStatus != EstateStatus.Liquidating)
			{
				if (!StartLiquidation())
				{
					return;
				}
			}

			if (!CanFinaliseLiquidation())
			{
				return;
			}

			var distributableCash = Assets.OfType<EstateAsset>()
				.Where(x => x.IsLiquidated && x.LiquidatedValue.HasValue)
				.Sum(x => x.LiquidatedValue.Value);

			var paidClaims = 0.0M;
			foreach (var claim in approvedClaims
				         .OrderByDescending(x => x.IsSecured)
				         .ThenBy(x => x.ClaimDate))
			{
				var payout = Math.Min(distributableCash - paidClaims, claim.Amount);
				if (payout <= 0.0M)
				{
					break;
				}

				RouteFunds(payout, claim.Claimant, $"Estate claim #{claim.Id} for character {Character.Id}");
				paidClaims += payout;
			}

			var residual = distributableCash - paidClaims;
			if (residual > 0.0M)
			{
				RouteResidual(residual, inheritor);
			}

			foreach (var asset in Assets.Where(x => !x.IsTransferred && !x.IsLiquidated))
			{
				TransferAsset(asset, inheritor);
			}
		}

		EstateStatus = EstateStatus.Finalised;
		FinalisationDate = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		Changed = true;
	}

	private void SetInheritor(IFrameworkItem inheritor)
	{
		_inheritor = inheritor;
		_inheritorReference = inheritor == null
			? null
			: new FrameworkItemReference(inheritor.Id, inheritor.FrameworkItemType, Gameworld);
	}

	private IFrameworkItem ResolveInheritor()
	{
		return Inheritor ?? EconomicZone.ControllingClan;
	}

	private IEstateAsset FindAsset(IFrameworkItem item)
	{
		return Assets.FirstOrDefault(x => x.Asset.FrameworkItemEquals(item.Id, item.FrameworkItemType));
	}

	private bool AssetNeedsAuctionListing(IEstateAsset asset)
	{
		if (asset.IsTransferred || asset.IsLiquidated)
		{
			return false;
		}

		if (asset.Asset is not IGameItem && asset.Asset is not IProperty)
		{
			return false;
		}

		var auctionHouse = EconomicZone.EstateAuctionHouse;
		if (auctionHouse == null)
		{
			return true;
		}

		return !auctionHouse.ActiveAuctionItems.Any(x =>
			       x.IsSeller(this) &&
			       x.Asset.FrameworkItemEquals(asset.Asset.Id, asset.Asset.FrameworkItemType)) &&
		       !auctionHouse.UnclaimedItems.Any(x =>
			       x.AuctionItem.IsSeller(this) &&
			       x.AuctionItem.Asset.FrameworkItemEquals(asset.Asset.Id, asset.Asset.FrameworkItemType)) &&
		       !auctionHouse.AuctionResults.Any(x =>
			       x.SellerId == Id &&
			       x.SellerType.EqualTo(FrameworkItemType) &&
			       x.AssetId == asset.Asset.Id &&
			       x.AssetType.EqualTo(asset.Asset.FrameworkItemType));
	}

	private bool CanCreateAuctionListing(IEstateAsset asset, IAuctionHouse auctionHouse)
	{
		if (asset.IsTransferred || asset.IsLiquidated)
		{
			return false;
		}

		if (asset.Asset is not IGameItem && asset.Asset is not IProperty)
		{
			return false;
		}

		return !auctionHouse.ActiveAuctionItems.Any(x =>
			       x.IsSeller(this) &&
			       x.Asset.FrameworkItemEquals(asset.Asset.Id, asset.Asset.FrameworkItemType)) &&
		       !auctionHouse.UnclaimedItems.Any(x =>
			       x.AuctionItem.IsSeller(this) &&
			       x.AuctionItem.Asset.FrameworkItemEquals(asset.Asset.Id, asset.Asset.FrameworkItemType));
	}

	private bool CanFinaliseLiquidation()
	{
		if (HasPendingLiquidationLots)
		{
			return false;
		}

		return !Assets.Any(AssetNeedsAuctionListing);
	}

	private decimal DefaultReservePrice(IEstateAsset asset)
	{
		return asset.Asset switch
		{
			IProperty property => property.LastSaleValue,
			IGameItem item => item.Prototype.CostInBaseCurrency /
			                  EconomicZone.Currency.BaseCurrencyToGlobalBaseCurrencyConversion,
			_ => 0.0M
		};
	}

	private void TransferAsset(IEstateAsset asset, IFrameworkItem inheritor)
	{
		switch (asset.Asset)
		{
			case IGameItem item when inheritor != null:
				item.SetOwner(inheritor);
				asset.IsTransferred = true;
				break;
			case IBankAccount account when inheritor != null:
				account.SetAccountOwner(inheritor);
				asset.IsTransferred = true;
				break;
			case IProperty property when inheritor != null:
				property.TransferProperty(inheritor);
				asset.IsTransferred = true;
				break;
		}

		if (inheritor == null)
		{
			switch (asset.Asset)
			{
				case IBankAccount account when account.CurrentBalance > 0.0M:
					EconomicZone.TotalRevenueHeld += account.CurrentBalance;
					account.WithdrawFromTransaction(account.CurrentBalance, $"Estate finalisation for {Character.Id}");
					asset.IsLiquidated = true;
					asset.LiquidatedValue = account.CurrentBalance;
					break;
			}
		}
	}

	private void RouteResidual(decimal residual, IFrameworkItem inheritor)
	{
		RouteFunds(residual, inheritor, $"Estate residue for character {Character.Id}");
	}

	private void RouteFunds(decimal amount, IFrameworkItem recipient, string reference)
	{
		if (amount <= 0.0M)
		{
			return;
		}

		switch (recipient)
		{
			case IBankAccount account:
				account.DepositFromTransaction(amount, reference);
				return;
			case IFrameworkItem frameworkItem:
			{
				var payoutAccount = Gameworld.BankAccounts.FirstOrDefault(x =>
					x.AccountStatus == BankAccountStatus.Active &&
					x.Currency == EconomicZone.Currency &&
					x.IsAccountOwner(frameworkItem));
				if (payoutAccount != null)
				{
					payoutAccount.DepositFromTransaction(amount, reference);
					return;
				}

				break;
			}
		}

		EconomicZone.TotalRevenueHeld += amount;
	}
}
