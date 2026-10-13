#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Magic;

/// <summary>Resolve these exact S2 identities through typed ownership before constructing stock.</summary>
public sealed record ArmageddonSimpleStockBindings(long VoidReserveResourceId, long AuthoredFireSchoolId)
{
	public void Validate()
	{
		ArmageddonPrimitiveSpellBindings.RequirePositive(VoidReserveResourceId, nameof(VoidReserveResourceId));
		ArmageddonPrimitiveSpellBindings.RequirePositive(AuthoredFireSchoolId, nameof(AuthoredFireSchoolId));
	}
}

/// <summary>
/// Locked keys, schools, floors, targets and lifetimes with editable native effect defaults.
/// Definition availability does not grant an admission or certify native gameplay.
/// </summary>
public static class ArmageddonSimpleStockContent
{
	public const string ArcaneMarkTag = "armageddon.arcane_mark";
	public const string VeilOfElementsKey = "arm.spell.veil_of_elements";

	public static IReadOnlyList<ArmageddonCanonicalSpellContribution> Create(ArmageddonSimpleStockBindings bindings)
	{
		ArgumentNullException.ThrowIfNull(bindings);
		bindings.Validate();
		return Array.AsReadOnly(new[]
		{
			Scene("fire", "fireworks", "Fireworks", 7, 40,
				"Brilliant sparks blossom overhead in fleeting patterns of coloured flame."),
			Scene("water", "distant_mirage", "Distant Mirage", 7, 40,
				"A distant scene shimmers at the edge of vision, wavering like heat above sand."),
			Scene("shadow", "shadowplay", "Shadowplay", 7, 40,
				"Unreal silhouettes dance across the surroundings, then dissolve into shadow."),
			Scene("lightning", "sky_lantern", "Sky Lantern", 7, 40,
				"A luminous phantom lantern hangs overhead, its colours flickering across the sky."),
			Scene("void", "phantasm", "Phantasm", 7, 40,
				"An insubstantial apparition takes shape nearby, its edges trembling and fading."),
			VeilOfElements(),
			Entry("wind", "buoyant_lift", "Buoyant Lift", 7, 60, "character", "60*grade",
				"Suspends the selected character through native levitation for 60 seconds per grade. " +
				"Authored editable defaults preserve the current room layer and grant no trait bonus.",
				new XElement("Effect", new XAttribute("type", "levitate"), new XElement("PreserveLayer", true),
					new XElement("TargetLayer", (int)RoomLayer.InAir), new XElement("SDescAddendum", new XCData("(levitating)")),
					new XElement("DescAddendum", new XCData("@ appears to be suspended in the air.")),
					new XElement("AddendumColour", Telnet.BoldCyan.Name))),
			Entry("wind", "windborne_flight", "Windborne Flight", 25, 40, "character", "60*grade",
				"Grants the native magical flight permission for 60 seconds per grade, without an implied trait bonus.",
				new XElement("Effect", new XAttribute("type", "flying"))),
			Entry("wind", "gentle_descent", "Gentle Descent", 20, 60, "character", "60*grade",
				"Applies native feather fall for 60 seconds per grade. Authored editable defaults set fall distance and damage multipliers to zero.",
				new XElement("Effect", new XAttribute("type", "featherfall"), new XElement("FallDistanceMultiplier", 0),
					new XElement("FallDamageMultiplier", 0), new XElement("SDescAddendum", new XCData("(falling lightly)")),
					new XElement("DescAddendum", new XCData("@ seems to drift lightly through the air.")),
					new XElement("AddendumColour", Telnet.BoldCyan.Name))),
			Barrier("earth", "sand_barrier", "Sand Barrier", 60),
			Barrier("wind", "wind_barrier", "Wind Barrier", 40),
			CloudedWill(bindings.VoidReserveResourceId),
			ArcaneMark(),
			Entry("void", "unmaking_ward", "Unmaking Ward", 25, 60, "character", "60*grade",
				"Applies one native personal school ward for 60 seconds per grade. Authored editable defaults block only the supplied Fire school, " +
				"in both directions, without subschools or reflection.", Ward("personalward", bindings.AuthoredFireSchoolId)),
			Entry("void", "bar_the_elements", "Bar the Elements", 33, 40, "room", "60*grade",
				"Applies one native room school ward for 60 seconds per grade. Authored editable defaults block only the supplied Fire school, " +
				"in both directions, without subschools or reflection.", Ward("roomward", bindings.AuthoredFireSchoolId))
		});
	}

	/// <summary>Optional builder stock only; deliberately has no historical capability membership.</summary>
	public static IReadOnlyList<ArmageddonUtilitySpellContent> OptionalWardcraft() => Array.AsReadOnly(new[]
	{
		Entry("earth", "wardcraft", "Wardcraft", 4, 40, "character", "60*grade",
			"Optional authored builder stock: one native tag-scoped personal ward for 60 seconds per grade. " +
			"Editable defaults fail matching Arcane Mark metadata in both directions; no school-wide or all-effects reflection is implied.",
			new XElement("Effect", new XAttribute("type", "personaltagward"), new XElement("Tag", new XCData(ArcaneMarkTag)),
				new XElement("Value", new XCData(string.Empty)), new XElement("MatchValue", false),
				new XElement("Mode", MagicInterdictionMode.Fail), new XElement("Coverage", MagicInterdictionCoverage.Both), new XElement("Prog", 0))).Content
	});

