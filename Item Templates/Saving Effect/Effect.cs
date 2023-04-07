using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
$if$ ($targetframeworkversion$ >= 4.5)using System.Threading.Tasks;
$endif$
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace $rootnamespace$.Concrete
{
    public class $safeitemrootname$ : Effect, IEffect {
		#region Static Initialisation
		public static void InitialiseEffectType() {
            RegisterFactory("$safeitemrootname$", (effect, owner) => new $safeitemrootname$(effect, owner));
        }
		#endregion
		
		#region Constructors
		public $safeitemrootname$(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg) {
		}
		
		protected $safeitemrootname$(XElement effect, IPerceivable owner) : base(effect, owner) {
			var root = effect.Element("Effect");
        }
		#endregion
		
		// Note: You can safely delete this entire region if your effect acts more like a flag and doesn't actually save any specific data on it (e.g. immwalk, admin telepathy, etc)
		#region Saving and Loading
		protected override XElement SaveDefinition()
        {
            return SaveToXml(new XElement("Example", 0));
        }
		#endregion
		
		#region Overrides of Effect
		protected override string SpecificEffectType => "$safeitemrootname$";
		
		public override string Describe(IPerceiver voyeur){
			return "An undescribed effect of type $fileinputname$.";
		}
		
		public override bool SavingEffect => true;
		#endregion
    }
}
