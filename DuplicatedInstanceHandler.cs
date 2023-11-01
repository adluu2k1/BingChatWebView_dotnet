using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace BingChat
{
    public static class DuplicatedInstanceListener
    {
        public static NamedPipeServerStream pipeServer;

        private static void Init() => 
            pipeServer = new NamedPipeServerStream("635706d3-4f55-4fa4-8a24-87898d9ff800",
                                                    PipeDirection.In,
                                                    1,
                                                    PipeTransmissionMode.Byte,
                                                    PipeOptions.CurrentUserOnly);

        public static void StartListening()
        {
            if (pipeServer == null)
            {
                Init();
            }
            Task.Run(WaitForDIConnection);
        }

        private static async Task WaitForDIConnection()
        {
            if (pipeServer == null)
            {
                Init();
            }

            while (true)
            {
                await pipeServer.WaitForConnectionAsync();
                App.MainWindow.DispatcherQueue.TryEnqueue(() => App.MainWindow.Show());
                pipeServer.Disconnect();
            }
        }
    }

    public static class DuplicatedInstanceHandler
    {
        public static void SendDIConnectionRequest()
        {
            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", "635706d3-4f55-4fa4-8a24-87898d9ff800",
                                                                  PipeDirection.Out,
                                                                  PipeOptions.CurrentUserOnly))
                {
                    pipeClient.Connect();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.Print(ex.ToString());
#endif
                if (ex.GetType() != typeof(InvalidOperationException))
                {
                    string process_name = System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath);
                    System.Windows.Forms.MessageBox.Show(
                        $"Another {process_name} process is already running!",
                        System.IO.Path.GetFileNameWithoutExtension(Environment.ProcessPath),
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
        }
    }
}
