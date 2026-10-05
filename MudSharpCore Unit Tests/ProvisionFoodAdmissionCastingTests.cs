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
        void Mutate() { if (++callbackCalls != 3) return; if (eligibility) firstMatch = true; else f.Actor.SetupGet(x => x.Location).Returns(Mock.Of<ICell>()); }
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
}
