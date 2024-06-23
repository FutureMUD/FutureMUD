using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Events.Hooks
{
	public class GPTHook : HookBase
	{
		public static void RegisterLoader()
		{
			HookLoaders.Add("GPTHook", (hook, gameworld) => new GPTHook(hook, gameworld));
		}

		/// <inheritdoc />
		protected GPTHook(Models.Hooks hook, IFuturemud gameworld) : base(hook, gameworld)
		{
		}

		/// <inheritdoc />
		public GPTHook(IFuturemud gameworld, EventType type, string name, IFutureProg prog, long gptThreadId, int maximumHistory) : base(gameworld, type)
		{
			_name = name;
			Category = "Uncategorised";
			ExecuteProgWithResults = prog;
			GPTThreadId = gptThreadId;
			MaximumHistory = maximumHistory;
			using (new FMDB())
			{
				var dbitem = new Models.Hooks();
				FMDB.Context.Hooks.Add(dbitem);
				dbitem.Name = name;
				dbitem.Definition = new XElement("Definition", new XElement("Prog", prog.Id), new XElement("Thread", GPTThreadId), new XElement("History", MaximumHistory)).ToString();
				dbitem.Category = "Uncategorised";
				dbitem.TargetEventType = (int)type;
				dbitem.Type = "GPTHook";
				FMDB.Context.SaveChanges();
				_id = dbitem.Id;
			}

			Gameworld.Add(this);
		}

		/// <inheritdoc />
		protected override XElement SaveDefinition()
		{
			return new XElement("Definition",
				new XElement("Prog", ExecuteProgWithResults.Id),
				new XElement("Thread", GPTThreadId),
				new XElement("History", MaximumHistory)
			);
		}

		public int MaximumHistory { get; protected set; }

		public long GPTThreadId { get; protected set; }

		public IFutureProg ExecuteProgWithResults { get; protected set; }

		#region Overrides of HookBase

		/// <inheritdoc />
		public override Func<EventType, object[], bool> Function { get; }

		/// <inheritdoc />
		public override string InfoForHooklist { get; }

		#endregion
	}
}
