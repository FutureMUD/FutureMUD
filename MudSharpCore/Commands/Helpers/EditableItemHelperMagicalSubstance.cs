using MudSharp.Effects.Concrete;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper MagicalSubstanceHelper { get; } = new()
	{
		ItemName = "Magical Substance", ItemNamePlural = "Magical Substances", CastToType = typeof(IMagicalSubstance),
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicalSubstance>>();
			if (item is IMagicalSubstance substance) actor.AddEffect(new BuilderEditingEffect<IMagicalSubstance>(actor) { EditingItem = substance });
		},
		GetEditableItemFunc = actor => actor.EffectsOfType<BuilderEditingEffect<IMagicalSubstance>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicalSubstances.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicalSubstances.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, text) => actor.Gameworld.MagicalSubstances.GetByIdOrName(text),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicalSubstance)item),
		EditableNewAction = (actor, input) => CreateSubstance(actor, input.SafeRemainingArgument, null),
		EditableCloneAction = (actor, input) =>
		{
			var source = actor.Gameworld.MagicalSubstances.GetByIdOrName(input.PopSpeech());
			if (source is not MagicalSubstance substance) { actor.Send("There is no such magical substance."); return; }
			CreateSubstance(actor, input.SafeRemainingArgument, substance);
		},
		GetListTableHeaderFunc = actor => ["Id", "Name", "Routes", "Ready"],
		GetListTableContentsFunc = (actor, items) => items.OfType<IMagicalSubstance>().Select(x => new List<string>
			{ x.Id.ToString("N0", actor), x.Name, x.Vectors.DescribeEnum(), (!x.ReadinessErrors.Any()).ToColouredString() }),
		CustomSearch = (items, _, _) => items,
		DefaultCommandHelp = @"Magical substances carry spell effects through liquids, gases and consumable items.

You can use the following options with this command:

	#3magic substance list#0 - lists magical substances
	#3magic substance edit new <name>#0 - creates a new magical substance
	#3magic substance edit <id|name>#0 - begins editing a magical substance
	#3magic substance clone <id|name> <new name>#0 - clones a magical substance
	#3magic substance show [<id|name>]#0 - shows a magical substance's settings and readiness
	#3magic substance set <setting>#0 - changes the substance you are editing
	#3magic substance close#0 - stops editing

Use #3magic substance set#0 while editing to see the available settings."
	};
	private static void CreateSubstance(ICharacter actor, string name, MagicalSubstance? source)
	{
		if (string.IsNullOrWhiteSpace(name) || name.Length > 200 || actor.Gameworld.MagicalSubstances.Any(x => x.Name.EqualTo(name)))
		{ actor.Send("Choose a unique name of 1 to 200 characters."); return; }
		var substance = new MagicalSubstance(name, actor.Gameworld, source);
		actor.Gameworld.Add(substance);
		actor.RemoveAllEffects<BuilderEditingEffect<IMagicalSubstance>>();
		actor.AddEffect(new BuilderEditingEffect<IMagicalSubstance>(actor) { EditingItem = substance });
		actor.Send($"You are now editing {name.ColourName()} (#{substance.Id.ToString("N0", actor)}).");
	}
}
