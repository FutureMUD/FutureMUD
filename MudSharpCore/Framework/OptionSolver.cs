using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MudSharp.Framework;

public class OptionSolver<T1, T2>
{
	public OptionSolver(IEnumerable<IChoice<T1, T2>> problems)
	{
		Problems = problems.AsList();
	}

	public List<IChoice<T1, T2>> Problems { get; }

	public (bool Success, List<(T1 Problem, T2 Option)> Solution, List<T1> UnsolvableChoices) SolveOptions()
	{
		var consideredProblemSets = new List<Solution<T1, T2>>();
		var currentSolution = new Solution<T1, T2>(Problems.Select(x => (x, x.BestOption)).ToList());
		if (currentSolution.IsValid())
		{
			return (true, currentSolution.GetOptions, new List<T1>());
		}

		consideredProblemSets.Add(currentSolution);
		var clash = currentSolution.FirstClashingOption();
		while (true)
		{
			var clashResolved = false;
			var clashingSolutions = currentSolution.Solutions.Where(x => x.Option.IsClash(clash)).ToList();
			foreach (var choiceSolution in clashingSolutions)
			{
				foreach (var option in choiceSolution.Choice.Options.Where(x => !x.IsClash(clash)))
				{
					var newSolution = new Solution<T1, T2>(currentSolution, choiceSolution.Choice, option);
					if (consideredProblemSets.Any(x => x.Equals(newSolution)))
					{
						continue;
					}

					if (newSolution.IsValid())
					{
						return (true, newSolution.GetOptions, new List<T1>());
					}

					consideredProblemSets.Add(newSolution);
					if (newSolution.HasClashOnOption(option))
					{
						continue;
					}

					clash = newSolution.FirstClashingOption();
					currentSolution = newSolution;
					clashResolved = true;
				}

				if (clashResolved)
				{
					break;
				}
			}

			if (!clashResolved)
			{
				return (false, new List<(T1 Problem, T2 Option)>(),
					clashingSolutions.Select(x => x.Choice.BaseItem).ToList());
			}
		}
	}
}

public class Solution<T1, T2>
{
	public Solution(IEnumerable<(IChoice<T1, T2> Problem, IOption<T2> Option)> solutions)
	{
		Solutions = solutions.AsList();
	}

	public Solution(Solution<T1, T2> original, IChoice<T1, T2> changedChoice, IOption<T2> changedOption)
	{
		Solutions = original.Solutions.ToList();
		for (var i = 0; i < Solutions.Count; i++)
		{
			if (Solutions[i].Choice == changedChoice)
			{
				Solutions[i] = (changedChoice, changedOption);
				break;
			}
		}
	}

	public List<(IChoice<T1, T2> Choice, IOption<T2> Option)> Solutions { get; private set; }

	public List<(T1 Choice, T2 Option)> GetOptions =>
		Solutions.Select(x => (x.Choice.BaseItem, x.Option.BaseItem)).ToList();

	public bool IsValid()
	{
		for (var i = 0; i < Solutions.Count; i++)
		for (var j = i + 1; j < Solutions.Count; j++)
		{
			if (Solutions[j].Option.IsClash(Solutions[i].Option))
			{
				return false;
			}
		}

		return true;
	}

	public IOption<T2> FirstClashingOption()
	{
		for (var i = 0; i < Solutions.Count; i++)
		for (var j = i + 1; j < Solutions.Count; j++)
		{
			if (Solutions[j].Option.IsClash(Solutions[i].Option))
			{
				return Solutions[i].Option;
			}
		}

		throw new InvalidOperationException();
	}

	public bool HasClashOnOption(IOption<T2> option)
	{
		return Solutions.Count(x => x.Option.IsClash(option)) > 1;
	}

	public bool Equals(Solution<T1, T2> otherSet)
	{
		return Solutions.SequenceEqual(otherSet.Solutions);
	}
}

public interface IChoice<out T1, T2>
{
	T1 BaseItem { get; }
	Func<T2, double> OptionScorer { get; set; }
	IEnumerable<IOption<T2>> Options { get; }
	IOption<T2> BestOption { get; }
}

public class Choice<T1, T2> : IChoice<T1, T2>
{
	public Choice(T1 baseItem, IEnumerable<IOption<T2>> options)
	{
		BaseItem = baseItem;
		Options = options;
	}

	public T1 BaseItem { get; }
	public Func<T2, double> OptionScorer { get; set; }
	public IEnumerable<IOption<T2>> Options { get; }

	public IOption<T2> BestOption => OptionScorer != null
		? Options.FirstMax(x => OptionScorer(x.BaseItem))
		: Options.GetRandomElement();
}

public interface IOption<T>
{
	T BaseItem { get; }
	Func<T, bool> ClashEvaluatorFunc { get; set; }
	bool IsClash(IOption<T> otherOption);
}

public class Option<T> : IOption<T>
{
	public Option(T baseItem)
	{
		BaseItem = baseItem;
	}

	public T BaseItem { get; }

	public Func<T, bool> ClashEvaluatorFunc { get; set; }

	public bool IsClash(IOption<T> otherOption)
	{
		return ClashEvaluatorFunc?.Invoke(otherOption.BaseItem) ?? BaseItem.Equals(otherOption.BaseItem);
	}
}

public class PerceivableOption : Option<IPerceivable>
{
	public PerceivableOption(IPerceivable baseItem) : base(baseItem)
	{
		ClashEvaluatorFunc = item => item.IsSelf(baseItem);
	}
}