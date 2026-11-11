#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Community.Templates;

namespace DatabaseSeeder.Seeders;

/// <summary>Editable English courtesy conventions; all religious policy stays in seeder-authored data.</summary>
internal static class CatholicAddressContent
{
	public static void Apply(CatholicTemplateDefinition source, ClanTemplateDefinition profile)
	{
		var churchRole = profile.IsAffiliationRoot ? "$self" : "church";
		ClanTemplateAddressRule Plain(string key, string text) => new() { Key = key, Text = text, IsPublic = true };
		ClanTemplateAddressRule Ordained(string key, string text, string rank, bool exact = false) => new()
		{
			Key = key, Text = text, IsPublic = true,
			Conditions = [new() { Role = churchRole, Kind = exact ? "exactrank" : "minimumrank", Key = rank,
				Error = "The required ordination is not recorded in the bound Church." }]
		};
		List<ClanTemplateAddressRule> MaleReligious(string? office = null) =>
		[
			Ordained("bishop", "Your Excellency", "bishop", true),
			Ordained("priest", office is null ? "Father" : $"Father {office}", "priest"),
			Plain("default", office is null ? "Brother" : $"Brother {office}")
		];
		var female = source.Appointments.Any(x => x.Name is "Abbess" or "Prioress" or "Novice Mistress");
		foreach (var rank in profile.Ranks)
		{
			if (source.RankModel == "ecclesial")
			{
				var address = rank.Key switch { "deacon" => "Deacon", "priest" => "Father", "bishop" => "Your Excellency", _ => null };
				if (address is not null) rank.FormsOfAddress = [Plain("default", address)];
			}
			else if (rank.Key is "novice" or "professed" or "professed_knight")
				rank.FormsOfAddress = female ? [Plain("default", "Sister")] : MaleReligious();
		}
		foreach (var office in profile.Appointments)
		{
			var address = office.Key switch
			{
				"pope" => "Your Holiness",
				"cardinal" => "Your Eminence",
				"ordinary" when source.Key == "catholic.archdiocese" => "Your Grace",
				"ordinary" or "auxiliary_bishop" or "coadjutor_bishop" => "Your Excellency",
				_ => null
			};
			if (address is not null)
			{
				office.FormsOfAddress = [Plain("default", address)];
				office.AddressPriority = office.Key == "pope" ? 300 : office.Key == "cardinal" ? 200 : office.Key == "ordinary" ? 110 : 100;
			}
			else if (office.Key is "superior" or "deputy")
			{
				if (office.Name.Contains("Abbess")) office.FormsOfAddress = [Plain("default", "Mother Abbess")];
				else if (office.Name.Contains("Prioress")) office.FormsOfAddress = [Plain("default", "Mother Prioress")];
				else if (office.Name.Contains("Abbot")) office.FormsOfAddress = MaleReligious("Abbot");
				else if (office.Name.Contains("Prior") || office.Name == "Subprior") office.FormsOfAddress = MaleReligious("Prior");
				if (office.FormsOfAddress.Count > 0) office.AddressPriority = office.Key == "superior" ? 50 : 40;
			}
		}
	}
}
