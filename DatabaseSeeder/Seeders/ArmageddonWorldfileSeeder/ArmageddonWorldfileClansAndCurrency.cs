#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private Coin _bit = null!;
	private readonly Dictionary<(int Clan, int Rank), Rank> _ranks = new();
	// The clan file supplies only rank zero in this snapshot. Executable specials still
	// reference these numeric minimums; create the native ranks without inventing titles or members.
	private static int RequiredSpecialRank(int clan) => clan switch { 4 => 4, 12 => 6, 23 => 3, 26 => 2, _ => 0 };
	private static readonly IReadOnlyDictionary<int, string[]> MaterialNames = new Dictionary<int, string[]>
	{
		[2] = ["Iron", "Wrought Iron", "Steel"], [3] = ["Obsidian"], [5] = ["Chitin"],
		[6] = ["Quartz", "Diamond", "Glass"], [7] = ["Glass", "Soda Lime Glass"],
		[8] = ["Human Skin", "Skin", "Leather"], [9] = ["Linen", "Cotton", "Wool"],
		[10] = ["Leather", "Cow Leather"], [11] = ["Bone", "Human Bone"],
		[12] = ["Earthenware", "Ceramic", "Stoneware"], [13] = ["Horn", "Keratin"],
		[15] = ["Human Flesh", "Flesh", "Meat"], [16] = ["Silk"], [17] = ["Ivory", "Bone"],
		[18] = ["Horn", "Keratin"]
	};

	private static Dictionary<int, Material> BindMaterials(FuturemudDatabaseContext db,
		ArmageddonInstallationSettings settings, IEnumerable<int> required)
	{
		var all = db.Materials.AsNoTracking().Where(x => x.Type == 0).ToArray();
		var result = new Dictionary<int, Material>();
		foreach (var type in required.Distinct())
		{
			var binding = type switch
			{
				0 or 4 => settings.NativeBindings[ArmageddonNativeBinding.Stone],
				1 => settings.NativeBindings[ArmageddonNativeBinding.Wood], _ => 0
			};
			if (binding > 0) { result.Add(type, all.Single(x => x.Id == binding)); continue; }
			if (!MaterialNames.TryGetValue(type, out var names))
				throw new InvalidOperationException($"Source material {type} has no qualified native mapping.");
			Material? material = null;
			foreach (var name in names)
			{
				var candidates = all.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
				if (candidates.Length > 1) throw new InvalidOperationException($"Native material {name} is ambiguous.");
				if (candidates.Length == 1) { material = candidates[0]; break; }
			}
			result.Add(type, material ?? throw new InvalidOperationException($"Install a native solid material for source material {type}: {string.Join(" / ", names)}."));
		}
		return result;
	}

	private void InstallClansAndCurrency()
	{
		foreach (var material in BindMaterials(_db, _settings, _source.Items.Select(x => x.Material).Append(3)))
			_materials.Add(material.Key, material.Value);
		_currency = new Currency { Id = Next<Currency>(), Name = "Obsidian", BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m };
		_db.Currencies.Add(_currency);
		var division = new CurrencyDivision
		{
			Id = Next<CurrencyDivision>(), CurrencyId = _currency.Id, Name = "bits", BaseUnitConversionRate = 1.0m
		};
		_db.CurrencyDivisions.Add(division);
		division.CurrencyDivisionAbbreviations.Add(new()
		{
			Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:b|bits|bit|sid|sids|obsidian))$"
		});
		foreach (var type in Enum.GetValues<CurrencyDescriptionPatternType>())
		{
			var pattern = new CurrencyDescriptionPattern
			{
				Id = Next<CurrencyDescriptionPattern>(), CurrencyId = _currency.Id, Type = (int)type, Order = 1,
				NegativePrefix = type is CurrencyDescriptionPatternType.Casual or CurrencyDescriptionPatternType.Wordy ? "negative " : "-"
			};
			pattern.CurrencyDescriptionPatternElements.Add(new()
			{
				CurrencyDivisionId = division.Id, Pattern = "{0} bit", Order = 1, ShowIfZero = true,
				PluraliseWord = "bit", RoundingMode = (int)RoundingMode.Truncate, SpecialValuesOverrideFormat = false
			});
			_db.CurrencyDescriptionPatterns.Add(pattern);
		}
		_bit = new Coin
		{
			Id = Next<Coin>(), CurrencyId = _currency.Id, Name = "obsidian bit", ShortDescription = "a piece of obsidian",
			FullDescription = "A small piece of black obsidian, cut for use in trade.", GeneralForm = "piece of obsidian",
			PluralWord = "piece", Value = 1.0m, Weight = 0.005 * _kilograms, UseForChange = true
		};
		_db.Coins.Add(_bit);
		Record("currency", nameof(Currency), _currency.Id, null,
			new { SourceUnit = "obsidian", BaseValue = 1, NativeCoinMassGrams = 5, Disposition = "Native currency; authored physical coin mass" });
		var calendar = _db.Calendars.OrderBy(x => x.Id).First();
		var clock = _db.Clocks.OrderBy(x => x.Id).First();
		var timezone = _db.Timezones.Single(x => x.Id == clock.PrimaryTimezoneId);
		foreach (var source in _source.Clans.Where(x => x.Index > 0))
		{
			var clan = new Clan
			{
				Id = Next<Clan>(), Name = source.Name.Trim(), FullName = source.Name.Trim(),
				Alias = "armclan" + source.Index, Description = source.Name.Trim(), IsTemplate = false,
				CalendarId = calendar.Id, PayIntervalType = (int)IntervalType.Monthly, PayIntervalModifier = 1,
				PayIntervalReferenceDate = calendar.Date, PayIntervalReferenceTime = timezone.Name + " 0:0:0",
				Sphere = "", ShowClanMembersInWho = false, ShowFamousMembersInNotables = false
			};
			_db.Clans.Add(clan);
			_clans.Add(source.Index, clan);
			var referenced = _source.Npcs.SelectMany(npc => npc.Clans)
				.Where(x => ResolveClan(x.GetProperty("Name").GetString() ?? "") == source.Index)
				.Select(x => x.GetProperty("Rank").GetInt32()).DefaultIfEmpty(0).Max();
			var highest = Math.Max(Math.Max(source.Ranks.Length - 1, referenced), RequiredSpecialRank(source.Index));
			if (highest > 1000) throw new InvalidOperationException($"Clan {source.Index} has an unreasonable rank {highest}.");
			for (var number = 0; number <= highest; number++)
			{
				var rank = new Rank
				{
					Id = Next<Rank>(), ClanId = clan.Id, Name = number < source.Ranks.Length ? source.Ranks[number] : "Rank " + number,
					RankNumber = number, RankPath = "", Privileges = 0, FameType = 0
				};
				rank.RanksTitles.Add(new() { Title = rank.Name, Order = 0 });
				_db.Ranks.Add(rank);
				_ranks.Add((source.Index, number), rank);
			}
			if (RequiredSpecialRank(source.Index) >= source.Ranks.Length)
				Adapt("clan ranks", source.Index, $"Source specials require numeric ranks through {RequiredSpecialRank(source.Index)}; missing rank labels use explicit Rank N placeholders. No memberships or automatic promotions are invented. Numeric source clan references take precedence over stale C comments.");
			Record("clan." + source.Index, nameof(Clan), clan.Id, null, source);
		}
		InstallClanLeaderAppointments();
		_counts["clans"] = _clans.Count;
		Adapt("clan politics", 0, "Source political relations and per-member job flags are retained as provenance. Approved Leader and Banker appointments remain initially unfilled; no appointment authority or salaries are invented.");
		Adapt("material", 0, "Unspecified source matter uses the installed stone baseline; generic material classes use the configured native solid equivalents.");
		Flush();
	}

	private void InstallClanLeaderAppointments()
	{
		// Source leadership is a per-member job bit, independent of rank, with no holder limit.
		// Only these clans need it for the retained Kohmar and noble-gate callbacks.
		foreach (var sourceId in new[] { 28, 41 })
		{
			if (!_clans.TryGetValue(sourceId, out var clan)) continue;
			var leader = new Appointment
			{
				Id = Next<Appointment>(), ClanId = clan.Id, Name = "Leader", MaximumSimultaneousHolders = 0,
				Privileges = 0, IsAppointedByElection = false
			};
			leader.AppointmentsTitles.Add(new() { AppointmentId = leader.Id, Title = "leader", Order = 0 });
			_db.Appointments.Add(leader);
			var permission = Prog($"ArmageddonWorldfileClan{sourceId}Leader", $"""
				var clan = toclan({clan.Id})
				var leader = toappointment(@clan, {leader.Id})
				if (isnull(@actor) or isnull(@clan) or isnull(@leader))
					return false
				end if
				return isclanmember(@actor, @clan, @leader)
				""", ProgVariableTypes.Boolean, ("actor", ProgVariableTypes.Character));
			Record($"clan.leader.{sourceId}", nameof(Appointment), leader.Id, null,
				new { SourceClan = sourceId, Clan = clan.Id, SourceJob = "CJOB_LEADER", LegacyMirror = "CFL_CLANLEADER", PermissionProg = permission.Id,
					Disposition = "Initially unfilled native Leader appointment, independent of rank. No automatic holders, rank requirement, holder limit, salary or administrative privileges." });
		}
	}

	private int? ResolveClan(string name)
	{
		if (int.TryParse(name, out var numeric)) return numeric;
		return _source.Clans.FirstOrDefault(x => x.Name.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))?.Index;
	}
}
