using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using EditableItem = MudSharp.Framework.Revision.EditableItem;

namespace MudSharp.Work.Foraging;

public class ForagableProfile : EditableItem, IForagableProfile
{
	public override bool CanSubmit()
	{
		return MaximumYieldPoints.Any();
	}

	public override string WhyCannotSubmit()
	{
		return "You must set up yield points for at least one foragable type.";
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Foragable Profile #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name.ColourName()} - {Status.DescribeColour()}");
		sb.AppendLine();
		sb.AppendLine("Yield types:");
		foreach (var type in MaximumYieldPoints.Keys)
		{
			sb.AppendLine(
				$"\t{type.ColourName()} - maximum {MaximumYieldPoints[type].ToString("N2", actor).ColourValue()}, recover {HourlyYieldPoints[type].ToString("N2", actor).ColourValue()} per hour");
		}

		sb.AppendLine();
		sb.Append(StringUtilities.GetTextTable(
			from item in Foragables
			select new[]
			{
				item.Id.ToString("N0", actor),
				item.Name,
				$"{item.ItemProto.Name} {item.ItemProto.Id:N0}r{item.ItemProto.RevisionNumber:N0}",
				item.QuantityDiceExpression,
				item.MinimumOutcome.Describe(),
				item.MaximumOutcome.Describe(),
				item.RelativeChance.ToString("N0", actor),
				item.ForagableTypes.Any(x => _maximumYieldPoints.ContainsKey(x)) ? "Y" : "N"
			},
			new[]
			{
				"ID",
				"Name",
				"Item Proto",
				"Quantity",
				"Min Outcome",
				"Max Outcome",
				"Chances",
				"Has Yield?"
			},
			actor.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));

		return sb.ToString();
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var dbnew = new Models.ForagableProfile
			{
				Id = Id,
				RevisionNumber = FMDB.Context.ForagableProfiles.Where(x => x.Id == Id).Select(x => x.RevisionNumber)
									 .AsEnumerable().DefaultIfEmpty(0).Max() +
								 1,
				Name = Name
			};

			foreach (var item in Foragables)
			{
				dbnew.ForagableProfilesForagables.Add(new ForagableProfilesForagables
				{
					ForagableProfile = dbnew,
					ForagableId = item.Id
				});
			}

			foreach (var item in MaximumYieldPoints)
			{
				dbnew.ForagableProfilesMaximumYields.Add(new ForagableProfilesMaximumYields
				{
					ForagableProfile = dbnew,
					ForageType = item.Key,
					Yield = item.Value
				});
			}

			foreach (var item in HourlyYieldPoints)
			{
				dbnew.ForagableProfilesHourlyYieldGains.Add(new ForagableProfilesHourlyYieldGains
				{
					ForagableProfile = dbnew,
					ForageType = item.Key,
					Yield = item.Value
				});
			}

			dbnew.EditableItem = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbnew.EditableItem);
			dbnew.EditableItem.BuilderDate = DateTime.UtcNow;
			dbnew.EditableItem.RevisionNumber = dbnew.RevisionNumber;
			dbnew.EditableItem.BuilderAccountId = initiator.Account.Id;
			dbnew.EditableItem.RevisionStatus = (int)RevisionStatus.UnderDesign;

			FMDB.Context.ForagableProfiles.Add(dbnew);
			FMDB.Context.SaveChanges();

