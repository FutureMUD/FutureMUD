using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Work.Crafts.Tools;

public class TagTool : BaseTool
{
    protected TagTool(Models.CraftTool tool, ICraft craft, IFuturemud gameworld) : base(tool, craft, gameworld)
    {
        XElement root = XElement.Parse(tool.Definition);
        TargetItemTag = gameworld.Tags.Get(long.Parse(root.Element("TargetItemTag").Value));
    }

    protected TagTool(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
    {
    }

    public ITag TargetItemTag { get; set; }

    /// <inheritdoc />
    public override bool RefersToTag(ITag tag)
    {
        return TargetItemTag?.IsA(tag) == true;
    }

    #region Overrides of BaseTool

    public override bool IsTool(IGameItem item)
    {
        return item.GetItemType<IToolItem>()?.CountAsTool(TargetItemTag) ??
               item?.Tags.Any(x => x.IsA(TargetItemTag)) == true;
    }

    public override double ToolFitness(IGameItem item)
    {
        var enhancements = item.EffectsOfType<IMagicCraftToolEnhancementEffect>(x =>
            x.AppliesToCraftTool(item, TargetItemTag)).ToList();
        IToolItem tool = item.GetItemType<IToolItem>();
        if (tool == null)
        {
            return Math.Max(0.0, 1.0 + enhancements.Sum(x => x.ToolFitnessBonus));
        }

        var multiplier = tool.ToolTimeMultiplier(TargetItemTag) *
                         enhancements.Aggregate(1.0, (current, effect) => current * effect.ToolSpeedMultiplier);
        if (multiplier <= 0.0)
        {
            return 0.0;
        }

        return Math.Max(0.0, 1.0 / multiplier + enhancements.Sum(x => x.ToolFitnessBonus));
    }

    public override void UseTool(IGameItem item, TimeSpan phaseLength, bool hasFailed)
    {
        if (!UseToolDuration)
        {
            return;
        }
        var usageMultiplier = item.EffectsOfType<IMagicCraftToolEnhancementEffect>(x =>
            x.AppliesToCraftTool(item, TargetItemTag))
            .Aggregate(1.0, (current, effect) => current * effect.ToolUsageMultiplier);
        item.GetItemType<IToolItem>()?.UseTool(TargetItemTag, TimeSpan.FromTicks((long)(phaseLength.Ticks * usageMultiplier)));
    }

    public override double PhaseLengthMultiplier(IGameItem item)
    {
        var multiplier = item.EffectsOfType<IMagicCraftToolEnhancementEffect>(x =>
            x.AppliesToCraftTool(item, TargetItemTag))
            .Aggregate(1.0, (current, effect) => current * effect.ToolSpeedMultiplier);
        return (item.GetItemType<IToolItem>()?.ToolTimeMultiplier(TargetItemTag) ?? 1.0) * multiplier;
    }

    public override string ToolType => "TagTool";

    protected override string SaveDefinition()
    {
        return new XElement("Definition", new XElement("TargetItemTag", TargetItemTag?.Id ?? 0)).ToString();
    }

    #endregion

    public static void RegisterCraftTool()
    {
        CraftToolFactory.RegisterCraftToolType("TagTool", (input, craft, game) => new TagTool(input, craft, game));
        CraftToolFactory.RegisterCraftToolTypeForBuilders("tag", (craft, game) => new TagTool(craft, game));
    }

    public override string Name
    {
        get
        {
            if (TargetItemTag == null)
            {
                return "an item with an unspecified tag".Colour(Telnet.Red);
            }

            return $"an item with the {TargetItemTag?.Name.Colour(Telnet.Cyan)} tag";
        }
    }

    public override string HowSeen(IPerceiver voyeur)
    {
        if (TargetItemTag == null)
        {
            return "an item with an unspecified tag".Colour(Telnet.Red);
        }

        return $"an item with the {TargetItemTag?.Name.Colour(Telnet.Cyan)} tag";
    }

    public override bool IsValid()
    {
        return TargetItemTag != null;
    }

    public override string WhyNotValid()
    {
        return "You must first set a target item tag.";
    }

    public override string BuilderHelpString =>
        $"{base.BuilderHelpString}\n\t#3tag <id|name>#0 - sets the target tag required for this tool";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "tag":
            case "item":
                return BuildingCommandTag(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandTag(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which tag do you want items to have to satisfy this tool?");
            return false;
        }

        List<ITag> matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
        if (matchedtags.Count == 0)
        {
            actor.OutputHandler.Send("There is no such tag.");
            return false;
        }

        if (matchedtags.Count > 1)
        {
            actor.OutputHandler.Send(
                $"Your text matched multiple tags. Please specify one of the following tags:\n\n{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
            return false;
        }

        ITag tag = matchedtags.Single();

        TargetItemTag = tag;
        ToolChanged = true;
        actor.OutputHandler.Send($"This tool will now target items with the {tag.FullName.Colour(Telnet.Cyan)} tag.");
        return true;
    }
}
