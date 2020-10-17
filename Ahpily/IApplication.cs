using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilySever
{
public     interface IApplication
    {
        //断开连接
        void OnDisconnect(ClientPeer client);

        //接收数据
        void OnReceive(ClientPeer client, SocketMsg msg);
        


    }
}
