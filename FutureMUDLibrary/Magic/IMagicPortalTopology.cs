#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using System.Collections.Generic;

namespace MudSharp.Magic;

public interface IMagicPortalNetwork : IEditableItem, ISaveable
{
	IMagicSchool? School { get; }
	bool IsActive { get; }
	bool AllowCrossZone { get; }
	string Verb { get; }
	string OutboundKeyword { get; }
	string InboundKeyword { get; }
	string OutboundTarget { get; }
	string InboundTarget { get; }
	string OutboundDescription { get; }
	string InboundDescription { get; }
	double TimeMultiplier { get; }
	IEnumerable<IMagicPortalEndpoint> Endpoints { get; }
	IEnumerable<IMagicPortalLink> Links { get; }
}

public interface IMagicPortalEndpoint : IFrameworkItem, IHaveFuturemud
{
	IMagicPortalNetwork Network { get; }
	string Key { get; }
	MagicPortalEndpointType EndpointType { get; }
	long? CellId { get; }
	long? GameItemId { get; }
	bool IsActive { get; }
	ICell? CurrentCell { get; }
	string WhyInvalid { get; }
}

public interface IMagicPortalLink : IFrameworkItem, IHaveFuturemud
{
	IMagicPortalNetwork Network { get; }
	IMagicPortalEndpoint SourceEndpoint { get; }
	IMagicPortalEndpoint DestinationEndpoint { get; }
	bool IsActive { get; }
	string WhyInvalid { get; }
}

public interface IMagicPortalTopologyService
{
	IEnumerable<IMagicPortalTopologyExit> MaterializedExits(IFuturemud gameworld);
	void RebuildAll(IFuturemud gameworld);
	void RebuildNetwork(IMagicPortalNetwork network);
	void RebuildNetworksForItem(IFuturemud gameworld, IGameItem item);
	void RemoveNetworkExits(IMagicPortalNetwork network);
	IMagicPortalEndpoint? CreateOrUpdateEndpoint(ICharacter actor, IMagicPortalNetwork network, string key, string name,
		MagicPortalEndpointType endpointType, ICell? cell, IGameItem? item, bool replace, long? spellId,
		out string reason);
	IMagicPortalLink? CreateLink(ICharacter actor, IMagicPortalNetwork network, IMagicPortalEndpoint source,
		IMagicPortalEndpoint destination, long? spellId, out string reason);
	void DeleteSpellCreatedTopology(IFuturemud gameworld, IEnumerable<long> endpointIds, IEnumerable<long> linkIds);
}
