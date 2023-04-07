using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace $rootnamespace$.Components
{
    public class $safeitemrootname$ : GameItemComponent
    {
		protected $safeitemrootname$Proto _prototype;
		public override IGameItemComponentProto Prototype => _prototype;
		
		protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto){
			_prototype = ($safeitemrootname$Proto)newProto;
		}
		
		#region Constructors
		public $safeitemrootname$($safeitemrootname$Proto proto, IGameItem parent, bool temporary = false) : base(parent, proto, temporary){
			_prototype = proto;
		}
		
		public $safeitemrootname$(Models.GameItemComponent component, $safeitemrootname$Proto proto, IGameItem parent) : base(component, parent) {
			_prototype = proto;
			_noSave = true;
			LoadFromXml(XElement.Parse(component.Definition));
			_noSave = false;
		}
		
		public $safeitemrootname$($safeitemrootname$ rhs, IGameItem newParent, bool temporary = false) : base(rhs, newParent, temporary) {
			_prototype = rhs._prototype;
		}
		
		protected void LoadFromXml(XElement root){
			// TODO
		}
		
		public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false){
			return new $safeitemrootname$(this, newParent, temporary);
		}
		#endregion
		
		#region Saving
		protected override string SaveToXml(){
			return new XElement("Definition").ToString();
		}
		#endregion
    }
}
