using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Database;
using MudSharp.Framework.Save;

namespace MudSharp.Combat;

public class RangedCover : SaveableItem, IRangedCover
{
        public RangedCover(IFuturemud gameworld, MudSharp.Models.RangedCover cover)
        {
                Gameworld = gameworld;
                _id = cover.Id;
                _name = cover.Name;
                CoverType = (CoverType)cover.CoverType;
                CoverExtent = (CoverExtent)cover.CoverExtent;
                HighestPositionState = PositionState.GetState(cover.HighestPositionState);
                DescriptionString = cover.DescriptionString;
                ActionDescriptionString = cover.ActionDescriptionString;
                MaximumSimultaneousCovers = cover.MaximumSimultaneousCovers;
                CoverStaysWhileMoving = cover.CoverStaysWhileMoving;
        }

        public RangedCover(IFuturemud gameworld, string name)
        {
                Gameworld = gameworld;
                _name = name;
                CoverType = CoverType.Soft;
                CoverExtent = CoverExtent.Marginal;
                HighestPositionState = PositionStanding.Instance;
                DescriptionString = "using $0 as cover";
                ActionDescriptionString = "@ take|takes cover behind $1";
                MaximumSimultaneousCovers = 0;
                CoverStaysWhileMoving = false;

                using (new FMDB())
                {
                        var dbitem = new Models.RangedCover
                        {
                                Name = name,
                                CoverType = (int)CoverType,
                                CoverExtent = (int)CoverExtent,
                                HighestPositionState = (int)HighestPositionState.Id,
                                DescriptionString = DescriptionString,
                                ActionDescriptionString = ActionDescriptionString,
                                MaximumSimultaneousCovers = MaximumSimultaneousCovers,
                                CoverStaysWhileMoving = CoverStaysWhileMoving
                        };
                        FMDB.Context.RangedCovers.Add(dbitem);
                        FMDB.Context.SaveChanges();
                        _id = dbitem.Id;
                }
        }

        public RangedCover(RangedCover rhs, string name)
        {
                Gameworld = rhs.Gameworld;
                _name = name;
                CoverType = rhs.CoverType;
                CoverExtent = rhs.CoverExtent;
                HighestPositionState = rhs.HighestPositionState;
                DescriptionString = rhs.DescriptionString;
                ActionDescriptionString = rhs.ActionDescriptionString;
                MaximumSimultaneousCovers = rhs.MaximumSimultaneousCovers;
                CoverStaysWhileMoving = rhs.CoverStaysWhileMoving;

                using (new FMDB())
                {
                        var dbitem = new Models.RangedCover
                        {
                                Name = name,
                                CoverType = (int)CoverType,
                                CoverExtent = (int)CoverExtent,
                                HighestPositionState = (int)HighestPositionState.Id,
                                DescriptionString = DescriptionString,
                                ActionDescriptionString = ActionDescriptionString,
                                MaximumSimultaneousCovers = MaximumSimultaneousCovers,
                                CoverStaysWhileMoving = CoverStaysWhileMoving
                        };
                        FMDB.Context.RangedCovers.Add(dbitem);
                        FMDB.Context.SaveChanges();
                        _id = dbitem.Id;
                }
        }

	public CoverType CoverType { get; set; }
	public CoverExtent CoverExtent { get; set; }
	public IPositionState HighestPositionState { get; set; }
	public string DescriptionString { get; set; }
	public string ActionDescriptionString { get; set; }
	public int MaximumSimultaneousCovers { get; set; }
        public bool CoverStaysWhileMoving { get; set; }

	public string Describe(ICharacter covered, IPerceivable coverProvider, IPerceiver voyeur)
	{
		return new EmoteOutput(new NoFormatEmote(DescriptionString, covered, coverProvider)).ParseFor(voyeur);
	}

	public IEmote DescribeAction(ICharacter covered, IPerceivable coverProvider)
	{
		return new Emote(ActionDescriptionString, covered, covered, coverProvider);
	}

        public override string FrameworkItemType => "RangedCover";

