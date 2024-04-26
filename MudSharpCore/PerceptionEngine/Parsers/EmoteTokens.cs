using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.PerceptionEngine.Parsers;

public partial class Emote
{
	private const char ObjectiveCharacterDelimiter = '~';
	private const char ObjectiveObjectDelimiter = '*';

	private const char SubjectiveDelimiter = '#';
	private const char PronounDelimiter = '!';
	private const char ReflexiveDelimiter = '?';

	private const char ReferenceDelimiter = '$';
	private const char ReferencePronounDelimiter = '&';


	private const char FirstThirdDelimiter = '|';

	private const char PersonalDescriptionDelimiter = '@';

	/// <summary>
	///     The internal token is used by in-game messages to send output referring to things. Forms are:
	///     $0 - a man / you
	///     $0's - a man's / your
	///     $0|one|two - one / two
	///     &0|stop|stops - stop / stops depending on plurality of $0 token
	///     %0|stop|stops  -stop / stops depending on the plurality of the &0 token, e.g. You stop, he/she/it stops, they stop
	///     one|two - one / two
	///     &0 - him / you
	///     #0 - he / you
	///     &0's - his / your
	///     %0 - himself / yourself
	///     !0 - man / you
	///     !0's man's / your
	/// </summary>
	private static readonly Regex InternalTokenRegex = new(@"(?<=([!.?:] |^){0,1})([!$&#%])(\d+)('s){0,1}",
		RegexOptions.Multiline);

	/// <summary>
	///     The player token is used by players in emotes to refer to other things. Forms are:
	///     ~2.tall.man - a man / you
	///     ~2.tall.man's - a man's / your
	///     ~2.tall.man|one|two - one / two
	///     ~!2.tall.man - him / you
	///     ~#2.tall.man - he / you
	///     ~!2.tall.man's - his / your
	///     Above forms target characters. Replace ~ with * to target game items.
	/// </summary>
	private static readonly Regex PlayerTokenRegex =
		new(@"(?<=([!.?:] |^){0,1})([~*])([#!?]){0,1}(\d*[\w.]*[\w]+)('s){0,1}", RegexOptions.Multiline);

	/// <summary>
	///     The first third internal token is used to offer differing output for grammatical reasons to a target, e.g. @
	///     give|gives you $0
	/// </summary>
	private static readonly Regex FirstThirdInternalTokenRegex =
		new(@"(?<=([!.?:] |^){0,1})(?:[$](\d*[\w.]+)\|)(\w+)\|(\w+)", RegexOptions.Multiline);

	/// <summary>
	/// The plurality token is used to differentiate between the plurality of a token. &0|text if single thing|text if group of things.
	/// </summary>
	private static readonly Regex PluralityTokenRegex =
		new(@"(?<=([!.?:] |^){0,1})(?:[&](?<index>\d*[\w.]+)\|)(?<first>\w+)\|(?<second>\w+)", RegexOptions.Multiline);

	/// <summary>
	/// This token is used to differentiate the number of a pronoun, e.g. %0|stop|stops would be stop for you/they and stops for he/she/it
	/// </summary>
	private static readonly Regex PronounPersonTokenRegex =
		new(@"(?<=([!.?:] |^){0,1})(?:[%](?<index>\d*[\w.]+)\|)(?<plural>\w+)\|(?<singular>\w+)",
			RegexOptions.Multiline);

	/// <summary>
	///     The first third player token is used when players, in emotes, give a different form for a specific target.
	/// </summary>
	private static readonly Regex FirstThirdPlayerTokenRegex =
		new(@"(?<=([!.?:] |^){0,1})(?:([~*]\d*[\w.]+)\|){0,1}(\w+)\|(\w+)", RegexOptions.Multiline);

	/// <summary>
	///     Used to test if a perceivable is null, and display different text depending on the outcome. Used like: @ lock|locks $0 $?2|on $2||$ $?1|with $1||$.
	/// </summary>
	private static readonly Regex NullPerceivableTokenRegex =
		new(@"\$\?(?<perceivable>\d+)\|(?<notnull>[^|]*)\|(?<null>[^|]*)\|\$");

	/// <summary>
	/// $0=1 - if $0=$1, himself/yourself, else $0
	/// </summary>
	private static readonly Regex OptionalItselfTokenRegex =
		new(@"(?<=([!.?:] |^){0,1})\$(?<firsttoken>\d+)=(?<secondtoken>\d+)(?<possessive>'s)*", RegexOptions.Multiline);

	/// <summary>
	///     The source token is used to parse uses of the @ symbol in emotes, to refer to the emote owner
	/// </summary>
	private static readonly Regex SourceTokenRegex = new(@"(?<=([!.?:] |^){0,1})([@])([#!]){0,1}('s){0,1}",
		RegexOptions.Multiline);

