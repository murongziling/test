using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Dto
{
    /// <summary>
    /// 房间数据对应的传输模型
    /// </summary>
    [Serializable]
    public class MatchRoomDto
    {
        /// <summary>
        /// 用户id对应的用户数据传输模型
        /// </summary>
        public Dictionary<int, UserDto> UIdUserDict;

        /// <summary>
        /// 准备的玩家id列表
        /// </summary>
        public List<int> ReadyUIdList;

        /// <summary>
        /// 存储玩家进入的顺序
        /// </summary>
        public List<int> uIdList;


        public MatchRoomDto()
        {
            this.UIdUserDict = new Dictionary<int, UserDto>();
            this.ReadyUIdList = new List<int>();
            this.uIdList = new List<int>();
        }

        public void Add(UserDto newUser)
        {
            this.UIdUserDict.Add(newUser.Id, newUser);
            this.uIdList.Add(newUser.Id);
        }

        public void Leave(int userId)
        {
            this.UIdUserDict.Remove(userId);
            this.uIdList.Remove(userId);
        }

        public void Ready(int userId)
        {
            this.ReadyUIdList.Add(userId);
        }

        public int LeftId;//左边玩家的id
        public int RightId; //代表右边玩家的id

        /// <summary>
        /// 重置位置
        /// 在每次玩家进入或者离开房间的时候 都需要调整一下位置
        /// </summary>
        public void ResetPosition(int myUserId)
        {
            LeftId = -1;
            RightId = -1;
            //1
            if(uIdList.Count==1)
            {
                
            }
            //2
          else  if(uIdList.Count==2)
            {
                if(uIdList[0]==myUserId)
                {
                    RightId = uIdList[1];
                }
                if(uIdList[1]==myUserId)
                {
                    LeftId = uIdList[0];
                }
            }
            //3
            else if(uIdList.Count==3)
            {
                if(uIdList[0]==myUserId)
                {
                    LeftId = uIdList[2];
                    RightId = uIdList[1];
                }
                if(uIdList[1]==myUserId)
                {
                    LeftId = uIdList[0];
                    RightId = uIdList[2];
                }
                if (uIdList[2] == myUserId)
                {
                    LeftId = uIdList[1];
                    RightId = uIdList[0];
                }
            }
        }
    }
}
