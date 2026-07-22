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

public partial class CombatSeeder : IDatabaseSeeder
{
	private static readonly string[] RpiLegacyCombatSkillNames =
	[
		"Small-Blade",
		"Sword",
		"Axe",
		"Polearm",
		"Club",
		"Flail",
		"Double-Handed",
		"Sole-Wield",
		"Shield-Use",
		"Dual-Wield",
		"Throwing",
		"Blowgun",
		"Sling",
		"Hunting-bow",
		"Warbow",
		"Avert"
	];

	internal static IReadOnlyCollection<string> RpiLegacyCombatSkillNamesForTesting => RpiLegacyCombatSkillNames;

	internal static IReadOnlyCollection<string> RpiNonGerundCombatSkillNamesForTesting =>
		new[]
		{
			"Block",
			"Dodge",
			"Brawling",
			"Subdue",
			"Ward",
			"Throwing",
			"Veterancy",
			"Parry"
		}
			.Concat(RpiLegacyCombatSkillNames)
			.ToArray();


    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions
        => new List<(string Id, string Question,
            Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
            Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
        {
            ("installmuskets",
                "Do you want to install some early gunpowder weapon types?\n\nPlease answer #3yes#f or #3no#f: ",
                (context, answers) => true,
                (answer, context) =>
                {
                    return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
                }),
            ("installguns",
                "Do you want to install some modern firearm types?\n\nPlease answer #3yes#f or #3no#f: ",
                (context, answers) => true,
                (answer, context) =>
                {
                    return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
                }),
            ("random", @"#DDamage Formulas#F

You can configure your damage formulas to be consistent or random. The engine already takes into account a number of variables such as relative success of attacker and defender, type of defense used, all of which ensure that the damage is mitigated differently each attack. However, a good hit is usually pretty impactful in that kind of setup.

Randomness in damage is sometimes used to add complexity or choice to weapon types when the outcome of the attack is fairly likely (see D20 systems where before long hitting is almost guaranteed). This can work too but it can be disappointing for someone to land a good blow with all the factors right and then simply do little damage because of RNG, whereas another just-barely hit might do full damage.

There are three options that you can choose for randomness:

#BStatic#F: In this option (which was used in LabMUD) base damage is static. A hit with the same quality weapon, the same strength and the same attack/defense result will lead to the same damage
#BPartial#F: In this option 30% of the damage will be random - this adds a little bit of uncertainty and variety but still makes hits /largely/ a function of relative success
#BRandom#F: In this option damage can be 20-100% of the maximum. This means outcomes will vary wildly.
Which option do you want to use for random results in your weapon damage formulas?",
                (context, answers) => !CombatBalanceProfileHelper.UsesCombatRebalance(context, answers),
                (answer, context) =>
                {
                    return (answer.EqualToAny("static", "partial", "random"),
                        "You must answer static, partial or random.");
                }),
            ("parryoption",
                @"Do you want to use a separate skill called 'parry' as the skill for parrying with weapons? If you answer no, the weapon will use its attacking trait for parrying instead.

Please answer #3yes#f or #3no#f: ", (context, answers) => true,
                (answer, context) =>
                {
                    return (answer.EqualToAny("yes", "y", "no", "n"), "You must answer yes or no.");
                }),
            ("skilloption", @"#DWeapon Skills#F

There are many different ways that you could set up combat skills. A few options are presented here. Keep in mind that you can always change the names of these options later, better that you pick something that lines up with the overall shape that you want than fixing on the names or such.

#BWeapons#f: In this option each weapon has its own skill, e.g. Swords, Axes, Spears, Daggers
#BBroad#f: In this option you have broad categories across multiple weapons such as Edged, Bludgeoning, Piercing, Two-Handed
#BSOI#f: This option sets up skills like old SOI had, with 9 combinations of light, medium, heavy and edged, bludgeon and piercing (e.g. medium-edge etc)

Which option do you want to choose for weapon skills? ",
                (context, answers) => true,
                (answer, context) =>
                {
                    return (answer.EqualToAny("weapons", "broad", "soi"),
                        "You must answer 'weapons', 'broad' or 'soi'.");
                }),
            ("messagestyle", @"#DCombat Messages#F

Combat messages can be presented in a number of different styles. Fundamentally, the attack and the defense against the attack are different messages. You can either have them come together to form a single sentence, or you can keep them separate sentences, or you can put them on entirely different lines. For example, here are the three options you could consider:

#BCompact#F

	A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger, who tries to dodge but gets hit on the head!

#BSentences#F

	A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger. He tries to dodge out of the way but is unsuccessful. He is hit on the head.

#BSparse#F

	A tall, bearded man swings a steel longsword at a pudgy, brown-haired codger.
	He tries to dodge out of the way but is unsuccessful.
	He is hit on the head!

You can change your decision later, you're just going to have to go and edit your combat messages (mostly the defenses) to match the style you want. One advantage to doing Sentences or Sparse is that you can easily colour whole elements if you prefer (some people prefer not to of course).

You can choose #3Compact#f, #3Sentences#f or #3Sparse#f",
                (context, answers) => true,
                (answer, context) =>
                {
                    return (answer.EqualToAny("compact", "sentences", "sparse"),
                        "You must answer Compact, Sentences or Sparse.");
                })
        };

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        IReadOnlyDictionary<string, string> effectiveAnswers =
            CombatBalanceProfileHelper.MergeQuestionAnswersWithRecordedChoice(context, questionAnswers);
        effectiveAnswers = CombatSeederMessageStyleHelper.MergeQuestionAnswersWithRecordedChoice(context, effectiveAnswers);

        context.Database.BeginTransaction();
        if (context.WeaponAttacks.Any())
        {
            string updateSummary = RefreshExistingCombatSeederContent(context, effectiveAnswers);
            context.Database.CommitTransaction();
            return updateSummary;
        }

        IReadOnlyDictionary<string, TraitDefinition> skills = SeedCoreData(context, effectiveAnswers);

        SeedDataUnarmed(context, effectiveAnswers, skills);
        SeedDataWeapons(context, effectiveAnswers, skills);
        EnsureExpandedStockCombatContent(context, effectiveAnswers, skills);
        SeedDataRanged(context, effectiveAnswers, skills);

        if (effectiveAnswers["installmuskets"].EqualToAny("yes", "y"))
        {
            SeedDataMuskets(context, effectiveAnswers);
        }

        if (effectiveAnswers["installguns"].EqualToAny("yes", "y"))
        {
            SeedDataGuns(context, effectiveAnswers);
        }

        SeedArmourTypes(context, effectiveAnswers);
        CombatAuxiliarySeederHelper.EnsureStockAuxiliaryContent(context);
        ManualCombatCommandSeederHelper.EnsureStockManualCombatCommands(context);

        context.Database.CommitTransaction();

        return "The operation completed successfully.";
    }

