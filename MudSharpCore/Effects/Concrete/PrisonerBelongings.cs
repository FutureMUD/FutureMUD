using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class PrisonerBelongings : Effect, IEffect, IRemoveOnGet
{
	public long CharacterOwnerId { get; }

	#region Static Initialisation

	public static void InitialiseEffectType()
	{
		RegisterFactory("PrisonerBelongings", (effect, owner) => new PrisonerBelongings(effect, owner));
	}

	#endregion

	#region Constructors

	public PrisonerBelongings(IGameItem owner, ICharacter characterOwner) : base(owner, null)
	{
		CharacterOwnerId = characterOwner.Id;
	}

	protected PrisonerBelongings(XElement effect, IPerceivable owner) : base(effect, owner)
	{
		var root = effect.Element("Effect");
		CharacterOwnerId = long.Parse(root.Attribute("owner").Value);
	}

	#endregion

	// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)

	#region Saving and Loading

	protected override XElement SaveDefinition()
	{
		return new XElement("Effect", new XAttribute("owner", CharacterOwnerId));
	}

	#endregion

	#region Overrides of Effect

	protected override string SpecificEffectType => "PrisonerBelongings";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Held belongings of prisoner #{CharacterOwnerId.ToString("N0", voyeur)}";
	}

	public override bool SavingEffect => true;

	public override bool Applies(object target)
	{
		if (target is ICharacter tch)
		{
			return tch.Id == CharacterOwnerId;
		}

		return base.Applies(target);
	}

	#endregion
}