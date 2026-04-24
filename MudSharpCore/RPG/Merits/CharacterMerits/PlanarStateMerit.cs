using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.Planes;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class PlanarStateMerit : CharacterMeritBase, IPlanarOverlayMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Planar State",
			(merit, gameworld) => new PlanarStateMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Planar State",
			(gameworld, name) => new PlanarStateMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Planar State",
			"Applies a permanent planar corporeality overlay while the merit applies",
			new PlanarStateMerit().HelpText);
	}

	protected PlanarStateMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var root = XElement.Parse(merit.Definition);
		PlanarPresenceDefinition = PlanarPresenceDefinition.FromXml(root.Element("PlanarData"), gameworld);
		PlanarPriority = int.Parse(root.Element("PlanarPriority")?.Value ?? "0");
		OverridesBasePlanarPresence = bool.Parse(root.Element("OverridesBase")?.Value ?? "true");
	}

	protected PlanarStateMerit()
	{
	}

	protected PlanarStateMerit(IFuturemud gameworld, string name)
		: base(gameworld, name, "Planar State", "@ have|has an altered planar state")
	{
		PlanarPresenceDefinition = PlanarPresenceDefinition.NonCorporeal(gameworld.DefaultPlane ?? gameworld.Planes.First());
		PlanarPriority = 0;
		OverridesBasePlanarPresence = true;
		DoDatabaseInsert();
	}

	public PlanarPresenceDefinition PlanarPresenceDefinition { get; private set; }
	public int PlanarPriority { get; private set; }
	public bool OverridesBasePlanarPresence { get; private set; }

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(
			PlanarPresenceDefinition.SaveToXml(),
			new XElement("PlanarPriority", PlanarPriority),
			new XElement("OverridesBase", OverridesBasePlanarPresence));
		return root;
	}

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Planar State: {PlanarPresenceDefinition.Describe(actor.Gameworld).ColourValue()}");
		sb.AppendLine($"Priority: {PlanarPriority.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Overrides Base: {OverridesBasePlanarPresence.ToColouredString()}");
	}

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3planar corporeal [plane]#0 - sets this merit to make the character corporeal on a plane
	#3planar noncorporeal [plane] [visible]#0 - sets this merit to make the character noncorporeal on a plane
	#3planar xml <xml>#0 - sets explicit PlanarData XML
	#3priority <number>#0 - sets overlay priority
	#3override#0 - toggles whether this replaces or merges with the base planar state";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "planar":
			case "plane":
			case "corporeality":
				return BuildingCommandPlanar(actor, command);
			case "priority":
				return BuildingCommandPriority(actor, command);
			case "override":
			case "overrides":
				return BuildingCommandOverride(actor);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandPlanar(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "corporeal":
			case "manifest":
			case "manifested":
				{
					var plane = command.IsFinished
						? actor.Gameworld.DefaultPlane
						: actor.Gameworld.Planes.GetByIdOrName(command.SafeRemainingArgument);
					if (plane is null)
					{
						actor.OutputHandler.Send("There is no such plane.");
						return false;
					}

					PlanarPresenceDefinition = PlanarPresenceDefinition.Manifested(plane);
					break;
				}
			case "noncorporeal":
			case "incorporeal":
			case "dissipate":
			case "dissipated":
				{
					var plane = actor.Gameworld.DefaultPlane;
					var visible = false;
					while (!command.IsFinished)
					{
						var token = command.PopSpeech();
						if (token.EqualToAny("visible", "seen", "manifest"))
						{
							visible = true;
							continue;
						}

						plane = actor.Gameworld.Planes.GetByIdOrName(token);
						if (plane is null)
						{
							actor.OutputHandler.Send($"There is no plane identified by {token.ColourCommand()}.");
							return false;
						}
					}

					PlanarPresenceDefinition = PlanarPresenceDefinition.NonCorporeal(plane, visible);
					break;
				}
			case "xml":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What PlanarData XML do you want to use?");
					return false;
				}

				PlanarPresenceDefinition = PlanarPresenceDefinition.FromXml(command.SafeRemainingArgument, actor.Gameworld);
				break;
			default:
				actor.OutputHandler.Send("You must specify corporeal, noncorporeal, or xml.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"This merit now applies {PlanarPresenceDefinition.Describe(actor.Gameworld).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPriority(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send("You must enter a valid priority number.");
			return false;
		}

		PlanarPriority = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit now has planar priority {PlanarPriority.ToString("N0", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandOverride(ICharacter actor)
	{
		OverridesBasePlanarPresence = !OverridesBasePlanarPresence;
		Changed = true;
		actor.OutputHandler.Send($"This merit will {OverridesBasePlanarPresence.NowNoLonger()} replace the base planar state.");
		return true;
	}
}
