#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeederRepeatabilityHelperTests
{
	private sealed class WitnessProfileGuardContext : FuturemudDatabaseContext
	{
		public WitnessProfileGuardContext(DbContextOptions<FuturemudDatabaseContext> options)
			: base(options)
		{
		}

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			AssertNoIncompleteWitnessProfiles();
			return base.SaveChanges(acceptAllChangesOnSuccess);
		}

		public override int SaveChanges()
		{
			AssertNoIncompleteWitnessProfiles();
			return base.SaveChanges();
		}

		private void AssertNoIncompleteWitnessProfiles()
		{
			var incompleteProfile = ChangeTracker.Entries<WitnessProfile>()
				.Where(x => x.State == EntityState.Added)
				.Select(x => x.Entity)
				.FirstOrDefault(x => x.IdentityKnownProg is null || x.ReportingMultiplierProg is null);
			if (incompleteProfile is null)
			{
				return;
			}

			throw new AssertFailedException(
				$"Witness profile '{incompleteProfile.Name}' was saved before both required progs were assigned.");
		}
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static FuturemudDatabaseContext BuildWitnessProfileGuardContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new WitnessProfileGuardContext(options);
	}

	private static void SetSeederProperty<T>(LawSeeder seeder, string propertyName, T value)
	{
		typeof(LawSeeder)
			.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.GetSetMethod(true)!
			.Invoke(seeder, [value]);
	}

	[TestMethod]
	public void EnsureEntity_UsesScopedClientEvaluation_ForCaseInsensitiveMatches()
	{
		using var context = BuildContext();
		context.Timezones.Add(new Timezone
		{
			Id = 1,
			Name = "utc",
			Description = "Existing timezone",
			ClockId = 42,
			OffsetHours = 0,
			OffsetMinutes = 0
		});
		context.SaveChanges();

		var created = false;
		var timezone = SeederRepeatabilityHelper.EnsureEntity(
			context.Timezones,
			x => x.ClockId == 42 && string.Equals(x.Name, "UTC", StringComparison.OrdinalIgnoreCase),
			x => x.ClockId == 42,
			() =>
			{
				created = true;
				var entity = new Timezone
				{
					Id = 2,
					Name = "UTC",
					ClockId = 42
				};
				context.Timezones.Add(entity);
				return entity;
			});

		Assert.IsFalse(created);
		Assert.AreEqual(1L, timezone.Id);
		Assert.AreEqual(1, context.Timezones.Count());
	}

	[TestMethod]
	public void EnsureNamedEntity_ReusesExistingRecord_IrrespectiveOfCase()
	{
		using var context = BuildContext();
		context.Currencies.Add(new Currency
		{
			Id = 1,
			Name = "bits",
			BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
		});
		context.SaveChanges();

		var created = false;
		var currency = SeederRepeatabilityHelper.EnsureNamedEntity(
			context.Currencies,
			"Bits",
			x => x.Name,
			() =>
			{
				created = true;
				var entity = new Currency
				{
					Id = 2,
					Name = "Bits",
					BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
				};
				context.Currencies.Add(entity);
				return entity;
			});

		Assert.IsFalse(created);
		Assert.AreEqual(1L, currency.Id);
		Assert.AreEqual(1, context.Currencies.Count());
	}

	[TestMethod]
	public void TimeSeeder_SeedData_ReusesExistingTimezoneWithDifferentCase()
	{
		using var context = BuildContext();
		context.Accounts.Add(new Account
		{
			Id = 1,
			Name = "SeederTest",
			Password = "password",
			Salt = 1,
			AccessStatus = 0,
			Email = "seeder@example.com",
			LastLoginIp = "127.0.0.1",
			FormatLength = 80,
			InnerFormatLength = 78,
			UseMxp = false,
			UseMsp = false,
			UseMccp = false,
			ActiveCharactersAllowed = 1,
			UseUnicode = true,
			TimeZoneId = "UTC",
			CultureName = "en-AU",
			RegistrationCode = string.Empty,
			IsRegistered = true,
			RecoveryCode = string.Empty,
			UnitPreference = "metric",
			CreationDate = DateTime.UtcNow,
			PageLength = 22,
			PromptType = 0,
			TabRoomDescriptions = false,
			CodedRoomDescriptionAdditionsOnNewLine = false,
			CharacterNameOverlaySetting = 0,
			AppendNewlinesBetweenMultipleEchoesPerPrompt = false,
			ActLawfully = false,
			HasBeenActiveInWeek = true,
			HintsEnabled = true,
			AutoReacquireTargets = false
		});
		context.Clocks.Add(new Clock
		{
			Id = 1,
			Definition = "<Clock><Alias>utc</Alias></Clock>",
			Seconds = 0,
			Minutes = 0,
			Hours = 0
		});
		context.Timezones.Add(new Timezone
		{
			Id = 1,
			Name = "utc",
			Description = "Existing timezone",
			ClockId = 1,
			OffsetHours = 0,
			OffsetMinutes = 0
		});
		context.SaveChanges();

		var seeder = new TimeSeeder();
		var result = seeder.SeedData(context, new Dictionary<string, string>
		{
			["secondsmultiplier"] = "2",
			["mode"] = "gregorian-uk",
			["startyear"] = "2010"
		});

		Assert.AreEqual("Successfully set up clock and calendar.", result);
		Assert.AreEqual(1, context.Timezones.Count(x => x.ClockId == 1));
		Assert.AreEqual("UTC", context.Timezones.Single(x => x.ClockId == 1).Name);
	}

	[TestMethod]
	public void EconomySeeder_ResolutionHelpers_AreCaseInsensitive()
	{
		using var context = BuildContext();
		context.Currencies.Add(new Currency
		{
			Id = 1,
			Name = "Bits",
			BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
		});
		context.Zones.Add(new Zone
		{
			Id = 1,
			Name = "Test Zone",
			ShardId = 1,
			Latitude = 0.0,
			Longitude = 0.0,
			Elevation = 0.0,
			AmbientLightPollution = 0.0
		});
		context.SaveChanges();

		var currency = (Currency?)typeof(EconomySeeder)
			.GetMethod("ResolveCurrencyOrNull", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [context, "bits"]);
		var zone = (Zone?)typeof(EconomySeeder)
			.GetMethod("ResolveZoneOrNull", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [context, "test zone"]);

		Assert.IsNotNull(currency);
		Assert.AreEqual(1L, currency.Id);
		Assert.IsNotNull(zone);
		Assert.AreEqual(1L, zone.Id);
	}

	[TestMethod]
	public void LawSeeder_CurrencyValidator_AcceptsCaseInsensitivePrefixes()
	{
		using var context = BuildContext();
		context.Currencies.Add(new Currency
		{
			Id = 1,
			Name = "Bits",
			BaseCurrencyToGlobalBaseCurrencyConversion = 1.0m
		});
		context.SaveChanges();

		var validator = new LawSeeder().SeederQuestions.Single(x => x.Id == "currency").Validator;
		var result = validator("bit", context);

		Assert.IsTrue(result.Success);
		Assert.AreEqual(string.Empty, result.error);
	}

	[TestMethod]
	public void LawSeeder_AddWitnessProfile_DoesNotFlushIncompleteWitnessProfiles()
	{
		using var context = BuildWitnessProfileGuardContext();
		var authority = new LegalAuthority
		{
			Id = 1,
			Name = "Test Authority"
		};
		context.LegalAuthorities.Add(authority);
		context.SaveChanges();

		var seeder = new LawSeeder();
		SetSeederProperty(seeder, nameof(LawSeeder.Context), context);
		SetSeederProperty(seeder, nameof(LawSeeder.Authority), authority);
		SetSeederProperty(seeder, nameof(LawSeeder.AuthorityName), authority.Name);
		SetSeederProperty(
			seeder,
			nameof(LawSeeder.Classes),
			new Dictionary<string, LegalClass>(StringComparer.OrdinalIgnoreCase));

		typeof(LawSeeder)
			.GetMethod("AddWitnessProfile", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(
				seeder,
				[
					"Always Report Everything",
					1.0,
					1.0,
					new[] { TimeOfDay.Morning, TimeOfDay.Afternoon },
					Array.Empty<string>(),
					Array.Empty<string>()
				]);

		context.SaveChanges();

		var profile = context.WitnessProfiles.Single();
		Assert.IsNotNull(profile.IdentityKnownProg);
		Assert.IsNotNull(profile.ReportingMultiplierProg);
		Assert.AreEqual(2, context.FutureProgs.Count());
		Assert.AreEqual(1, context.WitnessProfiles.Count());
	}
}
