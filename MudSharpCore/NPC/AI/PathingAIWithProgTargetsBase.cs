using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.NPC.AI;

public abstract class PathingAIWithProgTargetsBase : PathingAIBase
{
	public IFutureProg PathingEnabledProg { get; protected set; }

	public IFutureProg OnStartToPathProg { get; protected set; }

	public IFutureProg TargetLocationProg { get; set; }

	public IFutureProg FallbackLocationProg { get; set; }

	public IFutureProg WayPointsProg { get; set; }

	#region Overrides of PathingAIBase

	protected override bool IsPathingEnabled(ICharacter ch)
	{
		return (bool?)PathingEnabledProg.Execute(ch) ?? false;
	}

	#endregion

	protected PathingAIWithProgTargetsBase(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	#region Overrides of PathingAIBase

	protected override void LoadFromXML(XElement root)
	{
		PathingEnabledProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("PathingEnabledProg")?.Value ?? "0"));
		OnStartToPathProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnStartToPathProg")?.Value ?? "0"));
		OpenDoors = bool.Parse(root.Element("OpenDoors")?.Value ?? "false");
		UseKeys = bool.Parse(root.Element("UseKeys")?.Value ?? "false");
		SmashLockedDoors = bool.Parse(root.Element("SmashLockedDoors")?.Value ?? "false");
		CloseDoorsBehind = bool.Parse(root.Element("CloseDoorsBehind")?.Value ?? "false");
		UseDoorguards = bool.Parse(root.Element("UseDoorguards")?.Value ?? "false");
		TargetLocationProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("TargetLocationProg")?.Value ?? "0"));
		FallbackLocationProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("FallbackLocationProg")?.Value ?? "0"));
		WayPointsProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WayPointsProg")?.Value ?? "0"));
		MoveEvenIfObstructionInWay = bool.Parse(root.Element("MoveEvenIfObstructionInWay")?.Value ?? "false");
	}

	#endregion
}