#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework.Save;
using MudSharp.RPG.Law;
using MudSharp.TimeAndDate;

namespace MudSharpCore_Unit_Tests;

[TestClass]
public class RoomReferenceCompatibilityTests
{
	private static (Mock<IFuturemud> World, IRoom Room, IPerceiver Owner) Fixture()
	{
		var world = new Mock<IFuturemud>();
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(9000);
		room.SetupGet(x => x.FrameworkItemType).Returns("Room");
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		room.As<IPerceiver>();
		var rooms = new All<IRoom>();
		rooms.Add(room.Object);
		world.SetupGet(x => x.Rooms).Returns(rooms);
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition>());
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var registry = new Dictionary<string, Func<long, IPerceivable>>();
		world.Setup(x => x.RegisterPerceivableType(It.IsAny<string>(), It.IsAny<Func<long, IPerceivable>>()))
			.Callback<string, Func<long, IPerceivable>>((type, resolver) => registry.Add(type, resolver));
		Room.RegisterPerceivableType(world.Object);
		world.Setup(x => x.GetPerceivable(It.IsAny<string>(), It.IsAny<long>()))
			.Returns<string, long>((type, id) => registry.TryGetValue(type, out var resolver) ? resolver(id) : throw new NotSupportedException(type));
		var owner = new Mock<IPerceiver>();
		owner.SetupGet(x => x.Gameworld).Returns(world.Object);
		return (world, room.Object, owner.Object);
	}

	[TestMethod]
	public void Registry_CollidingHistoricalParentId_RefusesWhileExplicitChildFormatsResolve()
	{
		var f = Fixture();
		Assert.AreSame(f.Room, f.World.Object.GetPerceivable("Cell", 9000));
		Assert.AreSame(f.Room, f.World.Object.GetPerceivable("Room:v2", 9000));
		Assert.ThrowsException<InvalidOperationException>(() => f.World.Object.GetPerceivable("Room", 9000));
		Assert.ThrowsException<NotSupportedException>(() => f.World.Object.GetPerceivable("Room:v3", 9000));
		Assert.IsNull(f.World.Object.GetPerceivable("Cell", 8101));
	}

	[TestMethod]
	public void Tether_LegacyAndCurrentAnchors_RoundtripWithoutReinterpretingParents()
	{
		var f = Fixture();
		ZeroGravityTether.InitialiseEffectType();
		var original = new ZeroGravityTether(f.Owner, f.Room, 3);
		var xml = original.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		Assert.AreEqual("Room:v2", xml.Element("Effect")!.Element("AnchorType")!.Value);
		Assert.AreSame(f.Room, ((ZeroGravityTether)Effect.LoadEffect(xml, f.Owner)).Anchor);
		xml.Element("Effect")!.Element("AnchorType")!.Value = "Cell";
		Assert.AreSame(f.Room, ((ZeroGravityTether)Effect.LoadEffect(xml, f.Owner)).Anchor);
		xml.Element("Effect")!.Element("AnchorType")!.Value = "Room";
		Assert.ThrowsException<InvalidOperationException>(() => Effect.LoadEffect(xml, f.Owner));
	}

	[TestMethod]
	public void CheckResult_DifferentTargetAndToolAndCheckOutcome_PreserveEveryField()
	{
		var f = Fixture();
		var tool = new Mock<IFrameworkItem>();
		tool.SetupGet(x => x.Id).Returns(8101);
		tool.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		var check = new CheckResult(f.Owner, (CheckType)123, Difficulty.Normal, Outcome.MajorPass, target:f.Room, tool:tool.Object);
		var xml = check.SaveToXml(new Dictionary<IEffect,TimeSpan>());
		Assert.AreEqual("Room:v2", xml.Element("Effect")!.Attribute("TargetType")!.Value);
		Assert.AreEqual("GameItem", xml.Element("Effect")!.Attribute("ToolType")!.Value);
		var loaded = new CheckResult(xml, f.Owner);
		Assert.IsTrue(loaded.SameCheck((CheckType)123,Difficulty.Normal,f.Room,null,tool.Object));
		Assert.AreEqual(Outcome.MajorPass,loaded.Outcome);
		Assert.IsFalse(loaded.SameCheck((CheckType)123,Difficulty.Normal,f.Room,null,f.Room));
		xml.Element("Effect")!.Attribute("TargetType")!.Value = "Cell";
		Assert.IsTrue(new CheckResult(xml,f.Owner).SameCheck((CheckType)123,Difficulty.Normal,f.Room,null,tool.Object));
		Assert.IsTrue(loaded.SameResult(new CheckResult(xml,f.Owner)));
		Assert.IsFalse(loaded.ToString().Contains("Room:v2",StringComparison.Ordinal));
		xml.Element("Effect")!.Attribute("TargetType")!.Value = "Room";
		Assert.IsFalse(new CheckResult(xml,f.Owner).SameCheck((CheckType)123,Difficulty.Normal,f.Room,null,tool.Object));
		Assert.IsFalse(loaded.SameResult(new CheckResult(xml,f.Owner)));
	}

	[TestMethod]
	public void Emote_RoomTarget_StoresQualifiedTypeAndReadsLegacyCell()
	{
		var f = Fixture();
		var original = new Emote("$0", f.Owner, f.Room);
		Assert.IsTrue(original.Valid);
		var xml = original.SaveToXml();
		Assert.AreEqual("Room:v2", xml.Elements("Token").Single().Attribute("TargetType")!.Value);
		Assert.AreSame(f.Room, Emote.LoadEmote(xml,f.World.Object,f.Owner).Targets.Single());
		xml.Elements("Token").Single().Attribute("TargetType")!.Value = "Cell";
		Assert.AreSame(f.Room, Emote.LoadEmote(xml,f.World.Object,f.Owner).Targets.Single());
		xml.Elements("Token").Single().Attribute("TargetType")!.Value = "Room";
		Assert.ThrowsException<InvalidOperationException>(() => Emote.LoadEmote(xml,f.World.Object,f.Owner));
	}

	[TestMethod]
	public void Emote_OptionalSecondTarget_StoresAndRestoresBothRoomReferences()
	{
		var f = Fixture();
		var xml = new Emote("$0=1", f.Owner, f.Room, f.Room).SaveToXml();
		var token = xml.Elements("Token").Single();
		Assert.AreEqual("Room:v2",token.Attribute("TargetType")!.Value);
		Assert.AreEqual("Room:v2",token.Attribute("OtherType")!.Value);
		Assert.IsTrue(Emote.LoadEmote(xml,f.World.Object,f.Owner).Valid);
		token.Attribute("OtherType")!.Value="Cell";
		Assert.IsTrue(Emote.LoadEmote(xml,f.World.Object,f.Owner).Valid);
		token.Attribute("OtherType")!.Value="Room";
		Assert.ThrowsException<InvalidOperationException>(()=>Emote.LoadEmote(xml,f.World.Object,f.Owner));
	}

	[TestMethod]
	public void SpellTether_Anchor_UsesTheSameQualifiedReferenceBoundary()
	{
		var f = Fixture();
		SpellZeroGravityTetherEffect.InitialiseEffectType();
		var xml = new SpellZeroGravityTetherEffect(f.Owner,null,null,f.Room,3).SaveToXml(new Dictionary<IEffect,TimeSpan>());
		Assert.AreEqual("Room:v2",xml.Element("Effect")!.Element("AnchorType")!.Value);
		Assert.AreSame(f.Room,((SpellZeroGravityTetherEffect)Effect.LoadEffect(xml,f.Owner)).Anchor);
		xml.Element("Effect")!.Element("AnchorType")!.Value="Room";
		Assert.ThrowsException<InvalidOperationException>(()=>Effect.LoadEffect(xml,f.Owner));
	}

	[TestMethod]
	public void FixedPerceiver_LazyRoomReference_RejectsHistoricalParentToken()
	{
		var f=Fixture();
		OverrideSDescFromProg.InitialiseEffectType();
		var xml=new OverrideSDescFromProg(f.Owner,"description","tag",(IPerceiver)f.Room).SaveToXml(new Dictionary<IEffect,TimeSpan>());
		var reference=xml.Element("Effect")!.Element("FixedPerceiver")!;
		Assert.AreEqual("Room:v2",reference.Attribute("type")!.Value);
		Assert.AreSame(f.Room,((OverrideSDescFromProg)Effect.LoadEffect(xml,f.Owner)).FixedPerceiver);
		reference.Attribute("type")!.Value="Cell";
		Assert.AreSame(f.Room,((OverrideSDescFromProg)Effect.LoadEffect(xml,f.Owner)).FixedPerceiver);
		reference.Attribute("type")!.Value="Room";
		Assert.ThrowsException<InvalidOperationException>(()=>_ = ((OverrideSDescFromProg)Effect.LoadEffect(xml,f.Owner)).FixedPerceiver);
	}

	[DataTestMethod]
	[DataRow("Cell")]
	[DataRow("Room:v2")]
	public void Crime_LoadingThirdParty_RestoresIdAndTypeBeforeResolving(string type)
	{
		var f=Fixture();
		var law=new Mock<ILaw>();
		law.SetupGet(x=>x.Gameworld).Returns(f.World.Object);
		var crime=new Crime(new MudSharp.Models.Crime
		{
			Id=20,CriminalId=30,LocationId=9000,ThirdPartyId=9000,ThirdPartyIItemType=type,
			TimeOfCrime=MudDateTime.Never.GetDateTimeString(),RealTimeOfCrime=new DateTime(2026,10,7),
			CriminalCharacteristics="",WitnessIds="",CriminalShortDescription="fixture",CriminalFullDescription="fixture"
		},law.Object,f.World.Object);
		Assert.AreEqual(9000L,crime.ThirdPartyId);
		Assert.AreEqual(type,crime.ThirdPartyFrameworkItemType);
		Assert.AreSame(f.Room,crime.ThirdParty);
	}
}
