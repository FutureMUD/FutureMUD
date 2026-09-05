#nullable enable

using MudSharp.Magic;
using MudSharp.RPG.Law;

namespace MudSharp.Effects.Concrete;

/// <summary>A dispellable handle. The legal record remains the authority across body changes.</summary>
public sealed class WitnessRecallSuppressionEffect : TimedPsychicEffect
{
	public long CrimeId { get; }
	public long WitnessId { get; }
	public DateTime UntilUtc { get; }
	public WitnessRecallSuppressionEffect(ICharacter owner, IMagicPower power, Crime crime, CrimeWitnessMemory memory) : base(owner, power)
	{
		CrimeId = crime.Id;
		WitnessId = memory.SourceId;
		UntilUtc = memory.SuppressedUntilUtc!.Value;
	}
	private WitnessRecallSuppressionEffect(XElement xml, IPerceivable owner) : base(xml, owner)
	{
		var root = xml.Element("Effect")!;
		CrimeId = (long)root.Attribute("crime")!;
		WitnessId = (long)root.Attribute("witness")!;
		UntilUtc = (DateTime)root.Attribute("until")!;
	}
	public static void InitialiseEffectType() => RegisterFactory("WitnessRecallSuppression", (xml, owner) => new WitnessRecallSuppressionEffect(xml, owner));
	protected override string SpecificEffectType => "WitnessRecallSuppression";
	public override bool SavingEffect => true;
	public override string Describe(IPerceiver voyeur) => "An incident is beyond this mind's recall.";
	protected override XElement SaveDefinition() => WithOrigin(new XElement("Effect", new XAttribute("crime", CrimeId), new XAttribute("witness", WitnessId), new XAttribute("until", UntilUtc)));
	public override void RemovalEffect()
	{
		var crime = Gameworld.Crimes.Get(CrimeId) as Crime;
		var memory = crime?.WitnessMemories.FirstOrDefault(x => x.Kind == CrimeWitnessSourceKind.Character && x.SourceId == WitnessId);
		if (memory is not null && !memory.PermanentlyForgotten && memory.SuppressedUntilUtc == UntilUtc && Owner is ICharacter actor)
			crime!.RestoreWitness(memory, actor);
	}
}
