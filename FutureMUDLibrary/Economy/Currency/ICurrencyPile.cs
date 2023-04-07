using System;
using System.Collections.Generic;
using MudSharp.GameItems;

namespace MudSharp.Economy.Currency {
    public interface ICurrencyPile : IGameItemComponent {
        ICurrency Currency { get; set; }
        IEnumerable<Tuple<ICoin, int>> Coins { get; }
        decimal TotalValue { get; }

        void AddCoins(IEnumerable<Tuple<ICoin, int>> coins);
        void AddCoins(IEnumerable<KeyValuePair<ICoin, int>> coins);
        void AddCoins(IEnumerable<(ICoin, int)> coins);

        /// <summary>
        ///     Removes the listed coins from this CurrencyPile
        /// </summary>
        /// <param name="coins">A collection of the coins to remove</param>
        /// <returns>True if the currency pile still has coins in it</returns>
        bool RemoveCoins(IEnumerable<Tuple<ICoin, int>> coins);

        /// <summary>
        ///     Removes the listed coins from this CurrencyPile
        /// </summary>
        /// <param name="coins">A collection of the coins to remove</param>
        /// <returns>True if the currency pile still has coins in it</returns>
        bool RemoveCoins(IEnumerable<KeyValuePair<ICoin, int>> coins);

        /// <summary>
        ///     Removes the listed coins from this CurrencyPile
        /// </summary>
        /// <param name="coins">A collection of the coins to remove</param>
        /// <returns>True if the currency pile still has coins in it</returns>
        bool RemoveCoins(IEnumerable<(ICoin, int)> coins);
    }
}