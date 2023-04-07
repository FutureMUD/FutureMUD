namespace MudSharp.PerceptionEngine {
    public interface IHandleOutput {
        public IOutputHandler OutputHandler { get; }

        public void Register(IOutputHandler handler);
    }
}