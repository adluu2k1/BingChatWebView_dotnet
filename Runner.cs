using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingChat
{
    public class Runner : ApplicationContext
    {
        NotifyIcon trayIcon;

        public Runner()
        {
            var exitMenu = new ToolStripMenuItem("Exit", null, new EventHandler(OnExit));

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(exitMenu);

            string icon_path = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledPath, "Assets/trayicon.ico");

            trayIcon = new NotifyIcon()
            {
                Icon = new System.Drawing.Icon(icon_path),
                ContextMenuStrip = contextMenu,
                Text = "Bing Chat",
                Visible = true
            };
            trayIcon.Click += OnClick;
        }

        public static void Init()
        {
            Task runner = new Task(() =>
            {
                System.Windows.Forms.Application.Run(new Runner());

                // When the Runner exited, shut down the app
                App.MainWindow.DispatcherQueue.TryEnqueue(() => App.Current.Exit());
            });
            runner.Start();
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnClick(object sender, EventArgs e)
        {
            try
            {
                var mouseEventArgs = e as MouseEventArgs;
                if (mouseEventArgs.Button == MouseButtons.Left)
                {
                    App.MainWindow.DispatcherQueue.TryEnqueue(() => App.MainWindow.Show());
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
