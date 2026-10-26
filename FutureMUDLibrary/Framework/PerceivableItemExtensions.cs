using System.Collections;
using System.Collections.Generic;

namespace MudSharp.Framework
{
    public static class PerceivableItemExtensions
    {
        /// <summary>
        /// Enumerates each perceivable as an individual entity, expanding groups into their members in encounter order.
        /// </summary>
        public static IEnumerable<IPerceivable> GetIndividualPerceivables(this IEnumerable<IPerceivable> underlyingList)
        {
            foreach (IPerceivable item in underlyingList)
            {
                if (item.IsSingleEntity)
                {
                    yield return item;
                    continue;
                }

                foreach (IPerceivable subItem in ((IPerceivableGroup)item).Members)
                {
                    yield return subItem;
                }
            }
        }
    }
}
