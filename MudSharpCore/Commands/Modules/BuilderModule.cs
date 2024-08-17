using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Commands.Helpers;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Body.Disfigurements;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.FutureProg;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Commands.Modules;

internal class BuilderModule : BaseBuilderModule
{
	private BuilderModule()
		: base("Builder")
	{
		IsNecessary = true;
	}

	public static BuilderModule Instance { get; } = new();
	
	#region Characteristics

	private const string CharacteristicHelp =
		@"The characteristic command is used to work with characteristic definitions, values and profiles. Characteristics are also sometimes known as 'variables', and are used for both items (where they may represent things like variable colours, shapes, designs or the like) and also characters (where they can represent things like eye colour, hair style, etc).

Characteristic Definitions are the 'types' of characteristics.
Characteristic Values are the possible values that may match a characteristic type.
Characteristic Profiles are curated lists of characteristic values that control permitted values.

You must use one of the following subcommands of this command:

characteristic definition ... - work with characteristic definitions
characteristic value ... - work with characteristic values
characteristic profile ... - work with characteristic profiles";

	[PlayerCommand("Characteristic", "characteristic", "variable")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Characteristic(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "definition":
			case "def":
				Characteristic_Definition(actor, ss);
				break;
			case "value":
			case "val":
				Characteristic_Value(actor, ss);
				break;
			case "profile":
			case "prof":
				Characteristic_Profile(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(CharacteristicHelp);
				return;
		}
	}

	private static void Characteristic_Definition(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
			case "create":
				Characteristic_Definition_Add(actor, command, false, false);
				break;
			case "addchild":
			case "newchild":
			case "createchild":
				Characteristic_Definition_Add(actor, command, true, false);
				break;
			case "addbodypart":
			case "addpart":
			case "newbodypart":
			case "newpart":
			case "createbodypart":
			case "createpart":
				Characteristic_Definition_Add(actor, command, false, true);
				return;
			case "addbodypartchild":
			case "addpartchild":
			case "newbodypartchild":
			case "newpartchild":
			case "createbodypartchild":
			case "createpartchild":
				Characteristic_Definition_Add(actor, command, true, true);
				return;
			case "delete":
			case "remove":
			case "del":
			case "rem":
				Characteristic_Definition_Remove(actor, command);
				break;
			case "set":
				Characteristic_Definition_Set(actor, command);
				break;
			case "list":
				CharacteristicDefinitionList(actor, command);
				return;
			case "show":
			case "view":
				CharacteristicDefinitionShow(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command:

	list [<names...>] - lists all of the characteristic definitions
	show <which> - views detailed information about a characteristic definition
	add <name> <type> - creates a new characteristic of the specified type
	addchild <name> <parent> - creates a new child characteristic of another
	addpart <name> <shape> <type> - creates a new bodypart-linked characteristic of the specified type
	addpartchild <name> <shape> <parent> - creates a new bodypart-linked characteristic that is a child of another
	delete <name> - permanently deletes a characteristic type
	set <which> name <name> - changes the name of a characteristic definition
	set <which> desc <desc> - changes the description of a characteristic definition
	set <which> pattern <regex> - changes the regex pattern used to identify in descriptions
	set <which> parent <parent> - if already a child type, changes the parent
	set <which> ... - some types may have additional options");
				return;
		}
	}

	private static void CharacteristicDefinitionList(ICharacter actor, StringStack input)
	{
		var characteristics = actor.Gameworld.Characteristics.ToList();
		while (!input.IsFinished)
		{
			var cmd = input.PopSpeech();
			characteristics =
				characteristics.Where(x => x.Name.StartsWith(cmd, StringComparison.CurrentCultureIgnoreCase))
				               .ToList();
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from definition in characteristics
				select
					new[]
					{
						definition.Id.ToString("N0", actor),
						definition.Name,
						definition.Pattern.ToString(),
						definition.Description,
						definition.Type.ToString(),
						definition.Parent?.Name ?? ""
					},
				new[] { "ID#", "Name", "Pattern", "Description", "Type", "Parent" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void CharacteristicDefinitionShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic definition do you want to show?");
			return;
		}

		var definition = actor.Gameworld.Characteristics.GetByIdOrName(command.SafeRemainingArgument);
		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return;
		}

		actor.OutputHandler.Send(definition.Show(actor));
	}

	private static void Characteristic_Definition_Add(ICharacter actor, StringStack command, bool client, bool bodypart)
	{
		var name = command.PopSpeech().TitleCase();
		if (string.IsNullOrEmpty(name))
		{
			actor.OutputHandler.Send("What name do you want to give your new characteristic definition?");
			return;
		}

		IBodypartShape shape = null;
		if (bodypart)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("Which bodypart shape should this characteristic be tied to?");
				return;
			}

			shape = actor.Gameworld.BodypartShapes.GetByIdOrName(command.PopSpeech());
			if (shape == null)
			{
				actor.OutputHandler.Send("There is no such bodypart shape.");
				return;
			}
		}

