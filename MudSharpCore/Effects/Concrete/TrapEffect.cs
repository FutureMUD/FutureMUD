#nullable enable

using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.Traps;

namespace MudSharp.Effects.Concrete;

/// <summary>
/// The persisted runtime trap. It is an effect on an item or cell instead of a new item type, so traps can be
/// physical, magical, or natural without duplicating the engine's item, spell, and world-object persistence paths.
/// </summary>
public sealed class TrapEffect : Effect, ITrap, IHandleEventsEffect
{
	private readonly List<ISignalSourceComponent> _signalSources = [];
	private IProximityEventRegistration? _proximityRegistration;

	public static void InitialiseEffectType()
	{
		RegisterFactory("Trap", (effect, owner) => new TrapEffect(effect, owner));
	}

	public TrapEffect(IPerceivable owner, ITrapTemplate template, ICharacter? creator = null)
		: base(owner)
	{
		InstanceId = Guid.NewGuid();
		TemplateId = template.Id;
		TemplateRevision = template.RevisionNumber;
		CreatorId = creator?.Id ?? 0L;
		ChargesRemaining = template.Charges;
		State = TrapState.Armed;
	}

	internal TrapEffect(IPerceivable owner, ITrapTemplate template, Guid instanceId, long creatorId)
		: base(owner)
	{
		InstanceId = instanceId;
		TemplateId = template.Id;
		TemplateRevision = template.RevisionNumber;
		CreatorId = creatorId;
		ChargesRemaining = 0;
		State = TrapState.Resolving;
	}

