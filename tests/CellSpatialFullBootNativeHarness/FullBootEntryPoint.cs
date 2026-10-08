namespace FutureMUD.GatheringNativePersistenceHarness;

internal static class FullBootEntryPoint
{
	private static int Main(string[] args)
	{
		try { return GNHProgram.RunRoomSpatialFullBoot(args); }
		catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
	}
}
