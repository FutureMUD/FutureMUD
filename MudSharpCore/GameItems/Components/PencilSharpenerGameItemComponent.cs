using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class PencilSharpenerGameItemComponent : GameItemComponent, ISharpen
{
	protected PencilSharpenerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PencilSharpenerGameItemComponentProto)newProto;
	}

	#region Constructors

	public PencilSharpenerGameItemComponent(PencilSharpenerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public PencilSharpenerGameItemComponent(MudSharp.Models.GameItemComponent component,
		PencilSharpenerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PencilSharpenerGameItemComponent(PencilSharpenerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PencilSharpenerGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region ISharpen Implementation

	public bool CanSharpen(ICharacter actor, IGameItem otherItem)
	{
		var pencil = otherItem.GetItemType<PencilGameItemComponent>();
		if (pencil == null)
		{
			return false;
		}

		if (pencil.UsesSinceSharpening <= 0)
		{
			return false;
		}

		return true;
	}

	public string WhyCannotSharpen(ICharacter actor, IGameItem otherItem)
	{
		var pencil = otherItem.GetItemType<PencilGameItemComponent>();
		if (pencil == null)
		{
			return $"You cannot sharpen {otherItem.HowSeen(actor)} with that because it only sharpens pencils.";
		}

		if (pencil.UsesSinceSharpening <= 0)
		{
			return
				$"You cannot sharpen {otherItem.HowSeen(actor)} because it is already as sharp as it is going to get.";
		}

		throw new ApplicationException(
			"Unknown WhyCannotSharpen reason in PencilSharpenerGameItemComponent.WhyCannotSharpen.");
	}

	public bool Sharpen(ICharacter actor, IGameItem otherItem)
	{
		if (!CanSharpen(actor, otherItem))
		{
			actor.Send(WhyCannotSharpen(actor, otherItem));
			return false;
		}

		var pencil = otherItem.GetItemType<PencilGameItemComponent>();
		pencil.UsesSinceSharpening = 0;
		actor.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.SharpenEmote, actor, actor, Parent,
			otherItem)));
		return true;
	}

	#endregion
}