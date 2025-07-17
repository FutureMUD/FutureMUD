using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg.Statements.Manipulation;
using MudSharp.Models;

namespace MudSharp.NPC.AI;
public class MountAI : ArtificialIntelligenceBase
{
	public static void RegisterLoader()
	{
		RegisterAIType("Mount", (ai, gameworld) => new MountAI(ai, gameworld));
		RegisterAIBuilderInformation("mount", (gameworld, name) => new MountAI(gameworld, name), new MountAI().HelpText);
	}

	private MountAI()
	{
	}

	protected MountAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	protected MountAI(IFuturemud gameworld, string name) : base(gameworld, name, "Wanderer")
	{
		DatabaseInitialise();
	}

	private void LoadFromXml(XElement root)
	{
		
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition"
		).ToString();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				default:
					return false;
			}
		}

		return false;
	}
}
