using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Auctions;

public partial class AuctionHouse
{
	private IEmploymentHostState? _employment;

	public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.AuctionHouse;
	public IMarket? Market => null;
}
