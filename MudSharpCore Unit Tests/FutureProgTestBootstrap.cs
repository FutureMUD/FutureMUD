using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

internal static class FutureProgTestBootstrap
{
    private static readonly object SyncRoot = new();
    private static bool _initialised;

    public static IFuturemud Gameworld { get; } = new GameworldStub().ToMock();

    public static void EnsureInitialised()
    {
        if (_initialised)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (_initialised)
            {
                return;
            }

            FutureProg.Initialise();
            _initialised = true;
        }
    }
}
