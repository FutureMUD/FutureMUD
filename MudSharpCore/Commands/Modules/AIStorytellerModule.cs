using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Framework;

namespace MudSharp.Commands.Modules;

internal class AIStorytellerModule : BaseBuilderModule
{
	private AIStorytellerModule()
		: base("AIStoryteller")
	{
		IsNecessary = true;
	}

	public static AIStorytellerModule Instance { get; } = new();

	public const string AIStorytellerHelp = @"The #3AIStoryteller#0 command is used to create, edit and inspect AI storytellers.

You can use the following syntax:

	#3ais list#0 - lists all AI storytellers
	#3ais edit <id|name>#0 - opens a storyteller for editing
	#3ais edit new <name>#0 - creates a new storyteller
	#3ais edit#0 - shows the currently edited storyteller
	#3ais close#0 - closes the currently edited storyteller
	#3ais show <id|name>#0 - shows a storyteller
	#3ais set ...#0 - applies one of the storyteller building commands";

	[PlayerCommand("AIStoryteller", "aistoryteller", "ais")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("aistoryteller", AIStorytellerHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void AIStoryteller(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()), EditableItemHelper.AIStorytellerHelper);
	}

	public const string AIStorytellerReferenceHelp =
		@"The #3AIStorytellerReference#0 command is used to create, edit and inspect storyteller reference documents.

You can use the following syntax:

	#3aisr list#0 - lists all reference documents
	#3aisr edit <id|name>#0 - opens a document for editing
	#3aisr edit new <name>#0 - creates a new document
	#3aisr edit#0 - shows the currently edited document
	#3aisr close#0 - closes the currently edited document
	#3aisr show <id|name>#0 - shows a document
	#3aisr set ...#0 - applies one of the document building commands";

	[PlayerCommand("AIStorytellerReference", "aistorytellerreference", "aisr")]
	[CommandPermission(PermissionLevel.SeniorAdmin)]
	[HelpInfo("aistorytellerreference", AIStorytellerReferenceHelp, AutoHelp.HelpArgOrNoArg)]
	protected static void AIStorytellerReference(ICharacter actor, string command)
	{
		GenericBuildingCommand(actor, new StringStack(command.RemoveFirstWord()),
			EditableItemHelper.AIStorytellerReferenceDocumentHelper);
	}
}
