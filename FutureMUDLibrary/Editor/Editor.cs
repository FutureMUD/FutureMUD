using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp.Editor {
	public class Editor : IEditor {
		private readonly Action<IOutputHandler, object[]> _cancelAction;
		private readonly List<string> _lines = new();
		private readonly EditorOptions _options;
		private readonly Action<string, IOutputHandler, object[]> _postAction;
		private readonly object[] _suppliedArguments;
		private int _insertPosition = -1;
		public double CharacterLengthMultplier { get; set; }
		private readonly string _recallText;

		public Editor(object[] suppliedArguments, Action<string, IOutputHandler, object[]> postAction,
			Action<IOutputHandler, object[]> cancelAction, EditorOptions options, double lengthMultiplier, string recallText) {
			_suppliedArguments = suppliedArguments;
			_postAction = postAction;
			_cancelAction = cancelAction;
			_options = options;
			Status = EditorStatus.Editing;
			CharacterLengthMultplier = lengthMultiplier;
			_recallText = recallText;
		}

		#region IEditor Members

		public EditorStatus Status { get; private set; }
		public string FinalText { get; private set; }

		#endregion

		#region IHandleCommands Members

		public void HandleCommand(string command) {
			 if (command.Length > 0) {
				if (command[0] == '*') {
					var ss = new StringStack(command.RemoveFirstCharacter());
					switch (ss.Pop().ToLowerInvariant()) {
						case "len":
						case "length":
						case "chars":
							var charCount = (int)(_lines.Sum(x => x.RawTextLength()) * CharacterLengthMultplier);
							// Have to factor in the newlines that will end up in the final doc
							charCount += _lines.Count == 0 ? 0 : ((int)(_lines.Count - 1 * CharacterLengthMultplier));
							OutputHandler.Send($"Current effective length is {charCount} characters.");
							return;
						case "show":
							var line = 1;
							OutputHandler.Send(_lines.Select(x => $"{line++}> {x}")
								.ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: ""));
							return;
						case "recall":
							if (string.IsNullOrEmpty(_recallText)) {
								OutputHandler.Send("You don't have any stored text to recall.");
								return;
							}

							_lines.Clear();
							_insertPosition = -1;
							foreach (var addline in _recallText.Split('\n')) {
								_lines.Add(addline);
							}
							OutputHandler.Send($"You have recalled {_lines.Count} line{(_lines.Count == 1 ? "" : "s")} and {_lines.Sum(x => x.Length + 1) - 1} character{(_lines.Sum(x => x.Length + 1) - 1 == 1 ? "" : "s")} of stored text.");
							return;
						case "delete":
							if (!int.TryParse(ss.Pop(), out var value)) {
								OutputHandler.Send("You must supply a line number to delete.");
								return;
							}

							if (value < 1) {
								OutputHandler.Send("The line number must be a positive number.");
								return;
							}

							if (value > _lines.Count) {
								OutputHandler.Send("There is no such line to delete.");
								return;
							}

							if (int.TryParse(ss.Pop(), out var value2)) {
								if (value2 <= value) {
									OutputHandler.Send("The second line must be after the first line.");
									return;
								}

								if (value2 > _lines.Count) {
									OutputHandler.Send("There is no such line to delete.");
									return;
								}

								_lines.RemoveRange(value - 1, value2 - value);
								_insertPosition = -1;
								OutputHandler.Send("You delete lines " + value + " to " + value2 + ".");
								return;
							}
							OutputHandler.Send("You delete line " + value +
											   _lines[value - 1].SquareBrackets().LeadingSpaceIfNotEmpty());
							_lines.RemoveAt(value - 1);
							_insertPosition = -1;
							return;
						case "clear":
							_lines.Clear();
							_insertPosition = -1;
							OutputHandler.Send("You delete all the lines.");
							return;
						case "cancel":
							if (_options.HasFlag(EditorOptions.DenyCancel)) {
								OutputHandler.Send(
									"You are not permitted to cancel this document. You must submit with @.");
								return;
							}

							if (_cancelAction is not null)
							{
								_cancelAction(OutputHandler, _suppliedArguments);
							}
							
							Status = EditorStatus.Cancelled;
							return;
						case "goto":
							if (ss.Peek().ToLowerInvariant() == "end") {
								_insertPosition = -1;
								OutputHandler.Send("Now inserting text at end of document.");
								return;
							}

							if (!int.TryParse(ss.Pop(), out value)) {
								OutputHandler.Send("You must supply a line number to goto.");
								return;
							}

							if (value > _lines.Count) {
								OutputHandler.Send("There is no such line to goto.");
								return;
							}

							if (value < 1) {
								OutputHandler.Send("You must enter a positive number.");
								return;
							}

							_insertPosition = value;
							OutputHandler.Send("Now inserting text before line " + _insertPosition +
											   _lines[_insertPosition - 1].SquareBrackets().LeadingSpaceIfNotEmpty());
							return;

						case "help":
							OutputHandler.Send(
								$@"Valid commands include:

{"*show".Colour(Telnet.Cyan)}: shows what has been entered thus far.
{"*delete <linenum>".Colour(Telnet.Cyan)}: deletes the specified line.
{"*delete <start linenum> <end linenum>".Colour(Telnet.Cyan)}: deletes all lines between the two specified numbers.
{"*clear".Colour(Telnet.Cyan)}: deletes all input.
{"*cancel".Colour(Telnet.Cyan)}: cancels the current editor.
{"*goto <linenum>".Colour(Telnet.Cyan)}: begins inserting text before the specified line.
{"*goto end".Colour(Telnet.Cyan)}: returns to inserting text at the end of the document.
{"*length".Colour(Telnet.Cyan)}: shows the effective length of the current document.
{"*recall".Colour(Telnet.Cyan)}: recalls text that you previously entered in an approved editor within the last 30 minutes."
							);
							return;
						default:
							OutputHandler.Send("That is not a valid command. See *help for more information.");
							return;
					}
				}

				if (command == "@") {
					if ((_lines.Count == 0) ||
						((_lines.Sum(x => x.Length) == 0) && !_options.HasFlag(EditorOptions.PermitEmpty))) {
						OutputHandler.Send("You cannot submit without any text.");
						return;
					}

					FinalText = _lines.ListToString(separator: "\n", twoItemJoiner: "\n", conjunction: "");
					if (_postAction is not null)
					{
						
						_postAction(FinalText, OutputHandler, _suppliedArguments);
					}
					
					Status = EditorStatus.Submitted;
					return;
				}
			}

			if (_insertPosition == -1) {
				_lines.Add(command);
			}
			else {
				_lines.Insert(_insertPosition - 1, command);
			}
			OutputHandler.SendPrompt();
		}

		#endregion

		#region IHandleOutput Members

		public IOutputHandler OutputHandler { get; private set; }

		public void Register(IOutputHandler handler) {
			OutputHandler = handler;
		}

		#endregion
	}
}