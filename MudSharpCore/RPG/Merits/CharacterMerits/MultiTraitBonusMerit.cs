using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MultiTraitBonusMerit : CharacterMeritBase, ITraitBonusMerit
{
	private readonly HashSet<TraitBonusContext> _contexts = new();

	protected MultiTraitBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		var traits = (from item in definition.Element("Traits")?.Elements() ?? Enumerable.Empty<XElement>()
		              select gameworld.Traits.Get(long.Parse(item.Attribute("id")?.Value ?? "0"))).ToList();
		foreach (var trait in traits)
		{
			SpecificTraits.Add(trait);
		}

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

	public HashSet<ITraitDefinition> SpecificTraits { get; set; }
	public double SpecificBonus { get; set; }

	#region Implementation of ISkillBonusMerit

	public double BonusForTrait(ITraitDefinition trait, TraitBonusContext context)
	{
		return SpecificTraits.Contains(trait) && _contexts.Contains(context) ? SpecificBonus : 0.0;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Multi Trait Bonus",
			(merit, gameworld) => new MultiTraitBonusMerit(merit, gameworld));
	}
}