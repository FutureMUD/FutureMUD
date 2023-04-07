using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class SecondWindExhausted : Effect, ISecondWindExhaustedEffect
{
	public SecondWindExhausted(ICharacter owner, ISecondWindMerit merit) : base(owner)
	{
		Merit = merit;
	}

	protected SecondWindExhausted(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		Merit = Gameworld.Merits.Get(long.Parse(effect.Element("Merit").Value)) as ISecondWindMerit;
	}

	public override bool SavingEffect => true;

	protected override XElement SaveDefinition()
	{
		return new XElement("Merit", Merit?.Id ?? 0);
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("SecondWindExhausted", (effect, owner) => new SecondWindExhausted(effect, owner));
	}

	public ISecondWindMerit Merit { get; set; }

	protected override string SpecificEffectType => "SecondWindExhausted";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Second wind has been exhausted {(Merit == null ? "[Universal]" : Merit.Name)}";
	}

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		Owner.OutputHandler.Send(Merit?.RecoveryMessage ?? Gameworld.GetStaticString("OnSecondWindRecoverMessage"));
	}
}