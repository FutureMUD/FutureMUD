using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using System.Collections.Generic;

namespace MudSharp.Economy.Currency
{
    public enum RoundingMode
    {
        Truncate = 0,
        Round = 1,
        NoRounding = 2
    }

    public interface ICurrencyDescriptionPatternElement : IEditableItem, ISaveable
    {
        RoundingMode Rounding { get; }
        ICurrencyDivision TargetDivision { get; }
        bool ShowIfZero { get; }
        string Describe(decimal amount);
        string Pattern { get; }
        string PluraliseWord { get; }
        bool SpecialValuesOverridePattern { get; }
        string AlternatePattern { get; }
        int Order { get; set; }
        IReadOnlyDictionary<decimal, string> SpecialValues { get; }
        void Delete();
    }
}