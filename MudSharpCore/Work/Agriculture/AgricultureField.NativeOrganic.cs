using System.Globalization;
using MudSharp.Framework;
using MudSharp.Magic.Environment;

#nullable enable

namespace MudSharp.Work.Agriculture;

public partial class AgricultureField
{
	private const int NativeOrganicAccountingVersion = 2;

	private enum NativeRecoveryComponent
	{
		Health,
		Yield,
		Biomass
	}

	private sealed class NativeOrganicAccountingState
	{
		public long Generation { get; set; }
		public long Revision { get; set; }
		public decimal Prepaid { get; set; }
		public decimal HealthRemainder { get; set; }
		public decimal YieldRemainder { get; set; }
		public decimal BiomassRemainder { get; set; }
		public bool IsInvalid { get; set; }
		public string? Diagnostic { get; set; }
		public List<XElement> RawElements { get; } = new();
	}

	private readonly NativeOrganicAccountingState _cropNativeAccounting = new();
	private readonly NativeOrganicAccountingState _woodlandNativeAccounting = new();
	private readonly NativeOrganicAccountingState _pastureNativeAccounting = new();
	private readonly List<XElement> _unknownNativeAccountingElements = new();
	private readonly object _nativeOrganicOwnerSync = new();
	private XElement? _rawInvalidNativeAccountingRoot;
	private bool _nativeOrganicAccountingLoaded;
	private int _nativeOrganicOwnerSyncDepth;
	private bool _nativeOrganicSavePending;
	private bool _pendingPastureAssessment;

	private T SynchronizeNativeOrganicOwner<T>(Func<T> action)
	{
		var markChanged = false;
		var notifyEnvironmentalInputs = false;
		try
		{
			lock (_nativeOrganicOwnerSync)
			{
				_nativeOrganicOwnerSyncDepth++;
				try
				{
					return action();
				}
				finally
				{
					_nativeOrganicOwnerSyncDepth--;
					if (_nativeOrganicOwnerSyncDepth == 0)
					{
						markChanged = _nativeOrganicSavePending;
						_nativeOrganicSavePending = false;
						notifyEnvironmentalInputs = TryTakeEnvironmentalInputNotificationUnsafe();
					}
				}
			}
		}
		finally
		{
			try
			{
				if (markChanged)
				{
					Changed = true;
				}
			}
			finally
			{
				if (notifyEnvironmentalInputs)
				{
					DispatchEnvironmentalInputsChanged();
				}
			}
		}
	}

	private void SynchronizeNativeOrganicOwner(Action action)
	{
		SynchronizeNativeOrganicOwner(() =>
		{
			action();
			return true;
		});
	}

	private void RequestNativeOrganicOwnerSaveUnsafe()
	{
		_nativeOrganicSavePending = true;
	}

	private void RequestEnvironmentalInputNotificationUnsafe()
	{
		if (_environmentalInputNotificationsEnabled)
		{
			_environmentalInputChangePending = true;
		}
	}

	private void InitialiseLegacyNativeOrganicAccounting()
	{
		InitialiseLegacyNativeOrganicSource(_cropNativeAccounting, IsLivingCropSource());
		InitialiseLegacyNativeOrganicSource(_woodlandNativeAccounting, IsLivingWoodlandSource());
		InitialiseLegacyNativeOrganicSource(_pastureNativeAccounting, IsLivingPastureSource());
		_nativeOrganicAccountingLoaded = true;
	}

	private static void InitialiseLegacyNativeOrganicSource(NativeOrganicAccountingState state, bool isPresent)
	{
		state.Generation = isPresent ? 1L : 0L;
		state.Revision = 0L;
		state.Prepaid = 0m;
		state.HealthRemainder = 0m;
		state.YieldRemainder = 0m;
		state.BiomassRemainder = 0m;
		state.IsInvalid = false;
		state.Diagnostic = null;
		state.RawElements.Clear();
	}

