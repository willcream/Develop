using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Develop
{
    public class WillLogger
    {
        private static WillLogger logger = new WillLogger();
        private static readonly object objLock = new object();
        public static bool IsInitialized { get; set; }

        private Thread writeThread;
        public bool IsRun { get; set; }
        private bool finished;
        private List<string> msgList = new List<string>();
        private List<StringBuilder> cacheList = new List<StringBuilder>();
        private UTF8Encoding encoding = new UTF8Encoding();
        private int WriteQty = 180;
        private ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

        private int cacheLength = 1024;
        private int longestStringLen = 1;

        private int counter = 0;


        private WillLogger()
        {
            IsInitialized = false;
            IsRun = true;
            finished = true;
            IsInitialized = true;
            FileInfo fi = new FileInfo("E:\\Log.txt");
            writeThread = new Thread(delegate ()
           {
               while (IsRun)
               {
                   if (finished)
                   {
                       WriteIt(fi);
                   }
                   Thread.Sleep(1000);
               }
           });
            writeThread.Start();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static WillLogger Get()
        {
            lock (objLock)
            {
                if (logger == null)
                    logger = new WillLogger();
                return logger;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <param name="msg"></param>
        /// <param name="tag"></param>
        public static void Info(string msg, string tag = null)
        {
            string template = "{0} | Info | {1} | {2}";
            tag = tag == null ? "BASIC" : tag;
            string temp = string.Format(template, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), tag, msg);

            Get().Record(temp);
        }


        private void Record(string msg)
        {
            // 先记录
            msgList.Add(msg);
            if (msg.Length > longestStringLen)
                longestStringLen = msg.Length;

        }




        private bool WriteIt(FileInfo fi)
        {
            StringBuilder readySb = new StringBuilder(cacheLength);
            //int count1 = msgList.Count;
            //int actualQty = count1 > WriteQty ? WriteQty : msgList.Count;
            //for (int i = 0; i < actualQty; i++)
            //{
            //    readySb.Append(msgList[i]);
            //    readySb.Append(Environment.NewLine);
            //}
            //if (readySb.Length > 0)
            //{
            //    cacheList.Add(readySb);
            //    msgList.RemoveRange(0, actualQty);
            //}

            //if (count2 > maxRow || longestStringLen > maxSingleSize) { }

            int count2 = msgList.Count;
            // 如果msgList已经很大了，继续下去可能会撑爆内存
            // list属于托管对象，无法使用Marshal.SizeOf计算内存占用
            StringBuilder urgenSb = new StringBuilder(cacheLength * 6);
            for (int i = 0; i < count2; i++)
            {
                urgenSb.Append(msgList[i]);
                urgenSb.Append(Environment.NewLine);
            }
            if (urgenSb.Length > 0)
            {
                msgList.RemoveRange(0, count2);
                cacheList.Add(urgenSb);
                longestStringLen = 1; // 重置
                counter += count2;
            }

            try
            {
                finished = false;
                lockSlim.EnterWriteLock();
                using (StreamWriter sw = fi.AppendText())
                {
                    int listCount = cacheList.Count;
                    for (int i = 0; i < listCount; i++)
                    {
                        StringBuilder sb = cacheList[i];
                        sw.Write(sb.ToString());
                    }
                    cacheList.RemoveRange(0, listCount);
                    finished = true;

                    counter += listCount;
                    
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return false;
            }
            finally
            {
                if (lockSlim.IsWriteLockHeld)
                    lockSlim.ExitWriteLock();

                Console.WriteLine(counter);
            }

        }
    }
}
