#nullable enable

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace MudSharp.Email;

internal static class GoogleEmailAuthorization
{
	internal const string GmailApiScope = "https://www.googleapis.com/auth/gmail.send";
	internal const string SmtpScope = "https://mail.google.com/";

	public static async Task AuthorizeAsync(string mode, string clientSecretsPath, CancellationToken cancellationToken)
	{
		if (!TryGetScope(mode, out var scope))
		{
			throw new ArgumentException("Google email authorization mode must be gmail-api or smtp.", nameof(mode));
		}

		if (string.IsNullOrWhiteSpace(clientSecretsPath) || !File.Exists(clientSecretsPath))
		{
			throw new FileNotFoundException("Google OAuth client-secrets JSON file was not found.");
		}

		GoogleClientSecrets clientSecrets;
		await using (var stream = File.OpenRead(clientSecretsPath))
		{
			using var document = JsonDocument.Parse(stream);
			if (!document.RootElement.TryGetProperty("installed", out _))
			{
				throw new InvalidDataException("Google OAuth client-secrets JSON must describe a Desktop client.");
			}

			stream.Position = 0;
			clientSecrets = GoogleClientSecrets.FromStream(stream);
		}

		if (string.IsNullOrWhiteSpace(clientSecrets.Secrets.ClientId) ||
			string.IsNullOrWhiteSpace(clientSecrets.Secrets.ClientSecret))
		{
			throw new InvalidDataException("Google OAuth client-secrets JSON does not contain a desktop client ID and secret.");
		}

		var flowInitializer = new GoogleAuthorizationCodeFlow.Initializer
		{
			ClientSecrets = clientSecrets.Secrets,
			Scopes = [scope],
			Prompt = "consent"
		};
		var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(flowInitializer, [scope], "futuremud-email",
			true, cancellationToken, new InMemoryDataStore(), new LocalServerCodeReceiver());
		if (string.IsNullOrWhiteSpace(credential.Token.RefreshToken))
		{
			throw new InvalidOperationException("Google authorization completed without a refresh token. Revoke the application grant and retry.");
		}

		Console.WriteLine("Authorization succeeded. Store this refresh token once in a protected environment variable; it will not be saved by FutureMUD:");
		Console.WriteLine(credential.Token.RefreshToken);
	}

	internal static bool TryGetScope(string mode, out string scope)
	{
		if (mode.Equals("gmail-api", StringComparison.OrdinalIgnoreCase))
		{
			scope = GmailApiScope;
			return true;
		}

		if (mode.Equals("smtp", StringComparison.OrdinalIgnoreCase))
		{
			scope = SmtpScope;
			return true;
		}

		scope = string.Empty;
		return false;
	}

	private sealed class InMemoryDataStore : IDataStore
	{
		private readonly ConcurrentDictionary<string, object> _values = new(StringComparer.Ordinal);

		public Task StoreAsync<T>(string key, T value)
		{
			_values[key] = value!;
			return Task.CompletedTask;
		}

		public Task DeleteAsync<T>(string key)
		{
			_values.TryRemove(key, out _);
			return Task.CompletedTask;
		}

		public Task<T> GetAsync<T>(string key)
		{
			return Task.FromResult(_values.TryGetValue(key, out var value) && value is T typed ? typed : default!);
		}

		public Task ClearAsync()
		{
			_values.Clear();
			return Task.CompletedTask;
		}
	}
}
