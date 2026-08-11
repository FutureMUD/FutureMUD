#nullable enable

using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;
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
		sb.AppendLine($"Disarm: {DisarmPolicy.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Lifecycle: {LifecyclePolicy.DescribeEnum().ColourValue()}{(Lifespan.HasValue ? $" ({Lifespan.Value.Describe(actor)})" : string.Empty)}");
		sb.AppendLine();
		sb.AppendLine("Triggers:");
		for (var index = 0; index < _triggers.Count; index++)
		{
			var trigger = _triggers[index];
			sb.AppendLine($"\t{(index + 1).ToString("N0", actor).ColourValue()}. {trigger.TriggerType.DescribeEnum().ColourName()}");
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
			case "charges":
				return BuildingCommandCharges(actor, command);
			case "cooldown":
				return BuildingCommandCooldown(actor, command);
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

		if (_triggers.Any(x => !x.CompatibleSourceKinds.Contains(sourceKind)) || _payloads.Any(x => !x.CompatibleSourceKinds.Contains(sourceKind)))
		{
			actor.Send("That source kind is incompatible with one or more existing modules. Remove or replace them first.");
			return false;
		}

		SourceKind = sourceKind;
		Changed = true;
		actor.Send($"This is now a {sourceKind.DescribeEnum().ColourValue()} trap template.");
		return true;
	}

	private bool BuildingCommandTrigger(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Use trigger add <type>, trigger remove <number>, or trigger <number> parameter <name> <value>.");
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

		if (!int.TryParse(command.PopSpeech(), out var triggerIndex) || triggerIndex < 1 || triggerIndex > _triggers.Count || command.PopForSwitch() is not ("parameter" or "param"))
		{
			actor.Send("Use trigger <number> parameter <name> <value>.");
			return false;
		}

		if (_triggers[triggerIndex - 1] is not TrapTriggerDefinition triggerDefinition || command.IsFinished)
		{
			actor.Send("You must specify a parameter name and value.");
			return false;
		}

		var parameterName = command.PopSpeech();
		triggerDefinition.SetParameter(parameterName, command.SafeRemainingArgument);
		Changed = true;
		actor.Send($"You set the {parameterName.ColourName()} parameter on that trigger.");
		return true;
	}

	private bool BuildingCommandPayload(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Use payload add <type>, payload remove <number>, payload <number> delay <time>, payload <number> target <selector>, or payload <number> parameter <name> <value>.");
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

		switch (command.PopForSwitch())
		{
			case "delay" when TimeSpan.TryParse(command.SafeRemainingArgument, out var delay) && delay >= TimeSpan.Zero:
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
					actor.Send("You must specify a parameter name and value.");
					return false;
				}
				var parameterName = command.PopSpeech();
				payloadDefinition.SetParameter(parameterName, command.SafeRemainingArgument);
				Changed = true;
				actor.Send($"You set the {parameterName.ColourName()} parameter on that payload.");
				return true;
			default:
				actor.Send("Use payload <number> delay <time>, target <selector>, or parameter <name> <value>.");
				return false;
		}
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
		if (!TimeSpan.TryParse(command.SafeRemainingArgument, out var cooldown) || cooldown < TimeSpan.Zero)
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
			if (!TimeSpan.TryParse(command.SafeRemainingArgument, out var parsedLifespan) || parsedLifespan <= TimeSpan.Zero)
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
		Lifespan = TimeSpan.TryParse(root.Attribute("lifespan")?.Value, out var lifespan) ? lifespan : null;
		_triggers.AddRange(root.Element("Triggers")?.Elements("Trigger").Select(TrapTriggerDefinition.LoadFromXml) ?? Enumerable.Empty<TrapTriggerDefinition>());
		_payloads.AddRange(root.Element("Payloads")?.Elements("Payload").Select(TrapPayloadDefinition.LoadFromXml) ?? Enumerable.Empty<TrapPayloadDefinition>());
	}

	private XElement SaveDefinition()
	{
		return new XElement("TrapTemplate",
			new XAttribute("source", SourceKind),
			new XAttribute("disarm", DisarmPolicy),
			new XAttribute("lifecycle", LifecyclePolicy),
			new XAttribute("charges", Charges),
			new XAttribute("cooldown", Cooldown.ToString("c")),
			Lifespan.HasValue ? new XAttribute("lifespan", Lifespan.Value.ToString("c")) : null,
			new XElement("Triggers", _triggers.Select(x => XElement.Parse(x.SaveToXml()))),
			new XElement("Payloads", _payloads.Select(x => XElement.Parse(x.SaveToXml()))));
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
	#3charges <number>#0 - sets the number of activations
	#3cooldown <timespan>#0 - sets the delay before rearming
	#3disarm <policy>#0 - sets disarm behaviour
	#3lifecycle <policy> [lifespan]#0 - sets expiry behaviour
	#3validate#0 - reports whether the template can be submitted

	Common trigger parameters are #3chance#0, #3spotdifficulty#0, #3avoiddifficulty#0, #3filterprog#0 and #3triggerEcho#0.
	Common payload parameters are #3echo#0, #3spell#0, #3targetitem#0, #3prog#0, #3damage#0, #3damagetype#0, #3liquid#0, #3gas#0, #3amount#0, #3dose#0 and #3duration#0.";
}
