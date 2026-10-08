#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Resources;
using System.Linq;
namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class ProvisionFoodAdmissionCastingTests
{
    private sealed class CountingRandom : Random { public int Draws; public override int Next(int maxValue) { ++Draws; return 0; } }
    private sealed class FoodEffect(XElement root, IMagicSpell spell, CountingRandom random) : CreateItemEffect(root, spell)
    { protected override Random FoodProfileRandom => random; }
    private Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>? _factory;
    private static IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>> Factories =>
        (IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>>)typeof(SpellEffectFactory).GetField("_loadTimeFactories", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
    [TestCleanup] public void RestoreFactory() { if (_factory is not null) Factories["createitem"] = _factory; }
    private static GameItemProto NativeFood(IFuturemud world)
    {
        var food = (FoodGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(FoodGameItemComponentProto));
        typeof(FoodGameItemComponentProto).GetProperty(nameof(food.Bites))!.SetValue(food, 4.0);
        typeof(FoodGameItemComponentProto).GetProperty(nameof(food.Decorator))!.SetValue(food, Mock.Of<IStackDecorator>());
        var hold = (HoldableGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(HoldableGameItemComponentProto));
        var proto = (GameItemProto)RuntimeHelpers.GetUninitializedObject(typeof(GameItemProto));
        typeof(GameItemProto).GetProperty(nameof(proto.Gameworld))!.SetValue(proto, world);
        typeof(GameItemProto).GetProperty(nameof(proto.Status))!.SetValue(proto, RevisionStatus.Current);
        typeof(GameItemProto).GetProperty(nameof(proto.BaseItemQuality))!.SetValue(proto, ItemQuality.Standard);
        typeof(FrameworkItem).GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(proto, 12L);
        typeof(GameItemProto).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(proto, new List<IGameItemComponentProto> { hold, food });
        proto.OnLoadProgs = []; return proto;
    }
    [DataTestMethod, DataRow("eligibility", false), DataRow("eligibility", true), DataRow("eligibility-prepared-lifetime", false), DataRow("lifetime", false)]
    public void ActualFoodAdapter_FinalPreparationCallbackDrift_RefusesBeforePaymentWithoutRedraw(string callback, bool permanent)
    {
        var f = new MagicCastingFixture(); var random = new CountingRandom();
        _factory = Factories["createitem"]; Factories["createitem"] = (root, spell) => {
            var effect = new FoodEffect(root, spell, random);
            // Exercise the existing cached-lifetime return, without a second lifetime callback.
            if (callback == "eligibility-prepared-lifetime") typeof(CreateItemEffect).GetField("_preparedLifetimeSeconds", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(effect, 9450.0);
            return effect;
        };
        var native = NativeFood(f.World.Object); var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
        catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns(native); f.World.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
        var owned = new Mock<ISpellOwnedItemService>(); f.World.SetupGet(x => x.SpellOwnedItems).Returns(owned.Object);
        f.World.SetupGet(x => x.DefaultHooks).Returns([]);
        var firstMatch = false; var callbackCalls = 0; var policyCalls = 0;
        var policy = new Mock<IFutureProg>(); policy.SetupGet(x => x.Id).Returns(72); policy.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        policy.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
        policy.Setup(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>())).Returns((out object result, object[] _) => { ++policyCalls; result = firstMatch; return true; });
        var admission = new Mock<IFutureProg>(); admission.SetupGet(x => x.Id).Returns(73);
        var eligibility = callback.StartsWith("eligibility");
        admission.SetupGet(x => x.ReturnType).Returns(eligibility ? ProgVariableTypes.Boolean : ProgVariableTypes.Number);
        admission.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
        void Mutate() { if (++callbackCalls != 3) return; if (eligibility) firstMatch = true; else f.Actor.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>()); }
        admission.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() => { Mutate(); return true; });
        admission.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Returns(() => { Mutate(); return 1.0; });
        var known = new Mock<IFutureProg>(); known.SetupGet(x => x.Id).Returns(1); known.Setup(x => x.Execute<bool?>(It.IsAny<object[]>())).Returns(true);
        f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { known.Object, policy.Object, admission.Object }));
        var xml = ArmageddonSustainMealStock.Definition(10, 1, 12).Element("Effects")!.Element("Effect")!;
        var lifecycle = xml.Element("Lifecycle")!; if (permanent) lifecycle.Attribute("mode")!.Value = "Permanent";
        lifecycle.Add(new XElement(eligibility ? "EligibilityProg" : "LifetimeMultiplierProg", 73));
        lifecycle.Element("FoodProfiles")!.AddFirst(new XElement("Profile", new XAttribute("order", 1), new XAttribute("predicate", 72), new XElement("Prototype", 12)));
        var spell = f.NewSpell(2, "Actual Food Admission", xml.ToString());
        Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
        Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 2")));
        f.Store.Write(acquired: new(100, 2, 7, 1, f.Now, "fixture", DateTime.UnixEpoch, 0));
        var result = f.Service.Cast(new(f.Actor.Object, f.Earth.Id, 2, 7, false, "self"));
        Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
        Assert.AreEqual(3, callbackCalls); Assert.AreEqual(7, random.Draws);
        Assert.AreEqual(eligibility ? 3 : 2, policyCalls);
        Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.Balances[f.Resources[1]]);
        Assert.AreEqual(0, f.Store.Operations.Count);
        owned.Verify(x => x.Create(It.IsAny<IGameItemProto>(), It.IsAny<ICharacter>(), It.IsAny<ItemQuality>(), It.IsAny<SpellLifecycleOrigin>()), Times.Never);
    }

    [DataTestMethod, DataRow(false, "late-cap"), DataRow(false, "selection-balance"), DataRow(false, "selection-cap-definition"),
        DataRow(false, "selection-trait"), DataRow(false, "selection-location"), DataRow(false, "exact-debit"),
        DataRow(true, "late-cap"), DataRow(true, "selection-balance"), DataRow(true, "selection-cap-definition"),
        DataRow(true, "selection-trait"), DataRow(true, "selection-location"), DataRow(true, "exact-debit"),
        DataRow(true, "selection-device-custody"), DataRow(true, "selection-device-definition")]
    public void ActualFoodAdapter_FinalPaymentBoundary_RefusesDriftAndNeverReexecutesCapacityDuringDebit(bool focus, string scenario)
    {
        var random = new CountingRandom();
        _factory = Factories["createitem"]; Factories["createitem"] = (root, spell) => new FoodEffect(root, spell, random);
        var xml = ArmageddonSustainMealStock.Definition(10, 1, 12).Element("Effects")!.Element("Effect")!;
        xml.Element("Lifecycle")!.Add(new XElement("EligibilityProg", 73));
        xml.Element("Lifecycle")!.Element("FoodProfiles")!.AddFirst(new XElement("Profile", new XAttribute("order", 1), new XAttribute("predicate", 72), new XElement("Prototype", 12)));
        var device = focus ? new ChargedMagicDeviceTests.Fixture(xml.ToString()) : null;
        var f = device?.F ?? new MagicCastingFixture();
        if (device is not null) {
            device.Item.SetupGet(x => x.Components).Returns([device.Device]);
            f.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new[] { device.Item.Object }));
        }
        var native = NativeFood(f.World.Object); var catalogue = new Mock<IUneditableRevisableAll<IGameItemProto>>();
        catalogue.Setup(x => x.Get(It.IsAny<long>())).Returns(native); f.World.SetupGet(x => x.ItemProtos).Returns(catalogue.Object);
        var owned = new Mock<ISpellOwnedItemService>(); f.World.SetupGet(x => x.SpellOwnedItems).Returns(owned.Object);
        owned.Setup(x => x.Create(It.IsAny<IGameItemProto>(), It.IsAny<ICharacter>(), It.IsAny<ItemQuality>(), It.IsAny<SpellLifecycleOrigin>()))
            .Throws(new InvalidOperationException("Output fault after admitted debit"));
        f.World.SetupGet(x => x.DefaultHooks).Returns([]);
        var firstMatch = false; var eligibilityCalls = 0; var policyCalls = 0; var capCalls = 0; var debitCapCallbacks = 0; var paying = false;
        SimpleMagicResource reserve = null!;
        var policy = new Mock<IFutureProg>(); policy.SetupGet(x => x.Id).Returns(72); policy.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        policy.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
        policy.Setup(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>())).Returns((out object result, object[] _) => {
            if (++policyCalls == 4) {
                if (scenario == "selection-balance") f.Balances[reserve] = 80;
                if (scenario == "selection-cap-definition") reserve.ResourceCapProg = Mock.Of<IFutureProg>();
                if (scenario == "selection-trait") f.Skills[1] = 5;
                if (scenario == "selection-location") f.Actor.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
                if (scenario == "selection-device-custody") device!.Held.Clear();
                if (scenario == "selection-device-definition") Assert.IsTrue(device!.Proto.BuildingCommand(f.Actor.Object, new StringStack("capacity 6")));
            }
            result = firstMatch; return true;
        });
        var eligibility = new Mock<IFutureProg>(); eligibility.SetupGet(x => x.Id).Returns(73);
        eligibility.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        eligibility.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
        eligibility.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() => { ++eligibilityCalls; return true; });
        var cap = new Mock<IFutureProg>(); cap.SetupGet(x => x.Id).Returns(74);
        cap.Setup(x => x.ExecuteDouble(It.IsAny<double>(), It.IsAny<object[]>())).Returns(() => {
            ++capCalls;
            if (paying) { ++debitCapCallbacks; firstMatch = true; }
            if (eligibilityCalls == 3 && scenario == "late-cap") firstMatch = true;
            return 100.0;
        });
        var known = f.World.Object.FutureProgs.ToArray();
        f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => known.Concat([policy.Object, eligibility.Object, cap.Object])));
        reserve = new SimpleMagicResource(new MudSharp.Models.MagicResource { Id = 11, Name = "Food admission capacity", ShortName = "fac",
            Definition = "<Definition><ResourceCapProg>74</ResourceCapProg></Definition>" }, f.World.Object);
        f.Balances.Remove(f.Resources[1]); f.Resources[1] = reserve; f.Balances[reserve] = 100;
        f.Actor.SetupGet(x => x.Traits).Returns([f.NativeSkill.Object]); f.NativeSkill.SetupGet(x => x.RawValue).Returns(() => f.Skills[1]);
        f.Actor.Setup(x => x.UseResource(reserve, It.IsAny<double>())).Returns<IMagicResource, double>((resource, amount) => {
            using var debit = MagicResourceCapacityAdmission.BeginDebit(f.Actor.Object, resource, amount);
            Assert.IsTrue(MagicResourceCapacity.TryGetCap(resource, f.Actor.Object, out var capacity, out _));
            Assert.AreEqual(100.0, capacity); f.Balances[resource] -= amount; return true;
        });
        var spell = focus ? (MagicSpell)f.Spells.Single(x => x.Id == 2) : f.NewSpell(2, "Final food payment admission", xml.ToString());
        Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
        if (!focus) Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 2")));
        f.Store.Write(acquired: new(100, 2, 7, 1, f.Now, "fixture", DateTime.UnixEpoch, f.Store.Acquisition(100, 2)?.Version ?? 0));
        f.Checkpoint = stage => { if (stage == "Paying") paying = true; };
        var intent = new MagicCastingIntent(f.Actor.Object, f.Earth.Id, 2, 7, false, "self");
        var result = focus ? f.Service.CastDeviceFocus(intent, device!.Item.Object) : f.Service.Cast(intent);
        Assert.AreEqual(7, random.Draws); Assert.AreEqual(0, debitCapCallbacks);
        Assert.IsTrue(capCalls > 0); Assert.IsFalse(firstMatch && scenario != "late-cap");
        if (scenario == "exact-debit") {
            Assert.AreEqual(MagicCastingStatus.NeedsReview, result.Status, result.Message); Assert.IsTrue(paying);
            Assert.AreEqual(65.0, f.Balances[reserve]); Assert.AreEqual(1, f.Store.Operations.Count);
            // The exact scope is over: ordinary capacity evaluation executes the authored Prog again.
            var previous = capCalls; paying = false;
            Assert.IsTrue(MagicResourceCapacity.TryGetCap(reserve, f.Actor.Object, out _, out _)); Assert.AreEqual(previous + 1, capCalls);
            Assert.IsNull(MagicResourceCapacityAdmission.BeginDebit(f.Actor.Object, reserve, 35));
        } else {
            Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message); Assert.IsFalse(paying);
            Assert.IsNull(result.OperationId); Assert.AreEqual(0, f.Store.Operations.Count);
            Assert.AreEqual(scenario == "selection-balance" ? 80.0 : 100.0, f.Balances[reserve]);
            Assert.AreEqual(3, eligibilityCalls, result.Message); Assert.AreEqual(4, policyCalls, result.Message);
            owned.Verify(x => x.Create(It.IsAny<IGameItemProto>(), It.IsAny<ICharacter>(), It.IsAny<ItemQuality>(), It.IsAny<SpellLifecycleOrigin>()), Times.Never);
        }
    }
}
