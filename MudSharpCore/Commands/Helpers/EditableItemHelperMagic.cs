using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Powers;
using MudSharp.Magic.Resources;

namespace MudSharp.Commands.Helpers;
public partial class EditableItemHelper
{
	public static EditableItemHelper MagicSpellHelper { get; } = new()
	{
		ItemName = "Magic Spell",
		ItemNamePlural = "Magic Spells",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = (IMagicSpell)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicSpell>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicSpells.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicSpells.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicSpells.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicSpell)item),
		CastToType = typeof(IMagicSpell),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new magic spell.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a magic school for your spell to belong to.");
				return;
			}

			var school = actor.Gameworld.MagicSchools.GetByIdOrName(input.SafeRemainingArgument);
			if (school == null)
			{
				actor.OutputHandler.Send("There is no such magic school.");
				return;
			}

			if (actor.Gameworld.MagicSpells.Any(x => x.School == school && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a spell in the {school.Name.Colour(school.PowerListColour)} school of magic with that name. Names must be unique per school.");
				return;
			}

			var spell = new MagicSpell(name, school);
			actor.Gameworld.Add(spell);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = spell });
			actor.OutputHandler.Send(
				$"You create a new magic spell in the {school.Name.Colour(school.PowerListColour)} school of magic named {name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic spell do you want to clone?");
				return;
			}

			var spell = actor.Gameworld.MagicSpells.GetByIdOrName(input.PopSpeech());
			if (spell == null)
			{
				actor.OutputHandler.Send("There is no such magic spell.");
				return;
			}

			var spells = actor.Gameworld.MagicSpells.Where(x => x.Name.EqualTo(spell.Name)).ToList();
			if (!long.TryParse(input.Last, out _) && spells.Count > 1)
			{
				actor.OutputHandler.Send(
					$"The spell name you specified is ambiguous. For the sake of clarity, please use the ID instead from the following options:{spells.Select(x => $"#{x.Id.ToString("N0", actor)} - {x.Name.Colour(x.School.PowerListColour)} from {x.School.Name.Colour(x.School.PowerListColour)}").ListToLines(true)}");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned spell.");
				return;
			}

			var name = input.SafeRemainingArgument;
			if (actor.Gameworld.MagicSpells.Where(x => x.School == spell.School).Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a spell in the {spell.School.Name.Colour(spell.School.PowerListColour)} school of magic with that name. Names must be unique per school.");
				return;
			}

			var clone = new MagicSpell((MagicSpell)spell, name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSpell>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic spell {spell.Name.Colour(spell.School.PowerListColour)} to a new spell called {clone.Name.Colour(clone.School.PowerListColour)}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Blurb",
			"School"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicSpell>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.Blurb,
															  proto.School.Name
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic spells.

The core syntax is as follows:

	#3magic spell list#0 - shows all magic spells
	#3magic spell edit new <name> <school>#0 - creates a new magic spell
	#3magic spell clone <old> <new>#0 - clones an existing magic spell
	#3magic spell edit <which>#0 - begins editing a magic spell
	#3magic spell close#0 - closes an editing magic spell
	#3magic spell show <which>#0 - shows builder information about a spell
	#3magic spell show#0 - shows builder information about the currently edited spell
	#3magic spell edit#0 - an alias for magic spell show (with no args)
	#3magic spell set name <name>#0 - renames this spell
	#3magic spell set blurb <text>#0 - the summary text that appears in the SPELLS output
	#3magic spell set description#0 - drops you into an editor for a more detailed description
	#3magic spell set school <school>#0 - changes the school of this spell
	#3magic spell set prog <prog>#0 - sets the prog that controls a character knowing the spell
	#3magic spell set exclusivedelay <seconds>#0 - sets the post-cast lockout of all spells
	#3magic spell set nonexclusivedelay <seconds>#0 - sets the post-cast lockout of same school spells
	#3magic spell set trigger new <type> [...]#0 - changes the trigger for the spell to a new type
	#3magic spell set trigger set ...#0 - changes properties of the trigger. See individual trigger help.
	#3magic spell set effect add <type> [...]#0 - adds a new effect to the spell
	#3magic spell set effect remove <##>#0 - removes an effect from the spell
	#3magic spell set effect <##> ...#0 - changes the properties of a spell effect
	#3magic spell set castereffect add <type> [...]#0 - adds a new caster-only effect to the spell
	#3magic spell set castereffect remove <##>#0 - removes a caster-only effect from the spell
	#3magic spell set castereffect <##> ...#0 - changes the properties of a caster-only spell effect
	#3magic spell set material add held|wielded|inroom|consumed|consumedliquid ...#0 - adds a new material requirement to this spell
	#3magic spell set material delete <#>#0 - deletes a material requirement
	#3magic spell set cost <resource> <trait expression>#0 - sets the trait expression for casting cost for a resource
	#3magic spell set cost <resource> remove#0 - removes a casting cost for a resource
	#3magic spell set castemote <emote>#0 - sets the cast emote. $0 is caster, $1 is target (if any)
	#3magic spell set targetemote <emote>#0 - sets the target emote. $0 is caster, $1 is target (if any)
	#3magic spell set failcastemote <emote>#0 - sets the fail cast emote. $0 is caster, $1 is target (if any)
	#3magic spell set targetresistemote <emote>#0 - sets the target resist emote. $0 is caster, $1 is target (if any)
	#3magic spell set emoteflags <flags>#0 - changes the output flags for the casting emotes
	#3magic spell set targetemoteflags <flags>#0 - changes the output flags for the target emotes
	#3magic spell set trait <skill/attribute>#0 - sets the trait used for casting this spell
	#3magic spell set difficulty <difficulty>#0 - sets the difficulty of casting this spell
	#3magic spell set threshold <outcome>#0 - sets the minimum success threshold for casting the spell
	#3magic spell set resist none#0 - makes this spell not resisted
	#3magic spell set resist <trait> <difficulty>#0 - makes this spell resisted by a trait check
	#3magic spell set duration <trait expression>#0 - sets the trait expression that controls effect duration
	#3magic spell set exclusiveeffect#0 - toggles whether effects are exclusive (and overwrite) or not (and stack)",

		GetEditHeader = item => $"Magic Spell #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper MagicSchoolHelper { get; } = new()
	{
		ItemName = "Magic School",
		ItemNamePlural = "Magic School",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSchool>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicSchool>(actor) { EditingItem = (IMagicSchool)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicSchool>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicSchools.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicSchools.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicSchools.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicSchool)item),
		CastToType = typeof(IMagicSchool),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new magic school.");
				return;
			}

			var name = input.PopSpeech().TitleCase();

			if (actor.Gameworld.MagicSchools.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a magic school called {name.ColourName()}. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a verb used as a command to interact with that magic school.");
				return;
			}
			var verb = input.PopForSwitch();

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify an adjective to describe effects from that magic school.");
				return;
			}
			var adjective = input.PopSpeech().ToLowerInvariant();

			ANSIColour colour = Telnet.Magenta;
			if (!input.IsFinished)
			{
				colour = Telnet.GetColour(input.SafeRemainingArgument);
				if (colour is null)
				{
					actor.OutputHandler.Send($"That is not a valid colour option. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
					return;
				}
			}

			var school = new MagicSchool(actor.Gameworld, name, verb, adjective, colour);
			actor.Gameworld.Add(school);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSchool>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSchool>(actor) { EditingItem = school });
			actor.OutputHandler.Send(
				$"You create a new school of magic called {school.Name.Colour(school.PowerListColour)}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic school do you want to clone?");
				return;
			}

			var school = actor.Gameworld.MagicSchools.GetByIdOrName(input.PopSpeech());
			if (school == null)
			{
				actor.OutputHandler.Send("There is no such magic school.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned school.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicSchools.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic school with that name. Names must be unique.");
				return;
			}

			var clone = school.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicSchool>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicSchool>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic school {school.Name.Colour(school.PowerListColour)} as {clone.Name.Colour(clone.PowerListColour)}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Verb",
			"Adjective",
			"Parent",
			"Colour",
			"Spells",
			"Powers",
			"Capabilities"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicSchool>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.SchoolVerb,
															  proto.SchoolAdjective,
															  proto.ParentSchool?.Name ?? "",
															  proto.PowerListColour.Name.Colour(proto.PowerListColour),
															  proto.Gameworld.MagicSpells.Count(x => x.School == proto).ToString("N0", character),
															  proto.Gameworld.MagicPowers.Count(x => x.School == proto).ToString("N0", character),
															  proto.Gameworld.MagicCapabilities.Count(x => x.School == proto).ToString("N0", character),
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic schools.

