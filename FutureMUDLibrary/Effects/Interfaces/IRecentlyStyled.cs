using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Form.Characteristics;

namespace MudSharp.Effects.Interfaces
{
    public interface IRecentlyStyled : IEffectSubtype
    {
        ICharacteristicDefinition CharacteristicType {
            get;
        }
    }
}
