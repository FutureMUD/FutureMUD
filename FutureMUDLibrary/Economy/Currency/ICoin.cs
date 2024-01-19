using MudSharp.Framework;
using MudSharp.Framework.Revision;

namespace MudSharp.Economy.Currency {
    public interface ICoin : IFrameworkItem, IEditableItem {
	    ICurrency Currency { get; }
		string ShortDescription { get; }
        string FullDescription { get; }
        double Weight { get; }
        decimal Value { get; }
        string GeneralForm { get; }
        string PluralWord { get; }
        bool UseForChange { get; }
    }
}