namespace MudSharp.Economy.Currency {
    /// <summary>
    ///     Any object implementing IHaveCurrency has a current ICurrency
    /// </summary>
    public interface IHaveCurrency {
        ICurrency Currency { get; set; }
    }
}