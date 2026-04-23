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

        // nhap ten
        Console.Write("Nhap ten: ");
        string name = Console.ReadLine();

        byte[] nameData = Encoding.UTF8.GetBytes(name);
        stream.Write(nameData, 0, nameData.Length);

        // thread nhan tin
        Thread t = new Thread(() =>
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);

                    if (bytes == 0)
                    {
                        Console.WriteLine("Mat ket noi!");
                        break;
                    }

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytes);
                    Console.WriteLine(msg);
                }
                catch
                {
                    Console.WriteLine("Loi ket noi!");
                    break;
                }
            }
        });

        t.Start();

        // gui tin
        while (true)
        {
            string message = Console.ReadLine();

            if (message == "") continue;

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}