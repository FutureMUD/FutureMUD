#nullable enable

using MudSharp.Magic;

namespace MudSharp.Effects.Interfaces;

public interface IMagicPortalTopologyExit : IMagicPortalExit
{
	IMagicPortalNetwork Network { get; }
	IMagicPortalLink Link { get; }
	IMagicPortalEndpoint SourceEndpoint { get; }
	IMagicPortalEndpoint DestinationEndpoint { get; }
}
