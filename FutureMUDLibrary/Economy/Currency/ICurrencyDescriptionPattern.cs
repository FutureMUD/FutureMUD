using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using System.Collections.Generic;
using MudSharp.Framework.Save;

namespace MudSharp.Economy.Currency;
public enum CurrencyDescriptionPatternType
{
    Casual = 0,
    Short = 1,
    ShortDecimal = 2,
    Long = 3,
    Wordy = 4
}

public interface ICurrencyDescriptionPattern : IEditableItem, ISaveable
{
    CurrencyDescriptionPatternType Type { get; }
    int Order { get; set; }
    IFutureProg ApplicabilityProg { get; }
    string Describe(decimal value);
    IEnumerable<ICurrencyDescriptionPatternElement> Elements { get; }
    string NegativeValuePrefix { get; }
    void DivisionDeleted(ICurrencyDivision division);
    void Delete();
    void ReorderElement(ICurrencyDescriptionPatternElement element, int targetIndex);
}