using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Framework;

namespace MudSharp.RPG.Checks {
    public enum Outcome {
        None = 0,
        NotTested,
        MajorFail,
        Fail,
        MinorFail,
        MinorPass,
        Pass,
        MajorPass
    }

    public enum Difficulty {
        Automatic = 0,
        Trivial = 1,
        ExtremelyEasy = 2,
        VeryEasy = 3,
        Easy = 4,
        Normal = 5,
        Hard = 6,
        VeryHard = 7,
        ExtremelyHard = 8,
        Insane = 9,
        Impossible = 10
    }

    public interface ICheck : IFrameworkItem, IHaveFuturemud {
        CheckType Type { get; }
		bool ImproveTraits { get; }

		bool CanTraitBranchIfMissing { get;  }

		FailIfTraitMissingType FailIfTraitMissing { get;}

		/// <summary>
		///     A TraitExpression representing the Target Number of the check
		/// </summary>
		ITraitExpression TargetNumberExpression { get;  }

		/// <summary>
		///     Name of the Check Template that this check uses
		/// </summary>
		string CheckTemplateName { get; }

		double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
                            IPerceivable target = null, double externalBonus = 0.0, params (string Parameter, object value)[] customParameters);

        CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, IPerceivable target = null,
	        IUseTrait tool = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
	        params (string Parameter, object value)[] customParameters);

        CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
	        IPerceivable target = null, double externalBonus = 0.0,
	        TraitUseType traitUseType = TraitUseType.Practical,
	        params (string Parameter, object value)[] customParameters);

        Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee, Difficulty difficulty1,
	        Difficulty difficulty2, IPerceivable target = null, ITraitDefinition tool = null,
	        double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
	        params (string Parameter, object value)[] customParameters);

        Difficulty MaximumDifficultyForImprovement { get; }

        Dictionary<Difficulty, CheckOutcome> CheckAgainstAllDifficulties(IPerceivableHaveTraits checkee,
	        Difficulty referenceDifficulty, ITraitDefinition trait,
	        IPerceivable target = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
	        params (string Parameter, object value)[] customParameters);

        bool WouldBeAbjectFailure(IPerceivableHaveTraits checkee, ITraitDefinition trait = null);
    }

    public class CheckOutcome
    {
        public static CheckOutcome NotTested(CheckType type)
        {
            return new CheckOutcome
            {
                CheckType = type,
                Outcome = Outcome.NotTested,
                AcquiredTraits = Enumerable.Empty<ITraitDefinition>(),
                ImprovedTraits = Enumerable.Empty<ITraitDefinition>(),
                ActiveBonuses = Enumerable.Empty<Tuple<string, double>>(),
                Rolls = Enumerable.Empty<double>()
            };
        }

        public static Dictionary<Difficulty,CheckOutcome> NotTestedAllDifficulties(CheckType type)
        {
            return Enum.GetValues<Difficulty>().ToDictionary(x => x, x => NotTested(type));
        }

        public bool IsAbjectFailure { get; init; }
        public Outcome Outcome { get; init; }
        public CheckType CheckType { get; set; }
        public IEnumerable<ITraitDefinition> AcquiredTraits { get; set; }
        public IEnumerable<ITraitDefinition> ImprovedTraits { get; set; }
        public IEnumerable<Tuple<string, double>> ActiveBonuses { get; set; }
        public IEnumerable<double> Rolls { get; set; }

        public double FinalBonus { get; set; }
        public Difficulty OriginalDifficulty { get; set; }
        public double OriginalDifficultyModifier { get; set; }
        public Difficulty FinalDifficulty { get; set; }
        public double FinalDifficultyModifier { get; set; }
        public int Burden { get; set; }
        public double TargetNumber { get; set; }

        public string CheckTemplateName { get; set; }
    
        public static implicit operator Outcome(CheckOutcome co)
        {
            return co?.Outcome ?? Outcome.None;
        }
    }
}