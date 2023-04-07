using System.Collections.Generic;
using System.Linq;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp.Effects.Concrete;

public class FixedDescription : Effect, IOverrideDescEffect
{
	public string SDesc { get; set; }
	public string FDesc { get; set; }
	public List<IPerceiver> Perceivers { get; } = new();

	public FixedDescription(IPerceiver owner, IEnumerable<IPerceiver> perceivers, string sdesc, string fdesc) :
		base(owner)
	{
		SDesc = sdesc;
		FDesc = fdesc;
		Perceivers.AddRange(perceivers);
	}

	protected override string SpecificEffectType => "FixedDescription";

	public override string Describe(IPerceiver voyeur)
	{
		return "Displaying a fixed description to certain perceivers.";
	}

	public string Description(DescriptionType type, bool colour)
	{
		if (type == DescriptionType.Short)
		{
			return colour ? SDesc : SDesc.StripANSIColour();
		}

		return colour ? FDesc : FDesc.StripANSIColour();
	}

	public bool OverrideApplies(IPerceiver voyeur, DescriptionType type)
	{
		return (type == DescriptionType.Short || type == DescriptionType.Full) && Perceivers.Any(x => x.IsSelf(voyeur));
	}

	public override IEffect NewEffectOnItemMorph(IGameItem oldItem, IGameItem newItem)
	{
		if (oldItem == Owner)
		{
			return new FixedDescription(newItem, Perceivers, SDesc, FDesc);
		}

		return null;
	}
}