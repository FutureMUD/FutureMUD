namespace MudSharp.Effects.Interfaces {
    public interface ICommandDelay : IEffectSubtype {
        bool IsDelayed(string whichCommand);
        string Message { get; }
    }
}
