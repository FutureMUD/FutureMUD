#nullable enable
namespace MudSharp.Accounts;

/// <summary>Versioned password storage; legacy verification is kept separate from new credential creation.</summary>
public interface IAccountCredentialHasher
{
	string Hash(string password);
	bool Verify(string password, string encodedHash);
}
