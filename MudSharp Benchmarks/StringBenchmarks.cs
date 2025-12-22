using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MudSharp.Framework;

namespace MudSharp_Benchmarks;

[MemoryDiagnoser]
public class Latin1Benchmarks
{
	public IEnumerable<string> SantiseTexts => 
	[
		@"╔════╦═════════╦══════╦══════════════════╦═════════╗
║ ID ║ Name    ║ Room ║ Room Desc        ║ Account ║
╠════╬═════════╬══════╬══════════════════╬═════════╣
║ 1  ║ Japheth ║ 1    ║ The Guest Lounge ║ Japheth ║
╚════╩═════════╩══════╩══════════════════╩═════════╝",
		@"You know the following things about time:

  It is currently Night.
  It is half past eleven p.m on Nûndinârum F the 14th of Ianuarius, 1005 AUC.",
		@"Japheth (a toothbrush-moustached baby with round ears)
Ust: Legendary, Lst: Legendary, Hrd: Legendary, Res: Legendary, Dex: Legendary, Spd: Legendary, Int: Legendary, Wis: Legendary, San: Legendary, Ten:
Legendary, Cnf: Legendary, Aur: Legendary, Sns: Legendary, Cog: Legendary
You are a seventy one year old human male, and belong to the Admin culture.
You are an Elder.
Your account has 633 Build Points.
You are 1 metre 80 centimetres tall and weigh 90 kilograms.
You are right-handed.
Your birthday is Kalends, Nûndinârum H the 1st of Ianuarius, 935 AUC.
You are absolutely stuffed, completely sated and sober.
You are speaking in Admin Speech with a foreign accent.
You don't have any writing preferences set up.
You stroll if you are standing, crawl slowly if you are lying prone, swim if you are swimming, and fly if you are flying.
You use the Roman currency in any economic transactions.
You have played this character for a total of 5 days, 21 hours, and 57 minutes.
You can carry 250 kilograms, drag 1 tonne and are Unencumbered.
You are not carrying anything at all.
You are standing."
	];

	[ParamsSource(nameof(SantiseTexts))]
	public string SanitiseText { get; set; }

	//[Benchmark]
	//public string Original() => SanitiseText.ConvertToLatin1();

	[Benchmark]
	public string Revised() => SanitiseText.ConvertToLatin1();
}

[MemoryDiagnoser]

public class MxpBenchmarks
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

[MemoryDiagnoser]
public class ShuffleBenchmarks
{
	public class Tag
	{
		public required string Text { get; init; }
	}
	    
    public IEnumerable<int> Values { get; } = Enumerable.Range(0, 1000).ToList();

    
    public IEnumerable<Tag> ObjectCollection { get; } = Enumerable.Range(0, 1000).Select(x => new Tag { Text = $"Tag{x}" }).ToList();

	[Benchmark]
	public List<int> ShuffleValues() => Values.Shuffle().ToList();
    [Benchmark]
    public List<int> OldShuffleValues() => Values.OldShuffle().ToList();
    [Benchmark]
    public List<Tag> ShuffleObjects() => ObjectCollection.Shuffle().ToList();
    [Benchmark]
    public List<Tag> OldShuffleObjects() => ObjectCollection.OldShuffle().ToList();
}