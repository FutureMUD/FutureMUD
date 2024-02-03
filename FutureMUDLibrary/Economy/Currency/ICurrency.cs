using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Economy.Currency {
    public interface ICurrency : IEditableItem, IFutureProgVariable {
        /// <summary>
        ///     All Currency Divisions belonging to this Currency
        /// </summary>
        IEnumerable<ICurrencyDivision> CurrencyDivisions { get; }

        /// <summary>
        ///     All Coins belonging to this Currency
        /// </summary>
        IEnumerable<ICoin> Coins { get; }

        /// <summary>
        ///     A collection dictionary containing all of the CurrencyDescriptionPatterns by their CurrencyDescriptionPatternType
        /// </summary>
        CollectionDictionary<CurrencyDescriptionPatternType, ICurrencyDescriptionPattern> PatternDictionary { get; }

        /// <summary>
        ///     Requests that the Currency describe the stated amount in the specified pattern
        /// </summary>
        /// <param name="value">A decimal value in currency base units</param>
        /// <param name="type">A CurrencyDescriptionPatternType that controls which pattern is used</param>
        /// <returns>A string representation of the currency amount</returns>
        string Describe(decimal value, CurrencyDescriptionPatternType type);

        /// <summary>
        ///     Takes a user-supplied string representation of currency and attempts to return the base currency amount for it
        /// </summary>
        /// <param name="pattern">The user-supplied pattern</param>
        /// <param name="success">Whether the conversion was successful</param>
        /// <returns>A decimal value of base currency units</returns>
        decimal GetBaseCurrency(string pattern, out bool success);

        /// <summary>
        /// Takes a user-supplied string representation of currency and attempts to convert it to currency
        /// </summary>
        /// <param name="pattern">The user-supplied pattern</param>
        /// <param name="amount">The decimal value found, if found, otherwise 0</param>
        /// <returns>True if the conversion was successful</returns>
        bool TryGetBaseCurrency(string pattern, out decimal amount);

        Dictionary<ICoin, int> FindCoinsForAmount(decimal amount, out bool exactMatch);

        /// <summary>
        ///     Find specific coins in specific currency piles to remove to take an amount away
        /// </summary>
        /// <param name="targetPiles"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Dictionary<ICurrencyPile, Dictionary<ICoin, int>> FindCurrency(IEnumerable<ICurrencyPile> targetPiles,
            decimal amount);

        decimal BaseCurrencyToGlobalBaseCurrencyConversion { get; }
        void AddCoin(ICoin coin);
        ICurrency Clone(string name);
    }
}