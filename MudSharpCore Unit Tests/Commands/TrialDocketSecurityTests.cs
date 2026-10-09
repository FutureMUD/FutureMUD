#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Law;
using System;
using System.Globalization;
using System.Reflection;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class TrialDocketSecurityTests
{
	[DataTestMethod]
	[DataRow(false, true)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(true, false)]
	public void TrialDocket_DefendantScope_MatchesExistingJudgementAuthority(bool administrator, bool hasEnforcement)
	{
		var canReadRestricted = administrator && !hasEnforcement;
		var fixture = new Fixture(administrator, hasEnforcement, canReadRestricted);
		fixture.Run();
		if (hasEnforcement || administrator)
		{
			StringAssert.Contains(fixture.Output, "Citizen Defendant");
			StringAssert.Contains(fixture.Output, "PermittedCharge");
		}
		else
		{
			StringAssert.Contains(fixture.Output, "not authorised");
		}

		Assert.AreEqual(canReadRestricted, fixture.Output.Contains("Restricted Defendant", StringComparison.Ordinal));
		Assert.AreEqual(canReadRestricted, fixture.Output.Contains("SecretCharge", StringComparison.Ordinal));
		if (!canReadRestricted)
		{
			fixture.Restricted.VerifyGet(x => x.PersonalName, Times.Never);
			fixture.Authority.Verify(x => x.KnownCrimesForIndividual(fixture.Restricted.Object), Times.Never);
			fixture.Authority.Verify(x => x.ResolvedCrimesForIndividual(fixture.Restricted.Object), Times.Never);
		}
	}

	[TestMethod]
	public void TrialDocket_EnforcerWithoutConvictionAuthority_RejectsBeforeCaseAccess()
	{
		var fixture = new Fixture(false, true, false);
		fixture.Enforcement.SetupGet(x => x.CanConvict).Returns(false);
		fixture.Run();
		StringAssert.Contains(fixture.Output, "not authorised");
		fixture.Authority.VerifyGet(x => x.KnownCrimes, Times.Never);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ICharacter> Restricted { get; } = new();
		public Mock<ILegalAuthority> Authority { get; } = new();
		public Mock<IEnforcementAuthority> Enforcement { get; } = new();
		public string Output { get; private set; } = string.Empty;

		public Fixture(bool administrator, bool hasEnforcement, bool canReadRestricted)
		{
			var citizen = new Mock<ICharacter>();
			citizen.SetupGet(x => x.Id).Returns(5L);
			Restricted.SetupGet(x => x.Id).Returns(6L);
			Actor.SetupGet(x => x.Id).Returns(4L);
			var citizenClass = new Mock<ILegalClass>();
			var restrictedClass = new Mock<ILegalClass>();
			restrictedClass.SetupGet(x => x.Name).Returns("Restricted");
			var citizenCrime = new Mock<ICrime>();
			citizenCrime.SetupGet(x => x.Criminal).Returns(citizen.Object);
			citizenCrime.SetupGet(x => x.Name).Returns("PermittedCharge");
			citizenCrime.SetupGet(x => x.Id).Returns(101L);
			var restrictedCrime = new Mock<ICrime>();
			restrictedCrime.SetupGet(x => x.Criminal).Returns(Restricted.Object);
			restrictedCrime.SetupGet(x => x.Name).Returns("SecretCharge");
			restrictedCrime.SetupGet(x => x.Id).Returns(102L);
			Authority.SetupGet(x => x.Name).Returns("Test Jurisdiction");
			Authority.SetupGet(x => x.Id).Returns(1L);
			Authority.SetupGet(x => x.KnownCrimes).Returns([citizenCrime.Object, restrictedCrime.Object]);
			Authority.Setup(x => x.GetLegalClass(citizen.Object)).Returns(citizenClass.Object);
			Authority.Setup(x => x.GetLegalClass(Restricted.Object)).Returns(restrictedClass.Object);
			Enforcement.SetupGet(x => x.CanConvict).Returns(true);
			Enforcement.SetupGet(x => x.AccusableClasses).Returns([citizenClass.Object]);
			Authority.Setup(x => x.GetEnforcementAuthority(Actor.Object)).Returns(hasEnforcement ? Enforcement.Object : null!);
			Authority.Setup(x => x.KnownCrimesForIndividual(citizen.Object)).Returns([citizenCrime.Object]);
			Authority.Setup(x => x.KnownCrimesForIndividual(Restricted.Object)).Returns([restrictedCrime.Object]);
			SetUpDefendant(citizen, "Citizen Defendant");
			SetUpDefendant(Restricted, "Restricted Defendant");
			if (!canReadRestricted)
			{
				Restricted.SetupGet(x => x.PersonalName).Throws(new InvalidOperationException("Restricted name was read before authorization."));
			}

			var gameworld = new Mock<IFuturemud>();
			var authorities = new All<ILegalAuthority>();
			authorities.Add(Authority.Object);
			gameworld.SetupGet(x => x.LegalAuthorities).Returns(authorities);
			var account = new Mock<IAccount>();
			account.SetupGet(x => x.LineFormatLength).Returns(120);
			Actor.SetupGet(x => x.Account).Returns(account.Object);
			Actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			Actor.SetupGet(x => x.LineFormatLength).Returns(120);
			Actor.SetupGet(x => x.InnerLineFormatLength).Returns(116);
			Actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns((Type type) => CultureInfo.InvariantCulture.GetFormat(type));
			Actor.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(administrator);
			var output = new Mock<IOutputHandler>();
			output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<string, bool, bool>((text, _, _) => Output = text).Returns(true);
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		}

		private void SetUpDefendant(Mock<ICharacter> defendant, string fullName)
		{
			var name = new Mock<IPersonalName>();
			name.Setup(x => x.GetName(NameStyle.FullName)).Returns(fullName);
			defendant.SetupGet(x => x.PersonalName).Returns(name.Object);
			defendant.Setup(x => x.AffectedBy<AwaitingSentencing>(Authority.Object)).Returns(true);
			defendant.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
				It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(fullName);
		}

		public void Run() => typeof(CrimeModule).GetMethod("TrialDocket", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [Actor.Object, new StringStack(string.Empty)]);
	}
}
