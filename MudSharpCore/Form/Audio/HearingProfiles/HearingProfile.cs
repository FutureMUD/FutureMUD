using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Text;

namespace MudSharp.Form.Audio.HearingProfiles;

public abstract class HearingProfile : SaveableItem, IHearingProfile
{
	protected HearingProfile(MudSharp.Models.HearingProfile profile, IFuturemud game)
	{
		Gameworld = game;
		_id = profile.Id;
		_name = profile.Name;
		SurveyDescription = profile.SurveyDescription;
	}

	protected HearingProfile(IFuturemud game, string name, string type)
	{
		Gameworld = game;
		_name = name;
		SurveyDescription = string.Empty;
		using (new FMDB())
		{
			Models.HearingProfile dbitem = new()
			{
				Name = name,
				Type = type,
				SurveyDescription = SurveyDescription,
				Definition = "<Definition />"
			};
			FMDB.Context.HearingProfiles.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public abstract void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game);

	public static HearingProfile LoadProfile(MudSharp.Models.HearingProfile profile, IFuturemud game)
	{
		switch (profile.Type)
		{
			case "Simple":
				return new SimpleHearingProfile(profile, game);
			case "Temporal":
				return new TemporalHearingProfile(profile, game);
			case "TimeOfDay":
				return new TimeOfDayHearingProfile(profile, game);
			case "Weekday":
				return new WeekdayHearingProfile(profile, game);
			case "WeekdayTimeOfDay":
				return new WeekdayTimeOfDayHearingProfile(profile, game);
			default:
				throw new NotSupportedException("Invalid HearingProfile type in HearingProfile.LoadProfile");
		}
	}

	#region IHearingProfile Members

	public abstract Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity);

	public abstract HearingProfile Clone(string name);

	public string SurveyDescription { get; set; }

	public virtual IHearingProfile CurrentProfile(ILocation location)
	{
		return this;
	}

	public virtual bool DependsOn(IHearingProfile profile)
	{
		return ReferenceEquals(this, profile);
	}

	public abstract string Type { get; }

	protected abstract string SaveDefinition();

	public override void Save()
	{
		Models.HearingProfile dbitem = FMDB.Context.HearingProfiles.Find(Id);
		dbitem.Name = Name;
		dbitem.SurveyDescription = SurveyDescription;
		dbitem.Definition = SaveDefinition();
		dbitem.Type = Type;
		Changed = false;
	}

	protected void CopyBaseSettingsFrom(HearingProfile rhs)
	{
		SurveyDescription = rhs.SurveyDescription;
	}

	public virtual string HelpText => @"You can use the following options with this item:

	#3name <name>#0 - renames this hearing profile
	#3survey <text>#0 - sets the survey description";

	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "survey":
			case "description":
			case "desc":
				return BuildingCommandSurvey(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this hearing profile?");
			return false;
		}

		string name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.HearingProfiles.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a hearing profile called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the hearing profile {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandSurvey(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What survey description do you want to set?");
			return false;
		}

		SurveyDescription = command.SafeRemainingArgument;
		actor.OutputHandler.Send("Survey description set.");
		Changed = true;
		return true;
	}

	public virtual string Show(ICharacter actor)
	{
		StringBuilder sb = new();
		sb.AppendLine(
			$"Hearing Profile #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan,
				Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {Type.ColourValue()}");
		sb.AppendLine(
			$"Survey Description: {(string.IsNullOrEmpty(SurveyDescription) ? "None".ColourError() : SurveyDescription.ColourCommand())}");
		return sb.ToString();
	}

	#endregion
}
