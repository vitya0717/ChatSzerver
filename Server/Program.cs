using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace Server
{
    public class Program
    {

        static string ipv4 = "192.168.1.97";
        static int port = 50000;
        static IPAddress ip = null;
        static TcpListener listener = null;
        public static List<ClientCom> clients = new List<ClientCom>();
        public static List<KeyValuePair<string, string>> clientMessages = new List<KeyValuePair<string, string>>();
        public static List<Thread> clientThreads = new List<Thread>();
        public static Thread thread;
        public static int currentLine = 2;

        static void Main(string[] args)
        {
            ip = IPAddress.Parse(ipv4);
            listener = new TcpListener(ip, port);
            listener.Start();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"This server listening on the following address and port: ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{ip}:{port}");
            Thread commandExe = new Thread(commandExecutor);
            commandExe.Start();

            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientCom clientc = new ClientCom(client);
                    thread = new Thread(clientc.Start);
                    clients.Add(clientc);
                    clientThreads.Add(thread);
                    thread.Start();
                }
            }
        }

        private static void commandExecutor()
        {
            while (true)
            {
                switch (Console.ReadLine())
                {
                    case "commands":
                        Console.WriteLine("Parancsok listája: ");
                        Console.WriteLine("- clients | A klienseket fogja felsorolni");

                        break;
                    case "clients":
                        Console.WriteLine($"A jelenleg csatlakozott kliensek: (count: {clients.Count})");
                        foreach (var item in clients)
                        {
                            Console.WriteLine($" - Username: {item.Username}, IP: {item.clientIP}");
                        }
                        break;
                    default:
                        {
                            Console.WriteLine("Nincs ilyen parancs vagy elgépelted!\nA parancsokat megnézheted a 'commands' paranccsal!");
                            break;
                        }
                }
            }
        }
    }
}
