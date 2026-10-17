#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static bool S6Readiness => Environment.GetEnvironmentVariable("FUTUREMUD_S6_READINESS_SMOKE_ONLY") == "1";

	private static void PrepareS6ChargenFixture(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input, IReadOnlyDictionary<string, long> claims,
		string capabilityKey = "arm.capability.sorcerer", string accountName = "S6Applicant", string storageKey = "s6_chargen")
	{
		Require(S6Classes, "Full UI qualification requires an authentic classes-on base.");
		var settings = ArmageddonInstallation.Read(db)!;
		var stock = new ArmageddonStockProgression(settings.Mode);
		var spec = stock.Capabilities.Single(x => x.Key == capabilityKey);
		var original = db.ChargenRoles.AsNoTracking().Single(x => x.Id == claims[spec.RoleKey]);
		var stockCapability = db.MagicCapabilities.AsNoTracking().Single(x => x.Id == claims[spec.Key]);
		var capability = new Db.MagicCapability();
		db.Entry(capability).CurrentValues.SetValues(stockCapability);
		capability.Id = 0;
		capability.Name = $"S6 UI complete {spec.Name} qualification";
		var definition = XElement.Parse(capability.Definition);
		Require((bool?)definition.Element("Casting")!.Attribute("enabled") == false &&
			(bool?)definition.Element("Casting")!.Attribute("enrolOnClassSelection") == true, "UI source must retain inactive stock policy and normal selected-Class enrolment.");
		definition.Element("Casting")!.SetAttributeValue("enabled", true);
		definition.Element("Casting")!.SetAttributeValue("identity", Guid.NewGuid());
		capability.Definition = definition.ToString();
		db.MagicCapabilities.Add(capability);
		db.SaveChanges();
		var meritCopy = new Db.Merit();
		db.Entry(meritCopy).CurrentValues.SetValues(db.Merits.AsNoTracking().Single(x => x.Id == claims[spec.MeritKey]));
		meritCopy.Id = 0;
		meritCopy.Name = $"S6 UI complete {spec.Name} qualification caster";
		var meritDefinition = XElement.Parse(meritCopy.Definition);
		Require((long?)meritDefinition.Element("Capabilities")!.Element("Capability") == stockCapability.Id, "UI source merit must reference its canonical capability.");
		meritDefinition.Element("Capabilities")!.Element("Capability")!.Value = capability.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
		meritCopy.Definition = meritDefinition.ToString();
		db.Merits.Add(meritCopy);
		db.SaveChanges();
		var role = new Db.ChargenRole();
		db.Entry(role).CurrentValues.SetValues(original);
		role.Id = 0;
		role.Name = capabilityKey == "arm.capability.sorcerer" ? "S6 Qualification Sorcerer" : "S6 Qualification Earth";
		role.AvailabilityProgId = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg];
		db.ChargenRoles.Add(role);
		db.SaveChanges();
		foreach (var merit in db.ChargenRolesMerits.AsNoTracking().Where(x => x.ChargenRoleId == original.Id).ToArray())
			db.ChargenRolesMerits.Add(new() { ChargenRoleId = role.Id, MeritId = merit.MeritId == claims[spec.MeritKey] ? meritCopy.Id : merit.MeritId });
		foreach (var trait in db.ChargenRolesTraits.AsNoTracking().Where(x => x.ChargenRoleId == original.Id).ToArray())
			db.ChargenRolesTraits.Add(new() { ChargenRoleId = role.Id, TraitId = trait.TraitId, Amount = trait.Amount, GiveIfDoesntHave = trait.GiveIfDoesntHave });
		Require(!db.ChargenRolesCosts.Any(x => x.ChargenRoleId == original.Id) && !db.ChargenRolesCurrencies.Any(x => x.ChargenRoleId == original.Id) &&
			!db.ChargenRolesApprovers.Any(x => x.ChargenRoleId == original.Id) && !db.ChargenRolesClanMemberships.Any(x => x.ChargenRoleId == original.Id) &&
			!db.ChargenAdvicesChargenRoles.Any(x => x.ChargenRoleId == original.Id), "Qualification class copy needs an explicit contract for unexpected dependent stock grants.");
		var template = db.Accounts.AsNoTracking().Single(x => x.Id == db.Characters.Single(c => c.Id == input["character"]!.GetValue<long>()).AccountId);
		var account = new Db.Account();
		db.Entry(account).CurrentValues.SetValues(template);
		account.Id = 0;
		account.Name = accountName;
		account.Email = string.Empty;
		account.LastLoginTime = null;
		account.AuthorityGroupId = db.AuthorityGroups.Single(x => x.AuthorityLevel == (int)PermissionLevel.Player).Id;
		db.Accounts.Add(account);
		db.SaveChanges();
		Require(!db.Characters.Any(x => x.AccountId == account.Id) && !db.Chargens.Any(x => x.AccountId == account.Id), "UI applicant must have no copied character, body or application.");
		var schema = input["sql_schema"]!.AsArray();
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.Chargen>(db, "Chargens", ["Id", "AccountId", "Status", "SubmitTime", "ApprovedById", "ApprovalTime"])));
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.CharactersChargenRoles>(db, "CharactersChargenRoles", ["CharacterId", "ChargenRoleId"])));
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.PerceiverMerit>(db, "PerceiverMerits", ["CharacterId", "MeritId"])));
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.Character>(db, "Characters", ["Id", "AccountId", "BodyId", "ChargenId", "Status", "IsAdminAvatar", "Name"])));
		var startingTraits = definition.Element("Casting")!.Elements()
			.Where(x => x.Name.LocalName is "Admission" or "SupportGrant" && (bool?)x.Attribute("starting") == true)
			.GroupBy(x => (long)x.Attribute("trait")!)
			.ToDictionary(x => x.Key, x => x.Max(y => (double)y.Attribute("opening")!));
		Require(!db.ChargenRolesTraits.Any(x => x.ChargenRoleId == original.Id), "UI source Class must leave magic openings to authored enrolment.");
		input[storageKey] = JsonSerializer.SerializeToNode(new { account = account.Name, account_id = account.Id, role = role.Id, role_name = role.Name,
			report_key = storageKey, original_role = original.Id, capability = capability.Id, merit = meritCopy.Id, original_capability = stockCapability.Id, original_merit = claims[spec.MeritKey], resource = claims[spec.ResourceKey],
			roots = stock.Bindings.Where(x => x.CapabilityKey == spec.Key && x.Starting && !x.Support).Select(x => claims[x.ContentKey]).ToArray(),
			expected_skills = startingTraits, invoked = db.MagicSchools.Single(x => x.Id == capability.MagicSchoolId).SchoolVerb,
			trait = (long)definition.Element("Casting")!.Attribute("trait")!, capacity_attribute = settings.CapacityAttribute,
			resource_name = db.MagicResources.Single(x => x.Id == claims[spec.ResourceKey]).Name,
			limits = "New empty ordinary Player account; actual normal UI selection, submission, staff review/approval and character creation. One available disposable Class copy retains stock traits with separate complete capability/merit copies; all original gates remain false. No copied applicant body/character or injected magic state." });
		Console.WriteLine($"ARMS6-chargen-input=new empty Player account and separate available {spec.Name} Class copy; original stock gates untouched");
	}
}
