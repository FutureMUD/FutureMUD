using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Work.Projects.Impacts;

public abstract class BaseImpact : SaveableItem, ILabourImpact
{
	protected BaseImpact(Models.ProjectLabourImpact impact, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = impact.Id;
		_name = impact.Name;
		DescriptionForProjectsCommand = impact.Description;
		MinimumHoursForImpactToKickIn = impact.MinimumHoursForImpactToKickIn;
		_typeName = impact.Type;
	}

	protected BaseImpact(BaseImpact rhs, IProjectLabourRequirement newLabour, string type)
	{
		Gameworld = rhs.Gameworld;
		if (newLabour.LabourImpacts.Contains(rhs))
		{
			_name = newLabour.LabourImpacts.Select(x => x.Name).NameOrAppendNumberToName(rhs.Name);
		}
		else
		{
			_name = rhs.Name;
		}

		DescriptionForProjectsCommand = rhs.DescriptionForProjectsCommand;
		MinimumHoursForImpactToKickIn = rhs.MinimumHoursForImpactToKickIn;
		_typeName = type;
		using (new FMDB())
		{
			var dbitem = new Models.ProjectLabourImpact();
			FMDB.Context.ProjectLabourImpacts.Add(dbitem);
			dbitem.Name = Name;
			dbitem.Description = DescriptionForProjectsCommand;
			dbitem.Type = type;
			dbitem.ProjectLabourRequirementId = newLabour.Id;
			dbitem.Definition = rhs.SaveDefinition().ToString();
			dbitem.MinimumHoursForImpactToKickIn = MinimumHoursForImpactToKickIn;
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected BaseImpact(IProjectLabourRequirement requirement, string type, string name)
	{
		Gameworld = requirement.Gameworld;
		_name = requirement.LabourImpacts.Select(x => x.Name).NameOrAppendNumberToName(name);
		_typeName = type;
		DescriptionForProjectsCommand = "#3Impacting on your character in an undescribed way#0";
		using (new FMDB())
		{
			var dbitem = new Models.ProjectLabourImpact();
			FMDB.Context.ProjectLabourImpacts.Add(dbitem);
			dbitem.Name = Name;
			dbitem.Description = DescriptionForProjectsCommand;
			dbitem.Type = type;
			dbitem.ProjectLabourRequirementId = requirement.Id;
			dbitem.Definition = SaveDefinition().ToString();
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	protected abstract XElement SaveDefinition();
	private string _typeName;
	public string DescriptionForProjectsCommand { get; protected set; }

	public double MinimumHoursForImpactToKickIn { get; protected set; }

	public void Delete()
	{
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.ProjectLabourImpacts.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.ProjectLabourImpacts.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public abstract ILabourImpact Duplicate(IProjectLabourRequirement requirement);

	protected virtual string HelpText => @"You can use the following options with this building command:

	#3show#0 - view detailed information about this impact
	#3name <name>#0 - rename this impact
	#3description <description>#0 - sets the description
	#3hours <hours>#0 - hours before this impact kicks in";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command, IProjectLabourRequirement requirement)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command, requirement);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "hours":
			case "minhours":
			case "min hours":
			case "min_hours":
			case "minimumhours":
			case "minimum hours":
			case "minimum_hours":
				return BuildingCommandMinimumHours(actor, command);
			case "show":
				actor.OutputHandler.Send(Show(actor));
				return true;
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandMinimumHours(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"How many hours must someone work on this labour before this impact kicks in? Use 0.0 for it to always apply.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must specify a valid numer of minimum hours.");
			return false;
		}

		MinimumHoursForImpactToKickIn = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This labour impact will now kick in after {MinimumHoursForImpactToKickIn.ToString("N2", actor).ColourValue()} hours.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give to this project impact?");
			return false;
		}

		DescriptionForProjectsCommand = command.SafeRemainingArgument.ProperSentences();
		actor.OutputHandler.Send(
			$"You change the description of project impact {Name.ColourValue()} to {DescriptionForProjectsCommand.SubstituteANSIColour()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command, IProjectLabourRequirement requirement)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this impact?");
			return false;
		}

		var name = command.PopSpeech().TitleCase();
		if (requirement.LabourImpacts.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				"There is already a project impact with that name for that labour requirement. Names must be unique per labour requirement.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the project impact {_name.ColourValue()} to {name.ColourValue()}.");
		_name = name;
		Changed = true;
		return true;
	}

	public bool Applies(ICharacter actor)
	{
		return actor.CurrentProjectHours >= MinimumHoursForImpactToKickIn;
	}

	public virtual (bool Truth, string Error) CanSubmit()
	{
		return (true, string.Empty);
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Impact #{Id.ToString("N0", actor)} - {Name.ColourName()}");
		sb.AppendLine($"Type: {_typeName.ColourValue()}");
		sb.AppendLine($"Description: {DescriptionForProjectsCommand.SubstituteANSIColour()}");
		sb.AppendLine($"Minimum Hours: {MinimumHoursForImpactToKickIn.ToString("N2", actor).ColourValue()}");
		return sb.ToString();
	}

	public abstract string ShowFull(ICharacter actor);

	public abstract string ShowToPlayer(ICharacter actor);

	public sealed override void Save()
	{
		var dbitem = FMDB.Context.ProjectLabourImpacts.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = DescriptionForProjectsCommand;
		dbitem.Definition = SaveDefinition().ToString();
		dbitem.MinimumHoursForImpactToKickIn = MinimumHoursForImpactToKickIn;
		Changed = false;
	}

	public sealed override string FrameworkItemType => "ProjectLabourImpact";
}