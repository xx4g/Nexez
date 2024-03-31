using System;

namespace DarkMode
{
    /// <summary>
    /// Provides functionality for enabling or disabling immersive dark mode in a Windows application.
    /// </summary>
    internal class DarkModeInterop
    {
        /// <summary>
        /// Sets the window attribute to enable or disable immersive dark mode.
        /// </summary>
        /// <param name="handle">The handle to the window.</param>
        /// <param name="enabled">True to enable immersive dark mode, false to disable it.</param>
        /// <returns>True if the operation is successful, otherwise false.</returns>
        internal static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            try
            {
                // Define colors for dark mode
                int[] darkGrey = new int[] { 0x00333333 }; // Dark grey color (#222)
                int[] white = new int[] { 0x00FFFFFF }; // White color

                // Set window attributes for dark mode
                DwmSetWindowAttribute(handle, DWWMA_CAPTION_COLOR, darkGrey, 4); // Caption color
                DwmSetWindowAttribute(handle, DWWMA_BORDER_COLOR, darkGrey, 4); // Border color (same as background)
                DwmSetWindowAttribute(handle, DWMWA_TEXT_COLOR, white, 4); // Text color (white)

                return true; // Operation successful
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // Handle any exceptions
                // Log or display error message
                return false; // Operation failed
            }
        }

        // Constants for setting window attributes
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        const int DWWMA_CAPTION_COLOR = 35;
        const int DWWMA_BORDER_COLOR = 34;
        const int DWMWA_TEXT_COLOR = 36;

        // External method declaration for setting window attributes
        [System.Runtime.InteropServices.DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
    }
}
