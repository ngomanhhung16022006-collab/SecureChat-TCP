using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

public partial class Form1 : Form
{
    TcpClient client;
    NetworkStream stream;

    public Form1()
    {
        InitializeComponent();
    }

    private void btnConnect_Click(object sender, EventArgs e)
    {
        client = new TcpClient("127.0.0.1", 8888);
        stream = client.GetStream();

        byte[] data = Encoding.UTF8.GetBytes(txtUsername.Text);
        stream.Write(data, 0, data.Length);

        Thread t = new Thread(Receive);
        t.Start();
    }

    void Receive()
    {
        byte[] buffer = new byte[2048];

        while (true)
        {
            int byteCount = stream.Read(buffer, 0, buffer.Length);
            string msg = Encoding.UTF8.GetString(buffer, 0, byteCount);

            if (msg.StartsWith("ONLINE|"))
            {
                string[] users = msg.Replace("ONLINE|", "").Split(',');

                lstOnline.Invoke((MethodInvoker)delegate {
                    lstOnline.Items.Clear();
                    lstOnline.Items.AddRange(users);
                });
            }
            else
            {
                string[] parts = msg.Split('|');
                string user = parts[0];
                string encrypted = parts[1];

                string decrypted = AES.Decrypt(encrypted);

                txtChat.Invoke((MethodInvoker)delegate {
                    txtChat.AppendText(user + ": " + decrypted + Environment.NewLine);
                });
            }
        }
    }

    private void btnSend_Click(object sender, EventArgs e)
    {
        string encrypted = AES.Encrypt(txtMessage.Text);

        byte[] data = Encoding.UTF8.GetBytes(encrypted);
        stream.Write(data, 0, data.Length);

        txtChat.AppendText("Me: " + txtMessage.Text + Environment.NewLine);
        txtMessage.Clear();
    }
}
