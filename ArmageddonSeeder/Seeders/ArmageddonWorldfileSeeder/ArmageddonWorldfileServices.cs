#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Community;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static bool HasSpecial(ArmWorldNpc npc, string name) => npc.Programs.Any(x =>
		x.StartsWith("C ", StringComparison.Ordinal) && x.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() == name);

	private IEnumerable<(ArmWorldNpc Npc, Room Room)> ServiceLocations(params string[] specials)
	{
		var sources = _source.Npcs.Where(x => specials.Any(s => HasSpecial(x, s))).ToDictionary(x => x.Vnum);
		return _source.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && sources.ContainsKey(x.Arguments[1]))
			.Select(x => (Npc: sources[x.Arguments[1]], Room: _rooms[x.Arguments[3]]))
			.DistinctBy(x => (x.Npc.Vnum, x.Room.Id));
	}

	private void InstallNativeServices()
	{
		InstallClanBanking();
		InstallMountStables();
	}

	private void InstallClanBanking()
	{
		var locations = ServiceLocations("banker").ToArray();
		if (locations.Length == 0) return;
		if (_db.Banks.Any(x => x.Code == "ARMW")) throw new InvalidOperationException("Bank code ARMW is already in use.");
		var bank = new Bank
		{
			Id = Next<Bank>(), Name = "Clan Bank", Code = "ARMW", EconomicZoneId = _economicZones[0].Id,
			PrimaryCurrencyId = _currency.Id, MaximumBankAccountsPerCustomer = 1
		};
		_db.Banks.Add(bank);
		var noCharacters = Prog("ArmageddonWorldfileBankNoPersonalAccounts", "return false", ProgVariableTypes.Boolean,
			("customer", ProgVariableTypes.Character));
		var noShops = Prog("ArmageddonWorldfileBankNoShopAccounts", "return false", ProgVariableTypes.Boolean,
			("customer", ProgVariableTypes.Shop));
		var noClans = Prog("ArmageddonWorldfileBankAccountsPrecreated", "return false", ProgVariableTypes.Boolean,
			("customer", ProgVariableTypes.Clan));
		var accountType = new BankAccountType
		{
			Id = Next<BankAccountType>(), BankId = bank.Id, Name = "Clan Account",
			CustomerDescription = "A clan-owned obsidian account without fees, interest or overdraft.",
			CanOpenAccountProgCharacterId = noCharacters.Id, CanOpenAccountProgShopId = noShops.Id,
			CanOpenAccountProgClanId = noClans.Id
		};
		_db.BankAccountTypes.Add(accountType);
		_db.BankCurrencyReserves.Add(new BankCurrencyReserve { BankId = bank.Id, CurrencyId = _currency.Id, Amount = 0 });
		foreach (var room in locations.Select(x => x.Room).DistinctBy(x => x.Id))
			_db.BankBranches.Add(new BankBranch { BankId = bank.Id, RoomId = room.Id });
		var calendar = _db.Calendars.OrderBy(x => x.Id).First();
		var clock = _db.Clocks.OrderBy(x => x.Id).First();
		var timezone = _db.Timezones.Single(x => x.Id == clock.PrimaryTimezoneId);
		var created = $"{calendar.Id}_{calendar.Date}_{clock.Id}_{timezone.Name} {clock.Hours}:{clock.Minutes}:{clock.Seconds}";
		var number = 1;
		foreach (var (sourceId, clan) in _clans.OrderBy(x => x.Key))
		{
			foreach (var rank in _ranks.Where(x => x.Key.Clan == sourceId).Select(x => x.Value))
				rank.Privileges |= (long)ClanPrivilegeType.CanDepositBankAccounts;
			var account = new BankAccount
			{
				Id = Next<BankAccount>(), BankId = bank.Id, BankAccountTypeId = accountType.Id,
				Name = clan.Name, AccountNumber = number++, AccountOwnerClanId = clan.Id,
				CurrentBalance = 0, AccountStatus = 0, AccountCreationDate = created,
				AuthorisedBankPaymentItems = string.Empty
			};
			_db.BankAccounts.Add(account);
			clan.BankAccountId = account.Id;
			var banker = new Appointment
			{
				Id = Next<Appointment>(), ClanId = clan.Id, Name = "Banker", MaximumSimultaneousHolders = 0,
				Privileges = (long)ClanPrivilegeType.CanManageBankAccounts, IsAppointedByElection = false
			};
			banker.AppointmentsTitles.Add(new AppointmentsTitles { AppointmentId = banker.Id, Title = "banker", Order = 0 });
			_db.Appointments.Add(banker);
			Record($"clan.bank.{sourceId}", nameof(BankAccount), account.Id, null,
				new { Clan = clan.Id, BankerAppointment = banker.Id, OpeningBalance = 0, Disposition = "Native clan-owned account; per-member CJOB_WITHDRAW becomes an initially unfilled Banker appointment, independently of rank." });
		}
		_counts["bank branches"] = locations.Select(x => x.Room.Id).Distinct().Count();
		_counts["clan bank accounts"] = _clans.Count;
		Record("bank", nameof(Bank), bank.Id, null, new { Source = locations.Select(x => new { Npc = x.Npc.Vnum, Room = x.Room.UniqueName }), Disposition = "One native clan bank with fixed branches, zero fees, zero opening balances, and no historical member/balance data." });
	}

	private void InstallMountStables()
	{
		foreach (var location in ServiceLocations("animal_to_store", "luirs_stablehand").DistinctBy(x => x.Room.Id))
		{
			long? feeProg = null;
			if (location.Npc.Vnum == 64027 && _clans.TryGetValue(4, out var kurac))
				feeProg = Prog("ArmageddonWorldfileLuirStableFee", $"if (isclanmember(@customer, toclan({kurac.Id})))\n\treturn 0\nend if\nreturn 20",
					ProgVariableTypes.Number, ("customer", ProgVariableTypes.Character), ("mount", ProgVariableTypes.Character)).Id;
			var stable = new Stable
			{
				Id = Next<Stable>(), Name = "The Stables at " + location.Room.RoomOverlays.Single().RoomName,
				EconomicZoneId = _economicZones[0].Id, RoomId = location.Room.Id, IsTrading = true,
				LodgeFee = 20m, DailyFee = 0m, LodgeFeeProgId = feeProg, EmployeeRecords = "<Employees/>"
			};
			_db.Stables.Add(stable);
			Record($"stable.{location.Npc.Vnum}.{location.Room.UniqueName}", nameof(Stable), stable.Id, null,
				new { SourceNpc = location.Npc.Vnum, Room = location.Room.UniqueName, Disposition = "Existing native stable and persistent tickets; approved 20-bit charge occurs on lodging, with no daily fee. House Kurac exemption retained at Luir. Undefined clan 66 and vanished summoned-mount pricing have no imported counterpart." });
			_counts["native stables"] = _counts.GetValueOrDefault("native stables") + 1;
		}
		Flush();
	}
}
