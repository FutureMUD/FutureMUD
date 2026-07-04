using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Economy.Hospitals;

public partial class Hospital
{
	private IEmploymentHostState? _employment;

	public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Hospital;
	public IMarket? Market => null;
}