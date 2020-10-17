using AhpilySever;
using GameServer.Cache;
using GameServer.Cache.Fight;
using GameServer.Cache.Match;
using Protocol.Code;
using Protocol.Constant;
using Protocol.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GameServer.Logic
{
    public class ChatHandler: IHandler
    {

        private UserCache userCache = Caches.User;
        private MatchCache matchCache = Caches.Match;
        private FightCache fightCache = Caches.Fight;

        public void OnDisconnect(ClientPeer client)
        {
        
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch(subCode)
            {
                case ChatCode.CREQ:
                    chatRequest(client, (int)value);
                    break;
                default:
                    break;
            }
        }


        private void chatRequest(ClientPeer client,int chatType)
        {
            if (userCache.IsOnline(client) == false)
                return;
            int userId = userCache.GetId(client);
            //获取发送者的id userId
            //发送
            ChatDto dto = new ChatDto(userId, chatType);
            //广播给房间内的玩家
            if(matchCache.IsMatching(userId))
            {
                MatchRoom mRoom = matchCache.GetRoom(userId);
                mRoom.Brocast(OpCode.CHAT, ChatCode.SRES, dto);
            }
            if(fightCache.IsFighting(userId))
            {
                FightRoom fRoom = fightCache.GetRoomByUId(userId);
                fRoom.Brocast(OpCode.CHAT, ChatCode.SRES, dto,client);
            }
        }
    }
}


