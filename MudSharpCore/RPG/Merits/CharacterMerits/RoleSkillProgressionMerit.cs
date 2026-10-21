#nullable enable

using System.Globalization;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

/// <summary>Data-driven role trees; grants are composed once, rather than summed by each role.</summary>
public sealed class RoleSkillProgressionMerit : CharacterMeritBase, IRoleSkillProgressionMerit
{
	public const string MeritTypeName = "Role Skill Progression";
	public RoleSkillProgression Progression { get; private set; } = new([]);
	private RoleSkillProgressionMerit() { }
	private RoleSkillProgressionMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld) =>
		Progression = LoadRules(XElement.Parse(merit.Definition).Element("Rules") ?? new XElement("Rules"));
	private RoleSkillProgressionMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, MeritTypeName, "@ follow|follows role-specific skill progression") => DoDatabaseInsert();

	public bool AppliesToTemplate(ICharacterTemplate template)
	{
		if (ApplicabilityProg is null) return true;
		if (ApplicabilityProg.MatchesParameters([ProgVariableTypes.Chargen])) return ApplicabilityProg.ExecuteBool(template);
		if (ApplicabilityProg.MatchesParameters([])) return ApplicabilityProg.ExecuteBool();
		throw new InvalidOperationException("Role skill progression applicability must accept a chargen/toon or no parameters for initial grants. Use role availability for character-only restrictions.");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser(MeritTypeName, (merit, world) => new RoleSkillProgressionMerit(merit, world));
		MeritFactory.RegisterBuilderMeritInitialiser(MeritTypeName, (world, name) => new RoleSkillProgressionMerit(world, name));
		MeritFactory.RegisterMeritHelp(MeritTypeName, "Composes primary skill trees with secondary role associations", new RoleSkillProgressionMerit().HelpText);
	}

	private RoleSkillProgression LoadRules(XElement rules)
	{
		var result = new RoleSkillProgression(rules.Elements("Rule").Select(x => new RoleSkillRule(
			(long)x.Attribute("role")!, (long)x.Attribute("trait")!, (bool)x.Attribute("primary")!,
			(double)x.Attribute("opening")!, (double)x.Attribute("cap")!, (long?)x.Attribute("parent"),
			(double?)x.Attribute("threshold") ?? 0, (double?)x.Attribute("bonus") ?? 0)));
		// Merits load before roles. Role identities are checked by the installer and when used.
		if (result.Rules.Any(x => Gameworld.Traits.Get(x.TraitId) is not ISkillDefinition ||
			x.ParentTraitId is { } parent && Gameworld.Traits.Get(parent) is not ISkillDefinition))
			throw new InvalidOperationException("Role skill progression refers to a missing role or skill.");
		return result;
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Rules", Progression.Rules.Select(x => new XElement("Rule",
			new XAttribute("role", x.RoleId), new XAttribute("trait", x.TraitId), new XAttribute("primary", x.Primary),
			new XAttribute("opening", x.Opening), new XAttribute("cap", x.Cap),
			x.ParentTraitId is { } parent ? new XAttribute("parent", parent) : null,
			new XAttribute("threshold", x.Threshold), new XAttribute("bonus", x.OverlapBonus)))));
		return root;
	}
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb) =>
		sb.AppendLine($"Role Skill Rules: {Progression.Rules.Count.ToString("N0", actor).ColourValue()}");
	protected override string SubtypeHelp => $"{base.SubtypeHelp}\n\t#3rules <Rules XML>#0 - replaces the validated role/skill progression rules";
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (command.PopForSwitch() != "rules") return base.BuildingCommand(actor, command.GetUndo());
		try
		{
			var xml = XElement.Parse(command.SafeRemainingArgument);
			if (xml.Name != "Rules") throw new ArgumentException("The root element must be Rules.");
			Progression = LoadRules(xml);
			Changed = true;
			actor.OutputHandler.Send($"Replaced the progression rules with {Progression.Rules.Count.ToString("N0", actor).ColourValue()} validated rules.");
			return true;
		}
		catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or System.Xml.XmlException or FormatException)
		{
			actor.OutputHandler.Send($"Invalid progression rules: {ex.Message}");
			return false;
		}
	}
}
