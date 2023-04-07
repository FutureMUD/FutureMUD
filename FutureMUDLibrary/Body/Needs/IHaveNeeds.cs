namespace MudSharp.Body.Needs {
    public interface IHaveNeeds {
        INeedsModel NeedsModel { get; }
        NeedsResult FulfilNeeds(INeedFulfiller fulfiller, bool ignoreDelays = false);
        void DescribeNeedsResult(NeedsResult result);
        void NeedsHeartbeat();
        void StartNeedsHeartbeat();
    }
}