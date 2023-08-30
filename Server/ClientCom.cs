using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientCom
    {

        public StreamReader sr;
        public StreamWriter sw;
        private TcpClient tcpClient;
        private Socket socket;
        public IPAddress clientIP;
        private string username;
        private string consoleColor;
        

        public ClientCom(TcpClient tcpClient)
        {

            TcpClient = tcpClient;
            socket = TcpClient.Client;
            clientIP = IPAddress.Parse(((IPEndPoint)socket.RemoteEndPoint).Address.ToString());
            sr = new StreamReader(tcpClient.GetStream(), Encoding.UTF8);
            sw = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8);
        }

        public void Start()
        {

            bool end = false;
            while (!end)
            {
                try
                {
                    string command = sr.ReadLine();
                    if (command != null)
                    {
                        string[] parameters = command.Split('|');

                        switch (parameters[0])
                        {
                            case "setUsername":
                                Username = parameters[1];
                                ConsoleColor = parameters[2];
                                Console.ForegroundColor = System.ConsoleColor.DarkYellow;
                                Console.WriteLine($"{DateTime.Now}: {Username} joined the server");
                                Console.ForegroundColor = System.ConsoleColor.White;
                                break;
                            case "Message":
                                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), ConsoleColor, true);
                                Console.WriteLine($"{DateTime.Now}: ChatLog: {parameters[1]} texted: {parameters[2]}");
                                Program.clientMessages.Add(new KeyValuePair<string, string>(parameters[1], parameters[2]));
                                if (Program.clients.Count > 0)
                                {
                                    foreach (var client in Program.clients)
                                    {

                                        client.sw.WriteLine($"{parameters[1]}|{parameters[2]}|{ConsoleColor}|{Program.currentLine}");
                                        client.sw.Flush();
                                        Program.currentLine++;
                                    }
                                }
                                break;
                            case "LeaveFromServer":

                                break;
                            default:
                                {
                                    sw.WriteLine("Error. Hibás utasítás!");
                                    break;
                                }
                        }
                        sw.Flush();
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        sw.Close();
                        sr.Close();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    if (tcpClient.Connected) { tcpClient.Close(); }  
                    Program.clients.Remove(this);
                    if (Program.thread != null && Program.thread.IsAlive)
                    {
                        Program.thread.Abort();
                        Program.clientThreads.Remove(Program.thread);
                    }
                }
            }
        }
        public TcpClient TcpClient { get => tcpClient; private set => tcpClient = value; }
        public string Username { get => username; set => username = value; }
        public string ConsoleColor { get => consoleColor; set => consoleColor = value; }
    }
}
