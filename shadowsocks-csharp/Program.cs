using Shadowsocks.Controller;
using Shadowsocks.Properties;
using Shadowsocks.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

namespace Shadowsocks
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            Util.Utils.ReleaseMemory();
            if (args.GetLength(0) < 1)
            {
                return 1;
            }

            string quitEventName = "";
            int processID = 0;
            foreach (string item in args)
            {
                if (item.StartsWith("/quiteventname="))
                {
                    quitEventName = item.Substring(15);
                }
                else if (item.StartsWith("/processID="))
                {
                    processID = Convert.ToInt32(item.Substring(11));
                }
            }

            if (processID == 0 || string.IsNullOrEmpty(quitEventName))
            {
                return 1;
            }
            EventWaitHandle ProgramQuit = new EventWaitHandle(false, EventResetMode.AutoReset, quitEventName);
            using (Mutex mutex = new Mutex(false, "Global\\" + "71981632-A427-497F-AB91-241CD227EC1F"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if (!mutex.WaitOne(0, false))
                {
                    Process[] oldProcesses = Process.GetProcessesByName("Shadowsocks");
                    if (oldProcesses.Length > 0)
                    {
                        Process oldProcess = oldProcesses[0];
                    }
                    return 2;
                }
                Directory.SetCurrentDirectory(Application.StartupPath);
#if !_DEBUG
                // Logging.OpenLogFile();
#endif
                ShadowsocksController controller = new ShadowsocksController();

                MenuViewController viewController = new MenuViewController(controller);

                controller.Start();  

                Process process = Process.GetProcessById(processID);
                WaitHandle[] waitHandles = new WaitHandle[2];
                waitHandles[0] = ProgramQuit;

                ManualResetEvent resetEvent = new ManualResetEvent(true);
                var waitHandle = new ManualResetEvent(false);
                process.Exited += (sender, e) => waitHandle.Set();
                waitHandles[1] = waitHandle;
                WaitHandle.WaitAny(waitHandles);
                Console.Out.WriteLine("QuitPipeThread");
                controller.Stop();
                return 0;
            }
        }
    }
}
