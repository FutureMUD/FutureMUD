using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Work.Projects.Actions;

public abstract class BaseAction : SaveableItem, IProjectAction
{
	public sealed override string FrameworkItemType => "ProjectAction";

	protected BaseAction(Models.ProjectAction action, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = action.Id;
		_name = action.Name;
		Description = action.Description;
		SortOrder = action.SortOrder;
	}

	protected BaseAction(IProjectPhase phase, IFuturemud gameworld, string type)
	{
		Gameworld = gameworld;
		_name = phase.CompletionActions.Select(x => x.Name).NameOrAppendNumberToName("Action");
		Description = "Execute the action";
		SortOrder = 0;
		using (new FMDB())
		{
			var dbitem = new Models.ProjectAction();
			FMDB.Context.ProjectActions.Add(dbitem);
			dbitem.Name = Name;
			dbitem.Description = Description;
			dbitem.SortOrder = SortOrder;
			dbitem.Definition = SaveDefinition().ToString();
			dbitem.Type = type;
			dbitem.ProjectPhaseId = phase.Id;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected BaseAction(BaseAction rhs, IProjectPhase newPhase, string type)
	{
		Gameworld = rhs.Gameworld;
		if (newPhase.CompletionActions.Contains(rhs))
		{
			_name = newPhase.CompletionActions.Select(x => x.Name).NameOrAppendNumberToName("Action");
		}
		else
		{
			_name = rhs.Name;
		}

		Description = rhs.Description;
		SortOrder = rhs.SortOrder;
		using (new FMDB())
		{
			var dbitem = new Models.ProjectAction();
			FMDB.Context.ProjectActions.Add(dbitem);
			dbitem.Name = Name;
			dbitem.Description = Description;
			dbitem.SortOrder = SortOrder;
			dbitem.Definition = rhs.SaveDefinition().ToString();
			dbitem.Type = type;
			dbitem.ProjectPhaseId = newPhase.Id;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.ProjectActions.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.SortOrder = SortOrder;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	protected abstract XElement SaveDefinition();

	public string Description { get; protected set; }

	public int SortOrder { get; protected set; }

	public abstract void CompleteAction(IActiveProject project);

	public void Delete()
	{
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ProjectActions.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ProjectActions.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public abstract IProjectAction Duplicate(IProjectPhase newPhase);

	protected virtual string HelpText => @"You can use the following options with this building command:

	#3name <name>#0 - rename this action
	#3description <description>#0 - sets the description
	#3sort <number>#0 - set the sort order of action execution";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command, phase);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "sort":
			case "order":
			case "sortorder":
			case "sort order":
			case "sort_order":
				return BuildingCommandSortOrder(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandSortOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What number would you like to use for the sort order? Lower is earlier in the execution.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var value) || value < 0)
		{
			actor.OutputHandler.Send(
				"The sort order must be a number 0 or greater. Lower is earlier in the execution.");
			return false;
		}

		SortOrder = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the sort order of project action {Name.ColourValue()} to {SortOrder.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give to this project actions?");
			return false;
		}

		Description = command.SafeRemainingArgument.SubstituteANSIColour().ProperSentences().Fullstop();
		actor.OutputHandler.Send($"You change the description of project action {Name.ColourValue()} to {Description}");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this action?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (phase.CompletionActions.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a project action with that name. Names must be unique per phase.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the project action {_name.ColourValue()} to {name.ColourValue()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public virtual (bool Truth, string Error) CanSubmit()
	{
		return (true, string.Empty);
	}

	public abstract string Show(ICharacter actor);

	public abstract string ShowToPlayer(ICharacter actor);
}