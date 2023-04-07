using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Body.Grouping;

public abstract class BodypartGroupDescriber : FrameworkItem, IBodypartGroupDescriber
{
	public virtual void FinaliseLoad(MudSharp.Models.BodypartGroupDescriber describer, IFuturemud gameworld)
	{
	}

	public static IBodypartGroupDescriber LoadDescriber(MudSharp.Models.BodypartGroupDescriber describer,
		IFuturemud game)
	{
		switch (describer.Type)
		{
			case "shape":
				return new BodypartGroupShapeDescriber(describer, game);
			case "bodypart":
				return new BodypartGroupIDDescriber(describer, game);
			default:
				throw new NotImplementedException();
		}
	}

	public static string DescribeGroups<T>(IEnumerable<T> describers, IEnumerable<IBodypart> bodyparts)
		where T : IBodypartGroupDescriber
	{
		// Run the rule for the list of bodyparts and get a score. Higher scores at the start of the list.
		var results =
			describers.Select(x => x.Match(bodyparts))
			          .Where(x => x.IsMatch)
			          .OrderByDescending(x => x.MatchScore)
			          .ToList();

		// If we have no results there are no groups - use bodypart sdescs instead
		if (results.Count == 0)
		{
			return bodyparts.Select(x => x.ShortDescription(colour: false)).ListToString();
		}

		// Add the first match to the results and setup further processing
		var resultStrings = new List<string>();
		var firstResult = results.First();
		var remains = firstResult.Remains;
		resultStrings.Add(firstResult.Description);
		results.Remove(firstResult);

		// Keep adding groups while we have some remainder
		while (remains.Count > 0)
		{
			var currentResult = results.FirstOrDefault(x => x.Matches.All(remains.Contains));
			if (currentResult == null)
			{
				break;
			}

			resultStrings.Add(currentResult.Description);
			remains.RemoveAll(x => currentResult.Matches.Contains(x));
			results.Remove(currentResult);
		}

		resultStrings.AddRange(remains.Select(x => x.ShortDescription(colour: false)));
		return resultStrings.ListToString();
	}

	#region IBodypartGroupDescriber Members

	public string DescribedAs { get; protected set; }

	public string Comment { get; protected set; }

	public abstract BodypartGroupResult Match(IEnumerable<IBodypart> parts);

	#endregion
}