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

namespace MudSharp.CharacterCreation;

public class CharacterIntroTemplate : SaveableItem, ICharacterIntroTemplate
{
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

	public const string BuildHelpText = @"";

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

		actor.OutputHandler.Send(@"".SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandEchoText(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandEchoDelay(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandEchoDelete(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandEchoSwap(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandEchoAdd(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
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