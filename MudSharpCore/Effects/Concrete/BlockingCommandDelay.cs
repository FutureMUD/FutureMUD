using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class BlockingCommandDelay : CommandDelay
{
	public BlockingCommandDelay(ICharacter owner, string command, IEnumerable<string> blocks, string blockMessage,
		string message = "You must wait a short time before doing that again", Action onExpireAction = null)
		: base(owner, command, message, onExpireAction)
	{
		_blocks = blocks.ToList();
		_blockingDescription = blockMessage;
	}

	public BlockingCommandDelay(ICharacter owner, IEnumerable<string> commands, IEnumerable<string> blocks,
		string blockMessage, string message = "You must wait a short time before doing that again",
		Action onExpireAction = null)
		: base(owner, commands, message, onExpireAction)
	{
		_blocks = blocks.ToList();
		_blockingDescription = blockMessage;
	}

	private readonly List<string> _blocks;
	public override IEnumerable<string> Blocks => _blocks;

	private readonly string _blockingDescription;

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return _blockingDescription;
	}

	public override bool IsBlockingEffect(string blockingType)
	{
		return _blocks.Any(x => x.EqualTo(blockingType));
	}

	protected override string SpecificEffectType => "BlockingCommandDelay";
}