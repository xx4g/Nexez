using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


	namespace Nexus.Ui.Console
	{
		/// <summary>
		/// Custom TextWriter class that redirects the output to a TextBox in a WPF application.
		/// </summary>
		public class ConsoleStreamWriter : TextWriter
		{
			private readonly TextBox _output;

			/// <summary>
			/// Initializes a new instance of the TextBoxStreamWriter class with the specified TextBox.
			/// </summary>
			/// <param name="output">The TextBox to which the output will be redirected.</param>
			public ConsoleStreamWriter(TextBox output)
			{
				_output = output ?? throw new ArgumentNullException(nameof(output), "TextBox must not be null.");
			}

			/// <summary>
			/// Gets the encoding for this writer.
			/// </summary>
			public override Encoding Encoding => Encoding.UTF8;

			/// <summary>
			/// Writes a single character to the text box.
			/// </summary>
			/// <param name="value">The character to write to the text box.</param>
			public override void Write(char value)
			{
				_output.Dispatcher.Invoke(() => _output.AppendText(value.ToString()));
				_output.Dispatcher.Invoke(() => _output.ScrollToEnd());
			}

			/// <summary>
			/// Writes a string to the text box.
			/// </summary>
			/// <param name="value">The string to write to the text box.</param>
			public override void Write(string value)
			{
				_output.Dispatcher.Invoke(() => _output.AppendText(value));
				_output.Dispatcher.Invoke(() => _output.ScrollToEnd());
			}
		}
	}


