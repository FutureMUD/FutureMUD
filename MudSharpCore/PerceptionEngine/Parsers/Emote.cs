using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Communication.Language;

namespace MudSharp.PerceptionEngine.Parsers;

public partial class Emote : IEmote
{
	protected Emote(XElement definition, IFuturemud gameworld, IPerceiver source)
	{
		RawText = definition.Element("RawText")?.Value;
		ForcedSourceInclusion = definition.Attribute("ForcedSourceInclusion").Value == "True";
		Source = source;

		foreach (var token in definition.Elements("Token"))
		{
			_tokens.Add(EmoteToken.LoadToken(token, gameworld));
		}

		Valid = _tokens.All(x => x.Valid);
	}

	/// <summary>
	///     Basic constructor for emotes.
	/// </summary>
	/// <param name="emoteString">The string that will be parsed as an emote. It may not contain any '{' characters.</param>
	/// <param name="source">
	///     The Perceivable that serves as the perceiver that scouts targets to replace emote tokens. It does
	///     not need to Perceiver CanSee any of its targets - this is instead a coded lookup.
	/// </param>
	/// <param name="forceSourceInclusion">
	///     If the PersonalDescriptionDelimiter (currently '@') is not given, forces the
	///     inclusion of the source's Short Description at the beginning of the emote.
	/// </param>
	/// <param name="permitSpeech"></param>
	public Emote(string emoteString, IPerceiver source, bool forceSourceInclusion = false,
		PermitLanguageOptions permitSpeech = PermitLanguageOptions.PermitLanguage)
	{
		RawText = emoteString;
		ForcedSourceInclusion = forceSourceInclusion;
		Source = source;
		var languagePerceiver = source as ILanguagePerceiver;
		Valid = ScoutTargets(ForcedSourceInclusion, permitSpeech, false, languagePerceiver?.CurrentLanguage,
			languagePerceiver?.CurrentAccent, new IPerceivable[] { source });
	}

	/// <summary>
	///     Constructor for internal emotes (ones we already have the references for). Does not allow for forced source
	///     inclusion.
	/// </summary>
	/// <param name="rawEmote">
	///     The raw emote string. All references should be represented with the ReferenceDelimiter
	///     (currently '$') followed by its position index in the perceivables parameters. ' or 's can be used to form a
	///     possessive reference.
	/// </param>
	/// <param name="source">
	///     The Perceivable that serves as the perceiver that scouts targets to replace emote tokens. It does
	///     not need to Perceiver CanSee any of its targets - this is instead a coded lookup.
	/// </param>
	/// <param name="perceivables">The list of references referred to by the raw emote string.</param>
	public Emote(string rawEmote, IPerceiver source, params IPerceivable[] perceivables)
	{
		RawText = rawEmote;
		Source = source;
		ForcedSourceInclusion = false;
		var languagePerceiver = source as ILanguagePerceiver;
		Valid = ScoutTargets(ForcedSourceInclusion, PermitLanguageOptions.PermitLanguage, false,
			languagePerceiver?.CurrentLanguage, languagePerceiver?.CurrentAccent, perceivables);
	}

	protected Emote()
	{
	}

	public bool Valid { get; protected init; }

	//public bool Prepared {
	//    get;
	//    private set;
	//}

	public string ErrorMessage { get; protected set; }

	public string RawText { get; protected set; }

	public virtual bool FixedFormat => false;

	public IPerceiver Source { get; protected init; }

	public IEnumerable<IPerceivable> Targets
	{
		get { return _tokens.Select(x => x.Target); }
	}

	public bool ForcedSourceInclusion { get; protected init; }

	public virtual XElement SaveToXml()
	{
		return new XElement("Emote", new XAttribute("PlayerMode", false), new XElement("RawText", RawText),
			new XAttribute("ForcedSourceInclusion", ForcedSourceInclusion.ToString()), from token in _tokens
				select token.SaveToXml());
	}

	public static Emote LoadEmote(XElement definition, IFuturemud gameworld, IPerceiver source)
	{
		return definition.Attribute("PlayerMode")?.Value == "true"
			? new PlayerEmote(definition, gameworld, source)
			: new Emote(definition, gameworld, source);
	}

	///// <summary>
	///// Fluent method for setting this emote to force the inclusion of the source of the emote if not included in it.
	///// </summary>
	//public Emote ForceSourceInclusion() {
	//    ForcedSourceInclusion = true;
	//    return this;
	//}

	///// <summary>
	///// Fluent method for preparing an emote.
	///// </summary>
	//public Emote Prepare() {
	//    Valid = ScoutTargets(Source, ForcedSourceInclusion);
	//    Prepared = true;
	//    return this;
	//}

	public string ParseFor(IPerceiver perceiver, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None)
	{
		//if (!Prepared) throw new ApplicationException("This emote is has not been prepared!");
		if (!Valid)
		{
			return ErrorMessage;
		}

		var parsedTokens =
			_tokens.Select(
				x =>
					x.NullSafe || x.Target != null
						? x.Target?.IsSelf(perceiver) == true
							? x.DisplayFirstPerson()
							: x.DisplayThirdPerson(perceiver, flags)
						: string.Empty).ToArray<object>();

		if (FixedFormat)
		{
			return string.Format(RawText, parsedTokens);
		}

		return string.Format(RawText, parsedTokens);
	}
}

