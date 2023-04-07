using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class FuseGameItemComponent : GameItemComponent, ILightable
{
	protected FuseGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (FuseGameItemComponentProto)newProto;
	}

	#region Constructors

	public FuseGameItemComponent(FuseGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
		BurnTimeRemaining = _prototype.BurnTime;
	}

	public FuseGameItemComponent(MudSharp.Models.GameItemComponent component, FuseGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public FuseGameItemComponent(FuseGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		BurnTimeRemaining = TimeSpan.FromSeconds(double.Parse(root.Element("BurnTimeRemaining").Value));
		Lit = bool.Parse(root.Element("Lit").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new FuseGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("BurnTimeRemaining", BurnTimeRemaining.TotalSeconds),
			new XElement("Lit", _lit)
		).ToString();
	}

	#endregion

	public override void Delete()
	{
		base.Delete();
		if (Lit)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
		}
	}

	public override void Quit()
	{
		base.Quit();
		if (Lit)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
		}
	}

	private void HeartbeatManager_SecondHeartbeat()
	{
		BurnTimeRemaining -= TimeSpan.FromSeconds(1);
		Changed = true;
		if (BurnTimeRemaining <= TimeSpan.Zero)
		{
			Parent.GetItemType<IDetonatable>()?.Detonate();
		}
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full || type == DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				if (string.IsNullOrWhiteSpace(_prototype.IgnitedTagAddendum))
				{
					return description;
				}

				return
					$"{description}{(Lit ? _prototype.IgnitedTagAddendum.SubstituteANSIColour().LeadingSpaceIfNotEmpty() : "")}";
			case DescriptionType.Full:
				return
					$"{description}\n\n{(Lit ? string.Format(voyeur, "It is currently lit and looks to have about {0} of burn time remaining.", BurnTimeRemaining.Describe(voyeur).Colour(Telnet.BoldRed)) : string.Format(voyeur, "It looks to have {0} of burn time remaining.", BurnTimeRemaining.Describe(voyeur).Colour(Telnet.Red)))}";
			case DescriptionType.Evaluate:
				return
					$"It has a fuse that will burn for {BurnTimeRemaining.Describe(voyeur).Colour(Telnet.Green)} before causing a detonation.\nOnce lit, it {(_prototype.Extinguishable ? "can" : "cannot")} be extinguished.";
		}

		throw new NotSupportedException("Invalid Decorate type in FuseGameItemComponent.Decorate");
	}

	public override int DecorationPriority => int.MaxValue;

	#region ILightable Implementation

	public TimeSpan BurnTimeRemaining { get; protected set; }

	private bool _lit;

	public bool Lit
	{
		get => _lit;
		set
		{
			if (_lit != value)
			{
				_lit = value;
				Changed = true;
				if (_lit)
				{
					Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
					Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
				}
				else
				{
					Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
				}
			}
		}
	}

	public bool CanLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return false;
		}

		return !Lit;
	}

	public string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if (Lit)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is already lit.";
		}

		throw new ApplicationException("Invalid WhyCannotLight reason in FuseGameItemComponent.WhyCannotLight");
	}

	public bool Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)
	{
		if (!CanLight(lightee, ignitionSource))
		{
			lightee.Send(WhyCannotLight(lightee, ignitionSource));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.IgniteEmote, lightee, lightee, Parent, ignitionSource)).Append(
				playerEmote));
		Lit = true;
		return true;
	}

	public bool CanExtinguish(ICharacter lightee)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return false;
		}

		if (!_prototype.Extinguishable)
		{
			return false;
		}

		return Lit;
	}

	public string WhyCannotExtinguish(ICharacter lightee)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if (!_prototype.Extinguishable)
		{
			return
				$"You cannot extinguish {Parent.HowSeen(lightee)} because it is not something that can be extinguished once lit.";
		}

		if (!Lit)
		{
			return $"You cannot extinguish {Parent.HowSeen(lightee)} because it is not lit.";
		}

		throw new NotSupportedException("Invalid reason in FusehGameItemComponent.WhyCannotExtinguish");
	}

	public bool Extinguish(ICharacter lightee, IEmote playerEmote)
	{
		if (!CanExtinguish(lightee))
		{
			lightee.Send(WhyCannotExtinguish(lightee));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.ExtinguishEmote, lightee, lightee, Parent)).Append(playerEmote));
		Lit = false;
		return true;
	}

	#endregion
}