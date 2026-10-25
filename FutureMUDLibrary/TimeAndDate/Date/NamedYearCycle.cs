#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.TimeAndDate.Date;

/// <summary>An ordered, repeating list of year names. EpochYear is year one of cycle one.</summary>
public sealed class NamedYearCycle
{
	public NamedYearCycle(string name, int epochYear, IEnumerable<string> yearNames)
	{
		var names = yearNames.ToArray();
		if (string.IsNullOrWhiteSpace(name) || names.Length == 0 || names.Any(string.IsNullOrWhiteSpace) ||
		    names.Distinct(StringComparer.InvariantCultureIgnoreCase).Count() != names.Length)
		{
			throw new ArgumentException("A named year cycle needs a name and at least one unique, nonblank year name.");
		}

		Name = name;
		EpochYear = epochYear;
		YearNames = Array.AsReadOnly(names);
	}

	public string Name { get; }
	public int EpochYear { get; }
	public IReadOnlyList<string> YearNames { get; }

	public NamedYearInfo GetYear(int year)
	{
		var offset = (long)year - EpochYear;
		var index = (int)((offset % YearNames.Count + YearNames.Count) % YearNames.Count);
		return new NamedYearInfo(YearNames[index], index + 1, (offset - index) / YearNames.Count + 1);
	}
}

public sealed record NamedYearInfo(string Name, int YearInCycle, long CycleNumber);
