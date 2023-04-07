using System.Collections.Generic;
using MudSharp.Form.Characteristics;

namespace MudSharp.GameItems.Interfaces {
    public interface IVariable : IGameItemComponent {
        IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions { get; }
        ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type);
        void SetCharacteristic(ICharacteristicDefinition definition, ICharacteristicValue value);
        void SetRandom(ICharacteristicDefinition definition);
        void ExpireDefinition(ICharacteristicDefinition definition);
        void ExpireValue(ICharacteristicValue value);
    }
}