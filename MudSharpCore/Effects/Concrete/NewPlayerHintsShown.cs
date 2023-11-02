using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete
{
	public class NewPlayerHintsShown : Effect, IEffect
	{
		#region Static Initialisation
		public static void InitialiseEffectType()
		{
			RegisterFactory("NewPlayerHintsShown", (effect, owner) => new NewPlayerHintsShown(effect, owner));
		}
		#endregion

		#region Constructors
		public NewPlayerHintsShown(ICharacter owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg)
		{
		}

		protected NewPlayerHintsShown(XElement effect, IPerceivable owner) : base(effect, owner)
		{
			var root = effect.Element("Effect");
			LastHintShown = DateTime.FromOADate(double.Parse(root.Element("LastHintShown").Value));
			foreach (var element in root.Element("ShownHintIDs").Elements("Id"))
			{
				ShownHintIds.Add(long.Parse(element.Value));
			}
		}
		#endregion

		// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
		#region Saving and Loading
		protected override XElement SaveDefinition()
		{
			return new XElement("Effect",
				new XElement("LastHintShown", LastHintShown.ToOADate()),
				new XElement("ShownHintIDs",
					from id in ShownHintIds
					select new XElement("Id", id)
				)	
			);
		}
		#endregion

		#region Overrides of Effect
		protected override string SpecificEffectType => "NewPlayerHintsShown";

		public override string Describe(IPerceiver voyeur)
		{
			return $"Player has been shown {ShownHintIds.Count.ToString("N0", voyeur)} new player {"hint".Pluralise(ShownHintIds.Count != 1)}";
		}

		public override bool SavingEffect => true;
		#endregion

		public HashSet<long> ShownHintIds { get; } = new();
		public DateTime LastHintShown { get; set; }
	}
}
