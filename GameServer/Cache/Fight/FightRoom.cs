using AhpilySever;
using GameServer.Model;
using Protocol.Code;
using Protocol.Constant;
using Protocol.Dto.fight;
using Protocol.Dto.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache.Fight
{

    
    /// <summary>
    /// 战斗房间
    /// </summary>
    public class FightRoom
    {

        private UserCache userCache = Caches.User;
        /// <summary>
        /// 房间唯一标识码
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// 存储所有玩家
        /// </summary>
        public List<PlayerDto> PlayerList { get; set; }
        /// <summary>
        /// 中途退出的玩家id列表
        /// </summary>
        public List<int> LeaveUIdList { get; set; }

        /// <summary>
        /// 牌库
        /// </summary>
        public LibraryModel libraryModel { get; set; }
        /// <summary>
        /// 底牌
        /// </summary>
        public List<CardDto> TableCardList { get; set; }
        /// <summary>
        ///翻倍字典:id,倍数
        /// </summary>
        public Dictionary<int, int> multiDict { get; set; }
        /// <summary>
        /// 回合管理类
        /// </summary>
        public RoundModel roundModel { get; set; }

        /// <summary>
        /// 判断是否在翻倍
        /// </summary>
        public bool isMulti { get; set; }

        /// <summary>
        /// 构造方法 做初始化
        /// </summary>
        /// <param name="id"></param>
        public FightRoom(int id, List<int> uidList)
        {
            this.Id = id;
            this.PlayerList = new List<PlayerDto>();
            foreach (int uid in uidList)
            {
                PlayerDto player = new PlayerDto(uid);
                this.PlayerList.Add(player);
            }
            this.LeaveUIdList = new List<int>();
            this.libraryModel = new LibraryModel();
            this.TableCardList = new List<CardDto>();
            this.multiDict = new Dictionary<int, int>();
            this.roundModel = new RoundModel();
            this.isMulti = false;
        }

        public void Init(List<int> uidList)
        {
            foreach (int uid in uidList)
            {
                PlayerDto player = new PlayerDto(uid);
                this.PlayerList.Add(player);
            }
        }

        public bool IsOffline(int uid)
        {
            return LeaveUIdList.Contains(uid);
        }

        /// <summary>
        /// 装换出牌
        /// </summary>
        public int Turn()
        {
            int currUId = roundModel.CurrentUId;
            int nextUId = GetNextUId(currUId);
            //更改回合信息
            roundModel.CurrentUId = nextUId;
            return nextUId;
        }

        /// <summary>
        /// 如何计算下一个出牌者
        /// </summary>
        /// <param name="currUId"></param>
        /// <returns></returns>
        public int GetNextUId(int currUId)
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (PlayerList[i].UserId == currUId)
                {
                    if (i == 2)
                        return PlayerList[0].UserId;
                    else
                        return PlayerList[i + 1].UserId;
                }
            }
            throw new Exception("并没有这个出牌者");
        }

        /// <summary>
        /// 出牌(判断能不能压上一回合的牌)
        /// </summary>
        /// <returns></returns>
        public bool DealCard(int type, int weight, int length, int userId, List<CardDto> cardList)
        {
            bool canDeal = false;
            //用什么牌管什么牌 大的才能管小的
            if (type == roundModel.LastCardType && weight > roundModel.LastWeight)
            {
                //特殊的类型，顺子 还要进行长度的限制
                if (type == CardType.STRAIGHT || type == CardType.DOUBLE_STRAIGHT || type == CardType.TRIPLE_STRAIGHT)
                {
                    if (length == roundModel.LastLength)
                    {
                        //满足出牌条件
                        canDeal = true;
                    }
                }
                else
                {
                    //满足出牌条件
                    canDeal = true;
                }
            }
            //普通的炸弹 可以管
            else if (type == CardType.BOOM && roundModel.LastCardType != CardType.BOOM)
            {
                //满足出牌条件
                canDeal = true;
            }
            //王炸可以管任何牌
            else if (type == CardType.JOKER_BOOM)
            {
                //满足出牌条件
                canDeal = true;
            }
            //第一次出牌或者当前自己是最大出牌者
            else if(userId==roundModel.BiggestUId)
            {
                canDeal = true;
            }

            //出牌
            if (canDeal)
            {
                //移除玩家的手牌
                removeCards(userId, cardList);
                //可能会翻倍
                if (type == CardType.BOOM)
                {
                    foreach(var player in this.PlayerList)
                    {
                        if(player.UserId==userId)
                        {
                            UserModel um = userCache.GetModelById(player.UserId);
                            um.winBeen += 300;
                            this.singleBrocast(this, OpCode.FIGHT, FightCode.REWARD_BRO, 1,userId);
                        }
                    }
                }
                else if (type == CardType.JOKER_BOOM)
                {
                    foreach (var player in this.PlayerList)
                    {
                        if (player.UserId == userId)
                        {
                            UserModel um = userCache.GetModelById(player.UserId);
                            um.winBeen += 400;
                            this.singleBrocast(this, OpCode.FIGHT, FightCode.REWARD_BRO, 2, userId);
                        }
                    }
                }
                //保存回合信息
                roundModel.Change(length, type, weight, userId);
            }

            return canDeal;
        }

        /// <summary>
        /// 移除玩家手牌
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cardList"></param>
        private void removeCards(int userId, List<CardDto> cardList)
        {
            //获取玩家现有的手牌
            //List<CardDto> currList = GetUserCards(userId);
            //for (int i = currList.Count - 1; i >= 0; i--)
            //{
            //    foreach (CardDto temp in cardList)
            //    {
            //        if (currList[i].Name == temp.Name)
            //        {
            //            currList.Remove(currList[i]);
            //        }
            //    }
            //}

            List<CardDto> currList = GetUserCards(userId);
            List<CardDto> list = new List<CardDto>();
            foreach(var select in cardList)
            {
                for(int i=currList.Count-1;i>=0;i--)
                {
                    if(currList[i].Name==select.Name)
                    {
                        list.Add(currList[i]);
                        break;
                    }
                }
            }

            foreach (var card in list)
                currList.Remove(card);
        }

        /// <summary>
        /// 获取玩家现有手牌
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<CardDto> GetUserCards(int userId)
        {
            foreach (PlayerDto player in PlayerList)
            {
                if (player.UserId == userId)
                    return player.CardList;
            }
            throw new Exception("不存在玩家");
        }

        /// <summary>
        /// 发牌
        /// </summary>
        public void InitPlayerCards()
        {
            //54张牌 一个人17张
            //剩3张当底牌
            //谁是房主就给谁
            for (int i = 0; i < 17; i++)
            {
                CardDto card = libraryModel.Deal();
                PlayerList[0].Add(card);
            }
            for (int i = 0; i < 17; i++)
            {
                CardDto card = libraryModel.Deal();
                PlayerList[1].Add(card);
            }
            for (int i = 0; i < 17; i++)
            {
                CardDto card = libraryModel.Deal();
                PlayerList[2].Add(card);
            }
            //发底牌
            for(int i=0;i<3;i++)
            {
                CardDto card = libraryModel.Deal();
                TableCardList.Add(card);
            }
        }

        /// <summary>
        /// 设置地主身份
        /// </summary>
        public void SetLandlord(int userId)
        {
            foreach(PlayerDto player in PlayerList)
            {
                if(player.UserId==userId)
                {
                    //找对人了
                    player.Identity = Identity.LANDLORD;
                    //给地主发底牌
                    for(int i=0;i<TableCardList.Count;i++)
                    {
                        player.Add(TableCardList[i]);
                    }
                    //重新排序
                    this.Sort();
                    //开始回合
                    roundModel.Start(userId);
                }
            }
        }

        /// <summary>
        /// 获取玩家的数据模型
        /// </summary>
        public PlayerDto GetPlayerModel(int userId)
        {
            foreach(PlayerDto player in PlayerList)
            {
                if(player.UserId==userId)
                {
                    return player;
                }
            }
            throw new Exception("没有这个玩家！获取不到数据");

        }

        /// <summary>
        /// 获取用户身份
        /// </summary>
        public int GetPlayerIdentity(int userId)
        {
            return GetPlayerModel(userId).Identity;
            throw new Exception("没有这个玩家！获取不到数据");
        }

        /// <summary>
        /// 获取不同身份的用户id
        /// </summary>
        public List<int > GetDifferentIdentityUIds(int identity)
        {
            List<int> uids = new List<int>();
            foreach(PlayerDto player in PlayerList)
            {
                if(player.Identity!=identity)
                {
                    uids.Add(player.UserId);
                }
            }
            return uids;
        }

        /// <summary>
        /// 获取相同身份的用户id
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public List<int> GetSameIdentityUIds(int identity)
        {
            List<int> uids = new List<int>();
            foreach (PlayerDto player in PlayerList)
            {
                if (player.Identity == identity)
                {
                    uids.Add(player.UserId);
                }
            }
            return uids;
        }

        /// <summary>
        /// 获取房间内第一个玩家的id
        /// </summary>
        /// <returns></returns>
        public int GetFirstUId()
        {
            return PlayerList[0].UserId;
        }

        /// <summary>
        /// 排序手牌
        /// </summary>
        private void sortCard(List<CardDto> cardList,bool asc=true)
        {
            cardList.Sort(
                delegate (CardDto a, CardDto b)
                {
                    if (asc)
                        return a.Weight.CompareTo(b.Weight);
                    else
                        return a.Weight.CompareTo(b.Weight) * -1;
                });
        }

        /// <summary>
        /// 排序 默认升序
        /// </summary>
        /// <param name="asc"></param>
        public void Sort(bool asc =true)
        {
            sortCard(PlayerList[0].CardList, asc);
            sortCard(PlayerList[1].CardList, asc);
            sortCard(PlayerList[2].CardList, asc);
            sortCard(TableCardList, asc);
        }

        public void AddInMulti(int userId,int multi)
        {
            multiDict.Add(userId, multi);
        }
    
       

        /// <summary>
        /// 广播房间内的所有玩家信息
        /// </summary>
        public void Brocast(int opCode, int subCode, object value,ClientPeer exclient=null)
        {
            SocketMsg msg = new SocketMsg();
            msg.OpCode = opCode;
            msg.SubCode = subCode;
            msg.Value = value;
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);
           foreach(var player in PlayerList)
            {
                ClientPeer client = userCache.GetClientPeer(player.UserId);
                client.Send(packet);
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
            foreach (var player in this.PlayerList)
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
