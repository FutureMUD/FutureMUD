using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.RPG.AIStorytellers;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper AIStorytellerHelper { get; } = new()
	{
		ItemName = "AI Storyteller",
		ItemNamePlural = "AI Storytellers",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAIStoryteller>>();
			if (item is null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IAIStoryteller>(actor) { EditingItem = (IAIStoryteller)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAIStoryteller>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AIStorytellers.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AIStorytellers.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AIStorytellers.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAIStoryteller)item),
		CastToType = typeof(IAIStoryteller),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new AI storyteller.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.AIStorytellers.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already an AI storyteller called {name.ColourName()}. Names must be unique.");
				return;
			}

			var storyteller = new AIStoryteller(actor.Gameworld, name);
			actor.Gameworld.Add(storyteller);
			actor.RemoveAllEffects<BuilderEditingEffect<IAIStoryteller>>();
			actor.AddEffect(new BuilderEditingEffect<IAIStoryteller>(actor) { EditingItem = storyteller });
			actor.OutputHandler.Send(
				$"You create a new AI storyteller called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning is not yet supported for AI storytellers.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Model",
			"Paused",
			"Room",
			"5m",
			"10m",
			"30m",
			"1h"
		},
		GetListTableContentsFunc = (character, items) => from item in items.OfType<IAIStoryteller>()
		                                                 let concrete = item as AIStoryteller
		                                                 select new List<string>
		                                                 {
			                                                 item.Id.ToString("N0", character),
			                                                 item.Name,
			                                                 concrete?.Model ?? string.Empty,
			                                                 concrete?.IsPaused.ToColouredString() ?? string.Empty,
			                                                 concrete?.SubscribeToRoomEvents.ToColouredString() ??
			                                                 string.Empty,
			                                                 concrete?.SubscribeTo5mHeartbeat.ToColouredString() ??
			                                                 string.Empty,
			                                                 concrete?.SubscribeTo10mHeartbeat.ToColouredString() ??
			                                                 string.Empty,
			                                                 concrete?.SubscribeTo30mHeartbeat.ToColouredString() ??
			                                                 string.Empty,
			                                                 concrete?.SubscribeToHourHeartbeat.ToColouredString() ??
			                                                 string.Empty
		                                                 },
		CustomSearch = (items, keyword, gameworld) =>
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return items;
			}

			return items.OfType<IAIStoryteller>()
				.Where(x => x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase))
				.Cast<IEditableItem>()
				.ToList();
		},
		GetEditHeader = item => $"AI Storyteller #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = AIStorytellerModule.AIStorytellerHelp
	};

	public static EditableItemHelper AIStorytellerReferenceDocumentHelper { get; } = new()
	{
		ItemName = "AI Storyteller Reference Document",
		ItemNamePlural = "AI Storyteller Reference Documents",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IAIStorytellerReferenceDocument>>();
			if (item is null)
			{
				return;
			}

			actor.AddEffect(
				new BuilderEditingEffect<IAIStorytellerReferenceDocument>(actor)
					{ EditingItem = (IAIStorytellerReferenceDocument)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IAIStorytellerReferenceDocument>>().FirstOrDefault()
				?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.AIStorytellerReferenceDocuments.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.AIStorytellerReferenceDocuments.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.AIStorytellerReferenceDocuments.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IAIStorytellerReferenceDocument)item),
		CastToType = typeof(IAIStorytellerReferenceDocument),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new reference document.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.AIStorytellerReferenceDocuments.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a reference document called {name.ColourName()}. Names must be unique.");
				return;
			}

			var document = new AIStorytellerReferenceDocument(actor.Gameworld, name, "A reference document",
				"General", "General", string.Empty, string.Empty);
			actor.Gameworld.Add(document);
			actor.RemoveAllEffects<BuilderEditingEffect<IAIStorytellerReferenceDocument>>();
			actor.AddEffect(
				new BuilderEditingEffect<IAIStorytellerReferenceDocument>(actor) { EditingItem = document });
			actor.OutputHandler.Send(
				$"You create a new AI storyteller reference document called {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Cloning is not supported for AI storyteller reference documents.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Folder",
			"Type",
			"Keywords",
			"Visibility"
		},
		GetListTableContentsFunc = (character, items) =>
		{
			return from item in items.OfType<IAIStorytellerReferenceDocument>()
			       let concrete = item as AIStorytellerReferenceDocument
			       select new List<string>
			       {
				       item.Id.ToString("N0", character),
				       item.Name,
				       concrete?.FolderName ?? string.Empty,
				       concrete?.DocumentType ?? string.Empty,
				       concrete?.Keywords ?? string.Empty,
				       concrete?.RestrictedStorytellerIds.Any() == true ? "Restricted" : "Global"
			       };
		},
		CustomSearch = (items, keyword, gameworld) =>
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return items;
			}

			var parts = keyword.Split(':', 2, StringSplitOptions.TrimEntries);
			if (parts.Length == 2)
			{
				var selector = parts[0].ToLowerInvariant();
				var value = parts[1];
				switch (selector)
				{
					case "folder":
						return items.OfType<IAIStorytellerReferenceDocument>()
							.Where(x => (x as AIStorytellerReferenceDocument)?.FolderName.Contains(value,
								StringComparison.InvariantCultureIgnoreCase) == true)
							.Cast<IEditableItem>()
							.ToList();
					case "type":
						return items.OfType<IAIStorytellerReferenceDocument>()
							.Where(x => (x as AIStorytellerReferenceDocument)?.DocumentType.Contains(value,
								StringComparison.InvariantCultureIgnoreCase) == true)
							.Cast<IEditableItem>()
							.ToList();
					case "keyword":
					case "keywords":
						return items.OfType<IAIStorytellerReferenceDocument>()
							.Where(x => (x as AIStorytellerReferenceDocument)?.Keywords.Contains(value,
								StringComparison.InvariantCultureIgnoreCase) == true)
							.Cast<IEditableItem>()
							.ToList();
					case "storyteller":
					{
						var storyteller = gameworld.AIStorytellers.GetByIdOrName(value);
						if (storyteller is null)
						{
							return [];
						}

						return items.OfType<IAIStorytellerReferenceDocument>()
							.Where(x => x.IsVisibleTo(storyteller))
							.Cast<IEditableItem>()
							.ToList();
					}
				}
			}

			return items.OfType<IAIStorytellerReferenceDocument>()
				.Where(x => x.ReturnForSearch(keyword))
				.Cast<IEditableItem>()
				.ToList();
		},
		GetEditHeader = item => $"AI Storyteller Reference Document #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = AIStorytellerModule.AIStorytellerReferenceHelp
	};
}
