#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ClanSeeder
{
	private sealed record StockAddress(string Owner, string Text, bool Appointment = false, int Priority = 0,
		string? FemaleText = null, bool IsPublic = true, bool JuniorCourtesy = false);

	/// <summary>Authored vocatives, not an automatic conversion of every occupational label to an honorific.</summary>
	private static IReadOnlyDictionary<string, IReadOnlyList<StockAddress>> OtherTemplateAddresses()
	{
		var result = new Dictionary<string, IReadOnlyList<StockAddress>>(StringComparer.Ordinal);
		StockAddress Rank(string name, string? text = null) => new(name, text ?? name);
		StockAddress Officer(string name, string? text = null) => new(name, text ?? name, JuniorCourtesy: true);
		StockAddress Office(string name, string? text = null, int priority = 10) => new(name, text ?? name, true, priority);
		void Add(string template, params StockAddress[] addresses) => result.Add(template, addresses);
		StockAddress[] army =
		[
			Rank("Recruit"), Rank("Private"), Rank("Private First Class", "Private"), Rank("Specialist"), Rank("Corporal"),
			Rank("Sergeant"), Rank("Staff Sergeant", "Sergeant"), Rank("Sergeant First Class", "Sergeant"),
			Rank("Master Sergeant", "Sergeant"), Rank("Sergeant Major"),
			Officer("Second Lieutenant", "Lieutenant"), Officer("First Lieutenant", "Lieutenant"), Officer("Captain"), Officer("Major"),
			Officer("Lieutenant Colonel", "Colonel"), Officer("Colonel"), Officer("Brigadier General", "General"),
			Officer("Major General", "General"), Officer("Lieutenant General", "General"), Officer("General")
		];
		// The broad Army template mixes US-style grades with British-style warrant classes.
		Add("Army Template", [.. army.Where(x => x.Owner != "Specialist"), Officer("Warrant Officer Second Class", "Warrant Officer"),
			Officer("Warrant Officer First Class", "Warrant Officer"), Office("General of the Army", "General", 100) with { JuniorCourtesy = true }]);
		Add("Infantry Company Template", [.. army.Where(x => !x.Owner.Contains("General") && x.Owner != "Recruit"), Rank("First Sergeant"),
			Office("First Sergeant", "First Sergeant", 50)]);
		Add("Battalion Template", [.. army.Where(x => !x.Owner.Contains("General") && x.Owner != "Recruit"), Rank("First Sergeant"),
			Office("Command Sergeant Major", "Sergeant Major", 50)]);
		Add("Mercenary Company Template", Rank("Sergeant"), Rank("Lieutenant"), Rank("Captain"),
			Office("Mercenary Captain", "Captain", 50), Office("Company Lieutenant", "Lieutenant", 20), Office("Quartermaster"));

		StockAddress[] navy =
		[
			Rank("Seaman Recruit", "Seaman"), Rank("Seaman Apprentice", "Seaman"), Rank("Seaman"),
			Rank("Petty Officer Third Class", "Petty Officer"), Rank("Petty Officer Second Class", "Petty Officer"),
			Rank("Petty Officer First Class", "Petty Officer"), Rank("Chief Petty Officer", "Chief"),
			Rank("Senior Chief Petty Officer", "Senior Chief"), Rank("Master Chief Petty Officer", "Master Chief"),
			Officer("Ensign"), Officer("Lieutenant Junior Grade", "Lieutenant"), Officer("Lieutenant"),
			Officer("Lieutenant Commander", "Commander"), Officer("Commander"), Officer("Captain"),
			Officer("Rear Admiral (Lower Half)", "Admiral"), Officer("Rear Admiral", "Admiral"),
			Officer("Vice Admiral", "Admiral"), Officer("Admiral")
		];
		// This is a naval organisation rather than one vessel; its CO is not assumed to be a ship's captain.
		Add("Naval Organisation Template", [.. navy, Office("Command Master Chief", "Master Chief", 50), Office("Fleet Admiral", "Admiral", 100) with { JuniorCourtesy = true }]);
		Add("Capital Ship Template", [.. navy.Where(x => x.Owner is "Seaman Recruit" or "Seaman" or "Chief Petty Officer" or
			"Master Chief Petty Officer" or "Ensign" or "Lieutenant" or "Lieutenant Commander" or "Commander" or "Captain"),
			Rank("Petty Officer"), Office("Captain", "Captain", 100) with { JuniorCourtesy = true }, Office("Command Master Chief", "Master Chief", 50)]);
		Add("Air Force Template", Rank("Airman Basic", "Airman"), Rank("Airman"), Rank("Airman First Class", "Airman"),
			Rank("Senior Airman", "Airman"), Rank("Staff Sergeant", "Sergeant"), Rank("Technical Sergeant", "Sergeant"),
			Rank("Master Sergeant", "Sergeant"), Rank("Senior Master Sergeant", "Sergeant"), Rank("Chief Master Sergeant", "Chief"),
			Officer("Second Lieutenant", "Lieutenant"), Officer("First Lieutenant", "Lieutenant"), Officer("Captain"), Officer("Major"),
			Officer("Lieutenant Colonel", "Colonel"), Officer("Colonel"), Officer("Brigadier General", "General"),
			Officer("Major General", "General"), Officer("Lieutenant General", "General"), Officer("General"),
			Office("Command Chief Master Sergeant", "Chief", 50), Office("General of the Air Force", "General", 100) with { JuniorCourtesy = true });
		Add("Age of Sail Ship Template", Rank("Lieutenant"), Rank("Commander"), Office("Captain", "Captain", 100),
			Office("First Lieutenant", "Lieutenant"), Office("Second Lieutenant", "Lieutenant"),
			Office("Boatswain"), Office("Purser"));
		Add("UK Police Template", Rank("Probationary Constable", "Constable"), Rank("Constable"), Rank("Senior Constable", "Constable"),
			Rank("Sergeant"), Rank("Inspector"), Rank("Chief Inspector"), Rank("Superintendent"), Rank("Chief Superintendent"),
			Rank("Assistant Chief Constable"), Rank("Deputy Chief Constable"), Rank("Chief Constable"));

		// Explicit Latin vocatives: common nouns in -arius use -arie, not the -i of personal names.
		// Shortened role forms avoid perpetuating malformed qualifiers in legacy owner identities.
		Add("Roman Legion Template", Rank("Tesserarius Cohortis", "Tesserarie"), Rank("Tesserarius", "Tesserarie"),
			Rank("Signifer Cohortis", "Signifer"), Rank("Signifer Alae", "Signifer"), Rank("Signifer"),
			Rank("Optio Cohortis", "Optio"), Rank("Curator"), Rank("Optio"),
			Rank("Vexillarius Cohortis", "Vexillarie"), Rank("Vexillarius Alae", "Vexillarie"), Rank("Vexillarius", "Vexillarie"),
			Rank("Centurio Cohortis", "Centurio"), Rank("Decurio"), Rank("Centurio"),
			Rank("Centurio Primus Ordinus", "Centurio"), Rank("Centurio Primus Pilus", "Centurio"),
			Rank("Centurio Princeps Cohortis", "Centurio"), Rank("Decurio Princeps", "Decurio"),
			Rank("Beneficiarius Cohortis", "Beneficiarie"), Rank("Beneficiarius Alae", "Beneficiarie"),
			Rank("Praefectus Cohortis", "Praefecte"), Rank("Tribunus Cohortis", "Tribune"),
			Rank("Praefectus Alae", "Praefecte"), Rank("Praefectus Castrorum", "Praefecte"),
			Rank("Tribunus Angusticlavii", "Tribune"), Rank("Tribunus Laticlavius", "Tribune"), Rank("Legate", "Legate"));
		// Feminine civic variants follow the existing template's fictional office-holding conventions.
		Add("Roman City Template", new("Senator", "Senator", FemaleText: "Senatrix"),
			new("Proquaestor", "Proquaestor", FemaleText: "Proquaestrix"),
			new("Propraetor", "Propraetor", FemaleText: "Propraetrix"), Rank("Proconsul"),
			new("Quaestor", "Quaestor", true, 30, "Quaestrix"), Office("Aedilis", "Aedilis", 40),
			new("Praetor", "Praetor", true, 60, "Praetrix"), Office("Consul", "Consul", 90), Office("Censor", "Censor", 80),
			Office("Tribunus Plebis", "Tribune Plebis", 50), Office("Princeps Senatus", "Princeps Senatus", 70),
			new("Dictator", "Dictator", true, 100, "Dictatrix"));
		Add("Roman Religious Cult Template", Rank("Priest", "Sacerdos"), Rank("Senior Priest", "Sacerdos"),
			Office("Chief Pontiff", "Pontifex", 100), Office("Temple Priest", "Sacerdos", 20), Office("Augur", "Augur", 30));

		StockAddress[] peerage =
		[
			new("Knight", "Sir", FemaleText: "Dame"), new("Baron", "My Lord", FemaleText: "My Lady"),
			Rank("Duke", "Your Grace"), Rank("Sovereign", "Your Majesty"), Office("Prince", "Your Royal Highness", 100),
			new("Consort", "Your Royal Highness", true, 200, "Your Majesty")
		];
		Add("Feudalism Template", [.. peerage, new("Vassal Knight", "Sir", FemaleText: "Dame"),
			new("Knight Banneret", "Sir", FemaleText: "Dame")]);
		Add("Peerage Template", [.. peerage, new("Baronet", "Sir", FemaleText: "Dame"),
			new("Viscount", "My Lord", FemaleText: "My Lady"), new("Earl", "My Lord", FemaleText: "My Lady"),
			new("Marquess", "My Lord", FemaleText: "My Lady")]);
		Add("Chivalric Order Template", new("Knight Lieutenant", "Sir", FemaleText: "Dame"),
			new("Knight Commander", "Sir", FemaleText: "Dame"), new("Knight Grand Commander", "Sir", FemaleText: "Dame"),
			new("Knight Grand Cross", "Sir", FemaleText: "Dame"), new("Knight Marshal", "Sir", FemaleText: "Dame"));
		Add("Council Template", Office("Chairperson", "Chair"));
		foreach (var name in new[] { "Local Government Template - Popular Mayor", "Local Government Template - Councillor Mayor" })
			Add(name, Rank("Councillor"), Office("Mayor", "Mayor", 50), Office("Deputy Mayor", "Deputy Mayor", 20));
		Add("Gang Template", Rank("Boss", "Boss") with { IsPublic = false });

		Add("Merchant Guild Template", Office("Guildmaster", "Guildmaster", 50), Office("Guild Warden", "Warden", 20));
		Add("Craft Guild Template", Rank("Master"), Office("Guildmaster", "Guildmaster", 50), Office("Guild Warden", "Warden", 20));
		Add("Pirate Crew Template", Rank("Captain"), Office("Captain", "Captain", 100), Office("Quartermaster", "Quartermaster", 20), Office("Boatswain"));
		Add("University Template", Rank("Professor"), Rank("Distinguished Professor", "Professor"),
			Office("Chancellor", "Chancellor", 100), Office("Vice-Chancellor", "Vice-Chancellor", 80), Office("Dean", "Dean", 50));
		Add("Fire and Rescue Service Template", Rank("Station Officer"), Rank("Chief Officer", "Chief"),
			Office("Fire Chief", "Chief", 100), Office("Deputy Chief", "Deputy Chief", 80), Office("Fire Marshal", "Marshal", 50));
		Add("Intelligence Agency Template", Rank("Director") with { IsPublic = false },
			Office("Director-General", "Director", 100) with { IsPublic = false },
			Office("Deputy Director", "Deputy Director", 80) with { IsPublic = false });
		Add("Labour Union Template", Office("President", "President", 100), Office("General Secretary", "General Secretary", 50));
		Add("Space Colony Administration Template", Rank("Governor"), Office("Governor", "Governor", 100), Office("Deputy Governor", "Deputy Governor", 80));
		Add("Civilian Starship Crew Template", Rank("Ship Master", "Captain"), Office("Ship Master", "Captain", 100),
			Office("First Officer", "First Officer", 50), Office("Chief Engineer", "Chief Engineer", 20));
		Add("Exploration Corps Template", Rank("Expedition Commander", "Commander"), Office("Expedition Commander", "Commander", 100));
		return result;
	}

	/// <summary>Only clans created by this invocation acquire defaults; existing unowned templates are never adopted.</summary>
	private static void SeedNewTemplateAddresses(FuturemudDatabaseContext context, ISet<long> existingClanIds)
	{
		foreach (var (name, addresses) in OtherTemplateAddresses())
		{
			var clan = context.Clans.SingleOrDefault(x => x.Name == name && x.IsTemplate);
			if (clan is null || existingClanIds.Contains(clan.Id)) continue;
			foreach (var source in addresses)
			{
				var rules = new List<(string Text, long? ProgId, MudSharp.Community.ClanAddressRelation Relation)>();
				long Predicate(string name)
				{
					var prog = context.FutureProgs.Single(x => x.FunctionName == name);
					var parameters = prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex).ToList();
					if (prog.ReturnType != (long)ProgVariableTypes.Boolean || prog.AcceptsAnyParameters || parameters.Count != 1 ||
					    !ProgVariableTypes.Character.CompatibleWith((ProgVariableTypes)parameters[0].ParameterType))
						throw new InvalidOperationException($"Stock addresses require {name} to return boolean and accept one character.");
					return prog.Id;
				}
				if (source.JuniorCourtesy)
				{
					rules.Add(("Ma'am", Predicate("IsFemale"), MudSharp.Community.ClanAddressRelation.Lower));
					rules.Add(("Sir", Predicate("IsMale"), MudSharp.Community.ClanAddressRelation.Lower));
				}
				else if (source.FemaleText is not null)
					rules.Add((source.FemaleText, Predicate("IsFemale"), MudSharp.Community.ClanAddressRelation.Any));
				rules.Add((source.Text, null, MudSharp.Community.ClanAddressRelation.Any));
				if (source.Appointment)
				{
					var office = clan.Appointments.Single(x => x.Name == source.Owner);
					if (office.FormsOfAddress.Count > 0) continue;
					office.AddressPriority = source.Priority;
					foreach (var (rule, order) in rules.Select((x, i) => (x, i)))
						office.FormsOfAddress.Add(new AppointmentFormOfAddress { Id = Guid.NewGuid(), Order = order,
							Text = rule.Text, FutureProgId = rule.ProgId, IsPublic = source.IsPublic, Relations = (int)rule.Relation });
				}
				else
				{
					var rank = clan.Ranks.Single(x => x.Name == source.Owner);
					if (rank.FormsOfAddress.Count > 0) continue;
					foreach (var (rule, order) in rules.Select((x, i) => (x, i)))
						rank.FormsOfAddress.Add(new RankFormOfAddress { Id = Guid.NewGuid(), Order = order,
							Text = rule.Text, FutureProgId = rule.ProgId, IsPublic = source.IsPublic, Relations = (int)rule.Relation });
				}
			}
		}
		context.SaveChanges();
	}
}
