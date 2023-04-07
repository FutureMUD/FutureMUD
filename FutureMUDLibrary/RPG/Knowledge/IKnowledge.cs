using System;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Knowledge {
    [Flags]
    public enum LearnableType {
        NotLearnable = 0,
        LearnableAtSkillUp = 1 << 0,
        LearnableAtChargen = 1 << 1,
        LearnableFromTeacher = 1 << 2
    }

    public interface IKnowledge : IEditableItem, IFutureProgVariable {
        string Description { get; set; }
        string LongDescription { get; set; }
        string KnowledgeType { get; set; }
        string KnowledgeSubtype { get; set; }

        /// <summary>
        ///     Returns bool, takes a toon and a trait definition
        /// </summary>
        IFutureProg CanPickChargenProg { get; set; }

        /// <summary>
        ///     Returns bool, takes a character
        /// </summary>
        IFutureProg CanLearnProg { get; set; }

        LearnableType Learnable { get; set; }
        Difficulty TeachDifficulty { get; set; }
        Difficulty LearnDifficulty { get; set; }
        int LearnerSessionsRequired { get; set; }
        int ResourceCost(IChargenResource resource);
    }
}