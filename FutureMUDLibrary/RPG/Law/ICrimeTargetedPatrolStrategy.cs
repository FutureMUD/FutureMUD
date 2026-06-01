using System.Collections.Generic;

namespace MudSharp.RPG.Law;

public interface ICrimeTargetedPatrolStrategy : IConfigurablePatrolStrategy
{
	IEnumerable<ICrime> SelectDispatchCrimes(ILegalAuthority authority);
	bool ShouldDispatchForCrime(IPatrolRoute patrol, ICrime crime);
}
