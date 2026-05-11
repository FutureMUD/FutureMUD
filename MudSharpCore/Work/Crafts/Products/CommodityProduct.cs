using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Work.Crafts.Products;

public class CommodityProduct : BaseProduct
{
    public sealed class CommodityCharacteristicOutputRule
    {
        public ICharacteristicDefinition Definition { get; init; }
        public ICharacteristicValue FixedValue { get; init; }
        public int? InputIndex { get; init; }
    }

    public ISolid Material { get; set; }
    public double Weight { get; set; }
    public ITag Tag { get; set; }
    public Dictionary<ICharacteristicDefinition, CommodityCharacteristicOutputRule> CharacteristicOutputs { get; } = new();

    /// <inheritdoc />
    public override bool RefersToTag(ITag tag)
    {
        return Tag?.IsA(tag) == true;
    }

    /// <inheritdoc />
    public override bool IsItem(IGameItem item)
    {
        ICommodity commodity = item.GetItemType<ICommodity>();
        if (commodity is null)
        {
            return false;
        }

        if (commodity.Material != Material || commodity.Tag != Tag)
        {
            return false;
        }

        if (commodity.CommodityCharacteristics.Count != CharacteristicOutputs.Count)
        {
            return false;
        }

        foreach (CommodityCharacteristicOutputRule rule in CharacteristicOutputs.Values)
        {
            if (!commodity.CommodityCharacteristics.TryGetValue(rule.Definition, out ICharacteristicValue value))
            {
                return false;
            }

            if (rule.FixedValue is not null && value != rule.FixedValue)
            {
                return false;
            }
        }

        return true;
    }

