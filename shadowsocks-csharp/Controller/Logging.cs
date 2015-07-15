using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Shadowsocks.Controller
{
    public class Logging
    {
        public static string LogFile;

        public static void LogTrace(string msg)
        {
            Console.WriteLine("Trace: " + msg);
        }
        public static void LogError(string msg)
        {
            Console.WriteLine("Trace: " + msg);
        }
        public static void LogInformation(string msg)
        {
            Console.WriteLine("Information: " + msg);
        }
        public static void LogNotice(string msg)
        {
            Console.WriteLine("Notice: " + msg);
        }

        public static void LogUsefulException(Exception e)
        {
            // just log useful exceptions, not all of them
            if (e is SocketException)
            {
                SocketException se = (SocketException)e;
                if (se.SocketErrorCode == SocketError.ConnectionAborted)
                {
                    // closed by browser when sending
                    // normally happens when download is canceled or a tab is closed before page is loaded
                }
                else if (se.SocketErrorCode == SocketError.ConnectionReset)
                {
                    // received rst
                }
                else if (se.SocketErrorCode == SocketError.NotConnected)
                {
                    // close when not connected
                }
                else
                {
                    LogError(e.ToString());
                }
            }
            else
            {
                LogTrace(e.ToString());
            }
        }

    }

    // Simply extended System.IO.StreamWriter for adding timestamp workaround
    public class StreamWriterWithTimestamp : StreamWriter
    {
        public StreamWriterWithTimestamp(Stream stream) : base(stream)
        {
        }

        private string GetTimestamp()
        {
            return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ";
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(GetTimestamp() + value);
        }

        public override void Write(string value)
        {
            base.Write(GetTimestamp() + value);
        }
    }

}
