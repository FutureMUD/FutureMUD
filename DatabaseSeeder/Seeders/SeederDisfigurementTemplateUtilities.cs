#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Form.Colour;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

internal abstract record SeederDisfigurementTemplateDefinition(
    string Name,
    string ShortDescription,
    string FullDescription,
    IReadOnlyList<string>? BodypartShapeNames = null,
    IReadOnlyList<string>? BodypartAliases = null,
    bool CanSelectInChargen = false,
    string? CanSelectInChargenProgName = null,
    IReadOnlyDictionary<string, int>? ChargenCosts = null,
    string? OverrideCharacteristicPlain = null,
    string? OverrideCharacteristicWith = null
);

internal sealed record SeederTattooTextSlotDefinition(
    string Name,
    int MaximumLength,
    string? DefaultLanguageName = null,
    string? DefaultScriptName = null,
    string DefaultText = "",
    bool RequiredCustomText = false,
    WritingStyleDescriptors DefaultStyle = WritingStyleDescriptors.None,
    string? DefaultColourName = null,
    double DefaultMinimumSkill = 0.0,
    string DefaultAlternateText = ""
);

internal sealed record SeederTattooTemplateDefinition(
    string Name,
    string ShortDescription,
    string FullDescription,
    SizeCategory MinimumBodypartSize = SizeCategory.Nanoscopic,
    string? RequiredKnowledgeName = null,
    double MinimumSkill = 0.0,
    IReadOnlyDictionary<string, double>? InkColours = null,
    IReadOnlyList<string>? BodypartShapeNames = null,
    IReadOnlyList<string>? BodypartAliases = null,
    bool CanSelectInChargen = false,
    string? CanSelectInChargenProgName = null,
    IReadOnlyDictionary<string, int>? ChargenCosts = null,
    string? OverrideCharacteristicPlain = null,
    string? OverrideCharacteristicWith = null,
    IReadOnlyList<SeederTattooTextSlotDefinition>? TextSlots = null
) : SeederDisfigurementTemplateDefinition(
    Name,
    ShortDescription,
    FullDescription,
    BodypartShapeNames,
    BodypartAliases,
    CanSelectInChargen,
    CanSelectInChargenProgName,
    ChargenCosts,
    OverrideCharacteristicPlain,
    OverrideCharacteristicWith
);

internal sealed record SeederScarTemplateDefinition(
    string Name,
    string ShortDescription,
    string FullDescription,
    int SizeSteps = 0,
    int Distinctiveness = 1,
    bool Unique = false,
    double DamageHealingScarWeight = 0.0,
    double SurgeryHealingScarWeight = 0.0,
    IReadOnlyDictionary<DamageType, WoundSeverity>? DamageTypes = null,
    IReadOnlyList<SurgicalProcedureType>? SurgeryTypes = null,
    IReadOnlyList<string>? BodypartShapeNames = null,
    IReadOnlyList<string>? BodypartAliases = null,
    bool CanSelectInChargen = false,
    string? CanSelectInChargenProgName = null,
    IReadOnlyDictionary<string, int>? ChargenCosts = null,
    string? OverrideCharacteristicPlain = null,
    string? OverrideCharacteristicWith = null
) : SeederDisfigurementTemplateDefinition(
    Name,
    ShortDescription,
    FullDescription,
    BodypartShapeNames,
    BodypartAliases,
    CanSelectInChargen,
    CanSelectInChargenProgName,
    ChargenCosts,
    OverrideCharacteristicPlain,
    OverrideCharacteristicWith
);

internal static class SeederDisfigurementTemplateUtilities
{
    private static readonly int CurrentRevisionStatus = (int)RevisionStatus.Current;

