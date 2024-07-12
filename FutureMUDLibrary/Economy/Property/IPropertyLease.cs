using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Economy.Property
{
	public interface IPropertyLease : IFrameworkItem
	{
		IProperty Property { get; }
		IPropertyLeaseOrder LeaseOrder { get; }
		IFrameworkItem Leaseholder { get; set; }
		decimal PricePerInterval { get; set;}
		decimal BondPayment { get; set;}
		decimal PaymentBalance { get; set; }
		decimal BondClaimed { get; set; }
		RecurringInterval Interval { get; }
		MudDateTime LeaseStart { get; }
		MudDateTime LeaseEnd { get; }
		MudDateTime LastLeasePayment { get; }
		bool AutoRenew { get; set; }
		bool BondReturned { get; set; }
		IEnumerable<IFrameworkItem> DeclaredTenants { get; }
		void DeclareTenant(IFrameworkItem tenant);
		void RemoveTenant(IFrameworkItem tenant);
		bool IsTenant(ICharacter character, bool ignoreIndirectTenancy);
		void MakePayment(decimal paymentAmount);
		void Delete();
		void TerminateLease();
		bool IsAuthorisedLeaseHolder(ICharacter who);
		void SetupListeners();
	}
}