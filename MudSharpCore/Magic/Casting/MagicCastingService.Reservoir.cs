using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;
using MudSharp.Magic.Vancian;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	public static int? ReservoirGrade(string value)
	{
		if (int.TryParse(value, out var grade) && grade is >= 1 and <= 7) return grade;
		return ArmageddonPowerVocabulary.TryParse(value, out grade) ? grade : null;
	}

	public MagicCastingResult DonateReservoir(ICharacter actor, IGameItem item, int amount)
	{
		lock (_deviceGuard)
		lock (Guard(actor))
		{
			if (!_mutating.TryAdd(Owner(actor).Id, 0)) return new(MagicCastingStatus.Refused, "A magical mutation is already active.");
			ChargedMagicDeviceGameItemComponent? device = null;
			CastingOperation? operation = null;
			Guid? token = null;
			var claimOutcomeUnknown = false;
			try
			{
				device = item.GetItemType<IChargedMagicDevice>() as ChargedMagicDeviceGameItemComponent
					?? throw new InvalidOperationException("This item has no charged magic device component.");
				if (device.Mode != MagicDeviceMode.EnergyReservoir || device.ResourceId is not { } resourceId)
					throw new InvalidOperationException("This device does not accept energy donations.");
				if (amount <= 0) throw new InvalidOperationException("Donate a positive whole number of energy points.");
				var use = CaptureDeviceUse(actor, device);
				if (DeviceItemError(actor, device) is { } error) throw new InvalidOperationException(error);
				var accepted = amount;
				if (accepted > device.MaximumEnergy - device.Energy) throw new InvalidOperationException("The donation exceeds the reservoir's remaining capacity.");
				var resource = _world.MagicResources.Get(resourceId) ?? throw new InvalidOperationException("The configured energy resource is missing.");
				if (CastingQuarantineReason(actor, reserveId: resourceId, itemIds: [item.Id]) is { } quarantine)
					throw new InvalidOperationException(quarantine);
				var paymentAdmission = AdmitDevicePayments(actor, [new CastingPayment(actor, resource, accepted)], use, focusSpell: device.ReservoirSpellId);
				token = Guid.NewGuid();
				if (!device.ReserveReservoir(token.Value)) throw new InvalidOperationException("The device reservoir is already reserved.");
				PersistDevice(device);
				var receipt = DeviceReceipt(device, "DeviceEnergyDonation");
				receipt.SetAttributeValue("amount", accepted); receipt.SetAttributeValue("energyBefore", device.Energy);
				operation = new(token.Value, Owner(actor).Id, actor.InstanceId, actor.Body!.Id, 0, device.ReservoirSpellId!.Value,
					0, resourceId, "DeviceEnergyDonating", receipt.ToString(), _clock(), _clock());
				bool claimed;
				try { claimed = ClaimDeviceActivation(operation); }
				catch { claimOutcomeUnknown = true; throw; }
				if (!claimed) throw new InvalidOperationException("This donation has already been claimed.");
				_checkpoint?.Invoke("DeviceEnergyDonating");
				if (DeviceStructuralError(actor, use, token) is { } structural) throw new InvalidOperationException(structural);
				using (paymentAdmission.OpenScope())
					if (!actor.UseResource(resource, accepted))
						throw new InvalidOperationException("The resource payment was declined after revalidation.");
				var postPaymentStructure = DeviceStructuralError(actor, use, token);
				if (postPaymentStructure is not null || device.Energy + accepted > device.MaximumEnergy || device.ReservoirReservation != token)
					throw new InvalidOperationException(postPaymentStructure ?? "The captured reservoir changed after payment.");
				if (device.AddEnergy(token.Value, accepted) != accepted) throw new InvalidOperationException("The reservoir accepted a different amount after payment.");
				PersistDevice(device); _checkpoint?.Invoke("DeviceEnergyAdded"); Flush(actor);
				_store.Write(operation with { Stage = "Completed", Diagnostic = $"donated:{accepted}", UpdatedUtc = _clock() });
				return new(MagicCastingStatus.Succeeded, $"You donate {accepted} {resource.Name.Pluralise()} to the device.", token);
			}
			catch (Exception ex)
			{
				if (operation is not null && (claimOutcomeUnknown || _store.Operation(operation.Id) is not null)) return DeviceUncertain(operation, ex);
				if (device is not null && token is { } id)
				{
					device.ReleaseReservoir(id);
					try { PersistDevice(device); } catch (Exception releaseError) { return new(MagicCastingStatus.NeedsReview, $"The uncommitted reservation needs staff review: {releaseError.Message}", id); }
				}
				return new(MagicCastingStatus.Refused, ex.Message);
			}
			finally { _mutating.TryRemove(Owner(actor).Id, out _); }
		}
	}

	public MagicCastingResult ActivateReservoir(ICharacter actor, IGameItem item, string powerWord)
	{
		lock (_deviceGuard)
		lock (Guard(actor))
		{
			if (!_mutating.TryAdd(Owner(actor).Id, 0)) return new(MagicCastingStatus.Refused, "A magical mutation is already active.");
			ChargedMagicDeviceGameItemComponent? device = null;
			CastingOperation? operation = null;
			Guid? token = null;
			var claimOutcomeUnknown = false;
			try
			{
				device = item.GetItemType<IChargedMagicDevice>() as ChargedMagicDeviceGameItemComponent
					?? throw new InvalidOperationException("This item has no charged magic device component.");
				if (device.Mode != MagicDeviceMode.EnergyReservoir || device.ReservoirSpellId is not { } spellId)
					throw new InvalidOperationException("This device does not have an energy reservoir payload.");
				var use = CaptureDeviceUse(actor, device);
				var grade = ReservoirGrade(powerWord) ?? throw new InvalidOperationException("Choose a source power word (wek, yuqa, kral, een, pav, sul or mon), or grade 1-7.");
				if (!device.GradeCosts.TryGetValue(grade, out var cost) || cost <= 0) throw new InvalidOperationException("That grade is unavailable for this device.");
				if (device.Energy < cost) throw new InvalidOperationException($"The reservoir needs {cost} energy; it currently holds {device.Energy}.");
				if (DeviceItemError(actor, device) is { } itemError) throw new InvalidOperationException(itemError);
				if (VancianMagicService.CastingError(actor) is { } physical) throw new InvalidOperationException(physical);
				var source = _world.MagicSpells.Get(spellId) as MagicSpell ?? throw new InvalidOperationException("The fixed reservoir spell is missing.");
				if (source.CastingTrait is null || source.GradeProfile?.Grades.SingleOrDefault(x => x.Grade == grade) is not { } authoredGrade)
					throw new InvalidOperationException("The fixed spell does not support that grade or has no casting trait.");
				var spell = source.CastingCopy(actor, source.CastingTrait, grade, authoredGrade.Power, Difficulty.Normal);
				ValidateReservoirAdapter(device, spell);
				var (target, parameters, areaApplications) = PrepareReservoirDelivery(actor, device, spell, grade, authoredGrade.Power);
				var targetStates = CaptureDeviceTargets(target);
				if (DeviceStructuralError(actor, use, targets: targetStates) is { } structural) throw new InvalidOperationException(structural);
				var selections = CaptureReservoirSelections(actor, spell, target, parameters, authoredGrade.Power);
				token = Guid.NewGuid();
				if (!device.ReserveReservoir(token.Value)) throw new InvalidOperationException("The reservoir was reserved by another action.");
				PersistDevice(device);
				var payload = DeviceReceipt(device, "DeviceReservoirActivation");
				payload.SetAttributeValue("grade", grade); payload.SetAttributeValue("cost", cost);
				payload.SetAttributeValue("energyBefore", device.Energy); payload.SetAttributeValue("claim", Guid.NewGuid());
				operation = new(token.Value, Owner(actor).Id, actor.InstanceId, actor.Body!.Id, 0, spellId,
					spell.CastingTrait.Id, device.ResourceId ?? 0, "DeviceReservoirUsing", payload.ToString(), _clock(), _clock());
				var configured = new ConfiguredCastingExecution([])
				{
					FixedOutcome = Outcome.Pass,
					AreaApplications = areaApplications
				};
				var itemConfiguration = device.Prototype is ChargedMagicDeviceGameItemComponentProto proto ? proto.Configuration : string.Empty;
				var invocation = new SpellInvocationContext(SpellInvocationSource.ScrollActivation, Outcome.Pass, pay =>
				{
					// Confirm native prepared admissions; this carrier has no personal payments or material plan.
					pay();
					if (DeviceIdentityError(actor, use) is not null || device.ReservoirReservation != token || device.Energy < cost ||
						device.Prototype is not ChargedMagicDeviceGameItemComponentProto current || current.Configuration != itemConfiguration ||
						!ReferenceEquals(_world.MagicSpells.Get(spellId), source) ||
						DeviceCallbackError(actor, use, token) is not null || DeviceStructuralError(actor, use, token, targetStates) is not null ||
						areaApplications?.Any(x => !x.StillEligible()) == true || !ConfirmReservoirSelections(actor, spell, selections) ||
						targetStates.Any(x => !actor.CanInteractPlanar(x.Target, PlanarInteractionKind.Magic)) ||
						DeviceStructuralError(actor, use, token, targetStates) is not null) return false;
					bool claimed;
					try { claimed = ClaimDeviceActivation(operation); }
					catch { claimOutcomeUnknown = true; throw; }
					if (!claimed) return false;
					_checkpoint?.Invoke("DeviceReservoirUsing");
					device.SpendReservoir(token.Value, cost); PersistDevice(device); _checkpoint?.Invoke("DeviceReservoirSpent");
					return true;
				}) { Configured = configured };
				spell.CastVancian(actor, target, authoredGrade.Power, invocation, parameters);
				if (_store.Operation(token.Value) is null)
				{
					device.ReleaseReservoir(token.Value); PersistDevice(device);
					return new(MagicCastingStatus.Refused, "Activation preflight refused; no energy was spent.");
				}
				Flush(actor);
				_store.Write(operation with { Stage = "Completed", Diagnostic = invocation.Status.ToString(), UpdatedUtc = _clock() });
				return new(invocation.Status == MagicInvocationStatus.Succeeded ? MagicCastingStatus.Succeeded : MagicCastingStatus.Failed,
					$"The device spent {cost} energy once; native resistance and ward checks remain active.", token);
			}
			catch (Exception ex)
			{
				if (operation is not null && (claimOutcomeUnknown || _store.Operation(operation.Id) is not null)) return DeviceUncertain(operation, ex);
				if (device is not null && token is { } id)
				{
					device.ReleaseReservoir(id);
					try { PersistDevice(device); } catch (Exception releaseError) { return new(MagicCastingStatus.NeedsReview, $"The uncommitted reservation needs staff review: {releaseError.Message}", id); }
				}
				return new(MagicCastingStatus.Refused, ex.Message);
			}
			finally { _mutating.TryRemove(Owner(actor).Id, out _); }
		}
	}

	private (IPerceivable Target, SpellAdditionalParameter[] Parameters, IReadOnlyList<ConfiguredAreaApplication>? AreaApplications)
		PrepareReservoirDelivery(ICharacter actor, ChargedMagicDeviceGameItemComponent device, MagicSpell spell, int grade, SpellPower power)
	{
		if (device.RequiresOpenSky && actor.Location.OutdoorsType(actor) != RoomOutdoorsType.Outdoors)
			throw new InvalidOperationException("This device's spell requires an open sky.");
		var parameters = Array.Empty<SpellAdditionalParameter>();
		IReadOnlyList<ConfiguredAreaApplication>? applications = null;
		IEnumerable<IPerceivable> targets;
		switch (device.Delivery)
		{
			case MagicDeviceDelivery.NativeArea:
				applications = PrepareReservoirArea(actor, spell, grade);
				targets = applications.Select(x => (IPerceivable)x.Target).ToArray();
				break;
			case MagicDeviceDelivery.RoomCorpses:
				if (!spell.SpellEffects.OfType<AnimateCorpseSpellEffect>().Any()) throw new InvalidOperationException("Room corpse delivery requires a native animate-corpse effect.");
				targets = actor.Location.GameItems.Where(x => x.RoomLayer == actor.RoomLayer && ReferenceEquals(x.Location, actor.Location) &&
					x.ContainedIn is null && x.InInventoryOf is null && x.GetItemType<ICorpse>() is not null &&
					actor.CanInteractPlanar(x, PlanarInteractionKind.Magic)).OrderBy(x => x.Id).ToArray();
				targets = targets.Where(x => spell.SpellEffects.OfType<IMagicSpellEffectAdmission>()
					.All(effect => effect.TryPrepareApplication(actor, x, OpposedOutcomeDegree.None, power, TimeSpan.Zero, out _, out _))).ToArray();
				break;
			case MagicDeviceDelivery.OtherCharacters:
				var isBanishment = spell.SpellEffects.Any(x => x.SaveToXml().Attribute("type")?.Value == "banishtraveller");
				var hasSafeDamage = spell.SpellEffects.Any(x => x is DamageEffect { IdentitySafeDamage: true });
				if (spell.Trigger?.TargetTypes != "character" || !isBanishment && !hasSafeDamage)
					throw new InvalidOperationException("Room character delivery requires an identity-safe damage or native banishment spell.");
				targets = actor.Location.Characters.Where(x => !ReferenceEquals(x, actor) && !SameBody(x, actor) &&
					!x.IsAdministrator() && x.Body is not null && x.InstanceId > 0 && x.Body.Id > 0 && x.RoomLayer == actor.RoomLayer &&
					ReferenceEquals(x.Location, actor.Location) && actor.CanInteractPlanar(x, PlanarInteractionKind.Magic) &&
					(spell.Trigger is not CastingTriggerCharacter characterTrigger || characterTrigger.AllowsTarget(x, actor)))
					.OrderBy(x => x.Body!.Id).ThenBy(x => x.InstanceId).DistinctBy(x => x.Body!.Id).ToArray();
				break;
			default: throw new InvalidOperationException("Unsupported room-wide reservoir delivery policy.");
		}
		var chosen = targets.ToArray();
		if (chosen.Length == 0) throw new InvalidOperationException("No eligible target is present; no energy was spent.");
		if (chosen.Length > 256) throw new InvalidOperationException("This activation exceeds the 256-target native safety bound.");
		var group = (IPerceivable)new PerceivableGroup(chosen);
		if (spell.SpellEffects.Any(x => x.SaveToXml().Attribute("type")?.Value == "banishtraveller"))
		{
			parameters = [new SpellAdditionalParameter { ParameterName = SpellTravelDestinationRequest.ParameterName, Item = new SpellTravelDestinationRequest(null) }];
			spell.BindInvocationInputs(actor, group, parameters);
			var effect = spell.SpellEffects.OfType<IMagicSpellEffectPreparedSelection>().Single();
			chosen = chosen.Where(target =>
			{
				try { effect.CapturePreparedSelection(actor, target); return true; }
				catch { return false; }
			}).ToArray();
			if (chosen.Length == 0) throw new InvalidOperationException("No eligible traveller has a permitted weighted destination; no energy was spent.");
			group = new PerceivableGroup(chosen);
		}
		return (group, parameters, applications);
	}

	private static void ValidateReservoirAdapter(ChargedMagicDeviceGameItemComponent device, MagicSpell spell)
	{
		var types = spell.SpellEffects.Select(x => (string?)x.SaveToXml().Attribute("type") ?? "").ToArray();
		if (spell.CasterSpellEffects.Any())
			throw new InvalidOperationException("Reservoir delivery does not admit caster effects.");
		var supported = device.Delivery switch
		{
			MagicDeviceDelivery.NativeArea => spell.GradeProfile?.Area is { ChainLightning: null } && spell.Trigger is CastingTriggerCharacter &&
				types.Length > 0 && types.All(x => x is "damage" or "burning") && types.Contains("damage"),
			MagicDeviceDelivery.RoomCorpses => types.SequenceEqual(["animatecorpse"], StringComparer.OrdinalIgnoreCase),
			MagicDeviceDelivery.OtherCharacters => spell.Trigger?.TargetTypes == "character" &&
				(types.SequenceEqual(["banishtraveller"], StringComparer.OrdinalIgnoreCase) ||
					types.SequenceEqual(["damage"], StringComparer.OrdinalIgnoreCase) && spell.SpellEffects.Single() is DamageEffect { IdentitySafeDamage: true }),
			_ => false
		};
		if (!supported) throw new InvalidOperationException("The fixed spell has no safe native adapter for this reservoir delivery policy.");
	}

	private static bool SameBody(ICharacter first, ICharacter second) => first.Body is { } a && second.Body is { } b && a.Id == b.Id;
	private sealed record ReservoirSelection(IMagicSpellEffectPreparedSelection Effect, IPerceivable Recipient,
		IMagicSpellEffectPreparedSelectionToken? Token);
	private static ReservoirSelection[] CaptureReservoirSelections(ICharacter actor, MagicSpell spell, IPerceivable target,
		SpellAdditionalParameter[] parameters, SpellPower power)
	{
		var recipients = target is PerceivableGroup group ? group.Members : [target];
		var selections = new List<ReservoirSelection>();
		foreach (var effect in spell.SpellEffects.OfType<IMagicSpellEffectPreparedSelection>())
		foreach (var recipient in recipients)
		{
			var token = effect.CapturePreparedSelection(actor, recipient);
			if (token is not null) selections.Add(new(effect, recipient, token));
		}
		return selections.ToArray();
	}
	private static bool ConfirmReservoirSelections(ICharacter actor, MagicSpell spell, ReservoirSelection[] selections) =>
		selections.All(x => x.Effect.TryConfirmPreparedSelection(actor, x.Recipient, out _));
}
