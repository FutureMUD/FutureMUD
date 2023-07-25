using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

#nullable enable
public class OverrideSDescFromProg : Effect, IEffect, IOverrideDescEffect
{
	public string ModifiedDescription { get; }
	public string Tag { get; }
	public IPerceiver? FixedPerceiver
	{
		get
		{
			if (_fixedPerceiver is not null)
			{
				return _fixedPerceiver;
			}

			if (_fixedPerceiverId is null)
			{
				return null;
			}

			_fixedPerceiver = Gameworld.GetPerceivable(_fixedPerceiverType, _fixedPerceiverId.Value) as IPerceiver;
			return _fixedPerceiver;
		}
	}
	private IPerceiver? _fixedPerceiver;
	private readonly long? _fixedPerceiverId;
	private readonly string? _fixedPerceiverType;

	public string Description(DescriptionType type, bool colour)
	{
		return colour ? ModifiedDescription.SubstituteANSIColour() : ModifiedDescription;
	}

	public bool OverrideApplies(IPerceiver voyeur, DescriptionType type)
	{
		return type == DescriptionType.Short && (FixedPerceiver is null || FixedPerceiver.IsSelf(voyeur)) && Applies(Owner, voyeur);
	}

	#region Static Initialisation
	public static void InitialiseEffectType()
	{
		RegisterFactory("OverrideSDescFromProg", (effect, owner) => new OverrideSDescFromProg(effect, owner));
	}
	#endregion

	#region Constructors
	public OverrideSDescFromProg(IPerceivable owner, string modifiedDescription, string tag, IPerceiver? fixedPerceiver,  IFutureProg? applicabilityProg = null) : base(owner, applicabilityProg)
	{
		ModifiedDescription = modifiedDescription;
		Tag = tag;
		_fixedPerceiverId = fixedPerceiver?.Id;
		_fixedPerceiverType = fixedPerceiver?.FrameworkItemType;
		_fixedPerceiver = fixedPerceiver;
	}

	protected OverrideSDescFromProg(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		ModifiedDescription = root!.Element("ModifiedDescription")!.Value;
		Tag = root!.Element("Tag").Value;
		var fpe = root.Element("FixedPerceiver");
		var id = long.Parse(fpe!.Attribute("id")!.Value);
		_fixedPerceiverId = id != 0 ? id : null;
		_fixedPerceiverType = id != 0 ? fpe.Attribute("type")!.Value : null;
	}
	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
	#region Saving and Loading
	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("FixedPerceiver", new XAttribute("id", _fixedPerceiverId ?? 0), new XAttribute("type", _fixedPerceiverType ?? "none")),
			new XElement("ModifiedDescription", new XCData(ModifiedDescription)),
			new XElement("Tag", new XCData(Tag))
		);
	}
	#endregion

	#region Overrides of Effect
	protected override string SpecificEffectType => "OverrideSDescFromProg";

	public override string Describe(IPerceiver voyeur)
	{
		return $"{$"[{Tag ?? ""}]".ColourName()} Overriding SDesc to {ModifiedDescription.SubstituteANSIColour()}{(FixedPerceiver is not null ? $" for {FixedPerceiver.HowSeen(voyeur)}" : "")}{(ApplicabilityProg is not null ? $" when {ApplicabilityProg.MXPClickableFunctionName()} is true" : "")}.";
	}

	public override bool SavingEffect => true;
	#endregion
}
