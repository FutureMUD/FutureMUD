using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;

namespace MudSharp.Work.Projects
{
    public interface IProject : IEditableRevisableItem
    {
        bool AppearInJobsList { get; }
        string Tagline { get; }
        IFutureProg OnStartProg { get; }
        IFutureProg OnCancelProg { get; }
        IFutureProg OnFinishProg { get; }
        IEnumerable<string> ProjectCatalogueColumns(ICharacter actor);
        bool AppearInProjectList(ICharacter actor);
        bool CanInitiateProject(ICharacter actor);
        string WhyCannotInitiateProject(ICharacter actor);
        string ShowToPlayer(ICharacter actor);
        void InitiateProject(ICharacter actor);
        bool CanCancelProject(ICharacter actor, IActiveProject local);
        IActiveProject LoadActiveProject(Models.ActiveProject project);

        IEnumerable<IProjectPhase> Phases { get; }
    }
}
