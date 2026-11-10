#nullable enable

using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Climate;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Network;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private const int DiscordReportLimit = 100;

	private string ExecuteReport(DiscordRemoteRequest request, IAccount? account)
	{
		using var arguments = ParseRemoteArguments(request.Arguments);
		var root = arguments.RootElement;
		return request.Operation switch
		{
			"game.info" => GetPublishedSetting("game.info") ?? "Game information has not been published.",
			"game.helpsearch" => PublicHelpSearch(RemoteString(root, "query", required: true)),
			"game.time" => PublicTime(RemoteString(root, "place", required: true)),
			"game.calendar" => PublicCalendar(RemoteString(root, "calendar", required: true),
				RemoteInt(root, "year")),
			"game.weather" => PublicWeather(RemoteString(root, "region", required: true)),
			"game.chargenguide" => PublicChargenGuide(RemoteString(root, "topic")),
			"account.resources" => AccountResources(account),
			"account.applications" => AccountApplications(account),
			"account.options" => AccountOptions(account, RemoteLong(root, "draftId")),
			"admin.applications" => ApplicationQueue(),
			"admin.accountsearch" => AccountSearch(RemoteString(root, "query", required: true)),
			"admin.accountnotes" => AccountNotes(RemoteLong(root, "accountId"), RemoteString(root, "account")),
			"admin.addnote" => AddAccountNote(account, RemoteLong(root, "accountId"),
				RemoteString(root, "account"), RemoteString(root, "subject", required: true),
				RemoteString(root, "text", required: true)),
			"admin.resources" => AdjustAccountResource(account, RemoteLong(root, "accountId"),
				RemoteString(root, "account"), RemoteString(root, "resource", required: true),
				RemoteInt(root, "amount"), RemoteString(root, "reason", required: true)),
			"admin.ban" => BanAccount(account, RemoteLong(root, "accountId"),
				RemoteString(root, "account"), RemoteString(root, "reason", required: true)),
			"admin.unban" => UnbanAccount(account, RemoteLong(root, "accountId"),
				RemoteString(root, "account"), RemoteString(root, "reason", required: true)),
			"admin.connections" => ConnectedUsers(),
			"admin.performance" => PerformanceReport(),
			"admin.census" => RuntimeCensus(),
			"admin.resourceaudit" => ResourceAudit(RemoteString(root, "resource")),
			"admin.maintenance" => Maintenance(RemoteString(root, "setting"), RemoteString(root, "value")),
			"economy.summary" => EconomySummary(RemoteLong(root, "zoneId")),
			"economy.money" => EconomyMoney(RemoteLong(root, "zoneId"), RemoteLong(root, "currencyId")),
			"economy.holdings" => EconomyHoldings(RemoteLong(root, "zoneId"), RemoteLong(root, "currencyId")),
			"economy.volume" => EconomyVolume(RemoteString(root, "window", required: true),
				RemoteLong(root, "zoneId"), RemoteLong(root, "currencyId"), RemoteLong(root, "periodId")),
			"economy.income" => EconomyIncome(RemoteString(root, "window", required: true),
				RemoteLong(root, "zoneId"), RemoteLong(root, "currencyId"), RemoteLong(root, "periodId")),
			"economy.employment" => EconomyEmployment(RemoteLong(root, "zoneId")),
			"economy.trends" => EconomyTrends(RemoteString(root, "metric"),
				RemoteString(root, "classification"), RemoteLong(root, "zoneId"),
				RemoteLong(root, "currencyId"), RemoteInt(root, "count")),
			"economy.wealth" => EconomyWealth(RemoteLong(root, "zoneId"), RemoteLong(root, "currencyId"),
				RemoteInt(root, "count")),
			"economy.risks" => EconomyRisks(RemoteLong(root, "zoneId")),
			"economy.snapshotstatus" => EconomySnapshotStatus(),
			"economy.snapshot" => EconomySnapshot(RemoteLong(root, "zoneId")),
			"economy.configure" => EconomyConfigure(RemoteString(root, "setting"), RemoteString(root, "value")),
			_ => throw new ArgumentException("That report operation is not supported.")
		};
	}

	internal string ReportConfirmationFingerprint(DiscordRemoteRequest request, IAccount? actor)
	{
		using var arguments = ParseRemoteArguments(request.Arguments);
		var root = arguments.RootElement;
		if (request.Operation == "admin.maintenance")
		{
			var setting = RemoteString(root, "setting").ToLowerInvariant();
			if (setting is not ("login" or "accountlogin" or "chargen"))
			{
				throw new ArgumentException("Choose login, accountlogin, or chargen.");
			}

			if (!bool.TryParse(RemoteString(root, "value", required: true), out _))
			{
				throw new ArgumentException("Maintenance value must be true or false.");
			}

			return JsonSerializer.Serialize(new { _world.MaintenanceMode });
		}

		if (request.Operation == "economy.snapshot")
		{
			var analytics = _world.EconomyAnalytics;
			return JsonSerializer.Serialize(new { analytics.SnapshotsEnabled, analytics.LastSnapshotUtc });
		}

		if (request.Operation == "economy.configure")
		{
			var setting = RemoteString(root, "setting", required: true).ToLowerInvariant();
			var value = RemoteString(root, "value", required: true);
			var analytics = _world.EconomyAnalytics;
			var current = setting switch
			{
				"snapshots" => analytics.SnapshotsEnabled.ToString(),
				"interval" => analytics.SnapshotInterval.ToString("c", CultureInfo.InvariantCulture),
				"rollover" => analytics.RolloverSnapshotsEnabled.ToString(),
				"currency" => analytics.GlobalDisplayCurrency.Id.ToString(CultureInfo.InvariantCulture),
				_ => throw new ArgumentException("Choose snapshots, interval, rollover or currency.")
			};
			if (setting == "snapshots" && !bool.TryParse(value, out _) ||
			    setting == "rollover" && !bool.TryParse(value, out _) ||
			    setting == "interval" && (!TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var interval) || interval < TimeSpan.FromHours(1)) ||
			    setting == "currency" && _world.Currencies.GetByName(value) is null)
			{
				throw new ArgumentException("The economy configuration value is invalid.");
			}

			return JsonSerializer.Serialize(new { setting, current });
		}

		if (request.Operation is not ("admin.addnote" or "admin.resources" or "admin.ban" or "admin.unban"))
		{
			return string.Empty;
		}

		if (actor is null)
		{
			throw new ArgumentException("Link a staff account before confirming this operation.");
		}

		var targetId = ResolveTargetAccountId(RemoteLong(root, "accountId"), RemoteString(root, "account"));
		if (request.Operation == "admin.addnote" &&
		    (RemoteString(root, "subject", required: true).Trim().Length is 0 or > 200 ||
		     RemoteString(root, "text", required: true).Trim().Length is 0 or > 4000))
		{
			throw new ArgumentException("A note needs a subject (up to 200 characters) and text (up to 4,000 characters).");
		}

		if (request.Operation is "admin.resources" or "admin.ban" or "admin.unban")
		{
			_ = ValidateReason(RemoteString(root, "reason", required: true));
		}
		using (new FMDB())
		{
			var target = FMDB.Context.Accounts.AsNoTracking().FirstOrDefault(x => x.Id == targetId) ??
				throw new ArgumentException("That account does not exist.");
			if (request.Operation == "admin.unban" && (AccountStatus)target.AccessStatus != AccountStatus.Suspended)
			{
				throw new ArgumentException("That account is not suspended.");
			}
			if (request.Operation == "admin.resources")
			{
				var resourceName = RemoteString(root, "resource", required: true);
				var resource = _world.ChargenResources.FirstOrDefault(x =>
					x.Name.Equals(resourceName, StringComparison.OrdinalIgnoreCase) ||
					x.Alias.Equals(resourceName, StringComparison.OrdinalIgnoreCase)) ??
					throw new ArgumentException("That chargen resource does not exist.");
				var amount = RemoteInt(root, "amount") ?? 1;
				if (amount == 0 || Math.Abs((long)amount) > 100000 ||
				    (actor.Authority?.Level ?? PermissionLevel.Guest) < resource.PermissionLevelRequiredToAward ||
				    amount > 0 && amount > resource.MaximumNumberAwardedPerAward)
				{
					throw new ArgumentException("The resource adjustment is not permitted or its amount is out of range.");
				}
				var balance = FMDB.Context.AccountsChargenResources.AsNoTracking()
					.FirstOrDefault(x => x.AccountId == targetId && x.ChargenResourceId == resource.Id);
				if (amount > 0 && balance is not null &&
				    DateTime.UtcNow - balance.LastAwardDate <= resource.MinimumTimeBetweenAwards &&
				    (actor.Authority?.Level ?? PermissionLevel.Guest) < resource.PermissionLevelRequiredToCircumventMinimumTime)
				{
					throw new ArgumentException($"That account was awarded {resource.PluralName} too recently.");
				}
				return JsonSerializer.Serialize(new { targetId, target.AccessStatus, Balance = balance?.Amount ?? 0.0,
					LastAward = balance?.LastAwardDate, resource.Id, resource.MaximumNumberAwardedPerAward,
					MinimumTime = resource.MinimumTimeBetweenAwards.Ticks });
			}

		if (request.Operation == "admin.ban")
		{
			if (target.Id == actor.Id || target.AuthorityGroup.AuthorityLevel >= (int)(actor.Authority?.Level ?? PermissionLevel.Guest))
			{
				throw new ArgumentException("You cannot ban yourself or an account with equal or higher authority.");
			}
			if ((AccountStatus)target.AccessStatus != AccountStatus.Normal)
			{
				throw new ArgumentException("That account is already suspended.");
			}
		}

		return JsonSerializer.Serialize(new { target.Id, target.Name, target.AccessStatus, target.AuthorityGroup.AuthorityLevel });
		}
	}

	private static JsonDocument ParseRemoteArguments(string? arguments)
	{
		if (string.IsNullOrWhiteSpace(arguments))
		{
			return JsonDocument.Parse("{}");
		}

		try
		{
			var document = JsonDocument.Parse(arguments, new JsonDocumentOptions { MaxDepth = 8 });
			if (document.RootElement.ValueKind != JsonValueKind.Object)
			{
				document.Dispose();
				throw new ArgumentException("Arguments must be a JSON object.");
			}

			return document;
		}
		catch (JsonException)
		{
			throw new ArgumentException("Arguments are malformed.");
		}
	}

	private static string RemoteString(JsonElement root, string name, bool required = false)
	{
		if (!root.TryGetProperty(name, out var value) || value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
		{
			if (required)
			{
				throw new ArgumentException($"The {name} argument is required.");
			}

			return string.Empty;
		}

		if (value.ValueKind != JsonValueKind.String)
		{
			throw new ArgumentException($"The {name} argument must be text.");
		}

		return value.GetString() ?? string.Empty;
	}

	private static long? RemoteLong(JsonElement root, string name)
	{
		if (!root.TryGetProperty(name, out var value) || value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
		{
			return null;
		}

		if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt64(out var result))
		{
			throw new ArgumentException($"The {name} argument must be a whole number.");
		}

		return result;
	}

	private static int? RemoteInt(JsonElement root, string name)
	{
		var value = RemoteLong(root, name);
		if (value is null)
		{
			return null;
		}

		if (value < int.MinValue || value > int.MaxValue)
		{
			throw new ArgumentException($"The {name} argument is out of range.");
		}

		return (int)value.Value;
	}

	private string PublicHelpSearch(string query)
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			throw new ArgumentException("Enter a word or phrase to search public help.");
		}

		query = query.Trim();
		var matches = _world.Helpfiles
			.Where(x => x.CanView(null) &&
			            (x.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
			             x.Keywords.Any(y => y.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
			             x.TagLine.Contains(query, StringComparison.OrdinalIgnoreCase)))
			.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.Take(DiscordReportLimit)
			.Select(x => $"• {x.Name} — {x.TagLine}")
			.ToList();
		return matches.Count == 0 ? "No public help matched that search." : string.Join(Environment.NewLine, matches);
	}

	private string PublicTime(string place)
	{
		var places = ReadPublishedMap("game.places");
		if (!long.TryParse(ResolvePublishedValue(places, place), NumberStyles.None,
			CultureInfo.InvariantCulture, out var roomId) || _world.Rooms.Get(roomId) is not { } room)
		{
			throw new ArgumentException("That public time location is unavailable.");
		}

		var calendars = room.Calendars.ToList();
		if (calendars.Count == 0)
		{
			return $"No calendar is configured for {room.Name}.";
		}

		return string.Join(Environment.NewLine, calendars.Select(x =>
			$"{x.FullName}: {x.CurrentDateTime.GetDateTimeString()}"));
	}

	private string PublicCalendar(string calendarName, int? requestedYear)
	{
		var calendars = ReadPublishedMap("game.calendars");
		if (!long.TryParse(ResolvePublishedValue(calendars, calendarName), NumberStyles.None,
			CultureInfo.InvariantCulture, out var calendarId) || _world.Calendars.Get(calendarId) is not { } calendar)
		{
			throw new ArgumentException("That calendar is not published.");
		}

		var yearNumber = requestedYear ?? calendar.CurrentDate.Year;
		if (yearNumber is < -100000 or > 100000)
		{
			throw new ArgumentException("Choose a year between -100,000 and 100,000.");
		}

		var year = calendar.CreateYear(yearNumber);
		var builder = new StringBuilder();
		builder.AppendLine($"{calendar.FullName} — year {yearNumber}");
		builder.AppendLine($"Weekdays: {string.Join(", ", calendar.Weekdays)}");
		foreach (var month in year.Months)
		{
			builder.AppendLine($"{month.FullName}: {month.Days} days");
		}

		return builder.ToString().TrimEnd();
	}

	private string PublicWeather(string region)
	{
		var places = ReadPublishedMap("game.places");
		if (!long.TryParse(ResolvePublishedValue(places, region), NumberStyles.None,
			CultureInfo.InvariantCulture, out var roomId) || _world.Rooms.Get(roomId) is not { } room)
		{
			throw new ArgumentException("That public weather region is unavailable.");
		}

		var controller = room.WeatherController;
		if (controller is null)
		{
			throw new ArgumentException("That public region has no current weather.");
		}

		return $"{room.Name}: {controller.DescribeCurrentWeather} Current season: {controller.CurrentSeason?.DisplayName ?? "unknown"}.";
	}

	private string PublicChargenGuide(string topic)
	{
		var guide = ReadPublishedMap("game.chargenguide");
		if (guide.Count == 0)
		{
			return "Character creation guidance has not been published.";
		}

		if (string.IsNullOrWhiteSpace(topic))
		{
			return $"Published topics: {string.Join(", ", guide.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))}";
		}

		var helpName = ResolvePublishedValue(guide, topic);
		var help = _world.Helpfiles.FirstOrDefault(x =>
			x.CanView(null) && x.Name.Equals(helpName, StringComparison.OrdinalIgnoreCase));
		return help is null
			? "That character creation topic is not published."
			: $"{help.Name}{Environment.NewLine}{help.TagLine}{Environment.NewLine}{Environment.NewLine}{help.PublicText}";
	}

	private Dictionary<string, string> ReadPublishedMap(string key)
	{
		var value = GetPublishedSetting(key);
		if (string.IsNullOrWhiteSpace(value))
		{
			return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		try
		{
			using var document = JsonDocument.Parse(value);
			if (document.RootElement.ValueKind != JsonValueKind.Object)
			{
				throw new JsonException("Expected an object.");
			}

			return document.RootElement.EnumerateObject()
				.Where(x => x.Value.ValueKind is JsonValueKind.String or JsonValueKind.Number)
				.ToDictionary(x => x.Name, x => x.Value.ToString(), StringComparer.OrdinalIgnoreCase);
		}
		catch (JsonException)
		{
			throw new InvalidOperationException($"The published {key} setting contains invalid JSON.");
		}
	}

	private static string? ResolvePublishedValue(IReadOnlyDictionary<string, string> values, string key)
	{
		return values.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
	}

	private string AccountResources(IAccount? account)
	{
		if (account is null)
		{
			throw new ArgumentException("Link your MUD account to view its resources.");
		}

		using (new FMDB())
		{
			var rows = FMDB.Context.AccountsChargenResources.AsNoTracking()
			.Where(x => x.AccountId == account.Id)
				.OrderBy(x => x.ChargenResource.Name)
				.Select(x => new { x.ChargenResource.Name, x.Amount, x.LastAwardDate })
				.ToList();
			return rows.Count == 0
				? "Your account has no recorded chargen-resource balances."
				: string.Join(Environment.NewLine, rows.Select(x =>
					$"{x.Name}: {x.Amount.ToString("N2", CultureInfo.InvariantCulture)} (last award {x.LastAwardDate:u})"));
		}
	}

	private string AccountApplications(IAccount? account)
	{
		if (account is null)
		{
			throw new ArgumentException("Link your MUD account to view its applications.");
		}

		using (new FMDB())
		{
			var rows = FMDB.Context.Chargens.AsNoTracking()
				.Where(x => x.AccountId == account.Id)
				.OrderByDescending(x => x.SubmitTime)
			.Take(DiscordReportLimit)
			.Select(x => new { x.Id, x.Name, x.Status, x.SubmitTime })
			.ToList();
			return rows.Count == 0
				? "Your account has no character applications."
				: string.Join(Environment.NewLine, rows.Select(x =>
					$"#{x.Id} — {x.Name} — {((ChargenState)x.Status).DescribeEnum()} — {(x.SubmitTime.HasValue ? x.SubmitTime.Value.ToString("u", CultureInfo.InvariantCulture) : "not submitted")}"));
		}
	}

	private string AccountOptions(IAccount? account, long? draftId)
	{
		if (account is null)
		{
			throw new ArgumentException("Link your MUD account to inspect an application.");
		}

		if (!draftId.HasValue)
		{
			throw new ArgumentException("Choose one of your application IDs to inspect its current stage.");
		}

		using (new FMDB())
		{
			var draft = FMDB.Context.Chargens.AsNoTracking()
				.FirstOrDefault(x => x.Id == draftId.Value && x.AccountId == account.Id);
			if (draft is null)
			{
				throw new ArgumentException("That application does not belong to your linked account.");
			}

			if ((ChargenState)draft.Status != ChargenState.InProgress)
			{
				throw new ArgumentException("Only an in-progress application has creation options.");
			}

			var chargen = new MudSharp.CharacterCreation.Chargen(draft, _world, draft.Account);
			var stage = chargen.Stage;
			var storyboard = _world.ChargenStoryboard.StageScreenMap.GetValueOrDefault(stage);
			var choices = storyboard?.GetDiscordOptions(chargen)?.Take(100).ToList();
			if (choices is { Count: > 0 })
			{
				return $"Application #{draft.Id}: eligible choices for {stage.DescribeEnum()} (up to 100 shown)." +
				       Environment.NewLine + string.Join(Environment.NewLine, choices.Select((x, i) => $"{i + 1}. {x}"));
			}

			return $"Application #{draft.Id} is at {stage.DescribeEnum()}. This stage requires in-game interaction; " +
			       "Discord can show the stage but does not yet list its eligible choices. Resume the draft in game to review its controls and continue.";
		}
	}

	private string ApplicationQueue()
	{
		using (new FMDB())
		{
			var rows = FMDB.Context.Chargens.AsNoTracking()
				.Where(x => x.Status == (int)ChargenState.Submitted)
				.OrderBy(x => x.SubmitTime)
			.Take(DiscordReportLimit)
			.Select(x => new { x.Id, AccountName = x.Account.Name, x.Name, x.MinimumApprovalAuthority, x.SubmitTime })
			.ToList();
			return rows.Count == 0
				? "There are no submitted applications."
				: string.Join(Environment.NewLine, rows.Select(x =>
					$"#{x.Id} — {x.Name} ({x.AccountName}) — {(x.SubmitTime.HasValue ? x.SubmitTime.Value.ToString("u", CultureInfo.InvariantCulture) : "unknown submission time")} — minimum {(x.MinimumApprovalAuthority is { } level ? ((PermissionLevel)level).DescribeEnum() : "unspecified")}"));
		}
	}

	private string AccountSearch(string query)
	{
		query = query.Trim();
		var isNumericId = DiscordRemoteService.TryParseAccountId(query, out var accountId);
		if (!isNumericId && query.Length < 2)
		{
			throw new ArgumentException("Enter at least two characters.");
		}

		using (new FMDB())
		{
			var accountQuery = FMDB.Context.Accounts.AsNoTracking();
			var rows = (isNumericId
					? accountQuery.Where(x => x.Id == accountId)
					: accountQuery.Where(x => EF.Functions.Like(x.Name, $"%{EscapeLike(query)}%")))
			.OrderBy(x => x.Name)
			.Take(DiscordReportLimit)
			.Select(x => new { x.Id, x.Name, x.LastLoginTime })
			.ToList();
			return rows.Count == 0
				? "No accounts matched."
				: string.Join(Environment.NewLine, rows.Select(x =>
					$"#{x.Id} — {x.Name} — last login {(x.LastLoginTime?.ToString("u", CultureInfo.InvariantCulture) ?? "never")}"));
		}
	}

	private string AccountNotes(long? accountId, string accountName)
	{
		var id = ResolveTargetAccountId(accountId, accountName);
		using (new FMDB())
		{
			if (!FMDB.Context.Accounts.Any(x => x.Id == id))
			{
				throw new ArgumentException("Choose a valid account ID or exact account name.");
			}

			var rows = FMDB.Context.AccountNotes.AsNoTracking()
				.Where(x => x.AccountId == id && !x.IsJournalEntry)
				.OrderByDescending(x => x.TimeStamp)
			.Take(DiscordReportLimit)
				.Select(x => new { x.Subject, x.Text, x.TimeStamp, Author = x.Author == null ? null : x.Author.Name })
			.ToList();
			return rows.Count == 0
				? "There are no staff notes for that account."
				: string.Join(Environment.NewLine + Environment.NewLine,
				rows.Select(x => $"{x.Subject} — {x.Author ?? "(unknown author)"} — {x.TimeStamp:u}{Environment.NewLine}{x.Text}"));
		}
	}

	private string ConnectedUsers()
	{
		return string.Join(Environment.NewLine, _world.Connections
			.Where(x => x.State == ConnectionState.Open)
			.OrderBy(x => x.ControlPuppet?.Account?.Name)
			.Take(DiscordReportLimit)
			.Select(x => $"{x.ControlPuppet?.Account?.Name ?? "(no account)"} — {x.State}"));
	}

	private string PerformanceReport()
	{
		var builder = new StringBuilder();
		if (_world is MudSharp.Framework.Diagnostics.IRuntimePerformanceMonitorProvider provider)
		{
			provider.RuntimePerformanceMonitor.AppendReport(builder, CultureInfo.InvariantCulture);
		}
		else
		{
			builder.AppendLine("This gameworld does not provide runtime performance diagnostics.");
		}
		_world.HeartbeatManager.AppendPerformanceReport(builder);
		return builder.ToString().TrimEnd();
	}

	private string RuntimeCensus()
	{
		var builder = new StringBuilder();
		builder.AppendLine($"Actors: {_world.Actors.Count()}");
		builder.AppendLine($"Cached actors: {_world.CachedActors.Count()}");
		builder.AppendLine($"NPCs: {_world.NPCs.Count()}");
		builder.AppendLine($"Detached bodies: {_world.Bodies.Count()}");
		builder.AppendLine($"Attached bodies: {_world.Actors.Concat(_world.CachedActors).Concat(_world.NPCs)
			.Select(x => x.Body).Where(x => x is not null).Distinct(ReferenceEqualityComparer.Instance).Count()}");
		builder.AppendLine($"Rooms: {_world.Rooms.Count()}");
		builder.AppendLine($"Items: {_world.Items.Count()}");
		builder.AppendLine($"Main schedules: {_world.Scheduler.ScheduleCount}");
		builder.AppendLine($"Effect schedules: {_world.EffectScheduler.ScheduleCount}");
		_world.HeartbeatManager.AppendPerformanceReport(builder);
		return builder.ToString().TrimEnd();
	}

	private string ResourceAudit(string resource)
	{
		using (new FMDB())
		{
			var query = FMDB.Context.AccountsChargenResources.AsNoTracking();
			if (!string.IsNullOrWhiteSpace(resource))
			{
				query = query.Where(x => x.ChargenResource.Name == resource ||
				                         x.ChargenResource.Alias == resource);
			}

			var rows = query
				.GroupBy(x => new { x.ChargenResource.Name, x.ChargenResource.Alias })
				.Select(x => new
				{
					x.Key.Name,
					Accounts = x.Count(),
					Total = x.Sum(y => y.Amount),
					LastAward = x.Max(y => (DateTime?)y.LastAwardDate)
				})
			.OrderBy(x => x.Name)
			.Take(DiscordReportLimit)
			.ToList();
			return rows.Count == 0
				? "No resource balances matched."
				: string.Join(Environment.NewLine, rows.Select(x =>
					$"{x.Name}: {x.Total.ToString("N2", CultureInfo.InvariantCulture)} across {x.Accounts} accounts; latest recorded award {(x.LastAward?.ToString("u", CultureInfo.InvariantCulture) ?? "never")}"));
		}
	}

	private string Maintenance(string setting, string value)
	{
		if (string.IsNullOrWhiteSpace(setting) || string.IsNullOrWhiteSpace(value))
		{
			return $"Maintenance restrictions: {_world.MaintenanceMode.DescribeEnum()}";
		}

		if (!bool.TryParse(value, out var enabled))
		{
			throw new ArgumentException("Maintenance value must be true or false.");
		}

		var flag = setting.ToLowerInvariant() switch
		{
			"login" => MaintenanceModeSetting.NoLogin,
			"accountlogin" => MaintenanceModeSetting.NoAccountLogin,
			"chargen" => MaintenanceModeSetting.NoChargen,
			_ => throw new ArgumentException("Choose login, accountlogin, or chargen.")
		};
		_world.MaintenanceMode = enabled ? _world.MaintenanceMode | flag : _world.MaintenanceMode & ~flag;
		_world.SystemMessage(setting.ToLowerInvariant() switch
		{
			"login" => enabled ? "Character login has been put into maintenance mode." : "Character login maintenance restrictions have been lifted.",
			"accountlogin" => enabled ? "Account login has been put into maintenance mode." : "Account login maintenance restrictions have been lifted.",
			_ => enabled ? "Character creation has been put into maintenance mode." : "Character creation maintenance restrictions have been lifted."
		}, true);
		return $"Maintenance restrictions: {_world.MaintenanceMode.DescribeEnum()}";
	}

	private string EconomySummary(long? zoneId)
	{
		var service = _world.EconomyAnalytics;
		var currency = service.GlobalDisplayCurrency;
		var holdings = service.GetCurrentHoldings(zoneId);
		var volume = service.GetVolume(EconomyQueryWindowKind.RealDay, zoneId);
		var employment = service.GetEmploymentMarket(zoneId);
		var liquid = holdings.Where(x => x.Metric is EconomyHoldingMetric.PhysicalCash or EconomyHoldingMetric.BankDeposits or EconomyHoldingMetric.VirtualBalance).Sum(x => x.GlobalBaseValue);
		var pcWealth = holdings.Where(x => x.ControlBucket is EconomicControlBucket.DirectPc or EconomicControlBucket.SharedPcControlled && x.Metric is not EconomyHoldingMetric.BankDebt and not EconomyHoldingMetric.BankReserves).Sum(x => x.GlobalBaseValue);
		var deposits = holdings.Where(x => x.Metric == EconomyHoldingMetric.BankDeposits).Sum(x => x.GlobalBaseValue);
		var reserves = holdings.Where(x => x.Metric == EconomyHoldingMetric.BankReserves).Sum(x => x.GlobalBaseValue);
		var trend = service.GetTrends(EconomyHoldingMetric.BroadMoneySupply, null, zoneId, count: 2);
		var supplyDirection = trend.Count < 2 ? "insufficient history" : trend[0].GlobalBaseValue > trend[1].GlobalBaseValue ? "rising" : trend[0].GlobalBaseValue < trend[1].GlobalBaseValue ? "falling" : "flat";
		return string.Join(Environment.NewLine,
			"Economy summary (global-base values)",
			$"Broad liquid supply: {EconomyDisplayValue(currency, liquid)}",
			$"PC-controlled wealth including property: {EconomyDisplayValue(currency, pcWealth)}",
			$"Bank reserve coverage: {(deposits > 0 ? reserves / deposits : 1M):P1}",
			$"Last 24h exchange: {EconomyDisplayValue(currency, volume.ExchangeGlobalBaseValue)}",
			$"Last 24h movement: {EconomyDisplayValue(currency, volume.MovementGlobalBaseValue)}",
			$"Supply trend: {supplyDirection}",
			$"PC-accessible jobs: {employment.Openings.Count(x => x.AcceptsPcs)}",
			$"NPC workers employed/free: {employment.EmployedNpcs}/{employment.FreeNpcs}",
			$"Last 24h PC income: {EconomyDisplayValue(currency, volume.PcIncome.Values.Sum())}",
			$"Activity coverage starts: {service.ActivityCoverageStartUtc?.ToString("u", CultureInfo.InvariantCulture) ?? "no events recorded"}",
			$"Active risks: {service.GetRisks(holdings, zoneId).Count}");
	}

	private string EconomyMoney(long? zoneId, long? currencyId)
	{
		var holdings = _world.EconomyAnalytics.GetCurrentHoldings(zoneId, currencyId);
		var displayCurrency = _world.EconomyAnalytics.GlobalDisplayCurrency;
		return holdings
			.GroupBy(x => new { x.CurrencyId, x.Metric, x.ControlBucket })
			.OrderBy(x => x.Key.CurrencyId).ThenBy(x => x.Key.Metric).ThenBy(x => x.Key.ControlBucket)
			.Select(x => $"{_world.Currencies.Get(x.Key.CurrencyId)?.Name ?? x.Key.CurrencyId.ToString(CultureInfo.InvariantCulture)} | {x.Key.Metric.DescribeEnum()} | {x.Key.ControlBucket.DescribeEnum()} | {x.Sum(y => y.Amount):N2} native | {EconomyDisplayValue(displayCurrency, x.Sum(y => y.GlobalBaseValue))} | {x.Count()} records")
			.DefaultIfEmpty("No current holdings matched.")
			.Take(DiscordReportLimit)
			.Aggregate(new StringBuilder(), (builder, line) => builder.AppendLine(line)).ToString().TrimEnd();
	}

	private string EconomyHoldings(long? zoneId, long? currencyId)
	{
		var displayCurrency = _world.EconomyAnalytics.GlobalDisplayCurrency;
		var holdings = _world.EconomyAnalytics.GetCurrentHoldings(zoneId, currencyId)
			.OrderByDescending(x => x.GlobalBaseValue)
			.Take(DiscordReportLimit)
			.Select(x => $"{_world.Currencies.Get(x.CurrencyId)?.Name ?? x.CurrencyId.ToString(CultureInfo.InvariantCulture)} | {x.Metric.DescribeEnum()} | {x.ControlBucket.DescribeEnum()} | {x.Amount:N2} native | {EconomyDisplayValue(displayCurrency, x.GlobalBaseValue)} | {x.Description ?? "(no custody detail)"}")
			.ToList();
		return holdings.Count == 0 ? "No current holdings matched." : string.Join(Environment.NewLine, holdings);
	}

	private string EconomyVolume(string window, long? zoneId, long? currencyId, long? periodId)
	{
		var queryWindow = ParseWindow(window);
		if (queryWindow == EconomyQueryWindowKind.FinancialPeriod && !periodId.HasValue)
		{
			throw new ArgumentException("Supply periodId for a financial-period window.");
		}
		var result = _world.EconomyAnalytics.GetVolume(queryWindow, zoneId, currencyId, periodId);
		var displayCurrency = _world.EconomyAnalytics.GlobalDisplayCurrency;
		return string.Join(Environment.NewLine,
			$"Coverage starts: {result.CoverageStartUtc:u}",
			$"Window: {result.WindowStartUtc:u} to {result.WindowEndUtc:u}",
			$"Exchange: {EconomyDisplayValue(displayCurrency, result.ExchangeGlobalBaseValue)}",
			$"Gross movement: {EconomyDisplayValue(displayCurrency, result.MovementGlobalBaseValue)}",
			$"Events: {result.EventCount:N0}");
	}

	private string EconomyIncome(string window, long? zoneId, long? currencyId, long? periodId)
	{
		var queryWindow = ParseWindow(window);
		if (queryWindow == EconomyQueryWindowKind.FinancialPeriod && !periodId.HasValue)
		{
			throw new ArgumentException("Supply periodId for a financial-period window.");
		}
		var result = _world.EconomyAnalytics.GetVolume(queryWindow, zoneId, currencyId, periodId);
		var displayCurrency = _world.EconomyAnalytics.GlobalDisplayCurrency;
		return string.Join(Environment.NewLine,
			"Settled income to ordinary PCs; no historical backfill is implied.",
			$"Coverage starts: {result.CoverageStartUtc:u}",
			$"Jobs: {EconomyDisplayValue(displayCurrency, result.PcIncome.Where(x => x.Key.Activity == EconomicActivityType.Wage).Sum(x => x.Value))}",
			$"Projects: {EconomyDisplayValue(displayCurrency, result.PcIncome.Where(x => x.Key.Activity == EconomicActivityType.ProjectPayment).Sum(x => x.Value))}",
			$"Clan paydays: {EconomyDisplayValue(displayCurrency, result.PcIncome.Where(x => x.Key.Activity == EconomicActivityType.ClanPayment).Sum(x => x.Value))}",
			$"Total: {EconomyDisplayValue(displayCurrency, result.PcIncome.Values.Sum())}");
	}

	private string EconomyEmployment(long? zoneId)
	{
		var market = _world.EconomyAnalytics.GetEmploymentMarket(zoneId);
		return string.Join(Environment.NewLine,
			$"NPC workers employed/free: {market.EmployedNpcs:N0}/{market.FreeNpcs:N0}",
			$"Openings: {market.Openings.Count:N0}; PC-accessible: {market.Openings.Count(x => x.AcceptsPcs):N0}",
			$"Hosts with vacancies: {market.Hosts.Count(x => x.VacantPostings > 0):N0}",
			"Employment roles:",
			string.Join(Environment.NewLine, market.Roles.OrderBy(x => x.Name).Take(DiscordReportLimit)
				.Select(x => $"{x.Name}: {x.EmployedNpcs} workers, {x.VacantPostings} postings, {x.VacantPositions} positions")));
	}

	private string EconomyTrends(string metric, string classification, long? zoneId, long? currencyId, int? count)
	{
		var parsedMetric = ParseOptionalEnum<EconomyHoldingMetric>(metric, "metric");
		var parsedClassification = ParseOptionalEnum<EconomicVolumeClassification>(classification, "classification");
		var countMetric = parsedMetric is { } selectedMetric && selectedMetric >= EconomyHoldingMetric.PcOpenJobPostings;
		if (countMetric && currencyId.HasValue)
		{
			throw new ArgumentException("Count metrics cannot be filtered by currency.");
		}

		var points = _world.EconomyAnalytics.GetTrends(parsedMetric, parsedClassification, zoneId, currencyId,
			Math.Clamp(count ?? 30, 1, DiscordReportLimit));
		var displayCurrency = _world.EconomyAnalytics.GlobalDisplayCurrency;
		return points.Count == 0
			? "No historical economy snapshots match."
			: string.Join(Environment.NewLine, points.Select(x =>
				$"#{x.SnapshotId} {x.RealDateTimeUtc:u} zone={x.EconomicZoneId?.ToString(CultureInfo.InvariantCulture) ?? "all"} period={x.FinancialPeriodId?.ToString(CultureInfo.InvariantCulture) ?? "none"} value={FormatEconomyTrendValue(parsedMetric, x.GlobalBaseValue, displayCurrency)}"));
	}

	internal static string FormatEconomyTrendValue(EconomyHoldingMetric? metric, decimal value,
		MudSharp.Economy.Currency.ICurrency displayCurrency)
	{
		return metric is { } countMetric && countMetric >= EconomyHoldingMetric.PcOpenJobPostings
			? value.ToString("N0", CultureInfo.InvariantCulture)
			: EconomyDisplayValue(displayCurrency, value);
	}

	private string EconomyWealth(long? zoneId, long? currencyId, int? count)
	{
		var displayCurrency = _world.EconomyAnalytics.GlobalDisplayCurrency;
		var top = _world.EconomyAnalytics.GetCurrentHoldings(zoneId, currencyId)
			.Where(x => x.ControlBucket is EconomicControlBucket.DirectPc or EconomicControlBucket.SharedPcControlled)
			.Where(x => x.Metric is not EconomyHoldingMetric.BankDebt and not EconomyHoldingMetric.BankReserves)
			.GroupBy(x => new { x.ControllerType, x.ControllerId, x.ControlBucket })
			.Select(x => new
			{
				x.Key.ControllerType,
				x.Key.ControllerId,
				x.Key.ControlBucket,
				Value = x.Sum(y => y.GlobalBaseValue)
			})
			.OrderByDescending(x => x.Value)
			.Take(Math.Clamp(count ?? 10, 1, DiscordReportLimit))
			.ToList();
		return top.Count == 0 ? "No PC-controlled custody records matched." :
			"Grouped by controller; shared control can represent the same household more than once." +
			Environment.NewLine + string.Join(Environment.NewLine, top.Select((x, i) =>
				$"{i + 1}. {x.ControllerType ?? "unknown"} #{x.ControllerId?.ToString(CultureInfo.InvariantCulture) ?? "unknown"} ({x.ControlBucket.DescribeEnum()}): {EconomyDisplayValue(displayCurrency, x.Value)}"));
	}

	private string EconomyRisks(long? zoneId)
	{
		var risks = _world.EconomyAnalytics.GetRisks(zoneId).Take(DiscordReportLimit).ToList();
		var displayCurrency = _world.EconomyAnalytics.GlobalDisplayCurrency;
		return risks.Count == 0 ? "No economy risks are currently reported." :
			string.Join(Environment.NewLine, risks.Select(x =>
				$"{x.Code}: {x.Description}" + (x.GlobalBaseValue.HasValue ? $" ({EconomyDisplayValue(displayCurrency, x.GlobalBaseValue.Value)})" : string.Empty)));
	}

	private string EconomySnapshotStatus()
	{
		var service = _world.EconomyAnalytics;
		return string.Join(Environment.NewLine,
			$"Snapshots enabled: {service.SnapshotsEnabled}",
			$"Interval: {service.SnapshotInterval}",
			$"Rollover snapshots: {service.RolloverSnapshotsEnabled}",
			$"Last snapshot: {service.LastSnapshotUtc?.ToString("u", CultureInfo.InvariantCulture) ?? "never"}",
			$"Next periodic snapshot: {service.NextPeriodicSnapshotUtc?.ToString("u", CultureInfo.InvariantCulture) ?? "not scheduled"}",
			$"Activity coverage: {service.ActivityCoverageStartUtc?.ToString("u", CultureInfo.InvariantCulture) ?? "not established"}",
			$"Display currency: {service.GlobalDisplayCurrency.Name}");
	}

	private string EconomySnapshot(long? zoneId)
	{
		var id = _world.EconomyAnalytics.TakeSnapshot(EconomySnapshotReason.Manual, zoneId);
		return id.HasValue ? $"Economy snapshot #{id.Value} was recorded." : "The economy snapshot could not be recorded.";
	}

	private string EconomyConfigure(string setting, string value)
	{
		var service = _world.EconomyAnalytics;
		switch (setting.ToLowerInvariant())
		{
			case "snapshots":
				if (!bool.TryParse(value, out var enabled))
				{
					throw new ArgumentException("Snapshots must be set to true or false.");
				}

				service.SetSnapshotsEnabled(enabled);
				return $"Economy snapshots are {(enabled ? "enabled" : "disabled")}.";
			case "interval":
				if (!TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var interval))
				{
					throw new ArgumentException("Enter a valid interval of at least one hour.");
				}

				if (!service.TrySetSnapshotInterval(interval, out var error))
				{
					throw new ArgumentException(error);
				}

				return $"Snapshot interval is now {service.SnapshotInterval}.";
			case "rollover":
				if (!bool.TryParse(value, out var rollover))
				{
					throw new ArgumentException("Rollover must be set to true or false.");
				}

				service.SetRolloverSnapshotsEnabled(rollover);
				return $"Financial-period snapshots are {(rollover ? "enabled" : "disabled")}.";
			case "currency":
				var currency = _world.Currencies.GetByName(value) ??
				               throw new ArgumentException("That display currency does not exist.");
				if (!service.TrySetGlobalDisplayCurrency(currency, out var currencyError))
				{
					throw new ArgumentException(currencyError);
				}

				return $"Global economy values will be displayed in {currency.Name}.";
			default:
				throw new ArgumentException("Choose snapshots, interval, rollover or currency.");
		}
	}

	private static EconomyQueryWindowKind ParseWindow(string window)
	{
		return window.Trim().ToLowerInvariant() switch
		{
			"day" or "realday" => EconomyQueryWindowKind.RealDay,
			"week" or "realweek" => EconomyQueryWindowKind.RealWeek,
			"month" or "realmonth" => EconomyQueryWindowKind.RealMonth,
			"mudday" => EconomyQueryWindowKind.MudDay,
			"mudweek" => EconomyQueryWindowKind.MudWeek,
			"mudmonth" => EconomyQueryWindowKind.MudMonth,
			"period" or "financialperiod" => EconomyQueryWindowKind.FinancialPeriod,
			_ => throw new ArgumentException("Choose day, week, month, mudday, mudweek, mudmonth, or period.")
		};
	}

	private static T? ParseOptionalEnum<T>(string value, string label) where T : struct, Enum
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		return Enum.TryParse<T>(value, true, out var result) && Enum.IsDefined(result)
			? result
			: throw new ArgumentException($"That {label} is not recognised.");
	}

	private static string EconomyDisplayValue(MudSharp.Economy.Currency.ICurrency currency, decimal globalBaseValue)
	{
		return $"{(globalBaseValue / currency.BaseCurrencyToGlobalBaseCurrencyConversion):N2} {currency.Name}";
	}

	private static string EscapeLike(string value)
	{
		return value.Replace("\\", "\\\\", StringComparison.Ordinal)
			.Replace("%", "\\%", StringComparison.Ordinal)
			.Replace("_", "\\_", StringComparison.Ordinal);
	}
}
