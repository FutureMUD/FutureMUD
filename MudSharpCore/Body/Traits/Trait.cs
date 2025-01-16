using System;
using System.Collections.Generic;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits;

public abstract class Trait : FrameworkItem, ITrait
{
	#region Overrides of FrameworkItem

	public override string FrameworkItemType => "Trait";

	#endregion

	protected bool _changed;

	protected bool _nonSaving = false;
	protected IHaveTraits _owner;

	protected double _value;

	protected Trait(MudSharp.Models.Trait trait, IHaveTraits owner)
	{
		_value = trait.Value;
		_owner = owner;
	}

	protected Trait()
	{
	}

	public abstract ITraitDefinition Definition { get; }
	public IHaveTraits Owner => _owner;

	public virtual double Value
	{
		get => _value;
		set
		{
			if (value > MaxValue)
			{
				value = Math.Max(_value, MaxValue);
			}

			var oldVal = _value;
			_value = value;
			if (_value != oldVal)
			{
				TraitChanged(oldVal, _value);
			}
		}
	}

	public double RawValue => _value;

	public bool Hidden => Definition.Hidden;

	public virtual bool TraitUsed(IHaveTraits user, Outcome result, Difficulty difficulty, TraitUseType usetype, IEnumerable<Tuple<string, double>> bonuses)
	{
		return false;
	}

	public event EventHandler<TraitChangedEventArgs> TraitValueChanged;


	public virtual void Initialise(IHaveTraits owner)
	{
		_owner = owner;
	}

	public virtual bool Changed
	{
		get => _changed;
		set
		{
			if (value && !_changed && !_nonSaving)
			{
				Gameworld.SaveManager.Add(this);
			}

			_changed = value;
		}
	}

	public virtual void Save()
	{
		if (Owner == null || Definition == null)
		{
			Changed = false;
			return;
		}

		var dbtrait = FMDB.Context.Traits.Find(_owner.Id, Definition.Id);
		if (dbtrait is null)
		{
			dbtrait = new Models.Trait
			{
				BodyId = Owner.Id,
				TraitDefinitionId = Definition.Id,
				Value = 0.0,
				AdditionalValue = 0.0
			};
			FMDB.Context.Traits.Add(dbtrait);
		}
		dbtrait.Value = Value;
		Changed = false;
	}

	public IFuturemud Gameworld => Definition.Gameworld;

	public virtual double MaxValue => Definition.MaxValue;

	protected void TraitChanged(double oldval, double newval)
	{
		TraitValueChanged?.Invoke(this, new TraitChangedEventArgs(this, oldval, newval));
		Changed = true;
	}
}