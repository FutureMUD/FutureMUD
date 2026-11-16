#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Email;

namespace MudSharp_Unit_Tests.Email;

[TestClass]
public class PasswordChangeEmailTests
{
	[TestMethod]
	[DataRow("<p>Hello {0}; changed from {1}; password: {2}</p>")]
	[DataRow("<p>Hello {0}; changed from {1}</p>")]
	public void ComposePasswordChange_OldAndNewTemplates_DoNotRequireAPassword(string content)
	{
		var template = Template(content);
		using var message = EmailHelper.ComposeMessage("Our MUD", EmailTemplateTypes.AccountPasswordChanged, template,
			"player@example.test", "Player", "192.0.2.10");

		StringAssert.Contains(message.HtmlBody, "Player");
		StringAssert.Contains(message.HtmlBody, "192.0.2.10");
		Assert.AreEqual("Custom password notification", message.Subject);
		Assert.AreEqual("custom@example.test", message.From.Mailboxes.Single().Address);
		Assert.AreEqual("Our MUD", message.From.Mailboxes.Single().Name);
		Assert.AreEqual(content, template.Content);
	}

	[TestMethod]
	public void ComposePasswordChange_LegacyCallerSuppliesPassword_CannotPutItInTheMessage()
	{
		var template = Template("<p>Our custom brand: {0}, {1}, {2}. Literal {{2}}.</p>");
		const string replacementPassword = "synthetic-private-password";
		using var message = EmailHelper.ComposeMessage("Our MUD", EmailTemplateTypes.AccountPasswordChanged, template,
			"player@example.test", "Player", "192.0.2.10", replacementPassword);

		StringAssert.Contains(message.HtmlBody, "Our custom brand");
		StringAssert.Contains(message.HtmlBody, "not included in email");
		StringAssert.Contains(message.HtmlBody, "Literal {2}.");
		Assert.IsFalse(message.ToString().Contains(replacementPassword, StringComparison.Ordinal));
		Assert.AreEqual("<p>Our custom brand: {0}, {1}, {2}. Literal {{2}}.</p>", template.Content);
	}

	[TestMethod]
	public void ComposeStaffPasswordReset_StillIncludesItsGeneratedPassword()
	{
		using var message = EmailHelper.ComposeMessage("Our MUD", EmailTemplateTypes.AccountPasswordReset,
			Template("<p>{0}, {1} reset your password to {2}.</p>"), "player@example.test",
			"Player", "Staff", "synthetic-temporary-password");

		StringAssert.Contains(message.HtmlBody, "synthetic-temporary-password");
	}

	[TestMethod]
	public void ComposePasswordChange_MissingIdentityOrUnsupportedPlaceholder_RejectsInvalidMessage()
	{
		Assert.ThrowsExactly<ArgumentException>(() => EmailHelper.ComposeMessage("Our MUD",
			EmailTemplateTypes.AccountPasswordChanged, Template("{0} {1}"), "player@example.test", "Player"));
		Assert.ThrowsExactly<FormatException>(() => EmailHelper.ComposeMessage("Our MUD",
			EmailTemplateTypes.AccountPasswordChanged, Template("{3}"), "player@example.test", "Player", "192.0.2.10"));
	}

	private static EmailTemplate Template(string content) => new(new MudSharp.Models.EmailTemplate
	{
		TemplateType = (int)EmailTemplateTypes.AccountPasswordChanged,
		Content = content,
		Subject = "Custom password notification",
		ReturnAddress = "custom@example.test"
	});
}
