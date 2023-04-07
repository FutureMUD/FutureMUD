using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Communication.Language;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class NoAccentGain : Effect, INoAccentGainEffect
{
	public NoAccentGain(IPerceivable owner, params IAccent[] accents)
		: base(owner)
	{
		Accents.AddRange(accents);
	}

	public NoAccentGain(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var root = effect.Element("Effect");
		foreach (var accent in root.Elements("Accent"))
		{
			var theAccent = owner.Gameworld.Accents.Get(long.Parse(accent.Value));
			if (theAccent == null)
			{
				continue;
			}

			Accents.Add(theAccent);
		}
	}

	protected override string SpecificEffectType => "NoAccentGain";

	public List<IAccent> Accents { get; } = new();

	public override string Describe(IPerceiver voyeur)
	{
		return $"Not Gaining Familiarity With {Accents.Select(x => x.Name).ListToString()}.";
	}

	public override bool SavingEffect => true;

	public static void InitialiseEffectType()
	{
		RegisterFactory("NoAccentGain", (effect, owner) => new NoAccentGain(effect, owner));
	}

	public override string ToString()
	{
		return "NoAccentGain Effect";
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			from accent in Accents
			select new XElement("Accent", accent.Id)
		);
	}

	public override bool Applies(object target)
	{
		return base.Applies(target) && Accents.Contains(target as Accent);
	}
}