using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.NPC.AI;

public enum WouldOpenResponseType
{
	WontOpen,
	WillOpenIfMove,
	WillOpenIfSocial,
	WillOpenIfKnock
}

public class WouldOpenResponse
{
	public WouldOpenResponseType Response { get; set; }
	public string Social { get; set; }
	public bool DirectionRequired { get; set; }
	public bool SocialTargetRequired { get; set; }
}

public class DoorguardAI : ArtificialIntelligenceBase
{
	protected IFutureProg BaseDelayProg;
	protected IFutureProg CantOpenDoorActionProg;
	protected IFutureProg CloseDoorActionProg;
	protected IFutureProg OnWitnessDoorStopProg;
	protected IFutureProg OpenCloseDelayProg;
	protected IFutureProg OpenDoorActionProg;
	protected bool OwnSideOnly;
	protected string RequiredSocialTrigger;
	protected bool RespectGameRulesForOpeningDoors;
	protected bool RespondToSocialDirection;
	protected bool SocialTargettedOnly;

	protected IFutureProg WillOpenDoorForProg;
	protected IFutureProg WontOpenDoorForActionProg;

	public WouldOpenResponse WouldOpen(ICharacter doorguard, ICharacter target, ICellExit direction)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WontOpen
			};
		}

		if (WillOpenDoorForProg?.Execute<bool?>(doorguard, target, direction) != true)
		{
			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WontOpen
			};
		}

		if (doorguard.Location != direction.Origin)
		{
			if (OwnSideOnly)
			{
				return new WouldOpenResponse
				{
					Response = WouldOpenResponseType.WontOpen
				};
			}

			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WillOpenIfKnock
			};
		}

		if (SocialTargettedOnly)
		{
			return new WouldOpenResponse
			{
				Response = WouldOpenResponseType.WillOpenIfSocial,
				Social = RequiredSocialTrigger,
				DirectionRequired = RespondToSocialDirection,
				SocialTargetRequired = SocialTargettedOnly
			};
		}

		return new WouldOpenResponse
		{
			Response = WouldOpenResponseType.WillOpenIfMove
		};
	}

	public DoorguardAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	protected virtual void LoadFromXml(XElement root)
	{
		OwnSideOnly = bool.Parse(root.Element("OwnSideOnly")?.Value ?? "false");
		WillOpenDoorForProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WillOpenDoorForProg")?.Value ?? "0"));
		WontOpenDoorForActionProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("WontOpenDoorForActionProg")?.Value ?? "0"));
		CantOpenDoorActionProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("CantOpenDoorActionProg")?.Value ?? "0"));
		OpenDoorActionProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OpenDoorActionProg")?.Value ?? "0"));
		CloseDoorActionProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("CloseDoorActionProg")?.Value ?? "0"));
		BaseDelayProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("BaseDelayProg")?.Value ?? "0"));
		OpenCloseDelayProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OpenCloseDelayProg")?.Value ?? "0"));
		OnWitnessDoorStopProg =
			Gameworld.FutureProgs.Get(long.Parse(root.Element("OnWitnessDoorStopProg")?.Value ?? "0"));
		RespectGameRulesForOpeningDoors =
			bool.Parse(root.Element("RespectGameRulesForOpeningDoors")?.Value ?? "true");

		var element = root.Element("Social");
		if (element != null)
		{
			RequiredSocialTrigger = element.Attribute("Trigger").Value;
			SocialTargettedOnly = bool.Parse(element.Attribute("TargettedOnly").Value);
			RespondToSocialDirection = bool.Parse(element.Attribute("Direction").Value);
		}
	}

	public static void RegisterLoader()
	{
		RegisterAIType("Doorguard", (ai, gameworld) => new DoorguardAI(ai, gameworld));
	}

	protected virtual bool OnWitnessMove(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (exit.Exit.Door == null || !string.IsNullOrEmpty(RequiredSocialTrigger))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>() ||
		    doorguard.AffectedBy<IDoorguardOpeningDoorEffect>())
		{
			return false;
		}

		if ((bool?)WillOpenDoorForProg.Execute(doorguard, mover, exit) ?? false)
		{
			// TODO - can open might need to be more AI-based than capability based
			if (RespectGameRulesForOpeningDoors && !exit.Exit.Door.IsOpen && !exit.Exit.Door.CanOpen(doorguard.Body))
			{
				CantOpenDoorActionProg.Execute(doorguard, mover, exit);
				return true;
			}

			var baseDelay = Convert.ToDouble(BaseDelayProg.Execute(doorguard, mover, exit));
			doorguard.AddEffect(new DoorguardOpenDoor(doorguard,
					perceivable => { OpenDoorActionProg.Execute(doorguard, mover, exit); }),
				TimeSpan.FromMilliseconds(baseDelay));

			doorguard.AddEffect(new DoorguardOpeningDoor(doorguard));

			doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
					perceivable => { CloseDoorIfStillOpen(doorguard, mover, exit); }),
				TimeSpan.FromMilliseconds(baseDelay +
				                          Convert.ToDouble(OpenCloseDelayProg.Execute(doorguard, mover, exit))));
			return true;
		}

		return false;
	}

	protected virtual bool OnWitnessSocial(ICharacter doorguard, ICharacter socialite, string social,
		bool socialTarget, ICellExit socialDirection)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (string.IsNullOrEmpty(RequiredSocialTrigger) || (SocialTargettedOnly && !socialTarget))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>() ||
		    doorguard.AffectedBy<IDoorguardOpeningDoorEffect>())
		{
			return false;
		}

		if (!((bool?)WillOpenDoorForProg.Execute(doorguard, socialite, socialDirection) ?? false))
		{
			if (WontOpenDoorForActionProg != null)
			{
				WontOpenDoorForActionProg.Execute(doorguard, socialite, socialDirection);
				return true;
			}

			return false;
		}

		var exit = RespondToSocialDirection ? socialDirection : null;
		if (exit == null)
		{
			foreach (var direction in doorguard.Location.ExitsFor(doorguard))
			{
				if (!direction.Exit.Door?.IsOpen == true &&
				    ((bool?)WillOpenDoorForProg.Execute(doorguard, socialite, direction) ?? false))
				{
					exit = direction;
					break;
				}
			}
		}

		if (exit?.Exit.Door == null)
		{
			return false;
		}

		// TODO - can open might need to be more AI-based than capability based
		if (!exit.Exit.Door.IsOpen && !exit.Exit.Door.CanOpen(doorguard.Body))
		{
			CantOpenDoorActionProg.Execute(doorguard, socialite, exit);
			return true;
		}

		var baseDelay = Convert.ToDouble(BaseDelayProg.Execute(doorguard, socialite, exit));
		doorguard.AddEffect(new DoorguardOpenDoor(doorguard,
				perceiver => { OpenDoorActionProg.Execute(doorguard, socialite, exit); }),
			TimeSpan.FromMilliseconds(baseDelay));
		doorguard.AddEffect(new DoorguardOpeningDoor(doorguard));
		doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
				perceiver => { CloseDoorIfStillOpen(doorguard, socialite, exit); }),
			TimeSpan.FromMilliseconds(baseDelay +
			                          Convert.ToDouble(OpenCloseDelayProg.Execute(doorguard, socialite, exit))));
		return true;
	}

	protected virtual void CloseDoorIfStillOpen(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead) || doorguard.Location == null || exit == null)
		{
			return;
		}

		if (exit.Exit.Door?.IsOpen == true)
		{
			if (doorguard.Location.Characters.SelectNotNull(x => x.Movement).Any(x => x.Exit == exit))
			{
				doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
					perceivable => { CloseDoorIfStillOpen(doorguard, mover, exit); }
				), TimeSpan.FromSeconds(3));
				return;
			}

			CloseDoorActionProg.Execute(doorguard, mover, exit);
		}

		doorguard.RemoveAllEffects(
			x =>
				x.IsEffectType<IDoorguardOpeningDoorEffect>() || x.IsEffectType<DoorguardCloseDoor>());
	}

	protected virtual bool OnWitnessLeave(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardOpeningDoorEffect>())
		{
			return false;
		}

		CloseDoorIfStillOpen(doorguard, mover, exit);
		return true;
	}

	protected virtual bool OnStopMovementWitness(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>())
		{
			return false;
		}

		CloseDoorIfStillOpen(doorguard, mover, exit);
		return true;
	}

	protected virtual bool OnStopMovementClosedDoorWitness(ICharacter doorguard, ICharacter mover, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (!doorguard.AffectedBy<IDoorguardModeEffect>())
		{
			return false;
		}

		if (OnWitnessDoorStopProg != null)
		{
			OnWitnessDoorStopProg.Execute(doorguard, mover, exit);
			return true;
		}

		return false;
	}

	protected virtual bool OnDoorKnock(ICharacter doorguard, ICharacter knocker, ICellExit exit)
	{
		if (doorguard.State.HasFlag(CharacterState.Dead))
		{
			return false;
		}

		if (OwnSideOnly || !doorguard.AffectedBy<IDoorguardModeEffect>() || exit.Exit.Door == null ||
		    exit.Exit.Door.IsOpen)
		{
			return false;
		}

		if (!((bool?)WillOpenDoorForProg.Execute(doorguard, knocker, exit) ?? false))
		{
			if (WontOpenDoorForActionProg != null)
			{
				WontOpenDoorForActionProg.Execute(doorguard, knocker, exit);
				return true;
			}

			return false;
		}

		if (!exit.Exit.Door.CanOpen(doorguard.Body))
		{
			if (CantOpenDoorActionProg != null)
			{
				CantOpenDoorActionProg.Execute(doorguard, knocker, exit);
				return true;
			}

			return false;
		}

		var baseDelay = Convert.ToDouble(BaseDelayProg.Execute(doorguard, knocker, exit));
		doorguard.AddEffect(
			new DoorguardOpenDoor(doorguard, perceivable => { OpenDoorActionProg.Execute(doorguard, knocker, exit); }),
			TimeSpan.FromMilliseconds(baseDelay));
		doorguard.AddEffect(new DoorguardOpeningDoor(doorguard));
		doorguard.AddEffect(new DoorguardCloseDoor(doorguard,
				perceiver => { CloseDoorIfStillOpen(doorguard, knocker, exit); }
			),
			TimeSpan.FromMilliseconds(
				baseDelay + Convert.ToDouble(OpenCloseDelayProg.Execute(doorguard, knocker, exit))));
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.CharacterStopMovementWitness:
				return OnStopMovementWitness(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterStopMovementClosedDoorWitness:
				return OnStopMovementClosedDoorWitness(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterBeginMovementWitness:
				return OnWitnessMove(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterLeaveCellWitness:
				return OnWitnessLeave(arguments[3], arguments[0], arguments[2]);
			case EventType.CharacterSocialTarget:
				return OnWitnessSocial(arguments[2], arguments[0], arguments[1].Name, true, arguments[3]);
			case EventType.CharacterSocialWitness:
				return OnWitnessSocial(arguments[4], arguments[0], arguments[1].Name, false, arguments[3]);
			case EventType.CharacterDoorKnockedOtherSide:
				return OnDoorKnock(arguments[3], arguments[0], arguments[2]);
			default:
				return false;
		}
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.CharacterStopMovementWitness:
				case EventType.CharacterStopMovementClosedDoorWitness:
				case EventType.CharacterBeginMovementWitness:
				case EventType.CharacterLeaveCellWitness:
				case EventType.CharacterSocialTarget:
				case EventType.CharacterSocialWitness:
				case EventType.CharacterDoorKnockedOtherSide:
					return true;
			}
		}

		return false;
	}
}