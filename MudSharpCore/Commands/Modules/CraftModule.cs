using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Commands.Helpers;
using MudSharp.Framework.Revision;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Work.Butchering;
using MudSharp.Work.Crafts;
using MudSharp.Work.Projects;
using Humanizer;

namespace MudSharp.Commands.Modules;

internal class CraftModule : Module<ICharacter>
{
	private CraftModule() : base("Craft")
	{
		IsNecessary = true;
	}

	public static CraftModule Instance { get; } = new();

	[PlayerCommand("Crafts", "crafts")]
	protected static void Crafts(ICharacter actor, string input)
	{
		var crafts = (actor.IsAdministrator()
			             ? actor.Gameworld.Crafts
			             : actor.Gameworld.Crafts.Where(x => x.AppearInCraftsList(actor)))
		             .Where(x => x.Status == RevisionStatus.Current)
		             .ToList();
		var ss = new StringStack(input.RemoveFirstWord());
		var parameters = false;
		while (!ss.IsFinished)
		{
			parameters = true;
			var arg = ss.Pop();
			if (arg[0] == '+')
			{
				crafts = crafts
				         .Where(x => x.Name.Contains(arg.Substring(1), StringComparison.InvariantCultureIgnoreCase))
				         .ToList();
				continue;
			}

			if (arg[0] == '-')
			{
				crafts = crafts.Where(x =>
					!x.Name.Contains(arg.Substring(1), StringComparison.InvariantCultureIgnoreCase)).ToList();
				continue;
			}

			crafts = crafts.Where(x => x.Category.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase))
			               .ToList();
		}

		if (!crafts.Any())
		{
			actor.Send($"You do not know any crafts{(parameters ? " with those parameters" : "")}.");
			return;
		}

		actor.Send(StringUtilities.GetTextTable(
			crafts.OrderBy(x => x.Category).ThenBy(x => x.Name).Select(x => new[]
			{
				x.Name,
				x.Blurb,
				x.Category,
				x.CheckTrait is ISkillDefinition sd
					? sd.Improver.CanImprove(actor, actor.GetTrait(x.CheckTrait), x.CheckDifficulty,
						TraitUseType.Practical, true).ToColouredString()
					: "N/A"
			}),
			new[] { "Name", "Blurb", "Category", "Can Skill Up?" },
			actor.LineFormatLength, colour: Telnet.Green, truncatableColumnIndex: 1,
			unicodeTable: actor.Account.UseUnicode));
	}

	private static void CraftBegin(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which craft do you want to begin? See CRAFTS for a list of crafts that you can do.");
			return;
		}

		if (!actor.State.IsAble())
		{
			actor.OutputHandler.Send($"You cannot perform any crafts while you are {actor.State.DescribeEnum(true, Telnet.Red)}.");
			return;
		}

		if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			actor.OutputHandler.Send(
				$"You must first stop {actor.Effects.Where(x => x.IsBlockingEffect("general")).Select(x => x.BlockingDescription("general", actor)).ListToString()}");
			return;
		}

		var crafts = actor.Gameworld.Crafts.Where(x => x.AppearInCraftsList(actor)).ToList();
		var craftName = ss.SafeRemainingArgument.Trim();
		var craft = crafts.FirstOrDefault(x => x.Name.EqualTo(craftName)) ?? crafts.FirstOrDefault(x =>
			x.Name.Contains(craftName, StringComparison.InvariantCultureIgnoreCase));

		if (craft == null)
		{
			actor.Send("You don't know any such craft. See CRAFTS for a list of crafts that you can do.");
			return;
		}

		var (success, error) = craft.CanDoCraft(actor, null, true, false);
		if (!success)
		{
			actor.Send(error);
			return;
		}

		craft.BeginCraft(actor);
	}

	private static void CraftResume(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which in-progress craft did you want to resume? Target the craft-progress item itself.");
			return;
		}

		if (!actor.State.IsAble())
		{
			actor.OutputHandler.Send($"You cannot perform any crafts while you are {actor.State.DescribeEnum(true, Telnet.Red)}.");
			return;
		}

		if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			actor.OutputHandler.Send(
				$"You must first stop {actor.Effects.Where(x => x.IsBlockingEffect("general")).Select(x => x.BlockingDescription("general", actor)).ListToString()}");
			return;
		}

		var target = actor.TargetItem(ss.SafeRemainingArgument);
		if (target == null)
		{
			actor.Send("You don't see anything like that.");
			return;
		}

		if (!(target.GetItemType<ActiveCraftGameItemComponent>() is { } active))
		{
			actor.Send($"{target.HowSeen(actor, true)} is not a craft progress item.");
			return;
		}

		var (success, error) = actor.CanManipulateItem(target);
		if (!success)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		(success, error) = active.Craft.CanResumeCraft(actor, active);
		if (!success)
		{
			actor.Send(error);
			return;
		}

		active.Craft.ResumeCraft(actor, active);
	}

	private static void CraftShow(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which craft do you want to show? See CRAFTS for a list of crafts that you can do.");
			return;
		}


		var crafts = actor.Gameworld.Crafts.Where(x => x.AppearInCraftsList(actor)).ToList();
		var craftName = ss.SafeRemainingArgument.Trim();
		var craft = long.TryParse(craftName, out var value)
			? crafts.FirstOrDefault(x => x.Id == value)
			: crafts.FirstOrDefault(x => x.Name.EqualTo(craftName)) ?? crafts.FirstOrDefault(x =>
				x.Name.Contains(craftName, StringComparison.InvariantCultureIgnoreCase));

		if (craft == null)
		{
			actor.Send("You don't know any such craft. See CRAFTS for a list of crafts that you can do.");
			return;
		}

		actor.OutputHandler.Send(craft.DisplayCraft(actor));
	}

	private static void CraftPreview(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.Send("Which craft do you want to preview? See CRAFTS for a list of crafts that you can do.");
			return;
		}

		var crafts = actor.Gameworld.Crafts.Where(x => x.AppearInCraftsList(actor)).ToList();
		var craftName = ss.SafeRemainingArgument;
		var craft = crafts.FirstOrDefault(x => x.Name.EqualTo(craftName)) ?? crafts.FirstOrDefault(x =>
			x.Name.Contains(craftName, StringComparison.InvariantCultureIgnoreCase));

		if (craft == null)
		{
			actor.Send("You don't know any such craft. See CRAFTS for a list of crafts that you can do.");
			return;
		}

		actor.OutputHandler.Send(craft.GetMaterialPreview(actor));
	}

	private const string CraftHelpPlayer = @"The craft command can be used to start, resume, view or preview a craft.

