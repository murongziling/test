using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilySever.Concurrent
{
    //线程安全的int类型
  public  class ConcurrentInt
    {

        private int value;

        public ConcurrentInt(int value)
        {
            this.value = value;
        }

        //添加并获取

        public int Add_Get()
        {
            lock(this)
            {
                value++;
                return value;
            }
        }

        //减少并获取

        public int Reduce_Get()
        {
            lock (this)
            {
                value--;
                return value;
            }
        }

        //获取

        public int Get()
        {
            return value;
        }
    }
}
