using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;

namespace MudSharp.Work.Foraging {
    public interface IForagableProfile : IEditableRevisableItem {
        IReadOnlyDictionary<string, double> MaximumYieldPoints { get; }
        IReadOnlyDictionary<string, double> HourlyYieldPoints { get; }
        IEnumerable<IForagable> Foragables { get; }
        IForagable GetForageResult(ICharacter character, IReadOnlyDictionary<Difficulty,CheckOutcome> forageOutcome, string foragableType);
    }
}