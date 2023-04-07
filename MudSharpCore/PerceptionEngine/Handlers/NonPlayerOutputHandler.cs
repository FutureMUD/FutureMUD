using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.PerceptionEngine.Handlers;

public class NonPlayerOutputHandler : IOutputHandler
{
	public IPerceiver Perceiver { get; protected set; }

	public bool HasBufferedOutput => false;

	public string BufferedOutput => null;

	public bool Send(string text, bool newline = true, bool nopage = false)
	{
		return false;
	}

	public bool Send(IOutput output, bool newline = true, bool nopage = false)
	{
		return false;
	}

	public bool SendPrompt()
	{
		return false;
	}

	public void Flush()
	{
	}

	public bool Register(IPerceiver perceiver)
	{
		Perceiver = perceiver;
		if (perceiver != null && perceiver.OutputHandler == null)
		{
			perceiver.Register(this);
		}

		return true;
	}

	public bool QuietMode { get; set; }

	public void More()
	{
		// Do nothing
	}
}