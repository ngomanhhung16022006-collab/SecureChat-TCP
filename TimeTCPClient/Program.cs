using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
class Program
{
    static byte[] key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 byte
    static byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

    static string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor();
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    static string Decrypt(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
    static void Main()
    {
        Console.Write("Nhap IP server: ");
        string ip = Console.ReadLine();

        TcpClient client = new TcpClient(ip, 9999);
        NetworkStream stream = client.GetStream();

        Console.Write("Nhap ten: ");
        string name = Console.ReadLine();

        // gửi username (có xuống dòng)
        byte[] nameData = Encoding.UTF8.GetBytes(name + "\n");
        stream.Write(nameData, 0, nameData.Length);

        // ===== THREAD NHẬN =====
        Thread receiveThread = new Thread(() =>
        {
            byte[] buffer = new byte[1024];
            string dataBuffer = "";

            while (true)
            {
                try
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    if (bytes == 0) break;

                    // nối dữ liệu
                    dataBuffer += Encoding.UTF8.GetString(buffer, 0, bytes);

                    // tách theo dòng
                    string[] messages = dataBuffer.Split('\n');

                    // xử lý từng dòng hoàn chỉnh
                    for (int i = 0; i < messages.Length - 1; i++)
                    {
                        string msg = messages[i].Trim();
                        if (string.IsNullOrWhiteSpace(msg)) continue;

                        string[] parts = msg.Split('|');

                        if (parts[0] == "MSG" && parts.Length >= 3)
                        {
                            string encrypted = parts[2];
                            string decrypted = Decrypt(encrypted);

                            Console.WriteLine("[" + parts[1] + "]");
                            Console.WriteLine("Encrypted: " + encrypted);
                            Console.WriteLine("Decrypted: " + decrypted);
                            Console.WriteLine("---------------------");
                        }
                        else if (parts[0] == "JOIN" && parts.Length >= 2)
                        {
                            Console.WriteLine(parts[1] + " da tham gia");
                        }
                        else if (parts[0] == "LEAVE" && parts.Length >= 2)
                        {
                            Console.WriteLine(parts[1] + " da roi");
                        }
                        else if (parts[0] == "ONLINE" && parts.Length >= 2)
                        {
                            Console.WriteLine("Online: " + parts[1].Replace(",", ", "));
                        }
                        else
                        {
                            
                            
                        }
                    }

                    // giữ lại phần chưa hoàn chỉnh
                    dataBuffer = messages[messages.Length - 1];
                }
                catch
                {
                    Console.WriteLine("Mat ket noi server!");
                    break;
                }
            }
        });

        receiveThread.IsBackground = true;
        receiveThread.Start();

        // ===== GỬI TIN =====
        while (true)
        {
            string message = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(message)) continue;

            string encrypted = Encrypt(message);
            byte[] data = Encoding.UTF8.GetBytes(encrypted + "\n");
            stream.Write(data, 0, data.Length);
        }
    }
}