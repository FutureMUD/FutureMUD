#nullable enable
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static class CombinedEntryPoint
{
	public static int Main(string[] args) => GNHProgram.ProvisionStockMain(args);
}

internal static partial class GNHProgram
{
	private sealed record InstalledControl(ArmageddonMagicInstallPlan Plan, IReadOnlyDictionary<string, long> Identities);
	private static InstalledControl? _installedControl;
	static partial void InstallCombinedContent(TestDatabase database, FixtureIds fixture, NativeRuntime seed)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var skills = new Dictionary<string, long>();
		foreach (var content in ArmageddonMagicInstaller.Content(new(false, 0, 0, skills, 0, 0, 0, 0, 0, 0, 0, 0, 0)))
		{
			var skill = new Db.TraitDefinition { Name = "ARMINSTALLPROVISION " + content.Name, Type = 0, OwnerScope = 1, TraitGroup = "ARM02",
				Alias = "", ChargenBlurb = "Explicit combined fixture binding.", ValueExpression = "100", ExpressionId = fixture.TraitExpressionId };
			db.TraitDefinitions.Add(skill); db.SaveChanges(); skills.Add(content.Key, skill.Id);
		}
		var eligibility = new Db.FutureProg { FunctionName = "combinedInstallerMend", FunctionText = "return true",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), Category = "Harness", Subcategory = "Combined Installer", FunctionComment = "Disposable explicit policy." };
		foreach (var name in new[] { "target", "caster" }) eligibility.FutureProgsParameters.Add(new() { ParameterIndex = eligibility.FutureProgsParameters.Count,
			ParameterName = name, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(eligibility); db.SaveChanges();
		var authority = new Db.AuthorityGroup { Name = "ARMINSTALLPROVISION fixture authority" }; db.AuthorityGroups.Add(authority); db.SaveChanges();
		var builder = new Db.Account { Name = "ARMINSTALLPROVISION disposable builder", CreationDate = DateTime.UtcNow, AuthorityGroupId = authority.Id };
		foreach (var property in typeof(Db.Account).GetProperties().Where(x => x.PropertyType == typeof(string)))
			if (property.GetValue(builder) is null) property.SetValue(builder, "");
		db.Accounts.Add(builder); db.SaveChanges();
		var light = db.GameItemProtos.Single(x => x.Name == "ARM03B2B C2 light"); var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
		var plan = new ArmageddonMagicInstallPlan(true, seed.Capability.School.Id, seed.Resource.Id, skills,
			db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id, eligibility.Id, db.Liquids.Single(x => x.Name == "ARM03C2 water").Id,
			light.Id, light.RevisionNumber, hold.Id, hold.RevisionNumber, seed.World.Materials.First().Id, builder.Id);
		var players = PlayerSnapshot(database); var installed = InstallOwned(database, plan); RequireInstalled(installed);
		Require(installed.Identities.Count == 21 && players == PlayerSnapshot(database), "Combined install expanded ownership or changed players.");
		_installedControl = new(plan, installed.Identities);
		Console.WriteLine("ARMINSTALLPROVISION-installed=passed same-21-owned-records five-utilities two-blank-templates no-provision-manifest-expansion player-state-conserved");
	}
	static partial void LoadCombinedContent(RetirementHost host, TestDatabase database)
	{
		var control = _installedControl!; var world = host.Native.World; var progs = (All<IFutureProg>)world.FutureProgs;
		using var db = NewIndependentContext(database.ConnectionString);
		var support = control.Identities.Where(x => x.Key.EndsWith(".eligibility")).Select(x => x.Value)
			.Append(control.Plan.MendEligibilityProg).Append(control.Plan.AlwaysFalseProg).ToArray();
		foreach (var row in db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().Where(x => support.Contains(x.Id)))
			if (progs.Get(row.Id) is null) { var prog = new FutureProg(row, world); Require(prog.Compile(), prog.CompileError); progs.Add(prog); }
		var spells = (All<IMagicSpell>)world.MagicSpells;
		foreach (var content in ArmageddonMagicInstaller.Content(control.Plan))
		{
			var id = control.Identities[content.Key]; spells.Remove(spells.Get(id));
			var spell = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == id), world); spells.Add(spell);
			Require(spell.ReadyForGame, "Combined installed definition not ready: " + content.Key);
		}
		var existing = world.ItemComponentProtos;
		var components = db.GameItemComponentProtos.Include(x => x.EditableItem).AsNoTracking().Where(x => x.Type == "ChargedMagicDevice")
			.ToDictionary(x => (x.Id, x.RevisionNumber), x => (IGameItemComponentProto)new ChargedMagicDeviceGameItemComponentProto(x, world));
		foreach (var component in components.Values) Require(component.CanSubmit(), component.WhyCannotSubmit());
		var catalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
		catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, revision) => components.GetValueOrDefault((id, revision)) ?? existing.Get(id, revision));
		catalogue.Setup(x => x.GetEnumerator()).Returns(() => components.Values.Concat(existing).GetEnumerator()); host.Native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(catalogue.Object);
		foreach (var row in db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosTags).Include(x => x.GameItemProtosGameItemComponentProtos).AsNoTracking().Where(x => x.Name.StartsWith("Armageddon blank")))
			host.Prototypes[row.Id] = new GameItemProto(row, world);
		var mend = control.Identities[ArmageddonReviewedUtilityContent.MendFleshKey]; var trait = control.Plan.SpellSkills[ArmageddonReviewedUtilityContent.MendFleshKey];
		foreach (var command in new[] { $"casting entry add {mend}", $"casting entry trait {mend} {trait}", $"casting entry skill {mend} 30 60 relative", $"casting entry starting {mend} on" })
			Require(host.Native.Capability.BuildingCommand(host.Native.Actor, new StringStack(command)), "Combined installed route refused " + command);
	}
	static partial void VerifyCombinedContent(RetirementHost host, TestDatabase database, HarnessClock clock,
		MagicCastingService casting, MagicSpell food, SkillLevelBasedMagicCapability capability, Action<string> terrain)
	{
		var control = _installedControl!; var native = host.Native; var actor = native.Actor; var world = native.World;
		var mend = control.Identities[ArmageddonReviewedUtilityContent.MendFleshKey]; var trait = world.Traits.Get(control.Plan.SpellSkills[ArmageddonReviewedUtilityContent.MendFleshKey]);
		actor.SetTraitValue(trait, 60); actor.AddResource(native.Resource, 100); FlushCasting(native);
		var wand = (GameItem)host.Prototypes[control.Identities["arm.item.charged_wand"]].CreateNew(actor);
		world.Add(wand); actor.Location.Insert(wand, true); wand.Login(); world.SaveManager.Flush(); native.Body.Get(wand, silent: true);
		var device = (ChargedMagicDeviceGameItemComponent)wand.GetItemType<IChargedMagicDevice>(); Require(device.Charges == 0, "Combined installed device had free charges.");
		actor.RemoveAllEffects<MudSharp.Effects.Concrete.MagicSpellLockout>(null, true);
		var began = casting.BeginDeviceProduction(actor, wand, capability.Id, mend, 1, 1); Require(began.Status == MagicCastingStatus.Started, began.Message);
		clock.Advance(TimeSpan.FromSeconds(61)); var produced = casting.CompleteDeviceProduction(actor, began.OperationId!.Value);
		Require(produced.Status == MagicCastingStatus.Succeeded && device.Charges == 1, produced.Message);
		var state = new MagicCastingStateStore(); var acquired = state.Acquisition(actor.Id, mend); var raw = actor.TraitRawValue(trait); var balance = actor.MagicResourceAmounts[native.Resource];
		var wounds = native.Body.Wounds.Where(x => x.CanBeTreated(TreatmentType.Mend) != Difficulty.Impossible).ToArray(); var damage = wounds.Sum(x => x.CurrentDamage);
		Require(damage > 2, "Combined fixture native Mend wounds missing.");
		var activated = casting.ActivateDevice(actor, wand, "me");
		Require(activated.Status == MagicCastingStatus.Succeeded && device.Charges == 0 && wounds.Sum(x => x.CurrentDamage) == damage - 2 &&
			actor.MagicResourceAmounts[native.Resource] == balance && actor.TraitRawValue(trait) == raw && state.Acquisition(actor.Id, mend) == acquired,
			"Combined installed Mend charge did not preserve native healing/accounting: " + activated.Message);
		Require(casting.ActivateDevice(actor, wand, "me").Status == MagicCastingStatus.Refused, "Combined depleted wand activated.");
		Console.WriteLine("ARMINSTALLPROVISION-charge=passed installed-native-wand legitimate-paid-Mend-production heal-two depletion no-activation-payment-mastery-or-acquisition");
		var proto = (ChargedMagicDeviceGameItemComponentProto)device.Prototype;
		Require(proto.BuildingCommand(actor, new StringStack($"spell add {food.Id}")), "Combined focus builder whitelist edit failed.");
		// A real Cell locates itself and reports its visible room description. The
		// shared loose room fixture leaves both contracts at their default values.
		var room = actor.Location; var cell = Mock.Get(room);
		cell.SetupGet(x => x.Location).Returns(room);
		cell.Setup(x => x.HiddenFromPerception(It.IsAny<IPerceiver>(), It.IsAny<PerceptionTypes>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		Require(actor.CanSee(room) && actor.CanInteractPlanar(room, PlanarInteractionKind.Magic), "Combined native focus room fixture is not visible/material-reachable.");
		world.SaveManager.Flush();
		VerifyProvisionFinalPayment(food, host, database, capability, terrain, wand);
		Require(device.Charges == 0 && device.Reservation is null, "Focus provision changed the depleted installed charge bank.");
		var players = PlayerSnapshot(database); using var before = NewIndependentContext(database.ConnectionString);
		var provisionIds = world.MagicSpells.OfType<MagicSpell>().Where(x => x.StockIdentity == ArmageddonSustainMealStock.Key || x.StockIdentity == ArmageddonDrawWineStock.Key).Select(x => x.Id).ToArray();
		Require(provisionIds.Length == 2, "Combined ordinary builder provision definitions missing.");
		var provision = before.MagicSpells.AsNoTracking().Where(x => provisionIds.Contains(x.Id)).ToArray();
		var definitions = provision.ToDictionary(x => x.Id, x => x.Definition); var rerun = InstallOwned(database, control.Plan); RequireInstalled(rerun);
		using var after = NewIndependentContext(database.ConnectionString);
		Require(players == PlayerSnapshot(database) && rerun.Identities.All(x => control.Identities[x.Key] == x.Value) &&
			after.SeederManagedRecords.Count(x => x.Seeder == ArmageddonMagicInstaller.Package) == 21 &&
			definitions.All(x => after.MagicSpells.Find(x.Key)!.Definition == x.Value) &&
			!after.SeederManagedRecords.Any(x => provision.Select(p => p.Id).Contains(x.LogicalId!.Value) && x.EntityType == nameof(Db.MagicSpell)),
			"Combined installer rerun changed active players, provision content, ownership or identities.");
		var componentId = control.Identities["arm.component.charged_wand"];
		Require(after.GameItemComponentProtos.AsNoTracking().Single(x => x.Id == componentId).Definition.Contains($"<Spell>{food.Id}</Spell>"), "Rerun lost native builder focus whitelist override.");
		wand.Delete(); FlushCasting(native);
		Console.WriteLine("ARMINSTALLPROVISION-coexistence=passed actual-native-installed-focus late-cap-refusal exact-debit no-redraw no-free-charge scope-disposal active-rerun player-state-and-unowned-provision-preserved builder-override-preserved same-21-owned-identities");
	}
}
