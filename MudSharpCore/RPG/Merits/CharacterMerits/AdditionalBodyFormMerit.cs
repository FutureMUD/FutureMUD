#nullable enable
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AdditionalBodyFormMerit : CharacterMeritBase, IAdditionalBodyFormMerit
{
	private IRace _race = null!;
	private IEthnicity? _ethnicity;
	private Gender? _gender;
	private string? _alias;
	private int? _sortOrder;
	private BodySwitchTraumaMode _traumaMode = BodySwitchTraumaMode.Automatic;
	private bool _allowVoluntarySwitch;
	private IFutureProg? _canVoluntarilySwitchProg;
	private IFutureProg? _whyCannotVoluntarilySwitchProg;
	private IFutureProg? _canSeeFormProg;

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Additional Body Form",
			(merit, gameworld) => new AdditionalBodyFormMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Additional Body Form",
			(gameworld, name) => new AdditionalBodyFormMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Additional Body Form",
			"Ensures a character has access to an additional alternate body form.",
			new AdditionalBodyFormMerit().HelpText);
	}

	private AdditionalBodyFormMerit()
	{
	}

	private AdditionalBodyFormMerit(Merit merit, IFuturemud gameworld)
		: base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		_race = gameworld.Races.Get(long.Parse(definition.Element("Race")?.Value ?? "0"));
		_ethnicity = gameworld.Ethnicities.Get(long.Parse(definition.Element("Ethnicity")?.Value ?? "0"));
		if (int.TryParse(definition.Element("Gender")?.Value, out var genderValue))
		{
			_gender = (Gender)genderValue;
		}

		_alias = definition.Element("Alias")?.Value;
		if (int.TryParse(definition.Element("SortOrder")?.Value, out var sortOrder))
		{
			_sortOrder = sortOrder;
		}

		if (int.TryParse(definition.Element("TraumaMode")?.Value, out var traumaMode))
		{
			_traumaMode = (BodySwitchTraumaMode)traumaMode;
		}

		_allowVoluntarySwitch = bool.TryParse(definition.Element("AllowVoluntarySwitch")?.Value, out var allow) && allow;
		_canVoluntarilySwitchProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("CanVoluntarilySwitchProg")?.Value ?? "0"));
		_whyCannotVoluntarilySwitchProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("WhyCannotVoluntarilySwitchProg")?.Value ?? "0"));
		_canSeeFormProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("CanSeeFormProg")?.Value ?? "0"));
	}

	private AdditionalBodyFormMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, "Additional Body Form", "$0 have|has access to an additional body form")
	{
		_race = gameworld.Races.First();
		_alias = $"{_race.Name} form";
		_sortOrder = 0;
		DoDatabaseInsert();
	}

	public ICharacterFormSpecification FormSpecification => new CharacterFormSpecification
	{
		Race = _race,
		Ethnicity = _ethnicity,
		Gender = _gender,
		Alias = _alias,
		SortOrder = _sortOrder,
		TraumaMode = _traumaMode,
		AllowVoluntarySwitch = _allowVoluntarySwitch,
		CanVoluntarilySwitchProg = _canVoluntarilySwitchProg,
		WhyCannotVoluntarilySwitchProg = _whyCannotVoluntarilySwitchProg,
		CanSeeFormProg = _canSeeFormProg
	};

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(
			new XElement("Race", _race?.Id ?? 0L),
			new XElement("Ethnicity", _ethnicity?.Id ?? 0L),
			new XElement("Gender", _gender.HasValue ? (int)_gender.Value : -1),
			new XElement("Alias", _alias ?? string.Empty),
			new XElement("SortOrder", _sortOrder?.ToString() ?? string.Empty),
			new XElement("TraumaMode", (int)_traumaMode),
			new XElement("AllowVoluntarySwitch", _allowVoluntarySwitch),
			new XElement("CanVoluntarilySwitchProg", _canVoluntarilySwitchProg?.Id ?? 0L),
			new XElement("WhyCannotVoluntarilySwitchProg", _whyCannotVoluntarilySwitchProg?.Id ?? 0L),
			new XElement("CanSeeFormProg", _canSeeFormProg?.Id ?? 0L)
		);
		return root;
	}

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Provisioned Race: {_race?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Provisioned Ethnicity: {_ethnicity?.Name.ColourName() ?? "Auto".ColourValue()}");
		sb.AppendLine($"Provisioned Gender: {_gender?.DescribeEnum().ColourValue() ?? "Auto".ColourValue()}");
		sb.AppendLine($"Initial Alias: {(_alias ?? "auto").ColourCommand()}");
		sb.AppendLine($"Initial Sort Order: {_sortOrder?.ToString("N0", actor).ColourValue() ?? "append".ColourValue()}");
		sb.AppendLine($"Initial Trauma Mode: {_traumaMode.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Allow Voluntary Switching: {_allowVoluntarySwitch.ToColouredString()}");
		sb.AppendLine($"Voluntary Can Prog: {_canVoluntarilySwitchProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Voluntary Why-Cant Prog: {_whyCannotVoluntarilySwitchProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Visibility Prog: {_canSeeFormProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3race <which>#0 - sets the race for the provisioned form
	#3ethnicity <which>|clear#0 - sets or clears the ethnicity override for the provisioned form
	#3gender <which>|clear#0 - sets or clears the gender override for the provisioned form
	#3alias <text>|clear#0 - sets or clears the initial alias for the provisioned form
	#3sort <number>|clear#0 - sets or clears the initial sort order for the provisioned form
	#3trauma <auto|transfer|stash>#0 - sets the initial trauma handling mode
	#3allow [true|false]#0 - toggles or sets whether the form initially permits voluntary switching
	#3canprog <prog>|clear#0 - sets or clears the initial voluntary eligibility prog
	#3whycantprog <prog>|clear#0 - sets or clears the initial voluntary denial-message prog
	#3visibleprog <prog>|clear#0 - sets or clears the initial visibility prog";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "race":
				return BuildingCommandRace(actor, command);
			case "ethnicity":
				return BuildingCommandEthnicity(actor, command);
			case "gender":
				return BuildingCommandGender(actor, command);
			case "alias":
				return BuildingCommandAlias(actor, command);
			case "sort":
			case "sortorder":
				return BuildingCommandSortOrder(actor, command);
			case "trauma":
			case "traumamode":
				return BuildingCommandTrauma(actor, command);
			case "allow":
				return BuildingCommandAllow(actor, command);
			case "canprog":
				return BuildingCommandCanProg(actor, command);
			case "whycantprog":
				return BuildingCommandWhyCantProg(actor, command);
			case "visibleprog":
			case "visibilityprog":
				return BuildingCommandVisibleProg(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which race should this merit provision?");
			return false;
		}

		var race = Gameworld.Races.GetByIdOrName(command.SafeRemainingArgument);
		if (race is null)
		{
			actor.OutputHandler.Send("There is no such race.");
			return false;
		}

		_race = race;
		if (_ethnicity is not null && !_race.SameRace(_ethnicity.ParentRace))
		{
			_ethnicity = null;
		}

		Changed = true;
		actor.OutputHandler.Send($"This merit will now provision a {_race.Name.ColourName()} form.");
		return true;
	}

	private bool BuildingCommandEthnicity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity should this merit provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_ethnicity = null;
			Changed = true;
			actor.OutputHandler.Send("This merit will now auto-select a compatible ethnicity when provisioning its form.");
			return true;
		}

		var ethnicity = Gameworld.Ethnicities.GetByIdOrName(command.SafeRemainingArgument);
		if (ethnicity is null)
		{
			actor.OutputHandler.Send("There is no such ethnicity.");
			return false;
		}

		if (!_race.SameRace(ethnicity.ParentRace))
		{
			actor.OutputHandler.Send("That ethnicity is not compatible with the currently selected race.");
			return false;
		}

		_ethnicity = ethnicity;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now provision the {_ethnicity.Name.ColourName()} ethnicity.");
		return true;
	}

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which gender should this merit provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_gender = null;
			Changed = true;
			actor.OutputHandler.Send("This merit will now auto-select a compatible gender when provisioning its form.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Gender>(out var gender))
		{
			actor.OutputHandler.Send("That is not a valid gender.");
			return false;
		}

		_gender = gender;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now provision the {_gender.Value.DescribeEnum().ColourValue()} gender.");
		return true;
	}

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial alias should this merit use for its form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_alias = null;
			Changed = true;
			actor.OutputHandler.Send("This merit will now auto-select its initial alias from the race name.");
			return true;
		}

		_alias = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now initially name its provisioned form {_alias.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSortOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial sort order should this merit use for its form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_sortOrder = null;
			Changed = true;
			actor.OutputHandler.Send("This merit will now append its form after the character's existing forms.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var sortOrder))
		{
			actor.OutputHandler.Send("That is not a valid sort order.");
			return false;
		}

		_sortOrder = sortOrder;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now use sort order {_sortOrder.Value.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTrauma(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify auto, transfer or stash.");
			return false;
		}

		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "auto":
			case "automatic":
				_traumaMode = BodySwitchTraumaMode.Automatic;
				break;
			case "transfer":
				_traumaMode = BodySwitchTraumaMode.Transfer;
				break;
			case "stash":
			case "stasis":
				_traumaMode = BodySwitchTraumaMode.Stash;
				break;
			default:
				actor.OutputHandler.Send("You must specify auto, transfer or stash.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This merit will now use {_traumaMode.DescribeEnum().ColourValue()} trauma handling when provisioning its form.");
		return true;
	}

	private bool BuildingCommandAllow(ICharacter actor, StringStack command)
	{
		var allow = !_allowVoluntarySwitch;
		if (!command.IsFinished)
		{
			switch (command.SafeRemainingArgument.ToLowerInvariant())
			{
				case "true":
				case "yes":
				case "on":
				case "allow":
				case "enabled":
					allow = true;
					break;
				case "false":
				case "no":
				case "off":
				case "deny":
				case "disabled":
					allow = false;
					break;
				default:
					actor.OutputHandler.Send("You must specify true or false if you provide an explicit value.");
					return false;
			}
		}

		_allowVoluntarySwitch = allow;
		Changed = true;
		actor.OutputHandler.Send($"This merit will {_allowVoluntarySwitch.NowNoLonger()} initially permit voluntary switching.");
		return true;
	}

	private bool BuildingCommandCanProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control voluntary switching for the provisioned form?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none"))
		{
			_canVoluntarilySwitchProg = null;
			Changed = true;
			actor.OutputHandler.Send("This merit no longer sets a voluntary-switch eligibility prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_canVoluntarilySwitchProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now use the {prog.MXPClickableFunctionName()} prog for voluntary switch eligibility.");
		return true;
	}

	private bool BuildingCommandWhyCantProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should supply the voluntary-switch denial message for the provisioned form?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none"))
		{
			_whyCannotVoluntarilySwitchProg = null;
			Changed = true;
			actor.OutputHandler.Send("This merit no longer sets a denial-message prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
			[ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_whyCannotVoluntarilySwitchProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now use the {prog.MXPClickableFunctionName()} prog for denial text.");
		return true;
	}

	private bool BuildingCommandVisibleProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether the owner can see this provisioned form?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none"))
		{
			_canSeeFormProg = null;
			Changed = true;
			actor.OutputHandler.Send("This merit no longer sets a visibility prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_canSeeFormProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now use the {prog.MXPClickableFunctionName()} prog to decide whether the form is visible.");
		return true;
	}
}
