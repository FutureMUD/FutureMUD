using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.Economy.Property
{
	public interface IPropertyLeaseOrder : IFrameworkItem
	{
		IProperty Property { get; }
		decimal PricePerInterval { get; set; }
		decimal BondRequired { get; set; }
		IReadOnlyDictionary<IPropertyOwner, bool> PropertyOwnerConsent { get; }
		void SetConsent(IPropertyOwner owner);
		void ChangeConsentDueToSale(IPropertyOwner newOwner);
		void ResetConsent();
		RecurringInterval Interval { get; set; }
		[CanBeNull] IFutureProg CanLeaseProgCharacter { get; set; }
		[CanBeNull] IFutureProg CanLeaseProgClan { get; set; }
		TimeSpan MinimumLeaseDuration { get; set; }
		TimeSpan MaximumLeaseDuration { get; set; }
		bool AllowAutoRenew { get; set; }
		bool AutomaticallyRelistAfterLeaseTerm { get; set; }
                bool AllowLeaseNovation { get; set; }
                bool RekeyOnLeaseEnd { get; set; }
                decimal FeeIncreasePercentageAfterLeaseTerm { get; set; }
		bool ListedForLease { get; set; }
		void DoEndOfLease(IPropertyLease oldLease);
		IPropertyLease RenewLease(IPropertyLease oldLease);
		IPropertyLease CreateLease(IFrameworkItem lesee, TimeSpan duration);
		void Delete();
	}
}