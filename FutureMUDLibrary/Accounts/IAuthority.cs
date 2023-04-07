using MudSharp.Framework;

namespace MudSharp.Accounts
{
    public interface IAuthority : IFrameworkItem
    {
        PermissionLevel Level { get; }
        PermissionLevel PermittedInformationLevel { get; }
        Permissions RoomPermissions { get; }
        Permissions PlanePermissions { get; }

        /// <summary>
        ///     Permission to read/edit Characters like an admin.
        /// </summary>
        Permissions CharacterPermissions { get; }

        ApprovalLevel CharacterApprovalLevel { get; }
        ApprovalRisk CharacterApprovalRisk { get; }
        Permissions AccountPermissions { get; }
        Permissions ItemPermissions { get; }
    }
}