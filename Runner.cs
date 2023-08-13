using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace BingChat
{
    public class Runner : ApplicationContext
    {
        NotifyIcon trayIcon;
        MainWindow mainWindow;

        public Runner(MainWindow mainWindow)
        {
            var guestMenu = new ToolStripMenuItem("Guest Mode", null, new EventHandler(OnGuest))
            {
                Checked = false
            };
            var exitMenu = new ToolStripMenuItem("Exit", null, new EventHandler(OnExit));

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(exitMenu);
            contextMenu.Items.Add(guestMenu);

            string icon_path = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledPath, "Assets/trayicon.ico");

            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(icon_path),
                ContextMenuStrip = contextMenu,
                Text = "Bing Chat",
                Visible = true
            };
            trayIcon.Click += OnClick;

            this.mainWindow = mainWindow;
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnGuest(object sender, EventArgs e)
        {
            var guestMenu = sender as ToolStripMenuItem;
            guestMenu.Checked = !guestMenu.Checked;
        }

        private void OnClick(object sender, EventArgs e)
        {
            try
            {
                var mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs.Button == MouseButtons.Left)
                {
                    mainWindow.DispatcherQueue.TryEnqueue(() => App.ShowWindow(WinRT.Interop.WindowNative.GetWindowHandle(mainWindow)));
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
    }
}
