using AhpilySever;
using AhpilySever.Concurrent;
using AhpilySever.Timer;
using GameServer.Cache.Match;
using Protocol.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameServer.Cache.Match
{
    
    /// <summary>
    /// 匹配的缓存层
    /// </summary>
  public  class MatchCache
    {

        TimerManager timer = new TimerManager();

     /// <summary>
    /// 代表正在等待的用户id和房间id的映射
    /// </summary>
        private Dictionary<int, int> uidRoomIdDict = new Dictionary<int, int>();

        /// <summary>
        /// 代表正在等待的房间id和房间的数据模型的映射
        /// </summary>
        private Dictionary<int, MatchRoom> idModelDict = new Dictionary<int, MatchRoom>();

        /// <summary>
        /// 代表重用的房间队列
        /// </summary>
     private   Queue<MatchRoom> roomQueue = new Queue<MatchRoom>();
        /// <summary>
        /// 代表房间id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 进入匹配队列 进入匹配房间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom Enter(int userId,ClientPeer client)
        {
            int roomId ; 
            //遍历一下等待的房间 看一下有没有正在等待的 如果有 我们就把这玩家加进去
            foreach(MatchRoom mr in idModelDict.Values)
            {
                //房间满了 继续
                if (mr.IsFull())
                    continue;
                //没满的话
                mr.Enter(userId,client);
                uidRoomIdDict.Add(userId, mr.Id);
                return mr;
            }
            //如果调用到这里 代表没进去 ，因为没有等待的房间了
            //自己开个房
            MatchRoom room = null;
            //判断一下是否有重用的房间
            if (roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
                room.Enter(userId, client);
                idModelDict.Add(room.Id, room);
                uidRoomIdDict.Add(userId, room.Id);
                //1.给房间添加计时    2.计时到时前10秒的警告
                timer.AddTimeEvent(500000000,
               delegate
               {
                   room.Brocast(OpCode.MATCH, MatchCode.MESSAGE_BRO, null);
               });
                timer.AddTimeEvent(600000000,
                    delegate
                    {

                        if (room == null)
                        {
                            return;
                        }
                        room.Brocast(OpCode.MATCH, MatchCode.BACK_BRO, null);
                        Destroy(room);
                    }
                    );
            }
            else
            {

                room = new MatchRoom(id.Add_Get());
                roomId = room.Id;
                room.Enter(userId, client);
                idModelDict.Add(room.Id, room);
                uidRoomIdDict.Add(userId, room.Id);
                //1.给房间添加计时    2.计时到时前10秒的警告
                timer.AddTimeEvent(500000000,
                delegate
                {
                    room.Brocast(OpCode.MATCH, MatchCode.MESSAGE_BRO, null);
                });
                timer.AddTimeEvent(600000000,
                   delegate
                   {
                        if (room == null)
                       {
                           return;
                       }
                       room.Brocast(OpCode.MATCH, MatchCode.BACK_BRO, null);
                       Destroy(room);
                   }
                   );
            }
           
            return room;
              
        }

       



        /// <summary>
        /// 离开匹配房间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom Leave(int userId)
        {
            int roomId = uidRoomIdDict[userId];
            MatchRoom room = idModelDict[roomId];
            room.Leave(userId);
            //还需要进一步的处理
            uidRoomIdDict.Remove(userId);
            if(room.IsEmpty())
            {
                //如果房间空了 那就放到重用队列里面
                idModelDict.Remove(roomId);
                roomQueue.Enqueue(room);
            }
            return room;
        }

        /// <summary>
        /// 判断用户是否在匹配房间内
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMatching(int userId)
        {
            return uidRoomIdDict.ContainsKey(userId);
        }
        
        /// <summary>
        /// 获取玩家所在的等待房间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom GetRoom(int userId)
        {
            int roomId = uidRoomIdDict[userId];
            MatchRoom room = idModelDict[roomId];
            return room;
        }

        /// <summary>
        /// 摧毁房间
        /// </summary>
        public void Destroy(MatchRoom room)
        {
            idModelDict.Remove(room.Id);
            foreach(var userId in room.UIdClientDict.Keys)
            {
                uidRoomIdDict.Remove(userId);
            }
            //清空数据
            room.UIdClientDict.Clear();
            room.ReadyUIdList.Clear();
            roomQueue.Enqueue(room);

        }
    }
}
