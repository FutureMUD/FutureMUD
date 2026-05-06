using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Economy.Estates;

public class Estate : SaveableItem, IEstate, ILazyLoadDuringIdleTime
{
    private readonly record struct EstateAssetSnapshot(
        IEconomicZone Zone,
        IFrameworkItem Asset,
        bool PresumedOwnership,
        decimal OwnershipShare);

    public static IEnumerable<IEstate> CreateEstatesForCharacterDeath(ICharacter character)
    {
        if (!ShouldProduceEstate(character))
        {
            return Enumerable.Empty<IEstate>();
        }

        Dictionary<long, Estate> estates = character.Gameworld.Estates
            .Where(x => x.Character == character &&
                        x.EstateStatus != EstateStatus.Finalised &&
                        x.EstateStatus != EstateStatus.Cancelled)
            .OfType<Estate>()
            .GroupBy(x => x.EconomicZone.Id)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.EstateStartTime).First());
        Dictionary<long, List<EstateAssetSnapshot>> capturedAssets = CaptureAssetsForCharacter(character)
            .GroupBy(x => x.Zone.Id)
            .ToDictionary(x => x.Key, x => x.ToList());

        Estate GetEstate(IEconomicZone zone)
        {
            if (zone == null)
            {
                return null;
            }

            if (!estates.TryGetValue(zone.Id, out Estate estate) && zone.EstatesEnabled)
            {
                estate = new Estate(zone, character, character.EstateHeir);
                estates[zone.Id] = estate;
            }
            else if (estate?.EstateStatus == EstateStatus.EstateWill)
            {
                estate.ActivateWill(character.EstateHeir);
            }

            return estate;
        }

        static bool ClaimMatches(IEstateClaim claim, IFrameworkItem claimant, decimal amount, string reason,
            bool isSecured, IFrameworkItem targetItem)
        {
            if (!claim.Claimant.FrameworkItemEquals(claimant.Id, claimant.FrameworkItemType) ||
                claim.Amount != amount ||
                !claim.Reason.EqualTo(reason) ||
                claim.IsSecured != isSecured)
            {
                return false;
            }

            if (targetItem == null)
            {
                return claim.TargetItem == null;
            }

            return claim.TargetItem != null &&
                   claim.TargetItem.FrameworkItemEquals(targetItem.Id, targetItem.FrameworkItemType);
        }

        static void AddClaimIfMissing(Estate estate, IFrameworkItem claimant, decimal amount, string reason,
            bool isSecured, IFrameworkItem targetItem = null)
        {
            if (estate == null)
            {
                return;
            }

            if (estate.Claims.Any(x => ClaimMatches(x, claimant, amount, reason, isSecured, targetItem)))
            {
                return;
            }

            estate.AddClaim(new EstateClaim(estate, claimant, amount, reason, ClaimStatus.NotAssessed, isSecured,
                targetItem));
        }

        static void SyncEstateAssets(Estate estate, IEnumerable<EstateAssetSnapshot> assets)
        {
            if (estate == null)
            {
                return;
            }

            estate.SynchroniseAssets(assets);
        }

        foreach (KeyValuePair<long, List<EstateAssetSnapshot>> zoneAssets in capturedAssets)
        {
            Estate estate = GetEstate(zoneAssets.Value.First().Zone);
            SyncEstateAssets(estate, zoneAssets.Value);
        }

        foreach (IBankAccount account in character.Gameworld.BankAccounts.Where(x => x.IsAccountOwner(character)))
        {
            Estate estate = GetEstate(account.Bank.EconomicZone);
            decimal securedAmount = Math.Max(0.0M, account.CurrentMonthFees + Math.Max(0.0M, account.CurrentBalance * -1.0M));
            if (securedAmount > 0.0M)
            {
                AddClaimIfMissing(estate, account.Bank, securedAmount,
                    $"Secured bank debt for account {account.AccountReference}", true, account);
            }
        }

        foreach (Estate estate in estates.Values.Where(x => x.EstateStatus == EstateStatus.EstateWill).ToList())
        {
            if (capturedAssets.ContainsKey(estate.EconomicZone.Id))
            {
                continue;
            }

            SyncEstateAssets(estate, Enumerable.Empty<EstateAssetSnapshot>());
            if (!estate.Assets.Any())
            {
                estate.CancelWill();
            }
        }

        foreach (ILineOfCreditAccount loc in character.Gameworld.LineOfCreditAccounts.Where(x => x.IsAccountOwner(character)))
        {
            IEconomicZone zone = character.Location == null ? null : DetermineZone(character.Gameworld, character.Location);
            if (zone == null || !estates.TryGetValue(zone.Id, out Estate estate))
            {
                continue;
            }

            if (loc.OutstandingBalance > 0.0M)
            {
                AddClaimIfMissing(estate, loc, loc.OutstandingBalance,
                    $"Outstanding line of credit debt for {loc.AccountName}", true);
            }
        }

        return estates.Values;
    }

    private static IEnumerable<EstateAssetSnapshot> CaptureAssetsForCharacter(ICharacter character)
    {
        foreach (IProperty property in character.Gameworld.Properties
                     .Where(x => x.PropertyOwners.Any(y => y.Owner == character)))
        {
            decimal share = property.PropertyOwners
                .Where(x => x.Owner == character)
                .Sum(x => x.ShareOfOwnership);
            if (share <= 0.0M)
            {
                continue;
            }

            yield return new EstateAssetSnapshot(property.EconomicZone, property, false, share);
        }

        foreach (IBankAccount account in character.Gameworld.BankAccounts.Where(x => x.IsAccountOwner(character)))
        {
            yield return new EstateAssetSnapshot(account.Bank.EconomicZone, account, false, 1.0M);
        }

        foreach (var lostProperty in character.Gameworld.Properties
                     .SelectMany(x => x.HotelLostProperties)
                     .Where(x => x.Bundle != null && (x.OwnerId == character.Id || x.Bundle.IsOwnedBy(character))))
        {
            yield return new EstateAssetSnapshot(lostProperty.Property.EconomicZone, lostProperty.Bundle, false, 1.0M);
        }

        IEconomicZone itemZone = character.Location == null ? null : DetermineZone(character.Gameworld, character.Location);
        if (itemZone == null)
        {
            yield break;
        }

        foreach (IGameItem item in character.Body.AllItems.Distinct())
        {
            if (item.HasOwner && item.Owner != character)
            {
                continue;
            }

            yield return new EstateAssetSnapshot(itemZone, item, !item.HasOwner, 1.0M);
        }
    }

    private static bool ShouldProduceEstate(ICharacter character)
    {
        if (character.IsGuest)
        {
            return false;
        }

        if (character.IsPlayerCharacter)
        {
            return true;
        }

        IFutureProg prog = ResolveNpcEstateProg(character.Gameworld);
        return prog.MatchesParameters(Enumerable.Empty<ProgVariableTypes>())
            ? prog.ExecuteBool(false)
            : prog.ExecuteBool(false, character);
    }

    private static IFutureProg ResolveNpcEstateProg(IFuturemud gameworld)
    {
        try
        {
            string configuration = gameworld.GetStaticConfiguration("NpcEstateProg");
            if (long.TryParse(configuration, out long value))
            {
                IFutureProg prog = gameworld.FutureProgs.Get(value);
                if (prog != null &&
                    prog.ReturnType == ProgVariableTypes.Boolean &&
                    (prog.MatchesParameters(Enumerable.Empty<ProgVariableTypes>()) ||
                     prog.MatchesParameters(new[] { ProgVariableTypes.Character })))
                {
                    return prog;
                }
            }
        }
        catch
        {
            // Deliberately fall back to AlwaysFalse below.
        }

        return gameworld.AlwaysFalseProg;
    }

    public static IEconomicZone DetermineZone(IFuturemud gameworld, ICell cell)
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
        EstateStartTime = MudDateTime.FromStoredStringOrFallback(estate.EstateStartTime, gameworld,
            StoredMudDateTimeFallback.CurrentDateTime, "Estate", estate.Id, null, "EstateStartTime");
        FinalisationDate = string.IsNullOrWhiteSpace(estate.FinalisationDate)
            ? null
            : MudDateTime.FromStoredStringOrFallback(estate.FinalisationDate, gameworld,
                StoredMudDateTimeFallback.Never, "Estate", estate.Id, null, "FinalisationDate");
        if (estate.InheritorId.HasValue && !string.IsNullOrWhiteSpace(estate.InheritorType))
        {
            _inheritorReference = new FrameworkItemReference(estate.InheritorId.Value, estate.InheritorType, gameworld);
        }

        foreach (Models.EstateClaim claim in estate.EstateClaims)
        {
            _claims.Add(new EstateClaim(claim, this));
        }

        foreach (Models.EstateAsset asset in estate.EstateAssets)
        {
            _assets.Add(new EstateAsset(asset, this));
        }

        foreach (Models.EstatePayout payout in estate.EstatePayouts)
        {
            _payouts.Add(new EstatePayout(payout, this));
        }

        EconomicZone.AddEstate(this);
        Gameworld.SaveManager.AddLazyLoad(this);
    }

    public Estate(IEconomicZone economicZone, ICharacter character, IFrameworkItem inheritor,
        EstateStatus estateStatus = EstateStatus.Undiscovered)
    {
        Gameworld = economicZone.Gameworld;
        EconomicZone = economicZone;
        _character = character;
        _characterId = character.Id;
        EstateStatus = estateStatus;
        EstateStartTime = economicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        SetInheritor(inheritor);
        using (new FMDB())
        {
            Models.Estate dbitem = new()
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
        Models.Estate dbitem = FMDB.Context.Estates.Find(Id);
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

    private readonly List<IEstatePayout> _payouts = new();
    public IEnumerable<IEstatePayout> Payouts => _payouts;

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

    public void AddPayout(IEstatePayout payout)
    {
        _payouts.Add(payout);
        Changed = true;
    }

    public void RefreshWill()
    {
        if (EstateStatus != EstateStatus.EstateWill)
        {
            return;
        }

        SynchroniseAssets(CaptureAssetsForCharacter(Character).Where(x => x.Zone == EconomicZone));
        SetInheritor(Character.EstateHeir);
    }

    private void ActivateWill(IFrameworkItem inheritor)
    {
        if (EstateStatus != EstateStatus.EstateWill)
        {
            return;
        }

        EstateStatus = EstateStatus.Undiscovered;
        EstateStartTime = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        FinalisationDate = null;
        SetInheritor(inheritor);
        Changed = true;
    }

    private void CancelWill()
    {
        EstateStatus = EstateStatus.Cancelled;
        FinalisationDate = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
        Changed = true;
    }

    private void SynchroniseAssets(IEnumerable<EstateAssetSnapshot> assets)
    {
        List<EstateAssetSnapshot> snapshots = assets.ToList();
        foreach (EstateAssetSnapshot snapshot in snapshots)
        {
            EstateAsset existing = Assets
                .OfType<EstateAsset>()
                .FirstOrDefault(x => x.Asset.FrameworkItemEquals(snapshot.Asset.Id, snapshot.Asset.FrameworkItemType));
            if (existing == null)
            {
                AddAsset(new EstateAsset(this, snapshot.Asset, snapshot.PresumedOwnership, snapshot.OwnershipShare));
                continue;
            }

            existing.OwnershipShare = snapshot.OwnershipShare;
            UpdateClaimsForAsset(existing.Asset, existing.AssumedValue);
        }

        List<EstateAsset> staleAssets = Assets
            .OfType<EstateAsset>()
            .Where(x => !x.IsTransferred && !x.IsLiquidated)
            .Where(x => snapshots.All(y => !x.Asset.FrameworkItemEquals(y.Asset.Id, y.Asset.FrameworkItemType)))
            .ToList();
        foreach (EstateAsset asset in staleAssets)
        {
            RemoveAsset(asset);
            asset.Delete();
        }

        RevalidateClaimsAgainstCurrentAssets();
    }

    public bool OpenProbate()
    {
        if (EstateStatus != EstateStatus.Undiscovered && EstateStatus != EstateStatus.EstateWill)
        {
            return false;
        }

        EstateStatus = EstateStatus.ClaimPhase;
        FinalisationDate = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime +
                           EconomicZone.EstateClaimPeriodLength;
        Changed = true;
        return true;
    }

    public bool CheckStatus()
    {
        if (EstateStatus == EstateStatus.Finalised || EstateStatus == EstateStatus.Cancelled ||
            EstateStatus == EstateStatus.EstateWill)
        {
            return false;
        }

        if (EstateStatus == EstateStatus.ClaimPhase)
        {
            if (EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime >= FinalisationDate)
            {
                if (Claims.Any(x => x.Status == ClaimStatus.Approved && ClaimRequiresLiquidation(x)))
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
            return OpenProbate();
        }

        return false;
    }

    public bool StartLiquidation()
    {
        EstateStatus = EstateStatus.Liquidating;
        FinalisationDate = null;
        Changed = true;

        foreach (IEstateAsset asset in Assets.Where(x => !x.IsTransferred).ToList())
        {
            switch (asset.Asset)
            {
                case IBankAccount account:
                    {
                        decimal value = Math.Max(0.0M, account.CurrentBalance);
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
                    if (asset.OwnershipShare >= 1.0M && !property.PropertyOwners.Any(x => x.Owner == this))
                    {
                        property.TransferProperty(this);
                    }

                    break;
            }
        }

        IAuctionHouse auctionHouse = EconomicZone.EstateAuctionHouse;
        if (auctionHouse == null)
        {
            return true;
        }

        foreach (IEstateAsset asset in Assets.Where(AssetNeedsAuctionListing).ToList())
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
                if (asset.OwnershipShare < 1.0M)
                {
                    return false;
                }

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
            PropertyShare = asset.OwnershipShare,
            MinimumPrice = reservePrice,
            BuyoutPrice = buyoutPrice > reservePrice ? buyoutPrice : null
        });
        return true;
    }

    public void RecordAuctionCompletion(AuctionItem item, AuctionBid winningBid, decimal sellerProceeds)
    {
        IEstateAsset asset = FindAsset(item.Asset);
        if (asset == null)
        {
            return;
        }

        if (winningBid != null)
        {
            asset.IsLiquidated = true;
            asset.LiquidatedValue = sellerProceeds;
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
        IFrameworkItem inheritor = ResolveInheritor();
        List<IEstateClaim> approvedClaims = Claims.Where(x => x.Status == ClaimStatus.Approved).ToList();
        if (!approvedClaims.Any() && EstateStatus != EstateStatus.Liquidating)
        {
            foreach (IEstateAsset asset in Assets.Where(x => !x.IsTransferred && !x.IsLiquidated))
            {
                TransferAsset(asset, inheritor);
            }
        }
        else
        {
            if (EstateStatus != EstateStatus.Liquidating && approvedClaims.Any(ClaimRequiresLiquidation))
            {
                if (!StartLiquidation())
                {
                    return;
                }
            }

            if (EstateStatus == EstateStatus.Liquidating)
            {
                if (!CanFinaliseLiquidation())
                {
                    return;
                }

                decimal distributableCash = Assets.OfType<EstateAsset>()
                    .Where(x => x.IsLiquidated && x.LiquidatedValue.HasValue)
                    .Sum(x => x.LiquidatedValue.Value);

                decimal paidClaims = 0.0M;
                foreach (IEstateClaim claim in approvedClaims
                             .OrderByDescending(x => x.IsSecured)
                             .ThenBy(x => x.ClaimDate))
                {
                    decimal payout = Math.Min(distributableCash - paidClaims, LiquidationPayoutCap(claim));
                    if (payout <= 0.0M)
                    {
                        break;
                    }

                    RouteFunds(payout, ResolveClaimRecipient(claim), $"Estate claim #{claim.Id} for character {Character.Id}");
                    paidClaims += payout;
                }

                decimal residual = distributableCash - paidClaims;
                if (residual > 0.0M)
                {
                    RouteResidual(residual, inheritor);
                }

                foreach (IEstateAsset asset in Assets.Where(x => !x.IsTransferred && !x.IsLiquidated))
                {
                    TransferAsset(asset, inheritor);
                }
            }
            else
            {
                foreach (IEstateClaim claim in approvedClaims
                             .Where(ClaimTransfersSpecificAsset)
                             .OrderByDescending(x => x.IsSecured)
                             .ThenBy(x => x.ClaimDate))
                {
                    IEstateAsset asset = FindAsset(claim.TargetItem);
                    if (asset == null || asset.IsTransferred || asset.IsLiquidated)
                    {
                        continue;
                    }

                    TransferAsset(asset, claim.Claimant);
                }

                foreach (IEstateAsset asset in Assets.Where(x => !x.IsTransferred && !x.IsLiquidated))
                {
                    TransferAsset(asset, inheritor);
                }
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

    private void UpdateClaimsForAsset(IFrameworkItem targetItem, decimal amount)
    {
        foreach (IEstateClaim claim in Claims.Where(x =>
                     x.TargetItem != null &&
                     x.TargetItem.FrameworkItemEquals(targetItem.Id, targetItem.FrameworkItemType)))
        {
            claim.Amount = Math.Max(0.0M, amount);
        }
    }

    private IFrameworkItem ResolveClaimRecipient(IEstateClaim claim)
    {
        return claim.Claimant switch
        {
            IBank when claim.TargetItem is IBankAccount account => account,
            _ => claim.Claimant
        };
    }

    private IEstateAsset FindAsset(IFrameworkItem item)
    {
        return Assets.FirstOrDefault(x => x.Asset.FrameworkItemEquals(item.Id, item.FrameworkItemType));
    }

    private void RevalidateClaimsAgainstCurrentAssets()
    {
        foreach (IEstateClaim claim in Claims.Where(x => x.TargetItem != null).ToList())
        {
            IEstateAsset asset = FindAsset(claim.TargetItem);
            if (asset == null)
            {
                claim.Status = ClaimStatus.Rejected;
                claim.StatusReason = "The targeted asset is no longer part of the estate.";
                continue;
            }

            claim.Amount = Math.Max(0.0M, asset.AssumedValue);
        }
    }

    private IPropertyOwner ResolvePropertyOwnerForAsset(IProperty property, IEstateAsset asset)
    {
        return property.PropertyOwners.FirstOrDefault(x =>
                   x.Owner.FrameworkItemEquals(Id, FrameworkItemType)) ??
               property.PropertyOwners.FirstOrDefault(x =>
                   x.Owner.FrameworkItemEquals(Character.Id, Character.FrameworkItemType));
    }

    private void TransferPropertyShare(IProperty property, IEstateAsset asset, IFrameworkItem inheritor)
    {
        IPropertyOwner owner = ResolvePropertyOwnerForAsset(property, asset);
        if (owner == null)
        {
            return;
        }

        decimal transferShare = Math.Min(owner.ShareOfOwnership, asset.OwnershipShare);
        if (transferShare <= 0.0M)
        {
            return;
        }

        if (transferShare >= 1.0M && owner.ShareOfOwnership >= 1.0M && property.PropertyOwners.Count() == 1)
        {
            property.TransferProperty(inheritor);
            asset.IsTransferred = true;
            return;
        }

        property.DivestOwnership(owner, transferShare / owner.ShareOfOwnership, inheritor);
        asset.IsTransferred = true;
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

        if (asset.Asset is IProperty && asset.OwnershipShare < 1.0M)
        {
            return false;
        }

        IAuctionHouse auctionHouse = EconomicZone.EstateAuctionHouse;
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

        if (asset.Asset is IProperty && asset.OwnershipShare < 1.0M)
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
        return Math.Max(0.0M, asset.AssumedValue);
    }

    private bool ClaimTransfersSpecificAsset(IEstateClaim claim)
    {
        return claim.TargetItem != null && !claim.IsSecured;
    }

    private bool ClaimRequiresLiquidation(IEstateClaim claim)
    {
        return !ClaimTransfersSpecificAsset(claim);
    }

    private decimal LiquidationPayoutCap(IEstateClaim claim)
    {
        if (!ClaimTransfersSpecificAsset(claim))
        {
            return claim.Amount;
        }

        IEstateAsset asset = FindAsset(claim.TargetItem);
        return asset?.LiquidatedValue ?? 0.0M;
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
                TransferPropertyShare(property, asset, inheritor);
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
            case ILineOfCreditAccount lineOfCredit:
                lineOfCredit.PayoffAccount(amount);
                return;
            case IBank bank:
                bank.CurrencyReserves[bank.PrimaryCurrency] += amount;
                bank.Changed = true;
                return;
            case IFrameworkItem frameworkItem:
                {
                    IBankAccount payoutAccount = Gameworld.BankAccounts.FirstOrDefault(x =>
                        x.AccountStatus == BankAccountStatus.Active &&
                        x.Currency == EconomicZone.Currency &&
                        x.IsAccountOwner(frameworkItem));
                    if (payoutAccount != null)
                    {
                        payoutAccount.DepositFromTransaction(amount, reference);
                        return;
                    }

                    if (frameworkItem is ICharacter)
                    {
                        AddPayout(new EstatePayout(this, frameworkItem, amount, reference));
                        return;
                    }

                    break;
                }
        }

        EconomicZone.TotalRevenueHeld += amount;
    }
}
