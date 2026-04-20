using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static List<TcpClient> clients = new List<TcpClient>();


    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 9999);
        server.Start();

        Console.WriteLine("Server dang cho client...");
        // chap nhan ket noi tu client
        while (true)
        {

            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);

            Console.WriteLine("Co client moi ket noi!");

            Thread t = new Thread(() => HandleClient(client));
            t.Start();
        }


        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                Console.WriteLine(msg);

                // gửi cho tất cả client khác
                foreach (var c in clients)
                {
                    NetworkStream s = c.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    s.Write(data, 0, data.Length);
                }
            }


            clients.Remove(client);
        }
    }
}
        