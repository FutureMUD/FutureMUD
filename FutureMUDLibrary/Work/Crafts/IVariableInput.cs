using MudSharp.Form.Characteristics;

namespace MudSharp.Work.Crafts
{
    public interface IVariableInput : ICraftInput
    {
        bool DeterminesVariable(ICharacteristicDefinition definition);
        ICharacteristicValue GetValueForVariable(ICharacteristicDefinition definition, ICraftInputData data);
    }
}
