#nullable enable

using MudSharp.Community.Templates;
using MudSharp.Database;
using MudSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace MudSharp.Community;

public static partial class ClanTemplateProvisioning
{
	public static ClanTemplateProfile StageClone(IFuturemud world, IClan source, IClan clone, ClanTemplateDefinition definition)
	{
		var previous = ClanTemplateProfile.Read(world, source.Id);
		var profile = new ClanTemplateProfile { TemplateKey = definition.Key, ClanId = clone.Id, AddressContentVersion = previous?.AddressContentVersion ?? 0, Variants = previous?.Variants.ToList() ?? [], Definition = ClanTemplateDefinitions.Copy(definition) };
		foreach (var rank in definition.Ranks)
		{
			var original = previous is null ? source.Ranks.Single(x => x.Name == rank.Name) : source.Ranks.Single(x => x.Id == previous.Ranks[rank.Key]);
			profile.Ranks[rank.Key] = clone.Ranks.Single(x => x.Name == original.Name).Id;
		}
		foreach (var office in definition.Appointments)
		{
			var original = previous is null ? source.Appointments.Single(x => x.Name == office.Name) : source.Appointments.Single(x => x.Id == previous.Appointments[office.Key]);
			var target = clone.Appointments.Single(x => x.Name == original.Name);
			profile.Appointments[office.Key] = target.Id;
			// Cross-clan reuse is a fresh explicit choice; never retain the source's authority silently.
			if (target.ElectionConfirmationAppointment?.Clan != clone) target.ElectionConfirmationAppointment = null;
			target.Changed = true;
			target.Save();
		}
		SaveProfile(world, profile);
		GenerateProgs(world, clone, profile);
		return profile;
	}

	public static void SaveProfile(IFuturemud world, ClanTemplateProfile profile) =>
		SaveProg(world, ClanTemplateProfile.ProgName(profile.ClanId), profile.ToComment(), ProgVariableTypes.Boolean, "return false");

	private static IFutureProg SaveProg(IFuturemud world, string name, string comment, ProgVariableTypes returnType, string text)
	{
		var existing = world.FutureProgs.FirstOrDefault(x => x.FunctionName == name);
		if (existing is not null && (existing.Category != "Clan" || existing.Subcategory != "Template Profiles" ||
		    existing.ReturnType != returnType || !existing.Parameters.SequenceEqual([ProgVariableTypes.Character])))
			throw new InvalidOperationException($"Prog identity conflict: {name}.");
		using (new FMDB())
		{
			var model = existing is null ? new Models.FutureProg
			{
				FunctionName = name, Category = "Clan", Subcategory = "Template Profiles", Public = false,
				AcceptsAnyParameters = false, StaticType = (int)FutureProgStaticType.NotStatic, ReturnType = (long)returnType
			} : FMDB.Context.FutureProgs.Find(existing.Id)!;
			model.FunctionText = text;
			model.FunctionComment = comment;
			if (existing is null) model.FutureProgsParameters.Add(new FutureProgsParameter
			{
				ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Character
			});
			var compiled = new FutureProg.FutureProg(model, world);
			if (!compiled.Compile()) throw new InvalidOperationException($"Generated {name} failed to compile: {compiled.CompileError}");
			if (existing is null) FMDB.Context.FutureProgs.Add(model);
			FMDB.Context.SaveChanges();
			if (existing is null)
			{
				var runtime = new FutureProg.FutureProg(model, world);
				runtime.Compile();
				world.Add(runtime);
				return runtime;
			}
			existing.FunctionComment = comment;
			existing.FunctionText = text;
			existing.Compile();
			return existing;
		}
	}