    private string RefreshExistingCombatSeederContent(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        int updatedExpressionCount = 0;
        CombatStockExpansionResult expandedStockResult = CombatStockExpansionResult.Empty;
        if (questionAnswers["installweapons"].EqualToAny("yes", "y"))
        {
            TraitDefinition strength = GetStrengthAttribute(context);
            foreach (KeyValuePair<string, string> formula in BuildWeaponDamageExpressions(strength.Id, questionAnswers))
            {
                UpsertTraitExpression(context, formula.Key, formula.Value);
                updatedExpressionCount++;
            }

            expandedStockResult = EnsureExpandedStockCombatContent(context, questionAnswers);
        }

        if (questionAnswers["installunarmed"].EqualToAny("yes", "y"))
        {
            TraitDefinition strength = GetStrengthAttribute(context);
            foreach (KeyValuePair<string, string> formula in BuildUnarmedDamageExpressions(strength.Id, questionAnswers))
            {
                UpsertTraitExpression(context, formula.Key, formula.Value);
                updatedExpressionCount++;
            }
        }

        int primitiveRangedCount = 0;
        if (questionAnswers["installranged"].EqualToAny("yes", "y"))
        {
            primitiveRangedCount = EnsurePrimitiveRangedContent(context);
        }

        context.SaveChanges();
        List<string> updates = new();
        if (updatedExpressionCount > 0)
        {
            updates.Add($"updated {updatedExpressionCount} stock combat damage expressions");
        }

        if (primitiveRangedCount > 0)
        {
            updates.Add($"added {primitiveRangedCount} missing sling/blowgun stock entries");
        }

        if (expandedStockResult.HasChanges)
        {
            updates.Add(
                $"added expanded stock combat content ({expandedStockResult.ShieldTypes} shields, {expandedStockResult.WeaponTypes} weapon types, {expandedStockResult.WeaponAttacks} attacks, {expandedStockResult.Components} components)");
        }

        CombatAuxiliarySeedResult auxiliaryResult = CombatAuxiliarySeederHelper.EnsureStockAuxiliaryContent(context);
        if (auxiliaryResult.HasChanges)
        {
            updates.Add(
                $"refreshed auxiliary combat stock ({auxiliaryResult.Actions} actions, {auxiliaryResult.Messages} messages, {auxiliaryResult.RaceLinks} race links, {auxiliaryResult.Strategies} strategies)");
        }

        int manualCombatCommands = ManualCombatCommandSeederHelper.EnsureStockManualCombatCommands(context);
        if (manualCombatCommands > 0)
        {
            updates.Add($"refreshed {manualCombatCommands} stock manual combat command bindings");
        }

        return updates.Count > 0
            ? $"{updates.ListToString()}."
            : "Combat content already exists. No stock damage expressions, expanded weapon stock, or ranged additions were selected for update.";
    }

    private readonly record struct CombatStockExpansionResult(int ShieldTypes, int WeaponTypes, int WeaponAttacks,
        int Components)
    {
        public static CombatStockExpansionResult Empty => new(0, 0, 0, 0);

        public bool HasChanges => ShieldTypes > 0 || WeaponTypes > 0 || WeaponAttacks > 0 || Components > 0;
    }

	internal static bool ApplySeededMaximumTargets(WeaponAttack attack, int maximumTargets)
	{
		if (maximumTargets <= 1 || attack.MaximumTargets == maximumTargets)
		{
			return false;
		}

		attack.MaximumTargets = maximumTargets;
		return true;
	}

