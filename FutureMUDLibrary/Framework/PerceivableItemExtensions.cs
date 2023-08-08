using System.Collections;
using System.Collections.Generic;

namespace MudSharp.Framework
{
	public static class PerceivableItemExtensions
    {
        public static IEnumerable<IPerceivable> GetIndividualPerceivables(this IEnumerable<IPerceivable> underlyingList)
        {
            foreach (var item in underlyingList)
            {
                if (item.IsSingleEntity)
                {
                    yield return item;
                }

                foreach (var subItem in ((IPerceivableGroup)item).Members)
                {
                    yield return subItem;
                }
            }
        }
    }
}