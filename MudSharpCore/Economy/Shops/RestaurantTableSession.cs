using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using DbRestaurantTableParticipant = MudSharp.Models.RestaurantTableParticipant;
using DbRestaurantTableSession = MudSharp.Models.RestaurantTableSession;

#nullable enable

namespace MudSharp.Economy.Shops;

public sealed class RestaurantTableSession : SaveableItem, IRestaurantTableSession
{
	private readonly List<IRestaurantTableParticipant> _participants = new();
	private readonly List<IRestaurantOrder> _orders = new();
	private RestaurantTableSessionStatus _status;
	private DateTime? _closedAtUtc;
	private DateTime? _abandonmentPendingAtUtc;
	private bool _abandonmentReported;

	public RestaurantTableSession(Restaurant restaurant, IGameItem table, ICharacter firstParticipant)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		TableGameItemId = table.Id;
		_status = RestaurantTableSessionStatus.Active;
		CreatedAtUtc = DateTime.UtcNow;
		LastUpdatedAtUtc = CreatedAtUtc;

		using (new FMDB())
		{
			var dbitem = new DbRestaurantTableSession
			{
				RestaurantShopId = restaurant.Id,
				TableGameItemId = TableGameItemId,
				Status = (int)_status,
				CreatedAtUtc = CreatedAtUtc,
				LastUpdatedAtUtc = LastUpdatedAtUtc,
				AbandonmentReported = false
			};
			FMDB.Context.RestaurantTableSessions.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		AddParticipant(firstParticipant);
	}

	public RestaurantTableSession(DbRestaurantTableSession session, Restaurant restaurant)
	{
		Gameworld = restaurant.Gameworld;
		Restaurant = restaurant;
		_id = session.Id;
		TableGameItemId = session.TableGameItemId;
		_status = Enum.IsDefined(typeof(RestaurantTableSessionStatus), session.Status)
			? (RestaurantTableSessionStatus)session.Status
			: RestaurantTableSessionStatus.Closed;
		CreatedAtUtc = DateTime.SpecifyKind(session.CreatedAtUtc, DateTimeKind.Utc);
		LastUpdatedAtUtc = DateTime.SpecifyKind(session.LastUpdatedAtUtc, DateTimeKind.Utc);
		_closedAtUtc = session.ClosedAtUtc.HasValue ? DateTime.SpecifyKind(session.ClosedAtUtc.Value, DateTimeKind.Utc) : null;
		_abandonmentPendingAtUtc = session.AbandonmentPendingAtUtc.HasValue
			? DateTime.SpecifyKind(session.AbandonmentPendingAtUtc.Value, DateTimeKind.Utc)
			: null;
		_abandonmentReported = session.AbandonmentReported;

		foreach (var participant in session.Participants.OrderBy(x => x.Id))
		{
			_participants.Add(new RestaurantTableParticipant(participant, this));
		}
	}

	public override string FrameworkItemType => "RestaurantTableSession";
	public IRestaurant Restaurant { get; }
	public long TableGameItemId { get; }
	public RestaurantTableSessionStatus Status => _status;
	public DateTime CreatedAtUtc { get; }
	public DateTime LastUpdatedAtUtc { get; private set; }
	public DateTime? ClosedAtUtc => _closedAtUtc;
	public DateTime? AbandonmentPendingAtUtc => _abandonmentPendingAtUtc;
	public bool AbandonmentReported => _abandonmentReported;
	public IEnumerable<IRestaurantTableParticipant> Participants => _participants;
	public IEnumerable<IRestaurantOrder> Orders => _orders;

	public bool HasAcceptedParticipant(long characterId)
	{
		return _participants.Any(x => x.Accepted && x.CharacterId == characterId);
	}

	public RestaurantTableParticipant AddParticipant(ICharacter character)
	{
		var characterId = CharacterInstanceIdentityComparer.IdentityId(character);
		var existing = _participants.OfType<RestaurantTableParticipant>().FirstOrDefault(x => x.CharacterId == characterId);
		if (existing is not null)
		{
			existing.MarkPresent();
			return existing;
		}

		var participant = new RestaurantTableParticipant(this, character);
		_participants.Add(participant);
		Touch();
		return participant;
	}

	public void AddOrder(RestaurantOrder order, bool touch = true)
	{
		if (!_orders.Contains(order))
		{
			_orders.Add(order);
			if (touch)
			{
				Touch();
			}
		}
	}

