#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Character;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PhysicalDamageAbsorptionTests
{
	[TestMethod]
	public void Effect_IsSavedAndRestoredWithConfiguredAbsorption()
	{
		PhysicalDamageAbsorptionEffect.InitialiseEffectType();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(Mock.Of<IUneditableAll<IFutureProg>>());
		var owner = new Mock<IPerceivable>();
		owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		var original = new PhysicalDamageAbsorptionEffect(owner.Object, 4.5);
		var saved = original.SaveToXml(new Dictionary<IEffect, System.TimeSpan>());

		Assert.IsTrue(original.SavingEffect);
		var loaded = (PhysicalDamageAbsorptionEffect)Effect.LoadEffect(saved, owner.Object);
		Assert.AreEqual(4.5, loaded.PhysicalAbsorption);
		Assert.IsTrue(XNode.DeepEquals(saved, loaded.SaveToXml(new Dictionary<IEffect, System.TimeSpan>())));
	}

	[TestMethod]
	public void AddPhysicalAbsorptionEffect_UpdatesOneEffectAndZeroRemovesIt()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var registration = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == "addphysicalabsorptioneffect");
		var effects = new List<IEffect>();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Effects).Returns(() => effects.ToArray());
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
			.Callback<IEffect>(effect => effects.Add(effect));
		character.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, _) => effects.Remove(effect));

		var first = Execute(registration, character.Object, 2m);
		Assert.AreEqual(2.0, ((PhysicalDamageAbsorptionEffect)first!).PhysicalAbsorption);
		var replaced = Execute(registration, character.Object, 5m);
		Assert.AreEqual(5.0, ((PhysicalDamageAbsorptionEffect)replaced!).PhysicalAbsorption);
		Assert.AreEqual(1, effects.Count);
		Assert.AreSame(replaced, effects.Single());
		effects.Add(new PhysicalDamageAbsorptionEffect(character.Object, 5));
		effects.Add(new PhysicalDamageAbsorptionEffect(character.Object, 7));
		var canonical = Execute(registration, character.Object, 5m);
		Assert.AreSame(replaced, canonical);
		Assert.AreEqual(1, effects.Count);

		Assert.IsNull(Execute(registration, character.Object, 0m));
		Assert.AreEqual(0, effects.Count);
	}

	[TestMethod]
	public void EquipmentAndScriptAbsorption_ApplySequentiallyOnlyToPhysicalDamageAndStun()
	{
		var physical = new Damage { DamageType = DamageType.Crushing, DamageAmount = 10, StunAmount = 6, PainAmount = 4, ShockAmount = 3 };
		var equipment = EquipmentDamageAbsorption.Apply(physical, 2);
		var script = EquipmentDamageAbsorption.Apply(equipment, 3);
		Assert.AreEqual(5.0, script.DamageAmount);
		Assert.AreEqual(1.0, script.StunAmount);
		Assert.AreEqual(4.0, script.PainAmount);
		Assert.AreEqual(3.0, script.ShockAmount);

		var nonphysical = new Damage(physical) { DamageType = DamageType.Burning };
		Assert.AreSame(nonphysical, EquipmentDamageAbsorption.Apply(nonphysical, 2));
		Assert.AreSame(nonphysical, EquipmentDamageAbsorption.Apply(nonphysical, 3));
	}

	private static object? Execute(FunctionCompilerInformation registration, ICharacter character, decimal amount)
	{
		var values = new object[] { character, amount };
		var parameters = values.Select((value, index) =>
		{
			var variable = new Mock<IProgVariable>();
			variable.SetupGet(x => x.GetObject).Returns(value);
			var function = new Mock<IFunction>();
			function.SetupGet(x => x.ReturnType).Returns(registration.Parameters.ElementAt(index));
			function.SetupGet(x => x.Result).Returns(variable.Object);
			function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
			return function.Object;
		}).ToArray();
		var functionUnderTest = registration.CompilerFunction(parameters, Mock.Of<IFuturemud>());
		Assert.AreEqual(StatementResult.Normal, functionUnderTest.Execute(Mock.Of<IVariableSpace>()));
		return functionUnderTest.Result?.GetObject;
	}
}
