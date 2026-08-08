#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Email;
using MudSharp.Framework;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests.Email;

[TestClass]
public class EmailConfigurationTests
{
	[TestMethod]
	public void StaticDefault_DisablesVersion2OutboundEmailConfiguration()
	{
		var definition = XElement.Parse(DefaultStaticSettings.DefaultStaticConfigurations["EmailServer"]);

		var result = EmailConfigurationParser.Parse(definition, new TestSecretResolver());

		Assert.IsTrue(result.Success, result.Error);
		Assert.IsFalse(result.Configuration!.Enabled);
	}

	[TestMethod]
	public void Parse_Version2PasswordSmtp_UsesExplicitStartTlsAndEnvironmentSecret()
	{
		var result = EmailConfigurationParser.Parse(XElement.Parse(@"<Definition>
  <Version>2</Version><Enabled>true</Enabled><Transport>Smtp</Transport>
  <Smtp><Host>smtp.example.test</Host><Port>587</Port><TlsMode>StartTls</TlsMode>
    <AuthenticationMode>Password</AuthenticationMode><Username>game@example.test</Username>
    <PasswordReference>env:SMTP_PASSWORD</PasswordReference></Smtp>
</Definition>"), new TestSecretResolver());

		Assert.IsTrue(result.Success, result.Error);
		Assert.AreEqual(SmtpTlsMode.StartTls, result.Configuration!.Smtp!.TlsMode);
		Assert.AreEqual(SmtpAuthenticationMode.Password, result.Configuration.Smtp.AuthenticationMode);
		Assert.AreEqual("resolved-SMTP_PASSWORD", result.Configuration.Smtp.Password);
	}

	[TestMethod]
	public void Parse_UnencryptedSmtpWithoutExplicitOptIn_IsRejected()
	{
		var result = EmailConfigurationParser.Parse(XElement.Parse(@"<Definition>
  <Version>2</Version><Enabled>true</Enabled><Transport>Smtp</Transport>
  <Smtp><Host>relay.example.test</Host><Port>25</Port><TlsMode>None</TlsMode>
    <AuthenticationMode>None</AuthenticationMode></Smtp>
</Definition>"), new TestSecretResolver());

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Error!, "AllowUnencryptedTransport");
	}

	[TestMethod]
	public void Parse_UnencryptedSmtpWithExplicitOptIn_IsAccepted()
	{
		var result = EmailConfigurationParser.Parse(XElement.Parse(@"<Definition>
  <Version>2</Version><Enabled>true</Enabled><Transport>Smtp</Transport>
  <Smtp><Host>127.0.0.1</Host><Port>25</Port><TlsMode>None</TlsMode>
    <AllowUnencryptedTransport>true</AllowUnencryptedTransport><AuthenticationMode>None</AuthenticationMode></Smtp>
</Definition>"), new TestSecretResolver());

		Assert.IsTrue(result.Success, result.Error);
		Assert.AreEqual(SmtpTlsMode.None, result.Configuration!.Smtp!.TlsMode);
	}

	[TestMethod]
	public void Parse_GmailApi_UsesNarrowSendScopeAndSecretReferences()
	{
		var result = EmailConfigurationParser.Parse(XElement.Parse(@"<Definition>
  <Version>2</Version><Enabled>true</Enabled><Transport>GmailApi</Transport>
  <GmailApi><AccountAddress>game@example.test</AccountAddress><ClientId>client-id</ClientId>
    <ClientSecretReference>env:GOOGLE_CLIENT_SECRET</ClientSecretReference>
    <RefreshTokenReference>env:GOOGLE_REFRESH_TOKEN</RefreshTokenReference></GmailApi>
</Definition>"), new TestSecretResolver());

		Assert.IsTrue(result.Success, result.Error);
		Assert.AreEqual(GoogleEmailAuthorization.GmailApiScope, result.Configuration!.GmailApi!.GoogleOAuth.Scope);
		Assert.AreEqual("resolved-GOOGLE_REFRESH_TOKEN", result.Configuration.GmailApi.GoogleOAuth.RefreshToken);
	}

	[TestMethod]
	public void Parse_LegacyConfiguration_MapsSslAndUnauthenticatedRelay()
	{
		var result = EmailConfigurationParser.Parse(XElement.Parse(@"<Definition>
  <Host>relay.example.test</Host><Port>465</Port><EnableSSL>true</EnableSSL>
  <UseDefaultCredentials>true</UseDefaultCredentials><Credentials Username=""ignored"" Password=""ignored"" />
</Definition>"), new TestSecretResolver());

		Assert.IsTrue(result.Success, result.Error);
		Assert.AreEqual(SmtpTlsMode.SslOnConnect, result.Configuration!.Smtp!.TlsMode);
		Assert.AreEqual(SmtpAuthenticationMode.None, result.Configuration.Smtp.AuthenticationMode);
	}

	[TestMethod]
	public void Parse_LegacyInlinePassword_MapsStartTlsAndWarnsWithoutDisclosingSecret()
	{
		const string password = "not-for-logs";
		var result = EmailConfigurationParser.Parse(XElement.Parse($@"<Definition>
  <Host>smtp.example.test</Host><Port>587</Port><EnableSSL>false</EnableSSL>
  <UseDefaultCredentials>false</UseDefaultCredentials><Credentials Username=""game@example.test"" Password=""{password}"" />
</Definition>"), new TestSecretResolver());

		Assert.IsTrue(result.Success, result.Error);
		Assert.AreEqual(SmtpTlsMode.StartTls, result.Configuration!.Smtp!.TlsMode);
		Assert.AreEqual(SmtpAuthenticationMode.Password, result.Configuration.Smtp.AuthenticationMode);
		Assert.IsTrue(result.Warnings.Any(x => x.Contains("inline SMTP password", StringComparison.Ordinal)));
		Assert.IsFalse(result.Warnings.Any(x => x.Contains(password, StringComparison.Ordinal)));
	}

	[TestMethod]
	public void EnvironmentSecretResolver_RejectsInlineSecretReferences()
	{
		var resolver = new EnvironmentEmailSecretResolver();

		Assert.IsFalse(resolver.TryResolve("plaintext-password", out _, out var error));
		StringAssert.Contains(error, "env:VARIABLE_NAME");
	}

	[TestMethod]
	public void GoogleAuthorizationModes_SelectOnlyExpectedScopes()
	{
		Assert.IsTrue(GoogleEmailAuthorization.TryGetScope("gmail-api", out var gmailApiScope));
		Assert.AreEqual("https://www.googleapis.com/auth/gmail.send", gmailApiScope);
		Assert.IsTrue(GoogleEmailAuthorization.TryGetScope("smtp", out var smtpScope));
		Assert.AreEqual("https://mail.google.com/", smtpScope);
		Assert.IsFalse(GoogleEmailAuthorization.TryGetScope("invalid", out _));
	}

	private sealed class TestSecretResolver : IEmailSecretResolver
	{
		public bool TryResolve(string reference, out string secret, out string error)
		{
			secret = $"resolved-{reference[4..]}";
			error = string.Empty;
			return reference.StartsWith("env:", StringComparison.OrdinalIgnoreCase);
		}
	}
}
