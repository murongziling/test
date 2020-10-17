using AhpilySever;
using AhpilySever.Concurrent;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameServer.Cache
{
    /// <summary>
    /// 角色数据缓存层
    /// </summary>
 public    class UserCache
    {
        //角色id对应的角色数据模型
        private Dictionary<int, UserModel> idModelDict = new Dictionary<int, UserModel>();

        //账号id对应的角色id

        private Dictionary<int, int> accIdUIdDict = new Dictionary<int, int>();

       
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="name">角色名</param>
        /// <param name="accountId">账号id</param>
        public void Create( string name,int accountId)
        {
            UserModel model = new UserModel();
            model.Name = name;
            model.AccountId = accountId;
            model.Add();
            //保存到字典里
            idModelDict.Add(model.Id, model);
            accIdUIdDict.Add(accountId, model.Id);
        }

        /// <summary>
        /// 判断此账号是否有角色
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool IsExist(int accountId)
        {
            init(accountId);
            return accIdUIdDict.ContainsKey(accountId);
        }

        /// <summary>
        /// 根据账号di获取角色数据模型
        /// </summary>

        public UserModel GetModelByAccountId(int accountId)
        {
            init(accountId);
            if (!accIdUIdDict.ContainsKey(accountId)) return null;
            return idModelDict[accIdUIdDict[accountId]];
        }


        /// <summary>
        /// 根据账号di获取角色id
        /// </summary>
        /// <returns></returns>
        public int GetId(int accountId)
        {
            init(accountId);
            return accIdUIdDict[accountId];
        }

        //存储在线玩家，只有在线玩家才有这个(ClientPeer)对象
        private Dictionary<int, ClientPeer> idClientDict = new Dictionary<int, ClientPeer>();
        private Dictionary<ClientPeer, int> clientIdDict = new Dictionary<ClientPeer, int>();

        /// <summary>
        /// 是否在线
        /// </summary>
        /// <param name="clientPeer">客户端连接对象</param>
        /// <returns></returns>
        public bool IsOnline(ClientPeer client)
        {
            return clientIdDict.ContainsKey(client);
        }

        public bool IsOnline(int id)
        {
            return idClientDict.ContainsKey(id);
        }

        /// <summary>
        /// 角色上线
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        public void Online(ClientPeer client,int id)
        {
            idClientDict.Add(id, client);
            clientIdDict.Add(client, id);
        }


        /// <summary>
        /// 角色下线
        /// </summary>
        /// <param name="client"></param>
        public void Offline(ClientPeer client)
        {
            int id = clientIdDict[client];
            clientIdDict.Remove(client);
            idClientDict.Remove(id);
        }

        /// <summary>
        /// 根据连接对象获取角色模型
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public UserModel GetModelByClientPeer(ClientPeer client)
        {
            int id = clientIdDict[client];
            UserModel model = idModelDict[id];
            return model;
        }

        /// <summary>
        /// 根据用户id获取数据模型
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public UserModel GetModelById(int userId)
        {
          UserModel user=  idModelDict[userId];
            return user;
        }

        /// <summary>
        /// 根据角色id获取连接对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientPeer GetClientPeer(int id)
        {
            return idClientDict[id];
        }

        /// <summary>
        /// 根据在线玩家的连接对象获取角色id
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public int GetId(ClientPeer client)
        {
            if(!clientIdDict.ContainsKey(client))
            {
                throw new IndexOutOfRangeException("这个玩家不在线的字典里面存储！");
            }
            return clientIdDict[client];
           
        }

        private void init(int accountId)
        {
            if (accIdUIdDict.ContainsKey(accountId)) return;
            UserModel user = new UserModel(accountId);
            if(user.Id>=0)
            {
                accIdUIdDict.Add(accountId, user.Id);
                idModelDict.Add(user.Id, user);
            }
        }

        public void Update(UserModel user)
        {
            user.Update();
            idModelDict[user.Id] = user;
        }
    }
}
