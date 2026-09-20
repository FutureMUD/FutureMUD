#nullable enable

using MudSharp.Construction;

namespace MudSharp.Magic.Environment;

public sealed partial class EnvironmentalMagicCoordinator
{
	/// <summary>
	/// Keeps a Land payment's validated input state together until all of its own source changes finish.
	/// Ordinary single-source mutations still use their independent current-policy checks.
	/// </summary>
	public bool TryApplyLandDebitGroup(ICell cell, IReadOnlyList<EnvironmentalLandAmbientDebit> ambient,
		IReadOnlyList<NativeOrganicDebitPlan> native, out IReadOnlyList<long> appliedAmbient,
		out IReadOnlyList<NativeOrganicDebitPlan> appliedNative, out string? error)
	{
		appliedAmbient = [];
		appliedNative = [];
		if (_disposed || cell is not Cell concrete || concrete.Id <= 0 ||
		    !ReferenceEquals(concrete.Gameworld, _world) || ambient.Count + native.Count is < 1 or > 16 ||
		    ambient.Select(x => x.Resource.Id).Distinct().Count() != ambient.Count ||
		    native.Select(x => x.Selector).Distinct(StringComparer.Ordinal).Count() != native.Count ||
		    _evaluating.Contains(cell.Id) || concrete.PendingEnvironmentalOperationId is not null)
		{
			error = "A bounded, distinct, currently available Land source group is required.";
			return false;
		}
		var profileView = InspectOrganicProfile(cell);
		if (!profileView.ProfileId.HasValue || !profileView.HasOrganicConfiguration ||
		    profileView.HasPendingOperation || profileView.Errors.Count > 0)
		{
			error = "The Land environmental profile is no longer valid.";
			return false;
		}
		var outputs = new List<(EnvironmentalLandAmbientDebit Debit, EnvironmentalResourceSnapshot Snapshot)>();
		foreach (EnvironmentalLandAmbientDebit debit in ambient)
		{
			if (debit.Resource is null || !double.IsFinite(debit.Amount) || debit.Amount <= 0.0 ||
			    !TryInspectLandResource(cell, debit.Resource, out EnvironmentalResourceSnapshot output) ||
			    !output.IsValid || !double.IsFinite(output.Balance) || !double.IsFinite(output.Maximum) ||
			    debit.Amount > Math.Min(output.Balance, output.Maximum))
			{
				error = $"The complete recorded ambient allocation for {debit.Resource?.Id} is unavailable.";
				return false;
			}
			outputs.Add((debit, output));
		}
		foreach (NativeOrganicDebitPlan entry in native)
		{
			if (!TryPlanOrganicDebit(cell, entry.Selector, (double)entry.RequestedAmount,
			    out NativeOrganicDebitPlan current, out error) || current != entry)
			{
				error ??= "A native source changed before the complete Land group could be validated.";
				return false;
			}
		}
		if (EffectiveProfileId(concrete) != profileView.ProfileId ||
		    Profile(profileView.ProfileId.Value)?.Revision != profileView.Revision)
		{
			error = "The Land profile changed during group validation.";
			return false;
		}
		Register(cell);
		Registration registration = _registered[cell.Id];
		double now = Now;
		AdvancePlan settlement = ProjectSettlement(registration, now);
		var paidAmbient = new List<long>(ambient.Count);
		try
		{
			// A changed physical balance between validation and payment is external, not a
			// consequence of this group's planned debits. All checks happen before settlement.
			foreach (var (debit, snapshot) in outputs)
			{
				if (concrete.MagicResourceAmounts.GetValueOrDefault(debit.Resource) != snapshot.Balance)
				{
					error = "An ambient owner changed during Land group validation.";
					return false;
				}
			}
			ApplySettlement(registration, settlement, now, true);
			foreach (var (debit, snapshot) in outputs)
			{
				double balance = Math.Min(concrete.MagicResourceAmounts.GetValueOrDefault(debit.Resource), snapshot.Maximum);
				if (balance < debit.Amount || !double.IsFinite(balance - debit.Amount))
				{
					error = "An ambient owner changed during the validated Land group.";
					return false;
				}
				if (concrete.SetEnvironmentalResource(debit.Resource, balance - debit.Amount, true)) CountWrite();
				paidAmbient.Add(debit.Resource.Id);
			}
			if (native.Count > 0 && !ApplyOrganicDebitBatch(cell, native, false, out appliedNative, out error))
			{
				return false;
			}
			error = null;
			return true;
		}
		catch (Exception ex)
		{
			error = $"The validated Land source group became uncertain: {ex.Message}";
			return false;
		}
		finally
		{
			appliedAmbient = paidAmbient.AsReadOnly();
			// A newly invalid future formula is observed from the resulting state, but it
			// cannot revoke a payment that the complete group had already authorised.
			if (paidAmbient.Count > 0) Recheck(registration, Now, true);
		}
	}
}
