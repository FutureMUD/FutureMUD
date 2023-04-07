using System;
using MudSharp.Body;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.RPG.Checks;

namespace MudSharp.Health {
    public enum InfectionType {
        Simple,
        Necrotic,
        Infectious,
        Gangrene,
        FungalGrowth
    }

    //A general indication about how advanced the infection is
    //Stage names kept generic since the details of what happens in each stage
    //should be infection-type-specific.
    public enum InfectionStage
    {
        StageZero,      // Healed
        StageOne,       
        StageTwo,
        StageThree,
        StageFour,      // Spreading
        StageFive,
        StageSix        // Damaging
    }

    public interface IInfection : IFrameworkItem, ISaveable {
        InfectionType InfectionType { get; }
        double Intensity { get; set; }
        Difficulty VirulenceDifficulty { get; }
        double Pain { get; }
        double Immunity { get; }
        double Virulence { get; }
        IBodypart Bodypart { get; }
        IWound Wound { get; }
        void InfectionTick();
        void Spread(Outcome outcome);
        bool InfectionHealed();
        void Delete();
        string WoundTag(WoundExaminationType examType, Outcome outcome);

        bool InfectionIsDamaging();
        bool InfectionCanSpread();
        IDamage GetInfectionDamage();
    }

    public static class InfectionExtensions {
        public static string Describe(this InfectionType infection) {
            switch (infection) {
                case InfectionType.Simple:
	                return "a simple infection";
                case InfectionType.Infectious:
	                return "a contagious infection";
                case InfectionType.Necrotic:
                    return "a necrotic infection";
                case InfectionType.Gangrene:
                    return "gangrene";
                case InfectionType.FungalGrowth:
                    return "fungal growth";
                default:
                    throw new NotImplementedException("InfectionType does not have a string for Describe.");
            }
        }
    }
}