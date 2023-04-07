using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class StorytellerDescOverride : Effect, IOverrideDescEffect
{
	public StorytellerDescOverride(IPerceivable owner, DescriptionType type, string description,
		IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
	{
		OverridenType = type;
		OverridenDescription = description;
	}

	protected StorytellerDescOverride(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		OverridenDescription = root.Element("OverridenDescription").Value;
		OverridenType = (DescriptionType)int.Parse(root.Element("OverridenType").Value);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("StorytellerDescOverride", (effect, owner) => new StorytellerDescOverride(effect, owner));
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("OverridenDescription", new XCData(OverridenDescription)),
			new XElement("OverridenType", (int)OverridenType)
		);
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Their {OverridenType} description is overriden to {OverridenDescription}.";
	}

	protected override string SpecificEffectType => "StorytellerDescOverride";

	#endregion

	#region Implementation of IOverrideDescEffect

	public string OverridenDescription { get; set; }

	public DescriptionType OverridenType { get; set; }

	public bool OverrideApplies(IPerceiver voyeur, DescriptionType type)
	{
		return type == OverridenType;
	}

	public string Description(DescriptionType type, bool colour)
	{
		if (Owner.Sentient)
		{
			return colour ? OverridenDescription.ColourCharacter() : OverridenDescription;
		}

		return colour ? OverridenDescription.ColourObject() : OverridenDescription;
	}

	#endregion
}