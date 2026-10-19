#nullable enable
// Native combined Setting action qualification.
// Wiring: SeedSettingActionFixtures(database) BEFORE LoadProductionRestartWorld(clock).
// Inside the resulting real-world/DB scope, call QualifySettingNativeActions(world, factory, ids).
// Factory(roleKey, caseName) must create AND persist a new shared-Human actor with that
// authentic selected Class plus a separate fresh shared-Human target with no combat skills.
// Example coordinator wiring using the native role qualification helpers:
//   (roleKey, label) => {
//     var app = NewSettingQualificationApplication(world, accountId, cultureId, "Action" + label);
//     app.SelectedRoles.Add(world.Roles.Get(ids[roleKey]));
//     FinaliseAndSaveSettingApplication(app, SettingQualificationFreeProg(world));
//     var actor = PersistSettingQualificationCharacter(world, app);
//     var targetApp = NewSettingQualificationApplication(world, accountId, cultureId, "Target" + label);
//     FinaliseAndSaveSettingApplication(targetApp, SettingQualificationFreeProg(world));
//     return (actor, PersistSettingQualificationCharacter(world, targetApp));
//   }
// Use the disposable ZERO-start culture, never shared Admin's starting100 prog. This is
// positive functional qualification with authored tactical advantage; native RNG/check,
// body damage and ordinary combat cost remain real. No scheduler pumping between cases.
using System.Globalization;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;
namespace FutureMUD.GatheringNativePersistenceHarness;
internal static partial class GNHProgram
{
    private static void SeedSettingActionFixtures(TestDatabase database)
    {
        using var db = NewIndependentContext(database.ConnectionString);
        Require(TestDatabase.HasOwnedPrefix(database.Name), "Action fixtures require an owned disposable database.");
        Require(!db.GameItemProtos.Any(x=>x.Name == "Setting acceptance Knife" || x.Name == "Setting acceptance Blackjack"),
            "Action fixtures already exist; restore the sealed baseline rather than duplicating fixture prototypes.");
        var account = db.Accounts.OrderBy(x=>x.Id).First().Id;
        Db.EditableItem Revision() => new() { RevisionNumber=0, RevisionStatus=(int)RevisionStatus.Current, BuilderAccountId=account, BuilderDate=DateTime.UtcNow };
        var hold = db.GameItemComponentProtos.Include(x=>x.EditableItem).FirstOrDefault(x=>x.Type == "Holdable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
        if (hold is null) { hold = new() { Id=(db.GameItemComponentProtos.Select(x=>(long?)x.Id).Max()??0)+1, Name="Setting acceptance holdable", Type="Holdable", Description="Disposable fixture holdable", Definition="<Definition/>", EditableItem=Revision() }; db.GameItemComponentProtos.Add(hold); db.SaveChanges(); }
        // Human-only baseline need not contain Generic Health or Useful. These fixture
        // items receive a real persisted native item strategy without changing defaults.
        var health = db.HealthStrategies.FirstOrDefault(x=>x.Type == "GameItem");
        if (health is null) { health = new() { Name="Setting acceptance item health", Type="GameItem", Definition="<Definition/>" }; db.HealthStrategies.Add(health); db.SaveChanges(); }
        var kilogramsPerBaseUnit = double.Parse(db.StaticConfigurations.Single(x=>x.SettingName == "BaseWeightUOMToKilograms").Definition,CultureInfo.InvariantCulture);
        Require(double.IsFinite(kilogramsPerBaseUnit) && kilogramsPerBaseUnit>0,"Native mass conversion is invalid.");
        foreach (var type in new[] { "Knife", "Blackjack" })
        {
            var weapon = db.WeaponTypes.Single(x=>x.Name == type);
            var component = db.GameItemComponentProtos.Include(x=>x.EditableItem).AsEnumerable().Single(x=>x.Type == "MeleeWeapon" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current && (long?)XElement.Parse(x.Definition).Element("WeaponType") == weapon.Id);
            var proto = new Db.GameItemProto { Id=(db.GameItemProtos.Select(x=>(long?)x.Id).Max()??0)+1, Name="Setting acceptance " + type, Keywords=type.ToLowerInvariant(), ShortDescription="an acceptance " + type.ToLowerInvariant(), FullDescription="A disposable combat qualification fixture.", MaterialId=db.Materials.OrderBy(x=>x.Id).First().Id, HealthStrategyId=health.Id, ReadOnly=true, Size=(int)SizeCategory.Small, Weight=0.2/kilogramsPerBaseUnit, BaseItemQuality=(int)ItemQuality.Standard, EditableItem=Revision() }; // authored fixture mass0.2kg
            proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId=hold.Id, GameItemComponentRevision=hold.RevisionNumber });
            proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId=component.Id, GameItemComponentRevision=component.RevisionNumber });
            db.GameItemProtos.Add(proto); db.SaveChanges();
        }
    }
    private static void QualifySettingNativeActions(IFuturemud world,
        Func<string,string,(ICharacter Actor,ICharacter Target)> createPair, IReadOnlyDictionary<string,long> ids)
    {
        Require(world is Futuremud, "Action qualification requires an actual loaded production world.");
        // LoadProductionRestartWorld intentionally skips normal boot's final combat setup.
        // Complete those SAME native seams; never replace checks/RNG with mocks.
        if (!world.ManualCombatCommands.Any()) ((IFuturemudLoader)world).LoadManualCombatCommands();
        if (world.CombatMessageManager is null) SetPrivateMember(world,"CombatMessageManager",new CombatMessageManager(world));
        CombatBase.SetupCombat(world);
        var characters = new HashSet<long>();
        var bodies = new HashSet<long>();
        (ICharacter Actor,ICharacter Target) Pair(string roleKey, string label)
        {
            var (actor,target)=createPair(roleKey,label);
            Require(actor.Id>0 && target.Id>0 && actor.Body.Id>0 && target.Body.Id>0 &&
                characters.Add(actor.Id) && characters.Add(target.Id) && bodies.Add(actor.Body.Id) && bodies.Add(target.Body.Id),
                "Every action requires independently persisted actor/target identities and bodies.");
            Require(ReferenceEquals(actor.Gameworld,world) && ReferenceEquals(target.Gameworld,world) &&
                ReferenceEquals(actor.Body.Actor,actor) && ReferenceEquals(target.Body.Actor,target) &&
                actor.Race.Name == "Human" && target.Race.Name == "Human" && actor.Roles.Any(x=>x.Id == ids[roleKey]),
                "Factory must use actual shared Human bodies and the selected native Class.");
            Require(world.Actors.Any(x=>ReferenceEquals(x,actor)) && world.Actors.Any(x=>ReferenceEquals(x,target)),
                "Native action characters must be registered in the actual world's actor roots.");
            Require(actor.Combat is null && target.Combat is null && actor.State == CharacterState.Awake && target.State == CharacterState.Awake &&
                !actor.Body.Wounds.Any() && !target.Body.Wounds.Any(), "Native action fixture must start fresh, awake and unwounded.");
            actor.Register(null!); target.Register(null!);
            if (!actor.Location.Characters.Contains(actor)) actor.Location.Enter(actor);
            if (!target.Location.Characters.Contains(target)) target.Location.Enter(target);
            Require(actor.ColocatedWith(target) && actor.CanSee(target), "Native action room must colocate and illuminate the fresh characters.");
            actor.SetPosition(PositionStanding.Instance,PositionModifier.None,null,null);
            target.SetPosition(PositionStanding.Instance,PositionModifier.None,null,null);
            Require(actor.Engage(target,false), "Fresh native actor could not engage the fresh target.");
            Require(actor.Combat is not null && ReferenceEquals(actor.Combat,target.Combat), "Native engagement did not establish one shared combat.");
            actor.MeleeRange=target.MeleeRange=true;
            return (actor,target);
        }
        void EndCase(ICharacter actor,ICharacter target)
        {
            actor.Combat?.EndCombat(false);
            Require(actor.Combat is null && target.Combat is null, "Native case teardown retained combat state.");
            world.SaveManager.Flush();
        }
        foreach (var verb in new[] { "bash", "kick" })
        {
            var (actor,target)=Pair("arm.class.warrior",verb);
            var skill=world.Traits.Get(ids["arm.skill."+verb]);
            Require(actor.HasTrait(skill) && !target.HasTrait(skill), "Warrior root was not granted or fresh target has a counter skill.");
            actor.SetTraitValue(skill,80);
            ((Body)actor.Body).CurrentStamina=actor.MaximumStamina;
            actor.OffensiveAdvantage=1000; // disposable tactical advantage, native RNG/check/wounds remain real
            var command=world.ManualCombatCommands.Single(x=>x.PrimaryVerb == verb);
            var resolved=ManualCombatCommandResolver.TryResolve(actor,command,target);
            Require(resolved.Success, "Native "+verb+" selection refused: "+resolved.Error);
            var before=actor.CurrentStamina; var wounds=target.Body.Wounds.Sum(x=>x.CurrentDamage);
            actor.Combat.CombatAction(actor,resolved.Move);
            Require(actor.CurrentStamina < before, "Native "+verb+" did not spend its combat cost.");
            Require(target.Body.Wounds.Sum(x=>x.CurrentDamage)>wounds, "Native "+verb+" did not cause a body wound.");
            Require(verb != "bash" || target.PositionState == PositionSitting.Instance, "Successful Bash did not seat native victim.");
            Require(verb != "kick" || target.PositionState == PositionStanding.Instance, "Ordinary Kick unexpectedly seated its victim.");
            Console.WriteLine("SETTING-native-action="+verb+" PASS canonical-check real-combat-action native-wounds stamina posture");
            EndCase(actor,target);
        }
        foreach (var sample in new[] { (Type:"Knife",Verb:"backstab"), (Type:"Blackjack",Verb:"sap") })
        {
            var (actor,target)=Pair("arm.class.assassin",sample.Verb);
            var backstab=world.Traits.Get(ids["arm.skill.backstab"]);
            Require(actor.HasTrait(backstab), "Assassin did not receive its native Backstab root.");
            actor.SetTraitValue(backstab,80);
            // Sap is the Assassin child of Backstab80; use its real prerequisite path.
            var sap=world.Traits.Get(ids["arm.skill.sap"]);
            Require(!actor.HasTrait(sap), "Fresh Assassin unexpectedly began with the Sap child.");
            RoleSkillProgressionService.EvaluateBranches(actor);
            Require(actor.HasTrait(sap), "Backstab80 did not unlock the native Assassin Sap branch.");
            var skill=world.Traits.Get(ids["arm.skill."+sample.Verb]);
            actor.SetTraitValue(skill,80);
            var item=world.ItemProtos.Single(x=>x.Name == "Setting acceptance "+sample.Type).CreateNew(actor);
            world.Add(item); actor.Location.Insert(item,true);
            Require(actor.Body.CanGet(item,0), "Native fixture weapon could not be picked up.");
            actor.Body.Get(item,silent:true); // native Get returns void
            Require(actor.Body.Wield(item,silent:true) && actor.Body.WieldedItems.Contains(item), "Native fixture weapon could not be wielded.");
            actor.AddEffect(new FixedCombatFacing(actor,target,Facing.Rear));
            ((Body)actor.Body).CurrentStamina=actor.MaximumStamina;
            var choice=BackstabCombat.FindAttack(actor,target,out var error,sample.Verb);
            Require(choice is not null, "Native "+sample.Verb+" selection failed: "+error);
            Require(((IBackstabAttack)choice!.Attack).ProficiencyTraitId == skill.Id, "Wrong per-action proficiency.");
            Require(ReferenceEquals(choice.Weapon?.Parent,item) && ((IBackstabAttack)choice.Attack).CommandVerb == sample.Verb &&
                choice.Attack.Profile.DamageType == (sample.Verb == "sap" ? DamageType.Crushing : DamageType.Piercing),
                "Native opener selected the wrong weapon, command identity or damage type.");
            var before=actor.CurrentStamina; var wounds=target.Body.Wounds.Sum(x=>x.CurrentDamage+x.CurrentStun);
            actor.OffensiveAdvantage=1000;
            actor.Combat.CombatAction(actor,choice.CreateMove(actor,target));
            Require(actor.CurrentStamina < before && target.Body.Wounds.Sum(x=>x.CurrentDamage+x.CurrentStun)>wounds, "Native opening attempt failed to deliver stamina and wound/stun mutation.");
            Require(sample.Verb != "sap" || target.Body.Wounds.Any(x=>x.CurrentStun>0), "Native Sap did not deliver its configured stun channel.");
            Require(sample.Verb != "backstab" || target.Body.Wounds.Any(x=>x.CurrentDamage>0), "Native Backstab did not deliver injury.");
            Console.WriteLine("SETTING-native-action="+sample.Verb+" PASS independent-proficiency native-weapon-choice actual-wound/stun cost");
            EndCase(actor,target);
        }
    }
}
