#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Work.Crafts;

/// <summary>A read-only view of known possible recipes, never an item's manufacture history.</summary>
public static class CraftAnalysis
{
	public static IReadOnlyList<ICraft> FindRecipes(ICharacter actor, IGameItem item, IEnumerable<ICraft> crafts)
	{
		return crafts
			.Where(x => x.Status == RevisionStatus.Current && x.CraftIsValid)
			// Use the actual visibility prog: administrative listing privileges must not reveal hidden recipes.
			.Where(x => x.AppearInCraftsListProg?.ExecuteBool(actor) == true)
			.Where(x => x.CheckTrait is null ||
			            actor.HasTrait(x.CheckTrait) && actor.TraitRawValue(x.CheckTrait) > 0.0)
			.Where(x => x.CanUseProg?.ExecuteBool(actor) != false)
			.Where(x => x.Products.Any(product => product.MatchesForAnalysis(item)))
			.OrderBy(x => x.Name)
			.ToList();
	}

	public static string DescribeRecipes(ICharacter actor, IGameItem item, IReadOnlyList<ICraft> recipes)
	{
		if (recipes.Count == 0)
		{
			return "You cannot determine a recipe for that item with your current crafting understanding.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"You recognise the following possible recipes for {item.HowSeen(actor)}.");
		sb.AppendLine("These are recipe requirements; they do not establish how this particular item was made.");
		foreach (var craft in recipes)
		{
			sb.AppendLine();
			sb.AppendLine($"Recipe: {craft.Name.ColourName()}");
			sb.AppendLine("Ingredient requirements:");
			foreach (var input in craft.Inputs)
			{
				sb.AppendLine($"\t{input.HowSeen(actor)}");
			}

			if (!craft.Inputs.Any())
			{
				sb.AppendLine("\tNone.");
			}

			sb.AppendLine("Tool, fire and workstation requirements:");
			foreach (var tool in craft.Tools)
			{
				sb.AppendLine($"\t{tool.HowSeen(actor)} [{tool.DesiredState.Describe()}]");
			}

			if (!craft.Tools.Any())
			{
				sb.AppendLine("\tNone specified by this recipe.");
			}
		}

		return sb.ToString();
	}
}
