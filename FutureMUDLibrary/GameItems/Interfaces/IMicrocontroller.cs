#nullable enable

using System.Collections.Generic;

namespace MudSharp.GameItems.Interfaces;

public interface IMicrocontroller : ISignalSourceComponent, IConsumePower, ISwitchable, IOnOff
{
	IReadOnlyDictionary<string, double> Inputs { get; }
}
