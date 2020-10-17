using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AhpilySever;
using AhpilySever.Timer;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SeverPeer sever = new SeverPeer();
            //指定所关联的应用
            sever.SetApplication(new NetMsgCenter());
            sever.Start(9999, 10);
            EncodeTool.decodeObjDelegate = DecodeObj;
            //TimerManager timer = new TimerManager();
            Console.ReadKey();
       
        }

     


        private static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine(e.SignalTime.ToString());
        }





        public static object DecodeObj(byte[] valueBytes)
        {
            using (MemoryStream ms = new MemoryStream(valueBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object value = bf.Deserialize(ms);
                return value;
            }
        }
    }
}
