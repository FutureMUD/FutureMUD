#nullable enable

using System;

namespace DatabaseSeeder;

internal static class CatalogueToolsProgram
{
	private static int Main(string[] args)
	{
#if DEBUG
		try
		{
			if (EnvironmentalExposureCatalogueCapture.TryHandle(args)) return 0;
		}
		catch (Exception exception)
		{
			Console.Error.WriteLine(exception);
			return 1;
		}
#endif
		Console.Error.WriteLine("Use the Debug build with --capture-environmental-exposure.");
		return 1;
	}
}