	/// <summary>
	///     The culture token is used to parse player culture specific strings. The general format is
	///     (ampersand)culture1,culture2:text if culture|text if not(ampersand)
	/// </summary>
	private static readonly Regex CultureTokenRegex =
		new(@"[&](?:(?<cultures>[\w\-,]+)\:(?<primarytext>[^|&]*)\|)(?<alttext>[^|&]*)[&]",
			RegexOptions.IgnoreCase);

	/// <summary>
	///     The speech token is used to find player speech in emotes.
	/// </summary>
	private static readonly Regex SpeechTokenRegex = new("\"(?<speech>[^\"]+)\"");

	private readonly List<EmoteToken> _tokens = new();

	protected bool ScoutTargets(bool forceSourceInclusion, PermitLanguageOptions permitSpeech, bool playerMode,
		ILanguage language, IAccent accent,
		IList<IPerceivable> perceivables = null)
	{
#if DEBUG
#else
            try {
#endif
		int[] targetCounter = { 0 };
		var sourceIncluded = false;
		var text = RawText;

		if (permitSpeech == PermitLanguageOptions.LanguageIsError && SpeechTokenRegex.IsMatch(text))
		{
			ErrorMessage = "You are not permitted to include speech in that emote.";
			return false;
		}

		//See if we have an odd number of quote marks. If so, append another one on the end. If this ends up having unintended
		//side effects, may need to relocate this logic higher up the emote call stack.
		//According to some research I did, the Length/Replace() way of counting is actually faster than the following line
		//int quoteCount = text.Count(c => c == '\"');
		var quoteCount = text.Length - text.Replace("\"", "").Length;
		if (quoteCount % 2 == 1)
		{
			text = text.Append("\"");
		}

		text = SpeechTokenRegex.Replace(text, m =>
		{
			if (permitSpeech == PermitLanguageOptions.IgnoreLanguage)
			{
				return m.Groups[0].Value;
			}

			if (language == null || accent == null)
			{
				return m.Groups[0].Value;
			}

			var token = new LanguageToken(Source, m.Groups["speech"].Value, language, accent, permitSpeech);
			_tokens.Add(token);
			return "{" + targetCounter[0]++ + "}";
		});

		if (!playerMode)
		{
			text = NullPerceivableTokenRegex.Replace(text, m =>
			{
				if (perceivables == null)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere were references to perceivables but null was passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var index = Convert.ToInt32(m.Groups["perceivable"].Value);

				if (perceivables.Count <= index)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere was a reference to perceivable {index}, but there were only {perceivables.Count} perceivables passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var token = new NullPerceivableToken(Source, perceivables, perceivables[index],
					m.Groups["null"].Value,
					m.Groups["notnull"].Value);
				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});

			text = OptionalItselfTokenRegex.Replace(text, m =>
			{
				if (perceivables == null)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere were references to perceivables but null was passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var targetIndex = Convert.ToInt32(m.Groups["firsttoken"].Value);
				var otherIndex = Convert.ToInt32(m.Groups["secondtoken"].Value);

				if (perceivables.Count <= targetIndex)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere was a reference to perceivable {targetIndex}, but there were only {perceivables.Count} perceivables passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				if (perceivables.Count <= otherIndex)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere was a reference to perceivable {otherIndex}, but there were only {perceivables.Count} perceivables passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var token = new OptionalItselfToken(perceivables[targetIndex], perceivables[otherIndex]);
				_tokens.Add(token);
				return $"{{{targetCounter[0]++}}}";
			});

			text = FirstThirdInternalTokenRegex.Replace(text, m =>
			{
				if (perceivables == null)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere were references to perceivables but null was passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var index = Convert.ToInt32(m.Groups[2].Value);

				if (perceivables.Count <= index)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere was a reference to perceivable {index}, but there were only {perceivables.Count} perceivables passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var token = new FirstThirdEmoteToken(perceivables[index], m.Groups[3].Value, m.Groups[4].Value);
				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});

			text = PluralityTokenRegex.Replace(text, m =>
			{
				if (perceivables == null)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere were references to perceivables but null was passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var index = Convert.ToInt32(m.Groups["index"].Value);

				if (perceivables.Count <= index)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere was a reference to perceivable {index}, but there were only {perceivables.Count} perceivables passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var token = new PluralityEmoteToken(perceivables[index], m.Groups["first"].Value,
					m.Groups["second"].Value);
				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});

			text = PronounPersonTokenRegex.Replace(text, m =>
			{
				if (perceivables == null)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere were references to perceivables but null was passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var index = Convert.ToInt32(m.Groups["index"].Value);

				if (perceivables.Count <= index)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere was a reference to perceivable {index}, but there were only {perceivables.Count} perceivables passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var token = new PronounNumberToken(perceivables[index], m.Groups["singular"].Value,
					m.Groups["plural"].Value);
				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});

			text = FirstThirdPlayerTokenRegex.Replace(text, m =>
			{
				EmoteToken token = m.Groups[2].Length > 0
					? new FirstThirdEmoteToken(m.Groups[2].Value, Source, m.Groups[3].Value, m.Groups[4].Value)
					: new FirstThirdEmoteToken(Source, m.Groups[3].Value, m.Groups[4].Value);
				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});

			text = InternalTokenRegex.Replace(text, m =>
			{
				if (perceivables == null)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere were references to perceivables but null was passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				var index = Convert.ToInt32(m.Groups[3].Value);

				if (perceivables.Count <= index)
				{
					ErrorMessage =
						$"Invalid emote: {RawText}\nThere was a reference to perceivable {index}, but there were only {perceivables.Count} perceivables passed.";
					Futuremud.Games.First().DiscordConnection.NotifyBadEcho(ErrorMessage);
					return m.Groups[0].Value;
				}

				EmoteToken token = null;
				if (m.Groups[4].Length > 0)
				{
					if (perceivables[index]?.Sentient == true)
					{
						token = new PossessiveCharacterToken(perceivables[index], false,
							m.Groups[2].Value == "&",
							stripAAn: m.Groups[2].Value == "!");
					}
					else
					{
						token = new PossessiveObjectToken(perceivables[index], false, m.Groups[2].Value == "&",
							stripAAn: m.Groups[2].Value == "!");
					}
				}
				else
				{
					if (perceivables[index]?.Sentient == true)
					{
						token = new ObjectiveCharacterToken(perceivables[index], false,
							m.Groups[2].Value != "$" && m.Groups[2].Value != "!",
							m.Groups[2].Value == "#", m.Groups[2].Value == "%",
							stripAAn: m.Groups[2].Value == "!");
					}
					else
					{
						token = new ObjectiveObjectToken(perceivables[index], false,
							m.Groups[2].Value != "$" && m.Groups[2].Value != "!",
							m.Groups[2].Value == "#", m.Groups[2].Value == "%",
							stripAAn: m.Groups[2].Value == "!");
					}
				}

				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});
		}

		else
		{
			text = FirstThirdPlayerTokenRegex.Replace(text, m =>
			{
				EmoteToken token = m.Groups[2].Length > 0
					? new FirstThirdEmoteToken(m.Groups[2].Value, Source, m.Groups[3].Value, m.Groups[4].Value)
					: new FirstThirdEmoteToken(Source, m.Groups[3].Value, m.Groups[4].Value);
				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});

			text = PlayerTokenRegex.Replace(text, m =>
			{
				EmoteToken token = null;
				if (m.Groups[5].Length > 0)
				{
					if (m.Groups[2].Value == "~")
					{
						token = new PossessiveCharacterToken(m.Groups[4].Value, Source, false,
							m.Groups[3].Value == "!");
					}
					else
					{
						token = new PossessiveObjectToken(m.Groups[4].Value, Source, false,
							m.Groups[3].Value == "!");
					}
				}
				else
				{
					if (m.Groups[2].Value == "~")
					{
						token = new ObjectiveCharacterToken(m.Groups[3].Value + m.Groups[4].Value, Source, false,
							m.Groups[3].Length > 0);
					}
					else
					{
						token = new ObjectiveObjectToken(m.Groups[3].Value + m.Groups[4].Value, Source, false,
							m.Groups[3].Length > 0);
					}
				}

				_tokens.Add(token);
				return "{" + targetCounter[0]++ + "}";
			});
		}

		text = SourceTokenRegex.Replace(text, m =>
		{
			EmoteToken token;
			if (m.Groups[4].Length > 0)
			{
				if (Source.Sentient)
				{
					token = new PossessiveCharacterToken(Source, false, m.Groups[3].Length > 0);
				}
				else
				{
					token = new PossessiveObjectToken(Source, false, m.Groups[3].Length > 0);
				}

				sourceIncluded = true;
			}
			else
			{
				if (Source.Sentient)
				{
					token = new ObjectiveCharacterToken(Source, false, m.Groups[3].Length > 0,
						m.Groups[3].Value == "#", forcedSourceInclusion: ForcedSourceInclusion);
				}
				else
				{
					token = new ObjectiveObjectToken(Source, false, m.Groups[3].Length > 0,
						m.Groups[3].Value == "#");
				}

				if (m.Groups[3].Length == 0)
				{
					sourceIncluded = true;
				}
			}

			_tokens.Add(token);
			return "{" + targetCounter[0]++ + "}";
		});

		if (forceSourceInclusion && !sourceIncluded)
		{
			text = "{" + targetCounter[0]++ + "} " + text;
			if (Source.Sentient)
			{
				_tokens.Add(new ObjectiveCharacterToken(Source, true, forcedSourceInclusion: ForcedSourceInclusion));
			}
			else
			{
				_tokens.Add(new ObjectiveObjectToken(Source, true, coloured: false));
			}
		}

		RawText = text;
		if (_tokens.Any(x => !x.Valid))
		{
			ErrorMessage = _tokens.First(x => !x.Valid).Error;
		}

		return string.IsNullOrEmpty(ErrorMessage);
#if DEBUG
#else
            }
            catch (Exception e) {
                var exceptionText =
 $"Exception in Emote.ScoutTargets for emote:\n\n{RawText}\n\nTokens:\n\n{(from token in _tokens select (token?.Target?.HowSeen(Source, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf) ?? "null")).ListToString()}\n\n{e.Message}";
                throw new ApplicationException(exceptionText);
            }
#endif
	}