The syntax to use crafts is a follows:

	#3craft begin <craft name>#0 - begins a new craft as specified.
	#3craft resume <targetitem>#0 - resumes an in-progress craft by targeting the item.
	#3craft view <craft name>#0 - shows you information about a craft.
	#3craft preview <craft name>#0 - shows you what tools and materials you would consume if you executed that craft.
	#3craft categories#0 - shows you the categories of crafts that you know
	#3craft find <item>#0 - shows you what crafts you can do that use or produce the specified item

See the #3crafts#0 command for a list of crafts that you can do.";

	private const string CraftHelpAdmin = @"This command allows you to create, view and edit crafts.

The syntax for building crafts is as follows:

	#3craft list [<category>]|all|mine#0 - shows crafts in game
	#3craft show <craft>#0 - shows a craft
	#3craft show#0 - shows the currently editing craft
	#3craft edit <craft>#0 - opens a craft for editing
	#3craft edit close#0 - closes the open craft
	#3craft edit submit#0 - submits the open craft for review
	#3craft review <id>|all|mine#0 - reviews specified crafts
	#3craft edit new#0 - creates a new craft
	#3craft clone <craft> <newName>#0 - clones a craft
	#3craft categories#0 - lists all the existing categories of crafts
	#3craft set <...>#0 - sets properties of the craft. See CRAFT SET HELP for more detailed help.

The full list of filters for craft list is below:

	#6+<keyword>#0 - include crafts with the keyword in the name or blurb
	#6-<keyword>#0 - exclude crafts with the keyword in the name or blurb
	#6<category>#0 - include crafts with the listed category
	#6*<id>#0 - include crafts that use or produce the specified item proto
	#6&<tag>#0 - include crafts that use or produce items with the specified tag
	#6^<liquid>#0 - include crafts that use or produce items that count as the specified liquid

