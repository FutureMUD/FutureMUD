#nullable enable

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.Manipulation;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.NPC;
using MudSharp.Commands.Modules;
using MudSharp.PerceptionEngine;
using NativeCharacter = MudSharp.Character.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcBehaviourProgTests
{
	[TestMethod]
	public void TargetItem_PreservesNativeKeywordDisambiguationAndExcludesInvisibleCandidates()
	{
		var viewer = new Mock<ICharacter>(); viewer.SetupGet(x => x.GetObject).Returns(viewer.Object);
		var hidden = Item(10); var first = Item(11); var second = Item(12);
		viewer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		viewer.Setup(x => x.CanSee(hidden.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		var items = new CollectionVariable(new List<IGameItem> { hidden.Object, first.Object, second.Object }, ProgVariableTypes.Item);
		IGameItem? Find(string query)
		{
			var function = new TargetItemFunction([new ConstantFunction(viewer.Object), new ConstantFunction(items), new ConstantFunction(new TextVariable(query))]);
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			return function.Result?.GetObject as IGameItem;
		}
		Assert.AreSame(first.Object, Find("torch"));
		Assert.AreSame(second.Object, Find("2.torch"));
		Assert.AreSame(second.Object, Find("#2"));
		Assert.IsNull(Find("3.torch"));
		Assert.IsNull(Find(""));
		Assert.IsNull(Find("sword"));
		Mock<IGameItem> Item(long id)
		{
			var item = new Mock<IGameItem>();
			item.SetupGet(x => x.Id).Returns(id); item.SetupGet(x => x.GetObject).Returns(item.Object);
			item.Setup(x => x.HasKeyword("torch", viewer.Object, true, false)).Returns(true);
			return item;
		}
	}

	[TestMethod]
	public void RelationshipProperties_ReadActualFollowStateAndGiveSafePlayerBodyguardDefaults()
	{
		var character = (NativeCharacter)RuntimeHelpers.GetUninitializedObject(typeof(NativeCharacter));
		var variable = (IProgVariable)character;
		Assert.IsNull(variable.GetProperty("following"));
		Assert.IsNull(variable.GetProperty("bodyguarding"));
		Assert.AreEqual(0m, variable.GetProperty("bodyguardingid").GetObject);
		var leader = new Mock<ICharacter>(); leader.SetupGet(x => x.GetObject).Returns(leader.Object);
		typeof(NativeCharacter).GetProperty("Following", BindingFlags.Instance | BindingFlags.Public)!.SetValue(character, leader.Object);
		Assert.AreSame(leader.Object, variable.GetProperty("following").GetObject);
	}

	[TestMethod]
	public void BodyguardQueriesAndClear_DoNotMaterialiseUnloadedCharacters()
	{
		var world = new Mock<IFuturemud>(); var loaded = new All<ICharacter>();
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Id).Returns(42); target.SetupGet(x => x.GetObject).Returns(target.Object);
		loaded.Add(target.Object); world.SetupGet(x => x.Actors).Returns(loaded);
		var npc = (MudSharp.NPC.NPC)RuntimeHelpers.GetUninitializedObject(typeof(MudSharp.NPC.NPC));
		typeof(NativeCharacter).GetProperty("Gameworld")!.SetValue(npc, world.Object);
		typeof(MudSharp.NPC.NPC).GetField("_bodyguardingCharacterId", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(npc, 42L);
		Assert.AreSame(target.Object, ((IProgVariable)npc).GetProperty("bodyguarding").GetObject);
		world.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
		Assert.IsNull(((IProgVariable)npc).GetProperty("bodyguarding"));
		Assert.AreEqual(42m, ((IProgVariable)npc).GetProperty("bodyguardingid").GetObject);
		var actor = new Mock<INPC>(); actor.SetupProperty(x => x.BodyguardingCharacterID, 42L);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object); actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		var command = typeof(NPCOnlyModule).GetMethod("Bodyguard", BindingFlags.Static | BindingFlags.NonPublic)!;
		command.Invoke(null, [actor.Object, "bodyguard"]);
		command.Invoke(null, [actor.Object, "bodyguard off"]);
		Assert.IsNull(actor.Object.BodyguardingCharacterID);
		world.Verify(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
	}
}
