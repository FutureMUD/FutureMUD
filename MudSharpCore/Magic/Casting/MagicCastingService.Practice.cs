using System.Collections.Concurrent;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private sealed class ActivePractice(MagicCastingIntent intent, Prepared prepared, CastingOperation operation,
		XElement payload, IInventoryPlan plan, bool skillEligible, bool masteryEligible)
	{
		public MagicCastingIntent Intent { get; } = intent;
		public Prepared Prepared { get; } = prepared;
		public CastingOperation Operation { get; set; } = operation;
		public XElement Payload { get; } = payload;
		public IInventoryPlan Plan { get; } = plan;
		public bool SkillEligible { get; } = skillEligible;
		public bool MasteryEligible { get; } = masteryEligible;
		public ICell? Location { get; } = intent.Actor.Location;
		public DateTime Deadline { get; } = operation.CreatedUtc + prepared.Spell.GradeProfile!.Practice!.Duration;
		public MagicPracticeAction? Action { get; set; }
		public ControlledSpellProfile Profile { get; } = prepared.Spell.GradeProfile!;
		public bool Completing { get; set; }
		public string? LostInputReason { get; set; }
	}

	private readonly ConcurrentDictionary<Guid, ActivePractice> _activePractices = new();

	private MagicCastingResult StartPractice(MagicCastingIntent intent)
	{
		var actor = intent.Actor; var owner = Owner(actor);
		lock (Guard(actor))
		{
			if (intent.OriginId is { } originId && (_store.Operation(originId) is not null || _uncertain.ContainsKey(originId)))
				return new(MagicCastingStatus.Refused, "That invocation origin has already been consumed; it cannot practise again.", originId);
			if (!_mutating.TryAdd(owner.Id, 0)) return new(MagicCastingStatus.Refused, "A casting mutation is already active for this identity.");
			ActivePractice? active = null;
			try
			{
				NotifyCapacityChange(actor);
				if (_activePractices.Values.Any(x => x.Operation.CharacterId == owner.Id))
					throw new InvalidOperationException("You already have a practice action in progress.");
				var prepared = Prepare(intent); var resolved = prepared.Quote.Invocation!;
				var profile = prepared.Spell.GradeProfile!; var policy = profile.Practice!;
				var acquired = Acquisition(actor, intent.SpellId)!; var opportunity = _store.Opportunity(owner.Id, resolved.TraitId);
				var now = _clock(); var skillEligible = opportunity is null || opportunity.NextUtc <= now;
				var masteryEligible = intent.Overreach && acquired.NextMasteryUtc <= now;
				var plan = prepared.Spell.PracticeInventoryPlanTemplate!.CreatePlan(actor);
				var live = Prepare(intent);
				if (!Equivalent(prepared, live)) throw new InvalidOperationException("The practice route, costs, body or components changed before commitment.");
				if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible || !PlanItems(plan).Order().SequenceEqual(prepared.Items.Order()))
					throw new InvalidOperationException("The executing practice plan no longer matches its captured physical inputs.");
				var payload = new XElement("Casting", new XAttribute("version", 1), new XAttribute("mode", "Practice"),
					new XAttribute("admission", resolved.AdmissionId), new XAttribute("identity", resolved.CapabilityIdentity),
					new XAttribute("grade", resolved.Grade), new XAttribute("profile", resolved.ProfileVersion),
					new XAttribute("priorGrade", acquired.ControlledGrade), new XAttribute("controlledGrade", resolved.ControlledGrade),
					new XAttribute("skillEligible", skillEligible), new XAttribute("masteryEligible", masteryEligible),
					new XAttribute("skillBefore", owner.TraitRawValue(_world.Traits.Get(resolved.TraitId))),
					new XAttribute("deadlineUtc", now + policy.Duration), new XAttribute("location", actor.Location.Id),
					new XElement("Targets", ""), new XElement("PracticePlan", prepared.Spell.PracticeInventoryPlanTemplate.SaveToXml()),
					prepared.Payments.Select(x => new XElement("Cost", new XAttribute("holder", x.Holder.Id),
						new XAttribute("resource", x.Resource.Id), new XAttribute("amount", x.Amount),
						new XAttribute("before", x.Holder.MagicResourceAmounts.GetValueOrDefault(x.Resource)))),
					prepared.Items.Select(x => new XElement("Item", new XAttribute("id", x))));
				_checkpoint?.Invoke("BeforePayment");
				active = new(intent, prepared, new(resolved.Id, owner.Id, actor.InstanceId, actor.Body.Id,
					resolved.CapabilityId, resolved.SpellId, resolved.TraitId, resolved.ReserveId, "Paying",
					payload.ToString(SaveOptions.DisableFormatting), now, now), payload, plan, skillEligible, masteryEligible);
				_store.Write(active.Operation,
					masteryEligible ? acquired with { NextMasteryUtc = now + profile.MasteryInterval } : null,
					skillEligible ? new(owner.Id, resolved.TraitId, now + profile.SkillInterval, opportunity?.Version ?? 0) : null);
				_checkpoint?.Invoke("Paying");
				foreach (var payment in prepared.Payments)
					if (!payment.Holder.UseResource(payment.Resource, payment.Amount)) throw new InvalidOperationException("A revalidated practice payment was declined.");
				PracticeStage(active, "PaymentMutated");
				plan.ExecuteWholePlan(); Flush(actor); PracticeStage(active, "Committed");
				if (PracticeError(active) is { } changed)
				{
					CancelPractice(active, changed + " No progress or refund.");
					return new(MagicCastingStatus.Failed, "Paid practice lost its required inputs during commitment.", resolved.Id);
				}
				PracticeStage(active, "Practising");
				var work = active;
				active.Action = new(actor, resolved.Id, $"practising {prepared.Spell.Name}", policy.AllowMovement,
					() => CompletePractice(work), () => CancelPractice(work, "Practice was interrupted; no progress or refund."),
					() => PracticeError(work) is null);
				if (!_activePractices.TryAdd(resolved.Id, active)) throw new InvalidOperationException("Duplicate practice operation identity.");
				actor.AddEffect(active.Action, policy.Duration);
				return new(MagicCastingStatus.Started, $"You begin practising {prepared.Spell.Name}. The full cost is committed; interruption gives no progress or refund.", resolved.Id);
			}
			catch (Exception ex)
			{
				if (active is null) return new(MagicCastingStatus.Refused, ex.Message);
				_activePractices.TryRemove(active.Operation.Id, out _);
				return PracticeUncertain(active, ex);
			}
			finally { _mutating.TryRemove(owner.Id, out _); }
		}
	}

	private void PracticeStage(ActivePractice active, string stage)
	{
		var next = active.Operation with { Stage = stage, Definition = active.Payload.ToString(SaveOptions.DisableFormatting), UpdatedUtc = _clock() };
		_store.Write(next); active.Operation = next; _checkpoint?.Invoke(stage);
	}

	private string? PracticeError(ActivePractice active)
	{
		if (active.LostInputReason is { } lost) return lost;
		var actor = active.Intent.Actor; var captured = active.Prepared.Quote.Invocation!;
		if (Owner(actor).Id != active.Operation.CharacterId || actor.InstanceId != active.Operation.ActorId || actor.Body?.Id != active.Operation.BodyId)
			return "The practice identity, acting instance or body changed.";
		if (!active.Profile.Practice!.AllowMovement && !ReferenceEquals(actor.Location, active.Location))
			return "You moved away from the practice location.";
		if (actor.Combat is not null || actor.Movement is not null && !active.Profile.Practice.AllowMovement)
			return "Movement or combat interrupted practice.";
		if (ResolveRoute(actor, active.Intent.CapabilityId, active.Intent.SpellId, active.Intent.Grade, active.Intent.Overreach,
			out var capability, out var admission, out var spell, out var trait, out var difficulty,
			MagicCastingMode.Practice, active.Operation.Id) is { } error) return error;
		if (captured.CapabilityIdentity != capability.CastingPolicy!.Identity || captured.ConfigurationVersion != capability.CastingPolicy.Version ||
			captured.AdmissionId != admission.Key || captured.TraitId != trait.Id || captured.Difficulty != difficulty ||
			captured.ControlledGrade != Acquisition(actor, spell.Id)!.ControlledGrade ||
			active.Prepared.Configuration != CaptureConfiguration(spell)) return "The authored practice or acquisition inputs changed.";
		if (CastingQuarantineReason(actor, itemIds: active.Prepared.Items, continuingOperation: active.Operation.Id) is { } materials) return materials;
		return null;
	}

	private void CompletePractice(ActivePractice active)
	{
		var actor = active.Intent.Actor; var owner = Owner(actor);
		lock (Guard(actor))
		{
			if (!_activePractices.TryGetValue(active.Operation.Id, out var current) || !ReferenceEquals(current, active) || active.Completing) return;
			active.Completing = true;
			if (!_mutating.TryAdd(owner.Id, 0))
			{
				_activePractices.TryRemove(active.Operation.Id, out _);
				actor.OutputHandler.Send(PracticeUncertain(active, new InvalidOperationException("A casting mutation overlapped practice completion.")).Message); return;
			}
			try
			{
				if (StopFinalisedPractice(active)) return;
				if (_clock() < active.Deadline || PracticeError(active) is { })
				{
					CancelPractice(active, "Practice ended before its deadline or lost a required live input; no progress or refund."); return;
				}
				var captured = active.Prepared.Quote.Invocation!;
				PracticeStage(active, "CheckingPractice");
				if (StopFinalisedPractice(active)) return;
				if (PracticeError(active) is { } beforeCheck) { CancelPractice(active, beforeCheck + " No progress or refund."); return; }
				CheckOutcome check;
				using (new CheckImprovementScope(actor))
					check = _world.GetCheck(CheckType.CastSpellCheck).CheckAgainstAllDifficulties(actor, captured.Difficulty,
						_world.Traits.Get(captured.TraitId), null)[captured.Difficulty];
				if (StopFinalisedPractice(active)) return;
				if (PracticeError(active) is { } changed) { CancelPractice(active, changed + " No progress or refund."); return; }
				active.Payload.SetAttributeValue("outcome", check.Outcome);
				PracticeStage(active, "PracticeChecked");
				if (StopFinalisedPractice(active)) return;
				if (PracticeError(active) is { } beforeProgress) { CancelPractice(active, beforeProgress + " No progress or refund."); return; }
				// The live action has completed; the owner guard now owns its at-most-once progress mutation.
				_activePractices.TryRemove(active.Operation.Id, out _);
				RecordProgress(active.Intent, active.Prepared, check, check.Outcome >= active.Prepared.Spell.MinimumSuccessThreshold,
					active.SkillEligible, active.MasteryEligible, active.Payload, stage => PracticeStage(active, stage), () => active.Operation, active.Profile);
				active.Plan.FinalisePlan(); Flush(actor); PracticeStage(active, "Completed");
				actor.OutputHandler.Send(check.Outcome >= active.Prepared.Spell.MinimumSuccessThreshold ? "Your paid practice succeeds." : "Your paid practice fails.");
				try { NotifyProgress(actor, captured.TraitId, captured.SpellId); }
				catch (Exception ex) { _world.SystemMessage($"Practice {active.Operation.Id} completed, but prerequisite evaluation needs retry: {ex.Message}", true); }
			}
			catch (Exception ex) { actor.OutputHandler.Send(PracticeUncertain(active, ex).Message); }
			finally { _activePractices.TryRemove(active.Operation.Id, out _); _mutating.TryRemove(owner.Id, out _); }
		}
	}

	private void CancelPractice(ActivePractice active, string reason)
	{
		lock (Guard(active.Intent.Actor))
		{
			var ownerId = Owner(active.Intent.Actor).Id;
			var ownsMutation = _mutating.TryAdd(ownerId, 0);
			_activePractices.TryRemove(active.Operation.Id, out _);
			try
			{
				if (StopFinalisedPractice(active)) return;
				active.Plan.FinalisePlan(); Flush(active.Intent.Actor);
				if (StopFinalisedPractice(active)) return;
				active.Payload.SetAttributeValue("interruption", reason);
				PracticeStage(active, "PracticeInterrupted");
				active.Intent.Actor.OutputHandler.Send(reason);
			}
			catch (Exception ex) { active.Intent.Actor.OutputHandler.Send(PracticeUncertain(active, ex).Message); }
			finally { if (ownsMutation) _mutating.TryRemove(ownerId, out _); }
		}
	}

	private bool StopFinalisedPractice(ActivePractice active)
	{
		var durable = _store.Operation(active.Operation.Id);
		if (durable is null || !MagicCastingStateStore.TerminalStages.Contains(durable.Stage)) return false;
		active.Operation = durable;
		_activePractices.TryRemove(durable.Id, out _);
		try { active.Action?.Abandon(); active.Plan.FinalisePlan(); }
		catch (Exception ex) { _world.SystemMessage($"Finalised practice {durable.Id} needs transient cleanup: {ex.Message}", true); }
		return true;
	}

	private MagicCastingResult PracticeUncertain(ActivePractice active, Exception ex)
	{
		try { active.Action?.Abandon(); }
		catch (Exception cleanup) { ex = new AggregateException(ex, cleanup); }
		var diagnostic = $"At {active.Operation.Stage}: {ex.GetBaseException().Message}";
		if (diagnostic.Length > 4096) diagnostic = diagnostic[..4096];
		var uncertain = active.Operation with { Stage = "NeedsReview", Diagnostic = diagnostic, UpdatedUtc = _clock() };
		_uncertain[uncertain.Id] = uncertain;
		try { _store.Write(uncertain); } catch { /* Keep the earlier durable operation and in-memory quarantine. */ }
		return new(MagicCastingStatus.NeedsReview, $"Practice operation {uncertain.Id} requires staff reconciliation. {diagnostic}", uncertain.Id);
	}

	private void ValidatePractices(ICharacter actor)
	{
		lock (Guard(actor))
		{
			foreach (var active in _activePractices.Values.Where(x => x.Operation.CharacterId == Owner(actor).Id).ToArray())
				if (PracticeError(active) is { } reason)
				{
					active.LostInputReason ??= reason;
					// Completion owns its mutation fence; its next live recheck observes the latched loss.
					if (active.Completing) continue;
					active.Intent.Actor.RemoveEffect(active.Action!, true);
					CancelPractice(active, reason + " No progress or refund.");
				}
		}
	}

	public void NotifyPracticeInputsChanged(ICharacter actor) => ValidatePractices(actor);

	public void InterruptPractice(ICharacter actor, string reason)
	{
		lock (Guard(actor))
		{
			foreach (var active in _activePractices.Values.Where(x => x.Operation.CharacterId == Owner(actor).Id).ToArray())
			{
				active.Intent.Actor.RemoveEffect(active.Action!, true);
				CancelPractice(active, reason);
			}
		}
	}
}
