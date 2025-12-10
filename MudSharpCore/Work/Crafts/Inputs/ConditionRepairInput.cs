using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Inputs;

public class ConditionRepairInput : BaseInput, ICraftInputConsumesGameItem
{
    public override string InputType => "ConditionRepair";
    public ITag TargetTag { get; set; }
    public double RepairAmount { get; set; }

    public override bool RefersToTag(ITag tag)
    {
        return TargetTag?.IsA(tag) == true;
    }

    protected ConditionRepairInput(Models.CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft, gameworld)
    {
        var root = XElement.Parse(input.Definition);
        TargetTag = gameworld.Tags.Get(long.Parse(root.Element("TargetTagId").Value));
        RepairAmount = double.Parse(root.Element("RepairAmount").Value);
    }

    protected ConditionRepairInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
    {
        RepairAmount = 1.0;
    }

    public static void RegisterCraftInput()
    {
        CraftInputFactory.RegisterCraftInputType("ConditionRepair", (input, craft, game) => new ConditionRepairInput(input, craft, game));
        CraftInputFactory.RegisterCraftInputTypeForBuilders("repair", (craft, game) => new ConditionRepairInput(craft, game));
    }

    protected override string SaveDefinition()
    {
        return new XElement("Definition",
            new XElement("TargetTagId", TargetTag?.Id ?? 0),
            new XElement("RepairAmount", RepairAmount)
        ).ToString();
    }

    #region Building
    protected override string BuildingHelpString =>
        @"You can use the following options with this input type:

	#3quality <weighting>#0 - sets the weighting of this input in determining overall quality
	#3tag <id|name>#0 - sets the tag required
	#3quantity <amount>#0 - sets the amount required";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "tag":
            case "item":
                return BuildingCommandTag(actor, command);
            case "repair":
            case "repairamount":
            case "amount":
                return BuildingCommandRepairAmount(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandTag(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which tag do you want items to have to satisfy this input?");
            return false;
        }

        var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
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

        var tag = matchedtags.Single();

        TargetTag = tag;
        InputChanged = true;
        actor.OutputHandler.Send($"This input will now target items with the {tag.FullName.Colour(Telnet.Cyan)} tag.");
        return true;
    }

    private bool BuildingCommandRepairAmount(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify a percentage.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
            return false;
        }

        value = Math.Clamp(value, 0.0, 1.0);

        RepairAmount = value;
        InputChanged = true;
        actor.OutputHandler.Send($"This input will now repair {value.ToStringP2Colour(actor)} condition on the linked item.");
        return true;
    }

    public override bool IsValid()
    {
        return TargetTag != null;
    }

    public override string WhyNotValid()
    {
        return "You must first set a target tag for this input.";
    }
    #endregion

    #region Implementation of ICraftInput
    public override string Name
    {
        get
        {
            if (TargetTag == null)
            {
                return $"{RepairAmount.ToString("P2").ColourValue()} repair of an item with {"an unspecified tag".Colour(Telnet.Red)}";
            }

            return $"{RepairAmount.ToString("P2").ColourValue()} repair of an item with the {TargetTag.FullName.Colour(Telnet.Cyan)} tag";
        }
    }

    public override string HowSeen(IPerceiver voyeur)
    {
        if (TargetTag == null)
        {
            return $"{RepairAmount.ToString("P2").ColourValue()} repair of an item with {"an unspecified tag".Colour(Telnet.Red)}";
        }

        return $"{RepairAmount.ToString("P2").ColourValue()} repair of an item with the {TargetTag.FullName.Colour(Telnet.Cyan)} tag";
    }

    public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
    {
        var foundItems = new List<IGameItem>();
        foreach (var item in character.DeepContextualItems
            .Except(character.Body.WornItems)
        )
        {
            if (item.Tags.All(y => !y.IsA(TargetTag)))
            {
                continue;
            }

            if (item.Condition >= 1.0)
            {
                continue;
            }

            foundItems.Add(item);
        }

        return foundItems;
    }

    public override bool IsInput(IPerceivable item)
    {
        return item is IGameItem gi &&
               gi.Tags.Any(x => x.IsA(TargetTag)) &&
               gi.Condition < 1.0;
    }

    public override double ScoreInputDesirability(IPerceivable item)
    {
        return item is IGameItem gi
            ? (1.0 - gi.Condition)
            : 0.0;
    }

    public override void UseInput(IPerceivable item, ICraftInputData data)
    {
        var icidwi = (ICraftInputDataWithItems)data;
        foreach (var gameItem in icidwi.ConsumedItems)
        {
            var repairAmount = RepairAmount;
            if (gameItem.Condition + repairAmount > 1.0)
            {
                repairAmount = 1.0 - gameItem.Condition;
            }
            gameItem.Condition += repairAmount;
        }
    }

    public override ICraftInputData ReserveInput(IPerceivable input)
    {
        return new NonDeletingItemInputData([(IGameItem)input]);
    }

    public override ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld)
    {
        return new NonDeletingItemInputData(root, gameworld);
    }

    #endregion

    internal class NonDeletingItemInputData : ICraftInputDataWithItems
    {
        public NonDeletingItemInputData(XElement root, IFuturemud gameworld)
        {
            foreach (var element in root.Element("ConsumedItems").Elements())
            {
                var item = gameworld.TryGetItem(long.Parse(element.Value));
                ConsumedItems.Add(item);
            }

            ConsumedGroup = new PerceivableGroup(ConsumedItems);
        }

        public NonDeletingItemInputData(IEnumerable<IGameItem> items)
        {
            ConsumeInput(items);
        }

        public XElement SaveToXml()
        {
            return new XElement("Data",
                new XElement("ConsumedItems",
                    from item in ConsumedItems
                    select new XElement("ConsumedItem", item.Id)
                )
            );
        }

        public void ReleaseItemsAtCraftCompletion(ICell location, RoomLayer layer)
        {
            foreach (var item in ConsumedItems)
            {
                item.RoomLayer = layer;
                location.Insert(item);
                item.Login();
            }

            ConsumedItems.Clear();
        }

        public void FinaliseLoadTimeTasks()
        {
            foreach (var item in ConsumedItems)
            {
                item.FinaliseLoadTimeTasks();
            }
        }

        public void ConsumeInput(IEnumerable<IGameItem> items)
        {
            foreach (var item in items)
            {
                var target = item;
                item.InInventoryOf?.Take(target);
                item.Location?.Extract(target);
                item.ContainedIn?.Take(target);
                var connectable = target.GetItemType<IConnectable>();
                foreach (var attached in target.AttachedAndConnectedItems)
                {
                    attached.GetItemType<IConnectable>()?.RawDisconnect(connectable, true);
                }

                ConsumedItems.Add(target);
                item.Quit();
            }

            ConsumedGroup = new PerceivableGroup(ConsumedItems);
        }

        public ItemQuality InputQuality =>
            ConsumedItems.Select(x => (x.Quality, (double)x.Quantity)).GetNetQuality();

        public IPerceivable Perceivable => ConsumedGroup;

        public List<IGameItem> ConsumedItems { get; } = new();

        IEnumerable<IGameItem> ICraftInputDataWithItems.ConsumedItems => ConsumedItems;

        public PerceivableGroup ConsumedGroup { get; set; }

        public void Delete()
        {
            foreach (var item in ConsumedItems.ToList())
            {
                item.Delete();
            }
        }

        public void Quit()
        {
            foreach (var item in ConsumedItems.ToList())
            {
                item.Quit();
            }
        }
    }
}
