// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BingChat
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private static MainWindow m_window;
        private static Mutex mutex = new Mutex(true, "635706d3-4f55-4fa4-8a24-87898d9ff800");

        public static MainWindow MainWindow { get { return m_window; } }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            if (IsAnotherInstanceRunning())
            {
                DuplicatedInstanceHandler.SendDIConnectionRequest();
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                mutex.ReleaseMutex();
                InitializeComponent();
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                m_window = new MainWindow();

                // Hide m_window from taskbar and Alt+Tab
                SetWindowLongPtr(
                    WinRT.Interop.WindowNative.GetWindowHandle(m_window),
                    -20,
                    new IntPtr(0x00000080L));   // WS_EX_TOOLWINDOW

                m_window.Activate();
                Runner.Init();
                DuplicatedInstanceListener.StartListening();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.Print(ex.ToString());
#endif
                System.Windows.Forms.MessageBox.Show(
                    $"An error has occurred while launching the application.\n\n{ex.Message}",
                    System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);

                Application.Current.Exit();
            }
        }

        private bool IsAnotherInstanceRunning()
        {
            return !mutex.WaitOne(0, true);
        }
    }
}