The core syntax is as follows:

	#3magic school list - shows all magic schools
	#3magic school edit new <name> <school>#0 - creates a new magic school
	#3magic school clone <old> <new>#0 - clones an existing magic school
	#3magic school edit <which>#0 - begins editing a magic school
	#3magic school close#0 - closes an editing magic school
	#3magic school show <which>#0 - shows builder information about a school
	#3magic school show#0 - shows builder information about the currently edited school
	#3magic school edit#0 - an alias for magic school show (with no args)
	#3magic school set name <name>#0 - renames this school
	#3magic school set parent <which>#0 - sets a parent school
	#3magic school set parent none#0 - clears a parent school
	#3magic school set adjective <which>#0 - sets the adjective used to refer to powers in this school
	#3magic school set verb <which>#0 - sets the verb (command) used for this school
	#3magic school set colour <which>#0 - sets the ANSI colour for display with this school",

		GetEditHeader = item => $"Magic School #{item.Id:N0} ({item.Name})"
	};
	public static EditableItemHelper MagicCapabilityHelper { get; } = new()
	{
		ItemName = "Magic Capability",
		ItemNamePlural = "Magic Capabilities",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = (IMagicCapability)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicCapability>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicCapabilities.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicCapabilities.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicCapabilities.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicCapability)item),
		CastToType = typeof(IMagicCapability),
		EditableNewAction = (actor, input) =>
		{
			var capability = MagicCapabilityFactory.LoaderFromBuilderInput(actor.Gameworld, actor, input);
			if (capability is null)
			{
				return;
			}

			actor.Gameworld.Add(capability);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = capability });
			actor.OutputHandler.Send($"You create a new magic capability called {capability.Name.ColourName()}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic capability do you want to clone?");
				return;
			}

			var capability = actor.Gameworld.MagicCapabilities.GetByIdOrName(input.PopSpeech());
			if (capability == null)
			{
				actor.OutputHandler.Send("There is no such magic capability.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned magic capability.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicCapabilities.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic capability with that name. Names must be unique.");
				return;
			}

			var newCapability = capability.Clone(name);

			actor.Gameworld.Add(newCapability);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicCapability>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicCapability>(actor) { EditingItem = newCapability });
			actor.OutputHandler.Send($"You create a new magic capability called {newCapability.Name.ColourName()} as a clone of {capability.Name.ColourName()}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"School",
			"Power",
			"# Powers",
			"# Regens",
			"Resources"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicCapability>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.School.Name,
															  proto.PowerLevel.ToString("N0", character),
															  proto.AllPowers.Count().ToString("N0", character),
															  proto.Regenerators.Count().ToString("N0", character),
															  proto.Regenerators.SelectMany(x => x.GeneratedResources).Distinct().Select(x => x.Name.ColourValue()).ListToCommaSeparatedValues(", ")
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic resources.

The core syntax is as follows:

	#3magic capability list - shows all magic capabilities
	#3magic capability edit new <type> <name>`#0 - creates a new magic capability
	#3magic capability clone <old> <new>#0 - clones an existing magic capability
	#3magic capability edit <which>#0 - begins editing a magic capability
	#3magic capability close#0 - closes an editing magic capability
	#3magic capability show <which>#0 - shows builder information about a capability
	#3magic capability show#0 - shows builder information about the currently edited capability
	#3magic capability edit#0 - an alias for magic capability show (with no args)
	#3magic capability set ...#0 - edits the properties of a magic capability. See #3magic capability set ?#0 for more info.",

		GetEditHeader = item => $"Magic Capability #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper MagicResourceHelper { get; } = new()
	{
		ItemName = "Magic Resource",
		ItemNamePlural = "Magic Resources",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResource>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicResource>(actor) { EditingItem = (IMagicResource)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicResource>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicResources.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicResources.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicResources.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicResource)item),
		CastToType = typeof(IMagicResource),
		EditableNewAction = (actor, input) =>
		{
			var resource = BaseMagicResource.CreateResourceFromBuilderInput(actor, input);
			if (resource is null)
			{
				return;
			}

			actor.Gameworld.Add(resource);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResource>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResource>(actor) { EditingItem = resource });
			actor.OutputHandler.Send($"You create a new magic resource called {resource.Name.Colour(Telnet.BoldPink)}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic resource do you want to clone?");
				return;
			}

			var resource = actor.Gameworld.MagicResources.GetByIdOrName(input.PopSpeech());
			if (resource == null)
			{
				actor.OutputHandler.Send("There is no such magic resource.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned magic resource.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicResources.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic resource with that name. Names must be unique.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a short name for your new cloned magic resource.");
				return;
			}

			var shortName = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicResources.Any(x => x.ShortName.EqualTo(shortName)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic resource with that short name. Names must be unique.");
				return;
			}

			var clone = resource.Clone(name, shortName);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResource>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResource>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic resource {resource.Name.Colour(Telnet.BoldPink)} as {clone.Name.Colour(Telnet.BoldPink)} ({clone.ShortName.Colour(Telnet.BoldPink)}), which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Short",
			"Type",
			"Prompt",
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicResource>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.ShortName,
															  proto.ResourceType.DescribeEnum(),
															  proto.ClassicPromptString(1.0)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic resources.

