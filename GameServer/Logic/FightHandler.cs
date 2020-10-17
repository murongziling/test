using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AhpilySever;
using GameServer.Cache;
using GameServer.Cache.Fight;
using Protocol.Dto.fight;
using Protocol.Code;
using GameServer.Model;
using GameServer.Cache.Match;
using Protocol.Constant;

namespace GameServer.Logic
{
    public class FightHandler : IHandler
    {
        public FightCache fightCache = Caches.Fight;
        public UserCache userCache = Caches.User;
        public MatchCache matchCache = Caches.Match;

        public void OnDisconnect(ClientPeer client)
        {
            leave(client);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case FightCode.GRAB_LANDLORD_CREQ:
                    //如果是true 就是抢地主 如果是false 就是不抢地主
                    bool result = (bool)value;
                    grabLandlord(client, result);
                    break;
                case FightCode.DEAL_CREQ:
                    deal(client, value as DealDto);
                    break;
                case FightCode.PASS_CREQ:
                    pass(client,(bool)value);
                    break;
                case FightCode.LEAVE_CREQ:
                    leave(client);
                    break;
                case FightCode.MULTIPLE_CREQ:
                    multi(client, (int)value);
                    break;
                default:
                    break;

            }
        }

        /// <summary>
        /// 翻倍处理
        /// </summary>
        /// <param name="client"></param>
        /// <param name="multi"></param>
        private void multi(ClientPeer client, int multi)
        {
            int userId = userCache.GetId(client);
            FightRoom room = fightCache.GetRoomByUId(userId);
            room.AddInMulti(userId, multi);
            if(room.multiDict.Count==2)
            {
                room.isMulti = false;
            }
            if(room.isMulti==false)
            {
                foreach (var player in room.PlayerList)
                {
                    if (player.Identity == Identity.LANDLORD)
                    {
                        brocast(room, OpCode.FIGHT, FightCode.TURN_DEAL_BRO, player.UserId);
                    }
                }
            }
           
        }