	protected abstract class EmoteToken
	{
		public virtual bool NullSafe => false;
		protected readonly bool Coloured;
		protected readonly bool Proper;
		protected readonly bool StripAAn;
		protected string _error;
		protected IPerceivable _target;
		protected bool Pronouned;

		protected EmoteToken(IPerceivable target, bool proper = false, bool pronouned = false, bool coloured = true,
			bool stripAAn = false)
		{
			Valid = true;
			Proper = proper;
			Pronouned = pronouned;
			StripAAn = stripAAn;
			_target = target;
			Coloured = coloured;
		}

		protected EmoteToken(string lookup, IPerceiver source, bool proper = false, bool pronouned = false,
			bool coloured = true, bool stripAAn = false)
		{
			Proper = proper;
			Pronouned = pronouned;
			Coloured = coloured;
			StripAAn = stripAAn;
			Valid = Process(lookup, source);
		}

		protected EmoteToken(XElement root, IFuturemud gameworld)
		{
			Proper = Convert.ToBoolean(root.Attribute("Proper").Value);
			Pronouned = Convert.ToBoolean(root.Attribute("Pronouned").Value);
			Coloured = Convert.ToBoolean(root.Attribute("Coloured").Value);
			StripAAn = Convert.ToBoolean(root.Attribute("StripAAn")?.Value ?? "false");
			_target = gameworld.GetPerceivable(root.Attribute("TargetType").Value,
				long.Parse(root.Attribute("TargetId").Value));
			Valid = _target != null;
		}

