using System;
using System.Linq;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Accounts;

public static class AuthorityExtensions
{
	public static string Describe(this PermissionLevel level)
	{
		switch (level)
		{
			case PermissionLevel.Admin:
				return "Admin";
			case PermissionLevel.Any:
				return "Any";
			case PermissionLevel.Founder:
				return "Founder";
			case PermissionLevel.Guide:
				return "Guide";
			case PermissionLevel.HighAdmin:
				return "High Admin";
			case PermissionLevel.Inaccessible:
				return "Inaccessible";
			case PermissionLevel.JuniorAdmin:
				return "Junior Admin";
			case PermissionLevel.Player:
				return "Player";
			case PermissionLevel.SeniorAdmin:
				return "Senior Admin";
			case PermissionLevel.NPC:
				return "NPC";
			case PermissionLevel.Guest:
				return "Guest";
			default:
				throw new NotSupportedException();
		}
	}
}

public class Authority : FrameworkItem, IDisposable, IAuthority
{
	public Authority(AuthorityGroup authority)
	{
		_name = authority.Name;
		_id = authority.Id;

		Level = (PermissionLevel)authority.AuthorityLevel;
		PermittedInformationLevel = (PermissionLevel)authority.InformationLevel;

		AccountPermissions = (Permissions)authority.AccountsLevel;
		CharacterPermissions = (Permissions)authority.CharactersLevel;
		CharacterApprovalLevel = (ApprovalLevel)authority.CharacterApprovalLevel;
		CharacterApprovalRisk = (ApprovalRisk)authority.CharacterApprovalRisk;
		ItemPermissions = (Permissions)authority.ItemsLevel;
		PlanePermissions = (Permissions)authority.PlanesLevel;
		RoomPermissions = (Permissions)authority.RoomsLevel;
	}

	public override string FrameworkItemType => "Authority";

	public PermissionLevel Level { get; private set; }

	public PermissionLevel PermittedInformationLevel { get; private set; }

	public Permissions RoomPermissions { get; private set; }

	public Permissions PlanePermissions { get; private set; }

	/// <summary>
	///     Permission to read/edit Characters like an admin.
	/// </summary>
	public Permissions CharacterPermissions { get; private set; }

	public ApprovalLevel CharacterApprovalLevel { get; private set; }

	public ApprovalRisk CharacterApprovalRisk { get; private set; }

	public Permissions AccountPermissions { get; private set; }

	public Permissions ItemPermissions { get; private set; }

	public void Dispose()
	{
		Futuremud.Games.First().Destroy(this);
	}
}