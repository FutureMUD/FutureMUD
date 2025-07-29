using System;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System.Text;
using System.Linq;
using MudSharp.RPG.Checks;
using MudSharp.Database;
using MudSharp.Character;
using MudSharp.Character;

namespace MudSharp.Form.Audio.HearingProfiles;

public abstract class HearingProfile : SaveableItem, IHearingProfile
{
        protected HearingProfile(MudSharp.Models.HearingProfile profile)
        {
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
                        var dbitem = new MudSharp.Models.HearingProfile
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

	public static HearingProfile LoadProfile(MudSharp.Models.HearingProfile profile)
	{
		switch (profile.Type)
		{
			case "Simple":
				return new SimpleHearingProfile(profile);
			case "Temporal":
				return new TemporalHearingProfile(profile);
			case "Weekday":
				return new WeekdayHearingProfile(profile);
			default:
				throw new NotSupportedException("Invalid HearingProfile type in HearingProfile.LoadProfile");
		}
	}

	#region IHearingProfile Members

	public abstract Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity);

	public string SurveyDescription { get; set; }

        public virtual IHearingProfile CurrentProfile(ILocation location)
        {
                return this;
        }

        public abstract string Type { get; }

        protected abstract string SaveDefinition();

        public override void Save()
        {
                var dbitem = FMDB.Context.HearingProfiles.Find(Id);
                dbitem.Name = Name;
                dbitem.SurveyDescription = SurveyDescription;
                dbitem.Definition = SaveDefinition();
                dbitem.Type = Type;
                Changed = false;
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

                var name = command.SafeRemainingArgument.TitleCase();
                if (Gameworld.HearingProfiles.Any(x => x.Name.EqualTo(name)))
                {
                        actor.OutputHandler.Send($"There is already a hearing profile called {name.ColourName()}. Names must be unique.");
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
                var sb = new StringBuilder();
                sb.AppendLine($"Hearing Profile #{Id.ToString("N0", actor)} - {Name.ColourName()}");
                sb.AppendLine($"Type: {Type.ColourValue()}");
                sb.AppendLine($"Survey Description: {SurveyDescription.ColourCommand()}");
                return sb.ToString();
        }

	#endregion
}