		if (client)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					"Which other characteristic definition do you want to make this one a child definition of?");
				return;
			}

			var parent = actor.Gameworld.Characteristics.GetByIdOrName(command.SafeRemainingArgument);
			if (parent == null)
			{
				actor.OutputHandler.Send("There is no such characteristic definition.");
				return;
			}

			if (bodypart)
			{
				var child = new BodypartSpecificClientCharacteristicDefinition(name, name.ToLowerInvariant(),
					$"The {name} characteristic", shape, parent, actor.Gameworld);
				actor.OutputHandler.Send(
					$"You create a new child bodypart-specific characteristic definition called {name.ColourName()} with ID #{child.Id.ToString("N0", actor)}, which is linked to the characteristic {parent.Name.ColourName()} and the shape {shape.Name.ColourValue()}.");
			}
			else
			{
				var child = new ClientCharacteristicDefinition(name, name.ToLowerInvariant(),
					$"The {name} characteristic", parent, actor.Gameworld);
				actor.OutputHandler.Send(
					$"You create a new child characteristic definition called {name.ColourName()} with ID #{child.Id.ToString("N0", actor)}, which is linked to the characteristic {parent.Name.ColourName()}.");
			}

			return;
		}

		CharacteristicType ctype;
		var type = command.PopForSwitch();
		switch (type)
		{
			case "relativeheight":
			case "relheight":
				ctype = CharacteristicType.RelativeHeight;
				break;
			case "standard":
			case "basic":
				ctype = CharacteristicType.Standard;
				break;
			case "coloured":
			case "colored":
			case "colour":
			case "color":
				ctype = CharacteristicType.Coloured;
				break;
			case "otherbasic":
			case "multiform":
				ctype = CharacteristicType.Multiform;
				break;
			case "growth":
			case "growing":
			case "growable":
				ctype = CharacteristicType.Growable;
				break;
			default:
				actor.OutputHandler.Send("Valid types for Characteristic Definitions are " +
				                         new[] { "Standard", "Coloured", "Multiform", "Relative Height", "Growable" }
					                         .Select(
						                         x => x
							                         .Colour(
								                         Telnet
									                         .Cyan))
					                         .ListToString() +
				                         ".");
				return;
		}

		if (bodypart)
		{
			var definition = new BodypartSpecificCharacteristicDefinition(name, name.ToLowerInvariant(),
				$"The {name} characteristic", ctype, shape,
				actor.Gameworld);
			actor.OutputHandler.Send(
				$"You create a bodypart-specific characteristic definition called {name.ColourName()} with ID #{definition.Id.ToString("N0", actor)} of type {ctype.Describe().ColourValue()} linked to bodypart shape {shape.Name.ColourValue()}.");
		}
		else
		{
			var definition = new CharacteristicDefinition(name, name.ToLowerInvariant(), $"The {name} characteristic",
				ctype,
				actor.Gameworld);
			actor.OutputHandler.Send(
				$"You create a characteristic definition called {name.ColourName()} with ID #{definition.Id.ToString("N0", actor)} of type {ctype.Describe().ColourValue()}.");
		}
	}

	private static void Characteristic_Definition_Remove(ICharacter actor, StringStack command)
	{
		var name = command.SafeRemainingArgument;
		ICharacteristicDefinition definition = null;
		definition = actor.Gameworld.Characteristics.GetByIdOrName(name);

		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition for you to remove.");
			return;
		}

		actor.AddEffect(
			new Accept(actor, new CharacteristicDefinitionRemovalProposal(actor, definition)),
			TimeSpan.FromSeconds(60));
		actor.OutputHandler.Send(
			$"You are proposing to delete the characteristic definition {name.ColourName()} with ID #{definition.Id.ToString("N0", actor)}.\nThis will delete all associated values and profiles, and remove the variable from all item components and items.\n{Accept.StandardAcceptPhrasing}");
	}

	private static void Characteristic_Definition_Set(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		ICharacteristicDefinition definition = null;
		definition = actor.Gameworld.Characteristics.GetByIdOrName(name);

		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition for you to edit.");
			return;
		}

		definition.BuildingCommand(actor, command);
	}

	private static void Characteristic_Value(ICharacter actor, StringStack command)
	{
		switch (command.Pop().ToLowerInvariant())
		{
			case "add":
				Characteristic_Value_Add(actor, command);
				break;
			case "clone":
				CharacteristicValueClone(actor, command);
				return;
			case "delete":
			case "remove":
			case "del":
			case "rem":
				Characteristic_Value_Remove(actor, command);
				break;
			case "set":
				Characteristic_Value_Set(actor, command);
				break;
			case "list":
				CharacteristicValueList(actor, command);
				return;
			case "show":
			case "view":
				CharacteristicValueShow(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options with this command:

    list [<definition>|*<profile>|+<keyword>|-<keyword>] - shows all values meeting the optional filter criteria
    show <which> - shows detailed information about a characteristic value
    add <definition> <name> [<type specific extra args>] - creates a new value
	clone <which> <name> - creates a new value from an existing value
    remove <name> - permanently deletes a characteristic value
    set <which> ... - changes properties of a value. See values for more info.");
				return;
		}
	}

	private static void CharacteristicValueClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic value would you like to clone?");
			return;
		}

		var clone = actor.Gameworld.CharacteristicValues.GetByIdOrName(command.PopSpeech());
		if (clone == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your new cloned value?");
			return;
		}

		var newValue = clone.Clone(command.SafeRemainingArgument);
		actor.Gameworld.Add(newValue);
		actor.OutputHandler.Send(
			$"You clone the characteristic value {clone.Name.ColourValue()} into a new value called {newValue.Name.ColourValue()}.");
	}

	private static void CharacteristicValueShow(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic value do you want to show?");
			return;
		}

		var value = actor.Gameworld.CharacteristicValues.GetByIdOrName(command.SafeRemainingArgument);
		if (value == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value.");
			return;
		}

		actor.OutputHandler.Send(value.Show(actor));
	}

	private static void CharacteristicValueList(ICharacter actor, StringStack command)
	{
		var values = actor.Gameworld.CharacteristicValues.ToList();
		while (!command.IsFinished)
		{
			var cmd = command.PopSpeech();
			if (cmd[0] == '*' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				var profile = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(cmd);
				if (profile == null)
				{
					actor.OutputHandler.Send(
						$"There is no such characteristic profile to filter by as {cmd.ColourCommand()}.");
					return;
				}

				values = values.Where(x => profile.Values.Contains(x)).ToList();
			}
			else if (cmd[0] == '+' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				values = values
				         .Where(x =>
					         x.GetValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
					         x.GetBasicValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
					         x.GetFancyValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase)
				         )
				         .ToList();
			}
			else if (cmd[0] == '-' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				cmd = cmd.Substring(1);
				values = values
				         .Where(x =>
					         !x.GetValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
					         !x.GetBasicValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
					         !x.GetFancyValue.Contains(cmd, StringComparison.InvariantCultureIgnoreCase)
				         )
				         .ToList();
			}
			else
			{
				var definition = actor.Gameworld.Characteristics.FirstOrDefault(x => x.Pattern.IsMatch(cmd));
				if (definition == null)
				{
					actor.OutputHandler.Send(
						$"The argument {cmd.ColourCommand()} was not a valid characteristic definition.");
					return;
				}

				values = values.Where(x => x.Definition == definition).ToList();
			}
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from value in values
				select
					new[]
					{
						value.Id.ToString("N0", actor),
						value.Name,
						value.GetValue,
						value.GetBasicValue,
						value.ChargenApplicabilityProg?.MXPClickableFunctionName() ?? "",
						value.OngoingValidityProg?.MXPClickableFunctionName() ?? "",
						value.Definition.IsDefaultValue(value) ? "Y" : "N"
					},
				new[] { "ID#", "Name", "Value", "Basic Value", "Chargen", "Ongoing", "Default?" },
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void Characteristic_Value_Add(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		ICharacteristicDefinition definition = null;
		definition = long.TryParse(name, out var value)
			? actor.Gameworld.Characteristics.Get(value)
			: actor.Gameworld.Characteristics.Get(name).FirstOrDefault();

		if (definition == null)
		{
			actor.OutputHandler.Send(
				"There is no such Characteristic Definition with which you may create a new value.");
			return;
		}

		name = command.PopSpeech();
		if (string.IsNullOrEmpty(name))
		{
			actor.OutputHandler.Send("What name do you want to give your new Characteristic Value?");
			return;
		}

		ICharacteristicValue newvalue = null;
		var basic = command.PopSpeech();
		switch (definition.Type)
		{
			case CharacteristicType.Coloured:
			{
				IColour colour = null;
				colour = actor.Gameworld.Colours.GetByIdOrName(basic);

				if (colour == null)
				{
					actor.OutputHandler.Send("There is no such colour for you to use.");
					return;
				}

				newvalue = new ColourCharacteristicValue(name, definition, colour);
			}
				break;
			case CharacteristicType.RelativeHeight:
				actor.OutputHandler.Send("This kind of Characteristic cannot have explicitly created values.");
				return;
			case CharacteristicType.Multiform:

				var fancy = command.PopSpeech();
				if (string.IsNullOrEmpty(basic) || string.IsNullOrEmpty(fancy))
				{
					actor.OutputHandler.Send(
						"You must specify both a basic and fancy value for this kind of characteristic.");
					return;
				}

				newvalue = new MultiformCharacteristicValue(name, definition, basic, fancy);

				break;
			case CharacteristicType.Growable:
				fancy = command.PopSpeech();
				if (string.IsNullOrEmpty(basic) || string.IsNullOrEmpty(fancy))
				{
					actor.OutputHandler.Send(
						"You must specify both a basic and fancy value for this kind of characteristic.");
					return;
				}

				if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var length))
				{
					actor.OutputHandler.Send("You must specify a valid numerical growth stage for the value.");
					return;
				}

				newvalue = new GrowableCharacteristicValue(name, definition, basic, fancy, length);
				break;
			default:
				newvalue = new CharacteristicValue(name, definition, name, string.Empty);
				break;
		}

		actor.Gameworld.Add(newvalue);
		actor.Gameworld.SaveManager.Flush();
		actor.OutputHandler.Send(
			$"You create a new value for the {definition.Name.ColourName()} definition with a value of {newvalue.GetValue.ColourValue()} and ID of {newvalue.Id.ToString("N0", actor)}.");
	}

	private static void Characteristic_Value_Remove(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		var cvalue = actor.Gameworld.CharacteristicValues.GetByIdOrName(name);

		if (cvalue == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value for you to remove.");
			return;
		}

		actor.AddEffect(
			new Accept(actor, new CharacteristicValueRemovalProposal(actor, cvalue)),
			TimeSpan.FromSeconds(60));
		actor.OutputHandler.Send(
			$"You are proposing to permanently delete the characteristic value {name.ColourName()} with ID {cvalue.Id.ToString("N0", actor)}.\nThis will remove it from all associated profiles, and remove the variable from all item components and items (randomising them to a new value).\n{Accept.StandardAcceptPhrasing}");
	}

	private static void Characteristic_Value_Set(ICharacter actor, StringStack command)
	{
		var name = command.PopSpeech();
		var cvalue = actor.Gameworld.CharacteristicValues.GetByIdOrName(name);

		if (cvalue == null)
		{
			actor.OutputHandler.Send("There is no such characteristic value for you to edit.");
			return;
		}

		cvalue.BuildingCommand(actor, command);
	}

	private static void Characteristic_Profile(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
			case "create":
				CharacteristicProfileNew(actor, command);
				return;
			case "list":
				CharacteristicProfileList(actor, command);
				return;
			case "show":
			case "view":
				CharacteristicProfileView(actor, command);
				return;
			case "set":
				CharacteristicProfileSet(actor, command);
				return;
			case "clone":
				CharacteristicProfileClone(actor, command);
				return;
			default:
				actor.OutputHandler.Send(@"You can use the following options to edit characteristic profiles:

	characteristic profile list - lists all of the characteristic profiles
	characteristic profile show <which> - views detailed information about a profile
	characteristic profile new <definition> <type> <name> - creates a new profile
	characteristic profile clone <which> <name> - clones an existing profile to a new one
	characteristic profile set <which> ... - edits properties of a profile. See individual types for more help");
				return;
		}
	}

	private static void CharacteristicProfileClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic profile do you want to clone?");
			return;
		}

		var clone = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(command.PopSpeech());
		if (clone == null)
		{
			actor.OutputHandler.Send("There is no such characteristic profile.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this cloned profile?");
			return;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.CharacteristicProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a characteristic profile with that name. Names must be unique.");
			return;
		}

		var newProfile = clone.Clone(name);
		actor.Gameworld.Add(newProfile);
		actor.OutputHandler.Send(
			$"You clone the characteristic profile {clone.Name.ColourName()} into a new profile called {name.ColourName()}.");
	}

	private static void CharacteristicProfileSet(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic profile do you want to edit?");
			return;
		}

		var which = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(command.PopSpeech());
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such characteristic profile.");
			return;
		}

		which.BuildingCommand(actor, command);
	}

	private static void CharacteristicProfileView(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic profile do you want to view?");
			return;
		}

		var which = actor.Gameworld.CharacteristicProfiles.GetByIdOrName(command.PopSpeech());
		if (which == null)
		{
			actor.OutputHandler.Send("There is no such characteristic profile.");
			return;
		}

		actor.OutputHandler.Send(which.Show(actor));
	}

	private static void CharacteristicProfileList(ICharacter actor, StringStack command)
	{
		var characteristics = actor.Gameworld.CharacteristicProfiles.ToList();
		while (!command.IsFinished)
		{
			var cmd = command.PopSpeech();
			if (cmd[0] == '+' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				characteristics = characteristics
				                  .Where(x => x.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
				                              x.Description.Contains(cmd, StringComparison.InvariantCultureIgnoreCase))
				                  .ToList();
			}
			else if (cmd[0] == '-' && cmd.Length > 1)
			{
				cmd = cmd.Substring(1);
				characteristics = characteristics
				                  .Where(x => !x.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
				                              !x.Description.Contains(cmd, StringComparison.InvariantCultureIgnoreCase))
				                  .ToList();
			}
			else
			{
				var definition = actor.Gameworld.Characteristics.FirstOrDefault(x => x.Pattern.IsMatch(cmd));
				if (definition == null)
				{
					actor.OutputHandler.Send(
						$"The argument {cmd.ColourCommand()} was not a valid characteristic definition.");
					return;
				}

				characteristics = characteristics.Where(x => x.TargetDefinition == definition).ToList();
			}
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from definition in characteristics
				select
					new[]
					{
						definition.Id.ToString(),
						definition.Name,
						definition.TargetDefinition.Name,
						definition.Description,
						definition.Values.Count().ToString("N0", actor),
						definition.Type
					},
				new[] { "ID#", "Name", "Definition", "Description", "Values", "Type" }, actor.Account.LineFormatLength,
				colour: Telnet.Green,
				truncatableColumnIndex: 3,
				unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void CharacteristicProfileNew(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which characteristic definition do you want to make a profile for?");
			return;
		}

		var definition = actor.Gameworld.Characteristics.GetByIdOrName(command.PopSpeech());
		if (definition == null)
		{
			actor.OutputHandler.Send("There is no such characteristic definition.");
			return;
		}

		var typeText = command.PopForSwitch();

		switch (typeText)
		{
			case "standard":
			case "all":
			case "compound":
			case "weighted":
				break;
			default:
				actor.OutputHandler.Send(
					$"You must specify a valid type from the following list: {new[] { "standard", "compound", "all", "weighted" }.Select(x => x.ColourValue()).ListToString()}");
				return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to your profile?");
			return;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.CharacteristicProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a characteristic profile with that name. Names must be unique.");
			return;
		}

		switch (typeText)
		{
			case "standard":
				var profile = new CharacteristicProfile(name, definition, "Standard");
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new standard characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
			case "all":
				profile = new CharacterProfileAll(name, definition);
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new all-values characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
			case "compound":
				profile = new CharacteristicProfileCompound(name, definition);
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new compound characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
			case "weighted":
				profile = new WeightedCharacteristicProfile(name, definition);
				actor.Gameworld.Add(profile);
				actor.OutputHandler.Send(
					$"You create a new weighted characteristic profile called {name.ColourName()} for the {definition.Name.ColourName()} definition.");
				return;
		}
	}

	#endregion
	
	#region Dreams

	public const string DreamHelpText =
		@"You can use this command to create and edit dreams, and also to assign them to people.

The syntax for this command is as follows:

	dream list - lists all dreams
	dream show <which> - shows a particular dream
	dream edit <which> - begins editing a particular dream
	dream edit - an alias for dream show <editing item>
	dream close - stops editing a dream
	dream edit new <name> - creates a new dream
	dream clone <old> <new name> - clones an existing dream
	dream give <which> <character> - gives a dream to a character
	dream remove <which> <character> - removes a dream from a character
	dream set ... - edits the properties of the dream you have open";

	[PlayerCommand("Dream", "dream")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("dream", DreamHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Dream(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		GenericBuildingCommand(actor, ss, EditableItemHelper.DreamHelper);
	}

	#endregion

	#region Tags

	private const string TagHelp = @"The tag command is used to create, view and manage tags. 

Tags are used in numerous parts of the code to classify things and apply basic hierarchies. These hierarchies are best explained through an example.

Say you had a tag called #6Knife#0, and you wanted knives to be also #6Cutting Tools#0. The knife tag would be a child of the cutting tool tag, and would be represented as #6Cutting Tools / Knife#0.

In some places you might want to check directly for knives, but you could also check more generally for cutting tools, which might also pick up #6Cutting Tools / Saw#0.

Tags are used on items and crafts, but also other places in the code as well.

The syntax to use this command is as follows:

	#3tag list#0 - shows all the top level tags
	#3tag show <which>#0 - shows info about a particular tag
	#3tag new <name> [<parent>]#0 - creates a new tag (with optional parent)
	#3tag insert <name> <parent>#0 - creates a new tag as a child of a tag and takes its children
	#3tag parent <tag> <newparent>#0 - changes the parent of a tag
	#3tag rename <tag> <name>#0 - changes the name of a tag
	#3tag prog <tag> <which>#0 - sets a prog that controls visibility of the tag
	#3tag prog none#0 - clears a visiblity prog";

	[PlayerCommand("Tag", "tag")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("Tag", TagHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void Tag(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				TagAdd(actor, ss);
				return;
			case "remove":
			case "rem":
				TagRemove(actor, ss);
				return;
			case "insert":
			case "ins":
				TagInsert(actor, ss);
				return;
			case "view":
			case "show":
				TagView(actor, ss);
				return;
			case "list":
				TagList(actor, ss);
				return;
			case "rename":
				TagRename(actor, ss);
				return;
			case "parent":
				TagParent(actor, ss);
				return;
			case "prog":
				TagProg(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(TagHelp.SubstituteANSIColour());
				return;
		}
	}

	private static void TagProg(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to change the parent of?");
			return;
		}

		var tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
		if (tag is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.Last.ColourCommand()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either use {"none".ColourCommand()}, or specify a prog to be the new parent of the {tag.FullName.ColourName()} tag?");
			return;
		}

		if (ss.SafeRemainingArgument.EqualTo("none"))
		{
			tag.ShouldSeeProg = null;
			actor.OutputHandler.Send(
				$"The {tag.FullName.ColourName()} tag will not use any prog to control visibility (always visible instead).");
			return;
		}

		var prog = new FutureProgLookupFromBuilderInput(actor.Gameworld, actor, ss.SafeRemainingArgument,
			FutureProgVariableTypes.Boolean, new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character
			}).LookupProg();
		if (prog is null)
		{
			return;
		}

		tag.ShouldSeeProg = prog;
		actor.OutputHandler.Send(
			$"The {tag.FullName.ColourName()} prog now uses the {prog.MXPClickableFunctionName()} prog to control visibility.");
	}

	private static void TagParent(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to change the parent of?");
			return;
		}

		var tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
		if (tag is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.Last.ColourCommand()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What should be the new parent of the {tag.FullName.ColourName()} tag?");
			return;
		}

		var parent = actor.Gameworld.Tags.GetByIdOrName(ss.SafeRemainingArgument);
		if (parent is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.SafeRemainingArgument.ColourCommand()}.");
			return;
		}

		if (parent.IsA(tag))
		{
			actor.OutputHandler.Send(
				$"You cannot make this change as {parent.FullName.ColourName()} is already a {tag.FullName.ColourName()}, and this would create a loop.");
			return;
		}

		var old = tag.FullName.ColourName();
		tag.Parent = parent;
		actor.OutputHandler.Send($"You change the tag from {old} to {tag.FullName.ColourName()}.");
	}

	private static void TagRename(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to change the name of?");
			return;
		}

		var tag = actor.Gameworld.Tags.GetByIdOrName(ss.PopSpeech());
		if (tag is null)
		{
			actor.OutputHandler.Send($"There is no such tag identified by {ss.Last.ColourCommand()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What new name do you want to give to the {tag.FullName.ColourName()} tag?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase().Trim();
		if (actor.Gameworld.Tags.Except(tag).Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"The {actor.Gameworld.Tags.First(x => x.Name.EqualTo(name)).FullName.ColourName()} tag already has the name {name}. Names must be unique.");
			return;
		}

		var old = tag.FullName.ColourName();
		tag.SetName(name);
		actor.OutputHandler.Send($"You rename the tag {old} to {tag.FullName.ColourName()}.");
	}

	private static void TagList(ICharacter actor, StringStack command)
	{
		var topLevel = actor.Gameworld.Tags.Where(x => x.Parent == null).ToList();
		var sb = new StringBuilder();
		sb.AppendLine(
			"There are the following top level tags. To see any of their descendents please use TAG VIEW <tag>:");
		foreach (var tag in topLevel)
		{
			sb.AppendLine($"[{tag.Id.ToString("N0", actor)}] {tag.Name.Colour(Telnet.Cyan)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void TagView(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send($"What tag do you want to view? Syntax is {"tag view \"<tag>\"".Colour(Telnet.Yellow)}.");
			return;
		}

		var tags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (tags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return;
		}

		if (tags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{tags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return;
		}

		var targetTag = tags.Single();
		var children = actor.Gameworld.Tags.Except(targetTag).Where(x => x.IsA(targetTag)).ToList();
		var sb = new StringBuilder();

		void DescribeLevel(ITag tag, int level)
		{
			sb.AppendLine(new string('\t', level) + $"[{tag.Id.ToString("N0", actor)}] {tag.Name}");
			foreach (var branch in children.Where(x => x.Parent == tag).ToList())
			{
				DescribeLevel(branch, level + 1);
			}
		}

		DescribeLevel(targetTag, 0);
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void TagInsert(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What tag do you want to insert? Syntax is {"tag insert \"<tag>\" [\"<old parent>\"]".Colour(Telnet.Yellow)}.");
			return;
		}

		var tagName = command.PopSpeech();
		if (actor.Gameworld.Tags.Any(x => x.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a tag with that name. You must choose a new, unique name for your tag.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to insert this new tag between it and its children?");
			return;
		}

		var parentTag = actor.Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
		if (parentTag == null)
		{
			actor.Send("There is no such tag for you to insert between it and its children.");
			return;
		}

		var children = actor.Gameworld.Tags.Where(x => x.Parent == parentTag).ToList();
		var newTag = new Tag(tagName, parentTag, actor.Gameworld);
		actor.Gameworld.Add(newTag);
		foreach (var child in children)
		{
			child.Parent = newTag;
		}

		actor.OutputHandler.Send(
			$"You add the tag {newTag.FullName.Colour(Telnet.Cyan)}, taking all the children of its parent tag.");
	}

	private static void TagRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"What tag do you want to remove? Syntax is {"tag delete \"<tag>\"".Colour(Telnet.Yellow)}.");
			return;
		}

		var tags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (tags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return;
		}

		if (tags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"Your text matched multiple tags. Please specify one of the following tags:\n\n{tags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return;
		}

		var targetTag = tags.Single();

		var childrenCount = actor.Gameworld.Tags.Except(targetTag).Count(x => x.IsA(targetTag));
		actor.Send(
			$"Are you sure that you want to permanently delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag? This action cannot be undone.{(childrenCount > 0 ? $"This will also delete all {childrenCount} children tags of this tag." : "")}");
		actor.Send(
			$"Use the command {"accept".Colour(Telnet.Yellow)} to accept or {"decline".Colour(Telnet.Yellow)} to cancel.");
		actor.AddEffect(new Accept(actor, new GenericProposal(
			message =>
			{
				actor.Send($"You delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag.");
				var tagsToDelete =
					actor.Gameworld.Tags.Where(x => x.IsA(targetTag)).ToList();
				foreach (var tag in tagsToDelete)
				{
					tag.GetEditable.Delete();
				}
			},
			message =>
			{
				actor.Send(
					$"You decide not to delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag.");
			},
			() =>
			{
				actor.Send(
					$"You decide not to delete the {targetTag.FullName.Colour(Telnet.Cyan)} tag.");
			},
			$"Proposing to remove the tag {targetTag.FullName.Colour(Telnet.Cyan)}.",
			"tag", "remove", targetTag.Name
		)), TimeSpan.FromSeconds(120));
	}

	private static void TagAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What tag do you want to add? Syntax is {"tag add \"<tag>\" [\"<parent>\"]".Colour(Telnet.Yellow)}.");
			return;
		}

		var tagName = command.PopSpeech();
		if (actor.Gameworld.Tags.Any(x => x.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a tag with that name. You must choose a new, unique name for your tag.");
			return;
		}

		ITag parentTag = null;
		if (!command.IsFinished)
		{
			parentTag = actor.Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
			if (parentTag == null)
			{
				actor.Send("There is no such tag for you to make as the parent of your new tag.");
				return;
			}
		}

		var newTag = new Tag(tagName, parentTag, actor.Gameworld);
		actor.Gameworld.Add(newTag);
		actor.OutputHandler.Send(
			$"You add the {(parentTag is null ? "top level " : "")}tag {newTag.FullName.Colour(Telnet.Cyan)}");
	}

	#endregion

	#region Wear Profiles

	#region Wear Profile Subcommands

	private static void WearProfileSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IWearProfile>>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("You are not editing any wear profiles.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void WearProfileAdd(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send(
				$"Which type of Wear Profile do you want to add? You can add {"direct".Colour(Telnet.Yellow)} or {"shape".Colour(Telnet.Yellow)}.");
			return;
		}

		var type = ss.Pop().ToLowerInvariant();
		switch (type)
		{
			case "direct":
			case "shape":
				break;
			default:
				actor.Send(
					$"That is not a valid type of wear profile. You can add {"direct".Colour(Telnet.Yellow)} or {"shape".Colour(Telnet.Yellow)}.");
				return;
		}

		if (ss.IsFinished)
		{
			actor.Send("What name do you want to give your new wear profile?");
			return;
		}

		var name = ss.SafeRemainingArgument;
		if (actor.Gameworld.WearProfiles.GetByName(name) != null)
		{
			actor.Send("There is already a wear profile with that name. Wear profile names must be unique.");
			return;
		}

		using (new FMDB())
		{
			var dbitem = new Models.WearProfile();
			FMDB.Context.WearProfiles.Add(dbitem);
			dbitem.Name = name;
			dbitem.Description = "An undescribed wear profile";
			dbitem.WearAction1st = "put";
			dbitem.WearAction3rd = "puts";
			dbitem.WearAffix = "on";
			dbitem.Type = type.Proper();
			dbitem.WearStringInventory = "worn on";
			dbitem.WearlocProfiles = "<Profiles/>";
			FMDB.Context.SaveChanges();
			var newWearProfile = GameItems.Inventory.WearProfile.LoadWearProfile(dbitem, actor.Gameworld);
			actor.Gameworld.Add(newWearProfile);

			var dbcomponent = new Models.GameItemComponentProto
			{
				Id = actor.Gameworld.ItemComponentProtos.NextID(),
				RevisionNumber = 0,
				Name = $"Wear_{name.Replace(' ', '_')}",
				Description = $"Permits the item to be worn in the {name} wear configuration",
				Type = "Wearable",
				Definition =
					$"<Definition DisplayInventoryWhenWorn=\"true\" Bulky=\"false\"><Profiles Default=\"{newWearProfile.Id}\"><Profile>{newWearProfile.Id}</Profile></Profiles></Definition>"
			};
			dbcomponent.EditableItem = new Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = actor.Account.Id,
				BuilderDate = DateTime.UtcNow,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = actor.Account.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = DateTime.UtcNow
			};
			FMDB.Context.GameItemComponentProtos.Add(dbcomponent);
			FMDB.Context.SaveChanges();

			var component = actor.Gameworld.GameItemComponentManager.GetProto(dbcomponent, actor.Gameworld);
			actor.Gameworld.Add(component);

			actor.OutputHandler.Send(
				$"You create a new wear profile called {name.Colour(Telnet.Green)} with ID #{newWearProfile.Id.ToString("N0", actor)}, which you are now editing.\nAlso created a matching wear component called {component.Name.ColourName()} with ID #{component.Id.ToString("N0", actor)}.");
		}
	}
	private static void WearProfileClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which wear profile would you like to clone?");
			return;
		}

		var profile = actor.Gameworld.WearProfiles.GetByIdOrName(ss.PopSpeech());
		if (profile is null)
		{
			actor.OutputHandler.Send($"There is no wear profile identified by {ss.Last.ColourCommand()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new wear profile?");
			return;
		}

		var name = ss.SafeRemainingArgument.TitleCase();
		if (actor.Gameworld.WearProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a wear profile called {name.ColourName()}. Names must be unique.");
			return;
		}

		var newItem = profile.Clone(name);
		actor.Gameworld.Add(newItem);
		actor.RemoveAllEffects<BuilderEditingEffect<IWearProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IWearProfile>(actor) { EditingItem = profile });
		using (new FMDB())
		{
			var dbcomponent = new Models.GameItemComponentProto
			{
				Id = actor.Gameworld.ItemComponentProtos.NextID(),
				RevisionNumber = 0,
				Name = $"Wear_{name.Replace(' ', '_')}",
				Description = $"Permits the item to be worn in the {name} wear configuration",
				Type = "Wearable",
				Definition =
					$"<Definition DisplayInventoryWhenWorn=\"true\" Bulky=\"false\"><Profiles Default=\"{profile.Id}\"><Profile>{profile.Id}</Profile></Profiles></Definition>"
			};
			dbcomponent.EditableItem = new Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = actor.Account.Id,
				BuilderDate = DateTime.UtcNow,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = actor.Account.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = DateTime.UtcNow
			};
			FMDB.Context.GameItemComponentProtos.Add(dbcomponent);
			FMDB.Context.SaveChanges();

			var component = actor.Gameworld.GameItemComponentManager.GetProto(dbcomponent, actor.Gameworld);
			actor.Gameworld.Add(component);

			actor.OutputHandler.Send($"You clone the {profile.Name.ColourName()} wear profile into a new profile called {name.ColourName()} with ID #{profile.Id.ToString("N0", actor)}, which you are now editing.\nAlso created a matching wear component called {component.Name.ColourName()} with ID #{component.Id.ToString("N0", actor)}.");
		}
	}

	private static void WearProfileClose(ICharacter actor)
	{
		actor.RemoveAllEffects<BuilderEditingEffect<IWearProfile>>();
		actor.OutputHandler.Send("You are no longer editing any wear profiles.");
	}

	private static void WearProfileEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IWearProfile>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which wear profile do you want to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.ShowTo(actor));
			return;
		}

		var profile = actor.Gameworld.WearProfiles.GetByIdOrName(ss.SafeRemainingArgument);
		if (profile is null)
		{
			actor.OutputHandler.Send("There is no such wear profile.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IWearProfile>>();
		actor.AddEffect(new BuilderEditingEffect<IWearProfile>(actor) { EditingItem = profile });
		actor.OutputHandler.Send($"You are now editing the {profile.Name.ColourName()} wear profile.");
	}

	public static void WearProfileList(ICharacter actor, StringStack ss)
	{
		var profiles = actor.Gameworld.WearProfiles.ToList();

		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech();
			if (cmd.EqualTo("shape"))
			{
				profiles = profiles.Where(x => x is ShapeWearProfile).ToList();
				continue;
			}

			if (cmd.EqualTo("part"))
			{
				profiles = profiles.Where(x => x is DirectWearProfile).ToList();
				continue;
			}

			if (cmd.Length < 2)
			{
				actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid filter.");
				return;
			}

			var first = cmd[0];
			var second = cmd.Substring(1);
			switch (first)
			{
				case '+':
					profiles = profiles.Where(x => x.Name.Contains(second, StringComparison.InvariantCultureIgnoreCase) || x.Description.Contains(second, StringComparison.InvariantCultureIgnoreCase)).ToList();
					continue;
				case '-':
					profiles = profiles.Where(x =>
						!x.Name.Contains(second, StringComparison.InvariantCultureIgnoreCase) &&
						!x.Description.Contains(second, StringComparison.InvariantCultureIgnoreCase)
					).ToList();
					continue;
				case '*':
					profiles = profiles.Where(x =>
						x.AllProfiles.Any(y =>
							y.Key.Name.Contains(second, StringComparison.InvariantCultureIgnoreCase) ||
							y.Key.FullDescription().Contains(second, StringComparison.InvariantCultureIgnoreCase)
						)
					).ToList();
					continue;
				case '&':
					profiles = profiles.Where(x =>
						!x.AllProfiles.Any(y =>
							y.Key.Name.Contains(second, StringComparison.InvariantCultureIgnoreCase) ||
							y.Key.FullDescription().Contains(second, StringComparison.InvariantCultureIgnoreCase)
						)
					).ToList();
					continue;
				default:
					actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid filter.");
					return;
			}
		}

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from profile in profiles.OrderBy(x => x.Id)
				select
					new[]
					{
						profile.Id.ToString(), profile.Name?.Proper() ?? "Unnamed",
						profile.DesignedBody?.Name.Proper() ?? "None",
						profile.Description
					},
				new[] { "ID#", "Name", "Designed Body", "Description" }, actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void WearProfileShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.CombinedEffectsOfType<BuilderEditingEffect<IWearProfile>>().FirstOrDefault();
			if (effect is null)
			{
				actor.OutputHandler.Send("Which wear profile do you want to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.ShowTo(actor));
			return;
		}

		var profile = actor.Gameworld.WearProfiles.GetByIdOrName(ss.SafeRemainingArgument);
		if (profile is null)
		{
			actor.OutputHandler.Send("There is no such wear profile.");
			return;
		}

		actor.OutputHandler.Send(profile.ShowTo(actor));
	}
	#endregion

	private const string WearProfileHelp = @"The WearProfile command is used to create, edit and view wear profiles. 

Wear profiles are predefined ways in which garments can be worn and contain information about the bodyparts involved and some of their properties and descriptive terms. They are mostly used in conjunction with an item component of type 'wearable'.

The syntax to use this command is as follows:

	#3wearprofile list#0 - lists all of the wear profiles
	#3wearprofile edit <which>#0 - begin editing a wear profile
	#3wearprofile edit#0 - an alias for #wearprofile show#0 on your edited item
	#3wearprofile show <which>#0 - shows information about a wear profile
	#3wearprofile show#0 - shows your currently edited item
	#3wearprofile close#0 - stops editing your current wear profile
	#3wearprofile new shape|direct <name>#0 - creates a new wear profile
	#3wearprofile clone <which> <newname>#0 - clones an existing wear profile to another
	#3wearprofile set <...>#0 - edits the properties of a wear profile

You can use the following filters with #3wearprofile list#0:

	#6+<keyword>#0 - show only items with this keyword in their name or description
	#6-<keyword>#0 - exclude any items with this keyword in their name or description
	#6*<part>#0 - include items with any bodypart with this name
	#6&<part>#0 - exclude items with any bodypart with this name
	#6shape#0 - include only shape profiles
	#6part#0 - include only direct/bodypart profiles";

	[PlayerCommand("WearProfile", "wearprofile", "wp")]
	[CommandPermission(PermissionLevel.HighAdmin)]
	protected static void WearProfile(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.PopForSwitch())
		{
			case "show":
				WearProfileShow(actor, ss);
				return;
			case "list":
				WearProfileList(actor, ss);
				return;
			case "edit":
				WearProfileEdit(actor, ss);
				return;
			case "close":
				WearProfileClose(actor);
				return;
			case "clone":
				WearProfileClone(actor, ss);
				return;
			case "add":
			case "new":
				WearProfileAdd(actor, ss);
				break;
			case "set":
				WearProfileSet(actor, ss);
				break;
			default:
				actor.OutputHandler.Send(WearProfileHelp.SubstituteANSIColour());
				break;
		}
	}

	

	#endregion
	
	#region Trait Expressions

	public const string TraitExpressionHelp = @"The Trait Expression command is used to work with Trait Expressions, which are used in numerous points of the code to do mathematical formulas that can also parse the traits (i.e. skills and attributes) of a character. 

