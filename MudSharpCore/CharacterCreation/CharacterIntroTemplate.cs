using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework.Save;
using static System.Net.Mime.MediaTypeNames;

namespace MudSharp.CharacterCreation;

public class CharacterIntroTemplate : SaveableItem, ICharacterIntroTemplate
{
	public ICharacterIntroTemplate Clone(string newName)
	{
		return new CharacterIntroTemplate(this, newName);
	}

	private CharacterIntroTemplate(CharacterIntroTemplate rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		AppliesToCharacterProg = rhs.AppliesToCharacterProg;
		ResolutionPriority = rhs.ResolutionPriority;
		Echoes.AddRange(rhs.Echoes);
		Delays.AddRange(rhs.Delays);
		using (new FMDB())
		{
			var dbitem = new Models.CharacterIntroTemplate
			{
				Name = Name,
				ResolutionPriority = ResolutionPriority,
				AppliesToCharacterProgId = AppliesToCharacterProg.Id,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.CharacterIntroTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public CharacterIntroTemplate(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		AppliesToCharacterProg = Gameworld.AlwaysFalseProg;
		ResolutionPriority = 0;
		using (new FMDB())
		{
			var dbitem = new Models.CharacterIntroTemplate
			{
				Name = Name,
				ResolutionPriority = ResolutionPriority,
				AppliesToCharacterProgId = AppliesToCharacterProg.Id,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.CharacterIntroTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public CharacterIntroTemplate(MudSharp.Models.CharacterIntroTemplate template, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = template.Id;
		_name = template.Name;
		AppliesToCharacterProg = gameworld.FutureProgs.Get(template.AppliesToCharacterProgId);
		ResolutionPriority = template.ResolutionPriority;
		var root = XElement.Parse(template.Definition);
		foreach (var item in root.Elements("Echo"))
		{
			Echoes.Add(item.Value);
			Delays.Add(TimeSpan.FromSeconds(double.Parse(item.Attribute("delay").Value)));
		}
	}

	public override string FrameworkItemType => "CharacterIntroTemplate";

	public bool AppliesToCharacter(ICharacterTemplate template)
	{
		return 
			Echoes.Count > 0 && (bool?)AppliesToCharacterProg.Execute(template) == true;
	}

	public bool AppliesToCharacter(ICharacter character)
	{
		return Echoes.Count > 0 && (bool?)AppliesToCharacterProg.Execute(character) == true;
	}

	public IFutureProg AppliesToCharacterProg { get; private set; }

	public int ResolutionPriority { get; private set; }

	public List<string> Echoes { get; } = new();

	public List<TimeSpan> Delays { get; } = new();

	public ICharacterIntro GetCharacterIntro()
	{
		return new CharacterIntro
		{
			Echoes = new Queue<string>(Echoes),
			Delays = new Queue<TimeSpan>(Delays)
		};
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.CharacterIntroTemplates.Find(Id);
		dbitem.Name = Name;
		dbitem.AppliesToCharacterProgId = AppliesToCharacterProg.Id;
		dbitem.ResolutionPriority = ResolutionPriority;
		dbitem.Definition = SaveDefinition().ToString();
		
		Changed = false;
	}

	private XElement SaveDefinition()
	{
		return new XElement("Definition",
			from item in Echoes.Zip(Delays, (e, d) => (Echo: e, Delay: d))
			select new XElement("Echo",
				new XAttribute("delay", item.Delay.TotalSeconds),
				new XCData(item.Echo)
			)
		);
	}

	public const string BuildHelpText = @"You can use the following options with this command:

	#3name <name>#0 - changes the name of this template
	#3priority <##>#0 - sets the evaluation priority when deciding which to apply (higher number = higher priority)
	#3prog <prog>#0 - sets the prog that controls whether this is a valid template for a character
	#3echo add <seconds>#0 - drops you into an editor to write a new echo that lasts for the specified seconds amount
	#3echo add <seconds> <text>#0 - directly enters an echo without going into the editor
	#3echo remove <##>#0 - permanently deletes an echo
	#3echo text <##>#0 - drops you into an editor to edit a specific echo
	#3echo text <##> <text>#0 - directly overwrites an echo without going into the editor
	#3echo delay <##> <seconds>#0 - adjusts the delay on an echo
	#3echo swap <##> <##>#0 - swaps the order of two echoes";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "prog":
				return BuildingCommandProg(actor, command);
			case "echo":
				return BuildingCommandEcho(actor, command);
		}

		actor.OutputHandler.Send(BuildHelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "create":
			case "new":
				return BuildingCommandEchoAdd(actor, command);
			case "swap":
				return BuildingCommandEchoSwap(actor, command);
			case "delete":
			case "del":
			case "remove":
			case "rem":
				return BuildingCommandEchoDelete(actor, command);
			case "delay":
				return BuildingCommandEchoDelay(actor, command);
			case "text":
				return BuildingCommandEchoText(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with echo:

	#3echo add <seconds>#0 - drops you into an editor to write a new echo that lasts for the specified seconds amount
	#3echo add <seconds> <text>#0 - directly enters an echo without going into the editor
	#3echo remove <##>#0 - permanently deletes an echo
	#3echo text <##>#0 - drops you into an editor to edit a specific echo
	#3echo text <##> <text>#0 - directly overwrites an echo without going into the editor
	#3echo delay <##> <seconds>#0 - adjusts the delay on an echo
	#3echo swap <##> <##>#0 - swaps the order of two echoes".SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandEchoText(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which echo do you want to edit the text of?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var pos1))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (pos1 < 1 || pos1 > Echoes.Count)
		{
			actor.OutputHandler.Send($"There is no echo at position {pos1.ToString("N0", actor).ColourValue()} to edit.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Enter the echo in the editor below:\n");
			actor.EditorMode((text, handler, _) =>
			{
				Echoes[pos1 - 1] = text;
				Changed = true;
				handler.Send($"\n\nYou change the {pos1.ToOrdinal().ColourValue()} echo to:\n\n{text.SubstituteANSIColour()}");
			}, (handler, _) =>
			{
				handler.Send("You decide not to change the text of the echo.");
			});
			return true;
		}

		Echoes[pos1 - 1] = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"\n\nYou change the {pos1.ToOrdinal().ColourValue()} echo to:\n\n{command.SafeRemainingArgument.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandEchoDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which echo do you want to edit the delay of?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var pos1))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (pos1 < 1 || pos1 > Echoes.Count)
		{
			actor.OutputHandler.Send($"There is no echo at position {pos1.ToString("N0", actor).ColourValue()} to edit.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should this echo be displayed for, in seconds?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number greater than zero.");
			return false;
		}

		var timeSpan = TimeSpan.FromSeconds(value);
		Delays[pos1 - 1] = timeSpan;
		Changed = true;
		actor.OutputHandler.Send($"The {pos1.ToOrdinal().ColourValue()} echo will now be displayed for {timeSpan.DescribePreciseBrief().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEchoDelete(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which echo do you want to edit the delay of?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var pos1))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (pos1 < 1 || pos1 > Echoes.Count)
		{
			actor.OutputHandler.Send($"There is no echo at position {pos1.ToString("N0", actor).ColourValue()} to edit.");
			return false;
		}

		var text = Echoes[pos1 - 1];
		Echoes.RemoveAt(pos1-1);
		Delays.RemoveAt(pos1 - 1);
		Changed = true;
		actor.OutputHandler.Send($"You remove the {pos1.ToOrdinal().ColourValue()} echo, which had the following text:\n\n{text.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandEchoSwap(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which two numbered echoes do you want to swap the position of?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var pos1))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the index of the second echo you want to swap the position of?");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var pos2))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (pos1 == pos2)
		{
			actor.OutputHandler.Send("You cannot swap an echo with itself.");
			return false;
		}

		if (pos1 < 1 || pos1 > Echoes.Count)
		{
			actor.OutputHandler.Send($"There is no echo at position {pos1.ToString("N0", actor).ColourValue()} to swap.");
			return false;
		}

		if (pos2 < 1 || pos2 > Echoes.Count)
		{
			actor.OutputHandler.Send($"There is no echo at position {pos2.ToString("N0", actor).ColourValue()} to swap.");
			return false;
		}

		Echoes.Swap(pos1 - 1, pos2 - 1);
		Delays.Swap(pos1 - 1, pos2 - 1);
		Changed = true;
		actor.OutputHandler.Send($"You swap the order of the {pos1.ToOrdinal().ColourValue()} and {pos2.ToOrdinal().ColourValue()} echo.");
		return true;
	}

	private bool BuildingCommandEchoAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should this echo be displayed for, in seconds?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number greater than zero.");
			return false;
		}

		var timeSpan = TimeSpan.FromSeconds(value);

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Enter the echo in the editor below:\n");
			actor.EditorMode((text, handler, _) =>
			{
				Echoes.Add(text);
				Delays.Add(timeSpan);
				Changed = true;
				handler.Send($"\n\nYou add the following echo for {timeSpan.DescribePreciseBrief().ColourValue()}:\n\n{text.SubstituteANSIColour()}");
			}, (handler, _) =>
			{
				handler.Send("You decide not to add a new echo.");
			});
			return true;
		}

		Echoes.Add(command.SafeRemainingArgument);
		Delays.Add(timeSpan);
		Changed = true;
		actor.OutputHandler.Send($"\n\nYou add the following echo for {timeSpan.DescribePreciseBrief().ColourValue()}:\n\n{command.SafeRemainingArgument.SubstituteANSIColour()}");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog do you want to set to control whether a character can have this template applied?");
			return false;
		}

		var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean, [FutureProgVariableTypes.Toon]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		AppliesToCharacterProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This character intro template will now use the {prog.MXPClickableFunctionName()} prog to determine whether it applies.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What priority should this template be for applying to a new character?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		ResolutionPriority = value;
		Changed = true;
		actor.OutputHandler.Send($"This character intro template will now be evaluated at priority {value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give ot this character intro template?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.CharacterIntroTemplates.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a character intro template named {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename this character intro template from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Character Intro #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Resolution Priority: {ResolutionPriority.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Applies Prog: {AppliesToCharacterProg.MXPClickableFunctionName()}");
		sb.AppendLine();
		sb.AppendLine($"Echoes:");
		foreach (var echo in Echoes.Zip(Delays, (e, d) => (Echo: e, Delay: d)))
		{
			sb.AppendLine();
			sb.AppendLine($"{echo.Delay.DescribePreciseBrief(actor).ColourValue()} -> {echo.Echo.SubstituteANSIColour()}");
		}
		return sb.ToString();
	}
}