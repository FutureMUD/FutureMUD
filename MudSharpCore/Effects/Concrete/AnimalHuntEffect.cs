#nullable enable
using MudSharp.NPC.AI;
namespace MudSharp.Effects.Concrete;
/// <summary>Compatibility loader for existing AnimalHunt saves; mechanics live in the shared pursuit effect.</summary>
public sealed class AnimalHuntEffect : CreaturePursuitEffect
{
	public AnimalHuntEffect(ICharacter owner, AnimalAI ai, ICharacter target) : base(owner, ai, target) { }
	private AnimalHuntEffect(XElement root, IPerceivable owner) : base(root, owner) { }
	public static void InitialiseEffectType() => RegisterFactory("AnimalHunt", (xml, owner) => new AnimalHuntEffect(xml, owner));
	protected override string SpecificEffectType => "AnimalHunt";
}
