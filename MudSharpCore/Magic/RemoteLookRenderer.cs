#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using System.Linq;
using System.Text;

namespace MudSharp.Magic;

public static class RemoteLookRenderer
{
	public static string DescribeRemoteCell(ICharacter viewer, ICell location, RoomLayer layer)
	{
		var flags = PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreDark;
		var sb = new StringBuilder();
		sb.AppendLine(location.HowSeen(viewer, type: DescriptionType.Full));

		foreach (var move in location.Characters
		                             .Where(x => x != viewer && x.Movement is not null && x.RoomLayer == layer)
		                             .Select(x => x.Movement)
		                             .Distinct()
		                             .Where(move => move.SeenBy(viewer))
		                             .ToList())
		{
			sb.AppendLine(move.Describe(viewer).Wrap(viewer.InnerLineFormatLength).Colour(Telnet.Yellow));
		}

		var graffitis = location.EffectsOfType<IGraffitiEffect>(x => x.Layer == layer).ToList();
		if (graffitis.Any())
		{
			sb.AppendLine("There is graffiti in this location. Use LOOK GRAFFITI to view it.".Colour(Telnet.BoldCyan));
		}

		var items = location.LayerGameItems(layer)
		                    .Where(x => viewer.CanSee(x, flags))
		                    .ToList();
		if (items.GroupBy(x => x.ItemGroup?.Forms.Any() == true ? x.ItemGroup : null)
		         .Sum(x => x.Key is not null ? 1 : x.Count()) > 25 &&
		    GameItemProto.TooManyItemsGroup is not null)
		{
			sb.AppendLine(GameItemProto.TooManyItemsGroup.Describe(viewer, items, location).Fullstop()
			                               .Wrap(viewer.InnerLineFormatLength).Colour(Telnet.Cyan));
		}
		else
		{
			foreach (var group in items.GroupBy(x => x.ItemGroup?.Forms.Any() == true ? x.ItemGroup : null)
			                           .OrderBy(x => x.Key is null))
			{
				if (group.Key is not null)
				{
					sb.AppendLine(group.Key.Describe(viewer, group.AsEnumerable(), location)
					                       .Wrap(viewer.InnerLineFormatLength)
					                       .Colour(Telnet.Cyan));
					continue;
				}

				foreach (var item in group)
				{
					sb.AppendLine(item.HowSeen(viewer, true, DescriptionType.Long, flags: flags)
					                  .Wrap(viewer.InnerLineFormatLength));
				}
			}
		}

		foreach (var ch in location.Characters.Where(x =>
			         x != viewer && x.Movement is null && x.RoomLayer == layer && viewer.CanSee(x, flags)))
		{
			sb.AppendLine(ch.HowSeen(viewer, true, DescriptionType.Long, flags: flags).Wrap(viewer.InnerLineFormatLength));
		}

		return sb.ToString().Wrap(viewer.Account.LineFormatLength);
	}
}
