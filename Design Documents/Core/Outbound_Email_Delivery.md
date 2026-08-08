# Outbound Email Delivery

FutureMUD composes every outbound message once as a `MimeMessage` and sends it through one global transport. The selected transport is either generic SMTP or the Gmail API. There is no automatic provider detection, fallback, downgrade, or per-player email account configuration.

The email server configuration is the `EmailServer` static configuration. The seeded Version 2 configuration is deliberately disabled. Set `<Enabled>true</Enabled>` only after the transport, DNS, account, and secrets have been configured.

## Security model

All Version 2 secrets are references, never values. A secret element must use `env:VARIABLE_NAME`; FutureMUD resolves it from the process environment when email configuration is loaded. Do not put passwords, OAuth client secrets, refresh tokens, recipients, or message content in static configuration, screenshots, tickets, or source control.

SMTP TLS is explicit:

- `StartTls` requires the server to upgrade to TLS. It does not silently continue in plaintext.
- `SslOnConnect` uses TLS from connection establishment.
- `None` is only allowed when `AllowUnencryptedTransport` is also exactly `true`. Use it only for a local or otherwise trusted relay on a protected network.

Normal TLS certificate validation is retained. Do not add a permissive certificate-validation callback to make a connection succeed.

Unauthenticated SMTP is supported over TLS for trusted relays. Password SMTP is still useful for providers that allow it, but it is not the preferred integration for Google or Microsoft hosted accounts. OAuth and API credentials should be stored as protected environment variables by the operating-system service manager or hosting platform.

## SMTP configuration

This is password SMTP over required STARTTLS:

```xml
<Definition>
  <Version>2</Version>
  <Enabled>true</Enabled>
  <Transport>Smtp</Transport>
  <Smtp>
    <Host>smtp.example.net</Host>
    <Port>587</Port>
    <TlsMode>StartTls</TlsMode>
    <AuthenticationMode>Password</AuthenticationMode>
    <Username>game@example.net</Username>
    <PasswordReference>env:FUTUREMUD_SMTP_PASSWORD</PasswordReference>
  </Smtp>
  <FailureHandling>
    <MaxAttempts>6</MaxAttempts>
    <StoreFailedMessages>false</StoreFailedMessages>
    <Directory>FailedEmails</Directory>
  </FailureHandling>
</Definition>
```

For a trusted relay which requires no authentication, retain TLS where possible:

```xml
<Smtp>
  <Host>relay.internal.example.net</Host>
  <Port>587</Port>
  <TlsMode>StartTls</TlsMode>
  <AuthenticationMode>None</AuthenticationMode>
</Smtp>
```

Genuinely plaintext relays require both declarations below. This is an intentional operational exception, not a compatibility fallback:

```xml
<Smtp>
  <Host>127.0.0.1</Host>
  <Port>25</Port>
  <TlsMode>None</TlsMode>
  <AllowUnencryptedTransport>true</AllowUnencryptedTransport>
  <AuthenticationMode>None</AuthenticationMode>
</Smtp>
```

### Google SMTP OAuth

Google SMTP OAuth uses the broad `https://mail.google.com/` scope and explicit XOAUTH2 authentication. Gmail API is normally the better Google option because it requests only `gmail.send`.

```xml
<Definition>
  <Version>2</Version>
  <Enabled>true</Enabled>
  <Transport>Smtp</Transport>
  <Smtp>
    <Host>smtp.gmail.com</Host>
    <Port>587</Port>
    <TlsMode>StartTls</TlsMode>
    <AuthenticationMode>GoogleOAuth2</AuthenticationMode>
    <Username>game@example.com</Username>
    <GoogleOAuth>
      <ClientId>desktop-client-id.apps.googleusercontent.com</ClientId>
      <ClientSecretReference>env:FUTUREMUD_GOOGLE_CLIENT_SECRET</ClientSecretReference>
      <RefreshTokenReference>env:FUTUREMUD_GOOGLE_REFRESH_TOKEN</RefreshTokenReference>
    </GoogleOAuth>
  </Smtp>
</Definition>
```

## Gmail API configuration

Gmail API serializes the existing MIME message as base64url data and calls `users.messages.send` as `me`. It uses only `https://www.googleapis.com/auth/gmail.send`.

```xml
<Definition>
  <Version>2</Version>
  <Enabled>true</Enabled>
  <Transport>GmailApi</Transport>
  <GmailApi>
    <AccountAddress>game@example.com</AccountAddress>
    <ClientId>desktop-client-id.apps.googleusercontent.com</ClientId>
    <ClientSecretReference>env:FUTUREMUD_GOOGLE_CLIENT_SECRET</ClientSecretReference>
    <RefreshTokenReference>env:FUTUREMUD_GOOGLE_REFRESH_TOKEN</RefreshTokenReference>
  </GmailApi>
  <FailureHandling>
    <MaxAttempts>6</MaxAttempts>
    <StoreFailedMessages>false</StoreFailedMessages>
    <Directory>FailedEmails</Directory>
  </FailureHandling>
</Definition>
```

