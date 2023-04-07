using MudSharp.Framework;

namespace MudSharp.FutureProg;

public static class FutureProgFactory
{
	public static IFutureProg CreateNew(MudSharp.Models.FutureProg prog, IFuturemud gameworld)
	{
		return new FutureProg(prog, gameworld);
	}
}