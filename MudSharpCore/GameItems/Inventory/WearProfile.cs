using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Inventory;

/// <summary>
///     A DirectWearProfile is an implementation of IWearProfile that looks directly for specific wear locations when
///     finding matches
/// </summary>
public abstract class WearProfile : SaveableItem, IWearProfile
{
	public IBodyPrototype DesignedBody { get; protected set; }

	/// <summary>
	///     This is displayed at the start of the line when this wearprofile is worn in inventory, i.e. "worn on" for "worn on
	///     the left hand, and right hand" etc.
	/// </summary>
	public string WearStringInventory { get; protected set; }

	/// <summary>
	///     This is used in the resource when using the Wear command to wear
	/// </summary>
	public string WearAction1st { get; protected set; }

	/// <summary>
	///     This is used in the resource when using the Wear command to wear
	/// </summary>
	public string WearAction3rd { get; protected set; }

	public string WearAffix { get; protected set; }

	public string Description { get; protected set; }

	public abstract string Type { get; }

	public bool RequireContainerIsEmpty { get; protected set; }

	public abstract string ShowTo(ICharacter actor);
	public abstract Dictionary<IWear, IWearlocProfile>? Profile(IBody body);
	public abstract Dictionary<IWear, IWearlocProfile> AllProfiles { get; }

	public static IWearProfile LoadWearProfile(MudSharp.Models.WearProfile profile, IFuturemud game)
	{
		switch (profile.Type)
		{
			case "Direct":
				return new DirectWearProfile(profile, game);
			case "Shape":
				return new ShapeWearProfile(profile, game);
			default:
				throw new NotImplementedException();
		}
	}

	public abstract bool CompatibleWith(IWearProfile otherProfile);

	public abstract IWearProfile Clone(string newName);

	#region Building Commands

	protected abstract string SubtypeBuildingHelp { get; }

	public string BuildingHelp => $@"You can use the following options with this command:

	#3name <name>#0 - renames the profile
	#3description <desc>#0 - sets the description of the profile
	#3body <which>#0 - sets the body for which this wear profile is designed
	#3action1st <word>#0 - sets the 1st person verb for when worn
	#3action3rd <word>#0 - sets the 3rd person verb for when worn
	#3affix <word>#0 - sets the affix for how worn (on, over, around, etc)
	#3inv <phrase>#0 - sets the inventory description
	#3empty#0 - toggles whether this item needs to be empty to be worn (if container)
{SubtypeBuildingHelp}";

	public virtual void BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.Last.ToLowerInvariant())
		{
			case "name":
				BuildingCommandName(actor, command);
				return;
			case "description":
				BuildingCommandDescription(actor, command);
				return;
			case "wearaction3rd":
			case "wearaction3":
			case "wear3":
			case "wear3rd":
			case "action3":
			case "action3rd":
				BuildingCommandWearAction3rd(actor, command);
				return;
			case "wearaction1st":
			case "wearaction1":
			case "wear1":
			case "wear1st":
			case "action1":
			case "action1st":
				BuildingCommandWearAction1st(actor, command);
				return;
			case "wearaffix":
			case "affix":
				BuildingCommandWearAffix(actor, command);
				return;
			case "wearstringinventory":
			case "wearstring":
			case "string":
			case "inventory":
			case "inv":
				BuildingCommandWearStringInventory(actor, command);
				return;
			case "body":
				BuildingCommandBody(actor, command);
				return;
			case "empty":
				BuildingCommandEmpty(actor, command);
				return;
			default:
				actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
				return;
		}
	}

	private void BuildingCommandEmpty(ICharacter actor, StringStack command)
	{
		RequireContainerIsEmpty = !RequireContainerIsEmpty;
		Changed = true;
		actor.Send(
			$"This wearable will {(RequireContainerIsEmpty ? "now" : "no longer")} required that it is empty before being worn if it is a container.");
	}

	private void BuildingCommandBody(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which body do you want to set this wear profile for?");
			return;
		}

		var body = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.BodyPrototypes.Get(value)
			: Gameworld.BodyPrototypes.GetByName(command.Last);
		if (body == null)
		{
			actor.Send("There is no such body.");
			return;
		}

		DesignedBody = body;
		Changed = true;
		actor.Send($"This wear profile is now designed for the {body.Name.Colour(Telnet.Green)} body (ID #{body.Id}).");
	}

	private void BuildingCommandWearStringInventory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What wear string do you want to set? As an example, the default is \"worn on\".");
			return;
		}

		WearStringInventory = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) will now use the wear string \"{WearStringInventory}\" when displaying in inventory.");
	}

	private void BuildingCommandWearAffix(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What wear affix do you want to set? As an example, the default is \"on\".");
			return;
		}

		WearAffix = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) will now use the wear affix \"{WearAffix}\" when displaying in inventory.");
	}

	private void BuildingCommandWearAction1st(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What wear action do you want to set for the 1st person? As an example, the default is \"put\".");
			return;
		}

		WearAction1st = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) will now use the action description for 1st person of \"{WearAction1st}\" when being worn.");
	}

	private void BuildingCommandWearAction3rd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What wear action do you want to set for the 3rd person? As an example, the default is \"puts\".");
			return;
		}

		WearAction3rd = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.Send(
			$"Wear Profile {Id:N0} ({Name}) will now use the action description for 3rd person of \"{WearAction3rd}\" when being worn.");
	}

	private void BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How do you want to describe the Wear Profile?");
			return;
		}

		Description = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.Send($"Wear Profile {Id:N0} ({Name}) will now be described as: {Description}");
	}

	private void BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What do you want to rename this wear profile to?");
			return;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.WearProfiles.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a wear profile with that name. Wear profile names must be unique.");
			return;
		}

		_name = name;
		Changed = true;
		actor.Send($"Wear Profile {Id.ToString("N0", actor)} is now called {Name.ColourName()}.");
	}

	#endregion
}