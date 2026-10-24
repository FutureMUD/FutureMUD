#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.NPC.AI;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	[DataRow(false, 1)] [DataRow(false, 2)] [DataRow(false, 3)] [DataRow(false, 4)] [DataRow(false, 5)] [DataRow(false, 6)] [DataRow(false, 7)]
	[DataRow(true, 1)] [DataRow(true, 2)] [DataRow(true, 3)] [DataRow(true, 4)] [DataRow(true, 5)] [DataRow(true, 6)] [DataRow(true, 7)]
	public void WeightedTravel_AllGradesMoveOneExactBodyAndNeverDrawDuringQuotes(bool banish, int grade)
	{
		var f = new WeightedFixture(banish); var random = new TravelRandom(99);
		using var scope = Constants.PushRandom(random);
		var quote = f.Pair.F.Service.Quote(f.Intent(grade)); Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(0, random.Draws);
		var result = f.Pair.F.Service.Cast(f.Intent(grade));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(f.Pair.Destination.Object, banish ? f.Pair.TargetLocation : f.Pair.CasterLocation);
		Assert.AreSame(f.Pair.Source.Object, banish ? f.Pair.CasterLocation : f.Pair.TargetLocation);
		Assert.AreEqual(3, random.Draws); Assert.AreEqual(1, f.Moves);
		Assert.AreEqual(3, f.Pair.AlterationQuantity); Assert.AreEqual(3, f.Pair.TeleportationQuantity);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Pair.F.Service.Cast(f.Intent(grade) with { OriginId = result.OperationId }).Status);
		Assert.AreEqual(3, random.Draws); Assert.AreEqual(1, f.Moves);
	}

	[TestMethod]
	[DataRow(9, true)] [DataRow(10, false)] [DataRow(99, false)]
	public void WeightedTravel_UrbanTenWildNinetyBoundaryUsesGroupThenZoneThenRoom(int pick, bool urban)
	{
		var f = new WeightedFixture(false); using var scope = Constants.PushRandom(new TravelRandom(pick));
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Pair.F.Service.Cast(f.Intent(1)).Status);
		Assert.AreSame(urban ? f.Urban.Object : f.Pair.Destination.Object, f.Pair.CasterLocation);
	}

	[TestMethod]
	[DataRow("unmapped")] [DataRow("overlap")] [DataRow("indoors")] [DataRow("lethal")] [DataRow("changed")]
	public void WeightedTravel_UnsafeEmptyAmbiguousOrChangedPoolsRefuseWithoutPayment(string reason)
	{
		var f = new WeightedFixture(false); f.UrbanTags.Clear();
		if (reason == "unmapped") f.WildTags.Clear();
		if (reason == "overlap") f.WildTags.Add(f.UrbanTag);
		if (reason == "indoors") f.Pair.Destination.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(RoomOutdoorsType.Indoors);
		if (reason == "lethal") f.WildTags.Add(f.LethalTag);
		if (reason == "changed") f.Pair.F.Checkpoint = stage => { if (stage == "BeforePayment") f.WildTags.Clear(); };
		var result = f.Pair.F.Service.Cast(f.Intent(1));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(0, f.Moves); Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
	}

	[TestMethod]
	public void WeightedTravel_BanishUsesNativeResistanceAndDoesNotMintAFreeSaveWaiver()
	{
		var f = new WeightedFixture(true); f.Pair.F.Outcome = Outcome.MinorPass;
		f.Pair.F.Check.Setup(x => x.CheckAgainstAllDifficulties(It.Is<IPerceivableHaveTraits>(x => ReferenceEquals(x, f.Pair.Target.Object)),
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorPass)));
		f.Pair.F.Service.Cast(f.Intent(1)); Assert.AreEqual(0, f.Moves);
		Assert.IsTrue(f.Pair.F.Balances[f.Pair.F.Resources[1]] < 100.0); Assert.AreEqual(0, f.Pair.F.Samples);
	}

	private sealed class TravelRandom(int pick) : Random
	{
		public int Draws { get; private set; }
		public override int Next(int maxValue) { Draws++; return Draws == 1 ? Math.Min(pick, maxValue - 1) : 0; }
		public override double NextDouble() => 0.5;
	}

	[TestMethod]
	public void WeightedTravel_MovementCallbackActivatingAnExistingConditionalWardPreventsForwardArrival()
	{
		var f = new WeightedFixture(false); f.UrbanTags.Clear(); var active = false;
		var ward = new Mock<IMagicInterdictionEffect>(); var effect = ward.As<IEffect>();
		ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming);
		ward.Setup(x => x.ShouldInterdict(It.IsAny<ICharacter>(), It.IsAny<IMagicSchool>())).Returns(() => active);
		f.Pair.Destination.SetupGet(x => x.Effects).Returns([effect.Object]);
		f.Pair.Destination.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		f.BeforeMove = () => active = true;
		var result = f.Pair.F.Service.Cast(f.Intent(1));
		Assert.AreSame(f.Pair.Source.Object, f.Pair.CasterLocation);
		Assert.AreEqual(0, f.Pair.F.Samples); Assert.IsTrue(f.Pair.F.Balances[f.Pair.F.Resources[1]] < 100.0);
		Assert.AreEqual("false", (string?)System.Xml.Linq.XElement.Parse(f.Pair.F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied"), result.Message);
	}

	private sealed class WeightedFixture
	{
		public ExchangeFixture Pair { get; } = new();
		public Mock<IRoom> Urban { get; } = new() { DefaultValue = DefaultValue.Mock };
		public List<ITag> WildTags { get; } = [];
		public List<ITag> UrbanTags { get; } = [];
		public ITag UrbanTag { get; }
		public ITag LethalTag { get; }
		public int Moves { get; private set; }
		public Action? BeforeMove { get; set; }
		private readonly bool _banish;
		public WeightedFixture(bool banish)
		{
			_banish = banish; var f = Pair.F;
			Pair.DestinationMembers.Clear(); Pair.SourceMembers.Add(Pair.Target.Object); Pair.TargetLocation = Pair.Source.Object;
			var members = new List<ICharacter>();
			Urban.SetupGet(x => x.Id).Returns(303); Urban.SetupGet(x => x.Gameworld).Returns(f.World.Object);
			Urban.SetupGet(x => x.Zone).Returns(Pair.Source.Object.Zone); Urban.SetupGet(x => x.OwningZone).Returns(Pair.Source.Object.OwningZone);
			Urban.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!); Urban.SetupGet(x => x.Characters).Returns(members);
			Urban.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(RoomOutdoorsType.Outdoors);
			Mock.Get(Urban.Object.CurrentOverlay.Package).SetupGet(x => x.Status).Returns(RevisionStatus.Current);
			Urban.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(Pair.Source.Object.CurrentOverlay.Terrain);
			Mock.Get(Urban.Object.CurrentOverlay).SetupGet(x => x.Terrain).Returns(Pair.Source.Object.CurrentOverlay.Terrain);
			var tags = f.World.Object.Tags.ToList();
			ITag Tag(long id) { var tag = new Mock<ITag>(); tag.SetupGet(x => x.Id).Returns(id); tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(other => other?.Id == id); tags.Add(tag.Object); return tag.Object; }
			LethalTag = Tag(23); UrbanTag = Tag(24); var wild = Tag(25); Tag(26);
			WildTags.Add(wild); UrbanTags.Add(UrbanTag);
			Pair.Destination.SetupGet(x => x.Tags).Returns(WildTags); Urban.SetupGet(x => x.Tags).Returns(UrbanTags);
			f.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => tags));
			f.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => new[] { Pair.Source.Object, Pair.Destination.Object, Urban.Object }));
			var planes = new[] { 1L, 2L }.Select(id => Mock.Of<IPlane>(x => x.Id == id)).ToArray();
			f.World.SetupGet(x => x.DefaultPlane).Returns(planes[0]); f.World.SetupGet(x => x.Planes).Returns(MagicCastingFixture.Collection(() => planes));
			f.Actor.Setup(x => x.TargetActor("traveller", It.IsAny<PerceiveIgnoreFlags>())).Returns(Pair.Target.Object);
			f.Actor.Setup(x => x.CanSee(Pair.Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			f.Check.Setup(x => x.CheckAgainstAllDifficulties(It.Is<IPerceivableHaveTraits>(x => ReferenceEquals(x, Pair.Target.Object)),
				It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorFail)));
			var content = ArmageddonWeightedTravelSpellContent.Create(21, 23, 24, 25, 26, [2], 1)[banish ? 1 : 0].Content;
			var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new TraitExpression("0", f.World.Object) });
			void Move(bool target, IInvocation call)
			{
				var actor = target ? Pair.Target.Object : f.Actor.Object;
				Assert.IsFalse((bool)call.Arguments[2]); if ((bool)call.Arguments[3]) BeforeMove?.Invoke();
				if (!CommandExecutionScope.TryContinue(actor)) return;
				Moves++; Pair.SourceMembers.RemoveAll(x => ReferenceEquals(x, actor)); Pair.DestinationMembers.RemoveAll(x => ReferenceEquals(x, actor)); members.RemoveAll(x => ReferenceEquals(x, actor));
				var destination = (IRoom)call.Arguments[0];
				if (target) Pair.TargetLocation = destination; else Pair.CasterLocation = destination;
				(ReferenceEquals(destination, Urban.Object) ? members : ReferenceEquals(destination, Pair.Source.Object) ? Pair.SourceMembers : Pair.DestinationMembers).Add(actor);
			}
			f.Actor.Setup(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), false, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(new InvocationAction(call => Move(false, call)));
			Pair.Target.Setup(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), false, It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(new InvocationAction(call => Move(true, call)));
		}
		public MagicCastingIntent Intent(int grade) => new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, _banish ? "traveller" : "random", OriginId: Guid.NewGuid());
	}
}
