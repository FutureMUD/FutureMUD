using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Resources;
using MudSharp.RPG.Merits;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static FutureProg DeviceIndirectProg(RetirementHost host, TestDatabase database, string name, ProgVariableTypes type, string text)
	{
		var parameters = new[] { Tuple.Create(ProgVariableTypes.Character, "caster") };
		using var db = NewIndependentContext(database.ConnectionString);
		var model = new Db.FutureProg { FunctionName = name, FunctionText = text, FunctionComment = "Owned indirect device callback regression.",
			ReturnTypeDefinition = type.ToStorageString(), Category = "Harness", Subcategory = "ChargedDevices", StaticType = (int)FutureProgStaticType.NotStatic };
		model.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "caster", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(model); db.SaveChanges();
		var prog = new FutureProg(host.Native.World, name, type, parameters, text) { Id = model.Id, StaticType = FutureProgStaticType.NotStatic };
		Require(prog.Compile(), prog.CompileError); ((All<IFutureProg>)host.Native.World.FutureProgs).Add(prog); return prog;
	}
	private static void ReviewDeviceIndirectCallbacks(TestDatabase database, RetirementHost host, HarnessClock clock,
		MagicCastingService service, MagicSpell payload, SkillLevelBasedMagicCapability capability, IMerit merit,
		ChargedMagicDeviceGameItemComponent bank)
	{
		var native = host.Native; var actor = native.Actor; var world = native.World; var item = bank.Parent;
		var register = world.VariableRegister;
		var applicability = DeviceIndirectProg(host, database, "armdevReviewApplicability", ProgVariableTypes.Boolean,
			CountedPolicy(2, "silentdrop(@caster, getregister(@caster, \"armdev_review_item\"))"));
		SetPrivateMember(merit, "ApplicabilityProg", applicability);
		actor.SetMerits([merit]); register.SetValue(actor, "armdev_review_calls", new NumberVariable(0));
		if (!native.Body.HeldOrWieldedItems.Contains(item)) native.Body.Get(item, silent: true);
		void AssertRefusal(Func<MagicCastingResult> execute, IMagicResource resource, string name)
		{
			var balance = actor.MagicResourceAmounts[resource]; var definition = bank.PersistedDefinition;
			Dictionary<string, string> journal;
			using (var db = NewIndependentContext(database.ConnectionString)) journal = db.MagicCastingOperations.AsNoTracking().ToDictionary(x => x.Id.ToString(), x => x.Definition + x.Stage);
			var result = execute(); Require(result.Status == MagicCastingStatus.Refused, name + " admitted: " + result.Message);
			Require(!native.Body.HeldOrWieldedItems.Contains(item), name + " did not execute actual native silentdrop.");
			Require(actor.MagicResourceAmounts[resource] == balance && bank.PersistedDefinition == definition && bank.Reservation is null && !bank.Changed, name + " changed balances/bank/reservation.");
			using var context = NewIndependentContext(database.ConnectionString);
			var after = context.MagicCastingOperations.AsNoTracking().ToDictionary(x => x.Id.ToString(), x => x.Definition + x.Stage);
			Require(after.Count == journal.Count && after.All(x => journal.GetValueOrDefault(x.Key) == x.Value) &&
				context.GameItemComponents.AsNoTracking().Single(x => x.Id == bank.Id).Definition == definition, name + " changed durable bank or journal.");
			Console.WriteLine("ARMDEV-review-P2-indirect=passed " + name + " successful-policy-return final-evaluation-only native-drop no-balance-bank-reservation-journal-change");
		}
		AssertRefusal(() => service.ActivateDevice(actor, item, "self"), native.Resource, "actual-capability-merit-applicability-charged");
		Require(Convert.ToDecimal(register.GetValue(actor, "armdev_review_calls").GetObject) == 2, "Applicability mutation did not occur at final eligibility.");
		applicability.FunctionText = "return true"; Require(applicability.Compile(), applicability.CompileError);
		var oldResources = world.MagicResources;
		Db.MagicResource resourceModel;
		using (var db = NewIndependentContext(database.ConnectionString)) resourceModel = db.MagicResources.AsNoTracking().Single(x => x.Id == native.Resource.Id);
		// Use actual SimpleMagicResource: the baseline fixture's cap-100 override bypasses authored cap Progs.
		var actualResource = new SimpleMagicResource(resourceModel, world);
		var cap = DeviceIndirectProg(host, database, "armdevReviewResourceCap", ProgVariableTypes.Number, "return 100");
		actualResource.ResourceCapProg = cap;
		var resources = new All<IMagicResource>(); foreach (var resource in oldResources) resources.Add(resource.Id == actualResource.Id ? actualResource : resource);
		var balances = (DoubleCounter<IMagicResource>)GetPrivateField(actor, "_magicResourceAmounts")!;
		var before = balances[native.Resource]; balances.Remove(native.Resource); balances[actualResource] = before;
		native.WorldMock.SetupGet(x => x.MagicResources).Returns(resources);
		try
		{
			Require(register.RegisterVariable(ProgVariableTypes.Character, ProgVariableTypes.Boolean, "armdev_review_armed", false), "Final-cap flag registration failed.");
			cap.FunctionText = "if (getregister(@caster, \"armdev_review_armed\"))\nsetregister @caster \"armdev_review_armed\" false\nsetregister @caster \"armdev_review_calls\" (getregister(@caster, \"armdev_review_calls\")+1)\nsilentdrop(@caster, getregister(@caster, \"armdev_review_item\"))\nend if\nreturn 100";
			Require(cap.Compile(), cap.CompileError);
			var armed = false; var finalCapCalls = 0;
			var finalService = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, flush: () => world.SaveManager.Flush(), checkpoint: stage =>
			{
				if (stage != "DeviceAdmissionPolicies") return;
				armed = true; register.SetValue(actor, "armdev_review_calls", new NumberVariable(0));
				register.SetValue(actor, "armdev_review_armed", new BooleanVariable(true));
			});
			native.WorldMock.SetupGet(x => x.MagicCasting).Returns(finalService);
			if (!native.Body.HeldOrWieldedItems.Contains(item)) native.Body.Get(item, silent: true);
			AssertRefusal(() => finalService.CastDeviceFocus(new(actor, capability.Id, payload.Id, 2, false, "self"), item), actualResource, "actual-SimpleMagicResource-resourcecap-focus");
			finalCapCalls = Convert.ToInt32(register.GetValue(actor, "armdev_review_calls").GetObject);
			Require(armed && finalCapCalls == 1, "Actual resource cap was not evaluated exactly at final admission.");
			cap.FunctionText = "return 100"; Require(cap.Compile(), cap.CompileError);
			native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
			if (!native.Body.HeldOrWieldedItems.Contains(item)) native.Body.Get(item, silent: true);
			// Separate disposable equivalence fixture: exercise exact canonical forwarding and native cap clamping.
			var shell = NativeHarnessCharacter.Create(world, actor.Id + 1000000, actor.Location, actor.Culture); shell.HarnessIdentity = actor.Identity;
			balances[actualResource] = 120;
			using (new MagicResourceCapacityAdmission([new(actor, actualResource, 7.5, 100)]).OpenScope())
				Require(shell.UseResource(actualResource, 7.5) && balances[actualResource] == 92.5 && actor.ResourcesChanged, "Native canonical forwarding/cap clamp/fractional debit changed.");
			world.SaveManager.Flush();
			using (var db = NewIndependentContext(database.ConnectionString))
				Require(db.CharactersMagicResources.AsNoTracking().Single(x => x.CharacterId == actor.Id && x.MagicResourceId == actualResource.Id).Amount == 92.5, "Native admitted debit dirty state did not persist.");
			actor.AddResource(actualResource, 100); // Legitimate capped replenishment for the added controls and later lane cases.
			var focusBalance = actor.MagicResourceAmounts[actualResource];
			var focus = service.CastDeviceFocus(new(actor, capability.Id, payload.Id, 2, false, "self"), item);
			Require(focus.Status == MagicCastingStatus.Succeeded && actor.MagicResourceAmounts[actualResource] == focusBalance - 10, "Actual-cap positive focus debit failed: " + focus.Message);
			actor.RemoveAllEffects<SpellBlindnessEffect>(fireRemovalAction: true);
			var directBalance = actor.MagicResourceAmounts[actualResource];
			var direct = service.Cast(new(actor, capability.Id, payload.Id, 2, false, "self"));
			Require(direct.Status == MagicCastingStatus.Succeeded && actor.MagicResourceAmounts[actualResource] == directBalance - 10, "Actual-cap baseline direct cast failed: " + direct.Message);
			actor.RemoveAllEffects<SpellBlindnessEffect>(fireRemovalAction: true);
			var productionBalance = actor.MagicResourceAmounts[actualResource]; var charges = bank.Charges;
			var production = service.BeginDeviceProduction(actor, item, capability.Id, payload.Id, 2, 1);
			Require(production.Status == MagicCastingStatus.Started && actor.MagicResourceAmounts[actualResource] == productionBalance - 10, "Actual-cap positive production debit failed: " + production.Message);
			clock.Advance(TimeSpan.FromSeconds(1)); Require(service.CompleteDeviceProduction(actor, production.OperationId!.Value).Status == MagicCastingStatus.Succeeded && bank.Charges == charges + 1, "Actual-cap positive production completion failed.");
			Require(MagicResourceCapacity.TryGetCap(actualResource, actor, out var ordinaryCap, out _) && ordinaryCap == 100, "Ordinary capacity query changed after admitted payment.");
			Console.WriteLine("ARMDEV-review-native-payment-equivalence=passed actual-native-UseResource direct-focus-production canonical-owner-forwarding cap-clamp fractional-debit dirty-state-persistence ordinary-cap-query no-admission-leak");
		}
		finally
		{
			var balance = balances[actualResource]; balances.Remove(actualResource); balances[native.Resource] = balance;
			native.WorldMock.SetupGet(x => x.MagicResources).Returns(oldResources); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		}
	}
}
