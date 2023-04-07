using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.GameItems.Interfaces
{
    public interface IInsulating : IGameItemComponent
    {
        double InsulatingDegrees { get; }
        double ReflectingDegrees { get; }
    }
}