        public Difficulty MinimumRangedDifficulty
	{
		get
		{
			switch (CoverExtent)
			{
				case CoverExtent.Marginal:
					return Difficulty.Hard;
				case CoverExtent.Partial:
					return Difficulty.VeryHard;
				case CoverExtent.NearTotal:
					return Difficulty.ExtremelyHard;
				case CoverExtent.Total:
					return Difficulty.Impossible;
				default:
					return Difficulty.Normal;
			}
		}
	}

        #region Implementation of IKeyworded

        public IEnumerable<string> Keywords => Name.Split(' ').ToList();

        #endregion

        #region Implementation of IEditableItem

        public const string HelpText = @"You can use the following options with this command:

        #3name <name>#0 - renames this cover type
        #3type <hard|soft>#0 - sets the cover type
        #3extent <marginal|partial|neartotal|total>#0 - sets the extent of cover
        #3position <position>#0 - sets the highest position state for cover
        #3desc <emote>#0 - sets the description emote ($0 is the cover item)
        #3action <emote>#0 - sets the action emote ($0 is the character, $1 is the cover item)
        #3max <number>#0 - sets the maximum simultaneous covers (0 for unlimited)
        #3moving#0 - toggles cover staying while moving

For information on emote syntax see #3help emote#0.";

        public bool BuildingCommand(ICharacter actor, StringStack command)
        {
                switch (command.PopForSwitch())
                {
                        case "name":
                                return BuildingCommandName(actor, command);
                        case "type":
                                return BuildingCommandType(actor, command);
                        case "extent":
                                return BuildingCommandExtent(actor, command);
                        case "position":
                                return BuildingCommandPosition(actor, command);
                        case "desc":
                        case "description":
                                return BuildingCommandDesc(actor, command);
                        case "action":
                                return BuildingCommandAction(actor, command);
                        case "max":
                                return BuildingCommandMax(actor, command);
                        case "moving":
                                return BuildingCommandMoving(actor);
                        default:
                                actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
                                return false;
                }
        }

        private bool BuildingCommandName(ICharacter actor, StringStack command)
        {
                if (command.IsFinished)
                {
                        actor.OutputHandler.Send("What new name do you want to give to this cover type?");
                        return false;
                }

                var name = command.SafeRemainingArgument.TitleCase();
                if (Gameworld.RangedCovers.Except(this).Any(x => x.Name.EqualTo(name)))
                {
                        actor.OutputHandler.Send($"There is already a ranged cover called {name.ColourName()}. Names must be unique.");
                        return false;
                }

                _name = name;
                Changed = true;
                actor.OutputHandler.Send($"You rename the cover to {name.ColourName()}.");
                return true;
        }

        private bool BuildingCommandType(ICharacter actor, StringStack command)
        {
                if (command.IsFinished)
                {
                        actor.OutputHandler.Send($"What type should this cover be? The options are {Enum.GetValues<CoverType>().Select(x => x.Describe().ColourName()).ListToString()}.");
                        return false;
                }

                if (!command.SafeRemainingArgument.TryParseEnum<CoverType>(out var type))
                {
                        actor.OutputHandler.Send($"That is not a valid cover type. The options are {Enum.GetValues<CoverType>().Select(x => x.Describe().ColourName()).ListToString()}.");
                        return false;
                }

                CoverType = type;
                Changed = true;
                actor.OutputHandler.Send($"This cover is now of the {type.Describe().ColourValue()} type.");
                return true;
        }

        private bool BuildingCommandExtent(ICharacter actor, StringStack command)
        {
                if (command.IsFinished)
                {
                        actor.OutputHandler.Send($"What extent should this cover be? The options are {Enum.GetValues<CoverExtent>().Select(x => x.Describe().ColourName()).ListToString()}.");
                        return false;
                }

                if (!command.SafeRemainingArgument.TryParseEnum<CoverExtent>(out var extent))
                {
                        actor.OutputHandler.Send($"That is not a valid cover extent. The options are {Enum.GetValues<CoverExtent>().Select(x => x.Describe().ColourName()).ListToString()}.");
                        return false;
                }

                CoverExtent = extent;
                Changed = true;
                actor.OutputHandler.Send($"This cover now provides {extent.Describe().ColourValue()} cover.");
                return true;
        }

