#nullable enable
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class TransformFormEffect : IMagicSpellEffectTemplate
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

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("transformform", (root, spell) => new TransformFormEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("transformform", BuilderFactory,
			"Ensures or reuses a keyed alternate form and transforms the target into it for the spell duration.",
			HelpText,
			false,
			false,
			SpellTriggerFactory.MagicTriggerTypes
			                  .Where(x => IsCompatibleWithTrigger(SpellTriggerFactory.BuilderInfoForType(x).TargetTypes))
			                  .ToArray());
	}

	private static (IMagicSpellEffectTemplate Trigger, string Error) BuilderFactory(StringStack commands,
		IMagicSpell spell)
	{
		var defaultRace = spell.Gameworld.Races.First();
		return (new TransformFormEffect(new XElement("Effect",
			new XAttribute("type", "transformform"),
			new XElement("FormKey", "default"),
			new XElement("Race", defaultRace.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", -1),
			new XElement("Alias", $"{defaultRace.Name} form"),
			new XElement("SortOrder", string.Empty),
			new XElement("TraumaMode", (int)BodySwitchTraumaMode.Automatic),
			new XElement("TransformationEcho", new XAttribute("mode", "default"), string.Empty),
			new XElement("AllowVoluntarySwitch", false),
			new XElement("CanVoluntarilySwitchProg", 0L),
			new XElement("WhyCannotVoluntarilySwitchProg", 0L),
			new XElement("CanSeeFormProg", 0L),
			new XElement("ShortDescriptionPattern", 0L),
			new XElement("FullDescriptionPattern", 0L)
		), spell), string.Empty);
	}

	private TransformFormEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FormKey = root.Element("FormKey")?.Value ?? "default";
		_race = Gameworld.Races.Get(long.Parse(root.Element("Race")?.Value ?? "0"));
		_ethnicity = Gameworld.Ethnicities.Get(long.Parse(root.Element("Ethnicity")?.Value ?? "0"));
		if (int.TryParse(root.Element("Gender")?.Value, out var genderValue) && genderValue >= 0)
		{
			_gender = (Gender)genderValue;
		}

		_alias = root.Element("Alias")?.Value;
		if (int.TryParse(root.Element("SortOrder")?.Value, out var sortOrder))
		{
			_sortOrder = sortOrder;
		}

		if (int.TryParse(root.Element("TraumaMode")?.Value, out var traumaMode))
		{
			_traumaMode = (BodySwitchTraumaMode)traumaMode;
		}

		_transformationEcho = root.Element("TransformationEcho")?.Attribute("mode")?.Value switch
		{
			"none" => string.Empty,
			_ => root.Element("TransformationEcho")?.Value
		};
		if (_transformationEcho == string.Empty &&
		    root.Element("TransformationEcho")?.Attribute("mode")?.Value != "none")
		{
			_transformationEcho = null;
		}
		_allowVoluntarySwitch = bool.TryParse(root.Element("AllowVoluntarySwitch")?.Value, out var allow) && allow;
		_canVoluntarilySwitchProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanVoluntarilySwitchProg")?.Value ?? "0"));
		_whyCannotVoluntarilySwitchProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotVoluntarilySwitchProg")?.Value ?? "0"));
		_canSeeFormProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanSeeFormProg")?.Value ?? "0"));
		_shortDescriptionPattern = Gameworld.EntityDescriptionPatterns.Get(long.Parse(root.Element("ShortDescriptionPattern")?.Value ?? "0"));
		_fullDescriptionPattern = Gameworld.EntityDescriptionPatterns.Get(long.Parse(root.Element("FullDescriptionPattern")?.Value ?? "0"));
	}

	public IFuturemud Gameworld => Spell.Gameworld;
	public IMagicSpell Spell { get; }
	public string FormKey { get; private set; }

	private ICharacterFormSpecification FormSpecification => new CharacterFormSpecification
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

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "transformform"),
			new XElement("FormKey", FormKey),
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
			new XElement("FullDescriptionPattern", _fullDescriptionPattern?.Id ?? 0L)
		);
	}

	public bool IsInstantaneous => false;
	public bool RequiresTarget => false;

	public bool IsCompatibleWithTrigger(IMagicTrigger trigger)
	{
		return IsCompatibleWithTrigger(trigger.TargetTypes);
	}

	public static bool IsCompatibleWithTrigger(string types)
	{
		return true;
	}

	public IMagicSpellEffect? GetOrApplyEffect(ICharacter caster, IPerceivable? target, OpposedOutcomeDegree outcome,
		SpellPower power, IMagicSpellEffectParent parent, SpellAdditionalParameter[] additionalParameters)
	{
		var recipient = target as ICharacter ?? caster;
		if (!recipient.EnsureForm(FormSpecification,
			    new CharacterFormSource(CharacterFormSourceType.SpellEffect, Spell.Id, FormKey),
			    out var form,
			    out var whyNot))
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not ensure transform form '{FormKey}' for character #{recipient.Id.ToString("N0")}: {whyNot}",
				true
			);
			return null;
		}

		var priorBodyId = recipient.EffectsOfType<SpellTransformFormEffect>()
		                          .FirstOrDefault(x => x.Spell == Spell && x.FormKey.EqualTo(FormKey))
		                          ?.PriorBodyId ?? recipient.CurrentBody.Id;
		if (recipient.CurrentBody != form.Body &&
		    !recipient.SwitchToBody(form.Body, BodySwitchIntent.Scripted))
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not switch character #{recipient.Id.ToString("N0")} into form '{FormKey}'.",
				true
			);
			return null;
		}

		return new SpellTransformFormEffect(recipient, parent, FormKey, form.Body.Id, priorBodyId);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new TransformFormEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3formkey <text>#0 - sets the stable key used to reuse the same provisioned form
	#3race <which>#0 - sets the race for the provisioned form
	#3ethnicity <which>|clear#0 - sets or clears the ethnicity override
	#3gender <which>|clear#0 - sets or clears the gender override
	#3alias <text>|clear#0 - sets or clears the initial alias
	#3sort <number>|clear#0 - sets or clears the initial sort order
	#3trauma <auto|transfer|stash>#0 - sets the initial trauma mode
	#3echo <text>|default|none#0 - sets, defaults or suppresses the transformation echo
	#3allow [true|false]#0 - toggles or sets the initial voluntary-switch flag
	#3canprog <prog>|clear#0 - sets or clears the initial voluntary eligibility prog
	#3whycantprog <prog>|clear#0 - sets or clears the initial voluntary denial-message prog
	#3visibleprog <prog>|clear#0 - sets or clears the initial visibility prog
	#3sdescpattern <pattern>|random|clear#0 - sets or auto-randomises the initial short description pattern
	#3fdescpattern <pattern>|random|clear#0 - sets or auto-randomises the initial full description pattern";

	public string Show(ICharacter actor)
	{
		return
			$"TransformForm [{FormKey.ColourCommand()}] Race {_race?.Name.ColourName() ?? "None".ColourError()}, Ethnicity {_ethnicity?.Name.ColourName() ?? "Auto".ColourValue()}, Gender {_gender?.DescribeEnum().ColourValue() ?? "Auto".ColourValue()}, Alias {(_alias ?? "auto").ColourCommand()}, Trauma {_traumaMode.DescribeEnum().ColourValue()}, Echo {(_transformationEcho switch
			{
				null => "Default".ColourValue(),
				"" => "Suppressed".ColourError(),
				_ => _transformationEcho.ColourCommand()
			})}, Voluntary {_allowVoluntarySwitch.ToColouredString()}, CanProg {_canVoluntarilySwitchProg?.MXPClickableFunctionName() ?? "None".ColourError()}, WhyCant {_whyCannotVoluntarilySwitchProg?.MXPClickableFunctionName() ?? "None".ColourError()}, Visible {_canSeeFormProg?.MXPClickableFunctionName() ?? "None".ColourError()}, SDesc {_shortDescriptionPattern?.Pattern.ColourCommand() ?? "Random Valid".ColourValue()}, FDesc {_fullDescriptionPattern?.Pattern.ColourCommand() ?? "Random Valid".ColourValue()}";
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "formkey":
				return BuildingCommandFormKey(actor, command);
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
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandFormKey(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What stable form key should this effect use?");
			return false;
		}

		FormKey = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the stable form key {FormKey.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandRace(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which race should this spell provision?");
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

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision a {_race.Name.ColourName()} form.");
		return true;
	}

	private bool BuildingCommandEthnicity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which ethnicity should this spell provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_ethnicity = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select a compatible ethnicity.");
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
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision the {_ethnicity.Name.ColourName()} ethnicity.");
		return true;
	}

	private bool BuildingCommandGender(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which gender should this spell provision, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_gender = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select a compatible gender.");
			return true;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<Gender>(out var gender))
		{
			actor.OutputHandler.Send("That is not a valid gender.");
			return false;
		}

		_gender = gender;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now provision the {_gender.Value.DescribeEnum().ColourValue()} gender.");
		return true;
	}

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial alias should this spell use for its provisioned form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_alias = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now auto-select its initial alias from the race name.");
			return true;
		}

		_alias = command.SafeRemainingArgument;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now initially name its provisioned form {_alias.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSortOrder(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What initial sort order should this spell use for its form, or should it be cleared?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none", "auto"))
		{
			_sortOrder = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect will now append its form after the character's existing forms.");
			return true;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var sortOrder))
		{
			actor.OutputHandler.Send("That is not a valid sort order.");
			return false;
		}

		_sortOrder = sortOrder;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use sort order {_sortOrder.Value.ToString("N0", actor).ColourValue()}.");
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

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use {_traumaMode.DescribeEnum().ColourValue()} trauma handling.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What transformation echo should this effect use, or should it be defaulted or suppressed?");
			return false;
		}

		var text = command.SafeRemainingArgument;
		_transformationEcho = text.EqualToAny("clear", "default")
			? null
			: text.EqualToAny("none", "suppress", "blank")
				? string.Empty
				: text;
		Spell.Changed = true;
		actor.OutputHandler.Send(_transformationEcho switch
		{
			null => "This effect will now use the default transformation echo.",
			"" => "This effect will now suppress transformation echoes.",
			_ => $"This effect will now use {_transformationEcho.ColourCommand()} as its transformation echo."
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
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will {_allowVoluntarySwitch.NowNoLonger()} initially permit voluntary switching.");
		return true;
	}

	private bool BuildingCommandCanProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control voluntary switching for this spell-granted form?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none"))
		{
			_canVoluntarilySwitchProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect no longer sets a voluntary-switch eligibility prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_canVoluntarilySwitchProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the {prog.MXPClickableFunctionName()} prog for voluntary switch eligibility.");
		return true;
	}

	private bool BuildingCommandWhyCantProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should supply the denial message for this spell-granted form?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none"))
		{
			_whyCannotVoluntarilySwitchProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect no longer sets a denial-message prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
			[ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_whyCannotVoluntarilySwitchProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the {prog.MXPClickableFunctionName()} prog for denial text.");
		return true;
	}

	private bool BuildingCommandVisibleProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should control whether the owner can see this spell-granted form?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("clear", "none"))
		{
			_canSeeFormProg = null;
			Spell.Changed = true;
			actor.OutputHandler.Send("This effect no longer sets a visibility prog.");
			return true;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
			[ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		_canSeeFormProg = prog;
		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the {prog.MXPClickableFunctionName()} prog to decide whether the form is visible.");
		return true;
	}

	private bool BuildingCommandDescriptionPattern(ICharacter actor, StringStack command, EntityDescriptionType type)
	{
		var label = type == EntityDescriptionType.ShortDescription ? "short description" : "full description";
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which {label} pattern should this effect use, or should it be auto-randomised?");
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

			Spell.Changed = true;
			actor.OutputHandler.Send($"This effect will now auto-select a valid {label} pattern when its form is first created.");
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

		Spell.Changed = true;
		actor.OutputHandler.Send($"This effect will now use the {label} pattern {pattern.Pattern.ColourCommand()} when provisioning its form.");
		return true;
	}
}
