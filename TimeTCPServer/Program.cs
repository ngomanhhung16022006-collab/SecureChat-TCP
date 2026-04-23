using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

class Program
{
    static List<TcpClient> clients = new List<TcpClient>();
    static Dictionary<TcpClient, string> clientNames = new Dictionary<TcpClient, string>();

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

        string name = "";

        try
        {
            // 👉 Nhận username
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes == 0) return;

            name = Encoding.UTF8.GetString(buffer, 0, bytes);

            lock (clientNames)
            {
                clientNames[client] = name;
            }

            Console.WriteLine(name + " da tham gia");

            // 👉 Thông báo join
            Broadcast(name + " da tham gia", client);

            // 👉 Gửi danh sách online
            SendUserList();

            while (true)
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) break;

                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                string fullMsg = name + ": " + msg;

                Console.WriteLine(fullMsg);

                Broadcast(fullMsg, client);
            }
        }
        catch { }

        Console.WriteLine(name + " da roi");

        // 👉 Xóa client
        lock (clients)
        {
            clients.Remove(client);
        }

        lock (clientNames)
        {
            if (clientNames.ContainsKey(client))
                clientNames.Remove(client);
        }

        // 👉 Gửi lại danh sách online
        SendUserList();

        // 👉 Thông báo leave
        Broadcast(name + " da roi", client);
    }

    static void Broadcast(string message, TcpClient sender)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);

        lock (clients)
        {
            foreach (var c in clients)
            {
                if (c != sender)
                {
                    try
                    {
                        NetworkStream s = c.GetStream();
                        s.Write(data, 0, data.Length);
                    }
                    catch { }
                }
            }
        }
    }

    static void SendUserList()
    {
        string list = "Online: ";

        lock (clientNames)
        {
            foreach (var name in clientNames.Values)
            {
                list += name + ", ";
            }
        }

        byte[] data = Encoding.UTF8.GetBytes(list);

        lock (clients)
        {
            foreach (var c in clients)
            {
                try
                {
                    NetworkStream s = c.GetStream();
                    s.Write(data, 0, data.Length);
                }
                catch { }
            }
        }
    }
}