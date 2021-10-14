using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace TCP_Client_Test
{
    public partial class Form1 : Form
    {
        private delegate void delUpdateUI(string sMessage);

        TcpListener m_server; // 監聽是否有Client連線
        Thread m_thrListening; // 持續監聽是否有Client連線及收值的執行緒
        TcpClient client;



        public Form1()
        {
            InitializeComponent();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                int nPort = Convert.ToInt32(txtPort.Text); // 設定 Port
                IPAddress localAddr = IPAddress.Parse(txtIP.Text); // 設定IP

                // Create TcpListener 並開始監聽
                m_server = new TcpListener(localAddr, nPort);
                m_server.Start();
                m_thrListening = new Thread(Listening);
                m_thrListening.Start();
            }
            catch(SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
        }

        private void Listening()
        {
            try
            {
                byte[] btDatas = new byte[256]; // Receive data buffer
                string sData = null;

                while (true)
                {
                    UpdateStatus("Waiting for connection...");

                    client = m_server.AcceptTcpClient(); // 用戶端連結，要等有Client建立連線後才會繼續往下執行
                    UpdateStatus("Connect to client!");

                    sData = null;
                    NetworkStream stream = client.GetStream();

                    int i;
                    while ((i = stream.Read(btDatas, 0, btDatas.Length)) != 0)// 當有資料傳入時將資料顯示至介面上
                    {
                        sData = System.Text.Encoding.UTF8.GetString(btDatas, 0, i);
                        UpdateMessage("Client: " + sData);
                        Thread.Sleep(5);
                        
                    }

                    client.Close();
                    Thread.Sleep(5);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException : {0}", ex);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateStatus(string sStatus)
        {
            if (this.InvokeRequired) //非同一個執行緒
            {
                delUpdateUI del = new delUpdateUI(UpdateStatus); // 因連線執行緒與UI執行緒不同，兩者參數存取必須藉由委派(delegate)來執行。
                this.Invoke(del, sStatus);
            }
            else // 同一個執行緒
            {
                labStatus.Text = sStatus;
            }
        }

        private void UpdateMessage(string sReceiveData)
        {
            if (this.InvokeRequired)
            {
                delUpdateUI del = new delUpdateUI(UpdateMessage);
                this.Invoke(del, sReceiveData);
            }
            else
            {
                txtMessage.Text += sReceiveData + Environment.NewLine;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] btdata = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
                NetworkStream stream = client.GetStream();
                stream.Write(btdata, 0, btdata.Length);
                txtMessage.Text += "Server: " + textBox1.Text + Environment.NewLine;
                textBox1.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Write Exception: {0}", ex);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
