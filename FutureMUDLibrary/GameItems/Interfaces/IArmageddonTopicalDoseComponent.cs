using System;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

/// <summary>Typed instance dose data exposed to future native spell invocation bindings.</summary>
public interface IArmageddonTopicalDoseComponent : IGameItemComponent
{
	ArmageddonTopicalDoseBinding? DoseBinding { get; }
	void Bind(ArmageddonTopicalDoseBinding binding);
}

public sealed record ArmageddonTopicalDoseProfile(string CatalogueKey, double ReferenceDoseGrams, double BudgetPerGrade)
{
	public bool IsValid => CatalogueKey == "arm.item.restorative_mud" && double.IsFinite(ReferenceDoseGrams) && ReferenceDoseGrams > 0.0 &&
		double.IsFinite(BudgetPerGrade) && BudgetPerGrade is > 0.0 and <= 100.0;
}

public sealed record ArmageddonTopicalDoseBinding(string CatalogueKey, Guid LifecycleId, long SpellId, int Grade, long CreatorId, long CreatorInstanceId, double ReferenceDoseGrams, double BudgetPerGrade, bool Spent)
{
	public bool IsValid => CatalogueKey == "arm.item.restorative_mud" && LifecycleId != Guid.Empty && SpellId > 0 && CreatorId > 0 && CreatorInstanceId > 0 && Grade is >= 1 and <= 7 &&
													 double.IsFinite(ReferenceDoseGrams) && ReferenceDoseGrams > 0.0 && double.IsFinite(BudgetPerGrade) && BudgetPerGrade is > 0.0 and <= 100.0;

	public double HealingBudget => IsValid ? BudgetPerGrade * Grade : 0.0;

	public double HealingBudgetForConsumedDose(double consumedGrams)
	{
		if (!IsValid || !double.IsFinite(consumedGrams) || consumedGrams <= 0.0)
		{
			return 0.0;
		}

		return HealingBudget * Math.Clamp(consumedGrams / ReferenceDoseGrams, 0.0, 1.0);
	}

	public bool IsExactFullDose(double consumedGrams, double remainingGrams)
	{
		return IsValid && double.IsFinite(consumedGrams) && double.IsFinite(remainingGrams) &&
				   Math.Abs(consumedGrams - ReferenceDoseGrams) <= 0.000001 &&
				   Math.Abs(remainingGrams - ReferenceDoseGrams) <= 0.000001;
	}
	public bool CanConsume(double consumedGrams, double remainingGrams) => IsValid && !Spent && IsExactFullDose(consumedGrams, remainingGrams);
}
