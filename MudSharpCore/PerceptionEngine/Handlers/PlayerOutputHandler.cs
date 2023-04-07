using System.IO;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.PerceptionEngine.Handlers;

public class PlayerOutputHandler : IOutputHandler
{
	private bool _overrideSendPrompt;

	public PlayerOutputHandler(StringBuilder outgoing, ICharacter actor)
	{
		Outgoing = outgoing;
		Perceiver = actor;
		CharacterPerceiver = actor;
		QuietMode = false;
	}

	protected StringBuilder Outgoing { get; set; }

	protected string PagedString { get; set; }

	public IPerceiver Perceiver { get; protected set; }

	public ICharacter CharacterPerceiver { get; protected set; }

	public bool HasBufferedOutput => _overrideSendPrompt || Outgoing.Length > 0;

	public string BufferedOutput => Outgoing.ToString();

	public bool Register(IPerceiver perceiver)
	{
		Perceiver = perceiver;
		CharacterPerceiver = Perceiver as ICharacter;
		if (perceiver != null && perceiver.OutputHandler == null)
		{
			perceiver.Register(this);
		}

		return true;
	}

	public bool Send(string text, bool newline = true, bool nopage = false)
	{
		if (QuietMode || text == null)
		{
			return false;
		}

		if (CharacterPerceiver?.Account?.AppendNewlinesBetweenMultipleEchoesPerPrompt == true && Outgoing.Length > 0)
		{
			Outgoing.Append("\n");
		}

		if (!nopage && Perceiver != null && text.Count(x => x == '\n') > Perceiver.Account.PageLength * 1.25)
		{
			var reader = new StringReader(text);
			var sb = new StringBuilder();
			for (var i = 0; i < Perceiver.Account.PageLength; i++)
			{
				sb.AppendLine(reader.ReadLine());
			}

			PagedString = reader.ReadToEnd();
			sb.AppendLineFormat("*** Type more to read further ***".Colour(Telnet.Yellow));
			text = sb.ToString();
		}

		if (newline)
		{
			Outgoing.AppendLine(text);
		}
		else
		{
			Outgoing.Append(text);
		}

		return true;
	}

	public bool Send(IOutput output, bool newline = true, bool nopage = false)
	{
		if (output == null)
		{
			return false;
		}

		return output.ShouldSee(Perceiver) && Send(output.ParseFor(Perceiver), newline, nopage);
	}

	public bool SendPrompt()
	{
		_overrideSendPrompt = true;
		return true;
	}

	public void Flush()
	{
		Outgoing.Clear();
		_overrideSendPrompt = false;
	}

	public bool QuietMode { get; set; }

	public void More()
	{
		if (string.IsNullOrEmpty(PagedString))
		{
			Outgoing.AppendLine("There is nothing more to see.");
			return;
		}

		var reader = new StringReader(PagedString);
		var sb = new StringBuilder();
		for (var i = 0; i < Perceiver.Account.PageLength; i++)
		{
			if (reader.Peek() == -1)
			{
				// There wasn't enough to re-page, just short circuit the process
				Outgoing.Append(PagedString);
				PagedString = null;
				return;
			}

			sb.AppendLine(reader.ReadLine());
		}

		PagedString = reader.ReadToEnd();
		sb.AppendLineFormat("*** Type {0} to read further ***", "more".Colour(Telnet.Yellow));
		Outgoing.Append(sb);
	}

	public override string ToString()
	{
		return Outgoing.ToString();
	}
}