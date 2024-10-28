using System;
using System.Collections.Generic;
using MudSharp.Framework.Save;

namespace MudSharp.FutureProg {
    /// <summary>
    ///     The Variable Register tracks persistent prog variables for types and type instances
    /// </summary>
    public interface IVariableRegister : ISaveable {
        IProgVariable GetValue(IProgVariable item, string variable);
        IProgVariable GetDefaultValue(ProgVariableTypes type, string variable);
        bool SetValue(IProgVariable item, string variable, IProgVariable value);
        void SetDefaultValue(ProgVariableTypes item, string variable, IProgVariable value);
        ProgVariableTypes GetType(ProgVariableTypes type, string variable);

        bool RegisterVariable(ProgVariableTypes ownerType, ProgVariableTypes variableType, string variable, object defaultValue = null);
        bool DeregisterVariable(ProgVariableTypes ownerType, string variable);

        IEnumerable<Tuple<string, IProgVariable>> AllVariables(IProgVariable item);
        IEnumerable<Tuple<string, ProgVariableTypes>> AllVariables(ProgVariableTypes type);

        bool ValidValueType(ProgVariableTypes type, string value);
        bool ResetValue(IProgVariable item, string variable);
        bool IsRegistered(ProgVariableTypes type, string variable);
    }
}