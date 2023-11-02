using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.RPG.Hints
{
	public class NewPlayerHint : SaveableItem, INewPlayerHint
	{
		public NewPlayerHint(Models.NewPlayerHint hint, IFuturemud gameworld)
		{
			Gameworld = gameworld;
			_id = hint.Id;
			_name = $"Hint #{Id:N0}";
			Priority = hint.Priority;
			CanRepeat = hint.CanRepeat;
			FilterProg = Gameworld.FutureProgs.Get(hint.FilterProgId ?? 0L);
			Text = hint.Text;
		}

		public NewPlayerHint(IFuturemud gameworld, string text)
		{
			Gameworld = gameworld;
			Text = text;
			CanRepeat = false;
			FilterProg = Gameworld.AlwaysFalseProg;
			Priority = 0;
			using (new FMDB())
			{
				var dbitem = new Models.NewPlayerHint
				{
					CanRepeat = CanRepeat,
					Text = text,
					FilterProgId = FilterProg?.Id,
					Priority = Priority
				};
				FMDB.Context.NewPlayerHints.Add(dbitem);
				FMDB.Context.SaveChanges();
				_id = dbitem.Id;
				_name = $"Hint #{Id:N0}";
			}
		}

		public string Text { get; private set; }
		public IFutureProg FilterProg { get; private set; }
		public int Priority { get; private set; }
		public bool CanRepeat { get; private set; }

		public const string BuildingHelp = @"You can use the following options with this command:

	#3repeat#0 - toggles whether this hint can be repeated or only fires once
	#3priority <##>#0 - sets a priority for order shown. Higher priorities are shown first
	#3filter <prog>#0 - sets a prog to filter whether this hint will be shown
	#3text#0 - drops you into an editor to change the hint text";

		public bool BuildingCommand(ICharacter actor, StringStack command)
		{
			switch (command.PopSpeech().ToLowerInvariant().CollapseString())
			{
				case "repeat":
				case "repeats":
				case "canrepeat":
					return BuildingCommandCanRepeat(actor);
				case "priority":
					return BuildingCommandPriority(actor, command);
				case "prog":
				case "filter":
				case "filterprog":
				case "progfilter":
					return BuildingCommandFilterProg(actor, command);
				case "text":
					return BuildingCommandText(actor);
				default:
					actor.OutputHandler.Send(BuildingHelp.SubstituteANSIColour());
					return false;
			}
		}

		private bool BuildingCommandText(ICharacter actor)
		{
			if (!string.IsNullOrEmpty(Text))
			{
				actor.OutputHandler.Send($"Replacing:\n\n{Text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}\n");
			}

			actor.OutputHandler.Send("Enter the text in the editor below.");
			actor.EditorMode(BuildingCommandTextPost, BuildingCommandTextCancel, 1.0, null, EditorOptions.None,
				new object[] { actor });
			return true;
		}

		private void BuildingCommandTextCancel(IOutputHandler handler, object[] parameters)
		{
			handler.Send("You decide not to alter the text of the hint.");
		}

		private void BuildingCommandTextPost(string text, IOutputHandler handler, object[] parameters)
		{
			Text = text;
			Changed = true;
			var actor = (ICharacter)parameters[0];
			handler.Send($"You change the text of this hint to the following:\n\n{text.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength, "\t")}");
		}

		private bool BuildingCommandFilterProg(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What prog would you like to use as a filter prog for this hint?");
				return false;
			}

			var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Boolean, new[] { FutureProgVariableTypes.Character }).LookupProg();
			if (prog is null)
			{
				return false;
			}

			FilterProg = prog;
			Changed = true;
			actor.OutputHandler.Send($"This hint will now use the prog {prog.MXPClickableFunctionName()} to filter who sees it.");
			return true;
		}

		private bool BuildingCommandPriority(ICharacter actor, StringStack command)
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("What priority do you want this hint to be shown? Higher priority hints will be shown first.");
				return false;
			}

			if (!int.TryParse(command.SafeRemainingArgument, out var value))
			{
				actor.OutputHandler.Send("You must enter a valid number.");
				return false;
			}

			Priority = value;
			Changed = true;
			actor.OutputHandler.Send($"This hint is now at priority #{value.ToString("N0", actor).ColourValue()}, which means it is currently {((Gameworld.NewPlayerHints.OrderByDescending(x => x.Priority).ThenByDescending(x => x.Id).ToList().IndexOf(this))+1).ToOrdinal().ColourValue()} to be shown.");
			return true;
		}

		private bool BuildingCommandCanRepeat(ICharacter actor)
		{
			CanRepeat = !CanRepeat;
			Changed = true;
			actor.OutputHandler.Send($"This hint will {(!CanRepeat).NowNoLonger()} be shown only one time to new players.");
			return true;
		}

		public string Show(ICharacter actor)
		{
			var sb = new StringBuilder();
			sb.AppendLine($"New Player Hint #{Id.ToString("N0", actor)}".GetLineWithTitle(actor, Telnet.Green, Telnet.BoldWhite));
			sb.AppendLine();
			sb.AppendLine($"Filter Prog: {FilterProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
			sb.AppendLine($"Can Repeat: {CanRepeat.ToColouredString()}");
			sb.AppendLine($"Priority: {Priority.ToString("N0", actor).ColourValue()}");
			sb.AppendLine();
			sb.AppendLine("Text:");
			sb.AppendLine();
			sb.AppendLine($"#G[Hint]#0 {Text}".SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
			return sb.ToString();
		}

		public override void Save()
		{
			var dbitem = FMDB.Context.NewPlayerHints.Find(Id);
			dbitem.Text = Text;
			dbitem.Priority = Priority;
			dbitem.FilterProgId = FilterProg?.Id;
			dbitem.CanRepeat = CanRepeat;
			Changed = false;
		}

		public sealed override string FrameworkItemType => "NewPlayerHint";
	}
}
