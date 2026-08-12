#nullable enable

using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using MudSharp.Traps;
using ModelTrapTemplate = MudSharp.Models.TrapTemplate;

namespace MudSharp.Traps;

/// <summary>
/// A revisable definition for a trap family. Its XML deliberately stores modules rather than a hard-coded
/// item, spell or NPC schema so all three deployment domains use the same configuration surface.
/// </summary>
public sealed class TrapTemplate : EditableItem, ITrapTemplate
{
	private readonly List<ITrapTrigger> _triggers = [];
	private readonly List<ITrapPayload> _payloads = [];
	private readonly List<ITrapComponentRequirement> _componentRequirements = [];

	public TrapTemplate(ModelTrapTemplate template, IFuturemud gameworld) : base(template.EditableItem)
	{
		Gameworld = gameworld;
		_id = template.Id;
		_name = template.Name;
		LoadDefinition(template.Definition);
	}

	public TrapTemplate(IAccount originator) : base(originator)
	{
		Gameworld = originator.Gameworld;
		SourceKind = TrapSourceKind.Mechanical;
		DisarmPolicy = TrapDisarmPolicy.Risky;
		LifecyclePolicy = TrapLifecyclePolicy.Indefinite;
		Charges = 1;
		Cooldown = TimeSpan.Zero;
		SetupTime = TimeSpan.FromSeconds(10);
		DisarmTime = TimeSpan.FromSeconds(10);
		RecoveryTime = TimeSpan.FromSeconds(5);
		_name = "Unnamed Trap Template";

		using (new FMDB())
		{
			var dbitem = new ModelTrapTemplate
			{
				Id = Gameworld.TrapTemplates.NextID(),
				RevisionNumber = RevisionNumber,
				Name = Name,
				Definition = SaveDefinition().ToString()
			};
			var editable = new MudSharp.Models.EditableItem
			{
				BuilderAccountId = BuilderAccountID,
				BuilderDate = BuilderDate,
				RevisionStatus = (int)Status,
				RevisionNumber = RevisionNumber
			};
			dbitem.EditableItem = editable;
			FMDB.Context.TrapTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "TrapTemplate";
	public TrapSourceKind SourceKind { get; private set; }
	public IReadOnlyList<ITrapTrigger> Triggers => _triggers;
	public IReadOnlyList<ITrapPayload> Payloads => _payloads;
	public TrapDisarmPolicy DisarmPolicy { get; private set; }
	public TrapLifecyclePolicy LifecyclePolicy { get; private set; }
	public int Charges { get; private set; }
	public TimeSpan Cooldown { get; private set; }
	public TimeSpan? Lifespan { get; private set; }
	public TimeSpan SetupTime { get; private set; }
	public TimeSpan DisarmTime { get; private set; }
	public TimeSpan RecoveryTime { get; private set; }
	public IFutureProg? KnowledgeProg => _knowledgeProgId.HasValue
		? Gameworld.FutureProgs.Get(_knowledgeProgId.Value)
		: null;
	private long? _knowledgeProgId;
	public IReadOnlyList<ITrapComponentRequirement> ComponentRequirements => _componentRequirements;

	public override string EditHeader() => $"Trap Template {Name} ({Id:N0}r{RevisionNumber:N0})";

	public override bool CanSubmit() => string.IsNullOrEmpty(WhyCannotSubmit());

	public override string WhyCannotSubmit()
	{
		if (!_triggers.Any())
		{
			return "A trap template requires at least one trigger.";
		}

		if (!_payloads.Any())
		{
			return "A trap template requires at least one payload.";
		}

		var incompatibleTrigger = _triggers.FirstOrDefault(x => !x.CompatibleSourceKinds.Contains(SourceKind));
		if (incompatibleTrigger is not null)
		{
			return $"{incompatibleTrigger.TriggerType.DescribeEnum()} triggers are not compatible with {SourceKind.DescribeEnum()} traps.";
		}

		var incompatiblePayload = _payloads.FirstOrDefault(x => !x.CompatibleSourceKinds.Contains(SourceKind));
		if (incompatiblePayload is not null)
		{
			return $"{incompatiblePayload.PayloadType.DescribeEnum()} payloads are not compatible with {SourceKind.DescribeEnum()} traps.";
		}

		if (Charges < 1)
		{
			return "A trap template must have at least one charge.";
		}

		if (Cooldown < TimeSpan.Zero)
		{
			return "A trap template cannot have a negative cooldown.";
		}

		if (SetupTime < TimeSpan.Zero || DisarmTime < TimeSpan.Zero || RecoveryTime < TimeSpan.Zero)
		{
			return "Trap interaction times cannot be negative.";
		}

		if (SourceKind == TrapSourceKind.Mechanical && SetupTime <= TimeSpan.Zero)
		{
			return "Mechanical traps require a positive setup time for player deployment.";
		}

		if (SourceKind == TrapSourceKind.Mechanical &&
		    DisarmPolicy is TrapDisarmPolicy.Safe or TrapDisarmPolicy.Risky && DisarmTime <= TimeSpan.Zero)
		{
			return "Disarmable mechanical traps require a positive disarm time.";
		}

		if (SourceKind == TrapSourceKind.Mechanical && RecoveryTime <= TimeSpan.Zero)
		{
			return "Mechanical traps require a positive recovery time.";
		}

		if (SourceKind == TrapSourceKind.Mechanical &&
		    !_componentRequirements.Any(x => x.Role.HasFlag(TrapComponentRole.Trigger)))
		{
			return "Mechanical traps require at least one physical trigger component.";
		}

		if (SourceKind == TrapSourceKind.Mechanical &&
		    !_componentRequirements.Any(x => x.Role.HasFlag(TrapComponentRole.Payload)))
		{
			return "Mechanical traps require at least one physical payload component.";
		}

		if (SourceKind != TrapSourceKind.Mechanical && _componentRequirements.Any())
		{
			return "Only mechanical traps may require physical components.";
		}

		var invalidComponent = _componentRequirements.FirstOrDefault(x => x.Tag is null ||
			x.Role == TrapComponentRole.None || x.SpentRecoveryChance is < 0.0 or > 100.0 ||
			x.QualityWeight is < 0.0 or > 10.0);
		if (invalidComponent is not null)
		{
			return "Trap component requirements need a valid tag and role, a recovery chance from 0 to 100, and a quality weight from 0 to 10.";
		}

		if (_knowledgeProgId.HasValue && (KnowledgeProg is null ||
		    !KnowledgeProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
		    !KnowledgeProg.MatchesParameters([ProgVariableTypes.Character])))
		{
			return "The knowledge prog must be a boolean FutureProg accepting one character.";
		}

		if (LifecyclePolicy != TrapLifecyclePolicy.Indefinite && (!Lifespan.HasValue || Lifespan <= TimeSpan.Zero))
		{
			return $"{LifecyclePolicy.DescribeEnum()} traps require a positive lifespan.";
		}

		var invalidTrigger = _triggers
			.Select(ValidateTrigger)
			.FirstOrDefault(x => !string.IsNullOrEmpty(x));
		if (!string.IsNullOrEmpty(invalidTrigger))
		{
			return invalidTrigger;
		}

		var invalidPayload = _payloads
			.Select(ValidatePayload)
			.FirstOrDefault(x => !string.IsNullOrEmpty(x));
		if (!string.IsNullOrEmpty(invalidPayload))
		{
			return invalidPayload;
		}

		return string.Empty;
	}

	private string? ValidateTrigger(ITrapTrigger trigger)
	{
		if (trigger.Parameters.TryGetValue("chance", out var chance) &&
		    (!double.TryParse(chance, out var chanceValue) || chanceValue is < 0.0 or > 100.0))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger chance must be a percentage from 0 to 100.";
		}

		if (trigger.Parameters.TryGetValue("minimumvalue", out var minimumValue) &&
		    !double.TryParse(minimumValue, out _))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger minimumvalue must be numeric.";
		}

		if (trigger.Parameters.TryGetValue("maximumvalue", out var maximumValue) &&
		    !double.TryParse(maximumValue, out _))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger maximumvalue must be numeric.";
		}

