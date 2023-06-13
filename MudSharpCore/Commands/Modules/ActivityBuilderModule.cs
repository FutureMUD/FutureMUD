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
using MudSharp.Work.Butchering;

namespace MudSharp.Commands.Modules;

internal class ActivityBuilderModule : BaseBuilderModule
{
	private ActivityBuilderModule()
		: base("ActivityBuilder")
	{
		IsNecessary = true;
	}

	public new static ActivityBuilderModule Instance { get; } = new();

	#region Butchering

	public const string ButcheringHelp = "";

	[PlayerCommand("Butchering", "butchering", "butchery")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("butchering", ButcheringHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Butchering(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
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
	#3ez set jobs#0 - toggles your current location as a job listing and finding location";

	[PlayerCommand("EconomicZone", "economiczone", "ez")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
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
		@"This command is used to view and edit foragables. Foragables are records of items that can be loaded by the foraging system. The items need to be first built using the item system, and you also need to add the foragable record to a foragable profile before it will show up in the world. A single foragable record can be shared between multiple foragable profiles.

You can use the following options with this command:

    foragable list [+key, -key] - shows all the foragables
    foragable show <which> - shows a foragable
    foragable set ... - edits the properties of a foragable
    foragable edit <which> - opens a revision of a foragable
    foragable edit new - creates a new foragable
    foragable edit - an alias for foragable show on an opened foragable
    foragable edit close - closes the currently edited foragable
    foragable edit submit - submits the foragable for review
    foragable edit delete - deletes a non-approved revision
    foragable edit obsolete - marks a foragable as obsolete
    foragable review all|mine|<which> - opens a foragable for review
    foragable review list - shows all the foragables due to review
    foragable review history <which> - shows the history of a foragable";

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
				actor.OutputHandler.Send(ForagableHelpText);
				break;
		}
	}

	#endregion

	#region Foragable Profiles

	private const string ForagableProfileHelpText =
		@"This command is used to view and edit foragables profiles. Foragable profiles are attached typically to zones or terrain types, and control both what yield types appear in that location and what items can be foraged by players. See also the closely related FORAGABLE command, which you will also need to use.

You can use the following options with this command:

    fp list [+key, -key] - shows all the foragable profiles
    fp show <which> - shows a foragable profile
    fp set ... - edits the properties of a foragable profile
    fp edit <which> - opens a revision of a foragable profile
    fp edit new - creates a new foragable profile
    fp edit - an alias for fp show on an opened foragable profile
    fp edit close - closes the currently edited foragable profile
    fp edit submit - submits the foragable profile for review
    fp edit delete - deletes a non-approved revision
    fp edit obsolete - marks a foragable profile as obsolete
    fp review all|mine|<which> - opens a foragable profile for review
    fp review list - shows all the foragable profile due to review
    fp review history <which> - shows the history of a foragable profile";

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
				actor.OutputHandler.Send(ForagableProfileHelpText);
				break;
		}
	}

	#endregion
}