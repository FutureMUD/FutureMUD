using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Models;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using ExpressionEngine;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using NCalc.Helpers;
using System.Xml.Linq;

namespace MudSharp.CharacterCreation.Resources;

public abstract class ChargenResourceBase : SaveableItem, IChargenResource
{
	public sealed override string FrameworkItemType => "ChargenResource";

	protected ChargenResourceBase(IFuturemud gameworld, ChargenResource resource)
	{
		Gameworld = gameworld;
		_id = resource.Id;
		_name = resource.Name;
		PluralName = resource.PluralName;
		Alias = resource.Alias;
		MinimumTimeBetweenAwards = TimeSpan.FromMinutes(resource.MinimumTimeBetweenAwards);
		MaximumNumberAwardedPerAward = resource.MaximumNumberAwardedPerAward;
		PermissionLevelRequiredToAward = (PermissionLevel)resource.PermissionLevelRequiredToAward;
		PermissionLevelRequiredToCircumventMinimumTime =
			(PermissionLevel)resource.PermissionLevelRequiredToCircumventMinimumTime;
		ShowToPlayerInScore = resource.ShowToPlayerInScore;
		TextDisplayedToPlayerOnAward = resource.TextDisplayedToPlayerOnAward;
		TextDisplayedToPlayerOnDeduct = resource.TextDisplayedToPlayerOnDeduct;
		MaximumResourceExpression = new Expression(resource.MaximumResourceFormula);
		ControlProg = gameworld.FutureProgs.Get(resource.ControlProgId ?? 0);
	}

	protected ChargenResourceBase(IFuturemud gameworld, string name, string plural, string alias)
	{
		Gameworld = gameworld;
		_name = name;
		PluralName = plural;
		Alias = alias;
		TextDisplayedToPlayerOnAward = $"You have been awarded {plural}";
		TextDisplayedToPlayerOnDeduct = $"You have had {plural} deducted";
		PermissionLevelRequiredToAward = PermissionLevel.Admin;
		PermissionLevelRequiredToCircumventMinimumTime = PermissionLevel.SeniorAdmin;
		ControlProg = Gameworld.AlwaysTrueProg;
	}

