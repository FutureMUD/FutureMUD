#nullable enable

namespace MudSharp.Economy.Property;

/// <summary>
/// Resolves durable property child records through the properties that own them.
/// Property keys, leases and orders intentionally do not have duplicate global registries.
/// </summary>
internal static class PropertyReferenceLookup
{
	public static IPropertyKey? GetPropertyKey(IFuturemud gameworld, long id)
	{
		return gameworld.Properties
		                .SelectMany(x => x.PropertyKeys)
		                .FirstOrDefault(x => x.Id == id);
	}

	public static IPropertyLease? GetPropertyLease(IFuturemud gameworld, long id)
	{
		return AllPropertyLeases(gameworld).FirstOrDefault(x => x.Id == id);
	}

	public static IPropertyLeaseOrder? GetPropertyLeaseOrder(IFuturemud gameworld, long id)
	{
		return AllPropertyLeaseOrders(gameworld).FirstOrDefault(x => x.Id == id);
	}

	public static IPropertySaleOrder? GetPropertySaleOrder(IFuturemud gameworld, long id)
	{
		return gameworld.Properties
		                .Select(x => x.SaleOrder)
		                .FirstOrDefault(x => x?.Id == id);
	}

	public static IEnumerable<IPropertyLease> AllPropertyLeases(IFuturemud gameworld)
	{
		foreach (var property in gameworld.Properties)
		{
			foreach (var expiredLease in property.ExpiredLeases)
			{
				yield return expiredLease;
			}

			if (property.Lease is { } activeLease)
			{
				yield return activeLease;
			}
		}
	}

	public static IEnumerable<IPropertyLeaseOrder> AllPropertyLeaseOrders(IFuturemud gameworld)
	{
		foreach (var property in gameworld.Properties)
		{
			foreach (var expiredLeaseOrder in property.ExpiredLeaseOrders)
			{
				yield return expiredLeaseOrder;
			}

			if (property.LeaseOrder is { } activeLeaseOrder)
			{
				yield return activeLeaseOrder;
			}
		}
	}
}
