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
	private string? _transformationEcho;
	private bool _allowVoluntarySwitch;
	private IFutureProg? _canVoluntarilySwitchProg;
	private IFutureProg? _whyCannotVoluntarilySwitchProg;
	private IFutureProg? _canSeeFormProg;
	private IEntityDescriptionPattern? _shortDescriptionPattern;
	private IEntityDescriptionPattern? _fullDescriptionPattern;
	private bool _autoTransformWhenApplicable;
	private ForcedTransformationPriorityBand _forcedTransformationPriorityBand = ForcedTransformationPriorityBand.MeritOrIntrinsic;
	private int _forcedTransformationPriorityOffset;
	private ForcedTransformationRecheckCadence _applicabilityRecheckCadence = ForcedTransformationRecheckCadence.FuzzyHour;

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
		_race = gameworld.Races.Get(long.Parse(definition.Element("Race")?.Value ?? "0"))!;
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

		_transformationEcho = definition.Element("TransformationEcho")?.Attribute("mode")?.Value switch
		{
			"none" => string.Empty,
			_ => definition.Element("TransformationEcho")?.Value
		};
		if (_transformationEcho == string.Empty &&
		    definition.Element("TransformationEcho")?.Attribute("mode")?.Value != "none")
		{
			_transformationEcho = null;
		}
		_allowVoluntarySwitch = bool.TryParse(definition.Element("AllowVoluntarySwitch")?.Value, out var allow) && allow;
		_canVoluntarilySwitchProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("CanVoluntarilySwitchProg")?.Value ?? "0"));
		_whyCannotVoluntarilySwitchProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("WhyCannotVoluntarilySwitchProg")?.Value ?? "0"));
		_canSeeFormProg = gameworld.FutureProgs.Get(long.Parse(definition.Element("CanSeeFormProg")?.Value ?? "0"));
		_shortDescriptionPattern = gameworld.EntityDescriptionPatterns.Get(long.Parse(definition.Element("ShortDescriptionPattern")?.Value ?? "0"));
		_fullDescriptionPattern = gameworld.EntityDescriptionPatterns.Get(long.Parse(definition.Element("FullDescriptionPattern")?.Value ?? "0"));
		_autoTransformWhenApplicable = bool.TryParse(definition.Element("AutoTransformWhenApplicable")?.Value, out var autoTransform) &&
		                               autoTransform;
		if (int.TryParse(definition.Element("ForcedTransformationPriorityBand")?.Value, out var priorityBand))
		{
			_forcedTransformationPriorityBand = (ForcedTransformationPriorityBand)priorityBand;
		}

		_ = int.TryParse(definition.Element("ForcedTransformationPriorityOffset")?.Value, out _forcedTransformationPriorityOffset);
		if (int.TryParse(definition.Element("ApplicabilityRecheckCadence")?.Value, out var cadence))
		{
			_applicabilityRecheckCadence = (ForcedTransformationRecheckCadence)cadence;
		}
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
		TransformationEcho = _transformationEcho,
		AllowVoluntarySwitch = _allowVoluntarySwitch,
		CanVoluntarilySwitchProg = _canVoluntarilySwitchProg,
		WhyCannotVoluntarilySwitchProg = _whyCannotVoluntarilySwitchProg,
		CanSeeFormProg = _canSeeFormProg,
		ShortDescriptionPattern = _shortDescriptionPattern,
		FullDescriptionPattern = _fullDescriptionPattern
	};
	public bool AutoTransformWhenApplicable => _autoTransformWhenApplicable;
	public ForcedTransformationPriorityBand ForcedTransformationPriorityBand => _forcedTransformationPriorityBand;
	public int ForcedTransformationPriorityOffset => _forcedTransformationPriorityOffset;
	public ForcedTransformationRecheckCadence ApplicabilityRecheckCadence => _applicabilityRecheckCadence;

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(
			new XElement("Race", _race?.Id ?? 0L),
			new XElement("Ethnicity", _ethnicity?.Id ?? 0L),
			new XElement("Gender", _gender.HasValue ? (int)_gender.Value : -1),
			new XElement("Alias", _alias ?? string.Empty),
			new XElement("SortOrder", _sortOrder?.ToString() ?? string.Empty),
			new XElement("TraumaMode", (int)_traumaMode),
			new XElement("TransformationEcho",
				new XAttribute("mode", _transformationEcho switch
				{
					null => "default",
					"" => "none",
					_ => "custom"
				}),
				_transformationEcho ?? string.Empty),
			new XElement("AllowVoluntarySwitch", _allowVoluntarySwitch),
			new XElement("CanVoluntarilySwitchProg", _canVoluntarilySwitchProg?.Id ?? 0L),
			new XElement("WhyCannotVoluntarilySwitchProg", _whyCannotVoluntarilySwitchProg?.Id ?? 0L),
			new XElement("CanSeeFormProg", _canSeeFormProg?.Id ?? 0L),
			new XElement("ShortDescriptionPattern", _shortDescriptionPattern?.Id ?? 0L),
			new XElement("FullDescriptionPattern", _fullDescriptionPattern?.Id ?? 0L),
			new XElement("AutoTransformWhenApplicable", _autoTransformWhenApplicable),
			new XElement("ForcedTransformationPriorityBand", (int)_forcedTransformationPriorityBand),
			new XElement("ForcedTransformationPriorityOffset", _forcedTransformationPriorityOffset),
			new XElement("ApplicabilityRecheckCadence", (int)_applicabilityRecheckCadence)
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
		sb.AppendLine($"Initial Transformation Echo: {_transformationEcho switch
		{
			null => "Default".ColourValue(),
			"" => "Suppressed".ColourError(),
			_ => _transformationEcho.ColourCommand()
		}}");
		sb.AppendLine($"Allow Voluntary Switching: {_allowVoluntarySwitch.ToColouredString()}");
		sb.AppendLine($"Voluntary Can Prog: {_canVoluntarilySwitchProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Voluntary Why-Cant Prog: {_whyCannotVoluntarilySwitchProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Visibility Prog: {_canSeeFormProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Initial Short Description Pattern: {_shortDescriptionPattern?.Pattern.ColourCommand() ?? "Random Valid".ColourValue()}");
		sb.AppendLine($"Initial Full Description Pattern: {_fullDescriptionPattern?.Pattern.ColourCommand() ?? "Random Valid".ColourValue()}");
		sb.AppendLine($"Auto-Transform When Applicable: {_autoTransformWhenApplicable.ToColouredString()}");
		sb.AppendLine($"Forced Transformation Priority Band: {_forcedTransformationPriorityBand.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Forced Transformation Priority Offset: {_forcedTransformationPriorityOffset.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Applicability Recheck Cadence: {_applicabilityRecheckCadence.DescribeEnum().ColourValue()}");
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3race <which>#0 - sets the race for the provisioned form
	#3ethnicity <which>|clear#0 - sets or clears the ethnicity override for the provisioned form
	#3gender <which>|clear#0 - sets or clears the gender override for the provisioned form
	#3alias <text>|clear#0 - sets or clears the initial alias for the provisioned form
	#3sort <number>|clear#0 - sets or clears the initial sort order for the provisioned form
	#3trauma <auto|transfer|stash>#0 - sets the initial trauma handling mode
	#3echo <text>|default|none#0 - sets, defaults or suppresses the transformation echo
	#3allow [true|false]#0 - toggles or sets whether the form initially permits voluntary switching
	#3canprog <prog>|clear#0 - sets or clears the initial voluntary eligibility prog
	#3whycantprog <prog>|clear#0 - sets or clears the initial voluntary denial-message prog
	#3visibleprog <prog>|clear#0 - sets or clears the initial visibility prog
	#3sdescpattern <pattern>|random|clear#0 - sets or auto-randomises the initial short description pattern
	#3fdescpattern <pattern>|random|clear#0 - sets or auto-randomises the initial full description pattern
	#3autotransform [true|false]#0 - toggles or sets whether applicability should force the character into this form
	#3priorityband <merit|drug|spell|admin>#0 - sets the forced-transformation priority band
	#3priorityoffset <number>#0 - sets the forced-transformation priority offset within the band
	#3recheck <none|hour|minute>#0 - sets the fuzzy cadence for rechecking applicability while the character is online";

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
			case "echo":
			case "transformecho":
			case "transformationecho":
				return BuildingCommandEcho(actor, command);
			case "allow":
				return BuildingCommandAllow(actor, command);
			case "canprog":
				return BuildingCommandCanProg(actor, command);
			case "whycantprog":
				return BuildingCommandWhyCantProg(actor, command);
			case "visibleprog":
			case "visibilityprog":
				return BuildingCommandVisibleProg(actor, command);
			case "sdescpattern":
			case "shortdescpattern":
				return BuildingCommandDescriptionPattern(actor, command, EntityDescriptionType.ShortDescription);
			case "fdescpattern":
			case "descpattern":
			case "fulldescpattern":
				return BuildingCommandDescriptionPattern(actor, command, EntityDescriptionType.FullDescription);
			case "autotransform":
				return BuildingCommandAutoTransform(actor, command);
			case "priorityband":
			case "forcepriorityband":
				return BuildingCommandPriorityBand(actor, command);
			case "priorityoffset":
			case "forcepriorityoffset":
				return BuildingCommandPriorityOffset(actor, command);
			case "recheck":
			case "cadence":
			case "applicabilityrecheck":
				return BuildingCommandRecheck(actor, command);
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

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What transformation echo should this merit use, or should it be defaulted or suppressed?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		_transformationEcho = text.EqualToAny("clear", "default")
			? null
			: text.EqualToAny("none", "suppress", "blank")
				? string.Empty
				: text;
		Changed = true;
		actor.OutputHandler.Send(_transformationEcho switch
		{
			null => "This merit will now use the default transformation echo.",
			"" => "This merit will now suppress transformation echoes.",
			_ => $"This merit will now use {_transformationEcho.ColourCommand()} as its transformation echo."
		});
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

	private bool BuildingCommandDescriptionPattern(ICharacter actor, StringStack command, EntityDescriptionType type)
	{
		var label = type == EntityDescriptionType.ShortDescription ? "short description" : "full description";
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which {label} pattern should this merit use, or should it be auto-randomised?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto", "random"))
		{
			switch (type)
			{
				case EntityDescriptionType.ShortDescription:
					_shortDescriptionPattern = null;
					break;
				case EntityDescriptionType.FullDescription:
					_fullDescriptionPattern = null;
					break;
			}

			Changed = true;
			actor.OutputHandler.Send($"This merit will now auto-select a valid {label} pattern when its form is first created.");
			return true;
		}

		var pattern = Gameworld.EntityDescriptionPatterns.GetByIdOrName(command.SafeRemainingArgument);
		if (pattern is null || pattern.Type != type)
		{
			actor.OutputHandler.Send($"There is no such {label} pattern.");
			return false;
		}

		switch (type)
		{
			case EntityDescriptionType.ShortDescription:
				_shortDescriptionPattern = pattern;
				break;
			case EntityDescriptionType.FullDescription:
				_fullDescriptionPattern = pattern;
				break;
		}

		Changed = true;
		actor.OutputHandler.Send($"This merit will now use the {label} pattern {pattern.Pattern.ColourCommand()} when provisioning its form.");
		return true;
	}

	private bool BuildingCommandAutoTransform(ICharacter actor, StringStack command)
	{
		var autoTransform = !_autoTransformWhenApplicable;
		if (!command.IsFinished)
		{
			switch (command.SafeRemainingArgument.ToLowerInvariant())
			{
				case "true":
				case "yes":
				case "on":
				case "enabled":
				case "force":
					autoTransform = true;
					break;
				case "false":
				case "no":
				case "off":
				case "disabled":
					autoTransform = false;
					break;
				default:
					actor.OutputHandler.Send("You must specify true or false if you provide an explicit value.");
					return false;
			}
		}

		_autoTransformWhenApplicable = autoTransform;
		Changed = true;
		actor.OutputHandler.Send(_autoTransformWhenApplicable
			? "This merit will now force the character into its provisioned form whenever it applies."
			: "This merit will no longer force an automatic transformation when it applies.");
		return true;
	}

	private bool BuildingCommandPriorityBand(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify merit, drug, spell or admin.");
			return false;
		}

		if (!TryParsePriorityBand(command.SafeRemainingArgument, out var band))
		{
			actor.OutputHandler.Send("You must specify merit, drug, spell or admin.");
			return false;
		}

		_forcedTransformationPriorityBand = band;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merit will now use the {_forcedTransformationPriorityBand.DescribeEnum().ColourValue()} forced-transformation priority band.");
		return true;
	}

	private bool BuildingCommandPriorityOffset(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How large should the forced-transformation priority offset be?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var offset))
		{
			actor.OutputHandler.Send("That is not a valid priority offset.");
			return false;
		}

		_forcedTransformationPriorityOffset = offset;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merit will now use a forced-transformation priority offset of {_forcedTransformationPriorityOffset.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRecheck(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify none, hour or minute.");
			return false;
		}

		if (!TryParseRecheckCadence(command.SafeRemainingArgument, out var cadence))
		{
			actor.OutputHandler.Send("You must specify none, hour or minute.");
			return false;
		}

		_applicabilityRecheckCadence = cadence;
		Changed = true;
		actor.OutputHandler.Send(
			$"This merit will now use the {_applicabilityRecheckCadence.DescribeEnum().ColourValue()} applicability recheck cadence.");
		return true;
	}

	private static bool TryParsePriorityBand(string text, out ForcedTransformationPriorityBand band)
	{
		switch (text.ToLowerInvariant())
		{
			case "merit":
			case "intrinsic":
			case "meritorintrinsic":
			case "intrinsicormerit":
				band = ForcedTransformationPriorityBand.MeritOrIntrinsic;
				return true;
			case "drug":
			case "chemical":
			case "drugorchemical":
				band = ForcedTransformationPriorityBand.DrugOrChemical;
				return true;
			case "spell":
			case "power":
			case "spellorpower":
				band = ForcedTransformationPriorityBand.SpellOrPower;
				return true;
			case "admin":
			case "forced":
			case "adminforced":
				band = ForcedTransformationPriorityBand.AdminForced;
				return true;
			default:
				band = default;
				return false;
		}
	}

	private static bool TryParseRecheckCadence(string text, out ForcedTransformationRecheckCadence cadence)
	{
		switch (text.ToLowerInvariant())
		{
			case "none":
			case "off":
			case "never":
				cadence = ForcedTransformationRecheckCadence.None;
				return true;
			case "hour":
			case "hourly":
			case "fuzzyhour":
				cadence = ForcedTransformationRecheckCadence.FuzzyHour;
				return true;
			case "minute":
			case "minutely":
			case "fuzzyminute":
				cadence = ForcedTransformationRecheckCadence.FuzzyMinute;
				return true;
			default:
				cadence = default;
				return false;
		}
	}
}