	protected void DoDatabaseInsert(string type)
	{
		using (new FMDB())
		{
			var dbitem = new Models.ChargenResource
			{
				Name = Name,
				PluralName = PluralName,
				Alias = Alias,
				MinimumTimeBetweenAwards = (int)MinimumTimeBetweenAwards.TotalMinutes,
				MaximumNumberAwardedPerAward = MaximumNumberAwardedPerAward,
				PermissionLevelRequiredToAward = (int)PermissionLevelRequiredToAward,
				PermissionLevelRequiredToCircumventMinimumTime = (int)PermissionLevelRequiredToCircumventMinimumTime,
				ShowToPlayerInScore = ShowToPlayerInScore,
				TextDisplayedToPlayerOnAward = TextDisplayedToPlayerOnAward,
				TextDisplayedToPlayerOnDeduct = TextDisplayedToPlayerOnDeduct,
				MaximumResourceId = MaximumResourceReference?.Id,
				MaximumResourceFormula = MaximumResourceExpression.OriginalExpression,
				Type = type,
				ControlProgId = ControlProg?.Id,
			};
			FMDB.Context.ChargenResources.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public static IEnumerable<string> BuilderTypesAvailable =>
	[
		"simple",
		"regenerating",
		"realtime",
		"playtime"
	];

	public static IChargenResource LoadFromBuilderInput(IFuturemud gameworld, string type, string name, string plural, string alias)
	{
		switch (type.ToLowerInvariant())
		{
			case "simple":
				return new SimpleChargenResource(gameworld, name, plural, alias);
			case "regenerating":
				return new RegeneratingChargenResource(gameworld, name, plural, alias);
			case "realtime":
				return new RealtimeRegeneratingResource(gameworld, name, plural, alias);
			case "playtime":
				return new TotalPlaytimeResource(gameworld, name, plural, alias);
		}

		return null;
	}

	public static IChargenResource LoadFromDatabase(IFuturemud gameworld, ChargenResource resource)
	{
		switch (resource.Type)
		{
			case "Simple":
				return new SimpleChargenResource(gameworld, resource);
			case "Regenerating":
				return new RegeneratingChargenResource(gameworld, resource);
			case "Playtime":
				return new TotalPlaytimeResource(gameworld, resource);
			case "Realtime":
				return new RealtimeRegeneratingResource(gameworld, resource);
			default:
				throw new NotSupportedException(
					"Unsupported ChargenResource type in ChargeResourceBase.LoadFromDatabase.");
		}
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.ChargenResources.Find(Id);
		dbitem.Name = Name;
		dbitem.PluralName = PluralName;
		dbitem.Alias = Alias;
		dbitem.MinimumTimeBetweenAwards = (int)MinimumTimeBetweenAwards.TotalMinutes;
		dbitem.MaximumNumberAwardedPerAward = MaximumNumberAwardedPerAward;
		dbitem.PermissionLevelRequiredToAward = (int)PermissionLevelRequiredToAward;
		dbitem.PermissionLevelRequiredToCircumventMinimumTime = (int) PermissionLevelRequiredToCircumventMinimumTime;
		dbitem.ShowToPlayerInScore = ShowToPlayerInScore;
		dbitem.TextDisplayedToPlayerOnAward = TextDisplayedToPlayerOnAward;
		dbitem.TextDisplayedToPlayerOnDeduct = TextDisplayedToPlayerOnDeduct;
		dbitem.MaximumResourceFormula = MaximumResourceExpression.OriginalExpression;
		dbitem.ControlProgId = ControlProg?.Id;
		dbitem.MaximumResourceId = MaximumResourceReference?.Id;
		Changed = false;
	}

	#region IChargenResource Members

	public virtual void PerformPostLoadUpdate(ChargenResource resource, IFuturemud gameworld)
	{
		if (resource.MaximumResourceId.HasValue)
		{
			MaximumResourceReference = gameworld.ChargenResources.Get(resource.MaximumResourceId.Value);
		}
	}


	public Expression MaximumResourceExpression { get; protected set; }

	public IChargenResource MaximumResourceReference { get; protected set; }

	protected IFutureProg ControlProg { get; private set; }
	public string PluralName { get; protected set; }

	public string Alias { get; protected set; }

	IEnumerable<string> IHaveMultipleNames.Names => [Name, PluralName, Alias];

	public TimeSpan MinimumTimeBetweenAwards { get; protected set; }

	public double MaximumNumberAwardedPerAward { get; protected set; }

	public PermissionLevel PermissionLevelRequiredToAward { get; protected set; }

	public PermissionLevel PermissionLevelRequiredToCircumventMinimumTime { get; protected set; }

	public abstract void UpdateOnSave(ICharacter character, int oldMinutes, int newMinutes);

	public bool ShowToPlayerInScore { get; protected set; }

	public string TextDisplayedToPlayerOnAward { get; protected set; }

	public string TextDisplayedToPlayerOnDeduct { get; protected set; }

	public int GetMaximum(IAccount account)
	{
		if (MaximumResourceReference != null)
		{
			MaximumResourceExpression.Parameters["variable"] = account.AccountResources[MaximumResourceReference];
		}

		return Convert.ToInt32(MaximumResourceExpression.Evaluate());
	}

	public virtual bool DisplayChangesOnLogin => false;

	#endregion

	public abstract string TypeName { get; }

	public string HelpText => @$"You can use the following options with this command:

	#3name <name>#0 - renames the resource
	#3plural <name>#0 - sets the plural name
	#3alias <alias>#0 - sets the alias
	#3score#0 - toggles the resource showing in player scores
	#3permission <level>#0 - sets the permission level required to award
	#3permissiontime <level>#0 - sets the permission level required to skip the wait time
	#3time <timespan>#0 - sets the time between awards
	#3max <amount>#0 - sets the maximum amount that's awarded each award
	#3control <prog>#0 - sets the prog that determines eligibility for automatic awards
	#3resource <which>#0 - sets another resource that is used as a variable in the cap formula
	#3formula <formula>#0 - sets the formula used to control the cap on amount

Note - for the cap formula, use #6variable#0 to substitute the referenced resource, and use a fixed value of #6-1#0 to signify that there is no cap.";

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "plural":
				return BuildingCommandPlural(actor, command);
			case "alias":
				return BuildingCommandAlias(actor, command);
			case "score":
				return BuildingCommandScore(actor);
			case "permission":
				return BuildingCommandPermission(actor, command);
			case "permissiontime":
				return BuildingCommandPermissionTime(actor, command);
			case "time":
				return BuildingCommandTime(actor, command);
			case "max":
			case "maximum":
				return BuildingCommandMaximum(actor, command);
			case "control":
			case "prog":
			case "controlprog":
				return BuildingCommandControlProg(actor, command);
			case "resource":
				return BuildingCommandResource(actor, command);
			case "maxformula":
			case "formula":
				return BuildingCommandMaxFormula(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this resource?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.ChargenResources.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a resource with the name {name.ColourValue()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the chargen resource {Name.TitleCase().ColourValue()} to {name.ColourValue()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandPlural(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What plural name do you want to give to this resource?");
			return false;
		}

		PluralName = command.SafeRemainingArgument.TitleCase();
		Changed = true;
		actor.OutputHandler.Send($"The plural name of this resource is now {PluralName.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What alias do you want to give to this resource?");
			return false;
		}

		var alias = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.ChargenResources.Any(x => x.Alias.EqualTo(alias)))
		{
			actor.OutputHandler.Send($"There is already a resource with the alias {alias.ColourValue()}. Aliases must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You set the alias of resource {Name.TitleCase().ColourValue()} to {alias.ColourValue()}.");
		Alias = alias;
		Changed = true;
		return true;
	}

	private bool BuildingCommandScore(ICharacter actor)
	{
		ShowToPlayerInScore = !ShowToPlayerInScore;
		Changed = true;
		actor.OutputHandler.Send($"This resource will {ShowToPlayerInScore.NowNoLonger()} show to players in score.");
		return true;
	}

	private bool BuildingCommandPermission(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What permission level should be required to award this resource? The possible values are {Enum.GetValues<PermissionLevel>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<PermissionLevel>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid permission level. The possible values are {Enum.GetValues<PermissionLevel>().ListToColouredString()}.");
			return false;
		}

		PermissionLevelRequiredToAward = value;
		Changed = true;
		actor.OutputHandler.Send($"Staff will now require a minimum authority level of {value.DescribeEnum().ColourValue()} to award this resource.");
		return true;
	}

	private bool BuildingCommandPermissionTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What permission level should be required to circumvent the minimum time to award this resource? The possible values are {Enum.GetValues<PermissionLevel>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<PermissionLevel>(out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid permission level. The possible values are {Enum.GetValues<PermissionLevel>().ListToColouredString()}.");
			return false;
		}

		PermissionLevelRequiredToCircumventMinimumTime = value;
		Changed = true;
		actor.OutputHandler.Send($"Staff will now require a minimum authority level of {value.DescribeEnum().ColourValue()} to circumvent the minimum time between awards for this resource.");
		return true;
	}

	private bool BuildingCommandTime(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How long should be the minimum time between manual awards of this resource?");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid timespan.");
			return false;
		}

		MinimumTimeBetweenAwards = value;
		Changed = true;
		actor.OutputHandler.Send($"The minimum time between manual awards of this resource is now {MinimumTimeBetweenAwards.DescribePreciseBrief(actor).ColourValue()}.");
		return true;

	}

	private bool BuildingCommandMaximum(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What is the maximum amount of this resource that can be awarded at one time?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, actor, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		MaximumNumberAwardedPerAward = value;
		Changed = true;
		actor.OutputHandler.Send($"The maximum amount of this resource that can be awarded at one time is now {value.ToStringN2Colour(actor)}.");
		return true;
	}

	private bool BuildingCommandControlProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog should control whether a player is eligible for automatic system awards of this resource?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean, [ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ControlProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This resource now uses the {prog.MXPClickableFunctionName()} prog to control automatic award eligibility.");
		return true;
	}

	private bool BuildingCommandResource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which other resource should be used as the #3variable#0 parameter in the cap formula for this resource?".SubstituteANSIColour());
			return false;
		}

