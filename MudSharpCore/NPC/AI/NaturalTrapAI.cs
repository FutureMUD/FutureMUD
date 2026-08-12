#nullable enable

using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.Traps;

namespace MudSharp.NPC.AI;

/// <summary>
/// Lets a natural NPC maintain a natural trap at its current cell. It deliberately uses a template and normal
/// trap effect rather than a bespoke spider-web item, so natural hazards receive the same discovery, persistence,
/// trigger, and payload behaviour as other domains.
/// </summary>
public sealed class NaturalTrapAI : ArtificialIntelligenceBase
{
	public ITrapTemplate? TrapTemplate { get; private set; }
	public IFutureProg DeployEnabledProg { get; private set; } = null!;
	public IFutureProg SiteProg { get; private set; } = null!;

	public NaturalTrapAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private NaturalTrapAI()
	{
	}

	private NaturalTrapAI(IFuturemud gameworld, string name) : base(gameworld, name, "NaturalTrap")
	{
		DeployEnabledProg = gameworld.AlwaysTrueProg;
		SiteProg = gameworld.AlwaysTrueProg;
		DatabaseInitialise();
	}

	public override bool IsReadyToBeUsed =>
		TrapTemplate is not null &&
		TrapTemplate.SourceKind == TrapSourceKind.Natural &&
		TrapTemplate.Status == RevisionStatus.Current &&
		TrapTemplate.CanSubmit();

	public static void RegisterLoader()
	{
		RegisterAIType("NaturalTrap", (ai, gameworld) => new NaturalTrapAI(ai, gameworld));
		RegisterAIBuilderInformation(
			"naturaltrap",
			(gameworld, name) => new NaturalTrapAI(gameworld, name),
			new NaturalTrapAI().HelpText);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("TrapTemplateId", TrapTemplate?.Id ?? 0L),
			new XElement("TrapTemplateRevision", TrapTemplate?.RevisionNumber ?? 0),
			new XElement("DeployEnabledProg", DeployEnabledProg?.Id ?? 0L),
			new XElement("SiteProg", SiteProg?.Id ?? 0L)).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		var templateId = long.TryParse(root.Element("TrapTemplateId")?.Value, out var parsedTemplateId)
			? parsedTemplateId
			: 0L;
		var revision = int.TryParse(root.Element("TrapTemplateRevision")?.Value, out var parsedRevision)
			? parsedRevision
			: 0;
		TrapTemplate = templateId > 0 ? Gameworld.TrapTemplates.Get(templateId, revision) : null;
		DeployEnabledProg = Gameworld.FutureProgs.Get(
				long.TryParse(root.Element("DeployEnabledProg")?.Value, out var enabledId) ? enabledId : 0L) ??
			Gameworld.AlwaysTrueProg;
		SiteProg = Gameworld.FutureProgs.Get(
				long.TryParse(root.Element("SiteProg")?.Value, out var siteId) ? siteId : 0L) ??
			Gameworld.AlwaysTrueProg;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type != EventType.MinuteTick || arguments.Length == 0 || arguments[0] is not ICharacter character ||
		    character.State.IsDead() || character.State.IsInStatis() || character.Location is null ||
		    !DeployEnabledProg.ExecuteBool(false, character) ||
		    !SiteProg.ExecuteBool(false, character, character.Location) ||
		    !IsReadyToBeUsed)
		{
			return false;
		}

		var anchor = TrapEffect.IsValidAnchor(TrapTemplate!, character.Location) ? (IPerceivable)character.Location : character;
		if (anchor.EffectsOfType<TrapEffect>()
		    .Any(x => x.SourceKind == TrapSourceKind.Natural && x.State is not TrapState.Spent and not TrapState.Expired))
		{
			return false;
		}

		var trap = new TrapEffect(anchor, TrapTemplate!, character);
		if (TrapEffect.HasTimedLifetime(TrapTemplate!))
		{
			anchor.AddEffect(trap, TrapTemplate!.Lifespan!.Value);
		}
		else
		{
			anchor.AddEffect(trap);
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Contains(EventType.MinuteTick);
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Natural Trap Template: {TrapTemplate?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Deploy Enabled Prog: {DeployEnabledProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Site Prog: {SiteProg.MXPClickableFunctionName()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => @"	#3template <traptemplate>#0 - selects the current natural trap template to maintain
	#3enabled <prog>#0 - sets a boolean prog with character parameter controlling deployment
	#3site <prog>#0 - sets a boolean prog with character, location parameters selecting valid natural-trap cells";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "template":
				return BuildingCommandTemplate(actor, command);
			case "enabled":
			case "enabledprog":
				return BuildingCommandEnabledProg(actor, command);
			case "site":
			case "siteprog":
				return BuildingCommandSiteProg(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandTemplate(ICharacter actor, StringStack command)
	{
		var template = Gameworld.TrapTemplates.GetByIdOrName(command.SafeRemainingArgument);
		if (template is null || template.SourceKind != TrapSourceKind.Natural || template.Status != RevisionStatus.Current)
		{
			actor.Send("You must choose a current natural trap template.");
			return false;
		}

		if (!template.CanSubmit())
		{
			actor.Send($"That trap template is not ready: {template.WhyCannotSubmit()}");
			return false;
		}

		TrapTemplate = template;
		Changed = true;
		actor.Send($"This AI will now maintain {template.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandEnabledProg(ICharacter actor, StringStack command)
	{
		var prog = new ProgLookupFromBuilderInput(
			Gameworld,
			actor,
			command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			[ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		DeployEnabledProg = prog;
		Changed = true;
		actor.Send($"This AI will now use {prog.MXPClickableFunctionName()} to determine whether it can deploy traps.");
		return true;
	}

	private bool BuildingCommandSiteProg(ICharacter actor, StringStack command)
	{
		var prog = new ProgLookupFromBuilderInput(
			Gameworld,
			actor,
			command.SafeRemainingArgument,
			ProgVariableTypes.Boolean,
			[ProgVariableTypes.Character, ProgVariableTypes.Location]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		SiteProg = prog;
		Changed = true;
		actor.Send($"This AI will now use {prog.MXPClickableFunctionName()} to select natural trap cells.");
		return true;
	}
}
