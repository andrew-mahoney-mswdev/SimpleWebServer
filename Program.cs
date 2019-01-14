using System;
using System.Net;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.Principal;

namespace SimpleWebServer
{
    class Program
    {
        static void GetAdminPriviledges()
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                var startInfo = new ProcessStartInfo("SimpleWebServer.exe") { Verb = "runas" };
                Process.Start(startInfo); //Handle exception if priviledges declined.
                Environment.Exit(0);
            }
        }

        static void Main(string[] args)
        {
            GetAdminPriviledges();

            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://127.0.0.1/");
            server.Prefixes.Add("http://localhost/");
            server.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                HttpListenerContext context = server.GetContext();
                HttpListenerResponse response = context.Response;
                Console.WriteLine("Got request...");

                string page = Directory.GetCurrentDirectory() + context.Request.Url.LocalPath;

                if (context.Request.Url.LocalPath == "/")
                    page = "index.html";

                TextReader tr = new StreamReader(page);
                string msg = tr.ReadToEnd();

                byte[] buffer = Encoding.UTF8.GetBytes(msg);

                response.ContentLength64 = buffer.Length;
                Stream st = response.OutputStream;
                st.Write(buffer, 0, buffer.Length);
                Console.WriteLine("Sent file...");

                context.Response.Close();
            }
        }
    }
}
