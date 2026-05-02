using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Work.Crafts.Tools;

public class SimpleTool : BaseTool
{
    protected SimpleTool(Models.CraftTool tool, ICraft craft, IFuturemud gameworld) : base(tool, craft, gameworld)
    {
        XElement root = XElement.Parse(tool.Definition);
        TargetItemId = long.Parse(root.Element("TargetItemId").Value);
    }

    protected SimpleTool(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
    {
    }

    public long TargetItemId { get; set; }

    /// <inheritdoc />
    public override bool RefersToItemProto(long id)
    {
        return TargetItemId == id;
    }

    #region Overrides of BaseTool

    public override bool IsTool(IGameItem item)
    {
        return item?.Prototype.Id == TargetItemId;
    }

    public override double ToolFitness(IGameItem item)
    {
        return Math.Max(0.0, 1.0 + item.EffectsOfType<IMagicCraftToolEnhancementEffect>(x =>
            x.AppliesToCraftTool(item, null)).Sum(x => x.ToolFitnessBonus));
    }

    public override double PhaseLengthMultiplier(IGameItem item)
    {
        return item.EffectsOfType<IMagicCraftToolEnhancementEffect>(x =>
            x.AppliesToCraftTool(item, null))
            .Aggregate(1.0, (current, effect) => current * effect.ToolSpeedMultiplier);
    }

    public override void UseTool(IGameItem item, TimeSpan phaseLength, bool hasFailed)
    {
        if (!UseToolDuration)
        {
            return;
        }

        var usageMultiplier = item.EffectsOfType<IMagicCraftToolEnhancementEffect>(x =>
            x.AppliesToCraftTool(item, null))
            .Aggregate(1.0, (current, effect) => current * effect.ToolUsageMultiplier);
        item.GetItemType<IToolItem>()?.UseTool(null, TimeSpan.FromTicks((long)(phaseLength.Ticks * usageMultiplier)));
    }

    public override string ToolType => "SimpleTool";

    protected override string SaveDefinition()
    {
        return new XElement("Definition", new XElement("TargetItemId", TargetItemId)).ToString();
    }

    public override string BuilderHelpString =>
        $"{base.BuilderHelpString}\n\t#3item <id|name>#0 - the item to target for this tool";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "item":
            case "tool":
            case "target":
                return BuildingCommandItem(actor, command);
        }

        return base.BuildingCommand(actor, command);
    }

    private bool BuildingCommandItem(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which item should this tool target?");
            return false;
        }

        IGameItemProto item = long.TryParse(command.SafeRemainingArgument, out long value)
            ? Gameworld.ItemProtos.Get(value)
            : Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
        if (item == null)
        {
            actor.OutputHandler.Send("There is no such item prototype.");
            return false;
        }

        TargetItemId = item.Id;
        ToolChanged = true;
        actor.OutputHandler.Send(
            $"This tool will now target item prototype #{TargetItemId.ToString("N0", actor)} ({item.ShortDescription.ColourObject()}).");
        return true;
    }

    public override bool IsValid()
    {
        return TargetItemId != 0;
    }

    public override string WhyNotValid()
    {
        return "You must first set a target item for this tool.";
    }

    #endregion

    public static void RegisterCraftTool()
    {
        CraftToolFactory.RegisterCraftToolType("SimpleTool",
            (input, craft, game) => new SimpleTool(input, craft, game));
        CraftToolFactory.RegisterCraftToolTypeForBuilders("simple", (craft, game) => new SimpleTool(craft, game));
    }

    public override string Name => Gameworld.ItemProtos.Get(TargetItemId)?.ShortDescription ??
                                   "an unspecified item".Colour(Telnet.Red);

    public override string HowSeen(IPerceiver voyeur)
    {
        IGameItemProto proto = Gameworld.ItemProtos.Get(TargetItemId);
        return
            proto is null ? "an unspecified item".Colour(Telnet.Red) : $"{proto.ShortDescription.Colour(proto.CustomColour ?? Telnet.Green)} (#{proto.Id.ToStringN0(voyeur)})";
    }
}
