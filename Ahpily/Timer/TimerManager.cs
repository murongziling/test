using AhpilySever.Concurrent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AhpilySever.Timer
{
 public   class TimerManager
    {
        private static TimerManager instance = null;

        public static TimerManager Instance
        {
            get
            {
                lock(instance)
                {
                    if (instance == null)
                        instance = new TimerManager();
                    return instance;
                }

            }
        }

        //实现定时器的主要功能就是这个Timer类
        private System.Timers.Timer timer;
        //这个字典存储：任务id  和  任务模型的映射
        private ConcurrentDictionary<int, TimerModel> idModelDict = new ConcurrentDictionary<int, TimerModel>();
        //要移除的id列表
        private List<int> removeList = new List<int>();
        //用来表示id
        private ConcurrentInt id = new ConcurrentInt(-1);

        public long nowTime;
        public TimerManager()
        {
            timer = new System.Timers.Timer(10); //10为时间间隔
            /* timer.AutoReset = false;  true的话就每隔10ms执行Timer_Elapsed false的话就只执行1次*/
            //  timer.Enabled = true;      // true开启计时 false 关闭计时        
            timer.Enabled = true;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed; //到10ms就执行Timer_Elapsed方法
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            lock(removeList)
            {
                TimerModel tmpModel = null;
                foreach (var id in removeList)
                {
                    idModelDict.TryRemove(id, out tmpModel);
                }
                removeList.Clear();
            }
          
           foreach(var model in idModelDict.Values)
            {
                if (model.Time<=DateTime.Now.Ticks)
                {
                    model.Run();
                    removeList.Add(model.Id);
                }
               
            }
         
        }

        /// <summary>
        /// 添加定时任务  指定触发时间 2018年9月11日20:19:40
        /// </summary>
        /// <param name="datatime"></param>
        /// <param name="timeDelegate"></param>
        public void AddTimeEvent(DateTime datatime,TimeDelegate timeDelegate)
        {
            long delayTime=datatime.Ticks - DateTime.Now.Ticks;
            if(delayTime<=0)
                return;
           AddTimeEvent(delayTime, timeDelegate);
            
        }

        
        /// <summary>
        /// 添加定时任务 指定延迟的时间  30s
        /// </summary>
        /// <param name="delayTime">毫秒</param>
        /// <param name="timeDelegate"></param>
        public void AddTimeEvent(long delayTime,TimeDelegate timeDelegate)
        {
            TimerModel model = new TimerModel(id.Add_Get(), DateTime.Now.Ticks + delayTime, timeDelegate);
            idModelDict.TryAdd(model.Id, model);
        }
    }
}
