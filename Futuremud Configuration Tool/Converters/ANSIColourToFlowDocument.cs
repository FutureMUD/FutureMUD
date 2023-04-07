using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Futuremud_Configuration_Tool.Converters {
    public static class ANSIColourToFlowDocument {
        private static System.Windows.Media.Brush GetBrush(int number, bool bold) {
            if (bold) {
                switch (number) {
                    case 0:
                        return System.Windows.Media.Brushes.Black;
                    case 1:
                        return System.Windows.Media.Brushes.Red;
                    case 2:
                        return System.Windows.Media.Brushes.Green;
                    case 3:
                        return System.Windows.Media.Brushes.Yellow;
                    case 4:
                        return System.Windows.Media.Brushes.Blue;
                    case 5:
                        return System.Windows.Media.Brushes.Magenta;
                    case 6:
                        return System.Windows.Media.Brushes.Cyan;
                    case 7:
                        return System.Windows.Media.Brushes.White;
                    default:
                        throw new NotSupportedException();
                }
            }
            else {
                switch (number) {
                    case 0:
                        return System.Windows.Media.Brushes.Black;
                    case 1:
                        return System.Windows.Media.Brushes.DarkRed;
                    case 2:
                        return System.Windows.Media.Brushes.DarkGreen;
                    case 3:
                        return System.Windows.Media.Brushes.SandyBrown;
                    case 4:
                        return System.Windows.Media.Brushes.DarkBlue;
                    case 5:
                        return System.Windows.Media.Brushes.DarkMagenta;
                    case 6:
                        return System.Windows.Media.Brushes.DarkCyan;
                    case 7:
                        return System.Windows.Media.Brushes.Gray;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private static string ANSIResetBlock = "\x1B[30m";
        private static readonly Regex ANSIColourRegex = new Regex("\x1B\\[(1;){0,1}3(\\d)m");

        public static FlowDocument Convert(string input) {
            var document = new FlowDocument();
            document.PageWidth = 1000;
            var lines = input.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var line in lines) {
                var paragraph = new Paragraph();
                paragraph.FontFamily = new System.Windows.Media.FontFamily("Courier New");
                paragraph.FontSize = 12;
                paragraph.Margin = new System.Windows.Thickness(0);
                var currentRun = new List<char>();
                bool inColourTag = false;
                bool boldColour = false;
                int colour = 0;
                var currentANSI = new List<char>();
                bool inANSI = false;

                foreach (var ch in line) {
                    if (inANSI && ch == 'm') {
                        currentANSI.Add(ch);
                        if (new string(currentANSI.ToArray()) == ANSIResetBlock && inColourTag && currentRun.Any()) {
                            var run = new Run(new string(currentRun.ToArray()));
                            run.Foreground = GetBrush(colour, boldColour);
                            paragraph.Inlines.Add(run);
                            colour = 0;
                            inColourTag = false;
                            boldColour = false;
                            inANSI = false;
                            currentANSI.Clear();
                            currentRun.Clear();
                            continue;
                        }

                        Match m = ANSIColourRegex.Match(new string(currentANSI.ToArray()));
                        if (m.Success) {
                            if (m.Groups[1].Success) {
                                boldColour = true;
                            }
                            colour = int.Parse(m.Groups[2].Value);
                            inColourTag = true;
                        }
                        inANSI = false;
                        currentANSI.Clear();
                        continue;
                    }

                    if (inANSI) {
                        currentANSI.Add(ch);
                        continue;
                    }

                    if (ch == '\x1B'){
                        if (currentRun.Any() && !inColourTag) {
                            var run = new Run(new string(currentRun.ToArray()));
                            paragraph.Inlines.Add(run);
                            currentRun.Clear();
                        }
                        
                        inANSI = true;
                        currentANSI.Add(ch);
                        continue;
                    }

                    currentRun.Add(ch);
                    continue;
                }
                if (currentRun.Any()) {
                    paragraph.Inlines.Add(new Run(new string(currentRun.ToArray())));
                }

                document.Blocks.Add(paragraph);
            }

            return document;
        }
    }
}
