#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSettingsSourceTests
{
    [TestMethod]
    public void CombatModule_ProgTargetingCommands_UseProgLookupFromBuilderInput()
    {
        string source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "CombatModule.cs"));

        StringAssert.Contains(source,
            "new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,");
        StringAssert.Contains(source, "new[] { ProgVariableTypes.Character }).LookupProg();");
        StringAssert.Contains(source,
            "new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Number,");
        StringAssert.Contains(source, "new[] { ProgVariableTypes.Toon }).LookupProg();");
    }

    [TestMethod]
    public void CharacterCombatSettings_PriorityFor_NullPriorityProgDefaultsToZero()
    {
        string source = File.ReadAllText(GetSourcePath("MudSharpCore", "Combat", "CharacterCombatSettings.cs"));

        StringAssert.Contains(source, "return PriorityProg?.ExecuteDouble(0.0, who) ?? 0.0;");
    }

    [TestMethod]
    public void RaceAndNpcBuilders_CombatSettingOverrides_RequireGlobalTemplates()
    {
        string raceSource = File.ReadAllText(GetSourcePath("MudSharpCore", "Character", "Heritage", "RaceBuilding.cs"));
        string npcSource = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates", "NPCTemplateBase.cs"));

        StringAssert.Contains(raceSource, "if (!setting.GlobalTemplate)");
        StringAssert.Contains(raceSource, "Races can only use global combat settings as defaults.");
        StringAssert.Contains(npcSource, "if (!setting.GlobalTemplate)");
        StringAssert.Contains(npcSource, "NPC Templates can only use global combat settings as defaults.");
    }

    [TestMethod]
    public void WeaponAttack_UsableAttack_AllowsSwordAndBoardWhenOneHandedWeaponHasSeparateShield()
    {
        string source = File.ReadAllText(GetSourcePath("MudSharpCore", "Combat", "WeaponAttack.cs"));

        StringAssert.Contains(source, "private bool HandednessMatches");
        StringAssert.Contains(source, "HandednessOptions == handedness");
        StringAssert.Contains(source, "HandednessOptions == AttackHandednessOptions.SwordAndBoardOnly");
        StringAssert.Contains(source, "handedness == AttackHandednessOptions.OneHandedOnly");
        StringAssert.Contains(source, "character.Body.WieldedItems.Any(x => x != weapon && x.IsItemType<IShield>())");
        StringAssert.Contains(source, "HandednessMatches(attacker, weapon, handedness)");
    }

    private static string GetSourcePath(params string[] parts)
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            Path.Combine(parts)));
    }
}
