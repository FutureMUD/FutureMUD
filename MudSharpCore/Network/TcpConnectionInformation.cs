using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Network;

public class TcpConnectionInformation
{
	public DateTime StartOfPeriod { get; init; }
	public int NumberOfConnections { get; set; }
}