using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Community {
    public interface IPaygrade : IFrameworkItem, ISaveable, IProgVariable {
        string Abbreviation { get; set; }
        ICurrency PayCurrency { get; set; }
        decimal PayAmount { get; set; }
        IClan Clan { get; set; }
        void SetName(string name);
    }
}