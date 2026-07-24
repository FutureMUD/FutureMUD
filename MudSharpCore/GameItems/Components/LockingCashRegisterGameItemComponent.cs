using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems.Prototypes;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.GameItems.Components;

public class LockingCashRegisterGameItemComponent : CashRegisterGameItemComponent, ILockable, ILock
{
	private new LockingCashRegisterGameItemComponentProto _prototype;
	private readonly List<ILock> _locks = [];
	private bool _isLocked;
	private int _pattern;

	public LockingCashRegisterGameItemComponent(LockingCashRegisterGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public LockingCashRegisterGameItemComponent(Models.GameItemComponent component,
		LockingCashRegisterGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		var root = XElement.Parse(component.Definition);
		_isLocked = bool.Parse(root.Element("IsLocked")?.Value ?? "false");
		_pattern = int.Parse(root.Element("Pattern")?.Value ?? "0");
		_noSave = true;
		foreach (var item in root.Element("Locks")?.Elements("Lock")
			         .Select(x => Gameworld.TryGetItem(long.Parse(x.Value), true))
			         .Where(x => x?.IsItemType<ILock>() == true) ?? [])
		{
			InstallLock(item!.GetItemType<ILock>());
		}
		_noSave = false;
	}

	private LockingCashRegisterGameItemComponent(LockingCashRegisterGameItemComponent rhs, IGameItem newParent,
		bool temporary) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_isLocked = rhs._isLocked;
		_pattern = rhs._pattern;
	}

	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (LockingCashRegisterGameItemComponentProto)newProto;
		base.UpdateComponentNewPrototype(newProto);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new LockingCashRegisterGameItemComponent(this, newParent, temporary);
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(
			new XElement("Pattern", Pattern),
			new XElement("IsLocked", _isLocked),
			new XElement("Locks", Locks.Select(x => new XElement("Lock", x.Parent.Id)))
		);
		return root.ToString();
	}

	public IEnumerable<ILock> Locks => _locks;

	public bool InstallLock(ILock theLock, ICharacter? actor = null)
	{
		if (_locks.Contains(theLock))
		{
			return false;
		}

		_locks.Add(theLock);
		if (_noSave)
		{
			theLock.Parent.LoadTimeSetContainedIn(Parent);
		}
		else
		{
			theLock.Parent.ContainedIn = Parent;
		}

		Changed = true;
		return true;
	}

	public bool RemoveLock(ILock theLock)
	{
		if (!_locks.Remove(theLock))
		{
			return false;
		}

		theLock.Parent.ContainedIn = null;
		Changed = true;
		return true;
	}

	public bool CanBeInstalled => false;
	public bool IsLocked => _isLocked || Parent.EffectsOfType<IOverrideLockEffect>().Any(x => x.Applies());
	bool ILockable.IsLocked => IsLocked || Locks.Any(x => x.IsLocked);
	public string LockType => _prototype.LockType;
	public Difficulty ForceDifficulty => _prototype.ForceDifficulty;
	public Difficulty PickDifficulty => _prototype.PickDifficulty;

	public int Pattern
	{
		get => _pattern;
		set
		{
			_pattern = value;
			Changed = true;
		}
	}

	public bool CanUnlock(ICharacter actor, IKey key)
	{
		return actor?.IsAdministrator() != false ||
		       (Parent.EffectsOfType<IOverrideLockEffect>().All(x => !x.Applies(actor)) &&
		        key?.Unlocks(LockType, Pattern) == true);
	}

	public bool Unlock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanUnlock(actor, key) || !_isLocked)
		{
			return false;
		}

		_isLocked = false;
		Changed = true;
		EmitLockChange(actor, key, containingPerceivable, playerEmote, false);
		return true;
	}

	public bool CanLock(ICharacter actor, IKey key)
	{
		return !IsOpen && (actor?.IsAdministrator() != false || key?.Unlocks(LockType, Pattern) == true);
	}

	public bool Lock(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote)
	{
		if (!CanLock(actor, key) || _isLocked)
		{
			return false;
		}

		_isLocked = true;
		Changed = true;
		EmitLockChange(actor, key, containingPerceivable, playerEmote, true);
		return true;
	}

	public bool SetLocked(bool locked, bool echo)
	{
		if (_isLocked == locked || locked && IsOpen)
		{
			return false;
		}

		_isLocked = locked;
		Changed = true;
		if (echo)
		{
			Parent.OutputHandler.Handle(new EmoteOutput(
				new Emote(locked ? _prototype.LockEmoteNoActor : _prototype.UnlockEmoteNoActor, Parent, Parent),
				flags: OutputFlags.SuppressObscured));
		}

		HandleItemLockEvent(locked ? EventType.ItemLocked : EventType.ItemUnlocked,
			locked ? EventType.ItemLockedWitness : EventType.ItemUnlockedWitness, null, null, Parent);
		return true;
	}

	private void EmitLockChange(ICharacter actor, IKey key, IPerceivable containingPerceivable, IEmote playerEmote,
		bool locked)
	{
		actor.OutputHandler.Handle(new MixedEmoteOutput(
			new Emote(locked ? _prototype.LockEmote : _prototype.UnlockEmote, actor, actor, Parent, key.Parent),
			flags: OutputFlags.SuppressObscured).Append(playerEmote));
		HandleItemLockEvent(locked ? EventType.ItemLocked : EventType.ItemUnlocked,
			locked ? EventType.ItemLockedWitness : EventType.ItemUnlockedWitness, actor, key.Parent,
			containingPerceivable);
	}

	public void InstallLock(ILockable lockable, IExit exit, ICell installLocation)
	{
	}

	public string Inspect(ICharacter actor, string description)
	{
		return
			$"{description}\nIt has a built-in {LockType.ColourName()} lock which is {PickDifficulty.DescribeColoured()} to pick and {ForceDifficulty.DescribeColoured()} to force.";
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return base.PreventsMerging(component) || Pattern != 0 || Locks.Any();
	}

	public override void Delete()
	{
		base.Delete();
		foreach (var item in Locks.ToList())
		{
			_locks.Remove(item);
			item.Parent.Delete();
		}
	}

	public override void Login()
	{
		base.Login();
		foreach (var item in Locks)
		{
			item.Login();
		}
	}

	public override void Quit()
	{
		base.Quit();
		foreach (var item in Locks)
		{
			item.Quit();
		}
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
		foreach (var item in Locks)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	public override bool Take(IGameItem item)
	{
		if (item.GetItemType<ILock>() is { } theLock && _locks.Contains(theLock))
		{
			return RemoveLock(theLock);
		}

		return base.Take(item);
	}

	public override double ComponentWeight => base.ComponentWeight + Locks.Sum(x => x.Parent.Weight);

	public override double ComponentBuoyancy(double fluidDensity)
	{
		return base.ComponentBuoyancy(fluidDensity) + Locks.Sum(x => x.Parent.Buoyancy(fluidDensity));
	}
}
