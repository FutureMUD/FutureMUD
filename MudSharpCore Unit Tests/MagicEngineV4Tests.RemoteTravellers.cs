#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	[TestMethod]
	[DataRow(false, 1)] [DataRow(false, 2)] [DataRow(false, 3)] [DataRow(false, 4)] [DataRow(false, 5)] [DataRow(false, 6)] [DataRow(false, 7)]
	[DataRow(true, 1)] [DataRow(true, 2)] [DataRow(true, 3)] [DataRow(true, 4)] [DataRow(true, 5)] [DataRow(true, 6)] [DataRow(true, 7)]
	public void RemoteTravellers_AllGradesMoveExactlyOneRecognisedBodyAndConsumeOneSourceComponent(bool draw, int grade)
	{
		var f = new RemoteFixture(draw, 2); f.Pair.Target.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Pair.Moves);
		Assert.AreSame(draw ? f.Pair.Source.Object : f.Pair.Destination.Object, f.Pair.CasterLocation);
		Assert.AreSame(draw ? f.Pair.Source.Object : f.Pair.Destination.Object, f.Pair.TargetLocation);
		Assert.AreEqual(draw ? 2 : 3, f.Pair.AlterationQuantity); Assert.AreEqual(draw ? 3 : 2, f.Pair.TeleportationQuantity);
		Assert.IsTrue(f.Pair.F.Balances[f.Pair.F.Resources[1]] < 100.0);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Pair.F.Service.Cast(f.Intent(grade) with { OriginId = result.OperationId }).Status);
		Assert.AreEqual(1, f.Pair.Moves);
	}

	[TestMethod]
	[DataRow(false, 1)] [DataRow(false, 6)] [DataRow(true, 1)] [DataRow(true, 6)]
	public void RemoteTravellers_JoinStrictAndDrawInclusiveSourcePathBoundsRemainDistinct(bool draw, int grade)
	{
		var last = 20 * grade - (draw ? 0 : 1);
		var permitted = new RemoteFixture(draw, last);
		Assert.AreEqual(MagicCastingStatus.Succeeded, permitted.Cast(grade).Status);
		var beyond = new RemoteFixture(draw, last + 1);
		Assert.AreEqual(MagicCastingStatus.Refused, beyond.Cast(grade).Status);
		Assert.AreEqual(100.0, beyond.Pair.F.Balances[beyond.Pair.F.Resources[1]]); Assert.AreEqual(0, beyond.Pair.Moves);
	}

	[TestMethod]
	[DataRow(false)] [DataRow(true)]
	public void RemoteTravellers_GradeSevenRetainsItsSourceRangeExceptionWithoutBypassingEndpoints(bool draw)
	{
		var f = new RemoteFixture(draw, 1);
		f.Pair.Source.Setup(x => x.ExitsFor(f.Pair.F.Actor.Object, false)).Returns([]);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(6).Status);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(7).Status);
		Assert.AreEqual(1, f.Pair.Moves);
	}

	[TestMethod]
	[DataRow(false, "indoors")] [DataRow(true, "indoors")] [DataRow(false, "restricted")] [DataRow(true, "restricted")]
	[DataRow(false, "unverified")] [DataRow(true, "unverified")] [DataRow(false, "material")] [DataRow(true, "material")]
	public void RemoteTravellers_RequiredSourceAuthorityAndMaterialsRefuseBeforePayment(bool draw, string reason)
	{
		var f = new RemoteFixture(draw, 1);
		if (reason == "indoors") f.Pair.Destination.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(RoomOutdoorsType.Indoors);
		if (reason == "restricted") f.Pair.Source.Setup(x => x.IsA(f.Pair.Restricted)).Returns(true);
		if (reason == "unverified") f.Trace.Dub.SetupGet(x => x.HasVerifiedIdentity).Returns(false);
		if (reason == "material") f.Pair.Items.Remove(draw ? f.Pair.Alteration : f.Pair.Teleportation);
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status);
		Assert.AreEqual(0, f.Pair.Moves); Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
		Assert.AreEqual(3, f.Pair.AlterationQuantity); Assert.AreEqual(3, f.Pair.TeleportationQuantity);
	}

	[TestMethod]
	[DataRow(1, 0)] [DataRow(2, 0)] [DataRow(3, 1)] [DataRow(4, 2)] [DataRow(5, 3)] [DataRow(6, 4)] [DataRow(7, 6)]
	public void RemoteTravellers_DrawUsesExplicitExistingConjurationRanksIncludingSparseHighestSupply(int grade, int rank)
	{
		var f = new RemoteFixture(true, 1, rank);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast(grade).Status);
		Assert.AreEqual(2, f.Pair.AlterationQuantity);
		if (grade < 3) return;
		var insufficient = new RemoteFixture(true, 1, Math.Max(grade - 3, 0));
		Assert.AreEqual(MagicCastingStatus.Refused, insufficient.Cast(grade).Status);
		Assert.AreEqual(3, insufficient.Pair.AlterationQuantity);
		Assert.AreEqual(100.0, insufficient.Pair.F.Balances[insufficient.Pair.F.Resources[1]]);
	}

	[TestMethod]
	public void RemoteTravellers_DrawNativeResistancePreventsMovementButRetainsNormalPaidAttempt()
	{
		var f = new RemoteFixture(true, 1); f.Pair.F.Outcome = Outcome.MinorPass;
		f.Pair.F.Check.Setup(x => x.CheckAgainstAllDifficulties(It.Is<IPerceivableHaveTraits>(x => ReferenceEquals(x, f.Pair.Target.Object)),
			It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorPass)));
		f.Cast(1);
		Assert.AreEqual(0, f.Pair.Moves); Assert.AreEqual(0, f.Pair.F.Samples);
		Assert.IsTrue(f.Pair.F.Balances[f.Pair.F.Resources[1]] < 100.0);
	}

	[TestMethod]
	public void RemoteTravellers_JoinCompensatesFailedArrivalAndQuarantinesUnrepairableMembership()
	{
		var f = new RemoteFixture(false, 1); f.Pair.ThrowAfterCasterArrival = true;
		var result = f.Cast(1);
		Assert.AreEqual("false", (string?)XElement.Parse(f.Pair.F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied"),
			result.Message + "; " + f.Pair.F.Store.Operations[result.OperationId.Value].Definition);
		Assert.AreSame(f.Pair.Source.Object, f.Pair.CasterLocation); Assert.IsTrue(f.Pair.SourceMembers.Any(x => ReferenceEquals(x, f.Pair.F.Actor.Object)));
		Assert.AreEqual(0, f.Pair.F.Samples);
		var uncertain = new RemoteFixture(false, 1); uncertain.Pair.ThrowAfterFirstLeave = true;
		Assert.AreEqual(MagicCastingStatus.NeedsReview, uncertain.Cast(1).Status);
		Assert.AreEqual(0, uncertain.Pair.F.Samples);
	}

	[TestMethod]
	public void RemoteTravellers_NativeCallbackCannotRevokeIdentityAndStillCompleteForwardDisplacement()
	{
		var f = new RemoteFixture(true, 1); f.Pair.BeforeTargetMove = () => f.Trace.Dubs.Clear();
		var result = f.Cast(1);
		Assert.AreSame(f.Pair.Destination.Object, f.Pair.TargetLocation);
		Assert.AreEqual(0, f.Pair.F.Samples);
		Assert.AreEqual("false", (string?)XElement.Parse(f.Pair.F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied"), result.Message);
	}

	private sealed class RemoteFixture
	{
		public TraceFixture Trace { get; }
		public ExchangeFixture Pair => Trace.Pair;
		public RemoteFixture(bool draw, int distance, int componentRank = 6)
		{
			Trace = new(distance); var f = Pair.F;
			if (draw)
				f.Check.Setup(x => x.CheckAgainstAllDifficulties(It.Is<IPerceivableHaveTraits>(x => ReferenceEquals(x, Pair.Target.Object)),
					It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
					.Returns(Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Outcome.MajorFail)));
			var tags = f.World.Object.Tags.ToList();
			var ranks = new[] { 0, 1, 2, 3, 4, 6 }.Select(rank =>
			{
				var tag = new Mock<ITag>(); tag.SetupGet(x => x.Id).Returns(100 + rank);
				tag.SetupGet(x => x.Name).Returns("Conjuration rank " + rank);
				tag.Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(other => other is null || other.Id == 100 + rank);
				tags.Add(tag.Object); return (Rank: rank, Tag: tag.Object.Id);
			}).ToArray();
			f.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => tags));
			var tagIds = ranks.Select(x => x.Tag).ToHashSet();
			Mock.Get(Pair.Alteration).Setup(x => x.IsA(It.IsAny<ITag>())).Returns<ITag>(tag => tag is null || tagIds.Contains(tag.Id) && tag.Id == 100 + componentRank);
			Mock.Get(Pair.Teleportation).Setup(x => x.IsA((ITag)null!)).Returns(true);
			var content = (draw ? ArmageddonRemoteTravellerSpellContent.Draw(21, ranks, 1) :
				ArmageddonRemoteTravellerSpellContent.Join(21, Enumerable.Range(1, 6).Select(x => (long)x).ToArray())).Content;
			var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2; row.Definition = content.BuildDefinition(10, 1, 0).ToString();
			f.Spells.Clear(); f.Spells.Add(new MagicSpell(row, f.World.Object) { EffectDurationExpression = new MudSharp.Body.Traits.TraitExpression("0", f.World.Object) });
		}
		public MagicCastingIntent Intent(int grade) => new(Pair.F.Actor.Object, Pair.F.Earth.Id, 1, grade, false, "traveller", OriginId: Guid.NewGuid());
		public MagicCastingResult Cast(int grade) => Pair.F.Service.Cast(Intent(grade));
	}
}
