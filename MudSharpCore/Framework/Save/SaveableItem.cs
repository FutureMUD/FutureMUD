using System.Linq;
using MudSharp.Framework;
using MudSharp.Character;

namespace MudSharp.Framework.Save;

#nullable enable
public abstract class SaveableItem : FrameworkItem, ISaveable
{
	public override long Id
	{
		get
		{
			// If a SaveableItem is asked for its ID, it may not have saved yet - save first
			if (_id == 0 && !_noSave)
			{
				Gameworld.SaveManager.Flush();
			}

			return _id;
		}
		set => _id = value;
	}

	#region IHaveFuturemud Members

	private IFuturemud? _gameworld;

	public IFuturemud Gameworld
	{
		get => _gameworld ?? Futuremud.Games.First();
		protected init { _gameworld = value; }
	}

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
}

public abstract class SavableKeywordedItem : KeywordedItem, ISaveable
{
	public override long Id
	{
		get
		{
			// If a SavableKeywordedItem is asked for its ID, it may not have saved yet - save first
			if (_id == 0 && !_noSave)
			{
				Gameworld.SaveManager.Flush();
			}

			return _id;
		}
		set => _id = value;
	}

	#region IHaveFuturemud Members

	private IFuturemud? _gameworld;

	public IFuturemud Gameworld
	{
		get => _gameworld ?? Futuremud.Games.First();
		protected set { _gameworld = value; }
	}

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
				if (!_changed && !_noSave)
				{
					Gameworld?.SaveManager.Add(this);
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
}