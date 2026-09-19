#nullable enable

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.Magic.Generators;
using MudSharp.Work.Agriculture;

namespace MudSharp.Magic.Environment;

public sealed partial class EnvironmentalMagicCoordinator
{
	private readonly Dictionary<long, double> _lastLoggedOrganic = [];

	public IReadOnlyList<NativeOrganicSourceSnapshot> InspectOrganicSources(ICell cell)
	{
		if (cell is not Cell concrete || concrete.Id <= 0 || !ReferenceEquals(concrete.Gameworld, _world))
		{
			return Array.Empty<NativeOrganicSourceSnapshot>();
		}
		var profileId = EffectiveProfileId(concrete);
		if (!profileId.HasValue)
		{
			return Array.Empty<NativeOrganicSourceSnapshot>();
		}
		var profile = Profile(profileId.Value);
		if (profile is null)
		{
			return Array.Empty<NativeOrganicSourceSnapshot>();
		}
		return profile.OrganicSources
			.Select(source => WithOrganicConversionValidity(cell, InspectOrganicDeclaration(cell, profile, source)))
			.ToList().AsReadOnly();
	}

	public NativeOrganicSourceSnapshot InspectOrganicSource(ICell cell, string selector)
	{
		if (!TryCanonicalOrganicSelector(selector, out var canonical, out var kind, out _))
		{
			return OrganicFailure(cell, selector, kind, NativeOrganicSourceStatus.Invalid,
				"Specify crop, woodland, pasture, or forage:<yield-key>.");
		}
		if (cell is not Cell concrete || concrete.Id <= 0 || !ReferenceEquals(concrete.Gameworld, _world))
		{
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Indeterminate,
				"Native organic sources require a registered physical cell in this gameworld.");
		}
		var profileId = EffectiveProfileId(concrete);
		if (!profileId.HasValue)
		{
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Unauthorised,
				"The cell has no effective environmental profile authorising this source.");
		}
		var profile = Profile(profileId.Value);
		if (profile is null)
		{
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Invalid,
				$"Environmental profile #{profileId.Value} is missing or has the wrong type.", profileId);
		}
		var declarations = profile.OrganicSources.Where(x => x.Selector.EqualTo(canonical)).ToList();
		var sourceError = OrganicSourceValidationError(profile, canonical);
		if (declarations.Count == 0)
		{
			if (sourceError is not null)
			{
				return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Invalid, sourceError,
					profile.Id, profile.Revision);
			}
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Unauthorised,
				$"Environmental profile #{profile.Id} does not authorise {canonical}.", profile.Id, profile.Revision);
		}
		if (declarations.Count > 1 || sourceError is not null)
		{
			var diagnostic = declarations.Count > 1
				? $"Environmental profile #{profile.Id} has duplicate {canonical} declarations."
				: sourceError!;
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Invalid, diagnostic,
				profile.Id, profile.Revision);
		}
		return WithOrganicConversionValidity(cell, InspectOrganicDeclaration(cell, profile, declarations[0]));
	}

	private NativeOrganicSourceSnapshot WithOrganicConversionValidity(ICell cell,
		NativeOrganicSourceSnapshot source)
	{
		if (!source.IsEligible)
		{
			return source;
		}
		var profile = source.EnvironmentalProfileId.HasValue
			? Profile(source.EnvironmentalProfileId.Value)
			: null;
		if (profile is null)
		{
			return source with
			{
				Status = NativeOrganicSourceStatus.Invalid,
				Diagnostic = "The source's environmental profile is no longer available."
			};
		}
		var channels = source.Kind switch
		{
			NativeOrganicSourceKind.Forage => new[] { NativeOrganicPenaltyChannel.ForageReplenishment },
			NativeOrganicSourceKind.Crop => new[]
			{
				NativeOrganicPenaltyChannel.CropHealthRecovery,
				NativeOrganicPenaltyChannel.CropYieldRecovery
			},
			NativeOrganicSourceKind.Woodland => new[]
			{
				NativeOrganicPenaltyChannel.WoodlandHealthRecovery,
				NativeOrganicPenaltyChannel.WoodlandYieldRecovery
			},
			NativeOrganicSourceKind.Pasture => new[] { NativeOrganicPenaltyChannel.PastureRecovery },
			_ => Array.Empty<NativeOrganicPenaltyChannel>()
		};
		foreach (var channel in channels)
		{
			if (!profile.OrganicPenalties.Any(penalty => penalty.Channel == channel))
			{
				continue;
			}
			NativeOrganicPenaltyEvaluation evaluation;
			try
			{
				evaluation = InspectOrganicPenaltyFactor(cell, source, channel);
			}
			catch (Exception exception)
			{
				return source with
				{
					Status = NativeOrganicSourceStatus.Invalid,
					Diagnostic = $"{channel.DescribeEnum()}: {exception.Message}"
				};
			}
			if (evaluation.IsConfigured && (!evaluation.IsValid ||
			    !double.IsFinite(evaluation.Factor) || evaluation.Factor is < 0.0 or > 1.0))
			{
				return source with
				{
					Status = NativeOrganicSourceStatus.Invalid,
					Diagnostic = $"{channel.DescribeEnum()}: {evaluation.Error ?? "Invalid organic penalty."}"
				};
			}
		}
		return source;
	}

	private NativeOrganicSourceSnapshot InspectOrganicDeclaration(ICell cell, IEnvironmentalMagicProfile profile,
		NativeOrganicSourceDeclaration declaration)
	{
		if (!TryCanonicalOrganicSelector(declaration.Selector, out var canonical, out var kind, out var forageKey) ||
		    kind != declaration.Kind)
		{
			return OrganicFailure(cell, declaration.Selector, declaration.Kind, NativeOrganicSourceStatus.Invalid,
				"The environmental profile contains a malformed native source declaration.", profile.Id, profile.Revision);
		}
		if (OrganicSourceValidationError(profile, declaration) is { } sourceError)
		{
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Invalid,
				sourceError, profile.Id, profile.Revision);
		}

		if (kind == NativeOrganicSourceKind.Forage)
		{
			if (!cell.TryPeekForagableYield(forageKey!, out NativeForageYieldSnapshot forage))
			{
				return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Absent,
					$"The effective forage profile has no yield key '{forageKey}'.", profile.Id, profile.Revision);
			}
			if (!double.IsFinite(forage.Stock) || forage.Stock < 0.0 || !double.IsFinite(forage.Maximum) || forage.Maximum < 0.0)
			{
				return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Invalid,
					"The native forage owner reported invalid stock or capacity.", profile.Id, profile.Revision);
			}
			var lifecycle = new NativeOrganicLifecycleIdentity(cell.Id, null, 0, 0, forage.ProfileId,
				forage.ProfileRevision, forage.DefinitionRevision);
			return new NativeOrganicSourceSnapshot(canonical, kind,
				forage.Stock > 0.0 ? NativeOrganicSourceStatus.Available : NativeOrganicSourceStatus.Exhausted,
				lifecycle, forage.Stock, 0m, NativeOrganicRecoveryRemainders.Empty, forage.SourceRevision,
				profile.Id, profile.Revision, null, null);
		}

		var field = FieldFor(cell);
		if (field is null)
		{
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Absent,
				"This physical cell has no indexed agriculture field.", profile.Id, profile.Revision);
		}
		NativeOrganicSourceSnapshot owner;
		try
		{
			owner = field.InspectNativeOrganicSource(kind);
		}
		catch (Exception exception)
		{
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Indeterminate,
				$"The native field source could not be inspected: {exception.Message}", profile.Id, profile.Revision);
		}
		if (owner.Lifecycle is { } ownerLifecycle &&
		    (ownerLifecycle.CellId != cell.Id || ownerLifecycle.FieldId != field.Id))
		{
			return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Invalid,
				"The native field source returned a lifecycle identity for a different owner.", profile.Id, profile.Revision);
		}
		if (owner.Status is NativeOrganicSourceStatus.Available or NativeOrganicSourceStatus.Exhausted)
		{
			if (!owner.FieldUse.HasValue || owner.Lifecycle is null)
			{
				return OrganicFailure(cell, canonical, kind, NativeOrganicSourceStatus.Invalid,
					"An eligible native field source did not report its use and lifecycle.", profile.Id, profile.Revision);
			}
			if (declaration.AllowedFieldUses.Count > 0 && !declaration.AllowedFieldUses.Contains(owner.FieldUse.Value))
			{
				return owner with
				{
					Selector = canonical,
					Status = NativeOrganicSourceStatus.Absent,
					EnvironmentalProfileId = profile.Id,
					EnvironmentalProfileRevision = profile.Revision,
					Diagnostic = $"The current {owner.FieldUse.Value.DescribeEnum()} field use is outside this source declaration."
				};
			}
			if (declaration.DefinitionIds.Count > 0 && !declaration.DefinitionIds.Contains(owner.Lifecycle.DefinitionId))
			{
				return owner with
				{
					Selector = canonical,
					Status = NativeOrganicSourceStatus.Absent,
					EnvironmentalProfileId = profile.Id,
					EnvironmentalProfileRevision = profile.Revision,
					Diagnostic = $"Vegetation definition #{owner.Lifecycle.DefinitionId} is outside this source declaration."
				};
			}
		}
		return owner with
		{
			Selector = canonical,
			Kind = kind,
			EnvironmentalProfileId = profile.Id,
			EnvironmentalProfileRevision = profile.Revision
		};
	}

	public bool TryPlanOrganicDebit(ICell cell, string selector, double amount, out NativeOrganicDebitPlan plan,
		out string? error)
	{
		plan = null!;
		if (!NativeOrganicAccountingMath.TryAmount(amount, out var requested, out error))
		{
			return false;
		}
		var snapshot = InspectOrganicSource(cell, selector);
		if (!snapshot.IsEligible || snapshot.Lifecycle is null || !snapshot.EnvironmentalProfileId.HasValue)
		{
			error = snapshot.Diagnostic ?? $"Native source {snapshot.Selector} is {snapshot.Status.DescribeEnum()}.";
			return false;
		}

		int wholeDebit;
		decimal closingPrepaid;
		if (snapshot.Kind == NativeOrganicSourceKind.Forage)
		{
			if (!double.IsFinite(snapshot.NativeStock) || (double)requested > snapshot.NativeStock)
			{
				error = "The forage source does not have the full requested native amount.";
				return false;
			}
			wholeDebit = 0;
			closingPrepaid = 0m;
		}
		else
		{
			if (!TryIntegerStock(snapshot.NativeStock, out var stock, out error) ||
			    !NativeOrganicAccountingMath.TryPlanInteger(stock, snapshot.PrepaidFraction, requested,
				    out wholeDebit, out closingPrepaid, out error))
			{
				return false;
			}
		}

		plan = new NativeOrganicDebitPlan(snapshot.Selector, snapshot.Kind, snapshot.Lifecycle,
			snapshot.SourceRevision, snapshot.EnvironmentalProfileId.Value, snapshot.EnvironmentalProfileRevision,
			requested, snapshot.PrepaidFraction, wholeDebit, closingPrepaid, snapshot.NativeStock);
		error = null;
		return true;
	}

	public bool TryApplyOrganicDebit(ICell cell, NativeOrganicDebitPlan plan,
		out NativeOrganicSourceSnapshot result, out string? error)
	{
		result = InspectOrganicSource(cell, plan?.Selector ?? string.Empty);
		if (plan is null)
		{
			error = "A native organic debit plan is required.";
			return false;
		}
		if (_evaluating.Contains(cell.Id))
		{
			_recursive.Add(cell.Id);
			error = "Environmental input progs must be read-only.";
			return false;
		}
		if (!result.IsEligible || result.Lifecycle is null ||
		    !string.Equals(result.Selector, plan.Selector, StringComparison.Ordinal) || result.Kind != plan.Kind ||
		    result.EnvironmentalProfileId != plan.EnvironmentalProfileId ||
		    result.EnvironmentalProfileRevision != plan.EnvironmentalProfileRevision ||
		    result.SourceRevision != plan.SourceRevision || result.Lifecycle != plan.Lifecycle ||
		    result.PrepaidFraction != plan.OpeningPrepaidFraction || result.NativeStock != plan.ExpectedNativeStock)
		{
			error = result.Diagnostic ?? "The environmental profile, authorisation, source lifecycle or native stock changed after planning.";
			return false;
		}
		if (plan.RequestedAmount < NativeOrganicAccountingMath.MinimumDebit ||
		    plan.RequestedAmount > NativeOrganicAccountingMath.MaximumDebit)
		{
			error = "The planned native amount is outside the supported accounting range.";
			return false;
		}

		if (plan.Kind == NativeOrganicSourceKind.Forage)
		{
			if (plan.WholeNativeDebit != 0 || plan.ClosingPrepaidFraction != 0m || plan.OpeningPrepaidFraction != 0m ||
			    (double)plan.RequestedAmount > result.NativeStock ||
			    !cell.TryPeekForagableYield(plan.Selector[7..], out NativeForageYieldSnapshot forage) ||
			    forage.ProfileId != plan.Lifecycle.ForageProfileId ||
			    forage.ProfileRevision != plan.Lifecycle.ForageProfileRevision ||
			    forage.DefinitionRevision != plan.Lifecycle.ForageDefinitionRevision ||
			    forage.SourceRevision != plan.SourceRevision || forage.Stock != plan.ExpectedNativeStock)
			{
				error = "The forage debit plan is no longer current or has invalid arithmetic.";
				return false;
			}
			if (!cell.TryConsumeYield(forage, (double)plan.RequestedAmount, out var reason))
			{
				error = reason;
				result = InspectOrganicSource(cell, plan.Selector);
				return false;
			}
		}
		else
		{
			if (!TryIntegerStock(result.NativeStock, out var stock, out error) ||
			    !NativeOrganicAccountingMath.TryPlanInteger(stock, result.PrepaidFraction, plan.RequestedAmount,
				    out var wholeDebit, out var closingPrepaid, out error) ||
			    wholeDebit != plan.WholeNativeDebit || closingPrepaid != plan.ClosingPrepaidFraction)
			{
				return false;
			}
			var field = FieldFor(cell);
			if (field is null)
			{
				error = "The indexed agriculture field is no longer present.";
				result = InspectOrganicSource(cell, plan.Selector);
				return false;
			}
			if (!field.TryApplyNativeOrganicDebit(plan, out var reason))
			{
				error = reason;
				result = InspectOrganicSource(cell, plan.Selector);
				return false;
			}
		}

		result = InspectOrganicSource(cell, plan.Selector);
		error = null;
		return true;
	}

	public NativeOrganicPenaltyEvaluation EvaluateOrganicPenalty(ICell cell, NativeOrganicPenaltyChannel channel,
		NativeOrganicPenaltyContext context)
	{
		if (!Enum.IsDefined(channel))
		{
			return NativeOrganicPenaltyEvaluation.Invalid("The organic penalty channel is unknown.");
		}
		if (cell is not Cell concrete || concrete.Id <= 0 || !ReferenceEquals(concrete.Gameworld, _world))
		{
			return NativeOrganicPenaltyEvaluation.Invalid("Organic penalties require a physical cell in this gameworld.");
		}
		var profileId = EffectiveProfileId(concrete);
		if (!profileId.HasValue)
		{
			return NativeOrganicPenaltyEvaluation.Neutral;
		}
		var profile = Profile(profileId.Value);
		if (profile is null)
		{
			return OrganicPenaltyFailure(profileId.Value, channel,
				$"Environmental profile #{profileId.Value} is missing or has the wrong type.");
		}
		if (!profile.OrganicPenalties.Any(x => x.Channel == channel))
		{
			return NativeOrganicPenaltyEvaluation.Neutral;
		}
		var sourceKind = OrganicSourceKind(channel);
		if (context.Kind != sourceKind ||
		    !TryCanonicalOrganicSelector(context.Selector, out var contextSelector, out var selectorKind, out _) ||
		    selectorKind != sourceKind)
		{
			return OrganicPenaltyFailure(profile.Id, channel,
				"The native penalty context does not identify the channel's canonical source kind.");
		}
		var declarations = profile.OrganicSources
			.Where(source => source.Kind == sourceKind && source.Selector.EqualTo(contextSelector))
			.ToList();
		var sourceError = OrganicSourceValidationError(profile, contextSelector);
		if (declarations.Count == 0)
		{
			if (sourceError is not null)
			{
				return OrganicPenaltyFailure(profile.Id, channel, sourceError);
			}
			return NativeOrganicPenaltyEvaluation.Neutral;
		}
		if (declarations.Count != 1)
		{
			return OrganicPenaltyFailure(profile.Id, channel,
				$"Environmental profile #{profile.Id} has duplicate {contextSelector} declarations.");
		}
		if (sourceError is not null)
		{
			return OrganicPenaltyFailure(profile.Id, channel, sourceError);
		}
		if (!OrganicDeclarationApplies(cell, declarations[0]))
		{
			return NativeOrganicPenaltyEvaluation.Neutral;
		}
		if (profile is EnvironmentalMagicGenerator generator &&
		    generator.OrganicPenaltyValidationError(channel) is { } penaltyError)
		{
			return OrganicPenaltyFailure(profile.Id, channel, penaltyError);
		}
		if (!_evaluating.Add(cell.Id))
		{
			_recursive.Add(cell.Id);
			return OrganicPenaltyFailure(profile.Id, channel, "Recursive environmental native-penalty evaluation is not permitted.");
		}
		try
		{
			var state = concrete.EnvironmentState;
			var utcNow = UtcNow;
			var pressure = PressureAt(state, utcNow);
			if (state.SchemaVersion != 1 || !double.IsFinite(state.ScarDamage) || state.ScarDamage < 0.0 ||
			    !double.IsFinite(pressure) || pressure < 0.0)
			{
				return OrganicPenaltyFailure(profile.Id, channel, "The cell's environmental scar/pressure state is invalid.");
			}
			var values = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
			{
				["scardamage"] = state.ScarDamage,
				["pressure"] = pressure,
				["hasdefile"] = state.LastDefileUtc.HasValue ? 1.0 : 0.0,
				["minutessincedefile"] = state.LastDefileUtc.HasValue
					? Math.Max(0.0, (utcNow - state.LastDefileUtc.Value).TotalMinutes)
					: 0.0,
				["nativestock"] = context.NativeStock,
				["nativehealth"] = context.NativeHealth,
				["nativeyield"] = context.NativeYield,
				["nativecapacity"] = context.NativeCapacity,
				["fieldcondition"] = context.FieldCondition,
				["baselineincrease"] = context.BaselineIncrease
			};
			var required = profile.RequiredOrganicInputNames(channel);
			if (values.Where(x => required.Contains(x.Key)).Any(x => !double.IsFinite(x.Value)))
			{
				return OrganicPenaltyFailure(profile.Id, channel, "A required native penalty input is not finite.");
			}
			if (!TryCollectOrganicNamedInputs(cell, profile, required, values, out var error))
			{
				return OrganicPenaltyFailure(profile.Id, channel, error!);
			}
			var evaluation = profile.EvaluateOrganicPenalty(channel,
				new ReadOnlyDictionary<string, double>(values));
			if (!evaluation.IsValid)
			{
				return OrganicPenaltyFailure(profile.Id, channel,
					evaluation.Error ?? "The configured organic penalty is invalid.");
			}
			if (_recursive.Contains(cell.Id))
			{
				return OrganicPenaltyFailure(profile.Id, channel,
					"Recursive environmental native-penalty evaluation is not permitted.");
			}
			return evaluation;
		}
		catch (Exception exception)
		{
			return OrganicPenaltyFailure(profile.Id, channel,
				$"Environmental organic penalty evaluation failed: {exception.Message}");
		}
		finally
		{
			_evaluating.Remove(cell.Id);
			_recursive.Remove(cell.Id);
		}
	}

	internal NativeOrganicPenaltyEvaluation InspectOrganicPenaltyFactor(ICell cell,
		NativeOrganicSourceSnapshot source, NativeOrganicPenaltyChannel channel)
	{
		var stock = source.NativeStock;
		var health = 0.0;
		var nativeYield = stock;
		var capacity = source.Kind == NativeOrganicSourceKind.Forage ? Math.Max(stock, 0.0) : 100.0;
		var condition = 0.0;
		if (source.Kind == NativeOrganicSourceKind.Forage &&
		    source.Selector.StartsWith("forage:", StringComparison.Ordinal) &&
		    cell.TryPeekForagableYield(source.Selector[7..], out NativeForageYieldSnapshot forage))
		{
			capacity = forage.Maximum;
		}
		else if (FieldFor(cell) is { } field)
		{
			condition = field.Condition;
			switch (source.Kind)
			{
				case NativeOrganicSourceKind.Crop:
					health = field.CropHealth;
					nativeYield = field.CropYieldPotential;
					break;
				case NativeOrganicSourceKind.Woodland:
					health = field.WoodlandHealth;
					nativeYield = field.WoodlandYieldPotential;
					break;
				case NativeOrganicSourceKind.Pasture:
					health = field.Condition;
					nativeYield = field.Pasture;
					break;
			}
		}
		var baseline = channel switch
		{
			NativeOrganicPenaltyChannel.ForageReplenishment when cell is Cell concrete =>
				concrete.PeekNativeForageHourlyProduction(source.Selector[7..]),
			NativeOrganicPenaltyChannel.CropHealthRecovery => 1.0,
			NativeOrganicPenaltyChannel.CropYieldRecovery when FieldFor(cell) is { } cropField =>
				Math.Max(0, Math.Sign(cropField.Nutrients - 50)),
			NativeOrganicPenaltyChannel.WoodlandHealthRecovery or
				NativeOrganicPenaltyChannel.WoodlandYieldRecovery => 1.0,
			_ => 0.0
		};
		return EvaluateOrganicPenalty(cell, channel, new NativeOrganicPenaltyContext(source.Kind,
			source.Selector, stock, health, nativeYield, capacity, condition, baseline));
	}

	private bool TryCollectOrganicNamedInputs(ICell cell, IEnvironmentalMagicProfile profile,
		IReadOnlySet<string> required, Dictionary<string, double> values, out string? error)
	{
		error = null;
		IAgricultureField? field = null;
		var fieldRead = false;
		foreach (var input in profile.Inputs)
		{
			if (!required.Contains(input.Name) || values.ContainsKey(input.Name)) continue;
			double value;
			switch (input.Kind)
			{
				case EnvironmentalMagicInputKind.Forage:
					if (!cell.HasForagableProfile) value = 0.0;
					else if (!cell.TryPeekForagableYield(input.Source, out value))
					{
						error = $"Forage input {input.Name} refers to missing yield '{input.Source}'.";
						return false;
					}
					break;
				case EnvironmentalMagicInputKind.Agriculture:
					if (!fieldRead)
					{
						field = FieldFor(cell);
						fieldRead = true;
					}
					value = AgricultureValue(field, input.Source);
					break;
				case EnvironmentalMagicInputKind.Prog:
					if (input.Prog is null || input.Prog.StaticType != FutureProgStaticType.NotStatic)
					{
						error = $"Input {input.Name} requires a non-static numeric location prog.";
						return false;
					}
					var watch = Stopwatch.StartNew();
					_lastProgExecutions++;
					_totalProgExecutions++;
					var success = input.Prog.ExecuteWithStatus(out var progResult, cell);
					if (watch.Elapsed.TotalMilliseconds >= Options.SlowProgMilliseconds)
						RecordSlowProg(profile.Id, input.Prog.Id, watch.Elapsed.TotalMilliseconds);
					if (!success || progResult is null)
					{
						error = $"Input prog #{input.Prog.Id} failed.";
						return false;
					}
					value = Convert.ToDouble(progResult, CultureInfo.InvariantCulture);
					break;
				default:
					error = $"Input {input.Name} has an unsupported kind.";
					return false;
			}
			value *= input.Scale;
			if (!double.IsFinite(value))
			{
				error = $"Input {input.Name} is not finite.";
				return false;
			}
			values[input.Name] = value;
		}
		return true;
	}

	public bool RepairNativeOrganicAccounting(ICell cell, NativeOrganicSourceKind? kind, out string result)
	{
		if (kind == NativeOrganicSourceKind.Forage)
		{
			result = "Forage has no integer prepaid/progress accounting to repair.";
			return false;
		}
		if (kind.HasValue && !Enum.IsDefined(kind.Value))
		{
			result = "Specify crop, woodland, pasture or all.";
			return false;
		}
		var field = FieldFor(cell);
		if (field is null)
		{
			result = "This physical cell has no indexed agriculture field.";
			return false;
		}
		if (_evaluating.Contains(cell.Id))
		{
			result = "Environmental input progs must be read-only.";
			return false;
		}
		return field.RepairNativeOrganicAccounting(kind, out result);
	}

	private NativeOrganicPenaltyEvaluation OrganicPenaltyFailure(long profileId,
		NativeOrganicPenaltyChannel channel, string error)
	{
		var now = Now;
		if (!_lastLoggedOrganic.TryGetValue(profileId, out var last) || now - last >= 60.0)
		{
			_lastLoggedOrganic[profileId] = now;
			_world.SystemMessage(
				$"Environmental organic penalty: profile #{profileId}, {channel.DescribeEnum()}: {error}", true);
		}
		return NativeOrganicPenaltyEvaluation.Invalid(error);
	}

	private static bool TryIntegerStock(double value, out int stock, out string? error)
	{
		stock = 0;
		if (!double.IsFinite(value) || value < 0.0 || value > int.MaxValue || value != Math.Truncate(value))
		{
			error = "The native integer source reported an invalid stock value.";
			return false;
		}
		stock = (int)value;
		error = null;
		return true;
	}

	private static NativeOrganicSourceKind OrganicSourceKind(NativeOrganicPenaltyChannel channel) => channel switch
	{
		NativeOrganicPenaltyChannel.ForageReplenishment => NativeOrganicSourceKind.Forage,
		NativeOrganicPenaltyChannel.CropHealthRecovery or NativeOrganicPenaltyChannel.CropYieldRecovery or
			NativeOrganicPenaltyChannel.CropInitialisation => NativeOrganicSourceKind.Crop,
		NativeOrganicPenaltyChannel.WoodlandHealthRecovery or NativeOrganicPenaltyChannel.WoodlandYieldRecovery or
			NativeOrganicPenaltyChannel.WoodlandInitialisation => NativeOrganicSourceKind.Woodland,
		NativeOrganicPenaltyChannel.PastureRecovery or NativeOrganicPenaltyChannel.PastureInitialisation =>
			NativeOrganicSourceKind.Pasture,
		_ => (NativeOrganicSourceKind)(-1)
	};

	private static string? OrganicSourceValidationError(IEnvironmentalMagicProfile profile, string canonicalSelector) =>
		profile is EnvironmentalMagicGenerator generator
			? generator.OrganicSourceValidationError(canonicalSelector)
			: null;

	private static string? OrganicSourceValidationError(IEnvironmentalMagicProfile profile,
		NativeOrganicSourceDeclaration declaration) => profile is EnvironmentalMagicGenerator generator
		? generator.OrganicSourceValidationError(declaration)
		: null;

	private bool OrganicDeclarationApplies(ICell cell, NativeOrganicSourceDeclaration declaration)
	{
		if (declaration.Kind == NativeOrganicSourceKind.Forage)
		{
			return true;
		}
		var field = FieldFor(cell);
		if (field is null)
		{
			return false;
		}
		var compatibleUse = declaration.Kind switch
		{
			NativeOrganicSourceKind.Crop => field.CurrentUse is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard,
			NativeOrganicSourceKind.Woodland => field.CurrentUse == AgricultureFieldUse.Woodland,
			NativeOrganicSourceKind.Pasture => field.CurrentUse == AgricultureFieldUse.Pasture,
			_ => false
		};
		if (!compatibleUse)
		{
			return false;
		}
		if (declaration.AllowedFieldUses.Count > 0 && !declaration.AllowedFieldUses.Contains(field.CurrentUse))
		{
			return false;
		}
		if (declaration.DefinitionIds.Count == 0)
		{
			return true;
		}
		var definitionId = declaration.Kind switch
		{
			NativeOrganicSourceKind.Crop => field.CurrentCrop?.Id ?? 0L,
			NativeOrganicSourceKind.Woodland => field.CurrentWoodland?.Id ?? 0L,
			_ => 0L
		};
		return declaration.DefinitionIds.Contains(definitionId);
	}

	private static NativeOrganicSourceSnapshot OrganicFailure(ICell cell, string selector,
		NativeOrganicSourceKind kind, NativeOrganicSourceStatus status, string diagnostic,
		long? profileId = null, long profileRevision = 0) => new(selector, kind, status, null, 0.0, 0m,
		NativeOrganicRecoveryRemainders.Empty, 0, profileId, profileRevision, null, diagnostic);

	private static bool TryCanonicalOrganicSelector(string text, out string selector,
		out NativeOrganicSourceKind kind, out string? forageKey)
	{
		text = (text ?? string.Empty).Trim();
		forageKey = null;
		if (text.StartsWith("forage:", StringComparison.OrdinalIgnoreCase))
		{
			kind = NativeOrganicSourceKind.Forage;
			forageKey = NativeOrganicSourceSelectors.NormaliseForageKey(text[7..]);
			selector = NativeOrganicSourceSelectors.Canonical(kind, forageKey);
			return NativeOrganicSourceSelectors.IsValidForageKey(forageKey);
		}
		if (text.EqualTo("orchard")) text = "crop";
		if (Enum.TryParse<NativeOrganicSourceKind>(text, true, out kind) && Enum.IsDefined(kind) &&
		    kind != NativeOrganicSourceKind.Forage)
		{
			selector = NativeOrganicSourceSelectors.Canonical(kind);
			return true;
		}
		selector = string.Empty;
		kind = default;
		return false;
	}
}
