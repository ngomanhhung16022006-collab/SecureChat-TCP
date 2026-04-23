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

        Console.WriteLine("Server dang chay...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();

            lock (clients)
            {
                clients.Add(client);
            }

            Thread t = new Thread(() => Handle(client));
            t.Start();
        }
    }

    static void Handle(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        string name = "";

        try
        {
            // nhận username
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes == 0) return;

            name = Encoding.UTF8.GetString(buffer, 0, bytes).Trim();

            lock (clientNames)
            {
                clientNames[client] = name;
            }

            Console.WriteLine(name + " joined");
           
            // JOIN
            Broadcast("JOIN|" + name + "\n", client);
            SendUserList();

            string dataBuffer = "";

            while (true)
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) break;

                dataBuffer += Encoding.UTF8.GetString(buffer, 0, bytes);

                string[] messages = dataBuffer.Split('\n');

                for (int i = 0; i < messages.Length - 1; i++)
                {
                    string msg = messages[i].Trim();
                    if (msg == "") continue;
                    Console.WriteLine("RAW (server khong hieu): " + msg);
                    Broadcast("MSG|" + name + "|" + msg + "\n", client);
                }

                // giữ lại phần chưa hoàn chỉnh
                dataBuffer = messages[messages.Length - 1];
            }
        catch { }

        Console.WriteLine(name + " left");

        lock (clients)
        {
            clients.Remove(client);
        }

        lock (clientNames)
        {
            if (clientNames.ContainsKey(client))
                clientNames.Remove(client);
        }
        SendUserList();
        Broadcast("LEAVE|" + name + "\n", client);
       
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
        string list = "ONLINE|";

        lock (clientNames)
        {
            list += string.Join(",", clientNames.Values);
        }

        list += "\n"; // kết thúc 1 message

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