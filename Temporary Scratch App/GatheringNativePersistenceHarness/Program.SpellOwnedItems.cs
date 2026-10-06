#nullable enable

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Shops;
using MudSharp.Effects.Concrete;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts.Inputs;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record ItemReader(string Database, FixtureIds Fixture, DateTime Now, long Temporary, long Permanent,
		Guid TemporaryLifecycle, Guid PermanentLifecycle, DateTime Deadline, long ForeignBag, long ForeignChild,
		long FaultItem, Guid FaultLifecycle, Guid FaultInvocation, long Spell, long Capability, double PaidBalance);
	private sealed record RemovalReader(string Database, FixtureIds Fixture, DateTime Now, long Item, Guid Lifecycle, long Bag, long Sibling);
	private sealed class ItemLifetimeRandom : Random
	{
		public int Draws { get; private set; }
		public override int Next(int minValue, int maxValue) => ++Draws == 1 ? 1 : 0;
	}

	private static void SeedCreatedWeaponPrototypes(TestDatabase database, long material)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var skill = db.TraitDefinitions.Single(x => x.Name == "ARM02 Earth Proficiency");
		var type = new Db.WeaponType { Name = "ARM03C1 replacement fixture weapon profile", Classification = (int)WeaponClassification.Lethal,
			AttackTraitId = skill.Id, ParryTraitId = skill.Id, Reach = 1, StaminaPerParry = 1 };
		var tag = new Db.Tag { Name = "ARM03C1 creation component" }; db.WeaponTypes.Add(type); db.Tags.Add(tag); db.SaveChanges();
		var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
		long componentId = db.GameItemComponentProtos.Max(x => x.Id), prototypeId = db.GameItemProtos.Max(x => x.Id);
		Db.GameItemComponentProto Component(string kind, string definition)
		{
			var model = new Db.GameItemComponentProto { Id = ++componentId, Name = "ARM03B2B " + kind, Type = kind, Description = "ARM03C1 controlled replacement fixture",
				Definition = definition, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(model); db.SaveChanges(); return model;
		}
		var melee = Component("MeleeWeapon", $"<Definition><WeaponType>{type.Id}</WeaponType></Definition>");
		var salvage = Component("Salvageable", $"<Definition><Trait>{skill.Id}</Trait><Difficulty>5</Difficulty><ToolTag>0</ToolTag><Stages><Stage delay='1'>Salvage fixture</Stage></Stages><CommodityProducts><Product material='{material}' tag='0' fraction='true' success='0.5' failure='0.25'/></CommodityProducts><ItemProducts/></Definition>");
		foreach (var name in new[] { "flame knife", "mon staff", "creation token" })
		{
			var proto = new Db.GameItemProto { Id = ++prototypeId, Name = "ARM03B2B " + name, Keywords = name, ShortDescription = "a fixture " + name,
				FullDescription = "ARM03C1 controlled replacement content; no historical weapon statistics or event-unit conversion is asserted.",
				MaterialId = material, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard,
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = hold.Id });
			if (name != "creation token")
			{
				proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = melee.Id });
				proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = salvage.Id });
			}
			else proto.GameItemProtosTags.Add(new() { TagId = tag.Id });
			db.GameItemProtos.Add(proto); db.SaveChanges();
		}
	}

	private static int RunSpellOwnedItemChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		Console.WriteLine($"ARM03C1-created={database.Name}");
		var fixture = FixtureSeed.Create(database, "arm03c1_created_items", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) { db.Database.Migrate(); Require(!db.Database.HasPendingModelChanges(), "Item adapter model parity failed."); }
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true); var native = host.Native; var world = native.World; var actor = native.Actor;
		Mock.Get(native.Body.Race).Setup(x => x.GetMaximumLiftWeight(It.IsAny<ICharacter>())).Returns(10000);
		var items = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(items);
		var knife = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B flame knife");
		var staffProto = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B mon staff");
		var tokenProto = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B creation token");
		var source = new MagicSpell("ARM03C1 native ownership source fixture", native.Capability.School);
		((All<IMagicSpell>)world.MagicSpells).Add(source); world.SaveManager.Flush();
		GameItem New(GameItemProto proto)
		{
			var item = (GameItem)proto.CreateNew(actor); world.Add(item); actor.Location.Insert(item, true); item.Login(); world.SaveManager.Flush(); return item;
		}
		void Ground(IGameItem item) { item.InInventoryOf?.Take(item); item.ContainedIn?.Take(item); actor.Location.Insert(item, true); world.SaveManager.Flush(); }
		SpellLifecycleOrigin Origin(SpellLifecycleMode mode = SpellLifecycleMode.TemporaryCleanup, long? spell = null) =>
			new(Guid.NewGuid(), spell ?? source.Id, 3, actor.Id, "created-weapon", mode, RuntimeClock.UtcNow,
				mode == SpellLifecycleMode.Permanent ? null : RuntimeClock.UtcNow.AddSeconds(60), "ARM03C1 exact native leaf item fixture");
		(int Items, int Components, int Lifecycles) Rows()
		{ using var db = NewIndependentContext(database.ConnectionString); return (db.GameItems.Count(), db.GameItemComponents.Count(), db.MagicSpellLifecycles.Count()); }
		var sentinel = new Mock<ISaveable>(); sentinel.Setup(x => x.Save()).Throws(new InvalidOperationException("Private item creation flushed unrelated dirty state."));
		world.SaveManager.Add(sentinel.Object);
		var readOnlyRows = Rows();
		using (FMDB.BeginIsolatedScope(suppressEfWrites: true))
		{
			var caller = FMDB.Context;
			Refuse(() => items.Create(knife, actor, ItemQuality.Standard, Origin()), "Read-only native item creation escaped its caller's write suppression.");
			Require(ReferenceEquals(caller, FMDB.Context) && FMDB.WritesAreSuppressed, "Refused private creation replaced the read-only caller scope.");
		}
		Require(Rows() == readOnlyRows && world.SaveManager.IsQueued(sentinel.Object), "Suppressed item creation changed rows or unrelated queues.");
		Console.WriteLine("ARM03C1-readonly-authority=passed native-write-suppression-before-item-construction caller-context-preserved native-rows-and-unrelated-dirty-queue-conserved");
		var born = Origin(); var privateItem = items.Create(knife, actor, ItemQuality.Good, born);
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(privateItem.Id > 0 && privateItem.Components.All(x => x.Id > 0) && privateItem.RawQuality == ItemQuality.Good &&
				db.GameItems.Find(privateItem.Id)!.Quality == (int)ItemQuality.Good && db.GameItemComponents.Count(x => x.GameItemId == privateItem.Id) == 3 &&
				db.MagicSpellOwnedEntities.Count(x => x.LifecycleId == born.Id && x.EntityId == privateItem.Id) == 1 &&
				!host.Items.Has(privateItem.Id) && world.SaveManager.IsQueued(sentinel.Object) && !world.SaveManager.IsQueued((ISaveable)privateItem) &&
				db.MagicSpellLifecycles.Find(born.Id)!.Diagnostic == "", "Native private item/components were exposed or queued before exact ownership and activation committed.");
		Refuse(() => items.Create(knife, actor, ItemQuality.Good, born), "Native item origin replay created a duplicate.");
		var beforeFailure = Rows(); Refuse(() => items.Create(knife, actor, ItemQuality.Standard, Origin(spell: long.MaxValue)), "Unpersisted source spell admitted native rows.");
		Require(Rows() == beforeFailure && world.SaveManager.IsQueued(sentinel.Object), "Failed native creation changed rows or unrelated save queues.");
		world.SaveManager.Abort(sentinel.Object); world.Add(privateItem); actor.Location.Insert(privateItem, true);
		privateItem.Delete(); Require(privateItem.Deleted && host.Store.Find(born.Id)!.Reason == SpellRetirementReason.EarlyItemRemoval &&
			host.Store.Find(born.Id)!.State == SpellLifecycleState.Completed, "Early native deletion did not terminally retire its one item.");
		Console.WriteLine("ARM03C1-private-creation=passed real-native-prototype-item-three-components atomic-exact-claim registered-identities quality-before-exposure no-global-flush no-replay rollback early-native-Delete completion");

		var cap = (SkillLevelBasedMagicCapability)native.Capability; var skill = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var spell = new MagicSpell("ARM03C1 controlled Flame Knife tier fixture", cap.School); ((All<IMagicSpell>)world.MagicSpells).Add(spell);
		foreach (var command in new[] { "trigger new room", $"trait {skill.Id}", "difficulty easy", "threshold minorpass", "duration ARM02 Duration",
			$"cost {native.Resource.Id} ARM02 Cost", $"prog {world.FutureProgs.GetByName("rejuvenation_known")!.Id}",
			"castemote A controlled fixture weapon appears.", "failcastemote The controlled fixture fails.", "grades fixture", "effect add createitem",
			$"effect 1 item {knife.Id}", "effect 1 lifecycle temporarycleanup", "effect 1 family flame-knife", "effect 1 lifetime 20*grade",
			$"effect 1 permanent 7 {staffProto.Id}" }) Require(spell.BuildingCommand(actor, new StringStack(command)), $"Item fixture builder refused {command}.");
		spell.InventoryPlanTemplate = new InventoryPlanTemplate(world, new InventoryPlanActionConsume(world, 1, tokenProto.Tags.Single().Id, 0, _ => true, _ => true));
		foreach (var command in new[] { $"casting trait {skill.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive",
			$"casting entry add {spell.Id}", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(actor, new StringStack(command)), $"Item fixture casting policy refused {command}.");
		actor.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(MudSharp.Accounts.PermissionLevel.JuniorAdmin)).Returns(true);
		var service = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => { world.SaveManager.Flush(); FlushCasting(native); });
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(service);
		var enrolled = service.Enrol(staff.Object, actor, cap.Id, "Native item checkpoint");
		Require(enrolled.Allowed, "Item fixture enrolment refused: " + enrolled.Message);
		new MagicCastingStateStore().Write(acquired: service.Acquisition(actor, spell.Id)! with { ControlledGrade = 7 });
		actor.SetTraitValue(skill, 100); actor.AddResource(native.Resource, 100); FlushCasting(native);
		var token = New(tokenProto); var balance = actor.MagicResourceAmounts[native.Resource];
		var hook = new Mock<IDefaultHook>(); hook.SetupGet(x => x.PerceivableType).Returns("GameItem");
		native.WorldMock.SetupGet(x => x.DefaultHooks).Returns([hook.Object]);
		var refusalRows = Rows(); var refused = service.Cast(new(actor, cap.Id, spell.Id, 3, false, ""));
		Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null && refused.Message.Contains("default hooks") &&
			!token.Deleted && actor.MagicResourceAmounts[native.Resource] == balance && Rows() == refusalRows, "Known unsupported default hook spent payment or material.");
		hook.Verify(x => x.Applies(It.IsAny<MudSharp.FutureProg.IProgVariable>(), It.IsAny<string>()), Times.Never);
		native.WorldMock.SetupGet(x => x.DefaultHooks).Returns(Array.Empty<IDefaultHook>());
		Require(spell.BuildingCommand(actor, new StringStack("effect 1 lifetime 0")), "Zero lifetime fixture edit failed.");
		refused = service.Cast(new(actor, cap.Id, spell.Id, 3, false, ""));
		Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null && !token.Deleted && actor.MagicResourceAmounts[native.Resource] == balance && Rows() == refusalRows,
			"Known zero lifetime spent payment or material.");
		Require(spell.BuildingCommand(actor, new StringStack("effect 1 lifetime 20*grade")), "Lifetime fixture repair failed.");
		Console.WriteLine("ARM03C1-admission=passed configured-native-route default-hook-refusal-without-eligibility-program zero-lifetime-refusal no-operation no-resource-or-real-component-payment native-rows-conserved");
		token.Delete();
		var temporaryToken = items.Create(tokenProto, actor, ItemQuality.Standard, Origin());
		world.Add(temporaryToken); actor.Location.Insert(temporaryToken, true); temporaryToken.Login(); world.SaveManager.Flush();
		native.Body.Get(temporaryToken, silent: true);
		Require(native.Body.HeldItems.Contains(temporaryToken) && temporaryToken.IsA(tokenProto.Tags.Single()), "Temporary component was not an actually held, matching native material.");
		var beforeTemporaryComponent = Rows(); refused = service.Cast(new(actor, cap.Id, spell.Id, 3, false, ""));
		Require(refused.Status == MagicCastingStatus.Refused && refused.OperationId is null && !temporaryToken.Deleted &&
			actor.MagicResourceAmounts[native.Resource] == balance && Rows() == beforeTemporaryComponent,
			"Temporary casting material reached payment or created permanent output.");
		Ground(temporaryToken); token = New(tokenProto); native.Body.Get(token, silent: true);
		Require(native.Body.HeldItems.Contains(token), "Ordinary creation material was not actually held.");
		Console.WriteLine("ARM03C1-component-conservation=passed real-temporary-tagged-component-only prepayment-refusal no-operation-or-debit ordinary-native-component-admitted");
		Require(spell.BuildingCommand(actor, new StringStack("effect 1 lifetime rand(0,1)")), "Random lifetime fixture edit failed.");
		var lifetimeRandom = new ItemLifetimeRandom();
		using (ExpressionEngine.Expression.PushRandom(lifetimeRandom))
		{
			var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
				.Invoke(spell, [actor, skill, 3, SpellPower.Weak, Difficulty.Easy, 7])!;
			var effect = (CreateItemEffect)copy.SpellEffects.Single(); object?[] admission = [actor, null];
			Require((bool)typeof(CreateItemEffect).GetMethod("ValidateInvocation", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(effect, admission)!,
				"The first positive native lifetime sample was not admitted.");
			Require(effect.TryPrepareApplication(actor, actor.Location, default, SpellPower.Weak, TimeSpan.FromSeconds(30), out var application, out _),
				"The prepared positive lifetime was sampled again after admission.");
			application!.Create(new MagicSpellParent(actor.Location, copy, actor, SpellPower.Weak, default));
			var sampleItem = host.Items.Single(x => x.SpellCreationOrigin?.IsTemporary == true && x.Id != temporaryToken.Id);
			Require(lifetimeRandom.Draws == 1 && sampleItem.SpellCreationOrigin!.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(1),
				"Native output did not bind its one admitted positive lifetime sample."); sampleItem.Delete();
		}
		Require(spell.BuildingCommand(actor, new StringStack("effect 1 lifetime 20*grade")), "Lifetime fixture restoration failed.");
		Console.WriteLine("ARM03C1-lifetime-binding=passed native-casting-copy prepared-createitem controlled-expression-RNG positive-admitted-sample-next-draw-zero exact-one-second-deadline one-draw-only");

		var result = service.Cast(new(actor, cap.Id, spell.Id, 3, false, ""));
		Require(result.Status == MagicCastingStatus.Succeeded, "Paid timed item casting failed: " + result.Message);
		var timed = host.Items.Single(x => x.SpellCreationOrigin?.IsTemporary == true && x.Prototype.Id == knife.Id);
		var timedLife = host.Store.Find(timed.SpellCreationOrigin!.LifecycleId)!;
		Require(token.Deleted && actor.MagicResourceAmounts[native.Resource] == balance - 15 && timedLife.Origin.Grade == 3 &&
			timedLife.Origin.SpellId == spell.Id && timedLife.Origin.DeadlineUtc == RuntimeClock.UtcNow.AddSeconds(60), "Timed grade output or actual native component/resource payment was incorrect.");
		Require(!temporaryToken.Deleted, "Casting consumed the temporary component instead of the ordinary one."); temporaryToken.Delete();
		native.Body.Get(timed, silent: true);
		Require(native.Body.Wield(timed, silent: true), "The actual created melee weapon could not be held and wielded.");
		timed.SetOwner(Mock.Of<ICharacter>(x => x.Id == 424242 && x.Gameworld == world)); world.SaveManager.Flush();
		Require(timed.SpellCreationOrigin.DeadlineUtc == timedLife.Origin.DeadlineUtc && native.Body.WieldedItems.Contains(timed), "Transfer/equipment changed lifetime authority.");
		Console.WriteLine("ARM03C1-timed-cast=passed actual-native-casting real-material-Delete actual-resource-debit selected-grade-three absolute-sixty-second-deadline real-Body-Get-Wield legal-title-transfer-preserves-authority");

		var parent = new MagicSpellParent(timed, spell, actor, SpellPower.ExtremelyWeak, default);
		var invis = new MudSharp.Effects.Concrete.SpellEffects.SpellInvisibilityEffect(timed, parent, null!); timed.AddEffect(invis); world.SaveManager.Flush();
		var deaths = 0; ((GameItem)timed).OnDeath += _ => deaths++;
		Require(ReferenceEquals(((GameItem)timed).Die(), timed) && !timed.Deleted && !timed.Destroyed && deaths == 0 && native.Body.WieldedItems.Contains(timed),
			"A held item destruction hold mutated custody, native destruction or death callbacks.");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.BodiesGameItems.Any(x => x.BodyId == native.Body.Id && x.GameItemId == timed.Id) &&
				host.Store.Find(timedLife.Origin.Id)!.State == SpellLifecycleState.Active, "Destruction hold changed persisted custody or removal intent.");
		timed.RemoveEffect(invis); Ground(timed);
		Console.WriteLine("ARM03C1-destruction-hold=passed actual-held-native-melee-item real-persistent-spell-invisibility effect hold-before-Die callbacks Destroyed flag inventory-mutation or durable-join change");
		var callbackWeapon = items.Create(knife, actor, ItemQuality.Standard, Origin()); world.Add(callbackWeapon); actor.Location.Insert(callbackWeapon, true);
		native.Body.Get(callbackWeapon, silent: true); Require(native.Body.Wield(callbackWeapon, silent: true), "Callback destruction weapon was not natively wielded.");
		var callbackParent = new MagicSpellParent(callbackWeapon, spell, actor, SpellPower.ExtremelyWeak, default);
		var callbackInvis = new MudSharp.Effects.Concrete.SpellEffects.SpellInvisibilityEffect(callbackWeapon, callbackParent, null!);
		var callbackDeaths = 0; ((GameItem)callbackWeapon).OnDeath += _ => { callbackDeaths++; callbackWeapon.AddEffect(callbackInvis); };
		world.SaveManager.Flush();
		Require(ReferenceEquals(((GameItem)callbackWeapon).Die(), callbackWeapon) && !callbackWeapon.Deleted && !callbackWeapon.Destroyed &&
			native.Body.WieldedItems.Contains(callbackWeapon) && callbackDeaths == 1, "A death callback dependency changed flags or native custody before refusal.");
		Require(ReferenceEquals(((GameItem)callbackWeapon).Die(), callbackWeapon) && callbackDeaths == 1, "Held native death callback repeated.");
		callbackWeapon.RemoveEffect(callbackInvis);
		Require(((GameItem)callbackWeapon).Die() is null && callbackWeapon.Deleted && callbackWeapon.Destroyed && callbackDeaths == 1,
			"Dependency release did not complete one native destruction without repeating death observers.");
		Console.WriteLine("ARM03C1-death-callback-conservation=passed clean-actual-wielded-native-weapon death-callback-adds-effect refusal-preserves-custody-and-Destroyed once-only-observer writable-release-actual-Delete");
		var detachWeapon = items.Create(knife, actor, ItemQuality.Standard, Origin()); world.Add(detachWeapon); actor.Location.Insert(detachWeapon, true);
		var lodgedForeign = New(host.Prototypes.Values.Single(x => x.Name == "ARM03B2B goods"));
		var wound = new SimpleWound(world, detachWeapon, 1, DamageType.Crushing, null!, null!, null!, null!); detachWeapon.AddWound(wound);
		native.Body.Get(detachWeapon, silent: true); Require(native.Body.Wield(detachWeapon, silent: true), "Detach-callback weapon was not natively wielded.");
		world.SaveManager.Flush();
		void IntroduceLodgedValue(InventoryState oldState, InventoryState newState, IGameItem changed)
		{ if (ReferenceEquals(changed, detachWeapon) && newState == InventoryState.Dropped) wound.Lodged = lodgedForeign; }
		native.Body.OnInventoryChange += IntroduceLodgedValue;
		try { Refuse(detachWeapon.Delete, "Custody callback introduced foreign value after leaf admission but native deletion continued."); }
		finally { native.Body.OnInventoryChange -= IntroduceLodgedValue; }
		Require(!detachWeapon.Deleted && !detachWeapon.Destroyed && native.Body.WieldedItems.Contains(detachWeapon) && !lodgedForeign.Deleted &&
			ReferenceEquals(wound.Lodged, lodgedForeign), "Post-detach dependency refusal lost foreign goods or original native custody.");
		world.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.GameItems.Any(x => x.Id == lodgedForeign.Id) && db.BodiesGameItems.Any(x => x.BodyId == native.Body.Id && x.GameItemId == detachWeapon.Id),
				"Post-detach refusal lost durable foreign goods or original equipped custody.");
		wound.Lodged = null!; world.SaveManager.Flush();
		var reacquired = false;
		void Reacquire(InventoryState oldState, InventoryState newState, IGameItem changed)
		{
			if (!ReferenceEquals(changed, detachWeapon) || newState != InventoryState.Dropped || reacquired) return;
			reacquired = true; native.Body.Get(detachWeapon, silent: true);
		}
		native.Body.OnInventoryChange += Reacquire;
		try { Refuse(detachWeapon.Delete, "Same-body reacquisition left a deleted item in raw body membership."); }
		finally { native.Body.OnInventoryChange -= Reacquire; }
		Require(reacquired && !detachWeapon.Deleted && native.Body.WieldedItems.Count(x => ReferenceEquals(x, detachWeapon)) == 1 &&
			ReferenceEquals(detachWeapon.InInventoryOf, native.Body) && !native.Body.HeldItems.Contains(detachWeapon),
			"Reacquisition refusal did not restore exact original wielded membership without phantom held entries.");
		world.SaveManager.Flush(); detachWeapon.Delete();
		Require(detachWeapon.Deleted && !lodgedForeign.Deleted, "Released native wound dependency removed foreign material.");
		Console.WriteLine("ARM03C1-detach-callback-conservation=passed real-Body-OnInventoryChange native-SimpleWound foreign-lodged-item post-detach-revalidation rollback original-wielded-custody normal-save foreign-value-preserved exact-release");
		Require(!native.Body.AllItems.Any(x => x.Deleted), "Native body retained a deleted creation after callback release.");
		Console.WriteLine("ARM03C1-reacquisition-conservation=passed actual-same-body-Get-in-Dropped-callback raw-membership-gate rollback original-wielded-entry once-only no-phantom-Held-item normal-save successful-exact-release");

		var foreignBag = New(host.Prototypes.Values.Single(x => x.Name == "ARM03B2B bag"));
		var foreignChild = New(host.Prototypes.Values.Single(x => x.Name == "ARM03B2B goods"));
		foreignBag.GetItemType<IContainer>()!.Put(null, foreignChild, false); foreignBag.GetItemType<IContainer>()!.Put(null, timed, false); world.SaveManager.Flush();
		var salvage = timed.GetItemType<ISalvageable>()!;
		Require(!salvage.CanSalvage(out _), "Temporary melee item admitted salvage.");
		Refuse(() => salvage.CreateProducts(actor, true), "Direct native salvage created permanent products.");
		Refuse(() => timed.DeepCopy(true, true), "Temporary native copy created permanent material.");
		Refuse(() => new BaseInput.SimpleItemInputData([timed], 1), "Temporary native craft material was reserved.");
		var shop = (PermanentShop)RuntimeHelpers.GetUninitializedObject(typeof(PermanentShop));
		var payment = new Mock<IPaymentMethod>(MockBehavior.Strict);
		Refuse(() => shop.AddToStock(actor, timed, null!), "Temporary native stock admission changed stock.");
		Refuse(() => shop.Sell(actor, null!, payment.Object, timed), "Temporary native sale reached payment.");
		Refuse(() => shop.AddToStock(actor, foreignBag, null!), "Temporary child escaped a foreign container stock guard.");
		Require(!shop.CanBuyExact(actor, null!, 1, payment.Object, [foreignBag]).Truth, "Stocked foreign container with a temporary child admitted purchase.");
		payment.VerifyNoOtherCalls();
		Require(!timed.CanMerge(foreignChild) && !timed.Deleted && !foreignBag.Deleted && !foreignChild.Deleted &&
			foreignBag.GetItemType<IContainer>()!.Contents.Contains(timed) && foreignBag.GetItemType<IContainer>()!.Contents.Contains(foreignChild), "Value refusal changed native foreign custody.");
		Console.WriteLine("ARM03C1-value-conservation=passed actual-native-weapon-salvage-quote-and-direct-products copy craft shop-stock-sale-and-nested-purchase-guards no-payment-or-custody-mutation conservative-conversion-refusals");

		var monToken = New(tokenProto); native.Body.Get(monToken, silent: true); balance = actor.MagicResourceAmounts[native.Resource];
		result = service.Cast(new(actor, cap.Id, spell.Id, 7, false, ""));
		Require(result.Status == MagicCastingStatus.Succeeded, "Paid mon output failed: " + result.Message);
		var permanent = host.Items.Single(x => x.SpellCreationOrigin?.Mode == SpellLifecycleMode.Permanent && x.Prototype.Id == staffProto.Id);
		var permanentLife = host.Store.Find(permanent.SpellCreationOrigin!.LifecycleId)!;
		Require(monToken.Deleted && actor.MagicResourceAmounts[native.Resource] == balance - 35 && permanentLife.Origin.Grade == 7 &&
			permanentLife.Origin.DeadlineUtc is null && permanentLife.State == SpellLifecycleState.Completed && permanent.GetItemType<ISalvageable>()!.CanSalvage(out _),
			"Paid mon staff did not remain ordinary permanent material with its exact grade receipt.");
		world.SaveManager.Flush();
		Console.WriteLine("ARM03C1-mon-cast=passed exact-configured-grade-seven distinct-real-native-staff real-component-consumption resource-debit ordinary-salvage-eligibility no-magical-deadline permanent-journal-released");
		var faultToken = New(tokenProto); native.Body.Get(faultToken, silent: true); var faultBefore = Rows(); var faultInvocation = Guid.NewGuid();
		balance = actor.MagicResourceAmounts[native.Resource];
		native.WorldMock.Setup(x => x.Add(It.Is<IGameItem>(x => x.SpellCreationOrigin != null))).Callback<IGameItem>(item =>
		{
			if (!host.Items.Has(item.Id)) host.Items.Add(item);
			throw new InvalidOperationException("ARM03C1 injected failure after exact committed item publication");
		});
		result = service.Cast(new(actor, cap.Id, spell.Id, 2, false, "", OriginId: faultInvocation));
		var faultItem = host.Items.Single(x => x.SpellCreationOrigin?.IsTemporary == true && x.Id != timed.Id);
		var faultLife = host.Store.Find(faultItem.SpellCreationOrigin!.LifecycleId)!;
		var faultRows = Rows();
		Require(result.Status == MagicCastingStatus.NeedsReview && result.OperationId == faultInvocation && faultToken.Deleted &&
			actor.MagicResourceAmounts[native.Resource] == balance - 10 && faultRows.Items == faultBefore.Items &&
			faultRows.Components == faultBefore.Components + 2 && faultRows.Lifecycles == faultBefore.Lifecycles + 1 &&
			faultLife.Origin.Provenance.Contains(faultInvocation.ToString()) && faultItem.Location is null,
			"Postcommit native exposure failure did not preserve exact item ownership and paid uncertainty.");
		var replay = service.Cast(new(actor, cap.Id, spell.Id, 2, false, "", OriginId: faultInvocation));
		var alternative = service.Cast(new(actor, cap.Id, spell.Id, 3, false, ""));
		Require(replay.Status == MagicCastingStatus.Refused && alternative.Status == MagicCastingStatus.Refused && Rows() == faultRows &&
			actor.MagicResourceAmounts[native.Resource] == balance - 10, "Paid item exposure fault replayed creation, refunded payment or escaped quarantine.");
		Console.WriteLine("ARM03C1-paid-exposure-fault=passed real-component-delete resource-debit private-item-and-components-exactly-once world-publication-failure NeedsReview same-origin-and-new-invocation-refused no-refund no-duplicate");

		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var item in new[] { foreignBag, permanent }) if (!db.CellsGameItems.Any(x => x.GameItemId == item.Id)) db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = item.Id });
			db.SaveChanges();
		}
		RunItemReaderProcess(new ItemReader(database.Name, fixture, RuntimeClock.UtcNow, timed.Id, permanent.Id, timedLife.Origin.Id, permanentLife.Origin.Id,
			timedLife.Origin.DeadlineUtc!.Value, foreignBag.Id, foreignChild.Id, faultItem.Id, faultLife.Origin.Id, faultInvocation, spell.Id, cap.Id,
			actor.MagicResourceAmounts[native.Resource]));
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(!db.GameItems.Any(x => x.Id == timed.Id) && db.GameItems.Any(x => x.Id == permanent.Id) &&
				db.GameItems.Find(foreignChild.Id)!.ContainerId == foreignBag.Id && db.GameItems.Any(x => x.Id == foreignBag.Id) &&
				host.Store.Find(timedLife.Origin.Id)!.State == SpellLifecycleState.Completed, "Restart expiry lost foreign contents or permanent output.");
		Console.WriteLine("ARM03C1-restart-expiry=passed separate-process native-container-reload persisted-absolute-deadline no-reset native-item-Delete completed-once foreign-bag-and-child-preserved permanent-staff-survives");
		Console.WriteLine("ARM03C1-acceptance=passed controlled-replacement-fixtures N21-native-boundary installed-stock-not-qualified food-liquid-host-adapters-and-event-unit-conversion-pending");
		return 0;
	}

	private static void RunItemReaderProcess<T>(T input, string mode = "--spell-owned-item-reader")
	{
		var start = new ProcessStartInfo("dotnet") { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		start.ArgumentList.Add(Assembly.GetExecutingAssembly().Location); start.ArgumentList.Add(mode);
		start.ArgumentList.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
		using var process = Process.Start(start)!; var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
		if (!process.WaitForExit(60000)) { process.Kill(true); throw new TimeoutException("Native item restart exceeded sixty seconds."); }
		Console.Write(output.GetAwaiter().GetResult()); Require(process.ExitCode == 0, error.GetAwaiter().GetResult());
	}

	private static int RunSpellOwnedItemReader(string[] args)
	{
		Require(args.Length == 1, "Item reader needs one receipt.");
		var input = JsonSerializer.Deserialize<ItemReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args[0])))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true); var world = host.Native.World;
		var service = new SpellOwnedItemService(world); host.Native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(service);
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1,
			flush: () => { world.SaveManager.Flush(); FlushCasting(host.Native); });
		var beforeReplay = host.Native.Actor.MagicResourceAmounts[host.Native.Resource];
		var replay = casting.Cast(new(host.Native.Actor, input.Capability, input.Spell, 2, false, "", OriginId: input.FaultInvocation));
		Require(replay.Status == MagicCastingStatus.Refused && beforeReplay == input.PaidBalance &&
			host.Native.Actor.MagicResourceAmounts[host.Native.Resource] == input.PaidBalance && world.TryGetItem(input.FaultItem, true) is { Deleted: false },
			"Restart replayed or refunded the paid item exposure fault.");
		Console.WriteLine("ARM03C1-reader-paid-quarantine=passed new-process persisted-casting-operation no-replay-or-refund exact-owned-committed-item-loadable");
		var item = world.TryGetItem(input.Temporary, true)!; var permanent = world.TryGetItem(input.Permanent, true)!;
		Require(item.SpellCreationOrigin?.DeadlineUtc == input.Deadline && item.ContainedIn is null && !host.Items.Has(input.ForeignBag) &&
			permanent.SpellCreationOrigin?.Mode == SpellLifecycleMode.Permanent && permanent.SpellCreationOrigin.DeadlineUtc is null,
			"Fresh native loading lost creation authority or did not establish the cold-custodian fixture.");
		service.ReconcileRetirements(RuntimeClock.UtcNow); Require(!item.Deleted, "Restart expired an item before its persisted deadline.");
		Console.WriteLine("ARM03C1-reader-before-deadline=passed new-process actual-native-GameItem-and-container-component loading no-replayed-creation no-lifetime-reset no-early-expiry permanent-origin-retained");
		clock.Advance(TimeSpan.FromSeconds(61)); service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(!item.Deleted && host.Store.Find(input.TemporaryLifecycle)!.State == SpellLifecycleState.Retiring &&
			host.Store.Find(input.TemporaryLifecycle)!.Diagnostic.Contains("Persisted custody"), "Cold foreign containment was removed without a loaded custodian.");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(db.GameItems.Find(input.Temporary)!.ContainerId == input.ForeignBag && db.GameItems.Any(x => x.Id == input.ForeignChild), "Cold containment hold mutated persisted foreign custody.");
		Console.WriteLine("ARM03C1-reader-cold-custody=passed expired-real-native-leaf persisted-foreign-container recoverable-Retiring diagnostic no-child-or-host-deletion-before-custodian-load");
		var bag = world.TryGetItem(input.ForeignBag, true)!; bag.FinaliseLoadTimeTasks();
		Require(ReferenceEquals(item.ContainedIn, bag) && bag.GetItemType<IContainer>()!.Contents.Contains(item), "Native custodian loading did not reconnect the exact held item.");
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			// The only variable is the already-owned native 64-bit identity, never caller SQL text.
			var deletionFaultSql = $"CREATE TRIGGER arm03c1_delete_fault BEFORE DELETE ON GameItems FOR EACH ROW BEGIN IF OLD.Id={input.Temporary} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='ARM03C1 final owned DELETE fault'; END IF; END";
			db.Database.ExecuteSqlRaw(deletionFaultSql);
		}
		try
		{
			service.ReconcileRetirements(RuntimeClock.UtcNow);
			Require(!item.Deleted && !item.Destroyed && ReferenceEquals(item.ContainedIn, bag) && bag.GetItemType<IContainer>()!.Contents.Contains(item) &&
				host.Store.Find(input.TemporaryLifecycle)!.Diagnostic.Contains("Native item reconciliation held"), "Failed final DELETE did not restore live custody.");
			using var db = NewIndependentContext(database.ConnectionString);
			Require(db.GameItems.Find(input.Temporary)!.ContainerId == input.ForeignBag &&
				db.GameItemComponents.Where(x => x.GameItemId == input.ForeignBag).Any(x => x.Definition.Contains($">{input.Temporary}<")),
				"Failed final DELETE persisted a detached foreign container before owning row removal.");
		}
		finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER arm03c1_delete_fault"); }
		world.SaveManager.Flush();
		RunItemReaderProcess(new RemovalReader(input.Database, input.Fixture, RuntimeClock.UtcNow, input.Temporary, input.TemporaryLifecycle, input.ForeignBag, input.ForeignChild),
			"--spell-owned-item-removal-reader");
		Console.WriteLine("ARM03C1-reader-removal-rollback=passed actual-MySQL-final-DELETE-fault atomic-custodian-and-item-transaction native-rollback normal-save separate-process-reload exact-original-contained-item foreign-bag-and-sibling-conserved");
		service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(item.Deleted && !permanent.Deleted && !bag.Deleted && bag.GetItemType<IContainer>()!.Contents.All(x => x.Id != input.Temporary) &&
			bag.GetItemType<IContainer>()!.Contents.Any(x => x.Id == input.ForeignChild), "Native restart expiry did not detach only its exact item.");
		var version = host.Store.Find(input.TemporaryLifecycle)!.Version; service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(host.Store.Find(input.TemporaryLifecycle)!.Version == version && host.Store.Find(input.PermanentLifecycle)!.State == SpellLifecycleState.Completed,
			"Repeated item reconciliation reopened or rewrote terminal creation.");
		world.SaveManager.Flush();
		Console.WriteLine("ARM03C1-reader-expired=passed actual-native-Delete exact-item-and-component-rows-gone foreign-container-and-other-goods-retained normal-save repeated-terminal-idempotence no-permanent-expiry");
		using (var db = NewIndependentContext(database.ConnectionString))
			Require(!db.GameItems.Any(x => x.Id == input.FaultItem) && host.Store.Find(input.FaultLifecycle)!.State == SpellLifecycleState.Completed,
				"Paid exposure fault did not safely retire its exact abandoned temporary output after expiry.");
		return 0;
	}

	private static int RunSpellOwnedItemRemovalReader(string[] args)
	{
		Require(args.Length == 1, "Removal reader needs one receipt.");
		var input = JsonSerializer.Deserialize<RemovalReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args[0])))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true);
		host.Native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(new SpellOwnedItemService(host.Native.World));
		var bag = host.Native.World.TryGetItem(input.Bag, true)!; bag.FinaliseLoadTimeTasks();
		var item = host.Native.World.TryGetItem(input.Item, true)!;
		Require(!item.Deleted && !item.Destroyed && ReferenceEquals(item.ContainedIn, bag) &&
			bag.GetItemType<IContainer>()!.Contents.Any(x => x.Id == input.Item) && bag.GetItemType<IContainer>()!.Contents.Any(x => x.Id == input.Sibling) &&
			host.Store.Find(input.Lifecycle)!.State == SpellLifecycleState.Retiring, "Fresh native reload did not conserve the failed removal's exact custody and durable intent.");
		Console.WriteLine("ARM03C1-removal-fault-reader=passed new-process real-native-container-and-leaf reload after-final-DELETE-fault-and-normal-save same-origin same-foreign-container sibling-intact recoverable-retirement");
		return 0;
	}
}
