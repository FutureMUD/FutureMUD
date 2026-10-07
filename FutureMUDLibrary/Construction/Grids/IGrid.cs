using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Construction.Grids
{
    public interface IGrid : IFrameworkItem, ISaveable, IProgVariable
    {
        IEnumerable<IRoom> Locations { get; }
        void ExtendTo(IRoom room);
        void WithdrawFrom(IRoom room);

        void Delete();
        void LoadTimeInitialise();
        string GridType { get; }
        string Show(ICharacter actor);
    }
}
