using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Construction;

namespace MudSharp.Framework;

public static class MUDConstants
{
	public static readonly string[] TwoHandedStrings =
	{
		"2h",
		"2hand",
		"2handed",
		"two",
		"etwo",
		"2",
		"both",
		"2-hand",
		"2-handed",
		"twohanded",
		"twohand",
		"two-handed",
		"two-hand"
	};

	public static readonly string[] CharacterStatusStrings =
	{
		"In Creation".Colour(Telnet.Yellow),
		"Awaiting Approval".Colour(Telnet.Yellow),
		"Active".Colour(Telnet.Green),
		"Deceased".Colour(Telnet.Red),
		"Suspended".ColourBold(Telnet.Red),
		"Retired".Colour(Telnet.Cyan)
	};
}