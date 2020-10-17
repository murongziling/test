using AhpilySever;
using Protocol.Code;
using GameServer.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    public class NetMsgCenter : IApplication
    {
        IHandler account = new AccountHandler();
        IHandler user = new UserHandler();
       MatchHandler match = new MatchHandler();
        IHandler chat = new ChatHandler();
        FightHandler fight = new FightHandler();

        public NetMsgCenter()
        {
            match.startFight += fight.startFight;
        }

        public void OnDisconnect(ClientPeer client)
        {
            fight.OnDisconnect(client);
            chat.OnDisconnect(client);
            match.OnDisconnect(client);
            user.OnDisconnect(client);
            account.OnDisconnect(client);
        }


        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
                switch (msg.OpCode)
                {
                    case OpCode.ACCOUNT:
                        account.OnReceive(client, msg.SubCode, msg.Value);
                        break;
                  case OpCode.USER:
                    user.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.MATCH:
                    match.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                case OpCode.CHAT:
                    chat.OnReceive(client, msg.SubCode, msg.Value);
                        break;
                case OpCode.FIGHT:
                    fight.OnReceive(client, msg.SubCode, msg.Value);
                    break;
                    default:
                        break;
                }
        }
    }
}
