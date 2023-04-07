using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RandomNameProfile
    {
        public RandomNameProfile()
        {
            RandomNameProfilesDiceExpressions = new HashSet<RandomNameProfilesDiceExpressions>();
            RandomNameProfilesElements = new HashSet<RandomNameProfilesElements>();
        }

        public long Id { get; set; }
        public int Gender { get; set; }
        public long NameCultureId { get; set; }
        public string Name { get; set; }
        public long? UseForChargenSuggestionsProgId { get; set; }

        public virtual FutureProg UseForChargenSuggestionsProg { get; set; }

		public virtual NameCulture NameCulture { get; set; }
        public virtual ICollection<RandomNameProfilesDiceExpressions> RandomNameProfilesDiceExpressions { get; set; }
        public virtual ICollection<RandomNameProfilesElements> RandomNameProfilesElements { get; set; }
    }
}
