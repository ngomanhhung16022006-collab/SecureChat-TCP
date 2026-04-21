using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.Write("Nhap IP server: ");
        string ip = Console.ReadLine();

        TcpClient client = new TcpClient(ip, 9999);
        NetworkStream stream = client.GetStream();

        Console.WriteLine("Da ket noi server!");
        Console.WriteLine("Nhap ten:"); 
        String name = Console.ReadLine();
        byte[] nameData = Encoding.UTF8.GetBytes(name);
        stream.Write(nameData, 0, nameData.Length);



        // Thread nhận tin
        Thread receiveThread = new Thread(() =>
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                Console.WriteLine("\nTin nhan: " + msg);
            }
        });
        receiveThread.Start();

        // Gửi tin
        while (true)
        {
            string message = Console.ReadLine();
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}