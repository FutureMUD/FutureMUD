using MudSharp.Character;
using MudSharp.Effects.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class BlockLayerChange : BlockingCommandDelay
{
	public BlockLayerChange(ICharacter owner) : base(owner, new[] { "ascend", "dive", "climb", "land", "fly" },
		new[] { "general", "movement" }, "recovering from your recent swim, fly or climb",
		"You must wait a short time before doing that.",
		() => owner.OutputHandler.Send("You can now swim, climb or fly once again."))
	{
	}

	protected override string SpecificEffectType => "BlockLayerChange";
}