using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Components;

public class FlareAmmunitionGameItemComponent : AmmunitionGameItemComponent
{
	protected FlareAmmunitionGameItemComponentProto _flarePrototype;
	public override IGameItemComponentProto Prototype => _flarePrototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_flarePrototype = (FlareAmmunitionGameItemComponentProto)newProto;
	}

	#region Constructors

	public FlareAmmunitionGameItemComponent(FlareAmmunitionGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_flarePrototype = proto;
	}

	public FlareAmmunitionGameItemComponent(MudSharp.Models.GameItemComponent component,
		FlareAmmunitionGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_flarePrototype = proto;
	}

	public FlareAmmunitionGameItemComponent(FlareAmmunitionGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_flarePrototype = rhs._flarePrototype;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new FlareAmmunitionGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	#region IAmmo Implementation

	public override void Fire(ICharacter actor, IPerceiver target, Outcome shotOutcome, Outcome coverOutcome,
		OpposedOutcome defenseOutcome, IBodypart bodypart, IGameItem ammo, IRangedWeaponType weaponType,
		IEmoteOutput defenseEmote)
	{
		if (target == null && actor.Location.CurrentOverlay.OutdoorsType == CellOutdoorsType.Outdoors)
		{
			// Firing at the sky
			actor.Location.Zone.AddEffect(
				new FlareEffect(actor.Location.Zone, _flarePrototype.FlareIllumination,
					_flarePrototype.FlareZoneDescription, _flarePrototype.FlareZoneDescriptionColour,
					_flarePrototype.FlareEndEmote), _flarePrototype.FlareDuration);
			actor.Location.Zone.RecalculateLightLevel();

			var emote = new EmoteOutput(new Emote(_flarePrototype.FlareBeginEmote, actor));
			foreach (var cell in actor.Location.Zone.Cells.Where(x =>
				         x.CurrentOverlay.OutdoorsType == CellOutdoorsType.Outdoors))
			{
				cell.Handle(emote);
			}
		}

		base.Fire(actor, target, shotOutcome, coverOutcome, defenseOutcome, bodypart, ammo, weaponType, defenseEmote);
	}

	#endregion
}