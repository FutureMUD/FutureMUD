using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;

namespace MudSharp.Form.Characteristics
{
    public interface IStyleCharacterCharacteristics
    {
        IEnumerable<IGrowableCharacteristicValue> PossibleStyles(ICharacteristicDefinition definition);
        string WhyCannotStyle(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value);
        bool CanStyle(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value);
        bool Style(ICharacter target, ICharacteristicDefinition definition, IGrowableCharacteristicValue value, bool force = false);
    }
}
