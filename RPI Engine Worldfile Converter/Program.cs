using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Framework;

namespace RPI_Engine_Worldfile_Converter;

internal class Program
{
	public static Regex ItemDefinitionRegex = new Regex("^#\\d+$");
	static async Task Main(string[] args)
	{
		//Console.WriteLine("Input File:");
		//var path = Console.ReadLine();
		var items = new List<RPIItem>();
		var headPath = @"E:\User Data\OneDrive\SOI Worldfiles\soiregions-main\";
		foreach (var path in Directory.GetFiles(headPath, "objs.*"))
		{
			using (var fs = new FileStream(path, FileMode.Open))
			{
				using var sr = new StreamReader(fs, Encoding.ASCII);
				var sb = new StringBuilder();
				var line = await sr.ReadLineAsync();
				while (sr.Peek() != -1)
				{
					sb.AppendLine(line);
					while (true)
					{
						line = await sr.ReadLineAsync();
						if (line is null)
						{
							break;
						}
						if (ItemDefinitionRegex.IsMatch(line))
						{
							break;
						}

						sb.AppendLine(line);
					}

					try
					{
						var item = new RPIItem(sb.ToString());
						items.Add(item);
					}
					catch
					{
						Console.WriteLine($"Error with item {sb.ToString().Split(new []{"\n", "\r\n"}, StringSplitOptions.None)[0]} in file {path}. Skipped");
					}
					finally
					{
						sb.Clear();
					}
				}
			}
		}
		
		var sorted = new CollectionDictionary<RPIItemType, RPIItem>();
		var wearExamples = new List<RPIItem>();
		foreach (var item in items)
		{
			sorted[item.ItemType].Add(item);
			if (item.WearBits.GetFlags().OfType<RPIWearBits>().Except(new RPIWearBits[]
			    {
					RPIWearBits.Take,
					RPIWearBits.Wield,
					RPIWearBits.Sheath,
					RPIWearBits.Unused1,
					RPIWearBits.Unused2,
					RPIWearBits.Unused3,
			    }).Count() > 1)
			{
				wearExamples.Add(item);
			}
		}

		using (var fs = new FileStream("ItemTypes.csv", FileMode.Create))
		{
			using (var writer = new StreamWriter(fs))
			{
				foreach (var combo in sorted)
				{
					writer.WriteLine($"{combo.Key.DescribeEnum()},\"{combo.Value.Select(x => x.ShortDescription).ListToCommaSeparatedValues(" | ")}\"");
				}

				writer.Flush();
			}
		}

		var combos = new CollectionDictionary<RPIWearBits, string>();
		foreach (var bit in wearExamples.Select(x => x.WearBits).Distinct())
		{
			combos[bit].AddRange(wearExamples.Where(x => x.WearBits == bit).Select(x => x.ShortDescription));
		}

		using (var fs = new FileStream("RpiWearCombos.csv", FileMode.Create))
		{
			using (var writer = new StreamWriter(fs))
			{
				foreach (var combo in combos)
				{
					writer.WriteLine($"{combo.Key.GetAllFlags().Select(x => x.DescribeEnum()).ListToCommaSeparatedValues(" | ")},\"{combo.Value.ListToCommaSeparatedValues(" | ")}\"");
				}

				writer.Flush();
			}
		}
		
		Console.WriteLine("Hello, World!");
		Console.ReadLine();
	}
}