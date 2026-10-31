#nullable enable
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.Characters;
using MudSharp.FutureProg.Variables;
using MudSharp.NPC;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcRelationshipFunctionTests
{
	[TestMethod]
	public void SetNpcAi_UsesInstanceMutation_IdempotentlyAndRejectsUnknownIds()
	{
		var world = new Mock<IFuturemud>(); var npc = new Mock<INPC>(); var ai = new Mock<IArtificialIntelligence>();
		ai.SetupGet(x => x.Id).Returns(77);
		var definitions = new All<IArtificialIntelligence>(); definitions.Add(ai.Object);
		world.SetupGet(x => x.AIs).Returns(definitions);
		var attached = new List<IArtificialIntelligence>();
		npc.SetupGet(x => x.AIs).Returns(attached); npc.SetupGet(x => x.GetObject).Returns(npc.Object);
		npc.Setup(x => x.AddAI(ai.Object)).Callback(() => attached.Add(ai.Object));
		npc.Setup(x => x.RemoveAI(ai.Object)).Callback(() => attached.Remove(ai.Object));
		bool Run(decimal id, bool enabled)
		{
			var function = new NpcRelationshipFunctions([new ConstantFunction(npc.Object), new ConstantFunction(new NumberVariable(id)),
				new ConstantFunction(new BooleanVariable(enabled))], world.Object, "setnpcai");
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			return (bool)function.Result.GetObject;
		}
		Assert.IsTrue(Run(77, true)); Assert.IsTrue(Run(77, true));
		npc.Verify(x => x.AddAI(ai.Object), Times.Once);
		Assert.IsFalse(Run(78, true)); Assert.IsFalse(Run(77.5M, true)); Assert.IsFalse(Run(decimal.MaxValue, true));
		Assert.IsTrue(Run(77, false)); Assert.IsTrue(Run(77, false));
		npc.Verify(x => x.RemoveAI(ai.Object), Times.Once);
	}
}
