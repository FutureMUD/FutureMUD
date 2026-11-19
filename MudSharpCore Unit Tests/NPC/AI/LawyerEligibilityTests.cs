#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.NPC.AI;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests.NPC.AI;

[TestClass]
public class LawyerEligibilityTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void AvailableForHire_PrivateUsesDefendantPolicyAndCourtAppointmentUsesItsOwnSwitch(bool accepts)
	{
		var courtroom = Mock.Of<IRoom>();
		var lawyer = Mock.Of<ICharacter>(x => x.Location == courtroom);
		var defendant = Mock.Of<ICharacter>();
		var authority = Mock.Of<ILegalAuthority>(x => x.CourtLocation == courtroom);
		var policy = new Mock<IFutureProg>();
		policy.SetupGet(x => x.Id).Returns(1);
		policy.Setup(x => x.ExecuteBool(defendant, lawyer)).Returns(accepts);
		var world = Mock.Of<IFuturemud>(x => x.FutureProgs == new All<IFutureProg> { policy.Object });
		var ai = new FixtureLawyer(world);

		Assert.AreEqual(accepts, ai.AvailableForPrivateHire(lawyer, authority, defendant));
		Assert.IsTrue(ai.AvailableForCourtAppointment(lawyer, authority));
		policy.Verify(x => x.ExecuteBool(It.IsAny<object[]>()), Times.Once);
		policy.Verify(x => x.ExecuteBool(defendant, lawyer), Times.Once);
	}

	private sealed class FixtureLawyer(IFuturemud world) : LawyerAI(new MudSharp.Models.ArtificialIntelligence
	{
		Name = "Counsel",
		Definition = """
			<Definition>
			  <OpenDoors>false</OpenDoors><UseKeys>false</UseKeys><SmashLockedDoors>false</SmashLockedDoors>
			  <MoveEvenIfObstructionInWay>false</MoveEvenIfObstructionInWay><UseDoorguards>false</UseDoorguards>
			  <CanBeEngagedAsCourtAppointedLawyer>true</CanBeEngagedAsCourtAppointedLawyer>
			  <CanBeHiredProg>1</CanBeHiredProg><FeeProg>0</FeeProg><HomeBaseProg>0</HomeBaseProg><BankAccountProg>0</BankAccountProg>
			</Definition>
			"""
	}, world);
}
