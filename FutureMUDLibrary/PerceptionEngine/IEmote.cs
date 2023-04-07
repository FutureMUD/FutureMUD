using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.PerceptionEngine
{
    
    public enum PermitLanguageOptions {
        IgnoreLanguage,
        PermitLanguage,
        LanguageIsGasping,
        LanguageIsMuffling,
        LanguageIsChoking,
        LanguageIsClicking,
        LanguageIsError,
        LanguageIsBuzzing
    }


    public interface IEmote
    {
        bool Valid { get; }
        string ErrorMessage { get; }
        string RawText { get; }
        bool FixedFormat { get; }
        IPerceiver Source { get; }
        IEnumerable<IPerceivable> Targets { get; }
        bool ForcedSourceInclusion { get; }
        XElement SaveToXml();
        string ParseFor(IPerceiver perceiver, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None);
    }
}