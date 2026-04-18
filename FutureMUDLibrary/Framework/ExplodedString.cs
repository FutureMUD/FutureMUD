using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Framework
{
    /// <summary>
    ///     An exploded string is a string that has been exploded into words as specified by particular word boundaries. It
    ///     exposes those words so they can be altered, and can recombine them including their original joiners.
    /// </summary>
    public class ExplodedString
    {
        private static readonly HashSet<char> _bounds = new(new char[] { ' ', ',', '.', ';', '!', '?', ':', '\'', '\"', '-' });
        private readonly string _rawText;
        protected List<string> Joiners;

        public ExplodedString(string text)
        {
            Words = new List<string>();
            Joiners = new List<string>();
            SentenceLengths = new List<int>();
            _rawText = text;

            Explode();
        }

        public List<string> Words { get; protected set; }

        public List<int> SentenceLengths { get; protected set; }

        private void Explode()
        {
            bool inWord = true;
            int lastBoundary = 0;
            int currentSentenceWords = 0;

            if (_rawText.Length == 0)
            {
                return;
            }

            for (int i = 0; i < _rawText.Length; i++)
            {
                bool isWord = !_bounds.Contains(_rawText[i]);
                if (isWord != inWord)
                {
                    if (i - lastBoundary > 0)
                    {
                        if (isWord)
                        {
                            Joiners.Add(_rawText[lastBoundary..i]);
                        }
                        else
                        {
                            Words.Add(_rawText[lastBoundary..i]);
                            currentSentenceWords++;
                            if (Constants.SentenceEndingCharacters.Contains(_rawText[i]))
                            {
                                SentenceLengths.Add(currentSentenceWords);
                                currentSentenceWords = 0;
                            }
                        }

                        lastBoundary = i;
                    }
                }

                inWord = isWord;
            }

            if (inWord)
            {
                Words.Add(_rawText[lastBoundary..]);
                SentenceLengths.Add(++currentSentenceWords);
            }
            else
            {
                Joiners.Add(_rawText[lastBoundary..]);
            }

            if (SentenceLengths.Count == 0)
            {
                SentenceLengths.Add(currentSentenceWords);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < Words.Count; i++)
            {
                sb.Append(Words.Count > i ? Words[i] : "");
                sb.Append(Joiners.Count > i ? Joiners[i] : "");
            }

            return sb.ToString();
        }
    }
}