using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Work.Projects
{
    public interface ILabourImpact : IFrameworkItem
    {
        string DescriptionForProjectsCommand { get; }

        void Delete();
        ILabourImpact Duplicate(IProjectLabourRequirement requirement);
        bool BuildingCommand(ICharacter actor, StringStack command, IProjectLabourRequirement requirement);
        (bool Truth, string Error) CanSubmit();
        string Show(ICharacter actor);
        string ShowFull(ICharacter actor);
        string ShowToPlayer(ICharacter actor);
        double MinimumHoursForImpactToKickIn { get; }
        bool Applies(ICharacter actor);
    }
}
