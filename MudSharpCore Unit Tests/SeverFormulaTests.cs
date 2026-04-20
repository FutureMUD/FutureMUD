#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Character.Heritage;
using MudSharp.Health;
using System.Reflection;
using System.Runtime.Serialization;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeverFormulaTests
{
	private static Body BuildBody(Mock<IRace> race)
	{
		Body body = (Body)FormatterServices.GetUninitializedObject(typeof(Body));
		typeof(Body).GetProperty("Race", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(body, race.Object);
		return body;
	}

	private static Mock<IDamage> BuildDamage(IBodypart bodypart, double damageAmount, DamageType damageType)
	{
		Mock<IDamage> damage = new();
		damage.SetupGet(x => x.Bodypart).Returns(bodypart);
		damage.SetupGet(x => x.DamageAmount).Returns(damageAmount);
		damage.SetupGet(x => x.DamageType).Returns(damageType);
		return damage;
	}

	private static bool InvokeShouldSever(Body body, IDamage damage)
	{
		MethodInfo method = typeof(Body).GetMethod("ShouldSever", BindingFlags.Instance | BindingFlags.NonPublic)!;
		return (bool)method.Invoke(body, [damage])!;
	}

	[TestMethod]
	public void ShouldSever_FormulaPresent_TakesPrecedenceOverLegacyThreshold()
	{
		Mock<IBodypart> bodypart = new();
		bodypart.SetupGet(x => x.CanSever).Returns(true);
		bodypart.SetupGet(x => x.SeverFormula).Returns("0");

		Mock<IRace> race = new();
		race.Setup(x => x.ModifiedSeverthreshold(bodypart.Object)).Returns(1.0);

		Body body = BuildBody(race);
		Mock<IDamage> damage = BuildDamage(bodypart.Object, 25.0, DamageType.Chopping);

		Assert.IsFalse(InvokeShouldSever(body, damage.Object),
			"A sever formula should override the legacy threshold path when it is present.");
	}

	[TestMethod]
	public void ShouldSever_NoFormula_UsesLegacyThresholdAndSeverableDamageType()
	{
		Mock<IBodypart> bodypart = new();
		bodypart.SetupGet(x => x.CanSever).Returns(true);
		bodypart.SetupGet(x => x.SeverFormula).Returns((string?)null);

		Mock<IRace> race = new();
		race.Setup(x => x.ModifiedSeverthreshold(bodypart.Object)).Returns(10.0);

		Body body = BuildBody(race);
		Mock<IDamage> damage = BuildDamage(bodypart.Object, 12.0, DamageType.Chopping);

		Assert.IsTrue(InvokeShouldSever(body, damage.Object));
	}

	[TestMethod]
	public void ShouldSever_FormulaOnlyBodypart_CanUseDamageTypeParameter()
	{
		Mock<IBodypart> bodypart = new();
		bodypart.SetupGet(x => x.CanSever).Returns(true);
		bodypart.SetupGet(x => x.SeverFormula).Returns("if(damagetype < 3, 1, 0)");

		Mock<IRace> race = new();
		race.Setup(x => x.ModifiedSeverthreshold(bodypart.Object)).Returns(999.0);

		Body body = BuildBody(race);
		Mock<IDamage> damage = BuildDamage(bodypart.Object, 2.0, DamageType.Crushing);

		Assert.IsTrue(InvokeShouldSever(body, damage.Object),
			"The sever formula path should work even for damage types that the legacy CanSever rules would reject.");
	}
}
