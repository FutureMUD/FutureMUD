using MudSharp.Communication.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castle.DynamicProxy.Contributors;
using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Framework
{
    public static partial class StringUtilities
    {
        public static string HMark = " \x1B[1;33m-\x1B[0;39m ";
        public static string Indent = "   ";

        /// <summary>
        /// Form is #N, e.g. #2, #F, #0
        /// </summary>
        private static readonly Regex _substituteANSIColourRegex = new(@"(?<=(?:##){0,})#([0-9A-I#])", RegexOptions.IgnoreCase);
		/// <summary>
		/// Form is #`r;g;b; for example #`156;220;254;
		/// </summary>
		private static readonly Regex _substituteANSIColourRGBRegex = new(@"(?<=(?:##){0,})#`(?<red>[0-9]+);(?<green>[0-9]+);(?<blue>[0-9]+);", RegexOptions.IgnoreCase);
		private static readonly Regex _stripANSIRegex = new(@"\e\[(.*?)m");
        // Form is: writing{language,script,style=???,colour=???,skill>???}{text if can understand}{optional text if can't understand}
        private static readonly Regex LanguageReplacementRegex = new(@"writing\{(?<details>[a-z0-9 ,><=\.\-]+)\}\{(?<text>[^}]+)\}(?:\{(?<alt>[^}]+)\}){0,1}", RegexOptions.IgnoreCase);
        private static readonly Regex LanguageReplacementAttributeRegex = new(@"(?<attr>[a-z0-9 \-]+)(?<operator>[=><]+)(?<value>[a-z0-9 -]+)", RegexOptions.IgnoreCase);
        private static readonly Regex CheckReplacementRegex = new(@"check\{(?<trait>[^,]+),(?<difficulty>[0-9\.\,]+)\}\{(?<text>[^}]+)\}(?:\{(?<alt>[^}]+)\}){0,1}", RegexOptions.IgnoreCase);
        /// <summary>
        /// This function produces an ascii-text table surrounding the supplied data
        /// </summary>
        /// <param name="data">An IEnumerable of rows, each containing an IEnumerable of string column data</param>
        /// <param name="header">An IEnumerable of column heading strings</param>
        /// <param name="voyeur">The person viewing the table</param>
        /// <param name="colour">The ANSIColour to be applied to the ascii table dividers, if colour is being used. Otherwise null.</param>
        /// <param name="truncatableColumnIndex">The index of the column that is preferred to be the one truncated. If not specified, defaults to the longest one.</param>
        /// <param name="denseTable">Get rid of the spaces around values in the table to make it denser</param>
        /// <returns>An ascii text representation of a table containing the specified data</returns>
        public static string GetTextTable(IEnumerable<IEnumerable<string>> data, IEnumerable<string> header, IHaveAccount voyeur,
            ANSIColour colour = null, int truncatableColumnIndex = -1, bool denseTable = false)
        {
            return GetTextTable(data, header, voyeur.Account.LineFormatLength, colour != null, colour,
                truncatableColumnIndex, voyeur.Account.UseUnicode, denseTable);
        }

        /// <summary>
            ///     This function produces an ascii-text table surrounding the supplied data
            /// </summary>
            /// <param name="data">An IEnumerable of rows, each containing an IEnumerable of string column data</param>
            /// <param name="header">An IEnumerable of column heading strings</param>
            /// <param name="maxwidth">The maximum width of the table - if the table width would exceed this, columns will be truncated</param>
            /// <param name="bColourTable">Specifies whether the ascii representing the table dividers is to be coloured</param>
            /// <param name="colour">The ANSIColour to be applied to the ascii table dividers, if colour is being used. Otherwise null.</param>
            /// <param name="truncatableColumnIndex">The index of the column that is preferred to be the one truncated. If not specified, defaults to the longest one.</param>
            /// <param name="unicodeTable">Use unicode table characters instead of ASCII ones</param>
            /// <param name="denseTable">Get rid of the spaces around values in the table to make it denser</param>
            /// <returns>An ascii text representation of a table containing the specified data</returns>
            public static string GetTextTable(IEnumerable<IEnumerable<string>> data, IEnumerable<string> header,
            int maxwidth, bool bColourTable = true, ANSIColour colour = null, int truncatableColumnIndex = -1, bool unicodeTable = false, bool denseTable = false)
        {
            var sb = new StringBuilder();

            if (bColourTable && (colour == null))
            {
                colour = Telnet.Green;
            }

            int[] index = { 0 };
            var headerList = header as IList<string> ?? header.ToList();
            var columnWidths = headerList.ToDictionary(hdtxt => index[0]++, hdtxt => hdtxt.RawTextLength());
            var dataList = data as IList<IEnumerable<string>> ?? data.ToList();
            var tableCharacterWidth = headerList.Count + 1;
            var truncateLength = -1;

            foreach (var row in dataList)
            {
                index[0] = 0;
                foreach (var col in row.TakeWhile(col => columnWidths.ContainsKey(index[0])))
                {
                    columnWidths[index[0]] = Math.Max(col.RawTextLength(), columnWidths[index[0]]);
                    index[0]++;
                }
            }

            if (columnWidths.Sum(x => x.Value + (denseTable ? 0 : 2)) + tableCharacterWidth > maxwidth)
            {
                if (truncatableColumnIndex == -1)
                {
                    truncatableColumnIndex = columnWidths.FirstMax(x => x.Value).Key;
                }

                truncateLength = columnWidths[truncatableColumnIndex] -
                                 (columnWidths.Sum(x => x.Value + (denseTable ? 0 : 2)) + tableCharacterWidth - maxwidth) - 2;
                if (truncateLength < 5)
                {
                    truncateLength = -1;
                }
                else
                {
                    columnWidths[truncatableColumnIndex] = truncateLength;
                }
            }

            var ulsep = $"{(bColourTable ? colour.ToString() : "")}{(unicodeTable ? "╔" : "+")}";
            var ursep = $"{(unicodeTable ? "╗" : "+")}{(bColourTable ? colour.Reset() : "")}";
            var llsep = $"{(bColourTable ? colour.ToString() : "")}{(unicodeTable ? "╚" : "+")}";
            var lrsep = $"{(unicodeTable ? "╝" : "+")}{(bColourTable ? colour.Reset() : "")}";
            var lmsep = $"{(bColourTable ? colour.ToString() : "")}{(unicodeTable ? "╠" : "+")}";
            var rmsep = $"{(bColourTable ? colour.ToString() : "")}{(unicodeTable ? "╣" : "+")}";
            var topsep = $"{(unicodeTable ? "╦" : "+")}";
            var midsep = $"{(unicodeTable ? "╬" : "+")}";
            var bottomsep = $"{(unicodeTable ? "╩" : "+")}";
            var horizontal = unicodeTable ? '═' : '-';
            var vertical = $"{(bColourTable ? colour.ToString() : "")}{(unicodeTable ? "║" : "|")}{(bColourTable ? colour.Reset() : "")}";

            var topseparator = ulsep;
            var midseparator = lmsep;
            var bottomseparator = llsep;
            var headertext = vertical;

            var colIndex = 0;
            foreach (var col in columnWidths)
            {
	            topseparator += new string(horizontal, col.Value + (denseTable ? 0 : 2)) +
	                            (colIndex++ == (columnWidths.Count - 1) ? ursep : topsep);
	            midseparator += new string(horizontal, col.Value + (denseTable ? 0 : 2)) +
	                            (colIndex == columnWidths.Count ? rmsep : midsep);
	            bottomseparator += new string(horizontal, col.Value + (denseTable ? 0 : 2)) +
	                               (colIndex == columnWidths.Count ? lrsep : bottomsep);

                var value = headerList.ElementAt(col.Key);
                headertext += value.PadLeft(value.RawTextLength() + 1).PadRight((denseTable ? 0 : 2) + col.Value) + vertical;
            }

            topseparator += bColourTable ? colour.Reset() : "";
            midseparator += bColourTable ? colour.Reset() : "";
            bottomseparator += bColourTable ? colour.Reset() : "";

            sb.AppendLine(topseparator.NoWrap());
            sb.AppendLine(headertext.NoWrap());
            sb.AppendLine(midseparator.NoWrap());

            foreach (var row in dataList)
            {
                var rowList = row.ToList();
                var line = vertical.NoWrap();
                foreach (var col in columnWidths)
                {
                    var value = rowList.ElementAtOrDefault(col.Key) ?? "";

                    if ((truncateLength != -1) && (col.Key == truncatableColumnIndex) &&
                        (value.RawTextLength() > truncateLength))
                    {
                        var truncatedText = value.RawTextSubstring(0, truncateLength - 3) + "...";
                        line +=
                            truncatedText.RawTextPadLeft(truncatedText.RawTextLength() + 1)
                                .RawTextPadRight((denseTable ? 0 : 2) + truncateLength) + vertical;
                        continue;
                    }

                    line += value.RawTextPadLeft(value.RawTextLength() + 1).RawTextPadRight((denseTable ? 0 : 2) + col.Value) + vertical;
                }

                sb.AppendLine(line);
            }

            if (dataList.Count == 0)
            {
                var line = vertical;
                foreach (var col in columnWidths)
                {
                    line += "".RawTextPadLeft(1).RawTextPadRight((denseTable ? 0 : 2) + col.Value) + vertical;
                }
                sb.AppendLine(line);
            }

            sb.AppendLine(bottomseparator.NoWrap());
            return sb.ToString();
        }

        /// <summary>
        ///     Parses the string for colours specified using pound symbols and hexadecimal identifiers (e.g. #5a tall man#0) and
        ///     replaces them with ANSI colour sequences
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SubstituteANSIColour(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = _substituteANSIColourRGBRegex.Replace(input, match =>
            {
	            return $"\x1b[38;2;{int.Parse(match.Groups["red"].Value)};{int.Parse(match.Groups["green"].Value)};{int.Parse(match.Groups["blue"].Value)}m";
            });

            return _substituteANSIColourRegex.Replace(input, match => {
                switch (match.Groups[1].Value.ToLowerInvariant())
                {
                    case "0":
                        return Telnet.RESETALL;
                    case "1":
                        return Telnet.Red.ToString();
                    case "2":
                        return Telnet.Green.ToString();
                    case "3":
                        return Telnet.Yellow.ToString();
                    case "4":
                        return Telnet.Blue.ToString();
                    case "5":
                        return Telnet.Magenta.ToString();
                    case "6":
                        return Telnet.Cyan.ToString();
                    case "7":
                        return Telnet.BoldBlack.ToString();
                    case "8":
                        return Telnet.Orange.ToString();
                    case "9":
                        return Telnet.Red.Bold;
                    case "a":
                        return Telnet.Green.Bold;
                    case "b":
                        return Telnet.Yellow.Bold;
                    case "c":
                        return Telnet.Blue.Bold;
                    case "d":
                        return Telnet.Magenta.Bold;
                    case "e":
                        return Telnet.Cyan.Bold;
                    case "f":
                        return Telnet.White.Bold;
                    case "g":
                        return Telnet.Orange.Bold;
                    case "h":
                        return Telnet.Pink.Bold;
                    case "i":
                        return Telnet.Pink.ToString();
                    case "#":
                        return "#";
                    default:
                        return match.Groups[1].Value;
                }
            }
            );
        }

        public static string StripANSIColour(this string input)
        {
	        return _stripANSIRegex.Replace(input, string.Empty);
        }

        public static string StripANSIColour(this string input, bool strip)
        {
	        return strip ? input.StripANSIColour() : input;
        }

        /// <summary>
        /// Parses text looking for patterns like check{trait,minvalue}{you only live once}{something you don't understand}, and shows the appropriate text depending on the value of the trait.
        /// </summary>
        /// <param name="text">The text that is being parsed for pattern matching</param>
        /// <param name="voyeur">The observer for whom the language should be parsed</param>
        /// <param name="gameworld">The Gameworld for the perceiver</param>
        /// <returns>A parsed string with the appropriate marked up text for the voyeur</returns>
        public static string SubstituteCheckTrait(this string text, IPerceiver voyeur, IFuturemud gameworld)
        {
            var pht = voyeur as IPerceivableHaveTraits;
            text = CheckReplacementRegex.Replace(text, match =>
            {
                if (pht is null)
                {
                    return match.Groups["text"].Value;
                }

                var trait = gameworld.Traits.GetByIdOrName(match.Groups["trait"].Value);
                if (trait is null)
                {
                    return match.Groups["text"].Value;
                }

                if (!double.TryParse(match.Groups["value"].Value, out var value))
                {
                    return match.Groups["text"].Value;
                }

                if (pht.TraitValue(trait, TraitBonusContext.None) >= value)
                {
                    return match.Groups["text"].Value;
                }

                return match.Groups["alt"].Value ?? string.Empty;
            });
            return text;
        }

        /// <summary>
        /// Parses text looking for patterns like writing{english,latin}{you only live once}{something you don't understand}, and interprets them contextually as written language for the perceiver.
        /// </summary>
        /// <param name="text">The text that is being parsed for pattern matching</param>
        /// <param name="voyeur">The observer for whom the language should be parsed</param>
        /// <param name="gameworld">The Gameworld for the perceiver</param>
        /// <returns>A parsed string with the appropriate marked up text for the voyeur</returns>
        public static string SubstituteWrittenLanguage(this string text, IPerceiver voyeur, IFuturemud gameworld)
        {
            var languagePerceiver = voyeur as IHaveLanguage;

            text = LanguageReplacementRegex.Replace(text, match => {
                var split = match.Groups["details"].Value.Split(',');
                if (split.Length < 2 || languagePerceiver == null)
                {
                    return match.Groups["text"].Value;
                }

                var language = long.TryParse(split[0], out long value) ? gameworld.Languages.Get(value) : gameworld.Languages.GetByName(split[0]);
                if (language == null)
                {
                    return match.Groups["text"].Value;
                }

                var script = long.TryParse(split[1], out value) ? gameworld.Scripts.Get(value) : gameworld.Scripts.GetByName(split[1]);
                if (script == null)
                {
                    return match.Groups["text"].Value;
                }

                var requiredSkill = 0.0;
                var style = (WritingStyleDescriptors)gameworld.GetStaticInt("DefaultWritingStyleInText");
                var colour = gameworld.Colours.Get(gameworld.GetStaticLong("DefaultWritingColourInText"));
                var defaultStyleChanged = false;
                foreach (var item in split.Skip(2))
                {
                    var im = LanguageReplacementAttributeRegex.Match(item);
                    if (!im.Success)
                    {
                        continue;
                    }

                    switch (im.Groups["attr"].Value.ToLowerInvariant())
                    {
                        case "skill":
                            double.TryParse(im.Groups["value"].Value, out requiredSkill);
                            continue;
                        case "style":
                            if (!defaultStyleChanged)
                            {
                                style = WritingStyleDescriptors.None;
                                defaultStyleChanged = true;
                            }
                            style ^= WritingStyleDescriptors.None.Parse(im.Groups["value"].Value);
                            continue;
                        case "colour":
                        case "color":
                            colour = long.TryParse(im.Groups["value"].Value, out value) ? gameworld.Colours.Get(value) : gameworld.Colours.GetByName(im.Groups["value"].Value);
                            continue;
                        default:
                            continue;
                    }
                }

                var altText = match.Groups["alt"].Value;
                if (string.IsNullOrWhiteSpace(altText))
                {
                    altText = gameworld.GetStaticString("DefaultAlternateTextValue");
                }
                if (languagePerceiver.Languages.Contains(language) && languagePerceiver.Scripts.Contains(script))
                {
                    if ((languagePerceiver.GetTrait(language.LinkedTrait)?.Value ?? -1.0) >= requiredSkill)
                    {
                        return match.Groups["text"].Value.FluentTagMXP("send", $"href='look' hint='Language: {language.Name}, Script: {script.KnownScriptDescription}, Style: {style.Describe()}, Colour: {colour.Name}'");
                    }

                    return altText.FluentTagMXP("send", $"href='look' hint='Language: {language.Name}, Script: {script.KnownScriptDescription}, Style: {style.Describe()}, Colour: {colour.Name}. Skill not high enough to understand.'");
                }

                if (languagePerceiver.Scripts.Contains(script))
                {
                    return altText.FluentTagMXP("send", $"href='look' hint='Language: Unknown, Script: {script.KnownScriptDescription}, Style: {style.Describe()}, Colour: {colour.Name}'");
                }

                return altText.FluentTagMXP("send", $"href='look' hint='Language: Unknown, Script: {script.UnknownScriptDescription}, Style: {style.Describe()}, Colour: {colour.Name}'");
            });

            return text;
        }

        public static string DrawMap(ICharacter actor, int maxX, int maxY, ICell[,] cells, bool[,] hasNonCompass, bool[,] hasCartesianClashes, bool[,] hasBank, bool[,] hasShop, bool[,] hasAuctionHouse, bool[,] hasPlayers, bool[,] hasHostiles)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Map of the surrounds of {actor.Location.GetFriendlyReference(actor)}");
            sb.AppendLine();
            var unicode = actor.Account.UseUnicode;
            void DrawTopLine(int x, int y)
            {
                var cell = (x < maxX && y < maxY) ? cells[x, y] : default;

                var westernBorder = x == 0 || y >= maxY || cells[x - 1, y] is null;
                var northernBorder = y == 0 || x >= maxX || cells[x, y - 1] is null;
                var northwestBorder = (y == 0 || x == 0 || cells[x-1,y-1] is null);

                if (cell is null)
                {
                    if (westernBorder && northernBorder && northwestBorder)
                    {
                        sb.Append(" ");
                    }
                    else
                    {
                        sb.Append(Telnet.BoldWhite.Colour);
                        if (unicode)
                        {
                            if (northernBorder && westernBorder)
                            {
                                sb.Append("╝");
                            }
                            else if (northernBorder && northwestBorder)
                            {
                                sb.Append("╗");
                            }
                            else if (westernBorder && northwestBorder)
                            {
                                sb.Append("╚");
                            }
                            else if (westernBorder)
                            {
                                sb.Append("╩");
                            }
                            else if (northernBorder)
                            {
                                sb.Append("╣");
                            }
                            else
                            {
                                sb.Append("╬");
                            }
                        }
                        else
                        {
                            sb.Append("+");
                        }
                    }
                }
                else
                {
                    sb.Append(Telnet.BoldWhite.Colour);
                    if (unicode)
                    {
                        if (northernBorder && northwestBorder && westernBorder)
                        {
                            sb.Append("╔");
                        }
                        else if (northernBorder && northwestBorder)
                        {
                            sb.Append("╦");
                        }
                        else if (westernBorder && northwestBorder)
                        {
                            sb.Append("╠");
                        }
                        else
                        {
                            sb.Append("╬");
                        }
                    }
                    else
                    {
                        sb.Append("+");
                    }
                }

                if (cell is null)
                {
                    if (northernBorder)
                    {
                        sb.Append("         ");
                    }
                    else
                    {
                        if (unicode)
                        {
                            sb.Append("═════════");
                        }
                        else
                        {
                            sb.Append("---------");
                        }
                    }

                    return;
                }

                if (unicode)
                {
                    sb.Append("═══");
                }
                else
                {
                    sb.Append("---");
                }

                var northExit = cell.ExitsFor(actor, true)
                    .FirstOrDefault(exit => exit.OutboundDirection == CardinalDirection.North);
                if (northExit is null)
                {
                    if (unicode)
                    {
                        sb.Append("═══");
                    }
                    else
                    {
                        sb.Append("---");
                    }
                }
                else if (!northExit.Exit.AcceptsDoor)
                {
                    sb.Append("   ");
                }
                else if (northExit.Exit.Door is null)
                {
                    sb.Append("...");
                }
                else if (northExit.Exit.Door.IsOpen)
                {
                    if (unicode)
                    {
                        sb.Append("═");
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Yellow.Colour);
                        sb.Append("O");
                        sb.Append(Telnet.BoldWhite.Colour);
                        sb.Append("═");
                    }
                    else
                    {
                        sb.Append("-");
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Yellow.Colour);
                        sb.Append("O");
                        sb.Append(Telnet.BoldWhite.Colour);
                        sb.Append("-");
                    }
                }
                else if (northExit.Exit.Door.IsLocked)
                {
                    if (unicode)
                    {
                        sb.Append("═");
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Red.Colour);
                        sb.Append("L");
                        sb.Append(Telnet.BoldWhite.Colour);
                        sb.Append("═");
                    }
                    else
                    {
                        sb.Append("-");
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Red.Colour);
                        sb.Append("L");
                        sb.Append(Telnet.BoldWhite.Colour);
                        sb.Append("-");
                    }
                }
                else
                {
                    if (unicode)
                    {
                        sb.Append("═");
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Yellow.Colour);
                        sb.Append("C");
                        sb.Append(Telnet.BoldWhite.Colour);
                        sb.Append("═");
                    }
                    else
                    {
                        sb.Append("-");
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Yellow.Colour);
                        sb.Append("C");
                        sb.Append(Telnet.BoldWhite.Colour);
                        sb.Append("-");
                    }
                }
                if (unicode)
                {
                    sb.Append("═══");
                }
                else
                {
                    sb.Append("---");
                }
            }

            void Draw2ndLine(int x, int y)
            {
                var cell = (x < maxX && y < maxY) ? cells[x, y] : default;
                var drawWest = 
                    (cell is not null) ||
                    (x > 0 && y < maxY && cells[x - 1, y] is not null);
                sb.Append(Telnet.BoldWhite.Colour);
                if (drawWest)
                {
                    if (unicode)
                    {
                        sb.Append("║");
                    }
                    else
                    {
                        sb.Append("|");
                    }
                }
                else
                {
                    sb.Append(" ");
                }

                if (cell is null)
                {
                    sb.Append("         ");
                    return;
                }

                sb.Append("   ");

                sb.Append(Telnet.BoldYellow.Colour);
                if (hasAuctionHouse[x, y])
                {
                    sb.Append("A");
                }
                else
                {
                    sb.Append(" ");
                }
                if (hasBank[x, y])
                {
                    sb.Append("B");
                }
                else
                {
                    sb.Append(" ");
                }
                if (hasShop[x, y])
                {
                    sb.Append("S");
                }
                else
                {
                    sb.Append(" ");
                }

                sb.Append("   ");
            }

            void Draw3rdLine(int x, int y)
            {
                var cell = (x < maxX && y < maxY) ? cells[x, y] : default;
                var drawWest =
                    (cell is not null) ||
                    (x > 0 && y < maxY && cells[x - 1, y] is not null);
                sb.Append(Telnet.BoldWhite.Colour);
                if (drawWest)
                {
                    var westExit = cell is not null ? 
                        cell.ExitsFor(actor, true).FirstOrDefault(exit => exit.OutboundDirection == CardinalDirection.West) :
                        cells[x - 1, y].ExitsFor(actor, true).FirstOrDefault(x => x.OutboundDirection == CardinalDirection.East);

                    if (westExit is null)
                    {
                        if (unicode)
                        {
                            sb.Append("║");
                        }
                        else
                        {
                            sb.Append("|");
                        }
                    }
                    else if (!westExit.Exit.AcceptsDoor)
                    {
                        sb.Append(" ");
                    }
                    else if (westExit.Exit.Door is null)
                    {
                        sb.Append(":");
                    }
                    else if (westExit.Exit.Door.IsOpen)
                    {
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Yellow.Colour);
                        sb.Append("O");
                    }
                    else if (westExit.Exit.Door.IsLocked)
                    {
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Red.Colour);
                        if (unicode)
                        {
                            sb.Append("L");
                        }
                        else
                        {
                            sb.Append("L");
                        }
                    }
                    else
                    {
                        sb.Append(Telnet.RESETALL);
                        sb.Append(Telnet.Yellow.Colour);
                        if (unicode)
                        {
                            sb.Append("C");
                        }
                        else
                        {
                            sb.Append("C");
                        }
                    }
                }
                else
                {
                    sb.Append(" ");
                }

                if (cell is null)
                {
                    sb.Append("         ");
                    return;
                }

                sb.Append("  ");
                var terrain = cell.Terrain(actor);
                sb.Append(cell.Id.ToString("00000").ColourForegroundCustom(terrain.TerrainANSIColour));
                sb.Append("  ");
                sb.Append(Telnet.BoldWhite.Colour);
            }

            void Draw4thLine(int x, int y)
            {
                var cell = (x < maxX && y < maxY) ? cells[x, y] : default;
                var drawWest =
                    (cell is not null) ||
                    (x > 0 && y < maxY && cells[x - 1, y] is not null);
                sb.Append(Telnet.BoldWhite.Colour);
                if (drawWest)
                {
                    if (unicode)
                    {
                        sb.Append("║");
                    }
                    else
                    {
                        sb.Append("|");
                    }
                }
                else
                {
                    sb.Append(" ");
                }

                if (cell is null)
                {
                    sb.Append("         ");
                    return;
                }

                sb.Append(Telnet.BoldWhite.Colour);
                sb.Append("   ");

                if (hasNonCompass[x, y])
                {
                    sb.Append(Telnet.BoldBlue.Colour);
                    sb.Append("+");
                }
                else
                {
                    sb.Append(" ");
                }
                if (hasPlayers[x, y] && hasHostiles[x,y])
                {
                    sb.Append(Telnet.BoldOrange.Colour);
                    sb.Append("*");
                }
                else if (hasPlayers[x, y])
                {
                    sb.Append(Telnet.BoldPink.Colour);
                    sb.Append("P");
                }
                else if (hasHostiles[x, y])
                {
                    sb.Append(Telnet.BoldRed.Colour);
                    sb.Append("H");
                }
                else
                {
                    sb.Append(" ");
                }

                if (hasCartesianClashes[x, y])
                {
                    sb.Append(Telnet.BoldRed.Colour);
                    sb.Append("¢");
                }
                else
                {
                    sb.Append(" ");
                }

                sb.Append("   ");
            }

            for (var j = 0; j <= maxY; j++)
            {
                sb.Append(Telnet.NoWordWrap);
                for (var i = 0; i <= maxX; i++)
                {
                    DrawTopLine(i, j);
                }
                sb.AppendLine();
                sb.Append(Telnet.NoWordWrap);
                for (var i = 0; i <= maxX; i++)
                {
                    Draw2ndLine(i, j);
                }
                sb.AppendLine();
                sb.Append(Telnet.NoWordWrap);
                for (var i = 0; i <= maxX; i++)
                {
                    Draw3rdLine(i, j);
                }
                sb.AppendLine();
                sb.Append(Telnet.NoWordWrap);
                for (var i = 0; i <= maxX; i++)
                {
                    Draw4thLine(i, j);
                    
                }
                sb.AppendLine();
            }

            sb.Append(Telnet.RESETALL);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Legend:");
            sb.AppendLine();
            sb.AppendLine($"{"P".Colour(Telnet.BoldPink)} - a player is in this room");
            sb.AppendLine($"{"O".Colour(Telnet.Yellow)} - an open door");
            sb.AppendLine($"{"C".Colour(Telnet.Yellow)} - a closed door");
            sb.AppendLine($"{"L".Colour(Telnet.Red)} - a closed and locked door");
            if (hasHostiles.Cast<bool>().Any(x => x))
            {
                sb.AppendLine($"{"H".Colour(Telnet.BoldRed)} - hostile NPCs are in this room");
                sb.AppendLine($"{"*".Colour(Telnet.BoldOrange)} - hostile NPCs and a player are in this room");
            }

            if (hasCartesianClashes.Cast<bool>().Any(x => x))
            {
                sb.AppendLine(
                    $"{"¢".Colour(Telnet.BoldRed)} - this room had multiple matches with the same coordinates");
            }

            sb.AppendLine($"{"+".Colour(Telnet.BoldBlue)} - this room has non-compass exits (u/d/enter/leave etc)");

            if (hasAuctionHouse.Cast<bool>().Any(x => x))
            {
                sb.AppendLine(
                    $"{"A".Colour(Telnet.BoldYellow)} - this room is an auction house");
            }
            if (hasBank.Cast<bool>().Any(x => x))
            {
                sb.AppendLine(
                    $"{"B".Colour(Telnet.BoldYellow)} - this room is a bank branch");
            }
            if (hasShop.Cast<bool>().Any(x => x))
            {
                sb.AppendLine(
                    $"{"S".Colour(Telnet.BoldYellow)} - this room is a shop");
            }
            return sb.ToString().RemoveBlankLines();
        }
    }
}
