using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Work.Projects
{
    public interface IActiveProject : IFrameworkItem, IFutureProgVariable
    {
        ICharacter CharacterOwner { get; }
        IProject ProjectDefinition { get; }
        IProjectPhase CurrentPhase { get; }
        IReadOnlyDictionary<IProjectLabourRequirement, double> LabourProgress { get; }
        IReadOnlyDictionary<IProjectMaterialRequirement, double> MaterialProgress { get; }
        void Cancel(ICharacter actor);
        bool FulfilLabour(IProjectLabourRequirement labour, double progress);
        void FulfilMaterial(IProjectMaterialRequirement material, double progress);
        IEnumerable<(ICharacter Character, IProjectLabourRequirement Labour)> ActiveLabour { get; }
        void Join(ICharacter actor, IProjectLabourRequirement labour);
        void Leave(ICharacter actor);
        string ProjectsCommandOutput(ICharacter actor);
        void DoProjectsTick();
        string ShowToPlayer(ICharacter actor);
    }
}
