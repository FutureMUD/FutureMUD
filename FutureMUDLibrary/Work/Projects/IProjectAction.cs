using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Work.Projects
{
    public interface IProjectAction : IFrameworkItem
    {
        string Description { get; }
        void CompleteAction(IActiveProject project);
        void Delete();
        IProjectAction Duplicate(IProjectPhase newPhase);
        bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase);
        (bool Truth, string Error) CanSubmit();
        string Show(ICharacter actor);
        string ShowToPlayer(ICharacter actor);
        int SortOrder { get; }
    }
}
