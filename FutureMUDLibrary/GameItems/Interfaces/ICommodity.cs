using MudSharp.Form.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces
{
    #nullable enable
    public interface ICommodity : IGameItemComponent
    {
        ISolid Material { get; set; }
        double Weight { get; set; }
        ITag? Tag { get; set; }
        bool UseIndirectQuantityDescription { get; set; }

	}
    #nullable restore
}
