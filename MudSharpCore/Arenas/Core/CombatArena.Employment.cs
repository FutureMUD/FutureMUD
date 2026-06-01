using MudSharp.Economy;
using MudSharp.Economy.Employment;

#nullable enable

namespace MudSharp.Arenas;

public sealed partial class CombatArena
{
	private IEmploymentHostState? _employment;

	public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Arena;
	public IMarket? Market => null;
}
