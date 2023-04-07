using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SpecificTraitBonusMerit : CharacterMeritBase, ITraitBonusMerit
{
	private readonly HashSet<TraitBonusContext> _contexts = new();

	protected SpecificTraitBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificTrait = gameworld.Traits.Get(long.Parse(definition.Attribute("trait")?.Value ?? "0"));
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0.0");
		var element = definition.Element("Contexts");
		if (element == null)
		{
			foreach (var item in Enum.GetValues(typeof(TraitBonusContext)).OfType<TraitBonusContext>())
			{
				_contexts.Add(item);
			}
		}
		else
		{
			foreach (var item in element.Elements())
			{
				if (int.TryParse(item.Value, out var value))
				{
					if (Enum.IsDefined(typeof(TraitBonusContext), value))
					{
						_contexts.Add((TraitBonusContext)value);
					}
					else
					{
						Console.WriteLine(
							$"Warning: SpecificTraitBonusMerit {Id} had a context of {value}, which is not a valid value.");
					}
				}
				else
				{
					if (Enum.TryParse<TraitBonusContext>(item.Value, out var evalue))
					{
						_contexts.Add(evalue);
					}
					else
					{
						Console.WriteLine(
							$"Warning: SpecificTraitBonusMerit {Id} had a context of {item.Value}, which is not a valid value.");
					}
				}
			}
		}
	}

	public ITraitDefinition SpecificTrait { get; set; }
	public double SpecificBonus { get; set; }

	#region Implementation of ITraitBonusMerit

	public double BonusForTrait(ITraitDefinition trait, TraitBonusContext context)
	{
		return trait == SpecificTrait && _contexts.Contains(context) ? SpecificBonus : 0.0;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Specific Trait Bonus",
			(merit, gameworld) => new SpecificTraitBonusMerit(merit, gameworld));
	}
}