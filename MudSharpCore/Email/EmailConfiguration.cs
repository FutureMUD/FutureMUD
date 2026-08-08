#nullable enable

using System.Globalization;
using System.IO;
using MimeKit;

namespace MudSharp.Email;

internal enum EmailTransportKind
{
	Smtp,
	GmailApi
}

internal enum SmtpTlsMode
{
	None,
	StartTls,
	SslOnConnect
}

internal enum SmtpAuthenticationMode
{
	None,
	Password,
	GoogleOAuth2
}

internal sealed record EmailFailureHandling(int MaxAttempts, bool StoreFailedMessages, string Directory)
{
	public static EmailFailureHandling Default { get; } = new(6, false, "FailedEmails");
}

internal sealed record GoogleOAuthConfiguration(
	string AccountAddress,
	string ClientId,
	string ClientSecret,
	string RefreshToken,
	string Scope);

internal sealed record SmtpEmailConfiguration(
	string Host,
	int Port,
	SmtpTlsMode TlsMode,
	bool AllowUnencryptedTransport,
	SmtpAuthenticationMode AuthenticationMode,
	string? Username,
	string? Password,
	GoogleOAuthConfiguration? GoogleOAuth);

internal sealed record GmailApiEmailConfiguration(GoogleOAuthConfiguration GoogleOAuth);

internal sealed record EmailConfiguration(
	bool Enabled,
	EmailTransportKind Transport,
	SmtpEmailConfiguration? Smtp,
	GmailApiEmailConfiguration? GmailApi,
	EmailFailureHandling FailureHandling);

internal sealed record EmailConfigurationParseResult(
	EmailConfiguration? Configuration,
	IReadOnlyList<string> Warnings,
	string? Error)
{
	public bool Success => Configuration is not null && string.IsNullOrEmpty(Error);
}

internal static class EmailConfigurationParser
{
	public static EmailConfigurationParseResult Parse(XElement root, IEmailSecretResolver secretResolver)
	{
		var warnings = new List<string>();
		if (root.Element("Version") is null && root.Element("Host") is not null)
		{
			return ParseLegacy(root, secretResolver, warnings);
		}

		var version = RequiredValue(root, "Version", out var versionError);
		if (versionError is not null || !version.Equals("2", StringComparison.Ordinal))
		{
			return Failure(warnings, versionError ?? "Email configuration Version must be 2.");
		}

		if (!TryGetBoolean(root, "Enabled", out var enabled, out var enabledError))
		{
			return Failure(warnings, enabledError!);
		}

		var failureHandlingResult = ParseFailureHandling(root);
		if (failureHandlingResult.Error is not null)
		{
			return Failure(warnings, failureHandlingResult.Error);
		}

		if (!enabled)
		{
			return Success(new EmailConfiguration(false, EmailTransportKind.Smtp, null, null,
				failureHandlingResult.FailureHandling));
		}

		var transportText = RequiredValue(root, "Transport", out var transportError);
		if (transportError is not null || !Enum.TryParse<EmailTransportKind>(transportText, true, out var transport) ||
			!Enum.IsDefined(transport))
		{
			return Failure(warnings, transportError ?? "Email transport must be Smtp or GmailApi.");
		}

		return transport switch
		{
			EmailTransportKind.Smtp => ParseSmtp(root.Element("Smtp"), secretResolver, warnings,
				failureHandlingResult.FailureHandling),
			EmailTransportKind.GmailApi => ParseGmailApi(root.Element("GmailApi"), secretResolver, warnings,
				failureHandlingResult.FailureHandling),
			_ => Failure(warnings, "Email transport must be Smtp or GmailApi.")
		};
	}

