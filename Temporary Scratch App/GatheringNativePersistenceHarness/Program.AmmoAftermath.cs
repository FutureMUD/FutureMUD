#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Form.Audio;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private class AmmoInitialisationObserver : DispatchProxy
	{
		public AmmoInitialisationObserver() { }
		internal MudSharp.Framework.Save.ISaveManager Inner = null!;
		internal Action<GameItem>? Observe;
		protected override object? Invoke(MethodInfo? method, object?[]? arguments)
		{
			try
			{
				var result = method!.Invoke(Inner, arguments);
				if (method.Name == "AddInitialisation" && arguments![0] is GameItem item) Observe?.Invoke(item);
				return result;
			}
			catch (TargetInvocationException error) when (error.InnerException is not null)
			{ System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error.InnerException).Throw(); throw; }
		}
	}

	private static void SeedAmmoAftermathFixture(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var hold = db.GameItemComponentProtos.Single(x => x.Name == "ARM03B2B Holdable");
		var ammo = db.GameItemComponentProtos.Single(x => x.Name == "ARMRegression Ammunition");
		var original = db.GameItemProtos.Single(x => x.Name == "ARMRegression round");
		var ids = new System.Collections.Generic.Dictionary<string, long>();
		foreach (var name in new[] { "projectile", "casing", "aftermath-round" })
		{
			var row = new MudSharp.Models.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "ARMRegression " + name, Keywords = name,
				ShortDescription = "a regression " + name, FullDescription = "An ordinary disposable native ammunition lifecycle item.", MaterialId = original.MaterialId,
				Size = original.Size, Weight = original.Weight, BaseItemQuality = original.BaseItemQuality, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
			row.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = hold.Id });
			if (name == "aftermath-round") row.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = ammo.Id });
			db.GameItemProtos.Add(row); db.SaveChanges(); ids[name] = row.Id;
		}
		var xml = XElement.Parse(ammo.Definition); xml.Element("BulletProto")!.Value = ids["projectile"].ToString(CultureInfo.InvariantCulture); xml.Element("CasingProto")!.Value = ids["casing"].ToString(CultureInfo.InvariantCulture); ammo.Definition = xml.ToString();
		var gun = db.GameItemComponentProtos.Single(x => x.Name == "ARMRegression InternalMagazineGun"); xml = XElement.Parse(gun.Definition);
		xml.Element("CycleType")!.Value = "SelfLoading"; xml.Add(new XElement("EjectOnFire", false), new XElement("FireModes", new XElement("Mode", new XAttribute("type", "Single"), new XAttribute("rounds", 1)))); gun.Definition = xml.ToString(); db.SaveChanges();
	}

	private static int RunAmmoAftermath(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		using var globals = new CheckLearningGlobals();
		var native = host.Native; var world = native.World; var service = world.SpellOwnedCorpseAnimations!;
		var originalSave = world.SaveManager;
		var save = DispatchProxy.Create<MudSharp.Framework.Save.ISaveManager, AmmoInitialisationObserver>(); var observer = (AmmoInitialisationObserver)save; observer.Inner = originalSave;
		native.WorldMock.SetupGet(x => x.SaveManager).Returns(save);
		typeof(Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1000", world));
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1", world));
		typeof(RangedWeaponAttackBase).GetField("_targetExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new Expression("0"));
		foreach (var (name, value) in new[] { ("EncumbranceLimitRatioHeavy", 0.8), ("EncumbranceLimitRatioModerate", 0.5), ("EncumbranceLimitRatioLight", 0.25) }) native.WorldMock.Setup(x => x.GetStaticDouble(name)).Returns(value);
		var settings = (CharacterCombatSettings)caster.CombatSettings;
		settings.WeaponUsePercentage = 1; settings.NaturalWeaponPercentage = settings.AuxiliaryPercentage = settings.MagicUsePercentage = settings.PsychicUsePercentage = 0;
		foreach (var race in world.Races)
		{
			Mock.Get(race).SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true, CanUseWeapons = true, CanDefend = false, DefaultCombatSetting = settings });
			Mock.Get(race).SetupGet(x => x.RaceUsesStamina).Returns(true);
		}
		var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		var configured = new HashSet<CommandableAI>();
		var selected = Environment.GetEnvironmentVariable("FUTUREMUD_AMMO_ACCEPTANCE_CASE"); var first = true;
		var selectedCases = selected?.Split(',');
		var scenarios = new[] { "ordered-valid", "queued-revoked", "cycle-expire", "cycle-foreign-container", "cycle-policy-foreign-container", "postcommit-expire", "postcommit-casing-claim", "direct-valid" };
		Require(selected is null or "aftermath" || selectedCases!.All(x => scenarios.Contains(x.Replace("aftermath-", ""))), "Every requested aftermath scenario must be known and executed.");
		foreach (var scenario in scenarios.Where(x => selected == "aftermath" || selectedCases?.Contains("aftermath-" + x) == true || string.IsNullOrEmpty(selected)))
		{
			var actor = first ? animated : cast(); first = false; var body = (Body)actor.Body; actor.CombatSettings = settings;
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included fire")), "Allowlist the actual selected Fire command.");
			var unrelated = body.HeldItems.ToArray(); foreach (var item in unrelated) body.Drop(item, silent: true);
			GameItem New(string name)
			{
				var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
				world.Add(item); caster.Location.Insert(item, true); item.Login(); item.SetOwner(caster); world.SaveManager.Flush(); return item;
			}
			var generated = new List<GameItem>();
			observer.Observe = item => { if (item.Prototype.Name is "ARMRegression projectile" or "ARMRegression casing") generated.Add(item); };
			var bag = scenario is "cycle-foreign-container" or "cycle-policy-foreign-container" ? New("ARM03B2B bag") : null;
			var gunItem = New("ARMRegression gun"); var rounds = new[] { New("ARMRegression aftermath-round"), New("ARMRegression aftermath-round") };
			var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
			body.GetWithoutMerge(gunItem);
			foreach (var round in rounds) { body.GetWithoutMerge(round); gun.Load(actor); }
			Require(gun.MagazineContents.Count() == 2 && rounds.All(x => x.ContainedIn == gunItem), "Native Load must conserve and contain both exact ordinary rounds.");
			Require(gun.Ready(actor) && gun.ChamberedRound is not null, "Native Ready must chamber the exact first round.");
			Require(actor.SetTraitValue(trait, 40), "Native reaction trait baseline setup failed.");
			var canonical = ((ICharacter)actor.GetTrait(trait).Owner).Id;
			body.CurrentStamina = 100; world.SaveManager.Flush();
			void Read(string stage, long? staleChamber = null)
			{
				world.SaveManager.Flush();
				var items = rounds.Concat(generated).Concat(bag is null ? [] : new[] { bag }).Append(gunItem).Where(x => !x.Deleted).Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference, x.GetItemType<IHoldable>()!.HeldBy?.Id, x.DirectLocation?.Id, x.ContainedIn?.Id, Quantity: x.Quantity, Prototype: x.Prototype.Id)).ToArray();
				RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, canonical, body.Id, actor.CurrentStamina,
					trait.Id, actor.TraitRawValue(trait), foe.Body.Id, foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun),
					gunItem.Id, gunItem.Condition, gun.ChamberedRound?.Parent.Id, gun.MagazineContents.Select(x => x.Id).ToArray(), items, "ammo-aftermath-" + scenario + "-" + stage, staleChamber, Casing: gun.ChamberedCasing?.Id, DeletedItems: rounds.Where(x => x.Deleted).Select(x => x.Id).ToArray()), "--firearm-authority-reader");
			}

			var shot = (GameItem)gun.ChamberedRound!.Parent; var spare = gun.MagazineContents.Single();
			Require(rounds.Contains(shot) && rounds.Contains(spare) && shot != spare && rounds.All(x => x.ContainedIn == gunItem), "Native reload must conserve exact distinct chamber and magazine identities.");
			Read("loaded-chambered");
			void Expire()
			{
				var origin = service.CommandGrant(actor.InstanceId, caster.Id)!;
				var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
				clock.Advance(DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) - RuntimeClock.UtcNow);
				Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Expire only original command control, preserving physical body.");
			}
			var firing = false;
			shot.OnDeleted += _ =>
			{
				if (!firing) return;
				Require(generated.Count == 2, "Capture the exact two native constructor-created projectile/casing participants through native parent initialisation.");
				if (scenario is "postcommit-expire" or "postcommit-casing-claim")
				{
					Expire();
					if (scenario == "postcommit-casing-claim")
					{
						using var independent = CommandExecutionScope.EnterIndependent();
						var shell = generated.Single(x => x.Prototype.Name == "ARMRegression casing");
						Require(ReferenceEquals(body.GetWithoutMerge(shell), shell), "Independent reaction claims the exact new casing.");
					}
				}
			};
			var originalOutput = actor.OutputHandler; var cycleOutputs = 0;
			var output = new Moq.Mock<MudSharp.PerceptionEngine.IOutputHandler>(); output.SetupGet(x => x.Perceiver).Returns(actor);
			output.Setup(x => x.Send(It.IsAny<MudSharp.PerceptionEngine.IOutput>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<MudSharp.PerceptionEngine.IOutput, bool, bool>((message, newline, nopage) =>
			{
				if (scenario == "cycle-expire" && cycleOutputs == 0 && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(InternalMagazineGunGameItemComponent) && x.GetMethod()?.Name == "ChamberRound"))
				{ ++cycleOutputs; Require(gun.ChamberedCasing is not null && gun.ChamberedRound is null && gun.MagazineContents.Single() == spare, "Expiry observes the exact casing before cycling detach."); Expire(); }
				if (scenario == "cycle-foreign-container" && cycleOutputs == 0 && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(InternalMagazineGunGameItemComponent) && x.GetMethod()?.Name == "ChamberRound"))
				{
					++cycleOutputs; using var independent = CommandExecutionScope.EnterIndependent(); var shell = gun.ChamberedCasing!;
					shell.ContainedIn = null; bag!.GetItemType<IContainer>()!.Put(actor, shell);
					Require(shell.ContainedIn == bag && bag.GetItemType<IContainer>()!.Contents.Contains(shell), "Independent cycling output callback claims exact casing in a real native foreign bag.");
				}
				originalOutput.Send(message, newline, nopage);
			}).Returns(true);
			SetPrivateMember(actor, "OutputHandler", output.Object);
			order(actor, caster, "hit opponent"); actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
			actor.MeleeRange = false; foe.MeleeRange = false; actor.TargettedBodypart = foe.Body.Bodyparts.OfType<IExternalBodypart>().First();
			actor.Aim = new AimInformation(foe, actor, [], gun) { AimPercentage = 1 };
			var fireCheck = Mock.Get(world.GetCheck(gun.WeaponType.FireCheck)); var checks = 0;
			fireCheck.Setup(x => x.MultiDifficultyCheck(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(),
				It.IsAny<ITraitDefinition>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(() =>
				{ ++checks; return Tuple.Create(CheckOutcome.SimpleOutcome(gun.WeaponType.FireCheck, Outcome.MajorFail), CheckOutcome.SimpleOutcome(gun.WeaponType.FireCheck, Outcome.MajorPass)); });
			var policyField = typeof(CommandableAI).GetField("_canCommandProg", BindingFlags.Instance | BindingFlags.NonPublic)!;
			var policy = (IFutureProg)policyField.GetValue(ai)!; var faults = 0; var reactions = 0; var resolving = false;
			var fault = new Mock<IFutureProg>();
			fault.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(arguments =>
			{
				if (resolving && scenario == "cycle-policy-foreign-container" && faults == 0 && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType?.FullName == "MudSharp.GameItems.ComponentUnloadCompletion" && x.GetMethod()?.Name == "Detach"))
				{
					++faults; using var independent = CommandExecutionScope.EnterIndependent(); var shell = gun.ChamberedCasing!;
					shell.ContainedIn = null; bag!.GetItemType<IContainer>()!.Put(actor, shell);
					Require(shell.ContainedIn == bag && bag.GetItemType<IContainer>()!.Contents.Contains(shell), "Final Detach policy independently claims exact native casing in the foreign bag.");
				}
				if (resolving && scenario == "component-policy-revoked" && faults == 0 && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(FirearmBaseGameItemComponent) && x.GetMethod()?.Name == nameof(FirearmBaseGameItemComponent.Fire)))
				{ ++faults; Expire(); }
				return policy.ExecuteBool(arguments);
			});
			WoundEvent reaction = (wounded, wound) =>
			{
				if (!resolving || scenario != "postcommit-independent" || reactions != 0 || wound.ActorOrigin?.Identity.Id != actor.Identity.Id) return;
				++reactions; Expire(); using var independent = CommandExecutionScope.EnterIndependent();
				Require(actor.SetTraitValue(trait, 60), "Real independent wound reaction must retain its native trait write.");
			};
			foe.Body.OnWounded += reaction; policyField.SetValue(ai, fault.Object);
			try
			{
				var direct = scenario == "direct-valid";
				if (direct) { Expire(); using var independent = CommandExecutionScope.EnterIndependent(); actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectFireItem(actor, foe, gun)); }
				else order(actor, caster, "fire");
				var move = actor.ChooseMove();
				Require(move is RangedWeaponAttackMove && CommandExecutionAuthority.IsOrdered(move) == !direct, "Actual ChooseMove must select native ranged attack with exact controller provenance.");
				if (scenario == "queued-revoked") Expire();
				body.CurrentStamina = 100; world.SaveManager.Flush(); var before = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				resolving = firing = true; try { actor.Combat!.CombatAction(actor, move); } finally { resolving = firing = false; }
				var fired = scenario is not ("queued-revoked" or "component-policy-revoked");
				var hit = false; var after = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				Require(Same(actor.CurrentStamina, fired ? 97 : 100) && Same(gunItem.Condition, fired ? 0.99 : 1), $"Accepted shots must pay exactly3 stamina and0.01 condition once: {scenario} stamina:{actor.CurrentStamina} condition:{gunItem.Condition}.");
				Console.WriteLine($"ARMAmmoAftermath-state={scenario} shot-deleted:{shot.Deleted} chamber:{gun.ChamberedRound?.Parent.Id} casing:{gun.ChamberedCasing?.Id} magazine:{string.Join(',',gun.MagazineContents.Select(x => x.Id))} generated:{string.Join(',',generated.Select(x => x.Prototype.Name+":"+x.Id+":"+x.ContainedIn?.Id+":"+x.DirectLocation?.Id+":"+x.GetItemType<IHoldable>()!.HeldBy?.Id))}");
				var interrupted = scenario is "cycle-expire" or "cycle-foreign-container" or "cycle-policy-foreign-container" or "postcommit-expire" or "postcommit-casing-claim";
				Require(gun.ChamberedRound?.Parent == (!fired ? shot : interrupted ? null : spare) &&
					gun.MagazineContents.SequenceEqual(!fired || interrupted ? [spare] : Array.Empty<IGameItem>()) && checks == (fired ? 1 : 0), "Self-loading single shot cycles only while authority remains; refused shot preserves both original slots.");
				Require(hit ? after > before : Same(after, before), "Only accepted native hits may install wounds.");
				Require(reactions == (scenario == "postcommit-independent" ? 1 : 0) && Same(actor.TraitRawValue(trait), reactions == 1 ? 60 : 40), "Independent real wound reaction must survive once without being overwritten.");
				Require(shot.Deleted == fired && !((GameItem)spare).Deleted && spare.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, caster.Identity.Id), "Consumed original round disappears and spare retains its native title.");
				if (fired)
				{
					Require(generated.Count == 2 && generated.All(x => !x.Deleted && x.Quantity == 1 && x.OwnershipReference is null && x.Quality == shot.Quality), "Exactly one new bullet and casing retain authored null title and round quality.");
					var bullet = generated.Single(x => x.Prototype.Name == "ARMRegression projectile");
					var shell = generated.Single(x => x.Prototype.Name == "ARMRegression casing");
					Require(bullet.DirectLocation == foe.Location && bullet.ContainedIn is null && bullet.GetItemType<IHoldable>()!.HeldBy is null, "Accepted native miss puts exact zero-break projectile on the captured target floor.");
					Require(scenario == "cycle-expire" ? gun.ChamberedCasing == shell && shell.ContainedIn == gunItem && shell.DirectLocation is null :
						scenario is "cycle-foreign-container" or "cycle-policy-foreign-container" ? shell.ContainedIn == bag && bag!.GetItemType<IContainer>()!.Contents.Contains(shell) && gun.ChamberedCasing is null :
						scenario == "postcommit-casing-claim" ? shell.GetItemType<IHoldable>()!.HeldBy == body && shell.ContainedIn is null && shell.DirectLocation is null :
						shell.DirectLocation == actor.Location && shell.ContainedIn is null && shell.GetItemType<IHoldable>()!.HeldBy is null, "Casing has exact retained, floor or independent body custody after one shot.");
				}
				Require(cycleOutputs == (scenario is "cycle-expire" or "cycle-foreign-container" ? 1 : 0), "One actual cycling output expiry callback.");
				Require(faults == (scenario == "cycle-policy-foreign-container" ? 1 : 0), "One final-policy native foreign custody callback.");
				Read("shot");
				if (scenario == "cycle-expire")
				{
					using var independent = CommandExecutionScope.EnterIndependent(); Require(gun.Ready(actor) && gun.ChamberedRound!.Parent == spare && gun.ChamberedCasing is null && !gun.MagazineContents.Any(), "Independent native Ready recovers retained casing and exact spare once."); Read("recovered");
				}
				Console.WriteLine($"ARMAmmoAftermath={scenario} passed self-loading-single-Load-Ready-ChooseMove-RangedAttack-CombatAction-casing-cycle-projectile cost:{100-actor.CurrentStamina} condition:{gunItem.Condition} wound-delta:{after-before} reactions:{reactions} projectile:{shot.Id} raw:{actor.TraitRawValue(trait)}");
			}
			finally { resolving = false; policyField.SetValue(ai, policy); foe.Body.OnWounded -= reaction; SetPrivateMember(actor, "OutputHandler", originalOutput); }
			using (CommandExecutionScope.EnterIndependent())
			{
				Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
				body.Take(gunItem); gunItem.Delete(); foreach (var round in rounds.Concat(generated).Concat(bag is null ? [] : new[] { bag }).Where(x => !x.Deleted)) { if (round.GetItemType<IHoldable>()!.HeldBy is { } holder) holder.Take(round); round.Delete(); }
				foreach (var item in unrelated) body.GetWithoutMerge(item); world.SaveManager.Flush(); restored(actor);
			}
		}
		native.WorldMock.SetupGet(x => x.SaveManager).Returns(originalSave);
		return 0;
	}
}
