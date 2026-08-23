using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Shops;

/// <summary>
/// A transient acceptance proposal. It intentionally has no persistence: consent that has not
/// been granted must not survive a reboot, while accepted table membership is persisted by the
/// session itself.
/// </summary>
public sealed class RestaurantTableJoinProposal : IProposal
{
	private readonly Restaurant _restaurant;
	private readonly long _approverCharacterId;

	public RestaurantTableJoinProposal(Restaurant restaurant, Guid requestId, long approverCharacterId,
		string requesterName, string tableName)
	{
		_restaurant = restaurant;
		RequestId = requestId;
		_approverCharacterId = approverCharacterId;
		DescriptionString = $"Allow {requesterName.ColourName()} to join your table at {restaurant.Name.ColourName()} ({tableName.ColourObject()})";
	}

	public Guid RequestId { get; }
	public string DescriptionString { get; }
	public IEnumerable<string> Keywords => new[] { "restaurant", "table", "join" };

	public void Accept(string message = "")
	{
		_restaurant.ResolveTableJoinProposal(RequestId, _approverCharacterId, true);
	}

	public void Reject(string message = "")
	{
		_restaurant.ResolveTableJoinProposal(RequestId, _approverCharacterId, false);
	}

	public void Expire()
	{
		_restaurant.ResolveTableJoinProposal(RequestId, _approverCharacterId, false);
	}

	public string Describe(IPerceiver voyeur)
	{
		return DescriptionString;
	}
}
