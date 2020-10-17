﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code
{
    /// <summary>
    /// 有关匹配的一些操作码
    /// </summary>
  public  class MatchCode
    {
        //匹配
        public const int MATCH_CREQ = 11;
        public const int MATCH_SRES = 12;
        //进入匹配队列
        public const int ENTER_CREQ = 0;
        public const int ENTER_SRES = 1;
        public const int ENTER_BRO = 10;
        public const int ENTER_CREQ_OTHER = 13;
        //离开匹配队列
        public const int LEAVE_CREQ = 2;
     //public const int LEAVE_SRES= 3;
        public const int LEAVE_BRO = 3;
        //返回匹配队列
        public const int BACK_BRO = 14;
        //准备
        public const int READY_CREQ = 4;
        //public const int READY_SRES = 5;
        public const int READY_BRO = 5;
        //开始游戏
        //public const int START_CREQ = 6;
        // public const int START_SRES = 7; 
        public const int START_BRO = 6;

        public const int MESSAGE_BRO = 15;
    }
}