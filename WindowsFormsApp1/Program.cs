using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectionManagement;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO;


namespace WindowsFormsApp1
{
    static class Program
    {
        private static UdpServerWrapper _cm;
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            if (!EnsureSingleInstance())
            {
                Console.WriteLine("Program already running!");
                NamedPipeClientStream client = new NamedPipeClientStream("FD1AF2B4-575A-46E0-8DF5-8AB368CF6645");
                client.Connect(Timeout.Infinite);

                using (var writer = new BinaryWriter(client))
                {
                    writer.Write(args[0]);
                }
                return;
            }

            Thread readerThread = new Thread(new ThreadStart(ReaderThread));
            readerThread.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }

        static void ReaderThread()
        {    
            while(true)
            {
                NamedPipeServerStream server = new NamedPipeServerStream("FD1AF2B4-575A-46E0-8DF5-8AB368CF6645");
                server.WaitForConnection();
                using (var reader = new BinaryReader(server))
                {
                    string arguments = reader.ReadString();
                    Contacts form = (Contacts)Application.OpenForms["Contacts"];
                    if (form != null)
                    {
                        Console.WriteLine("Login already done!");
                        form.Invoke(form.updatePath, arguments);
                    }
                    Console.WriteLine("Received: {0}", arguments);
                }
            }
        }

        public static UdpServerWrapper CM
        {
            get { return _cm; }
            set { _cm = value; }
        }

        static bool EnsureSingleInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();

            var runningProcess = (from process in Process.GetProcesses()
                                  where
                                    process.Id != currentProcess.Id &&
                                    process.ProcessName.Equals(
                                      currentProcess.ProcessName,
                                      StringComparison.Ordinal)
                                  select process).FirstOrDefault();

            if (runningProcess != null)
            {
                ShowWindow(runningProcess.MainWindowHandle, SW_SHOWMAXIMIZED);
                SetForegroundWindow(runningProcess.MainWindowHandle);

                return false;
            }

            return true;
        }


        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        private const int SW_SHOWMAXIMIZED = 1;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }

}
