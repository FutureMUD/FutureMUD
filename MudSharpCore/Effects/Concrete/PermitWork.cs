using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Property;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;
#nullable enable
public class PermitWork : Effect, IEffect
{
	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("PermitWork", (effect, owner) => new PermitWork(effect, owner));
	}

	#endregion

	public IProperty? Property { get; init; }
	public ICell? Cell { get; init; }

	#region Constructors

	public PermitWork(IPerceivable owner) : base(owner, null)
	{
	}

	protected PermitWork(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		Property = Gameworld.Properties.Get(long.Parse(root.Element("Property")!.Value));
		Cell = Gameworld.Cells.Get(long.Parse(root.Element("Cell")!.Value));
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "PermitWork";

	public override string Describe(IPerceiver voyeur)
	{
		return Property is null
			? $"Permitted to work in the {Cell!.HowSeen(voyeur)} location"
			: $"Permitted to work on the {Property.Name.ColourName()} property";
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect",
			new XElement("Property", Property?.Id ?? 0),
			new XElement("Cell", Cell?.Id ?? 0)
		);
	}

	public override void RemovalEffect()
	{
		((ICharacter)Owner).OutputHandler.Send(
			$"You are no longer legally permitted to work on {(Property is not null ? $"the property {Property.Name.ColourName()}" : $"the location {Cell!.HowSeen((ICharacter)Owner)}")}");
	}

	#endregion
}