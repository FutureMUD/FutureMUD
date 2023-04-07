using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class VariableGameItemComponentProto : GameItemComponentProto
{
	protected readonly Dictionary<ICharacteristicDefinition, ICharacteristicProfile> _characteristicDefinitions =
		new();

	protected VariableGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type = "Variable")
		: base(gameworld, originator, type)
	{
	}

	protected VariableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IEnumerable<ICharacteristicDefinition> CharacteristicDefinitions => _characteristicDefinitions.Keys;
	public override string TypeDescription => "Variable";

	public ICharacteristicProfile ProfileFor(ICharacteristicDefinition definition)
	{
		return _characteristicDefinitions[definition];
	}

	public Dictionary<ICharacteristicDefinition, ICharacteristicValue> GetRandomValues()
	{
		var prePopulatedVariables = new Dictionary<ICharacteristicDefinition, ICharacteristicValue>();
		foreach (var definition in CharacteristicDefinitions)
		{
			prePopulatedVariables[definition] = ProfileFor(definition).GetRandomCharacteristic();
		}

		return prePopulatedVariables;
	}

	public Dictionary<ICharacteristicDefinition, ICharacteristicValue> GetValuesFromString(string paramString)
	{
		var prePopulatedVariables = new Dictionary<ICharacteristicDefinition, ICharacteristicValue>();
		var regex = new Regex("(\\w+)(?:=|\\:)(\"(:?[\\w ]+)\"|(:?[\\w]+))");
		foreach (Match match in regex.Matches(paramString))
		{
			ICharacteristicDefinition definition = null;
			ICharacteristicValue cvalue = null;

			definition =
				CharacteristicDefinitions.FirstOrDefault(x => x.Pattern.IsMatch(match.Groups[1].Value));
			if (definition == null)
			{
				continue;
			}

			var target = !string.IsNullOrEmpty(match.Groups[3].Value)
				? match.Groups[3].Value
				: match.Groups[4].Value;
			long valueid;
			if (target[0] == ':')
			{
				ICharacteristicProfile profile = null;
				profile = long.TryParse(target.Substring(1), out valueid)
					? Gameworld.CharacteristicProfiles.Get(valueid)
					: Gameworld.CharacteristicProfiles.FirstOrDefault(
						x => x.Name.StartsWith(target.Substring(1), StringComparison.InvariantCultureIgnoreCase));

				if (profile == null)
				{
					continue;
				}

				cvalue = profile.GetRandomCharacteristic();
			}
			else
			{
				cvalue = long.TryParse(target, out valueid)
					? Gameworld.CharacteristicValues.Get(valueid)
					: Gameworld.CharacteristicValues.Where(x => definition.IsValue(x))
					           .FirstOrDefault(
						           x => x.Name.StartsWith(target, StringComparison.InvariantCultureIgnoreCase));
			}

			if (cvalue == null)
			{
				continue;
			}

			if (!definition.IsValue(cvalue))
			{
				continue;
			}

			prePopulatedVariables.Add(definition, cvalue);
		}

		foreach (
			var definition in CharacteristicDefinitions.Where(x => !prePopulatedVariables.ContainsKey(x)).ToList())
		{
			prePopulatedVariables[definition] = ProfileFor(definition).GetRandomCharacteristic();
		}

		return prePopulatedVariables;
	}

	public void ExpireDefinition(ICharacteristicDefinition definition)
	{
		if (_characteristicDefinitions.ContainsKey(definition))
		{
			_characteristicDefinitions.Remove(definition);
			Changed = true;
		}
	}

	public void ExpireProfile(ICharacteristicProfile profile)
	{
		foreach (var definition in _characteristicDefinitions.Where(x => x.Value == profile).ToList())
		{
			_characteristicDefinitions.Remove(definition.Key);
			Changed = true;
		}
	}

	protected override void LoadFromXml(XElement root)
	{
		foreach (var element in root.Elements("Characteristic"))
		{
			var definition = Gameworld.Characteristics.Get(long.Parse(element.Attribute("Value").Value));
			var profile = Gameworld.CharacteristicProfiles.Get(long.Parse(element.Attribute("Profile").Value));
			if (definition != null && profile != null)
			{
				_characteristicDefinitions.Add(definition, profile);
			}
		}
	}

	private bool BuildingCommand_VariableAdd(ICharacter actor, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		ICharacteristicDefinition definition = null;
		definition = long.TryParse(cmd, out var value)
			? actor.Gameworld.Characteristics.Get(value)
			: actor.Gameworld.Characteristics.Get(cmd).FirstOrDefault();

		if (definition == null)
		{
			actor.OutputHandler.Send(
				"Which characteristic definition do you want to add to this variable component?");
			return false;
		}

		cmd = command.PopSpeech().ToLowerInvariant();
		ICharacteristicProfile profile = null;
		profile = long.TryParse(cmd, out value)
			? actor.Gameworld.CharacteristicProfiles.Get(value)
			: actor.Gameworld.CharacteristicProfiles.Get(cmd).FirstOrDefault();

		if (profile == null)
		{
			actor.OutputHandler.Send("Which characteristic profile do you want to add to this variable component?");
			return false;
		}

		_characteristicDefinitions[definition] = profile;
		actor.OutputHandler.Send("You add the " + definition.Name + " definition with a profile of " + profile.Name +
		                         " to this variable component. Use the pattern " +
		                         definition.Pattern.ToString().Colour(Telnet.Yellow) +
		                         " in a description to use this variable.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_VariableRemove(ICharacter actor, StringStack command)
	{
		var cmd = command.PopSpeech().ToLowerInvariant();
		if (string.IsNullOrEmpty(cmd))
		{
			actor.OutputHandler.Send("Which characteristic definition do you wish to remove?");
			return false;
		}

		var definition =
			_characteristicDefinitions.FirstOrDefault(
				x => x.Key.Name.StartsWith(cmd, StringComparison.CurrentCultureIgnoreCase));
		if (definition.Key == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition to remove.");
			return false;
		}

		_characteristicDefinitions.Remove(definition.Key);
		actor.OutputHandler.Send("You remove the " + definition.Key.Name +
		                         " characteristic definition from this variable component.");
		Changed = true;
		return true;
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tvariable add <which> <profile> - adds a variable with the specified random profile\n\tvariable remove <which> - removes a variable";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		switch (cmd)
		{
			case "variable":
			case "var":
				switch (command.Pop().ToLowerInvariant())
				{
					case "add":
					case "set":
						return BuildingCommand_VariableAdd(actor, command);
					case "remove":
					case "delete":
					case "rem":
					case "del":
						return BuildingCommand_VariableRemove(actor, command);
					default:
						actor.OutputHandler.Send(
							"Do you want to add or remove a variable from this variable component?");
						return false;
				}
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{2:N0}r{3:N0}, {4})\n\nThis is a variable item for the following variables: \n\t{1}",
			"Variable Item Component".Colour(Telnet.Cyan),
			(from def in _characteristicDefinitions
			 select
				 $"{def.Key.Name.Proper().Colour(Telnet.Green)} with the {def.Value.Name.Colour(Telnet.Green)} profile")
			.ListToString(separator: "\n\t", conjunction: "",
				twoItemJoiner: "\n\t"),
			Id,
			RevisionNumber,
			Name
		);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("variable", true,
			(gameworld, account) => new VariableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Variable",
			(proto, gameworld) => new VariableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Variable",
			$"Gives an item {"[characteristics]".Colour(Telnet.Yellow)} (a.k.a. variables) that can be randomised.",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new VariableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VariableGameItemComponent(component, this, parent);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new object[]
		{
			from value in _characteristicDefinitions
			select
				new XElement("Characteristic", new XAttribute("Profile", value.Value.Id),
					new XAttribute("Value", value.Key.Id))
		}).ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new VariableGameItemComponentProto(proto, gameworld));
	}
}