using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class BombGameItemComponent : GameItemComponent, IDetonatable
{
	protected BombGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (BombGameItemComponentProto)newProto;
	}

	#region Constructors

	public BombGameItemComponent(BombGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
	}

	public BombGameItemComponent(MudSharp.Models.GameItemComponent component, BombGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BombGameItemComponent(BombGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	protected void LoadFromXml(XElement root)
	{
		// TODO
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BombGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	#endregion

	public bool Detonate()
	{
		// The first thing we need to do is work out where we are, in terms of proximity
		var proximitything = Parent.LocationLevelPerceivable;
		if (proximitything?.Location == null)
		{
			throw new ApplicationException($"Bomb exploded in the ether.");
		}

		var damages = _prototype.Damages.Select(x =>
		{
			x.DamageExpression.Parameters["quality"] = (int)Parent.Quality;
			x.StunExpression.Parameters["quality"] = (int)Parent.Quality;
			var finalDamage = Convert.ToDouble(x.DamageExpression.Evaluate());
			return new Damage
			{
				DamageType = x.DamageType,
				DamageAmount = finalDamage,
				PainAmount = finalDamage,
				StunAmount = Convert.ToDouble(x.StunExpression.Evaluate())
			};
		}).ToList();

		// Echoes
		proximitything.OutputHandler.Handle(new EmoteOutput(new Emote(_prototype.ExplosionEmoteText, Parent, Parent),
			style: OutputStyle.Explosion));
		var vicinity = proximitything.CellsInVicinity((uint)_prototype.ExplosionVolume, false, false)
		                             .Except(proximitything.Location);
		foreach (var location in vicinity)
		{
			if (location.Characters.Any() || location.GameItems.Any())
			{
				var directions = location.ExitsBetween(proximitything.Location, 10).ToList();
				location.Handle(new EmoteOutput(
					new Emote($"An explosion can be heard {directions.DescribeDirectionsToFrom()}.", Parent),
					flags: OutputFlags.PurelyAudible | OutputFlags.IgnoreWatchers));
			}
		}

		// Damage Dealing
		var wounds = new List<IWound>();
		IExplosiveDamage damage;
		if (Parent.ContainedIn != null)
		{
			damage = new ExplosiveDamage(damages, 0.0, _prototype.ExplosionSize, _prototype.MaximumProximity, true,
				Parent);
			wounds.AddRange(Parent.ContainedIn.PassiveSufferDamageViaContainedItem(damage, Proximity.Intimate,
				Body.Facing.Front, Parent));
		}
		else if (Parent.InInventoryOf != null)
		{
			damage = new ExplosiveDamage(damages, 0.0, _prototype.ExplosionSize, _prototype.MaximumProximity, false,
				null);
			wounds.AddRange(Parent.InInventoryOf.InventoryExploded(Parent, damage));
		}
		else
		{
			damage = new ExplosiveDamage(damages, 0.0, _prototype.ExplosionSize, _prototype.MaximumProximity, false,
				null);
			wounds.AddRange(Parent.ExplosionEmantingFromPerceivable(damage));
		}

		wounds.ProcessPassiveWounds();
		// Delete the Bomb
		Parent.Delete();
		return true;
	}
}