#3Note: In order to see the player version of this help, turn mortal mode on.#0";

	[PlayerCommand("Craft", "craft")]
	[HelpInfo("craft", CraftHelpPlayer, AutoHelp.HelpArgOrNoArg, CraftHelpAdmin)]
	protected static void Craft(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		switch (ss.Pop().ToLowerInvariant())
		{
			case "begin":
			case "start":
				CraftBegin(actor, ss);
				return;
			case "resume":
			case "restart":
				CraftResume(actor, ss);
				return;
			case "view":
				CraftShow(actor, ss);
				return;
			case "preview":
			case "materials":
				CraftPreview(actor, ss);
				return;
			case "find":
				CraftFind(actor, ss);
				return;
			case "list":
				if (!actor.IsAdministrator())
				{
					goto default;
				}
				BaseBuilderModule.GenericRevisableList(actor, ss, EditableRevisableItemHelper.CraftHelper);
				return;
			case "show":
				if (!actor.IsAdministrator())
				{
					goto default;
				}
				BaseBuilderModule.GenericRevisableShow(actor, ss, EditableRevisableItemHelper.CraftHelper);
				return;
			case "edit":
				if (!actor.IsAdministrator())
				{
					goto default;
				}
				BaseBuilderModule.GenericRevisableEdit(actor, ss, EditableRevisableItemHelper.CraftHelper);
				return;
			case "set":
				if (!actor.IsAdministrator())
				{
					goto default;
				}
				BaseBuilderModule.GenericRevisableSet(actor, ss, EditableRevisableItemHelper.CraftHelper);
				return;
			case "review":
				if (!actor.IsAdministrator())
				{
					goto default;
				}
				BaseBuilderModule.GenericReview(actor, ss, EditableRevisableItemHelper.CraftHelper);
				return;
			case "categories":
				CraftCategories(actor);
				return;
			case "clone":
				if (!actor.IsAdministrator())
				{
					goto default;
				}
				CraftClone(actor, ss);
				return;
			default:
				actor.OutputHandler.Send((actor.IsAdministrator() ? CraftHelpAdmin : CraftHelpPlayer)
					.SubstituteANSIColour());
				return;
		}
	}

	private static void CraftFind(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What item do you want to lookup crafts for?");
			return;
		}

		var item = actor.TargetItem(ss.SafeRemainingArgument);
		if (item is null)
		{
			actor.OutputHandler.Send("You don't see any items like that.");
			return;
		}

		var crafts = (actor.IsAdministrator()
			             ? actor.Gameworld.Crafts
			             : actor.Gameworld.Crafts.Where(x => x.AppearInCraftsList(actor)))
		             .Where(x => x.Status == RevisionStatus.Current)
		             .Where(x => 
			             x.Inputs.Any(y => y.IsInput(item)) ||
						 x.Products.Any(y => y.IsItem(item)) ||
						 x.FailProducts.Any(y => y.IsItem(item)) ||
						 x.Tools.Any(y => y.IsTool(item))
		             )
		             .OrderBy(x => x.Category)
		             .ThenBy(x => x.Name)
		             .ToList();
		if (!crafts.Any())
		{
			actor.OutputHandler.Send($"You don't know any crafts that could use or produce {item.HowSeen(actor)}.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"You know the following crafts that can use or produce {item.HowSeen(actor)}:");
		sb.AppendLine();
		foreach (var craft in crafts)
		{
			sb.AppendLine($"\t{craft.Name.ColourName()} {craft.Category.SquareBrackets().ColourValue()}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private static void CraftClone(ICharacter actor, StringStack ss)
	{
		if (!actor.IsAdministrator())
		{
			actor.OutputHandler.Send("You are not authorised to use the craft command in that way.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which craft do you want to clone?");
			return;
		}

		var craft = actor.Gameworld.Crafts.GetByIdOrName(ss.PopSpeech());
		if (craft == null)
		{
			actor.OutputHandler.Send("There is no such craft.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to your cloned craft?");
			return;
		}

		var newName = ss.SafeRemainingArgument.ToLowerInvariant();
		if (actor.Gameworld.Crafts.Any(x =>
			    x.Status.In(RevisionStatus.Current, RevisionStatus.PendingRevision, RevisionStatus.UnderDesign) &&
			    x.Name.EqualTo(newName)))
		{
			actor.OutputHandler.Send(
				"There is already an approved or in-design craft with that name. Names must be unique.");
			return;
		}

		var newCraft = craft.Clone(actor.Account, newName);
		actor.Gameworld.Add(newCraft);
		actor.OutputHandler.Send(
			$"You clone craft #{craft.Id} ({craft.Name}) to a new craft with id #{newCraft.Id} ({newCraft.Name}), which you are now editing.");
		actor.RemoveAllEffects<BuilderEditingEffect<ICraft>>();
		actor.AddEffect(new BuilderEditingEffect<ICraft>(actor) { EditingItem = newCraft });
	}

	private static void CraftCategories(ICharacter actor)
	{
		if (actor.IsAdministrator())
		{


			actor.OutputHandler.Send(
				$"There are the following categories of crafts: {actor.Gameworld.Crafts.Where(x => x.Status == RevisionStatus.Current).Select(x => x.Category).Distinct().Select(x => x.ColourValue()).ListToString()}.");
			return;
		}

		var knownCrafts = actor.Gameworld.Crafts.Where(x => x.AppearInCraftsList(actor)).ToList();
		var categories = knownCrafts
									  .Where(x => x.Status == RevisionStatus.Current)
									  .Select(x => x.Category.TitleCase())
									  .Distinct()
									  .OrderBy(x => x)
									  .ToList();
		if (categories.Count == 0)
		{
			actor.OutputHandler.Send("You don't know any crafts.");
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"You know crafts in the following categories:");
		sb.AppendLine();
		foreach (var item in categories)
		{
			var kccc = knownCrafts.Count(x => x.Category.EqualTo(item));
			sb.AppendLine($"\t{item.ColourName()} ({kccc.ToString("N0", actor)} {"craft".Pluralise(kccc != 1)})");
		}
		actor.OutputHandler.Send(sb.ToString());
		
	}

	[PlayerCommand("Materials", "materials")]
	[HelpInfo("materials",
		"This command is an alias for the CRAFT PREVIEW option. It shows a preview of what materials would be used if you were to execute the craft at the moment. The syntax is MATERIALS <craft>.",
		AutoHelp.HelpArgOrNoArg)]
	protected static void Materials(ICharacter actor, string input)
	{
		CraftPreview(actor, new StringStack(input.RemoveFirstWord()));
	}

	[PlayerCommand("Projects", "projects")]
	[HelpInfo("projects",
		"This command shows you personal projects that you have in progress and projects in the local area.",
		AutoHelp.HelpArg)]
	protected static void Projects(ICharacter actor, string input)
	{
		var sb = new StringBuilder();
		if (actor.PersonalProjects.Any())
		{
			sb.AppendLine("You have the following personal projects in progress:");
			foreach (var project in actor.PersonalProjects)
			{
				sb.AppendLine($"\t{project.ProjectsCommandOutput(actor)}");
			}
		}
		else
		{
			sb.AppendLine("You do not currently have any personal projects in progress.");
		}

		sb.AppendLine();
		if (actor.Location.LocalProjects.Any())
		{
			sb.AppendLine("There are the following projects in progress locally:");
			foreach (var project in actor.Location.LocalProjects)
			{
				sb.AppendLine($"\t{project.ProjectsCommandOutput(actor)}");
			}
		}
		else
		{
			sb.AppendLine("There are currently no local projects in progress.");
		}

		sb.AppendLine();
		if (actor.CurrentProject.Project == null)
		{
			sb.AppendLine("You are not currently working on any projects.");
		}
		else
		{
			sb.AppendLine(
				$"You are currently working on {actor.CurrentProject.Labour.Name.ColourValue()} from the {actor.CurrentProject.Project.Name.Colour(Telnet.Cyan)} project [#{actor.CurrentProject.Project.Id.ToString("N0", actor)}].");
			var impacts = actor.CurrentProject.Labour.LabourImpacts.Select(x =>
				(x, x.Applies(actor) ? 0.0 : x.MinimumHoursForImpactToKickIn - actor.CurrentProjectHours)).ToList();
			if (impacts.Any())
			{
				sb.AppendLine();
				sb.AppendLine("Your project work is having the following affects on you:");
				sb.AppendLine();
				foreach (var (impact, hours) in impacts.OrderBy(x => x.Item2))
				{
					if (hours > 0.0)
					{
						sb.AppendLine(
							$"\t{impact.DescriptionForProjectsCommand.SubstituteANSIColour()} {$"[in {hours.ToString("N2", actor)}hrs]".ColourError()}");
						continue;
					}

					sb.AppendLine($"\t{impact.DescriptionForProjectsCommand.SubstituteANSIColour()}");
				}
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	private const string ProjectAdminHelp = @"This command allows you to view, start and join projects. Projects are on-going efforts that come in two flavours.

1) Personal projects are specific to you. Only you can see your personal projects or work on them.
2) Local projects are tied to a room and multiple people can often work on them.

You can only work on one project at a time, but you're always working on it, even when you're offline. In this manner, it differs from crafts in that it represents what you're doing ""off screen"". 

Some projects, particularly personal projects, have ""impacts"" that have some effect on you. Many of these impacts require you to be working on the same project for some period of time before they kick in, so changing between projects too often is not advisable.

You can use the following player options with this command:

	#3catalog#0 - shows all the projects you know about
	#3view <name>#0 - shows you detailed information about a catalog project
	#3start <name>#0 - starts a new project
	#3join <project>#0 - joins an active project
	#3quit <project>#0 - quits an active project you are working on
	#3cancel <project>#0 - cancels an active project
	#3supply <project> <req>#0 - supplies material for a particular material requirement
	#3preview <project> <req>#0 - shows you what material you would submit if you did #3project supply#0
	#3details <project>#0 - shows you details about an active project.

You can use the following admin options with this command:

	#3list#0 - lists all current projects
	#3list all#0 - lists all projects including old revisions
	#3show <project>#0 - shows builder info about a project
	#3edit <project>#0 - begins editing a project
	#3edit new <type>#0 - begins editing a new project of the specified type
	#3edit submit#0 - submits a project for review
	#3review all#0 - reviews all submitted projects
	#3review mine#0 - reviews all your own projects
	#3set ...#0 - sets some property about the project. See that command for more help.

Note: See the closely related #3projects#0 command for information about your current projects.";

	private const string ProjectPlayerHelp =
		@"This command allows you to view, start and join projects. Projects are on-going efforts that come in two flavours.

1) Personal projects are specific to you. Only you can see your personal projects or work on them.
2) Local projects are tied to a room and multiple people can often work on them.

You can only work on one project at a time, but you're always working on it, even when you're offline. In this manner, it differs from crafts in that it represents what you're doing ""off screen"". 

Some projects, particularly personal projects, have ""impacts"" that have some effect on you. Many of these impacts require you to be working on the same project for some period of time before they kick in, so changing between projects too often is not advisable.

You can use the following options with this command:

	#3catalog#0 - shows all the projects you know about
	#3view <name>#0 - shows you detailed information about a catalog project
	#3start <name>#0 - starts a new project
	#3join <project>#0 - joins an active project
	#3quit <project>#0 - quits an active project you are working on
	#3cancel <project>#0 - cancels an active project
	#3supply <project> <req>#0 - supplies material for a particular material requirement
	#3preview <project> <req>#0 - shows you what material you would submit if you did #3project supply#0
	#3details <project>#0 - shows you details about an active project.

Note: See the closely related #3projects#0 command for information about your current projects.";

	[PlayerCommand("Project", "project")]
	protected static void Project(ICharacter actor, string input)
	{
		var ss = new StringStack(input.RemoveFirstWord());
		var text = ss.PopSpeech().ToLowerInvariant();
		if (actor.IsAdministrator())
		{
			switch (text)
			{
				case "list":
					BaseBuilderModule.GenericRevisableList(actor, ss, EditableRevisableItemHelper.ProjectHelper);
					return;
				case "show":
					BaseBuilderModule.GenericRevisableShow(actor, ss, EditableRevisableItemHelper.ProjectHelper);
					return;
				case "edit":
					BaseBuilderModule.GenericRevisableEdit(actor, ss, EditableRevisableItemHelper.ProjectHelper);
					return;
				case "set":
					BaseBuilderModule.GenericRevisableSet(actor, ss, EditableRevisableItemHelper.ProjectHelper);
					return;
				case "review":
					BaseBuilderModule.GenericReview(actor, ss, EditableRevisableItemHelper.ProjectHelper);
					return;
				case "help":
				case "?":
					actor.OutputHandler.Send(ProjectAdminHelp.SubstituteANSIColour());
					return;
			}
		}

		switch (text)
		{
			case "start":
			case "begin":
				ProjectStart(actor, ss);
				return;
			case "join":
				ProjectJoin(actor, ss);
				return;
			case "leave":
			case "quit":
				ProjectLeave(actor, ss);
				return;
			case "queue":
				ProjectQueue(actor, ss);
				return;
			case "view":
				ProjectView(actor, ss);
				return;
			case "details":
				ProjectDetails(actor, ss);
				return;
			case "supply":
				ProjectSupply(actor, ss);
				return;
			case "preview":
			case "preview supply":
			case "previewsupply":
			case "preview_supply":
				ProjectPreviewSupply(actor, ss);
				return;
			case "catalogue":
			case "catalog":
				ProjectCatalogue(actor, ss);
				return;
			case "cancel":
				ProjectCancel(actor, ss);
				return;
			default:
				actor.OutputHandler.Send((actor.IsAdministrator() ? ProjectAdminHelp : ProjectPlayerHelp)
					.SubstituteANSIColour());
				return;
		}
	}

	private static void ProjectDetails(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which active project do you want to see the details of?");
			return;
		}

		var projects = actor.PersonalProjects.OfType<IActiveProject>().Concat(actor.Location.LocalProjects).ToList();
		var project = projects.GetByIdOrName(ss.PopSpeech());
		if (project == null)
		{
			actor.OutputHandler.Send("You are not aware of any such project.");
			return;
		}

		actor.OutputHandler.Send(project.ShowToPlayer(actor));
	}

	#region Project Subcommands

	private static void ProjectCatalogue(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				actor.Gameworld.Projects.Where(x => x.AppearInProjectList(actor))
				     .Select(x => x.ProjectCatalogueColumns(actor)),
				new[]
				{
					"Name",
					"Description",
					"Total Labour",
					"Personal"
				},
				actor.LineFormatLength,
				colour: Telnet.Cyan,
				unicodeTable: actor.Account.UseUnicode)
		);
	}

	private static void ProjectCancel(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which project do you want to cancel?");
			return;
		}

		var projects = actor.PersonalProjects.OfType<IActiveProject>().Concat(actor.Location.LocalProjects).ToList();
		var project = projects.GetByIdOrName(ss.PopSpeech());
		if (project == null)
		{
			actor.OutputHandler.Send("You are not aware of any such project.");
			return;
		}

		if (!project.ProjectDefinition.CanCancelProject(actor, project))
		{
			actor.OutputHandler.Send("You are not allowed to cancel that project.");
			return;
		}

		actor.OutputHandler.Send(
			$"Do you want to cancel project #{project.Id.ToString("N0", actor)}?\nYou can type {"accept".ColourCommand()} to proceed or {"decline".ColourCommand()} to change your mind.");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				if (!project.ProjectDefinition.CanCancelProject(actor, project))
				{
					actor.OutputHandler.Send("You are not allowed to cancel that project.");
					return;
				}

				project.Cancel(actor);
			},
			RejectAction = text => { actor.OutputHandler.Send("You decide not to cancel that project."); },
			ExpireAction = () => { actor.OutputHandler.Send("You decide not to cancel that project."); },
			DescriptionString = $"cancelling project {project.Id}",
			Keywords = new List<string> { "project", "cancel" }
		}), TimeSpan.FromSeconds(120));
	}

	private static void ProjectPreviewSupply(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which project do you want to preview supply?");
			return;
		}

		var projects = actor.PersonalProjects.OfType<IActiveProject>().Concat(actor.Location.LocalProjects).ToList();
		var project = projects.GetByIdOrName(ss.PopSpeech());
		if (project == null)
		{
			actor.OutputHandler.Send("You are not aware of any such project.");
			return;
		}

		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which material requirement of that project do you want to preview supply?");
			return;
		}

		var materialText = ss.PopSpeech();
		var materialRequirement =
			project.CurrentPhase.MaterialRequirements.FirstOrDefault(x => x.Name.EqualTo(materialText)) ??
			project.CurrentPhase.MaterialRequirements.FirstOrDefault(x => x.Name.StartsWith(materialText));
		if (materialRequirement == null)
		{
			actor.OutputHandler.Send("That project does not have any such material requirement.");
			return;
		}

		if (project.MaterialProgress[materialRequirement] >= materialRequirement.QuantityRequired)
		{
			actor.OutputHandler.Send(
				$"That project has already been fully supplied of its requirement for {materialRequirement.Name.ColourValue()}.");
			return;
		}

		var plan = materialRequirement.GetPlanForCharacter(actor);
		switch (plan.PlanIsFeasible())
		{
			case GameItems.Inventory.InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.OutputHandler.Send(
					$"You do not have access to any materials that would satisfy the requirement for the supply of {materialRequirement.Name.ColourValue()}.");
				return;
		}

		var target = plan.ScoutAllTargets().First(x => x.OriginalReference?.ToString() == "target").PrimaryTarget;
		materialRequirement.PeekSupplyItem(actor, target, project);
		plan.FinalisePlanNoRestore();
	}

	private static void ProjectSupply(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which project do you want to supply?");
			return;
		}

		var projects = actor.PersonalProjects.OfType<IActiveProject>().Concat(actor.Location.LocalProjects).ToList();
		var project = projects.GetByIdOrName(ss.PopSpeech());
		if (project == null)
		{
			actor.OutputHandler.Send("You are not aware of any such project.");
			return;
		}

		if (ss.IsFinished)
		{
			if (!project.CurrentPhase.MaterialRequirements.Any())
			{
				actor.OutputHandler.Send($"The {project.Name.ColourName()} project doesn't have any material supply requirements.");
				return;
			}
			var sb = new StringBuilder();
			sb.AppendLine($"Material Supply Requirements for {project.Name.ColourName()}:");
			sb.AppendLine();
			sb.AppendLine(StringUtilities.GetTextTable(
				from item in project.CurrentPhase.MaterialRequirements
				select
				new List<string>
				{
					item.Name,
					item.Description,
					project.MaterialProgress[item].ToString("P0", actor),
					item.IsMandatoryForProjectCompletion.ToColouredString()
				},
				new List<string>
				{
					"Name",
					"Description",
					"Mandatory",
					"Progress"
				},
				actor,
				Telnet.Green
			));
			actor.OutputHandler.Send(sb.ToString());
			return;
		}

		var materialText = ss.PopSpeech();
		var materialRequirement =
			project.CurrentPhase.MaterialRequirements.FirstOrDefault(x => x.Name.EqualTo(materialText)) ??
			project.CurrentPhase.MaterialRequirements.FirstOrDefault(x => x.Name.StartsWith(materialText));
		if (materialRequirement == null)
		{
			actor.OutputHandler.Send("That project does not have any such material requirement.");
			return;
		}

		if (project.MaterialProgress[materialRequirement] >= materialRequirement.QuantityRequired)
		{
			actor.OutputHandler.Send(
				$"That project has already been fully supplied of its requirement for {materialRequirement.Name.ColourValue()}.");
			return;
		}

		var plan = materialRequirement.GetPlanForCharacter(actor);
		switch (plan.PlanIsFeasible())
		{
			case GameItems.Inventory.InventoryPlanFeasibility.NotFeasibleMissingItems:
				actor.OutputHandler.Send(
					$"You do not have access to any materials that would satisfy the requirement for the supply of {materialRequirement.Name.ColourValue()}.");
				return;
		}

		var target = plan.ScoutAllTargets().First(x => x.OriginalReference?.ToString() == "target").PrimaryTarget;
		var progress = materialRequirement.SupplyItem(actor, target, project);
		project.FulfilMaterial(materialRequirement, progress);
		plan.FinalisePlanNoRestore();
	}

	private static void ProjectView(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which potential project do you want to view?");
			return;
		}

		var project = actor.Gameworld.Projects.Where(x => x.AppearInProjectList(actor))
		                   .GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (project == null)
		{
			actor.OutputHandler.Send(
				"You are not aware of any project you could potentially begin like that. See PROJECT CATALOGUE to see a list of all your possible options.");
			return;
		}

		actor.OutputHandler.Send(project.ShowToPlayer(actor));
	}

	private static void ProjectQueue(ICharacter actor, StringStack ss)
	{
		actor.OutputHandler.Send("Coming soon.");
	}

	private static void ProjectJoin(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which project do you want to join?");
			return;
		}

		var projects = actor.PersonalProjects.OfType<IActiveProject>().Concat(actor.Location.LocalProjects).ToList();
		var project = projects.GetByIdOrName(ss.PopSpeech());
		if (project == null)
		{
			actor.OutputHandler.Send("You are not aware of any such project.");
			return;
		}

		IProjectLabourRequirement labour = null;
		var potentiallyJoinable = project.CurrentPhase.LabourRequirements
		                                 .Where(x => x.CharacterIsQualified(actor) &&
		                                             project.ActiveLabour.Count(y => y.Labour == x) <
		                                             x.MaximumSimultaneousWorkers)
		                                 .ToList();
		if (ss.IsFinished)
		{
			if (potentiallyJoinable.Count == 0)
			{
				actor.OutputHandler.Send(
					"You are not qualified to be able to join any of the labour requirements for that project.");
				return;
			}

			if (potentiallyJoinable.Count > 1)
			{
				actor.OutputHandler.Send(
					$"You are qualified and able to join more than one of the labour requirements for that project, so you must specify which one you want to join. Your options are {potentiallyJoinable.Select(x => x.Name.ColourValue()).ListToString()}.");
				return;
			}

			labour = potentiallyJoinable.Single();
		}
		else
		{
			var labourText = ss.PopSpeech();
			labour = project.CurrentPhase.LabourRequirements.FirstOrDefault(x => x.Name.EqualTo(labourText)) ??
			         project.CurrentPhase.LabourRequirements.FirstOrDefault(x =>
				         x.Name.StartsWith(labourText, StringComparison.InvariantCultureIgnoreCase));
			if (labour == null)
			{
				actor.OutputHandler.Send(
					"The current phase of that project does not have any such labour requirement.");
				return;
			}

			if (!potentiallyJoinable.Contains(labour))
			{
				if (labour.CharacterIsQualified(actor))
				{
					actor.OutputHandler.Send(
						$"The project cannot benefit from any more people working on {labour.Name.ColourValue()}.");
					return;
				}

				actor.OutputHandler.Send(
					$"You are not qualified to do the {labour.Name.ColourValue()} labour requirement for that project.");
				return;
			}
		}

		project.Join(actor, labour);
	}

	private static void ProjectLeave(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which project do you want to leave?");
			return;
		}

		var projects = actor.PersonalProjects.OfType<IActiveProject>().Concat(actor.Location.LocalProjects).ToList();
		var project = projects.GetByIdOrName(ss.PopSpeech());
		if (project == null)
		{
			actor.OutputHandler.Send("You are not aware of any such project.");
			return;
		}

		if (project.ActiveLabour.All(x => x.Character != actor))
		{
			actor.OutputHandler.Send("You are not currently contributing any labour to that project.");
			return;
		}

		project.Leave(actor);
	}

	private static void ProjectStart(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which project from the catalogue do you want to start?");
			return;
		}

		var project = actor.Gameworld.Projects.Where(x => x.AppearInProjectList(actor))
		                   .GetFromItemListByKeyword(ss.PopSpeech(), actor);
		if (project == null)
		{
			actor.OutputHandler.Send(
				"You are not aware of any project you could potentially begin like that. See PROJECT CATALOGUE to see a list of all your possible options.");
			return;
		}

		if (!project.CanInitiateProject(actor))
		{
			actor.OutputHandler.Send(project.WhyCannotInitiateProject(actor));
			return;
		}

		project.InitiateProject(actor);
	}

	#endregion

	#region Butchering

	[PlayerCommand("Butcher", "butcher")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Butcher(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("?", "help"))
		{
			actor.Send(
				$"This command is used to break down organic corpses into their component parts.\nThe syntax is {"butcher <corpse|part> [<subcategory>]".ColourCommand()}.");
			return;
		}

		var target = actor.TargetLocalItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that to butcher.");
			return;
		}

		var targetAsButcherable = target.GetItemType<IButcherable>();
		if (targetAsButcherable?.OriginalCharacter?.Race.ButcheryProfile == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be butchered.");
			return;
		}

		if (targetAsButcherable.Decay >= DecayState.Decaying)
		{
			actor.Send($"{target.HowSeen(actor, true)} is too heavily decayed to be butchered.");
			return;
		}

		var profile = targetAsButcherable.OriginalCharacter.Race.ButcheryProfile;
		if (profile.Verb != ButcheryVerb.Butcher)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be butchered, but rather salvaged.");
			return;
		}

		if (!profile.CanButcher(actor, target))
		{
			actor.Send(profile.WhyCannotButcher(actor, target));
			return;
		}

		var subcategory = ss.Pop();
		if (!string.IsNullOrEmpty(subcategory))
		{
			if (!profile.Products.Where(x => x.Subcategory.EqualTo(subcategory)).Any(x => x.CanProduce(actor, target)))
			{
				actor.Send(
					$"{target.HowSeen(actor, true)} does not contain any such subcategory of things for you to butcher.");
				return;
			}

			if (targetAsButcherable.ButcheredSubcategories.Any(x => x.EqualTo(subcategory)))
			{
				actor.Send(
					$"Someone has already butchered {target.HowSeen(actor, true)} for its {subcategory.Colour(Telnet.Green)} parts.");
				return;
			}
		}

		var plan = profile.ToolTemplate.CreatePlan(actor);
		var results = plan.ExecuteWholePlan();
		actor.AddEffect(
			new Butchering(actor, targetAsButcherable, subcategory,
				results.First(x => x.ActionState == DesiredItemState.Held).PrimaryTarget), TimeSpan.FromSeconds(10));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to butcher $0.", actor, target)));
		plan.FinalisePlanNoRestore();
	}

	[PlayerCommand("Salvage", "salvage")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Salvage(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("?", "help"))
		{
			actor.Send(
				$"This command is used to take apart non-organic corpses and wrecks into their component parts.\nThe syntax is {"salvage <wreck|part> [<subcategory>]".ColourCommand()}.");
			return;
		}

		var target = actor.TargetLocalItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that to salvage.");
			return;
		}

		var targetAsButcherable = target.GetItemType<IButcherable>();
		if (targetAsButcherable?.OriginalCharacter?.Race.ButcheryProfile == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be salvaged.");
			return;
		}

		if (targetAsButcherable.Decay >= DecayState.Decaying)
		{
			actor.Send($"{target.HowSeen(actor, true)} is too heavily damaged by the passage of time to be salvaged.");
			return;
		}

		var profile = targetAsButcherable.OriginalCharacter.Race.ButcheryProfile;
		if (profile.Verb != ButcheryVerb.Salvage)
		{
			actor.Send(
				$"{target.HowSeen(actor, true)} is not something that can be salvaged, but rather must be butchered.");
			return;
		}

		if (!profile.CanButcher(actor, target))
		{
			actor.Send(profile.WhyCannotButcher(actor, target));
			return;
		}

		var subcategory = ss.Pop();
		if (!string.IsNullOrEmpty(subcategory))
		{
			if (!profile.Products.Where(x => x.Subcategory.EqualTo(subcategory)).Any(x => x.CanProduce(actor, target)))
			{
				actor.Send(
					$"{target.HowSeen(actor, true)} does not contain any such subcategory of things for you to salvage.");
				return;
			}

			if (targetAsButcherable.ButcheredSubcategories.Any(x => x.EqualTo(subcategory)))
			{
				actor.Send(
					$"Someone has already salvaged {target.HowSeen(actor, true)} for its {subcategory.Colour(Telnet.Green)} parts.");
				return;
			}
		}

		var plan = profile.ToolTemplate.CreatePlan(actor);
		var results = plan.ExecuteWholePlan();
		actor.AddEffect(
			new Butchering(actor, targetAsButcherable, subcategory,
				results.First(x => x.ActionState == DesiredItemState.Held).PrimaryTarget), TimeSpan.FromSeconds(10));
		actor.OutputHandler.Handle(new EmoteOutput(new Emote("@ begin|begins to salvage $0.", actor, target)));
		plan.FinalisePlanNoRestore();
	}

	[PlayerCommand("Skin", "skin")]
	[RequiredCharacterState(CharacterState.Able)]
	[NoCombatCommand]
	[NoHideCommand]
	[NoMovementCommand]
	[DelayBlock("general", "You must first stop {0} before you can do that.")]
	protected static void Skin(ICharacter actor, string command)
	{
		var ss = new StringStack(command.RemoveFirstWord());
		if (ss.IsFinished || ss.Peek().EqualToAny("?", "help"))
		{
			actor.Send(
				$"This command is used to remove the pelt, fur or skin from a corpse.\nThe syntax is {"skin <corpse>".ColourCommand()}.");
			return;
		}

		var target = actor.TargetLocalItem(ss.Pop());
		if (target == null)
		{
			actor.Send("You don't see anything like that to skin.");
			return;
		}

		var targetAsButcherable = target.GetItemType<IButcherable>();
		if (targetAsButcherable?.OriginalCharacter?.Race.ButcheryProfile == null)
		{
			actor.Send($"{target.HowSeen(actor, true)} is not something that can be skinned.");
			return;
		}

		if (!target.IsItemType<ICorpse>())
		{
			actor.Send($"Skinning can only be done on a whole corpse, not bodyparts.");
			return;
		}

		if (targetAsButcherable.Decay >= DecayState.Decaying)
		{
			actor.Send($"{target.HowSeen(actor, true)} is too heavily decayed to be skinned.");
			return;
		}

		var profile = targetAsButcherable.OriginalCharacter.Race.ButcheryProfile;
		if (!profile.Products.Where(x => x.IsPelt).Any(x => x.CanProduce(actor, target)))
		{
			actor.Send($"{target.HowSeen(actor, true)} does not have a pelt that you can remove intact.");
			return;
		}

		if (!profile.CanButcher(actor, target))
		{
			actor.Send(profile.WhyCannotButcher(actor, target));
			return;
		}

		if (target.GetItemType<ICorpse>().Skinned)
		{
			actor.Send($"{target.HowSeen(actor, true)} has already been skinned.");
			return;
		}

		var plan = profile.ToolTemplate.CreatePlan(actor);
		var results = plan.ExecuteWholePlan();
		var tool = results.First(x => x.ActionState == DesiredItemState.Held).PrimaryTarget;
		actor.AddEffect(new Skinning(actor, targetAsButcherable, tool), TimeSpan.FromSeconds(10));
		actor.OutputHandler.Handle(
			new EmoteOutput(new Emote("@ begin|begins to skin $0 with $1.", actor, target, tool)));
		plan.FinalisePlanNoRestore();
	}

	#endregion
}