using System.Collections;
using System.Collections.Generic;

namespace MudSharp.Framework
{
    public static class PerceivableItemExtensions
    {
        public static IEnumerable<IPerceivable> GetIndividualPerceivables(this IEnumerable<IPerceivable> underlyingList)
        {
            foreach (IPerceivable item in underlyingList)
            {
                if (item.IsSingleEntity)
                {
                    yield return item;
                }

                foreach (IPerceivable subItem in ((IPerceivableGroup)item).Members)
                {
                    yield return subItem;
                }
            }
        }
    }
}