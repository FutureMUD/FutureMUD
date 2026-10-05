using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using DatabaseSeeder.Seeders;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static class InstallerEntryPoint
{
	public static int Main(string[] args) => GNHProgram.InstallerMain(args);
}
internal static partial class GNHProgram
{
	private sealed record InstallerReader(string Database, ArmageddonMagicInstallPlan Plan, Dictionary<string, long> Identities);
	internal static int InstallerMain(string[] args)
	{
		try
		{
			OwnedConnections.Install();
			return args.FirstOrDefault() switch { "--installer-reader" => InstallerRestart(args[1]), "--installer-run" => InstallerNative(), _ => Main(args) };
		}
		catch (Exception error) { Console.Error.WriteLine(error); return 1; }
	}
	private static ArmageddonInstallResult InstallOwned(TestDatabase database, ArmageddonMagicInstallPlan plan, Action<ArmageddonInstallCheckpoint>? fault = null)
	{ using var db = NewIndependentContext(database.ConnectionString); return ArmageddonMagicInstaller.Install(db, plan, fault); }
	private static void RequireInstalled(ArmageddonInstallResult result) => Require(result.Status == ArmageddonInstallStatus.Completed, string.Join("\n", result.Messages));
	private static string PlayerSnapshot(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString); using var connection = database.OpenOwnedConnection();
		var tables = db.Model.GetEntityTypes().Where(x => x.ClrType.Name.StartsWith("Character") || x.ClrType.Name.StartsWith("Bodies") || x.ClrType.Name is "Body" or "Trait" or "GameItem" or "GameItemComponent" ||
			x.ClrType.Name.StartsWith("MagicCasting") || x.ClrType.Name.StartsWith("MagicSpellAcquisition") || x.ClrType.Name.StartsWith("MagicSkill") || x.ClrType.Name.StartsWith("MagicPractice"))
			.Select(x => x.GetTableName()!).Distinct().Order().ToArray();
		var rows = new List<string>();
		foreach (var table in tables)
		{
			using var command = connection.CreateCommand(); command.CommandText = $"SELECT * FROM `{table}`";
			using var reader = command.ExecuteReader();
			while (reader.Read()) rows.Add(table + ":" + JsonSerializer.Serialize(Enumerable.Range(0, reader.FieldCount).Select(i => reader.IsDBNull(i) ? null : reader.GetValue(i)).ToArray()));
		}
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n", rows.Order()))));
	}
	private static int InstallerNative()
	{
		using var globals = new ConsumableGlobals(); using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine("ARMINSTALL-database=" + database.Name);
		var fixture = FixtureSeed.Create(database, "installer_core_lane", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedConsumables(database, fixture, seed.World.Materials.First().Id);
		ArmageddonMagicInstallPlan plan;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var skills = new Dictionary<string, long>();
			foreach (var content in ArmageddonMagicInstaller.Content(new(false, 0, 0, skills, 0, 0, 0, 0, 0, 0, 0, 0, 0)))
			{
				var skill = new Db.TraitDefinition { Name = "ARMINSTALL " + content.Name, Type = 0, OwnerScope = 1, TraitGroup = "ARM02",
					Alias = "", ChargenBlurb = "Controlled fixture binding; no historical character IDs asserted.", ValueExpression = "100", ExpressionId = fixture.TraitExpressionId };
				db.TraitDefinitions.Add(skill); db.SaveChanges(); skills.Add(content.Key, skill.Id);
			}
			var eligibility = new Db.FutureProg { FunctionName = "installerMendEligibility", FunctionText = "return true",
				ReturnTypeDefinition = MudSharp.FutureProg.ProgVariableTypes.Boolean.ToStorageString(), Category = "Harness", Subcategory = "Installer", FunctionComment = "Controlled explicit fixture policy." };
			foreach (var name in new[] { "target", "caster" }) eligibility.FutureProgsParameters.Add(new() { ParameterIndex = eligibility.FutureProgsParameters.Count,
				ParameterName = name, ParameterTypeDefinition = MudSharp.FutureProg.ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(eligibility); db.SaveChanges();
			var light = db.GameItemProtos.Single(x => x.Name == "ARM03B2B C2 light"); var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
			var authority = new Db.AuthorityGroup { Name = "ARMINSTALL fixture authority" }; db.AuthorityGroups.Add(authority); db.SaveChanges();
			var builder = new Db.Account { Name = "ARMINSTALL disposable builder", CreationDate = DateTime.UtcNow, AuthorityGroupId = authority.Id };
			foreach (var property in typeof(Db.Account).GetProperties().Where(x => x.PropertyType == typeof(string)))
				if (property.GetValue(builder) is null) property.SetValue(builder, "");
			db.Accounts.Add(builder); db.SaveChanges();
			plan = new(true, seed.Capability.School.Id, seed.Resource.Id, skills, db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id,
				eligibility.Id, db.Liquids.Single(x => x.Name == "ARM03C2 water").Id, light.Id, light.RevisionNumber,
				hold.Id, hold.RevisionNumber, seed.World.Materials.First().Id, builder.Id);
		}
		var untouched = PlayerSnapshot(database);
		Require(InstallOwned(database, plan with { Install = false }).Status == ArmageddonInstallStatus.Declined && untouched == PlayerSnapshot(database), "Optional decline changed player state.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var collision = ArmageddonReviewedUtilityContent.MendFlesh().SpellRow(plan.School, plan.SpellSkills[ArmageddonReviewedUtilityContent.MendFleshKey], plan.AlwaysFalseProg);
			db.MagicSpells.Add(collision); db.SaveChanges();
			Require(InstallOwned(database, plan).Status == ArmageddonInstallStatus.Blocked && db.SeederManagedRecords.Count() == 0, "Unowned collision adopted or partially installed.");
			db.MagicSpells.Remove(collision); db.SaveChanges();
		}
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var failed = InstallOwned(database, plan, phase => { if (phase == boundary) throw new IOException("Owned installer interruption " + boundary); });
			using var db = NewIndependentContext(database.ConnectionString);
			Require(failed.Status == ArmageddonInstallStatus.Failed && !db.SeederManagedRecords.Any() && !db.MagicSpells.Any(x => x.Name == "Mend Flesh") && untouched == PlayerSnapshot(database), "Real MySQL rollback left partial content or ownership.");
			Console.WriteLine("ARMINSTALL-rollback=passed boundary:" + boundary);
		}
		var uncertain = InstallOwned(database, plan, phase => { if (phase == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("Lost installer confirmation"); });
		Require(uncertain.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && uncertain.Identities.Count == 21, "Uncertain commit receipt was misclassified.");
		var ids = uncertain.Identities.ToDictionary(x => x.Key, x => x.Value);
		RunInstallerReader(new(database.Name, plan, ids));
		Require(untouched == PlayerSnapshot(database), "Clean install or restarted rerun changed player state.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var mend = db.MagicSpells.Find(ids[ArmageddonReviewedUtilityContent.MendFleshKey])!;
			mend.Name = "ARMINSTALL builder remedy"; mend.Description = "Keep the builder's prose.";
			var duration = db.TraitExpressions.Find(ids[ArmageddonReviewedUtilityContent.SenseEnchantmentKey + ".duration"])!; duration.Expression = "42*grade";
			var clone = ArmageddonReviewedUtilityContent.MendFlesh().SpellRow(plan.School, plan.SpellSkills[ArmageddonReviewedUtilityContent.MendFleshKey], plan.AlwaysFalseProg);
			clone.Name = "ARMINSTALL unowned clone"; clone.Definition = mend.Definition; clone.EffectDurationExpressionId = mend.EffectDurationExpressionId;
			db.MagicSpells.Add(clone); db.SaveChanges();
		}
		for (var i = 0; i < 2; i++) RequireInstalled(InstallOwned(database, plan));
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(db.MagicSpells.Find(ids[ArmageddonReviewedUtilityContent.MendFleshKey])!.Description == "Keep the builder's prose." &&
				db.TraitExpressions.Find(ids[ArmageddonReviewedUtilityContent.SenseEnchantmentKey + ".duration"])!.Expression == "42*grade" && db.SeederManagedRecords.Count() == 21, "Rerun lost edits or claimed clone.");
			var sense = db.MagicSpells.Find(ids[ArmageddonReviewedUtilityContent.SenseEnchantmentKey])!;
			db.Remove(sense); db.SaveChanges();
			Require(InstallOwned(database, plan).Status == ArmageddonInstallStatus.Blocked && !db.MagicSpells.AsNoTracking().Any(x => x.Id == sense.Id), "Intentional deletion was resurrected.");
			// Restore only the exact disposable test row so native qualification can continue.
			db.Entry(sense).State = EntityState.Added; db.SaveChanges();
			var device = db.SeederManagedRecords.Single(x => x.StableKey == "arm.component.charged_wand"); device.Retired = true; db.SaveChanges();
			Require(InstallOwned(database, plan).Status == ArmageddonInstallStatus.Blocked, "Retired identity was overwritten.");
			device.Retired = false; db.SaveChanges();
		}
		Require(untouched == PlayerSnapshot(database), "Reconciliation changed player state.");
		Console.WriteLine("ARMINSTALL-ownership=passed clean rerun rename field-edit unowned-collision clone deletion retirement no-player-refresh");
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, consumablesAnatomy: true); var native = host.Native; var world = native.World; var actor = native.Actor;
		native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(new MudSharp.Effects.EffectScheduler(world, clock));
		native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(new MudSharp.Magic.Lifecycle.SpellOwnedItemService(world));
		var prototypes = world.ItemComponentProtos;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			// The shared controlled host loads Rejuvenation progs only; load the installer's
			// actual native support definitions before reloading its five spell rows.
			var progs = (All<MudSharp.FutureProg.IFutureProg>)world.FutureProgs;
			var supportIds = ids.Where(x => x.Key.EndsWith(".eligibility")).Select(x => x.Value).Append(plan.MendEligibilityProg).Append(plan.AlwaysFalseProg).ToArray();
			foreach (var row in db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().Where(x => supportIds.Contains(x.Id)))
				if (progs.Get(row.Id) is null) { var prog = new MudSharp.FutureProg.FutureProg(row, world); Require(prog.Compile(), prog.CompileError); progs.Add(prog); }
			var spells = (All<IMagicSpell>)world.MagicSpells;
			foreach (var content in ArmageddonMagicInstaller.Content(plan))
			{
				var id = ids[content.Key]; spells.Remove(spells.Get(id)); spells.Add(new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == id), world));
			}
			var components = db.GameItemComponentProtos.Include(x => x.EditableItem).AsNoTracking().Where(x => x.Type == "ChargedMagicDevice")
				.ToDictionary(x => (x.Id, x.RevisionNumber), x => (IGameItemComponentProto)new ChargedMagicDeviceGameItemComponentProto(x, world));
			foreach (var component in components.Values) Require(component.CanSubmit(), component.WhyCannotSubmit());
			var catalogue = new Mock<IUneditableRevisableAll<IGameItemComponentProto>>();
			catalogue.Setup(x => x.Get(It.IsAny<long>(), It.IsAny<int>())).Returns<long, int>((id, revision) => components.GetValueOrDefault((id, revision)) ?? prototypes.Get(id, revision));
			catalogue.Setup(x => x.GetEnumerator()).Returns(() => components.Values.Concat(prototypes).GetEnumerator()); native.WorldMock.SetupGet(x => x.ItemComponentProtos).Returns(catalogue.Object);
			foreach (var item in db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosTags).Include(x => x.GameItemProtosGameItemComponentProtos).AsNoTracking().Where(x => x.Name.StartsWith("Armageddon blank")))
				host.Prototypes[item.Id] = new GameItemProto(item, world);
		}
		foreach (var content in ArmageddonMagicInstaller.Content(plan))
		{ var spell = (MagicSpell)world.MagicSpells.Get(ids[content.Key]); if (!spell.ReadyForGame) throw new InvalidOperationException("Installed real spell did not load: " + content.Key + " " + spell.WhyNotReadyForGame(actor)); }
		var mendSpell = (MagicSpell)world.MagicSpells.Get(ids[ArmageddonReviewedUtilityContent.MendFleshKey]); var trait = world.Traits.Get(plan.SpellSkills[ArmageddonReviewedUtilityContent.MendFleshKey]);
		var cap = (SkillLevelBasedMagicCapability)native.Capability;
		foreach (var command in new[] { $"casting trait {trait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive", $"casting entry add {mendSpell.Id}",
			$"casting entry trait {mendSpell.Id} {trait.Id}", $"casting entry skill {mendSpell.Id} 30 60 relative", $"casting entry starting {mendSpell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(command)), "Installed Mend legitimate route refused " + command);
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var service = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, flush: () => FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		Require(service.Enrol(staff.Object, actor, cap.Id, "Owned installer native qualification").Allowed, "Installed Mend enrolment refused."); actor.SetTraitValue(trait, 60); actor.AddResource(native.Resource, 100); FlushCasting(native);
		var wand = (GameItem)host.Prototypes[ids["arm.item.charged_wand"]].CreateNew(actor); world.Add(wand); actor.Location.Insert(wand, true); wand.Login(); world.SaveManager.Flush(); native.Body.Get(wand, silent: true);
		var deviceComponent = (ChargedMagicDeviceGameItemComponent)wand.GetItemType<IChargedMagicDevice>(); Require(deviceComponent.Charges == 0, "Installed blank carried free charges.");
		var began = service.BeginDeviceProduction(actor, wand, cap.Id, mendSpell.Id, 1, 1); Require(began.Status == MagicCastingStatus.Started, began.Message);
		clock.Advance(TimeSpan.FromSeconds(61)); var produced = service.CompleteDeviceProduction(actor, began.OperationId!.Value); Require(produced.Status == MagicCastingStatus.Succeeded && deviceComponent.Charges == 1, produced.Message);
		var state = new MagicCastingStateStore(); var acquisition = state.Acquisition(actor.Id, mendSpell.Id); var raw = actor.TraitRawValue(trait); var balance = actor.MagicResourceAmounts[native.Resource];
		var wounds = native.Body.Wounds.Where(x => x.CanBeTreated(TreatmentType.Mend) != Difficulty.Impossible).ToArray(); var damage = wounds.Sum(x => x.CurrentDamage); Require(damage > 2, "Installed Mend native wounds missing.");
		var activation = service.ActivateDevice(actor, wand, "me"); Require(activation.Status == MagicCastingStatus.Succeeded && wounds.Sum(x => x.CurrentDamage) == damage - 2 && deviceComponent.Charges == 0 && actor.MagicResourceAmounts[native.Resource] == balance &&
			actor.TraitRawValue(trait) == raw && state.Acquisition(actor.Id, mendSpell.Id) == acquisition, "Installed paid charge failed native healing/conservation: " + activation.Message);
		Require(service.ActivateDevice(actor, wand, "me").Status == MagicCastingStatus.Refused, "Depleted installed wand activated.");
		Mock.Get(native.Body.Race).Setup(x => x.GetMaximumLiftWeight(It.IsAny<ICharacter>())).Returns(10000);
		var staffItem = (GameItem)host.Prototypes[ids["arm.item.charged_staff"]].CreateNew(actor); world.Add(staffItem); actor.Location.Insert(staffItem, true); staffItem.Login(); world.SaveManager.Flush(); native.Body.Get(staffItem, silent: true);
		var staffDevice = (ChargedMagicDeviceGameItemComponent)staffItem.GetItemType<IChargedMagicDevice>();
		Require(staffDevice.Charges == 0 && ((ChargedMagicDeviceGameItemComponentProto)staffDevice.Prototype).Capacity == 10, "Installed staff was not an empty ten-charge template.");
		var busyBalance = actor.MagicResourceAmounts[native.Resource];
		var busy = service.BeginDeviceProduction(actor, staffItem, cap.Id, mendSpell.Id, 1, 1);
		Require(busy.Status == MagicCastingStatus.Refused && staffDevice.Charges == 0 && actor.MagicResourceAmounts[native.Resource] == busyBalance, "Production without a free hand changed bank or balance.");
		native.Body.Drop(wand, silent: true); world.SaveManager.Flush(); Require(native.Body.FreeHands.Any() && native.Body.HeldItems.Contains(staffItem), "Staff qualification requires native custody and a free hand.");
		var staffBegan = service.BeginDeviceProduction(actor, staffItem, cap.Id, mendSpell.Id, 1, 1); Require(staffBegan.Status == MagicCastingStatus.Started, staffBegan.Message);
		clock.Advance(TimeSpan.FromSeconds(61)); Require(service.CompleteDeviceProduction(actor, staffBegan.OperationId!.Value).Status == MagicCastingStatus.Succeeded && staffDevice.Charges == 1, "Installed staff production failed.");
		var staffBalance = actor.MagicResourceAmounts[native.Resource]; var staffDamage = wounds.Sum(x => x.CurrentDamage);
		var staffActivation = service.ActivateDevice(actor, staffItem, "me"); Require(staffActivation.Status == MagicCastingStatus.Succeeded && staffDevice.Charges == 0 &&
			wounds.Sum(x => x.CurrentDamage) == staffDamage - 2 && actor.MagicResourceAmounts[native.Resource] == staffBalance && actor.TraitRawValue(trait) == raw && state.Acquisition(actor.Id, mendSpell.Id) == acquisition,
			"Installed staff activation failed native healing/conservation: " + staffActivation.Message);
		var activePlayers = PlayerSnapshot(database); RequireInstalled(InstallOwned(database, plan)); Require(activePlayers == PlayerSnapshot(database), "Installed rerun refreshed active player/charge state.");
		Console.WriteLine("ARMINSTALL-native=passed five-real-spells-loaded blank-caster-dual-wand-and-staff legitimate-paid-production native-heal depletion no-device-mastery active-rerun-conserved");
		return 0;
	}
	private static void RunInstallerReader(InstallerReader input)
	{
		var info = new ProcessStartInfo("dotnet") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true };
		info.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); info.ArgumentList.Add("--installer-reader"); info.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(info)!; var stdout = process.StandardOutput.ReadToEnd(); var stderr = process.StandardError.ReadToEnd(); process.WaitForExit();
		Require(process.ExitCode == 0, "Installer restart failed: " + stderr + stdout); Console.Write(stdout);
	}
	private static int InstallerRestart(string encoded)
	{
		var input = JsonSerializer.Deserialize<InstallerReader>(Encoding.UTF8.GetString(Convert.FromBase64String(encoded)))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var result = InstallOwned(database, input.Plan); RequireInstalled(result);
		Require(result.Identities.Count == input.Identities.Count && result.Identities.All(x => input.Identities[x.Key] == x.Value), "Restart duplicated identities.");
		Console.WriteLine("ARMINSTALL-restart=passed uncertain-commit same-21-owned-identities"); return 0;
	}
}
