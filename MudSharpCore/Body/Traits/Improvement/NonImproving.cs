using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement;

public class NonImproving : ImprovementModel
{
	public NonImproving()
	{
	}

	public NonImproving(IFuturemud gameworld, long id)
	{
		Gameworld = gameworld;
		_id = id;
	}

	/// <inheritdoc />
	public override IImprovementModel Clone(string name)
	{
		return null;
	}

	public override double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome,
		TraitUseType usetype)
	{
		trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
			"-- NoGain [Non-Improving Trait]");
		return 0.0;
	}

	/// <inheritdoc />
	public override bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType,
		bool ignoreTemporaryBlockers)
	{
		return false;
	}

	/// <inheritdoc />
	protected override string SubtypeHelpText => "";

	/// <inheritdoc />
	protected override XElement SaveDefinition()
	{
		return new XElement("Definition");
	}

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Improver #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Type: {ImproverType.TitleCase().ColourValue()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		actor.OutputHandler.Send($"Non-Improving Improvement Models have no properties that can be edited.");
		return false;
	}

	public override string Name => "Non Improving";

	/// <inheritdoc />
	public override string ImproverType => "non-improving";
}