// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Drawing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BingChat
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public WebView2 webView = new();

        public MainWindow()
        {
            this.InitializeComponent();
            this.Activated += MainWindowActivated;

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId WndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWnd = AppWindow.GetFromWindowId(WndId);

            Size desktopSize = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
            var rectWnd = new Windows.Graphics.RectInt32(
                (int)(desktopSize.Width * (2.0 / 3)) - 10,
                10,
                desktopSize.Width / 3,
                desktopSize.Height - 20);
            //appWnd.MoveAndResize(new Windows.Graphics.RectInt32(1920 - 600 - 10, 10, 600, 1000));
            appWnd.MoveAndResize(rectWnd);

            var presenter = appWnd.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
            presenter.SetBorderAndTitleBar(false, false);

            WebView2 webView = InitWebView();
            this.Content = webView;
        }

        private WebView2 InitWebView()
        {
            webView.Source = new Uri("https://www.bing.com/search?q=Bing+AI&showconv=1&FORM=hpcodx");
            webView.CoreWebView2Initialized += webView_CoreWebView2Initialized;

            return webView;
        }

        private void webView_CoreWebView2Initialized(object sender, CoreWebView2InitializedEventArgs e)
        {
            webView.CoreWebView2.NewWindowRequested += webView_CoreWebView2_NewWindowRequested;
        }

        private void webView_CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            Process.Start(new ProcessStartInfo() { FileName = e.Uri, UseShellExecute = true });
        }

        private void MainWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == WindowActivationState.Deactivated)
            {
                App.HideWindow(WinRT.Interop.WindowNative.GetWindowHandle(this));
            }
        }
    }
}