Examples of trait expressions might include damage formulas for weapons, the length of time someone can hold their breathe, caps for skills, etc.

Trait expressions can be edited with the following syntax:

	#3te list#0 - shows all trait expressions
	#3te show <id>#0 - shows a particular trait expression
	#3te edit <id>#0 - begins editing a trait expression
	#3te edit#0 - an alias for #3te show <editing id>#0
	#3te close#0 - stops editing a trait expression
	#3te clone <id>#0 - clones an existing expression and then begins editing the clone
	#3te new <name>#0 - creates a new trait expression
	#3te set name <name>#0 - edits the name of this trait expression
	#3te set formula <formula>#0 - edits the formula for the expression
	#3te set parameter <which> <trait>#0 - adds a new parameter named referring to the specified trait
	#3te set branch <which>#0 - toggles branching of an existing parameter
	#3te set improve <which>#0 - toggles improvement of a particular parameter
	#3te set trait <which> <trait>#0 - sets the trait for a particular parameter
	#3te set delete <which>#0 - deletes a particular parameter

You can use the following filters with the #3te list#0 command:

	#6+<keyword>#0 - formula or name should include the keyword
	#6-<keyword>#0 - formula and name should not include the keyword
	#6*<keyword>#0 - one of the parameters of the formula should be the named skill/attribute

