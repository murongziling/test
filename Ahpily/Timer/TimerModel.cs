using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilySever.Timer
{
    public delegate void TimeDelegate();

    //定时器任务的数据模型
  public  class TimerModel
    {
       public int Id;

        //任务执行的事件

        public long Time;
        private TimeDelegate timeDelegate;

        public TimerModel(int id,long time,TimeDelegate td)
        {
            this.Id = id;
            this.Time = time;
            this.timeDelegate = td;
        }


        //触发任务

        public void Run()
        {
            timeDelegate();
        }
    }
}
