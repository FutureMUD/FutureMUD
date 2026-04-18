using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Health;
using MudSharp.Health.Bloodtypes;
using MudSharp.Health.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
    public static EditableItemHelper BloodtypeAntigenHelper { get; } = new()
    {
        ItemName = "Blood Antigen",
        ItemNamePlural = "Blood Antigens",
        SetEditableItemAction = (actor, item) =>
        {
            actor.RemoveAllEffects<BuilderEditingEffect<IBloodtypeAntigen>>();
            if (item == null)
            {
                return;
            }

            actor.AddEffect(new BuilderEditingEffect<IBloodtypeAntigen>(actor) { EditingItem = (IBloodtypeAntigen)item });
        },
        GetEditableItemFunc = actor =>
            actor.CombinedEffectsOfType<BuilderEditingEffect<IBloodtypeAntigen>>().FirstOrDefault()?.EditingItem,
        GetAllEditableItems = actor => actor.Gameworld.BloodtypeAntigens.ToList(),
        GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.BloodtypeAntigens.Get(id),
        GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.BloodtypeAntigens.GetByIdOrName(input),
        AddItemToGameWorldAction = item => item.Gameworld.Add((IBloodtypeAntigen)item),
        CastToType = typeof(IBloodtypeAntigen),
        EditableNewAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a name for the blood antigen.");
                return;
            }

            string name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.BloodtypeAntigens.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a blood antigen called {name.ColourValue()}.");
                return;
            }

            BloodtypeAntigen antigen = new(actor.Gameworld, name);
            actor.Gameworld.Add(antigen);
            actor.RemoveAllEffects<BuilderEditingEffect<IBloodtypeAntigen>>();
            actor.AddEffect(new BuilderEditingEffect<IBloodtypeAntigen>(actor) { EditingItem = antigen });
            actor.OutputHandler.Send($"You create blood antigen #{antigen.Id.ToStringN0(actor)} ({antigen.Name.ColourName()}), which you are now editing.");
        },
        EditableCloneAction = null,
        GetListTableHeaderFunc = character => new List<string>
        {
            "Id",
            "Name"
        },
        GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IBloodtypeAntigen>()
                                                          select new List<string>
                                                          {
                                                              proto.Id.ToString("N0", character),
                                                              proto.Name
                                                          },
        CustomSearch = (protos, keyword, gameworld) => protos,
        DefaultCommandHelp = BuilderModule.BloodAntigenHelpText,
        GetEditHeader = item => $"Blood Antigen #{item.Id:N0} ({item.Name})"
    };

    public static EditableItemHelper BloodtypeHelper { get; } = new()
    {
        ItemName = "Blood Type",
        ItemNamePlural = "Blood Types",
        SetEditableItemAction = (actor, item) =>
        {
            actor.RemoveAllEffects<BuilderEditingEffect<IBloodtype>>();
            if (item == null)
            {
                return;
            }

            actor.AddEffect(new BuilderEditingEffect<IBloodtype>(actor) { EditingItem = (IBloodtype)item });
        },
        GetEditableItemFunc = actor =>
            actor.CombinedEffectsOfType<BuilderEditingEffect<IBloodtype>>().FirstOrDefault()?.EditingItem,
        GetAllEditableItems = actor => actor.Gameworld.Bloodtypes.ToList(),
        GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Bloodtypes.Get(id),
        GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Bloodtypes.GetByIdOrName(input),
        AddItemToGameWorldAction = item => item.Gameworld.Add((IBloodtype)item),
        CastToType = typeof(IBloodtype),
        EditableNewAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a name for the blood type.");
                return;
            }

            string name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.Bloodtypes.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a blood type called {name.ColourValue()}.");
                return;
            }

            Bloodtype type = new(actor.Gameworld, name);
            actor.Gameworld.Add(type);
            actor.RemoveAllEffects<BuilderEditingEffect<IBloodtype>>();
            actor.AddEffect(new BuilderEditingEffect<IBloodtype>(actor) { EditingItem = type });
            actor.OutputHandler.Send($"You create blood type #{type.Id.ToStringN0(actor)} ({type.Name.ColourName()}), which you are now editing.");
        },
        EditableCloneAction = null,
        GetListTableHeaderFunc = character => new List<string>
        {
            "Id",
            "Name",
            "Antigens"
        },
        GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IBloodtype>()
                                                          select new List<string>
                                                          {
                                                              proto.Id.ToString("N0", character),
                                                              proto.Name,
                                                              proto.Antigens.Select(x => x.Name).ListToCommaSeparatedValues(", ")
                                                          },
        CustomSearch = (protos, keyword, gameworld) => protos,
        DefaultCommandHelp = BuilderModule.BloodTypeHelpText,
        GetEditHeader = item => $"Blood Type #{item.Id:N0} ({item.Name})"
    };

    public static EditableItemHelper PopulationBloodModelHelper { get; } = new()
    {
        ItemName = "Population Blood Model",
        ItemNamePlural = "Population Blood Models",
        SetEditableItemAction = (actor, item) =>
        {
            actor.RemoveAllEffects<BuilderEditingEffect<IPopulationBloodModel>>();
            if (item == null)
            {
                return;
            }

            actor.AddEffect(new BuilderEditingEffect<IPopulationBloodModel>(actor) { EditingItem = (IPopulationBloodModel)item });
        },
        GetEditableItemFunc = actor =>
            actor.CombinedEffectsOfType<BuilderEditingEffect<IPopulationBloodModel>>().FirstOrDefault()?.EditingItem,
        GetAllEditableItems = actor => actor.Gameworld.PopulationBloodModels.ToList(),
        GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.PopulationBloodModels.Get(id),
        GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.PopulationBloodModels.GetByIdOrName(input),
        AddItemToGameWorldAction = item => item.Gameworld.Add((IPopulationBloodModel)item),
        CastToType = typeof(IPopulationBloodModel),
        EditableNewAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a name for the population blood model.");
                return;
            }

            string name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.PopulationBloodModels.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a population blood model called {name.ColourValue()}.");
                return;
            }

            PopulationBloodModel model = new(actor.Gameworld, name);
            actor.Gameworld.Add(model);
            actor.RemoveAllEffects<BuilderEditingEffect<IPopulationBloodModel>>();
            actor.AddEffect(new BuilderEditingEffect<IPopulationBloodModel>(actor) { EditingItem = model });
            actor.OutputHandler.Send($"You create population blood model #{model.Id.ToStringN0(actor)} ({model.Name.ColourName()}), which you are now editing.");
        },
        EditableCloneAction = null,
        GetListTableHeaderFunc = character => new List<string>
        {
            "Id",
            "Name",
            "Blood Model",
            "Blood Types"
        },
        GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IPopulationBloodModel>()
                                                          let weight = proto.BloodTypes.Sum(x => x.Weight)
                                                          select new List<string>
                                                          {
                                                              proto.Id.ToString("N0", character),
                                                              proto.Name,
                                                              proto.BloodModel?.Name ?? "None",
                                                              proto.BloodTypes.OrderByDescending(x => x.Weight).Select(x => $"{x.Bloodtype.Name} ({(x.Weight / weight):P2})").ListToCommaSeparatedValues(", ")
                                                          },
        CustomSearch = (protos, keyword, gameworld) => protos,
        DefaultCommandHelp = BuilderModule.PopulationBloodModelHelpText,
        GetEditHeader = item => $"Population Blood Model #{item.Id:N0} ({item.Name})"
    };

    public static EditableItemHelper HealthStrategyHelper { get; } = new()
    {
        ItemName = "Health Strategy",
        ItemNamePlural = "Health Strategies",
        SetEditableItemAction = (actor, item) =>
        {
            actor.RemoveAllEffects<BuilderEditingEffect<IHealthStrategy>>();
            if (item == null)
            {
                return;
            }

            actor.AddEffect(new BuilderEditingEffect<IHealthStrategy>(actor) { EditingItem = (IHealthStrategy)item });
        },
        GetEditableItemFunc = actor =>
            actor.CombinedEffectsOfType<BuilderEditingEffect<IHealthStrategy>>().FirstOrDefault()?.EditingItem,
        GetAllEditableItems = actor => actor.Gameworld.HealthStrategies.ToList(),
        GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.HealthStrategies.Get(id),
        GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.HealthStrategies.GetByIdOrName(input),
        AddItemToGameWorldAction = item => item.Gameworld.Add((IHealthStrategy)item),
        CastToType = typeof(IHealthStrategy),
        EditableNewAction = (actor, input) =>
        {
            if (input.CountRemainingArguments() < 2)
            {
                actor.OutputHandler.Send("You must specify a health strategy type and name.");
                return;
            }

            string type = input.PopSpeech();
            if (BaseHealthStrategy.Types.All(x => !x.EqualTo(type)))
            {
                actor.OutputHandler.Send($"The text {type.ColourCommand()} is not a valid health strategy type. See {"healthstrategy types".MXPSend()} for a list of options.");
                return;
            }

            type = BaseHealthStrategy.Types.First(x => x.EqualTo(type));
            string name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.HealthStrategies.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a health strategy called {name.ColourName()}. Names must be unique.");
                return;
            }

            IHealthStrategy strategy = BaseHealthStrategy.LoadStrategy(actor.Gameworld, type, name);
            actor.Gameworld.Add(strategy);
            actor.RemoveAllEffects<BuilderEditingEffect<IHealthStrategy>>();
            actor.AddEffect(new BuilderEditingEffect<IHealthStrategy>(actor) { EditingItem = strategy });
            actor.OutputHandler.Send($"You create a new {type.ColourValue()} health strategy called {name.ColourName()}, which you are now editing.");
        },
        EditableCloneAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("Which health strategy would you like to clone?");
                return;
            }

            IHealthStrategy template = actor.Gameworld.HealthStrategies.GetByIdOrName(input.PopSpeech());
            if (template == null)
            {
                actor.OutputHandler.Send("There is no such health strategy to clone.");
                return;
            }

            if (input.IsFinished)
            {
                actor.OutputHandler.Send("What name do you want to give to your cloned health strategy?");
                return;
            }

            string name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.HealthStrategies.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a health strategy called {name.ColourName()}. Names must be unique.");
                return;
            }

            IHealthStrategy clone = template.Clone(name);
            actor.Gameworld.Add(clone);
            actor.RemoveAllEffects<BuilderEditingEffect<IHealthStrategy>>();
            actor.AddEffect(new BuilderEditingEffect<IHealthStrategy>(actor) { EditingItem = clone });
            actor.OutputHandler.Send($"You clone the health strategy {template.Name.ColourName()} into a new strategy called {name.ColourName()}, which you are now editing.");
        },
        GetListTableHeaderFunc = character => new List<string>
        {
            "Id",
            "Name",
            "Type",
            "Owner"
        },
        GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IHealthStrategy>()
                                                          select new List<string>
                                                          {
                                                              proto.Id.ToString("N0", character),
                                                              proto.Name,
                                                              proto.HealthStrategyType,
                                                              proto.OwnerType.DescribeEnum()
                                                          },
        CustomSearch = (protos, keyword, gameworld) =>
        {
            if (keyword.Length > 1 && keyword[0] == '+')
            {
                keyword = keyword[1..];
                return protos
                       .OfType<IHealthStrategy>()
                       .Where(x =>
                           x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                           x.HealthStrategyType.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                       .Cast<IEditableItem>()
                       .ToList();
            }

            if (keyword.Length > 1 && keyword[0] == '-')
            {
                keyword = keyword[1..];
                return protos
                       .OfType<IHealthStrategy>()
                       .Where(x =>
                           !x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) &&
                           !x.HealthStrategyType.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
                       .Cast<IEditableItem>()
                       .ToList();
            }

            if (BaseHealthStrategy.Types.Any(x => x.EqualTo(keyword)))
            {
                return protos
                       .OfType<IHealthStrategy>()
                       .Where(x => x.HealthStrategyType.EqualTo(keyword))
                       .Cast<IEditableItem>()
                       .ToList();
            }

            if (Enum.TryParse<HealthStrategyOwnerType>(keyword, true, out HealthStrategyOwnerType ownerType))
            {
                return protos
                       .OfType<IHealthStrategy>()
                       .Where(x => x.OwnerType == ownerType)
                       .Cast<IEditableItem>()
                       .ToList();
            }

            HealthStrategyOwnerType describedOwnerType = Enum.GetValues<HealthStrategyOwnerType>()
                                         .FirstOrDefault(x => x.DescribeEnum().EqualTo(keyword));
            if (describedOwnerType.DescribeEnum().EqualTo(keyword))
            {
                return protos
                       .OfType<IHealthStrategy>()
                       .Where(x => x.OwnerType == describedOwnerType)
                       .Cast<IEditableItem>()
                       .ToList();
            }

            return protos;
        },
        DefaultCommandHelp = BuilderModule.HealthStrategyHelpText,
        GetEditHeader = item => $"Health Strategy #{item.Id:N0} ({item.Name})"
    };
}

