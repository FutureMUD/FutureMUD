using BenchmarkDotNet.Running;

namespace MudSharp_Benchmarks
{
    internal class Program
    {
        static void Main(string[] args)
        {
			if (args.Contains("--environmental-magic"))
			{
				EnvironmentalMagicPerformanceHarness.Run(args);
				return;
			}
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
