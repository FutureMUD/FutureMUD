using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MimeKit;
using MudSharp.Database;
using MailKit.Net.Smtp;
using MimeKit.Text;
using MudSharp.Framework;
using System.IO;
using System.Reflection;

namespace MudSharp.Email;

public class EmailHelper
{
	private readonly Dictionary<EmailTemplateTypes, EmailTemplate> _emailTemplates =
		new();

	private record QueuedMessage(MimeMessage Message, int NumberOfTries = 0);

	public void TestFailSendEmail()
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress(Futuremud.Games.First().Name, "fake@email.com"));
		message.To.Add(new MailboxAddress("Dummy Email", "dummy@email.com"));
		message.Subject = "This is a test email";
		message.Body = new TextPart(TextFormat.Html)
		{
			Text =
				"This is a test email which was automatically generated to test failure of the email system. Hopefully, you are reading this email from the directory where it was written to."
		};
		FailSendEmail(message);
	}

	private void FailSendEmail(MimeMessage message)
	{
		try
		{
			var info = Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) +
			                                     "\\FailedEmails");
			message.WriteTo(new FileStream(
				info.FullName + $"\\{DateTime.UtcNow.ToString("yyyyMMddhhmmss")} {message.Subject}.eml",
				FileMode.Create));
		}
		catch
		{
			// Swallow the exception
		}
	}

	private readonly Queue<QueuedMessage> _messageQueue = new();
	private Task _emailThread;
	private CancellationTokenSource _emailThreadCancellation;

	private bool _emailThreadStarted;

	private string _host;
	private int _port;
	private bool _ssl;
	private bool _defaultCredentials;
	private string _username;
	private string _password;

	private EmailHelper()
	{
	}

	public static EmailHelper Instance { get; } = new();

	private bool LoadFromXml(XElement root)
	{
		var element = root.Element("Host");
		if (element == null)
		{
			return false;
		}

		_host = element.Value;

		element = root.Element("Port");
		if (element == null)
		{
			return false;
		}

		_port = int.Parse(element.Value);

		element = root.Element("EnableSSL");
		if (element == null)
		{
			return false;
		}

		_ssl = bool.Parse(element.Value);

		element = root.Element("UseDefaultCredentials");
		if (element == null)
		{
			return false;
		}

		_defaultCredentials = bool.Parse(element.Value);


		element = root.Element("Credentials");
		if (element == null)
		{
			return false;
		}

		_username = element.Attribute("Username").Value;
		_password = element.Attribute("Password").Value;

		ServicePointManager.ServerCertificateValidationCallback =
			delegate { return true; };

		return true;
	}

	public static bool SetupEmailClient()
	{
		try
		{
			using (new FMDB())
			{
				if (
					!Instance.LoadFromXml(
						XElement.Parse(
							FMDB.Context.StaticConfigurations.First(x => x.SettingName == "EmailServer").Definition)))
				{
					return false;
				}

				foreach (var item in FMDB.Context.EmailTemplates)
				{
					Instance._emailTemplates[(EmailTemplateTypes)item.TemplateType] = new EmailTemplate(item);
				}
			}

			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine("Exception in SetupEmailClient: " + e.Message);
		}

		return false;
	}

	public void SendEmail(EmailTemplateTypes type, string email, params string[] arguments)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return;
		}

		if (!_emailTemplates.ContainsKey(type))
		{
			Console.WriteLine($"Warning: Had no email template for type {type.DescribeEnum()}.");
			return;
		}

		var template = _emailTemplates[type];
		lock (_messageQueue)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(Futuremud.Games.First().Name, template.ReturnAddress));
			message.To.Add(new MailboxAddress(email, email));
			message.Subject = template.Subject;
			message.Body = new TextPart(TextFormat.Html)
			{
				Text = string.Format(template.Content, arguments)
			};
			_messageQueue.Enqueue(new QueuedMessage(message, 0));
		}
	}

	public void ProcessEmails()
	{
#if DEBUG
		// Don't process emails on debug
		return;
#endif
		lock (_messageQueue)
		{
			if (_messageQueue.Count <= 0)
			{
				return;
			}
		}

		using var client = new SmtpClient();
		try
		{
			client.Connect(_host, _port, _ssl);
			if (!_defaultCredentials)
			{
				client.Authenticate(_username, _password);
			}
		}
		catch (Exception e)
		{
			Console.WriteLine("WARNING: Exception while connecting to the email server: {0}", e.Message);
			return;
		}

		lock (_messageQueue)
		{
			while (_messageQueue.Any())
			{
				var queued = _messageQueue.Dequeue();
				var message = queued.Message;
				try
				{
					client.Send(message);
				}
				catch (MailKit.ServiceNotAuthenticatedException e)
				{
					Console.WriteLine("WARNING: Email service was not authenticated: {0}", e.Message);
					Console.WriteLine();
					Console.WriteLine($"To: {message.To.ToString()}");
					Console.WriteLine($"Subject: {message.Subject}");

					Console.WriteLine();
					Console.WriteLine("Message:");
					Console.WriteLine(message.Body.ToString());
					Console.WriteLine();
					FailSendEmail(message);
				}
				catch (MailKit.ServiceNotConnectedException e)
				{
					Console.WriteLine("WARNING: Email service was not connected: {0}", e.Message);
					Console.WriteLine();
					Console.WriteLine($"To: {message.To.ToString()}");
					Console.WriteLine($"Subject: {message.Subject}");

					Console.WriteLine();
					Console.WriteLine("Message:");
					Console.WriteLine(message.Body.ToString());
					Console.WriteLine();
					if (queued.NumberOfTries >= 5)
					{
						FailSendEmail(message);
					}
					else
					{
						_messageQueue.Enqueue(queued with { NumberOfTries = queued.NumberOfTries + 1 });
					}
				}
				catch (MailKit.ProtocolException e)
				{
					Console.WriteLine("WARNING: Protocol exception in email service: {0}", e.Message);
					Console.WriteLine();
					Console.WriteLine($"To: {message.To.ToString()}");
					Console.WriteLine($"Subject: {message.Subject}");

					Console.WriteLine();
					Console.WriteLine("Message:");
					Console.WriteLine(message.Body.ToString());
					Console.WriteLine();
					if (queued.NumberOfTries >= 5)
					{
						FailSendEmail(message);
					}
					else
					{
						_messageQueue.Enqueue(queued with { NumberOfTries = queued.NumberOfTries + 1 });
					}
				}
				catch (MailKit.CommandException e)
				{
					Console.WriteLine("WARNING: Command exception in email service: {0}", e);
					Console.WriteLine();
					Console.WriteLine($"To: {message.To.ToString()}");
					Console.WriteLine($"Subject: {message.Subject}");

					Console.WriteLine();
					Console.WriteLine("Message:");
					Console.WriteLine(message.Body.ToString());
					Console.WriteLine();
					if (queued.NumberOfTries >= 5)
					{
						FailSendEmail(message);
					}
					else
					{
						_messageQueue.Enqueue(queued with { NumberOfTries = queued.NumberOfTries + 1 });
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("WARNING: Unknown Exception in Email Send: {0}", e);
				}
			}
		}

		client.Disconnect(true);
	}

	public void StartEmailThread()
	{
		if (_emailThreadStarted)
		{
			return;
		}

		ConsoleUtilities.WriteLine("#EStarting email handling thread...#0");
		_emailThreadCancellation = new CancellationTokenSource();
		_emailThread = Task.Factory.StartNew(EmailDelegate, _emailThreadCancellation.Token);
		ConsoleUtilities.WriteLine("#ASuccessfully started email handling thread.#0");
	}

	public void EndEmailThread()
	{
		if (_emailThread != null)
		{
			_emailThreadCancellation.Cancel();
			_emailThread = null;
			_emailThreadStarted = false;
		}
	}

	private void EmailDelegate()
	{
		while (true)
		{
			ProcessEmails();
			Thread.Sleep(10000);
		}
	}
}