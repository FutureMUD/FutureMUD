using System;

namespace MudSharp.Framework.Save;

public abstract class LateInitialisingItem : FrameworkItem, ILateInitialisingItem
{
	protected bool IdInitialised;
	public virtual bool IdHasBeenRegistered => IdInitialised;

	public override long Id
	{
		get
		{
			if (!IdHasBeenRegistered)
			{
				Gameworld?.SaveManager.Flush();
			}

			return _id;
		}
		set => _id = value;
	}

	#region IHaveFuturemud Members

	public virtual IFuturemud Gameworld { get; protected set; }

	#endregion

	#region ISaveable Members

	protected bool _noSave = false;
	private bool _changed;

	public bool Changed
	{
		get => _changed;
		set
		{
			if (value)
			{
				if (!_changed && !_noSave && Gameworld != null)
				{
					Gameworld.SaveManager.Add(this);
					_changed = true;
				}
			}
			else
			{
				_changed = false;
			}
		}
	}

	public abstract void Save();

	#endregion

	#region ILateInitialisingItem Implementation

	public event LateInitialisingItemDelegate IdRegistered;

	public Action InitialiseItem()
	{
		var obj = DatabaseInsert();
		if (obj != null)
		{
			IdInitialised = true;
		}

		return () =>
		{
			if (IdInitialised)
			{
				SetIDFromDatabase(obj);
				IdRegistered?.Invoke(this);
			}
		};
	}

	public abstract object DatabaseInsert();

	public abstract void SetIDFromDatabase(object dbitem);

	public virtual InitialisationPhase InitialisationPhase => InitialisationPhase.Second;

	public void SetNoSave(bool value)
	{
		_noSave = value;
	}

	public bool GetNoSave()
	{
		return _noSave;
	}

	#endregion
}

public abstract class LateKeywordedInitialisingItem : KeywordedItem, ILateInitialisingItem
{
	protected bool IdInitialised;

	public virtual bool IdHasBeenRegistered => IdInitialised;

	public override long Id
	{
		get
		{
			if (!IdHasBeenRegistered)
			{
				Gameworld.SaveManager.Flush();
			}

			return _id;
		}
		set => _id = value;
	}

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	#region ISaveable Members

	protected bool _noSave = false;
	private bool _changed;

	public bool Changed
	{
		get => _changed;
		set
		{
			if (value)
			{
				if (!_changed && !_noSave && Gameworld != null)
				{
					Gameworld.SaveManager.Add(this);
					_changed = true;
				}
			}
			else
			{
				_changed = false;
			}
		}
	}

	public abstract void Save();

	#endregion

	#region ILateInitialisingItem Implementation

	public event LateInitialisingItemDelegate IdRegistered;

	public void SetNoSave(bool value)
	{
		_noSave = value;
	}

	public bool GetNoSave()
	{
		return _noSave;
	}

	public Action InitialiseItem()
	{
		var obj = DatabaseInsert();
		if (obj != null)
		{
			IdInitialised = true;
		}

		return () =>
		{
			if (IdInitialised)
			{
				SetIDFromDatabase(obj);
				IdRegistered?.Invoke(this);
			}
		};
	}

	public abstract object DatabaseInsert();

	public abstract void SetIDFromDatabase(object dbitem);

	public virtual InitialisationPhase InitialisationPhase => InitialisationPhase.Second;

	#endregion
}