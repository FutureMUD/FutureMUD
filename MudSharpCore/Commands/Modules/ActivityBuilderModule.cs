using MudSharp.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Accounts;
using MudSharp.Commands.Helpers;
using MudSharp.Economy;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.Work.Butchering;

namespace MudSharp.Commands.Modules;

internal class ActivityBuilderModule : BaseBuilderModule
{
	private ActivityBuilderModule()
		: base("ActivityBuilder")
	{
		IsNecessary = true;
	}

	public static ActivityBuilderModule Instance { get; } = new();

	#region Butchering

	public const string ButcheringHelp = @"This command is used to view and edit race butchery profiles. 

These butchery profiles can be attached to races to show what happens when you use the #3skin#0, #3butcher#0 or #3salvage#0 commands on them. They control what items can be gained and how it all breaks down.

You must also separately make #6butchery products#0 with the #3butcheryproduct#0 command. See that command's helpfile for more information about how to use them.

You can use the following options with this command:

	#3butchery list [+key, -key]#0 - shows all the butchery profiles
	#3butchery show <which>#0 - shows a butchery profile
	#3butchery edit <which>#0 - opens a butchery profile for editing
	#3butchery edit new <name>#0 - creates a new butchery profile
	#3butchery edit#0 - an alias for #3butchery show#0 on an opened butchery profile
	#3butchery clone <which> <name>#0 - clones a butchery profile to a new one
	#3butchery shallowclone <which> <name>#0 - clones a butchery profile to a new one (excluding products)
	#3butchery close#0 - closes the currently edited foragable profile
	#3butchery set name <name>#0 - renames this profile
	#3butchery set verb butcher|salvage#0 - changes the verb used for interacting with these corpses
	#3butchery set tool <tag>#0 - sets the tag of tools required to interact with this
	#3butchery set skindiff <difficulty>#0 - sets the difficulty of the skinning check
	#3butchery set can <prog>#0 - sets a prog to control whether someone can butcher this
	#3butchery set why <prog>#0 - sets a prog for a custom error message on can butcher failure
	#3butchery set product <which>#0 - toggles a #6butchery product#0 being included in this profile

For all of the below phase emote echoes, you can use #6$0#0 for the actor, #6$1#0 for the corpse, and #6$2#0 for the tool item.

#FSkin Phases#0

	#3butchery set skinemote add <seconds> <emotetext>#0 - adds a new skinning phase with specified length and emote
	#3butchery set skinemote remove <##>#0 - removes the specified skin phase
	#3butchery set skinemote swap <##1> <##2>#0 - swaps the order of two skin phases
	#3butchery set skinemote edit <##> <new text>#0 - changes the echo for a skin emote
	#3butchery set skinemote delay <##> <seconds>#0 - changes the delay for a skin emote phase

#FButcher Phases#0

	#3butchery set emote add <seconds> <emotetext>#0 - adds a new phase with specified length and emote
	#3butchery set emote remove <##>#0 - removes the specified phase
	#3butchery set emote swap <##1> <##2>#0 - swaps the order of two phases
	#3butchery set emote edit <##> <new text>#0 - changes the echo for an emote
	#3butchery set emote delay <##> <seconds>#0 - changes the delay for an emote phase

#FSub-Component Butcher Phases#0

	#3butchery set subemote <which> add <seconds> <emotetext>#0 - adds a new phase with specified length and emote
	#3butchery set subemote <which> remove <##>#0 - removes the specified phase
	#3butchery set subemote <which> swap <##1> <##2>#0 - swaps the order of two phases
	#3butchery set subemote <which> edit <##> <new text>#0 - changes the echo for an emote
	#3butchery set subemote <which> delay <##> <seconds>#0 - changes the delay for an emote phase";

