using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper ClockHelper { get; } = new()
	{
		ItemName = "Clock",
		ItemNamePlural = "Clocks",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IClock>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IClock>(actor) { EditingItem = (IClock)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IClock>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.Clocks.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Clocks.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Clocks.GetByIdOrNames(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IClock)item),
		CastToType = typeof(IClock),
		EditableNewAction = (actor, input) =>
		{
			var alias = input.PopSpeech();
			if (string.IsNullOrEmpty(alias))
			{
				actor.OutputHandler.Send("You must specify an alias for the clock.");
				return;
			}

			if (actor.Gameworld.Clocks.Any(x => x.Alias.EqualTo(alias)))
			{
				actor.OutputHandler.Send($"There is already a clock with the alias {alias.ColourValue()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for the clock.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			var clock = new Clock(actor.Gameworld, name, alias);
			actor.Gameworld.Add(clock);
			actor.RemoveAllEffects<BuilderEditingEffect<IClock>>();
			actor.AddEffect(new BuilderEditingEffect<IClock>(actor) { EditingItem = clock });
			actor.OutputHandler.Send($"You create clock #{clock.Id.ToStringN0(actor)} ({clock.Name.ColourName()}), which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which clock do you want to clone?");
				return;
			}

			var original = actor.Gameworld.Clocks.GetByIdOrNames(input.PopSpeech());
			if (original is null)
			{
				actor.OutputHandler.Send("There is no such clock to clone.");
				return;
			}

			var alias = input.PopSpeech();
			if (string.IsNullOrEmpty(alias))
			{
				actor.OutputHandler.Send("You must specify an alias for the cloned clock.");
				return;
			}

			if (actor.Gameworld.Clocks.Any(x => x.Alias.EqualTo(alias)))
			{
				actor.OutputHandler.Send($"There is already a clock with the alias {alias.ColourValue()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for the cloned clock.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			var clone = original.Clone(name, alias);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IClock>>();
			actor.AddEffect(new BuilderEditingEffect<IClock>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone clock {original.Name.ColourName()} to new clock #{clone.Id.ToStringN0(actor)} ({clone.Name.ColourName()}), which you are now editing.");
		},
		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Alias",
			"Name",
			"Primary TZ",
			"Current Time"
		},
		GetListTableContentsFunc = (character, items) => from clock in items.OfType<IClock>()
								select new List<string>
								{
									clock.Id.ToString("N0", character),
									clock.Alias,
									clock.Name,
									clock.PrimaryTimezone?.Alias ?? string.Empty,
									clock.DisplayTime(clock.CurrentTime, TimeDisplayTypes.Immortal)
								},
		DefaultCommandHelp = RoomBuilderModule.ClockHelpText,
		GetEditHeader = item => $"Clock #{item.Id:N0} ({item.Name})"
	};
}