	private TrapEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		InstanceId = Guid.Parse(effect.Element("InstanceId")!.Value);
		TemplateId = long.Parse(effect.Element("TemplateId")!.Value);
		TemplateRevision = int.Parse(effect.Element("TemplateRevision")!.Value);
		CreatorId = long.Parse(effect.Element("CreatorId")?.Value ?? "0");
		ChargesRemaining = int.Parse(effect.Element("ChargesRemaining")?.Value ?? "0");
		State = Enum.TryParse(effect.Element("State")?.Value, true, out TrapState state)
			? state
			: TrapState.Spent;
	}

	public Guid InstanceId { get; }
	public long TemplateId { get; }
	public int TemplateRevision { get; }
	public int TemplateRevisionNumber => TemplateRevision;
	public long CreatorId { get; }
	public int ChargesRemaining { get; private set; }
	public int RemainingCharges => ChargesRemaining;
	public TrapState State { get; private set; }
	public ITrapTemplate? Template => Gameworld.TrapTemplates.Get(TemplateId, TemplateRevision);
	public TrapSourceKind SourceKind => Template?.SourceKind ?? TrapSourceKind.Mechanical;

	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "Trap";

	public override string Describe(IPerceiver voyeur)
	{
		var template = Template;
		return template is null
			? $"An orphaned trap instance ({InstanceId})."
			: $"{template.Name.ColourName()} ({State.DescribeEnum().ColourValue()}, {ChargesRemaining:N0} charges).";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("InstanceId", InstanceId),
			new XElement("TemplateId", TemplateId),
			new XElement("TemplateRevision", TemplateRevision),
			new XElement("CreatorId", CreatorId),
			new XElement("ChargesRemaining", ChargesRemaining),
			new XElement("State", State));
	}

	public override void InitialEffect()
	{
		base.InitialEffect();
		SubscribeSignalTriggers();
		SubscribeProximityTriggers();
	}

	public override void Login()
	{
		base.Login();
		SubscribeSignalTriggers();
		SubscribeProximityTriggers();
	}

	public override void RemovalEffect()
	{
		UnsubscribeSignalTriggers();
		UnsubscribeProximityTriggers();
		base.RemovalEffect();
	}

	public override void ExpireEffect()
	{
		if (State == TrapState.Armed && Template?.LifecyclePolicy == TrapLifecyclePolicy.Unstable)
		{
			ForceTrigger();
		}

		State = TrapState.Expired;
		Changed = true;
		base.ExpireEffect();
	}

	public static bool HasTimedLifetime(ITrapTemplate template)
	{
		return (template.LifecyclePolicy is TrapLifecyclePolicy.FixedExpiry or TrapLifecyclePolicy.Unstable) &&
		       template.Lifespan is { } lifespan && lifespan > TimeSpan.Zero;
	}

	/// <summary>
	/// Proximity is a relationship to a spatial object, not a cell-arrival shortcut. Existing cell effects are
	/// supported by the legacy branch in <see cref="MatchesEvent"/>, but new deployments must provide an anchor.
	/// </summary>
	public static bool IsValidAnchor(ITrapTemplate template, IPerceivable anchor)
	{
		return anchor is not ICell || template.Triggers.All(x => x.TriggerType != TrapTriggerType.Proximity);
	}

	public bool IsKnownBy(ICharacter character)
	{
		return character.IsAdministrator() ||
		       character.EffectsOfType<TrapKnowledgeEffect>().Any(x => x.TrapInstanceId == InstanceId);
	}

	public void MarkKnownBy(ICharacter character)
	{
		if (IsKnownBy(character))
		{
			return;
		}

		character.AddEffect(new TrapKnowledgeEffect(character, InstanceId, TemplateId, TemplateRevision));
	}

	public Difficulty SearchDifficulty => ParseDifficulty(
		Template?.Triggers.FirstOrDefault()?.Parameters ?? new Dictionary<string, string>(),
		"spotdifficulty",
		Difficulty.Hard);

	public bool Arm()
	{
		if (State is TrapState.Spent or TrapState.Expired || ChargesRemaining <= 0)
		{
			return false;
		}

		State = TrapState.Armed;
		Changed = true;
		return true;
	}

	public bool Disarm()
	{
		if (State is TrapState.Spent or TrapState.Expired or TrapState.Disarmed)
		{
			return false;
		}

		State = TrapState.Disarmed;
		Changed = true;
		return true;
	}

	public bool TriggerManually(ICharacter? triggerer = null)
	{
		return Trigger(triggerer, Template?.Triggers.FirstOrDefault(x => x.TriggerType == TrapTriggerType.Manual));
	}

	/// <summary>
	/// Forces the trap to resolve for staff tooling and deliberate FutureProg control. This is intentionally
	/// separate from <see cref="TriggerManually"/> because a template need not have a manual trigger module.
	/// </summary>
	public bool ForceTrigger(ICharacter? triggerer = null)
	{
		return Trigger(triggerer, null, true);
	}

	public bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (State != TrapState.Armed || Template is null)
		{
			return false;
		}

		ICharacter? triggerer = type switch
		{
			EventType.ItemOpened => arguments.Length > 0 ? arguments[0] as ICharacter : null,
			EventType.CharacterEnterCellWitness => arguments.Length > 0 ? arguments[0] as ICharacter : null,
			EventType.CharacterBeginMovementWitness => arguments.Length > 0 ? arguments[0] as ICharacter : null,
			EventType.TrapSignalReceived => arguments.Length > 0 ? arguments[0] as ICharacter : null,
			EventType.PerceivableProximityChanged => arguments.Length > 1 ? arguments[1] as ICharacter : null,
			_ => null
		};
		IPerceivable? triggerSource = type == EventType.TrapSignalReceived && arguments.Length > 2
			? arguments[2] as IPerceivable
			: triggerer;

		foreach (ITrapTrigger trigger in Template.Triggers)
		{
			if (!MatchesEvent(trigger, type, arguments))
			{
				continue;
			}

			if (Trigger(triggerer, trigger, false, triggerSource))
			{
				return true;
			}
		}

		return false;
	}

	public bool HandlesEvent(params EventType[] types)
	{
		return types.Any(x => x is EventType.ItemOpened or EventType.CharacterEnterCellWitness or
			EventType.CharacterBeginMovementWitness or EventType.TrapSignalReceived or EventType.PerceivableProximityChanged);
	}

	internal void ExecutePayload(int payloadIndex, long targetCharacterId)
	{
		var template = Template;
		if (template is null || payloadIndex < 0 || payloadIndex >= template.Payloads.Count)
		{
			return;
		}

		var target = targetCharacterId > 0 ? Gameworld.TryGetCharacter(targetCharacterId, true) : null;
		if (targetCharacterId > 0 && target is null)
		{
			return;
		}

		ExecutePayload(template.Payloads[payloadIndex], target);
	}

	private bool Trigger(ICharacter? triggerer, ITrapTrigger? trigger, bool force = false,
		IPerceivable? triggerSource = null)
	{
		var template = Template;
		if (template is null || (!force && trigger is null) || State != TrapState.Armed || ChargesRemaining <= 0)
		{
			return false;
		}

		if (!force)
		{
			var activeTrigger = trigger!;
			if (!PassesTriggerFilter(activeTrigger, triggerer, triggerSource) || !PassesChance(activeTrigger.Parameters))
			{
				return false;
			}
		}

		if (triggerer is not null)
		{
			if (!force && triggerer.IsAdministrator())
			{
				return false;
			}

			if (!force && IsKnownBy(triggerer) && AttemptsAvoidance(triggerer, trigger!))
			{
				return false;
			}

			if (!force)
			{
				TrySpot(triggerer, trigger!);
			}
		}

		State = TrapState.Resolving;
		Changed = true;
		if (trigger is not null)
		{
			SendEcho(trigger.Parameters, "triggerEcho", triggerer);
		}

		foreach ((ITrapPayload payload, int index) in template.Payloads.Select((payload, index) => (payload, index)))
		{
			if (payload.PayloadType is TrapPayloadType.DetonateItem or TrapPayloadType.EmitSignal or TrapPayloadType.GasCloud)
			{
				ScheduleOrExecutePayload(payload, index, null);
				continue;
			}

			var targets = ResolveTargets(payload, triggerer).ToList();
			if (!targets.Any() && payload.PayloadType == TrapPayloadType.ExecuteProg)
			{
				ScheduleOrExecutePayload(payload, index, null);
				continue;
			}

			foreach (ICharacter target in targets)
			{
				ScheduleOrExecutePayload(payload, index, target);
			}
		}

		ConsumeCharge(template);
		return true;
	}

	private bool MatchesEvent(ITrapTrigger trigger, EventType eventType, dynamic[] arguments)
	{
		switch (trigger.TriggerType)
		{
			case TrapTriggerType.Openable:
				return eventType == EventType.ItemOpened && Owner is IGameItem;

			case TrapTriggerType.CellEntry:
				if (!TrapEventRouting.IsCellArrivalWitness(eventType) || arguments.Length < 2 || arguments[1] is not ICell destination)
				{
					return false;
				}

				return AnchoredIn(destination, arguments[0] as ICharacter);

			case TrapTriggerType.Proximity:
				if (eventType == EventType.PerceivableProximityChanged && arguments.Length >= 5 &&
				    ReferenceEquals(arguments[0] as IPerceivable, Owner) && arguments[1] is ICharacter proximityTarget &&
				    arguments[2] is double previous && arguments[3] is double current)
				{
					var maximumProximity = ResolveMaximumProximity(trigger);
					return (Proximity)previous > maximumProximity && (Proximity)current <= maximumProximity;
				}

				// Existing cell-owned proximity traps pre-date spatial anchors. Retain their cell-entry behaviour while
				// preventing new trap placements from creating more of them.
				return Owner is ICell && TrapEventRouting.IsCellArrivalWitness(eventType) && arguments.Length >= 2 &&
				       arguments[1] is ICell proximityDestination && arguments[0] is ICharacter proximityCharacter &&
				       AnchoredIn(proximityDestination, proximityCharacter);

			case TrapTriggerType.ExitTraversal:
				if (eventType != EventType.CharacterBeginMovementWitness || arguments.Length < 2 || arguments[1] is not ICell origin)
				{
					return false;
				}

				return AnchoredIn(origin, arguments[0] as ICharacter);

			case TrapTriggerType.Signal:
				if (eventType != EventType.TrapSignalReceived || arguments.Length < 2 || arguments[1] is not double value)
				{
					return false;
				}

				return PassesSignalRange(trigger.Parameters, value);

			default:
				return false;
		}
	}

	private bool AnchoredIn(ICell cell, ICharacter? mover)
	{
		if (ReferenceEquals(Owner, cell))
		{
			return true;
		}

		if (!ReferenceEquals(Owner.Location, cell))
		{
			return false;
		}

		if (mover is null)
		{
			return true;
		}

		return Owner.RoomLayer == mover.RoomLayer;
	}

	private bool PassesTriggerFilter(ITrapTrigger trigger, ICharacter? target, IPerceivable? source)
	{
		if (trigger.Parameters.TryGetValue("filterprog", out var progText) &&
		    long.TryParse(progText, out var progId))
		{
			var prog = Gameworld.FutureProgs.Get(progId);
			if (prog is null)
			{
				return false;
			}

			if (target is not null && prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable]))
			{
				return prog.ExecuteBool(target, Owner);
			}

			if (target is not null && prog.MatchesParameters([ProgVariableTypes.Character]))
			{
				return prog.ExecuteBool(target);
			}

			if (source is not null && prog.MatchesParameters([ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable]))
			{
				return prog.ExecuteBool(source, Owner);
			}

			if (source is not null && prog.MatchesParameters([ProgVariableTypes.Perceivable]))
			{
				return prog.ExecuteBool(source);
			}

			return false;
		}

		return true;
	}

	private static bool PassesSignalRange(IReadOnlyDictionary<string, string> parameters, double value)
	{
		if (parameters.TryGetValue("minimumvalue", out var minimumText) &&
		    double.TryParse(minimumText, out var minimum) && value < minimum)
		{
			return false;
		}

		return !parameters.TryGetValue("maximumvalue", out var maximumText) ||
		       !double.TryParse(maximumText, out var maximum) || value <= maximum;
	}

	private bool PassesChance(IReadOnlyDictionary<string, string> parameters)
	{
		if (!parameters.TryGetValue("chance", out var chanceText) ||
		    !double.TryParse(chanceText, out var chance))
		{
			return true;
		}

		return RandomUtilities.DoubleRandom(0.0, 100.0) <= Math.Clamp(chance, 0.0, 100.0);
	}

	private bool AttemptsAvoidance(ICharacter target, ITrapTrigger trigger)
	{
		var difficulty = ParseDifficulty(trigger.Parameters, "avoiddifficulty", Difficulty.Normal);
		var outcome = Gameworld.GetCheck(CheckType.AvoidTrapCheck).Check(target, difficulty, target);
		return outcome.Outcome.IsPass();
	}

	private void TrySpot(ICharacter target, ITrapTrigger trigger)
	{
		if (IsKnownBy(target))
		{
			return;
		}

		var difficulty = ParseDifficulty(trigger.Parameters, "spotdifficulty", Difficulty.Hard);
		var outcome = Gameworld.GetCheck(CheckType.SpotTrapCheck).Check(target, difficulty, target);
		if (outcome.Outcome.IsPass())
		{
			MarkKnownBy(target);
			target.OutputHandler.Send("You notice the signs of a trap just as it is triggered.");
		}
	}

	private void ScheduleOrExecutePayload(ITrapPayload payload, int index, ICharacter? target)
	{
		if (payload.Delay > TimeSpan.Zero)
		{
			Owner.AddEffect(
				new TrapPayloadScheduleEffect(
					Owner,
					InstanceId,
					TemplateId,
					TemplateRevision,
					CreatorId,
					index,
					target?.Id ?? 0L),
				payload.Delay);
			return;
		}

		ExecutePayload(payload, target);
	}

	private IEnumerable<ICharacter> ResolveTargets(ITrapPayload payload, ICharacter? triggerer)
	{
		if (triggerer is null)
		{
			var anchorCell = Owner as ICell ?? Owner.Location;
			var occupants = anchorCell?.LayerCharacters(Owner.RoomLayer) ?? Enumerable.Empty<ICharacter>();
			return payload.TargetSelector switch
			{
				TrapTargetSelector.AnchorOccupants or TrapTargetSelector.SnapshotTarget => occupants,
				_ => Enumerable.Empty<ICharacter>()
			};
		}

		return payload.TargetSelector switch
		{
			TrapTargetSelector.Triggerer => [triggerer],
			TrapTargetSelector.AnchorOccupants => (Owner as ICell ?? Owner.Location)?.LayerCharacters(triggerer.RoomLayer)
				?? Enumerable.Empty<ICharacter>(),
			TrapTargetSelector.SnapshotTarget => ((Owner as ICell ?? Owner.Location)?.LayerCharacters(triggerer.RoomLayer)
				?? Enumerable.Empty<ICharacter>()).Where(x => !ReferenceEquals(x, triggerer)),
			_ => [triggerer]
		};
	}

	private void ConsumeCharge(ITrapTemplate template)
	{
		ChargesRemaining = Math.Max(0, ChargesRemaining - 1);
		if (ChargesRemaining <= 0)
		{
			State = TrapState.Spent;
			Changed = true;
			return;
		}

		if (template.Cooldown > TimeSpan.Zero)
		{
			State = TrapState.CoolingDown;
			Owner.AddEffect(new TrapResetEffect(Owner, InstanceId), template.Cooldown);
			Changed = true;
			return;
		}

		State = TrapState.Armed;
		Changed = true;
	}

	internal void ResetAfterCooldown()
	{
		if (State == TrapState.CoolingDown && ChargesRemaining > 0)
		{
			State = TrapState.Armed;
			Changed = true;
		}
	}

	private void ExecutePayload(ITrapPayload payload, ICharacter? target)
	{
		SendEcho(payload.Parameters, "echo", target);

		switch (payload.PayloadType)
		{
			case TrapPayloadType.DetonateItem:
				(Owner as IGameItem)?.GetItemType<IDetonatable>()?.Detonate();
				break;

			case TrapPayloadType.CastSpell:
				if (target is not null)
				{
					ExecuteSpellPayload(payload, target);
				}
				break;

			case TrapPayloadType.EmitSignal:
				ExecuteSignalPayload(payload);
				break;

			case TrapPayloadType.ExecuteProg:
				ExecuteProgPayload(payload, target);
				break;

			case TrapPayloadType.DirectDamage:
				if (target is not null)
				{
					ExecuteDamagePayload(payload, target);
				}
				break;

			case TrapPayloadType.LiquidDischarge:
				if (target is not null)
				{
					ExecuteLiquidPayload(payload, target);
				}
				break;

			case TrapPayloadType.GasCloud:
				ExecuteGasPayload(payload, target);
				break;

			case TrapPayloadType.Restraint:
				if (target is not null)
				{
					ExecuteRestraintPayload(payload, target);
				}
				break;
		}
	}

	private void ExecuteSpellPayload(ITrapPayload payload, ICharacter target)
	{
		if (!payload.Parameters.TryGetValue("spell", out var spellText) ||
		    !long.TryParse(spellText, out var spellId))
		{
			return;
		}

		var spell = Gameworld.MagicSpells.Get(spellId);
		var caster = CreatorId > 0 ? Gameworld.TryGetCharacter(CreatorId, true) : null;
		if (spell is not null && caster is not null)
		{
			var power = payload.Parameters.TryGetValue("power", out var powerText) &&
			            Enum.TryParse(powerText, true, out SpellPower parsedPower)
				? parsedPower
				: SpellPower.Standard;
			spell.ResolveTriggeredSpell(caster, target, power);
		}
	}

	private void ExecuteSignalPayload(ITrapPayload payload)
	{
		var targetItem = payload.Parameters.TryGetValue("targetitem", out var targetItemText) &&
		                 long.TryParse(targetItemText, out var targetItemId) &&
		                 targetItemId > 0L
			? Gameworld.TryGetItem(targetItemId, true)
			: Owner as IGameItem;
		if (targetItem is null)
		{
			return;
		}

		var value = payload.Parameters.TryGetValue("value", out var valueText) &&
		            double.TryParse(valueText, out var parsedValue)
			? parsedValue
			: 1.0;
		var signal = new ComputerSignal(value, null, null);
		var source = new TrapSignalSource(this, signal);
		foreach (ISignalSink sink in targetItem.Components.OfType<ISignalSink>())
		{
			sink.ReceiveSignal(signal, source);
		}
	}

	private void ExecuteProgPayload(ITrapPayload payload, ICharacter? target)
	{
		if (!payload.Parameters.TryGetValue("prog", out var progText) ||
		    !long.TryParse(progText, out var progId))
		{
			return;
		}

		var prog = Gameworld.FutureProgs.Get(progId);
		if (prog is null)
		{
			return;
		}

		if (target is not null && prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Perceivable]))
		{
			prog.Execute(target, Owner);
			return;
		}

		if (target is not null && prog.MatchesParameters([ProgVariableTypes.Character]))
		{
			prog.Execute(target);
			return;
		}

		if (prog.MatchesParameters([ProgVariableTypes.Perceivable]))
		{
			prog.Execute(Owner);
		}
	}

	private void ExecuteDamagePayload(ITrapPayload payload, ICharacter target)
	{
		var amount = payload.Parameters.TryGetValue("damage", out var amountText) &&
		             double.TryParse(amountText, out var parsedAmount)
			? Math.Max(0.0, parsedAmount)
			: 1.0;
		var damageType = payload.Parameters.TryGetValue("damagetype", out var damageTypeText) &&
		                 Enum.TryParse(damageTypeText, true, out DamageType parsedType)
			? parsedType
			: DamageType.Piercing;
		target.SufferDamage(new Damage
		{
			ActorOrigin = CreatorId > 0 ? Gameworld.TryGetCharacter(CreatorId, true) : null,
			ToolOrigin = Owner as IGameItem,
			Bodypart = target.Body.RandomBodypart,
			DamageAmount = amount,
			PainAmount = amount,
			StunAmount = amount,
			DamageType = damageType
		}).ProcessPassiveWounds();
		RecordHarmCrime(target);
	}

	private void ExecuteLiquidPayload(ITrapPayload payload, ICharacter target)
	{
		if (!payload.Parameters.TryGetValue("liquid", out var liquidText) ||
		    !long.TryParse(liquidText, out var liquidId))
		{
			return;
		}

		var liquid = Gameworld.Liquids.Get(liquidId);
		if (liquid is null)
		{
			return;
		}

		var amount = payload.Parameters.TryGetValue("amount", out var amountText) &&
		             double.TryParse(amountText, out var parsedAmount)
			? Math.Max(0.0, parsedAmount)
			: 0.1;
		var mixture = new LiquidMixture(liquid, amount, Gameworld);
		target.Body.ExposeToLiquid(mixture,
			target.Body.Limbs.GetRandomElement().Parts.OfType<IExternalBodypart>().FirstOrDefault(),
			LiquidExposureDirection.Irrelevant);
	}

	private void ExecuteGasPayload(ITrapPayload payload, ICharacter? target)
	{
		if (!payload.Parameters.TryGetValue("gas", out var gasText) ||
		    !long.TryParse(gasText, out var gasId))
		{
			return;
		}

		var gas = Gameworld.Gases.Get(gasId);
		var cell = Owner as ICell ?? Owner.Location;
		if (gas is null || cell is null)
		{
			return;
		}

		var duration = payload.Parameters.TryGetValue("duration", out var durationText) &&
		               TimeSpan.TryParse(durationText, out var parsedDuration) && parsedDuration > TimeSpan.Zero
			? parsedDuration
			: TimeSpan.FromSeconds(30);
		var dose = payload.Parameters.TryGetValue("dose", out var doseText) &&
		           double.TryParse(doseText, out var parsedDose)
			? Math.Max(0.0, parsedDose)
			: gas.DrugGramsPerUnitVolume;
		var echo = payload.Parameters.TryGetValue("cloudecho", out var cloudEcho)
			? cloudEcho
			: "A cloud of gas billows out.";
		cell.AddEffect(new TrapGasCloudEffect(cell, gas, dose, target?.RoomLayer ?? Owner.RoomLayer, echo), duration);
	}

	private void ExecuteRestraintPayload(ITrapPayload payload, ICharacter target)
	{
		var duration = payload.Parameters.TryGetValue("duration", out var durationText) &&
		               TimeSpan.TryParse(durationText, out var parsedDuration) && parsedDuration > TimeSpan.Zero
			? parsedDuration
			: TimeSpan.FromSeconds(30);
		var description = payload.Parameters.TryGetValue("description", out var descriptionText)
			? descriptionText
			: "caught by a trap";
		target.AddEffect(new TrapRestraintEffect(target, InstanceId, description), duration);
		RecordHarmCrime(target);
	}

	private void RecordHarmCrime(ICharacter target)
	{
		if (SourceKind == TrapSourceKind.Natural)
		{
			return;
		}

		var creator = CreatorId > 0 ? Gameworld.TryGetCharacter(CreatorId, true) : null;
		if (creator is null || creator.IsLawfulEnforcementActionAgainst(target, CrimeTypes.Assault))
		{
			return;
		}

		CrimeExtensions.CheckPossibleCrimeAllAuthorities(
			creator,
			CrimeTypes.Assault,
			target,
			Owner as IGameItem,
			Template?.Name ?? "trap");
	}

	private void SendEcho(IReadOnlyDictionary<string, string> parameters, string parameterName, ICharacter? target)
	{
		if (target is null || !parameters.TryGetValue(parameterName, out var echo) || string.IsNullOrWhiteSpace(echo))
		{
			return;
		}

		(target.Location ?? Owner.Location)?.HandleRoomEcho(new EmoteOutput(new Emote(echo, target, target)));
	}

	private static Difficulty ParseDifficulty(IReadOnlyDictionary<string, string> parameters, string parameterName,
		Difficulty fallback)
	{
		return parameters.TryGetValue(parameterName, out var text) &&
		       Enum.TryParse(text, true, out Difficulty difficulty)
			? difficulty
			: fallback;
	}

	private static Proximity ParseProximity(IReadOnlyDictionary<string, string> parameters, string parameterName,
		Proximity fallback)
	{
		return parameters.TryGetValue(parameterName, out var text) &&
		       Enum.TryParse(text, true, out Proximity proximity)
			? proximity
			: fallback;
	}

	private Proximity ResolveMaximumProximity(ITrapTrigger trigger)
	{
		if (trigger.Parameters.ContainsKey("maximumproximity"))
		{
			return ParseProximity(trigger.Parameters, "maximumproximity", Proximity.Distant);
		}

		var locationSource = Owner is IGameItem item ? item.LocationLevelPerceivable : Owner;
		return locationSource.Location?.RouteDefinition is null ? Proximity.Distant : Proximity.Immediate;
	}

	private void SubscribeProximityTriggers()
	{
		if (_proximityRegistration is not null || Owner is ICell || Template is null)
		{
			return;
		}

		var triggers = Template.Triggers.Where(x => x.TriggerType == TrapTriggerType.Proximity).ToList();
		if (triggers.Count == 0)
		{
			return;
		}

		_proximityRegistration = Gameworld.ProximityEventService.Register(Owner,
			triggers.Select(ResolveMaximumProximity).Max());
	}

	private void UnsubscribeProximityTriggers()
	{
		_proximityRegistration?.Dispose();
		_proximityRegistration = null;
	}

	private void SubscribeSignalTriggers()
	{
		if (_signalSources.Any() || Template?.Triggers.All(x => x.TriggerType != TrapTriggerType.Signal) != false ||
		    Owner is not IGameItem item)
		{
			return;
		}

		foreach (ISignalSourceComponent source in item.GetItemTypes<ISignalSourceComponent>())
		{
			source.SignalChanged += HandleSignal;
			_signalSources.Add(source);
		}
	}

	private void UnsubscribeSignalTriggers()
	{
		foreach (ISignalSourceComponent source in _signalSources)
		{
			source.SignalChanged -= HandleSignal;
		}

		_signalSources.Clear();
	}

	private void HandleSignal(ISignalSourceComponent source, ComputerSignal signal)
	{
		Owner.HandleEvent(EventType.TrapSignalReceived, Owner, signal.Value, source.Parent);
	}

	private sealed class TrapSignalSource(TrapEffect trap, ComputerSignal signal) : ISignalSource
	{
		public string Name => $"trap {trap.InstanceId}";
		public string EndpointKey => trap.InstanceId.ToString();
		public double CurrentValue => signal.Value;
		public TimeSpan? Duration => signal.Duration;
		public TimeSpan? PulseInterval => signal.PulseInterval;
	}
}

