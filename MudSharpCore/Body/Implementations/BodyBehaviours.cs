using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Strategies.BodyStratagies;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	#region Properties

	#region Local

	public new Gendering Gender { get; protected set; }

	#endregion

	#region Strategies

	public IBodyCommunicationStrategy Communications =>
		// TODO - merits, effects etc that might impact on this
		Race.CommunicationStrategy;

	#endregion

	#endregion

	#region ICommunicate

	public void Emote(string emote, bool permitSpeech = true, OutputFlags additionalConditions = OutputFlags.Normal)
	{
		if (CurrentLanguage == null)
		{
			permitSpeech = false;
		}

		Communications.Emote(this, emote, permitSpeech, additionalConditions);
	}

	public void Say(IPerceivable target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.Say(this, target, message, emote);
	}

	public void Talk(IPerceivable target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.Talk(this, target, message, emote);
	}

	public void Transmit(IGameItem target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.Transmit(this, target, message, emote);
	}

	public void Whisper(IPerceivable target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.Whisper(this, target, message, emote);
	}

	public void Shout(IPerceivable target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.Shout(this, target, message, emote);
	}

	public void LoudSay(IPerceivable target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.LoudSay(this, target, message, emote);
	}

	public void Yell(IPerceivable target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.Yell(this, target, message, emote);
	}

	public void Sing(IPerceivable target, string message, IEmote emote = null)
	{
		if (CurrentLanguage == null)
		{
			Actor.OutputHandler.Send(
				"You are not currently speaking any language. You must set a language before you can speak.");
			return;
		}

		Communications.Sing(this, target, message, emote);
	}

	#endregion

	#region IManipulator

	public bool CanOpen(IOpenable openable)
	{
		if (HoldLocs.All(x => CanUseBodypart(x) != CanUseBodypartResult.CanUse))
		{
			WhyCannotOpen = $" you do not have any undamaged {WielderDescriptionPlural.ToLowerInvariant()}.";
			return false;
		}

		// TODO - check for things like damage, effects
		if (openable.CanOpen(this))
		{
			return true;
		}

		switch (openable.WhyCannotOpen(this))
		{
			case WhyCannotOpenReason.AlreadyOpen:
				WhyCannotOpen = " is already open.";
				break;
			case WhyCannotOpenReason.Jammed:
				WhyCannotOpen = " is jammed shut.";
				break;
			case WhyCannotOpenReason.Locked:
				WhyCannotOpen = " must be unlocked before it can be opened.";
				break;
			case WhyCannotOpenReason.NotOpenable:
				WhyCannotOpen = " is not something that can be opened.";
				break;
			case WhyCannotOpenReason.AlternateMechanism:
				WhyCannotOpen = " has an alternate mechanism for opening, such as a button or automatic control.";
				break;
			default:
				WhyCannotOpen = " is not something that can be opened at this time.";
				break;
		}

		return false;
	}

	public void Open(IOpenable openable, ICharacter openableOwner, IEmote playerEmote, bool useCouldLogic = false)
	{
		if (useCouldLogic)
		{
			var why = openable.WhyCannotOpen(this);
			switch (why)
			{
				case WhyCannotOpenReason.Locked:
					var lockable = openable.Parent.GetItemType<ILockable>();
					if (lockable == null)
					{
						return;
					}

					var locks = lockable.Locks.ToList();
					var keysRequired = new List<IGameItem>();
					foreach (var key in ExternalItems.SelectMany(x => x.ShallowAccessibleItems(Actor))
					                                 .SelectNotNull(x => x.GetItemType<IKey>()))
					{
						if (locks.RemoveAll(x => x.CanUnlock(Actor, key)) > 0)
						{
							keysRequired.Add(key.Parent);
						}

						if (!locks.Any())
						{
							break;
						}
					}

					var plan = InventoryPlanFactory.CreatePhaseForEachItem(Actor, keysRequired, DesiredItemState.Held,
						"key");
					if (plan.PlanIsFeasible() != GameItems.Inventory.InventoryPlanFeasibility.Feasible)
					{
						return;
					}

					while (!plan.IsFinished)
					{
						var key = plan.ExecutePhase().First(x => x.OriginalReference.Equals("key")).PrimaryTarget
						              .GetItemType<IKey>();
						foreach (var theLock in lockable.Locks)
						{
							if (!theLock.IsLocked || !theLock.CanUnlock(Actor, key))
							{
								continue;
							}

							theLock.Unlock(Actor, key, lockable.Parent, null);
						}
					}

					return;
			}
		}

		if (openableOwner == null)
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ open|opens $0", this, openable.Parent)).Append(playerEmote));
			var door = openable as IDoor;
			door?.InstalledExit.Opposite(Location)
			    .Handle(new EmoteOutput(new Emote("@ is opened from the other side.", door.Parent)));
		}
		else
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ open|opens $1's !0", this, openable.Parent, openableOwner)).Append(
					playerEmote));
		}

		openable.Open();
	}

	public bool CouldOpen(IOpenable openable)
	{
		if (CanOpen(openable))
		{
			return true;
		}

		var why = openable.WhyCannotOpen(this);
		switch (why)
		{
			case WhyCannotOpenReason.AlreadyOpen:
				return true;
			case WhyCannotOpenReason.Locked:
				var lockable = openable.Parent.GetItemType<ILockable>();
				if (lockable == null)
				{
					return false;
				}

				var locks = lockable.Locks.ToList();
				foreach (var key in ExternalItems.SelectMany(x => x.ShallowAccessibleItems(Actor))
				                                 .SelectNotNull(x => x.GetItemType<IKey>()))
				{
					locks.RemoveAll(x => x.CanUnlock(Actor, key));
					if (!locks.Any())
					{
						break;
					}
				}

				return !locks.Any();
		}

		return false;
	}

	public string WhyCannotOpen { get; protected set; }

	public bool CanClose(IOpenable openable)
	{
		if (HoldLocs.All(x => CanUseBodypart(x) != CanUseBodypartResult.CanUse))
		{
			WhyCannotClose = $" you do not have any undamaged {WielderDescriptionPlural.ToLowerInvariant()}.";
			return false;
		}

		if (openable.CanClose(this))
		{
			return true;
		}

		switch (openable.WhyCannotClose(this))
		{
			case WhyCannotCloseReason.SingleUse:
				WhyCannotClose = " it cannot be closed once open.";
				break;
			case WhyCannotCloseReason.AlreadyClosed:
				WhyCannotClose = " it is already closed.";
				break;
			case WhyCannotCloseReason.Jammed:
				WhyCannotClose = " it is jammed open.";
				break;
			case WhyCannotCloseReason.Locked:
				WhyCannotClose = " it must be unlocked before it can be closed.";
				break;
			case WhyCannotCloseReason.NotOpenable:
				WhyCannotClose = " it is not something that can be closed.";
				break;
			default:
				WhyCannotClose = " it is not something that can be closed.";
				break;
		}

		return false;
	}

	public void Close(IOpenable openable, ICharacter openableOwner, IEmote playerEmote)
	{
		if (openableOwner == null)
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ close|closes $0", this, openable.Parent)).Append(playerEmote));
			if (openable is IDoor door)
			{
				var allExit = Gameworld.ExitManager.GetAllExits(Location);
				door.InstalledExit.Opposite(Location)
				    .Handle(new EmoteOutput(new Emote("@ is closed from the other side.", door.Parent)));
			}
		}
		else
		{
			OutputHandler.Handle(
				new MixedEmoteOutput(new Emote("@ close|closes $1's !0", this, openable.Parent, openableOwner)).Append(
					playerEmote));
		}

		openable.Close();
	}

	public string WhyCannotClose { get; protected set; }

	public bool CanConnect(IConnectable connectable, IConnectable other)
	{
		if (connectable == null || other == null)
		{
			return false;
		}

		return HoldLocs.Any(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse) &&
		       connectable.CanConnect(Actor, other);
	}

	public bool Connect(IConnectable connectable, IConnectable other, IPerceivable ownerConnectable = null,
		IPerceivable ownerOther = null, IEmote playerEmote = null)
	{
		if (!CanConnect(connectable, other))
		{
			OutputHandler.Send(WhyCannotConnect(connectable, other));
			return false;
		}

		connectable.Connect(Actor, other);
		OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ connect|connects $0$?2| on $2||$ to $1$?3| on $3 ||$", Actor,
					connectable.Parent, other.Parent, ownerConnectable, ownerOther))
				.Append(playerEmote));
		return true;
	}

	public string WhyCannotConnect(IConnectable connectable, IConnectable other)
	{
		if (HoldLocs.All(x => CanUseBodypart(x) != CanUseBodypartResult.CanUse))
		{
			return
				$"You do not have any undamaged {WielderDescriptionPlural.ToLowerInvariant()} with which to connect anything.";
		}

		return connectable?.WhyCannotConnect(Actor, other) ?? "You cannot connect that.";
	}

	public bool CanDisconnect(IConnectable connectable, IConnectable other)
	{
		if (connectable == null || other == null)
		{
			return false;
		}

		return HoldLocs.Any(x => CanUseBodypart(x) == CanUseBodypartResult.CanUse) &&
		       connectable.CanDisconnect(Actor, other);
	}

	public bool Disconnect(IConnectable connectable, IConnectable other, IPerceivable ownerConnectable = null,
		IPerceivable ownerOther = null, IEmote playerEmote = null)
	{
		if (!CanDisconnect(connectable, other))
		{
			OutputHandler.Send(WhyCannotDisconnect(connectable, other));
			return false;
		}

		connectable.Disconnect(Actor, other);
		OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ disconnect|disconnects $0$?2| on $2||$ from $1$?3| on $3 ||$", Actor,
				connectable.Parent,
				other.Parent, ownerConnectable, ownerOther)).Append(playerEmote));
		return true;
	}

	public string WhyCannotDisconnect(IConnectable connectable, IConnectable other)
	{
		if (HoldLocs.All(x => CanUseBodypart(x) != CanUseBodypartResult.CanUse))
		{
			return
				$"You do not have any undamaged {WielderDescriptionPlural.ToLowerInvariant()} with which to disconnect anything.";
		}

		return connectable?.WhyCannotDisconnect(Actor, other) ?? "You cannot disconnect that.";
	}

	#endregion
}