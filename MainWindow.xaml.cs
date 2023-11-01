// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BingChat
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        private IntPtr hWnd;

        public WebView2 webView = new();

        public MainWindow()
        {
            this.InitializeComponent();
            this.Activated += MainWindowActivated;

            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            Size desktopSize = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
            var rectWnd = new Windows.Graphics.RectInt32(
                (int)(desktopSize.Width * (2.0 / 3)) - 10,
                10,
                desktopSize.Width / 3,
                desktopSize.Height - 20);
            this.AppWindow.MoveAndResize(rectWnd);

            var presenter = this.AppWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.SetBorderAndTitleBar(false, false);

            WebView2 webView = InitWebView();
            this.Content = webView;
        }

        private WebView2 InitWebView()
        {
            webView.Source = new Uri("https://www.bing.com/chat?form=NTPCHB");
            webView.CoreWebView2Initialized += webView_CoreWebView2Initialized;

            return webView;
        }

        private void webView_CoreWebView2Initialized(object sender, CoreWebView2InitializedEventArgs e)
        {
            webView.CoreWebView2.NewWindowRequested += webView_CoreWebView2_NewWindowRequested;
        }

        private void webView_CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            try
            {
                e.Handled = true;
                Process.Start(new ProcessStartInfo() { FileName = e.Uri, UseShellExecute = true });
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.Print(ex.ToString());
#endif
                System.Windows.Forms.MessageBox.Show(
                    $"An error has occurred while processing the request.\n\n{ex.Message}",
                    System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void MainWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            try
            {
                if (e.WindowActivationState == WindowActivationState.Deactivated)
                {
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.Print(ex.ToString());
#endif
                System.Windows.Forms.MessageBox.Show(
                    $"An error has occurred while processing the request.\n\n{ex.Message}",
                    System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath),
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void Show()
        {
            this.AppWindow.Show(true);
            SetForegroundWindow(hWnd);
        }
        public void Hide()
        {
            this.AppWindow.Hide();
        }
    }
}