        /// <summary>
        /// 用户离开
        /// </summary>
        /// <param name="client"></param>
        private void leave(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
                () =>
                {
                    if (userCache.IsOnline(client) == false)
                    {
                        return;
                    }
                    int userId = userCache.GetId(client);
                    if (fightCache.IsFighting(userId) == false)
                    {
                        return;
                    }
                    FightRoom room = fightCache.GetRoomByUId(userId);
                    //就算中途退出的人
                    room.LeaveUIdList.Add(userId);
                    //给逃跑玩家添加逃跑场次
                    for (int i = 0; i < room.LeaveUIdList.Count; i++)
                    {
                        UserModel um = userCache.GetModelById(room.LeaveUIdList[i]);
                        um.RunCount++;
                        foreach(var player in room.PlayerList)
                        {
                            if(player.UserId== room.LeaveUIdList[i])
                            {    
                                    um.Been -= 500;
                            }
                        }
                        um.Exp += 0;
                        userCache.Update(um);
                    }
                    brocast(room, OpCode.FIGHT, FightCode.LEAVE_BRO, null);
                    fightCache.Destroy(room);

                }
                );
        }

        /// <summary>
        /// 不出的处理
        /// </summary>
        /// <param name="client"></param>
        private void pass(ClientPeer client,bool value)
        {
            SingleExecute.Instance.Execute(
                () =>
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);
                    FightRoom room = fightCache.GetRoomByUId(userId);
                    if(value==true)
                    {
                        if (room.roundModel.BiggestUId == userId)
                        {
                            //由于计时结束不出牌换人
                            client.Send(OpCode.FIGHT, FightCode.PASS_SRES, 0);
                            turn(room);
                            room.roundModel.BiggestUId = room.roundModel.CurrentUId;
                            return;
                        }
                        else
                        {
                            client.Send(OpCode.FIGHT, FightCode.PASS_SRES, 0);
                            turn(room);
                        }
                    }
                    else
                    {
                        if (room.roundModel.BiggestUId == userId)
                        {
                           //当前玩家是最大出牌者 没人管他，不能不出
                            client.Send(OpCode.FIGHT, FightCode.PASS_SRES, -1);
                            return;
                        }
                        else
                        {
                            client.Send(OpCode.FIGHT, FightCode.PASS_SRES, 0);
                            turn(room);
                        }
                    }
                });
        }

        /// <summary>
        /// 出牌的处理
        /// </summary>
        private void deal(ClientPeer client, DealDto dto)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线

                    int userId = userCache.GetId(client);

                    if (userId != dto.UserId)
                    {
                        return;
                    }
                    FightRoom room = fightCache.GetRoomByUId(userId);
                    //玩家出牌
                    //玩家已经中途退出 掉线
                    if (room.LeaveUIdList.Contains(userId))
                    {
                        //直接转换出牌
                        turn(room);
                    }
                    //玩家还在
                    bool canDeal = room.DealCard(dto.Type, dto.Weight, dto.Length, userId, dto.SelectCardList);
                    if (canDeal == false && room.roundModel.BiggestUId != userId)
                    {
                        //玩家出的牌管不上上一个玩家出的牌
                        client.Send(OpCode.FIGHT, FightCode.DEAL_SRES, -1);
                        return;
                    }
                    else
                    {
                        //出牌成功
                        //广播 给所有的客户端
                        List<CardDto> remainCardList = room.GetPlayerModel(userId).CardList;
                        dto.RemainCardList = remainCardList;
                        brocast(room, OpCode.FIGHT, FightCode.DEAL_BRO, dto);
                        //检测一下剩余手牌，如果手牌数为0，那就游戏结束了
                        if (remainCardList.Count == 0)
                        {
                            //游戏结束
                            gameOver(userId, room);
                        }
                        else
                        {
                            //转换出牌
                            turn(room);
                        }
                    }
                }
                );
        }

        /// <summary>
        /// 游戏结束
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="room"></param>
        private void gameOver(int userId, FightRoom room)
        {
            //获取获胜身份 所有玩家id
            int winIdentity = room.GetPlayerIdentity(userId);
            //给胜利玩家添加胜场
            List<int> winUIds = room.GetSameIdentityUIds(winIdentity);

            for (int i = 0; i < winUIds.Count; i++)
            {
                UserModel um = userCache.GetModelById(winUIds[i]);
                int multiple;
                room.multiDict.TryGetValue(winUIds[i], out multiple);
                um.WinCount++;
                foreach (var player in room.PlayerList)
                {
                    if (player.UserId==winUIds[i] )
                    {
                        if (player.Identity == Identity.FARMER)
                        {
                            um.winBeen += multiple * 100;
                            um.Been +=um.winBeen;
                           
                        }
                        else if(player.Identity==Identity.LANDLORD)
                        {
                            um.winBeen += 400;
                            um.Been += um.winBeen;
                        }
                    }
                }
                um.Exp += 100;
                int maxExp = um.Lv * 100;
                while (maxExp <= um.Exp)
                {
                    um.Lv++;
                    um.Exp -= maxExp;
                    maxExp = um.Lv * 100;
                }
                userCache.Update(um);

                //给客户端发消息 赢得身份是什么？谁赢了？加多少豆子？
                OverDto dto = new OverDto();
                dto.WinIdentity = winIdentity;
                dto.WinUIdList = winUIds;
                dto.tempUIdList = winUIds;
                dto.BeenCount = um.winBeen;
                singleBrocast(room, OpCode.FIGHT, FightCode.OVER_BRO, dto, winUIds[i]);
                um.winBeen = 0;
            }
            //给失败的玩家添加负场
            List<int> loseUIds = room.GetDifferentIdentityUIds(winIdentity);
            for (int i = 0; i < loseUIds.Count; i++)
            {
                UserModel um = userCache.GetModelById(loseUIds[i]);
                int multiple;
                room.multiDict.TryGetValue(loseUIds[i], out multiple);
                um.LoseCount++;
                foreach (var player in room.PlayerList)
                {
                    if (player.UserId == loseUIds[i])
                    {
                        if (player.Identity == Identity.FARMER)
                        {
                            um.winBeen -= multiple * 100;
                            if (um.winBeen >= 0)
                            {
                                um.Been -= um.winBeen;
                            }
                            else
                            {
                                um.Been+= um.winBeen;
                            }
                        }
                        else if (player.Identity == Identity.LANDLORD)
                        {
                            um.winBeen -= 400;
                            if(um.winBeen>=0)
                            {
                                um.Been -= um.winBeen;
                            }
                            else
                            {
                                um.Been += um.winBeen;
                            }
                        }
                    }
                }
                um.Exp += 10;
                int maxExp = um.Lv * 100;
                while (maxExp <= um.Exp)
                {
                    um.Lv++;
                    um.Exp -= maxExp;
                    maxExp = um.Lv * 100;
                }
                userCache.Update(um);
                //给客户端发消息 赢得身份是什么？谁赢了？加多少豆子？
                OverDto dto = new OverDto();
                dto.WinIdentity = winIdentity;
                dto.WinUIdList = winUIds;
                dto.tempUIdList = loseUIds;
                dto.BeenCount = Math.Abs(um.winBeen);
                singleBrocast(room, OpCode.FIGHT, FightCode.OVER_BRO, dto, loseUIds[i]);
                um.winBeen = 0;
            }
            //给逃跑玩家添加逃跑场次
            for (int i = 0; i < room.LeaveUIdList.Count; i++)
            {
                UserModel um = userCache.GetModelById(room.LeaveUIdList[i]);
                int multiple;
                room.multiDict.TryGetValue(loseUIds[i], out multiple);
                um.RunCount++;
                foreach (var player in room.PlayerList)
                {
                    if (player.UserId == winUIds[i])
                    {
                        if (player.Identity == Identity.FARMER)
                        {
                            um.winBeen -= multiple * 100;
                            if (um.winBeen >= 0)
                            {
                                um.Been -= um.winBeen;
                            }
                            else
                            {
                                um.Been += um.winBeen;
                            }
                        }
                        else if (player.Identity == Identity.LANDLORD)
                        {
                            um.winBeen -= 1200;
                            if (um.winBeen >= 0)
                            {
                                um.Been -= um.winBeen;
                            }
                            else
                            {
                                um.Been += um.winBeen;
                            }
                        }
                    }
                }
                um.Exp += 0;
                um.winBeen = 0;
                userCache.Update(um);
                
            }

            //在缓存层销毁房间数值
            fightCache.Destroy(room);
        }

        /// <summary>
        /// 转换出牌
        /// </summary>
        /// <param name="room"></param>
        private void turn(FightRoom room)
        {
            int nextUId = room.Turn();
            if (room.IsOffline(nextUId) == true)
            {
                //如果下一个玩家掉线了 递归直到不掉线的玩家出牌为止
                turn(room);
            }
            else
            {
                //玩家不掉线就给他发信息
                // ClientPeer nextClient = userCache.GetClientPeer(nextUId);
                // nextClient.Send(OpCode.FIGHT, FightCode.TURN_DEAL_BRO, nextUId);
                brocast(room, OpCode.FIGHT, FightCode.TURN_DEAL_BRO, nextUId);
            }
        }

        /// <summary>
        /// 抢地主的处理
        /// </summary>
        private void grabLandlord(ClientPeer client, bool result)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    if (userCache.IsOnline(client) == false)
                        return;
                    //必须确保在线
                    int userId = userCache.GetId(client);
                    FightRoom room = fightCache.GetRoomByUId(userId);
                    if (result == true)
                    {
                        room.isMulti = true;
                        //抢
                        room.SetLandlord(userId);
                        //给每一个客户发一个消息 谁当了地主
                        GrabDto dto = new GrabDto(userId, room.TableCardList, room.GetUserCards(userId));
                        brocast(room, OpCode.FIGHT, FightCode.GRAB_LANDLORD_BRO, dto);
                        foreach(var player in room.PlayerList)
                        {
                            if(player.Identity== Identity.FARMER)
                            {
                                //发送给平民翻倍命令
                                singleBrocast(room, OpCode.FIGHT, FightCode.MULTIPLE_BRO, 1, player.UserId);
                            }
                            else
                            {
                                singleBrocast(room, OpCode.FIGHT, FightCode.MULTIPLE_BRO, 2, player.UserId);
                            }
                        }
                    }
                    else
                    {
                        //不抢
                        int nextUId = room.GetNextUId(userId);
                        brocast(room, OpCode.FIGHT, FightCode.TURN_GRAB_BRO, nextUId);
                    }
                }
                );
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        public void startFight(List<int> uIdList)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                    //创建战斗房间
                    FightRoom room = fightCache.Create(uIdList);
                    room.InitPlayerCards();
                    room.Sort();
                    //发送设置按钮响应
                    brocast(room, OpCode.FIGHT, FightCode.BUTTON_BRO, null);
                    //发送给每个客户端 它自身有什么用
                    foreach (int uid in uIdList)
                    {
                        ClientPeer client = userCache.GetClientPeer(uid);
                        List<CardDto> cardList = room.GetUserCards(uid);
                        client.Send(OpCode.FIGHT, FightCode.GET_CARD_SRES, cardList);
                    }
                    //发送开始抢地主的响应
                    int firstUserId = room.GetFirstUId();
                    brocast(room, OpCode.FIGHT, FightCode.TURN_GRAB_BRO, firstUserId, null);
                });
        }

        /// <summary>
        /// 广播给一个房间内的所有玩家
        /// </summary>
        private void brocast(FightRoom room, int opCode, int subCode, object value, ClientPeer exClient = null)
        {

            SocketMsg msg = new SocketMsg();
            msg.OpCode = opCode;
            msg.SubCode = subCode;
            msg.Value = value;
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);
            foreach (var player in room.PlayerList)
            {
                if (userCache.IsOnline(player.UserId))
                {
                    ClientPeer client = userCache.GetClientPeer(player.UserId);
                    if (client == exClient)
                    {
                        continue;
                    }
                    client.Send(packet);
                }
            }
        }


        /// <summary>
        /// 广播给一个房间内的一个玩家
        /// </summary>
        /// <param name="room"></param>
        /// <param name="opCode"></param>
        /// <param name="subCode"></param>
        /// <param name="value"></param>
        /// <param name="exClient"></param>
        private void singleBrocast(FightRoom room, int opCode, int subCode, object value, int userId, ClientPeer exClient = null)
        {

            SocketMsg msg = new SocketMsg();
            msg.OpCode = opCode;
            msg.SubCode = subCode;
            msg.Value = value;
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);
            foreach (var player in room.PlayerList)
            {
                if (userCache.IsOnline(player.UserId))
                {
                    ClientPeer client = userCache.GetClientPeer(player.UserId);
                    if (player.UserId == userId)
                    {
                        if (client == exClient)
                        {
                            continue;
                        }
                        client.Send(packet);
                    }
                }

            }

        }


    }
}
