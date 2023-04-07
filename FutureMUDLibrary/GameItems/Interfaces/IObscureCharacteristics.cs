using MudSharp.Form.Characteristics;
using MudSharp.Framework;

namespace MudSharp.GameItems.Interfaces {
    public interface IObscureCharacteristics : IGameItemComponent {
        string RemovalEcho { get; }
        bool ObscuresCharacteristic(ICharacteristicDefinition type);
        string DescribeCharacteristic(ICharacteristicDefinition type, IPerceiver voyeur);
    }
}