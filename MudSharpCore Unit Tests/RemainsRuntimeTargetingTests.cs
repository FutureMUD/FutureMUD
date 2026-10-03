#nullable enable

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

public partial class RemainsRuntimeBoundaryTests
{
	private sealed class TargetCharacter : MudSharp.Character.Character
	{
		private TargetCharacter() : base(null!, null!, true) { }
		public static TargetCharacter Create(ICell cell, IBody body)
		{
			var actor = (TargetCharacter)RuntimeHelpers.GetUninitializedObject(typeof(TargetCharacter));
			actor.Location = cell; actor.Body = body; return actor;
		}
		public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => thing is not null;
	}

	[DataTestMethod]
	[DataRow(0, false, false)]
	[DataRow(0, true, true)]
	[DataRow(1, false, false)]
	[DataRow(1, true, true)]
	[DataRow(2, false, false)]
	[DataRow(2, true, true)]
	[DataRow(3, false, false)]
	[DataRow(3, true, true)]
	[DataRow(3, true, false)]
	[DataRow(3, false, true)]
	public void ActorTargeting_CorpseCandidate_RequiresResolvedFinalCurrentBody(int state, bool ignoreLayers, bool carried)
	{
		var f = new Fixture(); var body = new Mock<IBody>(); body.SetupGet(x => x.Id).Returns(1); f.Actor.SetupGet(x => x.Body).Returns(body.Object);
		f.Actor.SetupGet(x => x.PersonalName).Returns(Mock.Of<IPersonalName>());
		f.Actor.Setup(x => x.HasKeyword("owner", It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		if (state != 0) ResolveCorpse(f, state != 1);
		if (state == 2) f.Corpse.SetupGet(x => x.RepresentsFinalCharacterDeath).Returns(false);
		var observerBody = new Mock<IBody>(); observerBody.SetupGet(x => x.ExternalItems).Returns(carried ? [f.Item.Object] : Array.Empty<IGameItem>());
		f.Source.SetupGet(x => x.Perceivables).Returns(carried ? Array.Empty<IPerceivable>() : [f.Item.Object]);
		var observer = TargetCharacter.Create(f.Source.Object, observerBody.Object);
		Assert.AreEqual(state == 3 ? f.Actor.Object : null,
			observer.TargetActorOrCorpse("#1", ignoreLayers ? PerceiveIgnoreFlags.IgnoreLayers : PerceiveIgnoreFlags.None));
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ActorTargeting_IndependentlyVisibleLiveOwner_RemainsTargetableBesideUnresolvedCorpse(bool ignoreLayers)
	{
		var f = new Fixture(); f.Actor.SetupGet(x => x.PersonalName).Returns(Mock.Of<IPersonalName>());
		f.Actor.Setup(x => x.HasKeyword("owner", It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		f.Source.SetupGet(x => x.Perceivables).Returns([f.Item.Object, f.Actor.Object]); var body = new Mock<IBody>();
		var observer = TargetCharacter.Create(f.Source.Object, body.Object);
		Assert.AreSame(f.Actor.Object, observer.TargetActorOrCorpse("#1", ignoreLayers ? PerceiveIgnoreFlags.IgnoreLayers : PerceiveIgnoreFlags.None));
	}
}
