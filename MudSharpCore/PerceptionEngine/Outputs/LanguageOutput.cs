using System;
using System.Text;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.PerceptionEngine.Outputs;

public class LanguageOutput : Output
{
	protected LanguageInfo _languageText;
	protected IEmote _optionalEmote;
	protected IEmote _preLanguageEmote;

	public LanguageOutput(IEmote prelanguageEmote,
		LanguageInfo language,
		IEmote optionalOutput,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(visibility, style, flags)
	{
		_preLanguageEmote = prelanguageEmote;
		DefaultSource = _preLanguageEmote.Source;
		_languageText = language;
		_optionalEmote = optionalOutput;

		AllValid = _preLanguageEmote.Valid & (_optionalEmote?.Valid != false);
	}

	public IPerceivable DefaultSource { get; protected set; }

	public bool AllValid { get; protected set; }

		public override string RawString =>
				((_preLanguageEmote?.RawText ?? string.Empty) +
				 (_optionalEmote != null && !string.IsNullOrWhiteSpace(_optionalEmote.RawText)
						 ? ", " + _optionalEmote.RawText
						 : string.Empty) +
				 _languageText.RawText).Trim();

	public override string ParseFor(IPerceiver perceiver)
	{
				if (perceiver is not ILanguagePerceiver langperceiver)
				{
						return ((_preLanguageEmote != null ? _preLanguageEmote.ParseFor(perceiver) : "") +
								(_optionalEmote != null && _optionalEmote.RawText.Length > 0
										? ", " + _optionalEmote.ParseFor(perceiver)
										: "") +
								_languageText.RawText).Proper();
				}

		// TODO - how should perceivers without language see language output?
		return
			((_preLanguageEmote != null ? _preLanguageEmote.ParseFor(perceiver) : "") +
			 (_optionalEmote != null && _optionalEmote.RawText.Length > 0
				 ? ", " + _optionalEmote.ParseFor(perceiver)
				 : "") +
			 _languageText.ParseFor(langperceiver)).Proper();
	}

	public override bool ShouldSee(IPerceiver perceiver)
	{
		return
			base.ShouldSee(perceiver) &&
			(!Flags.HasFlag(OutputFlags.SuppressObscured) || (_languageText.Form == LanguageForm.Spoken
				? perceiver.CanHear(DefaultSource) || perceiver.CanSee(DefaultSource)
				: perceiver.CanSee(DefaultSource))) &&
			(!Flags.HasFlag(OutputFlags.SuppressSource) || perceiver != DefaultSource)
			;
	}
}

public class PriorLanguageOutput : Output
{
	protected LanguageInfo _languageText;
	protected IEmote _optionalEmote;
	protected IEmote _preLanguageEmote;

	public PriorLanguageOutput(IEmote prelanguageEmote,
		LanguageInfo language,
		IEmote optionalOutput,
		OutputVisibility visibility = OutputVisibility.Normal,
		OutputStyle style = OutputStyle.Normal,
		OutputFlags flags = OutputFlags.Normal)
		: base(visibility, style, flags)
	{
		_preLanguageEmote = prelanguageEmote;
		DefaultSource = _preLanguageEmote.Source;
		_languageText = language;
		_optionalEmote = optionalOutput;

		AllValid = _preLanguageEmote.Valid & (_optionalEmote?.Valid != false);
	}

	public IPerceivable DefaultSource { get; protected set; }

	public bool AllValid { get; protected set; }

		public override string RawString =>
				((_optionalEmote != null && !string.IsNullOrWhiteSpace(_optionalEmote.RawText)
						? _optionalEmote.RawText + ", "
						: string.Empty) +
				 (_preLanguageEmote?.RawText ?? string.Empty) +
				 _languageText.RawText).Trim();

	public override string ParseFor(IPerceiver perceiver)
	{
				if (perceiver is not ILanguagePerceiver langperceiver)
				{
						var sb2 = new StringBuilder();
						if (!string.IsNullOrWhiteSpace(_optionalEmote?.RawText))
						{
								sb2.Append(_optionalEmote.ParseFor(perceiver) + ", ");
						}

						if (!string.IsNullOrWhiteSpace(_preLanguageEmote?.RawText))
						{
								sb2.Append(_preLanguageEmote.ParseFor(perceiver));
						}

						sb2.Append(_languageText.RawText);
						return sb2.ToString().ProperSentences();
				}

		// TODO - how should perceivers without language see language output?
		var sb = new StringBuilder();
		if (!string.IsNullOrWhiteSpace(_optionalEmote?.RawText))
		{
			sb.Append(_optionalEmote.ParseFor(perceiver) + ", ");
		}

		if (!string.IsNullOrWhiteSpace(_preLanguageEmote?.RawText))
		{
			sb.Append(_preLanguageEmote.ParseFor(perceiver));
		}

		sb.Append(_languageText.ParseFor(langperceiver));
		return sb.ToString().ProperSentences();
	}

	public override bool ShouldSee(IPerceiver perceiver)
	{
		return
			base.ShouldSee(perceiver) &&
			(!Flags.HasFlag(OutputFlags.SuppressObscured) || (_languageText.Form == LanguageForm.Spoken
				? perceiver.CanHear(DefaultSource) || perceiver.CanSee(DefaultSource)
				: perceiver.CanSee(DefaultSource))) &&
			(!Flags.HasFlag(OutputFlags.SuppressSource) || perceiver != DefaultSource)
			;
	}
}