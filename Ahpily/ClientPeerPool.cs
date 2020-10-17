using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilySever
{
    //客户端连接池
    //作用：重用客户端连接对象
   public class ClientPeerPool
    {
        private Queue<ClientPeer> clientPeerQueue;
        public ClientPeerPool(int capacity)
        {
            clientPeerQueue = new Queue<ClientPeer>(capacity);
        }

        //回收连接对象
        public void Enqueue(ClientPeer client)
        {
            clientPeerQueue.Enqueue(client);
        }


        //取出连接对象
        public ClientPeer Dequeue()
        {
            return clientPeerQueue.Dequeue();
        }
    }
}
