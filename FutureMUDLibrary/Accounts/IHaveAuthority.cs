namespace MudSharp.Accounts {
    public interface IHaveAuthority {
        IAuthority Authority { get; }

        PermissionLevel PermissionLevel { get; }
    }
}