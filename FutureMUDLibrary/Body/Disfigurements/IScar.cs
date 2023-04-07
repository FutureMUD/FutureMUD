using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Body.Disfigurements
{
    public enum ScarFreshness
    {
        Fresh,
        Recent,
        Old
    }

    public interface IScar : IDisfigurement
    {
        IScarTemplate ScarTemplate { get; }
        ScarFreshness Freshness { get; }
        MudDateTime TimeOfScarring { get; }
        int Distinctiveness { get; }
        XElement SaveToXml();
    }
}
