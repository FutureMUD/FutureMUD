using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class DoorGameItemComponent : GameItemComponent, IDoor
{
	private DoorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new DoorGameItemComponent(this, newParent, temporary);
	}

	public override void Quit()
	{
		base.Quit();
		foreach (var item in Locks.ToList())
		{
			item.Parent.Quit();
		}
	}

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Locks.ToList())
		{
			item.Parent.Delete();
		}
	}

	public override void Login()
	{
		base.Login();
		foreach (var item in Locks.ToList())
		{
			item.Parent.Login();
		}
	}

	public override bool Take(IGameItem item)
	{
		return _locks.RemoveAll(x => x.Parent == item) > 0;
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override int DecorationPriority => 50000;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		var sb = new StringBuilder();
		sb.AppendLine(description);
		if (InstalledExit != null)
		{
			if (voyeur.Location == HingeCell)
			{
				sb.AppendLine();
				sb.AppendLine(
					$"The hinges of this door have been installed on this side. It would be {UninstallDifficultyHingeSide.DescribeColoured()} to remove with the right tools."
						.ColourIncludingReset(Telnet.Yellow));
			}
			else
			{
				sb.AppendLine();
				sb.AppendLine(
					$"The hinges of this door must be installed on the other side. It would be {UninstallDifficultyNotHingeSide.DescribeColoured()} to remove with the right tools."
						.ColourIncludingReset(Telnet.Yellow));
			}
		}

		if (Locks.Any())
		{
			sb.AppendLine();
			sb.AppendLine("It has the following locks:");
			foreach (var thelock in Locks)
			{
				sb.AppendLineFormat("\t{0}", thelock.Parent.HowSeen(voyeur));
			}
		}

		return sb.ToString();
	}

	public bool IsOpen => State == DoorState.Open;

	public bool CanOpen(IBody body)
	{
		if (!_prototype.CanBeOpenedByPlayers && body?.Actor.IsAdministrator() == false)
		{
			return false;
		}

		switch (State)
		{
			case DoorState.Uninstalled:
				return false;
			case DoorState.Open:
				return false;
		}

		if (Locks.Any(x => x.IsLocked) ||
		    Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(body?.Actor)))
		{
			return false;
		}

		return true;
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
		if (!_prototype.CanBeOpenedByPlayers && opener?.Actor.IsAdministrator() == false)
		{
			return WhyCannotOpenReason.NotOpenable;
		}

		switch (State)
		{
			case DoorState.Uninstalled:
				return WhyCannotOpenReason.NotOpenable;
			case DoorState.Open:
				return WhyCannotOpenReason.AlreadyOpen;
		}

		if (Locks.Any(x => x.IsLocked) ||
		    Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(opener?.Actor)))
		{
			return WhyCannotOpenReason.Locked;
		}

		return WhyCannotOpenReason.Unknown;
	}

	public void Open()
	{
		State = DoorState.Open;
		OnOpen?.Invoke(this);
	}

	public WhyCannotCloseReason WhyCannotClose(IBody closer)
	{
		if (!_prototype.CanBeOpenedByPlayers && closer?.Actor.IsAdministrator() == false)
		{
			return WhyCannotCloseReason.NotOpenable;
		}

		switch (State)
		{
			case DoorState.Uninstalled:
				return WhyCannotCloseReason.NotOpenable;
			case DoorState.Closed:
				return WhyCannotCloseReason.AlreadyClosed;
		}

		return WhyCannotCloseReason.Unknown;
	}

	public void Close()
	{
		State = DoorState.Closed;
		OnClose?.Invoke(this);
	}

	public event OpenableEvent OnOpen;
	public event OpenableEvent OnClose;

	public bool CanClose(IBody body)
	{
		if (!_prototype.CanBeOpenedByPlayers && body?.Actor.IsAdministrator() == false)
		{
			return false;
		}

		return State == DoorState.Open;
	}

	public override double ComponentWeight
	{
		get { return Locks.Sum(x => x.Parent.Weight); }
	}

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return Locks.Sum(x => x.Parent.Buoyancy(fluidDensity));
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
	{
		var newItemLockable = newItem?.GetItemType<ILockable>();
		if (newItemLockable != null)
		{
			foreach (var theLock in Locks.Where(theLock => !newItemLockable.InstallLock(theLock)))
			{
				if (location == null)
				{
					theLock.Delete();
					continue;
				}

				location.Insert(theLock.Parent);
				theLock.Parent.ContainedIn = null;
			}
		}
		else
		{
			foreach (var theLock in Locks.ToList())
			{
				if (location == null)
				{
					theLock.Delete();
					continue;
				}

				location.Insert(theLock.Parent);
				theLock.Parent.ContainedIn = null;
			}
		}

		_locks.Clear();

		var newItemDoor = newItem?.GetItemType<IDoor>();
		if (newItemDoor != null)
		{
			newItemDoor.State = State;
			newItemDoor.InstalledExit = InstalledExit;
			newItemDoor.HingeCell = HingeCell;
			newItemDoor.OpenDirectionCell = OpenDirectionCell;
			if (InstalledExit != null)
			{
				InstalledExit.Door = newItemDoor;
				InstalledExit.Changed = true;
			}

			return true;
		}

		if (InstalledExit != null)
		{
			InstalledExit.Door = null;
			InstalledExit.Changed = true;
		}

		return false;
	}

	public override bool AffectsLocationOnDestruction => true;

	public override bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		if (_locks.Any(x => x.Parent == existingItem) && newItem.IsItemType<ILock>())
		{
			_locks[_locks.IndexOf(existingItem.GetItemType<ILock>())] = newItem.GetItemType<ILock>();
			existingItem.ContainedIn = null;
			newItem.ContainedIn = Parent;
			Changed = true;
			return true;
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (DoorGameItemComponentProto)newProto;
	}

	#region IDoor Members

	public event DoorEvent OnRemovedFromExit;
	public event DoorEvent OnChangeCanFireThrough;

	private DoorState _state;

	public DoorState State
	{
		get => _state;
		set
		{
			_state = value;
			Changed = true;
		}
	}

	private IExit _exit;

	public IExit InstalledExit
	{
		get => _exit;
		set
		{
			_exit = value;
			if (_exit == null)
			{
				OnRemovedFromExit?.Invoke(this);
			}
		}
	}

	public bool CanPlayersUninstall => _prototype.CanPlayersUninstall;
	public bool CanPlayersSmash => _prototype.CanPlayersSmash;
	public Difficulty UninstallDifficultyHingeSide => _prototype.UninstallDifficultyHingeSide;
	public Difficulty UninstallDifficultyNotHingeSide => _prototype.UninstallDifficultyNotHingeSide;
	public Difficulty SmashDifficulty => _prototype.SmashDifficulty;
	public ITraitDefinition UninstallTrait => _prototype.UninstallTrait;
	public bool CanFireThrough => _prototype.CanFireThrough;

	private ICell _hingeCell;

	public ICell HingeCell
	{
		get => _hingeCell;
		set
		{
			_hingeCell = value;
			Changed = true;
		}
	}

	private ICell _openDirectionCell;

	public ICell OpenDirectionCell
	{
		get => _openDirectionCell;
		set
		{
			_openDirectionCell = value;
			Changed = true;
		}
	}

	public void Knock(ICharacter actor, IEmote playerEmote = null)
	{
		var targetExit = InstalledExit.CellExitFor(actor.Location);
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ knock|knocks upon $0", actor, Parent)).Append(playerEmote));
		var oppositeExit = targetExit.Exit.CellExitFor(targetExit.Destination);
		targetExit.Destination.Handle(new EmoteOutput(new Emote(
			$"You hear a knocking coming from the other side of $0 to {oppositeExit.OutboundDirectionDescription}.",
			actor, Parent), flags: OutputFlags.PurelyAudible));
		foreach (var witness in targetExit.Origin.EventHandlers.Except(actor))
		{
			witness.HandleEvent(EventType.CharacterDoorKnockedSameSide, actor, actor.Location, targetExit,
				witness);
		}

		foreach (var witness in actor.Body.ExternalItems)
		{
			witness.HandleEvent(EventType.CharacterDoorKnockedSameSide, actor, actor.Location, targetExit,
				witness);
		}

		foreach (var witness in targetExit.Destination.EventHandlers)
		{
			witness.HandleEvent(EventType.CharacterDoorKnockedOtherSide, actor, actor.Location, targetExit,
				witness);
		}

		Parent.HandleEvent(EventType.DoorKnocked, actor, actor.Location, targetExit, Parent);
	}

	public bool CanCross(IBody body)
	{
		return State == DoorState.Open;
	}

	public bool CanSeeThrough(IBody body)
	{
		// TODO - transparent materials per actor (x-ray vision, heat vision, etc.)
		return _prototype.SeeThrough;
	}

	protected readonly List<ILock> _locks = new();
	public IEnumerable<ILock> Locks => _locks;

	public bool InstallLock(ILock theLock, ICharacter actor)
	{
		_locks.Add(theLock);
		if (_noSave)
		{
			theLock.Parent.LoadTimeSetContainedIn(Parent);
		}
		else
		{
			theLock.Parent.ContainedIn = Parent;
		}

		var installSide = HingeCell;
		if (actor != null)
		{
			installSide = actor.Location;
		}

		theLock.InstallLock(this, InstalledExit, installSide);
		Changed = true;
		return true;
	}

	public bool RemoveLock(ILock theLock)
	{
		_locks.Remove(theLock);
		theLock.Parent.ContainedIn = null;
		theLock.InstallLock(null, null, null);
		Changed = true;
		return true;
	}

	public string InstalledExitDescription(IPerceiver voyeur)
	{
		return Parent.ParseCharacteristics(_prototype.InstalledExitDescription, voyeur, true);
	}

	#endregion

	#region Constructors

	public DoorGameItemComponent(DoorGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		_state = DoorState.Uninstalled;
	}

	public DoorGameItemComponent(DoorGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	protected void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("State");
		if (attr != null)
		{
			State = (DoorState)Convert.ToInt32(attr.Value);
		}
		else
		{
			State = DoorState.Uninstalled;
		}

		var element = root.Element("HingeCell");
		if (element != null)
		{
			HingeCell = Gameworld.Cells.Get(long.Parse(element.Value));
		}

		element = root.Element("OpenDirectionCell");
		if (element != null)
		{
			OpenDirectionCell = Gameworld.Cells.Get(long.Parse(element.Value));
		}

		foreach (var sub in root.Elements("Lock"))
		{
			var newItem = Gameworld.TryGetItem(long.Parse(sub.Value), true);
			if (newItem?.IsItemType<ILock>() == true)
			{
				_locks.Add(newItem.GetItemType<ILock>());
				newItem.Get(null);
				newItem.LoadTimeSetContainedIn(Parent);
			}
		}
	}

	protected override string SaveToXml()
	{
		var definition = new XElement("Definition", new XAttribute("State", (int)State), from theLock in _locks
			select new XElement("Lock", theLock.Parent.Id));

		if (HingeCell != null)
		{
			definition.Add(new XElement("HingeCell", HingeCell.Id));
		}

		if (OpenDirectionCell != null)
		{
			definition.Add(new XElement("OpenDirectionCell", OpenDirectionCell.Id));
		}

		return definition.ToString();
	}

	public DoorGameItemComponent(MudSharp.Models.GameItemComponent component, DoorGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public override void FinaliseLoad()
	{
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}
}

#endregion