#nullable enable

using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public enum PsionicCoerceMode
{
	Fatigue,
	Refresh,
	Thirst,
	Quench,
	Hunger,
	Full,
	Thought
}

