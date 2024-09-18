using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp.NPC.AI;
public class LawyerAI : PathingAIBase
{
	public static void RegisterLoader()
	{
		RegisterAIType("Lawyer", (ai, gameworld) => new LawyerAI(ai, gameworld));
		RegisterAIBuilderInformation("lawyer", (game, name) => new LawyerAI(game, name), new LawyerAI().HelpText);
	}

	protected LawyerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	private LawyerAI()
	{

	}

	private LawyerAI(IFuturemud gameworld, string name) : base(gameworld, name, "Lawyer")
	{
		CanBeEngagedAsCourtAppointedLawyer = true;
		CanBeHiredProg = Gameworld.AlwaysTrueProg;
		FeeProg = Gameworld.AlwaysOneProg;
		DatabaseInitialise();
	}

	/// <inheritdoc />
	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay),
			new XElement("CanBeEngagedAsCourtAppointedLawyer", CanBeEngagedAsCourtAppointedLawyer),
			new XElement("CanBeHiredProg", CanBeHiredProg.Id),
			new XElement("HomeBaseProg", HomeBaseProg?.Id ?? 0),
			new XElement("FeeProg", FeeProg.Id)
		).ToString();
	}

	public bool CanBeEngagedAsCourtAppointedLawyer { get; private set; }
	public IFutureProg FeeProg { get; private set; }
	public IFutureProg CanBeHiredProg { get; private set; }
	public IFutureProg HomeBaseProg { get; private set; }

	/// <inheritdoc />
	protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		throw new NotImplementedException();
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine($"Can Be Court Appointed: {CanBeEngagedAsCourtAppointedLawyer.ToColouredString()}");
		sb.AppendLine($"Fee Prog: {FeeProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Can Be Hired Prog: {CanBeHiredProg.MXPClickableFunctionName()}");
		sb.AppendLine($"Home Base Prog: {HomeBaseProg?.MXPClickableFunctionName() ?? "None"}");
		return sb.ToString();
	}

	/// <inheritdoc />
	protected override string TypeHelpText => $@"{base.TypeHelpText}
	";
}
