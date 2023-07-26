using MudSharp.Celestial;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Climate
{
    public interface ISeason : IFrameworkItem
    {
        string DisplayName { get; }
        string SeasonGroup { get; }
        int CelestialDayOnset { get; }
        ICelestialObject Celestial { get; }
    }
}
