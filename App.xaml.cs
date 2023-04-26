// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Windowing;
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
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                CheckForExistingInstance();
                m_window = new MainWindow();

                // Hide m_window from taskbar and Alt+Tab
                SetWindowLongPtr(
                    WinRT.Interop.WindowNative.GetWindowHandle(m_window),
                    -20,
                    new IntPtr(0x00000080L));

                m_window.Activate();
                StartRunner();
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

                Process.GetCurrentProcess().Kill();
            }
        }

        private void StartRunner()
        {
            Thread thr_runner = new Thread(() =>
            {
                System.Windows.Forms.Application.Run(new Runner(m_window));

                // When the Runner exited, shut down the app
                m_window.DispatcherQueue.TryEnqueue(() => this.Exit());
            });

            thr_runner.Start();
        }

        private void CheckForExistingInstance()
        {
            string process_name = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath);

            if (Process.GetProcessesByName(process_name).Length > 1)
            {
                System.Windows.Forms.MessageBox.Show($"Another {process_name} process is already running!");
                Process.GetCurrentProcess().Kill();
            }
        }

        private MainWindow m_window;


        public static void ShowWindow(IntPtr hWnd)
        {
            WindowId WndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow Wnd = AppWindow.GetFromWindowId(WndId);

            Wnd.Show(true);
            SetForegroundWindow(hWnd);
        }

        public static void HideWindow(IntPtr hWnd)
        {
            WindowId WndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow Wnd = AppWindow.GetFromWindowId(WndId);

            Wnd.Hide();
        }
    }
}