		public string Error => _error;

		public bool Valid { get; protected init; }

		public IPerceivable Target => _target;

		internal static EmoteToken LoadToken(XElement root, IFuturemud gameworld)
		{
			switch (root.Attribute("Type").Value)
			{
				case "ObjectiveCharacter":
					return new ObjectiveCharacterToken(root, gameworld);
				case "ObjectiveObject":
					return new ObjectiveObjectToken(root, gameworld);
				case "PossessiveCharacter":
					return new PossessiveCharacterToken(root, gameworld);
				case "PossessiveObject":
					return new PossessiveObjectToken(root, gameworld);
				case "FirstThird":
					return new FirstThirdEmoteToken(root, gameworld);
				case "Plurality":
					return new PluralityEmoteToken(root, gameworld);
				case "PronounNumber":
					return new PronounNumberToken(root, gameworld);
				default:
					throw new NotSupportedException();
			}
		}

		internal virtual XElement SaveToXml()
		{
			return new XElement("Token", new XAttribute("Proper", Proper.ToString()),
				new XAttribute("Pronouned", Pronouned.ToString()), new XAttribute("Coloured", Coloured.ToString()),
				new XAttribute("StripAAn", StripAAn.ToString()), new XAttribute("TargetType", Target.FrameworkItemType),
				new XAttribute("TargetId", Target.Id));
		}

		protected abstract bool Process(string lookup, IPerceiver source);

