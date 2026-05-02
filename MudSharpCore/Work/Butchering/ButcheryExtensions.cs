using MudSharp.Body;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Work.Butchering;

public static class ButcheryExtensions
{
	public static string NormaliseButcherySubcategory(this string subcategory)
	{
		return string.IsNullOrWhiteSpace(subcategory) ? string.Empty : subcategory.ToLowerInvariant();
	}

	public static bool ButcheryBodypartMatches(this IBodypart part, IBodypart requiredPart)
	{
		return part != null &&
		       requiredPart != null &&
		       (part == requiredPart || part.CountsAs(requiredPart) || requiredPart.CountsAs(part));
	}

	public static IEnumerable<IBodypart> MatchingBodyparts(this IButcheryProduct product, IButcherable target)
	{
		return target.Parts
		             .Where(part => product.RequiredBodyparts.Any(part.ButcheryBodypartMatches))
		             .Distinct();
	}

	public static bool AppliesTo(this IButcheryProduct product, IButcherable target)
	{
		if (product.TargetBody is not null && !target.OriginalBody.Prototype.CountsAs(product.TargetBody))
		{
			return false;
		}

		if (!product.RequiredBodyparts.Any())
		{
			return false;
		}

		return product.RequiredBodyparts.All(requiredPart =>
			target.Parts.Any(part => part.ButcheryBodypartMatches(requiredPart)));
	}

	/// <summary>
	/// Returns the string representation of the verb, e.g. Butcher, Salvage etc
	/// </summary>
	/// <param name="verb"></param>
	/// <param name="properCase"></param>
	/// <returns></returns>
	public static string Describe(this ButcheryVerb verb, bool properCase = true)
	{
		switch (verb)
		{
			case ButcheryVerb.Butcher:
				return properCase ? "Butcher" : "butcher";
			case ButcheryVerb.Salvage:
				return properCase ? "Salvage" : "salvage";
		}

		throw new ApplicationException("Unknown ButcheryVerb in ButcheryExtensions.Describe");
	}

	/// <summary>
	/// Returns the gerund of the verb's action, e.g. butchering, salvaging, etc
	/// </summary>
	/// <param name="verb"></param>
	/// <returns></returns>
	public static string DescribeGerund(this ButcheryVerb verb)
	{
		switch (verb)
		{
			case ButcheryVerb.Butcher:
				return "butchering";
			case ButcheryVerb.Salvage:
				return "salvaging";
		}

		throw new ApplicationException("Unknown ButcheryVerb in ButcheryExtensions.DescribeGerund");
	}
}