	public void MarkParticipantPresence(ICharacter character, bool present)
	{
		var participant = _participants.OfType<RestaurantTableParticipant>()
			.FirstOrDefault(x => x.CharacterId == CharacterInstanceIdentityComparer.IdentityId(character));
		if (participant is null)
		{
			return;
		}

		if (present)
		{
			participant.MarkPresent();
			if (_status == RestaurantTableSessionStatus.AbandonmentPending)
			{
				_status = RestaurantTableSessionStatus.Active;
				_abandonmentPendingAtUtc = null;
				Touch();
			}
			return;
		}

		participant.MarkLeft();
		Touch();
	}

	public bool HasPresentAcceptedParticipant(Func<long, bool> isPresent)
	{
		return _participants.Any(x => x.Accepted && isPresent(x.CharacterId));
	}

	public void BeginAbandonment()
	{
		if (_status is RestaurantTableSessionStatus.Closed or RestaurantTableSessionStatus.Abandoned || _abandonmentReported)
		{
			return;
		}

		_status = RestaurantTableSessionStatus.AbandonmentPending;
		_abandonmentPendingAtUtc ??= DateTime.UtcNow;
		Touch();
	}

	public void MarkAbandoned()
	{
		if (_abandonmentReported)
		{
			return;
		}

		_status = RestaurantTableSessionStatus.Abandoned;
		_abandonmentReported = true;
		Touch();
	}

	public void Close()
	{
		if (_status == RestaurantTableSessionStatus.Closed)
		{
			return;
		}

		_status = RestaurantTableSessionStatus.Closed;
		_closedAtUtc = DateTime.UtcNow;
		Touch();
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.RestaurantTableSessions.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Status = (int)_status;
		dbitem.LastUpdatedAtUtc = LastUpdatedAtUtc;
		dbitem.ClosedAtUtc = _closedAtUtc;
		dbitem.AbandonmentPendingAtUtc = _abandonmentPendingAtUtc;
		dbitem.AbandonmentReported = _abandonmentReported;
		Changed = false;
	}

	private void Touch()
	{
		LastUpdatedAtUtc = DateTime.UtcNow;
		Changed = true;
	}
}

public sealed class RestaurantTableParticipant : SaveableItem, IRestaurantTableParticipant
{
	private DateTime? _leftAtUtc;

	public RestaurantTableParticipant(RestaurantTableSession session, ICharacter character)
	{
		Gameworld = session.Gameworld;
		Session = session;
		CharacterId = CharacterInstanceIdentityComparer.IdentityId(character);
		CharacterName = character.PersonalName.GetName(NameStyle.FullName);
		Accepted = true;
		JoinedAtUtc = DateTime.UtcNow;

		using (new FMDB())
		{
			var dbitem = new DbRestaurantTableParticipant
			{
				RestaurantTableSessionId = session.Id,
				CharacterId = CharacterId,
				CharacterName = CharacterName,
				Accepted = true,
				JoinedAtUtc = JoinedAtUtc
			};
			FMDB.Context.RestaurantTableParticipants.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public RestaurantTableParticipant(DbRestaurantTableParticipant participant, RestaurantTableSession session)
	{
		Gameworld = session.Gameworld;
		Session = session;
		_id = participant.Id;
		CharacterId = participant.CharacterId;
		CharacterName = participant.CharacterName;
		Accepted = participant.Accepted;
		JoinedAtUtc = DateTime.SpecifyKind(participant.JoinedAtUtc, DateTimeKind.Utc);
		_leftAtUtc = participant.LeftAtUtc.HasValue ? DateTime.SpecifyKind(participant.LeftAtUtc.Value, DateTimeKind.Utc) : null;
	}

	public override string FrameworkItemType => "RestaurantTableParticipant";
	public IRestaurantTableSession Session { get; }
	public long CharacterId { get; }
	public string CharacterName { get; }
	public bool Accepted { get; }
	public DateTime JoinedAtUtc { get; }
	public DateTime? LeftAtUtc => _leftAtUtc;

	public void MarkPresent()
	{
		if (_leftAtUtc is null)
		{
			return;
		}

		_leftAtUtc = null;
		Changed = true;
	}

	public void MarkLeft()
	{
		if (_leftAtUtc is not null)
		{
			return;
		}

		_leftAtUtc = DateTime.UtcNow;
		Changed = true;
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.RestaurantTableParticipants.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.LeftAtUtc = _leftAtUtc;
		Changed = false;
	}
}
