#nullable enable

using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.Templates;

namespace MudSharp.Magic;

/// <summary>Builder-installed living guardians; explicit native template and timing mappings.</summary>
public static class ArmageddonAirGuardianStock
{
	public const string Key = "arm.spell.air_guardian";
	public const string Name = "Air Guardian";
	public const string Family = "air-guardian";
	// Authored native policy from the repertoire proposal, not recovered historical time units.
	public const string ProposedLifetimeSeconds = "120*grade";
	public const double MinimumNativeEnergy = 9;

	public static MagicSpell Create(IFuturemud world, IMagicSchool school, ITraitDefinition trait,
		IMagicResource resource, INPCTemplate template, string lifetimeSeconds)
	{
		if (NativeNpcCreationEligibility.TemplateError(template, world) is { } error)
			throw new InvalidOperationException(error);
		if (world.SpellOwnedNpcs is null) throw new InvalidOperationException("Spell-owned native NPC creation is unavailable.");
		if (string.IsNullOrWhiteSpace(lifetimeSeconds) || new TraitExpression(lifetimeSeconds, world).HasErrors())
			throw new InvalidOperationException("Select a valid lifetime expression in real seconds.");
		return ArmageddonUtilityStock.Create(world, school, trait, resource, Name,
			"Summon one living guardian per requested grade. Each protects and follows the creator through native combat and movement. " +
			"The selected approved template supplies its anatomy, equipment and combat settings. Temporary guardians dissipate without a corpse; " +
			"foreign possessions are conserved. Count, creator protection and lifecycle are editable. " +
			"The shared source-efficiency curve has an authored nine-energy minimum. The explicitly selected real-second lifetime is authored native policy; historical duration units remain unverified.",
			"0", MinimumNativeEnergy, "$0 summon|summons protecting spirits from the air.",
			(resourceId, costId, _) => Definition(resourceId, costId, template.Id, lifetimeSeconds));
	}

	public static XElement Definition(long resourceId, long costId, long templateId, string lifetimeSeconds) =>
		ArmageddonUtilityStock.Definition(Key, "room", resourceId, costId, 0, 30, MinimumNativeEnergy,
			new XElement("Effect", new XAttribute("type", "createnpc"), new XElement("NPCPrototypeId", templateId),
				new XElement("OnLoadProg", 0), new XElement("Lifecycle", new XAttribute("version", 1),
					new XAttribute("mode", "TemporaryCleanup"), new XElement("Family", Family),
					new XElement("Seconds", lifetimeSeconds), new XElement("Count", "grade"), new XElement("GuardCaster", true))));
}