    public static void SeedTemplates(
        FuturemudDatabaseContext context,
        BodyProto body,
        IEnumerable<SeederTattooTemplateDefinition>? tattooDefinitions = null,
        IEnumerable<SeederScarTemplateDefinition>? scarDefinitions = null)
    {
        foreach (SeederTattooTemplateDefinition definition in tattooDefinitions ?? Enumerable.Empty<SeederTattooTemplateDefinition>())
        {
            SeedTattooTemplate(context, body, definition);
        }

        foreach (SeederScarTemplateDefinition definition in scarDefinitions ?? Enumerable.Empty<SeederScarTemplateDefinition>())
        {
            SeedScarTemplate(context, body, definition);
        }

        context.SaveChanges();
    }

    public static bool HasMissingDefinitions(
        FuturemudDatabaseContext context,
        IEnumerable<SeederTattooTemplateDefinition>? tattooDefinitions = null,
        IEnumerable<SeederScarTemplateDefinition>? scarDefinitions = null)
    {
        HashSet<(string Type, string Name)> expected = new(StringComparerTupleComparer.Instance);
        foreach (SeederTattooTemplateDefinition definition in tattooDefinitions ?? Enumerable.Empty<SeederTattooTemplateDefinition>())
        {
            expected.Add(("Tattoo", definition.Name));
        }

        foreach (SeederScarTemplateDefinition definition in scarDefinitions ?? Enumerable.Empty<SeederScarTemplateDefinition>())
        {
            expected.Add(("Scar", definition.Name));
        }

        if (!expected.Any())
        {
            return false;
        }

        HashSet<(string Type, string Name)> existing = context.DisfigurementTemplates
            .Include(x => x.EditableItem)
            .Where(x => x.EditableItem != null && x.EditableItem.RevisionStatus == CurrentRevisionStatus)
            .AsEnumerable()
            .Select(x => (x.Type, x.Name))
            .ToHashSet(StringComparerTupleComparer.Instance);
        return expected.Any(x => !existing.Contains(x));
    }

    private static void SeedTattooTemplate(
        FuturemudDatabaseContext context,
        BodyProto body,
        SeederTattooTemplateDefinition definition)
    {
        DisfigurementTemplate template = GetOrCreateTemplate(context, "Tattoo", definition.Name);
        List<long> bodypartShapes = ResolveBodypartShapes(context, body, definition).ToList();
        long canSelectProg = ResolveProgIdOrZero(context, definition.CanSelectInChargenProgName);
        long requiredKnowledge = ResolveKnowledgeIdOrZero(context, definition.RequiredKnowledgeName);
        IEnumerable<(long ColourId, double Weight)> inkColours = ResolveInkColours(context, definition.InkColours);
        template.Name = definition.Name;
        template.Type = "Tattoo";
        template.ShortDescription = definition.ShortDescription;
        template.FullDescription = definition.FullDescription;
        template.Definition = new XElement("Tattoo",
            new XElement("MinBodypartsize", (int)definition.MinimumBodypartSize),
            BuildChargenElements(context, definition, canSelectProg),
            new XElement("RequiredKnowledge", requiredKnowledge),
            new XElement("MinimumSkill", definition.MinimumSkill),
            new XElement("OverrideCharacteristicPlain", definition.OverrideCharacteristicPlain ?? string.Empty),
            new XElement("OverrideCharacteristicWith", definition.OverrideCharacteristicWith ?? string.Empty),
            new XElement("Inks",
                from ink in inkColours
                select new XElement("Ink", new XAttribute("weight", ink.Weight), ink.ColourId)),
            new XElement("Shapes",
                from shapeId in bodypartShapes
                select new XElement("Shape", shapeId)),
            new XElement("TextSlots",
                from slot in definition.TextSlots ?? Enumerable.Empty<SeederTattooTextSlotDefinition>()
                select BuildTattooTextSlotElement(context, slot, definition.Name))
        ).ToString();
    }

