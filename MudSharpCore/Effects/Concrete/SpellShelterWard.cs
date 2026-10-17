#nullable enable

using MudSharp.Character;
using MudSharp.Magic;

namespace MudSharp.Effects.Concrete;

/// <summary>A shelter-owned participant in the existing native interdiction pipeline.</summary>
public sealed class SpellShelterWard : Effect, IMagicInterdictionEffect, IMagicContextualInterdictionEffect
{
	public const string EffectType = "SpellShelterWard";
	public static void InitialiseEffectType() => RegisterFactory(EffectType, (root, owner) => new SpellShelterWard(root, owner));
	public Guid LifecycleId { get; }
	public long SpellId { get; }
	public SpellShelterWardConfiguration Configuration { get; }
	public SpellShelterWard(XElement root, IPerceivable owner) : base(root, owner)
	{
		var definition = root.Element("Effect") ?? throw new FormatException("Missing shelter ward definition.");
		if (definition.Elements().Count() != 3 || definition.Elements("Lifecycle").Count() != 1 ||
			definition.Elements("Spell").Count() != 1 || definition.Elements("Ward").Count() != 1)
			throw new FormatException("Unsupported shelter ward effect schema.");
		LifecycleId = Guid.Parse(definition.Element("Lifecycle")!.Value);
		SpellId = (long)definition.Element("Spell")!;
		if (LifecycleId == Guid.Empty || SpellId <= 0) throw new FormatException("Invalid ward origin.");
		Configuration = SpellShelterWardConfiguration.Load(definition.Element("Ward")!);
	}
	public static XElement Envelope(Guid lifecycleId, long spellId, SpellShelterWardConfiguration configuration) =>
		new("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", EffectType), new XElement("Original", 0),
			new XElement("Remaining", 0), Definition(lifecycleId, spellId, configuration));
	private static XElement Definition(Guid lifecycleId, long spellId, SpellShelterWardConfiguration configuration) =>
		new("Effect", new XElement("Lifecycle", lifecycleId), new XElement("Spell", spellId), configuration.Save());
	protected override XElement SaveDefinition() => Definition(LifecycleId, SpellId, Configuration);
	protected override string SpecificEffectType => EffectType;
	public override bool SavingEffect => true;
	public override bool Applies() => true;
	public IMagicSchool School => Gameworld.MagicSpells.Get(SpellId).School;
	public MagicInterdictionCoverage Coverage => Configuration.Coverage;
	public MagicInterdictionMode Mode => MagicInterdictionMode.Fail;
	public bool IncludesSubschools => Configuration.IncludesSubschools;
	public bool ShouldInterdict(ICharacter source, IMagicSchool school) => Configuration.SchoolIds.Any(id =>
		school.Id == id || IncludesSubschools && Gameworld.MagicSchools.Get(id) is { } parent && school.IsChildSchool(parent));
	public bool ShouldInterdict(MagicInterdictionContext context) => ShouldInterdict(context.Source, context.School) ||
		context.Tags.Any(tag => Configuration.Tags.Any(selected => selected.EqualTo(tag.Tag)));
	public override string Describe(IPerceiver voyeur) => $"Shelter ward: {Coverage.DescribeEnum().ColourName()}, " +
		$"schools {Configuration.SchoolIds.Select(x => x.ToString("N0", voyeur)).ListToCommaSeparatedValues().ColourValue()}, " +
		$"invocation tags {Configuration.Tags.ListToCommaSeparatedValues().ColourValue()}.";

	internal bool Matches(SpellLifecycleOrigin origin, SpellShelterWardConfiguration configuration) =>
		LifecycleId == origin.Id && SpellId == origin.SpellId && XNode.DeepEquals(Configuration.Save(), configuration.Save());
}
