using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete
{
	public class ShopStallNoGetEffect : Effect, IEffect, INoGetEffect
	{
		#region Static Initialisation
		public static void InitialiseEffectType()
		{
			RegisterFactory("ShopStallNoGetEffect", (effect, owner) => new ShopStallNoGetEffect(effect, owner));
		}
		#endregion

		#region Constructors
		public ShopStallNoGetEffect(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
		{
		}

		protected ShopStallNoGetEffect(XElement effect, IPerceivable owner) : base(effect, owner)
		{
		}
		#endregion

		#region Overrides of Effect
		protected override string SpecificEffectType => "ShopStallNoGetEffect";

		public override string Describe(IPerceiver voyeur)
		{
			return "Shop stall can't be picked up while it is trading";
		}

		public override bool SavingEffect => true;

		public bool CombatRelated => false;
		#endregion
	}
}
