using System;
using System.Collections.Generic;
using MudSharp.Framework.Save;

namespace MudSharp.FutureProg {
    /// <summary>
    ///     The Variable Register tracks persistent prog variables for types and type instances
    /// </summary>
    public interface IVariableRegister : ISaveable {
        IFutureProgVariable GetValue(IFutureProgVariable item, string variable);
        IFutureProgVariable GetDefaultValue(FutureProgVariableTypes type, string variable);
        bool SetValue(IFutureProgVariable item, string variable, IFutureProgVariable value);
        void SetDefaultValue(FutureProgVariableTypes item, string variable, IFutureProgVariable value);
        FutureProgVariableTypes GetType(FutureProgVariableTypes type, string variable);

        bool RegisterVariable(FutureProgVariableTypes ownerType, FutureProgVariableTypes variableType, string variable, object defaultValue = null);
        bool DeregisterVariable(FutureProgVariableTypes ownerType, string variable);

        IEnumerable<Tuple<string, IFutureProgVariable>> AllVariables(IFutureProgVariable item);
        IEnumerable<Tuple<string, FutureProgVariableTypes>> AllVariables(FutureProgVariableTypes type);

        bool ValidValueType(FutureProgVariableTypes type, string value);
        bool ResetValue(IFutureProgVariable item, string variable);
        bool IsRegistered(FutureProgVariableTypes type, string variable);
    }
}