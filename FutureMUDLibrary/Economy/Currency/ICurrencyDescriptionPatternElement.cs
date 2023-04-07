namespace MudSharp.Economy.Currency {
    public enum RoundingMode {
        Truncate = 0,
        Round = 1,
        NoRounding = 2
    }

    public interface ICurrencyDescriptionPatternElement {
        RoundingMode Rounding { get; }
        ICurrencyDivision TargetDivision { get; }
        bool ShowIfZero { get; }
        string Describe(decimal amount);
    }
}