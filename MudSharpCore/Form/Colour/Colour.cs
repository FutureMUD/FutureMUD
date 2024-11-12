using System;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Form.Colour;

public class Colour : SaveableItem, IColour
{
	public bool _isValid;

	public Colour(MudSharp.Models.Colour proto, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = proto.Id;
		_name = proto.Name;
		Red = proto.Red;
		Green = proto.Green;
		Blue = proto.Blue;
		Basic = (BasicColour)proto.Basic;
		Fancy = string.IsNullOrEmpty(proto.Fancy) ? _name : proto.Fancy;
	}

	public Colour(IFuturemud gameworld, string name, BasicColour colour)
	{
		Gameworld = gameworld;
		_name = name;
		Basic = colour;
		(Red, Green, Blue) = colour.GetRGB();
		Fancy = name;
		using (new FMDB())
		{
			var dbitem = new Models.Colour
			{
				Name = Name,
				Basic = (int)Basic,
				Blue = Blue,
				Red = Red,
				Green = Green,
				Fancy = Fancy
			};
			FMDB.Context.Colours.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private Colour(Colour rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		Basic = rhs.Basic;
		Red = rhs.Red;
		Blue = rhs.Blue;
		Green = rhs.Green;
		Fancy = rhs.Fancy;
		using (new FMDB())
		{
			var dbitem = new Models.Colour
			{
				Name = Name,
				Basic = (int)Basic,
				Blue = Blue,
				Red = Red,
				Green = Green,
				Fancy = Fancy
			};
			FMDB.Context.Colours.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IColour Clone(string newName)
	{
		return new Colour(this, newName);
	}

	public override string FrameworkItemType => "Colour";

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Colours.Find(Id);
		dbitem.Name = Name;
		dbitem.Red = Red;
		dbitem.Green = Green;
		dbitem.Blue = Blue;
		dbitem.Basic = (int)Basic;
		dbitem.Fancy = Fancy;
		Changed = false;
	}

	/// <summary>
	///     Returns the Red component of the RGB value of this colour
	/// </summary>
	public int Red { get; protected set; }

	/// <summary>
	///     Returns the Green component of the RGB value of this colour
	/// </summary>
	public int Green { get; protected set; }

	/// <summary>
	///     Returns the Blue component of the RGB value of this colour
	/// </summary>
	public int Blue { get; protected set; }

	/// <summary>
	///     Returns the BasicColour equivalent of this colour. For instance, "Navy Blue" would return BasicColour.Blue
	/// </summary>
	public BasicColour Basic { get; protected set; }

	/// <summary>
	///     A fancified string version of the colour, for example, "Midnight Black" might have "The colour of a starless
	///     midnight sky" as its Fancy.
	/// </summary>
	public string Fancy { get; protected set; }

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames the colour
	#3basic <colour>#0 - sets the equivalent basic colour (red, green, yellow, etc)
	#3fancy <fancy>#0 - sets the fancy description of this colour
	#3red <0-255>#0 - sets the red value of this colour
	#3green <0-255>#0 - sets the green value of this colour
	#3blue <0-255>#0 - sets the blue value of this colour";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "basic":
				return BuildingCommandBasic(actor, command);
			case "fancy":
				return BuildingCommandFancy(actor, command);
			case "red":
				return BuildingCommandRed(actor, command);
			case "green":
				return BuildingCommandGreen(actor, command);
			case "blue":
				return BuildingCommandBlue(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this colour?");
			return false;
		}

		var name = command.SafeRemainingArgument.ToLowerInvariant();
		if (Gameworld.Colours.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a colour called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the colour {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandBasic(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which basic colour should represent this colour? The choices are {Enum.GetValues<BasicColour>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<BasicColour>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid colour. The choices are {Enum.GetValues<BasicColour>().ListToColouredString()}.");
			return false;
		}

		Basic = value;
		Changed = true;
		actor.OutputHandler.Send($"The basic form of this colour is now {value.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandFancy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the fancy form of this colour's description? Usually this begins with #3a#0, #3an#0 or #3the#0.".SubstituteANSIColour());
			return false;
		}

		Fancy = command.SafeRemainingArgument;
		actor.OutputHandler.Send($"The fancy form of this colour is now {Fancy.ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandRed(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a value between 0 and 255 for the red value.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0 || value > 255)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a number between 0 and 255.");
			return false;
		}

		Red = value;
		Changed = true;
		actor.OutputHandler.Send($"The red value for this colour is now {value.ToStringN0Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandGreen(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a value between 0 and 255 for the green value.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0 || value > 255)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a number between 0 and 255.");
			return false;
		}

		Green = value;
		Changed = true;
		actor.OutputHandler.Send($"The green value for this colour is now {value.ToStringN0Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandBlue(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must enter a value between 0 and 255 for the blue value.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0 || value > 255)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a number between 0 and 255.");
			return false;
		}

		Blue = value;
		Changed = true;
		actor.OutputHandler.Send($"The blue value for this colour is now {value.ToStringN0Colour(actor)}.");
		return true;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Colour #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Basic: {Basic.Describe().ColourValue()}");
		sb.AppendLine($"Fancy: {Fancy.ColourCommand()}");
		sb.AppendLine($"Red: {Red.ToStringN0Colour(actor)}");
		sb.AppendLine($"Green: {Green.ToStringN0Colour(actor)}");
		sb.AppendLine($"Blue: {Blue.ToStringN0Colour(actor)}");
		sb.AppendLine($"As ANSI Colour: {Name.Colour(Red, Green, Blue)}");
		return sb.ToString();
	}
}