#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.Effects;
using MudSharp.Testing.EnvironmentalMagic;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class GatheringAuthoringSupportTests
{
	[TestMethod]
	public void OfflineCompiler_UsesDeclaredRegisterTypesWithoutLoadingDatabaseOrLiveValues()
	{
		var definition = new Db.FutureProg { Id = 1, FunctionName = "typed_register_read",
			ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(),
			FunctionText = "return getregister(@actor, \"armageddon_ecology\")" };
		definition.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "actor",
			ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		var registers = new[] { new Db.VariableDefinition { OwnerTypeDefinition = ProgVariableTypes.Character.ToStorageString(),
			Property = "armageddon_ecology", ContainedTypeDefinition = ProgVariableTypes.Number.ToStorageString() } };
		using var compiler = new OfflineProgCompilation([definition], registers);
		Assert.AreEqual(ProgVariableTypes.Number, compiler.Compile(1).ReturnType);
		definition.FunctionText = "return getregister(@actor, \"undeclared_register\")";
		using var missing = new OfflineProgCompilation([definition], registers);
		Assert.ThrowsException<InvalidOperationException>(() => missing.Compile(1));
	}

	[TestMethod]
	public void DescriptionAddition_RepeatsOnceAndRetainsNativeColdStorage()
	{
		var fixture = DescriptionFixture();
		var function = Function(fixture.Owner.Object, "Grey, lifeless ash collects on the ground.", fixture.World.Object);
		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		var effect = fixture.Effects.OfType<DescriptionAddition>().Single();
		Assert.IsFalse(effect.PlayerSet);
		Assert.IsTrue(effect.SavingEffect);
		var saved = effect.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = new DescriptionAddition(saved, fixture.Owner.Object);
		Assert.AreEqual(effect.AdditionalText, loaded.AdditionalText);
		Assert.AreEqual(71L, loaded.ApplicabilityProg.Id);
		fixture.Owner.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Once);
	}

	[TestMethod]
	public void DescriptionAddition_PreservesDifferentRetainedAuthoringAndRefusesInvalidPolicy()
	{
		var fixture = DescriptionFixture();
		var builder = new DescriptionAddition(fixture.Owner.Object, "Retained builder text.", false, null!, fixture.Prog.Object);
		fixture.Effects.Add(builder);
		Assert.AreEqual(StatementResult.Error, Function(fixture.Owner.Object, "New text.", fixture.World.Object).Execute(new VariableSpace()));
		Assert.AreSame(builder, fixture.Effects.Single());
		Assert.AreEqual("Retained builder text.", builder.AdditionalText);
		fixture.Effects.Clear();
		fixture.Prog.SetupGet(x => x.AcceptsAnyParameters).Returns(true);
		Assert.AreEqual(StatementResult.Error, Function(fixture.Owner.Object, "New text.", fixture.World.Object).Execute(new VariableSpace()));
		Assert.AreEqual(0, fixture.Effects.Count);
	}

	[TestMethod]
	public void DescriptionAddition_CompilesExactPublicContract()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "description_contract", ProgVariableTypes.Boolean,
			new[] { Tuple.Create(ProgVariableTypes.Location, "room") },
			"return adddescriptionaddition(@room, \"Ash covers the ground.\", 71)");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
	}

	private static AddDescriptionAdditionFunction Function(IPerceivable owner, string text, IFuturemud world) =>
		new([Constant(owner, ProgVariableTypes.Location), Constant(text, ProgVariableTypes.Text),
			Constant(71.0M, ProgVariableTypes.Number)], world);

	private static (Mock<IPerceivable> Owner, Mock<IFuturemud> World, Mock<IFutureProg> Prog, List<IEffect> Effects) DescriptionFixture()
	{
		var world = new Mock<IFuturemud>();
		var owner = new Mock<IPerceivable>();
		var prog = new Mock<IFutureProg>();
		var effects = new List<IEffect>();
		prog.SetupGet(x => x.Id).Returns(71L);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.StaticType).Returns(FutureProgStaticType.NotStatic);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		var progs = new EnvironmentalMagicTestRegistry<IFutureProg>();
		progs.Add(prog.Object);
		world.SetupGet(x => x.FutureProgs).Returns(progs);
		owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		owner.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		owner.SetupGet(x => x.Effects).Returns(effects);
		owner.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		return (owner, world, prog, effects);
	}

	private static IFunction Constant(object value, ProgVariableTypes type)
	{
		var variable = new Mock<IProgVariable>();
		variable.SetupGet(x => x.GetObject).Returns(value);
		variable.SetupGet(x => x.Type).Returns(type);
		var function = new Mock<IFunction>();
		function.SetupGet(x => x.ReturnType).Returns(type);
		function.SetupGet(x => x.Result).Returns(variable.Object);
		function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		return function.Object;
	}
}
