#nullable enable

using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Http;
using Google.Apis.Services;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Http;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace MudSharp.Email;

internal interface IOutboundEmailTransport : IAsyncDisposable
{
	string Name { get; }
	Task SendAsync(MimeMessage message, CancellationToken cancellationToken);
}

internal interface IOAuthAccessTokenProvider : IAsyncDisposable
{
	Task<string> GetAccessTokenAsync(CancellationToken cancellationToken);
}

internal interface IGoogleOAuthCredentialProvider : IOAuthAccessTokenProvider
{
	IConfigurableHttpClientInitializer HttpClientInitializer { get; }
}

internal interface ISmtpClientAdapter : IAsyncDisposable
{
	bool IsConnected { get; }
	Task ConnectAsync(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken);
	Task AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
	Task AuthenticateAsync(SaslMechanism mechanism, CancellationToken cancellationToken);
	Task SendAsync(MimeMessage message, CancellationToken cancellationToken);
	Task DisconnectAsync(bool quit, CancellationToken cancellationToken);
}

internal interface ISmtpClientAdapterFactory
{
	ISmtpClientAdapter Create();
}

internal interface IGmailApiClient : IAsyncDisposable
{
	Task SendAsync(string rawMessage, CancellationToken cancellationToken);
}

internal sealed class SmtpEmailTransport : IOutboundEmailTransport
{
	private readonly SmtpEmailConfiguration _configuration;
	private readonly ISmtpClientAdapterFactory _clientFactory;
	private readonly IOAuthAccessTokenProvider? _oauthAccessTokenProvider;

	public SmtpEmailTransport(SmtpEmailConfiguration configuration, ISmtpClientAdapterFactory clientFactory,
		IOAuthAccessTokenProvider? oauthAccessTokenProvider = null)
	{
		_configuration = configuration;
		_clientFactory = clientFactory;
		_oauthAccessTokenProvider = oauthAccessTokenProvider;
	}

	public string Name => "Smtp";

	public async Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
	{
		await using var client = _clientFactory.Create();
		try
		{
			await client.ConnectAsync(_configuration.Host, _configuration.Port, ToSecureSocketOptions(_configuration.TlsMode),
				cancellationToken);
			switch (_configuration.AuthenticationMode)
			{
				case SmtpAuthenticationMode.Password:
					await client.AuthenticateAsync(_configuration.Username!, _configuration.Password!, cancellationToken);
					break;
				case SmtpAuthenticationMode.GoogleOAuth2:
					var accessToken = await _oauthAccessTokenProvider!.GetAccessTokenAsync(cancellationToken);
					await client.AuthenticateAsync(new SaslMechanismOAuth2(_configuration.Username!, accessToken), cancellationToken);
					break;
			}

			await client.SendAsync(message, cancellationToken);
		}
		finally
		{
			if (client.IsConnected)
			{
				try
				{
					await client.DisconnectAsync(true, CancellationToken.None);
				}
				catch
				{
					// The original send exception is the useful delivery result. The adapter disposal still runs.
				}
			}
		}
	}

	public async ValueTask DisposeAsync()
	{
		if (_oauthAccessTokenProvider is not null)
		{
			await _oauthAccessTokenProvider.DisposeAsync();
		}
	}

	internal static SecureSocketOptions ToSecureSocketOptions(SmtpTlsMode mode)
	{
		return mode switch
		{
			SmtpTlsMode.StartTls => SecureSocketOptions.StartTls,
			SmtpTlsMode.SslOnConnect => SecureSocketOptions.SslOnConnect,
			SmtpTlsMode.None => SecureSocketOptions.None,
			_ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unsupported SMTP TLS mode.")
		};
	}
}

internal sealed class GmailApiEmailTransport : IOutboundEmailTransport
{
	private readonly IGmailApiClient _client;

	public GmailApiEmailTransport(IGmailApiClient client)
	{
		_client = client;
	}

