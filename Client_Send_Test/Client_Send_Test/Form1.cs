using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Client_Send_Test
{
    public partial class Form1 : Form
    {
        private TcpClient m_client;
        Thread thr_received;
        private delegate void delUpdateUI(string sMessage);
        NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Create Tcp client.
                int nPort = 13000;
                m_client = new TcpClient("127.0.0.1", nPort);
                thr_received = new Thread(receiveData);
                thr_received.Start();

            }
            catch (ArgumentNullException a)
            {
                Console.WriteLine("ArgumentNullException: {0}", a);
            }
            catch(SocketException ex)
            {
                Console.WriteLine("SocketException:{0}", ex);
            }
        }
        private void receiveData()
        {
            byte[] btDatas = new byte[256]; // Receive data buffer
            string sData = null;
            try
            {
                stream = m_client.GetStream();
                int i;
                
                while ((i = stream.Read(btDatas, 0, btDatas.Length)) != 0)// 當有資料傳入時將資料顯示至介面上
                {
                    sData = System.Text.Encoding.UTF8.GetString(btDatas, 0, i);
                    UpdateMessage("Server: " + sData);
                    Thread.Sleep(5);
                    if (!m_client.Connected)
                    {
                        return;
                    }
                }

                //m_client.Close();
                Thread.Sleep(5);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //m_client.Close();
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
                richTextBox1.Text += sReceiveData + Environment.NewLine;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] btData = System.Text.Encoding.UTF8.GetBytes(txtData.Text); // Convert string to byte array.
            try
            {
                stream = m_client.GetStream();
                stream.Write(btData, 0, btData.Length); // Write data to server. 資料寫入Stream
                richTextBox1.Text += "Client: " + txtData.Text + Environment.NewLine;
                txtData.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Write Exception: {0}", ex);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            m_client.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