	private void LoadNativeOrganicAccounting(XElement fieldRoot)
	{
		InitialiseLegacyNativeOrganicAccounting();
		var root = fieldRoot.Element("NativeOrganicAccounting");
		if (root == null)
		{
			return;
		}

		if (!int.TryParse(root.Attribute("version")?.Value, NumberStyles.None, CultureInfo.InvariantCulture,
			    out var version) || version is not (1 or NativeOrganicAccountingVersion))
		{
			_rawInvalidNativeAccountingRoot = new XElement(root);
			InvalidateAllNativeOrganicAccounting("The saved native organic accounting version is missing or unsupported.");
			return;
		}
		// Version 1 and absent extensions describe retained legacy stock, already assessed.
		var assessment = root.Attribute("pastureAssessment")?.Value;
		if (version == NativeOrganicAccountingVersion && assessment is not ("pending" or "assessed"))
		{
			_rawInvalidNativeAccountingRoot = new XElement(root);
			InvalidateAllNativeOrganicAccounting("The saved pasture assessment state is missing or invalid.");
			return;
		}
		_pendingPastureAssessment = version == NativeOrganicAccountingVersion && assessment == "pending";

		var seenKinds = new HashSet<NativeOrganicSourceKind>();
		var firstElements = new Dictionary<NativeOrganicSourceKind, XElement>();
		foreach (var element in root.Elements())
		{
			if (!element.Name.LocalName.EqualTo("Source") ||
			    !TryParseNativeOrganicKind(element.Attribute("kind")?.Value, out var kind))
			{
				_unknownNativeAccountingElements.Add(new XElement(element));
				continue;
			}

			var state = AccountingFor(kind);
			if (!seenKinds.Add(kind))
			{
				if (state.RawElements.Count == 0 && firstElements.TryGetValue(kind, out var firstElement))
				{
					state.RawElements.Add(new XElement(firstElement));
				}

				state.RawElements.Add(new XElement(element));
				state.IsInvalid = true;
				state.Diagnostic = $"The saved {kind.ToString().ToLowerInvariant()} accounting contains duplicate source entries.";
				continue;
			}

			firstElements[kind] = new XElement(element);

			if (!TryLoadNativeOrganicSource(element, kind, state, out var diagnostic))
			{
				state.RawElements.Add(new XElement(element));
				state.IsInvalid = true;
				state.Diagnostic = diagnostic;
			}
		}

		_nativeOrganicAccountingLoaded = true;
	}

