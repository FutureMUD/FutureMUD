using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts;

namespace MudSharp.GameItems.Interfaces
{
    public interface IActiveCraftGameItemComponent : IGameItemComponent
    {
        Dictionary<ICraftInput, (IPerceivable Input, ICraftInputData Data)> ConsumedInputs { get; }
        Dictionary<ICraftProduct, ICraftProductData> ProducedProducts { get; }
        Dictionary<ICraftTool, (ItemQuality Quality, double Weight)> UsedToolQualities { get; }
        Outcome QualityCheckOutcome { get; set; }
        ICraft Craft { get; set; }
        int Phase { get; set; }
        bool HasFailed { get; set; }
        Outcome CheckOutcome { get; set; }
        bool HasFinished { get; }
        void CraftWasInterrupted();
        void ReleaseItems(ICell location, RoomLayer layer);
        (bool Success, bool Finished) DoNextPhase(IActiveCraftEffect effect);
    }
}
