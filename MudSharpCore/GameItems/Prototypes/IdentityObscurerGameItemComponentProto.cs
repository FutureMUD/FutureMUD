using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class IdentityObscurerGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "IdentityObscurer";

	public string OverriddenShortDescription { get; protected set; }
	public string OverriddenFullDescription { get; protected set; }
	public Difficulty SeeThroughDisguiseDifficulty { get; protected set; }
	public string RemovalEcho { get; protected set; }

	private readonly Dictionary<string, string> _keywordSubstitutions = new();
	public IReadOnlyDictionary<string, string> KeywordSubstitutions => _keywordSubstitutions;

	protected Dictionary<ICharacteristicDefinition, string> _obscuredForms =
		new();

	public IEnumerable<ICharacteristicDefinition> ObscuredCharacteristics => _obscuredForms.Keys;

	public string GetDescription(ICharacteristicDefinition definition)
	{
		return _obscuredForms[definition];
	}

	#region Constructors

	protected IdentityObscurerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "IdentityObscurer")
	{
		RemovalEcho = Gameworld.GetStaticConfiguration("DefaultIdentityObscurerRemovalEcho");
		OverriddenShortDescription = Gameworld.GetStaticConfiguration("DefaultIdentityObscurerShortDescription");
		OverriddenFullDescription = Gameworld.GetStaticConfiguration("DefaultIdentityObscurerFullDescription");
		SeeThroughDisguiseDifficulty = Difficulty.Insane;
		_keywordSubstitutions["hood"] = "hooded";
		_keywordSubstitutions["helm"] = "helmed";
		_keywordSubstitutions["helmet"] = "helmeted";
		_keywordSubstitutions["mask"] = "masked";
		_keywordSubstitutions["scarf"] = "scarfed";
		_keywordSubstitutions["veil"] = "veiled";
		_keywordSubstitutions["hijab"] = "hijabed";
		_keywordSubstitutions["niqab"] = "niqabed";
		_keywordSubstitutions["burka"] = "burkad";
		_keywordSubstitutions["balaclava"] = "balaclavad";
	}

	protected IdentityObscurerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		RemovalEcho = root.Element("RemovalEcho").Value;
		OverriddenShortDescription = root.Element("ShortDescription").Value;
		OverriddenFullDescription = root.Element("FullDescription").Value;
		SeeThroughDisguiseDifficulty = (Difficulty)int.Parse(root.Element("Difficulty").Value);

		foreach (var sub in root.Element("Keywords").Elements())
		{
			_keywordSubstitutions[sub.Attribute("key").Value] = sub.Attribute("value").Value;
		}

		foreach (var sub in root.Element("Characteristics")?.Elements("Characteristic") ?? Enumerable.Empty<XElement>())
		{
			_obscuredForms.Add(Gameworld.Characteristics.Get(long.Parse(sub.Attribute("definition").Value)),
				sub.Attribute("form").Value);
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XElement("RemovalEcho", new XCData(RemovalEcho ?? "")),
				new XElement("ShortDescription", new XCData(OverriddenShortDescription)),
				new XElement("FullDescription", new XCData(OverriddenFullDescription)),
				new XElement("Difficulty", (int)SeeThroughDisguiseDifficulty),
				new XElement("Keywords",
					from keyword in _keywordSubstitutions
					select new XElement("Keyword", new XAttribute("key", keyword.Key),
						new XAttribute("value", keyword.Value)),
					from value in _obscuredForms
					select
						new XElement("Characteristic", new XAttribute("Form", value.Value),
							new XAttribute("Definition", value.Key.Id)))
			).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new IdentityObscurerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new IdentityObscurerGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("IdentityObscurer".ToLowerInvariant(), true,
			(gameworld, account) => new IdentityObscurerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("IdentityObscurer",
			(proto, gameworld) => new IdentityObscurerGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"IdentityObscurer",
			$"Disguises the identity of the wearer by changing SDesc. Must be combined with another {"[wearable]".Colour(Telnet.BoldYellow)} component",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new IdentityObscurerGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tadd <characteristic> <value> - adds a characteristic to be obscured\n\tremove <characteristic> - removes an obscured characteristic\n\tdifficulty <difficulty> - sets the difficulty to see through the disguise\n\tkeyword <keyword> <replace> - sets a keyword to be replaced in the @key and @sub tags.\n\tkeyword remove <keyword> - removes a keyword to be replaced\n\tsdesc <desc> - sets the override sdesc\n\tfdesc <desc> - set the override full description\n\techo <echo> - sets the removal echo addendum";

	public override string ShowBuildingHelp => BuildingHelpText;

	private bool BuildingCommand_Add(ICharacter actor, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		ICharacteristicDefinition definition = null;
		definition = long.TryParse(cmd, out var value)
			? actor.Gameworld.Characteristics.Get(value)
			: actor.Gameworld.Characteristics.Get(cmd).FirstOrDefault();

		if (definition == null)
		{
			actor.OutputHandler.Send(
				"Which characteristic definition do you want to add to this identity obscurer component?");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What obscured form should this characteristic take?");
			return false;
		}

		cmd = command.SafeRemainingArgument;

		_obscuredForms[definition] = cmd;
		actor.OutputHandler.Send(
			$"You set this item to obscure the {definition.Name.ColourName()} definition with a value of {cmd.ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Remove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic definition do you wish to remove?");
			return false;
		}

		var cmd = command.SafeRemainingArgument;
		var definition =
			_obscuredForms.FirstOrDefault(x => x.Key.Name.StartsWith(cmd, StringComparison.CurrentCultureIgnoreCase));
		if (definition.Key == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition to remove.");
			return false;
		}

		_obscuredForms.Remove(definition.Key);
		actor.OutputHandler.Send(
			$"You remove the {definition.Key.Name.ColourName()} characteristic definition from this identity obscurer component.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_RemovalEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			RemovalEcho = string.Empty;
			actor.OutputHandler.Send("You remove the echo upon removal of this obscuring item.");
		}
		else
		{
			RemovalEcho = command.RemainingArgument;
			actor.OutputHandler.Send("Upon removal, this item will display the following echo: " + RemovalEcho);
		}

		return true;
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "removalecho":
			case "removal echo":
			case "echo":
				return BuildingCommand_RemovalEcho(actor, command);
			case "add":
			case "set":
			case "update":
				return BuildingCommand_Add(actor, command);
			case "remove":
			case "delete":
			case "rem":
			case "del":
				return BuildingCommand_Remove(actor, command);
			case "sdesc":
			case "short":
			case "shortdesc":
			case "shortdescription":
			case "short_desc":
			case "short desc":
			case "short description":
			case "short_description":
				return BuildingCommandSDesc(actor, command);
			case "full":
			case "full description":
			case "full desc":
			case "fulldescription":
			case "fulldesc":
			case "full_description":
			case "full_desc":
				return BuildingCommandDesc(actor, command);
			case "keyword":
			case "sub":
			case "substitution":
			case "key":
				return BuildingCommandKeyword(actor, command);
			case "difficulty":
			case "diff":
				return BuildingCommandDifficulty(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What difficulty should it be for observes to see through this disguise and see a person's real identity?");
			return false;
		}

		if (!Utilities.TryParseEnum<Difficulty>(command.PopSpeech(), out var difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		SeeThroughDisguiseDifficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send(
			$"It will now be {SeeThroughDisguiseDifficulty.Describe().ColourValue()} to determine the identity of a person wearing this item.");
		return true;
	}

	private bool BuildingCommandKeyword(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must either specify a keyword and a substitute word, or use REMOVE <keyword>.");
			return false;
		}

		if (command.Peek().EqualToAny("remove", "rem", "delete", "del"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which keyword did you want to remove?");
				return false;
			}

			var keyword = command.PopSpeech();
			if (!_keywordSubstitutions.ContainsKey(keyword))
			{
				actor.OutputHandler.Send("There is no such keyword to remove.");
				return false;
			}

			_keywordSubstitutions.Remove(keyword);
			Changed = true;
			actor.OutputHandler.Send(
				$"You remove the keyword substitution for the '{keyword.ColourCommand()}' keyword.");
			return true;
		}

		var newKeyword = command.PopSpeech().ToLowerInvariant();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What substitution do you want to make for the {newKeyword.ColourCommand()} keyword?");
			return false;
		}

		_keywordSubstitutions[newKeyword] = command.PopSpeech().ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send(
			$"The keyword {newKeyword.ColourCommand()} will now be replaced by {_keywordSubstitutions[newKeyword].ColourCommand()} in the overridden descriptions using the @sub tag.");
		return true;
	}

	private bool BuildingCommandDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the overriden description of someone using this obscurer be?");
			return false;
		}

		OverriddenFullDescription = command.SafeRemainingArgument.Trim().ProperSentences().Fullstop();
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the overriden description for this item to:\n\n{OverriddenFullDescription.Wrap(actor.InnerLineFormatLength, "  ")}");
		return true;
	}

	private bool BuildingCommandSDesc(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the short description for someone wearing this item be?");
			return false;
		}

		OverriddenShortDescription = command.SafeRemainingArgument.Trim();
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the overriden short description for this item to: {OverriddenShortDescription.ColourCharacter()}.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nIt is an identity obscurer when worn. It is {4} to see through.\nRemoval Echo: {5}\nSDesc Override: {6}\nFDesc Override: {7}\nObscures: {8}\nKeyword Substitions: {9}.",
			"IdentityObscurer Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			SeeThroughDisguiseDifficulty.Describe().ColourValue(),
			RemovalEcho.ColourCommand(),
			OverriddenShortDescription.ColourCharacter(),
			OverriddenFullDescription.ColourCommand(),
			(from obscure in _obscuredForms select $"{obscure.Key.Name.ColourName()}: {obscure.Value.ColourValue()}")
			.ListToString(),
			(from sub in _keywordSubstitutions select $"{sub.Key.ColourName()}->{sub.Value.ColourValue()}")
			.ListToString()
		);
	}
}