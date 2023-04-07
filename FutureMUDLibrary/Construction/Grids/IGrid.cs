using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Construction.Grids
{
    public interface IGrid : IFrameworkItem, ISaveable
    {
        IEnumerable<ICell> Locations { get; }
        void ExtendTo(ICell cell);
        void WithdrawFrom(ICell cell);

        void Delete();
        void LoadTimeInitialise();
        string GridType { get; }
        string Show(ICharacter actor);
    }
}
