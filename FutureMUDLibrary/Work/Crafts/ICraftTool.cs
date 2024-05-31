using System;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Work.Crafts
{
    public interface ICraftTool : IFrameworkItem, ISaveable
    {
        DesiredItemState DesiredState { get; }
        Func<IGameItem, bool> EvaluateToolFunction(ICraft craft, int phase);
        bool IsTool(IGameItem item);
        void UseTool(IGameItem item, TimeSpan phaseLength, bool hasFailed);
        bool UseToolDuration { get; }
        double ToolFitness(IGameItem item);
        double ToolQualityWeight { get; }
        double PhaseLengthMultiplier(IGameItem item);
        DateTime OriginalAdditionTime { get; }
        bool BuildingCommand(ICharacter actor, StringStack command);
        void CreateNewRevision(Models.Craft dbcraft);
        bool IsValid();
        string WhyNotValid();
        string HowSeen(IPerceiver voyeur);
        bool RefersToItemProto(long id);
        bool RefersToTag(ITag tag);
        bool RefersToLiquid(ILiquid liquid);
    }
}
