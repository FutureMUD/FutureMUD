using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace $rootnamespace$.Prototypes
{
    public class $safeitemrootname$ : GameItemComponentProto {
		public override string TypeDescription => "$fileinputname$";
		
		#region Constructors
		protected $safeitemrootname$(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "$fileinputname$") {
		}
		
		protected $safeitemrootname$(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld){
		}
		
		protected override void LoadFromXml(XElement root){
			// TODO
		}
		#endregion
		
		#region Saving
		protected override string SaveToXml(){
			return new XElement("Definition",
					new XElement("Example")
				).ToString();
		}
		#endregion
		
		#region Component Instance Initialising Functions
		public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false){
			return new $fileinputname$GameItemComponent(this, parent, temporary);
		}
		
		public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent){
			return new $fileinputname$GameItemComponent(component, this, parent);
		}
		#endregion
		
		#region Initialisation Tasks
		public static void RegisterComponentInitialiser(GameItemComponentManager manager){
			manager.AddBuilderLoader("$fileinputname$".ToLowerInvariant(), true, (gameworld, account) => new $safeitemrootname$(gameworld,account));
			manager.AddDatabaseLoader("$fileinputname$", (proto, gameworld) => new $safeitemrootname$(proto, gameworld));
			manager.AddTypeHelpInfo(
				"$fileinputname$", 
				$"A short description of what the item type does", 
				BuildingHelpText
			);
		}
		
		public override IEditableItem CreateNewRevision(ICharacter initiator){
			return CreateNewRevision(initiator, (proto, gameworld) => new $safeitemrootname$(proto, gameworld));
		}
		#endregion
		
		#region Building Commands

private const string BuildingHelpText = @"You can use the following options with this component:

	<name> - sets the name of the component
	desc <desc> - sets the description of the component";
		public override string ShowBuildingHelp => BuildingHelpText;
		
		public override bool BuildingCommand(ICharacter actor, StringStack command) {
			switch (command.PopSpeech().ToLowerInvariant().CollapseString()){
				default:
					return base.BuildingCommand(actor, command);
			}
		}
		#endregion
		
		public override string ComponentDescriptionOLC(ICharacter actor) {
            return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item needs a description.",
                "$fileinputname$ Game Item Component".Colour(Telnet.Cyan),
                ID,
                RevisionNumber,
                Name
                );
        }
    }
}
