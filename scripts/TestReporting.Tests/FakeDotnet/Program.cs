using System.Xml.Linq;

var operation = args.FirstOrDefault();
var scenario = Environment.GetEnvironmentVariable("FM_FAKE_SCENARIO") ?? "pass";
if (operation == "--version") { Console.WriteLine("10.0.401-fake"); return 0; }
if (operation == "build")
{
	if (scenario == "bootstrap-hang") { await Task.Delay(TimeSpan.FromSeconds(10)); return 0; }
	if (scenario == "buildfail") { Console.Error.WriteLine("sample.cs(1,1): error CS1001: expected identifier"); return 1; }
	if (scenario == "prereq") { Console.Error.WriteLine("error NU1301: Unable to load the service index"); return 1; }
	var configAt = Array.IndexOf(args, "-c");
	var configuration = configAt < 0 ? "Debug" : args[configAt + 1];
	var project = args[1];
	var binary = Path.Combine(Path.GetDirectoryName(project)!, "bin", configuration, "net10.0", Path.GetFileNameWithoutExtension(project) + ".dll");
	Directory.CreateDirectory(Path.GetDirectoryName(binary)!);
	File.WriteAllText(binary, "fake binary identity");
	Console.WriteLine("Build succeeded.");
	return 0;
}
if (operation != "test") { Console.Error.WriteLine("unexpected operation"); return 4; }
if (scenario == "timeout") { await Task.Delay(TimeSpan.FromSeconds(10)); return 0; }
if (scenario == "changed") File.AppendAllText("input.txt", "changed");
if (scenario is "huge" or "failhuge") Console.WriteLine(new string('W', 200_000));
var resultIndex = Array.IndexOf(args, "--results-directory");
if (resultIndex < 0 || resultIndex + 1 >= args.Length) return 4;
var directory = args[resultIndex + 1];
Directory.CreateDirectory(directory);
var report = Path.Combine(directory, "results.trx");
if (scenario == "missing") return 0;
if (scenario == "malformed") { File.WriteAllText(report, "<TestRun"); return 0; }
var fixture = Environment.GetEnvironmentVariable("FM_FAKE_TRX")!;
var document = XDocument.Load(fixture);
var ns = document.Root!.Name.Namespace;
if (scenario != "stale")
{
	var now = DateTimeOffset.UtcNow.ToString("O");
	var times = document.Root.Element(ns + "Times")!;
	foreach (var name in new[] { "creation", "queuing", "start", "finish" }) times.SetAttributeValue(name, now);
}
foreach (var definition in document.Root.Element(ns + "TestDefinitions")!.Elements(ns + "UnitTest"))
{
	definition.SetAttributeValue("storage", "Sample Tests.dll");
	definition.Element(ns + "TestMethod")?.SetAttributeValue("codeBase", "Sample Tests.dll");
}
var results = document.Root.Element(ns + "Results")!;
var rows = results.Elements(ns + "UnitTestResult").ToList();
var counters = document.Root.Element(ns + "ResultSummary")!.Element(ns + "Counters")!;
if (scenario is "fail" or "failhuge")
{
	rows[0].SetAttributeValue("outcome", "Failed");
	rows[0].Add(new XElement(ns + "Output", new XElement(ns + "ErrorInfo", new XElement(ns + "Message", "expected 2, got 3"), new XElement(ns + "StackTrace", "at fixture"))));
	counters.SetAttributeValue("passed", "35");
	counters.SetAttributeValue("failed", "1");
}
if (scenario == "skip")
{
	rows[0].SetAttributeValue("outcome", "NotExecuted");
	counters.SetAttributeValue("passed", "35");
	counters.SetAttributeValue("notExecuted", "1");
}
if (scenario == "zero")
{
	results.RemoveNodes();
	counters.SetAttributeValue("total", "0");
	counters.SetAttributeValue("executed", "0");
	counters.SetAttributeValue("passed", "0");
}
document.Save(report);
if (scenario == "contradiction") { Console.WriteLine("all passed"); return 7; }
return scenario is "fail" or "failhuge" ? 1 : 0;
