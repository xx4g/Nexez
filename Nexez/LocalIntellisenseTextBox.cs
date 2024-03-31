using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows;

namespace Nexez
{
    public class IntelliSenseTextBox : RichTextBox
    {
        private Dictionary<string, SolidColorBrush> keywordColors = new Dictionary<string, SolidColorBrush>
{
    // Control Flow
    {"if", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))}, // Light Blue
    {"else", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"elif", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"for", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"while", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"break", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"continue", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"return", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"yield", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"in", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"not", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"and", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"or", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    {"is", new SolidColorBrush(Color.FromArgb(255, 173, 216, 230))},
    // Types
    {"int", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))}, // Light Purple
    {"float", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))},
    {"str", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))},
    {"bool", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))},
    {"list", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))},
    {"dict", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))},
    {"tuple", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))},
    {"set", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))},
    {"None", new SolidColorBrush(Color.FromArgb(255, 221, 160, 221))}, // Python-specific NoneType
    // Function and Class Declarations
    {"def", new SolidColorBrush(Color.FromArgb(255, 144, 238, 144))}, // Light Green
    {"lambda", new SolidColorBrush(Color.FromArgb(255, 144, 238, 144))},
    {"class", new SolidColorBrush(Color.FromArgb(255, 144, 238, 144))},
    // Decorators
    {"@", new SolidColorBrush(Color.FromArgb(255, 255, 165, 0))}, // Light Orange
    // Imports
    {"import", new SolidColorBrush(Color.FromArgb(255, 255, 218, 185))}, // Light Coral
    {"from", new SolidColorBrush(Color.FromArgb(255, 255, 218, 185))},
    {"as", new SolidColorBrush(Color.FromArgb(255, 255, 218, 185))},
    // Exception Handling
    {"try", new SolidColorBrush(Color.FromArgb(255, 250, 128, 114))}, // Light Salmon
    {"except", new SolidColorBrush(Color.FromArgb(255, 250, 128, 114))},
    {"raise", new SolidColorBrush(Color.FromArgb(255, 250, 128, 114))},
    {"finally", new SolidColorBrush(Color.FromArgb(255, 250, 128, 114))},
    {"assert", new SolidColorBrush(Color.FromArgb(255, 250, 128, 114))},
    // Async
    {"async", new SolidColorBrush(Color.FromArgb(255, 240, 230, 140))}, // Light Khaki
    {"await", new SolidColorBrush(Color.FromArgb(255, 240, 230, 140))},
    // Variables and Scope
    {"global", new SolidColorBrush(Color.FromArgb(255, 216, 191, 216))}, // Light Thistle
    {"nonlocal", new SolidColorBrush(Color.FromArgb(255, 216, 191, 216))}, // Specific to Python
    // Boolean and Comparison
    {"True", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))}, // Light Cyan
    {"False", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))},
    {"<", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))},
    {">", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))},
    {"<=", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))},
    {">=", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))},
    {"==", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))},
    {"!=", new SolidColorBrush(Color.FromArgb(255, 224, 255, 255))},
    // Magic Methods and Properties
    {"__init__", new SolidColorBrush(Color.FromArgb(255, 255, 182, 193))}, // Light Pink
    {"__str__", new SolidColorBrush(Color.FromArgb(255, 255, 182, 193))},
    {"__repr__", new SolidColorBrush(Color.FromArgb(255, 255, 182, 193))},
    {"__call__", new SolidColorBrush(Color.FromArgb(255, 255, 182, 193))},
    {"__iter__", new SolidColorBrush(Color.FromArgb(255, 255, 182, 193))},
    {"__next__", new SolidColorBrush(Color.FromArgb(255, 255, 182, 193))}
        };


        public IntelliSenseTextBox()
        {
            this.TextChanged += (sender, e) => OnTextChangedAsync();
        }
        /// <summary>
        /// called when the text changes
        /// </summary>
        private async void OnTextChangedAsync()
        {
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var textWithPositions = GatherTextAndPositions();
                var formattingInstructions = AnalyzeTextForKeywords(textWithPositions);

                ApplyFormatting(formattingInstructions);
            });
        }
        /// <summary>
        /// Get the text positions
        /// </summary>
        /// <returns></returns>
        private List<(string text, TextPointer start, TextPointer end)> GatherTextAndPositions()
        {
            var result = new List<(string text, TextPointer start, TextPointer end)>();
            var pointer = this.Document.ContentStart;

            while (pointer != null && pointer.CompareTo(this.Document.ContentEnd) < 0)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var textRun = pointer.GetTextInRun(LogicalDirection.Forward);
                    var startPos = pointer;
                    var endPos = startPos?.GetPositionAtOffset(textRun.Length);

                    if (startPos != null && endPos != null)
                    {
                        result.Add((textRun, startPos, endPos));
                    }
                }

                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            return result;
        }
        /// <summary>
        /// Analyze the text using a keyword
        /// </summary>
        /// <param name="textWithPositions"></param>
        /// <returns></returns>
        private Dictionary<TextRange, SolidColorBrush> AnalyzeTextForKeywords(List<(string text, TextPointer start, TextPointer end)> textWithPositions)
        {
            var formattingInstructions = new Dictionary<TextRange, SolidColorBrush>();

            foreach (var (text, start, end) in textWithPositions)
            {
                foreach (var keyword in keywordColors.Keys)
                {
                    var matches = Regex.Matches(text, $@"\b{keyword}\b", RegexOptions.IgnoreCase);
                    foreach (Match match in matches)
                    {
                        var keywordStart = start.GetPositionAtOffset(match.Index);
                        var keywordEnd = keywordStart?.GetPositionAtOffset(match.Length);
                        if (keywordStart != null && keywordEnd != null)
                        {
                            var range = new TextRange(keywordStart, keywordEnd);
                            formattingInstructions.Add(range, keywordColors[keyword]);
                        }
                    }
                }
            }

            return formattingInstructions;
        }
        /// <summary>
        /// Apply the color to the text based one the dictionary
        /// </summary>
        /// <param name="formattingInstructions"></param>
        private void ApplyFormatting(Dictionary<TextRange, SolidColorBrush> formattingInstructions)
        {
            foreach (var instruction in formattingInstructions)
            {
                instruction.Key.ApplyPropertyValue(TextElement.ForegroundProperty, instruction.Value);
                instruction.Key.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }
    }
}