The client secret and refresh token are not displayed by FutureMUD configuration listings because the XML holds only their environment-variable names. Restart the host process or reload email configuration after changing an environment variable.

## Google authorization

Create a Google Cloud project, enable the Gmail API, configure the OAuth consent screen, and create a **Desktop** OAuth client. Add the intended test users while the consent screen is in testing. Before a production deployment, complete Google’s required consent-screen and verification/publishing process for the selected scope. Google classifies `gmail.send` as sensitive; the SMTP `mail.google.com` scope is restricted and is substantially broader.

Run one of these commands on an interactive machine with a browser, before starting the server normally:

```text
MudSharp --authorize-google-email gmail-api <client-secrets.json>
MudSharp --authorize-google-email smtp <client-secrets.json>
```

The command starts a loopback browser authorization flow with PKCE, offline authorization, and forced consent. It holds token data only in memory and prints the resulting refresh token once. Copy that value directly into the protected environment variable named by `RefreshTokenReference`; do not redirect the command output to ordinary logs. The command never prints the OAuth client secret and never writes either secret to the database or a token file.

For a Windows service, define secrets in the service account’s protected environment and restrict access to the service configuration. For Linux service managers, use the service manager’s environment/credential facility with a root-readable unit or credentials file rather than a world-readable shell profile.

## Migration from the previous configuration

Version 1 XML remains readable for this release:

- `EnableSSL=true` becomes `SslOnConnect`.
- `EnableSSL=false` becomes required `StartTls`; it no longer permits a plaintext downgrade.
- `UseDefaultCredentials=true` becomes unauthenticated relay mode.
- `UseDefaultCredentials=false` becomes password authentication.

An old inline `Credentials Password` continues to work temporarily but produces a prominent migration warning without disclosing the password. Replace it with `env:FUTUREMUD_SMTP_PASSWORD` and the Version 2 password configuration as soon as possible. Operators who really require plaintext SMTP must migrate explicitly to `TlsMode=None` and `AllowUnencryptedTransport=true`.

Invalid reloads are rejected before activation: the previous working transport and queued messages remain in place. At a fresh start, invalid configuration leaves outbound delivery disabled and emits only a sanitized diagnostic.

## Delivery lifecycle and privacy

The worker is one cancellable asynchronous scheduled queue. Starting and stopping it are idempotent, and network work does not hold the queue lock. It attempts delivery up to six times total. Retryable failures use delays of 10 seconds, 30 seconds, 2 minutes, 5 minutes, and 15 minutes.

Retryable errors are network/time-out failures, SMTP 4xx responses, and Gmail HTTP 408, 429, and 5xx responses. Authentication/configuration problems, SMTP 5xx responses, and other Gmail 4xx responses are permanent failures.

Each message has a correlation ID and template type. Diagnostics contain only the correlation ID, template type, attempt count, transport, safe status category, and a bounded error classification. FutureMUD never logs recipients, subjects, message bodies, credentials, OAuth tokens, or raw provider errors.

By default, exhausted messages are logged as metadata only. Set `<StoreFailedMessages>true</StoreFailedMessages>` only when an operator needs content for a delivery investigation. A stored message uses a timestamp-and-correlation-ID `.eml` filename below the configured relative `Directory`; subjects never influence the filename. On Unix, FutureMUD applies user-read/user-write mode (`0600`). On Windows, the administrator must restrict the directory ACL to the account running FutureMUD, for example by removing inherited access and granting that account full control. Treat dead-letter `.eml` files as sensitive player data and remove them through the normal retention process.

Malformed recipient addresses or template formatting are isolated inside the email subsystem and are recorded only as safe metadata, so account/recovery workflows continue instead of failing the game operation.

Debug builds do not enqueue or send live outbound email. Transport adapters remain independently testable without enabling live delivery from a development world.

## Provider choices and future work

Use Gmail API for Google where possible; it offers the narrowest permission required for sending. Generic SMTP remains appropriate for private servers, Yahoo, iCloud, transactional providers, and trusted relays. FutureMUD does not infer a provider from an email domain and does not fall back between the Gmail API and SMTP.

Microsoft Graph/Entra is the intended next OAuth/API transport extension. The transport and token interfaces are deliberately separated so it can share MIME composition without changing callers. Microsoft password SMTP remains generically usable only where the tenant/provider enables it; it is not the forward-looking Microsoft integration.

## References

- [Google Gmail API scopes](https://developers.google.com/workspace/gmail/api/auth/scopes)
- [Google Gmail API message sending](https://developers.google.com/workspace/gmail/api/guides/sending)
- [Google XOAUTH2 for SMTP](https://developers.google.com/workspace/gmail/imap/xoauth2-protocol)
- [MailKit TLS socket options](https://mimekit.net/docs/html/T_MailKit_Security_SecureSocketOptions.htm)
- [Microsoft SMTP AUTH timeline](https://techcommunity.microsoft.com/blog/exchange/updated-exchange-online-smtp-auth-basic-authentication-deprecation-timeline/4489835)