		public abstract string DisplayFirstPerson();
		public abstract string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags);
	}

	private class ObjectiveCharacterToken : EmoteToken
	{
		private readonly bool _forcedSourceInclusion;
		private bool _reflexive;
		private bool _subjective;

		public ObjectiveCharacterToken(IPerceivable target, bool proper = false, bool pronouned = false,
			bool subjective = false, bool reflexive = false, bool coloured = true, bool stripAAn = false,
			bool forcedSourceInclusion = false)
			: base(target, proper, pronouned, coloured, stripAAn)
		{
			_subjective = subjective;
			_reflexive = reflexive;
			_forcedSourceInclusion = forcedSourceInclusion;
		}

		public ObjectiveCharacterToken(string lookup, IPerceiver source, bool proper = false, bool pronouned = false,
			bool coloured = true, bool stripAAn = false, bool forcedSourceInclusion = false)
			: base(lookup, source, proper, pronouned, coloured, stripAAn)
		{
			_forcedSourceInclusion = forcedSourceInclusion;
		}

		public ObjectiveCharacterToken(XElement root, IFuturemud gameworld)
			: base(root, gameworld)
		{
			_subjective = root.Attribute("Subjective") != null && bool.Parse(root.Attribute("Subjective").Value);
			_reflexive = root.Attribute("Reflexive") != null && bool.Parse(root.Attribute("Reflexive").Value);
		}

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("Subjective", _subjective));
			xml.Add(new XAttribute("Reflexive", _reflexive));
			xml.Add(new XAttribute("Type", "ObjectiveCharacter"));
			return xml;
		}

		protected override bool Process(string text, IPerceiver source)
		{
			switch (text.First())
			{
				case PronounDelimiter:
					text = text.Substring(1);
					Pronouned = true;
					break;
				case ReflexiveDelimiter:
					text = text.Substring(1);
					_reflexive = true;
					break;
				case SubjectiveDelimiter:
					text = text.Substring(1);
					_subjective = true;
					break;
			}

			_target = (source as ITarget)?.TargetActor(text);

			if (_target == null)
			{
				_error = "Who is \"" + text + "\"?";
				return false;
			}

			return true;
		}

		public override string DisplayFirstPerson()
		{
			if (_forcedSourceInclusion && !Pronouned)
			{
				return DisplayThirdPerson(_target as IPerceiver, PerceiveIgnoreFlags.None);
			}

			return _reflexive
				? "yourself".FluentProper(Proper).FluentColour(Telnet.Magenta, Coloured)
				: "you".FluentProper(Proper).FluentColour(Telnet.Magenta, Coloured);
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			if (_target == null)
			{
				return string.Empty;
			}

			if (_reflexive)
			{
				return _target.ApparentGender(perceiver).Reflexive(Proper);
			}

			if (_subjective)
			{
				return _target.ApparentGender(perceiver).Subjective(Proper);
			}

			return Pronouned
				? _target.ApparentGender(perceiver).Objective(Proper)
				: _target.HowSeen(perceiver, Proper, colour: Coloured, flags: PerceiveIgnoreFlags.IgnoreSelf | flags)
				         .Strip_A_An(StripAAn);
		}
	}

	private class ObjectiveObjectToken : EmoteToken
	{
		private bool _reflexive;
		private bool _subjective;

		public ObjectiveObjectToken(IPerceivable target, bool proper = false, bool pronouned = false,
			bool subjective = false, bool reflexive = false, bool coloured = true, bool stripAAn = false)
			: base(target, proper, pronouned, coloured, stripAAn)
		{
			_subjective = subjective;
			_reflexive = reflexive;
		}

		public ObjectiveObjectToken(string lookup, IPerceiver source, bool proper = false, bool pronouned = false,
			bool coloured = true, bool stripAAn = false)
			: base(lookup, source, proper, pronouned, coloured, stripAAn)
		{
		}

		public ObjectiveObjectToken(XElement root, IFuturemud gameworld)
			: base(root, gameworld)
		{
			_subjective = root.Attribute("Subjective") != null && bool.Parse(root.Attribute("Subjective").Value);
			_reflexive = root.Attribute("Reflexive") != null && bool.Parse(root.Attribute("Reflexive").Value);
		}

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("Subjective", _subjective));
			xml.Add(new XAttribute("Reflexive", _reflexive));
			xml.Add(new XAttribute("Type", "ObjectiveObject"));
			return xml;
		}

		protected override bool Process(string text, IPerceiver source)
		{
			switch (text.First())
			{
				case PronounDelimiter:
					text = text.Substring(1);
					Pronouned = true;
					break;
				case ReflexiveDelimiter:
					text = text.Substring(1);
					_reflexive = true;
					break;
				case SubjectiveDelimiter:
					text = text.Substring(1);
					_subjective = true;
					break;
			}

			_target = (source as ITarget)?.TargetItem(text);

			if (_target == null)
			{
				_error = "What is \"" + text + "\"?";
				return false;
			}

			return true;
		}

		public override string DisplayFirstPerson()
		{
			return _reflexive ? "yourself".FluentProper(Proper) : "you".FluentProper(Proper);
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			if (_target == null)
			{
				return string.Empty;
			}

			if (_reflexive)
			{
				return _target.ApparentGender(perceiver).Reflexive(Proper);
			}

			if (_subjective)
			{
				return _target.ApparentGender(perceiver).Subjective(Proper);
			}

			return Pronouned
				? _target.ApparentGender(perceiver).Objective(Proper)
				: _target.HowSeen(perceiver, Proper, colour: Coloured, flags: PerceiveIgnoreFlags.IgnoreLayers | flags)
				         .Strip_A_An(StripAAn);
		}
	}

	private class PossessiveCharacterToken : EmoteToken
	{
		public PossessiveCharacterToken(IPerceivable target, bool proper = false, bool pronouned = false,
			bool coloured = true, bool stripAAn = false)
			: base(target, proper, pronouned, coloured, stripAAn)
		{
		}

		public PossessiveCharacterToken(string lookup, IPerceiver source, bool proper = false,
			bool pronouned = false, bool coloured = true, bool stripAAn = false)
			: base(lookup, source, proper, pronouned, coloured, stripAAn)
		{
		}

		public PossessiveCharacterToken(XElement root, IFuturemud gameworld)
			: base(root, gameworld)
		{
		}

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("Type", "PossessiveCharacter"));
			return xml;
		}

		protected override bool Process(string text, IPerceiver source)
		{
			if (text.First() == PronounDelimiter || text.First() == SubjectiveDelimiter)
			{
				text = text.Substring(1);
				Pronouned = true;
			}

			_target = (source as ITarget)?.TargetActor(text);

			if (_target == null)
			{
				_error = "Who is \"" + text + "\"?";
				return false;
			}

			return true;
		}

		public override string DisplayFirstPerson()
		{
			return "your".FluentProper(Proper).FluentColour(Telnet.Magenta, Coloured);
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			if (_target == null)
			{
				return string.Empty;
			}

			if (Pronouned)
			{
				return _target.ApparentGender(perceiver).Possessive(Proper);
			}

			var pch = _target.HowSeen(perceiver, Proper, colour: Coloured, flags: flags).Strip_A_An(StripAAn);
			return pch + (pch.Last() == 's' ? "'" : "'s");
		}
	}

	private class PossessiveObjectToken : EmoteToken
	{
		public PossessiveObjectToken(IPerceivable target, bool proper = false, bool pronouned = false,
			bool coloured = true, bool stripAAn = false)
			: base(target, proper, pronouned, coloured, stripAAn)
		{
		}

		public PossessiveObjectToken(string lookup, IPerceiver source, bool proper = false, bool pronouned = false,
			bool coloured = true, bool stripAAn = false)
			: base(lookup, source, proper, pronouned, coloured, stripAAn)
		{
		}

		public PossessiveObjectToken(XElement root, IFuturemud gameworld)
			: base(root, gameworld)
		{
		}

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("Type", "PossessiveObject"));
			return xml;
		}

		protected override bool Process(string text, IPerceiver source)
		{
			if (text.First() == PronounDelimiter || text.First() == SubjectiveDelimiter)
			{
				text = text.Substring(1);
				Pronouned = true;
			}

			_target = (source as ITarget)?.TargetItem(text);

			if (_target == null)
			{
				_error = "What is \"" + text + "\"?";
				return false;
			}

			return true;
		}

		public override string DisplayFirstPerson()
		{
			return "you".FluentProper(Proper);
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			if (_target == null)
			{
				return string.Empty;
			}

			if (Pronouned)
			{
				return _target.ApparentGender(perceiver).Possessive(Proper);
			}

			var pob = _target.HowSeen(perceiver, Proper, colour: Coloured, flags: flags).Strip_A_An(StripAAn);
			return pob + (pob.Last() == 's' ? "'" : "'s");
		}
	}

	private class FirstThirdEmoteToken : EmoteToken
	{
		public FirstThirdEmoteToken(IPerceivable target, string firstPersonString, string thirdPersonString,
			bool proper = false)
			: base(target, proper)
		{
			FirstPersonString = firstPersonString;
			ThirdPersonString = thirdPersonString;
		}

		public FirstThirdEmoteToken(string lookup, IPerceiver source, string firstPersonString,
			string thirdPersonString, bool proper = false)
			: base(lookup, null, proper)
		{
			_target = lookup[0] == '~'
				? (IPerceivable)(source as ITarget)?.TargetBody(lookup.Substring(1))
				: (source as ITarget)?.TargetItem(lookup.Substring(1));
			FirstPersonString = firstPersonString;
			ThirdPersonString = thirdPersonString;
		}

		public FirstThirdEmoteToken(XElement root, IFuturemud gameworld)
			: base(root, gameworld)
		{
			FirstPersonString = root.Attribute("FirstPerson").Value;
			ThirdPersonString = root.Attribute("ThirdPerson").Value;
		}

		public string FirstPersonString { get; }

		public string ThirdPersonString { get; }

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("FirstPerson", FirstPersonString));
			xml.Add(new XAttribute("ThirdPerson", ThirdPersonString));
			xml.Add(new XAttribute("Type", "FirstThird"));
			return xml;
		}

		protected override bool Process(string lookup, IPerceiver source)
		{
			return source != null;
		}

		public override string DisplayFirstPerson()
		{
			return FirstPersonString;
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			return ThirdPersonString;
		}
	}

	private class PronounNumberToken : EmoteToken
	{
		public PronounNumberToken(IPerceivable target, string singularString, string pluralString,
			bool proper = false)
			: base(target, proper)
		{
			SingularString = singularString;
			PluralString = pluralString;
		}

		public PronounNumberToken(string lookup, IPerceiver source, string singularString,
			string pluralString, bool proper = false)
			: base(lookup, null, proper)
		{
			_target = lookup[0] == '~'
				? (IPerceivable)(source as ITarget)?.TargetBody(lookup.Substring(1))
				: (source as ITarget)?.TargetItem(lookup.Substring(1));
			SingularString = singularString;
			PluralString = pluralString;
		}

		public PronounNumberToken(XElement root, IFuturemud gameworld)
			: base(root, gameworld)
		{
			SingularString = root.Attribute("SingularString").Value;
			PluralString = root.Attribute("PluralString").Value;
		}

		public string SingularString { get; }

		public string PluralString { get; }

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("SingularString", SingularString));
			xml.Add(new XAttribute("PluralString", PluralString));
			xml.Add(new XAttribute("Type", "PronounNumber"));
			return xml;
		}

		protected override bool Process(string lookup, IPerceiver source)
		{
			return source != null;
		}

		public override string DisplayFirstPerson()
		{
			return PluralString;
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			if (_target == null)
			{
				return string.Empty;
			}

			switch (_target.ApparentGender(perceiver).Enum)
			{
				case Form.Shape.Gender.Female:
				case Form.Shape.Gender.Male:
				case Form.Shape.Gender.Neuter:
					return SingularString;
				default:
					return PluralString;
			}
		}
	}

	private class PluralityEmoteToken : EmoteToken
	{
		public PluralityEmoteToken(IPerceivable target, string singularString, string pluralString,
			bool proper = false)
			: base(target, proper)
		{
			SingularString = singularString;
			PluralString = pluralString;
		}

		public PluralityEmoteToken(string lookup, IPerceiver source, string singularString,
			string pluralString, bool proper = false)
			: base(lookup, null, proper)
		{
			_target = lookup[0] == '~'
				? (IPerceivable)(source as ITarget)?.TargetBody(lookup.Substring(1))
				: (source as ITarget)?.TargetItem(lookup.Substring(1));
			SingularString = singularString;
			PluralString = pluralString;
		}

		public PluralityEmoteToken(XElement root, IFuturemud gameworld)
			: base(root, gameworld)
		{
			SingularString = root.Attribute("SingularString").Value;
			PluralString = root.Attribute("PluralString").Value;
		}

		public string SingularString { get; }

		public string PluralString { get; }

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("SingularString", SingularString));
			xml.Add(new XAttribute("PluralString", PluralString));
			xml.Add(new XAttribute("Type", "Plurality"));
			return xml;
		}

		protected override bool Process(string lookup, IPerceiver source)
		{
			return source != null;
		}

		public override string DisplayFirstPerson()
		{
			return _target?.IsSingleEntity == true ? SingularString : PluralString;
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			return _target?.IsSingleEntity == true ? SingularString : PluralString;
		}
	}

	private class NullPerceivableToken : EmoteToken
	{
		public override bool NullSafe => true;
		private readonly string _emoteText;
		private readonly IPerceivable[] _perceivables;
		private readonly IPerceiver _sourcePerceiver;

		public NullPerceivableToken(IPerceiver source, IList<IPerceivable> perceivables, IPerceivable target,
			string textIfNull, string textIfNotNull,
			bool proper = false) : base(target, proper)
		{
			_emoteText = target == null ? textIfNull : textIfNotNull;
			_sourcePerceiver = source;
			_perceivables = perceivables.ToArray();
		}

		protected override bool Process(string lookup, IPerceiver source)
		{
			return true;
		}

		public override string DisplayFirstPerson()
		{
			return DisplayThirdPerson(_sourcePerceiver, PerceiveIgnoreFlags.None);
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			return new Emote(_emoteText, _sourcePerceiver, _perceivables).ParseFor(perceiver, flags);
		}
	}

	private class LanguageToken : EmoteToken
	{
		private readonly LanguageInfo _languageInfo;
		private readonly ILanguagePerceiver _languagePerceiver;
		private readonly PermitLanguageOptions _permitLanguageOptions;

		public LanguageToken(IPerceiver source, string speechText, ILanguage language, IAccent accent,
			PermitLanguageOptions options) : base(source)
		{
			_languagePerceiver = source as ILanguagePerceiver;
			_permitLanguageOptions = options;
			if (_languagePerceiver == null)
			{
				_languageInfo = new EmoteSpokenLanguageInfo(language, accent, AudioVolume.Decent, speechText,
					Outcome.MajorPass, source, null);
				return;
			}

			_languageInfo = new EmoteSpokenLanguageInfo(language, accent, AudioVolume.Decent, speechText,
				_permitLanguageOptions == PermitLanguageOptions.PermitLanguage
					? source.Gameworld.GetCheck(CheckType.SpokenLanguageSpeakCheck)
					        .Check(_languagePerceiver, Difficulty.Normal, language.LinkedTrait)
					: Outcome.NotTested, source, null);
		}

		protected override bool Process(string lookup, IPerceiver source)
		{
			return true;
		}

		public override bool NullSafe => true;

		public override string DisplayFirstPerson()
		{
			if (_permitLanguageOptions == PermitLanguageOptions.PermitLanguage || _languagePerceiver == null)
			{
				return _languageInfo.ParseFor(_languagePerceiver);
			}

			return DisplayThirdPerson(_languagePerceiver, PerceiveIgnoreFlags.None);
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			if (!(perceiver is ILanguagePerceiver languagePerceiver))
			{
				return _languageInfo.ParseFor(_languagePerceiver);
			}

			switch (_permitLanguageOptions)
			{
				case PermitLanguageOptions.IgnoreLanguage:
					return DisplayFirstPerson();
				case PermitLanguageOptions.PermitLanguage:
					return _languageInfo.ParseFor(languagePerceiver);
				case PermitLanguageOptions.LanguageIsGasping:
					return "*** incomprehensible gasping ***".ColourBold(Telnet.Cyan);
				case PermitLanguageOptions.LanguageIsMuffling:
					return "*** muted, muffled speech ***".ColourBold(Telnet.Cyan);
				case PermitLanguageOptions.LanguageIsChoking:
					return "*** incomprehensible choking ***".ColourBold(Telnet.Cyan);
				case PermitLanguageOptions.LanguageIsClicking:
					return "*** odd clicking sounds ***".ColourBold(Telnet.Cyan);
				case PermitLanguageOptions.LanguageIsBuzzing:
					return "*** muted buzzing sounds ***".ColourBold(Telnet.Cyan);
				default:
					throw new ArgumentOutOfRangeException(nameof(perceiver));
			}
		}
	}

	private class OptionalItselfToken : EmoteToken
	{
		private IPerceivable Other { get; set; }
		private bool _possessive;

		public OptionalItselfToken(IPerceivable target, IPerceivable other, bool proper = false, bool pronouned = false,
			bool coloured = true, bool stripAAn = false, bool possessive = false) : base(target, proper, pronouned,
			coloured, stripAAn)
		{
			Other = other;
			_possessive = possessive;
		}

		public OptionalItselfToken(XElement root, IFuturemud gameworld) : base(root, gameworld)
		{
			Other = gameworld.GetPerceivable(root.Attribute("OtherType").Value,
				long.Parse(root.Attribute("OtherId").Value));
			_possessive = bool.Parse(root.Attribute("possessive")?.Value ?? "false");
			Valid = Other != null && _target != null;
		}

		internal override XElement SaveToXml()
		{
			var xml = base.SaveToXml();
			xml.Add(new XAttribute("OtherId", Other.Id));
			xml.Add(new XAttribute("OtherType", Other.FrameworkItemType));
			xml.Add(new XAttribute("Type", "OptionalItself"));
			xml.Add(new XAttribute("possessive", _possessive));
			return xml;
		}

		#region Overrides of EmoteToken

		protected override bool Process(string lookup, IPerceiver source)
		{
			return true;
		}

		public override string DisplayFirstPerson()
		{
			if (_target == null)
			{
				return string.Empty;
			}

			if (Other.IsSelf(_target))
			{
				if (_possessive)
				{
					return "your own".FluentProper(Proper)
					                 .FluentColour(Other is ICharacter ? Telnet.Magenta : Telnet.Green, Coloured);
				}

				return "yourself".FluentProper(Proper)
				                 .FluentColour(Other is ICharacter ? Telnet.Magenta : Telnet.Green, Coloured);
			}

			if (_possessive)
			{
				return "your".FluentProper(Proper)
				             .FluentColour(Other is ICharacter ? Telnet.Magenta : Telnet.Green, Coloured);
			}

			return "you".FluentProper(Proper)
			            .FluentColour(Other is ICharacter ? Telnet.Magenta : Telnet.Green, Coloured);
		}

		public override string DisplayThirdPerson(IPerceiver perceiver, PerceiveIgnoreFlags flags)
		{
			if (_target == null)
			{
				return string.Empty;
			}

			if (_possessive)
			{
				return Other.IsSelf(_target)
					? _target.ApparentGender(perceiver).Possessive(Proper)
					: _target.HowSeen(perceiver, Proper, Form.Shape.DescriptionType.Possessive, Coloured,
						PerceiveIgnoreFlags.IgnoreSelf | flags).Strip_A_An(StripAAn);
			}

			return Other.IsSelf(_target)
				? _target.ApparentGender(perceiver).Reflexive(Proper)
				: _target.HowSeen(perceiver, Proper, colour: Coloured, flags: PerceiveIgnoreFlags.IgnoreSelf | flags)
				         .Strip_A_An(StripAAn);
		}

		#endregion
	}
}