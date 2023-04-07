using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Form.Characteristics {
    public interface ICharacteristicProfile : IFrameworkItem, ISaveable
    {
	    ICharacteristicProfile Clone(string newName);
        IEnumerable<ICharacteristicValue> Values { get; }
        ICharacteristicDefinition TargetDefinition { get; }
        string Description { get; }
        string Type { get; }
        bool ContainsCharacteristic(ICharacteristicValue value);
        void ExpireCharacteristic(ICharacteristicValue value);
        ICharacteristicValue GetCharacteristic(string value);
        ICharacteristicValue GetRandomCharacteristic();
        ICharacteristicValue GetRandomCharacteristic(ICharacterTemplate template);
        ICharacteristicValue GetRandomCharacteristic(ICharacter character);
        bool IsProfileFor(ICharacteristicDefinition definition);

        void BuildingCommand(ICharacter actor, StringStack command);
        string Show(ICharacter actor);
    }
}