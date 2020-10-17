using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AhpilySever
{
    
 public   class SeverPeer
    {
        //服务器端的Socket对象
        private Socket severSocket;
        //限制客户端连接数量的信号量
        private Semaphore acceptSemaphore;
        //客户端对象连接池
        private ClientPeerPool clientPeerPool;
        //应用层
        private IApplication application;
        //设置应用层


        public void SetApplication(IApplication app)
        {
            this.application = app;
        }


        public void Start(int port,int maxNum)
        {
            try
            {
                severSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                acceptSemaphore = new Semaphore(maxNum,maxNum);

                clientPeerPool = new ClientPeerPool(maxNum);
                ClientPeer tmpClientPeer = null;
                for(int i=0;i<maxNum;i++)
                {
                    tmpClientPeer = new ClientPeer();
                    tmpClientPeer.ReceiveArgs.Completed += receive_Completed;
                    tmpClientPeer.receiveCompleted = receiveCompleted;
                    tmpClientPeer.sendDisconnect = Disconnect;
                    clientPeerPool.Enqueue(tmpClientPeer);
                }

                severSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                severSocket.Listen(10);

                Console.WriteLine("服务器启动中......");

                startAccept(null);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        //开始等待客户端的连接
        private void startAccept(SocketAsyncEventArgs e)
        {
            if(e==null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += accept_Completed;
            }
           
           bool result= severSocket.AcceptAsync(e);
            //返回true代表正在执行
            //返回false代表执行完毕
            if(result==false)
            {
                processAccept(e);
            }
        }

        //接受连接请求异步事件完成时候触发
        private void accept_Completed(object sender,SocketAsyncEventArgs e)
        {
            processAccept(e);
        }

        //处理连接请求
        private void processAccept(SocketAsyncEventArgs e)
        {
            //限制线程的访问
            acceptSemaphore.WaitOne();

            //得到客户端对象
            //Socket clientSocket = e.AcceptSocket;
            ClientPeer client = clientPeerPool.Dequeue();
            client.ClientSocket = e.AcceptSocket;

            Console.WriteLine("客户端连接成功：" + client.ClientSocket.RemoteEndPoint.ToString());

            //开始接收数据
            startReceive(client);
            e.AcceptSocket = null;
            startAccept(e);
        }

        #region 接收数据
        private void startReceive(ClientPeer client)
        {
            try
            {
                bool result = client.ClientSocket.ReceiveAsync(client.ReceiveArgs);
                if (result == false)
                {
                    processReceive(client.ReceiveArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        //处理接受的请求
        private void processReceive(SocketAsyncEventArgs e)
        {
            ClientPeer client = e.UserToken as ClientPeer;
            //判断网络消息是否接受成功
            if (client.ReceiveArgs.SocketError == SocketError.Success && client.ReceiveArgs.BytesTransferred > 0)
            {
                //拷贝数据到数据包
                byte[] packet = new byte[client.ReceiveArgs.BytesTransferred];
                Buffer.BlockCopy(client.ReceiveArgs.Buffer, 0, packet, 0, client.ReceiveArgs.BytesTransferred);
                //让客户端自身处理这个数据包 自身解析
                client.StartReceive(packet);
                //尾递归 让其他数据继续
                startReceive(client);
            }
            //断开连接了 如果没有传输的字节数 就代表断开连接了
            else if (client.ReceiveArgs.BytesTransferred == 0)
            {
                if (client.ReceiveArgs.SocketError == SocketError.Success)
                {
                    //客户端主动断开连接
                    Disconnect(client, "客户端主动断开连接");
                }
                else
                {
                    //由于网络异常导致被动断开连接
                    Disconnect(client, client.ReceiveArgs.SocketError.ToString());
                }
            }
        }

        //当接收完成时 触发的事件
        private void receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            processReceive(e);
        }

        //一条数据解析完成的处理
        private void receiveCompleted(ClientPeer client, SocketMsg msg)
        {
            //给应用层让其处理
            application.OnReceive(client, msg);
        }
        #endregion


        #region 断开连接
        public void Disconnect(ClientPeer client, string reason)
        {
            try
            {
                //清空数据
                if (client == null)
                    throw new Exception("当前指定的客户端连接对象为空，无法断开连接");
                Console.WriteLine(client.ClientSocket.RemoteEndPoint.ToString()+"客户端断开连接 原因："+reason);
                //通知应用层  这个客户断开连接了
                application.OnDisconnect(client);
                client.Disconnect();
                //回收对象方便下次使用
                clientPeerPool.Enqueue(client);
                acceptSemaphore.Release();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        } 
        #endregion
    }
}
