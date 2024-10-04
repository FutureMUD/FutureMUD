using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.GameItems;

public abstract class GameItemComponent : LateInitialisingItem, IGameItemComponent
{
	private MudSharp.Models.GameItem _parentDBItem;

	protected GameItemComponent(GameItemComponent rhs, IGameItem newParent, bool temporary = false)
	{
		Parent = newParent;
		_noSave = temporary;
		if (!temporary)
		{
			Gameworld.SaveManager.AddInitialisation(this);
		}
	}

	protected GameItemComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		Parent = parent;
		_id = component.Id;
		IdInitialised = true;
	}

	protected GameItemComponent(IGameItem parent, IGameItemComponentProto prototype, bool temporary = false)
	{
		Parent = parent;
		_noSave = temporary;
		if (!temporary)
		{
			Gameworld.SaveManager.AddInitialisation(this);
		}
	}

	#region Overrides of LateInitialisingItem

	public override IFuturemud Gameworld
	{
		get => Parent.Gameworld;
		protected set { }
	}

	/// <inheritdoc />
	public override string Name => Prototype.Name;

	#endregion

	public void PrimeComponentForInsertion(MudSharp.Models.GameItem parent)
	{
		_parentDBItem = parent;
	}

	public override string FrameworkItemType => "GameItemComponent";

	public abstract IGameItemComponent Copy(IGameItem newParent, bool temporary = false);

	public virtual bool PreventsMerging(IGameItemComponent component)
	{
		return false;
	}

	public virtual bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier)
	{
		return true;
	}

	public virtual void Delete()
	{
		_noSave = true;
		Changed = false;
		Gameworld.SaveManager.Abort(this);
	}

	public virtual void Quit()
	{
		// Do nothing
	}

	public virtual void Login()
	{
		// Do nothing
	}

	public bool CheckPrototypeForUpdate()
	{
		if (Prototype.Status == RevisionStatus.Obsolete || Prototype.Status == RevisionStatus.Revised)
		{
			var newProto =
				Gameworld.ItemComponentProtos.FirstOrDefault(
					x => x.Id == Prototype.Id && x.Status == RevisionStatus.Current);
			if (newProto == null)
			{
				return false;
			}

			var dbcomp = new MudSharp.Models.GameItemComponent
			{
				Id = Id,
				GameItemComponentProtoRevision = newProto.RevisionNumber,
				Definition = SaveToXml(),
				GameItemId = Parent.Id,
				GameItemComponentProtoId = Prototype.Id
			};
			if (FMDB.Context.Entry(dbcomp).State == EntityState.Detached)
			{
				FMDB.Context.GameItemComponents.Attach(dbcomp);
			}

			var entry = FMDB.Context.Entry(dbcomp);
			entry.State = EntityState.Modified;
			entry.Property(x => x.GameItemComponentProtoRevision).IsModified = true;
			UpdateComponentNewPrototype(newProto);
			return true;
		}

		return false;
	}

	public virtual bool Die(IGameItem newItem, ICell location)
	{
		return false;
	}

	public virtual bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
	{
		return false;
	}

	public virtual bool Take(IGameItem item)
	{
		return false;
	}

	public virtual bool PreventsMovement()
	{
		return false;
	}

	public virtual void FinaliseLoad()
	{
		// Do nothing
	}

	public virtual string WhyPreventsMovement(ICharacter mover)
	{
		throw new ApplicationException(
			"WhyPreventsMovement called on GameItemComponent that did not implement an override for it.");
	}

	public virtual void ForceMove()
	{
		// Do nothing
	}

	/// <summary>
	///     Indicates whether anything in the current status of this component prevents the item from being repositioned
	/// </summary>
	/// <returns></returns>
	public virtual bool PreventsRepositioning()
	{
		return false;
	}

	/// <summary>
	///     Returns an error message concerning why this component cannot be repositioned
	/// </summary>
	/// <returns></returns>
	public virtual string WhyPreventsRepositioning()
	{
		throw new NotSupportedException();
	}

	public virtual double ComponentWeight => 0;
	public virtual double ComponentWeightMultiplier => 1.0;

	public virtual double ComponentBuoyancy(double fluidDensity)
	{
		return 0.0;
	}

	public virtual IBody HaveABody => null;

	public virtual bool OverridesMaterial => false;

	public virtual ISolid OverridenMaterial => null;

	#region ISaveable Members

	public override void Save()
	{
		if (_noSave)
		{
			return;
		}

		var dbcomp = FMDB.Context.GameItemComponents.Find(Id);
		if (dbcomp != null)
		{
			dbcomp.Definition = SaveToXml();
		}

		Changed = false;
	}

	#endregion

	public override object DatabaseInsert()
	{
		var dbitem = new Models.GameItemComponent
		{
			Definition = SaveToXml(),
			GameItem = _parentDBItem ?? FMDB.Context.GameItems.Find(Parent.Id),
			GameItemComponentProtoId = Prototype.Id,
			GameItemComponentProtoRevision = Prototype.RevisionNumber
		};
		FMDB.Context.GameItemComponents.Add(dbitem);
		_parentDBItem = null;

		return dbitem;
	}

	public override void SetIDFromDatabase(object dbitem)
	{
		var item = (MudSharp.Models.GameItemComponent)dbitem;
		_id = item.Id;
	}

	protected abstract void UpdateComponentNewPrototype(IGameItemComponentProto newProto);
	protected abstract string SaveToXml();

	#region IGameItemComponent Members

	/// <summary>
	/// Whether or not we should warn the purger before purging this item
	/// </summary>
	public virtual bool WarnBeforePurge => Prototype.WarnBeforePurge;

	public IGameItem Parent { get; protected set; }

	public virtual bool DesignedForOffhandUse => false;

	public abstract IGameItemComponentProto Prototype { get; }

	public virtual bool WrapFullDescription { get; } = true;

	public virtual bool AffectsLocationOnDestruction => false;

	public virtual int ComponentDieOrder => 0;

	public virtual void Taken()
	{
		// Do nothing
	}

	public virtual bool ExposeToLiquid(LiquidMixture mixture)
	{
		return false;
	}

	#endregion

	#region Decoration Properties

	public virtual bool DescriptionDecorator(DescriptionType type)
	{
		return false;
	}

	/// <summary>
	///     Items with multiple decorating components are sorted in DecorationPriority order. Positive values of
	///     DecorationPriority are decorated BEFORE any colour is applied, Negative values are decorated AFTER colour is
	///     applied in descending order
	/// </summary>
	public virtual int DecorationPriority => 0;

	public virtual string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return description;
	}

	public event EventHandler DescriptionUpdate;

	protected void HandleDescriptionUpdate()
	{
		DescriptionUpdate?.Invoke(this, new EventArgs());
	}

	#endregion

	#region IHandleEvents Members

	public virtual bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		return false;
	}

	public virtual bool HandlesEvent(params EventType[] types)
	{
		return false;
	}

	public bool InstallHook(IHook hook)
	{
		return false;
	}

	public bool RemoveHook(IHook hook)
	{
		return false;
	}

	public bool HooksChanged
	{
		get => false;
		set { }
	}

	public IEnumerable<IHook> Hooks => Enumerable.Empty<IHook>();

	#endregion
}