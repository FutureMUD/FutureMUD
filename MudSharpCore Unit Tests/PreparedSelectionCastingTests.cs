#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using Moq;
using System.Linq;
namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class PreparedSelectionCastingTests
{
    private sealed class State { public int Choices; public int Reuses; public int Mutations; public bool Allowed = true; public bool PartialFailure; }
    private sealed record Token(IPerceivable Recipient, int Choice) : IMagicSpellEffectPreparedSelectionToken;
    private sealed class Template(State state) : IMagicSpellEffectTemplate, IMagicSpellEffectPreparedSelection, IMagicSpellEffectAdmission
    {
        private Token? _choice;
        public IMagicSpellEffectPreparedSelectionToken? CapturePreparedSelection(ICharacter caster, IPerceivable recipient)
        { return _choice ??= new(recipient, ++state.Choices); }
        public bool TryReusePreparedSelection(IMagicSpellEffectPreparedSelectionToken selection, ICharacter caster, IPerceivable recipient, out string? error)
        {
            ++state.Reuses; error = "Live fixture admission changed";
            if (!state.Allowed || selection is not Token token || !ReferenceEquals(token.Recipient, recipient)) return false;
            _choice = token; error = null; return true;
        }
        public bool IsInstantaneous => true; public bool RequiresTarget => true;
        public bool IsCompatibleWithTrigger(IMagicTrigger _) => true;
        public XElement SaveToXml() => new("Effect", new XAttribute("type", "preparedselectiontest"));
        public IMagicSpellEffectTemplate Clone() => new Template(state);
        public bool BuildingCommand(ICharacter _, StringStack __) => false;
        public string Show(ICharacter _) => "Prepared choice fixture";
        public IMagicSpellEffect GetOrApplyEffect(ICharacter _, IPerceivable? __, OpposedOutcomeDegree ___, SpellPower ____, IMagicSpellEffectParent _____, SpellAdditionalParameter[] ______) => throw new AssertFailedException();
        public bool TryPrepareApplication(ICharacter _, IPerceivable __, OpposedOutcomeDegree ___, SpellPower ____, TimeSpan _____, out IMagicSpellEffectApplication? application, out string? error)
        { application = new Application(state, _choice!.Choice); error = null; return true; }
    }
    private sealed record Application(State State, int Choice) : IMagicSpellEffectApplicationOperation
    {
        public IMagicSpellEffect Create(IMagicSpellEffectParent _) => throw new AssertFailedException();
        public MagicEffectOperation Apply(IMagicSpellEffectParent _)
        {
            Assert.AreEqual(1, Choice); ++State.Mutations;
            if (State.PartialFailure) throw new InvalidOperationException("One output already created");
            return new(MagicEffectOperationStatus.Applied, null);
        }
    }
    [TestCleanup]
    public void RemoveFactory() => ((IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>>)
        typeof(SpellEffectFactory).GetField("_loadTimeFactories", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!).Remove("preparedselectiontest");
    [DataTestMethod, DataRow("success"), DataRow("drift"), DataRow("partial")]
    public void FullCast_ReusesFirstChoiceAcrossBothPreparations_AndPreservesPaidUncertainty(string scenario)
    {
        var state = new State { PartialFailure = scenario == "partial" };
        SpellEffectFactory.RegisterLoadTimeFactory("preparedselectiontest", (_, _) => new Template(state));
        var f = new MagicCastingFixture();
        var spell = f.NewSpell(2, "Selection Fixture", "<Effect type='preparedselectiontest'/>");
        Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
        Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 2")));
        f.Store.Write(acquired: new(100, 2, 7, 1, f.Now, "fixture", DateTime.UnixEpoch, 0));
        f.Checkpoint = stage => { if (stage == "BeforePayment" && scenario == "drift") state.Allowed = false; };
        var intent = new MagicCastingIntent(f.Actor.Object, f.Earth.Id, 2, 1, false, "self", OriginId: Guid.NewGuid());
        var result = f.Service.Cast(intent);
        Assert.AreEqual(1, state.Choices); Assert.AreEqual(2, state.Reuses);
        Assert.AreEqual(scenario == "success" ? MagicCastingStatus.Succeeded : scenario == "drift" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview, result.Status, result.Message);
        Assert.AreEqual(scenario == "drift" ? 100.0 : 95.0, f.Balances[f.Resources[1]]);
        Assert.AreEqual(scenario == "drift" ? 0 : 1, state.Mutations);
        if (scenario == "drift") { Assert.IsNull(result.OperationId); return; }
        Assert.AreEqual(scenario == "partial" ? "NeedsReview" : "Completed", f.Store.Operation(result.OperationId!.Value)!.Stage);
        Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(intent).Status);
        Assert.AreEqual(1, state.Choices); Assert.AreEqual(2, state.Reuses); Assert.AreEqual(1, state.Mutations);
        if (scenario == "partial") {
            Assert.IsTrue(f.Service.ReconcileOperation(f.Staff.Object, f.Actor.Object, result.OperationId.Value, "Audited partial fixture").Allowed);
            Assert.AreEqual(95.0, f.Balances[f.Resources[1]]); Assert.AreEqual(1, state.Mutations);
        }
    }
    [DataTestMethod, DataRow("count"), DataRow("side"), DataRow("type"), DataRow("configuration"), DataRow("index")]
    public void SelectionPairing_RefusesChangedEffectStructure(string change)
    {
        var f = new MagicCastingFixture();
        var original = f.Spell.CastingCopy(f.Actor.Object, f.Traits[0], 1, SpellPower.ExtremelyWeak, Difficulty.Easy, 1);
        var fresh = f.Spell.CastingCopy(f.Actor.Object, f.Traits[0], 1, SpellPower.ExtremelyWeak, Difficulty.Easy, 1);
        var effects = (List<IMagicSpellEffectTemplate>)fresh.SpellEffects;
        if (change == "count") effects.Clear();
        if (change == "side") { ((List<IMagicSpellEffectTemplate>)fresh.CasterSpellEffects).Add(effects[0]); effects.Clear(); }
        if (change == "type") effects[0] = Mock.Of<IMagicSpellEffectTemplate>();
        if (change == "configuration") Assert.IsTrue(effects[0].BuildingCommand(f.Actor.Object, new StringStack("bonus 5")));
        if (change == "index") {
            var sourceEffects = (List<IMagicSpellEffectTemplate>)original.SpellEffects;
            sourceEffects.Add(sourceEffects[0].Clone()); Assert.IsTrue(sourceEffects[1].BuildingCommand(f.Actor.Object, new StringStack("bonus 5")));
            effects.Add(sourceEffects[1].Clone()); effects.Reverse();
        }
        var method = typeof(MagicCastingService).GetMethod("ReusePreparedSelections", BindingFlags.Static | BindingFlags.NonPublic)!;
        var error = Assert.ThrowsException<TargetInvocationException>(() => method.Invoke(null, [fresh, original, f.Actor.Object, new[] { f.Actor.Object }.Cast<IPerceivable>()]));
        Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
    }
}
