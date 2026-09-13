using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace MudSharp.Work.Foraging
{
    public interface IForagableProfile : IEditableRevisableItem
    {
        /// <summary>World-local revision of yield settings edited within the same builder revision.</summary>
        long YieldDefinitionRevision { get; }
        IReadOnlyDictionary<string, double> MaximumYieldPoints { get; }
        IReadOnlyDictionary<string, double> HourlyYieldPoints { get; }
        IEnumerable<IForagable> Foragables { get; }
        IForagable GetForageResult(ICharacter character, IReadOnlyDictionary<Difficulty, CheckOutcome> forageOutcome, string foragableType);
    }
}