		var resource = Gameworld.ChargenResources.GetByIdOrName(command.SafeRemainingArgument);
		if (resource is null)
		{
			actor.OutputHandler.Send($"There is no chargen resource identified by the text {command.SafeRemainingArgument.ColourCommand()}.");
			return false;
		}

		if (resource == this)
		{
			actor.OutputHandler.Send("A resource can't refer to itself for its cap formula.");
			return false;
		}

		MaximumResourceReference = resource;
		Changed = true;
		actor.OutputHandler.Send($"The cap formula for this resource will now use the {resource.Name.ColourValue()} resource as its {"variable".ColourCommand()} parameter.");
		return true;
	}

	private bool BuildingCommandMaxFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What formula do you want to use to set the cap formula for this resource?");
			return false;
		}

		var expr = new Expression(command.SafeRemainingArgument);
		if (expr.HasErrors())
		{
			actor.OutputHandler.Send(expr.Error);
			return false;
		}

		MaximumResourceExpression = expr;
		Changed = true;
		actor.OutputHandler.Send($"This resource now uses the formula {expr.OriginalExpression.ColourCommand()} for its cap.");
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Chargen Resource #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {TypeName.ColourValue()}");
		sb.AppendLine($"Plural Name: {PluralName.ColourValue()}");
		sb.AppendLine($"Alias: {Alias.ColourValue()}");
		sb.AppendLine($"Show In Score: {ShowToPlayerInScore.ToColouredString()}");
		sb.AppendLine($"Permission To Award: {PermissionLevelRequiredToAward.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Permission To Circumvent Time: {PermissionLevelRequiredToCircumventMinimumTime.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Minimum Time Between Awards: {MinimumTimeBetweenAwards.DescribePreciseBrief(actor).ColourValue()}");
		sb.AppendLine($"Maximum Per Award: {MaximumNumberAwardedPerAward.ToStringN2Colour(actor)}");
		sb.AppendLine($"Control Prog: {ControlProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Resource: {MaximumResourceReference?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Maximum Resource Expression: {MaximumResourceExpression.OriginalExpression.ColourCommand()}");
		if (!string.IsNullOrEmpty(ShowSubtype(actor)))
		{
			sb.Append(ShowSubtype(actor));
		}

		sb.AppendLine();
		sb.AppendLine("Text Shown on Award:");
		sb.AppendLine();
		sb.AppendLine(TextDisplayedToPlayerOnAward.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Text Shown on Deduct:");
		sb.AppendLine();
		sb.AppendLine(TextDisplayedToPlayerOnDeduct.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		return sb.ToString();
	}

	public virtual string ShowSubtype(ICharacter actor)
	{
		return "";
	}
}