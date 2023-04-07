using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp.Models
{
    public class ChargenScreenStoryboard
    {
        public ChargenScreenStoryboard()
        {
            DependentStages = new HashSet<ChargenScreenStoryboardDependentStage>();
        }

        public long Id { get; set; }
        public string ChargenType { get; set; }
        public int ChargenStage { get; set; }
        public int Order { get; set; }
        public string StageDefinition { get; set; }
        public int NextStage { get; set; }
        public virtual ICollection<ChargenScreenStoryboardDependentStage> DependentStages { get; set; }
    }
}
