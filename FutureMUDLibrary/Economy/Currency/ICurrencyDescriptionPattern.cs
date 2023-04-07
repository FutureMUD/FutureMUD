using MudSharp.FutureProg;

namespace MudSharp.Economy.Currency {
    public enum CurrencyDescriptionPatternType {
        Casual = 0,
        Short = 1,
        ShortDecimal = 2,
        Long = 3,
        Wordy = 4
    }

    public interface ICurrencyDescriptionPattern {
        CurrencyDescriptionPatternType Type { get; }
        int Order { get; }
        IFutureProg ApplicabilityProg { get; }
        string Describe(decimal value);
    }
}