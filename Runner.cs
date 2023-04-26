using System;
using System.Windows.Forms;

namespace BingChat
{
    public class Runner : ApplicationContext
    {
        NotifyIcon trayIcon = new();
        MainWindow mainWindow;

        public Runner(MainWindow mainWindow)
        {
            var exitMenu = new ToolStripMenuItem("Exit", null, new EventHandler(OnExit));

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(exitMenu);

            trayIcon.Icon = new System.Drawing.Icon("exe_icon.ico");
            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.Text = "Bing Chat";
            trayIcon.Visible = true;
            trayIcon.Click += OnClick;

            this.mainWindow = mainWindow;
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnClick(object sender, EventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs.Button == MouseButtons.Left)
            {
                mainWindow.DispatcherQueue.TryEnqueue(() => App.ShowWindow(WinRT.Interop.WindowNative.GetWindowHandle(mainWindow)));
            }
        }
    }
}
