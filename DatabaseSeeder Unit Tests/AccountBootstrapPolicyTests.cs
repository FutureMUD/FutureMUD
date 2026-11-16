#nullable enable
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseSeeder.UnitTests;

[TestClass]
public class AccountBootstrapPolicyTests
{
	[TestMethod]
	public void ImplementorBootstrap_CanOmitEmail_AndUsesTheAccountPasswordPolicy()
	{
		// Both validators are pure; constructing a model/database would add no useful coverage.
		var questions = new CoreDataSeeder().SeederQuestions.ToList();
		var email = questions.Single(x => x.Id == "email").Validator;
		var password = questions.Single(x => x.Id == "password").Validator;
		Assert.IsTrue(email("none", null!).Success);
		Assert.IsFalse(email("invalid", null!).Success);
		Assert.IsFalse(password("short", null!).Success);
		Assert.IsTrue(password("an uncommon installer passphrase", null!).Success);
	}
}