	private static EmailConfigurationParseResult ParseLegacy(XElement root, IEmailSecretResolver secretResolver,
		List<string> warnings)
	{
		var host = RequiredValue(root, "Host", out var hostError);
		if (hostError is not null)
		{
			return Failure(warnings, hostError);
		}

		if (!TryGetPort(root, "Port", out var port, out var portError))
		{
			return Failure(warnings, portError!);
		}

		if (!TryGetBoolean(root, "EnableSSL", out var enableSsl, out var sslError))
		{
			return Failure(warnings, sslError!);
		}

		if (!TryGetBoolean(root, "UseDefaultCredentials", out var useDefaultCredentials, out var credentialsError))
		{
			return Failure(warnings, credentialsError!);
		}

		var credentials = root.Element("Credentials");
		var username = credentials?.Attribute("Username")?.Value?.Trim();
		var password = credentials?.Attribute("Password")?.Value;
		if (!useDefaultCredentials && (string.IsNullOrWhiteSpace(username) || password is null))
		{
			return Failure(warnings, "Legacy password SMTP configuration requires Credentials Username and Password.");
		}

		if (!useDefaultCredentials && password!.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
		{
			if (!secretResolver.TryResolve(password, out password, out var secretError))
			{
				return Failure(warnings, secretError);
			}
		}
		else if (!useDefaultCredentials)
		{
			warnings.Add(
				"Legacy email configuration contains an inline SMTP password. Move it to an environment variable and set Credentials Password to env:VARIABLE_NAME.");
		}

		warnings.Add("Legacy EmailServer configuration is deprecated. Migrate it to Version 2 configuration.");
		return Success(new EmailConfiguration(
			true,
			EmailTransportKind.Smtp,
			new SmtpEmailConfiguration(
				host,
				port,
				enableSsl ? SmtpTlsMode.SslOnConnect : SmtpTlsMode.StartTls,
				false,
				useDefaultCredentials ? SmtpAuthenticationMode.None : SmtpAuthenticationMode.Password,
				useDefaultCredentials ? null : username,
				useDefaultCredentials ? null : password,
				null),
			null,
			EmailFailureHandling.Default), warnings);
	}

	private static EmailConfigurationParseResult ParseSmtp(XElement? element, IEmailSecretResolver secretResolver,
		List<string> warnings, EmailFailureHandling failureHandling)
	{
		if (element is null)
		{
			return Failure(warnings, "Enabled SMTP email configuration requires an Smtp element.");
		}

		var host = RequiredValue(element, "Host", out var hostError);
		if (hostError is not null)
		{
			return Failure(warnings, hostError);
		}

		if (!TryGetPort(element, "Port", out var port, out var portError))
		{
			return Failure(warnings, portError!);
		}

		if (!TryGetEnum(element, "TlsMode", out SmtpTlsMode tlsMode, out var tlsError))
		{
			return Failure(warnings, tlsError!);
		}

		var allowUnencryptedTransport = false;
		if (tlsMode == SmtpTlsMode.None &&
			(!TryGetBoolean(element, "AllowUnencryptedTransport", out allowUnencryptedTransport, out var allowError) ||
			 !allowUnencryptedTransport))
		{
			return Failure(warnings, allowError ??
				"TlsMode None requires AllowUnencryptedTransport=true as an explicit opt-in.");
		}

		if (!TryGetEnum(element, "AuthenticationMode", out SmtpAuthenticationMode authenticationMode,
			out var authenticationError))
		{
			return Failure(warnings, authenticationError!);
		}

		var username = Value(element, "Username");
		string? password = null;
		GoogleOAuthConfiguration? googleOAuth = null;
		switch (authenticationMode)
		{
			case SmtpAuthenticationMode.Password:
				if (string.IsNullOrWhiteSpace(username))
				{
					return Failure(warnings, "Password SMTP authentication requires Username.");
				}

				if (!TryResolveSecret(element, "PasswordReference", secretResolver, out password, out var passwordError))
				{
					return Failure(warnings, passwordError!);
				}

				break;
			case SmtpAuthenticationMode.GoogleOAuth2:
				if (string.IsNullOrWhiteSpace(username))
				{
					return Failure(warnings, "GoogleOAuth2 SMTP authentication requires Username.");
				}

				var oauthResult = ParseGoogleOAuth(element.Element("GoogleOAuth"), username, "https://mail.google.com/",
					secretResolver);
				if (oauthResult.Error is not null)
				{
					return Failure(warnings, oauthResult.Error);
				}

				googleOAuth = oauthResult.Configuration;
				break;
		}

		return Success(new EmailConfiguration(true, EmailTransportKind.Smtp,
			new SmtpEmailConfiguration(host, port, tlsMode, allowUnencryptedTransport, authenticationMode, username, password,
				googleOAuth), null, failureHandling), warnings);
	}

	private static EmailConfigurationParseResult ParseGmailApi(XElement? element, IEmailSecretResolver secretResolver,
		List<string> warnings, EmailFailureHandling failureHandling)
	{
		var oauthResult = ParseGoogleOAuth(element, null, "https://www.googleapis.com/auth/gmail.send", secretResolver);
		if (oauthResult.Error is not null)
		{
			return Failure(warnings, oauthResult.Error);
		}

		return Success(new EmailConfiguration(true, EmailTransportKind.GmailApi, null,
			new GmailApiEmailConfiguration(oauthResult.Configuration!), failureHandling), warnings);
	}

	private static (GoogleOAuthConfiguration? Configuration, string? Error) ParseGoogleOAuth(XElement? element,
		string? defaultAccountAddress, string scope, IEmailSecretResolver secretResolver)
	{
		if (element is null)
		{
			return (null, "Google email configuration requires a GoogleOAuth element.");
		}

		string accountAddress;
		string? accountError = null;
		if (defaultAccountAddress is null)
		{
			accountAddress = RequiredValue(element, "AccountAddress", out accountError);
		}
		else
		{
			accountAddress = defaultAccountAddress;
		}

		if (accountError is not null || string.IsNullOrWhiteSpace(accountAddress))
		{
			return (null, accountError ?? "Google email configuration requires AccountAddress.");
		}

		try
		{
			MailboxAddress.Parse(accountAddress);
		}
		catch (ParseException)
		{
			return (null, "Google email configuration AccountAddress is not a valid email address.");
		}

		var clientId = RequiredValue(element, "ClientId", out var clientIdError);
		if (clientIdError is not null)
		{
			return (null, clientIdError);
		}

		if (!TryResolveSecret(element, "ClientSecretReference", secretResolver, out var clientSecret,
			out var clientSecretError))
		{
			return (null, clientSecretError!);
		}

		if (!TryResolveSecret(element, "RefreshTokenReference", secretResolver, out var refreshToken,
			out var refreshTokenError))
		{
			return (null, refreshTokenError!);
		}

		return (new GoogleOAuthConfiguration(accountAddress, clientId, clientSecret!, refreshToken!, scope), null);
	}

	private static (EmailFailureHandling FailureHandling, string? Error) ParseFailureHandling(XElement root)
	{
		var element = root.Element("FailureHandling");
		if (element is null)
		{
			return (EmailFailureHandling.Default, null);
		}

		var maxAttempts = EmailFailureHandling.Default.MaxAttempts;
		var maxAttemptsText = Value(element, "MaxAttempts");
		if (!string.IsNullOrEmpty(maxAttemptsText) &&
			(!int.TryParse(maxAttemptsText, NumberStyles.None, CultureInfo.InvariantCulture, out maxAttempts) ||
			 maxAttempts is < 1 or > 6))
		{
			return (EmailFailureHandling.Default, "FailureHandling MaxAttempts must be between 1 and 6.");
		}

		var storeFailedMessages = false;
		var storeText = Value(element, "StoreFailedMessages");
		if (!string.IsNullOrEmpty(storeText) && !bool.TryParse(storeText, out storeFailedMessages))
		{
			return (EmailFailureHandling.Default, "FailureHandling StoreFailedMessages must be true or false.");
		}

		var directory = Value(element, "Directory") ?? EmailFailureHandling.Default.Directory;
		if (!IsSafeRelativeDirectory(directory))
		{
			return (EmailFailureHandling.Default,
				"FailureHandling Directory must be a relative directory below the application directory.");
		}

		return (new EmailFailureHandling(maxAttempts, storeFailedMessages, directory), null);
	}

	private static bool TryResolveSecret(XElement element, string name, IEmailSecretResolver resolver, out string? secret,
		out string? error)
	{
		secret = null;
		var reference = RequiredValue(element, name, out error);
		if (error is not null)
		{
			return false;
		}

		if (!resolver.TryResolve(reference, out var value, out var resolverError))
		{
			error = resolverError;
			return false;
		}

		secret = value;
		error = null;
		return true;
	}

	private static bool TryGetPort(XElement element, string name, out int port, out string? error)
	{
		port = 0;
		var text = RequiredValue(element, name, out error);
		if (error is not null)
		{
			return false;
		}

		if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out port) || port is < 1 or > 65535)
		{
			error = $"Email {name} must be between 1 and 65535.";
			return false;
		}