	public static void GenerateProgs(IFuturemud world, IClan clan, ClanTemplateProfile profile)
	{
		GenerateAddressProgs(world, clan, profile);
		foreach (var source in profile.Definition.Appointments)
		{
			var office = clan.Appointments.Single(x => x.Id == profile.Appointments[source.Key]);
			IFutureProg Prog(string function, ProgVariableTypes type, string text)
			{
				var name = $"ClanTemplate_{clan.Id}_{source.Key}_{function}";
				var prog = SaveProg(world, name,
					$"ClanTemplateProfile_v1 source={profile.TemplateKey}; appointment={source.Key}; bindings={profile.ToComment()}", type, text);
				profile.GeneratedProgs[$"{source.Key}.{function}"] = prog.Id;
				return prog;
			}
			string Call(string purpose) => $"ClanTemplateEligibilityError(@ch, {clan.Id}, {office.Id}, \"{purpose}\")";
			if (office.IsAppointedByElection)
			{
				office.CanNominateProg = Prog("nominate", ProgVariableTypes.Boolean, $"return {Call("nominate")} == \"\"");
				office.WhyCantNominateProg = Prog("why_not", ProgVariableTypes.Text, $"return {Call("nominate")}");
				office.NumberOfVotesProg = Prog("votes", ProgVariableTypes.Number,
					$"if ({Call("votes")} == \"\")\n return 1\nend if\nreturn 0");
			}
			else
			{
				office.CanNominateProg = null;
				office.WhyCantNominateProg = null;
				office.NumberOfVotesProg = null;
			}
			if (office.CanBeRecalled)
			{
				office.CanInitiateRecallProg = Prog("recall_initiate", ProgVariableTypes.Boolean, $"return {Call("recall")} == \"\"");
				office.RecallVotesProg = Prog("recall_votes", ProgVariableTypes.Number,
					$"if ({Call("recall")} == \"\")\n return 1\nend if\nreturn 0");
			}
			else { office.CanInitiateRecallProg = null; office.RecallVotesProg = null; }
			office.Changed = true;
			office.Save();
		}
		SaveProfile(world, profile);
	}

	/// <summary>Activates only fresh staged copies. Governance changes with history use native operations.</summary>
	public static IReadOnlyList<string> Activate(IFuturemud world, IClan clan, ClanTemplateProfile profile)
	{
		if (profile.StockTemplate || !clan.IsTemplate || profile.Activated || clan.Appointments.Any(x => x.Elections.Any()))
			return ["Only an unactivated staged copy without election history can use initial provisioning. Resolve live changes through native operations."];
		var errors = ClanTemplateEligibility.ValidateBindings(world, clan, profile, checkConfiguration: false).ToList();
		if (clan.ExternalControls.Any(x => x.VassalClan == clan)) errors.Add("Review and remove pre-existing external reservations before initial provisioning.");
		if (errors.Count > 0) return errors;
		ClanTemplateEligibility.RetainAuthorityIds(world, clan, profile);
		var definition = profile.Definition;
		// Bind exact confirmation offices and install reservations only for nonelected offices.
		foreach (var source in definition.Appointments)
		{
			var office = clan.Appointments.Single(x => x.Id == profile.Appointments[source.Key]);
			if (office.IsAppointedByElection != source.IsAppointedByElection)
				return [$"Election policy for {source.Key} differs from the selected profile. Review the staged policy before activation."];
		}
		// Compilation can fail without making the copy live or creating reservations.
		GenerateProgs(world, clan, profile);
		var confirmations = definition.Appointments.ToDictionary(x => profile.Appointments[x.Key],
			x => ClanTemplateEligibility.ResolveAuthority(world, clan, profile, x.ElectionConfirmationAppointmentRef));
		var controls = definition.ExternalControls.Select(recipe =>
		{
			var authority = ClanTemplateEligibility.ResolveAuthority(world, clan, profile, new ClanTemplateOfficeReference
			{ Clan = "$" + recipe.LiegeClanRole, AppointmentKey = recipe.ControllingAppointmentKey })!;
			return new Models.ExternalClanControl
			{
				VassalClanId = clan.Id, LiegeClanId = authority.Clan.Id,
				ControlledAppointmentId = profile.Appointments[recipe.ControlledAppointmentKey],
				ControllingAppointmentId = authority.Id, NumberOfAppointments = recipe.NumberOfAppointments
			};
		}).ToList();
		var profileProg = world.FutureProgs.Single(x => x.FunctionName == ClanTemplateProfile.ProgName(clan.Id));
		profile.Activated = true;
		using (new FMDB())
		{
			using var transaction = FMDB.Context.Database.BeginTransaction();
			var officeRows = confirmations.Keys.Select(id => FMDB.Context.Appointments.Find(id)!).ToList();
			var profileRow = FMDB.Context.FutureProgs.Find(profileProg.Id)!;
			var clanRow = FMDB.Context.Clans.Find(clan.Id)!;
			try
			{
				foreach (var row in officeRows) row.ElectionConfirmationAppointmentId = confirmations[row.Id]?.Id;
				FMDB.Context.ExternalClanControls.AddRange(controls);
				profileRow.FunctionComment = profile.ToComment();
				clanRow.IsTemplate = false;
				FMDB.Context.SaveChanges();
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				profile.Activated = false;
				// A surrounding FMDB handle may survive. Do not leak failed writes into its next save.
				foreach (var row in controls) FMDB.Context.Entry(row).State = EntityState.Detached;
				foreach (var row in officeRows) FMDB.Context.Entry(row).Reload();
				FMDB.Context.Entry(profileRow).Reload();
				FMDB.Context.Entry(clanRow).Reload();
				throw;
			}
		}
		// Publish runtime state only after the complete activation configuration is durable.
		profileProg.FunctionComment = profile.ToComment();
		foreach (var (id, authority) in confirmations)
			clan.Appointments.Single(x => x.Id == id).ElectionConfirmationAppointment = authority;
		foreach (var row in controls) _ = new ExternalClanControl(row, world);
		// The native setter starts elections once. A restart after this commit also resumes safely.
		clan.IsTemplate = false;
		return [];
	}

