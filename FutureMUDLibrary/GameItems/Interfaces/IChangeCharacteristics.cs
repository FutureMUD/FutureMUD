using MudSharp.Form.Characteristics;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces {
    public interface IChangeCharacteristics : IGameItemComponent {
        bool ChangesCharacteristic(ICharacteristicDefinition type);
        string DescribeCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur, bool basic = false);
        ICharacteristicValue GetCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur);
    }
}