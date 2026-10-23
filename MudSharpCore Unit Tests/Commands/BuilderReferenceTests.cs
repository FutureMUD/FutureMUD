#nullable enable
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Commands.Helpers;
using MudSharp.Economy;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class BuilderReferenceTests
{
	[TestMethod]
	public void BankTypeReferences_UsePersistentIdentityAndExcludeOtherTypes()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var target = Mock.Of<IBankAccountType>(x => x.Id == 7 && x.Name == "Savings");
		var accounts = new All<IBankAccount>();
		accounts.Add(Mock.Of<IBankAccount>(x => x.Id == 1 && x.Name == "Customer" && x.BankAccountType == Mock.Of<IBankAccountType>(y => y.Id == 7)));
		accounts.Add(Mock.Of<IBankAccount>(x => x.Id == 2 && x.Name == "Other" && x.BankAccountType == Mock.Of<IBankAccountType>(y => y.Id == 8)));
		world.SetupGet(x => x.BankAccounts).Returns(accounts);
		var references = BuilderReferenceReport.Find(world.Object, target);
		Assert.AreEqual(1, references.Count);
		StringAssert.Contains(references.Single().Source, "#1");
		Assert.AreEqual("Account type", references.Single().Field);
	}

	[TestMethod]
	public void ExpressionReferences_DistinguishEachConsumerField()
	{
		var world = BuilderWorkflowRegressionTests.World();
		var target = Mock.Of<ITraitExpression>(x => x.Id == 7);
		var profile = Mock.Of<IDamageProfile>(x => x.DamageExpression == target && x.PainExpression == target && x.StunExpression == Mock.Of<ITraitExpression>(y => y.Id == 8));
		var attacks = new All<IWeaponAttack>(); attacks.Add(Mock.Of<IWeaponAttack>(x => x.Id == 1 && x.Name == "Slash" && x.Profile == profile));
		world.SetupGet(x => x.WeaponAttacks).Returns(attacks);
		world.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition>());
		CollectionAssert.AreEquivalent(new[] { "Damage expression", "Pain expression" }, BuilderReferenceReport.Find(world.Object, target).Select(x => x.Field).ToArray());
	}
}
