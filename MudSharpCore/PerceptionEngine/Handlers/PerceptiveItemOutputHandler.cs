using System;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine;

namespace MudSharp.PerceptionEngine.Handlers;

public class PerceptiveItemOutputHandler : IOutputHandler
{
        private readonly IGameItem _item;

        public PerceptiveItemOutputHandler(IGameItem item)
        {
                _item = item;
                Perceiver = item;
        }

        public IPerceiver Perceiver { get; protected set; }

        public bool HasBufferedOutput => false;

        public string BufferedOutput => null;

        public bool Send(string text, bool newline = true, bool nopage = false)
        {
                if (QuietMode || string.IsNullOrEmpty(text))
                {
                        return false;
                }

                this.Handle(text, OutputRange.Local);
                return true;
        }

        public bool Send(IOutput output, bool newline = true, bool nopage = false)
        {
                if (output == null)
                {
                        return false;
                }

                this.Handle(output, OutputRange.Local);
                return true;
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