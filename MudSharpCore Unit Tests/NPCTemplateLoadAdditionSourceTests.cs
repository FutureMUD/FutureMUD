#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NPCTemplateLoadAdditionSourceTests
{
    [TestMethod]
    public void NpcTemplateLoadAdditions_AreSharedDefinitionXmlForSimpleAndVariableTemplates()
    {
        string baseSource = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates",
            "NPCTemplateBase.LoadAdditions.cs"));
        string simpleSource = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates",
            "SimpleNPCTemplate.cs"));
        string variableSource = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates",
            "VariableNPCTemplate.cs"));
        string interfaceSource = File.ReadAllText(GetSourcePath("FutureMUDLibrary", "NPC", "Templates",
            "INPCTemplate.cs"));

        StringAssert.Contains(interfaceSource, "ApplyTemplateLoadAdditions");
        StringAssert.Contains(baseSource, "\"TemplateLoadAdditions\"");
        StringAssert.Contains(baseSource, "\"ClanMemberships\"");
        StringAssert.Contains(baseSource, "\"Outfits\"");
        StringAssert.Contains(baseSource, "\"Hooks\"");
        StringAssert.Contains(baseSource, "\"BankAccounts\"");
        StringAssert.Contains(baseSource, "\"BodyLoadout\"");
        StringAssert.Contains(simpleSource, "SaveTemplateLoadAdditions()");
        StringAssert.Contains(variableSource, "SaveTemplateLoadAdditions()");
    }

    [TestMethod]
    public void NpcTemplateLoadAdditions_ExposeCommonBuilderCommands()
    {
        string baseSource = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates",
            "NPCTemplateBase.cs"));
        string builderSource = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates",
            "NPCTemplateBase.LoadAdditionBuilding.cs"));

        StringAssert.Contains(baseSource, "case \"clan\":");
        StringAssert.Contains(baseSource, "case \"outfit\":");
        StringAssert.Contains(baseSource, "case \"hook\":");
        StringAssert.Contains(baseSource, "case \"bank\":");
        StringAssert.Contains(baseSource, "case \"implant\":");
        StringAssert.Contains(baseSource, "case \"prosthetic\":");
        StringAssert.Contains(builderSource, "BuildingCommandClan");
        StringAssert.Contains(builderSource, "BuildingCommandOutfit");
        StringAssert.Contains(builderSource, "BuildingCommandHook");
        StringAssert.Contains(builderSource, "BuildingCommandBank");
        StringAssert.Contains(builderSource, "BuildingCommandImplant");
        StringAssert.Contains(builderSource, "BuildingCommandProsthetic");
    }

    [TestMethod]
    public void NpcTemplateLoadAdditions_RunBeforeOnLoadProgInNpcCreationPaths()
    {
        AssertApplyBeforeOnLoad("MudSharpCore", "Commands", "Modules", "NPCBuilderModule.cs");
        AssertApplyBeforeOnLoad("MudSharpCore", "FutureProg", "Functions", "BuiltIn", "LoadNpcFunction.cs");
        AssertApplyBeforeOnLoad("MudSharpCore", "NPC", "NPCSpawner.cs");
        AssertApplyBeforeOnLoad("MudSharpCore", "Magic", "SpellEffects", "CreateNPCEffect.cs");
        AssertApplyBeforeOnLoad("MudSharpCore", "Magic", "SpellEffects", "MagicPhase3Effects.cs");
        AssertApplyBeforeOnLoad("MudSharpCore", "Work", "Agriculture", "AgricultureField.cs");
        AssertApplyBeforeOnLoad("MudSharpCore", "Work", "Crafts", "Products", "NPCProduct.cs");
    }

    [TestMethod]
    public void NpcTemplateLoadAdditions_KeepPostIdAndDriftValidationRules()
    {
        string source = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "Templates",
            "NPCTemplateBase.LoadAdditions.cs"));

        StringAssert.Contains(source, "Gameworld.SaveManager.DirectInitialise(lateItem)");
        StringAssert.Contains(source, "type.CanOpenAccount(character)");
        StringAssert.Contains(source, "account.DoAccountCredit");
        StringAssert.Contains(source, "appointment.IsAppointedByElection");
        StringAssert.Contains(source, "!clan.FreePosition(appointment)");
        StringAssert.Contains(source, "HookIsValidForCharacter");
        StringAssert.Contains(source, "character.Body.InstallImplant(implant)");
        StringAssert.Contains(source, "character.Body.InstallProsthetic(prosthetic)");
    }

    private static void AssertApplyBeforeOnLoad(params string[] parts)
    {
        string source = File.ReadAllText(GetSourcePath(parts));
        int applyIndex = source.IndexOf(".ApplyTemplateLoadAdditions", StringComparison.Ordinal);
        int onLoadIndex = source.IndexOf(".OnLoadProg?.Execute", StringComparison.Ordinal);

        Assert.IsTrue(applyIndex >= 0, $"{parts[^1]} should apply NPC template load additions.");
        Assert.IsTrue(onLoadIndex >= 0, $"{parts[^1]} should still run the template OnLoadProg.");
        Assert.IsTrue(applyIndex < onLoadIndex,
            $"{parts[^1]} should apply NPC template load additions before the template OnLoadProg.");
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
