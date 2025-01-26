using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Knowledge;

namespace MudSharp.Communication.Language {
    public interface IScript : IEditableItem, IProgVariable {
        IKnowledge ScriptKnowledge { get; }
        IEnumerable<ILanguage> DesignedLanguages { get; }
        string UnknownScriptDescription { get; }
        string KnownScriptDescription { get; }
        double DocumentLengthModifier { get; }
        double InkUseModifier { get; }
    }
}
