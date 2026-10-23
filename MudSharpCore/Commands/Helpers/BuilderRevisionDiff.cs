#nullable enable
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;

namespace MudSharp.Commands.Helpers;

internal sealed record BuilderFieldChange(string Field, string Before, string After);

internal static class BuilderRevisionDiff
{
	internal static IReadOnlyList<BuilderFieldChange> Compare(IReadOnlyDictionary<string, string> before,
		IReadOnlyDictionary<string, string> after) => before.Keys.Union(after.Keys).OrderBy(x => x)
		.Select(key => new BuilderFieldChange(key, before.GetValueOrDefault(key, "(absent)"), after.GetValueOrDefault(key, "(absent)")))
		.Where(change => change.Before != change.After).ToList();

	public static Dictionary<string, string>? Snapshot(IEditableRevisableItem item)
	{
		if (item is IGameItemSkin skin)
		{
			return new Dictionary<string, string>
			{
				["Name"] = skin.Name, ["Prototype"] = Reference(skin.ItemProto),
				["Item name"] = skin.ItemName ?? "(inherit)", ["Short description"] = skin.ShortDescription ?? "(inherit)",
				["Full description"] = skin.FullDescription ?? "(inherit)", ["Long description"] = skin.LongDescription ?? "(inherit)",
				["Quality"] = skin.Quality?.DescribeEnum() ?? "(inherit)", ["Public"] = skin.IsPublic.ToString(),
				["Availability prog"] = Reference(skin.CanUseSkinProg)
			};
		}
		if (item is not IGameItemProto proto) return null;
		var result = new Dictionary<string, string>
		{
			["Name"] = proto.Name, ["Unique name"] = proto.UniqueName ?? "", ["Builder notes"] = proto.BuilderNotes ?? "",
			["Short description"] = proto.ShortDescription, ["Full description"] = proto.FullDescription,
			["Long description"] = proto.LongDescription, ["Overrides long description"] = proto.OverridesLongDescription.ToString(),
			["Material"] = Reference(proto.Material), ["Health strategy"] = Reference(proto.HealthStrategy),
			["Size"] = proto.Size.DescribeEnum(), ["Quality"] = proto.BaseItemQuality.DescribeEnum(),
			["Weight"] = proto.Weight.ToString("R", System.Globalization.CultureInfo.InvariantCulture),
			["Cost"] = proto.CostInBaseCurrency.ToString(System.Globalization.CultureInfo.InvariantCulture),
			["High priority"] = proto.HighPriority.ToString(), ["Player skins"] = proto.PermitPlayerSkins.ToString(),
			["Hidden"] = proto.IsHiddenFromPlayers.ToString(), ["Prevent load"] = proto.PreventManualLoad.ToString(),
			["Preserve variables"] = proto.PreserveRegisterVariables.ToString(), ["Item group"] = Reference(proto.ItemGroup),
			["Morph target"] = proto.MorphTargetId?.ToString() ?? "(none)", ["Morph time"] = proto.MorphTimeSpan.ToString(),
			["Morph emote"] = proto.MorphEmote, ["Refrigeration sensitive"] = proto.RefrigerationSensitive.ToString(),
			["Components"] = string.Join("\n", proto.Components.OrderBy(x => x.Id).ThenBy(x => x.RevisionNumber).Select(Reference))
		};
		foreach (var pair in proto.DefaultVariables) result[$"Variable: {pair.Key}"] = pair.Value;
		var index = 0;
		foreach (var extra in proto.ExtraDescriptions)
		{
			index++;
			result[$"Extra {index} prog"] = Reference(extra.Prog);
			result[$"Extra {index} short"] = extra.ShortDescription ?? "(inherit)";
			result[$"Extra {index} full"] = extra.FullDescription ?? "(inherit)";
			result[$"Extra {index} addendum"] = extra.FullDescriptionAddendum ?? "";
		}
		return result;
	}

	internal static string Reference(IFrameworkItem? item) => item is null ? "(none)" :
		$"{item.FrameworkItemType} #{item.Id}{(item is IRevisableItem revision ? $"r{revision.RevisionNumber}" : "")} ({item.Name})";
}
