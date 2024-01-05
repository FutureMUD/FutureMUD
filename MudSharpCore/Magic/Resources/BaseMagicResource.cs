using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.NPC;

namespace MudSharp.Magic.Resources;

public abstract class BaseMagicResource : SaveableItem, IMagicResource, IHaveFuturemud
{
	public static IMagicResource CreateResourceFromBuilderInput(ICharacter actor, StringStack input)
	{

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What type of resource do you want to create? The valid options are #6simple#0.");
			return null;
		}

		var type = input.PopSpeech().ToLowerInvariant();
		switch (type)
		{
			case "simple":
				break;
			default:
				actor.OutputHandler.Send("That is not a valid type of resource. The valid options are #6simple#0.");
				return null;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a name for your new magic resource.");
			return null;
		}

		var name = input.PopSpeech().TitleCase();
		if (actor.Gameworld.MagicResources.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a magic resource called {name.ColourName()}. Names must be unique.");
			return null;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a short name for your new magic resource.");
			return null;
		}

		var shortName = input.PopSpeech().TitleCase();
		if (actor.Gameworld.MagicResources.Any(x => x.ShortName.EqualTo(shortName)))
		{
			actor.OutputHandler.Send($"There is already a magic resource with a short name of {shortName.ColourName()}. Short names must be unique.");
			return null;
		}

		switch (type)
		{
			case "simple":
				return new SimpleMagicResource(actor.Gameworld, name, shortName);
		}
		return null;
	}


	public static IMagicResource LoadResource(Models.MagicResource resource, IFuturemud gameworld)
	{
		switch (resource.Type.ToLowerInvariant())
		{
			case "simple":
				return new SimpleMagicResource(resource, gameworld);
		}

		throw new NotImplementedException("Unknown MagicResource type: " + resource.Type);
	}

	protected BaseMagicResource(Models.MagicResource resource, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = resource.Id;
		_name = resource.Name;
		ResourceType = (MagicResourceType)resource.MagicResourceType;
		ShortName = resource.ShortName ?? Name;
		BottomColour = resource.BottomColour;
		MidColour = resource.MidColour;
		TopColour = resource.TopColour;
	}

	protected BaseMagicResource(BaseMagicResource rhs, string name, string shortname)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		ShortName = shortname;
		ResourceType = rhs.ResourceType;
		BottomColour = rhs.BottomColour;
		MidColour = rhs.MidColour;
		TopColour = rhs.TopColour;
	}

	protected BaseMagicResource(IFuturemud gameworld, string name, string shortName)
	{
		Gameworld = gameworld;
		_name = name;
		ShortName = shortName;
		ResourceType = MagicResourceType.PlayerResource | MagicResourceType.ItemResource | MagicResourceType.LocationResource;
		BottomColour = Telnet.Magenta.Colour;
		MidColour = Telnet.BoldMagenta.Colour;
		TopColour = Telnet.BoldPink.Colour;
	}

	#region Overrides of Item

	public sealed override string FrameworkItemType => "MagicResource";

	#endregion

	#region Implementation of IEditableItem
	public string HelpText => @$"You can use the following options with this magic resource type:

	#3name <name>#0 - sets a new name
	#3short <name>#0 - sets a short name or alias
	#3bottom <colour>#0 - sets the colour for the bottom 2 bars of the classic prompt
	#3mid <colour>#0 - sets the colour for the middle 2 bars of the classic prompt
	#3top <colour>#0 - sets the colour for the top 2 bars of the classic prompt
{SubtypeHelpText}";
	protected abstract string SubtypeHelpText { get; }
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "shortname":
			case "alias":
			case "short":
				return BuildingCommandShortName(actor, command);
			case "bottom":
			case "bottomcolour":
			case "bottomcolor":
			case "colourbottom":
			case "colorbottom":
				return BuildingCommandColourBottom(actor, command);
			case "mid":
			case "midcolour":
			case "midcolor":
			case "colourmid":
			case "colormid":
				return BuildingCommandColourMid(actor, command);
			case "top":
			case "topcolour":
			case "topcolor":
			case "colourtop":
			case "colortop":
				return BuildingCommandColourTop(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this magic resource?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MagicResources.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a magic resource with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this magic resource from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandShortName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new short name do you want to give to this magic resource?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MagicResources.Any(x => x.ShortName.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a magic resource with that short name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You change the short name this magic resource from {ShortName.ColourName()} to {name.ColourName()}.");
		ShortName = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandColourTop(ICharacter actor, StringStack command)
	{
		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"That is not a valid colour option. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		TopColour = colour.Colour;
		Changed = true;
		actor.OutputHandler.Send($"The classic prompt for this resource now looks like this: {ClassicPromptString(1.0)}.");
		return true;
	}

	private bool BuildingCommandColourMid(ICharacter actor, StringStack command)
	{
		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"That is not a valid colour option. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		MidColour = colour.Colour;
		Changed = true;
		actor.OutputHandler.Send($"The classic prompt for this resource now looks like this: {ClassicPromptString(1.0)}.");
		return true;
	}

	private bool BuildingCommandColourBottom(ICharacter actor, StringStack command)
	{
		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour is null)
		{
			actor.OutputHandler.Send($"That is not a valid colour option. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		BottomColour = colour.Colour;
		Changed = true;
		actor.OutputHandler.Send($"The classic prompt for this resource now looks like this: {ClassicPromptString(1.0)}.");
		return true;
	}

	public abstract string Show(ICharacter actor);
	#endregion

	#region Implementation of IMagicResource

	public MagicResourceType ResourceType { get; set; }
	public string ShortName { get; set; }
	public abstract bool ShouldStartWithResource(IHaveMagicResource thing);
	public abstract double StartingResourceAmount(IHaveMagicResource thing);
	public abstract double ResourceCap(IHaveMagicResource thing);
	public string BottomColour { get; set; } = Telnet.Magenta.Colour;
	public string MidColour { get; set; } = Telnet.BoldMagenta.Colour;
	public string TopColour { get; set; } = Telnet.BoldPink.Colour;

	public virtual string ClassicPromptString(double percentage)
	{
		return percentage switch
		{
			<= 0.0 => $"      ",
			<= 0.1667 => $"{BottomColour}|     {Telnet.RESET}",
			<= 0.3333 => $"{BottomColour}||    {Telnet.RESET}",
			<= 0.5 => $"{BottomColour}||{Telnet.RESET}{MidColour}|   {Telnet.RESET}",
			<= 0.6667 => $"{BottomColour}||{Telnet.RESET}{MidColour}||  {Telnet.RESET}",
			<= 0.8335 => $"{BottomColour}||{Telnet.RESET}{MidColour}||{Telnet.RESET}{TopColour}| {Telnet.RESET}",
			_ => $"{BottomColour}||{Telnet.RESET}{MidColour}||{Telnet.RESET}{TopColour}||{Telnet.RESET}"
		};
	}

	public abstract IMagicResource Clone(string newName, string newShortName);



	public sealed override void Save()
	{
		var dbitem = FMDB.Context.MagicResources.Find(Id);
		dbitem.Name = Name;
		dbitem.ShortName = ShortName;
		dbitem.TopColour = TopColour;
		dbitem.MidColour = MidColour;
		dbitem.BottomColour = BottomColour;
		dbitem.MagicResourceType = (int)ResourceType;
		dbitem.Definition = SaveDefinition();
		Changed = false;
	}

	protected abstract string SaveDefinition();
	#endregion
}