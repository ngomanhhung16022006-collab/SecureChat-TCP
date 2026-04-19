using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static TcpClient client1;
    static TcpClient client2;

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 9999);
        server.Start();

        Console.WriteLine("Server dang cho 2 client...");

        // Nhận client 1
        client1 = server.AcceptTcpClient();
        Console.WriteLine("Client 1 da ket noi");

        // Nhận client 2
        client2 = server.AcceptTcpClient();
        Console.WriteLine("Client 2 da ket noi");

        // Tạo 2 luồng chat
        Thread t1 = new Thread(HandleClient1);
        Thread t2 = new Thread(HandleClient2);

        t1.Start();
        t2.Start();
    }

    static void HandleClient1()
    {
        NetworkStream stream1 = client1.GetStream();
        NetworkStream stream2 = client2.GetStream();

        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytes = stream1.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

            Console.WriteLine("Client1: " + msg);

            // gửi sang client2
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream2.Write(data, 0, data.Length);
        }
    }

    static void HandleClient2()
    {
        NetworkStream stream1 = client1.GetStream();
        NetworkStream stream2 = client2.GetStream();

        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytes = stream2.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

            Console.WriteLine("Client2: " + msg);

            // gửi sang client1
            byte[] data = Encoding.UTF8.GetBytes(msg);
            stream1.Write(data, 0, data.Length);
        }
    }
}