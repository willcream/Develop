using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Develop
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "上面的代码会在编译的时候出错: Cannot convert anonymous method to type 'System.Delegate' because it is not a delegate type. 方法要求参数的是一个委托(delegate)类型, 而现在传递的只是一个匿名方法. 产生这个错误最根本的原因是编译器在处理匿名方法的时候, 没法推断出这个委托的方法返回的是什么类型, 也就不知道返回一个什么样的委托";
            Random random = new Random();
            DateTime start = DateTime.Now;
            Parallel.For(0, 100, e =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    WillLogger.Info(j+s);
                    Thread.Sleep(random.Next(10) * 10);
                }
            });
            DateTime end = DateTime.Now;

            TimeSpan span = end.Subtract(start);
            Console.WriteLine(string.Format("运行了{0}分{1}秒", span.Minutes, span.Seconds));

            //List<string> list = new List<string>();
            //new Thread(delegate ()
            //{
            //    while (true)
            //    {
            //        if (list.Count == 0)
            //            continue;
            //        int tempI = (list.Count-1) / 2;
            //        list.RemoveRange(0, tempI);
            //        Thread.Sleep(1);
            //    }
            //}).Start();

            //try
            //{
            //    for (int j = 0; j < 100000000; j++)
            //    {
            //        list.Add(j+"");
            //        Thread.Sleep(1);
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

        }

        //static int LogCount = 10000;
        //static int WritedCount = 0;
        //static int FailedCount = 0;

        //static void Main(string[] args)
        //{
        //    //迭代运行写入日志记录
        //    Parallel.For(0, LogCount, e =>
        //    {
        //        WriteLog();
        //    });

        //    Console.WriteLine(string.Format("\r\nLog Count:{0}.\t\tWrited Count:{1}.\tFailed Count:{2}.", LogCount.ToString(), WritedCount.ToString(), FailedCount.ToString()));
        //    Console.Read();
        //}

        ////读写锁，当资源处于写入模式时，其他线程写入需要等待本次写入结束之后才能继续写入
        //static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        //static void WriteLog()
        //{
        //    try
        //    {
        //        //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
        //        //注意：长时间持有读线程锁或写线程锁会使其他线程发生饥饿 (starve)。 为了得到最好的性能，需要考虑重新构造应用程序以将写访问的持续时间减少到最小。
        //        //      从性能方面考虑，请求进入写入模式应该紧跟文件操作之前，在此处进入写入模式仅是为了降低代码复杂度
        //        //      因进入与退出写入模式应在同一个try finally语句块内，所以在请求进入写入模式之前不能触发异常，否则释放次数大于请求次数将会触发异常
        //        LogWriteLock.EnterWriteLock();

        //        var logFilePath = "E:\\Log.txt";
        //        var now = DateTime.Now;
        //        var logContent = string.Format("Tid: {0}{1} {2}.{3}\r\n", Thread.CurrentThread.ManagedThreadId.ToString().PadRight(4), now.ToLongDateString(), now.ToLongTimeString(), now.Millisecond.ToString());

        //        File.AppendAllText(logFilePath, logContent);
        //        WritedCount++;
        //    }
        //    catch (Exception)
        //    {
        //        FailedCount++;
        //    }
        //    finally
        //    {
        //        //退出写入模式，释放资源占用
        //        //注意：一次请求对应一次释放
        //        //      若释放次数大于请求次数将会触发异常[写入锁定未经保持即被释放]
        //        //      若请求处理完成后未释放将会触发异常[此模式不下允许以递归方式获取写入锁定]
        //        LogWriteLock.ExitWriteLock();
        //    }
        //}
    }
}
