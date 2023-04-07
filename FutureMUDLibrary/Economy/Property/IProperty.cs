using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

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
		bool ApplyCriminalCodeInProperty { get; set; }
		string PreviewProperty(ICharacter voyeur);
		void SellProperty(IFrameworkItem newOwner);
		void DivestOwnership(IPropertyOwner owner, decimal percentage, IFrameworkItem newOwnerItem);

		bool IsAuthorisedOwner(ICharacter who);
		bool IsAuthorisedLeaseHolder(ICharacter who);
		bool HasUnclaimedBondPayments(ICharacter who);
        void ClaimShops(ICharacter who);
    }
}
