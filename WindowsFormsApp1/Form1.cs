using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;         //匯入網路通訊協定相關函數
using System.Net.Sockets; //匯入網路插座功能函數
using System.Threading;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket T;    //通訊物件
        Thread TH;//監聽執行緒
        string User; //使用者
        //登入伺服器 
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            string IP = textBox1.Text;                                 //伺服器IP
            int Port = int.Parse(textBox2.Text);                       //伺服器Port
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port); //伺服器的連線端點資訊
            //建立可以雙向通訊的TCP連線
            T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            User = textBox3.Text;                                     //使用者名稱 
            try
            {
                T.Connect(EP);                        //連上伺服器的端點EP(類似撥號給電話總機)
                TH = new Thread(Listen);
                TH.IsBackground = true;
                TH.Start();
                textBox_main.Text = "已連線伺服器" + "\r\n";
                Send("0" + User);                     //連線後隨即傳送自己的名稱給伺服器
            }
            catch (Exception)
            {
                textBox_main.Text = "無法連線至伺服器" + "\r\n";
                //MessageBox.Show("無法連上伺服器！");  //連線失敗時顯示訊息
                return;
            }
            button1.Enabled = false;                  //讓連線按鍵失效，避免重複連線 
        }
        //傳送訊息給 Server (Send Message to the Server)
        private void Send(string Str)
        {
            byte[] B = Encoding.Default.GetBytes(Str);//翻譯字串Str為Byte陣列B
            T.Send(B, 0, B.Length, SocketFlags.None); //使用連線物件傳送資料
        }
        //關閉視窗代表離線登出 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (button1.Enabled == false)
            {
                Send("9" + User); //傳送自己的離線訊息給伺服器
                T.Close();        //關閉網路通訊器T
            }
        }

        private void button3_Click(object sender, EventArgs e)//點擊送出
        {
            if (textBox5.Text == "")
            {
                return;
            }
            if(listBox_onlinelist.SelectedIndex<0)
            {
                Send("1" + User + "公告" + textBox5);
            }
            else
            {
                Send("2" + "來自" + User + ":" + textBox5.Text + "1"+listBox_onlinelist.SelectedItem);
            }
        }
        private void Listen()
        {

        }
    }
}
