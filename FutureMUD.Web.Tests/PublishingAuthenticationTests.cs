#nullable enable

using FutureMUD.Web.Configuration;
using FutureMUD.Web.Publishing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.Text;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class PublishingAuthenticationTests
{
	[TestMethod]
	public void AuthenticationAcceptsOnlyTheTokenMatchingTheConfiguredHash()
	{
		const string token = "a-long-random-publishing-token-with-adequate-entropy";
		var options = Options.Create(new FutureMudWebOptions
		{
			PublishingTokenSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)))
		});
		var authentication = new PublishingAuthentication(options);
		var accepted = new DefaultHttpContext();
		accepted.Request.Headers.Authorization = $"Bearer {token}";
		var rejected = new DefaultHttpContext();
		rejected.Request.Headers.Authorization = "Bearer wrong";
		var shortToken = new DefaultHttpContext();
		shortToken.Request.Headers.Authorization = "Bearer short";

		Assert.AreEqual(PublishingAuthenticationResult.Accepted, authentication.Authenticate(accepted.Request));
		Assert.AreEqual(PublishingAuthenticationResult.Rejected, authentication.Authenticate(rejected.Request));
		Assert.AreEqual(PublishingAuthenticationResult.Rejected, authentication.Authenticate(shortToken.Request));
	}
}
