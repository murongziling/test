using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AhpilySever
{
    //一个需要执行的方法
    public delegate void ExcuteDelegate();


    //单线程池
  public  class SingleExecute
    {
        private static SingleExecute instance = null;
        public static SingleExecute Instance
        {
            get
            {
                lock(o)
                {
                    if (instance == null)
                        instance = new SingleExecute();
                    return instance;
                }
            }
        }
        private static object o=1;

        //互斥锁
        public Mutex mutex;

        public SingleExecute()
        {
            mutex = new Mutex();
        }

        //单线程处理逻辑

        public void Execute(ExcuteDelegate executeDelegate)
        {
            lock(this)
            {
                mutex.WaitOne();
                executeDelegate();
                mutex.ReleaseMutex();
            }
        }

    }
}
