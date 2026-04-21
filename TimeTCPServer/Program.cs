using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

class Program
{
    static List<TcpClient> clients = new List<TcpClient>();
    static Dictionary<TcpClient, string> clientNames = new Dictionary<TcpClient, string>(); // luu ten user 

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 9999);
        server.Start();

        Console.WriteLine("Server dang cho client...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();

            lock (clients)
            {
                clients.Add(client);
            }

            Console.WriteLine("Co client moi ket noi!");

            Thread t = new Thread(() => HandleClient(client));
            t.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            //  Nhận username đầu tiên
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes == 0) return;

            string name = Encoding.UTF8.GetString(buffer, 0, bytes);

            lock (clientNames)
            {
                clientNames[client] = name;
            }

            Console.WriteLine(name + " da tham gia");

            while (true)
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                string fullMsg = clientNames[client] + ": " + msg;

                Console.WriteLine(fullMsg);

                //  Broadcast
                lock (clients)
                {
                    foreach (var c in clients)
                    {
                        if (c != client)
                        {
                            NetworkStream s = c.GetStream();
                            byte[] data = Encoding.UTF8.GetBytes(fullMsg);
                            s.Write(data, 0, data.Length);
                        }
                    }
                }
            }
        }
        catch { }

        //  Remove client
        lock (clients)
        {
            clients.Remove(client);
        }

        lock (clientNames)
        {
            if (clientNames.ContainsKey(client))
                clientNames.Remove(client);
        }
    }
}