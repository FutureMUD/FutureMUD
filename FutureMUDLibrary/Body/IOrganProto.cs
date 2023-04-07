namespace MudSharp.Body {
    public interface IOrganProto : IBodypart {
        double ImplantSpaceOccupied { get; }
        double RelativeInfectability { get; }
        double HypoxiaDamagePerTick { get; }
        bool RequiresSpinalConnection { get; }
        double OrganFunctionFactor(IBody body);

        void HandleChangedOrganFunction(IBody body, double oldFunctionFactor, double newFunctionFactor);
    }
}