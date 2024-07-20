using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MudSharp.Framework {
    /// <summary>
    ///     When a player is using MXP, this class contains information about which MXP tags are supported by their client
    /// </summary>
    public class MXPSupport {
        private static readonly Regex _supportRegex = new(@"\+(\w{1,})\.{0,1}(\w{0,})", RegexOptions.IgnoreCase);
        private readonly List<Tuple<string, string>> _supportOptions = new();

        private HashSet<string> _supportedTags = new();

        public MXPSupport() {
            UseMXP = false;
        }

        public MXPSupport(bool useMxp, params string[] supportedTags)
        {
	        UseMXP = useMxp;
	        foreach (var tag in supportedTags)
	        {
		        _supportedTags.Add(tag);
	        }
        }

        public bool UseMXP { get; set; }

        public bool SupportTag(string tag) {
            return UseMXP && _supportedTags.Contains(tag.ToLowerInvariant());
        }

        public void SetSupport(string response) {
            foreach (Match m in _supportRegex.Matches(response)) {
                _supportedTags.Add(m.Groups[1].Value.ToLowerInvariant());
                _supportOptions.Add(Tuple.Create(m.Groups[1].Value.ToLowerInvariant(),
                    m.Groups[2].Value.ToLowerInvariant()));
            }
            _supportedTags = _supportedTags.Distinct().ToHashSet();
        }

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
	        return $"Use MXP: {UseMXP} | Tags: {_supportedTags.ListToCompactString()}";
        }

        #endregion
    }

    /// <summary>
    ///     The MXP Class provides constants and routines to aide in the use of the MXP protocol
    /// </summary>
    public static class MXP {
        public const char BeginMXPChar = '\x03';
        public const string BeginMXP = "\x03";

        public const char EndMXPChar = '\x04';
        public const string EndMXP = "\x04";

        public const char AmpersandMXPChar = '\x05';
        public const string AmpersandMXP = "\x05";

        public const char QuoteMXPChar = '\x06';
        public const string QuoteMXP = "\x06";

        private static readonly Regex _mxpTagRegex = new("\x03(/{0,1})([a-z]{1,})([^\x04]{0,}?)\x04",
            RegexOptions.IgnoreCase);

        public static byte[] StartMXPBytes() {
            return Encoding.ASCII.GetBytes("\x1B[6z" + "<SUPPORT>");
        }

        /// <summary>
        ///     Fluent method to apply specified MXP tag and attributes to a block of text. If null is passed to the tag, it will
        ///     just return the text
        /// </summary>
        /// <param name="input">The string representing the block of text to be surrounded by the MXP tag</param>
        /// <param name="tag">The mxp tag you wish to apply, e.g. SEND, B, I, COLOR, etc.</param>
        /// <param name="attributes">Any attributes you wish to send along with the tag</param>
        /// <returns>The block of text surrounded by the MXP tag</returns>
        public static string FluentTagMXP(this string input, string tag, string attributes = "") {
            return tag != null
                ? TagMXP(tag + attributes.Replace('"', QuoteMXPChar).LeadingSpaceIfNotEmpty()) + input +
                  TagMXP("/" + tag)
                : input;
        }

        public static string MXPSend(this string input, string sendtext = "", string hinttext = "")
        {
	        if (string.IsNullOrEmpty(sendtext))
	        {
		        sendtext = input;
	        }
	        return input.FluentTagMXP("send", $"href='{sendtext.SanitiseMXP(null)}' hint='{hinttext?.SanitiseMXP(null) ?? $"Send the text {sendtext.SanitiseMXP(null)} to the MUD"}'");
        }

        /// <summary>
        ///     Creates a standalone MXP tag
        /// </summary>
        /// <param name="tag">The contents of the MXP tag</param>
        /// <returns>An MXP tag</returns>
        public static string TagMXP(string tag) {
            return BeginMXP + tag + EndMXP;
        }

        public static string ReplaceWebCharacters(this string input)
        {
	        return input
	               .Replace("&", "&amp;")
	               .Replace("\"", "&quot;")
	               .Replace("\'", "&apos;")
	               .Replace("<", "&lt;")
	               .Replace(">", "&gt;");
        }

        public static string StripMXP(this string input)
        {
	        return _mxpTagRegex.Replace(input, "");
		}

        public static string SanitiseMXP(this string input, MXPSupport support) {
            if (support?.UseMXP == false) {
                return _mxpTagRegex.Replace(input, "");
            }

            input = input
                .Replace("&", "&amp;")
                .Replace("\"", "&quot;")
                //.Replace("\'", "&apos;")
				.Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace(AmpersandMXP, "&")
                .Replace(QuoteMXP, "\"");

            var matches = _mxpTagRegex.Matches(input);
            return _mxpTagRegex.Replace(input,
                m =>
                    support?.SupportTag(m.Groups[2].Value) != false
                        ? "<" + m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value + ">"
                        : "");
        }
    }
}