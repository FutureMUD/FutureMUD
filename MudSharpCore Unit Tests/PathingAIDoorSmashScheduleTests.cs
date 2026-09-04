#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PathingAIDoorSmashScheduleTests
{
    private sealed class TestPathingAI : PathingAIBase
    {
        public TestPathingAI()
        {
        }

        public TestPathingAI(IFuturemud gameworld) : base(gameworld, "test", "test")
        {
        }

        public DateTime Clock { get; set; }
        public int SmashCount { get; private set; }
        public bool Enabled { get; set; } = true;

        public void SetDelayProg(IFutureProg? prog) => DoorSmashDelayProg = prog;
        public bool RunCheckSmash(ICharacter character) => CheckSmash(character);
        public string SavedDefinition => PrepareDefinitionForSave(SaveToXml());

        protected override DateTime UtcNow => Clock;
        protected override bool Smash(ICharacter ch, ICellExit exit)
        {
            SmashCount++;
            return true;
        }

        protected override bool IsPathingEnabled(ICharacter character) => Enabled;
        protected override (ICell? Target, IEnumerable<ICellExit>) GetPath(ICharacter ch) =>
            (null, Enumerable.Empty<ICellExit>());
        protected override string SaveToXml() => "<Definition><OpenDoors>false</OpenDoors></Definition>";
    }

    private sealed record SmashFixture(
        Mock<ICharacter> Character,
        Mock<ICell> Origin,
        Mock<ICellExit> CellExit,
        BreakDownDoor Focus,
        List<IEffect> Effects);

    private static SmashFixture CreateFixture()
    {
        var origin = new Mock<ICell>();
        var door = new Mock<IDoor>();
        door.SetupGet(x => x.IsOpen).Returns(false);
        var exit = new Mock<IExit>();
        exit.SetupGet(x => x.Door).Returns(door.Object);
        var cellExit = new Mock<ICellExit>();
        cellExit.SetupGet(x => x.Origin).Returns(origin.Object);
        cellExit.SetupGet(x => x.Exit).Returns(exit.Object);

        var character = new Mock<ICharacter>();
        character.SetupGet(x => x.Location).Returns(origin.Object);
        var focus = new BreakDownDoor(character.Object, cellExit.Object);
        var effects = new List<IEffect> { focus };
        character.SetupGet(x => x.Effects).Returns(effects);
        character.Setup(x => x.AffectedBy<BreakDownDoor>()).Returns(() => effects.Contains(focus));
        character.Setup(x => x.EffectsOfType<BreakDownDoor>(It.IsAny<Predicate<BreakDownDoor>>()))
            .Returns((Predicate<BreakDownDoor>? predicate) =>
                predicate is null || predicate(focus) ? new[] { focus } : Array.Empty<BreakDownDoor>());
        character.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()))
            .Callback((Predicate<IEffect> predicate, bool _) => effects.RemoveAll(predicate));
        return new SmashFixture(character, origin, cellExit, focus, effects);
    }

    private static Mock<IFutureProg> DelayProg(long id, Func<ICharacter, ICellExit, decimal> delay)
    {
        var prog = new Mock<IFutureProg>();
        prog.SetupGet(x => x.Id).Returns(id);
        prog.SetupGet(x => x.Name).Returns($"Delay {id}");
        prog.Setup(x => x.ExecuteDecimal(It.IsAny<object[]>()))
            .Returns((object[] args) => delay((ICharacter)args[0], (ICellExit)args[1]));
        return prog;
    }

    [TestMethod]
    public void ConfiguredCallback_DelaysFirstAttemptAndReschedulesAfterDueAttempt()
    {
        var fixture = CreateFixture();
        var prog = DelayProg(77, (_, _) => 60_000M);
        var ai = new TestPathingAI { Clock = new DateTime(2026, 8, 22, 0, 0, 0, DateTimeKind.Utc) };
        ai.SetDelayProg(prog.Object);

        Assert.IsTrue(ai.RunCheckSmash(fixture.Character.Object));
        Assert.AreEqual(0, ai.SmashCount);
        Assert.AreEqual(ai.Clock.AddMinutes(1), fixture.Focus.NextSmashAttemptUtc);

        ai.Clock = ai.Clock.AddSeconds(59);
        ai.RunCheckSmash(fixture.Character.Object);
        Assert.AreEqual(0, ai.SmashCount);

        ai.Clock = ai.Clock.AddSeconds(1);
        ai.RunCheckSmash(fixture.Character.Object);
        Assert.AreEqual(1, ai.SmashCount);
        Assert.AreEqual(ai.Clock.AddMinutes(1), fixture.Focus.NextSmashAttemptUtc);
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Exactly(2));
    }

    [TestMethod]
    public void ConfiguredCallback_KeepsIndependentDeadlinesForEachCharacter()
    {
        var first = CreateFixture();
        var second = CreateFixture();
        var prog = DelayProg(77, (character, _) => ReferenceEquals(character, first.Character.Object) ? 10_000M : 20_000M);
        var ai = new TestPathingAI { Clock = new DateTime(2026, 8, 22, 0, 0, 0, DateTimeKind.Utc) };
        ai.SetDelayProg(prog.Object);

        ai.RunCheckSmash(first.Character.Object);
        ai.RunCheckSmash(second.Character.Object);

        Assert.AreEqual(ai.Clock.AddSeconds(10), first.Focus.NextSmashAttemptUtc);
        Assert.AreEqual(ai.Clock.AddSeconds(20), second.Focus.NextSmashAttemptUtc);
        Assert.AreNotEqual(first.Focus.NextSmashAttemptUtc, second.Focus.NextSmashAttemptUtc);
    }

    [TestMethod]
    public void ReconstructedFocus_InvokesCallbackBeforeAnyAttempt()
    {
        var fixture = CreateFixture();
        Assert.IsNull(fixture.Focus.NextSmashAttemptUtc);
        var prog = DelayProg(77, (_, _) => 30_000M);
        var ai = new TestPathingAI { Clock = DateTime.UtcNow };
        ai.SetDelayProg(prog.Object);

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(0, ai.SmashCount);
        Assert.IsNotNull(fixture.Focus.NextSmashAttemptUtc);
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Once);
    }

    [TestMethod]
    public void InvalidOriginOrOpenDoor_RemovesFocusWithoutCallingCallbackOrSmashing()
    {
        var fixture = CreateFixture();
        fixture.Character.SetupGet(x => x.Location).Returns(new Mock<ICell>().Object);
        var prog = DelayProg(77, (_, _) => 1M);
        var ai = new TestPathingAI { Clock = DateTime.UtcNow };
        ai.SetDelayProg(prog.Object);

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(0, ai.SmashCount);
        Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Never);
    }

    [TestMethod]
    public void DisabledPathing_RemovesFocusWithoutCallingCallbackOrSmashing()
    {
        var fixture = CreateFixture();
        var prog = DelayProg(77, (_, _) => 1M);
        var ai = new TestPathingAI { Clock = DateTime.UtcNow, Enabled = false };
        ai.SetDelayProg(prog.Object);

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(0, ai.SmashCount);
        Assert.IsFalse(fixture.Effects.Contains(fixture.Focus));
        prog.Verify(x => x.ExecuteDecimal(It.IsAny<object[]>()), Times.Never);
    }

    [TestMethod]
    public void AbsentCallback_PreservesImmediateLegacyAttempt()
    {
        var fixture = CreateFixture();
        var ai = new TestPathingAI { Clock = DateTime.UtcNow };

        ai.RunCheckSmash(fixture.Character.Object);

        Assert.AreEqual(1, ai.SmashCount);
        Assert.IsNull(fixture.Focus.NextSmashAttemptUtc);
    }

    [TestMethod]
    public void DefinitionSave_WritesCanonicalOptionalCallbackElement()
    {
        var ai = new TestPathingAI();
        ai.SetDelayProg(DelayProg(77, (_, _) => 1M).Object);

        var definition = XElement.Parse(ai.SavedDefinition);

        Assert.AreEqual("77", definition.Element("DoorSmashDelayProg")?.Value);
        StringAssert.Contains(ai.HelpText, "Number(Character, Exit)");
        StringAssert.Contains(ai.HelpText, "milliseconds");
    }

    [TestMethod]
    public void Builder_AcceptsOnlyNumberCharacterExitCallback()
    {
        var valid = new Mock<IFutureProg>();
        valid.SetupGet(x => x.Public).Returns(true);
        valid.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
        valid.SetupGet(x => x.Parameters).Returns(new[] { ProgVariableTypes.Character, ProgVariableTypes.Exit });
        valid.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>()))
            .Returns<IEnumerable<ProgVariableTypes>>(x =>
                x.SequenceEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Exit }));

        var invalid = new Mock<IFutureProg>();
        invalid.SetupGet(x => x.Public).Returns(true);
        invalid.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
        invalid.SetupGet(x => x.Parameters).Returns(new[] { ProgVariableTypes.Character });
        invalid.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(false);

        var collection = new Mock<IUneditableAll<IFutureProg>>();
        collection.Setup(x => x.GetByIdOrName("valid", It.IsAny<bool>())).Returns(valid.Object);
        collection.Setup(x => x.GetByIdOrName("invalid", It.IsAny<bool>())).Returns(invalid.Object);
        var gameworld = new Mock<IFuturemud>();
        gameworld.SetupGet(x => x.FutureProgs).Returns(collection.Object);
        gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        var output = new Mock<IOutputHandler>();
        output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
        var actor = new Mock<ICharacter>();
        actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
        var ai = new TestPathingAI(gameworld.Object);

        Assert.IsTrue(ai.BuildingCommand(actor.Object, new StringStack("smashdelay valid")));
        Assert.AreSame(valid.Object, ai.DoorSmashDelayProg);
        Assert.IsFalse(ai.BuildingCommand(actor.Object, new StringStack("smashdelay invalid")));
        Assert.AreSame(valid.Object, ai.DoorSmashDelayProg);
        Assert.IsTrue(ai.BuildingCommand(actor.Object, new StringStack("smashdelay none")));
        Assert.IsNull(ai.DoorSmashDelayProg);
    }

    [TestMethod]
    public void PathToLocationLoadAndSave_RoundTripsCallbackWithoutChangingOwningDefinition()
    {
        var prog = DelayProg(77, (_, _) => 1M);
        var progs = new Mock<IUneditableAll<IFutureProg>>();
        progs.Setup(x => x.Get(It.IsAny<long>())).Returns((long id) => id == 77 ? prog.Object : null!);
        var gameworld = new Mock<IFuturemud>();
        gameworld.SetupGet(x => x.FutureProgs).Returns(progs.Object);
        var model = new ArtificialIntelligence
        {
            Id = 1,
            Name = "pathing",
            Type = "PathToLocation",
            Definition = "<Definition><PathingEnabledProg>0</PathingEnabledProg><OnStartToPathProg>0</OnStartToPathProg><TargetLocationProg>0</TargetLocationProg><FallbackLocationProg>0</FallbackLocationProg><WayPointsProg>0</WayPointsProg><OpenDoors>false</OpenDoors><UseKeys>false</UseKeys><SmashLockedDoors>true</SmashLockedDoors><CloseDoorsBehind>false</CloseDoorsBehind><UseDoorguards>false</UseDoorguards><MoveEvenIfObstructionInWay>false</MoveEvenIfObstructionInWay><DoorSmashDelayProg>77</DoorSmashDelayProg></Definition>"
        };
        var constructor = typeof(PathToLocationAI).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
            null, new[] { typeof(ArtificialIntelligence), typeof(IFuturemud) }, null);
        Assert.IsNotNull(constructor);
        var ai = (PathToLocationAI)constructor.Invoke(new object[] { model, gameworld.Object });
        var prepare = typeof(ArtificialIntelligenceBase).GetMethod("DefinitionForSave", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(prepare);

        var saved = XElement.Parse((string)prepare.Invoke(ai, null)!);

        Assert.AreSame(prog.Object, ai.DoorSmashDelayProg);
        Assert.AreEqual("77", saved.Element("DoorSmashDelayProg")?.Value);
    }
}
