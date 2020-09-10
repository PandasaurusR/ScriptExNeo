using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ScriptExNeo.Interface {

    /// <summary>
    /// Provides functionality to change terminal appearance
    /// </summary>
    public static class Theme {
        public static void Apply() {
            SetConsoleTransparency.Apply();
            DisableConsoleQuickEdit.Apply();
            SetConsoleWindowSize.Apply();
        }
    }

    /// <summary>
    /// Set console window size to predetermined value
    /// </summary>
    static class SetConsoleWindowSize {
        private const int width = 120;
        private const int height = 50;

        public static void Apply() {
            try {
                Console.SetWindowSize(width, height);
            }
            catch (Exception) {
                Program.Log.Add("* FAILED TO SET CONSOLE WINDOW SIZE *");
            }
        }
    }


    /// <summary>
    /// Set transparency of window
    /// </summary>
    static class SetConsoleTransparency {

        /// <summary>
        /// Used to set opacity of terminal
        /// </summary>
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        /// <summary>
        /// For conversion from COLORREF to uint
        /// Code snippet from https://www.pinvoke.net/default.aspx/Structures.COLORREF
        /// </summary>
        private static uint MakeCOLORREF(byte r, byte g, byte b) {
            return (((uint)r) | (((uint)g) << 8) | (((uint)b) << 16));
        }

        public static void Apply() {
            IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;

            SetLayeredWindowAttributes(
                hWnd,
                MakeCOLORREF(0, 0, 0),
                210, // Opacity number (0-255)
                0x00000002
            );
        }
    }

    /// <summary>
    /// Disable terminal quick edit mode
    /// Code from: https://stackoverflow.com/questions/13656846/how-to-programmatic-disable-c-sharp-console-applications-quick-edit-mode
    /// </summary>
    static class DisableConsoleQuickEdit {

        const uint ENABLE_QUICK_EDIT = 0x0040;

        // STD_INPUT_HANDLE (DWORD): -10 is the standard input device.
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        internal static bool Apply() {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            if (!GetConsoleMode(consoleHandle, out uint consoleMode)) {
                // ERROR: Unable to get console mode.
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode)) {
                // ERROR: Unable to set console mode
                return false;
            }

            return true;
        }
    }
}
