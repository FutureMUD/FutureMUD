using MudSharp.Framework;
using System.Collections.Generic;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.Economy.Currency {
    public enum RoundingMode {
        Truncate = 0,
        Round = 1,
        NoRounding = 2
    }

    public interface ICurrencyDescriptionPatternElement : IEditableItem, ISaveable {
        RoundingMode Rounding { get; }
        ICurrencyDivision TargetDivision { get; }
        bool ShowIfZero { get; }
        string Describe(decimal amount);
        string Pattern { get; }
        string PluraliseWord { get; }
        bool SpecialValuesOverridePattern { get; }
        string AlternatePattern { get; }
        int Order { get; }
        IReadOnlyDictionary<decimal, string> SpecialValues { get; }
        void Delete();
    }
}