	public static void ApplyVariant(IFuturemud world, IClan clan, ClanTemplateProfile profile, string variant)
	{
		if (profile.StockTemplate || !clan.IsTemplate || profile.Activated || clan.Appointments.Any(x => x.Elections.Any()))
			throw new InvalidOperationException("Variants can only be applied during initial staged provisioning, before any ballot history.");
		if (profile.Variants.Contains(variant)) throw new InvalidOperationException("That variant is already selected.");
		var before = profile.Definition;
		var selected = profile.Variants.Append(variant).ToList();
		var after = ClanTemplateDefinitions.WithVariant(before, variant);
		ValidateSharedAddressProgs(world, clan, profile, after, before);
		var addsRanks = after.Ranks.Any(x => !profile.Ranks.ContainsKey(x.Key));
		var reordersRanks = addsRanks || before.Ranks.Any(x => after.Ranks.Single(y => y.Key == x.Key).RankNumber != x.RankNumber);
		if (reordersRanks && clan.Ranks.Any(x => !profile.Ranks.ContainsValue(x.Id)))
			throw new InvalidOperationException("Apply rank-adding variants before adding custom ranks; their relative standing requires deliberate review.");
		foreach (var source in after.Ranks)
		{
			if (!profile.Ranks.TryGetValue(source.Key, out var id))
			{
				using (new FMDB())
				{
					var row = new Models.Rank { ClanId = clan.Id, Name = source.Name, RankPath = source.RankPath,
						RankNumber = source.RankNumber, FameType = source.FameType, Privileges = ClanTemplateDefinitions.Privileges(source.PrivilegeNames) };
					var addressRules = new List<ClanAddressRule>();
					UpdateAddressRules(addressRules, [], source.FormsOfAddress, "rank", source.Key);
					foreach (var (rule, order) in addressRules.Select((x, i) => (x, i))) row.FormsOfAddress.Add(ClanAddressPersistence.RankRow(rule, order));
					row.RanksTitles.Add(new RanksTitle { Title = source.Title, Order = 0 });
					row.RanksAbbreviations.Add(new RanksAbbreviations { Abbreviation = source.Abbreviation, Order = 0 });
					FMDB.Context.Ranks.Add(row);
					FMDB.Context.SaveChanges();
					var runtime = new Rank(row, clan);
					clan.Ranks.Add(runtime);
					profile.Ranks[source.Key] = runtime.Id;
				}
			}
			else
			{
				var old = before.Ranks.Single(x => x.Key == source.Key);
				var rank = clan.Ranks.Single(x => x.Id == id);
				if (old.Name != source.Name) rank.SetName(source.Name);
				if (old.Title != source.Title)
				{
					rank.TitlesAndProgs.Clear();
					rank.TitlesAndProgs.Add(Tuple.Create<IFutureProg, string>(null!, source.Title));
				}
				if (old.Abbreviation != source.Abbreviation)
				{
					rank.AbbreviationsAndProgs.Clear();
					rank.AbbreviationsAndProgs.Add(Tuple.Create<IFutureProg, string>(null!, source.Abbreviation));
				}
				UpdateAddressRules(rank.FormsOfAddress, old.FormsOfAddress, source.FormsOfAddress, "rank", source.Key);
				if (old.RankPath != source.RankPath) rank.RankPath = source.RankPath;
				if (old.FameType != source.FameType) rank.FameType = (ClanFameType)source.FameType;
				if (!old.PrivilegeNames.SequenceEqual(source.PrivilegeNames)) rank.Privileges = (ClanPrivilegeType)ClanTemplateDefinitions.Privileges(source.PrivilegeNames);
				rank.Changed = true;
				rank.Save();
			}
		}
		// Normalise only when a variant adds or reorders ranks; unrelated variants preserve builder edits.
		if (reordersRanks)
		{
			var ordinal = 0;
			foreach (var source in after.Ranks.OrderBy(x => x.RankNumber))
			{
				var rank = clan.Ranks.Single(x => x.Id == profile.Ranks[source.Key]);
				rank.RankNumber = ordinal++;
				rank.Changed = true;
				rank.Save();
			}
		}
		foreach (var source in after.Appointments)
		{
			var old = before.Appointments.Single(x => x.Key == source.Key);
			var office = clan.Appointments.Single(x => x.Id == profile.Appointments[source.Key]);
			UpdateAddressRules(office.FormsOfAddress, old.FormsOfAddress, source.FormsOfAddress, "office", source.Key);
			if (old.AddressPriority != source.AddressPriority) office.AddressPriority = source.AddressPriority;
			if (old.Name != source.Name) office.SetName(source.Name);
			if (old.Title != source.Title)
			{
				office.TitlesAndProgs.Clear();
				office.TitlesAndProgs.Add(Tuple.Create<IFutureProg, string>(null!, source.Title));
			}
			if (old.Abbreviation != source.Abbreviation)
			{
				office.AbbreviationsAndProgs.Clear();
				office.AbbreviationsAndProgs.Add(Tuple.Create<IFutureProg, string>(null!, source.Abbreviation));
			}
			if (old.MinimumRankKey != source.MinimumRankKey) office.MinimumRankToHold = clan.Ranks.Single(x => x.Id == profile.Ranks[source.MinimumRankKey]);
			if (old.MinimumRankToAppointKey != source.MinimumRankToAppointKey)
				office.MinimumRankToAppoint = source.MinimumRankToAppointKey is { } appointKey ? clan.Ranks.Single(x => x.Id == profile.Ranks[appointKey]) : null!;
			if (old.ParentAppointmentKey != source.ParentAppointmentKey)
				office.ParentPosition = source.ParentAppointmentKey is { } parentKey ? clan.Appointments.Single(x => x.Id == profile.Appointments[parentKey]) : null!;
			if (old.MaximumSimultaneousHolders != source.MaximumSimultaneousHolders) office.MaximumSimultaneousHolders = source.MaximumSimultaneousHolders;
			if (old.FameType != source.FameType) office.FameType = (ClanFameType)source.FameType;
			if (!old.PrivilegeNames.SequenceEqual(source.PrivilegeNames)) office.Privileges = (ClanPrivilegeType)ClanTemplateDefinitions.Privileges(source.PrivilegeNames);
			if (old.CanDirectlyAppoint != source.CanDirectlyAppoint) office.CanDirectlyAppoint = source.CanDirectlyAppoint;
			if (old.CanParentDismiss != source.CanParentDismiss) office.CanParentDismiss = source.CanParentDismiss;
			if (old.CanResign != source.CanResign) office.CanResign = source.CanResign;
			if (old.CanBeRecalled != source.CanBeRecalled) office.CanBeRecalled = source.CanBeRecalled;
			if (old.RecallThreshold != source.RecallThreshold) office.RecallThreshold = source.RecallThreshold;
			if (old.IsAppointedByElection != source.IsAppointedByElection) office.IsAppointedByElection = source.IsAppointedByElection;
			if (old.ElectionTermMinutes != source.ElectionTermMinutes) office.ElectionTerm = TimeSpan.FromMinutes(source.ElectionTermMinutes ?? 0);
			if (old.ElectionLeadTimeMinutes != source.ElectionLeadTimeMinutes) office.ElectionLeadTime = TimeSpan.FromMinutes(source.ElectionLeadTimeMinutes ?? 0);
			if (old.NominationPeriodMinutes != source.NominationPeriodMinutes) office.NominationPeriod = TimeSpan.FromMinutes(source.NominationPeriodMinutes ?? 0);
			if (old.VotingPeriodMinutes != source.VotingPeriodMinutes) office.VotingPeriod = TimeSpan.FromMinutes(source.VotingPeriodMinutes ?? 0);
			if (old.MaximumConsecutiveTerms != source.MaximumConsecutiveTerms) office.MaximumConsecutiveTerms = source.MaximumConsecutiveTerms ?? 0;
			if (old.MaximumTotalTerms != source.MaximumTotalTerms) office.MaximumTotalTerms = source.MaximumTotalTerms ?? 0;
			if (old.IsSecretBallot != source.IsSecretBallot) office.IsSecretBallot = source.IsSecretBallot ?? false;
			if (old.ElectionConfirmationAppointmentRef?.Clan != source.ElectionConfirmationAppointmentRef?.Clan ||
			    old.ElectionConfirmationAppointmentRef?.AppointmentKey != source.ElectionConfirmationAppointmentRef?.AppointmentKey)
				office.ElectionConfirmationAppointment = ClanTemplateEligibility.ResolveAuthority(world, clan, profile, source.ElectionConfirmationAppointmentRef)!;
			office.Changed = true;
			office.Save();
		}
		profile.Variants = selected;
		profile.Definition = after;
		SaveProfile(world, profile);
		GenerateProgs(world, clan, profile);
	}
}