	private static ArmageddonCanonicalSpellContribution Scene(string school, string suffix, string name, double floor, int opening, string text) =>
		Entry(school, suffix, name, floor, opening, "room", "60*grade",
			"Displays one noninteractive perceptible scene for 60 seconds per grade, without collision, equipment, identity or damage rights. " +
			"The scene text and audience are authored editable defaults, initially visible to everyone.", Phantom("arm.spell." + suffix, text));

	private static XElement Phantom(string key, string text) => new("Effect", new XAttribute("type", "phantomillusion"),
		new XElement("Text", new XCData(text)), new XElement("AudienceScope", "Everyone"), new XElement("ClanId", 0),
		new XElement("ViewerProg", 0), new XElement("Priority", 0), new XElement("IllusionKey", new XCData(key)), new XElement("Colour", "none"));

	private static ArmageddonCanonicalSpellContribution VeilOfElements()
	{
		var contribution = Entry("void", "veil_of_elements", "Veil of Elements", 20, 60, "room", "60*grade",
			"Adds a noninteractive perceptual scene and reduces room illumination by five lux per grade for 60 seconds per grade. " +
			"Native light lower bounds remain; there is no physical barrier. The scene text is an authored editable default.",
			Phantom(VeilOfElementsKey, "A dim, unreal veil softens the edges and colours of the surroundings."),
			new XElement("Effect", new XAttribute("type", "roomlight"), new XElement("AddedLuxPerPower", -5),
				new XElement("DescAddendum", new XCData(string.Empty)), new XElement("GlowAddendumColour", Telnet.BoldBlack.Name)));
		var content = contribution.Content;
		return contribution with { Content = content with { BuildDefinition = (resource, cost, filter) =>
		{
			var definition = content.BuildDefinition(resource, cost, filter);
			definition.Element("ControlledPower")!.Element("ScalarBindings")!.Add(new XElement("Binding", new XAttribute("list", "target"),
				new XAttribute("index", 1), new XAttribute("effect", "roomlight"), new XAttribute("field", "AddedLuxPerPower"), "-5*grade/power"));
			return definition;
		} } };
	}

	private static ArmageddonCanonicalSpellContribution Barrier(string school, string suffix, string name, int opening) =>
		Entry(school, suffix, name, 10, opening, "exit", "60*grade",
			"Denies traversal of the selected native exit for 60 seconds per grade through an editable magical barrier, without damage.",
			new XElement("Effect", new XAttribute("type", "exitbarrier")));

	private static ArmageddonCanonicalSpellContribution CloudedWill(long reserve) =>
		Entry("earth", "clouded_will", "Clouded Will", 20, 40, "character", "0",
			"Removes at most five units per grade from the exact bound Void reserve, floored at zero, with no caster credit.",
			new XElement("Effect", new XAttribute("type", "magicresourcedelta"), new XElement("Resource", reserve),
				new XElement("Formula", new XCData("-5*grade")), new XElement("ExactResourceDepletion", true)));

	private static ArmageddonCanonicalSpellContribution ArcaneMark() =>
		Entry("void", "arcane_mark", "Arcane Mark", 25, 60, "item", "300*grade",
			"Applies one informational metadata mark to the exact selected item for 300 seconds per grade. " +
			"The native spell parent retains caster attribution. Authored editable text grants no identity, ownership or equipment rights; other casters' marks remain.",
			new XElement("Effect", new XAttribute("type", "magictag"), new XElement("Tag", new XCData(ArcaneMarkTag)),
				new XElement("Value", new XCData("An angular arcane glyph.")), new XElement("ReplaceExisting", false)), exclusive: false);

	private static XElement Ward(string type, long school) => new("Effect", new XAttribute("type", type),
		new XElement("School", school), new XElement("Mode", MagicInterdictionMode.Fail), new XElement("Coverage", MagicInterdictionCoverage.Both),
		new XElement("IncludesSubschools", false), new XElement("Prog", 0));

	private static ArmageddonCanonicalSpellContribution Entry(string school, string suffix, string name, double floor, int opening,
		string trigger, string duration, string description, XElement effect, bool exclusive = true) =>
		Entry(school, suffix, name, floor, opening, trigger, duration, description, [effect], exclusive);

	private static ArmageddonCanonicalSpellContribution Entry(string school, string suffix, string name, double floor, int opening,
		string trigger, string duration, string description, XElement first, XElement second) =>
		Entry(school, suffix, name, floor, opening, trigger, duration, description, [first, second], true);

	private static ArmageddonCanonicalSpellContribution Entry(string school, string suffix, string name, double floor, int opening,
		string trigger, string duration, string description, XElement[] effects, bool exclusive)
	{
		var key = "arm.spell." + suffix;
		var content = new ArmageddonUtilitySpellContent(key, name, description, duration, floor,
			"$0 shape|shapes an enchantment around $1.", (resource, cost, filter) =>
			{
				ArmageddonPrimitiveSpellBindings.RequirePositive(resource, nameof(resource));
				ArmageddonPrimitiveSpellBindings.RequirePositive(cost, nameof(cost));
				if (filter < 0 || (trigger is "room" or "exit" && filter != 0))
					throw new ArgumentOutOfRangeException(nameof(filter), "This native trigger has no target-filter field; use zero.");
				var definition = ArmageddonUtilitySpellContent.Definition(key, trigger, resource, cost, filter, opening, floor, new XElement(effects[0]));
				definition.Element("Effects")!.ReplaceNodes(effects.Select(x => new XElement(x)));
				if (trigger is "room" or "exit") definition.Element("Trigger")!.Element("TargetFilterProg")!.Remove();
				return definition;
			}, ExclusiveEffects: exclusive);
		return new("arm.capability." + school, content);
	}
}
