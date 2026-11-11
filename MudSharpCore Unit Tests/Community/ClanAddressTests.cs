#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Community;

[TestClass]
public class ClanAddressTests
{
	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IClan> Clan { get; } = new();
		public Mock<IRank> Rank { get; } = new();
		public Mock<IRank> SpeakerRank { get; } = new();
		public Mock<ICharacter> Subject { get; } = new();
		public Mock<ICharacter> Speaker { get; } = new();
		public Mock<IClanMembership> Member { get; } = new();
		public Mock<IClanMembership> SpeakerMember { get; } = new();
		public List<ClanAddressRule> Rules { get; } = [];
		public List<IAppointment> Offices { get; } = [];
		public All<IFutureProg> Progs { get; } = new();
		public Fixture()
		{
			World.SetupGet(x => x.FutureProgs).Returns(Progs);
			Speaker.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
						Clan.SetupGet(x => x.Id).Returns(1);
			World.SetupGet(x => x.Clans).Returns(new All<IClan> { Clan.Object });
			Rank.SetupGet(x => x.Id).Returns(2);
			Rank.SetupGet(x => x.Clan).Returns(Clan.Object);
			Rank.SetupGet(x => x.Name).Returns("Captain");
			Rank.SetupGet(x => x.FormsOfAddress).Returns(Rules);
			Rank.SetupProperty(x => x.RankNumber, 5);
			SpeakerRank.SetupProperty(x => x.RankNumber, 1);
			Member.SetupGet(x => x.Clan).Returns(Clan.Object);
			Member.SetupProperty(x => x.Rank, Rank.Object);
			Member.SetupProperty(x => x.IsArchivedMembership, false);
			Member.SetupGet(x => x.MemberId).Returns(10);
			Member.SetupGet(x => x.Appointments).Returns(Offices);
			SpeakerMember.SetupGet(x => x.Clan).Returns(Clan.Object);
			SpeakerMember.SetupGet(x => x.Rank).Returns(SpeakerRank.Object);
			SpeakerMember.SetupProperty(x => x.IsArchivedMembership, false);
			SpeakerMember.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.CanViewMembers);
			Subject.SetupGet(x => x.Id).Returns(10);
			Speaker.SetupGet(x => x.Id).Returns(11);
			Subject.SetupGet(x => x.Gameworld).Returns(World.Object);
			Speaker.SetupGet(x => x.Gameworld).Returns(World.Object);
			Subject.SetupGet(x => x.ClanMemberships).Returns([Member.Object]);
			Speaker.SetupGet(x => x.ClanMemberships).Returns([SpeakerMember.Object]);
			Speaker.Setup(x => x.CanSee(Subject.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Clan.SetupGet(x => x.Memberships).Returns([Member.Object, SpeakerMember.Object]);
			Subject.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
			Subject.SetupGet(x => x.GetObject).Returns(Subject.Object);
			Subject.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(10));
			Speaker.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
			Speaker.SetupGet(x => x.GetObject).Returns(Speaker.Object);
			Speaker.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(11));
		}
		public ClanAddressResult Resolve(ICharacter? speaker = null) => ClanAddressResolver.Resolve(speaker ?? Speaker.Object, Subject.Object, Clan.Object);
		public Mock<IAppointment> Office(string text, int priority = 0)
		{
			var office = new Mock<IAppointment>();
			office.SetupGet(x => x.Id).Returns(100 + Offices.Count);
			office.SetupGet(x => x.Name).Returns(text);
			office.SetupGet(x => x.Clan).Returns(Clan.Object);
			office.SetupGet(x => x.AddressPriority).Returns(priority);
			office.SetupGet(x => x.FormsOfAddress).Returns([new() { Text = text }]);
			Offices.Add(office.Object);
			return office;
		}
	}

	[TestMethod]
	public void Resolve_OneDefaultAndEmptyConfiguration_HaveOptionalBehavior()
	{
		var f = new Fixture();
		Assert.AreEqual(ClanAddressStatus.NoMatch, f.Resolve().Status);
		f.Rules.Add(new() { Text = "Captain" });
		Assert.AreEqual("Captain", f.Resolve().Text);
	}

	[TestMethod]
	[DataRow(1, "Sir")]
	[DataRow(5, "Captain")]
	[DataRow(9, "Colleague")]
	public void Resolve_RelativeRank_UsesSpeakerRelativeToSubject(int speakerRank, string expected)
	{
		var f = new Fixture();
		f.SpeakerRank.Object.RankNumber = speakerRank;
		f.Rules.AddRange([new() { Text = "Sir", Relations = ClanAddressRelation.Lower },
			new() { Text = "Colleague", Relations = ClanAddressRelation.Higher }, new() { Text = "Captain" }]);
		Assert.AreEqual(expected, f.Resolve().Text);
		f.Rank.Object.RankNumber = 10;
		Assert.AreEqual("Sir", f.Resolve().Text, "Comparison must use current rank ordinals.");
	}

	[TestMethod]
	public void Resolve_NullSpeaker_TriesPublicOutsiderThenLowerAndHonorsSuppression()
	{
		var f = new Fixture();
		f.Rules.Add(new() { Text = "Sir", Relations = ClanAddressRelation.Lower, IsPublic = true });
		ClanAddressResult Resolve() => ClanAddressResolver.Resolve(null, f.Subject.Object, f.Clan.Object);
		Assert.AreEqual("Sir", Resolve().Text);
		f.Rules.Insert(0, new() { Text = "Captain", Relations = ClanAddressRelation.Outsider, IsPublic = true });
		Assert.AreEqual("Captain", Resolve().Text);
		f.Rules[0].Text = null;
		Assert.AreEqual(ClanAddressStatus.Suppressed, Resolve().Status);
		f.Rules.ForEach(x => x.IsPublic = false);
		Assert.AreEqual(ClanAddressStatus.NoMatch, Resolve().Status);
	}

	[TestMethod]
	public void Resolve_NonNullOutsider_DoesNotPretendToBeInferior()
	{
		var f = new Fixture();
		f.Speaker.SetupGet(x => x.ClanMemberships).Returns([]);
		f.Rules.Add(new() { Text = "Sir", Relations = ClanAddressRelation.Lower, IsPublic = true });
		Assert.AreEqual(ClanAddressStatus.NoMatch, f.Resolve().Status);
		f.Rules.Add(new() { Text = "Captain", IsPublic = true });
		Assert.AreEqual("Captain", f.Resolve().Text);
	}

	[TestMethod]
	public void Resolve_OfficesAndRank_DistinguishesNoMatchSuppressionAndPriorityConflicts()
	{
		var f = new Fixture(); f.Rules.Add(new() { Text = "Captain" });
		var first = f.Office("Director", 10);
		var second = f.Office("Chair", 10);
		Assert.AreEqual(ClanAddressStatus.Ambiguous, f.Resolve().Status);
		second.Object.FormsOfAddress[0].Text = "Director";
		Assert.AreEqual("Director", f.Resolve().Text);
		second.SetupGet(x => x.AddressPriority).Returns(20);
		second.Object.FormsOfAddress[0].Text = null;
		Assert.AreEqual(ClanAddressStatus.Suppressed, f.Resolve().Status);
		second.Object.FormsOfAddress.Clear(); first.Object.FormsOfAddress.Clear();
		Assert.AreEqual("Captain", f.Resolve().Text);
	}

	[TestMethod]
	public void Resolve_ArchivedHiddenAndSameIdentity_PreventDisclosure()
	{
		var f = new Fixture(); f.Rules.Add(new() { Text = "Captain" });
		f.SpeakerMember.Object.IsArchivedMembership = true;
		Assert.AreEqual(ClanAddressStatus.NoMatch, f.Resolve().Status);
		f.SpeakerMember.Object.IsArchivedMembership = false;
		f.Subject.Setup(x => x.IdentityIsObscuredTo(f.Speaker.Object)).Returns(true);
		Assert.AreEqual(ClanAddressStatus.Hidden, f.Resolve().Status);
		f.Subject.Setup(x => x.IdentityIsObscuredTo(f.Speaker.Object)).Returns(false);
		f.Speaker.Setup(x => x.CanSee(f.Subject.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Assert.AreEqual(ClanAddressStatus.Hidden, f.Resolve().Status);
		Assert.AreEqual(ClanAddressStatus.NoMatch, ClanAddressResolver.Resolve(f.Subject.Object, f.Subject.Object, f.Clan.Object).Status);
		f.Speaker.Setup(x => x.CanSee(f.Subject.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		f.Member.Object.IsArchivedMembership = true;
		Assert.AreEqual(ClanAddressStatus.NoMatch, f.Resolve().Status);
	}

	[TestMethod]
	public void Resolve_MissingOrFailedPredicate_DoesNotBecomeUnconditionalOrFallBack()
	{
		var f = new Fixture(); f.Rules.AddRange([new() { Text = "Special", FutureProgId = 99 }, new() { Text = "Captain" }]);
		Assert.AreEqual(ClanAddressStatus.Error, f.Resolve().Status);
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(99);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.SetupGet(x => x.Parameters).Returns([ProgVariableTypes.Character]);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		// ExecuteWithStatus returns false: execution failed, rather than a predicate returning false.
		f.Progs.Add(prog.Object);
		Assert.AreEqual(ClanAddressStatus.Error, f.Resolve().Status);
	}

	[TestMethod]
	public void Resolve_OneAndTwoCharacterPredicates_ReceiveSubjectAndSpeakerInDocumentedOrder()
	{
		var f = new Fixture();
		var one = Compile(f, "One", "return @subject.id == 10", ProgVariableTypes.Boolean, ("subject", ProgVariableTypes.Character));
		f.Rules.Add(new() { Text = "Subject", FutureProgId = one.Id });
		Assert.AreEqual("Subject", f.Resolve().Text);
		var two = Compile(f, "Two", "return @speaker.id == 11 and @subject.id == 10", ProgVariableTypes.Boolean,
			("speaker", ProgVariableTypes.Character), ("subject", ProgVariableTypes.Character));
		f.Rules[0].FutureProgId = two.Id;
		Assert.AreEqual("Subject", f.Resolve().Text);
	}

	[TestMethod]
	public void FutureProg_Functions_CompileAndReturnContextualTextIncludingNullSpeaker()
	{
		var f = new Fixture(); f.Rules.Add(new() { Text = "Captain", IsPublic = true });
		f.Clan.SetupGet(x => x.GetObject).Returns(f.Clan.Object);
		f.Clan.SetupGet(x => x.Type).Returns(ProgVariableTypes.Clan);
		f.Rank.SetupGet(x => x.GetObject).Returns(f.Rank.Object);
		f.Rank.SetupGet(x => x.Type).Returns(ProgVariableTypes.ClanRank);
		var office = f.Office("Director"); office.Object.FormsOfAddress[0].IsPublic = true;
		office.SetupGet(x => x.GetObject).Returns(office.Object);
		office.SetupGet(x => x.Type).Returns(ProgVariableTypes.ClanAppointment);
		foreach (var (name, type, source, expected) in new (string, ProgVariableTypes, IProgVariable, string)[]
		         { ("ClanFormOfAddress", ProgVariableTypes.Clan, f.Clan.Object, "Director"),
			         ("RankFormOfAddress", ProgVariableTypes.ClanRank, f.Rank.Object, "Captain"),
			         ("AppointmentFormOfAddress", ProgVariableTypes.ClanAppointment, office.Object, "Director") })
		{
			var prog = Compile(f, "Getter" + name, $"return {name}(@speaker, @subject, @source)", ProgVariableTypes.Text,
				("speaker", ProgVariableTypes.Character), ("subject", ProgVariableTypes.Character), ("source", type));
			Assert.AreEqual(expected, prog.ExecuteString(f.Speaker.Object, f.Subject.Object, source));
			Assert.AreEqual(expected, prog.ExecuteString(null!, f.Subject.Object, source));
			if (type == ProgVariableTypes.ClanAppointment)
			{
				f.Offices.Clear();
				Assert.AreEqual("", prog.ExecuteString(f.Speaker.Object, f.Subject.Object, source));
			}
		}
	}

	[TestMethod]
	public void FormatLook_ConfigurableLiteralTokens_DoNotReinterpretReplacementValues()
	{
		Assert.AreEqual("Call her Your Grace in the court. She expects respect.", ClanAddressResolver.FormatLook(
			"Call {objective} {address} in {clan}. {subjective} expects respect.", "Your Grace", "the court", "She", "her", "her"));
		Assert.AreEqual("{clan}", ClanAddressResolver.FormatLook("{address}", "{clan}", "secret", "", "", ""));
		Assert.AreEqual("{unknown}", ClanAddressResolver.FormatLook("{unknown}", "", "", "", "", ""));
	}

	[TestMethod]
	public void LookHint_GlobalAndClanSettings_UseCustomFormatWithoutLeakingArchivedOrObscuredMembership()
	{
		var f = new Fixture(); f.Rules.Add(new() { Text = "Sir", IsPublic = true });
		f.Clan.SetupGet(x => x.Name).Returns("the guard");
		f.Clan.SetupProperty(x => x.ShowFormsOfAddressInLook, true);
		f.World.Setup(x => x.GetStaticString("ClanFormOfAddressLookFormat")).Returns("Courtesy in {clan}: address {objective} as {address}.");
		f.Subject.Setup(x => x.ApparentGender(f.Speaker.Object)).Returns(Gendering.Get(Gender.Male));
		Assert.AreEqual("", ClanAddressResolver.LookHint(f.Speaker.Object, f.Subject.Object, f.Clan.Object));
		f.World.Setup(x => x.GetStaticBool("ClanSimilaritiesShowInLook")).Returns(true);
		var hint = ClanAddressResolver.LookHint(f.Speaker.Object, f.Subject.Object, f.Clan.Object);
		StringAssert.Contains(hint, "Courtesy in"); StringAssert.Contains(hint, "address him as"); StringAssert.Contains(hint, "Sir");
		f.Clan.Object.ShowFormsOfAddressInLook = false;
		Assert.AreEqual("", ClanAddressResolver.LookHint(f.Speaker.Object, f.Subject.Object, f.Clan.Object));
		f.Clan.Object.ShowFormsOfAddressInLook = true;
		f.SpeakerMember.Object.IsArchivedMembership = true;
		Assert.AreEqual("", ClanAddressResolver.LookHint(f.Speaker.Object, f.Subject.Object, f.Clan.Object));
		f.SpeakerMember.Object.IsArchivedMembership = false;
		f.Subject.Setup(x => x.IdentityIsObscuredTo(f.Speaker.Object)).Returns(true);
		Assert.AreEqual("", ClanAddressResolver.LookHint(f.Speaker.Object, f.Subject.Object, f.Clan.Object));
	}

	[TestMethod]
	public void Builder_OneDefaultExceptionsSuppressionAndClear_UseExistingPermissions()
	{
		var f = new Fixture();
		f.Speaker.Setup(x => x.IsAdministrator(It.IsAny<MudSharp.Accounts.PermissionLevel>())).Returns(true);
		f.Clan.SetupGet(x => x.Ranks).Returns([f.Rank.Object]);
		void Edit(string text) => typeof(ClanModule).GetMethod("ClanEditRank", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [f.Speaker.Object, new StringStack("Captain address " + text), f.Clan.Object]);
		Edit("\"Captain\""); Edit("add \"Sir\" from lower public");
		Assert.HasCount(2, f.Rules); Assert.AreEqual("Sir", f.Resolve().Text);
		Edit("add none from higher"); f.SpeakerRank.Object.RankNumber = 9;
		Assert.AreEqual(ClanAddressStatus.Suppressed, f.Resolve().Status);
		Edit("clear"); Assert.IsEmpty(f.Rules);
		f.Speaker.Setup(x => x.IsAdministrator(It.IsAny<MudSharp.Accounts.PermissionLevel>())).Returns(false);
		f.SpeakerMember.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.None);
		Edit("\"Forbidden\""); Assert.IsEmpty(f.Rules);
	}

	private static MudSharp.FutureProg.FutureProg Compile(Fixture f, string name, string text, ProgVariableTypes result,
		params (string Name, ProgVariableTypes Type)[] parameters)
	{
		MudSharp.FutureProg.FutureProg.Initialise();
		var row = new MudSharp.Models.FutureProg { Id = 1000 + f.Progs.Count(), FunctionName = name, FunctionText = text, ReturnType = (long)result };
		foreach (var (parameter, index) in parameters.Select((x, i) => (x, i)))
			row.FutureProgsParameters.Add(new MudSharp.Models.FutureProgsParameter { ParameterName = parameter.Name, ParameterType = (long)parameter.Type, ParameterIndex = index });
		var prog = new MudSharp.FutureProg.FutureProg(row, f.World.Object);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		f.Progs.Add(prog);
		return prog;
	}
}
