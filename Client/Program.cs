using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        private static TcpClient client = new TcpClient("192.168.1.97", 50000);
        private static StreamReader sr = new StreamReader(client.GetStream(), Encoding.UTF8);
        private static StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.UTF8);
        private static Socket socket = client.Client;
        public static IPAddress clientIP = IPAddress.Parse(((IPEndPoint)socket.RemoteEndPoint).Address.ToString());
        public static string username = "username";
        public static string selectedColor = "Green";
        private static List<KeyValuePair<string, string>> tempClientMessages = new List<KeyValuePair<string, string>>();


        static void Main(string[] args)
        {
            Thread messageObserver = new Thread(observer);
            messageObserver.Start();

            Console.Write("Add meg a felhasználóneved: ");
            string usernameField = Console.ReadLine();

            Console.Write("Add meg milyen színű legyen a chated: ");
            string selectedColorField = Console.ReadLine();

            //color and username set
            selectedColor = selectedColorField.ToUpper();
            username = usernameField;

            sw.WriteLine($"setUsername|{usernameField}|{selectedColorField}");
            sw.Flush();

            Thread messageSender = new Thread(sendMessage);

            messageSender.Start();
        }

        private static void sendMessage()
        {
            bool end = false;
            while (!end)
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), selectedColor, true);
                Console.Write($"{username} > ");
                string text = Console.ReadLine();
                string message = $"Message|{username}|{text}";
                sw.WriteLine(message);
                sw.Flush();
            }
        }

        private static void observer()
        {
            while (true)
            {
                try
                {
                    string receiveMessage = sr.ReadLine();
                    if (receiveMessage != null)
                    {
                        string[] dataSplit = receiveMessage.Split('|');
                        Console.Clear();
                        tempClientMessages.Add(new KeyValuePair<string, string>(dataSplit[0], dataSplit[1]));
                        foreach (var item in tempClientMessages)
                        {
                            Console.ForegroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), selectedColor, true);
                            if (item.Key != username)
                            {
                                Console.WriteLine($"{item.Key} > {item.Value}");
                            } else
                            {
                                Console.WriteLine($"{item.Key} > {item.Value}");
                            } 
                        }
                        Console.Write($"{username} > ");
                    }
                }

                catch (Exception ex)
                {

                    Console.WriteLine($"{ex.Message}\n{ex.InnerException}\n{ex.Source}");
                }
            }
        }
    }
}
