using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Banking;

public partial class Bank
{
	private IEmploymentHostState? _employment;

	public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Bank;
	public IMarket? Market => null;
}
