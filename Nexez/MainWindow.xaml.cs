using Microsoft.Win32;
using Nexus.Ui.Console;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace Nexez
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// If true the app will close
        /// </summary>
        static bool appShouldCloSe = false;
        /// <summary>
        /// The timer
        /// </summary>
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            Opacity = 0.01;
            Show();
            CenterWindow();
            // Set the owner to the WPF window
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr windowHandle = helper.Handle;
            if (windowHandle != IntPtr.Zero)
            {
                DarkMode.DarkModeInterop.UseImmersiveDarkMode(windowHandle, true);
            }
            //Set the console output
            Console.SetOut(new ConsoleStreamWriter(TextBoxOutput));
            //create a timer
            CreateTimer();
            ProgressBarLoading.Opacity = 0.0;
            ProgressBarAI.Opacity = 0.0;
            //add tab styles
            ApplyTabStyles();

            var  text = LoadTextFromFile();
            if (text != null && text != string.Empty)
            {
                TextboxGptModel.Text = text;
            }
        }

        /// <summary>
        /// Handles the click event of the Setup Python button.
        /// </summary>
        private async void ButtonSetupPython_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (ProgressBarAI.Opacity == 1.0 || ProgressBarLoading.Opacity == 1.0)
                {
                    Console.WriteLine("A task is in process please wait for it to finish.\n");
                    return;
                }
                await ShowProgressBar();
                string[] splitArray = new string[0];
                var text = TextBoxLibraries.Text;
                if (text == null || text == string.Empty || text.Trim() == string.Empty)
                {
                    Console.WriteLine("No Addition Libraries To Install.\n");
                }
                else
                {
                    splitArray = text.Split(',');
                    for (int i = 0; i < splitArray.Length; i++)
                    {
                        splitArray[i] = splitArray[i].Trim();
                    }
                }

                ProgressBarLoading.Opacity = 1.0;
                // Execute the Python setup code
                try
                {
                    Task t = Task.Run(() => { PythonUtilities.SetupPython(splitArray); });
                    await t;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                ProgressBarLoading.Opacity = 0.0;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Handles the click event of the Run Script button.
        /// </summary>
        private async void ButtonRunScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (ProgressBarAI.Opacity == 1.0 || ProgressBarLoading.Opacity == 1.0)
                {
                    Console.WriteLine("A task is in process please wait for it to finish.\n");
                    return;
                }
                ProgressBarLoading.Opacity = 1.0;
                // Create a FlowDocument
                var text = new TextRange(TextBoxCode.Document.ContentStart, TextBoxCode.Document.ContentEnd).Text; ;
                // Get the Python script code from the TextBoxCode
                string verbatimString = $@"{text}";

                // Execute the Python script
                Task t = Task.Run(() => { PythonCommandLineExecution.RunPythonScript(verbatimString); });
                await t;

                ProgressBarLoading.Opacity = 0.0;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Handles the click event of the Load Script button.
        /// </summary>
        private  void ButtonLoadScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProgressBarAI.Opacity == 1.0 || ProgressBarLoading.Opacity == 1.0)
                {
                    Console.WriteLine("A task is in process please wait for it to finish.\n");
                    return;
                }
                ProgressBarLoading.Opacity = 1.0;

                // Get the path to the scripts folder
                string scriptsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "scripts");

                // Create the scripts folder if it doesn't exist
                if (!Directory.Exists(scriptsFolderPath))
                {
                    Directory.CreateDirectory(scriptsFolderPath);
                }

                // Open a file dialog to select a script file
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = scriptsFolderPath;
                openFileDialog.Filter = "Python Script Files (*.py)|*.py|All Files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    string selectedFilePath = openFileDialog.FileName;

                    // Load the content of the selected script file to the TextBoxCode
                    try
                    {
                        string scriptContent = File.ReadAllText(selectedFilePath);
                        // Set RichTextBox text programmatically
                        TextBoxCode.Document.Blocks.Clear();
                        TextBoxCode.Document.Blocks.Add(new Paragraph(new Run(scriptContent)));

                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"Error reading the script file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                ProgressBarLoading.Opacity = 0.0;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Handles the click event of the Save Script button.
        /// </summary>
        private void ButtonSaveScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProgressBarAI.Opacity == 1.0 || ProgressBarLoading.Opacity == 1.0)
                {
                    Console.WriteLine("A task is in process please wait for it to finish.\n");
                    return;
                }
                ProgressBarLoading.Opacity = 1.0;

                // Get the path to the scripts folder
                string scriptsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "scripts");

                // Create the scripts folder if it doesn't exist
                if (!Directory.Exists(scriptsFolderPath))
                {
                    Directory.CreateDirectory(scriptsFolderPath);
                }

                // Open a file dialog to specify the save location and file name
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = scriptsFolderPath;
                saveFileDialog.Filter = "Python Script Files (*.py)|*.py|All Files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    string saveFilePath = saveFileDialog.FileName;

                    try
                    {
                        // Get RichTextBox text programmatically
                        string richText = new TextRange(TextBoxCode.Document.ContentStart, TextBoxCode.Document.ContentEnd).Text;
                        // Save the content of the TextBoxCode to the selected file
                        File.WriteAllText(saveFilePath, richText);
                        MessageBox.Show("Script saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"Error saving the script file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                ProgressBarLoading.Opacity = 0.0;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Handles the click event of the Load Libraries button.
        /// </summary>
        private void ButtonLoadLibraries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProgressBarAI.Opacity == 1.0 || ProgressBarLoading.Opacity == 1.0)
                {
                    Console.WriteLine("A task is in process please wait for it to finish.\n");
                    return;
                }
                ProgressBarLoading.Opacity = 1.0;

                // Get the path to the libraries folder
                string librariesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "libraries");

                // Create the libraries folder if it doesn't exist
                if (!Directory.Exists(librariesFolderPath))
                {
                    Directory.CreateDirectory(librariesFolderPath);
                }

                // Open a file dialog to select a libraries file
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = librariesFolderPath;
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                // Show the OpenFileDialog and check if the user selected a file
                if (openFileDialog.ShowDialog() == true)
                {
                    // Read the contents of the selected file and set it to the TextBoxLibraries
                    TextBoxLibraries.Text = ReadTextFromFile(openFileDialog.FileName);
                }

                ProgressBarLoading.Opacity = 0.0;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Handles the click event of the Save Libraries button.
        /// </summary>
        private void ButtonSaveLibraries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProgressBarAI.Opacity == 1.0 || ProgressBarLoading.Opacity == 1.0)
                {
                    Console.WriteLine("A task is in process please wait for it to finish.\n");
                    return;
                }
                ProgressBarLoading.Opacity = 1.0;

                // Get the path to the libraries folder
                string librariesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "libraries");

                // Create the libraries folder if it doesn't exist
                if (!Directory.Exists(librariesFolderPath))
                {
                    Directory.CreateDirectory(librariesFolderPath);
                }

                // Open a SaveFileDialog to specify the save location and file name
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = librariesFolderPath;
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;

                // Show the SaveFileDialog and check if the user selected a file
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Write the contents of the TextBoxLibraries to the selected file
                    WriteTextToFile(saveFileDialog.FileName, TextBoxLibraries.Text);
                }

                ProgressBarLoading.Opacity = 0.0;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Create a timer
        /// </summary>
        private void CreateTimer()
        {
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
            Python.Deployment.Installer.InstallPath = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + ".");
            Python.Deployment.Installer.LogMessage += Console.WriteLine; // Redirect installer messages to console
                                                                         //hide the loading bar
        }
        /// <summary>
        /// Apply Styles to the tabs
        /// </summary>
        private void ApplyTabStyles()
        {
            // System highlight color when selected
            // Create style for TabControl
            Style tabControlStyle = new Style(typeof(TabControl));
            tabControlStyle.Setters.Add(new Setter(TabControl.BackgroundProperty, new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22)))); // Dark gray background
            tabControlStyle.Setters.Add(new Setter(TabControl.ForegroundProperty, Brushes.White)); // White foreground

            // Create style for TabItem
            Style tabItemStyle = new Style(typeof(TabItem));
            tabItemStyle.Setters.Add(new Setter(TabItem.BackgroundProperty, new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22)))); // Dark gray background
            tabItemStyle.Setters.Add(new Setter(TabItem.ForegroundProperty, Brushes.White)); // White foreground

            // Create a trigger for TabItem IsSelected property
            DataTrigger isSelectedTrigger = new DataTrigger
            {
                Binding = new Binding("IsSelected") { RelativeSource = RelativeSource.Self },
                Value = true
            };
            isSelectedTrigger.Setters.Add(new Setter(TabItem.BackgroundProperty, new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33)))); // Slightly lighter gray when selected
            isSelectedTrigger.Setters.Add(new Setter(TabItem.ForegroundProperty, Brushes.Black)); // White foreground

            // Create a trigger for TabItem IsMouseOver property
            DataTrigger isMouseOverTrigger = new DataTrigger
            {
                Binding = new Binding("IsMouseOver") { RelativeSource = RelativeSource.Self },
                Value = true
            };
            isMouseOverTrigger.Setters.Add(new Setter(TabItem.BackgroundProperty, new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x44, 0x44)))); // Slightly lighter gray when mouse over
            isMouseOverTrigger.Setters.Add(new Setter(TabItem.ForegroundProperty, Brushes.Black)); // White foreground

            // Add triggers to TabItem style
            tabItemStyle.Triggers.Add(isSelectedTrigger);
            tabItemStyle.Triggers.Add(isMouseOverTrigger);

            // Apply styles to TabControl and TabItems
            MyTabControl.Style = tabControlStyle;
            foreach (TabItem tabItem in MyTabControl.Items)
            {
                tabItem.Style = tabItemStyle;
            }
        }

        /// <summary>
        /// Handles the Tick event of the timer.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Gradually increase the opacity of the window
                if (Opacity < 1.0)
                {
                    Opacity += 0.01;
                }

                // Close the application if appShouldClose is true
                if (appShouldCloSe)
                {
                    Close();
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Centers the window on the screen.
        /// </summary>
        private void CenterWindow()
        {
            try
            {
                // Calculate the center of the screen
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;

                // Calculate the new position of the window
                Left = (screenWidth - Width) / 2;
                Top = (screenHeight - Height) / 2;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Shows the progress bar for a certain duration.
        /// </summary>
        private async Task ShowProgressBar()
        {
            ProgressBarLoading.Opacity = 1.0;
            timer.Start();
            await Task.Delay(5000); // 5 seconds
            ProgressBarLoading.Opacity = 0.0;
        }
        /// <summary>
        /// Reads the contents of a text file.
        /// </summary>
        /// <param name="filePath">The path to the text file.</param>
        /// <returns>The contents of the text file as a string.</returns>
        private string ReadTextFromFile(string filePath)
        {
            try
            {
                // Read the contents of the file
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return string.Empty;
            }
        }

        /// <summary>
        /// Writes text to a text file.
        /// </summary>
        /// <param name="filePath">The path to the text file.</param>
        /// <param name="text">The text to write to the file.</param>
        private void WriteTextToFile(string filePath, string text)
        {
            try
            {
                // Write the text to the file
                File.WriteAllText(filePath, text);
                MessageBox.Show("Libraries saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving libraries: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        bool hasRun = false;
        /// <summary>
        /// This is called when the user clicks send
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextboxGptModel.Text != string.Empty)
                {
                    SaveTextToFile(TextboxGptModel.Text);
                    if (ProgressBarAI.Opacity == 1.0 || ProgressBarLoading.Opacity == 1.0)
                    {
                        Console.WriteLine("A task is in process please wait for it to finish.\n");
                        return;
                    }
                    ProgressBarAI.Opacity = 1.0;
                    var input = TextBoxHumanInput.Text;
                    // Update the UI with the response
                    TextBoxAIResponse.Text += input.ToString() + "\n";
                    TextBoxHumanInput.Text = "";
                    var gptPath = TextboxGptModel.Text;
                    var modelPath = Path.Combine(Environment.CurrentDirectory, $"models\\{gptPath}\\{gptPath}.onnx");
                    if (File.Exists(modelPath))
                    {
                        // Asynchronously get the response
                        StringBuilder result = await Task.Run(() =>
                        {


                            var myinferenceSession = OnnxInferenceSession.GetInstance(modelPath);
                            if (hasRun == false)
                            {
                                hasRun = true;
                                myinferenceSession.GetResponse($"User: Hello Ai Assistant: Hello How Can I Assist You?* User: I have a question or comment? Ai Assistant: Please go ahead and ask User: What is the fastest car?");
                                myinferenceSession.GetResponse($"User: What is the Capital of the U.S.? Ai Assistant: The Capital of the U.S. is Washington DC User: What is 4 + 4? Ai Assistant: 4 + 4 = 8 User: What is the Capital of the Russia?");
                                myinferenceSession.GetResponse($"You are a helpful knowledgable Ai Assistant that never fails to answer a question.");
                                myinferenceSession.GetResponse($"The capital of canada is ottowa.");
                            }
                            return myinferenceSession.GetResponse($"User: {input}.\n");
                        });
                        var response = StringTrimmer.TrimPrefix(StringTrimmer.TrimSuffix(result.ToString().Trim()));
                        // Update the UI with the response
                        TextBoxAIResponse.Text += response + "\n";
                    } 
                    else
                    {
                        Console.WriteLine($"Model not found {modelPath}");
                    }
                        ProgressBarAI.Opacity = 0.0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving libraries: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to save text to a file
        private void SaveTextToFile(string text)
        {
            // Get the path to the directory where the executable is located
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Construct the path to the file within the GptModel folder
            string filePath = Path.Combine(exeDirectory, "GptModel", "file.txt");

            try
            {
                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Save the text to the file
                File.WriteAllText(filePath, text);
            }
            catch (Exception ex)
            {
                // Handle any errors here, such as displaying a message to the user
                Console.WriteLine("Error saving text: " + ex.Message);
            }
        }
        private string LoadTextFromFile()
        {
            // Construct the file path dynamically to point to the GptModel folder
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GptModel");
            string fileName = "file.txt"; // This should match the file name used in SaveTextToFile
            string filePath = Path.Combine(folderPath, fileName);

            try
            {
                // Check if the file exists before trying to read
                if (File.Exists(filePath))
                {
                    // Read and return the text from the file
                    return File.ReadAllText(filePath);
                }
                else
                {
                    Console.WriteLine("File not found.");
                    return ""; // Or handle the absence of the file as appropriate
                }
            }
            catch (Exception ex)
            {
                // Handle any errors here
                Console.WriteLine("Error loading text: " + ex.Message);
                return ""; // Return an empty string or appropriate error message
            }
        }

    }
}