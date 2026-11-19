#nullable enable

using System;

namespace MudSharp.RPG.Law;

[Flags]
public enum PunishmentKind
{
	None = 0,
	Fine = 1,
	Custody = 2,
	GoodBehaviourBond = 4,
	Execution = 8
}
