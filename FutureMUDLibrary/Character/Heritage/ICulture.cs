using System.Collections.Generic;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.Character.Heritage {
    public interface ICulture : IEditableItem, IFutureProgVariable {
        string Description { get; }
        IEnumerable<INameCulture> NameCultures { get; }
        INameCulture NameCultureForGender(Gender gender);
        IFutureProg SkillStartingValueProg { get; }
        ICalendar PrimaryCalendar { get; }
        IFutureProg AvailabilityProg { get; }
        int ResourceCost(IChargenResource resource);
        int ResourceRequirement(IChargenResource resource);
        bool ChargenAvailable(ICharacterTemplate template);

        string PersonWord(Gender gender);
        IEnumerable<IChargenAdvice> ChargenAdvices { get; }
        bool ToggleAdvice(IChargenAdvice advice);
        double TolerableTemperatureFloorEffect { get; }
        double TolerableTemperatureCeilingEffect { get; }
    }
}