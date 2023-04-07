using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Work.Projects
{
    #nullable enable
    public interface IProjectLabourRequirement : IFrameworkItem, IHaveFuturemud
    {
        bool IsMandatoryForProjectCompletion { get; }
        bool CharacterIsQualified(ICharacter actor);
        double HourlyProgress(ICharacter actor, bool previewOnly = false);
        double ProgressMultiplierForOtherLabourPerPercentageComplete(IProjectLabourRequirement other, IActiveProject project);
        double TotalProgressRequired { get; }
        double TotalProgressRequiredForDisplay { get; }
        ITraitDefinition? RequiredTrait { get; }
        string Description { get; }
        int MaximumSimultaneousWorkers { get; }
        void Delete();
        IProjectLabourRequirement Duplicate(IProjectPhase newPhase);
        bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase);
        (bool Truth, string Error) CanSubmit();
        string Show(ICharacter actor);
        string ShowToPlayer(ICharacter actor);
        IEnumerable<ILabourImpact> LabourImpacts { get; }
        double HoursRemaining(IActiveProject project);
    }
}