/// <summary>Persists a payload delay across reboot and resolves it against the original trap instance.</summary>
public sealed class TrapPayloadScheduleEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("TrapPayloadSchedule", (effect, owner) => new TrapPayloadScheduleEffect(effect, owner));
	}

	public TrapPayloadScheduleEffect(IPerceivable owner, Guid trapInstanceId, long templateId, int templateRevision,
		long creatorId, int payloadIndex, long targetCharacterId)
		: base(owner)
	{
		TrapInstanceId = trapInstanceId;
		TemplateId = templateId;
		TemplateRevision = templateRevision;
		CreatorId = creatorId;
		PayloadIndex = payloadIndex;
		TargetCharacterId = targetCharacterId;
	}

	private TrapPayloadScheduleEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		var effect = root.Element("Effect")!;
		TrapInstanceId = Guid.Parse(effect.Element("TrapInstanceId")!.Value);
		TemplateId = long.Parse(effect.Element("TemplateId")?.Value ?? "0");
		TemplateRevision = int.Parse(effect.Element("TemplateRevision")?.Value ?? "0");
		CreatorId = long.Parse(effect.Element("CreatorId")?.Value ?? "0");
		PayloadIndex = int.Parse(effect.Element("PayloadIndex")!.Value);
		TargetCharacterId = long.Parse(effect.Element("TargetCharacterId")!.Value);
	}

	public Guid TrapInstanceId { get; }
	public long TemplateId { get; }
	public int TemplateRevision { get; }
	public long CreatorId { get; }
	public int PayloadIndex { get; }
	public long TargetCharacterId { get; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "TrapPayloadSchedule";

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("TrapInstanceId", TrapInstanceId),
			new XElement("TemplateId", TemplateId),
			new XElement("TemplateRevision", TemplateRevision),
			new XElement("CreatorId", CreatorId),
			new XElement("PayloadIndex", PayloadIndex),
			new XElement("TargetCharacterId", TargetCharacterId));
	}

	public override string Describe(IPerceiver voyeur) => "A delayed trap payload.";

	public override void ExpireEffect()
	{
		var trap = Owner.EffectsOfType<TrapEffect>()
			.FirstOrDefault(x => x.InstanceId == TrapInstanceId);
		if (trap is null && TemplateId > 0)
		{
			var template = Gameworld.TrapTemplates.Get(TemplateId, TemplateRevision);
			if (template is not null)
			{
				trap = new TrapEffect(Owner, template, TrapInstanceId, CreatorId);
			}
		}

		trap?.ExecutePayload(PayloadIndex, TargetCharacterId);
		base.ExpireEffect();
	}
}

/// <summary>Returns a reusable trap to its armed state after its configured cooldown.</summary>
public sealed class TrapResetEffect : Effect
{
	public static void InitialiseEffectType()
	{
		RegisterFactory("TrapReset", (effect, owner) => new TrapResetEffect(effect, owner));
	}

	public TrapResetEffect(IPerceivable owner, Guid trapInstanceId)
		: base(owner)
	{
		TrapInstanceId = trapInstanceId;
	}

	private TrapResetEffect(XElement root, IPerceivable owner)
		: base(root, owner)
	{
		TrapInstanceId = Guid.Parse(root.Element("Effect")!.Element("TrapInstanceId")!.Value);
	}

	public Guid TrapInstanceId { get; }
	public override bool SavingEffect => true;
	protected override string SpecificEffectType => "TrapReset";

	protected override XElement SaveDefinition() =>
		new("Effect", new XElement("TrapInstanceId", TrapInstanceId));

	public override string Describe(IPerceiver voyeur) => "A trap cooldown.";

	public override void ExpireEffect()
	{
		Owner.EffectsOfType<TrapEffect>()
			.FirstOrDefault(x => x.InstanceId == TrapInstanceId)?
			.ResetAfterCooldown();
		base.ExpireEffect();
	}
}
