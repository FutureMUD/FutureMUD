using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class LockingDoorGameItemComponent : GameItemComponent, IDoor, ILock
{
	protected LockingDoorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	#region GameItemComponent Overrides

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LockingDoorGameItemComponentProto)newProto;
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

	public override void FinaliseLoad()
	{
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

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

	public override bool Take(IGameItem item)
	{
		return _locks.RemoveAll(x => x.Parent == item) > 0;
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return Pattern != 0; // Locks that have been set don't stack
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

		sb.AppendLine(
			$"{description}\nIt has a built-in lock that accepts key of type {LockType.ColourName()} and {(IsLocked ? "is currently locked." : "is currently unlocked.").Colour(Telnet.Yellow)}");
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

	#endregion

	#region Constructors

	public LockingDoorGameItemComponent(LockingDoorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public LockingDoorGameItemComponent(Models.GameItemComponent component, LockingDoorGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public LockingDoorGameItemComponent(LockingDoorGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_isLocked = rhs._isLocked;
		_pattern = rhs._pattern;
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

		element = root.Element("IsLocked");
		if (element != null)
		{
			IsLocked = bool.Parse(element.Value);
		}

		element = root.Element("Pattern");
		if (element != null)
		{
			_pattern = int.Parse(element.Value);
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LockingDoorGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		var definition = new XElement("Definition", new XAttribute("State", (int)State), from theLock in _locks
				select new XElement("Lock", theLock.Parent.Id), new XElement("Pattern", Pattern),
			new XElement("IsLocked", IsLocked));

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

	#endregion

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

	private IExit _installedExit;

	public IExit InstalledExit
	{
		get => _installedExit;
		set
		{
			_installedExit = value;
			if (_installedExit == null)
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
			new MixedEmoteOutput(new Emote("@ knock|knocks upon $1", actor, actor, Parent)).Append(playerEmote));
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
		if (!_locks.Contains(theLock))
		{
			return false;
		}

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

	#region IOpenable Members

	public bool IsOpen => State == DoorState.Open;

	public bool CanOpen(IBody body)
	{
		switch (State)
		{
			case DoorState.Uninstalled:
				return false;
			case DoorState.Open:
				return false;
		}

		if (IsLocked || Locks.Any(x => x.IsLocked) ||
		    Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(body?.Actor)))
		{
			return false;
		}

		return true;
	}

	public WhyCannotOpenReason WhyCannotOpen(IBody opener)
	{
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
		return State == DoorState.Open;
	}

	#endregion

	#region ILock Members

	public bool CanBeInstalled => false;

	public void InstallLock(ILockable lockable, IExit exit, ICell installLocation)
	{
		// Do nothing
	}

	public bool SetLocked(bool locked, bool echo)
	{
		if (locked == IsLocked)
		{
			return false;
		}

		IsLocked = locked;
		if (echo)
		{
			Parent.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(IsLocked ? _prototype.LockEmoteNoActor : _prototype.UnlockEmoteNoActor, Parent, Parent),
					flags: OutputFlags.SuppressObscured));
		}

		return true;
	}

	public bool CanUnlock(ICharacter actor, IKey key)
	{
		if (actor?.IsAdministrator() != false)
		{
			return true;
		}

		if (Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies(actor)))
		{
			return false;
		}

		return key?.Unlocks(LockType, Pattern) == true;
	}

	public bool Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanUnlock(actor, key))
		{
			return false;
		}

		IsLocked = false;
		if (actor != null && key != null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_prototype.UnlockEmote, actor, actor, Parent, key?.Parent),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
			             .Single()
			             .Handle(
				             new EmoteOutput(new Emote(_prototype.UnlockEmoteOtherSide, actor, actor, Parent,
					             key?.Parent)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(
					new EmoteOutput(new Emote(_prototype.UnlockEmoteNoActor, Parent, Parent)));
			}
		}

		return true;
	}

	public bool CanLock(ICharacter actor, IKey key)
	{
		if (actor?.IsAdministrator() != false)
		{
			return true;
		}

		return key?.Unlocks(LockType, Pattern) == true;
	}

	public bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanLock(actor, key))
		{
			return false;
		}

		IsLocked = true;
		if (actor != null && key != null)
		{
			actor.OutputHandler.Handle(new MixedEmoteOutput(
				new Emote(_prototype.LockEmote, actor, actor, Parent, key.Parent),
				flags: OutputFlags.SuppressObscured).Append(playerEmote));
			InstalledExit?.Cells.Except(actor.Location)
			             .Single()
			             .Handle(
				             new EmoteOutput(new Emote(_prototype.LockEmoteOtherSide, actor, actor, Parent,
					             key?.Parent)));
		}
		else
		{
			foreach (var cell in Parent.TrueLocations)
			{
				cell.Handle(
					new EmoteOutput(new Emote(_prototype.LockEmoteNoActor, Parent, Parent)));
			}
		}

		return true;
	}

	private bool _isLocked;

	public bool IsLocked
	{
		get { return _isLocked || Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies()); }
		set
		{
			_isLocked = value;
			Changed = true;
		}
	}

	public string LockType => _prototype.LockType;

	private int _pattern;

	public int Pattern
	{
		get => _pattern;
		set
		{
			_pattern = value;
			Changed = true;
		}
	}

	public Difficulty ForceDifficulty => _prototype.ForceDifficulty;
	public Difficulty PickDifficulty => _prototype.PickDifficulty;

	public string Inspect(ICharacter actor, string description)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"You identify the following information about {Parent.HowSeen(actor)}");
		sb.AppendLine("");
		sb.AppendLine($"\tThis door's in-built lock is considered a {LockType.Colour(Telnet.Green)} style lock.");
		sb.AppendLine(Pattern == 0
			? "\tThis door's in-built lock has no combination set."
			: "\tThis door's in-built lock has a combination set so can have keys paired to it.");
		sb.AppendLine($"\tThis door's in-built lock appears to be {PickDifficulty.DescribeColoured()} to pick.");
		sb.AppendLine($"\tThis door's in-built lock appears to be {ForceDifficulty.DescribeColoured()} to force.");
		if (actor.IsAdministrator())
		{
			sb.AppendLine(
				$"  The combination for this door's in-built lock is: {Pattern.ToString("N0", actor).ColourValue()}");
		}

		return sb.ToString();
	}

	#endregion
}