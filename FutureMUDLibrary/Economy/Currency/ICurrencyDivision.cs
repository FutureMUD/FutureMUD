using System.Collections.Generic;
using System.Text.RegularExpressions;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Economy.Currency {
    public interface ICurrencyDivision : IFrameworkItem, IEditableItem {
        /// <summary>
        ///     Conversion between this division and the "base" unit for the currency, e.g. if cents is base unit, dollar is 100.
        /// </summary>
        decimal BaseUnitConversionRate { get; }

        /// <summary>
        ///     Abbreviations that can be used to refer to this currency division, e.g. $, dollars, bucks
        /// </summary>
        IEnumerable<Regex> Patterns { get; }
    }
}