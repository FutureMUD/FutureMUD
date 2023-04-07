using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PunishmentStrategies;

internal class PunishmentStrategyExecute : PunishmentStrategyBase
{
	public PunishmentStrategyExecute(IFuturemud gameworld) : base(gameworld)
	{
	}

	public PunishmentStrategyExecute(IFuturemud gameworld, XElement root) : base(gameworld, root)
	{
	}

	public override string TypeSpecificHelpText => @"";

	public override string Describe(IPerceiver voyeur)
	{
		return $"a death sentence";
	}

	public override bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			default:
				return base.BuildingCommand(actor, authority, command.GetUndo());
		}
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Death Sentence".ColourName());
		BaseShowText(actor, sb);
		return sb.ToString();
	}

	public override PunishmentResult GetResult(ICharacter actor, ICrime crime)
	{
		return new PunishmentResult { Execution = true };
	}

	protected override void SaveSpecificType(XElement root)
	{
		root.Add(new XAttribute("type", "execute"));
	}
}