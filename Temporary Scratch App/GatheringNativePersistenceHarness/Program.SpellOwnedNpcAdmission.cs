#nullable enable

using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void VerifyNativeNpcAdmission(TestDatabase database, NativeRuntime native, SimpleNPCTemplate template)
	{
		var actor = native.Actor; var world = native.World;
		var cap = (SkillLevelBasedMagicCapability)native.Capability;
		var skill = CastingRequired(world.Traits.GetByName("ARM02 Earth Proficiency"));
		var agility = CastingRequired(world.Traits.GetByName("ARM02 Agility"));
		actor.AddTrait(agility, 10);
		SetPrivateMember(template, "Status", RevisionStatus.Current);
		MagicSpell Author(string name, bool npc)
		{
			var spell = new MagicSpell(name, cap.School); ((All<IMagicSpell>)world.MagicSpells).Add(spell);
			foreach (var command in new[] { npc ? "trigger new room" : "trigger new self", $"trait {skill.Id}", "difficulty easy",
				"threshold minorpass", "duration ARM02 Duration", $"cost {native.Resource.Id} ARM02 Cost",
				$"prog {CastingRequired(world.FutureProgs.GetByName("rejuvenation_known")).Id}", "castemote A native admission test manifests.",
				"failcastemote The native admission test fails.", "grades fixture" }.Concat(npc
				? new[] { "effect add createnpc", $"effect 1 npc {template.Id}", "effect 1 lifecycle deathonexpiry", "effect 1 family admission-guardian", "effect 1 lifetime grade*60" }
				: new[] { "effect add spellarmour", "effect 1 absorb grade*10+variable", "effect add boost", $"effect 2 trait {agility.Id}", "effect 2 bonus 0" }))
				Require(spell.BuildingCommand(actor, new StringStack(command)), $"Native admission builder refused {command}.");
			Require(cap.BuildingCommand(actor, new StringStack($"casting entry add {spell.Id}")), "Native admission entry refused.");
			Require(cap.BuildingCommand(actor, new StringStack($"casting entry starting {spell.Id} on")), "Native admission starting entry refused.");
			return spell;
		}
		foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive" })
			Require(cap.BuildingCommand(actor, new StringStack(command)), $"Native admission capability refused {command}.");
		var spell = Author("ARM03B2A admission guardian", true);
		var control = Author("ARM03B2A admission shared trait and reserve control", false);
		Require(cap.BuildingCommand(actor, new StringStack("casting enable on")), "Native admission enable refused.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]);
		world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999);
		staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var samples = 0;
		var service = new MagicCastingService(world, random: () => { samples++; return 0.1; }, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		Require(service.Enrol(staff.Object, actor, cap.Id, "Native NPC admission regression").Allowed, "Native admission enrolment refused.");
		actor.SetTraitValue(skill, 42); actor.AddResource(native.Resource, 100); FlushCasting(native);
		var castingStore = new MagicCastingStateStore();
		castingStore.Write(acquired: service.Acquisition(actor, spell.Id)! with { ControlledGrade = 2 });
		castingStore.Write(acquired: service.Acquisition(actor, control.Id)! with { ControlledGrade = 2 });

		Db.Merit additionalModel; Db.Merit comboModel; long materialId;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			additionalModel = new() { Name = "Native admission additional form", Type = "Additional Body Form", MeritScope = (int)MeritScope.Character,
				Definition = $"<Definition><Race>{native.Body.Race.Id}</Race></Definition>" };
			db.Merits.Add(additionalModel); db.SaveChanges();
			comboModel = new() { Name = "Native admission combo", Type = "Combo", MeritScope = (int)MeritScope.Character,
				Definition = $"<Definition><Children><Child>{additionalModel.Id}</Child></Children></Definition>" };
			db.Merits.Add(comboModel);
			var material = NewLifecycleItem(); db.GameItems.Add(material); db.SaveChanges(); materialId = material.Id;
		}
		AdditionalBodyFormMerit.RegisterMeritInitialiser();
		var additional = (IAdditionalBodyFormMerit)MeritFactory.LoadMerit(additionalModel, world);
		var merits = new All<IMerit>(); merits.Add(additional);
		native.WorldMock.SetupGet(x => x.Merits).Returns(merits);
		var combo = new ComboMerit(comboModel, world); merits.Add(combo);
		var role = new Mock<IChargenRole>(); role.SetupGet(x => x.AdditionalMerits).Returns([additional]);
		var comboRole = new Mock<IChargenRole>(); comboRole.SetupGet(x => x.AdditionalMerits).Returns([combo]);
		foreach (var fixtureRole in new[] { role, comboRole })
		{
			fixtureRole.SetupGet(x => x.TraitAdjustments).Returns([]);
			fixtureRole.SetupGet(x => x.ClanMemberships).Returns([]);
		}
		var stack = new Mock<IStackable>(); stack.SetupProperty(x => x.Quantity, 3);
		var materialHost = new Mock<IGameItem>(); materialHost.SetupGet(x => x.Id).Returns(materialId);
		materialHost.SetupGet(x => x.Gameworld).Returns(world); materialHost.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		materialHost.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
		var priorHeld = typeof(MudSharp.Body.Implementations.Body).GetField("_heldItems", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(native.Body);
		SetPrivateField(native.Body, "_heldItems", new List<Tuple<IGameItem, IGrab>> { Tuple.Create(materialHost.Object, Mock.Of<IGrab>()) });
		native.WorldMock.SetupGet(x => x.Tags).Returns(new All<ITag>());
		spell.InventoryPlanTemplate = new InventoryPlanTemplate(world, new InventoryPlanActionConsume(world, 1, 0, 0, _ => true, _ => true));
		control.InventoryPlanTemplate = new InventoryPlanTemplate(world, new InventoryPlanActionConsume(world, 1, 0, 0, _ => true, _ => true));
		FlushCasting(native);
		var originalMerits = template.SelectedMerits.ToArray(); var originalRoles = template.SelectedRoles.ToArray();
		var sentinel = new Mock<ISaveable>(); sentinel.Setup(x => x.Save()).Throws(new InvalidOperationException("Admission flushed an unrelated pending save."));
		world.SaveManager.Add(sentinel.Object);
		Dictionary<string, object[]> Queues() => new[] { "_saveStack", "_delayedSaveStack", "_initialisationQueue", "_lazyLoaders" }
			.ToDictionary(field => field, field => ((IEnumerable)typeof(SaveManager).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance)!
				.GetValue(world.SaveManager)!).Cast<object>().ToArray());
		bool SameQueues(Dictionary<string, object[]> before)
		{
			var after = Queues();
			return before.All(pair => pair.Value.SequenceEqual(after[pair.Key], ReferenceEqualityComparer.Instance));
		}
		try
		{
			foreach (var variant in new[] { "direct", "role", "combo", "role-combo" })
			{
				var selected = Environment.GetEnvironmentVariable("FM_ARM03B2A_ADMISSION_CASE");
				if (!string.IsNullOrEmpty(selected) && selected != variant) continue;
				template.SelectedMerits.Clear(); template.SelectedRoles.Clear();
				if (variant == "direct") template.SelectedMerits.Add(additional);
				if (variant == "combo") template.SelectedMerits.Add(combo);
				if (variant == "role") template.SelectedRoles.Add(role.Object);
				if (variant == "role-combo") template.SelectedRoles.Add(comboRole.Object);
				var rows = AdmissionDatabaseFingerprint(database); var queues = Queues();
				var balance = actor.MagicResourceAmounts[native.Resource]; var quantity = stack.Object.Quantity;
				var acquisition = service.Acquisition(actor, spell.Id); var opportunity = castingStore.Opportunity(actor.Id, skill.Id);
				var result = service.Cast(new(actor, cap.Id, spell.Id, 3, true, ""));
				Console.WriteLine($"ARM03B2A-admission-observed-{variant}=status:{result.Status} operation:{result.OperationId} balance:{actor.MagicResourceAmounts[native.Resource]} material:{stack.Object.Quantity} samples:{samples} reason:{result.Message}");
				Require(result.Status == MagicCastingStatus.Refused && result.OperationId is null && result.Message.Contains("additional bodies", StringComparison.OrdinalIgnoreCase),
					$"Known unsupported {variant} additional-body merit must refuse before payment: {result.Status}: {result.Message}");
				var created = MudSharp.Framework.Scheduling.RuntimeClock.UtcNow;
				Refuse(() => template.CreateSpellOwnedCharacter(actor.SpatialLocation, new(Guid.NewGuid(), spell.Id, 3, actor.Id,
					"admission-guardian", SpellLifecycleMode.DeathOnExpiry, created, created.AddMinutes(5), "direct service revalidation")),
					$"The {variant} native construction service admitted unsupported additional bodies.");
				Require(actor.MagicResourceAmounts[native.Resource] == balance && stack.Object.Quantity == quantity && samples == 0 &&
					service.Acquisition(actor, spell.Id) == acquisition && castingStore.Opportunity(actor.Id, skill.Id) == opportunity &&
					AdmissionDatabaseFingerprint(database) == rows && SameQueues(queues) && world.SaveManager.IsQueued(sentinel.Object),
					$"The {variant} refusal changed native rows, resources, materials, progression opportunities or save queues.");
				template.SelectedMerits.Clear(); template.SelectedRoles.Clear();
				Require(service.Quote(new(actor, cap.Id, control.Id, 3, true, "self")).Allowed && SameQueues(queues),
					$"The {variant} refusal quarantined the shared trait/reserve or queued a save.");
				Console.WriteLine($"ARM03B2A-admission-{variant}=passed configured-native-casting real-loaded-merit-and-combo no-payment material-stack-unchanged exact-native-row-fingerprint progression-and-real-save-queues-conserved no-operation no-shared-quarantine");
			}
			template.SelectedMerits.Clear(); template.SelectedRoles.Clear();
			var nativeService = world.SpellOwnedNpcs;
			foreach (var unsupported in new[] { "withdrawn", "wrong-world", "unavailable-service" })
			{
				if (unsupported == "withdrawn") SetPrivateMember(template, "Status", RevisionStatus.UnderDesign);
				if (unsupported == "wrong-world") SetPrivateField(template, "_gameworld", Mock.Of<IFuturemud>());
				if (unsupported == "unavailable-service") native.WorldMock.SetupGet(x => x.SpellOwnedNpcs).Returns((ISpellOwnedNpcService)null!);
				try
				{
					var rows = AdmissionDatabaseFingerprint(database); var queues = Queues();
					var acquired = service.Acquisition(actor, spell.Id); var opportunity = castingStore.Opportunity(actor.Id, skill.Id);
					var refused = service.Cast(new(actor, cap.Id, spell.Id, 3, true, ""));
					Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null && actor.MagicResourceAmounts[native.Resource] == 100 &&
						stack.Object.Quantity == 3 && samples == 0 && service.Acquisition(actor, spell.Id) == acquired && castingStore.Opportunity(actor.Id, skill.Id) == opportunity &&
						AdmissionDatabaseFingerprint(database) == rows && SameQueues(queues) && world.SaveManager.IsQueued(sentinel.Object),
						$"The known {unsupported} configuration paid, wrote an operation or changed native/progression/queue state: {refused.Message}");
					Console.WriteLine($"ARM03B2A-admission-{unsupported}=passed configured-native-refusal-before-payment no-operation exact-native-rows resource-material-progression-and-save-queues-conserved");
				}
				finally
				{
					SetPrivateMember(template, "Status", RevisionStatus.Current); SetPrivateField(template, "_gameworld", world);
					native.WorldMock.SetupGet(x => x.SpellOwnedNpcs).Returns(nativeService);
				}
				Require(service.Quote(new(actor, cap.Id, control.Id, 3, true, "self")).Allowed, $"The {unsupported} refusal quarantined a shared route.");
			}
			sentinel.Verify(x => x.Save(), Times.Never); world.SaveManager.Abort(sentinel.Object);
			var paid = service.Cast(new(actor, cap.Id, control.Id, 3, true, "self"));
			Console.WriteLine($"ARM03B2A-admission-control-observed=status:{paid.Status} balance:{actor.MagicResourceAmounts[native.Resource]} material:{stack.Object.Quantity} samples:{samples} grade:{service.Acquisition(actor, control.Id)?.ControlledGrade}");
			Require(paid.Status == MagicCastingStatus.Succeeded && actor.MagicResourceAmounts[native.Resource] == 77.5 && stack.Object.Quantity == 2 && samples == 1,
				"The same trait/reserve control did not consume exactly one material and 22.5 resource units: " + paid.Message);
			using var read = NewIndependentContext(database.ConnectionString);
			Require(read.MagicCastingOperations.Count() == 1 && read.MagicCastingOperations.Single().Stage == "Completed" &&
				read.CharacterMagicSkillOpportunities.Any() && !read.MagicSpellLifecycles.Any(), "The safe control lost its completed receipt or progression opportunity.");
			Console.WriteLine("ARM03B2A-admission-control=passed same-trait-and-reserve actual-configured-payment-and-material-consumption one-completed-operation progression-available");
		}
		finally
		{
			world.SaveManager.Abort(sentinel.Object);
			template.SelectedMerits.Clear(); template.SelectedMerits.AddRange(originalMerits);
			template.SelectedRoles.Clear(); template.SelectedRoles.AddRange(originalRoles);
			SetPrivateField(native.Body, "_heldItems", priorHeld);
			actor.RemoveAllEffects<MagicSpellParent>(null, true); actor.RemoveAllEffects<MagicSpellLockout>(null, true);
		}
		Console.WriteLine("ARM03B2A-admission-qualifier=real-native-casting-service-store-skill-body-save-manager-and-loaded-merits controlled-material-item-stack-and-world-host all-owned-DB-rows-compared before-refusal no-extra-body-support no-full-installed-session");
	}

	private static string AdmissionDatabaseFingerprint(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var connection = db.Database.GetDbConnection(); connection.Open();
		var tables = db.Model.GetEntityTypes().Select(x => x.GetTableName()).Where(x => x is not null).Distinct().Order().ToArray();
		var fingerprint = new StringBuilder();
		foreach (var table in tables)
		{
			using var command = connection.CreateCommand(); command.CommandText = $"SELECT * FROM `{table}`";
			using var reader = command.ExecuteReader(); var rows = new List<string>();
			while (reader.Read())
			{
				var values = new string?[reader.FieldCount];
				for (var i = 0; i < values.Length; i++) values[i] = reader.IsDBNull(i) ? null : reader.GetValue(i) switch
				{
					byte[] bytes => Convert.ToBase64String(bytes), DateTime date => date.ToString("O", CultureInfo.InvariantCulture),
					IFormattable value => value.ToString(null, CultureInfo.InvariantCulture), var value => value.ToString()
				};
				rows.Add(JsonSerializer.Serialize(values));
			}
			fingerprint.AppendLine(table);
			foreach (var row in rows.Order(StringComparer.Ordinal)) fingerprint.AppendLine(row);
		}
		return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(fingerprint.ToString())));
	}
}