		error = null;
		return true;
	}

	private static bool TryGetBoolean(XElement element, string name, out bool value, out string? error)
	{
		value = false;
		var text = RequiredValue(element, name, out error);
		if (error is not null)
		{
			return false;
		}

		if (!bool.TryParse(text, out value))
		{
			error = $"Email {name} must be true or false.";
			return false;
		}

		error = null;
		return true;
	}

	private static bool TryGetEnum<T>(XElement element, string name, out T value, out string? error)
		where T : struct, Enum
	{
		value = default;
		var text = RequiredValue(element, name, out error);
		if (error is not null)
		{
			return false;
		}

		if (!Enum.TryParse(text, true, out value) || !Enum.IsDefined(value))
		{
			error = $"Email {name} has an invalid value.";
			return false;
		}

		error = null;
		return true;
	}

	private static string RequiredValue(XElement element, string name, out string? error)
	{
		var value = Value(element, name);
		if (string.IsNullOrWhiteSpace(value))
		{
			error = $"Email configuration requires {name}.";
			return string.Empty;
		}

		error = null;
		return value;
	}

	private static string? Value(XElement element, string name)
	{
		return element.Element(name)?.Value.Trim();
	}

	private static bool IsSafeRelativeDirectory(string directory)
	{
		if (string.IsNullOrWhiteSpace(directory) || Path.IsPathRooted(directory))
		{
			return false;
		}

		var fullPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, directory));
		var basePath = Path.GetFullPath(AppContext.BaseDirectory);
		return fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase);
	}

	private static EmailConfigurationParseResult Success(EmailConfiguration configuration, List<string>? warnings = null)
	{
		return new EmailConfigurationParseResult(configuration, warnings ?? [], null);
	}

	private static EmailConfigurationParseResult Failure(List<string> warnings, string error)
	{
		return new EmailConfigurationParseResult(null, warnings, error);
	}
}