	[PlayerCommand("Butchering", "butchering", "butchery")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("butchering", ButcheringHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Butchering(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				ButcheringList(actor, ss);
				return;
			case "edit":
				ButcheringEdit(actor, ss);
				return;
			case "show":
				ButcheringShow(actor, ss);
				return;
			case "close":
				ButcheringClose(actor);
				return;
			case "set":
				ButcheringSet(actor, ss);
				return;
			case "clone":
				ButcheringClone(actor, ss, true);
				return;
			case "shallowclone":
				ButcheringClone(actor, ss, false);
				return;
			default:
				actor.OutputHandler.Send(ButcheringHelp.SubstituteANSIColour());
				return;
		}
	}

	protected static void ButcheringList(ICharacter actor, StringStack ss)
	{
		var items = actor.Gameworld.RaceButcheryProfiles.ToList();
		// filters
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in items select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.Verb.Describe(),
				item.DifficultySkin.DescribeColoured(),
				item.RequiredToolTag?.FullName.ColourName() ?? "",
				item.CanButcherProg?.MXPClickableFunctionName() ?? "",
				item.WhyCannotButcherProg?.MXPClickableFunctionName() ?? "",
				item.Products.Count().ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Verb",
				"Skin",
				"Tool Tag",
				"Can Prog",
				"Why Can't Prog",
				"# Products"
			},
			actor,
			Telnet.Red
		));
	}

	private static void ButcheringClone(ICharacter actor, StringStack ss, bool copyproducts)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which butchery profile do you want to clone?");
			return;
		}

		var profile = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.RaceButcheryProfiles.Get(value)
			: actor.Gameworld.RaceButcheryProfiles.GetByName(ss.Last);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery profile.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the newly cloned butchery profile?");
			return;
		}

		var name = ss.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		if (actor.Gameworld.RaceButcheryProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already butchery profile with that name. Names must be unique.");
			return;
		}

		var newprofile = profile.Clone(name, copyproducts);
		actor.Gameworld.Add(newprofile);
		actor.RemoveAllEffects<BuilderEditingEffect<IRaceButcheryProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRaceButcheryProfile>(actor) { EditingItem = newprofile });
		actor.OutputHandler.Send(
			$"You clone butchery profile {profile.Name.ColourName()} as {name.ColourName()} (Id #{newprofile.Id.ToString("N0", actor)}), which you are now editing.");
	}

	private static void ButcheringEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
							   .FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which butchery profile do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new butchery profile?");
				return;
			}

			var name = ss.SafeRemainingArgument.ToLowerInvariant().TitleCase();
			if (actor.Gameworld.RaceButcheryProfiles.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a butchery profile with that name. Names must be unique.");
				return;
			}

			var newprofile = new RaceButcheryProfile(actor.Gameworld, name);
			actor.Gameworld.Add(newprofile);
			actor.RemoveAllEffects<BuilderEditingEffect<IRaceButcheryProfile>>();
			actor.AddEffect(new BuilderEditingEffect<IRaceButcheryProfile>(actor) { EditingItem = newprofile });
			actor.OutputHandler.Send(
				$"You create a new butchery profile called {name.ColourName()} (Id #{newprofile.Id.ToString("N0", actor)}), which you are now editing.");
			return;
		}

		var profile = long.TryParse(cmd, out var value)
			? actor.Gameworld.RaceButcheryProfiles.Get(value)
			: actor.Gameworld.RaceButcheryProfiles.GetByName(cmd);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery profile.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IRaceButcheryProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IRaceButcheryProfile>(actor) { EditingItem = profile });
		actor.OutputHandler.Send(
			$"You are now editing butchery profile #{profile.Id.ToString("N0", actor)} ({profile.Name.ColourName()}).");
	}

	private static void ButcheringShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
							   .FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which butchery profile do you want to show?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var profile = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.RaceButcheryProfiles.Get(value)
			: actor.Gameworld.RaceButcheryProfiles.GetByName(ss.SafeRemainingArgument);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery profile.");
			return;
		}

		actor.OutputHandler.Send(profile.Show(actor));
	}

	private static void ButcheringSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
						   .FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any butchery profiles.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ButcheringClose(ICharacter actor)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IRaceButcheryProfile>>()
						   .FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any butchery profiles.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} butchery profile.");
	}

	public const string ButcheringProductHelp = @"This command is used to view and edit butchering products, which are produced by race butchery profiles (see #6help butchering#0). 

