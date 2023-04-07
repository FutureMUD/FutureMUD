namespace MudSharp.FutureProg {
    public interface IVariableSpace {
        IFutureProgVariable GetVariable(string variable);
        bool HasVariable(string variable);
        void SetVariable(string variable, IFutureProgVariable value);
    }
}