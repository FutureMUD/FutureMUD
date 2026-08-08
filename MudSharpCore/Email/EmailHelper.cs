#nullable enable

using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;
using MimeKit;
using MimeKit.Text;
using MudSharp.Database;

namespace MudSharp.Email;

public class EmailHelper
{
	private static readonly TimeSpan[] RetryDelays =
	[
		TimeSpan.FromSeconds(10),
		TimeSpan.FromSeconds(30),
		TimeSpan.FromMinutes(2),
		TimeSpan.FromMinutes(5),
		TimeSpan.FromMinutes(15)
	];

	private readonly object _sync = new();
	private readonly SemaphoreSlim _queueSignal = new(0);
	private readonly SemaphoreSlim _transportGate = new(1, 1);
	private readonly PriorityQueue<QueuedMessage, DateTimeOffset> _messageQueue = new();
	private IReadOnlyDictionary<EmailTemplateTypes, EmailTemplate> _emailTemplates =
		new ReadOnlyDictionary<EmailTemplateTypes, EmailTemplate>(new Dictionary<EmailTemplateTypes, EmailTemplate>());
	private EmailConfiguration _configuration = new(false, EmailTransportKind.Smtp, null, null,
		EmailFailureHandling.Default);
	private IOutboundEmailTransport? _transport;
	private Task? _emailThread;
	private CancellationTokenSource? _emailThreadCancellation;
	private bool _emailThreadStarted;
	private static bool SuppressDeliveryInCurrentBuild
	{
		get
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}
	}

	private sealed record QueuedMessage(Guid CorrelationId, EmailTemplateTypes TemplateType, MimeMessage Message,
		int Attempt);

	private EmailHelper()
	{
	}

	public static EmailHelper Instance { get; } = new();

	public void TestFailSendEmail()
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress(Futuremud.Games.FirstOrDefault()?.Name ?? "FutureMUD", "fake@email.com"));
		message.To.Add(new MailboxAddress("Dummy Email", "dummy@email.com"));
		message.Subject = "This is a test email";
		message.Body = new TextPart(TextFormat.Html)
		{
			Text = "This message was generated to test configured email dead-letter storage."
		};

		EmailFailureHandling failureHandling;
		lock (_sync)
		{
			failureHandling = _configuration.FailureHandling;
		}

		if (!failureHandling.StoreFailedMessages)
		{
			Log("Email dead-letter test skipped because StoreFailedMessages is disabled.");
			return;
		}

		var result = FailedEmailStore.TryWriteAsync(message, Guid.NewGuid(), failureHandling, CancellationToken.None)
			.GetAwaiter().GetResult();
		Log(result is null
			? "Email dead-letter test completed."
			: $"Email dead-letter test did not write a message: {result}.");
	}

	public static bool SetupEmailClient()
	{
		XElement definition;
		Dictionary<EmailTemplateTypes, EmailTemplate> templates;
		try
		{
			using (new FMDB())
			{
				var staticConfiguration = FMDB.Context.StaticConfigurations
					.FirstOrDefault(x => x.SettingName == "EmailServer");
				if (staticConfiguration is null)
				{
					Log("Email configuration is missing. Outbound email configuration was not changed.");
					return false;
				}

				definition = XElement.Parse(staticConfiguration.Definition);
				templates = FMDB.Context.EmailTemplates
					.ToDictionary(x => (EmailTemplateTypes)x.TemplateType, x => new EmailTemplate(x));
			}
		}
		catch (Exception exception)
		{
			Log($"Email configuration could not be loaded. Outbound email configuration was not changed: {exception.GetType().Name}.");
			return false;
		}

		var parseResult = EmailConfigurationParser.Parse(definition, new EnvironmentEmailSecretResolver());
		if (!parseResult.Success)
		{
			Log($"Email configuration is invalid. Outbound email configuration was not changed: {parseResult.Error}");
			return false;
		}

		IOutboundEmailTransport? transport = null;
		try
		{
			if (parseResult.Configuration!.Enabled)
			{
				transport = new EmailTransportFactory().Create(parseResult.Configuration);
			}
		}
		catch (Exception exception)
		{
			Log($"Email transport could not be created. Outbound email configuration was not changed: {exception.GetType().Name}.");
			return false;
		}

		try
		{
			Instance.ApplyConfiguration(parseResult.Configuration!, templates, transport);
		}
		catch (Exception exception)
		{
			transport?.DisposeAsync().AsTask().GetAwaiter().GetResult();
			Log($"Email configuration could not be activated. Outbound email configuration was not changed: {exception.GetType().Name}.");
			return false;
		}

		foreach (var warning in parseResult.Warnings)
		{
			Log($"WARNING: {warning}");
		}

		Log(parseResult.Configuration!.Enabled
			? $"Outbound email configured with {parseResult.Configuration.Transport} transport."
			: "Outbound email is disabled by configuration.");
		return true;
	}

	public void SendEmail(EmailTemplateTypes type, string email, params string[] arguments)
	{
		if (SuppressDeliveryInCurrentBuild)
		{
			return;
		}

		var correlationId = Guid.NewGuid();
		if (string.IsNullOrWhiteSpace(email))
		{
			LogDelivery(correlationId, type, 0, "None", "InvalidRecipient", "EmptyRecipient");
			return;
		}

		EmailTemplate? template;
		EmailConfiguration configuration;
		lock (_sync)
		{
			_emailTemplates.TryGetValue(type, out template);
			configuration = _configuration;
		}

		if (template is null)
		{
			LogDelivery(correlationId, type, 0, "None", "MissingTemplate", "TemplateNotFound");
			return;
		}

		if (!configuration.Enabled)
		{
			LogDelivery(correlationId, type, 0, "None", "Disabled", "TransportDisabled");
			return;
		}

		try
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(Futuremud.Games.FirstOrDefault()?.Name ?? "FutureMUD", template.ReturnAddress));
			message.To.Add(new MailboxAddress(email, email));
			message.Subject = template.Subject;
			message.Body = new TextPart(TextFormat.Html)
			{
				Text = string.Format(CultureInfo.InvariantCulture, template.Content, arguments)
			};
			Enqueue(new QueuedMessage(correlationId, type, message, 0), DateTimeOffset.UtcNow);
		}
		catch (Exception exception) when (exception is FormatException or ParseException or ArgumentException)
		{
			LogDelivery(correlationId, type, 0, configuration.Transport.ToString(), "InvalidMessage",
				exception.GetType().Name);
		}
	}

	public void ProcessEmails()
	{
		ProcessOneDueMessageAsync(CancellationToken.None).GetAwaiter().GetResult();
	}

	public void StartEmailThread()
	{
		lock (_sync)
		{
			if (_emailThread is { IsCompleted: false })
			{
				return;
			}

			_emailThreadCancellation?.Dispose();
			_emailThreadCancellation = new CancellationTokenSource();
			_emailThreadStarted = true;
			_emailThread = Task.Run(() => EmailDelegateAsync(_emailThreadCancellation.Token));
		}

		ConsoleUtilities.WriteLine("#EStarting email handling thread...#0");
		ConsoleUtilities.WriteLine("#ASuccessfully started email handling thread.#0");
	}

	public void EndEmailThread()
	{
		Task? emailThread;
		CancellationTokenSource? cancellation;
		lock (_sync)
		{
			if (!_emailThreadStarted)
			{
				return;
			}

			_emailThreadStarted = false;
			emailThread = _emailThread;
			cancellation = _emailThreadCancellation;
			cancellation?.Cancel();
			_queueSignal.Release();
		}

		if (emailThread is null)
		{
			return;
		}

		try
		{
			emailThread.Wait(TimeSpan.FromSeconds(10));
		}
		catch (AggregateException)
		{
			// The worker records delivery failures itself; shutdown remains bounded.
		}

		if (!emailThread.IsCompleted)
		{
			Log("Email worker did not finish within the 10 second shutdown window.");
		}
	}

	private void ApplyConfiguration(EmailConfiguration configuration,
		Dictionary<EmailTemplateTypes, EmailTemplate> templates, IOutboundEmailTransport? transport)
	{
		_transportGate.Wait();
		try
		{
			IOutboundEmailTransport? oldTransport;
			lock (_sync)
			{
				oldTransport = _transport;
				_transport = transport;
				_configuration = configuration;
				_emailTemplates = new ReadOnlyDictionary<EmailTemplateTypes, EmailTemplate>(templates);
			}

			oldTransport?.DisposeAsync().AsTask().GetAwaiter().GetResult();
		}
		finally
		{
			_transportGate.Release();
		}

		_queueSignal.Release();
	}

	private async Task EmailDelegateAsync(CancellationToken cancellationToken)
	{
		try
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				var processed = await ProcessOneDueMessageAsync(cancellationToken);
				if (processed)
				{
					continue;
				}

				var delay = GetDelayUntilNextMessage();
				if (delay is null)
				{
					await _queueSignal.WaitAsync(cancellationToken);
				}
				else
				{
					await _queueSignal.WaitAsync(delay.Value, cancellationToken);
				}
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
		}
		finally
		{
			lock (_sync)
			{
				if (_emailThreadCancellation?.Token == cancellationToken)
				{
					_emailThreadStarted = false;
				}
			}
		}
	}

	private async Task<bool> ProcessOneDueMessageAsync(CancellationToken cancellationToken)
	{
		QueuedMessage? queuedMessage = null;
		lock (_sync)
		{
			if (_messageQueue.TryPeek(out var nextMessage, out var dueAt) && dueAt <= DateTimeOffset.UtcNow)
			{
				_messageQueue.Dequeue();
				queuedMessage = nextMessage;
			}
		}

		if (queuedMessage is null)
		{
			return false;
		}

		IOutboundEmailTransport? transport;
		EmailConfiguration configuration;
		await _transportGate.WaitAsync(cancellationToken);
		try
		{
			lock (_sync)
			{
				transport = _transport;
				configuration = _configuration;
			}

			if (!configuration.Enabled || transport is null)
			{
				Enqueue(queuedMessage, DateTimeOffset.UtcNow + TimeSpan.FromSeconds(10));
				return true;
			}

			try
			{
				await transport.SendAsync(queuedMessage.Message, cancellationToken);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				Enqueue(queuedMessage, DateTimeOffset.UtcNow);
				throw;
			}
			catch (Exception exception)
			{
				await HandleDeliveryFailureAsync(queuedMessage, configuration, transport.Name, exception, cancellationToken);
			}
		}
		finally
		{
			_transportGate.Release();
		}

		return true;
	}

	private async Task HandleDeliveryFailureAsync(QueuedMessage queuedMessage, EmailConfiguration configuration,
		string transportName, Exception exception, CancellationToken cancellationToken)
	{
		var failureKind = EmailDeliveryFailureClassifier.Classify(exception);
		var completedAttempt = queuedMessage.Attempt + 1;
		if (failureKind == EmailDeliveryFailureKind.Transient && completedAttempt < configuration.FailureHandling.MaxAttempts)
		{
			var retryDelay = RetryDelays[Math.Min(completedAttempt - 1, RetryDelays.Length - 1)];
			Enqueue(queuedMessage with { Attempt = completedAttempt }, DateTimeOffset.UtcNow + retryDelay);
			LogDelivery(queuedMessage.CorrelationId, queuedMessage.TemplateType, completedAttempt, transportName, "RetryScheduled",
				EmailDeliveryFailureClassifier.DescribeSafely(exception));
			return;
		}

		var category = failureKind == EmailDeliveryFailureKind.Permanent ? "PermanentFailure" : "AttemptsExhausted";
		LogDelivery(queuedMessage.CorrelationId, queuedMessage.TemplateType, completedAttempt, transportName, category,
			EmailDeliveryFailureClassifier.DescribeSafely(exception));
		var deadLetterResult = await FailedEmailStore.TryWriteAsync(queuedMessage.Message, queuedMessage.CorrelationId,
			configuration.FailureHandling, cancellationToken);
		if (deadLetterResult is not null)
		{
			LogDelivery(queuedMessage.CorrelationId, queuedMessage.TemplateType, completedAttempt, transportName,
				"DeadLetterWriteFailed", deadLetterResult);
		}
	}

	private void Enqueue(QueuedMessage message, DateTimeOffset dueAt)
	{
		lock (_sync)
		{
			_messageQueue.Enqueue(message, dueAt);
		}

		_queueSignal.Release();
	}

	private TimeSpan? GetDelayUntilNextMessage()
	{
		lock (_sync)
		{
			if (!_messageQueue.TryPeek(out _, out var dueAt))
			{
				return null;
			}

			var delay = dueAt - DateTimeOffset.UtcNow;
			return delay > TimeSpan.Zero ? delay : TimeSpan.Zero;
		}
	}

	private static void LogDelivery(Guid correlationId, EmailTemplateTypes templateType, int attempt, string transport,
		string category, string error)
	{
		Log($"Email delivery id={correlationId:N} template={templateType} attempt={attempt} transport={transport} " +
			$"category={category} error={BoundedSingleLine(error)}");
	}

	private static string BoundedSingleLine(string value)
	{
		var singleLine = value.Replace('\r', ' ').Replace('\n', ' ').Trim();
		return singleLine.Length <= 240 ? singleLine : singleLine[..240];
	}

	private static void Log(string message)
	{
		Console.WriteLine(message);
	}
}
