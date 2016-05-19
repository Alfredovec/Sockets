using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Sockets.Encryptor;

namespace Sockets.Client
{
    class Program
    {
        static string localhost = "127.0.0.1";
        private static string _password;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Type a port to connect: ");
                var port = Console.ReadLine();
                int intPort = int.Parse(port);

                var ipEndPoint = new IPEndPoint(IPAddress.Parse(localhost), intPort);
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipEndPoint);

                Console.Write("Type message:");
                var message = Console.ReadLine();
                Console.Write("Type server verification code:");
                _password = Console.ReadLine();

                message = _password.Encrypt() + ":" + message;
                var byteData = Encoding.Unicode.GetBytes(message);
                socket.Send(byteData);

                byteData = new byte[256];
                var stringBuilder = new StringBuilder();

                do
                {
                    var recievedBytes = socket.Receive(byteData, byteData.Length, 0);
                    stringBuilder.Append(Encoding.Unicode.GetString(byteData, 0, recievedBytes));
                } while (socket.Available > 0);

                Console.WriteLine("Server responded with a message: " + stringBuilder.ToString());

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
