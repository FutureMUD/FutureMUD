namespace MudSharp.RPG.Merits.Interfaces {
    public interface INeedRateChangingMerit : ICharacterMerit {
        double HungerMultiplier { get; }
        double ThirstMultiplier { get; }
        double DrunkennessMultiplier { get; }
    }
}