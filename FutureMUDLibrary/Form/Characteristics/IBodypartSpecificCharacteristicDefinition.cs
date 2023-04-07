using MudSharp.Body;
using MudSharp.Form.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Form.Characteristics
{
    public interface IBodypartSpecificCharacteristicDefinition : ICharacteristicDefinition
    {
        IBodypartShape TargetShape { get; }
        int OrdinaryCount { get; }
    }
}
