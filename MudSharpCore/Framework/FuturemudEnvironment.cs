#nullable enable

using MudSharp.Magic.Environment;
using MudSharp.Magic.Gathering;
using MudSharp.Magic;

namespace MudSharp.Framework;

public partial class Futuremud
{
	public IEnvironmentalMagicService? EnvironmentalMagic { get; private set; }
	public IMagicGatheringService? MagicGathering { get; private set; }
}
