using System;
using System.Windows.Forms;

namespace StopwatchApp
{
    /// <summary>
    /// Entry point for the Stopwatch application.
    /// Provides the Main method to start the Windows Forms application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Boots the Windows Forms application and opens the main form.
        /// Enables visual styles and runs the main application loop.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // Enable modern visual styles for the application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the application with the main stopwatch form
            Application.Run(new MainForm());
        }
    }
}