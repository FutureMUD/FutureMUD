using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine;

public class QuickEmote
{
	private readonly string _emote;
	private readonly IPerceiver _perceiver;
	private readonly IPerceivable[] _perceivables;

	public QuickEmote(string emote, IPerceiver perceiver, params IPerceivable[] perceivables)
	{
		_emote = emote;
		_perceiver = perceiver;
		_perceivables = perceivables;
	}

	public static implicit operator string(QuickEmote emote)
	{
		return new EmoteOutput(new Emote(emote._emote, emote._perceiver, emote._perceivables)).ParseFor(
			emote._perceiver);
	}
}