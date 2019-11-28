using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        public Server()
        {
            InitializeComponent();
            KetNoi();
        }

        //Đóng kết nối
        private void Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }

        //Gửi tin cho tất cả client
        private void btnSend_Click(object sender, EventArgs e)
        {
            foreach(Socket item in clientList)
            {
                GuiTin(item);
            }
            tbxMessage.Clear();
            
        }


        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;

        //Kết nối với server
        void KetNoi()
        {
            clientList = new List<Socket>();
            //IP là địa chỉ của server
            IP = new IPEndPoint(IPAddress.Any, 9999);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            server.Bind(IP);

            Thread Listen = new Thread(() =>
            {
                try {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);

                        Thread receive = new Thread(NhanTin);
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 9999);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }

            });
            
            Listen.IsBackground = true;
            Listen.Start();
        }

        //Đóng kết nối hiện thời
        void DongKetNoi()
        {
            server.Close();
        }

        //Gửi tin
        void GuiTin(Socket client)
        {
            if (tbxMessage.Text != string.Empty)
                client.Send(PhanManh(tbxMessage.Text));
        }

        //Nhận tin
        void NhanTin(object obj)
        {
            Socket client = obj as Socket;
            //Cố gắng nhận tin, không nổi thì close
            try
            {
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
                clientList.Remove(client);
                client.Close();
            }


        }

        //Thêm tin nhắn vào khung chat
        void ThemTinNhan(string s)
        {
            lvMessage.Items.Add(new ListViewItem() { Text = s });
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
