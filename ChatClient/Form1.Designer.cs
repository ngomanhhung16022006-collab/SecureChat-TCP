namespace ChatClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtUsername = new TextBox();
            btnConnect = new Button();
            txtChat = new TextBox();
            lstOnline = new ListBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            SuspendLayout();
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(10, 10);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(200, 31);
            txtUsername.TabIndex = 0;
            // 
            // btnConnect
            // 
            btnConnect.Location = new Point(220, 10);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(112, 34);
            btnConnect.TabIndex = 1;
            btnConnect.Text = "Connect";
            btnConnect.UseVisualStyleBackColor = true;
            btnConnect.Click += btnConnect_Click;
            // 
            // txtChat
            // 
            txtChat.Location = new Point(10, 50);
            txtChat.Multiline = true;
            txtChat.Name = "txtChat";
            txtChat.ReadOnly = true;
            txtChat.ScrollBars = ScrollBars.Vertical;
            txtChat.Size = new Size(500, 300);
            txtChat.TabIndex = 2;
            // 
            // lstOnline
            // 
            lstOnline.FormattingEnabled = true;
            lstOnline.Location = new Point(520, 50);
            lstOnline.Name = "lstOnline";
            lstOnline.Size = new Size(150, 279);
            lstOnline.TabIndex = 3;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(10, 370);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(500, 31);
            txtMessage.TabIndex = 4;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(520, 370);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(112, 34);
            btnSend.TabIndex = 5;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(809, 450);
            Controls.Add(btnSend);
            Controls.Add(txtMessage);
            Controls.Add(lstOnline);
            Controls.Add(txtChat);
            Controls.Add(btnConnect);
            Controls.Add(txtUsername);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        
        private TextBox txtUsername;
        private Button btnConnect;
        private TextBox txtChat;
        private ListBox lstOnline;
        private TextBox txtMessage;
        private Button btnSend;
    }
}
