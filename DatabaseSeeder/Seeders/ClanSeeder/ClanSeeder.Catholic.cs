#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Community.Templates;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ClanSeeder
{
	private static IReadOnlyList<CatholicTemplateDefinition> CatholicDefinitions =>
		CatholicTemplateCatalogue.Keys.Select(x => CatholicTemplateCatalogue.Get(x)).ToList();

	private static void ValidateCatholicInstall(FuturemudDatabaseContext context)
	{
		var definitions = CatholicDefinitions;
		if (definitions.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != definitions.Count ||
		    definitions.Select(x => x.Alias).Distinct(StringComparer.OrdinalIgnoreCase).Count() != definitions.Count)
			throw new InvalidOperationException("Duplicate Catholic template installation identity.");
		if (!context.Calendars.Any() || !context.Timezones.Any(x => x.Clock.PrimaryTimezoneId == x.Id))
			throw new InvalidOperationException("Catholic clan templates require a calendar and a primary clock timezone.");
		foreach (var definition in definitions)
		{
			var existing = context.Clans.FirstOrDefault(x => x.Name == definition.Name);
			if (existing is not null)
			{
				// Missing pieces have ambiguous ownership. Never repair a customised template in place.
				var profileProg = context.FutureProgs.FirstOrDefault(x => x.FunctionName == $"ClanTemplate_{existing.Id}_Profile");
				var rankIds = context.Ranks.Where(x => x.ClanId == existing.Id).Select(x => x.Id).ToList();
				var officeIds = context.Appointments.Where(x => x.ClanId == existing.Id).Select(x => x.Id).ToList();
				bool incomplete;
				if (profileProg?.FunctionComment?.StartsWith("ClanTemplateProfile_v1\n", StringComparison.Ordinal) == true)
				{
					using var profile = JsonDocument.Parse(profileProg.FunctionComment["ClanTemplateProfile_v1\n".Length..]);
					incomplete = profile.RootElement.GetProperty("Ranks").EnumerateObject().Any(x => !rankIds.Contains(x.Value.GetInt64())) ||
						profile.RootElement.GetProperty("Appointments").EnumerateObject().Any(x => !officeIds.Contains(x.Value.GetInt64()));
				}
				else
					incomplete = definition.Ranks.Any(rank => !context.Ranks.Any(x => x.ClanId == existing.Id && x.Name == rank.Name)) ||
						definition.Appointments.Any(office => !context.Appointments.Any(x => x.ClanId == existing.Id && x.Name == office.Name));
				if (incomplete)
					throw new InvalidOperationException($"Existing {definition.Name} is incomplete; review it manually. No rows were changed.");
				continue;
			}
			if (context.Clans.Any(x => x.Alias == definition.Alias))
				throw new InvalidOperationException($"Catholic template alias {definition.Alias} is already used by another clan.");
			foreach (var office in definition.Appointments.Where(x => x.IsAppointedByElection))
				foreach (var function in new[] { "nominate", "why_not", "votes" })
				{
					var name = CatholicTemplateCatalogue.Marker(definition.Key, office.Key) + "_" + function;
					var prog = context.FutureProgs.FirstOrDefault(x => x.FunctionName == name);
					if (prog is null) continue;
					var text = function == "nominate" ? "return false" : function == "votes" ? "return 0" :
						"return \"This template must be cloned and its governance configured before use.\"";
					var type = function == "nominate" ? ProgVariableTypes.Boolean : function == "votes" ? ProgVariableTypes.Number : ProgVariableTypes.Text;
					if (prog.Category != "Clan" || prog.Subcategory != "Catholic Templates" || prog.FunctionText != text ||
					    prog.ReturnType != (long)type || prog.Public || prog.AcceptsAnyParameters ||
					    prog.StaticType != (int)FutureProgStaticType.NotStatic ||
					    !context.FutureProgsParameters.Where(x => x.FutureProgId == prog.Id).OrderBy(x => x.ParameterIndex)
						    .AsEnumerable().Select(x => (x.ParameterIndex, x.ParameterName, x.ParameterType))
						    .SequenceEqual([(0, "ch", (long)ProgVariableTypes.Character)]))
						throw new InvalidOperationException($"Conflicting Catholic profile prog {name}; review before installation.");
				}
		}
	}

	private void SetupCatholicTemplates(FuturemudDatabaseContext context)
	{
		EnsureCatholicProvisioningSuite(context);
		foreach (var definition in CatholicDefinitions)
		{
			if (context.Clans.FirstOrDefault(x => x.Name == definition.Name) is { } existing)
			{ UpgradeCatholicAddresses(context, existing); continue; }
			var clan = CreateTemplateClan(context, definition.Name, definition.Alias, definition.Description, definition.Sphere);
			var ranks = definition.Ranks.ToDictionary(x => x.Key, x => AddRank(context, clan, x.Name, x.Abbreviation,
				x.RankNumber, CatholicTemplateCatalogue.Privileges(x.PrivilegeNames), x.RankPath, x.Title, x.FameType));
			var offices = new Dictionary<string, Appointment>();
			foreach (var source in definition.Appointments)
			{
				var office = AddAppointment(context, clan, source.Name, source.Abbreviation,
					CatholicTemplateCatalogue.Privileges(source.PrivilegeNames), ranks[source.MinimumRankKey],
					source.MinimumRankToAppointKey is { } rankKey ? ranks[rankKey] : null,
					maximumSimultaneousHolders: source.MaximumSimultaneousHolders, title: source.Title, fameType: source.FameType);
				offices.Add(source.Key, office);
				office.IsAppointedByElection = source.IsAppointedByElection;
				office.ElectionTermMinutes = source.ElectionTermMinutes;
				office.ElectionLeadTimeMinutes = source.ElectionLeadTimeMinutes;
				office.NominationPeriodMinutes = source.NominationPeriodMinutes;
				office.VotingPeriodMinutes = source.VotingPeriodMinutes;
				office.MaximumConsecutiveTerms = source.MaximumConsecutiveTerms;
				office.MaximumTotalTerms = source.MaximumTotalTerms;
				office.IsSecretBallot = source.IsSecretBallot;
				office.CanDirectlyAppoint = source.CanDirectlyAppoint;
				office.CanParentDismiss = source.CanParentDismiss;
				office.CanResign = source.CanResign;
				office.CanBeRecalled = source.CanBeRecalled;
				office.RecallThreshold = source.RecallThreshold;
				if (source.IsAppointedByElection)
				{
					FutureProg Placeholder(string function, ProgVariableTypes type, string text)
					{
						var name = CatholicTemplateCatalogue.Marker(definition.Key, source.Key) + "_" + function;
						EnsureTemplateElectionPlaceholderProg(context, name,
							$"CatholicTemplate_v1 source={definition.Key}; appointment={source.Key}; not configured.", type, text);
						var prog = context.FutureProgs.Single(x => x.FunctionName == name);
						prog.Subcategory = "Catholic Templates";
						return prog;
					}
					office.CanNominateProg = Placeholder("nominate", ProgVariableTypes.Boolean, "return false");
					office.WhyCantNominateProg = Placeholder("why_not", ProgVariableTypes.Text,
						"return \"This template must be cloned and its governance configured before use.\"");
					office.NumberOfVotesProg = Placeholder("votes", ProgVariableTypes.Number, "return 0");
				}
			}
			foreach (var source in definition.Appointments)
			{
				var office = offices[source.Key];
				office.ParentAppointment = source.ParentAppointmentKey is { } parent ? offices[parent] : null;
				if (source.ElectionConfirmationAppointmentRef is { Clan: "$self" } authority)
					office.ElectionConfirmationAppointment = offices[authority.AppointmentKey];
			}
			var addressProfile = CatholicTemplateProfiles.Create(definition.Key);
			InstallCatholicAddresses(context, clan, addressProfile, ranks, offices);
			var profileName = $"ClanTemplate_{clan.Id}_Profile";
			if (context.FutureProgs.Any(x => x.FunctionName == profileName))
				throw new InvalidOperationException($"Conflicting source profile {profileName}.");
			EnsureTemplateElectionPlaceholderProg(context, profileName, "ClanTemplateProfile_v1\n" + JsonSerializer.Serialize(new
			{
				TemplateKey = definition.Key, ClanId = clan.Id, Activated = false, StockTemplate = true,
				Ranks = ranks.ToDictionary(x => x.Key, x => x.Value.Id),
				Appointments = offices.ToDictionary(x => x.Key, x => x.Value.Id),
				Bindings = new Dictionary<string, object>(), Variants = Array.Empty<string>(), GeneratedProgs = new Dictionary<string, long>(),
				AddressContentVersion = 1, Definition = addressProfile
			}), ProgVariableTypes.Boolean, "return false");
			context.FutureProgs.Single(x => x.FunctionName == profileName).Subcategory = "Template Profiles";
			context.SaveChanges();
		}
	}
}
