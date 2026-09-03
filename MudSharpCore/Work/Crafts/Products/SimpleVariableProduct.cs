using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;

namespace MudSharp.Work.Crafts.Products;

public class SimpleVariableProduct : SimpleProduct
{
    protected SimpleVariableProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
        gameworld)
    {
        XElement root = XElement.Parse(product.Definition);
        foreach (XElement item in root.Elements("Variable"))
        {
            Characteristics.Add((gameworld.Characteristics.Get(long.Parse(item.Value)),
                int.Parse(item.Attribute("inputindex").Value)));
        }
		foreach (var item in root.Elements("FixedVariable"))
		{
			FixedCharacteristics.Add((gameworld.Characteristics.Get(long.Parse(item.Value)),
				gameworld.CharacteristicValues.Get(long.Parse(item.Attribute("value").Value))));
		}
    }

    protected SimpleVariableProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld,
        failproduct)
    {
    }

    public List<(ICharacteristicDefinition Definition, int InputIndex)> Characteristics { get; } = new();
	public List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> FixedCharacteristics { get; } = new();

    public new static void RegisterCraftProduct()
    {
        CraftProductFactory.RegisterCraftProductType("SimpleVariableProduct",
            (product, craft, game) => new SimpleVariableProduct(product, craft, game));
        CraftProductFactory.RegisterCraftProductTypeForBuilders("variable",
            (craft, game, fail) => new SimpleVariableProduct(craft, game, fail));
    }

    public override string ProductType => "SimpleVariableProduct";

    protected override string SaveDefinition()
    {
        return new XElement("Definition",
            new XElement("ProductProducedId", ProductProducedId),
            new XElement("Quantity", Quantity),
            new XElement("Skin", Skin?.Id ?? 0),
            from item in Characteristics
            select new XElement("Variable", new XAttribute("inputindex", item.InputIndex), item.Definition?.Id ?? 0),
			from item in FixedCharacteristics
			select new XElement("FixedVariable", new XAttribute("value", item.Value?.Id ?? 0), item.Definition?.Id ?? 0)
        ).ToString();
    }

    protected override string BuildingHelpText =>
        $"{base.BuildingHelpText}\n\t#3variable <definition> <input#>#0 - copies a characteristic from a variable-aware input\n\t#3variable <definition> value <value>#0 - selects an exact characteristic for this product\n\t#3variable <definition>#0 - removes either kind of characteristic rule";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "variable":
            case "var":
            case "characteristic":
            case "char":
            case "definition":
                return BuildingCommandVariable(actor, command);
        }

        return base.BuildingCommand(actor, new StringStack($"{command.Last} {command.SafeRemainingArgument}"));
    }

    private bool BuildingCommandVariable(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which characteristic definition did you want to change?");
            return false;
        }

        ICharacteristicDefinition definition = Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
        if (definition == null)
        {
            actor.OutputHandler.Send("There is no such characteristic definition.");
            return false;
        }

        if (command.IsFinished)
        {
            if (Characteristics.Any(x => x.Definition == definition) || FixedCharacteristics.Any(x => x.Definition == definition))
            {
                Characteristics.RemoveAll(x => x.Definition == definition);
				FixedCharacteristics.RemoveAll(x => x.Definition == definition);
                ProductChanged = true;
                actor.OutputHandler.Send(
                    $"This product will no longer be supplied the variable {definition.Name.Colour(Telnet.Yellow)}.");
                return true;
            }
        }

		if (command.PeekSpeech().EqualTo("value"))
		{
			command.PopSpeech();
			var text = command.SafeRemainingArgument;
			var candidates = Gameworld.CharacteristicValues
				.Where(definition.IsValue)
				.Where(x => long.TryParse(text, out var id) ? x.Id == id : x.Name.EqualTo(text))
				.ToArray();
			if (candidates.Length != 1)
			{
				actor.OutputHandler.Send("Specify an exact, unambiguous value name or ID belonging to that characteristic.");
				return false;
			}

			Characteristics.RemoveAll(x => x.Definition == definition);
			FixedCharacteristics.RemoveAll(x => x.Definition == definition);
			FixedCharacteristics.Add((definition, candidates[0]));
			ProductChanged = true;
			actor.OutputHandler.Send($"This product will select {candidates[0].Name.ColourValue()} for {definition.Name.ColourName()}; other copies of the item prototype are unchanged.");
			return true;
		}

        if (!int.TryParse(command.PopSpeech(), out int ivalue))
        {
            actor.OutputHandler.Send("Which input number do you want to pair this product with?");
            return false;
        }

        ICraftInput input = Craft.Inputs.ElementAtOrDefault(ivalue - 1);
        if (input == null)
        {
            actor.OutputHandler.Send("There is no such input for this craft.");
            return false;
        }
		if (input is not IVariableInput variableInput || !variableInput.DeterminesVariable(definition))
		{
			actor.OutputHandler.Send("That input does not provide this characteristic. Configure a variable-aware input first.");
			return false;
		}

        if (Characteristics.Any(x => x.Definition == definition))
        {
            Characteristics.RemoveAll(x => x.Definition == definition);
        }

        Characteristics.Add((definition, ivalue - 1));
		FixedCharacteristics.RemoveAll(x => x.Definition == definition);
        actor.OutputHandler.Send(
            $"This input will now be supplied the variable {definition.Name.Colour(Telnet.Yellow)} from the input {input.Name}.");
        ProductChanged = true;
        return true;
    }

    public override bool IsValid()
    {
        return base.IsValid() && !VariableErrors().Any();
    }

    public override string WhyNotValid()
    {
		var errors = VariableErrors().ToArray();
		return errors.Length > 0 ? string.Join("\n", errors) : base.WhyNotValid();
    }

	private IEnumerable<string> VariableErrors()
	{
		var definitions = new HashSet<ICharacteristicDefinition>();
		foreach (var (definition, inputIndex) in Characteristics)
		{
			if (definition is null)
			{
				yield return "A variable rule refers to a missing characteristic definition.";
				continue;
			}
			if (!definitions.Add(definition)) yield return $"Characteristic {definition.Name} has more than one product rule.";
			if (inputIndex < 0 || Craft.Inputs.ElementAtOrDefault(inputIndex) is not IVariableInput input || !input.DeterminesVariable(definition))
			{
				yield return $"Craft Input $i{inputIndex + 1} determining variable {definition.Name.ColourValue()} was not found or was not providing said variable.";
			}
		}
		foreach (var (definition, value) in FixedCharacteristics)
		{
			if (definition is null)
			{
				yield return "A fixed variable rule refers to a missing characteristic definition.";
				continue;
			}
			if (!definitions.Add(definition)) yield return $"Characteristic {definition.Name} has more than one product rule.";
			if (value is null || !definition.IsValue(value)) yield return $"Characteristic {definition.Name} has a missing or incompatible selected value.";
		}
	}

    public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
        ItemQuality referenceQuality)
    {
        IGameItemProto proto = Gameworld.ItemProtos.Get(ProductProducedId);
        if (proto is null)
        {
            throw new ApplicationException("Couldn't find a valid proto for craft product to load.");
        }

        var errors = VariableErrors().ToArray();
		if (errors.Length > 0) throw new ApplicationException(string.Join("\n", errors));
		List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)> variables = new(FixedCharacteristics);
        foreach ((ICharacteristicDefinition definition, int input) in Characteristics)
        {
            ICraftInput ivi = Craft.Inputs.ElementAt(input);
			if (!component.ConsumedInputs.TryGetValue(ivi, out var consumed))
			{
				throw new ApplicationException($"Craft Input $i{input + 1} has not been consumed; cannot resolve {definition.Name}.");
			}
			var value = ((IVariableInput)ivi).GetValueForVariable(definition, consumed.Data);
			if (value is null || !definition.IsValue(value))
			{
				throw new ApplicationException($"Craft Input $i{input + 1} did not supply a valid {definition.Name} value.");
			}
			variables.Add((definition, value));
        }

        ISolid material = DetermineOverrideMaterial(component);

        if (Quantity > 1 && proto.IsItemType<StackableGameItemComponentProto>())
        {
            IGameItem newItem = proto.CreateNew(null, Skin, Quantity, variables).First();
            newItem.RoomLayer = component.Parent.RoomLayer;
            Gameworld.Add(newItem);

            if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
            {
                newItem.Quality = referenceQuality;
            }

            if (material != null)
            {
                newItem.Material = material;
            }

            return new SimpleProductData(new[] { newItem });
        }

        List<IGameItem> items = proto.CreateNew(null, Skin, Quantity, variables).ToList();
        foreach (IGameItem item in items)
        {
            item.RoomLayer = component.Parent.RoomLayer;
            Gameworld.Add(item);

            if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
            {
                item.Quality = referenceQuality;
            }

            if (material != null)
            {
                item.Material = material;
            }

        }

        return new SimpleProductData(items);
    }

    public override string Name
    {
        get
        {
            StringBuilder sb = new();
            sb.Append(Quantity)
              .Append("x ");

            if (Skin is not null)
            {
                sb.Append((Skin.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription)
                    .ConcatIfNotEmpty($" [reskinned: #{Skin.Id:N0}]") ?? "an unspecified item".Colour(Telnet.Red));
            }
            else
            {
                sb.Append(Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ??
                          "an unspecified item".Colour(Telnet.Red));
            }

            foreach ((ICharacteristicDefinition definition, int input) in Characteristics)
            {
                sb.Append(" ").Append(definition?.Name ?? "missing characteristic").Append(" <- ");
                if (definition is not null && input >= 0 && Craft.Inputs.ElementAtOrDefault(input) is IVariableInput ivi && ivi.DeterminesVariable(definition))
                {
                    sb.Append(ivi.Name).Append($" ($i{input + 1})");
                }
                else
                {
                    sb.Append(" an invalid input".Colour(Telnet.Red));
                }
            }
			foreach (var (definition, value) in FixedCharacteristics)
			{
				sb.Append(" ").Append(definition?.Name ?? "missing characteristic").Append(" = ").Append(value?.Name ?? "missing value");
			}

            return sb.ToString();
        }
    }

    public override string HowSeen(IPerceiver voyeur)
    {
        StringBuilder sb = new();
        sb.Append(Quantity)
          .Append("x ");

        if (Skin is not null && voyeur is ICharacter ch && ch.IsAdministrator())
        {
            sb.Append((Skin.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription)
                      .ConcatIfNotEmpty($" [reskinned: #{Skin.Id.ToString("N0", voyeur)}]") ??
                      "an unspecified item".Colour(Telnet.Red));
        }
        else
        {
            sb.Append(Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ??
                      "an unspecified item".Colour(Telnet.Red));
        }

        foreach ((ICharacteristicDefinition definition, int input) in Characteristics)
        {
            sb.Append(" ").Append(definition?.Name ?? "missing characteristic").Append(" <- ");
            if (definition is not null && input >= 0 && Craft.Inputs.ElementAtOrDefault(input) is IVariableInput ivi && ivi.DeterminesVariable(definition))
            {
                sb.Append(ivi.Name).Append($" ($i{input + 1})");
            }
            else
            {
                sb.Append(" an invalid input".Colour(Telnet.Red));
            }
        }
		foreach (var (definition, value) in FixedCharacteristics)
		{
			sb.Append(" ").Append(definition?.Name ?? "missing characteristic").Append(" = ").Append(value?.Name ?? "missing value");
		}

        return sb.ToString();
    }
}
