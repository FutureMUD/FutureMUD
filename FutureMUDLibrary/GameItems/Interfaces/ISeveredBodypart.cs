using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Health;

namespace MudSharp.GameItems.Interfaces {
    public interface ISeveredBodypart : IContainer, IButcherable {
        long OriginalCharacterId { get; }

        double DecayPoints { get; set; }

        IBodypart RootPart { get; }
        double EatenWeight { get; set; }
        double RemainingEdibleWeight { get; }
        IEnumerable<IBone> Bones { get; set; }
        IEnumerable<IOrganProto> Organs { get; set; }
        IEnumerable<IWound> Wounds { get; set; }
        IEnumerable<ITattoo> Tattoos { get; set; }
        IEnumerable<IGameItem> Implants { get; set; }
        void SeveredBodypartWasInstalledInABody();
    }
}