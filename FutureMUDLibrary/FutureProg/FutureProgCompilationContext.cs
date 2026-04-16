#nullable enable

namespace MudSharp.FutureProg;

public enum FutureProgCompilationContext
{
	StandardFutureProg = 0,
	ComputerFunction = 1,
	ComputerProgram = 2
}

public static class FutureProgCompilationContextExtensions
{
	public static bool IsComputerContext(this FutureProgCompilationContext context)
	{
		return context is FutureProgCompilationContext.ComputerFunction or FutureProgCompilationContext.ComputerProgram;
	}

	public static string Describe(this FutureProgCompilationContext context)
	{
		return context switch
		{
			FutureProgCompilationContext.StandardFutureProg => "standard futureprog",
			FutureProgCompilationContext.ComputerFunction => "computer function",
			FutureProgCompilationContext.ComputerProgram => "computer program",
			_ => "futureprog"
		};
	}
}
