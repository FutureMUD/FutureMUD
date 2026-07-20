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
    private IReadOnlyDictionary<string, TraitDefinition> SeedCoreData(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        IReadOnlyDictionary<string, TraitDefinition> skills = SeedUniversalSkills(context, questionAnswers);
        if (!context.CombatMessages.Any(x => x.Type == (int)BuiltInCombatMoveType.Dodge))
        {
            SeedUnarmedCombatMessage(context, questionAnswers);
        }

        // Seed Combat Strategies
        SeedCombatStrategies(context, questionAnswers);
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Brawler");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Clincher");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Behemoth");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Skirmisher");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Swooper");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Drowner");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Dropper");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Physical Avoider");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Artillery");
		CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Aquatic Brawler");
		CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Aquatic Clincher");
		CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Aquatic Behemoth");
		CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Aquatic Skirmisher");
		CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Aquatic Artillery");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Coward");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Construct Brawler");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Construct Skirmisher");
        CombatStrategySeederHelper.EnsureCombatStrategy(context, "Construct Artillery");

        // Set up shield types
        if (!context.ShieldTypes.Any())
        {
            SeedShields(context, questionAnswers, skills);
        }

        SeedChecks(context, questionAnswers, skills);

        return skills;
    }

    private IReadOnlyDictionary<string, TraitDefinition> SeedUniversalSkills(FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        Dictionary<string, TraitDefinition> skills = new(StringComparer.OrdinalIgnoreCase);
        foreach (TraitDefinition? skill in context.TraitDefinitions.Where(x => x.Type == 0))
        {
            skills[skill.Name] = skill;
        }

        bool gerund =
            context.TraitDefinitions.FirstOrDefault(x => x.Name == "Track" || x.Name == "Tracking")?.Name ==
            "Tracking";
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
        TraitDefinition? armourUseSkill = context.TraitDefinitions.FirstOrDefault(x => x.Name == "Armour Use");
        bool useStats = armourUseSkill?.Expression.Expression != "70";

        TraitDefinition EnsureSkill(string name, string decorator = "General Skill")
        {
            if (skills.TryGetValue(name, out TraitDefinition? existing))
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
                DecoratorId = context.TraitDecorators.First(x => x.Name == decorator).Id,
                DerivedType = 0,
                ChargenBlurb = string.Empty,
                BranchMultiplier = 1.0
            };
            context.TraitDefinitions.Add(skill);
            context.SaveChanges();
            skills[name] = skill;
            return skill;
        }

        EnsureSkill(gerund ? "Blocking" : "Block");
        EnsureSkill(gerund ? "Dodging" : "Dodge");
        EnsureSkill(gerund ? "Brawling" : "Brawling");
        EnsureSkill(gerund ? "Wrestling" : "Subdue");
        EnsureSkill(gerund ? "Warding" : "Ward");
        EnsureSkill(gerund ? "Throwing" : "Throwing");
        EnsureSkill(gerund ? "Veterancy" : "Veterancy", "Veterancy Skill");
        if (questionAnswers["parryoption"].ToLowerInvariant().EqualToAny("yes", "y"))
        {
            EnsureSkill(gerund ? "Parrying" : "Parry");
        }

        if (!gerund)
        {
            foreach (var skillName in RpiLegacyCombatSkillNames)
            {
                EnsureSkill(skillName);
            }
        }

        return skills;
    }


    private void SeedChecks(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers,
        IReadOnlyDictionary<string, TraitDefinition> skills)
    {
        void AddCheck(CheckType type, TraitExpression expression, long templateId,
            Difficulty maximumImprovementDifficulty)
        {
            string teName = $"{type.DescribeEnum(true)} Formula";
            TraitExpression? existingExpression = context.TraitExpressions.FirstOrDefault(x => x.Name == teName);
            if (existingExpression is not null)
            {
                existingExpression.Expression = expression.Expression;
                expression = existingExpression;
            }
            else
            {
                context.TraitExpressions.Add(expression);
            }

            int intType = (int)type;
            Check? existingCheck = context.Checks.FirstOrDefault(x => x.Type == intType);
            if (existingCheck is null)
            {
                context.Checks.Add(new Check
                {
                    Type = (int)type,
                    CheckTemplateId = templateId,
                    MaximumDifficultyForImprovement = (int)maximumImprovementDifficulty,
                    TraitExpression = expression
                });
            }
            else
            {
                existingCheck.TraitExpression = expression;
                existingCheck.CheckTemplateId = templateId;
                existingCheck.MaximumDifficultyForImprovement = (int)maximumImprovementDifficulty;
            }

            context.SaveChanges();
        }

        Dictionary<string, TraitDefinition> attributes = context.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3)
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        TraitDefinition wilAttribute =
            attributes.GetValueOrDefault("Willpower") ??
            attributes.GetValueOrDefault("Wisdom") ??
            attributes["Spirit"];
        CheckTemplate template = context.CheckTemplates.First(x => x.Name == "Skill Check");
        string parryoption = questionAnswers["parryoption"].ToLowerInvariant();

        foreach (CheckType check in Enum.GetValues(typeof(CheckType)).OfType<CheckType>().Distinct().ToList())
        {
            switch (check)
            {
                case CheckType.NaturalWeaponAttack:
                case CheckType.RangedNaturalAttack:
                case CheckType.BreathWeaponAttack:
                case CheckType.SpitNaturalAttack:
                case CheckType.ExplosiveNaturalAttack:
                case CheckType.BuffetingNaturalAttack:
                    AddCheck(check,
                        new TraitExpression
                        { Expression = $"brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id}" },
                        template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.BreathWeaponSwoop:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id}) + (0.5 * fly:{(skills.GetValueOrDefault("Flying") ?? skills.GetValueOrDefault("Fly") ?? skills.GetValueOrDefault("Athletics") ?? skills.Values.First()).Id})"
                        },
                        template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.DodgeCheck:
                    AddCheck(check,
                        new TraitExpression
                        { Expression = $"dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}" },
                        template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.ParryCheck:
                    if (parryoption.EqualToAny("yes", "y"))
                    {
                        AddCheck(check,
                            new TraitExpression
                            {
                                Expression = $"parry:{(skills.GetValueOrDefault("Parrying") ?? skills["Parry"]).Id}"
                            }, template.Id, Difficulty.Impossible);
                        continue;
                    }

                    AddCheck(check, new TraitExpression { Expression = "variable" }, template.Id,
                        Difficulty.Impossible);
                    continue;
                case CheckType.BlockCheck:
                    AddCheck(check,
                        new TraitExpression
                        { Expression = $"block:{(skills.GetValueOrDefault("Blocking") ?? skills["Block"]).Id}" },
                        template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.FleeMeleeCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"running:{(skills.GetValueOrDefault("Running") ?? skills.GetValueOrDefault("Run") ?? skills["Athletics"]).Id} + (0.5 * {skills["Veterancy"].Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.OpposeFleeMeleeCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"running:{(skills.GetValueOrDefault("Running") ?? skills.GetValueOrDefault("Run") ?? skills["Athletics"]).Id} + (0.5 * {skills["Veterancy"].Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.FleeMovementUnmountedCheck:
                case CheckType.FleeMovementMountedCheck:
                case CheckType.PursuitMovementUnmountedCheck:
                case CheckType.PursuitMovementMountedCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"running:{(skills.GetValueOrDefault("Running") ?? skills.GetValueOrDefault("Run") ?? skills["Athletics"]).Id} + (0.5 * {skills["Veterancy"].Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.Ward:
                    AddCheck(check,
                        new TraitExpression
                        { Expression = $"ward:{(skills.GetValueOrDefault("Warding") ?? skills["Ward"]).Id}" },
                        template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.WardDefense:
                    AddCheck(check, new TraitExpression { Expression = $"veterancy:{skills["Veterancy"].Id}" },
                        template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.WardIgnore:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * veterancy:{skills["Veterancy"].Id}) + ({wilAttribute.Alias}:{wilAttribute.Id} * 2.5)"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.StartClinch:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.ResistClinch:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.BreakClinch:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.ResistBreakClinch:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.PushbackCheck:
                case CheckType.ForcedMovementCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id}) + (0.5 * wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.OpposePushbackCheck:
                case CheckType.OpposeForcedMovementCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.RescueCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * veterancy:{skills["Veterancy"].Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.OpposeRescueCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * veterancy:{skills["Veterancy"].Id}) + (0.5 * brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.StaggeringBlowDefense:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.25 * veterancy:{skills["Veterancy"].Id}) + (0.75 * endure:{(skills.GetValueOrDefault("Enduring") ?? skills.GetValueOrDefault("Endurance") ?? skills["Athletics"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
				case CheckType.BoatStabilityCheck:
					AddCheck(check,
						new TraitExpression
						{
							Expression =
								$"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * athletics:{skills["Athletics"].Id})"
						}, template.Id, Difficulty.Impossible);
					continue;
                case CheckType.StruggleFreeFromDrag:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.OpposeStruggleFreeFromDrag:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression =
                                $"(0.5 * dodge:{(skills.GetValueOrDefault("Dodging") ?? skills["Dodge"]).Id}) + (0.5 * wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id})"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.CounterGrappleCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.StruggleFreeFromGrapple:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.OpposeStruggleFreeFromGrapple:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.ExtendGrappleCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.InitiateGrapple:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.ScreechAttack:
                    AddCheck(check,
                        new TraitExpression
                        { Expression = $"brawl:{(skills.GetValueOrDefault("Brawling") ?? skills["Brawl"]).Id}" },
                        template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.StrangleCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.WrenchAttackCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.TakedownCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.BreakoutCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.OpposeBreakoutCheck:
                    AddCheck(check,
                        new TraitExpression
                        {
                            Expression = $"wrestle:{(skills.GetValueOrDefault("Wrestling") ?? skills.GetValueOrDefault("Subdue") ?? skills["Wrestle"]).Id}"
                        }, template.Id, Difficulty.Impossible);
                    continue;
                case CheckType.TossItemCheck:
                    AddCheck(check,
                        new TraitExpression
                        { Expression = $"throw:{(skills.GetValueOrDefault("Throwing") ?? skills["throw"]).Id}+30" },
                        template.Id, Difficulty.Impossible);
                    continue;
            }
        }
    }
}
