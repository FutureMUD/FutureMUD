using MudSharp.Celestial;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Climate
{
    public interface ISeason : IEditableItem
    {
        string DisplayName { get; }
        string SeasonGroup { get; }
        int CelestialDayOnset { get; }
        ICelestialObject Celestial { get; }
        ISeason Clone(string name);
    }
}
