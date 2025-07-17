using System.Collections.Generic;
using System.Linq;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Character;

public partial class Character
{
#nullable enable
	private ICharacter? _ridingMount;
	public ICharacter? RidingMount
	{
		get => _ridingMount;
		set
		{
			_ridingMount = value;
			Changed = true;
		}
	}
#nullable restore
	public bool CanEverBeMounted(ICharacter rider)
	{
		return false;
	}

	public bool CanBeMountedBy(ICharacter rider)
	{
		return false;
	}

	public string WhyCannotBeMountedBy(ICharacter rider)
	{
		return $"{HowSeen(rider, true)} is not something that can be mounted.";
	}

	private readonly List<ICharacter> _riders = new();

	public IEnumerable<ICharacter> Riders => _riders;

	public bool Mount(ICharacter rider)
	{
		if (!CanBeMountedBy(rider))
		{
			rider.OutputHandler.Send(new EmoteOutput(new Emote(WhyCannotBeMountedBy(rider), rider, rider, this)));
			return false;
		}

		var ai = (this as INPC)?.AIs.OfType<IMountableAI>().FirstOrDefault();
		if (ai is null)
		{
			rider.OutputHandler.Send(new EmoteOutput(new Emote("@ is not something that you can mount.", this, this)));
			return false;
		}

		_riders.Add(rider);
		rider.RidingMount = this;
		rider.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("DefaultCannotMountError"), rider, rider, this)));
		return false;
	}

	public void Dismount(ICharacter rider)
	{
		rider.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("DefaultDismountMessage"), rider, rider, this)));
		_riders.Remove(rider);
	}

	public void RemoveRider(ICharacter rider)
	{
		_riders.Remove(rider);
	}

	public Difficulty ControlMountDifficulty(ICharacter rider)
	{
		return Difficulty.Impossible;
	}

	public bool IsPrimaryRider(ICharacter rider)
	{
		return _riders.FirstOrDefault() == rider;
	}

	public bool BuckRider()
	{
		return false;
	}
}