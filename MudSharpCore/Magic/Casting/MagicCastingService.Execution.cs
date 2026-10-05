using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;
using System.Globalization;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	public MagicCastingResult Cast(MagicCastingIntent intent)
	{
		if (intent.Mode == MagicCastingMode.Practice) return StartPractice(intent);
		var actor = intent.Actor;
		var owner = Owner(actor);
		lock (Guard(actor))
		{
			if (intent.OriginId is { } originId && (_store.Operation(originId) is not null || _uncertain.ContainsKey(originId)))
				return new(MagicCastingStatus.Refused, "That invocation origin has already been consumed; it cannot cast again.", originId);
			if (!_mutating.TryAdd(owner.Id, 0)) return new(MagicCastingStatus.Refused, "A casting mutation is already active for this identity.");
			CastingOperation? operation = null;
			XElement? payload = null;
			try
			{
				NotifyCapacityChange(actor);
				var prepared = Prepare(intent);
				var selectionAdmission = CaptureSelectionAdmission(intent, prepared);
				var resolved = prepared.Quote.Invocation!;
				var profile = prepared.Spell.GradeProfile!;
				var acquired = Acquisition(actor, intent.SpellId)!;
				var opportunity = _store.Opportunity(owner.Id, resolved.TraitId);
				var now = _clock();
				var skillEligible = opportunity is null || opportunity.NextUtc <= now;
				var masteryEligible = intent.Overreach && acquired.NextMasteryUtc <= now;
				payload = new XElement("Casting", new XAttribute("version", 1), new XAttribute("admission", resolved.AdmissionId),
					new XAttribute("identity", resolved.CapabilityIdentity), new XAttribute("grade", resolved.Grade),
					new XAttribute("profile", resolved.ProfileVersion), new XAttribute("priorGrade", acquired.ControlledGrade),
					new XAttribute("controlledGrade", resolved.ControlledGrade),
					new XAttribute("skillEligible", skillEligible), new XAttribute("masteryEligible", masteryEligible),
					new XAttribute("skillBefore", owner.TraitRawValue(_world.Traits.Get(resolved.TraitId))),
					new XElement("Targets", new XCData(intent.Targets)),
					prepared.Area is { } area ? AreaReceipt(area) : null,
					resolved.Delivery is { } delivery ? new XElement("Speech", new XAttribute("origin", resolved.Id),
						new XAttribute("kind", delivery.UsesOriginalSpeech ? "PlayerInput" : "GeneratedCasting"),
						new XAttribute("method", delivery.Method), new XAttribute("volume", (int)delivery.Volume),
						new XAttribute("language", delivery.LanguageId), new XAttribute("energy", delivery.EnergyMultiplier),
						new XAttribute("difficulty", delivery.DifficultySteps), new XCData(delivery.Incantation),
						new XElement("Formula", new XCData(intent.Speech?.FormulaText ?? delivery.Incantation))) : null,
					prepared.Payments.Select(x => new XElement("Cost", new XAttribute("holder", x.Holder.Id),
						new XAttribute("resource", x.Resource.Id), new XAttribute("amount", x.Amount),
						new XAttribute("before", x.Holder.MagicResourceAmounts.GetValueOrDefault(x.Resource)))),
					prepared.Items.Select(x => new XElement("Item", new XAttribute("id", x))));
				void Stage(string stage)
				{
					var next = operation! with { Stage = stage, Definition = payload.ToString(SaveOptions.DisableFormatting), UpdatedUtc = _clock() };
					_store.Write(next);
					operation = next;
					_checkpoint?.Invoke(stage);
				}
				var execution = new ConfiguredCastingExecution(prepared.Payments)
				{
					BeforeMaterials = () => Stage("PaymentMutated"),
					AfterCommit = () => { Flush(actor); Stage("Committed"); }
				};
				var invocation = new SpellInvocationContext(SpellInvocationSource.ConfiguredCasting, Outcome.NotTested, pay =>
				{
					// The quote is advisory. Re-resolve body, permission, target, inventory and prices under the owner guard.
					var live = Prepare(intent, prepared.Spell);
					if (!Equivalent(prepared, live)) throw new InvalidOperationException("The route, prices, body, target or component inputs changed before commitment; request a fresh cast.");
					EmitIncantation(actor, resolved.Id, resolved.Delivery);
					_checkpoint?.Invoke("BeforePayment");
					var committed = Prepare(intent, prepared.Spell);
					if (!Equivalent(prepared, committed)) throw new InvalidOperationException("The native incantation changed casting eligibility or inputs before payment.");
					if (committed.Area is { } areaPlan)
					{
						// Select exactly once, after final revalidation. Quotes and recovery never draw or replay.
						var applications = SelectAreaApplications(areaPlan);
						payload.Element("Area")!.ReplaceWith(AreaReceipt(areaPlan, applications));
						execution.AreaApplications = Array.AsReadOnly(applications.Select(x => new ConfiguredAreaApplication(
							x.Target, x.Receipt.DamageMultiplier, () => AreaStillEligible(actor, x, areaPlan, prepared.Spell))).ToArray());
					}
					var devicePaymentAdmission = AdmitDeviceFocusPayment(intent, committed);
					var paymentAdmission = selectionAdmission is null ? devicePaymentAdmission :
						AdmitSelectionPayment(intent, committed, selectionAdmission, devicePaymentAdmission);
					operation = new(resolved.Id, owner.Id, actor.InstanceId, actor.Body.Id, resolved.CapabilityId, resolved.SpellId,
						resolved.TraitId, resolved.ReserveId, "Paying", payload.ToString(SaveOptions.DisableFormatting), now, now);
					_store.Write(operation,
						masteryEligible ? acquired with { NextMasteryUtc = now + profile.MasteryInterval } : null,
						skillEligible ? new(owner.Id, resolved.TraitId, now + profile.SkillInterval, opportunity?.Version ?? 0) : null);
					_checkpoint?.Invoke("Paying");
					using (paymentAdmission?.OpenScope()) pay();
					return true;
				}) { Configured = execution };
				prepared.Spell.CastVancian(actor, prepared.Target.Target, resolved.Power, invocation, prepared.Target.Parameters);
				if (invocation.Status == MagicInvocationStatus.Refused)
					return new(MagicCastingStatus.Refused, "The live casting preflight refused; no payment was committed.");
				payload.SetAttributeValue("outcome", execution.CheckResult?.Outcome.ToString() ?? "Unknown");
				payload.SetAttributeValue("applied", execution.AppliedIntendedOperation);
				Stage("EffectsExecuted");
				RecordProgress(intent, prepared, execution.CheckResult,
					invocation.Status == MagicInvocationStatus.Succeeded && execution.AppliedIntendedOperation,
					skillEligible, masteryEligible, payload, Stage, () => operation!);
				Flush(actor);
				Stage("Completed");
				return new(invocation.Status == MagicInvocationStatus.Succeeded ? MagicCastingStatus.Succeeded : MagicCastingStatus.Failed,
					invocation.Status == MagicInvocationStatus.Succeeded ? "The paid casting succeeded." : "The paid casting failed or its targets rejected it.", operation!.Id);
			}
			catch (Exception ex)
			{
				if (operation is null) return new(MagicCastingStatus.Refused, ex.Message);
				// Preserve the last durably known sample. Never turn a sample held only in memory into a recovery claim.
				var diagnostic = ex.Message;
				if (ex.GetBaseException() is { } cause && !ReferenceEquals(cause, ex)) diagnostic += $" ({cause.Message})";
				if (diagnostic.Length > 4096) diagnostic = diagnostic[..4096];
				var uncertain = operation with { Stage = "NeedsReview", Diagnostic = $"At {operation.Stage}: {diagnostic}", UpdatedUtc = _clock() };
				_uncertain[uncertain.Id] = uncertain;
				try { _store.Write(uncertain); } catch { /* The earlier non-final receipt remains a durable quarantine. */ }
				return new(MagicCastingStatus.NeedsReview, $"Operation {operation.Id} requires staff reconciliation. {uncertain.Diagnostic}", operation.Id);
			}
			finally
			{
				_mutating.TryRemove(owner.Id, out _);
				if (operation?.Stage == "Completed")
				{
					try { NotifyProgress(actor, operation.TraitId, operation.SpellId); }
					catch (Exception ex) { _world.SystemMessage($"Casting {operation.Id} completed, but prerequisite evaluation needs retry: {ex.Message}", true); }
				}
			}
		}
	}

	private void RecordProgress(MagicCastingIntent intent, Prepared prepared, CheckOutcome? check,
		bool successfulOperation, bool skillEligible, bool masteryEligible, XElement payload,
		Action<string> stage, Func<CastingOperation> operation, ControlledSpellProfile? capturedProfile = null)
	{
		var actor = intent.Actor; var owner = Owner(actor); var resolved = prepared.Quote.Invocation!;
		if (skillEligible && check is not null)
		{
			stage("ImprovingSkill");
			owner.GetTrait(_world.Traits.Get(resolved.TraitId)).TraitUsed(owner, check.Outcome,
				resolved.Difficulty.Lowest(_world.GetCheck(CheckType.CastSpellCheck).MaximumDifficultyForImprovement),
				TraitUseType.Practical, check.ActiveBonuses ?? []);
			Flush(actor); stage("SkillRecorded");
		}
		var profile = capturedProfile ?? prepared.Spell.GradeProfile!;
		if (!masteryEligible || !successfulOperation || check is null || check.Outcome < prepared.Spell.MinimumSuccessThreshold) return;
		stage("SamplingMastery");
		var sample = _random();
		if (!double.IsFinite(sample) || sample is < 0 or >= 1) throw new InvalidOperationException("Invalid mastery random sample.");
		_checkpoint?.Invoke("MasterySampledBeforeWrite");
		payload.SetAttributeValue("masterySample", sample);
		payload.SetAttributeValue("masteryAdvance", sample < profile.MasteryChance);
		stage("MasterySampleRecorded");
		if (sample >= profile.MasteryChance) return;
		var current = Acquisition(actor, intent.SpellId)!;
		_store.Write(operation(), current with { ControlledGrade = intent.Grade });
		_checkpoint?.Invoke("GradePersisted");
	}

	private static bool Equivalent(Prepared a, Prepared b)
	{
		var x = a.Quote.Invocation!; var y = b.Quote.Invocation!;
		return a.Configuration == b.Configuration && x.Mode == y.Mode && x.Delivery == y.Delivery && x.ActorId == y.ActorId && x.BodyId == y.BodyId && x.CapabilityIdentity == y.CapabilityIdentity && x.AdmissionId == y.AdmissionId &&
			(a.Area is null && b.Area is null || a.Area is { } areaA && b.Area is { } areaB &&
				ReferenceEquals(areaA.Location, areaB.Location) && ReferenceEquals(areaA.CasterBody, areaB.CasterBody) &&
				areaA.CasterLayer == areaB.CasterLayer && areaA.Candidates.Select(t => t.Receipt).SequenceEqual(areaB.Candidates.Select(t => t.Receipt)) &&
				areaA.Candidates.Select(t => t.Body).SequenceEqual(areaB.Candidates.Select(t => t.Body), ReferenceEqualityComparer.Instance)) &&
			x.ProfileVersion == y.ProfileVersion && x.TraitId == y.TraitId && x.Grade == y.Grade && x.ControlledGrade == y.ControlledGrade && x.Difficulty == y.Difficulty &&
			x.Costs.SequenceEqual(y.Costs) && a.Items.Order().SequenceEqual(b.Items.Order()) &&
			Targets(a.Target).SequenceEqual(Targets(b.Target), ReferenceEqualityComparer.Instance) && a.Target.Parameters.SequenceEqual(b.Target.Parameters);

		static IEnumerable<IPerceivable?> Targets(SpellTargetResolution r) =>
			r.Target is MudSharp.PerceptionEngine.Lists.PerceivableGroup g ? g.Members : [r.Target];
	}


	/// <summary>Staff acknowledgement can recover a proven sample, but never executes, refunds or rolls again.</summary>
	public MagicCastingGrant ReconcileOperation(ICharacter authority, ICharacter target, Guid id, string reason)
	{
		if (!authority.IsAdministrator() || string.IsNullOrWhiteSpace(reason)) return new(false, false, "Staff authority and a reconciliation reason are required.");
		lock (Guard(target))
		{
			var op = _store.Operation(id) ?? _uncertain.GetValueOrDefault(id);
			if (op is null || op.CharacterId != Owner(target).Id) return new(false, false, "No such operation for that canonical character.");
			if (_activePractices.ContainsKey(id) || _mutating.ContainsKey(Owner(target).Id))
				return new(false, false, "Casting work is still running. Stop the live action before reconciling its operation.");
			if (MagicCastingStateStore.TerminalStages.Contains(op.Stage)) return new(false, true, "Operation already finalised.");
			XElement? data = null;
			try { data = XElement.Parse(op.Definition); } catch (System.Xml.XmlException) { /* Staff can acknowledge corrupt state, without inferring progress. */ }
			var acquired = Acquisition(target, op.SpellId);
			AcquiredSpell? update = null;
			if ((string?)data?.Attribute("masteryAdvance") == "true" && acquired is not null &&
				double.TryParse((string?)data.Attribute("masterySample"), NumberStyles.Float, CultureInfo.InvariantCulture, out var sample) && sample is >= 0 and < 1 &&
				int.TryParse((string?)data.Attribute("profile"), out var profile) && acquired.ProfileVersion == profile &&
				int.TryParse((string?)data.Attribute("priorGrade"), out var prior) && acquired.ControlledGrade == prior &&
				int.TryParse((string?)data.Attribute("grade"), out var grade) && grade == prior + 1 && grade <= 7)
				update = acquired with { ControlledGrade = grade };
			_store.Write(op with { Stage = "Reconciled", Diagnostic = $"{op.Diagnostic}\nStaff {authority.Id}: {reason}", UpdatedUtc = _clock() }, update);
			_uncertain.TryRemove(id, out _);
			return new(true, true, "Reconciled without replay, refund or new random roll. Unproven progress remains at its durable value.");
		}
	}
}
