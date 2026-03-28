#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ProgLookupFromBuilderInputTests
{
	[TestMethod]
	public void LookupProg_NonAdmin_CannotSelectNonPublicProg()
	{
		var prog = CreateProg(publicProg: false);
		var actor = CreateActor(isAdministrator: false, prog.Object);

		var result = InvokeLookupProg(actor.Object, "eligibility");

		Assert.IsNull(result);
	}

	[TestMethod]
	public void LookupProg_Admin_CanSelectNonPublicProg()
	{
		var prog = CreateProg(publicProg: false);
		var actor = CreateActor(isAdministrator: true, prog.Object);

		var result = InvokeLookupProg(actor.Object, "eligibility");

		Assert.AreSame(prog.Object, result);
	}

	private static Mock<IFutureProg> CreateProg(bool publicProg)
	{
		var requiredParameters = new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Shop,
			ProgVariableTypes.Merchandise,
			ProgVariableTypes.Number,
			ProgVariableTypes.MudDateTime
		};
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Public).Returns(publicProg);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.Parameters).Returns(requiredParameters);
		prog.SetupGet(x => x.NamedParameters)
			.Returns(requiredParameters.Select(x => Tuple.Create(x, string.Empty)).ToList());
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
			.Returns<IEnumerable<ProgVariableTypes>>(parameters => parameters.SequenceEqual(requiredParameters));
		return prog;
	}

	private static Mock<ICharacter> CreateActor(bool isAdministrator, IFutureProg prog)
	{
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);

		var progCollection = new Mock<IUneditableAll<IFutureProg>>();
		progCollection.Setup(x => x.GetByIdOrName("eligibility", It.IsAny<bool>())).Returns(prog);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progCollection.Object);

		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(isAdministrator);
		return actor;
	}

	private static IFutureProg? InvokeLookupProg(ICharacter actor, string builderInput)
	{
		var requiredParameters = new[]
		{
			ProgVariableTypes.Character,
			ProgVariableTypes.Shop,
			ProgVariableTypes.Merchandise,
			ProgVariableTypes.Number,
			ProgVariableTypes.MudDateTime
		};
		var type = typeof(MudSharp.Economy.Shops.Shop).Assembly.GetType("MudSharp.FutureProg.ProgLookupFromBuilderInput", throwOnError: true)!;
		var constructor = type.GetConstructor(
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
			null,
			new[] { typeof(ICharacter), typeof(string), typeof(ProgVariableTypes), typeof(IEnumerable<ProgVariableTypes>) },
			null)!;
		var instance = constructor.Invoke(new object[] { actor, builderInput, ProgVariableTypes.Boolean, requiredParameters });
		var method = type.GetMethod("LookupProg", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
		return (IFutureProg?)method.Invoke(instance, null);
	}
}
