using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Economy.Property
{
    public enum PropertySaleOrderStatus
    {
        Proposed,
        Approved,
        Executed
    }

    public interface IProperty : IFrameworkItem, ISaveable, IEditableItem, IKeywordedItem
    {
        IEconomicZone EconomicZone { get; }
        IEnumerable<IPropertyOwner> PropertyOwners { get; }
        IEnumerable<ICell> PropertyLocations { get; }
        string DetailedDescription { get; }
        MudDateTime LastChangeOfOwnership { get; }
        decimal LastSaleValue { get; set; }
        [CanBeNull] IPropertySaleOrder SaleOrder { get; set; }
        [CanBeNull] IPropertyLeaseOrder LeaseOrder { get; set; }
        [CanBeNull] IPropertyLease Lease { get; set; }
        IEnumerable<IPropertyLease> ExpiredLeases { get; }
        IEnumerable<IPropertyLeaseOrder> ExpiredLeaseOrders { get; }
        IEnumerable<IPropertyKey> PropertyKeys { get; }
        void AddKey(IPropertyKey key);
        void RemoveKey(IPropertyKey key);
        void ExpireLease(IPropertyLease lease);
        void ExpireLeaseOrder(IPropertyLeaseOrder order);
        void RekeyAllLocks();
        bool ApplyCriminalCodeInProperty { get; set; }
        string PreviewProperty(ICharacter voyeur);
        void SellProperty(IFrameworkItem newOwner);
        void TransferProperty(IFrameworkItem newOwner, decimal? transferValue = null);
        void DivestOwnership(IPropertyOwner owner, decimal percentage, IFrameworkItem newOwnerItem);

        bool IsAuthorisedOwner(ICharacter who);
        bool IsAuthorisedLeaseHolder(ICharacter who);
        bool HasUnclaimedBondPayments(ICharacter who);
        void ClaimShops(ICharacter who);
        void ClaimStables(ICharacter who);
        HotelLicenseStatus HotelLicenseStatus { get; set; }
        IBankAccount HotelBankAccount { get; set; }
        IFutureProg HotelCanRentProg { get; set; }
        MudTimeSpan HotelLostPropertyRetention { get; set; }
        decimal HotelOutstandingTaxes { get; set; }
        IEnumerable<IHotelRoom> HotelRooms { get; }
        IEnumerable<IHotelLostProperty> HotelLostProperties { get; }
        IEnumerable<IHotelPatronBalance> HotelPatronBalances { get; }
        IEnumerable<long> HotelBannedPatronIds { get; }
        bool IsApprovedHotel { get; }
        bool IsBannedFromHotel(ICharacter patron);
        void BanFromHotel(ICharacter patron);
        void UnbanFromHotel(ICharacter patron);
        bool HasHotelBalance(ICharacter patron);
        decimal HotelBalanceFor(ICharacter patron);
        void AdjustHotelBalance(ICharacter patron, decimal amount);
        IHotelRoom AddHotelRoom(ICell cell, string name, decimal pricePerDay, decimal securityDeposit, TimeSpan minimumDuration, TimeSpan maximumDuration);
        void RemoveHotelRoom(IHotelRoom room);
        IHotelRoom HotelRoomForCell(ICell cell);
        bool CanRentHotelRoom(ICharacter patron, IHotelRoom room, TimeSpan duration, out string reason);
        IHotelRoomRental RentHotelRoom(ICharacter patron, IHotelRoom room, TimeSpan duration, decimal rentalCharge, decimal taxCharge);
        decimal CompleteHotelStay(IHotelRoom room, ICharacter actor, bool force);
        void ClaimHotelLostProperty(IHotelLostProperty property);
        void CheckHotelLostProperty();
    }
}
