#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using RuntimeAccount = MudSharp.Accounts.Account;
using DbAccount = MudSharp.Models.Account;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AccountRegistrationConfigurationTests
{
	[TestMethod]
	public void StaticDefaults_DisableAccountRegistrationByEmailDefaultsToFalse()
	{
		Assert.IsTrue(
			DefaultStaticSettings.DefaultStaticConfigurations.ContainsKey(RuntimeAccount.DisableAccountRegistrationByEmailStaticConfiguration));
		Assert.AreEqual(
			"false",
			DefaultStaticSettings.DefaultStaticConfigurations[RuntimeAccount.DisableAccountRegistrationByEmailStaticConfiguration]);
	}

	[TestMethod]
	public void IsRegistered_WhenEmailRegistrationDisabled_TreatsUnregisteredAccountAsRegistered()
	{
		var account = CreateAccount(isRegistered: false, disableAccountRegistrationByEmail: true);

		Assert.IsTrue(account.IsRegistered);
	}

	[TestMethod]
	public void IsRegistered_WhenEmailRegistrationEnabled_UsesStoredRegistrationFlag()
	{
		var account = CreateAccount(isRegistered: false, disableAccountRegistrationByEmail: false);

		Assert.IsFalse(account.IsRegistered);
	}

	[TestMethod]
	public void IsRegistered_WhenStoredRegistered_RemainsRegistered()
	{
		var account = CreateAccount(isRegistered: true, disableAccountRegistrationByEmail: false);

		Assert.IsTrue(account.IsRegistered);
	}

	private static RuntimeAccount CreateAccount(bool isRegistered, bool disableAccountRegistrationByEmail)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld
			.SetupGet(x => x.SaveManager)
			.Returns(new Mock<ISaveManager>().Object);
		gameworld
			.Setup(x => x.ChargenResources)
			.Returns(new All<IChargenResource>());
		gameworld
			.Setup(x => x.GetStaticBool(RuntimeAccount.DisableAccountRegistrationByEmailStaticConfiguration))
			.Returns(disableAccountRegistrationByEmail);

		var dbAccount = new DbAccount
		{
			Id = 1,
			Name = "tester",
			Email = "tester@example.com",
			IsRegistered = isRegistered,
			CultureName = "en-US",
			TimeZoneId = "UTC",
			UnitPreference = "Metric",
			RegistrationCode = "ABCDEF",
			CreationDate = System.DateTime.UtcNow,
			FormatLength = 80,
			InnerFormatLength = 80,
			PageLength = 50,
			ActLawfully = true,
			HintsEnabled = true,
			AutoReacquireTargets = true
		};

		return new RuntimeAccount(dbAccount, gameworld.Object);
	}
}