	public string Name => "GmailApi";

	public async Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
	{
		var rawMessage = await SerializeToBase64UrlAsync(message, cancellationToken);
		await _client.SendAsync(rawMessage, cancellationToken);
	}

	public ValueTask DisposeAsync()
	{
		return _client.DisposeAsync();
	}

	internal static async Task<string> SerializeToBase64UrlAsync(MimeMessage message, CancellationToken cancellationToken)
	{
		await using var stream = new MemoryStream();
		await message.WriteToAsync(stream, cancellationToken);
		return Convert.ToBase64String(stream.ToArray())
			.TrimEnd('=')
			.Replace('+', '-')
			.Replace('/', '_');
	}
}

internal sealed class GoogleOAuthAccessTokenProvider : IGoogleOAuthCredentialProvider
{
	private readonly UserCredential _credential;

	public GoogleOAuthAccessTokenProvider(GoogleOAuthConfiguration configuration)
	{
		var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
		{
			ClientSecrets = new ClientSecrets
			{
				ClientId = configuration.ClientId,
				ClientSecret = configuration.ClientSecret
			},
			Scopes = [configuration.Scope]
		});
		_credential = new UserCredential(flow, configuration.AccountAddress,
			new TokenResponse { RefreshToken = configuration.RefreshToken });
	}

	public IConfigurableHttpClientInitializer HttpClientInitializer => _credential;

	public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
	{
		return _credential.GetAccessTokenForRequestAsync(cancellationToken: cancellationToken);
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}

internal sealed class GmailApiClient : IGmailApiClient
{
	private readonly GmailService _service;

	public GmailApiClient(IGoogleOAuthCredentialProvider credentialProvider)
	{
		_service = new GmailService(new BaseClientService.Initializer
		{
			ApplicationName = "FutureMUD",
			HttpClientInitializer = credentialProvider.HttpClientInitializer
		});
	}

	public async Task SendAsync(string rawMessage, CancellationToken cancellationToken)
	{
		var request = _service.Users.Messages.Send(new Message { Raw = rawMessage }, "me");
		await request.ExecuteAsync(cancellationToken);
	}

	public ValueTask DisposeAsync()
	{
		_service.Dispose();
		return ValueTask.CompletedTask;
	}
}

internal sealed class EmailTransportFactory
{
	public IOutboundEmailTransport Create(EmailConfiguration configuration)
	{
		return configuration.Transport switch
		{
			EmailTransportKind.Smtp => CreateSmtp(configuration.Smtp!),
			EmailTransportKind.GmailApi => CreateGmailApi(configuration.GmailApi!),
			_ => throw new InvalidOperationException("Email transport configuration is invalid.")
		};
	}

	private static IOutboundEmailTransport CreateSmtp(SmtpEmailConfiguration configuration)
	{
		IOAuthAccessTokenProvider? tokenProvider = configuration.GoogleOAuth is null
			? null
			: new GoogleOAuthAccessTokenProvider(configuration.GoogleOAuth);
		return new SmtpEmailTransport(configuration, new MailKitSmtpClientAdapterFactory(), tokenProvider);
	}

	private static IOutboundEmailTransport CreateGmailApi(GmailApiEmailConfiguration configuration)
	{
		var tokenProvider = new GoogleOAuthAccessTokenProvider(configuration.GoogleOAuth);
		return new GmailApiEmailTransport(new GmailApiClient(tokenProvider));
	}
}

internal sealed class MailKitSmtpClientAdapterFactory : ISmtpClientAdapterFactory
{
	public ISmtpClientAdapter Create()
	{
		return new MailKitSmtpClientAdapter(new SmtpClient());
	}
}

internal sealed class MailKitSmtpClientAdapter : ISmtpClientAdapter
{
	private readonly SmtpClient _client;

	public MailKitSmtpClientAdapter(SmtpClient client)
	{
		_client = client;
	}

