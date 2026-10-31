#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Character;

public static class ClimbingAidRules
{
	public static double Bonus(IEnumerable<IGameItem> held, IEnumerable<IGameItem> worn) => held
		.Select(x => x.GetItemType<IClimbingAid>()).Where(x => x is { WorksHeld: true })
		.Concat(worn.Select(x => x.GetItemType<IClimbingAid>()).Where(x => x is { WorksWorn: true }))
		.Select(x => x!.CheckBonus).DefaultIfEmpty(0.0).Max();
}
