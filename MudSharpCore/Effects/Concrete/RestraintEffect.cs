using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class RestraintEffect : Effect, IRestraintEffect
{
	public IBody BodyOwner { get; set; }
	public IGameItem TargetItem { get; set; }
	public IGameItem RestraintItem { get; set; }

	public RestraintEffect(IBody owner, IEnumerable<ILimb> limbs, IGameItem targetItem, IGameItem restraintItem,
		IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		Limbs = limbs;
		BodyOwner = owner;
		TargetItem = targetItem;
		RestraintItem = restraintItem;
		RegisterEvents();
	}

	private void TargetItem_Invalid(IPerceivable owner)
	{
		Owner.OutputHandler.Handle(new EmoteOutput(new Emote("@ are|is freed from $0.", BodyOwner.Actor, TargetItem)));
		BodyOwner.Take(RestraintItem);
		BodyOwner.Location.Insert(RestraintItem);
		BodyOwner.RemoveEffect(this);
	}

	protected RestraintEffect(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Restrained from using {Limbs.Select(x => x.Name).ListToString()}.";
	}

	protected override string SpecificEffectType => "RestraintEffect";

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		ReleaseEvents();
	}

	protected void ReleaseEvents()
	{
		TargetItem.OnDeath -= TargetItem_Invalid;
		TargetItem.OnQuit -= TargetItem_Invalid;
		TargetItem.OnDeleted -= TargetItem_Invalid;
		TargetItem.OnRemovedFromLocation -= TargetItem_Invalid;
	}

	protected void RegisterEvents()
	{
		if (TargetItem != null)
		{
			TargetItem.OnDeath += TargetItem_Invalid;
			TargetItem.OnQuit += TargetItem_Invalid;
			TargetItem.OnDeleted += TargetItem_Invalid;
			TargetItem.OnRemovedFromLocation += TargetItem_Invalid;
		}
	}

	public override void Login()
	{
		RegisterEvents();
	}

	#endregion

	#region Implementation of ILimbIneffectiveEffect

	private IEnumerable<ILimb> Limbs { get; }

	public bool AppliesToLimb(ILimb limb)
	{
		return Limbs.Contains(limb);
	}

	public LimbIneffectiveReason Reason => LimbIneffectiveReason.Restrained;

	#endregion
}