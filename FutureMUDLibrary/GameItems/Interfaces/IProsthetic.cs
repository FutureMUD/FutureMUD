using System.Collections.Generic;
using MudSharp.Body;

namespace MudSharp.GameItems.Interfaces {
    public interface IProsthetic : IGameItemComponent {
        bool Obvious { get; }
        bool Functional { get; }
        IBodyPrototype TargetBody { get; }
        IBodypart TargetBodypart { get; }

        IEnumerable<IBodypart> IncludedParts { get; }
        IBody InstalledBody { get; }
        void InstallProsthetic(IBody body);
        void RemoveProsthetic();
    }
}