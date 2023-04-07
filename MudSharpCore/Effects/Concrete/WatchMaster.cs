using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Concrete;

public class WatchMaster : Effect, IEffectSubtype, ICheckBonusEffect
{
	public List<ICell> SpiedCells { get; } = new();
	public List<Watch> WatchEffects { get; } = new();
	public ICharacter CharacterOwner { get; }

	public WatchMaster(ICharacter owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		CharacterOwner = owner;
	}

	protected override string SpecificEffectType => "WatchMaster";

	public void RemoveSpiedCell(ICell cell)
	{
		SpiedCells.Remove(cell);
		cell.RemoveEffect(WatchEffects.First(x => x.Owner == cell), true);
		if (!SpiedCells.Any())
		{
			CharacterOwner.RemoveEffect(this);
		}
	}

	public void AddSpiedCell(ICell cell)
	{
		if (SpiedCells.Contains(cell))
		{
			return;
		}

		SpiedCells.Add(cell);
		var effect = new Watch(cell, CharacterOwner);
		cell.AddEffect(effect);
		WatchEffects.Add(effect);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Watching {WatchEffects.Count} locations";
	}

	public bool AppliesToCheck(CheckType type)
	{
		return (type.IsPerceptionCheck() && type != CheckType.WatchLocation) || type.IsGeneralActivityCheck() ||
		       type.IsTargettedFriendlyCheck() || type.IsTargettedHostileCheck();
	}

	public double CheckBonus => Gameworld.GetStaticDouble("WatchLocationPerceptionBonus");
}