using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Health.Bloodtypes;

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

            var name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.BloodtypeAntigens.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a blood antigen called {name.ColourValue()}.");
                return;
            }

            var antigen = new BloodtypeAntigen(actor.Gameworld, name);
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

            var name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.Bloodtypes.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a blood type called {name.ColourValue()}.");
                return;
            }

            var type = new Bloodtype(actor.Gameworld, name);
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

            var name = input.SafeRemainingArgument.TitleCase();
            if (actor.Gameworld.PopulationBloodModels.Any(x => x.Name.EqualTo(name)))
            {
                actor.OutputHandler.Send($"There is already a population blood model called {name.ColourValue()}.");
                return;
            }

            var model = new PopulationBloodModel(actor.Gameworld, name);
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
}

