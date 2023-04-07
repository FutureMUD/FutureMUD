using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Effects.Concrete;

public class AntisepticProtection : Effect, IAntisepticTreatmentEffect
{
	protected AntisepticProtection(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		Bodypart = Gameworld.BodypartPrototypes.Get(long.Parse(effect.Element("Bodypart").Value));
	}

	public AntisepticProtection(IPerceivable owner, IBodypart bodypart, IFutureProg applicabilityProg)
		: base(owner, applicabilityProg)
	{
		Bodypart = bodypart;
	}

	protected override string SpecificEffectType => "AntisepticProtection";

	public IBodypart Bodypart { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"{Owner.HowSeen(voyeur, true)} has antiseptically treated their {Bodypart.FullDescription()}.";
	}

	public override bool SavingEffect => true;

	public override void RemovalEffect()
	{
		if (Owner is IBody body && body.Wounds.All(x => x.Bodypart != Bodypart))
		{
			return;
		}

		Owner.OutputHandler.Send(
			$"You feel as if your {Bodypart.FullDescription()} could benefit from further topical antiseptic treatment.");
	}

	protected override XElement SaveDefinition()
	{
		return new XElement("Bodypart", Bodypart.Id);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("AntisepticProtection", (effect, owner) => new AntisepticProtection(effect, owner));
	}

	public override bool Applies(object target)
	{
		return Bodypart == target && Applies();
	}
}