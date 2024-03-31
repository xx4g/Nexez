using Python.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nexez
{
    internal class PythonUtilities
    {
        /// <summary>
        /// Sets up the Python environment.
        /// </summary>
        internal async static void SetupPython(string[] libraries)
        {
            try
            {
                int failCount = 0;
                bool succeeded = false;
                while (succeeded == false)
                {
                    if (failCount > 2)
                    {
                        Console.WriteLine("Error Installing, please restart the application");
                    }
                    failCount++;
                    succeeded = await InstallPythonInstance(libraries, succeeded);

                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
            finally
            {

                try
                {
                    // Shutdown PythonEngine and close application after delay
                    PythonEngine.Shutdown();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            Thread.Sleep(5000);
        }
        /// <summary>
        /// Install Python to the local folder
        /// </summary>
        /// <param name="libraries"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        internal static async Task<bool> InstallPythonInstance(string[] libraries, bool succeeded)
        {
            try
            {
                // Set up Python environment
                Python.Included.Installer.InstallPath = Environment.CurrentDirectory + ".";
                await Python.Included.Installer.SetupPython();
                Python.Included.Installer.LogMessage += Console.WriteLine;

                // Find Python executable path
                var exepath = PythonCommandLineExecution.FindPythonExecutable(Environment.CurrentDirectory);

                // If Python executable found, initialize PythonEngine and perform operations
                if (exepath != null)
                {
                    SetEnviromentPythonPath(exepath);

                    // Initialize PythonEngine
                    PythonEngine.Initialize();
                    succeeded = await InstallPython(libraries, succeeded);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return succeeded;
        }

        /// <summary>
        /// Sets the environment path for PYTHONUSERBASE
        /// </summary>
        /// <param name="exepath"></param>
        internal static void SetEnviromentPythonPath(string exepath)
        {
            // Set PYTHONUSERBASE environment variable
            var basePath = Directory.GetParent(exepath).FullName;
            string pythonLibPath = Path.Combine(basePath, "Lib");
            string sitePackagesPath = Path.Combine(pythonLibPath, "site-packages");
            Environment.SetEnvironmentVariable("PYTHONUSERBASE", basePath, EnvironmentVariableTarget.Process);
        }
        /// <summary>
        /// Installs python and modules using pip
        /// </summary>
        /// <param name="libraries"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        internal static async Task<bool> InstallPython(string[] libraries, bool succeeded)
        {
            try
            {

                using (Py.GIL())
                {
                    succeeded = await InstallModules(libraries, succeeded);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
            return succeeded;
        }
        /// <summary>
        /// Install all modules the user provided in the libraries textbox
        /// </summary>
        /// <param name="libraries"></param>
        /// <param name="succeeded"></param>
        /// <returns></returns>
        internal static async Task<bool> InstallModules(string[] libraries, bool succeeded)
        {
            // Import Python modules and perform operations
            dynamic sys = Py.Import("sys");
            dynamic os = Py.Import("os");

            dynamic site = Py.Import("site");
            LogPythonInfo(sys, os, site);

            // Perform Python operations and install required libraries
            await Python.Included.Installer.TryInstallPip();
            int successCount = 0;
            foreach (string library in libraries)
            {
                await InstallModule(library);
                dynamic pyModuleObject = PythonCommandLineExecution.InstallAndImportPythonModule(library);
                if (pyModuleObject != null)
                {
                    successCount++;
                }
            }
            if (successCount == libraries.Length)
            {
                succeeded = true;
            }

            return succeeded;
        }

        /// <summary>
        /// Logs Info About Python Install Folders
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="os"></param>
        /// <param name="site"></param>
        internal static void LogPythonInfo(dynamic sys, dynamic os, dynamic site)
        {
            // Display Python version information and paths
            Console.WriteLine($"Python executable: {sys.executable}");
            Console.WriteLine("Python version: " + sys.version);
            Console.WriteLine("Python version info: " + sys.version_info);
            Console.WriteLine("Python executable path: " + sys.executable);
            Console.WriteLine("Python prefix: " + sys.prefix);
            Console.WriteLine("Python base prefix: " + sys.base_prefix);
            Console.WriteLine("Python base executable: " + sys.base_exec_prefix);
            Console.WriteLine("Site packages directory: " + site.getsitepackages());
            Console.WriteLine("Standard library directories: " + sys.path);
            Console.WriteLine("User site packages directory: " + site.getusersitepackages());
            Console.WriteLine("User-specific configuration directory: " + site.getuserbase());
            Console.WriteLine("Home directory: " + os.path.expanduser("~"));
            Console.WriteLine("Temporary directory: " + os.path.abspath(os.path.join(os.sep, "tmp")));
        }

        /// <summary>
        /// Asynchronously installs a Python module using pip.
        /// </summary>
        /// <param name="library">The name of the Python module to install.</param>
        internal static async Task InstallModule(string library)
        {
            try
            {
                if (Python.Deployment.Installer.IsModuleInstalled(library) == false)
                {
                    Console.WriteLine($"Installing {library}.");
                    await Python.Deployment.Installer.PipInstallModule(library);
                    Console.WriteLine($"Installed {library}.");
                }
                else
                {
                    Console.WriteLine($"{library} is already installed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error installing {library}: {ex.Message}");
            }
        }
    }
}
