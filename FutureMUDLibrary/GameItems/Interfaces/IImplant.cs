using MudSharp.Body;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IImplant : IConsumePower
    {
        double FunctionFactor { get; }
        bool External { get; }
        string ExternalDescription { get; }

        IBodyPrototype TargetBody { get; }
        IBodypart TargetBodypart { get; set; }
        IBody InstalledBody { get; }
        void InstallImplant(IBody body);
        void RemoveImplant();
        double ImplantSpaceOccupied { get; }
        Difficulty InstallDifficulty { get; }
    }
}
