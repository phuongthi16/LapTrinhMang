using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            KetNoi();
        }

        //Gửi tin đi
        private void btnSend_Click(object sender, EventArgs e)
        {
            GuiTin();
            ThemTinNhan(tbxMessage.Text);
        }

        IPEndPoint IP;
        Socket client;

        //Kết nối với server
        void KetNoi()
        {
            //IP là địa chỉ của server
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream,ProtocolType.IP);

            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Không thể kết nối server!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        
            Thread listen = new Thread(NhanTin);
            listen.IsBackground = true;
            listen.Start();
        }

        //Đóng kết nối hiện thời
        void DongKetNoi()
        {
            client.Close();
        }

        //Gửi tin
        void GuiTin()
        {
            if (tbxMessage.Text != string.Empty)
                client.Send(PhanManh(tbxMessage.Text));
        }

        //Nhận tin
        void NhanTin()
        {
            //Cố gắng nhận tin, không nổi thì close
            try {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);

                    string message = (string)GomManh(data);
                    ThemTinNhan(message);
                }
            }
            catch
            {
                Close();
            }
            
            
        }

        //Thêm tin nhắn vào khung chat
        void ThemTinNhan(string s)
        {
            lvMessage.Items.Add(new ListViewItem() { Text = s });
            tbxMessage.Clear();
        }
        //Phân mảnh
        byte[] PhanManh(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        //Gom mảnh
        object GomManh(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }

        //Đóng kết nối khi đóng form
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
    }
}
