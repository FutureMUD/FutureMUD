using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper ManualCombatCommandHelper { get; } = new()
	{
		ItemName = "Manual Combat Command",
		ItemNamePlural = "Manual Combat Commands",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IManualCombatCommand>>();
			if (item is null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IManualCombatCommand>(actor) { EditingItem = (IManualCombatCommand)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IManualCombatCommand>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.ManualCombatCommands.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.ManualCombatCommands.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.ManualCombatCommands.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IManualCombatCommand)item),
		CastToType = typeof(IManualCombatCommand),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the primary verb for your new manual combat command.");
				return;
			}

			string verb = input.PopSpeech().ToLowerInvariant();
			if (!ManualCombatCommandRegistry.IsValidCommandWord(verb))
			{
				actor.OutputHandler.Send("Manual combat verbs must be a single alphabetic command word.");
				return;
			}

			if (ManualCombatCommandRegistry.HasReservedCommandCollision(verb) ||
			    actor.Gameworld.ManualCombatCommands.Any(x => x.CommandWords.Any(y => y.EqualTo(verb))))
			{
				actor.OutputHandler.Send($"The verb {verb.ColourCommand()} is already used by another command.");
				return;
			}

			string name = input.IsFinished ? verb.TitleCase() : input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.ManualCombatCommands.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a manual combat command called {name.ColourName()}.");
				return;
			}

			var item = new ManualCombatCommand(actor.Gameworld, name, verb);
			actor.Gameworld.Add(item);
			actor.RemoveAllEffects<BuilderEditingEffect<IManualCombatCommand>>();
			actor.AddEffect(new BuilderEditingEffect<IManualCombatCommand>(actor) { EditingItem = item });
			actor.OutputHandler.Send($"You create a new manual combat command called {name.ColourName()} with primary verb {verb.ColourCommand()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which manual combat command do you want to clone?");
				return;
			}

			IManualCombatCommand parent = actor.Gameworld.ManualCombatCommands.GetByIdOrName(input.PopSpeech());
			if (parent is null)
			{
				actor.OutputHandler.Send("There is no such manual combat command.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify the new primary verb for the cloned manual combat command.");
				return;
			}

			string verb = input.PopSpeech().ToLowerInvariant();
			if (!ManualCombatCommandRegistry.IsValidCommandWord(verb))
			{
				actor.OutputHandler.Send("Manual combat verbs must be a single alphabetic command word.");
				return;
			}

			if (ManualCombatCommandRegistry.HasReservedCommandCollision(verb) ||
			    actor.Gameworld.ManualCombatCommands.Any(x => x.CommandWords.Any(y => y.EqualTo(verb))))
			{
				actor.OutputHandler.Send($"The verb {verb.ColourCommand()} is already used by another command.");
				return;
			}

			string name = input.IsFinished ? verb.TitleCase() : input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.ManualCombatCommands.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a manual combat command called {name.ColourName()}.");
				return;
			}

			IManualCombatCommand clone = parent.Clone(name);
			actor.Gameworld.Add(clone);
			clone.BuildingCommand(actor, new StringStack($"verb {verb}"));
			actor.RemoveAllEffects<BuilderEditingEffect<IManualCombatCommand>>();
			actor.AddEffect(new BuilderEditingEffect<IManualCombatCommand>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone {parent.Name.ColourName()} to {clone.Name.ColourName()}, which you are now editing.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Verb",
			"Aliases",
			"Kind",
			"Target",
			"Player",
			"NPC",
			"AI"
		},
		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IManualCombatCommand>()
		                                                  select new List<string>
		                                                  {
			                                                  proto.Id.ToString("N0", character),
			                                                  proto.Name,
			                                                  proto.PrimaryVerb,
			                                                  proto.CommandWords.Where(x => !x.EqualTo(proto.PrimaryVerb)).ListToCommaSeparatedValues(", "),
			                                                  proto.ActionKind.DescribeEnum(),
			                                                  proto.ActionKind == ManualCombatActionKind.WeaponAttack
				                                                  ? proto.WeaponAttack?.Name ?? ""
				                                                  : proto.AuxiliaryAction?.Name ?? "",
			                                                  proto.PlayerUsable.ToColouredString(),
			                                                  proto.NpcUsable.ToColouredString(),
			                                                  proto.DefaultAiWeightMultiplier.ToString("N2", character)
		                                                  },
		CustomSearch = (protos, keyword, gameworld) =>
		{
			return protos.Cast<IManualCombatCommand>()
			             .Where(x =>
				             x.Name.Contains(keyword, System.StringComparison.InvariantCultureIgnoreCase) ||
				             x.CommandWords.Any(y => y.Contains(keyword, System.StringComparison.InvariantCultureIgnoreCase)))
			             .Cast<IEditableItem>()
			             .ToList();
		},
		GetEditHeader = item => $"Manual Combat Command #{item.Id:N0} ({item.Name})",
		DefaultCommandHelp = CombatBuilderModule.ManualCombatCommandHelp
	};
}
