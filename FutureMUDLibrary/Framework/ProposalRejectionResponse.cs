using System.Collections.Generic;

namespace MudSharp.Framework
{
    public class ProposalRejectionResponse
    {
        public bool IsRejected { get; set; } = false;
        private readonly List<string> _reasons = new();
        public IEnumerable<string> Reasons => _reasons;
        public void RejectWithReason(string reason)
        {
            IsRejected = true;
            _reasons.Add(reason);
        }
    }
}