	private static bool TryParseNativeOrganicKind(string? text, out NativeOrganicSourceKind kind)
	{
		if (Enum.TryParse(text, true, out kind) &&
		    kind is NativeOrganicSourceKind.Crop or NativeOrganicSourceKind.Woodland or NativeOrganicSourceKind.Pasture &&
		    string.Equals(Enum.GetName(kind), text, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		kind = default;
		return false;
	}

	private bool TryLoadNativeOrganicSource(XElement element, NativeOrganicSourceKind kind,
		NativeOrganicAccountingState state, out string diagnostic)
	{
		var isLiving = IsLivingSource(kind);
		if (!TryLoadNonNegativeLong(element, "generation", out var generation) ||
		    !TryLoadNonNegativeLong(element, "revision", out var revision) ||
		    (isLiving && generation == 0L))
		{
			diagnostic = $"The saved {kind.ToString().ToLowerInvariant()} accounting has an invalid generation or revision.";
			return false;
		}

		// A valid monotonic identity remains useful to explicit repair even when a fraction is malformed.
		state.Generation = generation;
		state.Revision = revision;
		if (!TryLoadFraction(element, "prepaid", out var prepaid) ||
		    !TryLoadFraction(element, "healthRemainder", out var health) ||
		    !TryLoadFraction(element, "yieldRemainder", out var yield) ||
		    !TryLoadFraction(element, "biomassRemainder", out var biomass) ||
		    (!isLiving && (prepaid != 0m || health != 0m || yield != 0m || biomass != 0m)) ||
		    (kind is NativeOrganicSourceKind.Crop or NativeOrganicSourceKind.Woodland && biomass != 0m) ||
		    (kind == NativeOrganicSourceKind.Pasture && (health != 0m || yield != 0m)))
		{
			diagnostic = $"The saved {kind.ToString().ToLowerInvariant()} accounting has an invalid prepaid fraction or recovery remainder.";
			return false;
		}

		state.Prepaid = prepaid;
		state.HealthRemainder = health;
		state.YieldRemainder = yield;
		state.BiomassRemainder = biomass;
		diagnostic = string.Empty;
		return true;
	}

	private static bool TryLoadNonNegativeLong(XElement element, string attributeName, out long value)
	{
		return long.TryParse(element.Attribute(attributeName)?.Value, NumberStyles.None,
			       CultureInfo.InvariantCulture, out value) && value < long.MaxValue;
	}

	private static bool TryLoadFraction(XElement element, string attributeName, out decimal value)
	{
		return decimal.TryParse(element.Attribute(attributeName)?.Value, NumberStyles.AllowDecimalPoint,
			       CultureInfo.InvariantCulture, out value) && value is >= 0m and < 1m;
	}

	private void InvalidateAllNativeOrganicAccounting(string diagnostic)
	{
		foreach (var kind in FieldNativeOrganicKinds())
		{
			var state = AccountingFor(kind);
			state.IsInvalid = true;
			state.Diagnostic = diagnostic;
		}

		_nativeOrganicAccountingLoaded = true;
	}

	private static IEnumerable<NativeOrganicSourceKind> FieldNativeOrganicKinds()
	{
		yield return NativeOrganicSourceKind.Crop;
		yield return NativeOrganicSourceKind.Woodland;
		yield return NativeOrganicSourceKind.Pasture;
	}

	private XElement SaveNativeOrganicAccounting()
	{
		return SynchronizeNativeOrganicOwner(SaveNativeOrganicAccountingUnsafe);
	}

	private XElement SaveNativeOrganicAccountingUnsafe()
	{
		if (_rawInvalidNativeAccountingRoot != null)
		{
			return new XElement(_rawInvalidNativeAccountingRoot);
		}

		var root = new XElement("NativeOrganicAccounting",
			new XAttribute("version", NativeOrganicAccountingVersion),
			new XAttribute("pastureAssessment", _pendingPastureAssessment ? "pending" : "assessed"));
		foreach (var kind in FieldNativeOrganicKinds())
		{
			var state = AccountingFor(kind);
			if (state.IsInvalid && state.RawElements.Count > 0)
			{
				root.Add(state.RawElements.Select(x => new XElement(x)));
				continue;
			}

			root.Add(SaveNativeOrganicSource(kind, state));
		}

		root.Add(_unknownNativeAccountingElements.Select(x => new XElement(x)));
		return root;
	}

	private static XElement SaveNativeOrganicSource(NativeOrganicSourceKind kind,
		NativeOrganicAccountingState state)
	{
		return new XElement("Source",
			new XAttribute("kind", kind),
			new XAttribute("generation", state.Generation),
			new XAttribute("revision", state.Revision),
			new XAttribute("prepaid", state.Prepaid.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("healthRemainder", state.HealthRemainder.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("yieldRemainder", state.YieldRemainder.ToString(CultureInfo.InvariantCulture)),
			new XAttribute("biomassRemainder", state.BiomassRemainder.ToString(CultureInfo.InvariantCulture)));
	}

	public NativeOrganicSourceSnapshot InspectNativeOrganicSource(NativeOrganicSourceKind kind)
	{
		return SynchronizeNativeOrganicOwner(() => InspectNativeOrganicSourceUnsafe(kind));
	}

	private NativeOrganicSourceSnapshot InspectNativeOrganicSourceUnsafe(NativeOrganicSourceKind kind)
	{
		if (kind is not (NativeOrganicSourceKind.Crop or NativeOrganicSourceKind.Woodland or
		    NativeOrganicSourceKind.Pasture))
		{
			return new NativeOrganicSourceSnapshot(NativeOrganicSourceSelectors.Canonical(kind), kind,
				NativeOrganicSourceStatus.Unauthorised, null, 0.0, 0m, NativeOrganicRecoveryRemainders.Empty,
				0L, null, 0L, CurrentUse, "This native source is not owned by an agriculture field.");
		}

		var state = AccountingFor(kind);
		var isLiving = IsLivingSource(kind);
		var lifecycle = isLiving
			? new NativeOrganicLifecycleIdentity(_cellId, Id, state.Generation, DefinitionIdFor(kind), null, 0, 0L)
			: null;
		var stock = NativeStockFor(kind);
		var status = state.IsInvalid || kind == NativeOrganicSourceKind.Pasture && _pendingPastureAssessment && isLiving
			? NativeOrganicSourceStatus.Invalid
			: !isLiving
				? NativeOrganicSourceStatus.Absent
				: stock > 0
					? NativeOrganicSourceStatus.Available
					: NativeOrganicSourceStatus.Exhausted;
		return new NativeOrganicSourceSnapshot(
			NativeOrganicSourceSelectors.Canonical(kind),
			kind,
			status,
			lifecycle,
			stock,
			state.Prepaid,
			new NativeOrganicRecoveryRemainders(state.HealthRemainder, state.YieldRemainder,
				state.BiomassRemainder),
			state.Revision,
			null,
			0L,
			CurrentUse,
			state.Diagnostic ?? (kind == NativeOrganicSourceKind.Pasture && _pendingPastureAssessment && isLiving
				? "This pasture allocation is still awaiting its initial assessment."
				: !isLiving ? AbsentDiagnostic(kind) : null));
	}

	public bool TryApplyNativeOrganicDebit(NativeOrganicDebitPlan plan, out string reason)
	{
		if (plan == null || plan.Kind is not (NativeOrganicSourceKind.Crop or NativeOrganicSourceKind.Woodland or
		    NativeOrganicSourceKind.Pasture))
		{
			reason = "The native debit plan does not identify an agriculture source.";
			return false;
		}

		using var environmentalChange = BeginEnvironmentalInputChange();
		var applyResult = SynchronizeNativeOrganicOwner(() =>
		{
			var snapshot = InspectNativeOrganicSourceUnsafe(plan.Kind);
			if (!snapshot.IsEligible || snapshot.Lifecycle == null)
			{
				return (false, snapshot.Diagnostic ?? "That native source is no longer eligible.");
			}

			if (!string.Equals(plan.Selector, snapshot.Selector, StringComparison.Ordinal) ||
			    plan.Lifecycle != snapshot.Lifecycle ||
			    plan.SourceRevision != snapshot.SourceRevision ||
			    plan.OpeningPrepaidFraction != snapshot.PrepaidFraction ||
			    plan.ExpectedNativeStock != snapshot.NativeStock ||
			    plan.EnvironmentalProfileId <= 0L || plan.EnvironmentalProfileRevision < 0L)
			{
				return (false, "The native source or environmental profile has changed since this debit was planned.");
			}

			var state = AccountingFor(plan.Kind);
			if (!NativeOrganicAccountingMath.TryPlanInteger((int)snapshot.NativeStock, state.Prepaid,
				    plan.RequestedAmount, out var wholeDebit, out var closingPrepaid, out var arithmeticError) ||
			    wholeDebit != plan.WholeNativeDebit || closingPrepaid != plan.ClosingPrepaidFraction)
			{
				return (false,
					arithmeticError ?? "The native debit arithmetic no longer matches the owner state.");
			}

			SetNativeStock(plan.Kind, (int)snapshot.NativeStock - wholeDebit);
			state.Prepaid = closingPrepaid;
			MarkNativeOrganicSourceChangedUnsafe(plan.Kind);
			return (true, string.Empty);
		});
		reason = applyResult.Item2;
		return applyResult.Item1;
	}

	public bool RepairNativeOrganicAccounting(NativeOrganicSourceKind? kind, out string result)
	{
		if (kind == NativeOrganicSourceKind.Forage)
		{
			result = "Forage accounting belongs to the cell rather than the agriculture field.";
			return false;
		}

		var repairKinds = kind.HasValue ? new[] { kind.Value } : FieldNativeOrganicKinds().ToArray();
		if (repairKinds.Any(x => x is not (NativeOrganicSourceKind.Crop or NativeOrganicSourceKind.Woodland or
		    NativeOrganicSourceKind.Pasture)))
		{
			result = "That accounting source is not owned by this agriculture field.";
			return false;
		}

		var repairResult = SynchronizeNativeOrganicOwner(() =>
		{
			if (_rawInvalidNativeAccountingRoot != null && kind.HasValue)
			{
				return (false, "The accounting root has an unsupported version. Repair all field sources together.");
			}

			var repaired = new List<string>();
			foreach (var repairKind in repairKinds)
			{
				var state = AccountingFor(repairKind);
				if (!state.IsInvalid && _rawInvalidNativeAccountingRoot == null)
				{
					continue;
				}

				var retainedGeneration = state.Generation;
				var retainedRevision = state.Revision;
				InitialiseLegacyNativeOrganicSource(state, IsLivingSource(repairKind));
				state.Generation = Math.Max(state.Generation, retainedGeneration);
				state.Revision = retainedRevision;
				repaired.Add(repairKind.ToString().ToLowerInvariant());
			}

			if (_rawInvalidNativeAccountingRoot != null)
			{
				_rawInvalidNativeAccountingRoot = null;
			}

			if (repaired.Count == 0)
			{
				return (true, "There is no malformed native organic accounting to repair.");
			}

			RequestNativeOrganicOwnerSaveUnsafe();
			RequestEnvironmentalInputNotificationUnsafe();
			return (true,
				$"Discarded malformed accounting for {repaired.ListToString()} without changing native stock.");
		});
		result = repairResult.Item2;
		return repairResult.Item1;
	}

	private NativeOrganicAccountingState AccountingFor(NativeOrganicSourceKind kind)
	{
		return kind switch
		{
			NativeOrganicSourceKind.Crop => _cropNativeAccounting,
			NativeOrganicSourceKind.Woodland => _woodlandNativeAccounting,
			NativeOrganicSourceKind.Pasture => _pastureNativeAccounting,
			_ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
		};
	}

	private bool IsLivingSource(NativeOrganicSourceKind kind)
	{
		return kind switch
		{
			NativeOrganicSourceKind.Crop => IsLivingCropSource(),
			NativeOrganicSourceKind.Woodland => IsLivingWoodlandSource(),
			NativeOrganicSourceKind.Pasture => IsLivingPastureSource(),
			_ => false
		};
	}

	private bool IsLivingCropSource()
	{
		return CurrentUse is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard &&
		       _cropDefinitionId > 0L && _cropHealth > 0 &&
		       CropStage is not (AgricultureCropStage.None or AgricultureCropStage.Failed);
	}

	private bool IsLivingWoodlandSource()
	{
		return CurrentUse == AgricultureFieldUse.Woodland && _woodlandDefinitionId > 0L && _woodlandHealth > 0;
	}

	private bool IsLivingPastureSource()
	{
		return CurrentUse == AgricultureFieldUse.Pasture;
	}

	private int NativeStockFor(NativeOrganicSourceKind kind)
	{
		return kind switch
		{
			NativeOrganicSourceKind.Crop => _cropYieldPotential,
			NativeOrganicSourceKind.Woodland => _woodlandYieldPotential,
			NativeOrganicSourceKind.Pasture => Pasture,
			_ => 0
		};
	}

	private long DefinitionIdFor(NativeOrganicSourceKind kind)
	{
		return kind switch
		{
			NativeOrganicSourceKind.Crop => _cropDefinitionId,
			NativeOrganicSourceKind.Woodland => _woodlandDefinitionId,
			_ => 0L
		};
	}

	private string AbsentDiagnostic(NativeOrganicSourceKind kind)
	{
		return kind switch
		{
			NativeOrganicSourceKind.Crop => "There is no eligible living crop or orchard in this field.",
			NativeOrganicSourceKind.Woodland => "There is no eligible living woodland stand in this field.",
			NativeOrganicSourceKind.Pasture => "This field is not currently in pasture use.",
			_ => "That source is absent."
		};
	}

	private void SetNativeStock(NativeOrganicSourceKind kind, int value)
	{
		value = value.ClampScore();
		switch (kind)
		{
			case NativeOrganicSourceKind.Crop:
				_cropYieldPotential = value;
				break;
			case NativeOrganicSourceKind.Woodland:
				_woodlandYieldPotential = value;
				break;
			case NativeOrganicSourceKind.Pasture:
				_pasture = value;
				break;
		}
	}

	private void MarkNativeOrganicSourceChanged(NativeOrganicSourceKind kind)
	{
		SynchronizeNativeOrganicOwner(() => MarkNativeOrganicSourceChangedUnsafe(kind));
	}

	private void MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind kind)
	{
		if (!_nativeOrganicAccountingLoaded)
		{
			RequestEnvironmentalInputNotificationUnsafe();
			return;
		}

		var state = AccountingFor(kind);
		if (state.IsInvalid)
		{
			RequestNativeOrganicOwnerSaveUnsafe();
			RequestEnvironmentalInputNotificationUnsafe();
			return;
		}

		if (state.Revision == long.MaxValue)
		{
			state.RawElements.Clear();
			state.RawElements.Add(SaveNativeOrganicSource(kind, state));
			state.IsInvalid = true;
			state.Diagnostic = $"The {kind.ToString().ToLowerInvariant()} source revision is exhausted.";
			RequestNativeOrganicOwnerSaveUnsafe();
			RequestEnvironmentalInputNotificationUnsafe();
			return;
		}

		state.Revision++;
		RequestNativeOrganicOwnerSaveUnsafe();
		RequestEnvironmentalInputNotificationUnsafe();
	}

	private void StartNativeOrganicLifecycle(NativeOrganicSourceKind kind)
	{
		SynchronizeNativeOrganicOwner(() => StartNativeOrganicLifecycleUnsafe(kind));
	}

	private void StartNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind kind)
	{
		var state = AccountingFor(kind);
		if (state.IsInvalid)
		{
			return;
		}

		if (state.Generation == long.MaxValue)
		{
			state.RawElements.Clear();
			state.RawElements.Add(SaveNativeOrganicSource(kind, state));
			state.IsInvalid = true;
			state.Diagnostic = $"The {kind.ToString().ToLowerInvariant()} lifecycle generation is exhausted.";
			RequestNativeOrganicOwnerSaveUnsafe();
			RequestEnvironmentalInputNotificationUnsafe();
			return;
		}

		state.Generation++;
		state.Prepaid = 0m;
		state.HealthRemainder = 0m;
		state.YieldRemainder = 0m;
		state.BiomassRemainder = 0m;
		MarkNativeOrganicSourceChangedUnsafe(kind);
	}

	private void EndNativeOrganicLifecycle(NativeOrganicSourceKind kind)
	{
		SynchronizeNativeOrganicOwner(() => EndNativeOrganicLifecycleUnsafe(kind));
	}

	private void EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind kind)
	{
		var state = AccountingFor(kind);
		if (state.IsInvalid)
		{
			return;
		}

		state.Prepaid = 0m;
		state.HealthRemainder = 0m;
		state.YieldRemainder = 0m;
		state.BiomassRemainder = 0m;
		MarkNativeOrganicSourceChangedUnsafe(kind);
	}

	private void TransitionNativeOrganicUse(AgricultureFieldUse newUse, int establishmentIncrease = 0)
	{
		var firstEstablishment = newUse == AgricultureFieldUse.Pasture && _pendingPastureAssessment;
		SynchronizeNativeOrganicOwner(() => TransitionNativeOrganicUseUnsafe(newUse));
		if (!firstEstablishment)
		{
			return;
		}

		var baseline = SynchronizeNativeOrganicOwner(() =>
		{
			var staged = _pasture;
			_pasture = 0;
			_pendingPastureAssessment = false;
			MarkNativeOrganicSourceChangedUnsafe(NativeOrganicSourceKind.Pasture);
			return staged + establishmentIncrease;
		});
		var assessed = ApplyPastureIncrease(baseline, NativeOrganicPenaltyChannel.PastureInitialisation);
		if (assessed > 0)
		{
			AdjustScore(AgricultureScoreType.Pasture, assessed);
		}
	}

	private void TransitionNativeOrganicUseUnsafe(AgricultureFieldUse newUse)
	{
		var oldUse = CurrentUse;
		if (oldUse is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard &&
		    newUse is not (AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard))
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
		}

		if (oldUse == AgricultureFieldUse.Woodland && newUse != AgricultureFieldUse.Woodland)
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Woodland);
		}

		if (oldUse == AgricultureFieldUse.Pasture && newUse != AgricultureFieldUse.Pasture)
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Pasture);
		}

		CurrentUse = newUse;
		if (oldUse is not (AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard) &&
		    newUse is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard && _cropDefinitionId > 0L)
		{
			StartNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
		}

		if (oldUse != AgricultureFieldUse.Woodland && newUse == AgricultureFieldUse.Woodland &&
		    _woodlandDefinitionId > 0L)
		{
			StartNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Woodland);
		}

		if (oldUse != AgricultureFieldUse.Pasture && newUse == AgricultureFieldUse.Pasture)
		{
			StartNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Pasture);
		}
	}

	private void BeginNativeOrganicReplacement(NativeOrganicSourceKind kind, AgricultureFieldUse newUse)
	{
		SynchronizeNativeOrganicOwner(() => BeginNativeOrganicReplacementUnsafe(kind, newUse));
	}

	private void BeginNativeOrganicReplacementUnsafe(NativeOrganicSourceKind kind, AgricultureFieldUse newUse)
	{
		if (CurrentUse is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard)
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Crop);
		}

		if (CurrentUse == AgricultureFieldUse.Woodland)
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Woodland);
		}

		if (CurrentUse == AgricultureFieldUse.Pasture)
		{
			EndNativeOrganicLifecycleUnsafe(NativeOrganicSourceKind.Pasture);
		}

		CurrentUse = newUse;
		StartNativeOrganicLifecycleUnsafe(kind);
	}

	private int ApplyPositiveNativeIncrease(NativeOrganicSourceKind kind, NativeOrganicPenaltyChannel channel,
		int baseline, int current, int capacity, NativeRecoveryComponent component, int sameOperationLoss = 0)
	{
		if (baseline <= 0)
		{
			return 0;
		}

		var input = SynchronizeNativeOrganicOwner(() =>
		{
			var state = AccountingFor(kind);
			var currentValue = NativeRecoveryCurrentFor(kind, component);
			if (capacity <= currentValue + sameOperationLoss)
			{
				SetNativeRecoveryRemainderUnsafe(kind, component, 0m);
				return (AtCapacity: true, Generation: state.Generation,
					Context: default(NativeOrganicPenaltyContext));
			}

			return (AtCapacity: false, Generation: state.Generation,
				Context: BuildNativeOrganicPenaltyContext(kind, capacity, baseline));
		});
		if (input.AtCapacity)
		{
			return 0;
		}

		var evaluation = NativeOrganicPenaltyEvaluation.Neutral;
		try
		{
			evaluation = Gameworld.EnvironmentalMagic?.EvaluateOrganicPenalty(Cell, channel, input.Context!) ??
			             NativeOrganicPenaltyEvaluation.Neutral;
		}
		catch
		{
			evaluation = NativeOrganicPenaltyEvaluation.Invalid("The ecological penalty could not be evaluated.");
		}

		if (!evaluation.IsConfigured)
		{
			evaluation = NativeOrganicPenaltyEvaluation.Neutral;
		}

		if (!evaluation.IsValid || !double.IsFinite(evaluation.Factor) || evaluation.Factor is < 0.0 or > 1.0)
		{
			SetNativeRecoveryRemainder(kind, component, 0m);
			return 0;
		}

		return SynchronizeNativeOrganicOwner(() =>
		{
			var state = AccountingFor(kind);
			if (state.Generation != input.Generation)
			{
				return 0;
			}

			var currentValue = NativeRecoveryCurrentFor(kind, component);
			if (capacity <= currentValue + sameOperationLoss)
			{
				SetNativeRecoveryRemainderUnsafe(kind, component, 0m);
				return 0;
			}

			if (state.IsInvalid)
			{
				return evaluation.IsConfigured ? 0 : Math.Min(capacity - currentValue - sameOperationLoss, baseline);
			}

			var progress = GetNativeRecoveryRemainder(state, component) + baseline * (decimal)evaluation.Factor;
			var whole = (int)decimal.Floor(progress);
			var applied = Math.Min(capacity - currentValue - sameOperationLoss, whole);
			var remainder = currentValue + sameOperationLoss + applied >= capacity || applied < whole ? 0m : progress - whole;
			SetNativeRecoveryRemainderUnsafe(kind, component, remainder);
			return applied;
		});
	}

	private NativeOrganicPenaltyContext BuildNativeOrganicPenaltyContext(NativeOrganicSourceKind kind,
		double capacity, double baseline) => new(kind, NativeOrganicSourceSelectors.Canonical(kind),
		NativeStockFor(kind),
		kind == NativeOrganicSourceKind.Crop ? _cropHealth :
		kind == NativeOrganicSourceKind.Woodland ? _woodlandHealth : _condition,
		kind == NativeOrganicSourceKind.Crop ? _cropYieldPotential :
		kind == NativeOrganicSourceKind.Woodland ? _woodlandYieldPotential : _pasture,
		capacity, _condition, baseline);

	public NativeOrganicPenaltyContext? InspectCurrentOrganicRecoveryContext(NativeOrganicPenaltyChannel channel)
	{
		var baseline = 0;
		var headroom = 0;
		NativeOrganicSourceKind kind;
		switch (channel)
		{
			case NativeOrganicPenaltyChannel.CropHealthRecovery:
			case NativeOrganicPenaltyChannel.CropYieldRecovery:
				var crop = CurrentCrop;
				if (crop == null || CropStage is AgricultureCropStage.Failed or AgricultureCropStage.Overripe)
					return null;
				var (stressed, pollinationHealth, pollinationYield) = CurrentCropTickContributions(crop,
					CropStage);
				if (stressed) return null;
				kind = NativeOrganicSourceKind.Crop;
				if (channel == NativeOrganicPenaltyChannel.CropHealthRecovery)
				{
					baseline = 1 + Math.Max(0, pollinationHealth);
					headroom = 100 - _cropHealth;
				}
				else
				{
					var nutrientYield = Math.Sign(Nutrients - 50);
					baseline = Math.Max(0, nutrientYield) + Math.Max(0, pollinationYield);
					headroom = 100 - (_cropYieldPotential + Math.Min(0, nutrientYield));
					if (nutrientYield > 0) headroom = 100 - _cropYieldPotential;
				}
				break;
			case NativeOrganicPenaltyChannel.WoodlandHealthRecovery:
			case NativeOrganicPenaltyChannel.WoodlandYieldRecovery:
				if (CurrentWoodland == null || _woodlandHealth <= 0 || Moisture < 15 || Moisture > 90 ||
				    Topsoil < 25 || Pests > 80) return null;
				kind = NativeOrganicSourceKind.Woodland;
				if (channel == NativeOrganicPenaltyChannel.WoodlandYieldRecovery &&
				    _woodlandGrowthDays + 1 <= CurrentWoodland.EstablishmentDays) return null;
				baseline = 1;
				headroom = channel == NativeOrganicPenaltyChannel.WoodlandHealthRecovery
					? 100 - _woodlandHealth : 100 - _woodlandYieldPotential;
				break;
			default:
				// Pasture recovery requires a future operation's positive score delta.
				return null;
		}

		return baseline <= 0 || headroom <= 0
			? null
			: SynchronizeNativeOrganicOwner(() => BuildNativeOrganicPenaltyContext(kind, 100, baseline));
	}

	private int NativeRecoveryCurrentFor(NativeOrganicSourceKind kind, NativeRecoveryComponent component)
	{
		return component switch
		{
			NativeRecoveryComponent.Health when kind == NativeOrganicSourceKind.Crop => _cropHealth,
			NativeRecoveryComponent.Health when kind == NativeOrganicSourceKind.Woodland => _woodlandHealth,
			NativeRecoveryComponent.Yield when kind == NativeOrganicSourceKind.Crop => _cropYieldPotential,
			NativeRecoveryComponent.Yield when kind == NativeOrganicSourceKind.Woodland => _woodlandYieldPotential,
			NativeRecoveryComponent.Biomass when kind == NativeOrganicSourceKind.Pasture => _pasture,
			_ => 0
		};
	}

	private static decimal GetNativeRecoveryRemainder(NativeOrganicAccountingState state,
		NativeRecoveryComponent component)
	{
		return component switch
		{
			NativeRecoveryComponent.Health => state.HealthRemainder,
			NativeRecoveryComponent.Yield => state.YieldRemainder,
			NativeRecoveryComponent.Biomass => state.BiomassRemainder,
			_ => 0m
		};
	}

	private void SetNativeRecoveryRemainder(NativeOrganicSourceKind kind, NativeRecoveryComponent component,
		decimal value)
	{
		SynchronizeNativeOrganicOwner(() => SetNativeRecoveryRemainderUnsafe(kind, component, value));
	}

	private void SetNativeRecoveryRemainderUnsafe(NativeOrganicSourceKind kind, NativeRecoveryComponent component,
		decimal value)
	{
		var state = AccountingFor(kind);
		if (state.IsInvalid || value is < 0m or >= 1m || GetNativeRecoveryRemainder(state, component) == value)
		{
			return;
		}

		switch (component)
		{
			case NativeRecoveryComponent.Health:
				state.HealthRemainder = value;
				break;
			case NativeRecoveryComponent.Yield:
				state.YieldRemainder = value;
				break;
			case NativeRecoveryComponent.Biomass:
				state.BiomassRemainder = value;
				break;
		}

		MarkNativeOrganicSourceChangedUnsafe(kind);
	}

	private int ApplyCropHealthIncrease(int baseline, NativeOrganicPenaltyChannel channel =
		NativeOrganicPenaltyChannel.CropHealthRecovery, int sameOperationLoss = 0)
	{
		return ApplyPositiveNativeIncrease(NativeOrganicSourceKind.Crop, channel, baseline, _cropHealth, 100,
			NativeRecoveryComponent.Health, sameOperationLoss);
	}

	private int ApplyCropYieldIncrease(int baseline, NativeOrganicPenaltyChannel channel =
		NativeOrganicPenaltyChannel.CropYieldRecovery, int sameOperationLoss = 0)
	{
		return ApplyPositiveNativeIncrease(NativeOrganicSourceKind.Crop, channel, baseline, _cropYieldPotential,
			100, NativeRecoveryComponent.Yield, sameOperationLoss);
	}

	private int ApplyWoodlandHealthIncrease(int baseline, NativeOrganicPenaltyChannel channel =
		NativeOrganicPenaltyChannel.WoodlandHealthRecovery)
	{
		return ApplyPositiveNativeIncrease(NativeOrganicSourceKind.Woodland, channel, baseline, _woodlandHealth,
			100, NativeRecoveryComponent.Health);
	}

	private int ApplyWoodlandYieldIncrease(int baseline, NativeOrganicPenaltyChannel channel =
		NativeOrganicPenaltyChannel.WoodlandYieldRecovery)
	{
		return ApplyPositiveNativeIncrease(NativeOrganicSourceKind.Woodland, channel, baseline,
			_woodlandYieldPotential, 100, NativeRecoveryComponent.Yield);
	}

	private int ApplyPastureIncrease(int baseline, NativeOrganicPenaltyChannel channel =
		NativeOrganicPenaltyChannel.PastureRecovery)
	{
		return ApplyPositiveNativeIncrease(NativeOrganicSourceKind.Pasture, channel, baseline, Pasture, 100,
			NativeRecoveryComponent.Biomass);
	}

}
