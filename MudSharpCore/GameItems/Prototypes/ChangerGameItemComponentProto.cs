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
using MudSharp.GameItems.Inventory;

namespace MudSharp.GameItems.Prototypes;

public class ChangerGameItemComponentProto : GameItemComponentProto
{
	protected readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> _values =
		new();

	protected ChangerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Changer")
	{
	}

	protected ChangerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	/// <summary>
	///     If not null, this specifies a Wear Profile which the item must be worn in to function as a Characteristic Changer
	/// </summary>
	public IWearProfile TargetWearProfile { get; protected set; }

	/// <summary>
	///     Specifies which ICharacteristicDefinitions are changed by this Characteristic Changer
	/// </summary>
	public IEnumerable<ICharacteristicDefinition> Definitions => _values.Keys;

	public override string TypeDescription => "Changer";

	/// <summary>
	///     Requests the CharacteristicValue associated with this Characteristic Changer
	/// </summary>
	/// <param name="definition"></param>
	/// <returns></returns>
	public ICharacteristicValue ValueFor(ICharacteristicDefinition definition)
	{
		return _values[definition];
	}

	protected override void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("TargetWearProfile");
		if (attr != null && attr.Value.Length > 0)
		{
			TargetWearProfile = Gameworld.WearProfiles.Get(long.Parse(attr.Value));
		}

		foreach (var value in root.Elements("Value"))
		{
			_values.Add(Gameworld.Characteristics.Get(long.Parse(value.Attribute("Definition").Value)),
				Gameworld.CharacteristicValues.Get(long.Parse(value.Attribute("Target").Value)));
		}
	}

	private bool BuildingCommand_TargetWearProfile(ICharacter actor, StringStack command)
	{
		var arg = command.PopSpeech();

		if (arg.Length == 0)
		{
			TargetWearProfile = null;
			actor.OutputHandler.Send("This Changer will no longer target any Wear Profiles.");
			Changed = true;
			return true;
		}

		IWearProfile profile = null;
		profile = long.TryParse(arg, out var value)
			? Gameworld.WearProfiles.Get(value)
			: Gameworld.WearProfiles.Get(arg).FirstOrDefault();

		if (profile == null)
		{
			actor.OutputHandler.Send("There is no such Wear Profile.");
			return false;
		}

		TargetWearProfile = profile;
		actor.OutputHandler.Send("You change the Target Wear Profile to " + profile.Name + " [#" + profile.Id + "].");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_SetValue(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		ICharacteristicDefinition definition = null;
		definition = actor.Gameworld.Characteristics.GetByIdOrName(cmd);

		if (definition == null)
		{
			actor.OutputHandler.Send("Which characteristic definition do you want to add to this changer component?");
			return false;
		}

		cmd = command.Pop().ToLowerInvariant();
		var cvalue = actor.Gameworld.CharacteristicValues.GetByIdOrName(cmd);

		if (cvalue == null)
		{
			actor.OutputHandler.Send("Which characteristic value do you want to add to this changer component?");
			return false;
		}

		if (!definition.IsValue(cvalue))
		{
			actor.OutputHandler.Send("That is not a valid value for that variable definition.");
			return false;
		}

		_values[definition] = cvalue;
		actor.OutputHandler.Send("You add the " + definition.Name + " definition with a value of " + cvalue.Name +
		                         " to this changer component.");
		Changed = true;
		return true;
	}

	private bool BuildingCommand_RemoveValue(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		if (string.IsNullOrEmpty(cmd))
		{
			actor.OutputHandler.Send("Which characteristic definition do you wish to remove?");
			return false;
		}

		var definition =
			_values.FirstOrDefault(x => x.Key.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));
		if (definition.Key == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition to remove.");
			return false;
		}

		_values.Remove(definition.Key);
		actor.OutputHandler.Send("You remove the " + definition.Key.Name +
		                         " characteristic definition from this changer component.");
		Changed = true;
		return true;
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\ttarget <profile> - requires a particular wear profile to be in effect\n\ttarget - removes a wear profile requirement\n\tadd <definition> <value> - adds a characteristic that this item changes\n\tremove <definition> - removes a characteristic from this item";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var cmd = command.Pop().ToLowerInvariant();
		switch (cmd)
		{
			case "target":
				return BuildingCommand_TargetWearProfile(actor, command);
			case "set":
			case "add":
				return BuildingCommand_SetValue(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommand_RemoveValue(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{3:N0}r{4:N0}, {5})\n\nWhen worn{1}, this item {2}",
			"Changer Item Component".Colour(Telnet.Cyan),
			TargetWearProfile != null
				? string.Format(actor, "in the {0} (#{1:N0}) wear profile", TargetWearProfile.Name,
					TargetWearProfile.Id)
				: "",
			_values.Any()
				? $"changes the values of {(from value in _values select value.Key.Name + " to " + value.Value.Name).ListToString()}."
				: "does not change any values.",
			Id,
			RevisionNumber,
			Name
		);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("changer", true,
			(gameworld, account) => new ChangerGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Changer",
			(proto, gameworld) => new ChangerGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Changer",
			$"Changes a character's characteristics when worn, combined with a {"[wearable]".Colour(Telnet.BoldYellow)}",
			BuildingHelpText
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ChangerGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ChangerGameItemComponent(component, this, parent);
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition",
				new XAttribute("TargetWearProfile", TargetWearProfile?.Id ?? 0),
				from value in _values
				select
					new XElement("Value", new XAttribute("Definition", value.Key.Id),
						new XAttribute("Target", value.Value.Id))).ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ChangerGameItemComponentProto(proto, gameworld));
	}
}