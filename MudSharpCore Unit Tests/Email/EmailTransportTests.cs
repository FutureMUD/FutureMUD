#nullable enable

using MailKit.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Email;
using MimeKit;
using MimeKit.Text;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests.Email;

[TestClass]
public class EmailTransportTests
{
	[TestMethod]
	public async Task SmtpTransport_UsesConfiguredStartTlsPasswordAuthenticationAndDisconnects()
	{
		var client = new RecordingSmtpClient();
		var transport = new SmtpEmailTransport(
			new SmtpEmailConfiguration("smtp.example.test", 587, SmtpTlsMode.StartTls, false,
				SmtpAuthenticationMode.Password, "game@example.test", "password", null),
			new RecordingSmtpClientFactory(client));

		await transport.SendAsync(CreateMessage(), CancellationToken.None);

		Assert.AreEqual(SecureSocketOptions.StartTls, client.SocketOptions);
		Assert.AreEqual("game@example.test", client.PasswordUsername);
		Assert.AreEqual("password", client.Password);
		Assert.IsTrue(client.Disconnected);
	}

	[TestMethod]
	public async Task SmtpTransport_UsesExplicitOAuth2Mechanism()
	{
		var client = new RecordingSmtpClient();
		var tokenProvider = new RecordingTokenProvider("access-token");
		var transport = new SmtpEmailTransport(
			new SmtpEmailConfiguration("smtp.gmail.com", 587, SmtpTlsMode.StartTls, false,
				SmtpAuthenticationMode.GoogleOAuth2, "game@example.test", null,
				new GoogleOAuthConfiguration("game@example.test", "id", "secret", "refresh", GoogleEmailAuthorization.SmtpScope)),
			new RecordingSmtpClientFactory(client), tokenProvider);

		await transport.SendAsync(CreateMessage(), CancellationToken.None);

		Assert.AreEqual("XOAUTH2", client.SaslMechanismName);
		Assert.IsTrue(tokenProvider.WasRequested);
	}

	[TestMethod]
	public async Task SmtpTransport_HonoursCancellationDuringConnection()
	{
		var client = new RecordingSmtpClient { ThrowCancellationOnConnect = true };
		var transport = new SmtpEmailTransport(
			new SmtpEmailConfiguration("smtp.example.test", 587, SmtpTlsMode.StartTls, false,
				SmtpAuthenticationMode.None, null, null, null), new RecordingSmtpClientFactory(client));
		using var cancellation = new CancellationTokenSource();
		cancellation.Cancel();

		await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => transport.SendAsync(CreateMessage(), cancellation.Token));
	}

	[TestMethod]
	public async Task GmailApiTransport_EncodesTheSerializedMimeMessageAsBase64Url()
	{
		var client = new RecordingGmailClient();
		var message = CreateMessage();
		var transport = new GmailApiEmailTransport(client);

		await transport.SendAsync(message, CancellationToken.None);

		await using var stream = new MemoryStream();
		await message.WriteToAsync(stream, CancellationToken.None);
		var expected = Convert.ToBase64String(stream.ToArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		Assert.AreEqual(expected, client.RawMessage);
		Assert.IsFalse(client.RawMessage!.Contains('='));
	}

	[TestMethod]
	public void FailureClassifier_TreatsNetworkAndAuthenticationFailuresDifferently()
	{
		Assert.AreEqual(EmailDeliveryFailureKind.Transient,
			EmailDeliveryFailureClassifier.Classify(new TimeoutException()));
		Assert.AreEqual(EmailDeliveryFailureKind.Permanent,
			EmailDeliveryFailureClassifier.Classify(new MailKit.ServiceNotAuthenticatedException("authentication failed")));
	}

	private static MimeMessage CreateMessage()
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("FutureMUD", "game@example.test"));
		message.To.Add(new MailboxAddress("Player", "player@example.test"));
		message.Subject = "Subject";
		message.Body = new TextPart(TextFormat.Html) { Text = "<p>Body</p>" };
		return message;
	}

	private sealed class RecordingSmtpClientFactory : ISmtpClientAdapterFactory
	{
		private readonly RecordingSmtpClient _client;

		public RecordingSmtpClientFactory(RecordingSmtpClient client)
		{
			_client = client;
		}

		public ISmtpClientAdapter Create()
		{
			return _client;
		}
	}

	private sealed class RecordingSmtpClient : ISmtpClientAdapter
	{
		public bool ThrowCancellationOnConnect { get; init; }
		public bool IsConnected { get; private set; }
		public SecureSocketOptions? SocketOptions { get; private set; }
		public string? PasswordUsername { get; private set; }
		public string? Password { get; private set; }
		public string? SaslMechanismName { get; private set; }
		public bool Disconnected { get; private set; }

		public Task ConnectAsync(string host, int port, SecureSocketOptions options, CancellationToken cancellationToken)
		{
			SocketOptions = options;
			if (ThrowCancellationOnConnect)
			{
				cancellationToken.ThrowIfCancellationRequested();
			}

			IsConnected = true;
			return Task.CompletedTask;
		}

		public Task AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
		{
			PasswordUsername = username;
			Password = password;
			return Task.CompletedTask;
		}

		public Task AuthenticateAsync(SaslMechanism mechanism, CancellationToken cancellationToken)
		{
			SaslMechanismName = mechanism.MechanismName;
			return Task.CompletedTask;
		}

		public Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public Task DisconnectAsync(bool quit, CancellationToken cancellationToken)
		{
			Disconnected = true;
			IsConnected = false;
			return Task.CompletedTask;
		}

		public ValueTask DisposeAsync()
		{
			return ValueTask.CompletedTask;
		}
	}

	private sealed class RecordingTokenProvider : IOAuthAccessTokenProvider
	{
		private readonly string _token;

		public RecordingTokenProvider(string token)
		{
			_token = token;
		}

		public bool WasRequested { get; private set; }

		public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
		{
			WasRequested = true;
			return Task.FromResult(_token);
		}

		public ValueTask DisposeAsync()
		{
			return ValueTask.CompletedTask;
		}
	}

	private sealed class RecordingGmailClient : IGmailApiClient
	{
		public string? RawMessage { get; private set; }

		public Task SendAsync(string rawMessage, CancellationToken cancellationToken)
		{
			RawMessage = rawMessage;
			return Task.CompletedTask;
		}

		public ValueTask DisposeAsync()
		{
			return ValueTask.CompletedTask;
		}
	}
}
