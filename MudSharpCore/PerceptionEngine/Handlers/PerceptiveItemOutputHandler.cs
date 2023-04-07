using System;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.PerceptionEngine.Handlers;

public class PerceptiveItemOutputHandler : IOutputHandler
{
	public PerceptiveItemOutputHandler(IGameItem item)
	{
		//Perceiver = item;
	}

	public IPerceiver Perceiver { get; protected set; }

	public bool HasBufferedOutput => throw new NotImplementedException();

	public string BufferedOutput => throw new NotImplementedException();

	public bool Send(string text, bool newline = true, bool nopage = false)
	{
		throw new NotImplementedException();
	}

	public bool Send(IOutput output, bool newline = true, bool nopage = false)
	{
		throw new NotImplementedException();
	}

	public bool SendPrompt()
	{
		throw new NotImplementedException();
	}

	public void Flush()
	{
		throw new NotImplementedException();
	}

	public bool Register(IPerceiver perceiver)
	{
		return false;
	}

	public bool QuietMode { get; set; }

	public void More()
	{
		// Do nothing
	}
}