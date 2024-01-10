using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Economy.Currency {
    public interface ICoin : IFrameworkItem, IEditableItem {
        string ShortDescription { get; }
        string FullDescription { get; }
        double Weight { get; }
        decimal Value { get; }
        string GeneralForm { get; }
        string PluralWord { get; }
    }
}