        private bool BuildingCommandPosition(ICharacter actor, StringStack command)
        {
                if (command.IsFinished)
                {
                        actor.OutputHandler.Send("Which position state should be the highest for this cover?");
                        return false;
                }

                var state = PositionState.GetState(command.SafeRemainingArgument);
                if (state == null || state == PositionUndefined.Instance)
                {
                        actor.OutputHandler.Send("That is not a valid position state.");
                        return false;
                }

                HighestPositionState = state;
                Changed = true;
                actor.OutputHandler.Send($"The highest position state for this cover is now {state.Name.ColourValue()}.");
                return true;
        }

       private bool BuildingCommandDesc(ICharacter actor, StringStack command)
       {
               if (command.IsFinished)
               {
                       actor.OutputHandler.Send("What should the description string be?");
                       return false;
               }

               var text = command.SafeRemainingArgument;
               var emote = new NoFormatEmote(text, new DummyPerceiver(), null);
               if (!emote.Valid)
               {
                       actor.OutputHandler.Send(emote.ErrorMessage);
                       return false;
               }

               DescriptionString = text;
               Changed = true;
               actor.OutputHandler.Send("Description string set.");
               return true;
       }

       private bool BuildingCommandAction(ICharacter actor, StringStack command)
       {
               if (command.IsFinished)
               {
                       actor.OutputHandler.Send("What should the action description string be?");
                       return false;
               }

               var text = command.SafeRemainingArgument;
               var emote = new Emote(text, new DummyPerceiver(), null);
               if (!emote.Valid)
               {
                       actor.OutputHandler.Send(emote.ErrorMessage);
                       return false;
               }

               ActionDescriptionString = text;
               Changed = true;
               actor.OutputHandler.Send("Action description string set.");
               return true;
       }

        private bool BuildingCommandMax(ICharacter actor, StringStack command)
        {
                if (command.IsFinished)
                {
                        actor.OutputHandler.Send("How many characters can use this cover simultaneously? (0 for unlimited)");
                        return false;
                }

                if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
                {
                        actor.OutputHandler.Send("You must enter a valid number 0 or greater.");
                        return false;
                }

                MaximumSimultaneousCovers = value;
                Changed = true;
                actor.OutputHandler.Send($"Maximum simultaneous covers is now {value.ToString("N0", actor).ColourValue()}.");
                return true;
        }

        private bool BuildingCommandMoving(ICharacter actor)
        {
                CoverStaysWhileMoving = !CoverStaysWhileMoving;
                Changed = true;
                actor.OutputHandler.Send($"This cover will {(CoverStaysWhileMoving ? "now" : "no longer")} remain when moving.");
                return true;
        }

        public string Show(ICharacter actor)
        {
                var sb = new StringBuilder();
                sb.AppendLine($"Ranged Cover #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
                sb.AppendLine($"Type: {CoverType.Describe().ColourValue()}  Extent: {CoverExtent.Describe().ColourValue()}");
                sb.AppendLine($"Highest Position: {HighestPositionState.Name.ColourValue()}");
                sb.AppendLine($"Max Covers: {MaximumSimultaneousCovers.ToString("N0", actor)}  Moving: {CoverStaysWhileMoving.ToColouredString()}");
                sb.AppendLine($"Description: {DescriptionString.SubstituteANSIColour()}");
                sb.AppendLine($"Action: {ActionDescriptionString.SubstituteANSIColour()}");
                return sb.ToString();
        }

        public override void Save()
        {
                var dbitem = FMDB.Context.RangedCovers.Find(Id);
                dbitem.Name = Name;
                dbitem.CoverType = (int)CoverType;
                dbitem.CoverExtent = (int)CoverExtent;
                dbitem.HighestPositionState = (int)HighestPositionState.Id;
                dbitem.DescriptionString = DescriptionString;
                dbitem.ActionDescriptionString = ActionDescriptionString;
                dbitem.MaximumSimultaneousCovers = MaximumSimultaneousCovers;
                dbitem.CoverStaysWhileMoving = CoverStaysWhileMoving;
                Changed = false;
        }

        #endregion
}