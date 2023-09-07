using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.ScriptedEvents
{
	internal class ScriptedEventMultipleChoiceQuestionAnswer : SaveableItem, IScriptedEventMultipleChoiceQuestionAnswer
	{
		public ScriptedEventMultipleChoiceQuestionAnswer(Models.ScriptedEventMultipleChoiceQuestionAnswer dbitem, IFuturemud gameworld)
		{
			Gameworld= gameworld;
			_id = dbitem.Id;
			AfterChoiceProg = Gameworld.FutureProgs.Get(dbitem.AfterChoiceProgId ?? 0L);
			AnswerFilterProg = Gameworld.FutureProgs.Get(dbitem.AnswerFilterProgId ?? 0L);
			DescriptionAfterChoice = dbitem.DescriptionAfterChoice;
			DescriptionBeforeChoice = dbitem.DescriptionBeforeChoice;
		}
		public override void Save()
		{
			var dbitem = FMDB.Context.ScriptedEventMultipleChoiceQuestionAnswers.Find(Id);
			dbitem.DescriptionBeforeChoice = DescriptionBeforeChoice;
			dbitem.DescriptionAfterChoice = DescriptionAfterChoice;
			dbitem.AnswerFilterProgId = AnswerFilterProg?.Id;
			dbitem.AfterChoiceProgId = AfterChoiceProg?.Id;
			Changed = false;
		}

		public override string FrameworkItemType { get; }
		public string DescriptionBeforeChoice { get; }
		public string DescriptionAfterChoice { get; }
		public IFutureProg AnswerFilterProg { get; }
		public IFutureProg AfterChoiceProg { get; }

		public bool BuildingCommand(ICharacter actor, StringStack command)
		{
			throw new NotImplementedException();
		}

		public string Show(ICharacter actor)
		{
			throw new NotImplementedException();
		}
	}
}
