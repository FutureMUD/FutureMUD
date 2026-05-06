using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using System.Linq;

namespace MudSharp.Community;

public class ClanPayrollHistoryEntry : IClanPayrollHistoryEntry
{
	private readonly IFuturemud _gameworld;
	private readonly long _characterId;
	private readonly long? _actorId;
	private ICharacter _character;
	private ICharacter _actor;

	public ClanPayrollHistoryEntry(MudSharp.Models.ClanPayrollHistory history, IClan clan)
	{
		_gameworld = ((Clan)clan).Gameworld;
		_id = history.Id;
		Clan = clan;
		_characterId = history.CharacterId;
		Rank = clan.Ranks.FirstOrDefault(x => x.Id == history.RankId);
		Paygrade = history.PaygradeId.HasValue ? clan.Paygrades.FirstOrDefault(x => x.Id == history.PaygradeId.Value) : null;
		Appointment = history.AppointmentId.HasValue ? clan.Appointments.FirstOrDefault(x => x.Id == history.AppointmentId.Value) : null;
		_actorId = history.ActorId;
		Currency = _gameworld.Currencies.Get(history.CurrencyId);
		Amount = history.Amount;
		EntryType = (ClanPayrollHistoryType)history.EntryType;
		DateTime = MudDateTime.FromStoredStringOrFallback(history.DateTime, _gameworld,
			StoredMudDateTimeFallback.CurrentDateTime, "ClanPayrollHistory", history.Id, clan.Name, "DateTime");
		Description = history.Description;
	}

	private ClanPayrollHistoryEntry(IClanMembership membership, ICurrency currency, decimal amount,
		ClanPayrollHistoryType entryType, string description, IPaygrade paygrade, IAppointment appointment,
		ICharacter actor)
	{
		_gameworld = ((Clan)membership.Clan).Gameworld;
		Clan = membership.Clan;
		_characterId = membership.MemberId;
		Rank = membership.Rank;
		Paygrade = paygrade;
		Appointment = appointment;
		_actor = actor;
		_actorId = actor?.Id;
		Currency = currency;
		Amount = amount;
		EntryType = entryType;
		DateTime = membership.Clan.Calendar.CurrentDateTime;
		Description = description;

		using (new FMDB())
		{
			var dbitem = new MudSharp.Models.ClanPayrollHistory
			{
				ClanId = Clan.Id,
				CharacterId = membership.MemberId,
				RankId = membership.Rank.Id,
				PaygradeId = paygrade?.Id,
				AppointmentId = appointment?.Id,
				ActorId = actor?.Id,
				CurrencyId = currency.Id,
				Amount = amount,
				EntryType = (int)entryType,
				DateTime = DateTime.GetDateTimeString(),
				Description = description
			};
			FMDB.Context.ClanPayrollHistories.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private readonly long _id;
	public long Id => _id;
	public string Name => $"Payroll Entry #{Id:N0}";
	public string FrameworkItemType => "ClanPayrollHistory";
	public IClan Clan { get; }
	public ICharacter Character => _character ??= _gameworld.TryGetCharacter(_characterId, true);
	public IRank Rank { get; }
	public IPaygrade Paygrade { get; }
	public IAppointment Appointment { get; }
	public ICharacter Actor => _actor ??= _actorId.HasValue ? _gameworld.TryGetCharacter(_actorId.Value, true) : null;
	public ICurrency Currency { get; }
	public decimal Amount { get; }
	public ClanPayrollHistoryType EntryType { get; }
	public MudDateTime DateTime { get; }
	public string Description { get; }

	public static IClanPayrollHistoryEntry Create(IClanMembership membership, ICurrency currency, decimal amount,
		ClanPayrollHistoryType entryType, string description, IPaygrade paygrade = null, IAppointment appointment = null,
		ICharacter actor = null)
	{
		return new ClanPayrollHistoryEntry(membership, currency, amount, entryType, description, paygrade, appointment,
			actor);
	}
}
