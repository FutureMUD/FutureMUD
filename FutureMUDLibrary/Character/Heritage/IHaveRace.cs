namespace MudSharp.Character.Heritage {
    public interface IHaveRace {
        IRace Race { get; }
        IEthnicity Ethnicity { get; }
        int AgeInYears { get; }
        AgeCategory AgeCategory { get; }
    }
}