		if (trigger.Parameters.TryGetValue("minimumvalue", out minimumValue) &&
		    trigger.Parameters.TryGetValue("maximumvalue", out maximumValue) &&
		    double.TryParse(minimumValue, out var minimum) && double.TryParse(maximumValue, out var maximum) &&
		    minimum > maximum)
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger minimumvalue cannot exceed maximumvalue.";
		}

		if (trigger.Parameters.TryGetValue("spotdifficulty", out var spotDifficulty) &&
		    !Enum.TryParse(spotDifficulty, true, out Difficulty _))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger spotdifficulty must be a difficulty.";
		}

		if (trigger.Parameters.TryGetValue("avoiddifficulty", out var avoidDifficulty) &&
		    !Enum.TryParse(avoidDifficulty, true, out Difficulty _))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger avoiddifficulty must be a difficulty.";
		}

		if (trigger.Parameters.TryGetValue("maximumproximity", out var maximumProximity) &&
		    !Enum.TryParse(maximumProximity, true, out Proximity _))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger maximumproximity must be a proximity.";
		}

		if (trigger.TriggerType == TrapTriggerType.ExitTraversal)
		{
			if (trigger.Parameters.TryGetValue("movementtypes", out var movementTypes) &&
			    !TryParseMovementTypes(movementTypes, out _))
			{
				return "Exit Traversal movementtypes must be a comma-separated list of movement types or All.";
			}

			if (trigger.Parameters.TryGetValue("minimumsize", out var minimumSize) &&
			    !minimumSize.TryParseEnum<SizeCategory>(out _))
			{
				return "Exit Traversal minimumsize must be a valid size category.";
			}

			if (trigger.Parameters.TryGetValue("maximumsize", out var maximumSize) &&
			    !maximumSize.TryParseEnum<SizeCategory>(out _))
			{
				return "Exit Traversal maximumsize must be a valid size category.";
			}

			if (trigger.Parameters.TryGetValue("minimumsize", out minimumSize) &&
			    trigger.Parameters.TryGetValue("maximumsize", out maximumSize) &&
			    minimumSize.TryParseEnum<SizeCategory>(out var minimumCategory) &&
			    maximumSize.TryParseEnum<SizeCategory>(out var maximumCategory) && minimumCategory > maximumCategory)
			{
				return "Exit Traversal minimumsize cannot exceed maximumsize.";
			}
		}

		if (!trigger.Parameters.TryGetValue("filterprog", out var filterProg))
		{
			return null;
		}

		if (!long.TryParse(filterProg, out var filterProgId))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger filterprog must be a FutureProg ID.";
		}

		var prog = Gameworld.FutureProgs.Get(filterProgId);
		if (prog is null || !prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
		{
			return $"{trigger.TriggerType.DescribeEnum()} trigger filterprog must refer to a boolean FutureProg.";
		}

		var supportedParameters = trigger.TriggerType == TrapTriggerType.Signal
			? prog.MatchesParameters([ProgVariableTypes.Perceivable]) ||
			  prog.MatchesParameters([ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable])
			: prog.MatchesParameters([ProgVariableTypes.Character]) ||
			  prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable]);
		return supportedParameters
			? null
			: $"{trigger.TriggerType.DescribeEnum()} trigger filterprog has no supported parameter signature.";
	}

	public static bool TryParseMovementTypes(string text, out MovementType movementTypes)
	{
		movementTypes = MovementType.None;
		foreach (var value in text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (!Enum.TryParse(value, true, out MovementType parsed) || parsed == MovementType.None)
			{
				return false;
			}

			movementTypes |= parsed;
		}

		return movementTypes != MovementType.None;
	}

	private string? ValidatePayload(ITrapPayload payload)
	{
		static bool RequiresPositiveNumber(IReadOnlyDictionary<string, string> parameters, string name) =>
			parameters.TryGetValue(name, out var value) && double.TryParse(value, out var number) && number > 0.0;

		static bool RequiresPositiveId(IReadOnlyDictionary<string, string> parameters, string name) =>
			parameters.TryGetValue(name, out var value) && long.TryParse(value, out var id) && id > 0L;

		if (payload.Delay < TimeSpan.Zero)
		{
			return $"{payload.PayloadType.DescribeEnum()} payload delay cannot be negative.";
		}

		if (payload.PayloadType == TrapPayloadType.CastSpell)
		{
			if (!RequiresPositiveId(payload.Parameters, "spell"))
			{
				return "Cast Spell payloads require a positive spell parameter.";
			}

			var spell = Gameworld.MagicSpells.Get(long.Parse(payload.Parameters["spell"]));
			return spell is null
				? "Cast Spell payload references an unknown spell."
				: !spell.ReadyForGame
					? "Cast Spell payload references a spell that is not ready for game."
					: null;
		}

		if (payload.PayloadType == TrapPayloadType.ExecuteProg)
		{
			if (!RequiresPositiveId(payload.Parameters, "prog"))
			{
				return "Execute Prog payloads require a positive prog parameter.";
			}

			var prog = Gameworld.FutureProgs.Get(long.Parse(payload.Parameters["prog"]));
			if (prog is null)
			{
				return "Execute Prog payload references an unknown FutureProg.";
			}

			return prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable]) ||
			       prog.MatchesParameters([ProgVariableTypes.Character]) ||
			       prog.MatchesParameters([ProgVariableTypes.Perceivable])
				? null
				: "Execute Prog payload FutureProg has no supported parameter signature.";
		}

		if (payload.PayloadType == TrapPayloadType.LiquidDischarge)
		{
			return !RequiresPositiveId(payload.Parameters, "liquid")
				? "Liquid Discharge payloads require a positive liquid parameter."
				: Gameworld.Liquids.Get(long.Parse(payload.Parameters["liquid"])) is null
					? "Liquid Discharge payload references an unknown liquid."
					: null;
		}

		if (payload.PayloadType == TrapPayloadType.GasCloud)
		{
			if (!RequiresPositiveId(payload.Parameters, "gas"))
			{
				return "Gas Cloud payloads require a positive gas parameter.";
			}

			var gas = Gameworld.Gases.Get(long.Parse(payload.Parameters["gas"]));
			if (gas is null)
			{
				return "Gas Cloud payload references an unknown gas.";
			}

			if (gas.Drug is not null && !gas.Drug.DrugVectors.HasFlag(DrugVector.Inhaled))
			{
				return "Gas Cloud payloads with a drug require a gas whose drug can be inhaled.";
			}

			if (payload.Parameters.TryGetValue("dose", out var dose) &&
			    (!double.TryParse(dose, out var doseValue) || doseValue <= 0.0))
			{
				return "Gas Cloud payload dose must be a positive number when supplied.";
			}

			if (payload.Parameters.TryGetValue("duration", out var duration) &&
			    (!TimeSpan.TryParse(duration, out var durationValue) || durationValue <= TimeSpan.Zero))
			{
				return "Gas Cloud payload duration must be a positive timespan when supplied.";
			}

			return null;
		}

		return payload.PayloadType switch
		{
			TrapPayloadType.EmitSignal when payload.Parameters.TryGetValue("targetitem", out var targetItem) &&
			                                  (!long.TryParse(targetItem, out var targetItemId) || targetItemId < 0L) =>
				"Emit Signal targetitem must be a non-negative item ID when supplied.",
			TrapPayloadType.DirectDamage when !RequiresPositiveNumber(payload.Parameters, "damage") =>
				"Direct Damage payloads require a positive damage parameter.",
			_ => null
		};
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Trap Template #{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)} - {Name.ColourName()} - {Status.DescribeColour()}");
		sb.AppendLine($"Source: {SourceKind.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Charges: {Charges.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Cooldown: {Cooldown.Describe(actor).ColourValue()}");
		sb.AppendLine($"Interaction Times: setup {SetupTime.Describe(actor).ColourValue()}, disarm {DisarmTime.Describe(actor).ColourValue()}, recover {RecoveryTime.Describe(actor).ColourValue()}");
		sb.AppendLine($"Known When: {(KnowledgeProg is null ? "everyone" : $"{KnowledgeProg.FunctionName} (#{KnowledgeProg.Id.ToString("N0", actor)})").ColourValue()}");
		sb.AppendLine($"Disarm: {DisarmPolicy.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Lifecycle: {LifecyclePolicy.DescribeEnum().ColourValue()}{(Lifespan.HasValue ? $" ({Lifespan.Value.Describe(actor)})" : string.Empty)}");
		if (_componentRequirements.Any())
		{
			sb.AppendLine("Physical Components:");
			for (var index = 0; index < _componentRequirements.Count; index++)
			{
				var requirement = _componentRequirements[index];
				sb.AppendLine($"\t{(index + 1).ToString("N0", actor).ColourValue()}. {requirement.Role.DescribeEnum().ColourName()}: {(requirement.Tag?.Name ?? $"missing tag #{requirement.TagId}").ColourValue()} - spent recovery {requirement.SpentRecoveryChance.ToString("N0", actor).ColourValue()}%, quality weight {requirement.QualityWeight.ToString("N2", actor).ColourValue()}");
			}
		}
		sb.AppendLine();
		sb.AppendLine("Triggers:");
		for (var index = 0; index < _triggers.Count; index++)
		{
			var trigger = _triggers[index];
			sb.AppendLine($"\t{(index + 1).ToString("N0", actor).ColourValue()}. {trigger.TriggerType.DescribeEnum().ColourName()} - {trigger.Parameters.Select(x => $"{x.Key}={x.Value}").ListToString()}");
		}

		sb.AppendLine("Payloads:");
		for (var index = 0; index < _payloads.Count; index++)
		{
			var payload = _payloads[index];
			sb.AppendLine($"\t{(index + 1).ToString("N0", actor).ColourValue()}. {payload.PayloadType.DescribeEnum().ColourName()} after {payload.Delay.Describe(actor).ColourValue()} targeting {payload.TargetSelector.DescribeEnum().ColourValue()}");
		}

		var readiness = WhyCannotSubmit();
		if (!string.IsNullOrEmpty(readiness))
		{
			sb.AppendLine();
			sb.AppendLine($"Not ready for review: {readiness.ColourError()}");
		}

		return sb.ToString();
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.TrapTemplates.Find(Id, RevisionNumber);
			if (dbitem is null)
			{
				return;
			}

			if (_statusChanged)
			{
				base.Save(dbitem.EditableItem);
			}

			dbitem.Name = Name;
			dbitem.Definition = SaveDefinition().ToString();
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		using (new FMDB())
		{
			var revision = FMDB.Context.TrapTemplates
				.Where(x => x.Id == Id)
				.Select(x => x.RevisionNumber)
				.AsEnumerable()
				.DefaultIfEmpty(0)
				.Max() + 1;
			var dbitem = new ModelTrapTemplate
			{
				Id = Id,
				RevisionNumber = revision,
				Name = Name,
				Definition = SaveDefinition().ToString(),
				EditableItem = new MudSharp.Models.EditableItem
				{
					BuilderAccountId = initiator.Account.Id,
					BuilderDate = DateTime.UtcNow,
					RevisionNumber = revision,
					RevisionStatus = (int)RevisionStatus.UnderDesign
				}
			};
			FMDB.Context.TrapTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			return new TrapTemplate(dbitem, Gameworld);
		}
	}

	protected override IEnumerable<IEditableRevisableItem> GetAllSameId() => Gameworld.TrapTemplates.GetAll(Id);

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(BuildingHelpText.SubstituteANSIColour());
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "domain":
			case "source":
				return BuildingCommandSource(actor, command);
			case "trigger":
				return BuildingCommandTrigger(actor, command);
			case "payload":
				return BuildingCommandPayload(actor, command);
			case "component":
			case "components":
				return BuildingCommandComponent(actor, command);
			case "charges":
				return BuildingCommandCharges(actor, command);
			case "cooldown":
				return BuildingCommandCooldown(actor, command);
			case "setuptime":
			case "setup":
				return BuildingCommandTime(actor, command, "setup", value => SetupTime = value);
			case "disarmtime":
				return BuildingCommandTime(actor, command, "disarm", value => DisarmTime = value);
			case "recoverytime":
			case "recovertime":
				return BuildingCommandTime(actor, command, "recovery", value => RecoveryTime = value);
			case "knowledgeprog":
			case "knowprog":
				return BuildingCommandKnowledgeProg(actor, command);
			case "disarm":
				return BuildingCommandDisarm(actor, command);
			case "lifecycle":
				return BuildingCommandLifecycle(actor, command);
			case "validate":
				actor.Send(string.IsNullOrEmpty(WhyCannotSubmit()) ? "This trap template is ready for review.".ColourValue() : WhyCannotSubmit().ColourError());
				return false;
			default:
				actor.Send(BuildingHelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What name should this trap template have?");
			return false;
		}

		_name = command.SafeRemainingArgument.TitleCase().Trim();
		Changed = true;
		actor.Send($"This trap template is now named {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (!Enum.TryParse(command.PopSpeech(), true, out TrapSourceKind sourceKind))
		{
			actor.Send($"You must choose one of {Enum.GetValues<TrapSourceKind>().Select(x => x.DescribeEnum().ColourCommand()).ListToString()}.");
			return false;
		}

		if (_triggers.Any(x => !x.CompatibleSourceKinds.Contains(sourceKind)) || _payloads.Any(x => !x.CompatibleSourceKinds.Contains(sourceKind)) ||
		    sourceKind != TrapSourceKind.Mechanical && _componentRequirements.Any())
		{
			actor.Send("That source kind is incompatible with one or more existing modules. Remove or replace them first.");
			return false;
		}

		SourceKind = sourceKind;
		Changed = true;
		actor.Send($"This is now a {sourceKind.DescribeEnum().ColourValue()} trap template.");
		return true;
	}

	private bool BuildingCommandComponent(ICharacter actor, StringStack command)
	{
		const string syntax = "Use component add <trigger|payload|both> <tag> [spent recovery %] [quality weight], or component remove <number>.";
		if (SourceKind != TrapSourceKind.Mechanical)
		{
			actor.Send("Only mechanical trap templates use physical component requirements.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "remove":
			case "delete":
				if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _componentRequirements.Count)
				{
					actor.Send("There is no such component requirement.");
					return false;
				}
				_componentRequirements.RemoveAt(index - 1);
				Changed = true;
				actor.Send("You remove that physical component requirement.");
				return true;
			case "add":
			case "new":
				break;
			default:
				actor.Send(syntax);
				return false;
		}

		var roleText = command.PopSpeech();
		var role = roleText.CollapseString() switch
		{
			"trigger" => TrapComponentRole.Trigger,
			"payload" => TrapComponentRole.Payload,
			"both" or "triggerandpayload" => TrapComponentRole.TriggerAndPayload,
			_ => TrapComponentRole.None
		};
		if (role == TrapComponentRole.None || command.IsFinished)
		{
			actor.Send(syntax);
			return false;
		}

		var tag = Gameworld.Tags.GetByIdOrName(command.PopSpeech());
		if (tag is null)
		{
			actor.Send("There is no such item tag.");
			return false;
		}

		var recoveryChance = 75.0;
		var qualityWeight = 1.0;
		if (!command.IsFinished && (!double.TryParse(command.PopSpeech().TrimEnd('%'), actor, out recoveryChance) || recoveryChance is < 0.0 or > 100.0))
		{
			actor.Send("The spent recovery chance must be a percentage from 0 to 100.");
			return false;
		}
		if (!command.IsFinished && (!double.TryParse(command.PopSpeech(), actor, out qualityWeight) || qualityWeight is < 0.0 or > 10.0))
		{
			actor.Send("The quality weight must be a number from 0 to 10.");
			return false;
		}
		if (!command.IsFinished)
		{
			actor.Send(syntax);
			return false;
		}

		_componentRequirements.Add(new TrapComponentRequirementDefinition(Gameworld, tag.Id, role, recoveryChance, qualityWeight));
		Changed = true;
		actor.Send($"You add a {role.DescribeEnum().ColourName()} component requiring the {tag.Name.ColourName()} tag, with {recoveryChance.ToString("N0", actor).ColourValue()}% spent recovery and {qualityWeight.ToString("N2", actor).ColourValue()} quality weight.");
		return true;
	}

	private bool BuildingCommandTrigger(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(TriggerBuildingHelp(actor));
			return false;
		}

		if (command.PeekSpeech().ToLowerInvariant().CollapseString() is "add" or "new")
		{
			command.Pop();
			if (!Enum.TryParse(command.PopSpeech(), true, out TrapTriggerType triggerType))
			{
				actor.Send($"Choose one of {Enum.GetValues<TrapTriggerType>().Select(x => x.DescribeEnum().ColourCommand()).ListToString()}.");
				return false;
			}

			var trigger = new TrapTriggerDefinition(triggerType);
			if (!trigger.CompatibleSourceKinds.Contains(SourceKind))
			{
				actor.Send($"{triggerType.DescribeEnum()} triggers are not compatible with {SourceKind.DescribeEnum()} traps.");
				return false;
			}
			_triggers.Add(trigger);
			Changed = true;
			actor.Send($"You add a {triggerType.DescribeEnum().ColourName()} trigger.");
			return true;
		}

		if (command.PeekSpeech().ToLowerInvariant().CollapseString() is "remove" or "delete")
		{
			command.Pop();
			if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _triggers.Count)
			{
				actor.Send("There is no such trigger.");
				return false;
			}
			_triggers.RemoveAt(index - 1);
			Changed = true;
			actor.Send("You remove that trigger.");
			return true;
		}

		if (!int.TryParse(command.PopSpeech(), out var triggerIndex) || triggerIndex < 1 || triggerIndex > _triggers.Count)
		{
			actor.Send(TriggerBuildingHelp(actor));
			return false;
		}

		if (_triggers[triggerIndex - 1] is not TrapTriggerDefinition triggerDefinition)
		{
			actor.Send(TriggerBuildingHelp(actor, triggerIndex));
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send(TriggerBuildingHelp(actor, triggerIndex));
			return false;
		}

		if (command.PopForSwitch() is not ("parameter" or "param"))
		{
			actor.Send(TriggerBuildingHelp(actor, triggerIndex));
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send(TriggerBuildingHelp(actor, triggerIndex));
			return false;
		}

		var parameterName = command.PopSpeech();
		if (!TrapTriggerDefinition.IsSupportedParameter(triggerDefinition.TriggerType, parameterName) || command.IsFinished)
		{
			actor.Send(TriggerBuildingHelp(actor, triggerIndex));
			return false;
		}
		triggerDefinition.SetParameter(parameterName, command.SafeRemainingArgument);
		Changed = true;
		actor.Send($"You set the {parameterName.ColourName()} parameter on that trigger.");
		return true;
	}

	private string TriggerBuildingHelp(ICharacter actor, int? triggerIndex = null)
	{
		if (!triggerIndex.HasValue)
		{
			return "Use trigger add <type>, trigger remove <number>, trigger <number>, or trigger <number> parameter <name> <value>.\n" +
			       $"Trigger types are {Enum.GetValues<TrapTriggerType>().Select(x => x.DescribeEnum().ColourCommand()).ListToString()}.";
		}

		var trigger = _triggers[triggerIndex.Value - 1];
		var sb = new StringBuilder();
		sb.AppendLine($"Trigger {triggerIndex.Value.ToString("N0", actor).ColourValue()}: {trigger.TriggerType.DescribeEnum().ColourName()}");
		sb.AppendLine("Parameters:");
		foreach (var parameter in TrapTriggerDefinition.ParametersFor(trigger.TriggerType))
		{
			var value = trigger.Parameters.GetValueOrDefault(parameter.Name) ?? parameter.DefaultValue;
			sb.AppendLine($"\t{parameter.Name.ColourCommand()} = {value.ColourValue()} - {parameter.Description}");
		}
		sb.AppendLine();
		sb.AppendLine($"Use {"trigger <number> parameter <name> <value>".ColourCommand()} to change a value.");
		return sb.ToString();
	}

	private bool BuildingCommandTime(ICharacter actor, StringStack command, string name, Action<TimeSpan> setter)
	{
		if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value) || value < TimeSpan.Zero)
		{
			actor.Send($"You must specify a non-negative timespan for the {name} time.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.Send($"The {name} time is now {value.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandKnowledgeProg(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			_knowledgeProgId = null;
			Changed = true;
			actor.Send("Everyone will now know this trap template.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null || !prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) ||
		    !prog.MatchesParameters([ProgVariableTypes.Character]))
		{
			actor.Send("You must specify a boolean FutureProg accepting one character, or none.");
			return false;
		}

		_knowledgeProgId = prog.Id;
		Changed = true;
		actor.Send($"This trap is now known when {prog.MXPClickableFunctionName().ColourName()} returns true.");
		return true;
	}

	private bool BuildingCommandPayload(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Use payload add <type>, payload remove <number>, or payload <number> to inspect and configure a payload.");
			return false;
		}

		if (command.PeekSpeech().ToLowerInvariant().CollapseString() is "add" or "new")
		{
			command.Pop();
			if (!Enum.TryParse(command.PopSpeech(), true, out TrapPayloadType payloadType))
			{
				actor.Send($"Choose one of {Enum.GetValues<TrapPayloadType>().Select(x => x.DescribeEnum().ColourCommand()).ListToString()}.");
				return false;
			}
			var payload = new TrapPayloadDefinition(payloadType);
			if (!payload.CompatibleSourceKinds.Contains(SourceKind))
			{
				actor.Send($"{payloadType.DescribeEnum()} payloads are not compatible with {SourceKind.DescribeEnum()} traps.");
				return false;
			}
			_payloads.Add(payload);
			Changed = true;
			actor.Send($"You add a {payloadType.DescribeEnum().ColourName()} payload.");
			return true;
		}

		if (command.PeekSpeech().ToLowerInvariant().CollapseString() is "remove" or "delete")
		{
			command.Pop();
			if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > _payloads.Count)
			{
				actor.Send("There is no such payload.");
				return false;
			}
			_payloads.RemoveAt(index - 1);
			Changed = true;
			actor.Send("You remove that payload.");
			return true;
		}

		if (!int.TryParse(command.PopSpeech(), out var payloadIndex) || payloadIndex < 1 || payloadIndex > _payloads.Count || _payloads[payloadIndex - 1] is not TrapPayloadDefinition payloadDefinition)
		{
			actor.Send("There is no such payload.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send(PayloadBuildingHelp(actor, payloadIndex));
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "delay" when TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var delay) && delay >= TimeSpan.Zero:
				payloadDefinition.SetDelay(delay);
				Changed = true;
				actor.Send($"That payload will now wait {delay.Describe(actor).ColourValue()}.");
				return true;
			case "target" when Enum.TryParse(command.PopSpeech(), true, out TrapTargetSelector selector):
				payloadDefinition.SetTargetSelector(selector);
				Changed = true;
				actor.Send($"That payload will now target {selector.DescribeEnum().ColourValue()}.");
				return true;
			case "parameter":
				if (command.IsFinished)
				{
					actor.Send(PayloadBuildingHelp(actor, payloadIndex));
					return false;
				}
				var parameterName = command.PopSpeech();
				if (!TrapPayloadDefinition.IsSupportedParameter(payloadDefinition.PayloadType, parameterName) || command.IsFinished)
				{
					actor.Send(PayloadBuildingHelp(actor, payloadIndex));
					return false;
				}
				payloadDefinition.SetParameter(parameterName, command.SafeRemainingArgument);
				Changed = true;
				actor.Send($"You set the {parameterName.ColourName()} parameter on that payload.");
				return true;
			default:
				actor.Send(PayloadBuildingHelp(actor, payloadIndex));
				return false;
		}
	}

	private string PayloadBuildingHelp(ICharacter actor, int payloadIndex)
	{
		var payload = _payloads[payloadIndex - 1];
		var sb = new StringBuilder();
		sb.AppendLine($"Payload {payloadIndex.ToString("N0", actor).ColourValue()}: {payload.PayloadType.DescribeEnum().ColourName()}");
		sb.AppendLine($"Delay: {payload.Delay.Describe(actor).ColourValue()} - use {"payload <number> delay <timespan>".ColourCommand()} to change it.");
		sb.AppendLine($"Target: {payload.TargetSelector.DescribeEnum().ColourValue()} - valid selectors are {Enum.GetValues<TrapTargetSelector>().Select(x => x.DescribeEnum().ColourCommand()).ListToString()}. Use {"payload <number> target <selector>".ColourCommand()} to change it.");
		sb.AppendLine("Parameters:");
		foreach (var parameter in TrapPayloadDefinition.ParametersFor(payload.PayloadType))
		{
			var value = payload.Parameters.GetValueOrDefault(parameter.Name) ?? parameter.DefaultValue;
			sb.AppendLine($"\t{parameter.Name.ColourCommand()} = {value.ColourValue()} - {parameter.Description}");
		}
		sb.AppendLine();
		sb.AppendLine($"Use {"payload <number> parameter <name> <value>".ColourCommand()} to change a parameter.");
		return sb.ToString();
	}

	private bool BuildingCommandCharges(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.PopSpeech(), out var charges) || charges < 1)
		{
			actor.Send("You must specify at least one charge.");
			return false;
		}
		Charges = charges;
		Changed = true;
		actor.Send($"This trap template now has {charges.ToString("N0", actor).ColourValue()} charges.");
		return true;
	}

	private bool BuildingCommandCooldown(ICharacter actor, StringStack command)
	{
		if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var cooldown) || cooldown < TimeSpan.Zero)
		{
			actor.Send("You must specify a non-negative timespan.");
			return false;
		}
		Cooldown = cooldown;
		Changed = true;
		actor.Send($"This trap template now has a {cooldown.Describe(actor).ColourValue()} cooldown.");
		return true;
	}

	private bool BuildingCommandDisarm(ICharacter actor, StringStack command)
	{
		if (!Enum.TryParse(command.PopSpeech(), true, out TrapDisarmPolicy policy))
		{
			actor.Send($"Choose one of {Enum.GetValues<TrapDisarmPolicy>().Select(x => x.DescribeEnum().ColourCommand()).ListToString()}.");
			return false;
		}
		DisarmPolicy = policy;
		Changed = true;
		actor.Send($"This trap template now uses the {policy.DescribeEnum().ColourValue()} disarm policy.");
		return true;
	}

	private bool BuildingCommandLifecycle(ICharacter actor, StringStack command)
	{
		if (!Enum.TryParse(command.PopSpeech(), true, out TrapLifecyclePolicy policy))
		{
			actor.Send($"Choose one of {Enum.GetValues<TrapLifecyclePolicy>().Select(x => x.DescribeEnum().ColourCommand()).ListToString()}.");
			return false;
		}

		if (policy == TrapLifecyclePolicy.Indefinite && !command.IsFinished)
		{
			actor.Send("Indefinite traps cannot have a lifespan.");
			return false;
		}

		TimeSpan? lifespan = null;
		if (!command.IsFinished)
		{
			if (!TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var parsedLifespan) || parsedLifespan <= TimeSpan.Zero)
			{
				actor.Send("You must specify a positive lifespan.");
				return false;
			}
			lifespan = parsedLifespan;
		}
		if (policy != TrapLifecyclePolicy.Indefinite && !lifespan.HasValue)
		{
			actor.Send($"{policy.DescribeEnum()} traps require a lifespan.");
			return false;
		}
		LifecyclePolicy = policy;
		Lifespan = lifespan;
		Changed = true;
		actor.Send($"This trap template now uses the {policy.DescribeEnum().ColourValue()} lifecycle.");
		return true;
	}

	private void LoadDefinition(string definition)
	{
		var root = XElement.Parse(definition);
		SourceKind = Enum.TryParse(root.Attribute("source")?.Value, true, out TrapSourceKind sourceKind)
			? sourceKind
			: TrapSourceKind.Mechanical;
		DisarmPolicy = Enum.TryParse(root.Attribute("disarm")?.Value, true, out TrapDisarmPolicy disarmPolicy)
			? disarmPolicy
			: TrapDisarmPolicy.Risky;
		LifecyclePolicy = Enum.TryParse(root.Attribute("lifecycle")?.Value, true, out TrapLifecyclePolicy lifecyclePolicy)
			? lifecyclePolicy
			: TrapLifecyclePolicy.Indefinite;
		Charges = int.TryParse(root.Attribute("charges")?.Value, out var charges) ? Math.Max(1, charges) : 1;
		Cooldown = TimeSpan.TryParse(root.Attribute("cooldown")?.Value, out var cooldown) ? cooldown : TimeSpan.Zero;
		SetupTime = TimeSpan.TryParse(root.Attribute("setuptime")?.Value, out var setupTime) ? setupTime : TimeSpan.FromSeconds(10);
		DisarmTime = TimeSpan.TryParse(root.Attribute("disarmtime")?.Value, out var disarmTime) ? disarmTime : TimeSpan.FromSeconds(10);
		RecoveryTime = TimeSpan.TryParse(root.Attribute("recoverytime")?.Value, out var recoveryTime) ? recoveryTime : TimeSpan.FromSeconds(5);
		_knowledgeProgId = long.TryParse(root.Attribute("knowledgeprog")?.Value, out var knowledgeProgId) ? knowledgeProgId : null;
		Lifespan = TimeSpan.TryParse(root.Attribute("lifespan")?.Value, out var lifespan) ? lifespan : null;
		_triggers.AddRange(root.Element("Triggers")?.Elements("Trigger").Select(TrapTriggerDefinition.LoadFromXml) ?? Enumerable.Empty<TrapTriggerDefinition>());
		_payloads.AddRange(root.Element("Payloads")?.Elements("Payload").Select(TrapPayloadDefinition.LoadFromXml) ?? Enumerable.Empty<TrapPayloadDefinition>());
		_componentRequirements.AddRange(root.Element("Components")?.Elements("Component")
			.Select(x => TrapComponentRequirementDefinition.LoadFromXml(x, Gameworld)) ?? []);
	}

	private XElement SaveDefinition()
	{
		return new XElement("TrapTemplate",
			new XAttribute("source", SourceKind),
			new XAttribute("disarm", DisarmPolicy),
			new XAttribute("lifecycle", LifecyclePolicy),
			new XAttribute("charges", Charges),
			new XAttribute("cooldown", Cooldown.ToString("c")),
			new XAttribute("setuptime", SetupTime.ToString("c")),
			new XAttribute("disarmtime", DisarmTime.ToString("c")),
			new XAttribute("recoverytime", RecoveryTime.ToString("c")),
			_knowledgeProgId.HasValue ? new XAttribute("knowledgeprog", _knowledgeProgId.Value) : null,
			Lifespan.HasValue ? new XAttribute("lifespan", Lifespan.Value.ToString("c")) : null,
			new XElement("Triggers", _triggers.Select(x => XElement.Parse(x.SaveToXml()))),
			new XElement("Payloads", _payloads.Select(x => XElement.Parse(x.SaveToXml()))),
			new XElement("Components", _componentRequirements.Select(x => new XElement("Component",
				new XAttribute("tag", x.TagId),
				new XAttribute("role", x.Role),
				new XAttribute("recovery", x.SpentRecoveryChance),
				new XAttribute("qualityweight", x.QualityWeight)))));
	}

	private const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - renames this trap template
	#3domain <mechanical|magical|natural>#0 - sets the template source domain
	#3trigger add <type>#0 - adds an OR trigger
	#3trigger remove <number>#0 - removes a trigger
	#3trigger <number> parameter <name> <value>#0 - configures a trigger
	#3payload add <type>#0 - adds an ordered payload
	#3payload remove <number>#0 - removes a payload
	#3payload <number> delay <timespan>#0 - sets its delay
	#3payload <number> target <selector>#0 - sets its target selector
	#3payload <number> parameter <name> <value>#0 - configures a payload
	#3component add <trigger|payload|both> <tag> [spent recovery %] [quality weight]#0 - requires a tagged physical part
	#3component remove <number>#0 - removes a physical component requirement
	#3charges <number>#0 - sets the number of activations
	#3cooldown <timespan>#0 - sets the delay before rearming
	#3setuptime <timespan>#0 - sets the mundane trap laying time
	#3disarmtime <timespan>#0 - sets the mundane disarming time
	#3recoverytime <timespan>#0 - sets the mundane recovery time
	#3knowprog <prog|none>#0 - controls which characters know this template
	#3disarm <policy>#0 - sets disarm behaviour
	#3lifecycle <policy> [lifespan]#0 - sets expiry behaviour
	#3validate#0 - reports whether the template can be submitted

	Common trigger parameters are #3chance#0, #3spotdifficulty#0, #3avoiddifficulty#0, #3filterprog#0 and #3triggerEcho#0.
	Common payload parameters are #3echo#0, #3spell#0, #3targetitem#0, #3prog#0, #3damage#0, #3damagetype#0, #3liquid#0, #3gas#0, #3amount#0, #3dose#0 and #3duration#0.";
}
