using MudSharp.Framework;
using System.Text;
using System.Text.RegularExpressions;

namespace RPI_Engine_Worldfile_Converter;

internal class Program
{
    public static Regex ItemDefinitionRegex = new("^#\\d+$");
    static async Task Main(string[] args)
    {
        //Console.WriteLine("Input File:");
        //var path = Console.ReadLine();
        List<RPIItem> items = new();
        string headPath = @"C:\Users\luker\OneDrive\source\repos\FutureMUD\RPI Engine Worldfile Converter\soiregions-main\";
        foreach (string path in Directory.GetFiles(headPath, "objs.*"))
        {
            using (FileStream fs = new(path, FileMode.Open))
            {
                using StreamReader sr = new(fs, Encoding.ASCII);
                StringBuilder sb = new();
                string? line = await sr.ReadLineAsync();
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
                        RPIItem item = new(sb.ToString());
                        items.Add(item);
                    }
                    catch
                    {
                        Console.WriteLine($"Error with item {sb.ToString().Split(new[] { "\n", "\r\n" }, StringSplitOptions.None)[0]} in file {path}. Skipped");
                    }
                    finally
                    {
                        sb.Clear();
                    }
                }
            }
        }

        CollectionDictionary<RPIItemType, RPIItem> sorted = new();
        List<RPIItem> wearExamples = new();
        foreach (RPIItem item in items)
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

        using (FileStream fs = new("ItemTypes.csv", FileMode.Create))
        {
            using (StreamWriter writer = new(fs))
            {
                foreach (KeyValuePair<RPIItemType, List<RPIItem>> combo in sorted)
                {
                    writer.WriteLine($"{combo.Key.DescribeEnum()},\"{combo.Value.Select(x => x.ShortDescription).ListToCommaSeparatedValues(" | ")}\"");
                }

                writer.Flush();
            }
        }

        CollectionDictionary<RPIWearBits, string> combos = new();
        foreach (RPIWearBits bit in wearExamples.Select(x => x.WearBits).Distinct())
        {
            combos[bit].AddRange(wearExamples.Where(x => x.WearBits == bit).Select(x => x.ShortDescription));
        }

        using (FileStream fs = new("RpiWearCombos.csv", FileMode.Create))
        {
            using (StreamWriter writer = new(fs))
            {
                foreach (KeyValuePair<RPIWearBits, List<string>> combo in combos)
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