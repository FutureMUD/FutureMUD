using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp.Effects;

public abstract partial class Effect : FrameworkItem, IEffect
{
	private static readonly Dictionary<string, Func<XElement, IPerceivable, IEffect>> EffectFactories =
		new();

	public IFuturemud Gameworld { get; }

	protected Effect(IPerceivable owner, IFutureProg applicabilityProg = null)
	{
		Owner = owner;
		ApplicabilityProg = applicabilityProg;
		Gameworld = Owner.Gameworld;
	}

	protected Effect(XElement root, IPerceivable owner)
	{
		Owner = owner;
		Gameworld = owner.Gameworld;
		ApplicabilityProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ApplicabilityProg").Value));
	}

	public XElement SaveToXml(Dictionary<IEffect, TimeSpan> scheduledEffects)
	{
		var scheduled = scheduledEffects.ContainsKey(this);
		return new XElement("Effect",
			new XElement("ApplicabilityProg", ApplicabilityProg?.Id ?? 0),
			new XElement("Type", SpecificEffectType),
			new XElement("Original", scheduled ? (long)scheduledEffects[this].TotalMilliseconds : 0),
			new XElement("Remaining", scheduled ? (long)scheduledEffects[this].TotalMilliseconds : 0),
			SaveDefinition()
		);
	}

	public IFutureProg ApplicabilityProg { get; set; }

	public override string FrameworkItemType => SpecificEffectType;
	public bool LoadErrors { get; protected set; } = false;

	/// <summary>
	///     Calls the " void InitialiseEffectType ()" method on any sub-classes of Effect if they have one, giving them an
	///     opportunity to do boot-up time tasks.
	///     Is called inside an FMDB Context, so can make context calls
	/// </summary>
	public static void InitialiseEffects()
	{
		foreach (var type in Futuremud.GetAllTypes().Where(x => x.IsSubclassOf(typeof(Effect))))
		{
			var method = type.GetMethod("InitialiseEffectType", BindingFlags.Public | BindingFlags.Static);
			method?.Invoke(null, null);
		}
	}

	protected static void RegisterFactory(string effectName, Func<XElement, IPerceivable, IEffect> creationFunc)
	{
		EffectFactories[effectName] = creationFunc;
	}

	public static IEffect LoadEffect(XElement effect, IPerceivable owner)
	{
		try
		{
			return EffectFactories[effect.Element("Type").Value](effect, owner);
		}
		catch (KeyNotFoundException)
		{
			throw new ApplicationException(
				$"Unknown effect type {effect.Element("Type").Value}");
		}
	}

	public override string ToString()
	{
		return Owner is IPerceiver ownerPerceiver ? Describe(ownerPerceiver) : $"Effect type {SpecificEffectType}.";
	}

	#region IEffect Members

	public IPerceivable Owner { get; protected set; }

	public abstract string Describe(IPerceiver voyeur);

	public virtual bool Applies()
	{
		return ApplicabilityProg?.ExecuteBool(Owner, null, null) ?? true;
	}

	public virtual bool Applies(object target)
	{
		return ApplicabilityProg?.ExecuteBool(Owner, target as IPerceivable, null) ?? true;
	}

	public virtual bool Applies(object target, object thirdparty)
	{
		return ApplicabilityProg?.ExecuteBool(Owner, target as IPerceivable, thirdparty as IPerceivable) ?? true;
	}

	public virtual bool Applies(object target, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		return ApplicabilityProg?.ExecuteBool(Owner, target as IPerceivable, flags) ?? true;
	}

	public virtual bool IsEffectType<T>() where T : class, IEffect
	{
		return this is T && Applies();
	}

	public virtual bool IsEffectType<T>(object target) where T : class, IEffect
	{
		return this is T && Applies(target);
	}

	public virtual bool IsEffectType<T>(object target, object thirdparty) where T : class, IEffect
	{
		return this is T && Applies(target, thirdparty);
	}

	public virtual bool IsBlockingEffect(string blockingType)
	{
		return Blocks.Contains(blockingType);
	}

	public virtual string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return string.Empty;
	}

	public virtual bool CanBeStoppedByPlayer => false;

	public virtual IEnumerable<string> Blocks => Enumerable.Empty<string>();

	public virtual bool SavingEffect => false;

	protected virtual XElement SaveDefinition()
	{
		return new XElement("Blank");
	}

	protected abstract string SpecificEffectType { get; }

	public virtual void ExpireEffect()
	{
		Owner.RemoveEffect(this, true);
	}

	public virtual void RemovalEffect()
	{
		// Do nothing
	}

	public virtual void CancelEffect()
	{
		RemovalEffect();
	}

	public virtual void InitialEffect()
	{
		// Do nothing
	}

	public virtual void Login()
	{
		// Do nothing
	}

	public virtual IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		return null;
	}

	public virtual bool PreventsItemFromMerging(IGameItem effectOwnerItem, IGameItem targetItem)
	{
		return false;
	}

	public virtual PerceptionTypes PerceptionDenying => PerceptionTypes.None;

	public virtual PerceptionTypes Obscuring => PerceptionTypes.None;

	public T GetSubtype<T>() where T : class, IEffect
	{
		return this as T;
	}

	public bool Changed
	{
		get => Owner.EffectsChanged;
		set => Owner.EffectsChanged = value;
	}

	#endregion
}