#6Using parameters#0

The heart of the formulas is parameters. Each parameter has a name, and when the formula is evaluated by the engine that parameter will be replaced with an actual value. For example, you might have a parameter called #6strength#0 and at evaluate time, it points to the strength attribute of the character the expression is about.

There are two options for working with parameters:

1) You can add them at the top level, as per #3te set parameter#0
2) You can refer to them directly in the formula with the syntax #6name:id#0, e.g. #6strength:1#0

There is also a special parameter called #6variable#0, which some parts of the code will substitute with a skill value (or something else). This is the case with expressions for weapon checks for example, where the weapon skill will be substituted for the #6variable#0 parameter.

Additionally, you can mimic the effects of the #6nobranch#0 and #6noimprove#0 flags with type 2 variables by using syntax like the following: #6sorcery:123{nobranch,noimprove}#0. 

Finally, all of these parameters can be reused. So once you've done #6str:1#0 earlier in a formula, simply refering to #6str#0 later in the formula will be sufficient.

#6Special Functions#0

There are a number of special functions you can use (varname in the below stores the outcome in a variable that can be referred back to, it can otherwise be something perfunctory):

#5{varname race=race names/ids split by commas}#0 - 1 if the character is any of the listed races, 0 otherwise
#5{varname culture=culture names/ids split by commas}#0 - 1 if the character is any of the listed cultures, 0 otherwise
#5{varname merit=merit names/ids split by commas}#0 - 1 if the character has any of the listed merits, 0 otherwise
#5{varname role=role names/ids split by commas}#0 - 1 if the character has any of the listed roles, 0 otherwise
#5{varname class=class names/ids split by commas}#0 - 1 if the character has is any of the listed classes, 0 otherwise

#6Formula Functions#0

You can also use the following functions in your formula:

#3dice(num,sides)#0 - a random dice roll, e.g. dice(1,6) rolls a 6-sided dice
#3xdy#0 - e.g. #32d6#0 - a shorthand for doing #3dice(x,y)#0
#3rand(min,max)#0 - a random whole number between min and max
#3drand(min,max)#0 - a random decimal number between min and max
#3not(num)#0 - if num is exactly 0, then returns 1. Otherwise returns 0.
#3abs(num)#0 - Returns the absolute value of a specified number
#3acos(num)#0 - Returns the angle whose cosine is the specified number
#3asin(num)#0 - Returns the angle whose sine is the specified number
#3atan(num)#0 - Returns the angle whose tangent is the specified number
#3ceiling(num)#0 - Returns the smallest integer greater than or equal to the specified number
#3cos(num)#0 - Returns the cosine of the specified angle
#3exp(num)#0 - Returns e raised to the specified power
#3floor(num)#0 - Returns the largest integer less than or equal to the specified number
#3IEEERemainder(num,div)#0 - Returns the remainder resulting from the division of a specified number by another specified number
#3log(num)#0 - Returns the logarithm of a specified number
#3log10(num)#0 - Returns the base 10 logarithm of a specified number
#3max(num1,num2)#0 - Returns the larger of two specified numbers
#3min(num1,num2)#0 - Returns the smaller of two numbers
#3pow(num,power)#0 - Returns a specified number raised to the specified power
#3round(num)#0 - Rounds a value to the nearest integer or specified number of decimal places
#3sin(num)#0 - Returns the sine of the specified angle
#3sqrt(num)#0 - Returns the square root of a specified number
#3tan(num)#0 - Returns the tangent of the specified angle
#3truncate(num)#0	Calculates an integral part of a number
#3in(num1,num2,...,numn)#0 - Returns whether an element is in a set of values
#3if(cond,truevalue,falsevalue)#0 - Returns a value based on a condition

#6Examples#0

Here are some examples of plausible trait expressions applying the above:

1) #327#0

	This formula would return 27 every single time

2) #3axes:37{nobranch}#0 

	This would substitute the character's value for axes (skill 37), and not permit it to branch if they don't have it

3) #01d10+truncate(str:1/10)#0

	This would roll 1 ten-sided dice and add +1 for every 10 whole points of strength the character had

4) #3{ismartial class=warrior,paladin,barbarian,rogue,monk}*{str:1}+{isarcane class=mage,sorcerer,warlock}*{int:2}#0

	This would use the value of a character's strength if they were a martial class, or intelligence if they were arcane

5) #3swimming:45 + {value merit=Natural Swimmer}*50#0

	This formula combines swimming with a +50 bonus if they have the natural swimmer merit (likely to be a trait expression for a check)

6) #3listening:12 + {value merit=Good Ears}*30 + {elf race=Elf}*50#0

	This formula combines the listening skill, a +30 bonus for the merit Good Ears, and an additional +50 bonus for the Elf race

7) #3min(99, (2 * dex:4) + (1 * str:1) + (2 * wil:5))#0

	This formula is probably a skill cap combining 2*dex, str, and 2*wil, with a cap of 99";

	[PlayerCommand("TraitExpression", "traitexpression", "te")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("traitexpression", TraitExpressionHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void TraitExpression(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "new":
				TraitExpressionNew(actor, ss);
				return;
			case "edit":
				TraitExpressionEdit(actor, ss);
				return;
			case "close":
				TraitExpressionClose(actor, ss);
				return;
			case "set":
				TraitExpressionSet(actor, ss);
				return;
			case "delete":
				TraitExpressionDelete(actor, ss);
				return;
			case "clone":
				TraitExpressionClone(actor, ss);
				return;
			case "list":
				TraitExpressionList(actor, ss);
				return;
			case "show":
			case "view":
				TraitExpressionView(actor, ss);
				return;
		}

		actor.OutputHandler.Send(TraitExpressionHelp.SubstituteANSIColour());
	}

	private static void TraitExpressionView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished && !actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You must specify a trait expression to show if you are not editing one.");
			return;
		}

		ITraitExpression expression;
		if (ss.IsFinished)
		{
			expression = actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().First().EditingItem;
		}
		else
		{
			expression = long.TryParse(ss.PopSpeech(), out var value)
				? actor.Gameworld.TraitExpressions.Get(value)
				: actor.Gameworld.TraitExpressions.GetByName(ss.Last);
			if (expression == null)
			{
				actor.OutputHandler.Send("There is no such trait expression to show you.");
				return;
			}
		}

		actor.OutputHandler.Send(expression.ShowBuilder(actor));
	}

	private static void TraitExpressionList(ICharacter actor, StringStack ss)
	{
		var expressions = actor.Gameworld.TraitExpressions.ToList();
		// Filters
		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech();
			if (cmd.Length < 2)
			{
				continue;
			}

			var first = cmd[0];
			cmd = cmd.Substring(1);
			switch (first)
			{
				case '+':
					expressions = expressions
					              .Where(x => 
						              x.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
									  x.OriginalFormulaText.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) ||
									  x.Parameters.Any(y => y.Key.Contains(cmd, StringComparison.InvariantCultureIgnoreCase))
						            )
					              .ToList();
					continue;
				case '-':
					expressions = expressions
					              .Where(x =>
						              !x.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
						              !x.OriginalFormulaText.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) &&
									  !x.Parameters.Any(y => y.Key.Contains(cmd, StringComparison.InvariantCultureIgnoreCase))
					              )
					              .ToList();
					continue;
				case '*':
					expressions = expressions
					              .Where(x => x.Parameters.Any(y => y.Value.Trait?.Name.Contains(cmd, StringComparison.InvariantCultureIgnoreCase) == true))
					              .ToList();
					continue;
				default:
					actor.OutputHandler.Send($"Invalid filter: {ss.Last.ColourCommand()}");
					return;
			}
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in expressions
			select new[]
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.OriginalFormulaText
			},
			new[] { "ID", "Name", "Formula" },
			actor.LineFormatLength, truncatableColumnIndex: 2, colour: Telnet.Green,
			unicodeTable: actor.Account.UseUnicode
		));
	}

	private static void TraitExpressionClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which trait expression would you like to clone?");
			return;
		}

		var expression = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.TraitExpressions.Get(value)
			: actor.Gameworld.TraitExpressions.GetByName(ss.Last);
		if (expression == null)
		{
			actor.OutputHandler.Send("There is no such trait expression to clone.");
			return;
		}

		var newexpr = new TraitExpression((TraitExpression)expression);
		actor.Gameworld.Add(newexpr);
		actor.OutputHandler.Send(
			$"You clone trait expression #{expression.Id.ToString("N0", actor)} into a new expression with id #{newexpr.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
		actor.AddEffect(new BuilderEditingEffect<ITraitExpression>(actor) { EditingItem = newexpr });
	}

	private static void TraitExpressionDelete(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any trait expressions.");
			return;
		}

		actor.OutputHandler.Send("TODO");
	}

	private static void TraitExpressionSet(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any trait expressions.");
			return;
		}

		actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().First().EditingItem.BuildingCommand(actor, ss);
	}

	private static void TraitExpressionClose(ICharacter actor, StringStack ss)
	{
		if (!actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
		{
			actor.OutputHandler.Send("You aren't editing any trait expressions.");
			return;
		}

		actor.OutputHandler.Send($"You are no longer editing any trait expressions.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
	}

	private static void TraitExpressionEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			if (actor.EffectsOfType<BuilderEditingEffect<ITraitExpression>>().Any())
			{
				TraitExpressionView(actor, ss);
				return;
			}

			actor.OutputHandler.Send("Which trait expression would you like to edit?");
			return;
		}

		var expr = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.TraitExpressions.Get(value)
			: actor.Gameworld.TraitExpressions.GetByName(ss.Last);
		if (expr == null)
		{
			actor.OutputHandler.Send("There is no such trait expression to edit.");
			return;
		}

		actor.OutputHandler.Send($"You are now editing trait expression #{expr.Id.ToString("N0", actor)}.");
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
		actor.AddEffect(new BuilderEditingEffect<ITraitExpression>(actor) { EditingItem = expr });
	}

	private static void TraitExpressionNew(ICharacter actor, StringStack ss)
	{
		var name = "Unnamed Trait Expression";
		if (!ss.IsFinished)
		{
			name = ss.SafeRemainingArgument;
			if (actor.Gameworld.TraitExpressions.Any(x => x.Name.EqualTo(name)))
			{
				actor.OutputHandler.Send("There is already a Trait Expression with that name. Names must be unique.");
				return;
			}
		}

		var newexpr = new TraitExpression(actor.Gameworld, name);

		actor.Gameworld.Add(newexpr);
		actor.RemoveAllEffects(x => x.IsEffectType<BuilderEditingEffect<ITraitExpression>>());
		actor.AddEffect(new BuilderEditingEffect<ITraitExpression>(actor) { EditingItem = newexpr });
		actor.OutputHandler.Send(
			$"You create a new trait expression with ID #{newexpr.Id}, which you are now editing.");
	}

	#endregion

	#region Materials

	private const string GasHelpText =
		@"The gas command allows you to create and edit gases and their properties. These gases in turn can be used by items, characters and other effects.

