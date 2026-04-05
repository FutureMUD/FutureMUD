using MudSharp.Framework;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Body.Disfigurements
{
    public interface IDisfigurement
    {
        IDisfigurementTemplate Template { get; }
        string ShortDescription { get; }
        string FullDescription { get; }
        SizeCategory Size { get; }
        IBodypart Bodypart { get; }
    }
}
