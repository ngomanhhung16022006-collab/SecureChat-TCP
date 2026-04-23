using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Linq;

class MyChatClient
{
    TcpClient client;
    NetworkStream stream;

    public event Action<string> OnMessage;
    public event Action<string[]> OnUserList;

    // ===== AES =====
    byte[] key = Encoding.UTF8.GetBytes("1234567890123456");
    byte[] iv = Encoding.UTF8.GetBytes("1234567890123456");

    string Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

            byte[] data = Encoding.UTF8.GetBytes(plainText);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }
    }

    string Decrypt(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);

            return sr.ReadToEnd();
        }
    }

    // ===== CONNECT =====
    public void Connect(string ip, string username)
    {
        client = new TcpClient(ip, 9999);
        stream = client.GetStream();

        // gửi username
        byte[] nameData = Encoding.UTF8.GetBytes(username + "\n");
        stream.Write(nameData, 0, nameData.Length);

        Thread t = new Thread(Receive);
        t.IsBackground = true;
        t.Start();
    }

    // ===== RECEIVE =====
    void Receive()
    {
        byte[] buffer = new byte[1024];
        string dataBuffer = "";

        while (true)
        {
            try
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                if (bytes == 0) break;

                dataBuffer += Encoding.UTF8.GetString(buffer, 0, bytes);

                string[] messages = dataBuffer.Split('\n');

                for (int i = 0; i < messages.Length - 1; i++)
                {
                    string msg = messages[i].Trim();
                    if (msg == "") continue;

                    Console.WriteLine("RECV: " + msg);

                    string[] parts = msg.Split('|');

                    if (parts[0] == "MSG" && parts.Length >= 3)
                    {
                        string user = parts[1];
                        string encrypted = parts[2];
                        string decrypted = Decrypt(parts[2]);

                        OnMessage?.Invoke(user + ": " + decrypted);

                        OnMessage?.Invoke(
                            user + ":\n" +
                            "🔐 AES: " + encrypted + "\n" +
                            "💬 Plain: " + decrypted + "\n"
                        );
                    }
                    else if (parts[0] == "JOIN")
                    {
                        OnMessage?.Invoke(parts[1] + " da tham gia");
                    }
                    else if (parts[0] == "LEAVE")
                    {
                        OnMessage?.Invoke(parts[1] + " da roi");
                    }
                    else if (parts[0] == "ONLINE")
                    {
                        OnUserList?.Invoke(parts[1].Split(','));
                    }
                }

                dataBuffer = messages[messages.Length - 1];
            }
            catch
            {
                break;
            }
        }
    }
    // ===== SEND =====
    public void Send(string message)
    {
        string encrypted = Encrypt(message);
        byte[] data = Encoding.UTF8.GetBytes(encrypted + "\n");
        stream.Write(data, 0, data.Length);
    }
}