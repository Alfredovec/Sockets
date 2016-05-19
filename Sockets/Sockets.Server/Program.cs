using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Sockets.Encryptor;

namespace Sockets.Server
{
    class Program
    {
        private static readonly int defaultPort = 7777;
        private static readonly string _localhost = "127.0.0.1";
        private static string _password;

        static void Main(string[] args)
        {
            Console.WriteLine("Type port in a range 7000-8000: ");
            var port = Console.ReadLine();
            int intPort;
            if (!int.TryParse(port, out intPort) || intPort > 8000 || intPort < 7000)
            {
                intPort = defaultPort;
            }

            Console.Write("Type server verification code:");
            _password = Console.ReadLine();

            var ipEndPoint = new IPEndPoint(IPAddress.Parse(_localhost), intPort);
            var listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listenSocket.Bind(ipEndPoint);
                listenSocket.Listen(10);

                Console.WriteLine("Server is up and running with a port " + intPort + ". Waiting for connections...");

                while (true)
                {
                    var handler = listenSocket.Accept();

                    var builder = new StringBuilder();
                    var dataBuffer = new byte[256];

                    do
                    {
                        var receivedBytes = handler.Receive(dataBuffer);
                        builder.Append(Encoding.Unicode.GetString(dataBuffer, 0, receivedBytes));
                    } while (handler.Available > 0);

                    var message = builder.ToString();
                    string response;
                    var splittedMessage = message.Split(':');
                    if (splittedMessage[0].Decrypt() == _password)
                    {
                        message = string.Join("", splittedMessage.Skip(1));
                        var allWords = message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                        var voWords = allWords.Where(w => w.EndsWith("во"));
                        var descVords = allWords.OrderByDescending(w => w);

                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + string.Join(" ", descVords));

                        response = "OK";
                    }
                    else
                    {
                        response = "ERROR";

                    }

                    dataBuffer = Encoding.Unicode.GetBytes(response);
                    handler.Send(dataBuffer);

                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
