#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class CombatSeeder
{
    private void SeedDataWeapons(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers,
        IReadOnlyDictionary<string, TraitDefinition> coreSkills)
    {
        Dictionary<string, TraitDefinition> attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        TraitDefinition strength =
            attributes.GetValueOrDefault("Strength") ??
            attributes.GetValueOrDefault("Physique") ??
            attributes["Body"];

        TraitDefinition dex =
            attributes.GetValueOrDefault("Agility") ??
            attributes.GetValueOrDefault("Dexterity") ??
            attributes.GetValueOrDefault("Agility") ??
            attributes.GetValueOrDefault("Speed") ??
            attributes["Body"];

        string parryoption = questionAnswers["parryoption"].ToLowerInvariant();
        string skilloption = questionAnswers["skilloption"].ToLowerInvariant();

        SeedCombatMessageStyle messageStyle = CombatSeederMessageStyleHelper.Parse(questionAnswers["messagestyle"]);
        IReadOnlyDictionary<string, string> damageExpressions = BuildWeaponDamageExpressions(strength.Id, questionAnswers);
        TraitExpression trainingDamage = UpsertTraitExpression(context, "Weapon Damage - Training",
            damageExpressions["Weapon Damage - Training"]);
        TraitExpression terribleDamage = UpsertTraitExpression(context, "Weapon Damage - Terrible",
            damageExpressions["Weapon Damage - Terrible"]);
        TraitExpression badDamage = UpsertTraitExpression(context, "Weapon Damage - Bad",
            damageExpressions["Weapon Damage - Bad"]);
        TraitExpression poorDamage = UpsertTraitExpression(context, "Weapon Damage - Poor",
            damageExpressions["Weapon Damage - Poor"]);
        TraitExpression normalDamage = UpsertTraitExpression(context, "Weapon Damage - Normal",
            damageExpressions["Weapon Damage - Normal"]);
        TraitExpression goodDamage = UpsertTraitExpression(context, "Weapon Damage - Good",
            damageExpressions["Weapon Damage - Good"]);
        TraitExpression veryGoodDamage = UpsertTraitExpression(context, "Weapon Damage - Very Good",
            damageExpressions["Weapon Damage - Very Good"]);
        TraitExpression greatDamage = UpsertTraitExpression(context, "Weapon Damage - Great",
            damageExpressions["Weapon Damage - Great"]);
        TraitExpression coupdegraceDamage = UpsertTraitExpression(context, "Weapon Damage - Coup de Grace",
            damageExpressions["Weapon Damage - Coup de Grace"]);

        void AddAttack(string name, BuiltInCombatMoveType moveType, MeleeWeaponVerb verb, Difficulty attacker,
            Difficulty dodge, Difficulty parry, Difficulty block, Alignment alignment, Orientation orientation,
            double stamina, double relativeSpeed, WeaponType type, TraitExpression damage, string attackMessage,
            DamageType damageType = DamageType.Crushing, double weighting = 100,
            CombatMoveIntentions intentions =
                CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill,
			string? additionalInfo = null, AttackHandednessOptions handedness = AttackHandednessOptions.Any,
			int maximumTargets = 1)
        {
            WeaponAttack attack = new()
            {
                Verb = (int)verb,
                BaseAttackerDifficulty = (int)attacker,
                BaseBlockDifficulty = (int)block,
                BaseDodgeDifficulty = (int)dodge,
                BaseParryDifficulty = (int)parry,
                MoveType = (int)moveType,
                RecoveryDifficultySuccess = (int)Difficulty.Easy,
                RecoveryDifficultyFailure = (int)Difficulty.Hard,
                Intentions = (long)intentions,
                Weighting = weighting,
				MaximumTargets = maximumTargets,
                ExertionLevel = (int)ExertionLevel.Heavy,
                DamageType = (int)damageType,
                DamageExpression = damage,
                StunExpression = damage,
                PainExpression = damage,
                WeaponTypeId = type.Id,
                StaminaCost = stamina,
                BaseDelay = relativeSpeed,
                Name = name,
                Orientation = (int)orientation,
                Alignment = (int)alignment,
                HandednessOptions = (int)handedness,
                AdditionalInfo = additionalInfo
            };
            context.WeaponAttacks.Add(attack);
            context.SaveChanges();

            string formattedAttackMessage = CombatSeederMessageStyleHelper.FormatAttackMessage(attackMessage, messageStyle);
            CombatMessage message = new()
            {
                Type = (int)moveType,
                Message = formattedAttackMessage,
                Priority = 50,
                Verb = (int)verb,
                Chance = 1.0,
                FailureMessage = formattedAttackMessage
            };
            message.CombatMessagesWeaponAttacks.Add(new CombatMessagesWeaponAttacks
            { CombatMessage = message, WeaponAttack = attack });
            context.CombatMessages.Add(message);
            context.SaveChanges();
        }

        string ForcedMovementAttackData(Difficulty resist, ForcedMovementTypes types, ForcedMovementVerbs verbs,
            ForcedMovementRange range)
        {
            return new XElement("Data",
                new XElement("Resist", resist.ToString()),
                new XElement("Types", types.ToString()),
                new XElement("Verbs", verbs.ToString()),
                new XElement("Range", range.ToString())
            ).ToString();
        }

        TraitDefinition? armourUseSkill = context.TraitDefinitions.FirstOrDefault(x => x.Name == "Armour Use");
        bool useStats = armourUseSkill?.Expression.Expression != "70";

        TraitDefinition CreateSkill(string name)
        {
            if (coreSkills.TryGetValue(name, out TraitDefinition? existing))
            {
                return existing;
            }

            TraitExpression expression = new()
            {
                Name = $"{name} Skill Cap",
                Expression = useStats ? $"min(99,2*{strength.Alias}:{strength.Id}+3*{dex.Alias}:{dex.Id})" : "70"
            };
            TraitDefinition skill = new()
            {
                Name = name,
                Type = (int)TraitType.Skill,
                Expression = expression,
                TraitGroup = "Combat",
                AvailabilityProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
                TeachableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
                LearnableProg = context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
                TeachDifficulty = 7,
                LearnDifficulty = 7,
                Hidden = false,
                ImproverId = context.Improvers.First(x => x.Name == "Skill Improver").Id,
                DecoratorId = context.TraitDecorators.First(x => x.Name == "General Skill").Id,
                DerivedType = 0,
                ChargenBlurb = string.Empty,
                BranchMultiplier = 1.0
            };
            context.TraitDefinitions.Add(skill);
            context.SaveChanges();
            return skill;
        }

        Account dbaccount = context.Accounts.First();
        DateTime now = DateTime.UtcNow;

        void CreateWeaponComponent(WeaponType type)
        {
            GameItemComponentProto component = new()
            {
                Id = context.GameItemComponentProtos.Max(x => x.Id) + 1,
                RevisionNumber = 0,
                EditableItem = new EditableItem
                {
                    RevisionNumber = 0,
                    RevisionStatus = 4,
                    BuilderAccountId = dbaccount.Id,
                    BuilderDate = now,
                    BuilderComment = "Auto-generated by the system",
                    ReviewerAccountId = dbaccount.Id,
                    ReviewerComment = "Auto-generated by the system",
                    ReviewerDate = now
                },
                Type = "MeleeWeapon",
                Name = $"Melee_{type.Name}",
                Description = $"Turns an item into a {type.Name} melee weapon",
                Definition =
                    $"<Definition><WeaponType>{type.Id}</WeaponType></Definition>"
            };
            context.GameItemComponentProtos.Add(component);
            context.SaveChanges();
        }

        TraitDefinition? parrySkill =
            coreSkills.GetValueOrDefault("Parrying") ?? coreSkills.GetValueOrDefault("Parry");

        Dictionary<string, TraitDefinition> skills = new(StringComparer.InvariantCultureIgnoreCase);
        Dictionary<string, TraitDefinition> parrySkills = new(StringComparer.InvariantCultureIgnoreCase);
        TraitDefinition skill;
        switch (skilloption)
        {
            case "soi":
                skill = CreateSkill("Light-Blunt");
                skills["Mace"] = skill;
                parrySkills["Mace"] = parrySkill ?? skill;
                skill = CreateSkill("Light-Edge");
                skills["Short Sword"] = skill;
                parrySkills["Short Sword"] = parrySkill ?? skill;
                skills["Knife"] = skill;
                parrySkills["Knife"] = parrySkill ?? skill;
                skill = CreateSkill("Light-Pierce");
                skills["Dagger"] = skill;
                parrySkills["Dagger"] = parrySkill ?? skill;
                skill = CreateSkill("Medium-Blunt");
                skills["Club"] = skill;
                parrySkills["Club"] = parrySkill ?? skill;
                skills["Improvised"] = skill;
                parrySkills["Improvised"] = parrySkill ?? skill;
                skill = CreateSkill("Medium-Edge");
                skills["Long Sword"] = skill;
                parrySkills["Long Sword"] = parrySkill ?? skill;
                skills["Axe"] = skill;
                parrySkills["Axe"] = parrySkill ?? skill;
                skill = CreateSkill("Medium-Pierce");
                skills["Rapier"] = skill;
                parrySkills["Rapier"] = parrySkill ?? skill;
                skill = CreateSkill("Heavy-Blunt");
                skills["Warhammer"] = skill;
                parrySkills["Warhammer"] = parrySkill ?? skill;
                skill = CreateSkill("Heavy-Edge");
                skills["Two Handed Sword"] = skill;
                parrySkills["Two Handed Sword"] = parrySkill ?? skill;
                skill = CreateSkill("Heavy-Pierce");
                skills["Spear"] = skill;
                parrySkills["Spear"] = parrySkill ?? skill;
                skills["Mattock"] = skill;
                parrySkills["Mattock"] = parrySkill ?? skill;
                skill = CreateSkill("Staff");
                skill = CreateSkill("Polearm");
                skills["Halberd"] = skill;
                parrySkills["Halberd"] = parrySkill ?? skill;
                skill = CreateSkill("Dual-Wielding");
                break;
            case "broad":
                skill = CreateSkill("Bludgeoning Weapons");
                skills["Mace"] = skill;
                parrySkills["Mace"] = parrySkill ?? skill;
                skills["Club"] = skill;
                parrySkills["Club"] = parrySkill ?? skill;
                skills["Improvised"] = skill;
                parrySkills["Improvised"] = parrySkill ?? skill;
                skills["Warhammer"] = skill;
                parrySkills["Warhammer"] = parrySkill ?? skill;
                skill = CreateSkill("Edged Weapons");
                skills["Short Sword"] = skill;
                parrySkills["Short Sword"] = parrySkill ?? skill;
                skills["Dagger"] = skill;
                parrySkills["Dagger"] = parrySkill ?? skill;
                skills["Knife"] = skill;
                parrySkills["Knife"] = parrySkill ?? skill;
                skills["Long Sword"] = skill;
                parrySkills["Long Sword"] = parrySkill ?? skill;
                skills["Axe"] = skill;
                parrySkills["Axe"] = parrySkill ?? skill;
                skills["Rapier"] = skill;
                parrySkills["Rapier"] = parrySkill ?? skill;

                skill = CreateSkill("Two Handed Weapons");
                skills["Warhammer"] = skill;
                parrySkills["Warhammer"] = parrySkill ?? skill;
                skills["Two Handed Sword"] = skill;
                parrySkills["Two Handed Sword"] = parrySkill ?? skill;
                skills["Spear"] = skill;
                parrySkills["Spear"] = parrySkill ?? skill;
                skills["Halberd"] = skill;
                parrySkills["Halberd"] = parrySkill ?? skill;
                skills["Mattock"] = skill;
                parrySkills["Mattock"] = parrySkill ?? skill;
                break;
            case "weapons":
                skill = CreateSkill("Maces");
                skills["Mace"] = skill;
                parrySkills["Mace"] = parrySkill ?? skill;
                skill = CreateSkill("Swords");
                skills["Short Sword"] = skill;
                parrySkills["Short Sword"] = parrySkill ?? skill;
                skills["Long Sword"] = skill;
                parrySkills["Long Sword"] = parrySkill ?? skill;
                skills["Rapier"] = skill;
                parrySkills["Rapier"] = parrySkill ?? skill;
                skill = CreateSkill("Daggers");
                skills["Dagger"] = skill;
                parrySkills["Dagger"] = parrySkill ?? skill;
                skill = CreateSkill("Knife");
                skills["Knife"] = skill;
                parrySkills["Knife"] = parrySkill ?? skill;
                skill = CreateSkill("Clubs");
                skills["Club"] = skill;
                parrySkills["Club"] = parrySkill ?? skill;
                skills["Improvised"] = skill;
                parrySkills["Improvised"] = parrySkill ?? skill;
                skill = CreateSkill("Axes");
                skills["Axe"] = skill;
                parrySkills["Axe"] = parrySkill ?? skill;
                skill = CreateSkill("Warhammers");
                skills["Warhammer"] = skill;
                parrySkills["Warhammer"] = parrySkill ?? skill;
                skill = CreateSkill("Two Handed Swords");
                skills["Two Handed Sword"] = skill;
                parrySkills["Two Handed Sword"] = parrySkill ?? skill;
                skill = CreateSkill("Spears");
                skills["Spear"] = skill;
                parrySkills["Spear"] = parrySkill ?? skill;
                skill = CreateSkill("Polearms");
                skills["Halberd"] = skill;
                parrySkills["Halberd"] = parrySkill ?? skill;
                skill = CreateSkill("Mattock");
                skills["Mattock"] = skill;
                parrySkills["Mattock"] = parrySkill ?? skill;
                break;
        }

        #region Knife
        WeaponType knife = new()
        {
            Name = "Knife",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Knife"],
            ParryTrait = parrySkills["Knife"],
            ParryBonus = -1,
            Reach = 1,
            StaminaPerParry = 1.5
        };
        context.WeaponTypes.Add(knife);
        context.SaveChanges();
        CreateWeaponComponent(knife);
        WeaponType trainingKnife = new()
        {
            Name = "Training Knife",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Knife"],
            ParryTrait = parrySkills["Knife"],
            ParryBonus = -1,
            Reach = 1,
            StaminaPerParry = 1.5
        };
        context.WeaponTypes.Add(trainingKnife);
        context.SaveChanges();
        CreateWeaponComponent(trainingKnife);
        AddAttack("Knife Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.FrontRight,
            Orientation.High, 3.0, 0.9, knife, badDamage, "@ swing|swings $2 across &0's body at $1",
            DamageType.Slashing);
        AddAttack("Knife Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.Front, Orientation.High,
            3.0, 0.9, knife, badDamage, "@ swing|swings $2 in a reverse slash at $1", DamageType.Slashing);
        AddAttack("Knife Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, knife,
            terribleDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
        AddAttack("Knife High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front, Orientation.Highest, 4.0,
            1.0, knife, terribleDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2",
            DamageType.Piercing);
        AddAttack("Knife Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low, 4.0, 1.0, knife,
            terribleDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
        AddAttack("Knife Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front, Orientation.High, 3.0, 0.6,
            knife, badDamage, "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Knife Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Easy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            knife, terribleDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Knife Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Easy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 3.0, 0.4,
            knife, terribleDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Knife Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Easy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Low, 3.0, 0.4,
            knife, terribleDamage, "@ stab|stabs down at $1's leg with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Hinder);
        AddAttack("Knife Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Hard,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Appendage, 3.0,
            0.4, knife, terribleDamage, "@ stab|stabs out at $1's arm with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Disarm);

        AddAttack("Knife Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            knife, terribleDamage, "@ slash|slashes $1 with $2", DamageType.Slashing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Knife Low Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 3.0, 0.4,
            knife, terribleDamage, "@ slash|slashes $1 with $2", DamageType.Slashing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Knife Leg Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Low, 3.0, 0.4,
            knife, terribleDamage, "@ slash|slashes down at $1's leg with $2", DamageType.Slashing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Hinder);
        AddAttack("Knife Arm Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Appendage, 3.0,
            0.4, knife, terribleDamage, "@ slash|slashes out at $1's arm with $2", DamageType.Slashing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Disarm);

        AddAttack("Knife Throat Cut", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, knife,
            veryGoodDamage, "@ run|runs $2 in a deep slash of $1's throat from ear to ear.", DamageType.Slashing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Knife Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, knife,
            veryGoodDamage, "@ plunge|plunges $2 deep into $1's throat.", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Knife Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            knife, badDamage, "@ stab|stabs $1 with $2", DamageType.Piercing);

        AddAttack("Training Knife Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
            Alignment.FrontRight, Orientation.High, 3.0, 0.9, trainingKnife, trainingDamage,
            "@ swing|swings $2 across &0's body at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Knife Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
            Alignment.Front, Orientation.High, 3.0, 0.9, trainingKnife, trainingDamage,
            "@ swing|swings $2 in a reverse slash at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Knife Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.High, 4.0, 1.0, trainingKnife, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Knife High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.Highest, 4.0, 1.0, trainingKnife, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Knife Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.Low, 4.0, 1.0, trainingKnife, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Knife Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front,
            Orientation.High, 3.0, 0.6, trainingKnife, trainingDamage,
            "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Knife Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            trainingKnife, trainingDamage, "@ stab|stabs $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Knife Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 3.0, 0.4, trainingKnife, trainingDamage, "@ stab|stabs $1 with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Knife Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 3.0, 0.4, trainingKnife, trainingDamage, "@ stab|stabs down at $1's leg with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Hinder);
        AddAttack("Training Knife Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Appendage, 3.0, 0.4, trainingKnife, trainingDamage, "@ stab|stabs out at $1's arm with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Disarm);
        AddAttack("Training Knife Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            trainingKnife, trainingDamage, "@ slash|slashes $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Knife Low Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 3.0, 0.4, trainingKnife, trainingDamage, "@ slash|slashes $1 with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Knife Leg Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 3.0, 0.4, trainingKnife, trainingDamage, "@ slash|slashes down at $1's leg with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Hinder);
        AddAttack("Training Knife Arm Slash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Appendage, 3.0, 0.4, trainingKnife, trainingDamage, "@ slash|slashes out at $1's arm with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Disarm);
        #endregion

        #region Dagger

        WeaponType dagger = new()
        {
            Name = "Dagger",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Dagger"],
            ParryTrait = parrySkills["Dagger"],
            ParryBonus = 1,
            Reach = 1,
            StaminaPerParry = 1.5
        };
        context.WeaponTypes.Add(dagger);
        context.SaveChanges();
        CreateWeaponComponent(dagger);
        WeaponType trainingDagger = new()
        {
            Name = "Training Dagger",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Dagger"],
            ParryTrait = parrySkills["Dagger"],
            ParryBonus = 1,
            Reach = 1,
            StaminaPerParry = 1.5
        };
        context.WeaponTypes.Add(trainingDagger);
        context.SaveChanges();
        CreateWeaponComponent(trainingDagger);

        AddAttack("Dagger Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.FrontRight,
            Orientation.High, 3.0, 0.9, dagger, terribleDamage, "@ swing|swings $2 across &0's body at $1",
            DamageType.Slashing);
        AddAttack("Dagger Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.Front, Orientation.High,
            3.0, 0.9, dagger, terribleDamage, "@ swing|swings $2 in a reverse slash at $1", DamageType.Slashing);
        AddAttack("Dagger Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, dagger,
            badDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
        AddAttack("Dagger High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front, Orientation.Highest, 4.0,
            1.0, dagger, badDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2",
            DamageType.Piercing);
        AddAttack("Dagger Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low, 4.0, 1.0, dagger,
            badDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
        AddAttack("Dagger Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front, Orientation.High, 3.0, 0.6,
            dagger, terribleDamage, "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Dagger Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            dagger, badDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Dagger Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 3.0, 0.4,
            dagger, badDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Dagger Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Low, 3.0, 0.4,
            dagger, badDamage, "@ stab|stabs down at $1's leg with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Hinder);
        AddAttack("Dagger Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Appendage, 3.0,
            0.4, dagger, badDamage, "@ stab|stabs out at $1's arm with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Disarm);

        AddAttack("Dagger Throat Cut", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, dagger,
            veryGoodDamage, "@ run|runs $2 in a deep slash of $1's throat from ear to ear.", DamageType.Slashing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Dagger Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 4.0, 1.0, dagger,
            veryGoodDamage, "@ plunge|plunges $2 deep into $1's throat.", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Dagger Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            dagger, badDamage, "@ stab|stabs $1 with $2", DamageType.Piercing);

        AddAttack("Training Dagger Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
            Alignment.FrontRight, Orientation.High, 3.0, 0.9, trainingDagger, trainingDamage,
            "@ swing|swings $2 across &0's body at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Dagger Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
            Alignment.Front, Orientation.High, 3.0, 0.9, trainingDagger, trainingDamage,
            "@ swing|swings $2 in a reverse slash at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Dagger Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.High, 4.0, 1.0, trainingDagger, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Dagger High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.Highest, 4.0, 1.0, trainingDagger, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Dagger Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.Low, 4.0, 1.0, trainingDagger, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Dagger Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front,
            Orientation.High, 3.0, 0.6, trainingDagger, trainingDamage,
            "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Dagger Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.VeryEasy,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.0, 0.4,
            trainingDagger, trainingDamage, "@ stab|stabs $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Dagger Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 3.0, 0.4, trainingDagger, trainingDamage, "@ stab|stabs $1 with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Dagger Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 3.0, 0.4, trainingDagger, trainingDamage, "@ stab|stabs down at $1's leg with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Hinder);
        AddAttack("Training Dagger Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Appendage, 3.0, 0.4, trainingDagger, trainingDamage, "@ stab|stabs out at $1's arm with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Disarm);

        #endregion

        #region Clubs, Maces, Improvised

        WeaponType club = new()
        {
            Name = "Club",
            Classification = (int)WeaponClassification.NonLethal,
            AttackTrait = skills["Club"],
            ParryTrait = parrySkills["Club"],
            ParryBonus = 0,
            Reach = 2,
            StaminaPerParry = 3.0
        };
        context.WeaponTypes.Add(club);
        context.SaveChanges();
        CreateWeaponComponent(club);
        WeaponType trainingClub = new()
        {
            Name = "Training Club",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Club"],
            ParryTrait = parrySkills["Club"],
            ParryBonus = 0,
            Reach = 2,
            StaminaPerParry = 3.0
        };
        context.WeaponTypes.Add(trainingClub);
        context.SaveChanges();
        CreateWeaponComponent(trainingClub);
        WeaponType mace = new()
        {
            Name = "Mace",
            Classification = (int)WeaponClassification.NonLethal,
            AttackTrait = skills["Mace"],
            ParryTrait = parrySkills["Mace"],
            ParryBonus = -1,
            Reach = 1,
            StaminaPerParry = 2.0
        };
        context.WeaponTypes.Add(mace);
        context.SaveChanges();
        CreateWeaponComponent(mace);
        WeaponType trainingmace = new()
        {
            Name = "Training Mace",
            Classification = (int)WeaponClassification.NonLethal,
            AttackTrait = skills["Mace"],
            ParryTrait = parrySkills["Mace"],
            ParryBonus = -1,
            Reach = 1,
            StaminaPerParry = 2.0
        };
        context.WeaponTypes.Add(trainingmace);
        context.SaveChanges();
        CreateWeaponComponent(trainingmace);
        WeaponType improvised = new()
        {
            Name = "Improvised Bludgeon",
            Classification = (int)WeaponClassification.Improvised,
            AttackTrait = skills["Improvised"],
            ParryTrait = parrySkills["Improvised"],
            ParryBonus = -2,
            Reach = 0,
            StaminaPerParry = 3.0
        };
        context.WeaponTypes.Add(improvised);
        context.SaveChanges();
		context.StaticConfigurations.Find("DefaultBowMeleeWeaponType")!.Definition = improvised.Id.ToString();
		context.StaticConfigurations.Find("DefaultCrossbowMeleeWeaponType")!.Definition = improvised.Id.ToString();
		context.StaticConfigurations.Find("DefaultGunMeleeWeaponType")!.Definition = improvised.Id.ToString();
		context.StaticConfigurations.Find("DefaultMusketMeleeWeaponType")!.Definition = improvised.Id.ToString();

        CreateWeaponComponent(improvised);

        AddAttack("Club 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, club, normalDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Club 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.1, club, normalDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Club 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, club, normalDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Club 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Club 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's arms", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Club 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 6.0, 1.3, club, goodDamage, "@ swing|swings $2 in an overhead blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Club 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 4.0, 1.3, club, badDamage, "@ heave|heaves the haft of $2 down towards $1's head");

        AddAttack("Club Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3, club,
            goodDamage, "@ swing|swings $2 at $1");
        AddAttack("Club Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
        AddAttack("Club Crush Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
        AddAttack("Club Crush Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
        AddAttack("Club Crush Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
        AddAttack("Club Crush Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
        AddAttack("Club Crush Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, club,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());


        AddAttack("Club 2-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 6.0, 1.0, club,
            normalDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
            ((int)Difficulty.Normal).ToString());
        AddAttack("Club 2-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 6.0, 1.1, club, normalDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
            ((int)Difficulty.Normal).ToString());
        AddAttack("Club 2-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 6.0, 1.0, club, normalDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
            ((int)Difficulty.Normal).ToString());
        AddAttack("Club 2-Handed Leg Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 6.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
            ((int)Difficulty.Normal).ToString());
        AddAttack("Club 2-Handed Arm Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 6.0, 1.1, club, normalDamage, "@ swing|swings $2 at $1's arms", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
            ((int)Difficulty.Normal).ToString());
        AddAttack("Club 2-Handed Overhead Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 7.5, 1.3, club, goodDamage, "@ swing|swings $2 in an overhead blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo:
            ((int)Difficulty.Normal).ToString());

        AddAttack("Training Club 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's legs",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 6.0, 1.3, trainingClub, trainingDamage, "@ swing|swings $2 in an overhead blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        AddAttack("Training Club 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 6.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 6.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 6.0, 1.0, trainingClub, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 6.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's legs",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 6.0, 1.1, trainingClub, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Club 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 7.5, 1.3, trainingClub, trainingDamage, "@ swing|swings $2 in an overhead blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        AddAttack("Mace Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 4.0, 0.85,
            mace, poorDamage, "@ swing|swings $2 at $1");
        AddAttack("Mace High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.Highest, 4.0, 0.95,
            mace, poorDamage, "@ swing|swings $2 in a high blow at $1");
        AddAttack("Mace Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 4.0, 0.85,
            mace, poorDamage, "@ swing|swings $2 in a low blow at $1");
        AddAttack("Mace Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low, 4.0, 0.95, mace,
            poorDamage, "@ swing|swings $2 at $1's legs");
        AddAttack("Mace Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight, Orientation.Appendage, 4.0,
            0.95, mace, poorDamage, "@ swing|swings $2 at $1's arms");
        AddAttack("Mace Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 5.0, 1.15,
            mace, normalDamage, "@ swing|swings $2 in an overhead blow at $1");
        AddAttack("Mace Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash, Difficulty.VeryHard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 3.0, 1.15,
            mace, badDamage, "@ heave|heaves the haft of $2 down towards $1's head");

        AddAttack("Mace Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3, mace,
            goodDamage, "@ swing|swings $2 at $1");
        AddAttack("Mace Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, mace,
            greatDamage, "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        AddAttack("Training Mace Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.0, 0.85, trainingmace, trainingDamage, "@ swing|swings $2 at $1");
        AddAttack("Training Mace High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 4.0, 0.95, trainingmace, trainingDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Mace Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 4.0, 0.85, trainingmace, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Mace Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 4.0, 0.95, trainingmace, trainingDamage, "@ swing|swings $2 at $1's legs",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Mace Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 4.0, 0.95, trainingmace, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Mace Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.0, 1.15, trainingmace, trainingDamage, "@ swing|swings $2 in an overhead blow at $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Mace Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 3.0, 1.15, trainingmace, trainingDamage,
            "@ heave|heaves the haft of $2 down towards $1's head", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        AddAttack("Improvised Bludgeon Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.5, 1.1, improvised, poorDamage, "@ heft|hefts $2 around in an awkward swing at $1");
        AddAttack("Improvised Bludgeon High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 4.5, 1.2, improvised, poorDamage, "@ heave|heaves $2 up into a clumsy high blow at $1");
        AddAttack("Improvised Bludgeon Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 4.5, 1.1, improvised, poorDamage, "@ hook|hooks $2 in a low, ugly swing at $1");
        AddAttack("Improvised Bludgeon Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
            4.5, 1.2, improvised, poorDamage, "@ swipe|swipes $2 awkwardly at $1's legs");
        AddAttack("Improvised Bludgeon Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 4.5, 1.2, improvised, poorDamage, "@ club|clubs at $1's arms with $2");
        AddAttack("Improvised Bludgeon Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.5, 1.4, improvised, normalDamage, "@ labor|labors through an overhead smash with $2 at $1");
        AddAttack("Improvised Bludgeon Clinch Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.ExtremelyHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 3.5, 1.25, improvised, terribleDamage, "@ jam|jams $2 toward $1's head at close quarters");

        AddAttack("Improvised Bludgeon Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 6.5, 1.4, improvised, badDamage, "@ hammer|hammers $2 into $1");
        AddAttack("Improvised Bludgeon Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 9.0, 1.6, improvised, goodDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        #endregion

        #region Swords

        #region Shortsword

        WeaponType shortsword = new()
        {
            Name = "Shortsword",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Short Sword"],
            ParryTrait = parrySkills["Short Sword"],
            ParryBonus = 0,
            Reach = 2,
            StaminaPerParry = 2.0
        };
        context.WeaponTypes.Add(shortsword);
        context.SaveChanges();
        CreateWeaponComponent(shortsword);
        WeaponType trainingshortsword = new()
        {
            Name = "Training Shortsword",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Short Sword"],
            ParryTrait = parrySkills["Short Sword"],
            ParryBonus = 0,
            Reach = 2,
            StaminaPerParry = 2.0
        };
        context.WeaponTypes.Add(trainingshortsword);
        context.SaveChanges();
        CreateWeaponComponent(trainingshortsword);

        AddAttack("Shortsword Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.FrontRight,
            Orientation.High, 4.2, 1.0, shortsword, badDamage, "@ swing|swings $2 across &0's body at $1",
            DamageType.Slashing);
        AddAttack("Shortsword Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial, Alignment.Front,
            Orientation.High, 4.2, 1.0, shortsword, badDamage, "@ swing|swings $2 in a reverse slash at $1",
            DamageType.Slashing);
        AddAttack("Shortsword Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 5.0, 1.2,
            shortsword, poorDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2",
            DamageType.Piercing);
        AddAttack("Shortsword High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, shortsword, poorDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Piercing);
        AddAttack("Shortsword Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.Low,
            5.0, 1.2, shortsword, poorDamage, "@ lunge|lunges forward and attempt|attempts to stab $1 with $2",
            DamageType.Piercing);
        AddAttack("Shortsword Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front, Orientation.High, 4.2, 0.8,
            shortsword, badDamage, "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Shortsword Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 4.2, 0.7,
            shortsword, poorDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Shortsword Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight, Orientation.Centre, 4.2, 0.7,
            shortsword, poorDamage, "@ stab|stabs $1 with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast);
        AddAttack("Shortsword Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Low, 4.2, 0.7,
            shortsword, poorDamage, "@ stab|stabs down at $1's leg with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Hinder);
        AddAttack("Shortsword Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab, Difficulty.Hard,
            Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight, Orientation.Appendage, 4.2,
            0.7, shortsword, poorDamage, "@ stab|stabs out at $1's arm with $2", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Fast | CombatMoveIntentions.Disarm);

        AddAttack("Shortsword Throat Cut", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 5.0, 1.0,
            shortsword, goodDamage, "@ run|runs $2 in a deep slash of $1's throat from ear to ear.",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Shortsword Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front, Orientation.High, 5.0, 1.0,
            shortsword, goodDamage, "@ plunge|plunges $2 deep into $1's throat.", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Shortsword Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.2, 0.4, shortsword, poorDamage, "@ stab|stabs $1 with $2", DamageType.Piercing);

        AddAttack("Training Shortsword Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
            Alignment.FrontRight, Orientation.High, 4.2, 1.0, trainingshortsword, trainingDamage,
            "@ swing|swings $2 across &0's body at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Shortsword Reverse Slash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.VeryEasy, Difficulty.ExtremelyEasy, Difficulty.ExtremelyEasy, Difficulty.Trivial,
            Alignment.Front, Orientation.High, 4.2, 1.0, trainingshortsword, trainingDamage,
            "@ swing|swings $2 in a reverse slash at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Shortsword Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.High, 5.0, 1.2, trainingshortsword, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Shortsword High Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, trainingshortsword, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Shortsword Low Lunge", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryEasy, Alignment.Front,
            Orientation.Low, 5.0, 1.2, trainingshortsword, trainingDamage,
            "@ lunge|lunges forward and attempt|attempts to stab $1 with $2", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Shortsword Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryEasy, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.ExtremelyEasy, Alignment.Front,
            Orientation.High, 4.2, 0.8, trainingshortsword, trainingDamage,
            "@ lash|lashes out with a quick jab of $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Shortsword Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs $1 with $2",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Shortsword Low Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs $1 with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Shortsword Leg Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs down at $1's leg with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Hinder);
        AddAttack("Training Shortsword Arm Stab", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Appendage, 4.2, 0.7, trainingshortsword, trainingDamage, "@ stab|stabs out at $1's arm with $2",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast |
                        CombatMoveIntentions.Disarm);

        #endregion

        #region Longswords and 2Handers

        WeaponType longsword = new()
        {
            Name = "Longsword",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Long Sword"],
            ParryTrait = parrySkills["Long Sword"],
            ParryBonus = 1,
            Reach = 3,
            StaminaPerParry = 3.0
        };
        context.WeaponTypes.Add(longsword);
        context.SaveChanges();
        CreateWeaponComponent(longsword);
        WeaponType traininglongsword = new()
        {
            Name = "Training Longsword",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Long Sword"],
            ParryTrait = parrySkills["Long Sword"],
            ParryBonus = 1,
            Reach = 3,
            StaminaPerParry = 3.0
        };
        context.WeaponTypes.Add(traininglongsword);
        context.SaveChanges();
        CreateWeaponComponent(traininglongsword);

        WeaponType zweihander = new()
        {
            Name = "Two Handed Sword",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Two Handed Sword"],
            ParryTrait = parrySkills["Two Handed Sword"],
            ParryBonus = 0,
            Reach = 4,
            StaminaPerParry = 5.0
        };
        context.WeaponTypes.Add(zweihander);
        context.SaveChanges();
        CreateWeaponComponent(zweihander);
        WeaponType trainingzweihander = new()
        {
            Name = "Training Two Handed Sword",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Two Handed Sword"],
            ParryTrait = parrySkills["Two Handed Sword"],
            ParryBonus = 0,
            Reach = 4,
            StaminaPerParry = 5.0
        };
        context.WeaponTypes.Add(trainingzweihander);
        context.SaveChanges();
        CreateWeaponComponent(trainingzweihander);

        AddAttack("Longsword 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.1, longsword, normalDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.1, longsword, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.1, longsword, normalDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.3, longsword, goodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, longsword, normalDamage,
            "@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, longsword, normalDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Hinder);
        AddAttack("Longsword 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 0.8,
            longsword, badDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.8, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 0.8, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Longsword Pommel Strike", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.5, 0.8, longsword, badDamage,
            "@ reverse|reverses $2 and smash|smashes the pommel into $1 at close quarters", DamageType.Crushing);

        AddAttack("Longsword Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3,
            longsword, goodDamage, "@ swing|swings $2 at $1", DamageType.Slashing);
        AddAttack("Longsword Slash Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            longsword, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
        AddAttack("Longsword Slash Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            longsword, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
        AddAttack("Longsword Slash Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, longsword, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
        AddAttack("Longsword Slash Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, longsword, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
        AddAttack("Longsword Slash Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, longsword, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
        AddAttack("Longsword Slash Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            longsword, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());
        AddAttack("Longsword Chest Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            longsword, coupdegraceDamage,
            "@ position|positions $2 above $1's heart and thrust|thrusts down in a brutal stab", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
        AddAttack("Longsword Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            longsword, coupdegraceDamage,
            "@ position|positions $2 above $1's throat and thrust|thrusts down in a brutal stab", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());


        AddAttack("Longsword 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.9, longsword, normalDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 0.9, longsword, normalDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.0, longsword, normalDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.2, longsword, goodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.9, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 5.0, 0.9, longsword, normalDamage,
            "@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 5.0, 0.9, longsword, normalDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 0.9, longsword, normalDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Hinder);
        AddAttack("Longsword 2-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 0.7,
            longsword, badDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.7, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Longsword 2-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 0.7, longsword, badDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);


        AddAttack("Training Longsword 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal,
            Alignment.FrontRight, Orientation.Highest, 5.0, 1.1, traininglongsword, trainingDamage,
            "@ swing|swings $2 in a high blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
            5.0, 1.1, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.1, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy,
            Alignment.Front, Orientation.Highest, 6.0, 1.3, traininglongsword, trainingDamage,
            "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 low at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
        AddAttack("Training Longsword 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.8, traininglongsword, trainingDamage, "@ quickly jab|jabs $2 at $1",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Training Longsword Pommel Strike", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.5, 0.8, traininglongsword, trainingDamage,
            "@ reverse|reverses $2 and tap|taps the pommel into $1 at close quarters", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.9, traininglongsword, trainingDamage, "@ swing|swings $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal,
            Alignment.FrontRight, Orientation.Highest, 5.0, 1.0, traininglongsword, trainingDamage,
            "@ swing|swings $2 in a high blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 0.9, traininglongsword, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.0, traininglongsword, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy,
            Alignment.Front, Orientation.Highest, 6.0, 1.2, traininglongsword, trainingDamage,
            "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.9, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 5.0, 0.9, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 5.0, 0.9, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 low at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Longsword 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 0.9, traininglongsword, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
        AddAttack("Training Longsword 2-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.7, traininglongsword, trainingDamage, "@ quickly jab|jabs $2 at $1",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Training Longsword 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.8, traininglongsword, trainingDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Training Longsword 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Jab, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy,
            Alignment.FrontRight, Orientation.Centre, 5.0, 0.8, traininglongsword, trainingDamage,
            "@ quickly counter jab|jabs $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Training Longsword 2-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.7, traininglongsword, trainingDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Training Longsword 2-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Jab, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy,
            Alignment.FrontRight, Orientation.Centre, 5.0, 0.7, traininglongsword, trainingDamage,
            "@ quickly counter jab|jabs $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.TwoHandedOnly);

        AddAttack("Zweihander 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.3, zweihander, goodDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.3, zweihander, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.3, zweihander, goodDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Slashing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.5, zweihander, greatDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.2, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.2, zweihander, goodDamage,
            "@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.2, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.2, zweihander, goodDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Hinder);
        AddAttack("Zweihander 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, zweihander, badDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, zweihander, badDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Zweihander 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, zweihander, badDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);

        AddAttack("Zweihander Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 6.0, 1.5, zweihander, greatDamage, "@ swing|swings $2 at $1", DamageType.Slashing);
        AddAttack("Zweihander Slash Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            zweihander, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
        AddAttack("Zweihander Slash Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            zweihander, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
        AddAttack("Zweihander Slash Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
        AddAttack("Zweihander Slash Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
        AddAttack("Zweihander Slash Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
        AddAttack("Zweihander Slash Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, zweihander, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Slashing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());
        AddAttack("Zweihander Chest Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            zweihander, coupdegraceDamage,
            "@ position|positions $2 above $1's heart and thrust|thrusts down in a brutal stab", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
        AddAttack("Zweihander Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            zweihander, coupdegraceDamage,
            "@ position|positions $2 above $1's throat and thrust|thrusts down in a brutal stab", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());

        AddAttack("Zweihander 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.1, zweihander, goodDamage, "@ swing|swings $2 at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.1, zweihander, goodDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Slashing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.2, zweihander, goodDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.2, zweihander, greatDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Slashing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.1, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.1, zweihander, goodDamage,
            "@ lunge|lunges forward and stab|stabs $2 high at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.1, zweihander, goodDamage, "@ lunge|lunges forward and stab|stabs $2 low at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.1, zweihander, goodDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's feet", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Hinder);
        AddAttack("Zweihander 2-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.9, zweihander, poorDamage, "@ quickly jab|jabs $2 at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.9, zweihander, poorDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 0.9, zweihander, poorDamage, "@ quickly counter jab|jabs $2 at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Zweihander 2-Handed Downed Cleave", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 7.0, 1.3, zweihander, greatDamage,
            "@ raise|raises $2 high and hew|hews down at $1's {0} while #1 %1|lay|lays sprawled on the ground",
            DamageType.Slashing, handedness: AttackHandednessOptions.TwoHandedOnly);

        #endregion

        #region Rapier

        WeaponType rapier = new()
        {
            Name = "Rapier",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Rapier"],
            ParryTrait = parrySkills["Rapier"],
            ParryBonus = 2,
            Reach = 3,
            StaminaPerParry = 1.0
        };
        context.WeaponTypes.Add(rapier);
        context.SaveChanges();
        CreateWeaponComponent(rapier);
        WeaponType trainingrapier = new()
        {
            Name = "Training Rapier",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Rapier"],
            ParryTrait = parrySkills["Rapier"],
            ParryBonus = 2,
            Reach = 3,
            StaminaPerParry = 1.0
        };
        context.WeaponTypes.Add(trainingrapier);
        context.SaveChanges();
        CreateWeaponComponent(trainingrapier);

        AddAttack("Rapier 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.0, 0.85, rapier, normalDamage, "@ extend|extends $2 in a straight thrust at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 4.0, 0.85, rapier, normalDamage, "@ extend|extends $2 high in a straight thrust at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 4.0, 0.85, rapier, normalDamage, "@ dip|dips the point of $2 and thrust|thrusts low at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 4.0, 0.85, rapier, normalDamage, "@ drop|drops the point of $2 and thrust|thrusts at $1's foot",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Hinder);
        AddAttack("Rapier 1-Handed Cross Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
            4.0, 0.85, rapier, normalDamage, "@ slide|slides $2 across the line and thrust|thrusts at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab, Difficulty.Easy,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 3.5, 0.6,
            rapier, poorDamage, "@ flick|flicks $2 out in a quick jab at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier 1-Handed Close Thrust", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 4.0, 0.7, rapier, terribleDamage,
            "@ drive|drives $2 forward in a cramped close-quarters thrust at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 3.5, 0.6, rapier, poorDamage, "@ flick|flicks $2 out in a sharp counter-jab at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 3.5, 0.6, rapier, poorDamage, "@ flick|flicks $2 low in a counter-jab at $1",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Kill | CombatMoveIntentions.Wound |
                        CombatMoveIntentions.Fast, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier Pommel Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Bash,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.5, 0.8, rapier, terribleDamage,
            "@ reverse|reverses $2 and crack|cracks the pommel at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Rapier Throat Skewer", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High, 4.5, 0.9, rapier,
            greatDamage, "@ line|lines up $2 and drive|drives it straight through $1's {0}", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());

        AddAttack("Training Rapier 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.0, 0.85, trainingrapier, trainingDamage,
            "@ extend|extends $2 in a straight thrust at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Rapier 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 4.0, 0.85, trainingrapier, trainingDamage,
            "@ extend|extends $2 high in a straight thrust at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Rapier 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 4.0, 0.85, trainingrapier, trainingDamage,
            "@ dip|dips the point of $2 and thrust|thrusts low at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Rapier 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 4.0, 0.85, trainingrapier, trainingDamage,
            "@ drop|drops the point of $2 and thrust|thrusts at $1's foot", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
        AddAttack("Training Rapier 1-Handed Cross Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 4.0, 0.85, trainingrapier, trainingDamage,
            "@ slide|slides $2 across the line and thrust|thrusts at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Rapier 1-Handed Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 3.5, 0.6, trainingrapier, trainingDamage, "@ flick|flicks $2 out in a quick jab at $1",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Training Rapier 1-Handed Close Thrust", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryHard, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 4.0, 0.7, trainingrapier, trainingDamage,
            "@ drive|drives $2 forward in a cramped close-quarters thrust at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Training Rapier 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 3.5, 0.6, trainingrapier, trainingDamage, "@ flick|flicks $2 out in a sharp counter-jab at $1",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Training Rapier 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 3.5, 0.6, trainingrapier, trainingDamage, "@ flick|flicks $2 low in a counter-jab at $1",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Training Rapier Pommel Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.5, 0.8, trainingrapier, trainingDamage,
            "@ reverse|reverses $2 and crack|cracks the pommel at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        #endregion

        #endregion

        #region Axes

        WeaponType axe = new()
        {
            Name = "Axe",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Axe"],
            ParryTrait = parrySkills["Axe"],
            ParryBonus = -2,
            Reach = 3,
            StaminaPerParry = 3.5
        };
        context.WeaponTypes.Add(axe);
        context.SaveChanges();
        CreateWeaponComponent(axe);
        WeaponType trainingaxe = new()
        {
            Name = "Training Axe",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Axe"],
            ParryTrait = parrySkills["Axe"],
            ParryBonus = -2,
            Reach = 3,
            StaminaPerParry = 3.5
        };
        context.WeaponTypes.Add(trainingaxe);
        context.SaveChanges();
        CreateWeaponComponent(trainingaxe);

        AddAttack("Axe 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            axe, goodDamage, "@ swing|swings $2 at $1", DamageType.Chopping,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Axe 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.1, axe, goodDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Chopping, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Axe 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 in a low blow at $1", DamageType.Chopping,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Axe 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.1, axe, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Chopping,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Axe 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.1, axe, goodDamage, "@ swing|swings $2 at $1's arms", DamageType.Chopping,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Axe 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.3, axe, veryGoodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Chopping,
            handedness: AttackHandednessOptions.OneHandedOnly);

        AddAttack("Axe Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3, axe,
            goodDamage, "@ swing|swings $2 at $1", DamageType.Chopping);
        AddAttack("Axe Chop Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
        AddAttack("Axe Chop Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
        AddAttack("Axe Chop Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
        AddAttack("Axe Chop Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
        AddAttack("Axe Chop Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
        AddAttack("Axe Chop Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, axe,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());

        AddAttack("Axe 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight, Orientation.High, 5.0, 0.9,
            axe, goodDamage, "@ swing|swings $2 at $1", DamageType.Chopping,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Axe 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Chopping, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Axe 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 0.9, axe, goodDamage, "@ swing|swings $2 in a low blow at $1", DamageType.Chopping,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Axe 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 at $1's legs", DamageType.Chopping,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Axe 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.0, axe, goodDamage, "@ swing|swings $2 at $1's arms", DamageType.Chopping,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Axe 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.2, axe, veryGoodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Chopping,
            handedness: AttackHandednessOptions.TwoHandedOnly);

        AddAttack("Training Axe 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.1, trainingaxe, trainingDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.1, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's legs",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.1, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.3, trainingaxe, trainingDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 0.9, trainingaxe, trainingDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 0.9, trainingaxe, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 2-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's legs",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 2-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 5.0, 1.0, trainingaxe, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Axe 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.2, trainingaxe, trainingDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        #endregion

        #region Halberd
        WeaponType halberd = new()
        {
            Name = "Halberd",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Halberd"],
            ParryTrait = parrySkills["Halberd"],
            ParryBonus = 1,
            Reach = 5,
            StaminaPerParry = 2.5
        };
        context.WeaponTypes.Add(halberd);
        context.SaveChanges();
        CreateWeaponComponent(halberd);
        WeaponType trainingHalberd = new()
        {
            Name = "Training Halberd",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Halberd"],
            ParryTrait = parrySkills["Halberd"],
            ParryBonus = 1,
            Reach = 4,
            StaminaPerParry = 2
        };
        context.WeaponTypes.Add(trainingHalberd);
        context.SaveChanges();
        CreateWeaponComponent(trainingHalberd);

        AddAttack("Halberd 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            halberd, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing);
        AddAttack("Halberd 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, halberd, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing);
        AddAttack("Halberd 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, halberd, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing);
        AddAttack("Halberd 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, halberd, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing |
                        CombatMoveIntentions.Hinder);
        AddAttack("Halberd 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, halberd, normalDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing |
                        CombatMoveIntentions.Hinder);
        AddAttack("Halberd 1-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 3.0, 0.6, halberd, terribleDamage,
            "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Normal).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Hinder);
        AddAttack("Halberd 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, halberd, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Halberd 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, halberd, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);

        AddAttack("Halberd 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            halberd, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing);
        AddAttack("Halberd 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, halberd, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing);
        AddAttack("Halberd 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, halberd, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing);
        AddAttack("Halberd 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, halberd, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing |
                        CombatMoveIntentions.Hinder);
        AddAttack("Halberd 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, halberd, goodDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Piercing |
                        CombatMoveIntentions.Hinder);
        AddAttack("Halberd 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.4, halberd, veryGoodDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Chopping,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill | CombatMoveIntentions.Slow | CombatMoveIntentions.Chopping
            );

        AddAttack("Halberd 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, halberd, badDamage,
            "@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
                        CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Normal).ToString());
        AddAttack("Halberd 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight, Orientation.Lowest, 3.0, 0.6,
            halberd, terribleDamage, "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
                        CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Halberd 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
            5.0, 1.0, halberd, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Halberd 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Centre, 5.0, 1.0, halberd, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Halberd 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryHard, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
            Orientation.Highest, 5.0, 1.0, halberd, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);

        AddAttack("Halberd Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            halberd, badDamage, "@ stab|stabs $2 at $1", DamageType.Piercing);
        AddAttack("Halberd Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            halberd, greatDamage, "@ brutally stab|stabs $2 into $1's throat", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Halberd Heart Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            halberd, greatDamage, "@ brutally stab|stabs $2 into $1's chest right above &1's heart",
            DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
        AddAttack("Halberd Belly Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            halberd, greatDamage, "@ brutally stab|stabs $2 into $1's belly", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "belly").Id.ToString());
        AddAttack("Halberd Neck Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            halberd, greatDamage,
            "@ line|lines up $2 with the back of $1's neck and brutally push|pushes the spear in", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
        AddAttack("Halberd Chop Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5, halberd,
            coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());

        AddAttack("Training Halberd 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingHalberd, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 1-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 3.0, 0.6, trainingHalberd, trainingDamage,
            "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Normal).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingHalberd, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        AddAttack("Training Halberd 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingHalberd, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.Highest,
            6.0, 1.4, trainingHalberd, trainingDamage, "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Slow);

        AddAttack("Training Halberd 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.Normal).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 3.0, 0.6, trainingHalberd, trainingDamage,
            "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
            5.0, 1.0, trainingHalberd, trainingDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy,
            Alignment.Front, Orientation.Centre, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Halberd 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard,
            Alignment.Front, Orientation.Highest, 5.0, 1.0, trainingHalberd, trainingDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        #endregion

        #region Spears

        WeaponType spear = new()
        {
            Name = "Short Spear",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Spear"],
            ParryTrait = parrySkills["Spear"],
            ParryBonus = 1,
            Reach = 4,
            StaminaPerParry = 1.5
        };
        context.WeaponTypes.Add(spear);
        context.SaveChanges();
        CreateWeaponComponent(spear);
        WeaponType longspear = new()
        {
            Name = "Long Spear",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Spear"],
            ParryTrait = parrySkills["Spear"],
            ParryBonus = 1,
            Reach = 5,
            StaminaPerParry = 2.0
        };
        context.WeaponTypes.Add(longspear);
        context.SaveChanges();
        CreateWeaponComponent(longspear);
        WeaponType trainingspear = new()
        {
            Name = "Training Spear",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Spear"],
            ParryTrait = parrySkills["Spear"],
            ParryBonus = 1,
            Reach = 4,
            StaminaPerParry = 1.5
        };
        context.WeaponTypes.Add(trainingspear);
        context.SaveChanges();
        CreateWeaponComponent(trainingspear);

        AddAttack("Spear 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, spear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's foot",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 1-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight, Orientation.Lowest, 3.0, 0.6,
            spear, terribleDamage, "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
                        CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Normal).ToString());
        AddAttack("Spear 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, spear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, spear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);

        AddAttack("Spear 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, spear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's foot",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, spear, badDamage, "@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
                        CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Normal).ToString());
        AddAttack("Spear 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight, Orientation.Lowest, 3.0, 0.6,
            spear, terribleDamage, "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
                        CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Spear 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
            5.0, 1.0, spear, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Centre, 5.0, 1.0, spear, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryHard, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
            Orientation.Highest, 5.0, 1.0, spear, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);

        AddAttack("Spear Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            spear, normalDamage, "@ stab|stabs $2 at $1", DamageType.Piercing);
        AddAttack("Spear Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            spear, greatDamage, "@ brutally stab|stabs $2 into $1's throat", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Spear Heart Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            spear, greatDamage, "@ brutally stab|stabs $2 into $1's chest right above &1's heart", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
        AddAttack("Spear Belly Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            spear, greatDamage, "@ brutally stab|stabs $2 into $1's belly", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "belly").Id.ToString());
        AddAttack("Spear Neck Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            spear, greatDamage, "@ line|lines up $2 with the back of $1's neck and brutally push|pushes the spear in",
            DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());

        AddAttack("Spear 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, longspear, normalDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, longspear, normalDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Piercing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, longspear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);
        AddAttack("Spear 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryHard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, longspear, poorDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.OneHandedOnly);

        AddAttack("Spear 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, longspear, goodDamage, "@ lunge|lunges forward and stab|stabs $2 at $1's leg",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.Normal, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, longspear, goodDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Piercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Spear 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight, Orientation.Low,
            5.0, 1.0, longspear, badDamage,
            "@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
                        CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Normal).ToString());
        AddAttack("Spear 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight, Orientation.Lowest, 3.0, 0.6,
            longspear, terribleDamage, "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Trip |
                        CombatMoveIntentions.Hinder, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Spear 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
            5.0, 1.0, longspear, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Hard, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Centre, 5.0, 1.0, longspear, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);
        AddAttack("Spear 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryHard, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard, Alignment.Front,
            Orientation.Highest, 5.0, 1.0, longspear, goodDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.TwoHandedOnly);

        AddAttack("Spear Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            longspear, badDamage, "@ stab|stabs $2 at $1", DamageType.Piercing);
        AddAttack("Spear Throat Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            longspear, greatDamage, "@ brutally stab|stabs $2 into $1's throat", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "throat").Id.ToString());
        AddAttack("Spear Heart Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            longspear, greatDamage, "@ brutally stab|stabs $2 into $1's chest right above &1's heart",
            DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());
        AddAttack("Spear Belly Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            longspear, greatDamage, "@ brutally stab|stabs $2 into $1's belly", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "belly").Id.ToString());
        AddAttack("Spear Neck Stab", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Stab, Difficulty.Normal,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight, Orientation.High, 5.0, 1.0,
            longspear, greatDamage,
            "@ line|lines up $2 with the back of $1's neck and brutally push|pushes the spear in", DamageType.Piercing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());

        AddAttack("Training Spear 1-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingspear, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 1-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 1-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 1-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 1-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 1-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 3.0, 0.6, trainingspear, trainingDamage,
            "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Normal).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 1-Handed Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingspear, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 1-Handed Low Counter Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        AddAttack("Training Spear 2-Handed Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 5.0, 1.0, trainingspear, trainingDamage, "@ lunge|lunges forward and stab|stabs $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed High Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Highest, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed Low Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed Leg Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's leg", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed Foot Stab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Stab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.VeryEasy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 5.0, 1.0, trainingspear, trainingDamage,
            "@ lunge|lunges forward and stab|stabs $2 at $1's foot", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed Leg Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Hard, Alignment.FrontRight,
            Orientation.Low, 5.0, 1.0, trainingspear, trainingDamage,
            "@ sweep|sweeps $2 around at $1's legs in an attempt to knock &1 off balance", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.Normal).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed Trip", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.VeryHard, Alignment.FrontRight,
            Orientation.Lowest, 3.0, 0.6, trainingspear, trainingDamage,
            "@ knock|knocks at $1's legs and feet with $2 in an attempt to trip &1 up", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed Counter Stab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Stab,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy, Alignment.Front, Orientation.High,
            5.0, 1.0, trainingspear, trainingDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating stab of $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed Low Counter Stab", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Normal, Difficulty.Easy,
            Alignment.Front, Orientation.Centre, 5.0, 1.0, trainingspear, trainingDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating low stab of $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Spear 2-Handed High Counter Stab", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Stab, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.Hard, Difficulty.Hard,
            Alignment.Front, Orientation.Highest, 5.0, 1.0, trainingspear, trainingDamage,
            "@ duck|ducks inside the blow and lunge|lunges forward with a devastating high stab of $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);

        #endregion

        #region Mattock
        WeaponType mattock = new()
        {
            Name = "Mattock",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Mattock"],
            ParryTrait = parrySkills["Mattock"],
            ParryBonus = -3,
            Reach = 4,
            StaminaPerParry = 5.0
        };
        context.WeaponTypes.Add(mattock);
        context.SaveChanges();
        CreateWeaponComponent(mattock);
        WeaponType trainingmattock = new()
        {
            Name = "Training Mattock",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Mattock"],
            ParryTrait = parrySkills["Mattock"],
            ParryBonus = -3,
            Reach = 4,
            StaminaPerParry = 5.0
        };
        context.WeaponTypes.Add(trainingmattock);
        context.SaveChanges();
        CreateWeaponComponent(trainingmattock);

        AddAttack("Mattock 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.4, mattock, veryGoodDamage, "@ swing|swings $2 at $1", DamageType.ArmourPiercing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Mattock 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.6, mattock, veryGoodDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Mattock 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 6.5, 1.4, mattock, veryGoodDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Mattock 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
            6.5, 1.6, mattock, veryGoodDamage, "@ swing|swings $2 at $1's legs", DamageType.ArmourPiercing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Mattock 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 6.5, 1.6, mattock, veryGoodDamage, "@ swing|swings $2 at $1's arms",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Mattock 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.8, mattock, greatDamage, "@ swing|swings $2 in an overhead blow at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Mattock 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.0, 1.3, mattock, badDamage,
            "@ heave|heaves the haft of $2 down towards $1's head");

        AddAttack("Mattock Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3,
            mattock, veryGoodDamage, "@ swing|swings $2 at $1");
        AddAttack("Mattock Strike Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            mattock, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.ArmourPiercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
        AddAttack("Mattock Strike Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            mattock, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.ArmourPiercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
        AddAttack("Mattock Strike Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, mattock, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.ArmourPiercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
        AddAttack("Mattock Strike Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, mattock, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.ArmourPiercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
        AddAttack("Mattock Strike Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, mattock, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.ArmourPiercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
        AddAttack("Mattock Strike Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            mattock, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.ArmourPiercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());

        AddAttack("Mattock 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.2, mattock, veryGoodDamage, "@ swing|swings $2 at $1", DamageType.ArmourPiercing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Mattock 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.2, mattock, veryGoodDamage, "@ swing|swings $2 high at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Mattock 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 6.5, 1.2, mattock, veryGoodDamage,
            "@ swing|swings $2 low at $1", DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Mattock 2-Handed Heavy Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.High, 8.5, 1.4, mattock, greatDamage, "@ swing|swings $2 at $1", DamageType.ArmourPiercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Mattock 2-Handed Heavy High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.ExtremelyEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy,
            Alignment.FrontRight, Orientation.Highest, 8.5, 1.4, mattock, greatDamage, "@ swing|swings $2 high at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Mattock 2-Handed Heavy Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy, Alignment.FrontRight,
            Orientation.Centre, 8.5, 1.4, mattock, greatDamage, "@ swing|swings $2 low at $1", DamageType.ArmourPiercing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Mattock 2-Handed Downed Killing Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy, Alignment.Front,
            Orientation.Highest, 8.5, 1.4, mattock, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Mattock 2-Handed Downed Hobbling Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Lowest, 8.5, 1.4, mattock, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Mattock 2-Handed Downed Maiming Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Appendage, 8.5, 1.4, mattock, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Mattock 2-Handed Downed Finishing Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 8.5, 1.4, mattock, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());

        AddAttack("Training Mattock 1-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.4, trainingmattock, trainingDamage, "@ swing|swings $2 at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Mattock 1-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.6, trainingmattock, trainingDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Mattock 1-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 6.5, 1.4, trainingmattock, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Mattock 1-Handed Leg Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
            6.5, 1.6, trainingmattock, trainingDamage, "@ swing|swings $2 at $1's legs", DamageType.ArmourPiercing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Mattock 1-Handed Arm Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 6.5, 1.6, trainingmattock, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Mattock 1-Handed Overhead Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Easy,
            Alignment.Front, Orientation.Highest, 8.0, 1.8, trainingmattock, trainingDamage,
            "@ swing|swings $2 in an overhead blow at $1", DamageType.ArmourPiercing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Mattock 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.0, 1.3, trainingmattock, trainingDamage,
            "@ heave|heaves the haft of $2 down towards $1's head");

        AddAttack("Training Mattock 2-Handed Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.2, trainingmattock, trainingDamage, "@ swing|swings $2 at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Training Mattock 2-Handed High Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.2, trainingmattock, trainingDamage, "@ swing|swings $2 high at $1",
            DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Training Mattock 2-Handed Low Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 6.5, 1.2, trainingmattock, trainingDamage,
            "@ swing|swings $2 low at $1", DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Training Mattock 2-Handed Heavy Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy,
            Alignment.FrontRight, Orientation.High, 8.5, 1.4, trainingmattock, trainingDamage,
            "@ swing|swings $2 at $1", DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Training Mattock 2-Handed Heavy High Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.ExtremelyEasy, Difficulty.ExtremelyHard,
            Difficulty.VeryEasy, Alignment.FrontRight, Orientation.Highest, 8.5, 1.4, trainingmattock, trainingDamage,
            "@ swing|swings $2 high at $1", DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Training Mattock 2-Handed Heavy Low Swing", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 8.5, 1.4, trainingmattock, trainingDamage,
            "@ swing|swings $2 low at $1", DamageType.ArmourPiercing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        #endregion

        #region Warhammers

        WeaponType warhammer = new()
        {
            Name = "Warhammer",
            Classification = (int)WeaponClassification.Lethal,
            AttackTrait = skills["Warhammer"],
            ParryTrait = parrySkills["Warhammer"],
            ParryBonus = -3,
            Reach = 4,
            StaminaPerParry = 5.0
        };
        context.WeaponTypes.Add(warhammer);
        context.SaveChanges();
        CreateWeaponComponent(warhammer);
        WeaponType trainingwarhammer = new()
        {
            Name = "Training Warhammer",
            Classification = (int)WeaponClassification.Training,
            AttackTrait = skills["Warhammer"],
            ParryTrait = parrySkills["Warhammer"],
            ParryBonus = -3,
            Reach = 4,
            StaminaPerParry = 5.0
        };
        context.WeaponTypes.Add(trainingwarhammer);
        context.SaveChanges();
        CreateWeaponComponent(trainingwarhammer);

        AddAttack("Warhammer 1-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.4, warhammer, veryGoodDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Warhammer 1-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.6, warhammer, veryGoodDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Warhammer 1-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 6.5, 1.4, warhammer, veryGoodDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Warhammer 1-Handed Leg Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
            6.5, 1.6, warhammer, veryGoodDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Warhammer 1-Handed Arm Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 6.5, 1.6, warhammer, veryGoodDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Warhammer 1-Handed Overhead Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.8, warhammer, greatDamage, "@ swing|swings $2 in an overhead blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Warhammer 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.0, 1.3, warhammer, badDamage,
            "@ heave|heaves the haft of $2 down towards $1's head");

        AddAttack("Warhammer Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing, Difficulty.Easy,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 6.0, 1.3,
            warhammer, veryGoodDamage, "@ swing|swings $2 at $1");
        AddAttack("Warhammer Crush Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            warhammer, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());
        AddAttack("Warhammer Crush Neck", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            warhammer, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());
        AddAttack("Warhammer Crush Right Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, warhammer, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rhand").Id.ToString());
        AddAttack("Warhammer Crush Left Hand", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, warhammer, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lhand").Id.ToString());
        AddAttack("Warhammer Crush Right Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 8.0, 1.5, warhammer, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rshin").Id.ToString());
        AddAttack("Warhammer Crush Left Leg", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            warhammer, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1 in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "lshin").Id.ToString());

        AddAttack("Warhammer 2-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.2, warhammer, veryGoodDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly, additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Warhammer 2-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.2, warhammer, veryGoodDamage, "@ swing|swings $2 high at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Warhammer 2-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 6.5, 1.2, warhammer, veryGoodDamage,
            "@ swing|swings $2 low at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Warhammer 2-Handed Heavy Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.High, 8.5, 1.4, warhammer, greatDamage, "@ swing|swings $2 at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Warhammer 2-Handed Heavy High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.ExtremelyEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy,
            Alignment.FrontRight, Orientation.Highest, 8.5, 1.4, warhammer, greatDamage, "@ swing|swings $2 high at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Warhammer 2-Handed Heavy Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy, Alignment.FrontRight,
            Orientation.Centre, 8.5, 1.4, warhammer, greatDamage, "@ swing|swings $2 low at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Warhammer 2-Handed Downed Killing Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy, Alignment.Front,
            Orientation.Highest, 8.5, 1.4, warhammer, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Warhammer 2-Handed Downed Hobbling Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Lowest, 8.5, 1.4, warhammer, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Warhammer 2-Handed Downed Maiming Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Appendage, 8.5, 1.4, warhammer, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Warhammer 2-Handed Downed Finishing Blow", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 8.5, 1.4, warhammer, greatDamage,
            "@ raise|raises $2 above &0's head and bring|brings it crashing down towards $1's {0} as #1 %1|lay|lays prone and vulnerable",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());

        AddAttack("Training Warhammer 1-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.4, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Warhammer 1-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.6, trainingwarhammer, trainingDamage, "@ swing|swings $2 in a high blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Warhammer 1-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 6.5, 1.4, trainingwarhammer, trainingDamage, "@ swing|swings $2 in a low blow at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Warhammer 1-Handed Leg Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.Easy, Alignment.FrontRight, Orientation.Low,
            6.5, 1.6, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1's legs", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Warhammer 1-Handed Arm Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Hard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Appendage, 6.5, 1.6, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1's arms",
            DamageType.Crushing, handedness: AttackHandednessOptions.OneHandedOnly,
            additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Warhammer 1-Handed Overhead Swing", BuiltInCombatMoveType.StaggeringBlow,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Easy,
            Alignment.Front, Orientation.Highest, 8.0, 1.8, trainingwarhammer, trainingDamage,
            "@ swing|swings $2 in an overhead blow at $1", DamageType.Crushing,
            handedness: AttackHandednessOptions.OneHandedOnly, additionalInfo: ((int)Difficulty.Hard).ToString());
        AddAttack("Training Warhammer 1-Handed Haft Bash", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryHard, Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.0, 1.3, trainingwarhammer, trainingDamage,
            "@ heave|heaves the haft of $2 down towards $1's head");

        AddAttack("Training Warhammer 2-Handed Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.High, 6.5, 1.2, trainingwarhammer, trainingDamage, "@ swing|swings $2 at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Training Warhammer 2-Handed High Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy, Alignment.FrontRight,
            Orientation.Highest, 6.5, 1.2, trainingwarhammer, trainingDamage, "@ swing|swings $2 high at $1",
            DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Training Warhammer 2-Handed Low Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Normal, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 6.5, 1.2, trainingwarhammer, trainingDamage,
            "@ swing|swings $2 low at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.VeryHard).ToString());
        AddAttack("Training Warhammer 2-Handed Heavy Swing", BuiltInCombatMoveType.StaggeringBlow,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.VeryEasy, Difficulty.ExtremelyHard, Difficulty.VeryEasy,
            Alignment.FrontRight, Orientation.High, 8.5, 1.4, trainingwarhammer, trainingDamage,
            "@ swing|swings $2 at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Training Warhammer 2-Handed Heavy High Swing", BuiltInCombatMoveType.StaggeringBlow,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.ExtremelyEasy, Difficulty.ExtremelyHard,
            Difficulty.VeryEasy, Alignment.FrontRight, Orientation.Highest, 8.5, 1.4, trainingwarhammer, trainingDamage,
            "@ swing|swings $2 high at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());
        AddAttack("Training Warhammer 2-Handed Heavy Low Swing", BuiltInCombatMoveType.StaggeringBlow,
            MeleeWeaponVerb.Swing, Difficulty.Hard, Difficulty.Easy, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
            Alignment.FrontRight, Orientation.Centre, 8.5, 1.4, trainingwarhammer, trainingDamage,
            "@ swing|swings $2 low at $1", DamageType.Crushing, handedness: AttackHandednessOptions.TwoHandedOnly,
            additionalInfo: ((int)Difficulty.ExtremelyHard).ToString());

        #endregion

        #region Shields

        TraitDefinition blockingSkill = coreSkills.GetValueOrDefault("Blocking") ?? coreSkills["Block"];
        WeaponType shield = new()
        {
            Name = "Shield",
            Classification = (int)WeaponClassification.Shield,
            AttackTrait = blockingSkill,
            ParryTrait = blockingSkill,
            ParryBonus = -2,
            Reach = 0,
            StaminaPerParry = 3.5
        };
        context.WeaponTypes.Add(shield);
        context.SaveChanges();
        CreateWeaponComponent(shield);
        context.StaticConfigurations.Add(new StaticConfiguration
        { SettingName = "DefaultShieldMeleeWeaponType", Definition = shield.Id.ToString() });
        context.SaveChanges();

        AddAttack("Shield Bash", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Bash, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Insane, Difficulty.Hard, Alignment.FrontLeft, Orientation.High, 4.0, 0.8,
            shield, poorDamage, "@ drive|drives $2 forward in a shield bash at $1", DamageType.Crushing,
            additionalInfo: ((int)Difficulty.Trivial).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Shield);
        AddAttack("Shield Drive", BuiltInCombatMoveType.Pushback, MeleeWeaponVerb.Bash, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Insane, Difficulty.Hard, Alignment.FrontLeft, Orientation.Centre, 4.5, 0.9,
            shield, poorDamage, "@ drive|drives $2 hard into $1 to force &1 back", DamageType.Crushing,
            additionalInfo: ((int)Difficulty.Normal).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Shield);
        AddAttack("Shield Shove", BuiltInCombatMoveType.ForcedMovement, MeleeWeaponVerb.Bash, Difficulty.Hard,
            Difficulty.Normal, Difficulty.Insane, Difficulty.Hard, Alignment.FrontLeft, Orientation.Centre, 5.0, 1.1,
            shield, poorDamage, "@ shove|shoves $2 hard into $1", DamageType.Crushing,
            additionalInfo: ForcedMovementAttackData(Difficulty.Normal, ForcedMovementTypes.All,
                ForcedMovementVerbs.Shove, ForcedMovementRange.Melee),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Disadvantage | CombatMoveIntentions.Shield);
        AddAttack("Shield Head Smash", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Slam, Difficulty.Normal,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft, Orientation.Highest, 4.0, 0.8,
            shield, poorDamage, "@ whip|whips the rim of $2 down at $1's head", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Shield);
        AddAttack("Shield Foot Stomp", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Slam, Difficulty.Hard,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Alignment.FrontLeft, Orientation.Lowest, 4.0, 0.8,
            shield, poorDamage, "@ chop|chops $2 downward toward $1's legs", DamageType.Crushing,
            additionalInfo: ((int)Difficulty.Easy).ToString(),
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Shield |
                        CombatMoveIntentions.Cripple);
        AddAttack("Shield Smash Head", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing, Difficulty.Hard,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Easy, Alignment.Front, Orientation.Highest, 8.0, 1.5,
            shield, coupdegraceDamage,
            "@ hold|holds $2 up above &0's head and bring|brings it down at $1's {0} in a devastating swing",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        #endregion
    }
}