The syntax for this command is as follows:

	#3gas list#0 - Lists all gases
	#3gas show <which>#0 - shows information about a gases
	#3gas edit <which>#0 - begins editing a gas
	#3gas edit#0 - same as #3material show#0 for your currently edited gas
	#3gas clone <which> <new name> <new description>#0 - clones a gas
	#3gas new <name>#0 - creates a new gas
	#3gas set organic#0 - toggles counting as organic
	#3gas set description <description>#0 - sets the description
	#3gas set density <value>#0 - sets density in kg/m3
	#3gas set electrical <value>#0 - sets electrical conductivity in siemens
	#3gas set thermal <value>#0 - sets the thermal conductivity in watts/kelvin
	#3gas set specificheat <value>#0 - sets the specific heat capacity in J/Kg.Kelvin
	#3gas set colour <ansi>#0 - sets the display colour
	#3gas set drug <which>#0 - sets the contained drug
	#3gas set drug none#0 - clears the drug
	#3gas set drugvolume <amount>#0 - sets the drug volume
	#3gas set viscosity <viscosity>#0 - sets the viscosity in cSt
	#3gas set smell <intensity> <smell> [<vague smell>]#0 - sets the smell
	#3gas set countsas <gas>#0 - sets a gas that this counts as
	#3gas set countsas none#0 - clears a counts-as gas
	#3gas set quality <quality>#0 - sets the maximum quality of the gas when counting-as
	#3gas set condensation <temp>|none#0 - sets or clears the temperature at which this gas becomes a liquid
	#3gas set liquid <liquid>|none#0 - sets or clears the liquid form of this gas";

	[PlayerCommand("Gas", "gas")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Gas(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clone":
				GasClone(actor, ss);
				return;
			case "new":
				GasNew(actor, ss);
				return;
			case "edit":
				GasEdit(actor, ss);
				return;
			case "set":
				GasSet(actor, ss);
				return;
			case "close":
				GasClose(actor, ss);
				return;
			case "show":
			case "view":
				GasShow(actor, ss);
				return;
			case "list":
				GasList(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(GasHelpText.SubstituteANSIColour());
				return;
		}
	}


	private static void GasShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any gases. You must either edit one or specify which one you'd like to show.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var gas = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Gases.Get(value)
			: actor.Gameworld.Gases.GetByName(ss.Last);
		if (gas == null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return;
		}

		actor.OutputHandler.Send(gas.Show(actor));
	}


	private static void GasClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any gases.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.OutputHandler.Send("You are no longer editing any gases.");
	}

	private static void GasSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any gases.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void GasEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<IGas>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("You must specify a gas that you wish to edit.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var gas = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Gases.Get(value)
			: actor.Gameworld.Gases.GetByName(ss.Last);
		if (gas == null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.AddEffect(new BuilderEditingEffect<IGas>(actor) { EditingItem = gas });
		actor.OutputHandler.Send($"You are now editing gas {gas.Name.Colour(gas.DisplayColour)}.");
	}

	private static void GasNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give your new gas?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Gases.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a gas with that name. Names must be unique.");
			return;
		}

		var newGas = new Gas(name, actor.Gameworld);
		actor.OutputHandler.Send(
			$"You create a new gas called {name.Colour(newGas.DisplayColour)} with ID #{newGas.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.AddEffect(new BuilderEditingEffect<IGas>(actor) { EditingItem = newGas });
	}

	private static void GasClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which gas do you want to clone?");
			return;
		}

		var gas = actor.Gameworld.Gases.GetByIdOrName(ss.PopSpeech());
		if (gas == null)
		{
			actor.OutputHandler.Send("There is no such gas.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your cloned gas?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Gases.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a gas with that name. Gas names must be unique.");
			return;
		}

		var newMaterial = gas.Clone(name);
		actor.OutputHandler.Send(
			$"You clone the gas {gas.Name.Colour(gas.DisplayColour)} as {name.Colour(gas.DisplayColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IGas>>();
		actor.AddEffect(new BuilderEditingEffect<IGas>(actor) { EditingItem = newMaterial });
	}

	private static void GasList(ICharacter actor, StringStack command)
	{
		var gases = actor.Gameworld.Gases.AsEnumerable();
		while (!command.IsFinished)
		{
			// Filter
			var cmd = command.PopSpeech();
			switch (cmd.ToLowerInvariant().CollapseString())
			{
				default:
					actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid filter for gases.");
					return;
			}
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from gas in gases
			select new List<string>
			{
				gas.Id.ToString("N0", actor),
				gas.Name,
				gas.MaterialDescription.Colour(gas.DisplayColour),
				gas.CountsAs is not null
					? $"{gas.CountsAs.Name.Colour(gas.CountsAs.DisplayColour)} @ {gas.CountsAsQuality.Describe().ColourValue()}"
					: "",
				gas.Drug is not null
					? $"{gas.DrugGramsPerUnitVolume.ToString("N3", actor).ColourValue()}g/L {gas.Drug.Name.ColourName()}"
					: ""
			},
			new List<string>
			{
				"Id",
				"Name",
				"Description",
				"Count As",
				"Drug"
			},
			actor,
			Telnet.Orange
		));
	}

	private const string LiquidHelpText =
		@"The material command allows you to create and edit solid materials and their properties. These materials in turn can be used by items, characters and other effects.

The syntax for this command is as follows:

	#3liquid list#0 - Lists all liquids
	#3liquid show <which>#0 - shows information about a liquid
	#3liquid edit <which>#0 - begins editing a liquid
	#3liquid edit#0 - same as #3liquid show#0 for your currently edited liquid
	#3liquid clone <which> <new name> <new description>#0 - clones a liquid
	#3liquid new <name>#0 - creates a new liquid
	#3liquid set organic#0 - toggles counting as organic
	#3liquid set description <description>#0 - sets the description
	#3liquid set density <value>#0 - sets density in kg/m3
	#3liquid set electrical <value>#0 - sets electrical conductivity in siemens
	#3liquid set thermal <value>#0 - sets the thermal conductivity in watts/kelvin
	#3liquid set specificheat <value>#0 - sets the specific heat capacity in J/Kg.Kelvin
	#3liquid set colour <ansi>#0 - sets the display colour
	#3liquid set drug <which>#0 - sets the contained drug
	#3liquid set drug none#0 - clears the drug
	#3liquid set drugvolume <amount>#0 - sets the drug volume
	#3liquid set viscosity <viscosity>#0 - sets the viscosity in cSt
	#3liquid set smell <intensity> <smell> [<vague smell>]#0 - sets the smell
	#3liquid set taste <intensity> <taste> [<vague taste>]#0 - sets the taste
	#3liquid set ldesc <desc>#0 - sets the more detailed description when looked at
	#3liquid set alcohol <litres per litre>#0 - how many litres of pure alcohol per litre of liquid
	#3liquid set thirst <hours>#0 - how many hours of thirst quenched per litre drunk
	#3liquid set hunger <hours>#0 - how many hours of hunger quenched per litre drunk
	#3liquid set water <litres per litre>#0 - how many litres of hydrating water per litre of liquid#B*#0
	#3liquid set calories <calories per litre>#0 - how many calories per litre of liquid#B*#0
	#3liquid set prog <which>#0 - sets a prog to be executed when the liquid is drunk
	#3liquid set prog none#0 - clears the draught prog
	#3liquid set solvent <liquid>#0 - sets a solvent required for cleaning this liquid off things
	#3liquid set solvent none#0 - no solvent required for cleaning
	#3liquid set solventratio <percentage>#0 - sets the volume of solvent to contaminant required
	#3liquid set residue <solid>#0 - sets a material to leave behind as a residue when dry
	#3liquid set residue none#0 - dry clean, leave no residue
	#3liquid set residueamount <percentage>#0 - percentage of weight of liquid that is residue
	#3liquid set countsas <liquid>#0 - sets another liquid that this one counts as
	#3liquid set countsas none#0 - this liquid counts as no other liquid
	#3liquid set countquality <quality>#0 - sets the maximum quality for this when counting as
	#3liquid set freeze <temp>#0 - sets the freeze temperature of this liquid#B*#0
	#3liquid set boil <temp>#0 - sets the boil temperature of this liquid#B*#0
	#3liquid set ignite <temp>#0 - sets the ignite temperature of this liquid#B*#0
	#3liquid set ignite none#0 - clears the ignite temperature of this liquid#B*#0

