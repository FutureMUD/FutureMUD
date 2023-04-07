using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.TimeAndDate;

namespace MudSharp.Economy.Property
{
	public interface IPropertySaleOrder : IFrameworkItem
	{
		IProperty Property { get; }
		decimal ReservePrice { get; set; }
		IReadOnlyDictionary<IPropertyOwner, bool> PropertyOwnerConsent { get; }
		void SetConsent(IPropertyOwner owner);
		void ChangeConsentDueToSale(IPropertyOwner newOwner);
		void ResetConsent();
		PropertySaleOrderStatus OrderStatus { get; set; }
		MudDateTime StartOfListing { get; set; }
		TimeSpan DurationOfListing { get; }
		bool ShowForSale { get; }
		void Delete();
	}
}