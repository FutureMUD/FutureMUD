using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper OutfitTemplateHelper { get; } = new()
	{
		ItemName = "Outfit Template",
		ItemNamePlural = "Outfit Templates",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IOutfitTemplate>>();
			if (item is null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IOutfitTemplate>(actor)
			{
				EditingItem = (IOutfitTemplate)item
			});
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IOutfitTemplate>>()
			     .FirstOrDefault()
			     ?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.OutfitTemplates
		                                    .Cast<IEditableItem>()
		                                    .ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.OutfitTemplates.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.OutfitTemplates.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IOutfitTemplate)item),
		CastToType = typeof(IOutfitTemplate),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give your new outfit template?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.OutfitTemplates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already an outfit template named {name.ColourName()}.");
				return;
			}

			var template = new TemplateOutfit(actor.Gameworld, name);
			actor.Gameworld.Add(template);
			actor.RemoveAllEffects<BuilderEditingEffect<IOutfitTemplate>>();
			actor.AddEffect(new BuilderEditingEffect<IOutfitTemplate>(actor) { EditingItem = template });
			actor.OutputHandler.Send(
				$"You create a new outfit template called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which outfit template do you want to clone?");
				return;
			}

			var source = actor.Gameworld.OutfitTemplates.GetByIdOrName(input.PopSpeech());
			if (source is null)
			{
				actor.OutputHandler.Send("There is no such outfit template.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give the cloned outfit template?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.OutfitTemplates.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already an outfit template named {name.ColourName()}.");
				return;
			}

			var clone = source.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IOutfitTemplate>>();
			actor.AddEffect(new BuilderEditingEffect<IOutfitTemplate>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone {source.Name.ColourName()} into a new outfit template called {name.ColourName()}, which you are now editing.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Items",
			"Exclusivity",
			"Warnings"
		},
		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IOutfitTemplate>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.Items.Count().ToString("N0", character),
			                                                  proto.Exclusivity.Describe(),
			                                                  proto.ValidationWarnings.Count().ToString("N0", character)
		                                                  },
		CustomSearch = (protos, keyword, gameworld) =>
		{
			if (keyword.Length > 1 && keyword[0] == '+')
			{
				keyword = keyword[1..];
				return protos
				       .OfType<IOutfitTemplate>()
				       .Where(x => x.Name.Contains(keyword, System.StringComparison.InvariantCultureIgnoreCase) ||
				                   x.Description.Contains(keyword, System.StringComparison.InvariantCultureIgnoreCase) ||
				                   x.Items.Any(y => y.TemplateKey.Contains(keyword, System.StringComparison.InvariantCultureIgnoreCase)))
				       .Cast<IEditableItem>()
				       .ToList();
			}

			return protos
			       .Where(x => x.Name.Contains(keyword, System.StringComparison.InvariantCultureIgnoreCase))
			       .ToList();
		},
		GetEditHeader = item => $"Outfit Template #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = ItemBuilderModule.OutfitTemplateHelp
	};

	public static EditableItemHelper CommoditySpoilageRuleHelper { get; } = new()
	{
		ItemName = "Commodity Spoilage Rule",
		ItemNamePlural = "Commodity Spoilage Rules",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<ICommoditySpoilageRule>>();
			if (item is null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<ICommoditySpoilageRule>(actor)
			{
				EditingItem = (ICommoditySpoilageRule)item
			});
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<ICommoditySpoilageRule>>()
			     .FirstOrDefault()
			     ?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.CommoditySpoilageRules
		                                    .Cast<IEditableItem>()
		                                    .ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.CommoditySpoilageRules.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.CommoditySpoilageRules.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((ICommoditySpoilageRule)item),
		CastToType = typeof(ICommoditySpoilageRule),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give your new commodity spoilage rule?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.CommoditySpoilageRules.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a commodity spoilage rule named {name.ColourName()}.");
				return;
			}

			var rule = new CommoditySpoilageRule(actor.Gameworld, name);
			actor.Gameworld.Add(rule);
			actor.RemoveAllEffects<BuilderEditingEffect<ICommoditySpoilageRule>>();
			actor.AddEffect(new BuilderEditingEffect<ICommoditySpoilageRule>(actor) { EditingItem = rule });
			actor.OutputHandler.Send(
				$"You create a new commodity spoilage rule called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which commodity spoilage rule do you want to clone?");
				return;
			}

			var source = actor.Gameworld.CommoditySpoilageRules.GetByIdOrName(input.PopSpeech());
			if (source is null)
			{
				actor.OutputHandler.Send("There is no such commodity spoilage rule.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give the cloned commodity spoilage rule?");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.CommoditySpoilageRules.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a commodity spoilage rule named {name.ColourName()}.");
				return;
			}

			var clone = source.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<ICommoditySpoilageRule>>();
			actor.AddEffect(new BuilderEditingEffect<ICommoditySpoilageRule>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone {source.Name.ColourName()} into a new commodity spoilage rule called {name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Enabled",
			"Priority",
			"Match",
			"Output",
			"Time"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<ICommoditySpoilageRule>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.Enabled.ToString(),
			                                                  proto.Priority.ToString("N0", character),
			                                                  CommoditySpoilageMatchSummary(proto),
			                                                  CommoditySpoilageOutputSummary(proto),
			                                                  proto.SecondsUntilSpoiled.Describe(character)
		                                                  },

		CustomSearch = (protos, keyword, gameworld) =>
		{
			if (keyword.EqualTo("enabled"))
			{
				return protos
				       .OfType<ICommoditySpoilageRule>()
				       .Where(x => x.Enabled)
				       .Cast<IEditableItem>()
				       .ToList();
			}

			if (keyword.EqualTo("disabled"))
			{
				return protos
				       .OfType<ICommoditySpoilageRule>()
				       .Where(x => !x.Enabled)
				       .Cast<IEditableItem>()
				       .ToList();
			}

			return protos
			       .Where(x => x.Name.Contains(keyword, System.StringComparison.InvariantCultureIgnoreCase))
			       .ToList();
		},
		GetEditHeader = item => $"Commodity Spoilage Rule #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = ItemBuilderModule.CommoditySpoilageHelp
	};

	private static string CommoditySpoilageMatchSummary(ICommoditySpoilageRule rule)
	{
		var match = rule.Material is not null
			? $"material {rule.Material.Name}"
			: rule.MaterialTag is not null
				? $"material tag {rule.MaterialTag.FullName}"
				: "invalid";

		return rule.CommodityTag is null
			? match
			: $"{match}; pile tag {rule.CommodityTag.FullName}";
	}

	private static string CommoditySpoilageOutputSummary(ICommoditySpoilageRule rule)
	{
		return rule.ResultCommodityTag is null
			? rule.ResultMaterial.Name
			: $"{rule.ResultMaterial.Name}; pile tag {rule.ResultCommodityTag.FullName}";
	}
}
