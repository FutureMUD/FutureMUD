using MudSharp.Character;
using MudSharp.Strategies.BodyStratagies;

namespace MudSharp.Body {
    public partial interface IBody : ICommunicate {
        IBodyCommunicationStrategy Communications { get; }
    }
}