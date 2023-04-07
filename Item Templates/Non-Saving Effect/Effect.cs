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
		public $safeitemrootname$(IPerceivable owner, IFutureProg applicabilityProg = null) : base(owner, applicabilityProg) {
		}
		
		protected override string SpecificEffectType => "$fileinputname$";
		
		public override string Describe(IPerceiver voyeur){
			return "An undescribed effect of type $fileinputname$.";
		}
    }
}
