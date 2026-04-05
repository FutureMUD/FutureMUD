#nullable enable

using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class RobotSeeder
{
    private int AddProcedureIfMissing(
        string name,
        string procedureName,
        string school,
        SurgicalProcedureType type,
        CheckType check,
        string gerund,
        string emote,
        string description,
        BodyProto targetBody,
        string knowledgeName,
        string definition,
        Action<MudSharp.Models.SurgicalProcedure> configurePhases)
    {
        SurgicalProcedure? existing = _context.SurgicalProcedures.FirstOrDefault(x => x.Name == name);
        if (existing is not null)
        {
            _procedures[name] = existing;
            return 0;
        }

        SurgicalProcedure procedure = new()
        {
            Name = name,
            ProcedureName = procedureName,
            Procedure = (int)type,
            MedicalSchool = school,
            BaseCheckBonus = 0.0,
            KnowledgeRequired = _context.Knowledges.First(x => x.Name == knowledgeName),
            ProcedureBeginEmote = emote,
            ProcedureGerund = gerund,
            ProcedureDescriptionEmote = description,
            Check = (int)check,
            Definition = definition,
            TargetBodyType = targetBody
        };
        _context.SurgicalProcedures.Add(procedure);
        _context.SaveChanges();

        configurePhases(procedure);
        _procedures[name] = procedure;
        _context.SaveChanges();
        return 1;
    }

    private void AddSurgicalProcedurePhase(MudSharp.Models.SurgicalProcedure procedure, int phaseNumber, double seconds,
        string? special, string emote, string? inventoryPlan = null)
    {
        procedure.SurgicalProcedurePhases.Add(new MudSharp.Models.SurgicalProcedurePhase
        {
            SurgicalProcedureId = procedure.Id,
            PhaseNumber = phaseNumber,
            BaseLengthInSeconds = seconds,
            PhaseEmote = emote,
            PhaseSpecialEffects = special ?? string.Empty,
            InventoryActionPlan = inventoryPlan
        });
    }

    private string ProduceInventoryPlanDefinition(params (InventoryState State, string Tag, int Quantity)[] actions)
    {
        List<XElement> elements = new();
        foreach ((InventoryState State, string Tag, int Quantity) action in actions)
        {
            if (!_tags.TryGetValue(action.Tag, out Tag? tag))
            {
                throw new InvalidOperationException($"RobotSeeder requires the tag '{action.Tag}', but it has not been installed.");
            }

            elements.Add(action.State switch
            {
                InventoryState.Held => new XElement("Action",
                    new XAttribute("state", "held"),
                    new XAttribute("tag", tag.Id),
                    new XAttribute("quantity", action.Quantity),
                    new XAttribute("optionalquantity", false)),
                InventoryState.Wielded => new XElement("Action",
                    new XAttribute("state", "wielded"),
                    new XAttribute("tag", tag.Id),
                    new XAttribute("wieldstate", (int)AttackHandednessOptions.Any)),
                InventoryState.Worn => new XElement("Action",
                    new XAttribute("state", "worn"),
                    new XAttribute("tag", tag.Id)),
                InventoryState.Dropped => new XElement("Action",
                    new XAttribute("state", "dropped"),
                    new XAttribute("tag", tag.Id)),
                InventoryState.Sheathed => new XElement("Action",
                    new XAttribute("state", "sheathed"),
                    new XAttribute("tag", tag.Id)),
                _ => throw new ArgumentOutOfRangeException()
            });
        }

        return new XElement("Plan", elements).ToString();
    }

    private string GetDefinitionForTargets(BodyProto targetBody, params string[] partAliases)
    {
        return new XElement(
            "Definition",
            new XElement(
                "Parts",
                new XAttribute("forbidden", false),
                from alias in partAliases
                let part = FindBodypartOnBody(targetBody, alias)
                where part is not null
                select new XElement("Part", part.Id)
            )
        ).ToString();
    }

    private string ScalpelPlan()
    {
        return ProduceInventoryPlanDefinition((InventoryState.Held, "Scalpel", 1));
    }

    private string BonesawPlan()
    {
        return ProduceInventoryPlanDefinition((InventoryState.Held, "Bonesaw", 1));
    }

    private string ForcepsPlan()
    {
        return ProduceInventoryPlanDefinition((InventoryState.Held, "Forceps", 1));
    }

    private string ClampPlan()
    {
        return ProduceInventoryPlanDefinition((InventoryState.Held, "Arterial Clamp", 1));
    }

    private string SutureNeedlePlan()
    {
        return ProduceInventoryPlanDefinition((InventoryState.Held, "Surgical Suture Needle", 1));
    }
}