	public bool IsConnected => _client.IsConnected;

	public Task ConnectAsync(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken)
	{
		return _client.ConnectAsync(host, port, options, cancellationToken);
	}

	public Task AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
	{
		return _client.AuthenticateAsync(username, password, cancellationToken);
	}

	public Task AuthenticateAsync(SaslMechanism mechanism, CancellationToken cancellationToken)
	{
		return _client.AuthenticateAsync(mechanism, cancellationToken);
	}

	public Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
	{
		return _client.SendAsync(message, cancellationToken);
	}

	public Task DisconnectAsync(bool quit, CancellationToken cancellationToken)
	{
		return _client.DisconnectAsync(quit, cancellationToken);
	}

	public ValueTask DisposeAsync()
	{
		_client.Dispose();
		return ValueTask.CompletedTask;
	}
}

internal enum EmailDeliveryFailureKind
{
	Transient,
	Permanent
}

internal static class EmailDeliveryFailureClassifier
{
	public static EmailDeliveryFailureKind Classify(Exception exception)
	{
		return exception switch
		{
			OperationCanceledException => EmailDeliveryFailureKind.Transient,
			ServiceNotAuthenticatedException => EmailDeliveryFailureKind.Permanent,
			SmtpCommandException smtpException when IsTransientSmtpStatus(smtpException.StatusCode) => EmailDeliveryFailureKind.Transient,
			SmtpCommandException => EmailDeliveryFailureKind.Permanent,
			GoogleApiException googleException when IsTransientGoogleStatus(googleException.HttpStatusCode) =>
				EmailDeliveryFailureKind.Transient,
			GoogleApiException => EmailDeliveryFailureKind.Permanent,
			TimeoutException or IOException or SocketException or HttpRequestException or ServiceNotConnectedException or ProtocolException =>
				EmailDeliveryFailureKind.Transient,
			_ => EmailDeliveryFailureKind.Permanent
		};
	}

	public static string DescribeSafely(Exception exception)
	{
		return exception switch
		{
			SmtpCommandException smtpException => $"SMTP status {(int)smtpException.StatusCode}",
			GoogleApiException googleException => $"Gmail HTTP {(int)googleException.HttpStatusCode}",
			_ => exception.GetType().Name
		};
	}

	private static bool IsTransientSmtpStatus(SmtpStatusCode statusCode)
	{
		var value = (int)statusCode;
		return value is >= 400 and < 500;
	}

	private static bool IsTransientGoogleStatus(System.Net.HttpStatusCode statusCode)
	{
		var value = (int)statusCode;
		return value == 408 || value == 429 || value is >= 500 and <= 599;
	}
}

internal static class FailedEmailStore
{
	public static async Task<string?> TryWriteAsync(MimeMessage message, Guid correlationId, EmailFailureHandling failureHandling,
		CancellationToken cancellationToken)
	{
		if (!failureHandling.StoreFailedMessages)
		{
			return null;
		}

		try
		{
			var directory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, failureHandling.Directory));
			var applicationDirectory = Path.GetFullPath(AppContext.BaseDirectory);
			if (!directory.StartsWith(applicationDirectory, StringComparison.OrdinalIgnoreCase))
			{
				return "InvalidDeadLetterDirectory";
			}

			Directory.CreateDirectory(directory);
			var filename = $"{DateTime.UtcNow:yyyyMMddTHHmmssfffZ}-{correlationId:N}.eml";
			var path = Path.Combine(directory, filename);
			await using (var file = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920,
				FileOptions.Asynchronous))
			{
				await message.WriteToAsync(file, cancellationToken);
			}

			if (!OperatingSystem.IsWindows())
			{
				File.SetUnixFileMode(path, UnixFileMode.UserRead | UnixFileMode.UserWrite);
			}

			return null;
		}
		catch (Exception exception) when (exception is not OperationCanceledException)
		{
			return exception.GetType().Name;
		}
	}
}
