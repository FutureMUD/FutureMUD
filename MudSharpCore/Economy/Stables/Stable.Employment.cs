using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Stables;

public partial class Stable
{
	private IEmploymentHostState? _employment;

	public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Stable;
	public IMarket? Market => null;
}