    protected CommodityProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
        gameworld)
    {
        XElement root = XElement.Parse(product.Definition);
        Weight = double.Parse(root.Element("Weight")?.Value ?? "0");
        Material = Gameworld.Materials.Get(long.Parse(root.Element("Material")?.Value ?? "0"));
        Tag = Gameworld.Tags.Get(long.Parse(root.Element("Tag")?.Value ?? "0"));
        LoadCharacteristicOutputs(root.Element("Characteristics"));
    }

    protected CommodityProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld,
        failproduct)
    {
    }

    protected override string SaveDefinition()
    {
        return new XElement("Definition",
            new XElement("Material", Material?.Id ?? 0),
            new XElement("Weight", Weight),
            new XElement("Tag", Tag?.Id ?? 0),
            new XElement("Characteristics",
                from rule in CharacteristicOutputs.Values.OrderBy(x => x.Definition.Id)
                select new XElement("Characteristic",
                    new XAttribute("definition", rule.Definition.Id),
                    new XAttribute("value", rule.FixedValue?.Id ?? 0L),
                    new XAttribute("input", rule.InputIndex ?? -1)
                )
            )
        ).ToString();
    }

    private void LoadCharacteristicOutputs(XElement root)
    {
        CharacteristicOutputs.Clear();
        if (root is null)
        {
            return;
        }

        foreach (XElement element in root.Elements("Characteristic"))
        {
            long definitionId = long.Parse(element.Attribute("definition")?.Value ?? element.Attribute("Definition")?.Value ?? "0");
            ICharacteristicDefinition definition = Gameworld.Characteristics.Get(definitionId);
            if (definition is null)
            {
                continue;
            }

            long valueId = long.Parse(element.Attribute("value")?.Value ?? element.Attribute("Value")?.Value ?? "0");
            ICharacteristicValue value = valueId == 0 ? null : Gameworld.CharacteristicValues.Get(valueId);
            if (value is not null && !definition.IsValue(value))
            {
                continue;
            }

            int inputIndex = int.Parse(element.Attribute("input")?.Value ?? element.Attribute("Input")?.Value ?? "-1");
            if (value is null && inputIndex < 0)
            {
                continue;
            }

            CharacteristicOutputs[definition] = new CommodityCharacteristicOutputRule
            {
                Definition = definition,
                FixedValue = value,
                InputIndex = inputIndex >= 0 ? inputIndex : null
            };
        }
    }

    public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
        ItemQuality referenceQuality)
    {
        IGameItem newitem = GameItems.Prototypes.CommodityGameItemComponentProto.CreateNewCommodity(Material, Weight, Tag, false,
            ResolveProductCharacteristics(component));
        newitem.RoomLayer = component.Parent.RoomLayer;
        Gameworld.Add(newitem);
        return new SimpleProductData(new[]
        {
            newitem
        });
    }

    private IEnumerable<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> ResolveProductCharacteristics(IActiveCraftGameItemComponent component)
    {
        foreach (CommodityCharacteristicOutputRule rule in CharacteristicOutputs.Values.OrderBy(x => x.Definition.Id))
        {
            if (rule.FixedValue is not null)
            {
                yield return (rule.Definition, rule.FixedValue);
                continue;
            }

            if (!rule.InputIndex.HasValue ||
                Craft.Inputs.ElementAtOrDefault(rule.InputIndex.Value) is not IVariableInput input ||
                !component.ConsumedInputs.ContainsKey(input))
            {
                continue;
            }

            ICharacteristicValue value = input.GetValueForVariable(rule.Definition, component.ConsumedInputs[input].Data);
            if (value is not null && rule.Definition.IsValue(value))
            {
                yield return (rule.Definition, value);
            }
        }
    }

    public override string ProductType => "CommodityProduct";

    public static void RegisterCraftProduct()
    {
        CraftProductFactory.RegisterCraftProductType("CommodityProduct",
            (product, craft, game) => new CommodityProduct(product, craft, game));
        CraftProductFactory.RegisterCraftProductTypeForBuilders("commodity",
            (craft, game, fail) => new CommodityProduct(craft, game, fail));
    }

    protected override string BuildingHelpText => $@"{base.BuildingHelpText}
	#3commodity <material>#0 - sets the target material
	#3weight <weight>#0 - sets the required weight of material
	#3tag <which>|none#0 - sets or clears the tag of the commodity
	#3characteristic <definition> <value>|from <input#>|remove#0 - sets, copies or clears a commodity characteristic output";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "commodity":
            case "commoditymaterial":
            case "commoditymat":
            case "commodity mat":
            case "commodity_mat":
            case "commodity material":
            case "commodity_material":
                return BuildingCommandMaterial(actor, command);
            case "weight":
            case "amount":
            case "quantity":
                return BuildingCommandQuantity(actor, command);
            case "tag":
                return BuildingCommandTag(actor, command);
            case "characteristic":
            case "characteristics":
            case "char":
            case "variable":
            case "var":
                return BuildingCommandCharacteristic(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandCharacteristic(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which characteristic definition do you want to set for this commodity product?");
            return false;
        }

        ICharacteristicDefinition definition = Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
        if (definition is null)
        {
            actor.OutputHandler.Send("There is no such characteristic definition.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Do you want to set {definition.Name.ColourName()} to a fixed value, copy it from an input with FROM <input#>, or REMOVE it?");
            return false;
        }

        string next = command.PopSpeech();
        if (next.EqualToAny("remove", "delete", "clear", "none"))
        {
            if (CharacteristicOutputs.Remove(definition))
            {
                ProductChanged = true;
                actor.OutputHandler.Send($"This product will no longer set the {definition.Name.ColourName()} commodity characteristic.");
                return true;
            }

            actor.OutputHandler.Send($"This product was not setting the {definition.Name.ColourName()} commodity characteristic.");
            return false;
        }

        if (next.EqualTo("from"))
        {
            if (command.IsFinished || !int.TryParse(command.PopSpeech(), out int inputNumber))
            {
                actor.OutputHandler.Send("Which input number do you want to copy this characteristic from?");
                return false;
            }

            if (Craft.Inputs.ElementAtOrDefault(inputNumber - 1) is not IVariableInput input)
            {
                actor.OutputHandler.Send("There is no variable-capable input with that number.");
                return false;
            }

            if (!input.DeterminesVariable(definition))
            {
                actor.OutputHandler.Send($"Input $i{inputNumber.ToString("N0", actor)} does not provide the {definition.Name.ColourName()} characteristic.");
                return false;
            }

            CharacteristicOutputs[definition] = new CommodityCharacteristicOutputRule
            {
                Definition = definition,
                InputIndex = inputNumber - 1
            };
            ProductChanged = true;
            actor.OutputHandler.Send($"This product will copy the {definition.Name.ColourName()} commodity characteristic from input $i{inputNumber.ToString("N0", actor)}.");
            return true;
        }

        string valueText = command.IsFinished ? next : $"{next} {command.SafeRemainingArgument}";
        ICharacteristicValue value = CommodityCharacteristicRequirement.GetCharacteristicValue(Gameworld, valueText);
        if (value is null || !definition.IsValue(value))
        {
            actor.OutputHandler.Send($"There is no {definition.Name.ColourName()} characteristic value identified by {valueText.ColourCommand()}.");
            return false;
        }

        CharacteristicOutputs[definition] = new CommodityCharacteristicOutputRule
        {
            Definition = definition,
            FixedValue = value
        };
        ProductChanged = true;
        actor.OutputHandler.Send($"This product will set the {definition.Name.ColourName()} commodity characteristic to {value.GetValue.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandTag(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either specify a tag for the commodity or use {"none".ColourCommand()} to clear it.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            Tag = null;
            ProductChanged = true;
            actor.OutputHandler.Send("This product will now not set any tag on the commodity pile.");
            return true;
        }

        ITag tag = Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
        if (tag is null)
        {
            actor.OutputHandler.Send("There is no such tag.");
            return false;
        }

        Tag = tag;
        ProductChanged = true;
        actor.OutputHandler.Send(
            $"This product will set the {Tag.FullName.ColourName()} tag on the loaded commodity pile.");
        return true;
    }

    private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
    {
        if (Material == null)
        {
            actor.OutputHandler.Send("You must first set a material before you set a weight.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How much of the material do you want this product to produce?");
            return false;
        }

        double amount = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, Framework.Units.UnitType.Mass,
            out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid weight.");
            return false;
        }

        Weight = amount;
        ProductChanged = true;
        actor.OutputHandler.Send(
            $"This product will now produce {Gameworld.UnitManager.DescribeExact(Weight, Framework.Units.UnitType.Mass, actor).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}.");
        return true;
    }

    private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which material did you want this product to produce?");
            return false;
        }

        ISolid material = long.TryParse(command.PopSpeech(), out long value)
            ? Gameworld.Materials.Get(value)
            : Gameworld.Materials.GetByName(command.Last);
        if (material == null)
        {
            actor.OutputHandler.Send("There is no such material.");
            return false;
        }

        Material = material;
        ProductChanged = true;
        actor.OutputHandler.Send(
            $"This product will now produce the {Material.Name.Colour(Material.ResidueColour)} material.");
        return true;
    }

    public override bool IsValid()
    {
        return Material != null &&
               Weight > 0.0 &&
               CharacteristicOutputs.Values.All(x =>
                   (x.FixedValue is not null && x.Definition.IsValue(x.FixedValue)) ||
                   (x.InputIndex.HasValue &&
                    Craft.Inputs.ElementAtOrDefault(x.InputIndex.Value) is IVariableInput input &&
                    input.DeterminesVariable(x.Definition)));
    }

    public override string WhyNotValid()
    {
        if (Material == null)
        {
            return "You must first set a material for this product to produce.";
        }

        if (Weight <= 0.0)
        {
            return "You must set a positive weight of material for this product to produce.";
        }

        foreach (CommodityCharacteristicOutputRule rule in CharacteristicOutputs.Values)
        {
            if (rule.FixedValue is not null && !rule.Definition.IsValue(rule.FixedValue))
            {
                return $"The fixed commodity characteristic output for {rule.Definition.Name} is not a valid value for that definition.";
            }

            if (rule.InputIndex.HasValue &&
                (Craft.Inputs.ElementAtOrDefault(rule.InputIndex.Value) is not IVariableInput input ||
                 !input.DeterminesVariable(rule.Definition)))
            {
                return $"The commodity characteristic output for {rule.Definition.Name} is not supplied by input #{rule.InputIndex.Value + 1}.";
            }

            if (rule.FixedValue is null && !rule.InputIndex.HasValue)
            {
                return $"The commodity characteristic output for {rule.Definition.Name} does not have a fixed value or input source.";
            }
        }

        throw new ApplicationException("Unknown WhyNotValid reason in CommodityProduct.");
    }

    public override string Name => Material != null
        ? $"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, DummyAccount.Instance).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}{(Tag is not null ? $" {Tag.Name.Pluralise()}" : "")} commodity{DescribeCharacteristicOutputs()}"
        : "An unspecified amount of an unspecified commodity";

    public override string HowSeen(IPerceiver voyeur)
    {
        if (Material != null)
        {
            return
                $"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}{(Tag is not null ? $" {Tag.Name.Pluralise()}" : "")} commodity{DescribeCharacteristicOutputs()}";
        }

        return "An unspecified amount of an unspecified commodity";
    }

    private string DescribeCharacteristicOutputs()
    {
        if (CharacteristicOutputs.Count == 0)
        {
            return "";
        }

        StringBuilder sb = new();
        sb.Append(" [");
        sb.Append(CharacteristicOutputs.Values.OrderBy(x => x.Definition.Name).ThenBy(x => x.Definition.Id).Select(x =>
        {
            if (x.FixedValue is not null)
            {
                return $"{x.Definition.Name.ColourName()}={x.FixedValue.GetValue.ColourValue()}";
            }

            return x.InputIndex.HasValue
                ? $"{x.Definition.Name.ColourName()}<-{"$i" + (x.InputIndex.Value + 1).ToString("N0")}"
                : $"{x.Definition.Name.ColourName()}<-{"invalid".ColourError()}";
        }).ListToString());
        sb.Append(']');
        return sb.ToString();
    }
}