    private static void SeedScarTemplate(
        FuturemudDatabaseContext context,
        BodyProto body,
        SeederScarTemplateDefinition definition)
    {
        DisfigurementTemplate template = GetOrCreateTemplate(context, "Scar", definition.Name);
        List<long> bodypartShapes = ResolveBodypartShapes(context, body, definition).ToList();
        long canSelectProg = ResolveProgIdOrZero(context, definition.CanSelectInChargenProgName);
        template.Name = definition.Name;
        template.Type = "Scar";
        template.ShortDescription = definition.ShortDescription;
        template.FullDescription = definition.FullDescription;
        template.Definition = new XElement("Scar",
            BuildChargenElements(context, definition, canSelectProg),
            new XElement("OverrideCharacteristicPlain", definition.OverrideCharacteristicPlain ?? string.Empty),
            new XElement("OverrideCharacteristicWith", definition.OverrideCharacteristicWith ?? string.Empty),
            new XElement("SizeSteps", definition.SizeSteps),
            new XElement("Distinctiveness", definition.Distinctiveness),
            new XElement("Unique", definition.Unique),
            new XElement("DamageHealingScarWeight", definition.DamageHealingScarWeight),
            new XElement("SurgeryHealingScarWeight", definition.SurgeryHealingScarWeight),
            new XElement("Shapes",
                from shapeId in bodypartShapes
                select new XElement("Shape", shapeId)),
            new XElement("Surgeries",
                from surgery in definition.SurgeryTypes ?? Enumerable.Empty<SurgicalProcedureType>()
                select new XElement("Surgery", (int)surgery)),
            new XElement("Damages",
                from damage in definition.DamageTypes ?? Enumerable.Empty<KeyValuePair<DamageType, WoundSeverity>>()
                select new XElement("Damage",
                    new XAttribute("severity", (int)damage.Value),
                    new XAttribute("type", (int)damage.Key)))
        ).ToString();
    }

