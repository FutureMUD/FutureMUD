using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.PerceptionEngine.Handlers;

public class IgnorantItemOutputHandler : IOutputHandler
{
	private readonly IGameItem _item;

	public IgnorantItemOutputHandler(IGameItem item)
	{
		_item = item;
	}

	public IPerceiver Perceiver => _item;

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
		return false;
	}

	public bool QuietMode { get; set; }

	public void More()
	{
		// Do nothing
	}
}