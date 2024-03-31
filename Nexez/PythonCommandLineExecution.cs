using Python.Runtime;
using System;
using System.Diagnostics;
using System.IO;

namespace Nexez
{
    /// <summary>
    /// Class for executing Python scripts via command line and managing Python modules.
    /// </summary>
    public class PythonCommandLineExecution
    {
        /// <summary>
        /// Executes a Python script via command line.
        /// </summary>
        /// <param name="pythonCode">Python code to be executed.</param>
        public static void RunPythonScript(string pythonCode)
        {
            // Save the Python code to a temporary file
            string scriptFilePath = Path.Combine(Path.GetTempPath(), "temp_script.py");
            File.WriteAllText(scriptFilePath, pythonCode);

            // Find the path to the local Python executable recursively
            string pythonExecutablePath = FindPythonExecutable(Environment.CurrentDirectory);

            // Infer the path to the Python environment from the directory of the executable
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string pythonEnvironmentPath = Path.GetDirectoryName(exePath);

            // Run the Python script via command line
            if (!string.IsNullOrEmpty(pythonExecutablePath))
            {
                var process = new Process();
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                    psi.WorkingDirectory = pythonEnvironmentPath;
                    psi.RedirectStandardInput = true;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;

                    process.StartInfo = psi;
                    process.Start();
                    process.StandardInput.WriteLine($"\"{pythonExecutablePath}\" \"{scriptFilePath}\"");
                    process.StandardInput.Flush();
                    process.StandardInput.Close();

                    string output = process.StandardOutput.ReadToEnd();
                    Console.WriteLine("Python script output:");
                    Console.WriteLine(output);

                    string erroroutput = process.StandardError.ReadToEnd();
                    Console.WriteLine("Python Error output:");
                    Console.WriteLine(erroroutput);

                    process.WaitForExit();
                }
                finally
                {
                    // Ensure the process is properly disposed
                    process?.Dispose();
                }
            }
            else
            {
                Console.WriteLine("Python executable not found.");
            }

            // Delete the temporary Python script file
            File.Delete(scriptFilePath);
        }

        /// <summary>
        /// Recursively finds the path to the Python executable.
        /// </summary>
        /// <param name="directory">Directory to search for the Python executable.</param>
        /// <returns>Path to the Python executable if found, otherwise null.</returns>
        public static string FindPythonExecutable(string directory)
        {
            try
            {
                string[] files = Directory.GetFiles(directory, "python.exe");
                if (files.Length > 0)
                {
                    return files[0];
                }

                string[] subDirectories = Directory.GetDirectories(directory);
                foreach (string subDirectory in subDirectories)
                {
                    string pythonExecutable = FindPythonExecutable(subDirectory);
                    if (!string.IsNullOrEmpty(pythonExecutable))
                    {
                        return pythonExecutable;
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions
            }
            return null;
        }

        /// <summary>
        /// Installs and imports a Python module.
        /// </summary>
        /// <param name="moduleName">Name of the Python module to install and import.</param>
        /// <returns>The imported Python module if successful, otherwise null.</returns>
        public static dynamic InstallAndImportPythonModule(string moduleName)
        {
            // Attempt to import the module
            dynamic importedModule = null;

            using (Py.GIL()) // Acquire the Python Global Interpreter Lock
            {
                try
                {
                    importedModule = Py.Import(moduleName);
                    Console.WriteLine($"{moduleName} module imported successfully.");
                }
                catch (PythonException e)
                {
                    Console.WriteLine($"Error importing {moduleName}: {e.Message}");
                    return null; // Exit the function if import fails
                }

                // If the module was imported successfully, try to print its version
                if (importedModule != null)
                {
                    try
                    {
                        PyObject version = importedModule.GetAttr("__version__");
                        Console.WriteLine($"{moduleName} version: {version}");
                    }
                    catch (PythonException)
                    {
                        Console.WriteLine($"The {moduleName} module does not have a '__version__' attribute.");
                        // Optionally, perform some fallback action or log the absence of the version attribute
                    }
                }
            }
            return importedModule;

            // Consider shutting down PythonEngine only if you're done using Python for the rest of your application.
            // PythonEngine.Shutdown();
        }
    }
}
