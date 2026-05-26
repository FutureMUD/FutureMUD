using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Shops;

public abstract partial class Shop
{
	private IEmploymentHostState? _employment;

	public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Shop;
	public IMarket? Market => MarketForPricingPurposes;
}