The core syntax is as follows:

	#3magic resource list - shows all magic resources
	#3magic resource edit new <type> <name> <shortname>#0 - creates a new magic resource
	#3magic resource clone <old> <new>#0 - clones an existing magic resource
	#3magic resource edit <which>#0 - begins editing a magic resource
	#3magic resource close#0 - closes an editing magic resource
	#3magic resource show <which>#0 - shows builder information about a resource
	#3magic resource show#0 - shows builder information about the currently edited resource
	#3magic resource edit#0 - an alias for magic resource show (with no args)
	#3magic resource set ...#0 - edits the properties of a magic resource. See #3magic resource set ?#0 for more info.",

		GetEditHeader = item => $"Magic Resource #{item.Id:N0} ({item.Name})"
	};
	public static EditableItemHelper MagicRegeneratorHelper { get; } = new()
	{
		ItemName = "Magic Regenerator",
		ItemNamePlural = "Magic Regenerators",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResourceRegenerator>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicResourceRegenerator>(actor) { EditingItem = (IMagicResourceRegenerator)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicResourceRegenerator>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicResourceRegenerators.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicResourceRegenerators.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicResourceRegenerators.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicResourceRegenerator)item),
		CastToType = typeof(IMagicResourceRegenerator),
		EditableNewAction = (actor, input) =>
		{
			var regenerator = BaseMagicResourceGenerator.LoadFromBuilderInput(actor, input);
			if (regenerator is null)
			{
				return;
			}

			actor.Gameworld.Add(regenerator);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResourceRegenerator>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResourceRegenerator>(actor) { EditingItem = regenerator });
			actor.OutputHandler.Send($"You create a new magic regenerator called {regenerator.Name.Colour(Telnet.Cyan)}, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic regenerator do you want to clone?");
				return;
			}

			var regenerator = actor.Gameworld.MagicResourceRegenerators.GetByIdOrName(input.PopSpeech());
			if (regenerator == null)
			{
				actor.OutputHandler.Send("There is no such magic regenerator.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a name for your new cloned magic regenerator.");
				return;
			}

			var name = input.SafeRemainingArgument.TitleCase();
			if (actor.Gameworld.MagicResourceRegenerators.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send(
					$"There is already a magic regenerator with that name. Names must be unique.");
				return;
			}

			var clone = regenerator.Clone(name);
			actor.Gameworld.Add(clone);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicResourceRegenerator>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicResourceRegenerator>(actor) { EditingItem = clone });
			actor.OutputHandler.Send(
				$"You clone the magic regenerator {regenerator.Name.Colour(Telnet.Cyan)} as {clone.Name.Colour(Telnet.Cyan)}, which you are now editing.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"Type",
			"Resources",
			"# Times Used"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicResourceRegenerator>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.RegeneratorTypeName,
															  proto.GeneratedResources.Select(x => x.Name.Colour(Telnet.BoldPink)).ListToCommaSeparatedValues(", "),
															  proto.Gameworld.MagicCapabilities.Count(x => x.Regenerators.Contains(proto)).ToString("N0", character)
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic regenerators.

The core syntax is as follows:

	#3magic regenerator list - shows all magic regenerators
	#3magic regenerator edit new <type> <name> <resource>#0 - creates a new magic regenerator
	#3magic regenerator clone <old> <new>#0 - clones an existing magic regenerator
	#3magic regenerator edit <which>#0 - begins editing a magic regenerator
	#3magic regenerator close#0 - closes an editing magic regenerator
	#3magic regenerator show <which>#0 - shows builder information about a regenerator
	#3magic regenerator show#0 - shows builder information about the currently edited regenerator
	#3magic regenerator edit#0 - an alias for magic regenerator show (with no args)
	#3magic regenerator set ...#0 - edits the properties of a magic regenerator. See #3magic regenerator set ?#0 for more info.",

		GetEditHeader = item => $"Magic Regenerator #{item.Id:N0} ({item.Name})"
	};

	public static EditableItemHelper MagicPowerHelper { get; } = new()
	{
		ItemName = "Magic Power",
		ItemNamePlural = "Magic Powers",
		SetEditableItemAction = (actor, item) =>
		{
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicPower>>();
			if (item == null)
			{
				return;
			}

			actor.AddEffect(new BuilderEditingEffect<IMagicPower>(actor) { EditingItem = (IMagicPower)item });
		},
		GetEditableItemFunc = actor =>
			actor.CombinedEffectsOfType<BuilderEditingEffect<IMagicPower>>().FirstOrDefault()?.EditingItem,
		GetAllEditableItems = actor => actor.Gameworld.MagicPowers.ToList(),
		GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.MagicPowers.Get(id),
		GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.MagicPowers.GetByIdOrName(input),
		AddItemToGameWorldAction = item => item.Gameworld.Add((IMagicPower)item),
		CastToType = typeof(IMagicPower),
		EditableNewAction = (actor, input) =>
		{
			if (input.IsFinished)
			{
				actor.OutputHandler.Send($"Which type of magic power do you want to create? Valid types are {MagicPowerFactory.BuilderTypes.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			var typeText = input.PopSpeech();
			if (!MagicPowerFactory.BuilderTypes.Any(x => x.EqualTo(typeText)))
			{
				actor.OutputHandler.Send($"The text {typeText.ColourCommand()} is not a valid magic power type. Valid types are {MagicPowerFactory.BuilderTypes.Select(x => x.ColourName()).ListToString()}.");
				return;
			}

			if (input.IsFinished)
			{
				actor.OutputHandler.Send("Which magic school is your new power for?");
				return;
			}

			var school = actor.Gameworld.MagicSchools.GetByIdOrName(input.PopSpeech());
			if (school is null)
			{
				actor.OutputHandler.Send("There is no such magic school.");
				return;
			}

			var name = input.PopSpeech().TitleCase();
			if (actor.Gameworld.MagicPowers.Any(x => x.School == school && x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send($"There is already a magic power in the {school.Name.Colour(school.PowerListColour)} with the name {name.ColourName()}. Names must be unique.");
				return;
			}

			var power = MagicPowerFactory.LoadPowerFromBuilderInput(actor.Gameworld, school, name, typeText, actor, input);
			if (power is null)
			{
				return;
			}

			actor.Gameworld.Add(power);
			actor.RemoveAllEffects<BuilderEditingEffect<IMagicPower>>();
			actor.AddEffect(new BuilderEditingEffect<IMagicPower>(actor)
			{
				EditingItem
			 = power
			});
			actor.OutputHandler.Send($"You create a new power of type {typeText.ColourName} called {name.ColourName()} in the {school.Name.ColourName()} school, which you are now editing.");
		},
		EditableCloneAction = (actor, input) =>
		{
			actor.OutputHandler.Send("Not yet implemented.");
		},

		GetListTableHeaderFunc = character => new List<string>
		{
			"Id",
			"Name",
			"School",
			"Type"
		},

		GetListTableContentsFunc = (character, protos) => from proto in protos.OfType<IMagicPower>()
														  select new List<string>
														  {
															  proto.Id.ToString("N0", character),
															  proto.Name,
															  proto.School.Name,
															  proto.PowerType
														  },

		CustomSearch = (protos, keyword, gameworld) => protos,

		DefaultCommandHelp = @"This command is used to work with and edit magic powers.

The core syntax is as follows:

	#3magic power list - shows all magic powers
	#3magic power edit new <type> <school> <name>#0 - creates a new magic power
	#3magic power clone <old> <new>#0 - clones an existing magic power
	#3magic power edit <which>#0 - begins editing a magic power
	#3magic power close#0 - closes an editing magic power
	#3magic power show <which>#0 - shows builder information about a power
	#3magic power show#0 - shows builder information about the currently edited power
	#3magic power edit#0 - an alias for magic power show (with no args)
	#3magic power set ...#0 - edits the properties of a magic power. See #3magic power set ?#0 for more info.",

		GetEditHeader = item => $"Magic Power #{item.Id:N0} ({item.Name})"
	};
}
