#nullable enable

using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.SpellEffects;

public class AstralProjectionSpellEffect : IMagicSpellEffectTemplate
{
	public const string AstralProjectionTransitionProfile = "astralprojection";
	public const string DefaultProjectionEcho = "Your awareness slips free into an astral projection.";
	public const string DefaultAnchorEcho = "Your body slackens as your awareness slips outward.";
	public const string DefaultCollapseEcho =
		"Your astral projection collapses and your focus returns to your primary body.";

	private IRace _race = null!;
	private IEthnicity? _ethnicity;
	private Gender? _gender;
	private string? _alias;
	private int? _sortOrder;
	private long _planeId;
	private AstralProjectionAnchorPolicy _anchorPolicy = AstralProjectionAnchorPolicy.Helpless;
	private string _projectionEcho = DefaultProjectionEcho;
	private string _anchorEcho = DefaultAnchorEcho;
	private string _collapseEcho = DefaultCollapseEcho;
	private string _backlashEcho = string.Empty;

	public static void RegisterFactory()
	{
		SpellEffectFactory.RegisterLoadTimeFactory("astralprojection",
			(root, spell) => new AstralProjectionSpellEffect(root, spell));
		SpellEffectFactory.RegisterBuilderFactory("astralprojection", BuilderFactory,
			"Creates a temporary player-focusable astral projection from a keyed alternate form.",
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
		var plane = DefaultAstralPlane(spell.Gameworld);
		return (new AstralProjectionSpellEffect(new XElement("Effect",
			new XAttribute("type", "astralprojection"),
			new XElement("FormKey", "astral"),
			new XElement("Race", defaultRace.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", -1),
			new XElement("Alias", $"{defaultRace.Name} astral projection"),
			new XElement("SortOrder", string.Empty),
			new XElement("Plane", plane?.Id ?? 0L),
			new XElement("AnchorPolicy", AstralProjectionAnchorPolicy.Helpless),
			new XElement("ProjectionEcho", new XCData(DefaultProjectionEcho)),
			new XElement("AnchorEcho", new XCData(DefaultAnchorEcho)),
			new XElement("CollapseEcho", new XCData(DefaultCollapseEcho)),
			new XElement("BacklashEcho", new XCData(string.Empty))
		), spell), string.Empty);
	}

	private AstralProjectionSpellEffect(XElement root, IMagicSpell spell)
	{
		Spell = spell;
		FormKey = root.Element("FormKey")?.Value ?? "astral";
		_race = Gameworld.Races.Get(long.Parse(root.Element("Race")?.Value ?? "0")) ?? Gameworld.Races.First();
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

		_planeId = long.Parse(root.Element("Plane")?.Value ?? "0");
		if (_planeId == 0)
		{
			_planeId = DefaultAstralPlane(Gameworld)?.Id ?? 0;
		}

		if (!((root.Element("AnchorPolicy")?.Value ?? string.Empty).TryParseEnum<AstralProjectionAnchorPolicy>(
			    out _anchorPolicy)))
		{
			_anchorPolicy = AstralProjectionAnchorPolicy.Helpless;
		}

		_projectionEcho = root.Element("ProjectionEcho")?.Value ?? DefaultProjectionEcho;
		_anchorEcho = root.Element("AnchorEcho")?.Value ?? DefaultAnchorEcho;
		_collapseEcho = root.Element("CollapseEcho")?.Value ?? DefaultCollapseEcho;
		_backlashEcho = root.Element("BacklashEcho")?.Value ?? string.Empty;
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
		SortOrder = _sortOrder
	};

	public XElement SaveToXml()
	{
		return new XElement("Effect",
			new XAttribute("type", "astralprojection"),
			new XElement("FormKey", FormKey),
			new XElement("Race", _race.Id),
			new XElement("Ethnicity", _ethnicity?.Id ?? 0L),
			new XElement("Gender", _gender.HasValue ? (int)_gender.Value : -1),
			new XElement("Alias", _alias ?? string.Empty),
			new XElement("SortOrder", _sortOrder?.ToString() ?? string.Empty),
			new XElement("Plane", _planeId),
			new XElement("AnchorPolicy", _anchorPolicy),
			new XElement("ProjectionEcho", new XCData(_projectionEcho)),
			new XElement("AnchorEcho", new XCData(_anchorEcho)),
			new XElement("CollapseEcho", new XCData(_collapseEcho)),
			new XElement("BacklashEcho", new XCData(_backlashEcho))
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
		var anchor = recipient.Identity.PrimaryInstance as ICharacter ?? recipient;
		if (anchor.InstanceId != recipient.InstanceId)
		{
			caster.OutputHandler.Send("You must be focused on your primary body to cast that spell.");
			return null;
		}

		if (!anchor.IsPlayerCharacter || anchor.IsGuest)
		{
			caster.OutputHandler.Send("Only player characters can use astral projection.");
			return null;
		}

		if (anchor.State.HasFlag(CharacterState.Dead) || anchor.State.HasFlag(CharacterState.Stasis))
		{
			caster.OutputHandler.Send("You cannot project while dead or in stasis.");
			return null;
		}

		if (anchor.Location is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not project character #{anchor.Id.ToString("N0")}: the character has no location.",
				true
			);
			return null;
		}

		if (anchor.AffectedBy<IAstralProjectionEffect>(x => x.AnchorInstanceId == anchor.InstanceId))
		{
			caster.OutputHandler.Send("You are already astrally projected.");
			return null;
		}

		if (!anchor.EnsureForm(FormSpecification,
			    new CharacterFormSource(CharacterFormSourceType.SpellEffect, Spell.Id, FormKey),
			    out var form,
			    out var whyNot))
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not ensure astral projection form '{FormKey}' for character #{anchor.Id.ToString("N0")}: {whyNot}",
				true
			);
			return null;
		}

		var result = CharacterInstanceService.SpawnSecondaryInstance(
			CharacterInstanceService.CreateAstralProjectionSpawnOptions(
				anchor,
				form,
				anchor.Location,
				anchor.RoomLayer,
				EffectivePlaneId(),
				_anchorPolicy,
				Spell.Id,
				FormKey));
		if (!result.Success || result.Instance is null)
		{
			Gameworld.SystemMessage(
				$"Spell #{Spell.Id.ToString("N0")} could not spawn astral projection for character #{anchor.Id.ToString("N0")}: {result.Message}",
				true
			);
			caster.OutputHandler.Send("The projection fails to take shape.");
			return null;
		}

		return new SpellAstralProjectionEffect(
			anchor,
			parent,
			FormKey,
			result.Instance.InstanceId,
			result.Instance.Body.Id,
			anchor.InstanceId,
			EffectivePlaneId(),
			_anchorPolicy,
			_projectionEcho,
			_anchorEcho,
			_collapseEcho,
			_backlashEcho);
	}