    private static MudSharp.Models.DisfigurementTemplate GetOrCreateTemplate(
        FuturemudDatabaseContext context,
        string type,
        string name)
    {
        DisfigurementTemplate? template = context.DisfigurementTemplates.Local.FirstOrDefault(x =>
                string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) &&
                x.EditableItem?.RevisionStatus == CurrentRevisionStatus) ??
                       context.DisfigurementTemplates
                           .Include(x => x.EditableItem)
                           .AsEnumerable()
                           .FirstOrDefault(x =>
                               string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase) &&
                               string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) &&
                               x.EditableItem?.RevisionStatus == CurrentRevisionStatus);
        if (template is not null)
        {
            EnsureEditableItemMetadata(context, template.EditableItem);
            return template;
        }

        EditableItem editableItem = CreateEditableItem(context);
        template = new MudSharp.Models.DisfigurementTemplate
        {
            Id = GetNextDisfigurementTemplateId(context),
            RevisionNumber = 0,
            EditableItem = editableItem
        };
        context.DisfigurementTemplates.Add(template);
        return template;
    }

    private static EditableItem CreateEditableItem(FuturemudDatabaseContext context)
    {
        Account account = context.Accounts.First();
        DateTime now = DateTime.UtcNow;
        EditableItem editableItem = new()
        {
            RevisionNumber = 0,
            RevisionStatus = CurrentRevisionStatus,
            BuilderAccountId = account.Id,
            BuilderDate = now,
            BuilderComment = "Auto-generated by the seeder",
            ReviewerAccountId = account.Id,
            ReviewerComment = "Auto-generated by the seeder",
            ReviewerDate = now
        };
        context.EditableItems.Add(editableItem);
        return editableItem;
    }

    private static long GetNextDisfigurementTemplateId(FuturemudDatabaseContext context)
    {
        // Editable-item tables with (Id, RevisionNumber) keys do not always use database-generated IDs.
        // Disfigurement templates are one of those cases, so the seeder must allocate the next stable ID itself.
        return context.DisfigurementTemplates.Local
            .Select(x => x.Id)
            .Concat(context.DisfigurementTemplates
                .AsNoTracking()
                .Select(x => x.Id)
                .AsEnumerable())
            .DefaultIfEmpty(0L)
            .Max() + 1;
    }

    private static void EnsureEditableItemMetadata(FuturemudDatabaseContext context, EditableItem editableItem)
    {
        Account account = context.Accounts.First();
        DateTime now = DateTime.UtcNow;
        editableItem.RevisionNumber = 0;
        editableItem.RevisionStatus = CurrentRevisionStatus;
        editableItem.BuilderAccountId = account.Id;
        editableItem.BuilderDate = editableItem.BuilderDate == default ? now : editableItem.BuilderDate;
        editableItem.BuilderComment ??= "Auto-generated by the seeder";
        editableItem.ReviewerAccountId ??= account.Id;
        editableItem.ReviewerComment ??= "Auto-generated by the seeder";
        editableItem.ReviewerDate ??= now;
    }

    private static IEnumerable<long> ResolveBodypartShapes(
        FuturemudDatabaseContext context,
        BodyProto body,
        SeederDisfigurementTemplateDefinition definition)
    {
        HashSet<long> shapeIds = new();
        foreach (string shapeName in definition.BodypartShapeNames ?? Enumerable.Empty<string>())
        {
            BodypartShape? shape = context.BodypartShapes
                .AsEnumerable()
                .FirstOrDefault(x => string.Equals(x.Name, shapeName, StringComparison.OrdinalIgnoreCase));
            if (shape is null)
            {
                throw new InvalidOperationException(
                    $"Could not resolve bodypart shape {shapeName} for disfigurement template {definition.Name}.");
            }

            shapeIds.Add(shape.Id);
        }

        foreach (string alias in definition.BodypartAliases ?? Enumerable.Empty<string>())
        {
            BodypartProto? bodypart = SeederBodyUtilities.FindBodypartOnBodyOrAncestors(context, body, alias);
            if (bodypart is null)
            {
                throw new InvalidOperationException(
                    $"Could not resolve bodypart alias {alias} on body {body.Name} for disfigurement template {definition.Name}.");
            }

            shapeIds.Add(bodypart.BodypartShapeId);
        }

        return shapeIds.OrderBy(x => x).ToList();
    }

    private static XElement[] BuildChargenElements(
        FuturemudDatabaseContext context,
        SeederDisfigurementTemplateDefinition definition,
        long canSelectInChargenProgId)
    {
        return
        [
            new XElement("CanBeSelectedInChargen", definition.CanSelectInChargen),
            new XElement("CanBeSelectedInChargenProg", canSelectInChargenProgId),
            new XElement("ChargenCosts",
                from cost in ResolveChargenCosts(context, definition.ChargenCosts)
                select new XElement("Cost",
                    new XAttribute("resource", cost.ResourceId),
                    new XAttribute("amount", cost.Amount)))
        ];
    }

    private static IEnumerable<(long ResourceId, int Amount)> ResolveChargenCosts(
        FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, int>? chargenCosts)
    {
        foreach ((string? resourceName, int amount) in chargenCosts ?? new Dictionary<string, int>())
        {
            ChargenResource? resource = context.ChargenResources
                .AsEnumerable()
                .FirstOrDefault(x =>
                    string.Equals(x.Name, resourceName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Alias, resourceName, StringComparison.OrdinalIgnoreCase));
            if (resource is null)
            {
                throw new InvalidOperationException($"Could not resolve chargen resource {resourceName}.");
            }

            yield return (resource.Id, amount);
        }
    }

    private static long ResolveProgIdOrZero(FuturemudDatabaseContext context, string? progName)
    {
        if (string.IsNullOrWhiteSpace(progName))
        {
            return 0L;
        }

        FutureProg? prog = context.FutureProgs
            .AsEnumerable()
            .FirstOrDefault(x => string.Equals(x.FunctionName, progName, StringComparison.OrdinalIgnoreCase));
        if (prog is null)
        {
            throw new InvalidOperationException($"Could not resolve future prog {progName}.");
        }

        return prog.Id;
    }

    private static long ResolveKnowledgeIdOrZero(FuturemudDatabaseContext context, string? knowledgeName)
    {
        if (string.IsNullOrWhiteSpace(knowledgeName))
        {
            return 0L;
        }

        Knowledge? knowledge = context.Knowledges
            .AsEnumerable()
            .FirstOrDefault(x => string.Equals(x.Name, knowledgeName, StringComparison.OrdinalIgnoreCase));
        if (knowledge is null)
        {
            throw new InvalidOperationException($"Could not resolve knowledge {knowledgeName}.");
        }

        return knowledge.Id;
    }

    private static IEnumerable<(long ColourId, double Weight)> ResolveInkColours(
        FuturemudDatabaseContext context,
        IReadOnlyDictionary<string, double>? inkColours)
    {
        foreach ((string? colourName, double weight) in inkColours ?? new Dictionary<string, double>())
        {
            Colour? colour = context.Colours
                .AsEnumerable()
                .FirstOrDefault(x => string.Equals(x.Name, colourName, StringComparison.OrdinalIgnoreCase));
            if (colour is null)
            {
                throw new InvalidOperationException($"Could not resolve ink colour {colourName}.");
            }

            yield return (colour.Id, weight);
        }
    }

    private static XElement BuildTattooTextSlotElement(
        FuturemudDatabaseContext context,
        SeederTattooTextSlotDefinition definition,
        string tattooName)
    {
        if (definition.MaximumLength <= 0)
        {
            throw new InvalidOperationException(
                $"Tattoo text slot {definition.Name} for {tattooName} must have a positive maximum length.");
        }

        return new XElement("TextSlot",
            new XAttribute("name", definition.Name),
            new XAttribute("maxlength", definition.MaximumLength),
            new XAttribute("required", definition.RequiredCustomText),
            new XElement("Language", ResolveLanguageIdOrZero(context, definition.DefaultLanguageName)),
            new XElement("Script", ResolveScriptIdOrZero(context, definition.DefaultScriptName)),
            new XElement("Style", (int)definition.DefaultStyle),
            new XElement("Colour", ResolveColourIdOrZero(context, definition.DefaultColourName)),
            new XElement("MinimumSkill", definition.DefaultMinimumSkill),
            new XElement("Text", new XCData(definition.DefaultText ?? string.Empty)),
            new XElement("AlternateText", new XCData(definition.DefaultAlternateText ?? string.Empty))
        );
    }

    private static long ResolveLanguageIdOrZero(FuturemudDatabaseContext context, string? languageName)
    {
        if (string.IsNullOrWhiteSpace(languageName))
        {
            return 0L;
        }

        Language? language = context.Languages
            .AsEnumerable()
            .FirstOrDefault(x => string.Equals(x.Name, languageName, StringComparison.OrdinalIgnoreCase));
        if (language is null)
        {
            throw new InvalidOperationException($"Could not resolve language {languageName}.");
        }

        return language.Id;
    }

    private static long ResolveScriptIdOrZero(FuturemudDatabaseContext context, string? scriptName)
    {
        if (string.IsNullOrWhiteSpace(scriptName))
        {
            return 0L;
        }

        Script? script = context.Scripts
            .AsEnumerable()
            .FirstOrDefault(x => string.Equals(x.Name, scriptName, StringComparison.OrdinalIgnoreCase));
        if (script is null)
        {
            throw new InvalidOperationException($"Could not resolve script {scriptName}.");
        }

        return script.Id;
    }

    private static long ResolveColourIdOrZero(FuturemudDatabaseContext context, string? colourName)
    {
        if (string.IsNullOrWhiteSpace(colourName))
        {
            return 0L;
        }

        Colour? colour = context.Colours
            .AsEnumerable()
            .FirstOrDefault(x => string.Equals(x.Name, colourName, StringComparison.OrdinalIgnoreCase));
        if (colour is null)
        {
            throw new InvalidOperationException($"Could not resolve colour {colourName}.");
        }

        return colour.Id;
    }

    private sealed class StringComparerTupleComparer : IEqualityComparer<(string Type, string Name)>
    {
        public static StringComparerTupleComparer Instance { get; } = new();

        public bool Equals((string Type, string Name) x, (string Type, string Name) y)
        {
            return string.Equals(x.Type, y.Type, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode((string Type, string Name) obj)
        {
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Type),
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name));
        }
    }
}
