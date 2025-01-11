using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Climate;

public class Season : SaveableItem, ISeason
{
	public Season(MudSharp.Models.Season season, [NotNull] IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = season.Id;
		_name = season.Name;
		DisplayName = season.DisplayName;
		SeasonGroup = season.SeasonGroup;
		CelestialDayOnset = season.CelestialDayOnset;
		Celestial = gameworld.CelestialObjects.Get(season.CelestialId);
	}

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.Season
			{
				Name = Name,
				DisplayName = DisplayName,
				CelestialId = Celestial.Id,
				CelestialDayOnset = CelestialDayOnset,
				SeasonGroup = SeasonGroup
			};
			FMDB.Context.Seasons.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Season(IFuturemud gameworld, string name, int onset)
	{
		Gameworld = gameworld;
		_name = name;
		Celestial = Gameworld.CelestialObjects.First();
		CelestialDayOnset = onset;
		DisplayName = name;
		DoDatabaseInsert();
	}

	private Season(Season rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		Celestial = rhs.Celestial;
		CelestialDayOnset = rhs.CelestialDayOnset;
		SeasonGroup = rhs.SeasonGroup;
		DisplayName = name;
		DoDatabaseInsert();
	}

	public ISeason Clone(string name)
	{
		return new Season(this, name);
	}

	#region Overrides of Item

	public override string FrameworkItemType => "Season";

	#endregion

	public string DisplayName { get; protected set; }
	public string SeasonGroup { get; protected set; }

	public int CelestialDayOnset { get; protected set; }
	public ICelestialObject Celestial { get; protected set; }

	#region Overrides of SaveableItem

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Seasons.Find(Id);
		dbitem.Name = Name;
		dbitem.DisplayName = DisplayName;
		dbitem.CelestialDayOnset = CelestialDayOnset;
		dbitem.SeasonGroup = SeasonGroup;
		dbitem.CelestialId = Celestial.Id;
		Changed = false;
	}

	#endregion

	#region Implementation of IEditableItem

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - sets the name of the season
	#3display <name>#0 - sets the display name in the time command
	#3onset <##>#0 - sets the day number that this season begins
	#3group <group>#0 - sets the group that this season belongs to
	#3group none#0 - sets the season to belong to no group
	#3celestial <id|name>#0 - sets the celestial this season is tied to";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "display":
			case "displayname":
				return BuildingCommandDisplayName(actor, command);
			case "onset":
				return BuildingCommandOnset(actor, command);
			case "group":
				return BuildingCommandGroup(actor, command);
			case "celestial":
				return BuildingCommandCelestial(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandCelestial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which celestial should this season be tied to?");
			return false;
		}

		var celestial = Gameworld.CelestialObjects.GetByIdOrName(command.SafeRemainingArgument);
		if (celestial is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} does not represent a valid celestial object.");
			return false;
		}

		Celestial = celestial;
		Changed = true;
		actor.OutputHandler.Send($"This season is now tied to the {celestial.Name.ColourValue()} celestial object. Make sure to examine the onsets.");
		return true;
	}

	private bool BuildingCommandGroup(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What season group should this season belong to? Use #3none#0 to remove a season group.");
			return false;
		}

		var group = command.SafeRemainingArgument.ToLowerInvariant();
		if (group.EqualTo("none"))
		{
			SeasonGroup = string.Empty;
			Changed = true;
			actor.OutputHandler.Send("This season now does not belong to any season group.");
			return true;
		}

		SeasonGroup = group;
		Changed = true;
		actor.OutputHandler.Send($"This season now belongs to the {group.ColourValue()} group.");
		return true;
	}

	private bool BuildingCommandOnset(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What celestial day should the onset of this season be?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		CelestialDayOnset = value;
		Changed = true;
		actor.OutputHandler.Send($"This season now has a celestial day onset of day #{value.ToStringN0Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandDisplayName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What display name do you want to give to this season?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();

		actor.OutputHandler.Send($"The display name for this season is now {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this season?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Seasons.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a season with the name {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the season from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Season #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Display Name: {DisplayName.ColourValue()}");
		sb.AppendLine($"Group: {SeasonGroup?.ColourValue() ?? ""}");
		sb.AppendLine($"Onset Day: {CelestialDayOnset.ToStringN0Colour(actor)}");
		sb.AppendLine($"Celestial: {Celestial.Name.ColourValue()}");
		return sb.ToString();
	}

	#endregion
}