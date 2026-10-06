using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Resources;
using MudSharp.Magic.SpellEffects;

#nullable enable
namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed class ProvisionPaymentRandom : Random
	{
		internal int Draws;
		public override int Next(int maxValue) { ++Draws; return 0; }
	}
	private sealed class ProvisionPaymentFood(XElement root, IMagicSpell spell, ProvisionPaymentRandom random) : CreateItemEffect(root, spell)
	{ protected override Random FoodProfileRandom => random; }

	private static void VerifyProvisionFinalPayment(MagicSpell food, RetirementHost host, TestDatabase database,
		SkillLevelBasedMagicCapability capability, Action<string> terrain, MudSharp.GameItems.IGameItem? focus = null)
	{
		var native = host.Native; var actor = native.Actor; var world = native.World;
		var resources = (All<IMagicResource>)world.MagicResources; var oldResource = native.Resource;
		// Replace the harness's fixed-cap wrapper with the actual SimpleMagicResource + authored cap Prog.
		MudSharp.Models.MagicResource model;
		using (var db = NewIndependentContext(database.ConnectionString)) model = db.MagicResources.AsNoTracking().Single(x => x.Id == oldResource.Id);
		var resource = new SimpleMagicResource(model, world);
		var amounts = (DoubleCounter<IMagicResource>)actor.MagicResourceAmounts;
		var balance = amounts[oldResource]; amounts.Remove(oldResource); amounts[resource] = balance;
		resources.Remove(oldResource); resources.Add(resource); SetPrivateMember(native, "Resource", resource);
		var factories = (IDictionary<string, Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>>)typeof(SpellEffectFactory)
			.GetField("_loadTimeFactories", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		var oldFactory = factories["createitem"]; var progs = (All<IFutureProg>)world.FutureProgs;
		var eligibilityCalls = 0; var capCalls = 0; var debitCapCalls = 0; var debitWindow = false; var payingSeen = false; var drift = true;
		var eligibility = new Mock<IFutureProg>(); var eligibilityId = progs.Max(x => x.Id) + 1;
		eligibility.SetupGet(x => x.Id).Returns(eligibilityId); eligibility.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		eligibility.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		eligibility.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() => { ++eligibilityCalls; return true; });
		var cap = new Mock<IFutureProg>(); cap.SetupGet(x => x.Id).Returns(eligibilityId + 1); cap.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Number);
		cap.Setup(x => x.ExecuteDouble(It.IsAny<double>(), It.IsAny<object[]>())).Returns(() => {
			++capCalls;
			if (debitWindow) { ++debitCapCalls; terrain("Silt"); }
			if (drift && eligibilityCalls == 3) terrain("Silt");
			return 100.0;
		});
		progs.Add(eligibility.Object); progs.Add(cap.Object); resource.ResourceCapProg = cap.Object;
		var random = new ProvisionPaymentRandom(); factories["createitem"] = (root, spell) => new ProvisionPaymentFood(root, spell, random);
		Require(food.BuildingCommand(actor, new StringStack($"effect 1 eligibility {eligibilityId}")), "Final food cap eligibility fixture");
		var originalCasting = world.MagicCasting;
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, checkpoint: stage => {
			if (stage == "Paying") { payingSeen = true; debitWindow = true; }
			if (stage == "PaymentMutated") debitWindow = false;
		}, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		MagicCastingResult Cast() { var intent = new MagicCastingIntent(actor, capability.Id, food.Id, 7, false, ""); return focus is null ? casting.Cast(intent) : casting.CastDeviceFocus(intent, focus); }
		try {
			actor.RemoveAllEffects<MagicSpellLockout>(null, true); actor.AddResource(resource, 100); FlushCasting(native);
			using var before = NewIndependentContext(database.ConnectionString);
			var counts = (before.MagicCastingOperations.Count(), before.GameItems.Count(), before.MagicSpellLifecycles.Count()); var reserveBefore = amounts[resource];
			var refusal = Cast();
			using var after = NewIndependentContext(database.ConnectionString);
			Require(refusal.Status == MagicCastingStatus.Refused && refusal.OperationId is null && !payingSeen && amounts[resource] == reserveBefore &&
				counts == (after.MagicCastingOperations.Count(), after.GameItems.Count(), after.MagicSpellLifecycles.Count()) &&
				eligibilityCalls == 3 && random.Draws == 7 && capCalls > 0 && debitCapCalls == 0,
				"Actual final cap mutation did not refuse without payment/output/redraw: " + refusal.Message);
			Console.WriteLine($"PROVISION-final-cap=passed mode:{(focus is null ? "ordinary" : "installed-focus")} actual-SimpleMagicResource authored-cap-Prog late-first-match-drift no-Paying no-reserve-change no-operation-output-lifecycle seven-draws-no-redraw");
			drift = false; terrain("Desert"); eligibilityCalls = 0; random.Draws = 0; actor.RemoveAllEffects<MagicSpellLockout>(null, true);
			var existing = host.Items.Select(x => x.Id).ToHashSet();
			var paid = Cast(); var outputs = host.Items.Where(x => !existing.Contains(x.Id)).ToArray();
			Require(paid.Status == MagicCastingStatus.Succeeded && payingSeen && !debitWindow && debitCapCalls == 0 && reserveBefore - amounts[resource] == 50 &&
				outputs.Length == 7 && random.Draws == 7 && (bool)XElement.Parse(new MagicCastingStateStore().Operation(paid.OperationId!.Value)!.Definition).Attribute("applied")!,
				"Exact final capacity admission did not protect the real native debit/application: " + paid.Message);
			var sampled = capCalls; Require(MagicResourceCapacity.TryGetCap(resource, actor, out var live, out _) && live == 100 && capCalls == sampled + 1,
				"Capacity admission leaked into ordinary queries");
			foreach (var item in outputs) item.Delete(); FlushCasting(native);
			Require(outputs.All(x => x.Deleted) && amounts[resource] == reserveBefore - 50, "Exact fixture output cleanup refunded payment");
			Console.WriteLine($"PROVISION-exact-debit=passed mode:{(focus is null ? "ordinary" : "installed-focus")} actual-native-Character.UseResource no-cap-Prog-during-debit paid50 applied-seven-food seven-draws-no-redraw scope-ended-ordinary-cap-live exact-output-cleanup-no-refund");
		} finally {
			debitWindow = false; terrain("Desert"); factories["createitem"] = oldFactory;
			Require(food.BuildingCommand(actor, new StringStack("effect 1 eligibility none")), "Restore provision admission fixture");
			native.WorldMock.SetupGet(x => x.MagicCasting).Returns(originalCasting);
			balance = amounts[resource]; amounts.Remove(resource); amounts[oldResource] = balance;
			resources.Remove(resource); resources.Add(oldResource); SetPrivateMember(native, "Resource", oldResource);
			progs.Remove(eligibility.Object); progs.Remove(cap.Object);
		}
	}
}
