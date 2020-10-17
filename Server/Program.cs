using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program
    {

        private static Socket serverSocket = null;


        static void Main(string[] args)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //进行绑定
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 9999);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(10);
            //开始监听
            Console.WriteLine("开始监听：");
            Thread clientThread = new Thread(ListenToClient);
            clientThread.Start();
            while(true){ }
        }
         
        private static void ListenToClient()  //监听
        {
            Socket clientSocket = serverSocket.Accept();  //客户端
            Console.WriteLine("客户端连接成功");
            clientSocket.Send(Encoding.Default.GetBytes("我是服务器，你连接我成功了"));
            Thread recThread = new Thread(ReceiveClient);
            recThread.Start(clientSocket);
        }

        private static void ReceiveClient(object clientSocket)  //接收
        {
            Socket cSocket = clientSocket as Socket;
            byte[]  buffer= new byte[1024];
            int length=cSocket.Receive(buffer);
            Console.WriteLine("接收到客户端发来的消息："+Encoding.Default.GetString(buffer,0,length));
        }
    }
}
