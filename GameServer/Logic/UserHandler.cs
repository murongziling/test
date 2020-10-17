using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AhpilySever;
using GameServer.Cache;
using GameServer.Model;
using Protocol.Code;
using Protocol.Dto;

namespace GameServer.Logic
{
    /// <summary>
    /// 用户逻辑处理层
    /// </summary>
  public  class UserHandler:IHandler
    {

        private UserCache userCache = Caches.User;
        private AccountCache accountCache = Caches.Account;

        public void OnDisconnect(ClientPeer client)
        {
            if (userCache.IsOnline(client))
                userCache.Offline(client);
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
           switch(subCode)
            {
                case UserCode.CREATE_CREQ:
                    create(client, value.ToString());
                    break;
                case UserCode.GET_INFO_CREQ:
                    getInfo(client);
                    break;
                case UserCode.ONLINE_CREQ:
                    online(client);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="client">客户端的连接对象</param>
        /// <param name="name">客户端传过来的名字</param>
        private void create(ClientPeer client,string name)
        {
            SingleExecute.Instance.Execute(
                delegate ()
                {
                   //判断这个客户端是不是非法登陆
                   if (!accountCache.IsOnline(client))
                    {
                        client.Send(OpCode.USER, UserCode.CREATE_SRES, -1); //客户端非法登陆
                       return;
                    }

                   //获取账号id
                   int accountId = accountCache.GetId(client);
                   //判断一下这个账号以前有没有角色
                   if (userCache.IsExist(accountId))
                    {
                        client.Send(OpCode.USER, UserCode.CREATE_SRES, -2);//已经有角色，不能重复创建
                       return;
                    }

                   //没有问题才能创建
                   userCache.Create(name, accountId);
                    client.Send(OpCode.USER, UserCode.CREATE_SRES, 0);//创建成功、
               });
        }
        
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="client"></param>
        private void getInfo(ClientPeer client)
        {
            SingleExecute.Instance.Execute(
               delegate ()
               {
                   //判断这个客户端是不是非法登陆
                   if (!accountCache.IsOnline(client))
                   {
                       //client.Send(OpCode.USER, UserCode.GET_INFO_SRES, -1); //客户端非法登陆
                       return;
                   }
                   int accountId = accountCache.GetId(client);
                   if (userCache.IsExist(accountId) == false)
                   {
                       client.Send(OpCode.USER, UserCode.GET_INFO_SRES, null); //没有创建角色 不能获取信息
                       return;
                   }
                   //代码执行到这里就代表有角色
                   //上线角色
                   //防止二次调用上线的方法
                   if(userCache.IsOnline(client)==false)
                   {
                       online(client);
                   }
                   //给客户端发送自己的角色信息
                   UserModel model = userCache.GetModelByAccountId(accountId);
                   UserDto dto = new UserDto(model.Id,model.Name, model.Been, model.WinCount, model.LoseCount, model.RunCount, model.Lv, model.Exp);
                   client.Send(OpCode.USER, UserCode.GET_INFO_SRES, dto); //获取成功
                
               });
        }

        /// <summary>
        /// 角色上线
        /// </summary>
        /// <param name="client"></param>
        private void online(ClientPeer client)
        {

            SingleExecute.Instance.Execute(
               delegate ()
               {
                   if (!accountCache.IsOnline(client))
                   {
                       client.Send(OpCode.USER, UserCode.ONLINE_SRES, -1);
                       return;
                   }
                   int accountId = accountCache.GetId(client);
                   if (userCache.IsExist(accountId) == false)
                   {
                       client.Send(OpCode.USER, UserCode.ONLINE_SRES, -2); //没有角色，不能上线
                       return;
                   }

                   //上线成功了
                   int userId = userCache.GetId(accountId);
                   Console.WriteLine("角色id"+accountId);
                   userCache.Online(client, userId);
                   client.Send(OpCode.USER, UserCode.ONLINE_SRES, 0); //上线成功
               });
                
        }
    }
}
