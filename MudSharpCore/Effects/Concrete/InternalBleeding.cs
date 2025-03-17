using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Units;

namespace MudSharp.Effects.Concrete;

public class InternalBleeding : Effect, IInternalBleedingEffect, IPertainToBodypartEffect
{
	private double _bloodlossPerTick;

	private double _bloodlossTotal;

	public IBody BodyOwner { get; set; }

	public InternalBleeding(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		BodyOwner = (IBody)owner;
		LoadFromXML(effect.Element("Effect"));
	}

	public InternalBleeding(IBody owner, IOrganProto organ, double initialRate) : base(owner)
	{
		Organ = organ;
		BloodlossPerTick = initialRate;
		BodyOwner = owner;
	}

	protected override string SpecificEffectType => "Internal Bleeding";

	public double BloodlossPerTick
	{
		get => _bloodlossPerTick;
		set
		{
			_bloodlossPerTick = value;
			if (_bloodlossPerTick < 0)
			{
				_bloodlossPerTick = 0.0;
			}

			Changed = true;
		}
	}

	public double BloodlossTotal
	{
		get => _bloodlossTotal;
		set
		{
			var oldOrganFunction = Organ?.OrganFunctionFactor(BodyOwner);
			_bloodlossTotal = value;
			Changed = true;
			var newOrganFunction = Organ?.OrganFunctionFactor(BodyOwner);
		}
	}

	public IOrganProto Organ { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return
			$"{Owner.HowSeen(voyeur, true)} is bleeding from {Owner.ApparentGender(voyeur).Possessive()} {Organ?.ShortDescription() ?? "insides"} at a rate of {Gameworld.UnitManager.Describe(BloodlossPerTick, UnitType.FluidVolume, voyeur).Colour(Telnet.Green)} per tick, having lost {Gameworld.UnitManager.Describe(BloodlossTotal, UnitType.FluidVolume, voyeur).Colour(Telnet.Green)} so far.";
	}

	public override bool SavingEffect { get; } = true;

	private void LoadFromXML(XElement root)
	{
		Organ = ((IBody)Owner).Prototype.Organs.FirstOrDefault(x => x.Id == long.Parse(root.Element("Organ").Value));
		BloodlossPerTick = double.Parse(root.Element("BloodlossPerTick").Value);
		_bloodlossTotal = double.Parse(root.Element("BloodlossTotal").Value);
	}

	protected override XElement SaveDefinition()
	{
		return
			new XElement("Effect", new XElement("Organ", Organ?.Id ?? 0),
				new XElement("BloodlossPerTick", BloodlossPerTick), new XElement("BloodlossTotal", BloodlossTotal));
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("Internal Bleeding", (effect, owner) => new InternalBleeding(effect, owner));
	}

	#region Implementation of IPertainToBodypartEffect

	public IBodypart Bodypart => Organ;

	#endregion
}