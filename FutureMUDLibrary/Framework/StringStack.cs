using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Framework {
    public class StringStack {
        private static readonly char[] _separators = {' '};
        private static readonly char[] _speechSeparator = {'\"', '“', '”'};
        private static readonly char[] _closingParenthesis = {')'};

        private readonly List<string> _memory = new();

        public StringStack(string argument) {
            RemainingArgument = argument;
            IsFinished = RemainingArgument.Length == 0;
        }

        public IEnumerable<string> Memory => _memory;

        public string Last => _memory.LastOrDefault();

        public bool IsFinished { get; private set; }

        public string RemainingArgument { get; private set; }

        public string this[int i]
        {
            get
            {
                if (i <= -1) {
                    return null;
                }
                if (_memory.Count > i) {
                    return _memory[i];
                }
                throw new IndexOutOfRangeException("StringStack access index out of StringStack memory range.");
            }
        }

        public string SafeRemainingArgument {
            get {
                var result = new List<char>(RemainingArgument.Length);
                var open = false;
                for (var i = 0; i < RemainingArgument.Length; i++)
                {
                    if (!open && RemainingArgument[i].In('"', '“'))
                    {
                        open = true;
                        continue;
                    }

                    if (open && RemainingArgument[i].In('"', '”'))
                    {
                        open = false;
                        continue;
                    }

                    result.Add(RemainingArgument[i]);
                }

                return new string(result.ToArray()).Trim();
            }
        }

        /// <summary>
        /// Performs a similar algorithm to PopSpeech but doesn't pop if the next argument matches a parenthesis
        /// </summary>
        /// <returns></returns>
        public string PopSafe()
        {
            if (IsFinished || RemainingArgument.Length <= 0)
            {
                return string.Empty;
            }

            if (RemainingArgument.StartsWith("(", StringComparison.Ordinal))
            {
                if (RemainingArgument.IndexOfAny(_closingParenthesis) != -1)
                {
                    return string.Empty;
                }
            }

            return PopSpeech();
        }

        // TODO: Need to trim off pop?
        public string Pop() {
            if ((RemainingArgument.Length > 0) && !IsFinished) {
                var splitResult = RemainingArgument.Split(_separators, 2, StringSplitOptions.RemoveEmptyEntries);

                if (splitResult.Length > 1) {
                    RemainingArgument = splitResult[1];
                }
                else if (splitResult.Length == 1) {
                    RemainingArgument = string.Empty;
                    IsFinished = true;
                }
                else {
                    RemainingArgument = string.Empty;
                    return string.Empty;
                }

                _memory.Add(splitResult[0]);
                return splitResult[0];
            }
            IsFinished = true;
            return string.Empty;
        }

        public string Peek() {
            return (RemainingArgument.Length > 0) && !IsFinished
                ? RemainingArgument.Split(_separators, 2, StringSplitOptions.RemoveEmptyEntries)[0]
                : string.Empty;
        }

        public string PopSpeech() {
            if ((RemainingArgument.Length > 0) && !IsFinished && _speechSeparator.Contains(RemainingArgument[0])) {
                var splitResult = RemainingArgument.Split(_speechSeparator, 3);

                if (splitResult.Length > 2) {
                    RemainingArgument = splitResult[2].TrimStart(_separators);
                }
                else {
                    IsFinished = true;
                    RemainingArgument = string.Empty;
                    return string.Empty;
                }

                IsFinished = !RemainingArgument.Any();
                _memory.Add(splitResult[1]);
                return splitResult[1];
            }
            return Pop();
        }

        /// <summary>
        /// Equivalent to PopSpeech().ToLowerInvariant().CollapseString(). Used commonly with switch statements for building commands.
        /// </summary>
        /// <returns></returns>
        public string PopForSwitch()
        {
	        return PopSpeech().ToLowerInvariant().CollapseString();
        }

        public string PeekSpeech() {
            return (RemainingArgument.Length > 0) && !IsFinished && RemainingArgument.StartsWith("\"", StringComparison.Ordinal)
                ? RemainingArgument.Split(_speechSeparator, 3)[1]
                : Peek();
        }

        public string PopParentheses() {
            if ((RemainingArgument.Length > 0) && !IsFinished && RemainingArgument.StartsWith("(", StringComparison.Ordinal)) {
                var splitResult = RemainingArgument.Split(_closingParenthesis, 2);

                if (splitResult.Length > 1) {
                    RemainingArgument = splitResult[1].TrimStart(_separators);
                }
                else {
                    return string.Empty;
                }

                IsFinished = !RemainingArgument.Any();
                splitResult[0] = splitResult[0].RemoveFirstCharacter();
                _memory.Add(splitResult[0]);
                return splitResult[0];
            }
            return string.Empty;
        }

        public string PeekParentheses() {
            return (RemainingArgument.Length > 0) && !IsFinished && RemainingArgument.StartsWith("(", StringComparison.Ordinal)
                ? RemainingArgument.Split(_closingParenthesis, 2)[0].RemoveFirstCharacter()
                : string.Empty;
        }

        public IEnumerable<string> PopAll() {
            while (!IsFinished) {
                Pop();
            }

            return Memory;
        }

        public IEnumerable<string> PopSpeechAll() {
            while (!IsFinished) {
                PopSpeech();
            }

            return Memory;
        }

        //Breaks remaining argument up into individual parameters of either quoted text or single words and returns
        //the number of them found.
        public int CountRemainingArguments()
        {
            var count = 0;

            var args = new StringStack(RemainingArgument);

            while (args.IsFinished == false)
            {
                if (args.RemainingArgument.StartsWith("\"", StringComparison.Ordinal))
                {
                    args.PopSpeech();
                    ++count;
                    continue;
                }

                args.Pop();
                ++count;
            }

            return count;
        }

        /// <summary>
        /// Returns a new StringStack object that undoes that last Pop
        /// </summary>
        /// <returns></returns>
        public StringStack GetUndo()
        {
            if (_memory.Any())
            {
                return new StringStack($"\"{Last}\" {RemainingArgument}");
            }

            return new StringStack(RemainingArgument);
        }
    }
}