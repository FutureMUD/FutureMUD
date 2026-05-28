using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Employment;

public static class EmploymentHostAccessExtensions
{
	public static IEnumerable<IEmploymentContract> ActiveEmploymentContracts(this IEmploymentHost host)
	{
		return host.EmploymentContracts.Where(x => x.Status == EmploymentStatus.Active);
	}

	public static bool HasActiveEmploymentContract(this IEmploymentHost host, ICharacter? actor)
	{
		return actor is not null &&
		       host.ActiveEmploymentContracts().Any(x => x.Employee.Id == actor.Id);
	}

	public static bool HasActiveEmploymentRole(this IEmploymentHost host, ICharacter? actor,
		params EmploymentRole[] roles)
	{
		return actor is not null &&
		       host.ActiveEmploymentContracts().Any(x =>
			       x.Employee.Id == actor.Id &&
			       roles.Contains(x.Role));
	}

	public static bool HasManagerEmploymentAccess(this IEmploymentHost host, ICharacter? actor)
	{
		return actor?.IsAdministrator() == true ||
		       host.HasActiveEmploymentRole(actor, EmploymentRole.Manager, EmploymentRole.Proprietor);
	}

	public static bool HasProprietorEmploymentAccess(this IEmploymentHost host, ICharacter? actor)
	{
		return actor?.IsAdministrator() == true ||
		       host.HasActiveEmploymentRole(actor, EmploymentRole.Proprietor);
	}

	public static string ActiveEmploymentContractsTable(this IEmploymentHost host, ICharacter actor)
	{
		var contracts = host.ActiveEmploymentContracts()
		                    .OrderBy(x => x.Role)
		                    .ThenBy(x => x.Employee.Name)
		                    .ToList();
		if (!contracts.Any())
		{
			return "\tNone.".ColourError();
		}

		return StringUtilities.GetTextTable(
			from contract in contracts
			select new List<string>
			{
				contract.Id.ToString("N0", actor),
				contract.Employee.HowSeen(actor, colour: false),
				contract.Role.DescribeEnum(),
				contract.Authority.Authorities.DescribeEnum(),
				contract.StartedAt.ToString("g", actor)
			},
			new List<string>
			{
				"Contract",
				"Employee",
				"Role",
				"Authority",
				"Started"
			},
			actor,
			Telnet.Yellow);
	}
}
