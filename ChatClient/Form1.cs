using System;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
       

        MyChatClient chat = new MyChatClient();

        public Form1()
        {
            InitializeComponent();
            
        }

       

        private void btnConnect_Click(object sender, EventArgs e)
        {
            chat.Connect("127.0.0.1", txtUsername.Text);

            chat.OnMessage += (msg) =>
            {
                Invoke((MethodInvoker)(() =>
                {
                    txtChat.AppendText(msg + Environment.NewLine);
                    txtChat.SelectionStart = txtChat.Text.Length;
                    txtChat.ScrollToCaret();
                }));
            };

            chat.OnUserList += (users) =>
            {
                Invoke((MethodInvoker)(() =>
                {
                    lstOnline.Items.Clear();
                    lstOnline.Items.AddRange(users);
                }));
            };
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            chat.Send(txtMessage.Text);

            txtChat.AppendText("Me: " + txtMessage.Text + Environment.NewLine);
            txtMessage.Clear();
        }

        
    }
}