    private CombatStockExpansionResult EnsureExpandedStockCombatContent(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers,
        IReadOnlyDictionary<string, TraitDefinition>? seededSkills = null)
    {
        int shieldTypesAdded = 0;
        int weaponTypesAdded = 0;
        int weaponAttacksAdded = 0;
        int componentsAdded = 0;
        Account dbaccount = context.Accounts.First();
        DateTime now = DateTime.UtcNow;

        EditableItem CreateEditableItem()
        {
            return new EditableItem
            {
                RevisionNumber = 0,
                RevisionStatus = 4,
                BuilderAccountId = dbaccount.Id,
                BuilderDate = now,
                BuilderComment = "Auto-generated by the system",
                ReviewerAccountId = dbaccount.Id,
                ReviewerComment = "Auto-generated by the system",
                ReviewerDate = now
            };
        }

        GameItemComponentProto EnsureComponent(string type, string name, string description, string definition)
        {
            GameItemComponentProto? existing = context.GameItemComponentProtos
                .FirstOrDefault(x => x.Type == type && x.Name == name);
            if (existing is not null)
            {
                return existing;
            }

            GameItemComponentProto component = new()
            {
                Id = context.GameItemComponentProtos.Any() ? context.GameItemComponentProtos.Max(x => x.Id) + 1 : 1,
                RevisionNumber = 0,
                EditableItem = CreateEditableItem(),
                Type = type,
                Name = name,
                Description = description,
                Definition = definition
            };
            context.GameItemComponentProtos.Add(component);
            context.SaveChanges();
            componentsAdded++;
            return component;
        }

        void EnsureMeleeWeaponComponent(WeaponType type)
        {
            EnsureComponent("MeleeWeapon", $"Melee_{type.Name}", $"Turns an item into a {type.Name} melee weapon",
                $"<Definition><WeaponType>{type.Id}</WeaponType></Definition>");
        }

        void EnsureShieldComponent(ShieldType type)
        {
            EnsureComponent("Shield", $"Shield_{type.Name.Replace(' ', '_')}",
                $"Turns an item into a {type.Name} shield",
                $"<Definition><ShieldType>{type.Id}</ShieldType></Definition>");
        }

        if (!context.ShieldTypes.Any())
        {
            SeedShields(context, questionAnswers, seededSkills ?? new Dictionary<string, TraitDefinition>());
        }

        ArmourType shieldArmour = context.ArmourTypes.First(x => x.Name == "Shield Armour");
        TraitDefinition blockSkill = context.TraitDefinitions.First(x => x.Name == "Blocking" || x.Name == "Block");
        foreach ((string name, double blockBonus, double staminaPerBlock) in new (string, double, double)[]
                 {
                     ("Hide", -1.0, 6.0),
                     ("Wicker", -0.5, 6.5),
                     ("Parma", -0.5, 5.0),
                     ("Dhal", 0.0, 5.5),
                     ("Adarga", 0.0, 6.0),
                     ("Targe", 0.0, 5.0),
                     ("Rotella", 0.0, 6.0),
                     ("Thureos", 0.5, 7.5),
                     ("Aspis", 1.0, 9.0),
                     ("Scutum", 1.0, 9.5),
                     ("Pavise", 1.5, 12.0),
                     ("Trench", 0.5, 9.0),
                     ("Riot", 1.0, 7.0),
                     ("Ballistic", 1.5, 11.0)
                 })
        {
            ShieldType? shield = context.ShieldTypes.FirstOrDefault(x => x.Name == name);
            if (shield is null)
            {
                shield = new ShieldType
                {
                    Name = name,
                    EffectiveArmourType = shieldArmour,
                    BlockBonus = blockBonus,
                    BlockTrait = blockSkill,
                    StaminaPerBlock = staminaPerBlock
                };
                context.ShieldTypes.Add(shield);
                context.SaveChanges();
                shieldTypesAdded++;
            }

            EnsureShieldComponent(shield);
        }

        TraitDefinition? FindSkill(params string[] names)
        {
            if (seededSkills is not null)
            {
                TraitDefinition? seeded = seededSkills
                    .FirstOrDefault(x => names.Contains(x.Key, StringComparer.OrdinalIgnoreCase))
                    .Value;
                if (seeded is not null)
                {
                    return seeded;
                }
            }

            return context.TraitDefinitions
                .AsEnumerable()
                .FirstOrDefault(x => names.Contains(x.Name, StringComparer.OrdinalIgnoreCase));
        }

        WeaponType GetExistingWeaponType(string name)
        {
            return context.WeaponTypes
                .Include(x => x.AttackTrait)
                .Include(x => x.ParryTrait)
                .First(x => x.Name == name);
        }

        TraitDefinition ResolveAttackSkill(string[] preferredNames, params string[] donorWeaponTypes)
        {
            return FindSkill(preferredNames) ??
                   donorWeaponTypes
                       .Select(x => GetExistingWeaponType(x).AttackTrait)
                       .First(x => x is not null);
        }

        TraitDefinition ResolveParrySkill(string[] preferredNames, params string[] donorWeaponTypes)
        {
            TraitDefinition? parry = FindSkill("Parrying", "Parry");
            if (parry is not null)
            {
                return parry;
            }

            return FindSkill(preferredNames) ??
                   donorWeaponTypes
                       .Select(x => GetExistingWeaponType(x).ParryTrait)
                       .First(x => x is not null);
        }

        WeaponType EnsureWeaponType(string name, WeaponClassification classification, TraitDefinition attackTrait,
            TraitDefinition parryTrait, double parryBonus, int reach, double staminaPerParry)
        {
            WeaponType? existing = context.WeaponTypes.FirstOrDefault(x => x.Name == name);
            if (existing is not null)
            {
                EnsureMeleeWeaponComponent(existing);
                return existing;
            }

            WeaponType created = new()
            {
                Name = name,
                Classification = (int)classification,
                AttackTrait = attackTrait,
                ParryTrait = parryTrait,
                ParryBonus = parryBonus,
                Reach = reach,
                StaminaPerParry = staminaPerParry
            };
            context.WeaponTypes.Add(created);
            context.SaveChanges();
            weaponTypesAdded++;
            EnsureMeleeWeaponComponent(created);
            return created;
        }

        TraitDefinition staffAttack = ResolveAttackSkill(
            ["Staff", "Clubs", "Bludgeoning Weapons", "Medium-Blunt", "Club"], "Club");
        TraitDefinition staffParry = ResolveParrySkill(
            ["Staff", "Clubs", "Bludgeoning Weapons", "Medium-Blunt", "Club"], "Club");
        WeaponType quarterstaff = EnsureWeaponType("Quarterstaff", WeaponClassification.Lethal, staffAttack,
            staffParry, 2.0, 4, 2.0);
        WeaponType trainingQuarterstaff = EnsureWeaponType("Training Quarterstaff", WeaponClassification.Training,
            staffAttack, staffParry, 2.0, 4, 1.5);

        TraitDefinition polearmAttack = ResolveAttackSkill(
            ["Polearms", "Polearm", "Spears", "Heavy-Pierce", "Two Handed Weapons"], "Halberd", "Long Spear");
        TraitDefinition polearmParry = ResolveParrySkill(
            ["Polearms", "Polearm", "Spears", "Heavy-Pierce", "Two Handed Weapons"], "Halberd", "Long Spear");
        WeaponType pike = EnsureWeaponType("Pike", WeaponClassification.Lethal, polearmAttack, polearmParry, 0.0, 6,
            2.5);
        WeaponType trainingPike = EnsureWeaponType("Training Pike", WeaponClassification.Training, polearmAttack,
            polearmParry, 0.0, 6, 2.0);

        TraitDefinition twoHandedAxeAttack = ResolveAttackSkill(
            ["Axes", "Axe", "Two Handed Weapons", "Heavy-Edge", "Double-Handed"], "Axe", "Two Handed Sword");
        TraitDefinition twoHandedAxeParry = ResolveParrySkill(
            ["Axes", "Axe", "Two Handed Weapons", "Heavy-Edge", "Double-Handed"], "Axe", "Two Handed Sword");
        WeaponType twoHandedAxe = EnsureWeaponType("Two Handed Axe", WeaponClassification.Lethal,
            twoHandedAxeAttack, twoHandedAxeParry, -1.0, 4, 4.5);
        WeaponType trainingTwoHandedAxe = EnsureWeaponType("Training Two Handed Axe", WeaponClassification.Training,
            twoHandedAxeAttack, twoHandedAxeParry, -1.0, 4, 3.5);

        TraitDefinition flailAttack = ResolveAttackSkill(
            ["Flail", "Maces", "Bludgeoning Weapons", "Medium-Blunt", "Heavy-Blunt"], "Mace", "Club");
        TraitDefinition flailParry = ResolveParrySkill(
            ["Flail", "Maces", "Bludgeoning Weapons", "Medium-Blunt", "Heavy-Blunt"], "Mace", "Club");
        WeaponType flail = EnsureWeaponType("Flail", WeaponClassification.Lethal, flailAttack, flailParry, -3.0, 3,
            4.0);
        WeaponType trainingFlail = EnsureWeaponType("Training Flail", WeaponClassification.Training, flailAttack,
            flailParry, -3.0, 3, 3.0);

        TraitDefinition pickAttack = ResolveAttackSkill(
            ["Warhammers", "Maces", "Bludgeoning Weapons", "Heavy-Blunt", "Medium-Blunt"], "Warhammer", "Mace");
        TraitDefinition pickParry = ResolveParrySkill(
            ["Warhammers", "Maces", "Bludgeoning Weapons", "Heavy-Blunt", "Medium-Blunt"], "Warhammer", "Mace");
        WeaponType militaryPick = EnsureWeaponType("Military Pick", WeaponClassification.Lethal, pickAttack,
            pickParry, -2.0, 3, 3.5);
        WeaponType trainingMilitaryPick = EnsureWeaponType("Training Military Pick", WeaponClassification.Training,
            pickAttack, pickParry, -2.0, 3, 2.5);

        TraitDefinition estocAttack = ResolveAttackSkill(
            ["Swords", "Rapier", "Edged Weapons", "Medium-Pierce", "Medium-Edge"], "Rapier", "Longsword");
        TraitDefinition estocParry = ResolveParrySkill(
            ["Swords", "Rapier", "Edged Weapons", "Medium-Pierce", "Medium-Edge"], "Rapier", "Longsword");
        WeaponType estoc = EnsureWeaponType("Estoc", WeaponClassification.Lethal, estocAttack, estocParry, 1.0, 4,
            2.5);
        WeaponType trainingEstoc = EnsureWeaponType("Training Estoc", WeaponClassification.Training, estocAttack,
            estocParry, 1.0, 4, 2.0);

        WeaponType spear = GetExistingWeaponType("Short Spear");
        WeaponType longspear = GetExistingWeaponType("Long Spear");
        WeaponType trainingspear = GetExistingWeaponType("Training Spear");

        TraitDefinition strength = GetStrengthAttribute(context);
        SeedCombatMessageStyle messageStyle = CombatSeederMessageStyleHelper.Parse(questionAnswers["messagestyle"]);
        IReadOnlyDictionary<string, string> damageExpressions = BuildWeaponDamageExpressions(strength.Id,
            questionAnswers);
        TraitExpression trainingDamage = UpsertTraitExpression(context, "Weapon Damage - Training",
            damageExpressions["Weapon Damage - Training"]);
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

        string SecondaryDifficulty(Difficulty difficulty)
        {
            return ((int)difficulty).ToString();
        }

        void AddAttack(string name, BuiltInCombatMoveType moveType, MeleeWeaponVerb verb, Difficulty attacker,
            Difficulty dodge, Difficulty parry, Difficulty block, Alignment alignment, Orientation orientation,
            double stamina, double relativeSpeed, WeaponType type, TraitExpression damage, string attackMessage,
            DamageType damageType = DamageType.Crushing, double weighting = 100,
            CombatMoveIntentions intentions =
                CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill,
			string? additionalInfo = null, AttackHandednessOptions handedness = AttackHandednessOptions.Any,
			int maximumTargets = 1)
        {
            WeaponAttack? existingAttack = context.WeaponAttacks.FirstOrDefault(x =>
                x.WeaponTypeId == type.Id &&
                x.Name == name &&
                x.MoveType == (int)moveType &&
                x.HandednessOptions == (int)handedness);
            if (existingAttack is not null)
            {
				if (ApplySeededMaximumTargets(existingAttack, maximumTargets))
				{
					context.SaveChanges();
				}

                return;
            }

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
                AdditionalInfo = additionalInfo,
                RequiredPositionStateIds = "1 16 17 18"
            };
            context.WeaponAttacks.Add(attack);
            context.SaveChanges();
            weaponAttacksAdded++;

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

        AddAttack("Quarterstaff Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 3.0, 0.7, quarterstaff, normalDamage,
            "@ snap|snaps $2 forward in a quick jab at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast);
        AddAttack("Quarterstaff Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontLeft,
            Orientation.Low, 4.0, 1.0, quarterstaff, poorDamage,
            "@ sweep|sweeps $2 low toward $1's legs", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Hinder | CombatMoveIntentions.Disadvantage,
			additionalInfo: SecondaryDifficulty(Difficulty.Normal), maximumTargets: 3);
		AddAttack("Quarterstaff Whirling Strike", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Whirl,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
			Orientation.Centre, 5.5, 1.3, quarterstaff, normalDamage,
			"@ whirl|whirls $2 in a broad arc through $1's position", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Flashy |
			            CombatMoveIntentions.Slow,
			handedness: AttackHandednessOptions.TwoHandedOnly, maximumTargets: 3);
		AddAttack("Quarterstaff Hook and Pull", BuiltInCombatMoveType.PullToMelee, MeleeWeaponVerb.Sweep,
			Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
			Orientation.Centre, 5.0, 1.1, quarterstaff, poorDamage,
			"@ hook|hooks $2 around $1 and haul|hauls &1 into close reach", DamageType.Crushing,
			intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Hinder |
			            CombatMoveIntentions.Disadvantage,
			additionalInfo: SecondaryDifficulty(Difficulty.Hard),
			handedness: AttackHandednessOptions.TwoHandedOnly, maximumTargets: 2);
        AddAttack("Quarterstaff Counter-Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 3.5, 0.6, quarterstaff, normalDamage,
            "@ turn|turns the ward aside and jab|jabs $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast);
        AddAttack("Quarterstaff Butt Strike", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 3.0, 0.5, quarterstaff, poorDamage,
            "@ drive|drives the butt of $2 into $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Fast);
        AddAttack("Quarterstaff Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Bash,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.0, 0.8, quarterstaff, normalDamage, "@ smash|smashes $2 into $1",
            DamageType.Crushing);
        AddAttack("Quarterstaff Skull Crack", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Bash,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, quarterstaff, goodDamage,
            "@ bring|brings $2 down in a finishing blow on $1's skull", DamageType.Crushing,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        AddAttack("Training Quarterstaff Jab", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Jab,
            Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 3.0, 0.7, trainingQuarterstaff, trainingDamage,
            "@ snap|snaps $2 forward in a quick jab at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Quarterstaff Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontLeft,
            Orientation.Low, 4.0, 1.0, trainingQuarterstaff, trainingDamage,
            "@ sweep|sweeps $2 low toward $1's legs", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder |
                        CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Normal));
        AddAttack("Training Quarterstaff Counter-Jab", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Jab,
            Difficulty.Easy, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 3.5, 0.6, trainingQuarterstaff, trainingDamage,
            "@ turn|turns the ward aside and jab|jabs $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Quarterstaff Butt Strike", BuiltInCombatMoveType.ClinchAttack, MeleeWeaponVerb.Bash,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.Centre, 3.0, 0.5, trainingQuarterstaff, trainingDamage,
            "@ drive|drives the butt of $2 into $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Fast);
        AddAttack("Training Quarterstaff Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Bash,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 4.0, 0.8, trainingQuarterstaff, trainingDamage, "@ smash|smashes $2 into $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Quarterstaff Skull Crack", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Bash,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, trainingQuarterstaff, trainingDamage,
            "@ bring|brings $2 down in a finishing blow on $1's skull", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        AddAttack("Pike Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 4.5, 1.1, pike, goodDamage, "@ thrust|thrusts $2 at $1 from long reach",
            DamageType.Piercing);
        AddAttack("Pike High Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, pike, goodDamage, "@ lift|lifts $2 and thrust|thrusts high at $1",
            DamageType.Piercing);
        AddAttack("Pike Low Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Low, 4.5, 1.1, pike, normalDamage, "@ drive|drives $2 low toward $1's legs",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Pike Hooking Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.Low, 5.0, 1.2, pike, poorDamage, "@ hook|hooks $2 around $1's legs and pull|pulls",
            DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Hinder | CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Pike Set Counter", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.0, 0.8, pike, goodDamage, "@ set|sets $2 and counter-thrust|counter-thrusts at $1",
            DamageType.Piercing);
        AddAttack("Pike Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 4.0, 1.0, pike, normalDamage, "@ jab|jabs $2 hard into $1",
            DamageType.Piercing);
        AddAttack("Pike Chest Impale", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.High, 5.5, 1.4, pike, greatDamage, "@ set|sets $2 and drive|drives it through $1's chest",
            DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());

        AddAttack("Training Pike Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 4.5, 1.1, trainingPike, trainingDamage,
            "@ thrust|thrusts $2 at $1 from long reach", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Pike High Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, trainingPike, trainingDamage,
            "@ lift|lifts $2 and thrust|thrusts high at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Pike Low Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Low, 4.5, 1.1, trainingPike, trainingDamage,
            "@ drive|drives $2 low toward $1's legs", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
        AddAttack("Training Pike Hooking Sweep", BuiltInCombatMoveType.UnbalancingBlow, MeleeWeaponVerb.Sweep,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.Low, 5.0, 1.2, trainingPike, trainingDamage,
            "@ hook|hooks $2 around $1's legs and pull|pulls", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder |
                        CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Training Pike Set Counter", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.0, 0.8, trainingPike, trainingDamage,
            "@ set|sets $2 and counter-thrust|counter-thrusts at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Pike Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 4.0, 1.0, trainingPike, trainingDamage, "@ jab|jabs $2 hard into $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Pike Chest Impale", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.High, 5.5, 1.4, trainingPike, trainingDamage,
            "@ set|sets $2 and drive|drives it through $1's chest", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());

        AddAttack("Two Handed Axe Chop", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Chop,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.1, twoHandedAxe, goodDamage, "@ chop|chops $2 heavily at $1",
            DamageType.Chopping);
        AddAttack("Two Handed Axe Cleave", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Chop,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 6.0, 1.4, twoHandedAxe, greatDamage, "@ cleave|cleaves $2 in a brutal arc at $1",
            DamageType.Chopping);
        AddAttack("Two Handed Axe Hooking Blow", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Chop,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.High, 6.0, 1.3, twoHandedAxe, goodDamage, "@ hook|hooks $2 in a heavy blow at $1",
            DamageType.Chopping, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                                           CombatMoveIntentions.Kill | CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Two Handed Axe Downward Hew", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Chop,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Hard, Difficulty.Normal, Alignment.Front,
            Orientation.Low, 5.5, 1.2, twoHandedAxe, greatDamage, "@ hew|hews down at $1 with $2",
            DamageType.Chopping);
        AddAttack("Two Handed Axe Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Chop,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.1, twoHandedAxe, greatDamage, "@ chop|chops $2 into $1",
            DamageType.Chopping);
        AddAttack("Two Handed Axe Neck Hew", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Chop,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.High, 6.0, 1.4, twoHandedAxe, greatDamage, "@ hew|hews $2 down into $1's neck",
            DamageType.Chopping, additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());

        AddAttack("Training Two Handed Axe Chop", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Chop,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.FrontRight,
            Orientation.High, 5.0, 1.1, trainingTwoHandedAxe, trainingDamage,
            "@ chop|chops $2 heavily at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Two Handed Axe Cleave", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Chop,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 6.0, 1.4, trainingTwoHandedAxe, trainingDamage,
            "@ cleave|cleaves $2 in a brutal arc at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Two Handed Axe Hooking Blow", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Chop,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.High, 6.0, 1.3, trainingTwoHandedAxe, trainingDamage,
            "@ hook|hooks $2 in a heavy blow at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training |
                        CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Training Two Handed Axe Downward Hew", BuiltInCombatMoveType.DownedAttack, MeleeWeaponVerb.Chop,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Hard, Difficulty.Normal, Alignment.Front,
            Orientation.Low, 5.5, 1.2, trainingTwoHandedAxe, trainingDamage, "@ hew|hews down at $1 with $2",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Two Handed Axe Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Chop,
            Difficulty.VeryEasy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 5.0, 1.1, trainingTwoHandedAxe, trainingDamage, "@ chop|chops $2 into $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Two Handed Axe Neck Hew", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Chop,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.High, 6.0, 1.4, trainingTwoHandedAxe, trainingDamage, "@ hew|hews $2 down into $1's neck",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "bneck").Id.ToString());

        AddAttack("Flail Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 4.5, 1.1, flail, normalDamage, "@ swing|swings $2 in a snapping arc at $1",
            DamageType.Crushing);
        AddAttack("Flail Wrap Strike", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Whirl,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.Centre, 5.0, 1.2, flail, goodDamage, "@ whip|whips $2 around toward $1",
            DamageType.Crushing);
        AddAttack("Flail Heavy Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 5.5, 1.3, flail, goodDamage, "@ swing|swings $2 in a heavy, staggering arc at $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound |
                                           CombatMoveIntentions.Kill | CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Flail Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 4.5, 1.0, flail, goodDamage, "@ smash|smashes $2 into $1",
            DamageType.Crushing);
        AddAttack("Flail Skull Crush", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 5.5, 1.3, flail, greatDamage, "@ crash|crashes $2 down onto $1's head",
            DamageType.Crushing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        AddAttack("Training Flail Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 4.5, 1.1, trainingFlail, trainingDamage,
            "@ swing|swings $2 in a snapping arc at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Flail Wrap Strike", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Whirl,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.Centre, 5.0, 1.2, trainingFlail, trainingDamage, "@ whip|whips $2 around toward $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Flail Heavy Swing", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 5.5, 1.3, trainingFlail, trainingDamage,
            "@ swing|swings $2 in a heavy, staggering arc at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training |
                        CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Training Flail Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 4.5, 1.0, trainingFlail, trainingDamage, "@ smash|smashes $2 into $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Flail Skull Crush", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 5.5, 1.3, trainingFlail, trainingDamage,
            "@ crash|crashes $2 down onto $1's head", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        AddAttack("Military Pick Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 4.0, 1.0, militaryPick, normalDamage, "@ swing|swings $2 in a tight arc at $1",
            DamageType.Piercing);
        AddAttack("Military Pick Hook", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Chop,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.Centre, 4.5, 1.1, militaryPick, goodDamage, "@ hook|hooks $2 toward $1",
            DamageType.Piercing);
        AddAttack("Military Pick Puncture", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.5, 1.0, militaryPick, goodDamage, "@ punch|punches the point of $2 at $1",
            DamageType.Piercing);
        AddAttack("Military Pick Staggering Hook", BuiltInCombatMoveType.StaggeringBlow, MeleeWeaponVerb.Chop,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.High, 5.0, 1.2, militaryPick, goodDamage,
            "@ wrench|wrenches $2 in a staggering hook at $1", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Military Pick Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Chop,
            Difficulty.Normal, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 4.0, 1.0, militaryPick, goodDamage, "@ hammer|hammers $2 into $1",
            DamageType.Piercing);
        AddAttack("Military Pick Temple Spike", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Chop,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, militaryPick, greatDamage, "@ drive|drives $2 into $1's temple",
            DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        AddAttack("Training Military Pick Swing", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Swing,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.High, 4.0, 1.0, trainingMilitaryPick, trainingDamage,
            "@ swing|swings $2 in a tight arc at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Military Pick Hook", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Chop,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.FrontLeft,
            Orientation.Centre, 4.5, 1.1, trainingMilitaryPick, trainingDamage, "@ hook|hooks $2 toward $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Military Pick Puncture", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.5, 1.0, trainingMilitaryPick, trainingDamage,
            "@ punch|punches the point of $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Military Pick Staggering Hook", BuiltInCombatMoveType.StaggeringBlow,
            MeleeWeaponVerb.Chop, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal,
            Alignment.FrontLeft, Orientation.High, 5.0, 1.2, trainingMilitaryPick, trainingDamage,
            "@ wrench|wrenches $2 in a staggering hook at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training |
                        CombatMoveIntentions.Disadvantage,
            additionalInfo: SecondaryDifficulty(Difficulty.Hard));
        AddAttack("Training Military Pick Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Chop,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Centre, 4.0, 1.0, trainingMilitaryPick, trainingDamage, "@ hammer|hammers $2 into $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Military Pick Temple Spike", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Chop,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 5.0, 1.2, trainingMilitaryPick, trainingDamage,
            "@ drive|drives $2 into $1's temple", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "scalp").Id.ToString());

        AddAttack("Estoc Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 3.5, 0.9, estoc, normalDamage, "@ thrust|thrusts $2 straight at $1",
            DamageType.Piercing);
        AddAttack("Estoc Half-Sword Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Centre, 4.0, 1.0, estoc, goodDamage,
            "@ grip|grips $2 close and drive|drives a short thrust at $1", DamageType.Piercing);
        AddAttack("Estoc Low Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Low, 3.5, 0.9, estoc, normalDamage, "@ thrust|thrusts $2 low toward $1's legs",
            DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder);
        AddAttack("Estoc Counter-Thrust", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 3.5, 0.8, estoc, goodDamage, "@ slip|slips past the ward and thrust|thrusts $2 at $1",
            DamageType.Piercing);
        AddAttack("Estoc Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 3.5, 0.9, estoc, normalDamage, "@ thrust|thrusts $2 into $1",
            DamageType.Piercing);
        AddAttack("Estoc Heart Thrust", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.0, 1.0, estoc, greatDamage, "@ drive|drives $2 into $1's heart",
            DamageType.Piercing, additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());

        AddAttack("Training Estoc Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 3.5, 0.9, trainingEstoc, trainingDamage,
            "@ thrust|thrusts $2 straight at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Estoc Half-Sword Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Centre, 4.0, 1.0, trainingEstoc, trainingDamage,
            "@ grip|grips $2 close and drive|drives a short thrust at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Estoc Low Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.Low, 3.5, 0.9, trainingEstoc, trainingDamage,
            "@ thrust|thrusts $2 low toward $1's legs", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder);
        AddAttack("Training Estoc Counter-Thrust", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Hard, Difficulty.Normal, Difficulty.Easy, Alignment.Front,
            Orientation.High, 3.5, 0.8, trainingEstoc, trainingDamage,
            "@ slip|slips past the ward and thrust|thrusts $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Estoc Smash", BuiltInCombatMoveType.MeleeWeaponSmashItem, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.VeryHard, Difficulty.Insane, Difficulty.Normal, Alignment.Front,
            Orientation.Centre, 3.5, 0.9, trainingEstoc, trainingDamage, "@ thrust|thrusts $2 into $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training);
        AddAttack("Training Estoc Heart Thrust", BuiltInCombatMoveType.CoupDeGrace, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.0, 1.0, trainingEstoc, trainingDamage, "@ drive|drives $2 into $1's heart",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            additionalInfo: context.BodypartProtos.First(x => x.Name == "rbreast").Id.ToString());

        AddAttack("Spear Shield-Line Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.High, 3.5, 0.8, spear, normalDamage,
            "@ brace|braces behind a shield and thrust|thrusts $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Spear Shield-Line High Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 4.0, 0.9, spear, normalDamage,
            "@ lift|lifts $2 over the shield rim and thrust|thrusts high at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Spear Shield-Line Low Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Low, 3.5, 0.8, spear, poorDamage,
            "@ thrust|thrusts $2 low from behind a shield toward $1's legs", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Spear Shield-Line Leg Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 4.0, 0.9, spear, normalDamage,
            "@ stab|stabs $2 around a shield at $1's leg", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Spear Shield-Line Counter Thrust", BuiltInCombatMoveType.WardFreeAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.High, 3.5, 0.7, spear, goodDamage,
            "@ catch|catches the ward on a shield and counter-thrust|counter-thrusts with $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.SwordAndBoardOnly);

        AddAttack("Long Spear Shield-Line Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.High, 4.0, 0.9, longspear, normalDamage,
            "@ brace|braces behind a shield and thrust|thrusts $2 at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Long Spear Shield-Line High Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Hard, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Highest, 4.5, 1.0, longspear, goodDamage,
            "@ lift|lifts $2 over the shield rim and thrust|thrusts high at $1", DamageType.Piercing,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Long Spear Shield-Line Low Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Normal, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.Front,
            Orientation.Low, 4.0, 0.9, longspear, normalDamage,
            "@ thrust|thrusts $2 low from behind a shield toward $1's legs", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Long Spear Shield-Line Leg Thrust", BuiltInCombatMoveType.UseWeaponAttack, MeleeWeaponVerb.Thrust,
            Difficulty.Hard, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Alignment.FrontRight,
            Orientation.Low, 4.5, 1.0, longspear, normalDamage,
            "@ stab|stabs $2 around a shield at $1's leg", DamageType.Piercing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Wound | CombatMoveIntentions.Kill |
                        CombatMoveIntentions.Hinder,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Long Spear Shield-Line Counter Thrust", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Thrust, Difficulty.Hard, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal,
            Alignment.Front, Orientation.High, 4.0, 0.8, longspear, goodDamage,
            "@ catch|catches the ward on a shield and counter-thrust|counter-thrusts with $2 at $1",
            DamageType.Piercing, handedness: AttackHandednessOptions.SwordAndBoardOnly);

        AddAttack("Training Spear Shield-Line Thrust", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Thrust, Difficulty.VeryEasy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal,
            Alignment.Front, Orientation.High, 3.5, 0.8, trainingspear, trainingDamage,
            "@ brace|braces behind a shield and thrust|thrusts $2 at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Training Spear Shield-Line High Thrust", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Thrust, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal, Difficulty.Normal,
            Alignment.Front, Orientation.Highest, 4.0, 0.9, trainingspear, trainingDamage,
            "@ lift|lifts $2 over the shield rim and thrust|thrusts high at $1", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Training Spear Shield-Line Low Thrust", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Thrust, Difficulty.VeryEasy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal,
            Alignment.Front, Orientation.Low, 3.5, 0.8, trainingspear, trainingDamage,
            "@ thrust|thrusts $2 low from behind a shield toward $1's legs", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Training Spear Shield-Line Leg Thrust", BuiltInCombatMoveType.UseWeaponAttack,
            MeleeWeaponVerb.Thrust, Difficulty.Easy, Difficulty.Easy, Difficulty.Normal, Difficulty.Normal,
            Alignment.FrontRight, Orientation.Low, 4.0, 0.9, trainingspear, trainingDamage,
            "@ stab|stabs $2 around a shield at $1's leg", DamageType.Crushing,
            intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training | CombatMoveIntentions.Hinder,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);
        AddAttack("Training Spear Shield-Line Counter Thrust", BuiltInCombatMoveType.WardFreeAttack,
            MeleeWeaponVerb.Thrust, Difficulty.Easy, Difficulty.Hard, Difficulty.Normal, Difficulty.Normal,
            Alignment.Front, Orientation.High, 3.5, 0.7, trainingspear, trainingDamage,
            "@ catch|catches the ward on a shield and counter-thrust|counter-thrusts with $2 at $1",
            DamageType.Crushing, intentions: CombatMoveIntentions.Attack | CombatMoveIntentions.Training,
            handedness: AttackHandednessOptions.SwordAndBoardOnly);

        return new CombatStockExpansionResult(shieldTypesAdded, weaponTypesAdded, weaponAttacksAdded,
            componentsAdded);
    }

    private static TraitDefinition GetStrengthAttribute(FuturemudDatabaseContext context)
    {
        Dictionary<string, TraitDefinition> attributes = context.TraitDefinitions
            .Where(x => x.Type == 1 || x.Type == 3)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        return attributes.GetValueOrDefault("Strength") ??
               attributes.GetValueOrDefault("Physique") ??
               attributes["Body"];
    }

    private static int EnsurePrimitiveRangedContent(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, TraitDefinition>? seededSkills = null)
    {
        int added = 0;
        Account dbaccount = context.Accounts.First();
        DateTime now = DateTime.UtcNow;
        Dictionary<string, TraitDefinition> attributes = context.TraitDefinitions
            .Where(x => x.Type == 1 || x.Type == 3)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        TraitDefinition strength =
            attributes.GetValueOrDefault("Strength") ??
            attributes.GetValueOrDefault("Physique") ??
            attributes["Body"];
        TraitDefinition dex =
            attributes.GetValueOrDefault("Dexterity") ??
            attributes.GetValueOrDefault("Agility") ??
            attributes.GetValueOrDefault("Speed") ??
            attributes["Body"];
        TraitDefinition agi =
            attributes.GetValueOrDefault("Agility") ??
            attributes.GetValueOrDefault("Dexterity") ??
            attributes.GetValueOrDefault("Speed") ??
            attributes["Body"];
        TraitDefinition per =
            attributes.GetValueOrDefault("Perception") ??
            attributes.GetValueOrDefault("Intelligence") ??
            attributes.GetValueOrDefault("Wisdom") ??
            attributes["Intellect"];

        TraitDefinition? FindSkill(params string[] names)
        {
            foreach (string name in names)
            {
                if (seededSkills?.TryGetValue(name, out TraitDefinition? seeded) == true)
                {
                    return seeded;
                }

                TraitDefinition? existing = context.TraitDefinitions
                    .FirstOrDefault(x => x.Type == (int)TraitType.Skill && x.Name == name);
                if (existing != null)
                {
                    return existing;
                }
            }

            return null;
        }

        TraitDefinition EnsureSkill(string[] names, string createName, string expressionText)
        {
            TraitDefinition? existing = FindSkill(names);
            if (existing != null)
            {
                return existing;
            }

            TraitExpression expression = new()
            {
                Name = $"{createName} Skill Cap",
                Expression = expressionText
            };
            TraitDefinition skill = new()
            {
                Name = createName,
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
            added++;
            return skill;
        }

        long NextComponentProtoId()
        {
            return (context.GameItemComponentProtos.Any() ? context.GameItemComponentProtos.Max(x => x.Id) : 0) + 1;
        }

        EditableItem NewEditableItem()
        {
            return new EditableItem
            {
                RevisionNumber = 0,
                RevisionStatus = 4,
                BuilderAccountId = dbaccount.Id,
                BuilderDate = now,
                BuilderComment = "Auto-generated by the system",
                ReviewerAccountId = dbaccount.Id,
                ReviewerComment = "Auto-generated by the system",
                ReviewerDate = now
            };
        }

        GameItemComponentProto EnsureComponent(string type, string name, string description, string definition)
        {
            GameItemComponentProto? existing = context.GameItemComponentProtos
                .FirstOrDefault(x => x.Type == type && x.Name == name);
            if (existing != null)
            {
                return existing;
            }

            GameItemComponentProto component = new()
            {
                Id = NextComponentProtoId(),
                RevisionNumber = 0,
                EditableItem = NewEditableItem(),
                Type = type,
                Name = name,
                Description = description,
                Definition = definition
            };
            context.GameItemComponentProtos.Add(component);
            context.SaveChanges();
            added++;
            return component;
        }

        RangedWeaponTypes EnsureRangedType(string name, RangedWeaponType type, TraitDefinition skill, int range,
            string accuracy, string damage, string ammo, double staminaToFire, double staminaPerLoad, double coverBonus,
            Difficulty aimDifficulty, double loadDelay, double readyDelay, double fireDelay, bool alwaysTwoHanded)
        {
            RangedWeaponTypes? existing = context.RangedWeaponTypes.FirstOrDefault(x => x.Name == name);
            if (existing != null)
            {
                return existing;
            }

            RangedWeaponTypes ranged = new()
            {
                Name = name,
                Classification = (int)WeaponClassification.Lethal,
                FireTrait = skill,
                OperateTrait = skill,
                FireableInMelee = false,
                DefaultRangeInRooms = range,
                AccuracyBonusExpression = accuracy,
                DamageBonusExpression = damage,
                AmmunitionLoadType = (int)AmmunitionLoadType.Direct,
                SpecificAmmunitionGrade = ammo,
                AmmunitionCapacity = 1,
                RangedWeaponType = (int)type,
                StaminaToFire = staminaToFire,
                StaminaPerLoadStage = staminaPerLoad,
                CoverBonus = coverBonus,
                BaseAimDifficulty = (int)aimDifficulty,
                LoadDelay = loadDelay,
                ReadyDelay = readyDelay,
                FireDelay = fireDelay,
                AimBonusLostPerShot = 1.0,
                RequiresFreeHandToReady = false,
                AlwaysRequiresTwoHandsToWield = alwaysTwoHanded
            };
            context.RangedWeaponTypes.Add(ranged);
            context.SaveChanges();
            added++;
            return ranged;
        }

        AmmunitionTypes EnsureAmmoType(string name, string specific, RangedWeaponType type, double accuracy,
            AudioVolume loudness, double breakOnHit, double breakOnMiss, Difficulty block, Difficulty dodge,
            DamageType damageType, string damageExpression)
        {
            AmmunitionTypes? existing = context.AmmunitionTypes.FirstOrDefault(x => x.Name == name);
            if (existing != null)
            {
                EnsureComponent("Ammunition", $"Ammo_{name.CollapseString()}",
                    $"Turns an item into {name.A_An()}",
                    $"<Definition><AmmoType>{existing.Id}</AmmoType></Definition>");
                return existing;
            }

            AmmunitionTypes ammo = new()
            {
                Name = name,
                SpecificType = specific,
                DamageType = (int)damageType,
                RangedWeaponTypes = ((int)type).ToString(),
                BaseAccuracy = accuracy,
                BreakChanceOnHit = breakOnHit,
                BreakChanceOnMiss = breakOnMiss,
                Loudness = (int)loudness,
                BaseBlockDifficulty = (int)block,
                BaseDodgeDifficulty = (int)dodge,
                DamageExpression = damageExpression,
                StunExpression = damageExpression,
                PainExpression = damageExpression
            };
            context.AmmunitionTypes.Add(ammo);
            context.SaveChanges();
            added++;

            EnsureComponent("Ammunition", $"Ammo_{name.CollapseString()}",
                $"Turns an item into {name.A_An()}",
                $"<Definition><AmmoType>{ammo.Id}</AmmoType></Definition>");
            return ammo;
        }

        void EnsureVariableCheck(CheckType type)
        {
            string teName = $"{type.DescribeEnum(true)} Formula";
            TraitExpression? expression = context.TraitExpressions.FirstOrDefault(x => x.Name == teName);
            if (expression == null)
            {
                expression = new TraitExpression
                {
                    Name = teName,
                    Expression = "variable"
                };
                context.TraitExpressions.Add(expression);
                context.SaveChanges();
                added++;
            }

            int intType = (int)type;
            Check? existing = context.Checks.FirstOrDefault(x => x.Type == intType);
            if (existing != null)
            {
                return;
            }

            CheckTemplate template = context.CheckTemplates.First(x => x.Name == "Skill Check");
            context.Checks.Add(new Check
            {
                Type = intType,
                CheckTemplateId = template.Id,
                MaximumDifficultyForImprovement = (int)Difficulty.Impossible,
                TraitExpression = expression
            });
            context.SaveChanges();
            added++;
        }

        TraitDefinition sling = EnsureSkill(
            ["Slinging", "Sling"],
            "Sling",
            $"min(99,2*{strength.Alias}:{strength.Id}+2*{agi.Alias}:{agi.Id}+1*{per.Alias}:{per.Id})");
        TraitDefinition blowgun = EnsureSkill(
            ["Blowgunning", "Blowgun"],
            "Blowgun",
            $"min(99,2*{dex.Alias}:{dex.Id}+2*{per.Alias}:{per.Id}+1*{agi.Alias}:{agi.Id})");

        EnsureVariableCheck(CheckType.FireSling);
        EnsureVariableCheck(CheckType.FireBlowgun);

        RangedWeaponTypes slingType = EnsureRangedType(
            "Sling",
            RangedWeaponType.Sling,
            sling,
            2,
            "(-3.0*range)-(pow(1-aim,2)*3.0)",
            $"quality - (3.0*range) + max(0,({strength.Alias}:{strength.Id}-10)*1.5)",
            "Sling Bullet",
            6.0,
            2.0,
            -3.0,
            Difficulty.Hard,
            0.25,
            0.1,
            0.2,
            false);
        EnsureComponent("Sling", "Sling", "Turns an item into a sling",
            $"<Definition><RangedWeaponType>{slingType.Id}</RangedWeaponType><StaminaPerTick>3</StaminaPerTick></Definition>");

        RangedWeaponTypes staffSlingType = EnsureRangedType(
            "Staff Sling",
            RangedWeaponType.Sling,
            sling,
            2,
            "(-3.5*range)-(pow(1-aim,2)*3.0)",
            $"2*quality - (4.0*range) + max(0,({strength.Alias}:{strength.Id}-10)*2.0)",
            "Sling Bullet",
            10.0,
            3.0,
            -3.0,
            Difficulty.Hard,
            0.35,
            0.1,
            0.3,
            true);
        EnsureComponent("Sling", "Staff Sling", "Turns an item into a staff sling",
            $"<Definition><RangedWeaponType>{staffSlingType.Id}</RangedWeaponType><StaminaPerTick>4</StaminaPerTick></Definition>");

        RangedWeaponTypes blowgunType = EnsureRangedType(
            "Blowgun",
            RangedWeaponType.Blowgun,
            blowgun,
            1,
            "(-5.0*range)-(pow(1-aim,2)*4.0)",
            "quality - (8.0*range)",
            "Blowgun Dart",
            1.0,
            1.0,
            -4.0,
            Difficulty.Normal,
            0.35,
            0.1,
            0.2,
            false);
        EnsureComponent("Blowgun", "Blowgun", "Turns an item into a blowgun",
            $"<Definition><RangedWeaponType>{blowgunType.Id}</RangedWeaponType></Definition>");

        EnsureAmmoType("Sling Bullet", "Sling Bullet", RangedWeaponType.Sling, 0.0, AudioVolume.Quiet, 0.2, 0.2,
            Difficulty.Easy, Difficulty.Hard, DamageType.Crushing, "quality * 0.75 * degree");
        EnsureAmmoType("Lead Sling Bullet", "Sling Bullet", RangedWeaponType.Sling, -0.5, AudioVolume.Quiet, 0.1, 0.2,
            Difficulty.Easy, Difficulty.Hard, DamageType.Crushing, "5 + quality * degree");
        EnsureAmmoType("Blowgun Dart", "Blowgun Dart", RangedWeaponType.Blowgun, 1.0, AudioVolume.Silent, 0.2, 0.3,
            Difficulty.VeryEasy, Difficulty.Hard, DamageType.Piercing, "5 + quality * 0.5 * degree + (pointblank * 5)");
        EnsureAmmoType("Barbed Blowgun Dart", "Blowgun Dart", RangedWeaponType.Blowgun, 0.0, AudioVolume.Silent, 0.1,
            0.3, Difficulty.VeryEasy, Difficulty.Hard, DamageType.Piercing,
            "8 + quality * 0.5 * degree + (pointblank * 5)");

        return added;
    }

    private static string ResolveDamageRandomness(IReadOnlyDictionary<string, string> questionAnswers)
    {
        return CombatBalanceProfileHelper.Parse(questionAnswers.GetValueOrDefault(CombatBalanceProfileHelper.QuestionId)) ==
               CombatBalanceProfile.CombatRebalance
            ? "rebalance"
            : questionAnswers["random"].ToLowerInvariant();
    }

    private static IReadOnlyDictionary<string, string> BuildWeaponDamageExpressions(long strengthId,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        if (CombatBalanceProfileHelper.Parse(questionAnswers.GetValueOrDefault(CombatBalanceProfileHelper.QuestionId)) ==
            CombatBalanceProfile.CombatRebalance)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Weapon Damage - Training"] = "max(1,8-quality)",
                ["Weapon Damage - Terrible"] = $"max(1,0.08 * (str:{strengthId} * quality) * sqrt(degree+1))",
                ["Weapon Damage - Bad"] = $"max(1,0.12 * (str:{strengthId} * quality) * sqrt(degree+1))",
                ["Weapon Damage - Poor"] = $"max(1,0.17 * (str:{strengthId} * quality) * sqrt(degree+1))",
                ["Weapon Damage - Normal"] = $"max(1,0.22 * (str:{strengthId} * quality) * sqrt(degree+1))",
                ["Weapon Damage - Good"] = $"max(1,0.28 * (str:{strengthId} * quality) * sqrt(degree+1))",
                ["Weapon Damage - Very Good"] = $"max(1,0.32 * (str:{strengthId} * quality) * sqrt(degree+1))",
                ["Weapon Damage - Great"] = $"max(1,0.36 * (str:{strengthId} * quality) * sqrt(degree+1))",
                ["Weapon Damage - Coup de Grace"] = $"max(1,0.60 * str:{strengthId} * quality * sqrt(degree+1))"
            };
        }

        string randomPortion = "";
        double startingMultiplier = 1.0;
        switch (ResolveDamageRandomness(questionAnswers))
        {
            case "static":
                startingMultiplier = 0.6;
                break;
            case "partial":
                randomPortion = " * rand(0.7,1.0)";
                startingMultiplier = 0.705882;
                break;
            case "random":
                randomPortion = " * rand(0.2,1.0)";
                break;
        }

        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Weapon Damage - Training"] = $"max(1,10-quality){randomPortion}",
            ["Weapon Damage - Terrible"] =
                $"max(1,{0.1 * startingMultiplier} * (str:{strengthId} * quality) * sqrt(degree+1){randomPortion})",
            ["Weapon Damage - Bad"] =
                $"max(1,{0.2 * startingMultiplier} * (str:{strengthId} * quality) * sqrt(degree+1){randomPortion})",
            ["Weapon Damage - Poor"] =
                $"max(1,{0.25 * startingMultiplier} * (str:{strengthId} * quality) * sqrt(degree+1){randomPortion})",
            ["Weapon Damage - Normal"] =
                $"max(1,{0.3 * startingMultiplier} * (str:{strengthId} * quality) * sqrt(degree+1){randomPortion})",
            ["Weapon Damage - Good"] =
                $"max(1,{0.4 * startingMultiplier} * (str:{strengthId} * quality) * sqrt(degree+1){randomPortion})",
            ["Weapon Damage - Very Good"] =
                $"max(1,{0.45 * startingMultiplier} * (str:{strengthId} * quality) * sqrt(degree+1){randomPortion})",
            ["Weapon Damage - Great"] =
                $"max(1,{0.5 * startingMultiplier} * (str:{strengthId} * quality) * sqrt(degree+1){randomPortion})",
            ["Weapon Damage - Coup de Grace"] =
                $"max(1,{1.0 * startingMultiplier} * str:{strengthId} * quality * sqrt(degree+1){randomPortion})"
        };
    }

    private static IReadOnlyDictionary<string, string> BuildUnarmedDamageExpressions(long strengthId,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        if (CombatBalanceProfileHelper.Parse(questionAnswers.GetValueOrDefault(CombatBalanceProfileHelper.QuestionId)) ==
            CombatBalanceProfile.CombatRebalance)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Unarmed Damage - Terrible"] = $"0.15 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1)",
                ["Unarmed Damage - Bad"] = $"0.30 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1)",
                ["Unarmed Damage - Normal"] = $"0.45 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1)",
                ["Unarmed Damage - Good"] = $"0.60 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1)",
                ["Unarmed Damage - Great"] = $"0.75 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1)"
            };
        }

        string randomPortion = "";
        switch (ResolveDamageRandomness(questionAnswers))
        {
            case "partial":
                randomPortion = " * rand(0.7,1.0)";
                break;
            case "random":
                randomPortion = " * rand(0.2,1.0)";
                break;
        }

        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Unarmed Damage - Terrible"] = $"0.33333 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1){randomPortion}",
            ["Unarmed Damage - Bad"] = $"0.66666 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1){randomPortion}",
            ["Unarmed Damage - Normal"] = $"1.0 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1){randomPortion}",
            ["Unarmed Damage - Good"] = $"1.25 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1){randomPortion}",
            ["Unarmed Damage - Great"] = $"1.5 * (str:{strengthId} + (2 * quality)) * sqrt(degree+1){randomPortion}"
        };
    }

    private static TraitExpression UpsertTraitExpression(FuturemudDatabaseContext context, string name, string expression)
    {
        TraitExpression? existing = context.TraitExpressions.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            existing.Expression = expression;
            return existing;
        }

        TraitExpression created = new()
        {
            Name = name,
            Expression = expression
        };
        context.TraitExpressions.Add(created);
        context.SaveChanges();
        return created;
    }

    private string ArmourDissipateModifier(DamageType type, double power, IEnumerable<DamageType> strongTypes,
        IEnumerable<DamageType> weakTypes, IEnumerable<DamageType> zeroTypes, IEnumerable<DamageType> superTypes)
    {
        if (zeroTypes.Contains(type))
        {
            return "";
        }

        switch (type)
        {
            case DamageType.Slashing:
            case DamageType.Chopping:
            case DamageType.Piercing:
            case DamageType.Ballistic:
            case DamageType.Bite:
            case DamageType.Claw:
            case DamageType.Shearing:
            case DamageType.Shrapnel:
            case DamageType.Crushing:
            case DamageType.Shockwave:
            case DamageType.Wrenching:
            case DamageType.Burning:
            case DamageType.Freezing:
            case DamageType.Chemical:
            case DamageType.Electrical:
            case DamageType.Sonic:
            case DamageType.Necrotic:
            case DamageType.Eldritch:
            case DamageType.Falling:
            case DamageType.Arcane:
            case DamageType.BallisticArmourPiercing:
            case DamageType.ArmourPiercing:
                if (superTypes.Contains(type))
                {
                    return $"-(quality*{power * 2.0})";
                }

                if (strongTypes.Contains(type))
                {
                    return $"-(quality*{power * 0.65})";
                }

                if (weakTypes.Contains(type))
                {
                    return $"-(quality*{power * 0.1})";
                }

                return $"-(quality*{power * 0.35})";

            case DamageType.Hypoxia:
            case DamageType.Cellular:
                return "";
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private string ArmourAbsorbModifier(DamageType type, double power, IEnumerable<DamageType> strongTypes,
        IEnumerable<DamageType> weakTypes, IEnumerable<DamageType> zeroTypes, IEnumerable<DamageType> superTypes)
    {
        if (zeroTypes.Contains(type))
        {
            return "";
        }

        switch (type)
        {
            case DamageType.Slashing:
            case DamageType.Chopping:
            case DamageType.Piercing:
            case DamageType.Ballistic:
            case DamageType.Bite:
            case DamageType.Claw:
            case DamageType.Shearing:
            case DamageType.Shrapnel:
            case DamageType.Crushing:
            case DamageType.Shockwave:
            case DamageType.Wrenching:
            case DamageType.Burning:
            case DamageType.Freezing:
            case DamageType.Chemical:
            case DamageType.Electrical:
            case DamageType.Sonic:
            case DamageType.Necrotic:
            case DamageType.Eldritch:
            case DamageType.Falling:
            case DamageType.Arcane:
            case DamageType.BallisticArmourPiercing:
            case DamageType.ArmourPiercing:
                if (superTypes.Contains(type))
                {
                    return $"*({1.0 - power * 0.05}-(quality*{power * 0.05}))";
                }

                if (strongTypes.Contains(type))
                {
                    return $"*({1.0 - power * 0.03}-(quality*{power * 0.03}))";
                }

                if (weakTypes.Contains(type))
                {
                    return $"*({1.0 - power * 0.01}-(quality*{power * 0.01}))";
                }

                return $"*({1.0 - power * 0.02}-(quality*{power * 0.02}))";
            case DamageType.Hypoxia:
            case DamageType.Cellular:
                return "";

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (context.WeaponAttacks.Any())
        {
            return ShouldSeedResult.MayAlreadyBeInstalled;
        }

        if (!context.Races.Any(x => x.Name == "Human"))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        return ShouldSeedResult.ReadyToInstall;
    }

    public int SortOrder => 90;
    public string Name => "Combat";
    public string Tagline => "Attacks, Echoes, Weapon Types and all the fun stuff";

    public string FullDescription =>
        @"This seeder will set up everything you need to get combat going in your MUD, including things like weapon types and attacks. There is a lot of customisation you can do with this system and it's easily possible to further extend it once you run this importer." + " " + @"

However, I will say that there are actually some very big decisions up front in setting it up, particularly when it comes to weapon attacks." + " " + @"

I'd recommend that if you're uncertain about what options you might select or you're unfamiliar with how the engine does things that you might want to play around with a local copy of your database with a few iterations of this seeder until you find something you're comfortable with.";
}
