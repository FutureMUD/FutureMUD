using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.GameItems.Inventory {
    public interface IWearProfile : IFrameworkItem, ISaveable {
        IBodyPrototype DesignedBody { get; }

        string Description { get; }

        string WearStringInventory { get; }

        string WearAction1st { get; }

        string WearAction3rd { get; }

        string WearAffix { get; }

        string Type { get; }
        bool RequireContainerIsEmpty { get; }
        Dictionary<IWear, IWearlocProfile> AllProfiles { get; }

        Dictionary<IWear, IWearlocProfile> Profile(IBody body);
        string ShowTo(ICharacter actor);
        void BuildingCommand(ICharacter actor, StringStack command);
        bool CompatibleWith(IWearProfile otherProfile);
        IWearProfile Clone(string newName);

    }
}