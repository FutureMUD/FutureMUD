#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Analytics;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using TimeSpanParserUtil;

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	private const string EconomyAnalyticsHelp = @"The #3economy#0 command gives administrators a live view of money supply, exchange, movement, PC-controlled wealth, trends, and risks. Historical volume begins when the analytics ledger is deployed; old shop and bank histories are not treated as a complete backfill.

	#3economy summary [<zone>|all]#0
	#3economy money [<zone>|all] [<currency>] [details]#0
	#3economy volume real <day|week|month> [<zone>|all] [<currency>]#0
	#3economy volume mud <zone> <day|week|month|period> [<currency>]#0
	#3economy trends <supply|exchange|movement|pcwealth|reserves> [<zone>|all] [<currency>] [<count>]#0
	#3economy wealth [<zone>|all] [<currency>] [top <count>]#0
	#3economy risks [<zone>|all]#0
	#3economy snapshot [<zone>|all]#0
	#3economy config#0
	#3economy config snapshots <on|off>#0
	#3economy config interval <timespan>#0
	#3economy config rollover <on|off>#0

Running #3economy#0 without a subcommand only displays this help. Live census reports include persisted offline holdings and may take noticeable time on very large worlds, so they must be requested explicitly. Reports and manual snapshots require Admin. Configuration changes require High Admin. The minimum periodic interval is one hour; disabling snapshots preserves existing history and leaves live reports and the activity ledger running.";

	[PlayerCommand("EconomyAnalytics", "economy")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("economy", EconomyAnalyticsHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void EconomyAnalytics(ICharacter actor, string input)
	{
		var command = new StringStack(input.RemoveFirstWord());
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(EconomyAnalyticsHelp.SubstituteANSIColour());
			return;
		}

		var verb = command.PopForSwitch();
		switch (verb)
		{
			case "summary":
				EconomyAnalyticsSummary(actor, command);
				return;
			case "money":
				EconomyAnalyticsMoney(actor, command);
				return;
			case "volume":
				EconomyAnalyticsVolume(actor, command);
				return;
			case "trends":
			case "trend":
				EconomyAnalyticsTrends(actor, command);
				return;
			case "wealth":
				EconomyAnalyticsWealth(actor, command);
				return;
			case "risks":
			case "risk":
				EconomyAnalyticsRisks(actor, command);
				return;
			case "snapshot":
				EconomyAnalyticsSnapshot(actor, command);
				return;
			case "config":
				EconomyAnalyticsConfig(actor, command);
				return;
			default:
				actor.OutputHandler.Send(EconomyAnalyticsHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void EconomyAnalyticsSummary(ICharacter actor, StringStack command)
	{
		if (!TryParseAnalyticsZone(actor, command, true, out var zoneId))
		{
			return;
		}

		var service = actor.Gameworld.EconomyAnalytics;
		var holdings = service.GetCurrentHoldings(zoneId);
		var volume = service.GetVolume(EconomyQueryWindowKind.RealDay, zoneId);
		var supply = holdings
			.Where(x => x.Metric is EconomyHoldingMetric.PhysicalCash or EconomyHoldingMetric.BankDeposits or
				EconomyHoldingMetric.VirtualBalance)
			.Sum(x => x.GlobalBaseValue);
		var pcWealth = holdings
			.Where(x => (x.ControlBucket is EconomicControlBucket.DirectPc or EconomicControlBucket.SharedPcControlled) &&
			            x.Metric is not EconomyHoldingMetric.BankDebt and not EconomyHoldingMetric.BankReserves)
			.Sum(x => x.GlobalBaseValue);
		var pcControlledLiquid = holdings
			.Where(x => (x.ControlBucket is EconomicControlBucket.DirectPc or EconomicControlBucket.SharedPcControlled) &&
			            (x.Metric is EconomyHoldingMetric.PhysicalCash or EconomyHoldingMetric.BankDeposits or
				            EconomyHoldingMetric.VirtualBalance))
			.Sum(x => x.GlobalBaseValue);
		var deposits = holdings.Where(x => x.Metric == EconomyHoldingMetric.BankDeposits).Sum(x => x.GlobalBaseValue);
		var reserves = holdings.Where(x => x.Metric == EconomyHoldingMetric.BankReserves).Sum(x => x.GlobalBaseValue);
		var risks = service.GetRisks(holdings, zoneId);
		var trends = service.GetTrends(EconomyHoldingMetric.BroadMoneySupply, null, zoneId, count: 2);
		var direction = trends.Count < 2
			? "insufficient history"
			: trends[0].GlobalBaseValue > trends[1].GlobalBaseValue
				? "rising"
				: trends[0].GlobalBaseValue < trends[1].GlobalBaseValue ? "falling" : "flat";
		var sb = new StringBuilder();
		sb.AppendLine("Economy Summary".ColourName());
		sb.AppendLine($"Broad liquid supply: {supply.ToString("N2", actor).ColourValue()} global-base units");
		sb.AppendLine($"PC-controlled liquid share: {(supply > 0.0M ? pcControlledLiquid / supply : 0.0M).ToString("P1", actor).ColourValue()}");
		sb.AppendLine($"PC-controlled wealth including property: {pcWealth.ToString("N2", actor).ColourValue()} global-base units");
		sb.AppendLine($"Bank reserve coverage: {(deposits > 0.0M ? reserves / deposits : 1.0M).ToString("P1", actor).ColourValue()}");
		sb.AppendLine($"Last 24h exchange: {volume.ExchangeGlobalBaseValue.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Last 24h gross movement: {volume.MovementGlobalBaseValue.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Supply trend: {direction.ColourName()}");
		sb.AppendLine($"Snapshot collection: {service.SnapshotsEnabled.ToColouredString()}");
		sb.AppendLine($"Latest snapshot: {(service.LastSnapshotUtc?.ToString("g", actor) ?? "never").ColourValue()}");
		sb.AppendLine($"Ledger coverage begins: {(service.ActivityCoverageStartUtc?.ToString("g", actor) ?? "no events recorded").ColourValue()}");
		sb.AppendLine($"Active warnings: {risks.Count.ToString("N0", actor).Colour(risks.Count > 0 ? Telnet.Red : Telnet.Green)}");
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EconomyAnalyticsMoney(ICharacter actor, StringStack command)
	{
		if (!TryParseAnalyticsFilters(actor, command, out var zoneId, out var currencyId, out var details))
		{
			return;
		}

		var holdings = actor.Gameworld.EconomyAnalytics.GetCurrentHoldings(zoneId, currencyId);
		var rows = holdings
			.GroupBy(x => new { x.CurrencyId, x.Metric, x.ControlBucket })
			.OrderBy(x => x.Key.CurrencyId)
			.ThenBy(x => x.Key.Metric)
			.ThenBy(x => x.Key.ControlBucket)
			.Select(x => new[]
			{
				AnalyticsCurrencyName(actor, x.Key.CurrencyId),
				x.Key.Metric.DescribeEnum(),
				x.Key.ControlBucket.DescribeEnum(),
				x.Sum(y => y.Amount).ToString("N2", actor),
				x.Sum(y => y.GlobalBaseValue).ToString("N2", actor),
				x.Count().ToString("N0", actor)
			});
		var sb = new StringBuilder();
		sb.AppendLine("Current Money Holdings".ColourName());
		sb.AppendLine(StringUtilities.GetTextTable(rows,
			new[] { "Currency", "Layer", "Control", "Native", "Global Base", "Records" },
			actor.LineFormatLength, colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		if (details)
		{
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(holdings
				.OrderByDescending(x => x.GlobalBaseValue)
				.Take(100)
				.Select(x => new[]
				{
					AnalyticsCurrencyName(actor, x.CurrencyId), x.Metric.DescribeEnum(),
					x.ControlBucket.DescribeEnum(), x.Amount.ToString("N2", actor),
					x.Description ?? string.Empty
				}), new[] { "Currency", "Layer", "Control", "Native", "Custody" },
				actor.LineFormatLength, colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EconomyAnalyticsVolume(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want a real-time or MUD-time volume window?");
			return;
		}

		var clock = command.PopForSwitch();
		long? zoneId = null;
		long? currencyId = null;
		long? periodId = null;
		EconomyQueryWindowKind window;
		if (clock == "real")
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Choose day, week, or month for the real-time window.");
				return;
			}

			window = command.PopForSwitch() switch
			{
				"day" => EconomyQueryWindowKind.RealDay,
				"week" => EconomyQueryWindowKind.RealWeek,
				"month" => EconomyQueryWindowKind.RealMonth,
				_ => (EconomyQueryWindowKind)(-1)
			};
			if ((int)window < 0 || !TryParseAnalyticsFilters(actor, command, out zoneId, out currencyId, out _))
			{
				actor.OutputHandler.Send("Choose day, week, or month for the real-time window.");
				return;
			}
		}
		else if (clock == "mud")
		{
			if (command.IsFinished || !TryResolveAnalyticsZone(actor, command.PopSpeech(), false, out zoneId))
			{
				return;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Choose day, week, month, or period for the MUD-time window.");
				return;
			}

			window = command.PopForSwitch() switch
			{
				"day" => EconomyQueryWindowKind.MudDay,
				"week" => EconomyQueryWindowKind.MudWeek,
				"month" => EconomyQueryWindowKind.MudMonth,
				"period" => EconomyQueryWindowKind.FinancialPeriod,
				_ => (EconomyQueryWindowKind)(-1)
			};
			if ((int)window < 0)
			{
				actor.OutputHandler.Send("Choose day, week, month, or period for the MUD-time window.");
				return;
			}

			var zone = actor.Gameworld.EconomicZones.First(x => x.Id == zoneId);
			periodId = window == EconomyQueryWindowKind.FinancialPeriod ? zone.CurrentFinancialPeriod.Id : null;
			if (!command.IsFinished && !TryResolveAnalyticsCurrency(actor, command.PopSpeech(), out currencyId))
			{
				return;
			}
		}
		else
		{
			actor.OutputHandler.Send("Do you want a real-time or MUD-time volume window?");
			return;
		}

		var result = actor.Gameworld.EconomyAnalytics.GetVolume(window, zoneId, currencyId, periodId);
		var sb = new StringBuilder();
		sb.AppendLine("Economic Volume".ColourName());
		sb.AppendLine($"Events: {result.EventCount.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Exchange: {result.ExchangeGlobalBaseValue.ToString("N2", actor).ColourValue()} global-base units");
		sb.AppendLine($"Gross movement: {result.MovementGlobalBaseValue.ToString("N2", actor).ColourValue()} global-base units");
		sb.AppendLine($"Coverage begins: {result.CoverageStartUtc.ToString("g", actor).ColourValue()}");
		if (result.ByActivity.Count > 0)
		{
			sb.AppendLine(StringUtilities.GetTextTable(result.ByActivity
				.OrderByDescending(x => x.Value)
				.Select(x => new[] { x.Key.DescribeEnum(), x.Value.ToString("N2", actor) }),
				new[] { "Activity", "Global Base" }, actor.LineFormatLength,
				colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		}
		if (result.ByPcInvolvement.Count > 0)
		{
			sb.AppendLine(StringUtilities.GetTextTable(result.ByPcInvolvement
				.OrderByDescending(x => x.Value)
				.Select(x => new[]
				{
					x.Key == EconomicControlBucket.SharedPcControlled ? "PC involved" : "Other",
					x.Value.ToString("N2", actor)
				}), new[] { "Participation", "Global Base" }, actor.LineFormatLength,
				colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EconomyAnalyticsTrends(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which trend do you want: supply, exchange, movement, pcwealth, or reserves?");
			return;
		}

		var requested = command.PopForSwitch();
		var metric = requested switch
		{
			"supply" => EconomyHoldingMetric.BroadMoneySupply,
			"exchange" => EconomyHoldingMetric.ExchangeVolume,
			"movement" => EconomyHoldingMetric.GrossMovement,
			"pcwealth" => EconomyHoldingMetric.PcControlledWealth,
			"reserves" => EconomyHoldingMetric.BankReserves,
			_ => (EconomyHoldingMetric)(-1)
		};
		if ((int)metric < 0)
		{
			actor.OutputHandler.Send("Which trend do you want: supply, exchange, movement, pcwealth, or reserves?");
			return;
		}

		long? zoneId = null;
		long? currencyId = null;
		var count = 30;
		while (!command.IsFinished)
		{
			var token = command.PopSpeech();
			if (int.TryParse(token, out var parsedCount))
			{
				count = Math.Clamp(parsedCount, 1, 100);
				continue;
			}

			if (token.EqualTo("all"))
			{
				continue;
			}

			if (!zoneId.HasValue && TryResolveAnalyticsZone(actor, token, true, out var parsedZone, false))
			{
				zoneId = parsedZone;
				continue;
			}

			if (!currencyId.HasValue && TryResolveAnalyticsCurrency(actor, token, out var parsedCurrency))
			{
				currencyId = parsedCurrency;
				continue;
			}

			return;
		}

		var points = actor.Gameworld.EconomyAnalytics.GetTrends(metric, null, zoneId, currencyId, count);
		var sb = new StringBuilder();
		sb.AppendLine($"{requested.TitleCase()} Trend".ColourName());
		sb.AppendLine($"Snapshot collection: {actor.Gameworld.EconomyAnalytics.SnapshotsEnabled.ToColouredString()}");
		if (points.Count == 0)
		{
			sb.AppendLine("There is no matching snapshot history.");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(points.Select(x => new[]
			{
				x.RealDateTimeUtc.ToString("g", actor), x.Reason.DescribeEnum(),
				x.EconomicZoneId?.ToString("N0", actor) ?? "All", x.GlobalBaseValue.ToString("N2", actor)
			}), new[] { "Captured", "Reason", "Zone", "Global Base" }, actor.LineFormatLength,
				colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EconomyAnalyticsWealth(ICharacter actor, StringStack command)
	{
		long? zoneId = null;
		long? currencyId = null;
		var top = 10;
		while (!command.IsFinished)
		{
			var token = command.PopSpeech();
			if (token.EqualTo("top"))
			{
				if (command.IsFinished || !int.TryParse(command.PopSpeech(), out top))
				{
					actor.OutputHandler.Send("How many top controllers should be shown?");
					return;
				}

				top = Math.Clamp(top, 1, 100);
				continue;
			}

			if (token.EqualTo("all"))
			{
				continue;
			}

			if (!zoneId.HasValue && TryResolveAnalyticsZone(actor, token, true, out var parsedZone, false))
			{
				zoneId = parsedZone;
				continue;
			}

			if (!currencyId.HasValue && TryResolveAnalyticsCurrency(actor, token, out var parsedCurrency))
			{
				currencyId = parsedCurrency;
				continue;
			}

			return;
		}

		var holdings = actor.Gameworld.EconomyAnalytics.GetCurrentHoldings(zoneId, currencyId)
			.Where(x => x.Metric is not EconomyHoldingMetric.BankDebt and not EconomyHoldingMetric.BankReserves)
			.ToList();
		var direct = holdings
			.Where(x => x.ControlBucket == EconomicControlBucket.DirectPc && x.ControllerId.HasValue)
			.GroupBy(x => x.ControllerId!.Value)
			.Select(x => (Id: x.Key, Wealth: x.Sum(y => y.GlobalBaseValue)))
			.OrderByDescending(x => x.Wealth)
			.ToList();
		var shared = holdings
			.Where(x => x.ControlBucket == EconomicControlBucket.SharedPcControlled)
			.Sum(x => x.GlobalBaseValue);
		var orderedWealth = direct.Select(x => x.Wealth).OrderBy(x => x).ToList();
		var median = orderedWealth.Count == 0
			? 0.0M
			: orderedWealth.Count % 2 == 1
				? orderedWealth[orderedWealth.Count / 2]
				: (orderedWealth[orderedWealth.Count / 2 - 1] + orderedWealth[orderedWealth.Count / 2]) / 2.0M;
		var totalDirect = direct.Sum(x => x.Wealth);
		var topDecileCount = Math.Max(1, (int)Math.Ceiling(direct.Count * 0.1));
		var topDecileShare = totalDirect <= 0.0M ? 0.0M : direct.Take(topDecileCount).Sum(x => x.Wealth) / totalDirect;
		var sb = new StringBuilder();
		sb.AppendLine("PC-Controlled Wealth".ColourName());
		sb.AppendLine($"Direct PC wealth: {totalDirect.ToString("N2", actor).ColourValue()} global-base units");
		sb.AppendLine($"Shared institutions/property: {shared.ToString("N2", actor).ColourValue()} global-base units");
		sb.AppendLine($"Median direct wealth: {median.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Top-decile share: {topDecileShare.ToString("P1", actor).ColourValue()}");
		sb.AppendLine($"Gini coefficient: {EconomyAnalyticsMath.Gini(direct.Select(x => x.Wealth)).ToString("N3", actor).ColourValue()}");
		if (direct.Count > 0)
		{
			sb.AppendLine(StringUtilities.GetTextTable(direct.Take(top).Select((x, index) => new[]
			{
				(index + 1).ToString("N0", actor), x.Id.ToString("N0", actor), x.Wealth.ToString("N2", actor)
			}), new[] { "Rank", "PC ID", "Global Base" }, actor.LineFormatLength,
				colour: Telnet.Yellow, unicodeTable: actor.Account.UseUnicode));
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void EconomyAnalyticsRisks(ICharacter actor, StringStack command)
	{
		if (!TryParseAnalyticsZone(actor, command, true, out var zoneId))
		{
			return;
		}

		var risks = actor.Gameworld.EconomyAnalytics.GetRisks(zoneId);
		if (risks.Count == 0)
		{
			actor.OutputHandler.Send("No economy analytics risks are currently detected.".Colour(Telnet.Green));
			return;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(risks.Select(x => new[]
		{
			x.Code, x.Description, x.GlobalBaseValue?.ToString("N2", actor) ?? string.Empty,
			x.EconomicZoneId?.ToString("N0", actor) ?? "All"
		}), new[] { "Risk", "Description", "Global Base", "Zone" }, actor.LineFormatLength,
			colour: Telnet.Red, unicodeTable: actor.Account.UseUnicode));
	}

	private static void EconomyAnalyticsSnapshot(ICharacter actor, StringStack command)
	{
		if (!actor.Gameworld.EconomyAnalytics.SnapshotsEnabled)
		{
			actor.OutputHandler.Send("Snapshot collection is disabled. Enable it before taking manual snapshots.".ColourError());
			return;
		}

		if (!TryParseAnalyticsZone(actor, command, true, out var zoneId))
		{
			return;
		}

		var id = actor.Gameworld.EconomyAnalytics.TakeSnapshot(EconomySnapshotReason.Manual, zoneId);
		actor.OutputHandler.Send(id.HasValue
			? $"Captured economy snapshot #{id.Value.ToString("N0", actor).ColourValue()}."
			: "A snapshot could not be captured because another capture is already running.");
	}

	private static void EconomyAnalyticsConfig(ICharacter actor, StringStack command)
	{
		var service = actor.Gameworld.EconomyAnalytics;
		if (command.IsFinished)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Economy Analytics Configuration".ColourName());
			sb.AppendLine($"Snapshots enabled: {service.SnapshotsEnabled.ToColouredString()}");
			sb.AppendLine($"Periodic interval: {service.SnapshotInterval.Describe(actor).ColourValue()}");
			sb.AppendLine($"Rollover snapshots: {service.RolloverSnapshotsEnabled.ToColouredString()}");
			sb.AppendLine($"Last snapshot: {(service.LastSnapshotUtc?.ToString("g", actor) ?? "never").ColourValue()}");
			sb.AppendLine($"Next periodic snapshot: {(service.NextPeriodicSnapshotUtc?.ToString("g", actor) ?? "disabled").ColourValue()}");
			sb.AppendLine($"Ledger coverage begins: {(service.ActivityCoverageStartUtc?.ToString("g", actor) ?? "no events recorded").ColourValue()}");
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		if (!actor.IsAdministrator(PermissionLevel.HighAdmin))
		{
			actor.OutputHandler.Send("Only a High Admin may change economy analytics configuration.".ColourError());
			return;
		}

		var option = command.PopForSwitch();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What value do you want to set?");
			return;
		}

		switch (option)
		{
			case "snapshots":
				if (!TryParseOnOff(command.PopForSwitch(), out var enabled))
				{
					actor.OutputHandler.Send("Specify either on or off.");
					return;
				}

				service.SetSnapshotsEnabled(enabled);
				actor.OutputHandler.Send($"Economy snapshots are now {enabled.ToColouredString()}.");
				return;
			case "rollover":
				if (!TryParseOnOff(command.PopForSwitch(), out var rollover))
				{
					actor.OutputHandler.Send("Specify either on or off.");
					return;
				}

				service.SetRolloverSnapshotsEnabled(rollover);
				actor.OutputHandler.Send($"Financial-period rollover snapshots are now {rollover.ToColouredString()}.");
				return;
			case "interval":
				var error = string.Empty;
				if (!TimeSpanParser.TryParse(command.SafeRemainingArgument, Units.Days, Units.Minutes, out var interval) ||
				    !service.TrySetSnapshotInterval(interval, out error))
				{
					actor.OutputHandler.Send((string.IsNullOrEmpty(error)
						? "That is not a valid snapshot interval."
						: error).ColourError());
					return;
				}

				actor.OutputHandler.Send($"Periodic economy snapshots will now be taken every {interval.Describe(actor).ColourValue()}.");
				return;
			default:
				actor.OutputHandler.Send("You can configure snapshots, interval, or rollover.");
				return;
		}
	}

	private static bool TryParseOnOff(string text, out bool value)
	{
		if (text is "on" or "yes" or "true")
		{
			value = true;
			return true;
		}

		if (text is "off" or "no" or "false")
		{
			value = false;
			return true;
		}

		value = false;
		return false;
	}

	private static bool TryParseAnalyticsFilters(ICharacter actor, StringStack command, out long? zoneId,
		out long? currencyId, out bool details)
	{
		zoneId = null;
		currencyId = null;
		details = false;
		while (!command.IsFinished)
		{
			var token = command.PopSpeech();
			if (token.EqualTo("details"))
			{
				details = true;
				continue;
			}

			if (token.EqualTo("all"))
			{
				continue;
			}

			if (!zoneId.HasValue && TryResolveAnalyticsZone(actor, token, true, out var parsedZone, false))
			{
				zoneId = parsedZone;
				continue;
			}

			if (!currencyId.HasValue && TryResolveAnalyticsCurrency(actor, token, out var parsedCurrency))
			{
				currencyId = parsedCurrency;
				continue;
			}

			return false;
		}

		return true;
	}

	private static bool TryParseAnalyticsZone(ICharacter actor, StringStack command, bool allowAll,
		out long? zoneId)
	{
		if (command.IsFinished)
		{
			zoneId = null;
			return true;
		}

		return TryResolveAnalyticsZone(actor, command.SafeRemainingArgument, allowAll, out zoneId);
	}

	private static bool TryResolveAnalyticsZone(ICharacter actor, string text, bool allowAll, out long? zoneId,
		bool reportError = true)
	{
		if (allowAll && text.EqualTo("all"))
		{
			zoneId = null;
			return true;
		}

		var zone = long.TryParse(text, out var id)
			? actor.Gameworld.EconomicZones.FirstOrDefault(x => x.Id == id)
			: actor.Gameworld.EconomicZones.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			  actor.Gameworld.EconomicZones.FirstOrDefault(x => x.Name.StartsWith(text,
				  StringComparison.InvariantCultureIgnoreCase));
		if (zone is null)
		{
			if (reportError)
			{
				actor.OutputHandler.Send($"There is no economic zone identified by {text.ColourCommand()}.");
			}
			zoneId = null;
			return false;
		}

		zoneId = zone.Id;
		return true;
	}

	private static bool TryResolveAnalyticsCurrency(ICharacter actor, string text, out long? currencyId)
	{
		var currency = long.TryParse(text, out var id)
			? actor.Gameworld.Currencies.FirstOrDefault(x => x.Id == id)
			: actor.Gameworld.Currencies.FirstOrDefault(x => x.Name.EqualTo(text)) ??
			  actor.Gameworld.Currencies.FirstOrDefault(x => x.Name.StartsWith(text,
				  StringComparison.InvariantCultureIgnoreCase));
		if (currency is null)
		{
			actor.OutputHandler.Send($"There is no currency identified by {text.ColourCommand()}.");
			currencyId = null;
			return false;
		}

		currencyId = currency.Id;
		return true;
	}

	private static string AnalyticsCurrencyName(ICharacter actor, long currencyId)
	{
		return actor.Gameworld.Currencies.FirstOrDefault(x => x.Id == currencyId)?.Name ??
		       (currencyId == 0 ? "Unknown" : $"#{currencyId:N0}");
	}
}
