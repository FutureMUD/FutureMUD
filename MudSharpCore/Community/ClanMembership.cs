using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.Community;

public class ClanMembership : SaveableItem, IClanMembership, ILazyLoadDuringIdleTime
{
	protected ClanMembership()
	{
		BackPayDiciontary = new Dictionary<ICurrency, decimal>();
		Appointments = new List<IAppointment>();
	}

	public ClanMembership(IFuturemud gameworld) : this()
	{
		Gameworld = gameworld;
	}

	public ClanMembership(MudSharp.Models.ClanMembership membership, IClan clan, IFuturemud gameworld)
		: this()
	{
		Gameworld = gameworld;
		MemberId = membership.CharacterId;
		Clan = clan;
		Rank = Clan.Ranks.First(x => membership.RankId == x.Id);
		Paygrade = membership.PaygradeId.HasValue
			? Clan.Paygrades.FirstOrDefault(x => membership.PaygradeId == x.Id)
			: null;
		JoinDate = clan.Calendar.GetDate(membership.JoinDate);
		ManagerId = membership.ManagerId;

		PersonalName = new PersonalName(XElement.Parse(membership.PersonalName), Gameworld);

		foreach (var item in membership.ClanMembershipsAppointments)
		{
			Appointments.Add(Clan.Appointments.First(x => item.AppointmentId == x.Id));
		}

		foreach (var item in membership.ClanMembershipsBackpay)
		{
			BackPayDiciontary.Add(gameworld.Currencies.Get(item.CurrencyId), item.Amount);
		}

		IsArchivedMembership = membership.ArchivedMembership;
		Gameworld.SaveManager.AddLazyLoad(this);
	}

	public long MemberId { get; set; }
	private ICharacter _memberCharacter;

	public ICharacter MemberCharacter
	{
		get
		{
			if (_memberCharacter is null)
			{
				_memberCharacter = Gameworld.TryGetCharacter(MemberId, true);
				Gameworld.SaveManager.AbortLazyLoad(this);
			}

			return _memberCharacter;
		}
	}

	public IClan Clan { get; set; }
	public IRank Rank { get; set; }
	public IPaygrade Paygrade { get; set; }
	public List<IAppointment> Appointments { get; set; }
	public MudDate JoinDate { get; set; }
	public long? ManagerId { get; set; }
	public Dictionary<ICurrency, decimal> BackPayDiciontary { get; set; }
	public IPersonalName PersonalName { get; set; }
	public bool IsArchivedMembership { get; set; }

	public ClanPrivilegeType NetPrivileges
	{
		get
		{
			return Rank.Privileges |
			       (Appointments.Any()
				       ? Appointments.Select(x => x.Privileges).Aggregate((sum, val) => sum | val)
				       : ClanPrivilegeType.None);
		}
	}

	public override string FrameworkItemType => "ClanMembership";

	#region Overrides of FrameworkItem

	public override string Name => PersonalName.GetName(NameStyle.FullName);

	#endregion

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ClanMemberships.Find(Clan.Id, MemberId);
			if (dbitem == null)
			{
				Changed = false;
				return;
			}

			dbitem.RankId = Rank.Id;
			dbitem.PaygradeId = Paygrade?.Id;
			dbitem.ManagerId = ManagerId;
			dbitem.JoinDate = JoinDate.GetDateString();
			dbitem.ArchivedMembership = IsArchivedMembership;
			FMDB.Context.ClanMembershipsAppointments.RemoveRange(dbitem.ClanMembershipsAppointments);
			foreach (var item in Appointments)
			{
				dbitem.ClanMembershipsAppointments.Add(new ClanMembershipsAppointments
					{ ClanMembership = dbitem, AppointmentId = item.Id });
			}

			FMDB.Context.ClanMembershipsBackpay.RemoveRange(dbitem.ClanMembershipsBackpay);
			foreach (var item in BackPayDiciontary)
			{
				var dbbackpay = new ClanMembershipBackpay();
				dbitem.ClanMembershipsBackpay.Add(dbbackpay);
				dbbackpay.CurrencyId = item.Key.Id;
				dbbackpay.Amount = item.Value;
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public void DoLoad()
	{
		_memberCharacter = Gameworld.TryGetCharacter(MemberId, true);
	}

	public int LazyLoadPriority => IsArchivedMembership ? 100 : -1;

	public void DeleteFromDb()
	{
		if (Changed)
		{
			Gameworld.SaveManager.Abort(this);
		}

		Gameworld.SaveManager.Flush();
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ClanMemberships.Find(Clan.Id, MemberId);
			FMDB.Context.ClanMemberships.Remove(dbitem);
			FMDB.Context.SaveChanges();
		}
	}

	private decimal MaximumBackpayForCurrency(ICurrency currency)
	{
		if (Clan.MaximumPeriodsOfUncollectedBackPay == null)
		{
			return decimal.MaxValue;
		}

		var amount = 0.0M;
		if (Paygrade != null && Paygrade.PayCurrency == currency)
		{
			amount += Paygrade.PayAmount;
		}

		foreach (var position in Appointments)
		{
			if (position.Paygrade != null && position.Paygrade.PayCurrency == currency)
			{
				amount += position.Paygrade.PayAmount;
			}
		}

		return amount * Clan.MaximumPeriodsOfUncollectedBackPay.Value;
	}

	public void AwardPay(ICurrency currency, decimal amount)
	{
		if (BackPayDiciontary == null)
		{
			BackPayDiciontary = new Dictionary<ICurrency, decimal>();
		}

		if (!BackPayDiciontary.ContainsKey(currency))
		{
			BackPayDiciontary[currency] = 0;
		}

		BackPayDiciontary[currency] += amount;
		if (BackPayDiciontary[currency] > MaximumBackpayForCurrency(currency))
		{
			BackPayDiciontary[currency] = MaximumBackpayForCurrency(currency);
		}

		Changed = true;
	}

	public void SetRank(IRank rank)
	{
		Rank = rank;
		if (!Rank.Paygrades.Contains(Paygrade))
		{
			Paygrade = Rank.Paygrades.OrderBy(x => x.PayAmount).FirstOrDefault();
		}

		Changed = true;
	}

	public void AppointToPosition(IAppointment appointment)
	{
		Appointments.Add(appointment);
		if (appointment.MinimumRankToHold.RankNumber > Rank.RankNumber)
		{
			SetRank(appointment.MinimumRankToHold);
		}

		Changed = true;
	}
}