using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.Models;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Effects.Interfaces;

namespace MudSharp.NPC.AI;
internal class StealthAI : ArtificialIntelligenceBase
{
	public static void RegisterLoader()
	{
		RegisterAIType("Stealth", (ai, gameworld) => new StealthAI(ai, gameworld));
		RegisterAIBuilderInformation("stealth", (gameworld, name) => new StealthAI(gameworld, name), new StealthAI().HelpText);
	}

	private StealthAI(ArtificialIntelligence ai, IFuturemud gameworld)
		: base(ai, gameworld)
	{
		LoadFromXml(XElement.Parse(ai.Definition));
	}

	private StealthAI()
	{
	}

	private StealthAI(IFuturemud gameworld, string name) : base(gameworld, name, "Stealth")
	{
		WillHide = true;
		WillSneak = true;
		HideWhenOthersArePresent = true;
		UseSubtleSneak = false;
		DatabaseInitialise();
	}

	private void LoadFromXml(XElement root)
	{
		HideWhenOthersArePresent = bool.Parse(root.Element("HideWhenOthersArePresent").Value);
		WillHide = bool.Parse(root.Element("WillHide").Value);
		WillSneak = bool.Parse(root.Element("WillSneak").Value);
		UseSubtleSneak = bool.Parse(root.Element("UseSubtleSneak").Value);
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("HideWhenOthersArePresent", HideWhenOthersArePresent),
			new XElement("WillHide", WillHide),
			new XElement("WillSneak", WillSneak),
			new XElement("UseSubtleSneak", UseSubtleSneak)
		).ToString();
	}

	public bool HideWhenOthersArePresent { get; protected set; }
	public bool WillHide { get; protected set; }
	public bool WillSneak { get; protected set; }
	public bool UseSubtleSneak { get; protected set; }

	/// <inheritdoc />
	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		var ch = (ICharacter)arguments[0];
		if (ch is null || ch.State.IsDead() || ch.State.IsInStatis())
		{
			return false;
		}

		switch (type)
		{
			case EventType.TenSecondTick:
				return HandleTenSecondTick(ch);
		}

		throw new NotImplementedException();
	}

	private bool HandleTenSecondTick(ICharacter ch)
	{
		if (WillSneak && !ch.AffectedBy<ISneakEffect>())
		{
			ch.AddEffect(UseSubtleSneak ? new SneakSubtle(ch) : new Sneak(ch));
		}

		if (!WillHide || ch.AffectedBy<IHideEffect>())
		{
			return false;
		}

		if (!HideWhenOthersArePresent && ch.Location.LayerCharacters(ch.RoomLayer).Except(ch).Any(x => ch.CanSee(x)))
		{
			return false;
		}

		ch.ExecuteCommand("hide");
		return true;
	}

	/// <inheritdoc />
	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.TenSecondTick:
					return true;
			}
		}
		
		return false;
	}

	/// <inheritdoc />
	protected override string TypeHelpText => @"	#3hide#0 - toggles hiding
	#3sneak#0 - toggles sneaking
	#3subtle#0 - toggles using subtle sneak
	#3alone#0 - toggles hiding when not alone";

	/// <inheritdoc />
	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Artificial Intelligence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Type: {AIType.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine($"Will Hide: {WillHide.ToColouredString()}");
		sb.AppendLine($"Will Sneak: {WillSneak.ToColouredString()}");
		sb.AppendLine($"Sneak Subtly: {UseSubtleSneak.ToColouredString()}");
		sb.AppendLine($"Hide When Not Alone: {HideWhenOthersArePresent.ToColouredString()}");
		return sb.ToString();
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "hide":
				return BuildingCommandHide(actor);
			case "sneak":
				return BuildingCommandSneak(actor);
			case "subtle":
				return BuildingCommandSubtle(actor);
			case "alone":
				return BuildingCommandAlone(actor);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandAlone(ICharacter actor)
	{
		HideWhenOthersArePresent = !HideWhenOthersArePresent;
		Changed = true;
		actor.OutputHandler.Send($"This AI will {HideWhenOthersArePresent.NowNoLonger()} hide when other characters are present in the room.");
		return true;
	}

	private bool BuildingCommandSubtle(ICharacter actor)
	{
		UseSubtleSneak = !UseSubtleSneak;
		Changed = true;
		actor.OutputHandler.Send($"This AI will {UseSubtleSneak.NowNoLonger()} use subtle sneak instead of regular sneak when it sneaks.");
		return true;
	}

	private bool BuildingCommandSneak(ICharacter actor)
	{
		WillSneak = !WillSneak;
		Changed = true;
		actor.OutputHandler.Send($"This AI will {WillSneak.NowNoLonger()} now attempt to sneak.");
		return true;

	}

	private bool BuildingCommandHide(ICharacter actor)
	{
		WillHide = !WillHide;
		Changed = true;
		actor.OutputHandler.Send($"This AI will {WillHide.NowNoLonger()} now attempt to hide.");
		return true;
	}
}