Butchery products are essentially an item or group of items that is produced from butchering, skinning or salvaging when a nominated bodypart is present. They control what is loaded, how damaged bodyparts impact the final product, and the categories in which the product appears for subsystem breakdowns.

You can use the following options with this command:

	#3butcheringproduct list [+key, -key]#0 - shows all the butchering products
	#3butcheringproduct show <which>#0 - shows a butchering product
	#3butcheringproduct edit <which>#0 - opens a butchering product for editing
	#3butcheringproduct edit new <name>#0 - creates a new butchering product
	#3butcheringproduct edit#0 - an alias for #3butcheringproduct show#0 on an opened butchering product
	#3butcheringproduct clone <which> <name>#0 - clones a butchering products to a new one
	#3butcheringproduct close#0 - closes the currently edited butchering product
	#3butcheringproduct set name <name>#0 - renames this product
	#3butcheringproduct set category <category>#0 - changes the category of this product
	#3butcheringproduct set pelt#0 - toggles whether this is a pelt (i.e. from SKIN) or not (i.e. BUTCHER/SALVAGE)
	#3butcheringproduct set body <which> [<parts ...>]#0 - changes which body type this profile targets
	#3butcheringproduct set part <which>#0 - toggles requiring the corpse to have this bodypart
	#3butcheringproduct set prog <which>#0 - sets the prog that controls whether the item is produced
	#3butcheringproduct set item add <number> <proto> [<number> <damaged> <damage%>]#0 - adds a new item product
	#3butcheringproduct set item delete <##>#0 - deletes an item product
	#3butcheringproduct set item <##> quantity <number>#0 - changes the quantity of items produced
	#3butcheringproduct set item <##> proto <id>#0 - changes the proto produced
	#3butcheringproduct set item <##> threshold <%>#0 - changes the damage percentage for normal/damaged items
	#3butcheringproduct set item <##> damaged <quantity> <proto>#0 - changes the damaged proto
	#3butcheringproduct set item <##> nodamaged#0 - clears the damaged proto";

	[PlayerCommand("ButcheryProduct", "butcheringproduct", "butcheryproduct")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("butcheringproduct", ButcheringProductHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void ButcheryProduct(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "list":
				ButcheryProductList(actor, ss);
				return;
			case "edit":
				ButcheryProductEdit(actor, ss);
				return;
			case "close":
				ButcheryProductClose(actor);
				return;
			case "clone":
				ButcheryProductClone(actor, ss);
				return;
			case "show":
			case "view":
				ButcheryProductShow(actor, ss);
				return;
			case "set":
				ButcheryProductSet(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(ButcheringProductHelp.SubstituteANSIColour());
				return;
		}
	}

	protected static void ButcheryProductList(ICharacter actor, StringStack ss)
	{
		var items = actor.Gameworld.ButcheryProducts.ToList();
		// filters
		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in items select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name ?? "",
				item.Subcategory ?? "",
				item.IsPelt.ToColouredString(),
				item.TargetBody?.Name ?? "",
				item.RequiredBodyparts.Select(x => x.Name).ListToCommaSeparatedValues(", "),
				item.CanProduceProg?.MXPClickableFunctionName() ?? "",
				item.ProductItems.Count().ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Category",
				"Pelt?",
				"Body",
				"Parts",
				"Prog",
				"# Items"
			},
			actor,
			Telnet.Red
		));
	}

	private static void ButcheryProductClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which butchery product do you want to clone?");
			return;
		}

		var profile = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.ButcheryProducts.Get(value)
			: actor.Gameworld.ButcheryProducts.GetByName(ss.Last);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery product.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the newly cloned butchery product?");
			return;
		}

		var name = ss.SafeRemainingArgument.ToLowerInvariant().TitleCase();
		if (actor.Gameworld.ButcheryProducts.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already butchery product with that name. Names must be unique.");
			return;
		}

		var newprofile = profile.Clone(name);
		actor.Gameworld.Add(newprofile);
		actor.RemoveAllEffects<BuilderEditingEffect<IButcheryProduct>>();
		actor.AddEffect(new BuilderEditingEffect<IButcheryProduct>(actor) { EditingItem = newprofile });
		actor.OutputHandler.Send(
			$"You clone butchery product {profile.Name.ColourName()} as {name.ColourName()} (Id #{newprofile.Id.ToString("N0", actor)}), which you are now editing.");
	}

	private static void ButcheryProductEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IButcheryProduct>>()
							   .FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which butchery product do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var cmd = ss.PopSpeech();
		if (cmd.EqualTo("new"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to your new butchery product?");
				return;
			}

			var name = ss.PopSpeech().ToLowerInvariant().TitleCase();
			if (actor.Gameworld.ButcheryProducts.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a butchery product with that name. Names must be unique.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("Which body type should this butchery product apply to?");
				return;
			}

			var body = actor.Gameworld.BodyPrototypes.GetByIdOrName(ss.PopSpeech());
			if (body is null)
			{
				actor.OutputHandler.Send("There is no such body prototype.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which bodypart from that body must be present for this butchery product to apply?");
				return;
			}

			var part = body.AllBodyparts.GetByIdOrName(ss.PopSpeech());
			if (part is null)
			{
				actor.OutputHandler.Send($"The {body.Name.ColourName()} body does not have any such bodypart.");
				return;
			}

			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("What item should this butchery product produce?");
				return;
			}

			var proto = actor.Gameworld.ItemProtos.GetByIdOrName(ss.PopSpeech());
			if (proto is null)
			{
				actor.OutputHandler.Send("There is no such item prototype.");
				return;
			}

			int quantity = 1;
			if (!ss.IsFinished)
			{
				if (!int.TryParse(ss.SafeRemainingArgument, out quantity) || quantity < 1)
				{
					actor.OutputHandler.Send("You must enter a valid number 1 or greater for the quantity.");
					return;
				}
			}
			
			var newprofile = new ButcheryProduct(name, body, part, proto, quantity);
			actor.Gameworld.Add(newprofile);
			actor.RemoveAllEffects<BuilderEditingEffect<IButcheryProduct>>();
			actor.AddEffect(new BuilderEditingEffect<IButcheryProduct>(actor) { EditingItem = newprofile });
			actor.OutputHandler.Send(
				$"You create a new butchery product called {name.ColourName()} (Id #{newprofile.Id.ToString("N0", actor)}), which you are now editing.");
			return;
		}

		var profile = long.TryParse(cmd, out var value)
			? actor.Gameworld.ButcheryProducts.Get(value)
			: actor.Gameworld.ButcheryProducts.GetByName(cmd);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery product.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IButcheryProduct>>();
		actor.AddEffect(new BuilderEditingEffect<IButcheryProduct>(actor) { EditingItem = profile });
		actor.OutputHandler.Send(
			$"You are now editing butchery product #{profile.Id.ToString("N0", actor)} ({profile.Name.ColourName()}).");
	}

	private static void ButcheryProductShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IButcheryProduct>>()
							   .FirstOrDefault();
			if (editing == null)
			{
				actor.OutputHandler.Send("Which butchery product do you want to show?");
				return;
			}

			actor.OutputHandler.Send(editing.EditingItem.Show(actor));
			return;
		}

		var profile = long.TryParse(ss.SafeRemainingArgument, out var value)
			? actor.Gameworld.ButcheryProducts.Get(value)
			: actor.Gameworld.ButcheryProducts.GetByName(ss.SafeRemainingArgument);
		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such butchery product.");
			return;
		}

		actor.OutputHandler.Send(profile.Show(actor));
	}

	private static void ButcheryProductSet(ICharacter actor, StringStack ss)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IButcheryProduct>>()
						   .FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any butchery products.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, ss);
	}

	private static void ButcheryProductClose(ICharacter actor)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IButcheryProduct>>()
						   .FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any butchery products.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} butchery product.");
	}

	#endregion

	#region Economic Zones

	public const string EconomicZoneHelp =
		@"This command allows you to view, create and edit economic zones, which are the backbones of all economic activity on your MUD.