#9Note#0: Liquid properties marked with a #B*#0 above are currently not used by the engine but will see inclusion in the future.";

	[PlayerCommand("Liquid", "liquid")]
	[CommandPermission(PermissionLevel.Admin)]
	protected static void Liquid(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clone":
				LiquidClone(actor, ss);
				return;
			case "new":
				LiquidNew(actor, ss);
				return;
			case "edit":
				LiquidEdit(actor, ss);
				return;
			case "set":
				LiquidSet(actor, ss);
				return;
			case "close":
				LiquidClose(actor, ss);
				return;
			case "show":
			case "view":
				LiquidShow(actor, ss);
				return;
			case "list":
				LiquidList(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(LiquidHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void LiquidList(ICharacter actor, StringStack command)
	{
		var liquids = actor.Gameworld.Liquids.AsEnumerable();
		while (!command.IsFinished)
		{
			// Filter
			var cmd = command.PopSpeech();
			switch (cmd.ToLowerInvariant().CollapseString())
			{
				default:
					actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid filter for liquids.");
					return;
			}
		}

		actor.Send(
			StringUtilities.GetTextTable(
				from liquid in liquids
				select new[]
				{
					liquid.Id.ToString("N0", actor),
					liquid.Name.TitleCase(),
					liquid.MaterialDescription.Proper().Colour(liquid.DisplayColour),
					liquid.Organic.ToColouredString(),
					liquid.WaterLitresPerLitre.ToString("P0", actor),
					liquid.DrinkSatiatedHoursPerLitre.ToString("N1", actor),
					liquid.CaloriesPerLitre.ToString("N0", actor),
					liquid.FoodSatiatedHoursPerLitre.ToString("N1", actor),
					liquid.AlcoholLitresPerLitre.ToString("P0", actor),
					liquid.DraughtProg?.MXPClickableFunctionNameWithId() ?? "None",
					liquid.Drug != null
						? $"{liquid.DrugGramsPerUnitVolume.ToString("N4", actor).ColourValue()}g/L {liquid.Drug.Name.ColourName()}"
						: ""
				},
				new[]
				{
					"ID",
					"Name",
					"Description",
					"Organic",
					"Water",
					"Thirst",
					"Calories",
					"Food",
					"Alcohol",
					"Draught",
					"Drug"
				},
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	private static void LiquidShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any liquids. You must either edit one or specify which one you'd like to show.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var liquid = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Liquids.Get(value)
			: actor.Gameworld.Liquids.GetByName(ss.Last);
		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return;
		}

		actor.OutputHandler.Send(liquid.Show(actor));
	}

	private static void LiquidClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any liquids.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.OutputHandler.Send("You are no longer editing any liquids.");
	}

	private static void LiquidSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any liquids.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void LiquidEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ILiquid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("You must specify a liquid that you wish to edit.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var liquid = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Liquids.Get(value)
			: actor.Gameworld.Liquids.GetByName(ss.Last);
		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.AddEffect(new BuilderEditingEffect<ILiquid>(actor) { EditingItem = liquid });
		actor.OutputHandler.Send($"You are now editing liquid {liquid.Name.Colour(liquid.DisplayColour)}.");
	}

	private static void LiquidNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give your new liquid?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Liquids.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a liquid with that name. Names must be unique.");
			return;
		}

		var newLiquid = new Liquid(name, actor.Gameworld);
		actor.OutputHandler.Send(
			$"You create a new liquid called {name.Colour(newLiquid.DisplayColour)} with ID #{newLiquid.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.AddEffect(new BuilderEditingEffect<ILiquid>(actor) { EditingItem = newLiquid });
	}

	private static void LiquidClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which liquid do you want to clone?");
			return;
		}

		var liquid = actor.Gameworld.Liquids.GetByIdOrName(ss.PopSpeech());
		if (liquid == null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your cloned liquid?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Liquids.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a liquid with that name. Liquid names must be unique.");
			return;
		}

		var newMaterial = liquid.Clone(name);
		actor.OutputHandler.Send(
			$"You clone the liquid {liquid.Name.Colour(liquid.DisplayColour)} as {name.Colour(liquid.DisplayColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ILiquid>>();
		actor.AddEffect(new BuilderEditingEffect<ILiquid>(actor) { EditingItem = newMaterial });
	}

	private const string MaterialHelpText =
		@"The material command allows you to create and edit solid materials and their properties. These materials in turn can be used by items, characters and other effects.

The syntax for this command is as follows:

	#3material list#0 - Lists all materials
	#3material show <which>#0 - shows information about a material
	#3material edit <which>#0 - begins editing a material
	#3material edit#0 - same as #3material show#0 for your currently edited material
	#3material clone <which> <new name> <new description>#0 - clones a material
	#3material new <type> <name>#0 - creates a new material of the specified type
	#3material set organic#0 - toggles counting as organic
	#3material set description <description>#0 - sets the description
	#3material set density <value>#0 - sets density in kg/m3
	#3material set electrical <value>#0 - sets electrical conductivity in siemens
	#3material set thermal <value>#0 - sets the thermal conductivity in watts/kelvin
	#3material set specificheat <value>#0 - sets the specific heat capacity in J/Kg.Kelvin
	#3material set impactyield <value>#0 - sets the impact yield strength in kPa
	#3material set impactfracture <value>#0 - sets the impact fracture strength in kPa
	#3material set impactstrain <value>#0 - sets the strain at yield for impact
	#3material set shearyield <value>#0 - sets the shear yield strength in kPa
	#3material set shearfracture <value>#0 - sets the shear fracture strength in kPa
	#3material set shearstrain <value>#0 - sets the strain at yield for shear
	#3material set heatdamage <temp>|none#0 - sets or clears the temperature for heat damage
	#3material set ignition <temp>|none#0 - sets or clears the temperature for ignition
	#3material set melting <temp>|none#0 - sets or clears the temperature for melting
	#3material set absorbency <%>#0 - sets the absorbency for liquids
	#3material set solvent <liquid>|none#0 - sets or clears the required solvent for residues
	#3material set solventratio <%>#0 - sets the ratio of solvent to removed contaminant by mass
	#3material set liquidform <liquid>|none#0 - sets or clears a liquid as the liquid form of this
	#3material set residuecolour <colour>#0 - sets the colour of this material and its residues
	#3material set residuesdesc <tag>|none#0 - sets or clears the sdesc tag for residues of this
	#3material set residuedesc <desc>|none#0 - sets or clears the added description for residues of this";

	[PlayerCommand("Material", "material")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("material", MaterialHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Material(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "clone":
				MaterialClone(actor, ss);
				return;
			case "new":
				MaterialNew(actor, ss);
				return;
			case "edit":
				MaterialEdit(actor, ss);
				return;
			case "set":
				MaterialSet(actor, ss);
				return;
			case "close":
				MaterialClose(actor, ss);
				return;
			case "show":
			case "view":
				MaterialShow(actor, ss);
				return;
			case "list":
				MaterialList(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(MaterialHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void MaterialShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send(
					"You are not editing any material. You must either edit one or specify which one you'd like to show.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var material = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Materials.Get(value)
			: actor.Gameworld.Materials.GetByName(ss.Last);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return;
		}

		actor.OutputHandler.Send(material.Show(actor));
	}

	private static void MaterialClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any materials.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.OutputHandler.Send("You are no longer editing any materials.");
	}

	private static void MaterialSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any materials.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void MaterialEdit(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			var effect = actor.EffectsOfType<BuilderEditingEffect<ISolid>>().FirstOrDefault();
			if (effect == null)
			{
				actor.OutputHandler.Send("You must specify a material that you wish to edit.");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var material = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Materials.Get(value)
			: actor.Gameworld.Materials.GetByName(ss.Last);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.AddEffect(new BuilderEditingEffect<ISolid>(actor) { EditingItem = material });
		actor.OutputHandler.Send($"You are now editing material {material.Name.Colour(Telnet.Yellow)}.");
	}

	private static void MaterialNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What behaviour type did you want to set for your new material? Valid options are: {Enum.GetValues(typeof(MaterialBehaviourType)).OfType<MaterialBehaviourType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return;
		}

		if (!ss.PopSpeech().TryParseEnum<MaterialBehaviourType>(out var behaviour))
		{
			actor.OutputHandler.Send(
				$"That is not a valid behaviour type. Valid options are: {Enum.GetValues(typeof(MaterialBehaviourType)).OfType<MaterialBehaviourType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give your new material?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Materials.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a material with that name. Names must be unique.");
			return;
		}

		var newMaterial = new Solid(name, behaviour, actor.Gameworld);
		actor.OutputHandler.Send(
			$"You create a new {behaviour.DescribeEnum().ColourValue()} material called {name.Colour(newMaterial.ResidueColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.AddEffect(new BuilderEditingEffect<ISolid>(actor) { EditingItem = newMaterial });
	}

	private static void MaterialClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which material do you want to clone?");
			return;
		}

		var material = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Materials.Get(value)
			: actor.Gameworld.Materials.GetByName(ss.Last);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your cloned material?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (actor.Gameworld.Materials.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a material with that name. Material names must be unique.");
			return;
		}

		var description = name;
		if (!ss.IsFinished)
		{
			description = ss.PopSpeech();
		}

		var newMaterial = material.Clone(name, description);
		actor.OutputHandler.Send(
			$"You clone the material {material.Name.Colour(material.ResidueColour)} as {name.Colour(material.ResidueColour)} with ID #{newMaterial.Id.ToString("N0", actor)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ISolid>>();
		actor.AddEffect(new BuilderEditingEffect<ISolid>(actor) { EditingItem = newMaterial });
	}

	private static void MaterialList(ICharacter actor, StringStack command)
	{
		var materials = actor.Gameworld.Materials.AsEnumerable();
		while (!command.IsFinished)
		{
			if (!command.PopSpeech().TryParseEnum<MaterialBehaviourType>(out var materialType))
			{
				actor.Send("There is no such material general type to filter by.");
				return;
			}

			materials = materials.Where(x => x.BehaviourType == materialType);
		}

		actor.Send(
			StringUtilities.GetTextTable(
				from material in materials
				select new[]
				{
					material.Id.ToString("N0", actor),
					material.Name,
					material.MaterialDescription.Colour(material.ResidueColour),
					material.BehaviourType.DescribeEnum(),
					$"{material.Density.ToString("N0").ColourValue()}kg/m3",
					material.Solvent is not null
						? $"{material.SolventRatio.ToString("P2", actor).ColourValue()} x {material.Solvent.Name.Colour(material.Solvent.DisplayColour)}"
						: ""
				},
				new[]
				{
					"ID",
					"Name",
					"Description",
					"Type",
					"Density",
					"Solvent"
				},
				actor.Account.LineFormatLength,
				colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode
			)
		);
	}

	#endregion

	#region Tattoos

	protected const string TattooHelp =
		@$"The tattoo command is used to view, create and inscribe tattoos. Players are able to create and submit their own tattoos but only admins can approve them.

The following options are used to view, edit and create tattoo designs:

	#3tattoo list [all|mine|+key|-key]#0 - lists all tattoos
	#3tattoo edit <id|name>#0 - opens the specified tattoo for editing
	#3tattoo edit new <name>#0 - creates a new tattoo for editing
	#3tattoo edit#0 - equivalent of doing SHOW on your currently editing tattoo
	#3tattoo clone <id|name> <new name>#0 - creates a carbon copy of a tattoo for editing
	#3tattoo show <id|name>#0 - shows a particular tattoo.
	#3tattoo set <subcommand>#0 - changes something about the tattoo. See its help for more info.
	#3tattoo edit submit#0 - submits a tattoo for review

{GenericReviewableSearchList}

The following commands are used to put tattoos on people:

	tattoo inscribe <target> <tattoo id|name> <bodypart> - begins inscribing a tattoo on someone
	tattoo continue <target> <tattoo keyword> - continues inscribing an unfinished tattoo on someone";

	protected const string TattooAdminHelp =
		@$"The tattoo command is used to view, create and inscribe tattoos. Players are able to create and submit their own tattoos but only admins can approve them.

The following options are used to view, edit and create tattoo designs:

	#3tattoo list [all|mine|+key|-key]#0 - lists all tattoos
	#3tattoo edit <id|name>#0 - opens the specified tattoo for editing
	#3tattoo edit new <name>#0 - creates a new tattoo for editing
	#3tattoo edit#0 - equivalent of doing SHOW on your currently editing tattoo
	#3tattoo clone <id|name> <new name>#0 - creates a carbon copy of a tattoo for editing
	#3tattoo show <id|name>#0 - shows a particular tattoo.
	#3tattoo set <subcommand>#0 - changes something about the tattoo. See its help for more info.
	#3tattoo edit submit#0 - submits a tattoo for review
	#3tattoo review all|mine|<id|name>#0 - reviews a submitted tattoo
	#3tattoo review list#0 - shows all tattoos submitted for review

{GenericReviewableSearchList}

The following commands are used to put tattoos on people:

	#3tattoo inscribe <target> <tattoo id|name> <bodypart>#0 - begins inscribing a tattoo on someone
	#3tattoo continue <target> <tattoo keyword>#0 - continues inscribing an unfinished tattoo on someone

Also, as an admin you should see the two related commands #3GIVETATTOO#0 and #3FINISHTATTOO#0.";

	[PlayerCommand("Tattoo", "tattoo")]
	[RequiredCharacterState(CharacterState.Able)]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[HelpInfo("tattoo", TattooHelp, AutoHelp.HelpArgOrNoArg, TattooAdminHelp)]
	protected static void Tattoo(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(TattooHelp.SubstituteANSIColour());
			return;
		}

		switch (ss.PopSpeech())
		{
			case "list":
				TattooList(actor, ss);
				break;
			case "edit":
				TattooEdit(actor, ss);
				break;
			case "set":
				TattooSet(actor, ss);
				break;
			case "clone":
				TattooClone(actor, ss);
				break;
			case "show":
			case "view":
				TattooView(actor, ss);
				break;
			case "review":
				TattooReview(actor, ss);
				break;
			case "inscribe":
				TattooInscribe(actor, ss);
				break;
			case "continue":
				TattooContinue(actor, ss);
				break;
			case "help":
			case "?":
			default:
				actor.OutputHandler.Send(TattooHelp.SubstituteANSIColour());
				return;
		}
	}

	private static IInventoryPlanTemplate _tattooNeedlePlan;

	private static IInventoryPlanTemplate TattooNeedlePlan
	{
		get
		{
			if (_tattooNeedlePlan == null)
			{
				_tattooNeedlePlan = new InventoryPlanTemplate(Futuremud.Games.First(), new[]
				{
					new InventoryPlanPhaseTemplate(1, new[]
					{
						new InventoryPlanActionHold(Futuremud.Games.First(),
							Futuremud.Games.First().GetStaticLong("TattooNeedleTag"), 0, null, null)
						{
							OriginalReference = "tattoo tool",
							ItemsAlreadyInPlaceOverrideFitnessScore = true
						}
					})
				});
			}

			return _tattooNeedlePlan;
		}
	}

	private static void TattooContinue(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Whose unfinished tattoo do you want to continue work on?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You do not see anyone like that.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You cannot work on your own tattoos.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which tattoo of {target.ApparentGender(actor).GeneralPossessive()} do you want to finish?");
			return;
		}

		var tattoo = target.Body.Tattoos.Where(x => target.Body.ExposedBodyparts.Contains(x.Bodypart))
		                   .GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (tattoo == null)
		{
			actor.OutputHandler.Send(
				$"You cannot see any such tattoo on {target.HowSeen(actor, false, DescriptionType.Possessive)} body.");
			return;
		}

		if (tattoo.CompletionPercentage >= 1.0)
		{
			actor.OutputHandler.Send("That tattoo is already complete and needs no further work.");
			return;
		}

		if (!tattoo.TattooTemplate.CanProduceTattoo(actor))
		{
			actor.OutputHandler.Send("You don't know how to finish that tattoo off.");
			return;
		}

		var plan = TattooNeedlePlan.CreatePlan(actor);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
		}

		void BeginTattooAction()
		{
			if (!CharacterState.Able.HasFlag(actor.State))
			{
				actor.OutputHandler.Send(
					$"You can no longer continue to work on that tattoo because you are {actor.State.Describe()}.");
				return;
			}

			plan = TattooNeedlePlan.CreatePlan(actor);
			switch (plan.PlanIsFeasible())
			{
				case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
			}

			if (!actor.ColocatedWith(target))
			{
				actor.OutputHandler.Send("Your target is no longer in the same location as you.");
				return;
			}

			if (!target.Body.Tattoos.Contains(tattoo))
			{
				actor.OutputHandler.Send("The target of your tattoo inscription no longer has the tattoo...");
				return;
			}

			if (!target.Body.ExposedBodyparts.Contains(tattoo.Bodypart))
			{
				actor.OutputHandler.Send(
					"The target bodypart for your tattoo inscription is covered up. You must have bare skin to work with.");
				return;
			}

			if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
			{
				actor.OutputHandler.Send(
					$"You must first stop {actor.Effects.First(x => x.IsBlockingEffect("general")).BlockingDescription("general", actor)} before you can begin inscribing a tattoo.");
				return;
			}

			if (actor.Movement != null || target.Movement != null)
			{
				actor.OutputHandler.Send("Neither you nor your target can be moving if you want to inscribe a tattoo.");
				return;
			}

			if (actor.Combat != null || target.Combat != null)
			{
				actor.OutputHandler.Send("You cannot inscribe tattoos while you or your target are in combat.");
				return;
			}

			if (target.EffectsOfType<HavingTattooInked>().Any())
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} is already having a tattoo inked. Only one tattoo can be worked on at a time.");
				return;
			}

			var inkplan = tattoo.TattooTemplate.GetInkPlan(actor);
			if (inkplan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
			{
				actor.OutputHandler.Send("You do not have the inks you require to work on that tattoo.");
				return;
			}

			var results = plan.ExecuteWholePlan();
			inkplan.ExecuteWholePlan();
			plan.FinalisePlanNoRestore();
			inkplan.FinalisePlanNoRestore();
			actor.AddEffect(
				new InkingTattoo(actor, target, tattoo,
					results.First(x => x.OriginalReference?.ToString().EqualTo("tattoo tool") == true).PrimaryTarget),
				InkingTattoo.EffectTimespan);
		}

		if (!target.UnableToResistInterventions(actor))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is proposing to begin continue working on $1's !2.", actor, actor, target,
				new DummyPerceivable(
					perc => tattoo.ShortDescription.SubstituteWrittenLanguage(perc, actor.Gameworld).Strip_A_An(),
					perc => ""))));
			target.OutputHandler.Send(Accept.StandardAcceptPhrasing);
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = perc => BeginTattooAction(),
				RejectAction = perc =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting tattoo work from $1.", target, target, actor)));
				},
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting tattoo work from $1.", target, target, actor)));
				},
				DescriptionString = "Proposing to continue a tattoo",
				Keywords = new List<string> { "tattoo", "inscribe" }
			}), TimeSpan.FromSeconds(120));
		}
		else
		{
			BeginTattooAction();
		}
	}

	private static void TattooInscribe(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which tattoo do you want to inscribe?");
			return;
		}

		if ((long.TryParse(ss.PopSpeech(), out var value)
			    ? actor.Gameworld.DisfigurementTemplates.Get(value)
			    : actor.Gameworld.DisfigurementTemplates.GetByName(ss.Last)) is not ITattooTemplate template ||
		    !template.CanSeeTattooInList(actor))
		{
			actor.OutputHandler.Send("There is no such tattoo.");
			return;
		}

		if (template.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("That tattoo template is not approved for use.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to inscribe that tatoo on?");
			return;
		}

		var target = actor.TargetActor(ss.PopSpeech());
		if (target == null)
		{
			actor.OutputHandler.Send("You don't see anyone like that here.");
			return;
		}

		if (target == actor)
		{
			actor.OutputHandler.Send("You are not able to inscribe tattoos on yourself.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which bodypart do you want to put the tattoo on?");
			return;
		}

		if (target.Body.GetTargetBodypart(ss.PopSpeech()) is not IExternalBodypart bodypart)
		{
			actor.OutputHandler.Send($"{target.HowSeen(actor, true)} does not have any such bodypart.");
			return;
		}

		if (!target.Body.ExposedBodyparts.Contains(bodypart))
		{
			actor.OutputHandler.Send(
				$"{target.HowSeen(actor, true, DescriptionType.Possessive)} {bodypart.FullDescription()} is covered up. You must have bare skin to tattoo.");
			return;
		}

		if (!template.CanBeAppliedToBodypart(target.Body, bodypart))
		{
			actor.OutputHandler.Send(
				$"The {template.Name.Colour(Telnet.Cyan)} tattoo cannot be applied to the {bodypart.FullDescription().ColourValue()} bodypart.");
			return;
		}

		if (!template.CanProduceTattoo(actor))
		{
			actor.OutputHandler.Send("You are not capable of inscribing that tattoo.");
			return;
		}

		// TODO - skill requirements
		var plan = TattooNeedlePlan.CreatePlan(actor);
		switch (plan.PlanIsFeasible())
		{
			case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
				actor.OutputHandler.Send(
					$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
			case InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
				plan.FinalisePlanNoRestore();
				return;
		}

		void BeginTattooAction()
		{
			if (!CharacterState.Able.HasFlag(actor.State))
			{
				actor.OutputHandler.Send(
					$"You can no longer begin a tattoo inscription because you are {actor.State.Describe()}.");
				return;
			}

			plan = TattooNeedlePlan.CreatePlan(actor);
			switch (plan.PlanIsFeasible())
			{
				case InventoryPlanFeasibility.NotFeasibleNotEnoughHands:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleNotEnoughWielders:
					actor.OutputHandler.Send(
						$"You do not have enough free {actor.Body.WielderDescriptionPlural} to pick up the tool you need to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
				case InventoryPlanFeasibility.NotFeasibleMissingItems:
					actor.OutputHandler.Send($"You are lacking a tool with which to inscribe tattoos.");
					plan.FinalisePlanNoRestore();
					return;
			}

			if (!actor.ColocatedWith(target))
			{
				actor.OutputHandler.Send("Your target is no longer in the same location as you.");
				return;
			}

			if (!target.Body.Bodyparts.Contains(bodypart))
			{
				actor.OutputHandler.Send(
					"The target of your tattoo inscription no longer has the bodypart you were planning on tattooing.");
				return;
			}

			if (!target.Body.ExposedBodyparts.Contains(bodypart))
			{
				actor.OutputHandler.Send(
					"The target bodypart for your tattoo inscription is covered up. You must have bare skin to work with.");
				return;
			}

			if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
			{
				actor.OutputHandler.Send(
					$"You must first stop {actor.Effects.First(x => x.IsBlockingEffect("general")).BlockingDescription("general", actor)} before you can begin inscribing a tattoo.");
				return;
			}

			if (actor.Movement != null || target.Movement != null)
			{
				actor.OutputHandler.Send("Neither you nor your target can be moving if you want to inscribe a tattoo.");
				return;
			}

			if (actor.Combat != null || target.Combat != null)
			{
				actor.OutputHandler.Send("You cannot inscribe tattoos while you or your target are in combat.");
				return;
			}

			if (target.EffectsOfType<HavingTattooInked>().Any())
			{
				actor.OutputHandler.Send(
					$"{target.HowSeen(actor, true)} is already having a tattoo inked. Only one tattoo can be worked on at a time.");
				return;
			}

			var inkplan = template.GetInkPlan(actor);
			if (inkplan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
			{
				actor.OutputHandler.Send("You do not have the inks you require to work on that tattoo.");
				return;
			}

			var results = plan.ExecuteWholePlan();
			inkplan.ExecuteWholePlan();
			plan.FinalisePlanNoRestore();
			inkplan.FinalisePlanNoRestore();
			var tattoo = template.ProduceTattoo(actor, target, bodypart);
			target.Body.AddTattoo(tattoo);
			actor.AddEffect(
				new InkingTattoo(actor, target, tattoo,
					results.First(x => x.OriginalReference?.ToString().EqualTo("tattoo tool") == true).PrimaryTarget),
				InkingTattoo.EffectTimespan);
		}

		if (!target.UnableToResistInterventions(actor))
		{
			actor.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ are|is proposing to begin inscribing a tattoo on $1's {bodypart.FullDescription()}.", actor, actor,
				target)));
			target.OutputHandler.Send(Accept.StandardAcceptPhrasing);
			target.AddEffect(new Accept(target, new GenericProposal
			{
				AcceptAction = perc => BeginTattooAction(),
				RejectAction = perc =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting a tattoo from $1.", target, target, actor)));
				},
				ExpireAction = () =>
				{
					target.OutputHandler.Handle(new EmoteOutput(
						new Emote("@ decide|decides against getting a tattoo from $1.", target, target, actor)));
				},
				DescriptionString = "Proposing to inscribe a tattoo",
				Keywords = new List<string> { "tattoo", "inscribe" }
			}), TimeSpan.FromSeconds(120));
		}
		else
		{
			BeginTattooAction();
		}
	}

	private static void TattooReview(ICharacter actor, StringStack command)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("Only administrators can review tattoos at this time.");
			return;
		}

		GenericReview(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooView(ICharacter actor, StringStack command)
	{
		GenericRevisableShow(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooClone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tattoo do you want to clone?");
			return;
		}

		var tattoo =
			(long.TryParse(command.PopSpeech(), out var value)
				? actor.Gameworld.DisfigurementTemplates.Get(value)
				: actor.Gameworld.DisfigurementTemplates.GetByName(command.Last)) as ITattooTemplate;
		if (tattoo == null)
		{
			actor.OutputHandler.Send("There is no such tattoo.");
			return;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new tattoo?");
			return;
		}

		var name = command.PopSpeech();
		if (actor.Gameworld.DisfigurementTemplates.OfType<ITattooTemplate>().Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a tattoo with that name. Tattoo names must be unique.");
			return;
		}

		var newTattoo = (ITattooTemplate)tattoo.Clone(actor.Account, name);
		actor.Gameworld.Add(newTattoo);
		actor.RemoveAllEffects<BuilderEditingEffect<ITattooTemplate>>();
		actor.AddEffect(new BuilderEditingEffect<ITattooTemplate>(actor) { EditingItem = newTattoo });
		actor.OutputHandler.Send(
			$"You clone the {tattoo.Name.ColourName()} into a new tattoo, called {name.ColourName()}, which you are now editing.");
	}

	private static void TattooSet(ICharacter actor, StringStack command)
	{
		GenericRevisableSet(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooEdit(ICharacter actor, StringStack command)
	{
		GenericRevisableEdit(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	private static void TattooList(ICharacter actor, StringStack command)
	{
		GenericRevisableList(actor, command, EditableRevisableItemHelper.TattooHelper);
	}

	#endregion

	#region Bodyparts

	private const string BodypartCommandHelpText = @"You can use the following options with this command:

	#3bodypart list [<filters>]#0 - lists all bodyparts
	#3bodypart edit <body> <part>#0 - edits a bodypart
	#3bodypart edit#0 - equivalent to bodypart show on your edited bodypart
	#3bodypart clone <newname>#0 - clones your currently edited bodypart
	#3bodypart close#0 - closes your editing bodypart
	#3bodypart show <body> <part>#0 - shows a bodypart
	#3bodypart set <...>#0 - changes something about a bodypart. See command for more info.

The list of filters you can use with #3bodypart list#0 are as follows:

	#6part#0 - only show external bodyparts
	#6!part#0 - only show parts that are not external bodyparts
	#6organ#0 - only show organs
	#6!organ#0 - only show parts that are not organs
	#6bone#0 - only show bones
	#6!bone#0 - only show parts that are not bones
	#6<id|name>#0 - only show parts that belong to the specified body prototype";

	[PlayerCommand("Bodypart", "bodypart")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("Bodypart", BodypartCommandHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Bodypart(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());

		switch (ss.PopSpeech())
		{
			case "edit":
				BodypartEdit(actor, ss);
				return;
			case "close":
				BodypartClose(actor, ss);
				return;
			case "set":
				BodypartSet(actor, ss);
				return;
			case "view":
			case "show":
				BodypartShow(actor, ss);
				return;
			case "clone":
				BodypartClone(actor, ss);
				return;
			case "list":
				BodypartList(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(BodypartCommandHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void BodypartList(ICharacter actor, StringStack ss)
	{
		var parts = actor.Gameworld.BodypartPrototypes.AsEnumerable();
		var filters = new List<string>();
		IBodyPrototype bodyProto = null;
		while (!ss.IsFinished)
		{
			var cmd = ss.PopSpeech().ToLowerInvariant();
			switch (cmd)
			{
				case "organs":
				case "organ":
					parts = parts.Where(x => x is IOrganProto);
					filters.Add("...which are organs");
					continue;
				case "bones":
				case "bone":
					parts = parts.Where(x => x is IBone);
					filters.Add("...which are bones");
					continue;
				case "parts":
				case "part":
					parts = parts.Where(x => x is IExternalBodypart);
					filters.Add("...which are external body parts");
					continue;
				case "!organs":
				case "!organ":
					parts = parts.Where(x => x is not IOrganProto);
					filters.Add("...which not are organs");
					continue;
				case "!bones":
				case "!bone":
					parts = parts.Where(x => x is not IBone);
					filters.Add("...which not are bones");
					continue;
				case "!parts":
				case "!part":
					parts = parts.Where(x => x is not IExternalBodypart);
					filters.Add("...which not are external body parts");
					continue;
			}

			if (cmd.Length > 1)
			{
				var cmd1 = cmd.Substring(1);
				switch (cmd[0])
				{
					case '%':
						continue;
				}
			}

			bodyProto = actor.Gameworld.BodyPrototypes.GetByIdOrName(cmd);
			if (bodyProto is null)
			{
				actor.OutputHandler.Send($"The text {cmd.ColourCommand()} is not a valid filter.");
				return;
			}

			var body = bodyProto;
			parts = parts.Where(x => body.CountsAs(x.Body));
			filters.Add($"...which belong to the body proto {body.Name.ColourName()}");
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Listing all bodyparts...");
		foreach (var filter in filters)
		{
			sb.AppendLine(filter);
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			from part in parts
			select new List<string>
			{
				part.Id.ToString("N0", actor),
				part.Name,
				part.FullDescription(),
				part.Body.Name,
				part.BodypartType.DescribeEnum(),
				part.Alignment.Describe(),
				part.Orientation.Describe(),
				part.DamageModifier.ToString("P2", actor),
				part.StunModifier.ToString("P2", actor),
				part.PainModifier.ToString("P2", actor),
				part.BleedModifier.ToString("P2", actor),
				part.RelativeHitChance.ToString("N0", actor),
				part.MaxLife.ToString("N0", actor),
				part.SeveredThreshold.ToString("N0", actor),
				part.UpstreamConnection?.Name ?? ""
			},
			new List<string>
			{
				"Id",
				"Name",
				"Desc",
				"Body",
				"Type",
				"Alignment",
				"Orientation",
				"Dam%",
				"Stun%",
				"Pain%",
				"Bleed%",
				"Hits",
				"HP",
				"Sever",
				"Upstream"
			},
			actor,
			Telnet.Green));
		actor.OutputHandler.Send(sb.ToString());
	}

	private static void BodypartClone(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not currently editing any bodyparts.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				"What name do you want to give to the new bodypart that you clone from the currently edited one?");
			return;
		}

		var name = ss.PopSpeech().ToLowerInvariant();
		if (effect.EditingItem.Body.AllBodypartsBonesAndOrgans.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a bodypart with that name. Bodypart names must be unique.");
			return;
		}

		var newPart = effect.EditingItem.Clone(name);
		effect.EditingItem.Body.UpdateBodypartRole(newPart,
			effect.EditingItem.Body.CoreBodyparts.Contains(effect.EditingItem)
				? BodypartRole.Core
				: BodypartRole.Extra);
		actor.OutputHandler.Send(
			$"You clone the bodypart {effect.EditingItem.FullDescription().Colour(Telnet.Yellow)} as {name.Colour(Telnet.Yellow)}, which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<IBodypart>>();
		actor.AddEffect(new BuilderEditingEffect<IBodypart>(actor) { EditingItem = newPart });
	}

	private static void BodypartShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which body do you want to view a bodypart for?");
			return;
		}

		var body = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.BodyPrototypes.Get(value)
			: actor.Gameworld.BodyPrototypes.GetByName(ss.Last);
		if (body == null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which bodypart from the {body.Name.Colour(Telnet.Cyan)} body do you want to view?");
			return;
		}

		var text = ss.PopSpeech();
		var bodypart = body.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Name.EqualTo(text));
		if (bodypart == null)
		{
			actor.OutputHandler.Send($"The {body.Name.Colour(Telnet.Cyan)} body has no such bodypart.");
			return;
		}

		actor.OutputHandler.Send(bodypart.ShowToBuilder(actor));
	}

	private static void BodypartSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not currently editing any bodyparts.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void BodypartClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not currently editing any bodyparts.");
			return;
		}

		actor.RemoveEffect(effect);
		actor.OutputHandler.Send("You are no longer editing any bodyparts.");
	}

	private static void BodypartEdit(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<IBodypart>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (effect != null)
			{
				actor.OutputHandler.Send(effect.EditingItem.ShowToBuilder(actor));
				return;
			}

			actor.OutputHandler.Send("Which body do you want to edit a bodypart from?");
			return;
		}

		var body = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.BodyPrototypes.Get(value)
			: actor.Gameworld.BodyPrototypes.GetByName(ss.Last);
		if (body == null)
		{
			actor.OutputHandler.Send("There is no such body prototype.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which bodypart from the {body.Name.Colour(Telnet.Cyan)} body do you want to edit?");
			return;
		}

		var text = ss.PopSpeech();
		var bodypart = body.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Name.EqualTo(text));
		if (bodypart == null)
		{
			actor.OutputHandler.Send($"The {body.Name.Colour(Telnet.Cyan)} body has no such bodypart.");
			return;
		}

		actor.RemoveAllEffects<BuilderEditingEffect<IBodypart>>();
		actor.AddEffect(new BuilderEditingEffect<IBodypart>(actor) { EditingItem = bodypart });
		actor.OutputHandler.Send(
			$"You open the {bodypart.FullDescription().Colour(Telnet.Yellow)} bodypart from the {body.Name.Colour(Telnet.Cyan)} body for editing.");
	}

	#endregion

	#region BodypartShapes

	public const string BodypartShapesHelp =
		@"The Bodypart Shapes command is used to view, create and manage bodypart shapes. Bodypart shapes are used on bodyparts (and things that work with them, like worn items) to represent the generic concept of things like 'hands' or 'toe'. So the shape represents the general case of the bodypart.

You can use the following syntax with this command:

	#3bodypartshape list#0 - lists all the bodypart shapes
	#3bodypartshape show <which>#0 - shows a bodypart shape
	#3bodypartshape edit <which>#0 - starts editing a bodypart shape
	#3bodypartshape edit#0 - an alias for show when editing a shape
	#3bodypartshape close#0 - stops editing a bodypart shape
	#3bodypartshape new <name>#0 - creates a new bodypart shape
	#3bodypartshape set name <name>#0 - renames a bodypart shape";

	[PlayerCommand("BodypartShape", "bodypartshapes", "bps", "shape")]
	protected static void BodypartShape(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		GenericBuildingCommand(actor, ss, EditableItemHelper.BodypartShapeHelper);
	}

	#endregion

	#region Terrain

	private const string TerrainHelpText = @"This command is used to create, edit and view terrains.

You can use the following options with this command:

	#3terrain list#0 - view a list of all terrain types
	#3terrain new <name>#0 - creates a new terrain
	#3terrain clone <terrain> <name>#0 - creates a new terrain from an existing one
	#3terrain edit <terrain>#0 - begins editing a terrain
	#3terrain edit#0 - alias for show with no argument
	#3terrain show#0 - shows the terrain you are currently editing
	#3terrain show <terrain>#0 - shows a particular terrain
	#3terrain close#0 - closes the terrain you're editing
	#3terrain override <terrain> [<prog>]#0 - overrides the normal building of the terrain you're in
	#3terrain reset#0 - resets the terrain type to the building overlay for the location you're in
	#3terrain planner#0 - gets the terrain output for the terrain planner tool

You can also edit the following specific properties:

	#3terrain set name <name>#0 - renames this terrain type
	#3terrain set atmosphere none#0 - sets the terrain to have no atmosphere
	#3terrain set atmosphere gas <gas>#0 - sets the atmosphere to a specified gas
	#3terrain set atmosphere liquid <liquid>#0 - sets the atmosphere to specified liquid
	#3terrain set movement <multiplier>#0 - sets the movement speed multiplier
	#3terrain set stamina <cost>#0 - sets the stamina cost for movement
	#3terrain set hide <difficulty>#0 - sets the hide difficulty
	#3terrain set spot <difficulty>#0 - sets the minimum spot difficulty
	#3terrain set forage none#0 - removes the forage profile from this terrain
	#3terrain set forage <profile>#0 - sets the foragable profile
	#3terrain set weather none#0 - removes a weather controller
	#3terrain set weather <controller>#0 - sets the weather controller
	#3terrain set cover <cover>#0 - toggles a ranged cover
	#3terrain set default#0 - sets this terrain as the default for new rooms
	#3terrain set infection <type> <difficulty> <virulence>#0 - sets the infection for this terrain
	#3terrain set outdoors|indoors|exposed|cave|windows#0 - sets the default behaviour type
	#3terrain set model <model>#0 - sets the layer model. See TERRAIN SET MODEL for a list of valid values.
	#3terrain set mapcolour <0-255>#0 - sets the ANSI colour for the MAP command
	#3terrain set editorcolour <#00000000>#0 - sets the hexadecimal colour for the terrain planner
	#3terrain set editortext <1 or 2 letter code>#0 - sets a code to appear on the terrain planner tile";

	[PlayerCommand("Terrain", "terrain")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("terrain", TerrainHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Terrain(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		switch (ss.PopSpeech().ToLowerInvariant())
		{
			case "new":
				TerrainNew(actor, ss);
				return;
			case "clone":
				TerrainClone(actor, ss);
				return;
			case "edit":
				TerrainEdit(actor, ss);
				return;
			case "close":
				TerrainClose(actor, ss);
				return;
			case "set":
				TerrainSet(actor, ss);
				return;
			case "view":
			case "show":
				TerrainView(actor, ss);
				return;
			case "list":
				ShowModule.Show_Terrain(actor, ss);
				return;
			case "planner":
				TerrainPlanner(actor);
				return;
			case "override":
				TerrainOverride(actor, ss);
				return;
			case "reset":
				TerrainReset(actor, ss);
				return;
			default:
				actor.OutputHandler.Send(TerrainHelpText.SubstituteANSIColour());
				return;
		}
	}

	private static void TerrainReset(ICharacter actor, StringStack ss)
	{
		var effect = actor.Location.EffectsOfType<OverrideTerrain>().FirstOrDefault();
		if (effect is null)
		{
			actor.OutputHandler.Send("The terrain at this location is not being overriden currently.");
			return;
		}

		actor.Location.RemoveEffect(effect, true);
		actor.OutputHandler.Send("You reset the terrain at your current location to its default based on overlays.");
		actor.Location.CheckFallExitStatus();
	}

	private static void TerrainOverride(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What terrain do you want to override this location with?");
			return;
		}

		var terrain = actor.Gameworld.Terrains.GetByIdOrName(ss.PopSpeech());
		if (terrain is null)
		{
			actor.OutputHandler.Send("There is no such terrain type.");
			return;
		}

		IFutureProg prog = null;
		if (!ss.IsFinished)
		{
			prog = new FutureProgLookupFromBuilderInput(actor.Gameworld, actor, ss.SafeRemainingArgument,
				FutureProgVariableTypes.Boolean, 
				new List<IEnumerable<FutureProgVariableTypes>>
				{
					new FutureProgVariableTypes[] { },
					new FutureProgVariableTypes[] { FutureProgVariableTypes.Location }
				}
				).LookupProg();
			if (prog is null)
			{
				return;
			}
		}

		actor.Location.RemoveAllEffects<OverrideTerrain>(x => true, true);
		actor.Location.AddEffect(new OverrideTerrain(actor.Location, terrain, prog));
		actor.OutputHandler.Send($"This location has had its terrain override to {terrain.Name.ColourForegroundCustom(terrain.TerrainANSIColour)}{(prog is not null ? $" only when {prog.MXPClickableFunctionName()} is true" : "")}.");
		actor.Location.CheckFallExitStatus();
	}

	private static void TerrainPlanner(ICharacter actor)
	{
		var terrains = JsonSerializer.Serialize(actor.Gameworld.Terrains.Select(x =>
			new
			{
				Id = x.Id, Name = x.Name, TerrainEditorColour = x.TerrainEditorColour,
				TerrainEditorText = x.TerrainEditorText
			}).ToList(), new JsonSerializerOptions
		{
			WriteIndented = true
		});
		actor.OutputHandler.Send($"Terrain file for terrain builder:\n\n{terrains}\n\n", false, true);
	}

	private static void TerrainView(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (effect == null)
			{
				actor.OutputHandler.Send("which terrain type would you like to view?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var terrain = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Terrains.Get(value)
			: actor.Gameworld.Terrains.GetByName(ss.Last);
		if (terrain == null)
		{
			actor.OutputHandler.Send("There is no such terrain.");
			return;
		}

		actor.OutputHandler.Send(terrain.Show(actor));
	}

	private static void TerrainSet(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any terrain types.");
			return;
		}

		effect.EditingItem.BuildingCommand(actor, ss);
	}

	private static void TerrainClose(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (effect == null)
		{
			actor.OutputHandler.Send("You are not editing any terrain types.");
			return;
		}

		actor.RemoveEffect(effect);
		actor.OutputHandler.Send(
			$"You are no longer editing the {effect.EditingItem.Name.Colour(Telnet.Cyan)} terrain type.");
	}

	private static void TerrainEdit(ICharacter actor, StringStack ss)
	{
		var effect = actor.EffectsOfType<BuilderEditingEffect<ITerrain>>().FirstOrDefault();
		if (ss.IsFinished)
		{
			if (effect == null)
			{
				actor.OutputHandler.Send("which terrain type would you like to edit?");
				return;
			}

			actor.OutputHandler.Send(effect.EditingItem.Show(actor));
			return;
		}

		var terrain = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Terrains.Get(value)
			: actor.Gameworld.Terrains.GetByName(ss.Last);
		if (terrain == null)
		{
			actor.OutputHandler.Send("There is no such terrain.");
			return;
		}

		effect = new BuilderEditingEffect<ITerrain>(actor) { EditingItem = terrain };
		actor.AddEffect(effect);
		actor.OutputHandler.Send($"You are now editing the {terrain.Name.Colour(Telnet.Cyan)} terrain.");
	}

	private static void TerrainClone(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which terrain would you like to clone?");
			return;
		}

		var clone = long.TryParse(ss.PopSpeech(), out var value)
			? actor.Gameworld.Terrains.Get(value)
			: actor.Gameworld.Terrains.GetByName(ss.Last);
		if (clone == null)
		{
			actor.OutputHandler.Send("There is no such terrain for you to clone.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new terrain?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (actor.Gameworld.Terrains.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("That name is not unique. The name of the terrain must be unique.");
			return;
		}

		var terrain = new Terrain(clone, name);
		actor.Gameworld.Add(terrain);
		actor.RemoveAllEffects<BuilderEditingEffect<ITerrain>>();
		actor.AddEffect(new BuilderEditingEffect<ITerrain>(actor) { EditingItem = terrain });
		actor.OutputHandler.Send(
			$"You create the new terrain type {name.Colour(Telnet.Cyan)} as a clone of {clone.Name.Colour(Telnet.Cyan)}, which you are now editing.");
	}

	private static void TerrainNew(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to the new terrain?");
			return;
		}

		var name = ss.PopSpeech().TitleCase();
		if (actor.Gameworld.Terrains.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("That name is not unique. The name of the terrain must be unique.");
			return;
		}

		var terrain = new Terrain(actor.Gameworld, name);
		actor.Gameworld.Add(terrain);
		actor.RemoveAllEffects<BuilderEditingEffect<ITerrain>>();
		actor.AddEffect(new BuilderEditingEffect<ITerrain>(actor) { EditingItem = terrain });
		actor.OutputHandler.Send(
			$"You create the new terrain type {name.Colour(Telnet.Cyan)}, which you are now editing.");
	}

	#endregion

	#region Chargen Advices

	public const string ChargenAdviceHelp =
		@"Character Creation Advices (or Chargen Advices) are little pieces of information that can be presented to a player who is creating a character offering them advice or instructions on the choices that they are making.

These advices appear on a particular screen (such as race selection, skill selection, background comment, etc.) and can be set to appear if someone has selected a particular race, culture, ethnicity or role.

The syntax for this command is as follows:

	#3chargenadvice list#0 - lists all of the chargen advices
	#3chargenadvice edit <which>#0 - begins editing a chargen advice
	#3chargenadvice edit new <stage> <title>#0 - creates a new chargen advice
	#3chargenadvice clone <old>#0 - clones an existing chargen advice to a new one
	#3chargenadvice close#0 - stops editing a chargen advice
	#3chargenadvice show <which>#0 - views information about a chargen advice
	#3chargenadvice show#0 - views information about your currently editing chargen advice
	#3chargenadvice set ...#0 - edits properties of a chargen advice";

	[PlayerCommand("ChargenAdvice", "chargenadvice")]
	[CommandPermission(PermissionLevel.JuniorAdmin)]
	[HelpInfo("chargenadvice", ChargenAdviceHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void ChargenAdvice(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		GenericBuildingCommand(actor, ss, EditableItemHelper.ChargenAdviceHelper);
	}

	#endregion

	#region New Player Hints

	public const string NewPlayerHintHelp = @"This command is used to create and edit new player hints.

New player hints are echoes that are shown to players based on some conditions that you set. They can give information about the engine, your world, or promote your game's discord or forums for example.

The syntax for this command is as follows:

	#3nph list#0 - lists all of the new player hints
	#3nph edit <which>#0 - begins editing a new player hint
	#3nph edit new#0 - creates a new new player hint (drops into an editor)
	#3nph close#0 - stops editing a new player hint
	#3nph show <which>#0 - views information about a new player hint
	#3nph show#0 - views information about your currently editing new player hint
	#3nph set repeat#0 - toggles whether this hint can be repeated or only fires once
	#3nph set priority <##>#0 - sets a priority for order shown. Higher priorities are shown first
	#3nph set filter <prog>#0 - sets a prog to filter whether this hint will be shown
	#3nph set text#0 - drops you into an editor to change the hint text";

	[PlayerCommand("NewPlayerHint", "newplayerhint", "nph")]
	protected static void NewPlayerHint(ICharacter actor, string input)
	{
		GenericBuildingCommand(actor, new StringStack(input.RemoveFirstWord()), EditableItemHelper.NewPlayerHintHelper);
	}
	#endregion

	#region Improvers

	public const string ImproverHelpText = @"The Improver command is used to edit improvement models, which are used by skills to determine how and when they improve.

You can use the following syntax with this command:

	#3improver list#0 - lists all of the improvers
	#3improver edit <which>#0 - begins editing an improver
	#3improver edit new <type> <name>#0 - creates a new improver
	#3improver close#0 - stops editing an improver
	#3improver show <which>#0 - views information about an improver
	#3improver show#0 - views information about your currently editing improver
	#3improver set ...#0 - edits the properties of an improver. See type specific help text";
	[PlayerCommand("Improver", "improver")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("Improver", ImproverHelpText, AutoHelp.HelpArgOrNoArg)]
	protected static void Improver(ICharacter actor, string input)
	{
		GenericBuildingCommand(actor, new StringStack(input.RemoveFirstWord()), EditableItemHelper.ImproverHelper);
	}
	#endregion

	#region Height Weight Models

	public const string HeightWeightModelHelp = @"The #3hwmodel#0 command is used to edit height/weight models, which are models that the engine uses to randomly determine heights and weights for characters.

When creating them, you can either use a height and a BMI or a height and a weight. If at all possible I recommend you use BMI (for humanoids at least) because then your weights will correspond with your heights a little more directly. However with animals you're probably better off using weights as it's easier to look up the average weight of a hippopotamus than the average BMI of a hippopotamus.

You can use the following syntax with this command:

	#3hwmodel list#0 - lists all of the height/weight models
	#3hwmodel edit <which>#0 - begins editing a height/weight model
	#3hwmodel edit new <type> <name>#0 - creates a new height/weight model
	#3hwmodel close#0 - stops editing a height/weight model
	#3hwmodel show <which>#0 - views information about a height/weight model
	#3hwmodel show#0 - views information about your currently editing height/weight model
	#3hwmodel set name <name>#0 - renames this model
	#3hwmodel set meanbmi <value>#0 - sets the mean (average) BMI
	#3hwmodel set stddevbmi <value>#0 - sets the standard deviation of BMI
	#3hwmodel set meanheight <value>#0 - sets the mean (average) height
	#3hwmodel set stddevheight <value>#0 - sets the standard deviation of height
	#3hwmodel set meanweight <value>#0 - sets the mean (average) weight
	#3hwmodel set stddevweight <value#0 - sets the standard deviation of weight";

	[PlayerCommand("HWModel", "hwmodel")]
	[CommandPermission(PermissionLevel.Admin)]
	[HelpInfo("HWModel", HeightWeightModelHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void HWModel(ICharacter actor, string input)
	{
		GenericBuildingCommand(actor, new StringStack(input.RemoveFirstWord()), EditableItemHelper.HeightWeightModelHelper);
	}
	#endregion
}