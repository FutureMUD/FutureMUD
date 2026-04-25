#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicPhase2Tests
{
	[TestInitialize]
	public void TestInitialize()
	{
		SpellTriggerFactory.SetupFactory();
		SpellEffectFactory.SetupFactory();
	}

	[TestMethod]
	public void SpellTriggerFactory_RegistersPhase2TriggerTypes()
	{
		Assert.AreEqual("exit", SpellTriggerFactory.BuilderInfoForType("exit").TargetTypes);
		Assert.AreEqual("character&exit", SpellTriggerFactory.BuilderInfoForType("characterexit").TargetTypes);
		Assert.AreEqual("character", SpellTriggerFactory.BuilderInfoForType("progcharacter").TargetTypes);
		Assert.AreEqual("item", SpellTriggerFactory.BuilderInfoForType("progitem").TargetTypes);
		Assert.AreEqual("character&room", SpellTriggerFactory.BuilderInfoForType("progcharacterroom").TargetTypes);
		Assert.AreEqual("item&room", SpellTriggerFactory.BuilderInfoForType("progitemroom").TargetTypes);
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersPhase2EffectTypes()
	{
		(string _, string _, bool instant, bool requiresTarget, string[] matchingTriggers) =
			SpellEffectFactory.BuilderInfoForType("forcedexitmovement");
		Assert.IsTrue(instant);
		Assert.IsTrue(requiresTarget);
		CollectionAssert.Contains(matchingTriggers, "characterexit");

		Assert.IsTrue(SpellEffectFactory.BuilderInfoForType("exitbarrier").RequiresTarget);
		Assert.IsTrue(SpellEffectFactory.BuilderInfoForType("roomward").RequiresTarget);
		Assert.IsTrue(SpellEffectFactory.BuilderInfoForType("personalward").RequiresTarget);
	}

	[TestMethod]
	public void CastingTriggerExit_DoTriggerCast_ResolvesLocalExitAndSuppliesExitParameter()
	{
		Mock<ICharacter> actor = CreateActor();
		Mock<ICellExit> exit = CreateExit("north");
		Mock<IExit> sharedExit = new();
		exit.SetupGet(x => x.Exit).Returns(sharedExit.Object);

		Mock<ICell> location = new();
		location.Setup(x => x.ExitsFor(actor.Object, It.IsAny<bool>())).Returns([exit.Object]);
		actor.SetupGet(x => x.Location).Returns(location.Object);

		Mock<IMagicSpell> spell = CreateSpellMock();
		SpellCastCapture capture = CaptureSpellCast(spell);
		(IMagicTrigger? trigger, string error) =
			SpellTriggerFactory.LoadTriggerFromBuilderInput("exit", new StringStack(string.Empty), spell.Object);

		Assert.AreEqual(string.Empty, error);

		((ICastMagicTrigger)trigger!).DoTriggerCast(actor.Object, new StringStack("insignificant north"));

		Assert.AreSame(sharedExit.Object, capture.Target);
		Assert.AreEqual(SpellPower.Insignificant, capture.Power);
		Assert.IsNotNull(capture.Parameters);
		Assert.AreEqual(1, capture.Parameters!.Length);
		Assert.AreEqual("exit", capture.Parameters[0].ParameterName);
		Assert.AreSame(exit.Object, capture.Parameters[0].Item);
	}

	[TestMethod]
	public void CastingTriggerCharacterExit_DoTriggerCast_ResolvesCharacterAndExit()
	{
		Mock<ICharacter> actor = CreateActor();
		Mock<ICharacter> targetCharacter = CreateActor();
		Mock<ICellExit> exit = CreateExit("north");

		Mock<ICell> location = new();
		location.Setup(x => x.ExitsFor(actor.Object, It.IsAny<bool>())).Returns([exit.Object]);
		actor.SetupGet(x => x.Location).Returns(location.Object);
		actor.Setup(x => x.TargetActorOrCorpse("bob")).Returns(targetCharacter.Object);

		Mock<IMagicSpell> spell = CreateSpellMock();
		SpellCastCapture capture = CaptureSpellCast(spell);
		(IMagicTrigger? trigger, string error) =
			SpellTriggerFactory.LoadTriggerFromBuilderInput("characterexit", new StringStack(string.Empty), spell.Object);

		Assert.AreEqual(string.Empty, error);

		((ICastMagicTrigger)trigger!).DoTriggerCast(actor.Object, new StringStack("insignificant bob north"));

		Assert.AreSame(targetCharacter.Object, capture.Target);
		Assert.AreEqual(SpellPower.Insignificant, capture.Power);
		Assert.IsNotNull(capture.Parameters);
		Assert.AreEqual("exit", capture.Parameters![0].ParameterName);
		Assert.AreSame(exit.Object, capture.Parameters[0].Item);
	}

	[TestMethod]
	public void CastingTriggerProgCharacterRoom_DoTriggerCast_UsesProgResolvedCharacterAndRoom()
	{
		Mock<ICharacter> actor = CreateActor();
		Mock<ICharacter> targetCharacter = CreateActor();
		Mock<ICell> room = new();
		Mock<IFutureProg> targetProg = CreateProgMock(1L, ProgVariableTypes.Character, targetCharacter.Object);
		Mock<IFutureProg> roomProg = CreateProgMock(2L, ProgVariableTypes.Location, room.Object);

		Mock<IMagicSpell> spell = CreateSpellMock(
			gameworld: CreateGameworld(
				CreateCollectionMock<IFutureProg>(targetProg.Object, roomProg.Object).Object,
				CreateCollectionMock<IMagicSchool>().Object).Object);
		SpellCastCapture capture = CaptureSpellCast(spell);
		IMagicTrigger trigger = SpellTriggerFactory.LoadTrigger(
			new XElement("Trigger",
				new XAttribute("type", "progcharacterroom"),
				new XElement("MinimumPower", (int)SpellPower.Insignificant),
				new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
				new XElement("TargetProg", 1L),
				new XElement("TargetRoomProg", 2L)
			), spell.Object);

		((ICastMagicTrigger)trigger).DoTriggerCast(actor.Object, new StringStack("insignificant remote target"));

		Assert.AreSame(targetCharacter.Object, capture.Target);
		Assert.AreEqual(SpellPower.Insignificant, capture.Power);
		Assert.IsNotNull(capture.Parameters);
		Assert.AreEqual("room", capture.Parameters![0].ParameterName);
		Assert.AreSame(room.Object, capture.Parameters[0].Item);
	}

	[TestMethod]
	public void CastingTriggerProgItemRoom_DoTriggerCast_AllowsNullTargetAndStillSuppliesRoom()
	{
		Mock<ICharacter> actor = CreateActor();
		Mock<ICell> room = new();
		Mock<IFutureProg> targetProg = CreateProgMock(1L, ProgVariableTypes.Item, null);
		Mock<IFutureProg> roomProg = CreateProgMock(2L, ProgVariableTypes.Location, room.Object);

		Mock<IMagicSpell> spell = CreateSpellMock(
			gameworld: CreateGameworld(
				CreateCollectionMock<IFutureProg>(targetProg.Object, roomProg.Object).Object,
				CreateCollectionMock<IMagicSchool>().Object).Object);
		SpellCastCapture capture = CaptureSpellCast(spell);
		IMagicTrigger trigger = SpellTriggerFactory.LoadTrigger(
			new XElement("Trigger",
				new XAttribute("type", "progitemroom"),
				new XElement("MinimumPower", (int)SpellPower.Insignificant),
				new XElement("MaximumPower", (int)SpellPower.RecklesslyPowerful),
				new XElement("TargetProg", 1L),
				new XElement("TargetRoomProg", 2L)
			), spell.Object);

		((ICastMagicTrigger)trigger).DoTriggerCast(actor.Object, new StringStack("insignificant summon"));

		Assert.IsNull(capture.Target);
		Assert.AreEqual(SpellPower.Insignificant, capture.Power);
		Assert.IsNotNull(capture.Parameters);
		Assert.AreEqual("room", capture.Parameters![0].ParameterName);
		Assert.AreSame(room.Object, capture.Parameters[0].Item);
	}

	[TestMethod]
	public void ForcedExitMovementEffect_GetOrApplyEffect_MovesTargetWhenMovementIsLegal()
	{
		Mock<IMagicSpell> spell = CreateSpellMock();
		(IMagicSpellEffectTemplate? effect, string error) =
			SpellEffectFactory.LoadEffectFromBuilderInput("forcedexitmovement", new StringStack(string.Empty), spell.Object);
		Assert.AreEqual(string.Empty, error);

		Mock<ICharacter> target = CreateActor();
		Mock<ICellExit> exit = CreateExit("north");
		target.Setup(x => x.CanMove(exit.Object,
			CanMoveFlags.IgnoreWhetherExitCanBeCrossed |
			CanMoveFlags.IgnoreCancellableActionBlockers |
			CanMoveFlags.IgnoreSafeMovement))
		      .Returns(new CanMoveResponse
		      {
			      Result = true,
			      ErrorMessage = string.Empty
		      });
		target.Setup(x => x.CanCross(exit.Object))
		      .Returns((true, null!));
		target.Setup(x => x.Move(exit.Object, null, true)).Returns(true);

		effect!.GetOrApplyEffect(
			CreateActor().Object,
			target.Object,
			OpposedOutcomeDegree.None,
			SpellPower.Insignificant,
			new Mock<IMagicSpellEffectParent>().Object,
			[new SpellAdditionalParameter { ParameterName = "exit", Item = exit.Object }]
		);

		target.Verify(x => x.Move(exit.Object, null, true), Times.Once);
	}

	[TestMethod]
	public void ForcedExitMovementEffect_GetOrApplyEffect_StopsWhenExitCannotBeCrossed()
	{
		Mock<IMagicSpell> spell = CreateSpellMock();
		(IMagicSpellEffectTemplate? effect, string error) =
			SpellEffectFactory.LoadEffectFromBuilderInput("forcedexitmovement", new StringStack(string.Empty), spell.Object);
		Assert.AreEqual(string.Empty, error);

		Mock<ICharacter> target = CreateActor();
		Mock<ICellExit> exit = CreateExit("north");
		target.Setup(x => x.CanMove(exit.Object,
			CanMoveFlags.IgnoreWhetherExitCanBeCrossed |
			CanMoveFlags.IgnoreCancellableActionBlockers |
			CanMoveFlags.IgnoreSafeMovement))
		      .Returns(new CanMoveResponse
		      {
			      Result = true,
			      ErrorMessage = string.Empty
		      });
		target.Setup(x => x.CanCross(exit.Object))
		      .Returns((false, null!));

		effect!.GetOrApplyEffect(
			CreateActor().Object,
			target.Object,
			OpposedOutcomeDegree.None,
			SpellPower.Insignificant,
			new Mock<IMagicSpellEffectParent>().Object,
			[new SpellAdditionalParameter { ParameterName = "exit", Item = exit.Object }]
		);

		target.Verify(x => x.Move(It.IsAny<ICellExit>(), It.IsAny<IEmote>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void MagicInterdictionHelper_UsesAdditionalRoomIncomingWard()
	{
		Mock<IMagicSchool> school = CreateSchool();
		Mock<ICharacter> source = CreateActor();
		Mock<IPerceivable> target = CreatePerceivable(null);
		Mock<ICell> room = CreateCell();
		Mock<IMagicInterdictionEffect> ward = CreateInterdictionEffect(MagicInterdictionCoverage.Incoming,
			MagicInterdictionMode.Fail, true);

		room.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		    .Returns([ward.Object]);

		MagicInterdictionResult? result = MagicInterdictionHelper.GetInterdiction(
			source.Object,
			target.Object,
			school.Object,
			true,
			new SpellAdditionalParameter { ParameterName = "room", Item = room.Object });

		Assert.IsNotNull(result);
		Assert.AreSame(room.Object, result.Owner);
		Assert.AreEqual(MagicInterdictionMode.Fail, result.Mode);
	}

	[TestMethod]
	public void MagicInterdictionHelper_DowngradesReflectionWhenNotAllowed()
	{
		Mock<IMagicSchool> school = CreateSchool();
		Mock<ICharacter> source = CreateActor();
		Mock<ICharacter> target = CreateActor();
		Mock<IMagicInterdictionEffect> ward = CreateInterdictionEffect(MagicInterdictionCoverage.Incoming,
			MagicInterdictionMode.Reflect, true);

		target.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		      .Returns([ward.Object]);

		MagicInterdictionResult? result = MagicInterdictionHelper.GetInterdiction(
			source.Object,
			target.Object,
			school.Object,
			false);

		Assert.IsNotNull(result);
		Assert.AreSame(target.Object, result.Owner);
		Assert.AreEqual(MagicInterdictionMode.Fail, result.Mode);
	}

	[TestMethod]
	public void MagicInterdictionHelper_BothCoverageAppliesToOutgoingMagic()
	{
		Mock<IMagicSchool> school = CreateSchool();
		Mock<ICharacter> source = CreateActor();
		Mock<IPerceivable> target = CreatePerceivable(null);
		Mock<IMagicInterdictionEffect> ward = CreateInterdictionEffect(MagicInterdictionCoverage.Both,
			MagicInterdictionMode.Fail, true);

		source.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		      .Returns([ward.Object]);

		MagicInterdictionResult? result = MagicInterdictionHelper.GetInterdiction(
			source.Object,
			target.Object,
			school.Object,
			true);

		Assert.IsNotNull(result);
		Assert.AreSame(source.Object, result.Owner);
	}

	[TestMethod]
	public void SpellPersonalWardEffect_ShouldInterdictChildSchoolWhenProgAllows()
	{
		Mock<ICharacter> source = CreateActor();
		Mock<IPerceivable> owner = CreatePerceivable(null);
		Mock<IMagicSchool> parentSchool = CreateSchool(1L, "Mind");
		Mock<IMagicSchool> childSchool = CreateSchool(2L, "Telepathy");
		childSchool.Setup(x => x.IsChildSchool(parentSchool.Object)).Returns(true);

		Mock<IFutureProg> prog = CreateProgMock(10L, ProgVariableTypes.Boolean, true);
		ProgVariableTypes[] expectedParameters =
		[
			ProgVariableTypes.Character,
			ProgVariableTypes.Perceivable,
			ProgVariableTypes.MagicSchool
		];
		prog.Setup(x => x.MatchesParameters(It.Is<IEnumerable<ProgVariableTypes>>(parameters =>
				parameters.SequenceEqual(expectedParameters))))
		    .Returns(true);
		prog.Setup(x => x.ExecuteBool(source.Object, owner.Object, childSchool.Object)).Returns(true);

		SpellPersonalWardEffect effect = new(owner.Object, new Mock<IMagicSpellEffectParent>().Object,
			parentSchool.Object, MagicInterdictionMode.Fail, MagicInterdictionCoverage.Both, true, prog.Object);

		Assert.IsTrue(effect.ShouldInterdict(source.Object, childSchool.Object));
	}

	private static Mock<ICharacter> CreateActor()
	{
		Mock<IOutputHandler> output = new();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		output.Setup(x => x.Send(It.IsAny<IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);

		Mock<ICharacter> actor = new();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.HasDubFor(It.IsAny<IKeyworded>(), It.IsAny<string>())).Returns(false);
		actor.Setup(x => x.HasDubFor(It.IsAny<IKeyworded>(), It.IsAny<IEnumerable<string>>())).Returns(false);
		actor.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		     .Returns([]);
		actor.SetupGet(x => x.Location).Returns((ICell)null!);
		return actor;
	}

	private static Mock<IPerceivable> CreatePerceivable(ICell? location)
	{
		Mock<IPerceivable> perceivable = new();
		perceivable.SetupGet(x => x.Location).Returns(location!);
		perceivable.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		          .Returns([]);
		return perceivable;
	}

	private static Mock<ICell> CreateCell()
	{
		Mock<ICell> cell = new();
		cell.Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>()))
		    .Returns([]);
		return cell;
	}

	private static Mock<ICellExit> CreateExit(params string[] keywords)
	{
		Mock<ICellExit> exit = new();
		exit.SetupGet(x => x.Keywords).Returns(keywords);
		exit.Setup(x => x.GetKeywordsFor(It.IsAny<IPerceiver>())).Returns(keywords);
		exit.Setup(x => x.HasKeyword(It.IsAny<string>(), It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>()))
		    .Returns<string, IPerceiver, bool, bool>((keyword, _, abbreviated, useContains) =>
			    keywords.Any(candidate =>
				    abbreviated
					    ? (useContains
						    ? candidate.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
						    : candidate.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase))
					    : candidate.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)));
		return exit;
	}

	private static Mock<IMagicSchool> CreateSchool(long id = 1L, string name = "Magic")
	{
		Mock<IMagicSchool> school = new();
		school.SetupGet(x => x.Id).Returns(id);
		school.SetupGet(x => x.Name).Returns(name);
		school.SetupGet(x => x.SchoolVerb).Returns("cast");
		school.SetupGet(x => x.SchoolAdjective).Returns("magical");
		school.SetupGet(x => x.PowerListColour).Returns(Telnet.Green);
		school.Setup(x => x.IsChildSchool(It.IsAny<IMagicSchool>())).Returns(false);
		return school;
	}

	private static Mock<IFutureProg> CreateProgMock(long id, ProgVariableTypes returnType, object? result)
	{
		Mock<IFutureProg> prog = new();
		prog.SetupGet(x => x.Id).Returns(id);
		prog.SetupGet(x => x.ReturnType).Returns(returnType);
		prog.SetupGet(x => x.NamedParameters).Returns([]);
		prog.SetupGet(x => x.Parameters).Returns([]);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(false);
		prog.Setup(x => x.Execute<IPerceivable?>(It.IsAny<object[]>())).Returns(result as IPerceivable);
		prog.Setup(x => x.Execute<ICell?>(It.IsAny<object[]>())).Returns(result as ICell);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(result as bool? ?? false);
		prog.Setup(x => x.ExecuteBool(It.IsAny<bool>(), It.IsAny<object[]>())).Returns(result as bool? ?? false);
		return prog;
	}

	private static Mock<IUneditableAll<T>> CreateCollectionMock<T>(params T[] items) where T : class, IFrameworkItem
	{
		Dictionary<long, T> byId = items.ToDictionary(x => x.Id, x => x);
		Mock<IUneditableAll<T>> collection = new();
		collection.SetupGet(x => x.Count).Returns(items.Length);
		collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => byId.GetValueOrDefault(id));
		collection.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>())).Returns((T?)null);
		collection.Setup(x => x.GetEnumerator()).Returns(((IEnumerable<T>)items).GetEnumerator());
		return collection;
	}

	private static Mock<IFuturemud> CreateGameworld(IUneditableAll<IFutureProg> futureProgs,
		IUneditableAll<IMagicSchool> magicSchools)
	{
		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs);
		gameworld.SetupGet(x => x.MagicSchools).Returns(magicSchools);
		return gameworld;
	}

	private static Mock<IMagicSpell> CreateSpellMock(IFuturemud? gameworld = null, IMagicSchool? school = null)
	{
		Mock<IMagicSpell> spell = new();
		spell.SetupGet(x => x.Gameworld).Returns(gameworld ?? CreateGameworld(
			CreateCollectionMock<IFutureProg>().Object,
			CreateCollectionMock<IMagicSchool>().Object).Object);
		spell.SetupGet(x => x.School).Returns(school ?? CreateSchool().Object);
		spell.SetupProperty(x => x.Changed);
		return spell;
	}

	private static SpellCastCapture CaptureSpellCast(
		Mock<IMagicSpell> spell)
	{
		SpellCastCapture capture = new();
		spell.Setup(x => x.CastSpell(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(), It.IsAny<SpellPower>(),
				It.IsAny<SpellAdditionalParameter[]>()))
		    .Callback<ICharacter, IPerceivable, SpellPower, SpellAdditionalParameter[]>((_, t, p, extra) =>
		    {
			    capture.Target = t;
			    capture.Power = p;
			    capture.Parameters = extra;
		    });
		return capture;
	}

	private static Mock<IMagicInterdictionEffect> CreateInterdictionEffect(MagicInterdictionCoverage coverage,
		MagicInterdictionMode mode, bool shouldInterdict)
	{
		Mock<IMagicInterdictionEffect> effect = new();
		effect.SetupGet(x => x.Coverage).Returns(coverage);
		effect.SetupGet(x => x.Mode).Returns(mode);
		effect.Setup(x => x.ShouldInterdict(It.IsAny<ICharacter>(), It.IsAny<IMagicSchool>())).Returns(shouldInterdict);
		return effect;
	}

	private sealed class SpellCastCapture
	{
		public IPerceivable? Target { get; set; }
		public SpellPower? Power { get; set; }
		public SpellAdditionalParameter[]? Parameters { get; set; }
	}
}
