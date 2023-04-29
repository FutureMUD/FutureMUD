using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.CharacterCreation.Screens;

namespace MudSharp.GameItems.Prototypes;

public class KeyringGameItemComponentProto : GameItemComponentProto
{
	public int MaximumNumberOfKeys { get; protected set; }

	public override string TypeDescription => "Keyring";

	#region Constructors
	protected KeyringGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Keyring")
	{
		MaximumNumberOfKeys = 10;
	}

	protected KeyringGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		MaximumNumberOfKeys = int.Parse(root.Element("MaximumNumberOfKeys").Value);
	}
	#endregion

	#region Saving
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MaximumNumberOfKeys", MaximumNumberOfKeys)
		).ToString();
	}
	#endregion

	#region Component Instance Initialising Functions
	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new KeyringGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new KeyringGameItemComponent(component, this, parent);
	}
	#endregion

	#region Initialisation Tasks
	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("Keyring".ToLowerInvariant(), true, (gameworld, account) => new KeyringGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Keyring", (proto, gameworld) => new KeyringGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Keyring",
			$"A {"[container]".Colour(Telnet.BoldGreen)} that holds {"[key]".Colour(Telnet.Yellow)} items and can be used as one in syntax",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new KeyringGameItemComponentProto(proto, gameworld));
	}
	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3<name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3keys <##>#0 - sets the number of keys this keyring can hold";
	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "keys":
			case "number":
			case "count":
			case "maximum":
			case "max":
				return BuildingCommandKeys(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandKeys(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many keys should this keyring be able to hold?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a positive integer greater than zero.");
			return false;
		}

		MaximumNumberOfKeys = value;
		Changed = true;
		actor.OutputHandler.Send($"This keyring will now hold {value.ToString("N0", actor).ColourValue()} {"key".Pluralise(value != 1)}.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, @"{0} (#{1:N0}r{2:N0}, {3})

This item is a special kind of container that only holds keys. It can count as any key it contains for the inventory system.

This keyring holds {4:N0} {5}.",
			"Keyring Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			MaximumNumberOfKeys,
			MaximumNumberOfKeys == 1 ? "key" : "keys"
		);
	}
}