public class PlayerEmote : Emote
{
	/// <summary>
	///     Basic constructor for emotes.
	/// </summary>
	/// <param name="emoteString">The string that will be parsed as an emote. It may not contain any '{' characters.</param>
	/// <param name="source">
	///     The Perceivable that serves as the perceiver that scouts targets to replace emote tokens. It does
	///     not need to Perceiver CanSee any of its targets - this is instead a coded lookup.
	/// </param>
	/// <param name="forceSourceInclusion">
	///     If the PersonalDescriptionDelimiter (currently '@') is not given, forces the
	///     inclusion of the source's Short Description at the beginning of the emote.
	/// </param>
	/// <param name="permitSpeech"></param>
	public PlayerEmote(string emoteString, IPerceiver source, bool forceSourceInclusion = false,
		PermitLanguageOptions permitSpeech = PermitLanguageOptions.PermitLanguage)
	{
		RawText = emoteString.Sanitise();
		ForcedSourceInclusion = forceSourceInclusion;
		Source = source;
		var languagePerceiver = source as ILanguagePerceiver;
		Valid = ScoutTargets(ForcedSourceInclusion, permitSpeech, true, languagePerceiver?.CurrentLanguage,
			languagePerceiver?.CurrentAccent, new IPerceivable[] { source });
	}

	/// <summary>
	///     Constructor for internal emotes (ones we already have the references for). Does not allow for forced source
	///     inclusion.
	/// </summary>
	/// <param name="rawEmote">
	///     The raw emote string. All references should be represented with the ReferenceDelimiter
	///     (currently '$') followed by its position index in the perceivables parameters. ' or 's can be used to form a
	///     possessive reference.
	/// </param>
	/// <param name="source">
	///     The Perceivable that serves as the perceiver that scouts targets to replace emote tokens. It does
	///     not need to Perceiver CanSee any of its targets - this is instead a coded lookup.
	/// </param>
	/// <param name="perceivables">The list of references referred to by the raw emote string.</param>
	public PlayerEmote(string rawEmote, IPerceiver source, params IPerceivable[] perceivables)
	{
		RawText = rawEmote;
		Source = source;
		ForcedSourceInclusion = false;
		var languagePerceiver = source as ILanguagePerceiver;
		Valid = ScoutTargets(ForcedSourceInclusion, PermitLanguageOptions.PermitLanguage, true,
			languagePerceiver?.CurrentLanguage, languagePerceiver?.CurrentAccent, perceivables);
	}

	public PlayerEmote(XElement definition, IFuturemud gameworld, IPerceiver source)
		: base(definition, gameworld, source)
	{
	}

	#region Overrides of Emote

	public override XElement SaveToXml()
	{
		var emote = base.SaveToXml();
		emote.Attribute("PlayerMode").Value = "false";
		return emote;
	}

	#endregion
}

public class NoLanguageEmote : Emote
{
	public NoLanguageEmote(string rawEmote, IPerceiver source, params IPerceivable[] perceivables)
	{
		RawText = rawEmote;
		Source = source;
		ForcedSourceInclusion = false;

		Valid = ScoutTargets(ForcedSourceInclusion, PermitLanguageOptions.IgnoreLanguage, false, null, null,
			perceivables);
	}
}

public class FixedLanguageEmote : Emote
{
	public FixedLanguageEmote(string rawEmote, IPerceiver source, ILanguage language, IAccent accent,
		params IPerceivable[] perceivables)
	{
		RawText = rawEmote;
		Source = source;
		ForcedSourceInclusion = false;


		Valid = ScoutTargets(ForcedSourceInclusion, PermitLanguageOptions.PermitLanguage, false, language, accent,
			perceivables);
	}
}

public class NoFormatEmote : Emote
{
	public override bool FixedFormat => true;

	public NoFormatEmote(string emoteString, IPerceiver source, bool forceSourceInclusion = false,
		PermitLanguageOptions permitSpeech = PermitLanguageOptions.PermitLanguage) : base(emoteString, source,
		forceSourceInclusion, permitSpeech)
	{
	}

	/// <summary>
	///     Constructor for internal emotes (ones we already have the references for). Does not allow for forced source
	///     inclusion.
	/// </summary>
	/// <param name="rawEmote">
	///     The raw emote string. All references should be represented with the ReferenceDelimiter
	///     (currently '$') followed by its position index in the perceivables parameters. ' or 's can be used to form a
	///     possessive reference.
	/// </param>
	/// <param name="source">
	///     The Perceivable that serves as the perceiver that scouts targets to replace emote tokens. It does
	///     not need to Perceiver CanSee any of its targets - this is instead a coded lookup.
	/// </param>
	/// <param name="perceivables">The list of references referred to by the raw emote string.</param>
	public NoFormatEmote(string rawEmote, IPerceiver source, params IPerceivable[] perceivables) : base(rawEmote,
		source, perceivables)
	{
	}
}