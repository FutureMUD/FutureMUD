using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.Effects.Interfaces;

#nullable enable
namespace MudSharp.Effects.Concrete;

/// <summary>Timed non-magical speech disruption using native babbling communication rules.</summary>
public sealed class SymptomBabbleEffect : Effect, IBabbleSpeechEffect
{
	public SymptomBabbleEffect(IPerceivable owner) : base(owner) { }
	private SymptomBabbleEffect(XElement root, IPerceivable owner) : base(root, owner) { }
	public static void InitialiseEffectType() => RegisterFactory("SymptomBabble", (root, owner) => new SymptomBabbleEffect(root, owner));
	protected override string SpecificEffectType => "SymptomBabble";
	public override bool SavingEffect => true;
	protected override XElement SaveDefinition() => new("Effect");
	public override string Describe(IPerceiver voyeur) => "Speech is temporarily scrambled by an involuntary symptom.";
}
