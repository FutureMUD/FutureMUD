using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Law;
using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace MudSharp.Effects.Concrete;

public class ConsideringPlea : Effect
{
	public ILegalAuthority LegalAuthority { get; set; }
	public ICrime Crime { get; set; }

	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("ConsideringPlea", (effect, owner) => new ConsideringPlea(effect, owner));
	}

	#endregion

	#region Constructors

	public ConsideringPlea(ICharacter owner, ILegalAuthority legalAuthority, ICrime crime) : base(owner, null)
	{
		LegalAuthority = legalAuthority;
		Crime = crime;
	}

	protected ConsideringPlea(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		LegalAuthority = Gameworld.LegalAuthorities.Get(long.Parse(root.Element("LegalAuthority").Value));
		Crime = Gameworld.Crimes.Get(long.Parse(root.Element("Crime").Value));
	}

	#endregion


	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("LegalAuthority", LegalAuthority.Id),
			new XElement("Crime", Crime.Id)
		);
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "ConsideringPlea";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Considering their plea in the {LegalAuthority.Name.ColourName()} authority for the crime of {Crime.DescribeCrime(voyeur)}.";
	}

	public override bool SavingEffect => true;
	#endregion
}