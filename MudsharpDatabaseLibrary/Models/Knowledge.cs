using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Knowledge
    {
        public Knowledge()
        {
            CharacterKnowledges = new HashSet<CharacterKnowledge>();
            Scripts = new HashSet<Script>();
            SurgicalProcedures = new HashSet<SurgicalProcedure>();
            KnowledgesCosts = new HashSet<KnowledgesCosts>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public string Type { get; set; }
        public string Subtype { get; set; }
        public int LearnableType { get; set; }
        public int LearnDifficulty { get; set; }
        public int TeachDifficulty { get; set; }
        public int LearningSessionsRequired { get; set; }
        public long? CanAcquireProgId { get; set; }
        public long? CanLearnProgId { get; set; }

        public virtual FutureProg CanAcquireProg { get; set; }
        public virtual FutureProg CanLearnProg { get; set; }
        public virtual ICollection<CharacterKnowledge> CharacterKnowledges { get; set; }
        public virtual ICollection<Script> Scripts { get; set; }
        public virtual ICollection<SurgicalProcedure> SurgicalProcedures { get; set; }
        public virtual ICollection<KnowledgesCosts> KnowledgesCosts { get; set; }
    }
}
