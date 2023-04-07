using MudSharp.Framework;

namespace MudSharp.Economy.Currency {
    public interface ICoin : IFrameworkItem {
        string ShortDescription { get; }
        string FullDescription { get; }
        double Weight { get; }
        decimal Value { get; }
        string GeneralForm { get; }
        string PluralWord { get; }
    }
}