The syntax for this command is as follows:

	#3ez list#0 - lists all of the economic zones
	#3ez edit <which>#0 - begins editing an economic zone
	#3ez edit new <zone> <name>#0 - generates a new economic zone using a real zone as a reference location
	#3ez clone <old> <new name>#0 - clones an existing economic zone to a new one
	#3ez close#0 - stops editing an economic zone
	#3ez show <which>#0 - views information about an economic zone
	#3ez show#0 - views information about your currently editing economic zone
	#3ez set name <name>#0 - renames this economic zone
	#3ez set currency <currency>#0 - changes the currency used in this zone
	#3ez set clock <clock>#0 - changes the clock used in this zone
	#3ez set calendar <calendar>#0 - changes the calendar used in this zone
	#3ez set interval <type> <amount> <offset>#0 - sets the interval for financial periods
	#3ez set time <time>#0 - sets the reference time for financial periods
	#3ez set timezone <tz>#0 - sets the reference timezone for this zone
	#3ez set zone <zone>#0 - sets the physical zone used as a reference for current time
	#3ez set previous <amount>#0 - sets the number of previous financial periods to keep records for
	#3ez set permitloss#0 - toggles permitting taxable losses
	#3ez set clan <clan>#0 - assigns a new clan to custody of this economic zone
	#3ez set clan none#0 - clears clan control of this economic zone
	#3ez set salestax add <type> <name>#0 - adds a new sales tax
	#3ez set salestax remove <name>#0 - removes a sales tax
	#3ez set salestax <which> <...>#0 - edit properties of a particular tax
	#3ez set profittax add <type> <name>#0 - adds a new profit tax
	#3ez set profittax remove <name>#0 - removes a profit tax
	#3ez set profittax <which> <...>#0 - edit properties of a particular tax
	#3ez set realty#0 - toggles your current location as a conveyancing/realty location
	#3ez set jobs#0 - toggles your current location as a job listing and finding location
	#3ez set forgive <shop> <amount>#0 - forgives a certain amount of owing tax for a shop (excess gives credits)
	#3ez set forgive <shop> all#0 - forgives all owing taxes for a shop
	#3ez set shops#0 - lists all shops in this economic zone
	#3ez set shop <which>#0 - shows tax information about a shop in the zone
	#3ez set taxinfo#0 - shows you information about tax revenues in this zone";

	[PlayerCommand("EconomicZone", "economiczone", "ez")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("economiczone", EconomicZoneHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void EconomicZone(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "list":
				EconomicZoneList(actor, ss);
				return;
			case "edit":
				EconomicZoneEdit(actor, ss);
				return;
			case "close":
				EconomicZoneClose(actor, ss);
				return;
			case "show":
			case "view":
				EconomicZoneShow(actor, ss);
				return;
			case "set":
				EconomicZoneSet(actor, ss);
				return;
			case "clone":
				EconomicZoneClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(EconomicZoneHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void EconomicZoneClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to clone?");
			return;
		}

		var zone = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.EconomicZones.Get(value)
			: actor.Gameworld.EconomicZones.GetByName(command.Last);
		if (zone == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your newly cloned zone?");
			return;
		}

		var name = command.SafeRemainingArgument;
		if (actor.Gameworld.EconomicZones.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already an economic zone with that name. Names must be unique.");
			return;
		}

		var clone = zone.Clone(name);
		actor.Gameworld.Add(clone);
		actor.OutputHandler.Send(
			$"You clone the {zone.Name.ColourName()} economic zone into a new zone called {clone.Name.ColourName()}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IEconomicZone>>();
		actor.AddEffect(new BuilderEditingEffect<IEconomicZone>(actor) { EditingItem = clone });
	}

	private static void EconomicZoneClose(ICharacter actor, StringStack command)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing any economic zones.");
			return;
		}

		actor.RemoveEffect(editing);
		actor.OutputHandler.Send(
			$"You are no longer editing the {editing.EditingItem.Name.ColourName()} economic zone.");
	}

	private static void EconomicZoneShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
			if (editing != null)
			{
				actor.OutputHandler.Send(editing.EditingItem.Show(actor));
				return;
			}

			actor.OutputHandler.Send("Which economic zone do you want to show?");
			return;
		}

		var target = long.TryParse(command.SafeRemainingArgument, out var id)
			? actor.Gameworld.EconomicZones.Get(id)
			: actor.Gameworld.EconomicZones.GetByName(command.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("There is no such economic zone.");
			return;
		}

		actor.OutputHandler.Send(target.Show(actor));
	}

	private static void EconomicZoneSet(ICharacter actor, StringStack command)
	{
		var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
		if (editing == null)
		{
			actor.OutputHandler.Send("You are not editing an economic zone.");
			return;
		}

		editing.EditingItem.BuildingCommand(actor, command);
	}

	private static void EconomicZoneEdit(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			var editing = actor.CombinedEffectsOfType<BuilderEditingEffect<IEconomicZone>>().FirstOrDefault();
			if (editing != null)
			{
				actor.OutputHandler.Send(editing.EditingItem.Show(actor));
				return;
			}

			actor.OutputHandler.Send("Which economic zone do you want to edit?");
			return;
		}

		if (command.PeekSpeech().EqualTo("new"))
		{
			command.PopSpeech();
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which zone do you want to use as a reference zone for time for this economic zone?");
				return;
			}

			var zone = long.TryParse(command.PopSpeech(), out var value)
				? actor.Gameworld.Zones.Get(value)
				: actor.Gameworld.Zones.GetByName(command.Last);
			if (zone == null)
			{
				actor.OutputHandler.Send("There is no such zone.");
				return;
			}

			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What name do you want to give to the new economic zone?");
				return;
			}

			var name = command.SafeRemainingArgument.ToLowerInvariant().TitleCase();
			if (actor.Gameworld.EconomicZones.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already an economic zone with that name. Names must be unique.");
				return;
			}

			var economicZone = new EconomicZone(actor.Gameworld, zone, name);
			actor.Gameworld.Add(economicZone);
			actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IEconomicZone>>());
			actor.AddEffect(new BuilderEditingEffect<IEconomicZone>(actor) { EditingItem = economicZone });
			actor.OutputHandler.Send(
				$"You create a new economic zone called {name.ColourName()} based around the {zone.Name.ColourName()} zone.");
			return;
		}

		var target = long.TryParse(command.SafeRemainingArgument, out var id)
			? actor.Gameworld.EconomicZones.Get(id)
			: actor.Gameworld.EconomicZones.GetByName(command.SafeRemainingArgument);
		if (target == null)
		{
			actor.OutputHandler.Send("There is no such economic zone to edit.");
			return;
		}

		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<IEconomicZone>>());
		actor.AddEffect(new BuilderEditingEffect<IEconomicZone>(actor) { EditingItem = target });
		actor.OutputHandler.Send($"You are now editing the {target.Name.ColourName()} economic zone.");
	}

	private static void EconomicZoneList(ICharacter actor, StringStack command)
	{
		var zones = actor.Gameworld.EconomicZones.ToList();
		actor.OutputHandler.Send("Economic Zones:\n" + StringUtilities.GetTextTable(
			from zone in zones
			select new List<string>
			{
				zone.Id.ToString("N0", actor),
				zone.Name,
				zone.Currency.Name,
				actor.Gameworld.Shops.Count(x => x.EconomicZone == zone).ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"Currency",
				"# Shops"
			},
			actor.LineFormatLength,
			colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	#endregion
		
	#region Foragables

	private const string ForagableHelpText =
		@$"This command is used to view and edit foragables. Foragables are records of items that can be loaded by the foraging system. The items need to be first built using the item system, and you also need to add the foragable record to a foragable profile before it will show up in the world. A single foragable record can be shared between multiple foragable profiles.

You can use the following options with this command:

	#3foragable list [+key, -key]#0 - shows all the foragables
	#3foragable show <which>#0 - shows a foragable
	#3foragable edit <which>#0 - opens a revision of a foragable
	#3foragable edit new#0 - creates a new foragable
	#3foragable edit#0 - an alias for foragable show on an opened foragable
	#3foragable edit close#0 - closes the currently edited foragable
	#3foragable edit submit#0 - submits the foragable for review
	#3foragable edit delete#0 - deletes a non-approved revision
	#3foragable edit obsolete#0 - marks a foragable as obsolete
	#3foragable review all|mine|<which>#0 - opens a foragable for review
	#3foragable review list#0 - shows all the foragables due to review
	#3foragable review history <which>#0 - shows the history of a foragable
	#3foragable set name <name>#0 - renames this foragable
	#3foragable set proto <which>#0 - sets the proto for this foragable to load
	#3foragable set chance <#>#0 - the relative weight of this option being found
	#3foragable set quanity <# or dice>#0 - a number or dice expression for the quantity found
	#3foragable set difficulty <difficulty>#0 - the difficulty that the result is evaluated against for this item
	#3foragable set outcome <min> <max>#0 - the minimum and maximum check outcome that this item can appear on
	#3foragable set types <type1> [<type2>] ... [<typen>]#0 - sets the yield types that this foragable appears against
	#3foragable set canforage <prog>#0 - sets a prog that controls whether this foragable can be found
	#3foragable set canforage clear#0 - clears the can-forage prog
	#3foragable set onforage <prog>#0 - sets a prog that will run when this item is foraged
	#3foragable set onforage clear#0 - clears the on-forage prog

{GenericReviewableSearchList}";

	[PlayerCommand("Foragable", "foragable")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("foragable", ForagableHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Foragable(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "review":
				GenericReview(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "edit":
				GenericRevisableEdit(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "set":
				GenericRevisableSet(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "list":
				GenericRevisableList(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			case "show":
				GenericRevisableShow(actor, ss, EditableRevisableItemHelper.ForagableHelper);
				break;
			default:
				actor.OutputHandler.Send(ForagableHelpText.SubstituteANSIColour());
				break;
		}
	}

	#endregion

	#region Foragable Profiles

	private const string ForagableProfileHelpText =
		@$"This command is used to view and edit foragable profiles. 

Foragable profiles are attached typically to zones or terrain types, and control both what yield types appear in that location and what items can be foraged by players. Each zone or terrain type has a maximum of one foragable profile. The foragable profile controls what things can be found in that area with foraging and how fast the resources regenerate.

The individual foraged items are built using the #Bforagable#0 command. These foragables can be shared between multiple foragable profiles. 

See #6terrain set forage <id|name>#0 to set a default foragable profile for a terrain type
See #6zone set <which> forage <id|name>#0 to set a default foragable profile for a zone
See #6cell set forage <id|name>#0 to set a foragable profile at a room level.

You can use the following options with this command:

	#3fp list [+key, -key]#0 - shows all the foragable profiles
	#3fp show <which>#0 - shows a foragable profile
	#3fp edit <which>#0 - opens a revision of a foragable profile
	#3fp edit new#0 - creates a new foragable profile
	#3fp edit#0 - an alias for fp show on an opened foragable profile
	#3fp edit close#0 - closes the currently edited foragable profile
	#3fp edit submit#0 - submits the foragable profile for review
	#3fp edit delete#0 - deletes a non-approved revision
	#3fp edit obsolete#0 - marks a foragable profile as obsolete
	#3fp review all|mine|<which>#0 - opens a foragable profile for review
	#3fp review list#0 - shows all the foragable profile due to review
	#3fp review history <which>#0 - shows the history of a foragable profile
	#3fp set name <name>#0 - renames this foragable profile
	#3fp set yield <which> <max> <hourly regain>#0 - sets up a yield for this profile
	#3fp set yield <which> 0#0 - removes a yield from this profile
	#3fp set foragable <which>#0 - toggles a foragable belonging to this profile

{GenericReviewableSearchList}";

	[PlayerCommand("ForagableProfile", "foragableprofile", "fp")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("foragableprofile", ForagableProfileHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void ForagableProfile(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "review":
				GenericReview(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "edit":
				GenericRevisableEdit(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "set":
				GenericRevisableSet(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "list":
				GenericRevisableList(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "show":
				GenericRevisableShow(actor, ss, EditableRevisableItemHelper.ForagableProfileHelper);
				break;
			case "load":
				Item_Load(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(ForagableProfileHelpText.SubstituteANSIColour());
				break;
		}
	}

	#endregion
}