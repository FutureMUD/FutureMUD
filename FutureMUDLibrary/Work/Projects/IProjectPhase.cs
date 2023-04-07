using System.Collections.Generic;
using MudSharp.Framework;

namespace MudSharp.Work.Projects
{
    public interface IProjectPhase : IFrameworkItem
    {
        int PhaseNumber { get; set; }
        IEnumerable<IProjectLabourRequirement> LabourRequirements { get; }
        IEnumerable<IProjectMaterialRequirement> MaterialRequirements { get; }
        string Description { get; set; }
        void Delete();
        IProjectPhase Duplicate(IProject newProject);
        void AddLabour(IProjectLabourRequirement labour);
        void RemoveLabour(IProjectLabourRequirement labour);

        void AddMaterial(IProjectMaterialRequirement material);
        void RemoveMaterial(IProjectMaterialRequirement material);
        (bool Truth, string Error) CanSubmit();
        IEnumerable<IProjectAction> CompletionActions { get; }
        void AddAction(IProjectAction action);
        void RemoveAction(IProjectAction action);
    }
}
