using BenchmarkDotNet.Running;

namespace MudSharp_Benchmarks
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//BenchmarkRunner.Run<Latin1Benchmarks>();
			//BenchmarkRunner.Run<StringBenchmarks>();
			//BenchmarkRunner.Run<ShuffleBenchmarks>();
			BenchmarkRunner.Run<JsonEscapeBenchmarks>();
			Console.ReadLine();
		}
	}
} 