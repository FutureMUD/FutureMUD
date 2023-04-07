using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;

namespace MudSharp.Work.Projects
{
    public interface IProjectMaterialRequirement : IFrameworkItem
    {
        bool IsMandatoryForProjectCompletion { get; }
        bool ItemCounts(IGameItem item);
        void SupplyItem(ICharacter actor, IGameItem item, IActiveProject project);
        void PeekSupplyItem(ICharacter actor, IGameItem item, IActiveProject project);
        string DescribeQuantity(ICharacter actor);
        string Description { get; }
        double QuantityRequired { get; }
        IInventoryPlan GetPlanForCharacter(ICharacter actor);
        void Delete();
        IProjectMaterialRequirement Duplicate(IProjectPhase newPhase);
        bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase);
        (bool Truth, string Error) CanSubmit();
        string Show(ICharacter actor);
        string ShowToPlayer(ICharacter actor);
    }
}
