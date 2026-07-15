#nullable enable

using FutureMUD.Web.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace FutureMUD.Web.Publishing;

public sealed class PublishingAuthentication
{
	private readonly byte[]? _expectedHash;

	public PublishingAuthentication(IOptions<FutureMudWebOptions> options)
	{
		var configured = options.Value.PublishingTokenSha256;
		if (!string.IsNullOrWhiteSpace(configured) && configured.Length == 64)
		{
			try
			{
				_expectedHash = Convert.FromHexString(configured);
			}
			catch (FormatException)
			{
				_expectedHash = null;
			}
		}
	}

	public PublishingAuthenticationResult Authenticate(HttpRequest request)
	{
		if (_expectedHash is null)
		{
			return PublishingAuthenticationResult.NotConfigured;
		}

		var header = request.Headers.Authorization.ToString();
		if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
		{
			return PublishingAuthenticationResult.Rejected;
		}

		var token = header[7..];
		if (token.Length < 32)
		{
			return PublishingAuthenticationResult.Rejected;
		}

		var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
		return CryptographicOperations.FixedTimeEquals(suppliedHash, _expectedHash)
			? PublishingAuthenticationResult.Accepted
			: PublishingAuthenticationResult.Rejected;
	}
}

public enum PublishingAuthenticationResult
{
	Accepted,
	Rejected,
	NotConfigured
}
