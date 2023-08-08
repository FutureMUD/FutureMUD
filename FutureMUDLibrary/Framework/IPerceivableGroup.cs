using System.Collections.Generic;

namespace MudSharp.Framework
{
	public interface IPerceivableGroup : IPerceivable
	{
        IEnumerable<IPerceivable> Members { get; }
	}
}