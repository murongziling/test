using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Program
    {
        private static Socket clientSocket = null;

        static void Main(string[] args)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            clientSocket.Connect(remoteEP);
            Console.WriteLine("连接到远程服务器");
       
            byte[] buffer = new byte[1024];
            //接受数据的长度
            int length = clientSocket.Receive(buffer);
            //显示长度
            Console.WriteLine("收到消息：" + Encoding.Default.GetString(buffer, 0, length));
            clientSocket.Send(Encoding.Default.GetBytes("服务器你好，我是客户端"));
            while(true)
            {

            }
        }
    }
}
