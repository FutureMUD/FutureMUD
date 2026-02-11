using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public class AIStorytellerCustomToolCall
{
	public bool IsValid
	{
		get
		{
			if (Prog is null)
			{
				return false;
			}

			if (!string.IsNullOrEmpty(Prog.CompileError))
			{
				return false;
			}

			RecheckFunctionParameters();

			var parameterNames = Prog.NamedParameters.Select(x => x.Item2).ToList();
			if (parameterNames.Count != ParameterDescriptions.Count)
			{
				return false;
			}

			if (parameterNames.Any(x => !ParameterDescriptions.ContainsKey(x)))
			{
				return false;
			}

			return true;
		}
	}

	private void RecheckFunctionParameters()
	{
		foreach (var parameter in Prog?.NamedParameters ?? [])
		{
			if (!ParameterDescriptions.ContainsKey(parameter.Item2))
			{
				_parameterDescriptions[parameter.Item2] = "";
			}
		}
	}

	public string Name { get; set; }
	public string Description { get; set; }
	public IFutureProg Prog { get; set; }
	private Dictionary<string, string> _parameterDescriptions;
	public IReadOnlyDictionary<string, string> ParameterDescriptions => _parameterDescriptions;

	public void RefreshParameterDescriptions()
	{
		RecheckFunctionParameters();
	}

	public void SetParameterDescription(string parameterName, string description)
	{
		_parameterDescriptions[parameterName] = description;
	}

	public XElement SaveToXml(bool includeWithEcho)
	{
		return new XElement("ToolCall",
			new XElement("Name", new XCData(Name)),
			new XElement("Description", new XCData(Description)),
			new XElement("Prog", Prog?.Id ?? 0L),
			new XElement("IncludeWithEcho", includeWithEcho),
			new XElement("ParameterDescriptions",
				from item in ParameterDescriptions
				select new XElement("Description", new XAttribute("name", item.Key), new XCData(item.Value))
			)
		);
	}

	public AIStorytellerCustomToolCall(string name, string description, IFutureProg prog)
	{
		Name = name;
		Description = description;
		Prog = prog;
		_parameterDescriptions = new Dictionary<string, string>();
		RecheckFunctionParameters();
	}

	public AIStorytellerCustomToolCall(XElement root, IFuturemud gameworld)
	{
		Name = root.Element("Name")?.Value ?? "UnnamedTool";
		Description = root.Element("Description")?.Value ?? string.Empty;
		Prog = gameworld.FutureProgs.Get(long.Parse(root.Element("Prog")?.Value ?? "0"));
		var dictionary = new Dictionary<string, string>();
		foreach (var description in root.Element("ParameterDescriptions")?.Elements("Description") ??
		                             Enumerable.Empty<XElement>())
		{
			var name = description.Attribute("name")?.Value;
			if (string.IsNullOrWhiteSpace(name))
			{
				continue;
			}

			dictionary[name] = description.Value;
		}
		_parameterDescriptions = dictionary;
		RecheckFunctionParameters();
	}
}

