using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class ObscurerGameItemComponentProto : GameItemComponentProto
{
	protected Dictionary<ICharacteristicDefinition, string> _obscuredForms =
		new();

	protected ObscurerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected ObscurerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Obscurer")
	{
	}

	/// <summary>
	///     An IEnumerable with all of the Characteristics which are obscured by this Obscurer
	/// </summary>
	public IEnumerable<ICharacteristicDefinition> ObscuredCharacteristics => _obscuredForms.Keys;

	public string RemovalEcho { get; set; }
	public override string TypeDescription => "Obscurer";

	/// <summary>
	///     Asks for the string pattern associated with a particular definition
	/// </summary>
	/// <param name="definition"></param>
	/// <returns></returns>
	public string GetDescription(ICharacteristicDefinition definition)
	{
		return _obscuredForms[definition];
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{3:N0}r{4:N0}, {5})\n\nIt obscures the following variables: {1}\n\nWhen removed, it adds the following emote to the remove echo: {2}",
			"Obscurer Item Component".Colour(Telnet.Cyan),
			(from def in _obscuredForms select def.Key.Description + " with a value of " + def.Value).ListToString(),
			RemovalEcho,
			Id,
			RevisionNumber,
			Name
		);
	}

	private bool BuildingCommand_Add(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		ICharacteristicDefinition definition = null;
		definition = long.TryParse(cmd, out var value)
			? actor.Gameworld.Characteristics.Get(value)
			: actor.Gameworld.Characteristics.Get(cmd).FirstOrDefault();

		if (definition == null)
		{
			actor.OutputHandler.Send(
				"Which characteristic definition do you want to add to this obscurer component?");
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
			$"You set this item to obscure the {definition.Name.ColourValue()} definition with a value of {cmd.ColourCommand()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_Remove(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		if (string.IsNullOrEmpty(cmd))
		{
			actor.OutputHandler.Send("Which characteristic definition do you wish to remove?");
			return false;
		}

		var definition =
			_obscuredForms.FirstOrDefault(x => x.Key.Name.StartsWith(cmd, StringComparison.CurrentCultureIgnoreCase));
		if (definition.Key == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition to remove.");
			return false;
		}

		_obscuredForms.Remove(definition.Key);
		actor.OutputHandler.Send("You remove the " + definition.Key.Name +
		                         " characteristic definition from this obscurer component.");
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
			RemovalEcho = command.SafeRemainingArgument;
			actor.OutputHandler.Send(
				$"Upon removal, this item will display the following echo: {RemovalEcho.ColourCommand()}");
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
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tadd <which> <description> - adds a characteristic to obscure with a particular new description\n\tremove <definition> - no longer obscures the specified definition\n\techo <emote> - sets the echo addendum for when someone removes this obscurer";

	public override string ShowBuildingHelp => BuildingHelpText;

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("RemovalEcho");
		if (element != null)
		{
			RemovalEcho = element.Value;
		}

		foreach (var sub in root.Elements("Characteristic"))
		{
			_obscuredForms.Add(Gameworld.Characteristics.Get(long.Parse(sub.Attribute("Definition").Value)),
				sub.Attribute("Form").Value);
		}
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("obscurer", true,
			(gameworld, account) => new ObscurerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Obscurer",
			(proto, gameworld) => new ObscurerGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Obscurer",
			$"Obscures some characteristics of an individual, combined with a {"[wearable]".Colour(Telnet.BoldYellow)}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ObscurerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ObscurerGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ObscurerGameItemComponentProto(proto, gameworld));
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XElement("RemovalEcho", new XCData(RemovalEcho ?? "")),
				from value in _obscuredForms
				select
					new XElement("Characteristic", new XAttribute("Form", value.Value),
						new XAttribute("Definition", value.Key.Id))).ToString();
	}
}