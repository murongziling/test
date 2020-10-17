using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace AhpilySever
{
   public  class ClientPeer
    {
        //设置连接对象
       public  Socket ClientSocket { get; set; }

        public ClientPeer()
        {
           this.ReceiveArgs = new SocketAsyncEventArgs();
            this.ReceiveArgs.UserToken = this;
            this.ReceiveArgs.SetBuffer(new byte[1024], 0, 1024);
            this.SendArgs = new SocketAsyncEventArgs();
            this.SendArgs.Completed += SendArgs_Completed;
        }


        #region 接收数据
        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg msg);
        //一个消息解析完成的回调
        public ReceiveCompleted receiveCompleted;
        //一旦接收到数据 就存到缓存区里面
        private List<byte> dataCache = new List<byte>();
        //粘包拆包问题解决决策：分成消息头和消息尾
        public SocketAsyncEventArgs ReceiveArgs { get; set; }
        //是否正在处理接收的数据
        private bool isReceiveProcess = false;
        //自身处理数据包
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if (!isReceiveProcess)
                processReceive();
        }

        //处理接收的数据
        private void processReceive()
        {
            isReceiveProcess = true;
            //解析数据包
            byte[] data = EncodeTool.DecodePacket(ref dataCache);
            if (data == null)
            {
                isReceiveProcess = false;
                return;
            }

            //
            SocketMsg msg = EncodeTool.DecodeMsg(data);
            //回调给上层
            if (receiveCompleted != null)
                receiveCompleted(this, msg);
            //尾递归
            processReceive();
        }
        #endregion


        #region 断开连接
        public void Disconnect()
        {

             //清空数据
                dataCache.Clear();
                isSendProcess = false;
                sendQueue.Clear();
                isSendProcess = false;
                //给发送数据那里预留的
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
                ClientSocket = null;

        }
        #endregion


        #region 发送数据
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private bool isSendProcess = false;
        private SocketAsyncEventArgs SendArgs;

        public delegate void SendDisconnect(ClientPeer client, string reason);

        public SendDisconnect sendDisconnect;

        public void Send(int opCode, int subCode, object value)
        {
            SocketMsg msg = new SocketMsg(opCode,subCode,value);
            msg.OpCode = opCode;
            msg.SubCode = subCode;
            msg.Value = value;
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);

            Send(packet);
        }

        public void Send(byte[] packet)
        {
            //存入消息队列里面
            sendQueue.Enqueue(packet);
            if (!isSendProcess)
                send();
        }


        /// <summary>
        /// 处理发送的消息
        /// </summary>
        private void send()
        {
            isSendProcess = true;
            //如果数据的发送条数等于0的话 停止发送
            if (sendQueue.Count == 0)
            {
                isSendProcess = false;
                return;
            }
            //取出一条数据
            byte[] packet = sendQueue.Dequeue();
            //设置消息 发送的异步套接字操作的发送数据缓冲区
            SendArgs.SetBuffer(packet, 0, packet.Length);
            bool result = ClientSocket.SendAsync(SendArgs);
            if (result == false)
            {
                processSend();
            }
        }

        private void SendArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            processSend();
        }


        //当异步发送请求完成的时候调用
        private void processSend()
        {
            if (SendArgs.SocketError != SocketError.Success)
            {
                //发送出错 客户端断开连接了
                sendDisconnect(this, SendArgs.SocketError.ToString());
            }
            else
            {
                send();
            }
        } 
        #endregion
    }
}
