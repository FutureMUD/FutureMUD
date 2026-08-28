#nullable enable

using MudSharp.Database;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.NPC.Templates;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
	public static EditableItemHelper NPCSkillPackageHelper { get; } = new()
	{
		ItemName = "NPC Skill Package",
		ItemNamePlural = "NPC Skill Packages",
		CommandName = "npcskillpackage",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<INPCSkillPackage>>();
			if (item is not null)
			{
				actor.AddEffect(new BuilderEditingEffect<INPCSkillPackage>(actor)
				{
					EditingItem = (INPCSkillPackage)item
				});
			}
		},
		GetEditableItemFunc = actor => actor.CombinedEffectsOfType<BuilderEditingEffect<INPCSkillPackage>>()
			.FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.NpcSkillPackages.OrderBy(x => x.Id).ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.NpcSkillPackages.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.NpcSkillPackages.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((INPCSkillPackage)item),
		CastToType = typeof(INPCSkillPackage),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give the new NPC skill package?");
				return;
			}

			if (!NPCSkillPackage.TryNormaliseName(input.SafeRemainingArgument, out var name, out var error))
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (actor.Gameworld.NpcSkillPackages.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already an NPC skill package named {name.ColourName()}.");
				return;
			}

			var package = new NPCSkillPackage(actor.Gameworld, name);
			actor.Gameworld.Add(package);
			actor.RemoveAllEffects<BuilderEditingEffect<INPCSkillPackage>>();
			actor.AddEffect(new BuilderEditingEffect<INPCSkillPackage>(actor) { EditingItem = package });
			actor.OutputHandler.Send($"You create {name.ColourName()} and begin editing it.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.CountRemainingArguments() < 2)
			{
				actor.OutputHandler.Send("You must specify the source package and a new name.");
				return;
			}

			var source = actor.Gameworld.NpcSkillPackages.GetByIdOrName(input.PopSpeech());
			if (source is null)
			{
				actor.OutputHandler.Send("There is no such NPC skill package.");
				return;
			}

			if (!NPCSkillPackage.TryNormaliseName(input.SafeRemainingArgument, out var name, out var error))
			{
				actor.OutputHandler.Send(error);
				return;
			}

			if (actor.Gameworld.NpcSkillPackages.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already an NPC skill package named {name.ColourName()}.");
				return;
			}

			var clone = source.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<INPCSkillPackage>>();
			actor.AddEffect(new BuilderEditingEffect<INPCSkillPackage>(actor) { EditingItem = clone });
			actor.OutputHandler.Send($"You clone {source.Name.ColourName()} as {name.ColourName()} and begin editing it.");
		},
		EditableDeleteAction = (actor, item) =>
		{
			var package = (INPCSkillPackage)item;
			foreach (var race in actor.Gameworld.Races.Where(x => x.DirectDefaultSkillPackages.Contains(package)))
			{
				race.RemoveDefaultSkillPackage(package);
			}

			if (package is ISaveable saveable)
			{
				actor.Gameworld.SaveManager.Abort(saveable);
			}
			using (new FMDB())
			{
				var dbitem = FMDB.Context.NpcSkillPackages.Find(package.Id);
				if (dbitem is not null)
				{
					FMDB.Context.NpcSkillPackages.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}

			actor.Gameworld.Destroy(package);
			actor.RemoveAllEffects<BuilderEditingEffect<INPCSkillPackage>>();
			actor.OutputHandler.Send($"You permanently delete {package.Name.ColourName()}.");
		},
		GetListTableHeaderFunc = _ => ["Id", "Name", "Skills"],
		GetListTableContentsFunc = (actor, items) => items.OfType<INPCSkillPackage>()
			.Select(x => new List<string>
			{
				x.Id.ToString("N0", actor), x.Name, x.Skills.Count.ToString("N0", actor)
			}),
		CustomSearch = (items, keyword, _) => items.OfType<INPCSkillPackage>()
			.Where(x => x.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
			            x.Skills.Any(y => y.Skill.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)))
			.Cast<IEditableItem>()
			.ToList(),
		DefaultCommandHelp = NPCBuilderModule.NPCSkillPackageHelp,
		GetEditHeader = item => $"NPC Skill Package #{item.Id:N0} ({item.Name})"
	};
}