	public IMagicSpellEffectTemplate Clone()
	{
		return new AstralProjectionSpellEffect(SaveToXml(), Spell);
	}

	public const string HelpText = @"You can use the following options with this effect:

	#3formkey <text>#0 - sets the stable key used to reuse the same provisioned astral form
	#3race <which>#0 - sets the race for the provisioned astral form
	#3ethnicity <which>|clear#0 - sets or clears the ethnicity override
	#3gender <which>|clear#0 - sets or clears the gender override
	#3alias <text>|clear#0 - sets or clears the initial alias
	#3sort <number>|clear#0 - sets or clears the initial sort order
	#3plane <which>#0 - sets the plane where the projection is present
	#3anchorpolicy <helpless|sleep|stasis|none>#0 - sets what happens to the primary body while projected
	#3projectionecho <text>|default|none#0 - sets the private echo sent to the projection
	#3anchorecho <text>|default|none#0 - sets the private echo sent to the primary body
	#3collapseecho <text>|default|none#0 - sets the focus-return echo on collapse
	#3backlashecho <text>|default|none#0 - sets optional private backlash text";

	public string Show(ICharacter actor)
	{
		var plane = Gameworld.Planes.Get(EffectivePlaneId());
		return
			$"AstralProjection [{FormKey.ColourCommand()}] Race {_race.Name.ColourName()}, Ethnicity {_ethnicity?.Name.ColourName() ?? "Auto".ColourValue()}, Gender {_gender?.DescribeEnum().ColourValue() ?? "Auto".ColourValue()}, Alias {(_alias ?? "auto").ColourCommand()}, Plane {(plane?.Name ?? $"#{EffectivePlaneId().ToString("N0", actor)}").ColourName()}, Anchor {_anchorPolicy.DescribeEnum().ColourValue()}, ProjectionEcho {DescribeEcho(_projectionEcho, DefaultProjectionEcho)}, AnchorEcho {DescribeEcho(_anchorEcho, DefaultAnchorEcho)}, CollapseEcho {DescribeEcho(_collapseEcho, DefaultCollapseEcho)}, Backlash {DescribeEcho(_backlashEcho, string.Empty)}";
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
			case "plane":
				return BuildingCommandPlane(actor, command);
			case "anchor":
			case "anchorpolicy":
				return BuildingCommandAnchorPolicy(actor, command);
			case "projectionecho":
			case "projection":
				return BuildingCommandEcho(actor, command, "projection", DefaultProjectionEcho,
					value => _projectionEcho = value);
			case "anchorecho":
			case "anchorbodyecho":
				return BuildingCommandEcho(actor, command, "anchor", DefaultAnchorEcho,
					value => _anchorEcho = value);
			case "collapseecho":
			case "collapse":
				return BuildingCommandEcho(actor, command, "collapse", DefaultCollapseEcho,
					value => _collapseEcho = value);
			case "backlashecho":
			case "backlash":
				return BuildingCommandEcho(actor, command, "backlash", string.Empty,
					value => _backlashEcho = value);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	public static PlanarPresenceDefinition CreateAstralProjectionPlanarPresence(IFuturemud gameworld, long planeId)
	{
		var astralPlaneId = planeId > 0
			? planeId
			: DefaultAstralPlane(gameworld)?.Id ?? gameworld.DefaultPlane?.Id ?? 1L;
		var defaultPlaneId = gameworld.DefaultPlane?.Id ?? astralPlaneId;
		var perceives = new[] { astralPlaneId, defaultPlaneId }.Distinct().ToArray();
		var astralOnly = new[] { astralPlaneId };
		var interactions = Enum.GetValues<PlanarInteractionKind>()
		                       .ToDictionary(
			                       x => x,
			                       x => x is PlanarInteractionKind.Observe or PlanarInteractionKind.Hear or
				                       PlanarInteractionKind.Speak or PlanarInteractionKind.Magic
				                       ? (IEnumerable<long>)astralOnly
				                       : Array.Empty<long>());

		return new PlanarPresenceDefinition(
			astralOnly,
			astralOnly,
			perceives,
			interactions,
			true,
			false,
			true,
			false,
			true,
			false,
			false,
			AstralProjectionTransitionProfile);
	}

	private long EffectivePlaneId()
	{
		return _planeId > 0 ? _planeId : DefaultAstralPlane(Gameworld)?.Id ?? Gameworld.DefaultPlane?.Id ?? 1L;
	}

	private static IPlane? DefaultAstralPlane(IFuturemud gameworld)
	{
		return gameworld.Planes.GetByIdOrName("Astral Plane") ??
		       gameworld.DefaultPlane ??
		       gameworld.Planes.FirstOrDefault();
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
		actor.OutputHandler.Send($"This effect will now provision a {_race.Name.ColourName()} astral form.");
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

	private bool BuildingCommandPlane(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which plane should the astral projection inhabit?");
			return false;
		}

		var plane = Gameworld.Planes.GetByIdOrName(command.SafeRemainingArgument);
		if (plane is null)
		{
			actor.OutputHandler.Send("There is no such plane.");
			return false;
		}

		_planeId = plane.Id;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The projection will now inhabit {plane.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandAnchorPolicy(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify helpless, sleep, stasis or none.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<AstralProjectionAnchorPolicy>(out var policy))
		{
			actor.OutputHandler.Send("You must specify helpless, sleep, stasis or none.");
			return false;
		}

		_anchorPolicy = policy;
		Spell.Changed = true;
		actor.OutputHandler.Send($"The anchor body policy is now {_anchorPolicy.DescribeEnum().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command, string label, string defaultEcho,
		Action<string> setter)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What {label} echo should this effect use, or should it be defaulted or suppressed?");
			return false;
		}

		var echo = BodyBackupEffect.NormaliseEcho(command.SafeRemainingArgument, defaultEcho);
		setter(echo);
		Spell.Changed = true;
		actor.OutputHandler.Send(echo switch
		{
			"" => $"This effect will now suppress {label} echoes.",
			_ when echo == defaultEcho => $"This effect will now use the default {label} echo.",
			_ => $"This effect will now use {echo.ColourCommand()} as its {label} echo."
		});
		return true;
	}

	private static string DescribeEcho(string echo, string defaultEcho)
	{
		return echo switch
		{
			"" => "Suppressed".ColourError(),
			_ when echo == defaultEcho => "Default".ColourValue(),
			_ => echo.ColourCommand()
		};
	}
}
