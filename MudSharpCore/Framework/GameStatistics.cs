using System;
using System.Globalization;
using System.Linq;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp.Framework;

public class GameStatistics : IGameStatistics
{
	private readonly IFuturemud _parent;

	public GameStatistics(IFuturemud parent)
	{
		_parent = parent;
	}

	public int RecordOnlinePlayers { get; set; }

	public DateTime RecordOnlinePlayersDateTime { get; set; }

	public DateTime LastBootTime { get; set; }
	public TimeSpan LastStartupSpan { get; set; }

	public WeeklyStatistic GetOrCreateWeeklyStatistic()
	{
		var currentWeekStart = DateTime.UtcNow.GetStartOfWeek();
		var dbitem = FMDB.Context.WeeklyStatistics.FirstOrDefault(x => x.Start == currentWeekStart);
		if (dbitem == null)
		{
			dbitem = new WeeklyStatistic
			{
				Start = currentWeekStart,
				End = currentWeekStart.AddDays(7),
				TotalAccounts = FMDB.Context.Accounts.Count(),
				ActiveAccounts = 0,
				NewAccounts = 0,
				ApplicationsSubmitted = 0,
				ApplicationsApproved = 0,
				PlayerDeaths = 0,
				NonPlayerDeaths = 0
			};
			FMDB.Context.WeeklyStatistics.Add(dbitem);
			foreach (var account in FMDB.Context.Accounts)
			{
				account.HasBeenActiveInWeek = false;
			}

			FMDB.Context.SaveChanges();
		}

		return dbitem;
	}

	public void UpdateActiveAccount(Account account)
	{
		var dbitem = GetOrCreateWeeklyStatistic();
		account.HasBeenActiveInWeek = true;
		dbitem.ActiveAccounts += 1;
		FMDB.Context.SaveChanges();
	}

	public void UpdateNewAccount()
	{
		using (new FMDB())
		{
			var dbitem = GetOrCreateWeeklyStatistic();
			dbitem.TotalAccounts += 1;
			dbitem.NewAccounts += 1;
			FMDB.Context.SaveChanges();
		}
	}

	public void UpdateApplicationSubmitted()
	{
		using (new FMDB())
		{
			var dbitem = GetOrCreateWeeklyStatistic();
			dbitem.ApplicationsSubmitted += 1;
			FMDB.Context.SaveChanges();
		}
	}

	public void UpdateApplicationApproved()
	{
		using (new FMDB())
		{
			var dbitem = GetOrCreateWeeklyStatistic();
			dbitem.ApplicationsApproved += 1;
			FMDB.Context.SaveChanges();
		}
	}

	public void UpdatePlayerDeath()
	{
		using (new FMDB())
		{
			var dbitem = GetOrCreateWeeklyStatistic();
			dbitem.PlayerDeaths += 1;
			FMDB.Context.SaveChanges();
		}
	}

	public void UpdateNonPlayerDeath()
	{
		using (new FMDB())
		{
			var dbitem = GetOrCreateWeeklyStatistic();
			dbitem.NonPlayerDeaths += 1;
			FMDB.Context.SaveChanges();
		}
	}

	public void DoPlayerActivitySnapshot()
	{
		var players = _parent.Characters.Where(x => !x.IsAdministrator() && !x.IsGuest)
		                     .Concat(_parent.NPCs.Where(x => x.CombinedEffectsOfType<ICountForWho>().Any())).ToList();
		var count = players.Count;
		var admins = _parent.Characters.Count(x => x.IsAdministrator());
		var guestCount = _parent.Characters.Count(x => x.IsGuest);
		var availableAdmins = _parent.Characters.Count(x => x.AffectedBy<IAdminAvailableEffect>());
		var idlers = players.Count(x => (x.CharacterController?.InactivityMilliseconds ?? 0) > 600000);
		var locations = players.Select(x => x.Location).Distinct().Count();

		using (new FMDB())
		{
			var snapshot = new PlayerActivitySnapshot
			{
				DateTime = DateTime.UtcNow,
				OnlinePlayers = count,
				OnlineAdmins = admins,
				AvailableAdmins = availableAdmins,
				IdlePlayers = idlers,
				UniquePCLocations = locations,
				OnlineGuests = guestCount
			};
			FMDB.Context.PlayerActivitySnapshots.Add(snapshot);
			FMDB.Context.SaveChanges();
		}
	}

	public void LoadStatistics(IFuturemudLoader game)
	{
		var recordPlayers = game.GetStaticConfiguration("RecordOnlinePlayers");
		if (recordPlayers != null)
		{
			RecordOnlinePlayers = int.Parse(recordPlayers);
		}

		var recordPlayersDateTime = game.GetStaticConfiguration("RecordOnlinePlayersDateTime");
		if (recordPlayersDateTime != null)
		{
			RecordOnlinePlayersDateTime = DateTime.Parse(recordPlayersDateTime,
				CultureInfo.InvariantCulture,
				DateTimeStyles.AssumeUniversal).ToUniversalTime();
		}
		else
		{
			RecordOnlinePlayersDateTime = DateTime.UtcNow;
		}
	}

	public bool UpdateOnlinePlayers()
	{
		var count = _parent.Characters.Count(x => !x.IsAdministrator() && !x.IsGuest);
		if (count <= RecordOnlinePlayers)
		{
			return false;
		}

		RecordOnlinePlayers = count;
		RecordOnlinePlayersDateTime = DateTime.UtcNow;
		using (new FMDB())
		{
			var dbitem = FMDB.Context.StaticConfigurations.Find("RecordOnlinePlayers");
			if (dbitem == null)
			{
				dbitem = new StaticConfiguration();
				FMDB.Context.StaticConfigurations.Add(dbitem);
				dbitem.SettingName = "RecordOnlinePlayers";
			}

			dbitem.Definition = RecordOnlinePlayers.ToString(CultureInfo.InvariantCulture);

			dbitem = FMDB.Context.StaticConfigurations.Find("RecordOnlinePlayersDateTime");
			if (dbitem == null)
			{
				dbitem = new StaticConfiguration();
				FMDB.Context.StaticConfigurations.Add(dbitem);
				dbitem.SettingName = "RecordOnlinePlayersDateTime";
			}

			dbitem.Definition = RecordOnlinePlayersDateTime.ToString("g", CultureInfo.InvariantCulture);
			FMDB.Context.SaveChanges();
		}

		return true;
	}
}