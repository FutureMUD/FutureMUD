namespace MudSharp.FutureProg {
    public interface IVariableSpace {
        IProgVariable GetVariable(string variable);
        bool HasVariable(string variable);
        void SetVariable(string variable, IProgVariable value);
    }
}