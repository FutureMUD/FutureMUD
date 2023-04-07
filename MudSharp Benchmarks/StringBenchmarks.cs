using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MudSharp.Framework;

namespace MudSharp_Benchmarks
{
	[MemoryDiagnoser]

	public class StringBenchmarks
	{
		[ParamsSource(nameof(MxpSupports))]
		public MXPSupport MxpSupport { get; set; }

		public IEnumerable<MXPSupport> MxpSupports => new[]
		{
			new MXPSupport(),
			new MXPSupport(true, "send")
		};

		[ParamsSource(nameof(DirtyStrings))]
		public string DirtyString { get; set; }

		public IEnumerable<string> DirtyStrings => new[]{
			"\x1B[32mAn example item with big shiny lights\x1B[0m is here, doing a whole bunch of Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
			"\x1B[32mAn mxp item with big shiny lights\x1B[0m is here, doing a \x03send href=\x06dance monkey\x06\x04whole bunch\x03/send\x04 of Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
		};

		[Benchmark]
		public string SanitiseMxp() => DirtyString.SanitiseMXP(MxpSupport);
	}
}
