using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class AdminSpyMaster : Effect, IEffectSubtype
{
	public List<ICell> SpiedCells { get; } = new();
	public List<AdminSpy> SpyEffects { get; } = new();
	public ICharacter CharacterOwner { get; }

	public AdminSpyMaster(ICharacter owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		CharacterOwner = owner;
		CharacterOwner.OnQuit += CharacterOwner_OnQuit;
	}

	private void CharacterOwner_OnQuit(IPerceivable owner)
	{
		foreach (var effect in SpyEffects)
		{
			effect.Owner.RemoveEffect(effect);
		}

		SpyEffects.Clear();
		CharacterOwner.OnQuit -= CharacterOwner_OnQuit;
	}

	public AdminSpyMaster(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		CharacterOwner = (ICharacter)owner;
		CharacterOwner.OnQuit += CharacterOwner_OnQuit;
		foreach (var spy in effect.Element("Effect").Elements("Spy"))
		{
			var cell = Gameworld.Cells.Get(long.Parse(spy.Value));
			if (cell != null)
			{
				SpiedCells.Add(cell);
				var childEffect = new AdminSpy(cell, CharacterOwner);
				cell.AddEffect(childEffect);
				SpyEffects.Add(childEffect);
			}
		}
	}

	public void RemoveSpiedCell(ICell cell)
	{
		SpiedCells.Remove(cell);
		cell.RemoveEffect(SpyEffects.FirstOrDefault(x => x.Owner == cell), true);
		if (!SpiedCells.Any())
		{
			CharacterOwner.RemoveEffect(this);
		}

		Changed = true;
	}

	public void AddSpiedCell(ICell cell)
	{
		if (SpiedCells.Contains(cell))
		{
			return;
		}

		SpiedCells.Add(cell);
		var effect = new AdminSpy(cell, CharacterOwner);
		cell.AddEffect(effect);
		SpyEffects.Add(effect);
		Changed = true;
	}

	#region Overrides of Effect

	public override bool SavingEffect => true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("AdminSpyMaster", (effect, owner) => new AdminSpyMaster(effect, owner));
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			from spy in SpiedCells
			select new XElement("Spy", spy.Id)
		);
	}

	public override string Describe(IPerceiver voyeur)
	{
		return $"Spying on {SpyEffects.Count} locations";
	}

	protected override string SpecificEffectType => "AdminSpyMaster";

	public override void Login()
	{
		CharacterOwner.OnQuit += CharacterOwner_OnQuit;
		foreach (var cell in SpiedCells)
		{
			var child = new AdminSpy(cell, CharacterOwner);
			cell.AddEffect(child);
			SpyEffects.Add(child);
		}
	}

	#endregion
}