			return new ForagableProfile(dbnew, Gameworld);
		}
	}

	public override string EditHeader()
	{
		return $"Foragable Profile {Name} ({Id:N0}r{RevisionNumber:N0})";
	}

	public override string FrameworkItemType => "ForagableProfile";

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.ForagableProfiles.Find(Id, RevisionNumber);
			if (_statusChanged)
			{
				base.Save(dbitem.EditableItem);
			}

			dbitem.Name = Name;
			FMDB.Context.ForagableProfilesForagables.RemoveRange(dbitem.ForagableProfilesForagables);
			foreach (var item in Foragables)
			{
				dbitem.ForagableProfilesForagables.Add(new ForagableProfilesForagables
				{
					ForagableProfile = dbitem,
					ForagableId = item.Id
				});
			}

			FMDB.Context.ForagableProfilesMaximumYields.RemoveRange(dbitem.ForagableProfilesMaximumYields);
			foreach (var item in MaximumYieldPoints)
			{
				dbitem.ForagableProfilesMaximumYields.Add(new ForagableProfilesMaximumYields
				{
					ForagableProfile = dbitem,
					ForageType = item.Key,
					Yield = item.Value
				});
			}

			FMDB.Context.ForagableProfilesHourlyYieldGains.RemoveRange(dbitem.ForagableProfilesHourlyYieldGains);
			foreach (var item in HourlyYieldPoints)
			{
				dbitem.ForagableProfilesHourlyYieldGains.Add(new ForagableProfilesHourlyYieldGains
				{
					ForagableProfile = dbitem,
					ForageType = item.Key,
					Yield = item.Value
				});
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#region Constructors

	public ForagableProfile(MudSharp.Models.ForagableProfile profile, IFuturemud gameworld) : base(profile.EditableItem)
	{
		Gameworld = gameworld;
		LoadFromDb(profile);
	}

	private void LoadFromDb(MudSharp.Models.ForagableProfile profile)
	{
		_id = profile.Id;
		_name = profile.Name;
		foreach (var item in profile.ForagableProfilesForagables)
		{
			_foragableIds.Add(item.ForagableId);
		}

		_maximumYieldPoints = profile.ForagableProfilesMaximumYields.ToDictionary(item => item.ForageType.ToLowerInvariant(),
			item => item.Yield, StringComparer.InvariantCultureIgnoreCase);
		_hourlyYieldPoints = profile.ForagableProfilesHourlyYieldGains.ToDictionary(item => item.ForageType.ToLowerInvariant(),
			item => item.Yield, StringComparer.InvariantCultureIgnoreCase);
	}

	public ForagableProfile(IAccount originator) : base(originator)
	{
		Gameworld = originator.Gameworld;
		using (new FMDB())
		{
			var dbitem = new Models.ForagableProfile
			{
				Id = Gameworld.ForagableProfiles.NextID()
			};
			FMDB.Context.ForagableProfiles.Add(dbitem);
			var dbedit = new Models.EditableItem();
			FMDB.Context.EditableItems.Add(dbedit);
			dbitem.EditableItem = dbedit;
			dbedit.BuilderAccountId = BuilderAccountID;
			dbedit.BuilderDate = BuilderDate;
			dbedit.RevisionStatus = (int)Status;
			dbedit.RevisionNumber = 0;

			_name = "Unnamed Foragable Profile";
			_maximumYieldPoints = new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase);
			_hourlyYieldPoints = new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase);

			dbitem.Name = _name;
			FMDB.Context.SaveChanges();
			LoadFromDb(dbitem);
		}
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId()
	{
		return Gameworld.ForagableProfiles.GetAll(Id);
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this foragable profile
	#3yield <which> <max> <hourly regain>#0 - sets up a yield for this profile
	#3yield <which> 0#0 - removes a yield from this profile
	#3foragable <which>#0 - toggles a foragable belonging to this profile";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "yield":
			case "yields":
				return BuildingCommandYield(actor, command);
			case "forage":
			case "forageable":
			case "foraged":
			case "foragable":
				return BuildingCommandForagable(actor, command);
			default:
				actor.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandForagable(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which foragable do you want to add or remove from this profile?");
			return false;
		}

		var foragable = long.TryParse(command.PopSpeech(), out var value)
			? actor.Gameworld.Foragables.Get(value)
			: actor.Gameworld.Foragables.GetByName(command.Last, true);

		if (foragable == null)
		{
			actor.Send("There is no such foragable.");
			return false;
		}

		if (Foragables.Any(x => x.Id == foragable.Id))
		{
			_foragableIds.RemoveAll(x => x == foragable.Id);
			Changed = true;
			actor.Send("You remove foragable {0} {1:N0} from the foragable profile.", foragable.Name, foragable.Id);
			return true;
		}

		if (foragable.Status != RevisionStatus.Current)
		{
			actor.Send(
				"You can only add approved foragables to a forage profile. Profile {0} {1:N0}r{2:N0} is {3}",
				foragable.Name, foragable.Id, foragable.RevisionNumber, foragable.Status.Describe());
			return false;
		}

		_foragableIds.Add(foragable.Id);
		Changed = true;
		actor.Send("You add foragable {0} {1:N0} from the foragable profile.", foragable.Name, foragable.Id);
		return true;
	}

	private bool BuildingCommandYield(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which forage type do you want to set the yield for?");
			return false;
		}

		var forageType = command.PopSpeech().ToLowerInvariant();
		if (command.IsFinished)
		{
			actor.Send("You must specify both a maximum yield and yield recovery per hour.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var maximumYield))
		{
			actor.Send("The maximum yield must be a number.");
			return false;
		}

		if (maximumYield <= 0.0)
		{
			if (!_maximumYieldPoints.ContainsKey(forageType))
			{
				actor.OutputHandler.Send(
					"There is no such yield to remove. If you're trying to add a new yield, it must have a maximum of greater than zero.");
				return false;
			}

			_maximumYieldPoints.Remove(forageType);
			_hourlyYieldPoints.Remove(forageType);
			Changed = true;
			actor.OutputHandler.Send(
				$"This foragable profile will no longer contain the {forageType.ColourName()} yield type.");
			return true;
		}

		if (command.IsFinished)
		{
			actor.Send("You must specify both a maximum yield and yield recovery per hour.");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var hourlyGain))
		{
			actor.Send("The hourly yield recovery must be a number.");
			return false;
		}

		_maximumYieldPoints[forageType] = maximumYield;
		_hourlyYieldPoints[forageType] = hourlyGain;
		actor.OutputHandler.Send(
			$"You set the maximum yield for the {forageType.ColourName()} forage type to {maximumYield.ToString("N2", actor).ColourValue()} and the hourly yield recovery to {hourlyGain.ToString("N2", actor).ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What name do you want to give to this forage profile?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase().Trim();
		Changed = true;
		actor.Send("You set the name of this forage profile to {0}.", Name.Colour(Telnet.Cyan));
		return true;
	}

	#endregion

	#region IForagableProfile Members

	private Dictionary<string, double> _maximumYieldPoints;

	public IReadOnlyDictionary<string, double> MaximumYieldPoints => _maximumYieldPoints;

	private Dictionary<string, double> _hourlyYieldPoints;

	public IReadOnlyDictionary<string, double> HourlyYieldPoints => _hourlyYieldPoints;

	private readonly List<long> _foragableIds = new();
	public IEnumerable<IForagable> Foragables => _foragableIds.SelectNotNull(x => Gameworld.Foragables.Get(x));

	public IForagable GetForageResult(ICharacter character, IReadOnlyDictionary<Difficulty, CheckOutcome> forageOutcome,
		string foragableType)
	{
		return
			Foragables.Where(
						  x =>
							  x.ForagableTypes.Any(y =>
								  y.Equals(foragableType, StringComparison.InvariantCultureIgnoreCase)) &&
							  x.CanForage(character, forageOutcome[x.ForageDifficulty]))
					  .GetWeightedRandom(x => x.RelativeChance);
	}

	#endregion
}