using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body.Grouping {
    public class BodypartGroupResult {
        public string Description;

        public BodypartGroupResult(bool ismatch, int score, string description = null,
            IEnumerable<IBodypart> matches = null, IEnumerable<IBodypart> remains = null) {
            Description = description;
            MatchScore = score;
            Matches = matches?.ToList();
            Remains = remains?.ToList();
            IsMatch = ismatch;
        }

        public List<IBodypart> Matches { get; set; }
        public List<IBodypart> Remains { get; set; }

        public int MatchScore { get; set; }
        public bool IsMatch { get; set; }
    }
}