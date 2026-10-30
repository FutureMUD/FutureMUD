#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.Templates;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NativeNpcPurchaseTests
{
	[TestMethod]
	public void PrivatePlacement_SetsCoordinatesWithoutWorldOrMovementCallbacks()
	{
		var npc = (MudSharp.NPC.NPC)RuntimeHelpers.GetUninitializedObject(typeof(MudSharp.NPC.NPC));
		var room = Mock.Of<IRoom>();
		var point = new SpatialLocation(room, RoomLayer.GroundLevel, 2.5);
		npc.SetPreparedNativeNpcLocation(point);
		Assert.AreEqual(point, npc.SpatialLocation);
	}

	[TestMethod]
	public void Purchase_InvalidTemplateIds_RefusesBeforeTemplateLookupOrPayment()
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		var buyer = Mock.Of<ICharacter>(x => x.Body == Mock.Of<IBody>());
		var seller = Mock.Of<ICharacter>(x => x.Body == Mock.Of<IBody>());
		foreach (var id in new[] { -1m, 0m, 1.25m, decimal.MaxValue })
		{
			var function = new PurchaseNpcFunction([
				Constant(buyer, ProgVariableTypes.Character), Constant(seller, ProgVariableTypes.Character),
				new ConstantFunction(new NumberVariable(id)), Constant(Mock.Of<ICurrency>(), ProgVariableTypes.Currency),
				new ConstantFunction(new NumberVariable(5m))], world.Object);
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			Assert.IsNull(function.Result);
		}
		world.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void Preparation_UnsupportedWorld_RefusesWithoutPublishingOrAllocatingDatabaseRows()
	{
		var world = new Mock<IFuturemud>(MockBehavior.Strict);
		var buyer = new Mock<ICharacter>();
		var seller = new Mock<ICharacter>();
		buyer.SetupGet(x => x.Gameworld).Returns(world.Object);
		seller.SetupGet(x => x.Gameworld).Returns(world.Object);
		buyer.SetupGet(x => x.State).Returns(CharacterState.Awake);
		seller.SetupGet(x => x.State).Returns(CharacterState.Awake);
		buyer.Setup(x => x.ColocatedWith(seller.Object)).Returns(true);
		var template = new Mock<INPCTemplate>();
		template.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		using var participant = new NativeNpcPurchaseParticipant(buyer.Object, seller.Object, template.Object);
		Assert.IsFalse(participant.Prepare());
		Assert.IsFalse(participant.IsCurrent);
		Assert.IsNull(participant.DeliveredNpc);
		world.VerifyNoOtherCalls();
	}

	private static IFunction Constant(object value, ProgVariableTypes type) => new ConstantFunction(new ValueVariable(value, type));
	private sealed record ValueVariable(object Value, ProgVariableTypes Type) : IProgVariable
	{
		public object GetObject => Value;
		public IProgVariable GetProperty(string property) => throw new NotSupportedException();
	}
	private sealed class ConstantFunction(IProgVariable result) : IFunction
	{
		public IProgVariable Result => result;
		public ProgVariableTypes ReturnType => result.Type;
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;
		public StatementResult Execute(IVariableSpace variables) => StatementResult.Normal;
		public bool IsReturnOrContainsReturnOnAllBranches() => false;
	}
}
